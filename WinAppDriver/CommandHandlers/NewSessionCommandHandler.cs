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
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

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

            if (!desiredCapabilities.TryGetValue("processName", out var processName) 
                & !desiredCapabilities.TryGetValue("exePath", out var exePath))
            {
                return Response.CreateMissingParametersResponse("processName or exePath");
            }


            Process process = null;
            if (processName != null)
            {
                
                process = Process.GetProcessesByName(processName.ToString()).FirstOrDefault();

                if (process == null)
                {
                    var processes = Process.GetProcesses();
                    process = processes.Where(x =>
                    {
                        return x.ProcessName.Contains(processName.ToString())
                        || processName.ToString().Contains(x.ProcessName);
                    }).FirstOrDefault();
                }

                if (process == null && exePath == null)
                {
                    return Response.CreateErrorResponse(-1, $"Cannot attach to process '{processName}', no such process found.");
                }
            }

            if (process == null && exePath != null)
            {
                try
                {
                    process = StartProcessFromPath(exePath.ToString());
                    if (process == null || process.MainWindowHandle.ToInt32() == 0)
                    {
                        return Response.CreateErrorResponse(-1, "Process starting timeout expired.");
                    }
                }
                catch (FileNotFoundException) 
                {
                    return Response.CreateErrorResponse(-1, $"Cannot start process from file '{exePath.ToString()}' file not found.");
                }
                catch(ObjectDisposedException)
                {
                    return Response.CreateErrorResponse(-1, "The object you tring access already desposed.");
                }
                catch(InvalidOperationException)
                {
                    return Response.CreateErrorResponse(-1, "Cannot run application without user interface.");
                }
            }

            var sessionId = process.MainWindowHandle.ToString();
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

        private Process StartProcessFromPath(string path, int timeout = 3000)
        {
            var process = Process.Start(path);
            process.WaitForInputIdle(timeout);
            process.Refresh();
            if (process.MainWindowHandle.ToInt32() == 0)
            {
                int time = 0;
                while (!process.HasExited)
                {
                    process.Refresh();
                    if (process.MainWindowHandle.ToInt32() != 0)
                    {
                        return process;
                    }
                    Thread.Sleep(50);
                    time += 10;
                    if (time > timeout)
                    {
                        throw new TimeoutException("Process starting timeout expired.");
                    }
                }
            }
            return process;
        }
    }
}
