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
    }
}
