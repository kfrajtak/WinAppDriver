using System;
using System.Collections.Generic;
using WinAppDriver.Exceptions;

namespace WinAppDriver.Extensions
{
    public static class DictionaryExtensions
    {
        public static T GetParameterValue<T>(this Dictionary<string, object> parameters, string parameterName)
        {
            if (!parameters.TryGetValue(parameterName, out var value))
            {
                throw new MissingParameterException(parameterName);
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                throw new InvalidParameterValueException(parameterName, value, typeof(T), ex);
            }
        }

        public static bool TryGetParameterValue<T>(this Dictionary<string, object> parameters, string parameterName, out T parameterValue)
        {
            parameterValue = default(T);

            if (!parameters.TryGetValue(parameterName, out var value))
            {
                return false;
            }

            try
            {
                parameterValue = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
