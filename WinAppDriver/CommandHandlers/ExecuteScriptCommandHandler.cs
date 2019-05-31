// <copyright file="ExecuteScriptCommandHandler.cs" company="Salesforce.com">
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

using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the execute script command.
    /// </summary>
    internal class ExecuteScriptCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
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

            // TODO for some reason the GetAttribute method is invoked through this ... 
            // if (driver.IsSpecificationCompliant) Execute(DriverCommand.ExecuteScript, dictionary);
            var getAtomMethod = typeof(RemoteWebElement).GetMethod("GetAtom", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var atom = getAtomMethod.Invoke(null, new object[] { "getAttribute.js" });
            if (atom.Equals(script.ToString()))
            {
                var keyValuePairs = JArray.Parse(args.ToString());
                var elementId = keyValuePairs[0].Last().Last().ToString();
                var attributeName = keyValuePairs[1].ToString();
                return CommandHandlerFactory.Instance.GetHandler(DriverCommand.GetElementAttribute).Execute(environment, new Dictionary<string, object>
                {
                    { "NAME", attributeName },
                    { "ID", elementId }
                }, cancellationToken);
            }

            atom = getAtomMethod.Invoke(null, new object[] { "isDisplayed.js" });
            if (atom.Equals(script.ToString()))
            {
                var keyValuePairs = JArray.Parse(args.ToString());
                var elementId = keyValuePairs[0].Last().Last().ToString();
                return CommandHandlerFactory.Instance.GetHandler(DriverCommand.IsElementDisplayed).Execute(environment, new Dictionary<string, object>
                {
                    { "ID", elementId }
                }, cancellationToken);
            }

            if (script.ToString() == "var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};")
            {
                var keyValuePairs = JArray.Parse(args.ToString());
                var elementId = keyValuePairs[0].Last().Last().ToString();
                return CommandHandlerFactory.Instance.GetHandler(DriverCommand.GetElementRect).Execute(environment, new Dictionary<string, object>
                {
                    { "ID", elementId }
                }, cancellationToken);
            }

            if (script.ToString() == "return window.name")
            {
                var keyValuePairs = JArray.Parse(args.ToString());
                /*var elementId = keyValuePairs[0].Last().Last().ToString();
                return CommandHandlerFactory.Instance.GetHandler(DriverCommand.GetElementRect).Execute(environment, new Dictionary<string, object>
                {
                    { "ID", elementId }
                });*/
                return Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedJavaScriptError, "Cannot get window.name");
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedJavaScriptError, "Scripting is not supported.");
        }
    }
}
