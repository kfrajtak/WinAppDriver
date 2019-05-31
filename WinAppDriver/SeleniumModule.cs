using WinAppDriver.Infrastructure;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using WinAppDriver.Server.CommandHandlers;
using System.Text;
using WinAppDriver.Behaviors;
using System.Threading;

namespace WinAppDriver.Server
{
    public class SeleniumModule : NancyModule
    {
        public SeleniumModule()
        {
            var repository = new W3CWireProtocolCommandInfoRepository();
            var field = repository.GetType().BaseType.GetField("commandDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            var commandDictionary = (Dictionary<string, CommandInfo>)field.GetValue(repository);

            After += AfterEach;

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

        private void AfterEach(NancyContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{context.Request.Method} {context.Request.Url}");
            foreach(var header in context.Request.Headers)
            {
                sb.AppendLine($"{header.Key}: {string.Join(",", header.Value)}");
            }

            context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            sb.AppendLine(context.Request.Body.AsString());
            sb.AppendLine();
            sb.AppendLine($"{context.Response.StatusCode}");
            foreach (var header in context.Response.Headers)
            {
                sb.AppendLine($"{header.Key}: {string.Join(",", header.Value)}");
            }

            Console.Out.WriteLine(sb.ToString());
            Console.Out.WriteLine();
        }

        private object HandleCommand(string commandName, dynamic data, Dictionary<string, object> parameters = null)
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

            var sessionId = command.SessionId?.ToString();

            CommandEnvironment commandEnvironment = null;
            if (sessionId != null)
            {
                if (!CacheStore.CommandStore.TryGetValue(sessionId, out commandEnvironment))
                {
                    if (commandName != DriverCommand.Close && commandName != DriverCommand.Quit)
                    {
                        return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, "Cache not available for session " + sessionId);
                    }
                }
            }

            commandEnvironment = commandEnvironment ?? new CommandEnvironment();

            var cancellationToken = commandEnvironment.GetCancellationToken();

            try
            {
                // check for unexpected alert (modal dialog)
                /*if (commandEnvironment.GetModalWindow() != null)
                {
                    return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen, "An alert was found open unexpectedly.");
                }*/

                Task<Response> t;

                var commandHandler = CommandHandlerFactory.Instance.GetHandler(command.CommandName);

                System.Diagnostics.Debug.WriteLine($"{commandName} [{Thread.CurrentThread.ManagedThreadId}]");

                // check for unexpected active windows
                if (commandEnvironment.Handler == null)
                {
                    commandEnvironment.Handler = new UnexpectedAlertBehavior2.Handler(null, commandEnvironment.ApplicationWindowHandle, commandEnvironment);
                }
                else
                {
                    if (commandHandler.UnexpectedAlertCheckRequired && commandEnvironment.Handler.IsFaulty)// .IsTopMostActiveWindowDifferent(out var args))
                    {
                        System.Diagnostics.Trace.WriteLine(commandEnvironment.Handler.Unexpected);
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
                    t = Task.Factory.StartNew(() =>
                    {
                        return commandHandler.Execute(commandEnvironment, command.Parameters, cancellationToken);
                    });
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

                commandEnvironment.Handler.Update();// .IsTopMostActiveWindowDifferent(out var args))
                System.Diagnostics.Trace.WriteLine("****** ____________" + commandEnvironment.Handler.IsFaulty);

                var response = t2.Result;
                response.SessionId = response.SessionId ?? commandEnvironment?.SessionId;
                return response;
            }
            catch (Exception ex)
            {
                commandEnvironment.CancellationTokenSource.Cancel(); // stop all work
                return FromException(ex, commandEnvironment);
            }
        }

        private Response FromException(Exception exception, CommandEnvironment commandEnvironment)
        {
            if (exception is AggregateException aggregateException)
            {
                return FromException(aggregateException.InnerException, commandEnvironment);
            }

            System.Diagnostics.Debug.WriteLine(exception.ToString());

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

            return Server.Response.CreateErrorResponse(
                WebDriverStatusCode.UnknownCommand, 
                exception.Message, 
                sessionId: commandEnvironment?.SessionId);
        }
        
        private static IDictionary<string, T> ToDictionary<T>(object source)
        {
            var dictionary = new Dictionary<string, T>();

            void AddPropertyToDictionary(PropertyDescriptor property)
            {
                object value = property.GetValue(source);
                if (value is T t)
                {
                    dictionary.Add(property.Name, t);
                }
            }
                        
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                AddPropertyToDictionary(property);
            }

            return dictionary;
        }
    }
}
