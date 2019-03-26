using WinAppDriver.Extensions;
using WinAppDriver.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using UIAutomationClient;
using ManagedWinapi.Windows;

namespace WinAppDriver.Behaviors
{
    public enum UnexpectedAlertBehaviorReaction
    {
        Accept,
        AcceptAndNotify,
        Dismiss,
        DismissAndNotify,
        Ignore
    }

    public class UnexpectedAlertEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string[] Content { get; set; }

        public override string ToString()
        {
            return base.ToString() + ":" + Title + " " + Content;
        }
    }

    public class UnexpectedAlertBehavior
    {
        private static AutomationEventHandler _prev;

        public static string Setting = "ignore";

        public delegate void UnexpectedAlertEventHandler(object sender, UnexpectedAlertEventArgs e);

        public void TryRegister(Dictionary<string, object> desiredCapabilities)
        {
            desiredCapabilities.TryGetValue(OpenQA.Selenium.Remote.CapabilityType.UnexpectedAlertBehavior, out var unexpectedAlertBehavior);
            Setting = unexpectedAlertBehavior?.ToString().ToLowerInvariant() ?? "ignore";
        }

        public static IDisposable CreateHandler(AutomationElement automationElement, IntPtr hWnd, CommandEnvironment commandEnvironment)
        {
            return new Handler(automationElement, hWnd, commandEnvironment);
        }

        public class Handler : IDisposable
        {
            private AutomationEventHandler _handler;
            private readonly AutomationElement _automationElement;
            private IUIAutomation _automation;
            private Thread _thread, _threadB;
            private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            private SystemWindow _rootWindow;
            private IList<SystemWindow> _childWindows;

            public event UnexpectedAlertEventHandler OnUnexpectedAlert;

            public Handler(AutomationElement automationElement, IntPtr hWnd, CommandEnvironment commandEnvironment)
            {
                var behavior = commandEnvironment.UnexpectedAlertBehavior;
                if (behavior == UnexpectedAlertBehaviorReaction.Ignore)
                {
                    return;
                }

                _rootWindow = new SystemWindow(hWnd);

                IList<SystemWindow> GetWindows()
                {
                    // dialog windows happen not be in the same tree as the root window
                    return SystemWindow.DesktopWindow.AllDescendantWindows
                        .Where(w => w.Visible && w.TopMost && w.Process.MainWindowHandle == hWnd)
                        .ToList();
                }

                _childWindows = GetWindows();

                var token = _cancellationTokenSource.Token;
                _thread = new Thread(() =>
                {
                    while (true)
                    {
                        //System.Diagnostics.Debug.WriteLine("Thread work ... " + Thread.CurrentThread.ManagedThreadId);
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        var windows = GetWindows();
                        UnexpectedAlertEventArgs args = null;
                        var raiseEvent = false;

                        foreach (var window in windows)
                        {
                            if (_childWindows.Contains(window) || !window.Visible)
                            {
                                continue;
                            }

                            raiseEvent = 
                                behavior == UnexpectedAlertBehaviorReaction.DismissAndNotify ||
                                behavior == UnexpectedAlertBehaviorReaction.AcceptAndNotify;

                            if (raiseEvent)
                            {
                                args = new UnexpectedAlertEventArgs
                                {
                                    Title = window.Title,
                                    Content = window.AllDescendantWindows
                                        .Where(s => s.Visible)
                                        .Select(d => d.Title?.Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .ToArray()
                                };
                            }

                            try
                            {
                                // menus were windows too
                                var aw = AutomationElement.FromHandle(window.HWnd);
                            }
                            catch (ElementNotAvailableException)
                            {
                                continue; // but menus cause this exception
                            }

                            System.Diagnostics.Debug.WriteLine("New modal window " + window.HWnd);

                            switch (behavior)
                            {
                                case UnexpectedAlertBehaviorReaction.Ignore:
                                    break;
                                case UnexpectedAlertBehaviorReaction.Dismiss:
                                case UnexpectedAlertBehaviorReaction.DismissAndNotify:
                                case UnexpectedAlertBehaviorReaction.Accept:
                                case UnexpectedAlertBehaviorReaction.AcceptAndNotify:
                                    //window.SendClose();
                                    break;
                            }
                        }

                        if (raiseEvent)
                        {
                            OnUnexpectedAlert?.Invoke(this, args);
                        }

                        _childWindows = GetWindows();

                        Thread.Sleep(1000);
                    }
                });

                _thread.Start(); // TODO new thread for each session (OK), not disposed properly
            }

            public void Dispose()
            {
                _cancellationTokenSource.Cancel();

                _thread = null;
            }
        }
    }
}
