// <copyright file="MouseMoveCommandHandler.cs" company="Salesforce.com">
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
using System.Drawing;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the click element command.
    /// </summary>
    internal class MouseMoveCommandHandler : ElementCommandHandler
    {
        public MouseMoveCommandHandler()
        {
            _parameterName = "element";
        }

        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            var offsetX = 0;
            if (parameters.ContainsKey("xoffset") && !int.TryParse(parameters["xoffset"]?.ToString(), out offsetX))
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.ExpectedError, "Invalid 'xoffset' value");
            }

            var offsetY = 0;
            if (parameters.ContainsKey("yoffset") && !int.TryParse(parameters["yoffset"]?.ToString(), out offsetX))
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.ExpectedError, "Invalid 'yoffset' value");
            }
            
            var origin = automationElement.GetClickablePoint();
            Microsoft.Test.Input.Mouse.MoveTo(new Point((int)origin.X - offsetX, (int)origin.Y - offsetY));
            return Response.CreateSuccessResponse();
        }
    }
}
