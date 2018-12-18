// <copyright file="SendKeysToActiveElementCommandHandler.cs" company="Salesforce.com">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the send keys to active element command in the advanced user interaction API.
    /// </summary>
    internal class SendKeysToActiveElementCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object keys;
            if (!parameters.TryGetValue("value", out keys))
            {
                return Response.CreateMissingParametersResponse("value");
            }

            string keysAsString = string.Empty;
            object[] keysAsArray = keys as object[];
            if (keysAsArray != null)
            {
                foreach (object key in keysAsArray)
                {
                    keysAsString += key.ToString();
                }
            }

            // Normalize line endings to single line feed, as that's what the atom expects.
            keysAsString = keysAsString.Replace("\r\n", "\n");
            Microsoft.Test.Input.Keyboard.Type(keysAsString);
            /*string result = this.EvaluateAtom(environment, WebDriverAtoms.SendKeysToActiveElement, keysAsString, environment.KeyboardState, environment.CreateFrameObject());
            Response atomResponse = Response.FromJson(result);
            Dictionary<string, object> keyboardState = atomResponse.Value as Dictionary<string, object>;
            if (atomResponse.Value != null)
            {
                environment.KeyboardState = keyboardState;
            }*/

            return Response.CreateSuccessResponse();
        }
    }
}
