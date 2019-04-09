// <copyright file="CommandEnvironment.cs" company="Salesforce.com">
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

using WinAppDriver.Behaviors;
using WinAppDriver.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;

namespace WinAppDriver.Server
{
    /// <summary>
    /// The environment in which commands are run.
    /// </summary>
    public class CommandEnvironment : IDisposable
    {
        /// <summary>
        /// The key used to denote a window object.
        /// </summary>
        public const string WindowObjectKey = "WINDOW";

        /// <summary>
        /// The key used to denote an element object.
        /// </summary>
        public const string ElementObjectKey = "ELEMENT";

        /// <summary>
        /// The global window handle string used, since the driver only supports one window.
        /// </summary>
        public const string GlobalWindowHandle = "WPDriverWindowHandle";

        private IntPtr _hwnd, _windowHwnd;
        private string focusedFrame = string.Empty;
        private Dictionary<string, object> keyboardState = null;
        private Dictionary<string, object> mouseState = new Dictionary<string, object>();

        private Dictionary<string, object> _desiredCapabilities;

        private CancellationTokenSource _cancellationTokenSource;

        private int implicitWaitTimeout = 60000;
        private int asyncScriptTimeout = -1;
        private int pageLoadTimeout = -1;
        private bool isBlocked;
        private string alertText = string.Empty;
        private string alertType = string.Empty;

        private readonly UnexpectedAlertBehaviorReaction _unexpectedAlertBehavior = UnexpectedAlertBehaviorReaction.DismissAndNotify;

        public UnexpectedAlertBehaviorReaction UnexpectedAlertBehavior => _unexpectedAlertBehavior;

        /*// <summary>
        /// Initializes a new instance of the <see cref="CommandEnvironment"/> class.
        /// </summary>
        /// <param name="root">The root control</param>
        public CommandEnvironment(IntPtr hwnd)
        {
            _hwnd = hwnd;
            _windowHwnd = hwnd;

            SwitchToWindow(AutomationElement.FromHandle(hwnd));
            //Cache = new ElementCache(hwnd, AutomationElement.FromHandle(hwnd));
        }*/

        public CommandEnvironment(string sessionId, Dictionary<string, object> desiredCapabilities)
        {
            SessionId = sessionId;
           
            _hwnd = new IntPtr(int.Parse(sessionId));
            var element = AutomationElement.FromHandle(_hwnd);

            _desiredCapabilities = desiredCapabilities ?? new Dictionary<string, object>();

            SwitchToWindow(_hwnd, element);

            if (!desiredCapabilities.TryGetValue(OpenQA.Selenium.Remote.CapabilityType.UnexpectedAlertBehavior, out var unexpectedAlertBehavior) ||
                !Enum.TryParse(unexpectedAlertBehavior?.ToString(), true, out _unexpectedAlertBehavior))
            {
                _unexpectedAlertBehavior = UnexpectedAlertBehaviorReaction.DismissAndNotify;
            }

            Cache.AddHandler(Behaviors.UnexpectedAlertBehavior.CreateHandler(element, _hwnd, this));
            CacheStore.CommandStore.AddOrUpdate(sessionId, this, (k, c) => c);
        }

        public CommandEnvironment() { }

        public UnexpectedAlertBehavior2.Handler Handler { get; set; }

        public UnexpectedAlertEventArgs Unexpected { get; set; }

        public CancellationToken GetCancellationToken()
        {
            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                //_cancellationTokenSource.CancelAfter(ImplicitWaitTimeout);
            }
            return _cancellationTokenSource.Token;
        }

        /// <summary>
        /// Gets or sets the keyboard state of the driver.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Values are typed correctly for JSON serialization/deserialization.")]
        public Dictionary<string, object> KeyboardState
        {
            get { return this.keyboardState; }
            set { this.keyboardState = value; }
        }

        /// <summary>
        /// Gets or sets the mouse state of the driver.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Values are typed correctly for JSON serialization/deserialization.")]
        public Dictionary<string, object> MouseState
        {
            get { return this.mouseState; }
            set { this.mouseState = value; }
        }

        /// <summary>
        /// Gets the text of the active alert.
        /// </summary>
        public string AlertText
        {
            get { return this.alertText; }
        }

        /// <summary>
        /// Gets the type of the active alert.
        /// </summary>
        public string AlertType
        {
            get { return this.alertType; }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<IntPtr, ElementCache> _elementCache
            = new System.Collections.Concurrent.ConcurrentDictionary<IntPtr, ElementCache>();

        public ElementCache Cache => _elementCache[WindowHandle];

        public IntPtr ApplicationWindowHandle => _hwnd;

        public IntPtr WindowHandle => _windowHwnd;

        /// <summary>
        /// Gets a value indicating whether execution of the next command should be blocked.
        /// </summary>
        public bool IsBlocked
        {
            get { return this.isBlocked; }
        }

        /// <summary>
        /// Gets or sets the ID of the currently focused frame in the browser.
        /// </summary>
        public string FocusedFrame
        {
            get { return this.focusedFrame; }
            set { this.focusedFrame = value; }
        }

        /// <summary>
        /// Gets or sets the implicit wait timeout in milliseconds.
        /// </summary>
        public int ImplicitWaitTimeout
        {
            get { return this.implicitWaitTimeout; }
            set { this.implicitWaitTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the asynchronous script timeout in milliseconds.
        /// </summary>
        public int AsyncScriptTimeout
        {
            get { return this.asyncScriptTimeout; }
            set { this.asyncScriptTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the page load timeout in milliseconds.
        /// </summary>
        public int PageLoadTimeout
        {
            get { return this.pageLoadTimeout; }
            set { this.pageLoadTimeout = value; }
        }

        public string SessionId { get; private set; }

        /// <summary>
        /// Creates a serializable object for the currently focused frame.
        /// </summary>
        /// <returns>A <see cref="Dictionary{string, object}"/> representing the currently focused
        /// frame that can be serialized into a format the atoms will expect.</returns>
        public Dictionary<string, object> CreateFrameObject()
        {
            if (string.IsNullOrEmpty(this.focusedFrame))
            {
                return null;
            }

            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            returnValue[WindowObjectKey] = this.focusedFrame;
            return returnValue;
        }

        /// <summary>
        /// Clears the alert status of the driver.
        /// </summary>
        public void ClearAlertStatus()
        {
            this.isBlocked = false;
            this.alertType = string.Empty;
            this.alertText = string.Empty;
        }

        private void ClearCache()
        {
            //await this.browser.ClearInternetCacheAsync();
        }

        private void BrowserNavigatingEventHandler(object sender, EventArgs e)
        {
            this.mouseState.Clear();
            Dictionary<string, object> clientXY = new Dictionary<string, object>();
            clientXY["x"] = 0;
            clientXY["y"] = 0;
            this.mouseState["clientXY"] = clientXY;
            this.mouseState["element"] = null;
        }

        private void BrowserScriptNotifyEventHandler(object sender, EventArgs e)
        {
            if (!this.isBlocked)
            { 
                /*string[] valueParts = e.Value.Split(new char[] { ':' }, 2);
                this.alertType = valueParts[0];
                this.alertText = valueParts[1];
                this.isBlocked = true;*/
            }
            else
            {
                this.ClearAlertStatus();
            }
        }

        public void SwitchToWindow(AutomationElement window)
        {
            SwitchToWindow(window.NativeElement.CurrentNativeWindowHandle, window);
        }

        private void SwitchToWindow(IntPtr windowHwnd, AutomationElement window)
        {
            if (windowHwnd.ToInt64() == 0)
            {
                throw new Exception("Window handle is 0.");
            }

            var cache = _elementCache.AddOrUpdate(windowHwnd,
                hwnd => new ElementCache(window),
                (e, c) => c);

            cache.PrevWindowsHandle = _windowHwnd;
            _windowHwnd = cache.Handle;
            window.SetFocus();
        }

        public void CloseWindow(IntPtr hwnd)
        {
            if (ApplicationWindowHandle == hwnd)
            {
                return;
            }

            if (_elementCache.TryRemove(hwnd, out var cache))
            {
                _windowHwnd = cache.PrevWindowsHandle;
                cache.Dispose();
            }
        }

        public void Dispose()
        {
            if (Cache != null)
            {
                Cache.Dispose();
            }

            Microsoft.Test.Input.Mouse.Reset();
            Microsoft.Test.Input.Keyboard.Reset();
        }
    }
}
