using System;
using System.Windows.Automation;

namespace WinAppDriver.Infrastructure
{
    public class ElementCacheFactory
    {
        public static ElementCache Get(string sessionId)
        {
            var hwnd = new IntPtr(int.Parse(sessionId));
            return new ElementCache(AutomationElement.FromHandle(hwnd))
            {
                Handle = hwnd
            };
        }
    }
}
