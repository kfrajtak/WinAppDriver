// <copyright file="NewSessionCommandHandler.cs" company="Salesforce.com">
//
// Copyright (c) 2014 Salesforce.com, Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
// following conditions are met:
//
//    Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//    disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//    following disclaimer in the documentation and/or other materials provided with the distribution.
//
//    Neither the name of Salesforce.com nor the names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>

using WinAppDriver.Behaviors;
using WinAppDriver.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using WinAppDriver;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the new session command.
    /// </summary>
    internal class NewSessionCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            Dictionary<string, object> responseValue = new Dictionary<string, object>();
            responseValue["browserName"] = "WinFormsApp";
            responseValue["cssSelectorsEnabled"] = false;
            responseValue["javascriptEnabled"] = false;
            responseValue["takesScreenshot"] = true;
            responseValue["handlesAlerts"] = true;

            var desiredCapabilities = JObject.Parse(parameters["desiredCapabilities"]?.ToString() ?? "{}").ToObject<Dictionary<string, object>>();
            //parameters["desiredCapabilities"] as Dictionary<string, string> ?? new Dictionary<string, string>();


            // extend capabilities with one more required parameter
            if (!desiredCapabilities.TryGetValue("mode", out var mode))
            {
                return Response.CreateMissingParametersResponse("mode");
            }

            // check does mode is process and capabilities contain processName simultaneounsy
            if (mode?.ToString() == "attach" & !desiredCapabilities.TryGetValue("processName", out var processName))
            {
                return Response.CreateMissingParametersResponse("processName");
            }

            // check does mode is process and capabilities contain exePath simultaneounsy
            if (mode?.ToString() == "start" & !desiredCapabilities.TryGetValue("exePath", out var exePath))
            {
                return Response.CreateMissingParametersResponse("exePath");
            }


            Process process = null;
            if (processName != null)
            {
                
                process = Process.GetProcessesByName(processName.ToString()).FirstOrDefault();

                // searching by name as regular expression pattern
                if (process == null)
                {
                        var regex = new Regex(processName.ToString());
                        process = Process.GetProcesses()
                            .Where(x => regex.IsMatch(x.ProcessName)).FirstOrDefault();            
                }

                if (process == null)
                {
                    return Response.CreateErrorResponse(-1, $"Cannot attach to process '{processName}', no such process found.");
                }
            }

            if (exePath != null)
            {
                process = ApplicationProcess.StartProcessFromPath(exePath.ToString());
                if (process == null)
                {
                    return Response.CreateErrorResponse(-1, "Cannot start process.");
                }
            }

            var sessionId = process?.MainWindowHandle.ToString();
            if (sessionId != null)
            {
                /*if (CacheStore.Store.TryGetValue(sessionId, out var elementCache))
                {
                    CacheStore.Store.TryRemove(sessionId, out elementCache);
                }
                else
                {
                    CacheStore.Store.AddOrUpdate(sessionId, ElementCacheFactory.Get(sessionId), (k, c) =>
                    {
                        c.Handle = process.MainWindowHandle;
                        return c;
                    });
                }*/
                if (CacheStore.CommandStore.TryGetValue(sessionId, out var commandEnvironment))
                {
                    CacheStore.CommandStore.TryRemove(sessionId, out commandEnvironment);
                }

                //var cache = ElementCacheFactory.Get(sessionId);
                commandEnvironment = new CommandEnvironment(sessionId, desiredCapabilities);
                //var e = cache.AutomationElement;
                //e = AutomationElement.FromHandle(cache.Handle);
                //cache.AddHandler(UnexpectedAlertBehavior.CreateHandler(e, cache.Handle, commandEnvironment));
                //CacheStore.CommandStore.AddOrUpdate(sessionId, commandEnvironment, (k, c) => c);
            }

            Response response = Response.CreateSuccessResponse(responseValue);
            response.SessionId = sessionId;
            response.Status = null;
            return response;
        }

        
    }
}
