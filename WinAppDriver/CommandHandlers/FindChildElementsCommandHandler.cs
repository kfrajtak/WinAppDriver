// <copyright file="FindChildElementsCommandHandler.cs" company="Salesforce.com">
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Infrastructure;
using System;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the find child element command.
    /// </summary>
    internal class FindChildElementsCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("using", out var mechanism))
            {
                return Response.CreateMissingParametersResponse("using");
            }

            if (!parameters.TryGetValue("value", out var criteria))
            {
                return Response.CreateMissingParametersResponse("value");
            }

            var elements = environment.Cache.FindElements(automationElement, new ElementFinder(mechanism.ToString(), criteria.ToString()), cancellationToken)
                .ToList()
                .Distinct(new TupleEqualityComparer())
                .ToList();

            if (cancellationToken.IsCancellationRequested)
            {
                string errorMessage = $"No elements found using {mechanism} and criteria '{criteria}', operation timed out.";
                return Response.CreateErrorResponse(WebDriverStatusCode.Timeout, errorMessage);
            }

            environment.Cache.AddToCache(elements.ToArray());

            return new Response
            {
                Status = WebDriverStatusCode.Success,
                SessionId = environment.SessionId,
                Value = elements.Select(e => new Dictionary<string, object>
                    {
                        { CommandEnvironment.ElementObjectKey, e.Item1 },
                        { string.Empty, e.Item2.Current.AutomationId }
                    }).ToList()
            };
        }

        private class TupleEqualityComparer : IEqualityComparer<Tuple<string, AutomationElement>>
        {
            public bool Equals(Tuple<string, AutomationElement> x, Tuple<string, AutomationElement> y)
            {
                if (x != null && y != null)
                {
                    return x.Item1 == y.Item1 && x.Item2.Current.AutomationId == y.Item2.Current.AutomationId;
                }

                return false;
            }

            public int GetHashCode(Tuple<string, AutomationElement> obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
