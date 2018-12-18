using WinAppDriver.Extensions;
using WinAppDriver.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using UIAutomationClient;

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

    public class UnexpectedAlertBehavior
    {
        private static AutomationEventHandler _prev;

        public static string Setting = "ignore";

        public void TryRegister(Dictionary<string, object> desiredCapabilities)
        {
            desiredCapabilities.TryGetValue(OpenQA.Selenium.Remote.CapabilityType.UnexpectedAlertBehavior, out var unexpectedAlertBehavior);
            Setting = unexpectedAlertBehavior?.ToString().ToLowerInvariant() ?? "ignore";
        }

        public static IDisposable CreateHandler(AutomationElement automationElement, CommandEnvironment commandEnvironment)
        {
            return new Handler(automationElement, commandEnvironment);
        }

        public class Handler : IDisposable//, IUIAutomationEventHandler, IUIAutomationStructureChangedEventHandler
        {
            private AutomationEventHandler _handler;
            private readonly AutomationElement _automationElement;
            private IUIAutomation _automation;
            private Thread _thread;
            private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

            public Handler(AutomationElement automationElement, CommandEnvironment commandEnvironment)
            {
                var token = _cancellationTokenSource.Token;
                _automationElement = automationElement;
                _automation = new CUIAutomation8();

                var behavior = commandEnvironment.UnexpectedAlertBehavior;

                if (automationElement.Current.ControlType != ControlType.Window)
                {
                    return;
                }

                var condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
                var before = _automationElement.FindAll(System.Windows.Automation.TreeScope.Children, condition).Cast<AutomationElement>().ToList();

                _thread = new Thread(() =>
                {
                    while (true)
                    {
                        System.Diagnostics.Debug.WriteLine("Thread work ...");
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        var windows = _automationElement.FindAll(System.Windows.Automation.TreeScope.Children, condition);
                        //.NativeElement.FindAll(UIAutomationClient.TreeScope.TreeScope_Children, _automation.CreateTrueCondition());
                        foreach (AutomationElement window in windows) //for (int i = 0; i < windows.Length; i++)
                        {
                            if (before.Contains(window))// windows.GetElement(i)))
                            {
                                continue;
                            }

                            if (!window.IsTopMostWindow())
                            {
                                continue;
                            }

                            if (window.IsModalWindow())
                            {
                                System.Diagnostics.Debug.WriteLine("New modal window " + window.ToDiagString());
                                ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();

                                switch (behavior)
                                {
                                    /*case UnexpectedAlertBehaviorReaction.Ignore:
                                        break;
                                    case UnexpectedAlertBehaviorReaction.Dismiss:
                                        ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
                                        break;
                                    case UnexpectedAlertBehaviorReaction.DismissAndNotify:
                                        ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
                                        throw new Exception(behavior.ToString());
                                    case UnexpectedAlertBehaviorReaction.Accept:
                                        break;*/
                                    case UnexpectedAlertBehaviorReaction.DismissAndNotify:
                                    case UnexpectedAlertBehaviorReaction.AcceptAndNotify:
                                        ((WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern)).Close();
                                        throw new Exception(behavior.ToString());
                                }
                            }
                        }

                        Thread.Sleep(500);
                    }
                });

                _thread.Start();

                // TODO does not work
                Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, _automationElement, System.Windows.Automation.TreeScope.Descendants,
                   (sender, e) =>
                   {
                       System.Diagnostics.Debug.WriteLine("XXXX");
                       var element = sender as AutomationElement;
                       if (element.GetCurrentPropertyValue(
                        AutomationElement.ClassNameProperty) as string != "#32770")
                       {
                           return;
                       }

                       var text = element.FindFirst(System.Windows.Automation.TreeScope.Children,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, "65535"));
                       if (text != null)
                       {
                           System.Console.WriteLine(text.Current.Name);
                       }
                   });
                
                _automationElement = automationElement;

                
                IUIAutomationCacheRequest cacheRequest = _automation.CreateCacheRequest();

                cacheRequest.AddProperty(30011/*UIA_AutomationIdPropertyId*/);
                cacheRequest.AddProperty(30003/*UIA_ControlTypePropertyId*/);
                //cacheRequest.AddProperty(_propertyIdBoundingRectangle);

                // The above properties are all we'll need, so we have have no need for a reference 
                // to the source element when we receive the event. 
                //cacheRequest.AutomationElementMode = UIAutomationClient.AutomationElementMode.AutomationElementMode_None;
                /*
                _automation.AddAutomationEventHandler(
                   WindowPattern.WindowOpenedEvent.Id, 
                   automationElement.NativeElement,
                   UIAutomationClient.TreeScope.TreeScope_Element | UIAutomationClient.TreeScope.TreeScope_Descendants,
                   cacheRequest, 
                   this);

                // Now set up the event handler. 
                _automation.AddStructureChangedEventHandler(
                    //WindowPattern.WindowOpenedEvent.Id, 
                    automationElement.NativeElement, 
                    UIAutomationClient.TreeScope.TreeScope_Element | UIAutomationClient.TreeScope.TreeScope_Descendants, 
                    cacheRequest, 
                    this);*/
                /*
                IUIAutomation uiAutomation = new CUIAutomation8();

                IUIAutomationElement rootElement = uiAutomation.GetRootElement();
                
                uiAutomation.AddAutomationEventHandler(
                    20016, // UIA_Window_WindowOpenedEventId
                    automationElement.NativeElement,
                    UIAutomationClient.TreeScope.TreeScope_Descendants,
                    null,
                    this);*/

                //_fAddedEventHandler = true;



                _handler = (sender, e) =>
                {
                    var element = sender as AutomationElement;
                    System.Diagnostics.Debug.WriteLine($"Window was opened {element.ToDiagString()}.");

                    /*

                    var text = element.FindFirst(TreeScope.Children,
                      new PropertyCondition(AutomationElement.AutomationIdProperty,
                                            "65535"));
                    if (text != null && text.Current.Name == "Succeeded!")
                    {
                        succeeded = true;
                        var okButton = element.FindFirst(TreeScope.Children,
                          new PropertyCondition(AutomationElement.AutomationIdProperty,
                                                "2"));
                        var invokePattern = okButton.GetCurrentPattern(
                          InvokePattern.Pattern) as InvokePattern;
                        invokePattern.Invoke();
                    }

                    resultReady.Set();*/
                };

                /*ACCEPT, ACCEPT_AND_NOTIFY, DISMISS, DISMISS_AND_NOTIFY, IGNORE */
                //Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, automationElement, TreeScope.Children, OnWindowOpenOrClose);
                //Automation.AddStructureChangedEventHandler(automationElement, System.Windows.Automation.TreeScope.Element, OnStructureChanged);
            }
            /*
            private void OnStructureChanged(object sender, StructureChangedEventArgs e)
            {
                var element = sender as AutomationElement;
                System.Diagnostics.Debug.WriteLine($"Structural change {element.ToDiagString()} + {e.StructureChangeType}");
            }

            private void OnWindowOpenOrClose(object sender, AutomationEventArgs e)
            {
                var element = sender as AutomationElement;
                System.Diagnostics.Debug.WriteLine($"Window was opened {element.ToDiagString()}.");
            }*/

            public void Dispose()
            {
                _cancellationTokenSource.Cancel();

                _thread = null;

                if (_automationElement != null)
                {
                    //Automation.RemoveAutomationEventHandler(WindowPattern.WindowOpenedEvent, _automationElement, OnWindowOpenOrClose);

                    //_automation.RemoveAutomationEventHandler(WindowPattern.WindowOpenedEvent.Id, _automationElement.NativeElement, this);
                    //_automation.RemoveStructureChangedEventHandler(_automationElement.NativeElement, this);
                    //Automation.RemoveStructureChangedEventHandler(_automationElement, OnStructureChanged);
                }                
            }
            /*
            void IUIAutomationEventHandler.HandleAutomationEvent(IUIAutomationElement sender, int eventId)
            {
                var automationId = sender.GetCachedPropertyValue(30011);
                //var element = sender as AutomationElement;
                System.Diagnostics.Debug.WriteLine($"Window was opened.");
            }

            void IUIAutomationStructureChangedEventHandler.HandleStructureChangedEvent(IUIAutomationElement sender, UIAutomationClient.StructureChangeType changeType, Array runtimeId)
            {
                var automationId = sender.GetCachedPropertyValue(30011);
                var contolType = sender.GetCachedPropertyValue(30003);
                System.Diagnostics.Debug.WriteLine($"Structural change");
            }*/
        }

        public static class Interop
        {
            public enum ShellEvents : int
            {
                HSHELL_WINDOWCREATED = 1,
                HSHELL_WINDOWDESTROYED = 2,
                HSHELL_ACTIVATESHELLWINDOW = 3,
                HSHELL_WINDOWACTIVATED = 4,
                HSHELL_GETMINRECT = 5,
                HSHELL_REDRAW = 6,
                HSHELL_TASKMAN = 7,
                HSHELL_LANGUAGE = 8,
                HSHELL_ACCESSIBILITYSTATE = 11,
                HSHELL_APPCOMMAND = 12
            }
            [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int RegisterWindowMessage(string lpString);
            [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int DeregisterShellHookWindow(IntPtr hWnd);
            [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int RegisterShellHookWindow(IntPtr hWnd);
            [DllImport("user32", EntryPoint = "GetWindowTextA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder lpString, int cch);
            [DllImport("user32", EntryPoint = "GetWindowTextLengthA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int GetWindowTextLength(IntPtr hwnd);
        }
    }
}
