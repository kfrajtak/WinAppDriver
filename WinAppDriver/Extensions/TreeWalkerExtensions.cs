using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace WinAppDriver.Extensions
{
    public static class TreeWalkerExtensions
    {
        public static AutomationElement GetFirstChild(this TreeWalker treeWalker, AutomationElement element, System.Threading.CancellationToken cancellationToken, int numberOfRetries = 10)
        {
            Exception e = null;
            while (numberOfRetries-- >= 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                try
                {
                    return treeWalker.GetFirstChild(element);
                }
                catch (COMException comEx)
                {
                    e = comEx;
                    // when application is busy, following exception is thrown
                    // System.Runtime.InteropServices.COMException (0x80131505): Operation timed out. (Exception from HRESULT: 0x80131505)
                    // this method will wait try to wait for the application to be accessible via UIA again
                    if (comEx.Message.Contains("0x80131505"))
                    {
                        System.Threading.Thread.Sleep(250);
                        continue;
                    }

                    throw;
                }
            }

            throw e;
        }
    }
}
