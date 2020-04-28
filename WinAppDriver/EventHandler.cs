using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WinAppDriver
{
    public class EventHandler
    {
        public static IDisposable RegisterHandler(AutomationEvent automationEvent, AutomationElement automationElement, AutomationEventHandler automationEventHandler)
        {
            return new Handler(automationEvent, automationElement, automationEventHandler);
        }

        private class Handler : IDisposable//, IUIAutomationEventHandler, IUIAutomationStructureChangedEventHandler
        {
            private AutomationEventHandler _handler;
            private readonly AutomationElement _automationElement;
            private readonly AutomationEvent _automationEvent;

            public Handler(AutomationEvent automationEvent, AutomationElement automationElement, AutomationEventHandler automationEventHandler)
            {
                _automationEvent = automationEvent;
                _automationElement = automationElement;
                _handler = automationEventHandler;

                Automation.AddAutomationEventHandler(_automationEvent, _automationElement, TreeScope.Element, _handler);
            }

            public void Dispose()
            {
                Automation.RemoveAutomationEventHandler(_automationEvent, _automationElement, _handler);
            }
        }
    }
}