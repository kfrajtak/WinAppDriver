// <copyright file="ClickElementCommandHandler.cs" company="Salesforce.com">
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the click element command.
    /// </summary>
    internal class ClickElementCommandHandler : ElementCommandHandler
    {
        private readonly System.Threading.ManualResetEvent _manualResetEvent = new System.Threading.ManualResetEvent(false);

        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (automationElement.TryGetCurrentPattern(InvokePattern.Pattern, out var objPattern))
            {
                /*Task.Factory.StartNew(p =>
                {
                    ((InvokePattern)p).Invoke();
                    System.Diagnostics.Debug.WriteLine("Finished");
                    _manualResetEvent.Set();
                }, objPattern);

                _manualResetEvent.WaitOne();
                System.Diagnostics.Debug.WriteLine("ClickElementCommandHandler done");*/
                //have to click in a separate thread because box is modal.  From http://social.msdn.microsoft.com/forums/en-US/windowsaccessibilityandautomation/thread/7f0bdc7c-be85-4fde-9f8a-cbb3f16ba5f4/
                ThreadStart threadStart = new ThreadStart(((InvokePattern)objPattern).Invoke);
                Thread thread = new Thread(threadStart);
                thread.Start();

                return Response.CreateSuccessResponse();
            }

            if (automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out objPattern))
            {
                ((SelectionItemPattern)objPattern).Select();
                return Response.CreateSuccessResponse();
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, "InvokePattern not availeble");
        }
    }
}
