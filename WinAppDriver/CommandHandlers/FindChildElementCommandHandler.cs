// <copyright file="FindChildElementCommandHandler.cs" company="Salesforce.com">
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
using System.Windows.Automation;
using WinAppDriver.Exceptions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the find child element command.
    /// </summary>
    internal class FindChildElementCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue("using", out var mechanism))
            {
                return Response.CreateMissingParametersResponse("using");
            }

            if (!parameters.TryGetValue("value", out var criteria))
            {
                return Response.CreateMissingParametersResponse("value");
            }

            var token = environment.GetCancellationToken();

            var element = environment.Cache.FindElements(automationElement, mechanism.ToString(), criteria.ToString(), token).FirstOrDefault();
            if (element == null)
            {
                throw new NoSuchElementException();
            }

            environment.Cache.AddToCache(element);

            var response = new Response
            {
                Status = WebDriverStatusCode.Success,
                SessionId = environment.SessionId,
                Value = new Dictionary<string, object>
                    {
                        { CommandEnvironment.ElementObjectKey, element.Item1 },
                        { string.Empty, element.Item2.Current.AutomationId }
                    }
            };

            if (response.Status == WebDriverStatusCode.Success)
            {
                // Return early for success
                var foundElement = response.Value as Dictionary<string, object>;
                if (foundElement != null && foundElement.ContainsKey(CommandEnvironment.ElementObjectKey))
                {
                    return response;
                }
            }
            else if (response.Status != WebDriverStatusCode.NoSuchElement)
            {
                if (mechanism.ToString().ToUpperInvariant() != "XPATH" && response.Status == WebDriverStatusCode.InvalidSelector)
                {
                    //continue;
                }

                // Also return early for response of not NoSuchElement.
                return response;
            }

            string errorMessage = string.Format(CultureInfo.InvariantCulture, "No element found for {0} == '{1}'", mechanism.ToString(), criteria.ToString());
            response = Response.CreateErrorResponse(WebDriverStatusCode.NoSuchElement, errorMessage);
            return response;
        }
    }
}
