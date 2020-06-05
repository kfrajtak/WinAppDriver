using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Extensions
{
    public static class COMExceptionExtensions
    {
        public static bool IsTimeout(this COMException comEx)
        {
            // Operation timed out. (Exception from HRESULT: 0x80131505)
            return comEx.Message.StartsWith("Operation timed out") || comEx.Message.Contains("0x80131505");
        }
    }
}
