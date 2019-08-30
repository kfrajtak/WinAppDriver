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
using System.Linq;
using WinAppDriver.Extensions;

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

        private Dictionary<string, object> _desiredCapabilities;

        private bool isBlocked;
        private string alertText = string.Empty;
        private string alertType = string.Empty;

        private readonly UnexpectedAlertBehaviorReaction _unexpectedAlertBehavior = UnexpectedAlertBehaviorReaction.DismissAndNotify;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

            if (!desiredCapabilities.TryGetValue(OpenQA.Selenium.Remote.CapabilityType.UnexpectedAlertBehavior, out var unexpectedAlertBehavior)
                || !Enum.TryParse(unexpectedAlertBehavior?.ToString(), true, out _unexpectedAlertBehavior))
            {
                _unexpectedAlertBehavior = UnexpectedAlertBehaviorReaction.DismissAndNotify;
            }

            Cache.AddHandler(Behaviors.UnexpectedAlertBehavior.CreateHandler(element, _hwnd, this));
            CacheStore.CommandStore.AddOrUpdate(sessionId, this, (_, c) => c);

            ImplicitWaitTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

            AutomationEventHandler eventHandler = new AutomationEventHandler(OnWindowOpenOrClose);
            Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, element, TreeScope.Element, eventHandler);
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, element, TreeScope.Element, eventHandler);            
        }

        public CommandEnvironment() { }

        public AutomationElement RootElement => AutomationElement.FromHandle(_hwnd);

        public UnexpectedAlertBehavior2.Handler Handler { get; set; }

        public UnexpectedAlertEventArgs Unexpected { get; set; }

        public object GetDesiredCapabilityValue(string capability)
        {
            if (_desiredCapabilities.TryGetValue(capability, out var value))
            {
                return value;
            }

            return null;
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
        /// Gets or sets the implicit wait timeout in milliseconds.
        /// </summary>
        public int ImplicitWaitTimeout { get; set; }

        /// <summary>
        /// Gets or sets the page load timeout in milliseconds.
        /// </summary>
        public int PageLoadTimeout { get; set; } = -1;

        public string SessionId { get; }

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

        public IEnumerable<AutomationElement> GetWindows(CancellationToken cancellationToken)
        {
            return new BreadthFirstSearch().Find(RootElement, ControlType.Window, cancellationToken)
                .Where(w => w.Current.ControlType == ControlType.Window)
                .ToList();
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

            string elementKey = null;
            ElementCache parentCache = null;
            // find the window in cache
            if (_elementCache.Count > 0)
            {
                parentCache = Cache;
                Cache.TryGetElementKey(window, out elementKey);
            }

            var cache = _elementCache.AddOrUpdate(windowHwnd,
                _ => new ElementCache(elementKey, window, parentCache),
                (_, c) => c);

            cache.PrevWindowsHandle = _windowHwnd;
            _windowHwnd = cache.Handle;
            window.SetFocus();
        }

        public AutomationElement GetModalWindow(CancellationToken cancellationToken)
        {
            if (_hwnd == IntPtr.Zero)
            {
                return null;
            }

            foreach (AutomationElement automationElement in GetWindows(cancellationToken))
            {
                if (automationElement.IsModalWindow())
                {
                    return automationElement;
                }
            }

            return null;
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
            Cache?.Dispose();

            Microsoft.Test.Input.Mouse.Reset();
            Microsoft.Test.Input.Keyboard.Reset();
        }
    }
}
