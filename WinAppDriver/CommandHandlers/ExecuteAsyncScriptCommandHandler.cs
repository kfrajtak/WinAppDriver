// <copyright file="ExecuteAsyncScriptCommandHandler.cs" company="Salesforce.com">
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the execute async script command.
    /// </summary>
    internal class ExecuteAsyncScriptCommandHandler : CommandHandler
    {
        private bool navigationStarted;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object script;
            if (!parameters.TryGetValue("script", out script))
            {
                return Response.CreateMissingParametersResponse("script");
            }

            object args;
            if (!parameters.TryGetValue("args", out args))
            {
                return Response.CreateMissingParametersResponse("args");
            }

            if ("clickElement()".Equals(script.ToString()))
            {
                var keyValuePairs = JArray.Parse(args.ToString());
                var elementId = keyValuePairs[0].Last().Last().ToString();

                var automationElement = environment.Cache.GetElement(elementId);

                AutoIt.AutoItX.ControlClick(environment.WindowHandle, automationElement.NativeElement.CurrentNativeWindowHandle);
                return Response.CreateSuccessResponse();
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedJavaScriptError, "Not implemented");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        private string WaitForAsyncScriptResult(CommandEnvironment environment)
        {
            string result = string.Empty;
            while (string.IsNullOrEmpty(result) && !this.navigationStarted)
            {
                ManualResetEvent synchronizer = new ManualResetEvent(false);
                /*environment.Browser.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        result = environment.Browser.InvokeScript("eval", "window.top.__wd_fn_result").ToString();
                    }
                    catch (Exception ex)
                    {
                        result = string.Format(CultureInfo.InvariantCulture, "{{ \"status\": {2}, \"value\": {{ \"message\": \"Unexpected exception ({0}) - '{1}'\" }} }}", ex.GetType().ToString(), ex.Message, WebDriverStatusCode.UnhandledError);
                    }
                    finally
                    {
                        synchronizer.Set();
                    }
                });*/

                synchronizer.WaitOne();
            }

            return result;
        }
    }
}
