using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WinAppDriver.Extensions
{
    public static class AutomationElementExtensions
    {
        public static bool IsModalWindow(this AutomationElement automationElement)
        {
            return (bool)automationElement.GetCurrentPropertyValue(WindowPattern.IsModalProperty);
        }

        public static bool IsTopMostWindow(this AutomationElement automationElement)
        {
            return (bool)automationElement.GetCurrentPropertyValue(WindowPattern.IsTopmostProperty);
        }

        public static bool IsBlockedByModalWindow(this AutomationElement automationElement)
        {
            var value = (WindowInteractionState)automationElement.GetCurrentPropertyValue(WindowPattern.WindowInteractionStateProperty);
            return value == WindowInteractionState.BlockedByModalWindow;
        }

        public static IEnumerable<AutomationElement> GetChildren(this AutomationElement automationElement, System.Threading.CancellationToken cancellationToken)
        {
            var child = TreeWalker.ControlViewWalker.GetFirstChild(automationElement);
            while (child != null)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                yield return child;
                child = TreeWalker.ControlViewWalker.GetNextSibling(child);
            }
        }
    }
}
