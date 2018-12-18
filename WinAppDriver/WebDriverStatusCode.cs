// <copyright file="WebDriverStatusCode.cs" company="Salesforce.com">
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

namespace WinAppDriver.Server
{
    /// <summary>
    /// Values describing the list of status codes understood by a remote server using the JSON wire protocol.
    /// NOTE: Copied from the WebDriver source code. Used to avoid an extra reference to the WebDriver
    /// assembly in the Windows Phone application.
    /// </summary>
    public static class WebDriverStatusCode
    {
        /// <summary>
        /// The action was successful.
        /// </summary>
        public static readonly int Success = 0;

        /// <summary>
        /// The index specified for the action was out of the acceptable range.
        /// </summary>
        public static readonly int IndexOutOfBounds = 1;

        /// <summary>
        /// No collection was specified.
        /// </summary>
        public static readonly int NoCollection = 2;

        /// <summary>
        /// No string was specified.
        /// </summary>
        public static readonly int NoString = 3;

        /// <summary>
        /// No string length was specified.
        /// </summary>
        public static readonly int NoStringLength = 4;

        /// <summary>
        /// No string wrapper was specified.
        /// </summary>
        public static readonly int NoStringWrapper = 5;

        /// <summary>
        /// No driver matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchDriver = 6;

        /// <summary>
        /// No element matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchElement = 7;

        /// <summary>
        /// No frame matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchFrame = 8;

        /// <summary>
        /// The functionality is not supported.
        /// </summary>
        public static readonly int UnknownCommand = 9;

        /// <summary>
        /// The specified element is no longer valid.
        /// </summary>
        public static readonly int ObsoleteElement = 10;

        /// <summary>
        /// The specified element is not displayed.
        /// </summary>
        public static readonly int ElementNotDisplayed = 11;

        /// <summary>
        /// The specified element is not enabled.
        /// </summary>
        public static readonly int InvalidElementState = 12;

        /// <summary>
        /// An unhandled error occurred.
        /// </summary>
        public static readonly int UnhandledError = 13;

        /// <summary>
        /// An error occurred; but it was expected.
        /// </summary>
        public static readonly int ExpectedError = 14;

        /// <summary>
        /// The specified element is not selected.
        /// </summary>
        public static readonly int ElementNotSelectable = 15;

        /// <summary>
        /// No document matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchDocument = 16;

        /// <summary>
        /// An unexpected JavaScript error occurred.
        /// </summary>
        public static readonly int UnexpectedJavaScriptError = 17;

        /// <summary>
        /// No result is available from the JavaScript execution.
        /// </summary>
        public static readonly int NoScriptResult = 18;

        /// <summary>
        /// The result from the JavaScript execution is not recognized.
        /// </summary>
        public static readonly int XPathLookupError = 19;

        /// <summary>
        /// No collection matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchCollection = 20;

        /// <summary>
        /// A timeout occurred.
        /// </summary>
        public static readonly int Timeout = 21;

        /// <summary>
        /// A null pointer was received.
        /// </summary>
        public static readonly int NullPointer = 22;

        /// <summary>
        /// No window matching the criteria exists.
        /// </summary>
        public static readonly int NoSuchWindow = 23;

        /// <summary>
        /// An illegal attempt was made to set a cookie under a different domain than the current page.
        /// </summary>
        public static readonly int InvalidCookieDomain = 24;

        /// <summary>
        /// A request to set a cookie's value could not be satisfied.
        /// </summary>
        public static readonly int UnableToSetCookie = 25;

        /// <summary>
        /// An alert was found open unexpectedly.
        /// </summary>
        public static readonly int UnexpectedAlertOpen = 26;

        /// <summary>
        /// A request was made to switch to an alert; but no alert is currently open.
        /// </summary>
        public static readonly int NoAlertPresent = 27;

        /// <summary>
        /// An asynchronous JavaScript execution timed out.
        /// </summary>
        public static readonly int AsyncScriptTimeout = 28;

        /// <summary>
        /// The coordinates of the element are invalid.
        /// </summary>
        public static readonly int InvalidElementCoordinates = 29;

        /// <summary>
        /// The selector used (CSS/XPath) was invalid.
        /// </summary>
        public static readonly int InvalidSelector = 32;
    }
}
