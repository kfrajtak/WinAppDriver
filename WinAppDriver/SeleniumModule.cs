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

            //sb.AppendLine();
            //sb.AppendLine(context.Response.Contents.AsString());
            Console.Out.WriteLine(sb.ToString());
            Console.Out.WriteLine();
        }

        private object HandleCommand(string commandName, dynamic data, Dictionary<string, object> parameters = null)
        {
            try
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
                var commandEnvironment = new CommandEnvironment();
                if (sessionId != null)
                {
                    if (CacheStore.CommandStore.TryGetValue(sessionId, out commandEnvironment))
                    {
                        /*commandEnvironment = new CommandEnvironment(elementCache)
                        {
                            SessionId = sessionId
                        };*/
                    }
                    else
                    {
                        if (commandName != DriverCommand.Close && commandName != DriverCommand.Quit)
                        {
                            throw new NotSupportedException("Cache not available for session " + sessionId);
                        }
                    }
                }

                Task<Response> t;

                var commandHandler = CommandHandlerFactory.Instance.GetHandler(command.CommandName);

                System.Diagnostics.Debug.WriteLine($"{commandName} [{System.Threading.Thread.CurrentThread.ManagedThreadId}]");

                // check for unexpected active windows
                if (commandEnvironment.Handler == null)
                {
                    commandEnvironment.Handler = new UnexpectedAlertBehavior2.Handler(null, commandEnvironment.WindowHandle, commandEnvironment);
                }
                else
                {
                    if (commandHandler.UnexpectedAlertCheckRequired && commandEnvironment.Handler.IsFaulty)// .IsTopMostActiveWindowDifferent(out var args))
                    {
                        return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen, 
                            "An alert was found open unexpectedly.", commandEnvironment.Handler.Unexpected);
                    }
                }

                bool conversion = false;
                if (commandHandler is AsyncCommandHandler asyncCommandHandler)
                {
                    conversion = true;
                    t = asyncCommandHandler.ExecuteAsync(commandEnvironment, command.Parameters);                    
                }
                else
                {
                    t = Task.Factory.StartNew(() =>
                    {
                        return commandHandler.Execute(commandEnvironment, command.Parameters);
                    }, commandEnvironment.GetCancellationToken());
                }

                var t2 = t.ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return FromException(task.Exception?.InnerException);
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

                return t2.Result;
            }
            catch (Exception ex)
            {
                return FromException(ex);
            }
        }

        private Response FromException(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                return FromException(aggregateException.InnerException);
            }

            System.Diagnostics.Debug.WriteLine(exception.ToString());

            if (exception is System.Windows.Automation.ElementNotEnabledException enee)
            {
                return Server.Response.CreateErrorResponse(WebDriverStatusCode.InvalidElementState, enee.Message);
            }

            return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnknownCommand, exception.Message);
        }

        //private 

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
