using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppDriver.Exceptions
{
    public class InvalidParameterValueException : Exception
    {
        public InvalidParameterValueException(string name, object value, Type type, Exception innerException)
            : base($"InvalidParameterValueException {name} {value} {type}", innerException)
        {

        }
    }
}
