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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using WinAppDriver.Behaviors;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the click element command.
    /// </summary>
    internal class ClickElementCommandHandler : AsyncElementCommandHandler
    {
        private readonly System.Threading.ManualResetEvent _manualResetEvent = new System.Threading.ManualResetEvent(false);

        protected override Task<Response> GetResponseAsync(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            return new Task<Response>(() =>
            {
                return GetResponse(automationElement, environment, parameters);
            });
        }

        protected Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (automationElement.TryGetCurrentPattern(InvokePattern.Pattern, out var objPattern))
            {
                var manualResetEvent = new ManualResetEvent(false);
                var alertBehaviorHandler = environment.Cache.GetHandler<UnexpectedAlertBehavior.Handler>();
                UnexpectedAlertEventArgs args = null; 

                // handle situation with an unexpected alert (dialog)
                void UnexpectedAlertEventEventHandler(object sender, UnexpectedAlertEventArgs e)
                {
                    args = e;
                    manualResetEvent.Set();
                }

                alertBehaviorHandler.OnUnexpectedAlert += UnexpectedAlertEventEventHandler;

                /*Task.Factory.StartNew(p =>
                {
                    ((InvokePattern)p).Invoke();
                    System.Diagnostics.Debug.WriteLine("Finished");
                    _manualResetEvent.Set();
                }, objPattern);
                

                _manualResetEvent.WaitOne();
                System.Diagnostics.Debug.WriteLine("ClickElementCommandHandler done");*/
                //have to click in a separate thread because box is modal.  From http://social.msdn.microsoft.com/forums/en-US/windowsaccessibilityandautomation/thread/7f0bdc7c-be85-4fde-9f8a-cbb3f16ba5f4/
                ThreadStart threadStart = new ThreadStart(() =>
                {
                    var handler = EventHandler.RegisterHandler(InvokePattern.InvokedEvent, automationElement, (sender, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine("invoked");
                        alertBehaviorHandler.OnUnexpectedAlert += UnexpectedAlertEventEventHandler;
                        manualResetEvent.Set();
                    });

                    try
                    {
                        ((InvokePattern)objPattern).Invoke();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }

                    handler.Dispose();
                });

                // run asynchronously
                // execution will wait for the Invoke operation to finish, but some work has to be executed at the same time 
                // TODO what work
                var thread = new Thread(threadStart);
                thread.Start();
                // but wait for signal
                manualResetEvent.WaitOne();

                alertBehaviorHandler.OnUnexpectedAlert -= UnexpectedAlertEventEventHandler;

                if (args != null) // alert was displayed, now decide what to do next
                {

                }

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
