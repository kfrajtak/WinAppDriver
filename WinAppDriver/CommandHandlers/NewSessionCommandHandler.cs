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

using WinAppDriver.Infrastructure;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the new session command.
    /// </summary>
    internal class NewSessionCommandHandler : CommandHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            Dictionary<string, object> responseValue = new Dictionary<string, object>();
            responseValue["browserName"] = "WinFormsApp";
            responseValue["cssSelectorsEnabled"] = false;
            responseValue["javascriptEnabled"] = false;
            responseValue["takesScreenshot"] = true;
            responseValue["handlesAlerts"] = true;

            var desiredCapabilities = JObject.Parse(parameters["desiredCapabilities"]?.ToString() ?? "{}").ToObject<Dictionary<string, object>>();

            // extend capabilities with one more required parameter
            if (!desiredCapabilities.TryGetValue("mode", out var mode))
            {
                mode = "start";
                //return Response.CreateMissingParametersResponse("mode");
            }

            var mainWindowTitle = "";
            if (desiredCapabilities.TryGetValue("mainWindowTitle", out var mwt))
            {
                mainWindowTitle = mwt?.ToString() ?? "";
            }

            var attached = false;
            Process process = null;
            // check does mode is process and capabilities contain processName simultaneounsy
            if (mode?.ToString() == "attach")// & !desiredCapabilities.TryGetValue("processName", out var processName))
            {
                var processName = desiredCapabilities.GetParameterValue<string>("processName");

                process = Process.GetProcessesByName(processName).FirstOrDefault();

                // searching by name as regular expression pattern
                if (process == null)
                {
                    var regex = new Regex(processName);
                    process = Process.GetProcesses().FirstOrDefault(x => regex.IsMatch(x.ProcessName));
                }

                if (process == null)
                {
                    return Response.CreateErrorResponse(-1, $"Cannot attach to process '{processName}', no such process found.");
                }

                attached = true;
            }
            else
            {
                object exePath = string.Empty;
                // check does mode is process and capabilities contain exePath simultaneounsy
                if (mode?.ToString() == "start")
                {
                    if (!desiredCapabilities.TryGetValue("exePath", out exePath)
                        && !desiredCapabilities.TryGetValue("app", out exePath))
                    {
                        return Response.CreateMissingParametersResponse("exePath or app");
                    }

                    desiredCapabilities.TryGetParameterValue<string>("processName", out var processName);

                    process = ApplicationProcess.StartProcessFromPath(exePath.ToString(), cancellationToken, processName, mainWindowTitle);
                    if (process == null)
                    {
                        return Response.CreateErrorResponse(-1, "Cannot start process.");
                    }
                }
            }

            if (process == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError,
                    $"Cannot start process or attach to process");
            }

            var sessionId = process.MainWindowHandle.ToString();
            NLog.MappedDiagnosticsContext.Set("SessionId", sessionId);
            var processId = $"'{process.ProcessName}', pid = {process.Id}";
            if (sessionId == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError,
                    $"Cannot get main windows handle for the process {processId}.");
            }

            // new session is starting, remove the old command environment context
            if (CacheStore.CommandStore.TryGetValue(sessionId, out var commandEnvironment))
            {
                CacheStore.CommandStore.TryRemove(sessionId, out commandEnvironment);
                commandEnvironment.Dispose();
            }

            commandEnvironment = new CommandEnvironment(sessionId, desiredCapabilities);
            if (!attached)
            {
                commandEnvironment.Pid = process.Id;
                Logger.Info($"Session '{sessionId}' created process {processId}.");
            }
            else
            {
                Logger.Info($"Session '{sessionId}' attached to process {processId}.");
            }

            CacheStore.CommandStore.AddOrUpdate(sessionId, commandEnvironment, (key, _) => _);

            Response response = Response.CreateSuccessResponse(responseValue);
            response.SessionId = sessionId;
            response.Status = null;
            return response;
        }
    }
}
