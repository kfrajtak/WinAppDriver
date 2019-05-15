// <copyright file="ScreenshotCommandHandler.cs" company="Salesforce.com">
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the screenshot command.
    /// </summary>
    internal class ScreenshotCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var capture = new Cyotek.Demo.SimpleScreenshotCapture.ScreenshotCapture();

            var region = capture.GetRectangle(environment.ApplicationWindowHandle);

            // expand the screeshot region to include all application windows 
            var windows = environment.GetWindows();
            foreach (var window in windows)
            {
                var windowRegion = capture.GetRectangle(new IntPtr(window.Current.NativeWindowHandle));
                if (windowRegion.Left < region.Left)
                {
                    region = Rectangle.FromLTRB(windowRegion.Left, region.Top, region.Right, region.Bottom);
                }

                if (windowRegion.Right > region.Right)
                {
                    region = Rectangle.FromLTRB(region.Left, region.Top, windowRegion.Right, region.Bottom);
                }

                if (windowRegion.Top < region.Top)
                {
                    region = Rectangle.FromLTRB(region.Left, windowRegion.Top, region.Right, region.Bottom);
                }

                if (windowRegion.Bottom > region.Bottom)
                {
                    region = Rectangle.FromLTRB(region.Left, region.Top, region.Right, windowRegion.Bottom);
                }
            }

            var bitmap = capture.CaptureRegion(region);

            string screenshot = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();

                screenshot = Convert.ToBase64String(imageBytes);
            }

            return Response.CreateSuccessResponse(screenshot);
        }
    }
}
