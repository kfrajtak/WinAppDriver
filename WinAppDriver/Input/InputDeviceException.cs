using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Input
{
    public class InputDeviceException : Exception
    {
        public InputDeviceException(string message) : base(message) { }
    }
}
