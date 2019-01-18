using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Exceptions
{
    public class MissingParameterException : Exception
    {
        public MissingParameterException(string parameterName)
        {

        }
    }
}
