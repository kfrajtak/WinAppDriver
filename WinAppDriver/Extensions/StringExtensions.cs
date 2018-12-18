using WinAppDriver.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Automation;

namespace WinAppDriver.Extensions
{
    public static class StringExtensions
    {
        private static Dictionary<string, ControlType> _controlTypeCache = new Dictionary<string, ControlType>();
        public static ControlType AsControlType(this string value)
        {
            value = value.ToLowerInvariant();
            if (value == "textbox")
            {
                value = "edit";
            }

            if (value == "label")
            {
                value = "text";
            }

            if (_controlTypeCache.TryGetValue(value, out var ct))
            {
                return ct;
            }

            var fieldInfo = typeof(ControlType).GetField(value,
                        BindingFlags.GetField | BindingFlags.Public |
                        BindingFlags.Static | BindingFlags.Instance |
                        BindingFlags.IgnoreCase);

            if (fieldInfo == null)
            {
                throw new InvalidSelectorException(value + " is not supported control type");
            }

            ct = (ControlType)fieldInfo.GetValue(null);
            _controlTypeCache.Add(value, ct);
            return ct;
        }
    }
}
