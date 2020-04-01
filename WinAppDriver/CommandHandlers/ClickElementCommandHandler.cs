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
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            return GetResponse(automationElement, environment);
        }

        public Response GetResponse(AutomationElement automationElement, CommandEnvironment environment)
        { 
            if (automationElement.TryGetCurrentPattern(InvokePattern.Pattern, out var objPattern))
            {
                //AutoIt.AutoItX.ControlClick(environment.WindowHandle, automationElement.NativeElement.CurrentNativeWindowHandle);
                //return;
                var manualResetEvent = new ManualResetEvent(false);
                var alertBehaviorHandler = environment.Cache.GetHandler<UnexpectedAlertBehavior.Handler>();
                //UnexpectedAlertEventArgs args = null; 

                // handle situation with an unexpected alert (dialog)
                void UnexpectedAlertEventEventHandler(object sender, UnexpectedAlertEventArgs e)
                {
                    //args = e;
                    environment.Unexpected = e;
                    manualResetEvent.Set();
                }

                if (alertBehaviorHandler != null)
                {
                    alertBehaviorHandler.OnUnexpectedAlert += UnexpectedAlertEventEventHandler;
                }

                /*Task.Factory.StartNew(p =>
                {
                    ((InvokePattern)p).Invoke();
                    System.Diagnostics.Debug.WriteLine("Finished");
                    _manualResetEvent.Set();
                }, objPattern);*/

                // have to click in a separate thread because box is modal.  From http://social.msdn.microsoft.com/forums/en-US/windowsaccessibilityandautomation/thread/7f0bdc7c-be85-4fde-9f8a-cbb3f16ba5f4/
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
                    // waits for end ...
                    manualResetEvent.Set();
                });

                // run asynchronously
                // execution will wait for the Invoke operation to finish, but some work has to be executed at the same time 
                // TODO what work
                var thread = new Thread(threadStart);
                // https://social.msdn.microsoft.com/Forums/windowsdesktop/en-US/05a890ba-6f91-4380-9672-e8d173243ac3/ui-automation-hangs-when-showing-a-form-from-another-form?forum=windowsaccessibilityandautomation
                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();
                // but wait for signal
                manualResetEvent.WaitOne();

                if (alertBehaviorHandler != null)
                {
                    alertBehaviorHandler.OnUnexpectedAlert -= UnexpectedAlertEventEventHandler;
                }

                /*if (args != null) // alert was displayed, now decide what to do next
                {
                    return Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen, 
                        args.Title + ";" + string.Join(",", args.Content));
                }*/

                return Response.CreateSuccessResponse();
            }

            if (automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out objPattern))
            {
                ((SelectionItemPattern)objPattern).Select();
                return Response.CreateSuccessResponse();
            }

            if (automationElement.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out objPattern))
            {
                var prop = (ExpandCollapseState)automationElement.GetCurrentPropertyValue(ExpandCollapsePattern.ExpandCollapseStateProperty);
                switch (prop)
                {
                    case ExpandCollapseState.Collapsed:
                        ((ExpandCollapsePattern)objPattern).Expand();
                        break;
                    case ExpandCollapseState.Expanded:
                    case ExpandCollapseState.PartiallyExpanded:
                        ((ExpandCollapsePattern)objPattern).Collapse();
                        break;
                    case ExpandCollapseState.LeafNode:
                        ((ExpandCollapsePattern)objPattern).Expand();
                        break;
                }
                return Response.CreateSuccessResponse();
            }

            AutoIt.AutoItX.ControlClick(environment.WindowHandle, automationElement.NativeElement.CurrentNativeWindowHandle);
            return Response.CreateSuccessResponse();
            //return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, "InvokePattern not available");
        }
    }
}