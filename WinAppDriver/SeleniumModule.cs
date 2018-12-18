using WinAppDriver.Infrastructure;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WinAppDriver.Server
{
    public class SeleniumModule : NancyModule
    {
        public SeleniumModule()
        {
            var repository = new W3CWireProtocolCommandInfoRepository();
            var field = repository.GetType().BaseType.GetField("commandDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            var commandDictionary = (Dictionary<string, CommandInfo>)field.GetValue(repository);

            //Before += BeforeEach;

            foreach (var command in commandDictionary)
            {
                switch (command.Value.Method)
                {
                    case CommandInfo.GetCommand:
                        Get[command.Value.ResourcePath] = p => { return HandleCommand(command.Key, p); };
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
        /*
        private object BeforeEach(NancyContext context)
        {
            return context;
        }*/

        private object HandleCommand(string commandName, dynamic data, Dictionary<string, object> parameters = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(commandName);

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

                command.CommandHandler = CommandHandlerFactory.Instance.GetHandler(command.CommandName);
                var response = command.Execute(commandEnvironment);
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Server.Response.CreateErrorResponse(WebDriverStatusCode.UnknownCommand, ex.Message);
            }
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
