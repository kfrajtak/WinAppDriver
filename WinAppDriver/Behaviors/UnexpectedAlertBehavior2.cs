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
    public class UnexpectedAlertBehavior2
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
            private SystemWindow _rootWindow;
            private IList<SystemWindow> _childWindows;

            public event UnexpectedAlertEventHandler OnUnexpectedAlert;

            public Handler(AutomationElement automationElement, IntPtr hWnd, CommandEnvironment commandEnvironment)
            {
                /*return;
                var behavior = commandEnvironment.UnexpectedAlertBehavior;
                if (behavior == UnexpectedAlertBehaviorReaction.Ignore)
                {
                    return;
                }
                */
                _rootWindow = new SystemWindow(hWnd);

                _childWindows = GetWindows();

                /*var token = _cancellationTokenSource.Token;
                _thread = new Thread(() =>
                {
                    while (true)
                    {
                        System.Diagnostics.Debug.WriteLine("Thread work ... " + Thread.CurrentThread.ManagedThreadId);
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }*/
                        /*
                var windows = GetWindows();
                UnexpectedAlertEventArgs args = null;
                var raiseEvent = false;

                foreach (var window in windows)
                {
                    if (_childWindows.Contains(window))
                    {
                        continue;
                    }
                    */
                    /*raiseEvent =
                        behavior == UnexpectedAlertBehaviorReaction.DismissAndNotify ||
                        behavior == UnexpectedAlertBehaviorReaction.AcceptAndNotify;*/
                        /*
                    if (raiseEvent)
                    {
                        args = new UnexpectedAlertEventArgs
                        {
                            Title = window.Title,
                            Hwnd = window.HWnd,
                            Content = window.AllDescendantWindows
                                .Where(s => s.Visible)
                                .Select(d => d.Title?.Trim())
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToArray()
                        };
                    }

                    System.Diagnostics.Debug.WriteLine("New modal window " + window.HWnd);*/
                    /*
                            switch (behavior)
                            {
                                case UnexpectedAlertBehaviorReaction.Ignore:
                                    break;
                                case UnexpectedAlertBehaviorReaction.Dismiss:
                                case UnexpectedAlertBehaviorReaction.DismissAndNotify:
                                case UnexpectedAlertBehaviorReaction.Accept:
                                case UnexpectedAlertBehaviorReaction.AcceptAndNotify:
                                    window.SendClose();
                                    break;
                            }*/
                //}

                /*if (raiseEvent)
                {
                    OnUnexpectedAlert?.Invoke(this, args);
                }

                _childWindows = GetWindows();

                Thread.Sleep(1000);*/
            }
            /*});
             * 
             * public 

            _thread.Start();*/
            private IList<SystemWindow> GetWindows()
            {
                // dialog windows happen not be in the same tree as the root window
                return SystemWindow.DesktopWindow.AllDescendantWindows
                    .Where(w => w.Visible && w.TopMost && w.Process.MainWindowHandle == _rootWindow.HWnd)
                    .ToList();
            }

            private UnexpectedAlertEventArgs _args;

            public void Update()
            {
                if (!IsTopMostActiveWindowDifferent(out _args))
                {
                    _args = null;
                }
            }

            public bool IsFaulty => _args != null;

            public UnexpectedAlertEventArgs Unexpected => _args;

            public bool IsTopMostActiveWindowDifferent(out UnexpectedAlertEventArgs args)
            {
                var windows = GetWindows();
                args = null;
                foreach (var window in windows)
                {
                    if (_childWindows.Contains(window))
                    {
                        continue;
                    }

                    /*raiseEvent =
                        behavior == UnexpectedAlertBehaviorReaction.DismissAndNotify ||
                        behavior == UnexpectedAlertBehaviorReaction.AcceptAndNotify;*/

                    /*if (raiseEvent)
                    {*/
                    args = new UnexpectedAlertEventArgs
                    {
                        Title = window.Title,
                        Content = window.AllDescendantWindows
                             .Where(s => s.Visible)
                             .Select(d => d.Title?.Trim())
                             .Where(s => !string.IsNullOrEmpty(s))
                             .ToArray()
                    };

                    // TODO requires rewrite

                    // menus slip through
                    AutomationElement aw = null;
                    try
                    {
                        aw = AutomationElement.FromHandle(window.HWnd);
                    }
                    catch (ElementNotAvailableException)
                    {
                        continue; // menus are causing this exception
                    }

                    if (aw.Current.ControlType != ControlType.Window)
                    {
                        continue; // but sometimes they slip through
                    }

                    System.Diagnostics.Debug.WriteLine("New modal window " + window.HWnd);

                    return true;                    
                }

                return false;
            }

            public void Dispose()
            {
                
            }
        }
    }
}
