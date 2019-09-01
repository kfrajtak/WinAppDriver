// <copyright file="DriverCommand.cs" company="Salesforce.com">
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
    /// Values describing the list of commands understood by a remote server using the JSON wire protocol.
    /// NOTE: Copied from the WebDriver source code. Used to avoid an extra reference to the WebDriver
    /// assembly in the Windows Phone application.
    /// </summary>
    public static class DriverCommand
    {
        /// <summary>
        /// Represents the Actions command.
        /// </summary>
        public static readonly string Actions = "actions";

        /// <summary>
        /// Represents the Status command.
        /// </summary>
        public static readonly string Status = "status";

        /// <summary>
        /// Represents a New Session command
        /// </summary>
        public static readonly string NewSession = "newSession";

        /// <summary>
        /// Represents the Get Session List command
        /// </summary>
        public static readonly string GetSessionList = "getSessionList";

        /// <summary>
        /// Represents the Get Session Capabilities command
        /// </summary>
        public static readonly string GetSessionCapabilities = "getSessionCapabilities";

        /// <summary>
        /// Represents a Browser close command
        /// </summary>
        public static readonly string Close = "close";

        /// <summary>
        /// Represents a browser quit command
        /// </summary>
        public static readonly string Quit = "quit";

        /// <summary>
        /// Represents a GET command
        /// </summary>
        public static readonly string Get = "get";

        /// <summary>
        /// Represents a Browser going back command
        /// </summary>
        public static readonly string GoBack = "goBack";

        /// <summary>
        /// Represents a Browser going forward command
        /// </summary>
        public static readonly string GoForward = "goForward";

        /// <summary>
        /// Represents a Browser refreshing command
        /// </summary>
        public static readonly string Refresh = "refresh";

        /// <summary>
        /// Represents adding a cookie command
        /// </summary>
        public static readonly string AddCookie = "addCookie";

        /// <summary>
        /// Represents getting all cookies command
        /// </summary>
        public static readonly string GetAllCookies = "getCookies";

        /// <summary>
        /// Represents deleting a cookie command
        /// </summary>        
        public static readonly string DeleteCookie = "deleteCookie";

        /// <summary>
        /// Represents Deleting all cookies command
        /// </summary>
        public static readonly string DeleteAllCookies = "deleteAllCookies";

        /// <summary>
        /// Represents FindElement command
        /// </summary>
        public static readonly string FindElement = "findElement";

        /// <summary>
        /// Represents FindElements command
        /// </summary>
        public static readonly string FindElements = "findElements";

        /// <summary>
        /// Represents FindChildElement command
        /// </summary>
        public static readonly string FindChildElement = "findChildElement";

        /// <summary>
        /// Represents FindChildElements command
        /// </summary>
        public static readonly string FindChildElements = "findChildElements";

        /// <summary>
        /// Describes an element
        /// </summary>
        public static readonly string DescribeElement = "describeElement";

        /// <summary>
        /// Represents ClearElement command
        /// </summary>
        public static readonly string ClearElement = "clearElement";

        /// <summary>
        /// Represents ClickElement command
        /// </summary>
        public static readonly string ClickElement = "clickElement";

        /// <summary>
        /// Represents SendKeysToElements command
        /// </summary>
        public static readonly string SendKeysToElement = "sendKeysToElement";

        /// <summary>
        /// Represents SubmitElement command
        /// </summary>
        public static readonly string SubmitElement = "submitElement";

        /// <summary>
        /// Represents GetCurrentWindowHandle command
        /// </summary>
        public static readonly string GetCurrentWindowHandle = "getCurrentWindowHandle";

        /// <summary>
        /// Represents GetWindowHandles command
        /// </summary>
        public static readonly string GetWindowHandles = "getWindowHandles";

        /// <summary>
        /// Represents SwitchToWindow command
        /// </summary>
        public static readonly string SwitchToWindow = "switchToWindow";

        /// <summary>
        /// Represents SwitchToFrame command
        /// </summary>
        public static readonly string SwitchToFrame = "switchToFrame";

        /// <summary>
        /// Represents GetActiveElement command
        /// </summary>
        public static readonly string GetActiveElement = "getActiveElement";

        /// <summary>
        /// Represents GetCurrentUrl command
        /// </summary>
        public static readonly string GetCurrentUrl = "getCurrentUrl";

        /// <summary>
        /// Represents GetPageSource command
        /// </summary>
        public static readonly string GetPageSource = "getPageSource";

        /// <summary>
        /// Represents GetTitle command
        /// </summary>
        public static readonly string GetTitle = "getTitle";

        /// <summary>
        /// Represents ExecuteScript command
        /// </summary>
        public static readonly string ExecuteScript = "executeScript";

        /// <summary>
        /// Represents ExecuteAsyncScript command
        /// </summary>
        public static readonly string ExecuteAsyncScript = "executeAsyncScript";

        /// <summary>
        /// Represents GetElementText command
        /// </summary>
        public static readonly string GetElementText = "getElementText";

        /// <summary>
        /// Represents GetElementTagName command
        /// </summary>
        public static readonly string GetElementTagName = "getElementTagName";

        /// <summary>
        /// Represents IsElementSelected command
        /// </summary>
        public static readonly string IsElementSelected = "isElementSelected";

        /// <summary>
        /// Represents IsElementEnabled command
        /// </summary>
        public static readonly string IsElementEnabled = "isElementEnabled";

        /// <summary>
        /// Represents IsElementDisplayed command
        /// </summary>
        public static readonly string IsElementDisplayed = "isElementDisplayed";

        /// <summary>
        /// Represents GetElementLocation command
        /// </summary>
        public static readonly string GetElementLocation = "getElementLocation";

        /// <summary>
        /// Represents GetElementLocationOnceScrolledIntoView command
        /// </summary>
        public static readonly string GetElementLocationOnceScrolledIntoView = "getElementLocationOnceScrolledIntoView";

        /// <summary>
        /// Represents GetElementSize command
        /// </summary>
        public static readonly string GetElementSize = "getElementSize";

        /// <summary>
        /// Represents GetElementAttribute command
        /// </summary>
        public static readonly string GetElementAttribute = "getElementAttribute";

        /// <summary>
        /// Represents GetElementValueOfCSSProperty command
        /// </summary>
        public static readonly string GetElementValueOfCssProperty = "getElementValueOfCssProperty";

        /// <summary>
        /// Represents ElementEquals command
        /// </summary>
        public static readonly string ElementEquals = "elementEquals";

        /// <summary>
        /// Represents Screenshot command
        /// </summary>
        public static readonly string Screenshot = "screenshot";

        /// <summary>
        /// Represents GetOrientation command
        /// </summary>
        public static readonly string GetOrientation = "getOrientation";

        /// <summary>
        /// Represents SetOrientation command
        /// </summary>
        public static readonly string SetOrientation = "setOrientation";

        /// <summary>
        /// Represents GetWindowSize command
        /// </summary>
        public static readonly string GetWindowSize = "getWindowSize";

        /// <summary>
        /// Represents GetWindowRect command
        /// </summary>
        public static readonly string GetWindowRect = "getWindowRect";

        /// <summary>
        /// Represents SetWindowSize command
        /// </summary>
        public static readonly string SetWindowSize = "setWindowSize";

        /// <summary>
        /// Represents SetWindowRect command
        /// </summary>
        public static readonly string SetWindowRect = "setWindowRect";

        /// <summary>
        /// Represents GetWindowPosition command
        /// </summary>
        public static readonly string GetWindowPosition = "getWindowPosition";

        /// <summary>
        /// Represents SetWindowPosition command
        /// </summary>
        public static readonly string SetWindowPosition = "setWindowPosition";

        /// <summary>
        /// Represents MaximizeWindow command
        /// </summary>
        public static readonly string MaximizeWindow = "maximizeWindow";

        /// <summary>
        /// Represents MinimizeWindow command
        /// </summary>
        public static readonly string MinimizeWindow = "minimizeWindow";

        /// <summary>
        /// Represents the DismissAlert command
        /// </summary>
        public static readonly string DismissAlert = "dismissAlert";

        /// <summary>
        /// Represents the AcceptAlert command
        /// </summary>
        public static readonly string AcceptAlert = "acceptAlert";

        /// <summary>
        /// Represents the GetAlertText command
        /// </summary>
        public static readonly string GetAlertText = "getAlertText";

        /// <summary>
        /// Represents the SetAlertValue command
        /// </summary>
        public static readonly string SetAlertValue = "setAlertValue";

        /// <summary>
        /// Represents the ImplicitlyWait command
        /// </summary>
        public static readonly string ImplicitlyWait = "implicitlyWait";

        /// <summary>
        /// Represents the SetAsyncScriptTimeout command
        /// </summary>
        public static readonly string SetAsyncScriptTimeout = "setScriptTimeout";

        /// <summary>
        /// Represents the SetTimeout command
        /// </summary>
        public static readonly string SetTimeout = "setTimeout";

        /// <summary>
        /// Represents the MouseClick command.
        /// </summary>
        public static readonly string MouseClick = "mouseClick";

        /// <summary>
        /// Represents the MouseDoubleClick command.
        /// </summary>
        public static readonly string MouseDoubleClick = "mouseDoubleClick";

        /// <summary>
        /// Represents the MouseDown command.
        /// </summary>
        public static readonly string MouseDown = "mouseDown";

        /// <summary>
        /// Represents the MouseUp command.
        /// </summary>
        public static readonly string MouseUp = "mouseUp";

        /// <summary>
        /// Represents the MouseMoveTo command.
        /// </summary>
        public static readonly string MouseMoveTo = "mouseMoveTo";

        /// <summary>
        /// Represents the SendKeysToActiveElement command.
        /// </summary>
        public static readonly string SendKeysToActiveElement = "sendKeysToActiveElement";

        /// <summary>
        /// Represents the UploadFile command.
        /// </summary>
        public static readonly string UploadFile = "uploadFile";

        /// <summary>
        /// Represents the TouchSingleTap command.
        /// </summary>
        public static readonly string TouchSingleTap = "touchSingleTap";

        /// <summary>
        /// Represents the TouchPress command.
        /// </summary>
        public static readonly string TouchPress = "touchDown";

        /// <summary>
        /// Represents the TouchRelease command.
        /// </summary>
        public static readonly string TouchRelease = "touchUp";

        /// <summary>
        /// Represents the TouchMove command.
        /// </summary>
        public static readonly string TouchMove = "touchMove";

        /// <summary>
        /// Represents the TouchScroll command.
        /// </summary>
        public static readonly string TouchScroll = "touchScroll";

        /// <summary>
        /// Represents the TouchDoubleTap command.
        /// </summary>
        public static readonly string TouchDoubleTap = "touchDoubleTap";

        /// <summary>
        /// Represents the TouchLongPress command.
        /// </summary>
        public static readonly string TouchLongPress = "touchLongPress";

        /// <summary>
        /// Represents the TouchFlick command.
        /// </summary>
        public static readonly string TouchFlick = "touchFlick";

        public static readonly string GetElementRect = "getElementRect";

        public static readonly string SetTimeouts = "setTimeouts";
    }
}