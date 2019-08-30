using WinAppDriver.Infrastructure;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinAppDriver.Server.CommandHandlers;
using System.Text;
using WinAppDriver.Behaviors;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace WinAppDriver.Server
{
    public class SeleniumModule : NancyModule
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public SeleniumModule()
        {
            var repository = new W3CWireProtocolCommandInfoRepository();
            var field = repository.GetType().BaseType.GetField("commandDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            var commandDictionary = (Dictionary<string, CommandInfo>)field.GetValue(repository);

            After += AfterEach;

            Before += BeforeEach;

            foreach (var command in commandDictionary)
            {
                switch (command.Value.Method)
                {
                    case CommandInfo.GetCommand:
                        Get[command.Value.ResourcePath] = p => HandleCommand(command.Key, p);
                        break;
                    case CommandInfo.PostCommand:
                        Post[command.Value.ResourcePath] = p =>
                        {
                            var body = Request.Body.AsString();
                            var parameters = JObject.Parse(body).ToObject<Dictionary<string, object>>();
                            return HandleCommand(command.Key, p, parameters);
                        };
                        break;
                    case CommandInfo.DeleteCommand:
                        Delete[command.Value.ResourcePath] = p =>
                        {
                            var body = Request.Body.AsString();
                            if (body == null || body.Trim().Length == 0)
                            {
                                body = "{}";
                            }
                            var parameters = JObject.Parse(body).ToObject<Dictionary<string, object>>();
                            return HandleCommand(command.Key, p, parameters);
                        };
                        break;
                }
            }
        }

        private Nancy.Response BeforeEach(NancyContext context)
        {
            var sw = new Stopwatch();
            sw.Start();

            var sb = new StringBuilder();
            sb.Append($"{context.Request.Method} {context.Request.Url}, headers: ");
            sb.Append(string.Join(", ", context.Request.Headers.Select(header => $"{header.Key}={string.Join(",", header.Value)}")));
            sb.Append(", request body: ");
            context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            sb.Append(context.Request.Body.AsString());
            context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            Logger.Info(sb.ToString());
            context.Items["sw"] = sw;
            return null;
        }

        private void AfterEach(NancyContext context)
        {
            var sw = context.Items["sw"] as Stopwatch;
            sw.Stop();

            var sb = new StringBuilder();
            sb.Append($"Request end {context.Response.StatusCode}, elapsed {sw.Elapsed}, headers: ");
            sb.Append(string.Join(", ", context.Response.Headers.Select(header => $"{header.Key}={string.Join(",", header.Value)}")));
            sb.Append(", ");
            if (Context.Items["response"] is Response response && response.Value != null)
            {
                sb.Append("response body: ").Append(JsonConvert.SerializeObject(response.Value));
            }
            else
            {
                sb.Append("empty response body.");
            }

            Logger.Info(sb.ToString());
        }

        private Response HandleCommand(string commandName, dynamic data, Dictionary<string, object> parameters = null)
        {
            var response = HandleCommand_(commandName, data, parameters);
            Context.Items["response"] = response;
            return response;
        }

        private Response HandleCommand_(string commandName, dynamic data, Dictionary<string, object> parameters = null)
        {
            var p2 = new Dictionary<string, object>(data);
            parameters = parameters ?? new Dictionary<string, object>();

            foreach (var pair in parameters)
            {
                if (!p2.ContainsKey(pair.Key))
                {
                    p2.Add(pair.Key, pair.Value);
                }
            }

            if (p2.ContainsKey("id"))
            {
                p2.Add("ID", p2["id"]);
            }

            var command = new Command
            {
                CommandName = commandName,
                Parameters = p2,
                SessionId = data.sessionId
            };

            var sessionId = command.SessionId;

            NLog.MappedDiagnosticsContext.Set("SessionId", sessionId);

            CommandEnvironment commandEnvironment = null;
            if (sessionId != null && !CacheStore.CommandStore.TryGetValue(sessionId, out commandEnvironment) && commandName != DriverCommand.Close && commandName != DriverCommand.Quit)
            {
                return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, $"Cache for session '{sessionId}' was not found in the cache store, that session was probably closed earlier.");
            }

            commandEnvironment = commandEnvironment ?? new CommandEnvironment();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = GetCancellationToken(commandEnvironment, cancellationTokenSource);

            try
            {
                // check for unexpected alert (modal dialog)
                /*if (commandEnvironment.GetModalWindow() != null)
                {
                    return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen, "An alert was found open unexpectedly.");
                }*/

                Task<Response> t;

                var commandHandler = CommandHandlerFactory.Instance.GetHandler(command.CommandName);

                Logger.Debug($"Handling command '{commandName}' with {commandHandler.GetType().Name}.");

                // check for unexpected active windows
                if (commandEnvironment.Handler == null)
                {
                    commandEnvironment.Handler = new UnexpectedAlertBehavior2.Handler(null, commandEnvironment.ApplicationWindowHandle, commandEnvironment);
                }
                else
                {
                    if (commandHandler.UnexpectedAlertCheckRequired && commandEnvironment.Handler.IsFaulty)
                    {
                        return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen,
                            "An alert was found open unexpectedly.",
                            sessionId: commandEnvironment.SessionId,
                            payload: commandEnvironment.Handler.Unexpected);
                    }
                }

                bool conversion = false;
                if (commandHandler is IAsyncCommandHandler asyncCommandHandler)
                {
                    conversion = true;
                    t = asyncCommandHandler.ExecuteAsync(commandEnvironment, command.Parameters, cancellationToken);
                }
                else
                {
                    t = Task.Factory.StartNew(() => commandHandler.Execute(commandEnvironment, command.Parameters, cancellationToken));
                }

                var t2 = t.ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return FromException(task.Exception, commandEnvironment);
                    }

                    return task.Result;
                });

                if (conversion)
                {
                    t.Start();
                }

                t.Wait();

                commandEnvironment.Handler.Update();

                var response = t2.Result;
                response.SessionId = response.SessionId ?? commandEnvironment?.SessionId;
                return response;
            }
            catch (Exception ex)
            {
                cancellationTokenSource.Cancel(); // stop all work
                return FromException(ex, commandEnvironment);
            }
        }

        private CancellationToken GetCancellationToken(CommandEnvironment commandEnvironment, CancellationTokenSource cancellationTokenSource)
        {
            if (!Debugger.IsAttached)
            {
                cancellationTokenSource.CancelAfter(commandEnvironment.ImplicitWaitTimeout);
            }
            return cancellationTokenSource.Token;
        }

        /// <summary>
        /// Creates an error response from an exception.
        /// </summary>
        /// <param name="exception">Exception to create an error response from.</param>
        /// <param name="commandEnvironment">Command environment</param>
        private Response FromException(Exception exception, CommandEnvironment commandEnvironment)
        {
            if (exception is AggregateException aggregateException)
            {
                return FromException(aggregateException.InnerException, commandEnvironment);
            }

            if (exception is Exceptions.IRemoteException re)
            {
                var response = re.GetResponse();
                response.SessionId = commandEnvironment?.SessionId;
                return response;
            }

            if (exception is System.Windows.Automation.ElementNotEnabledException enee)
            {
                return Server.Response.CreateErrorResponse(
                    WebDriverStatusCode.InvalidElementState,
                    enee.Message,
                    sessionId: commandEnvironment?.SessionId);
            }

            // log the unexpected exception
            Logger.Error(exception, "An unxpected error has occurred");

            return Server.Response.CreateErrorResponse(
                WebDriverStatusCode.UnknownCommand,
                exception.Message,
                sessionId: commandEnvironment?.SessionId);
        }
    }
}
