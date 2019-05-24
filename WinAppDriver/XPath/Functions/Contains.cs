using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.XPath.Functions
{
    public class Contains : FunctionElementBase, ICondition, IEvaluate
    {
        private readonly IList<IXPathExpression> _args;

        public Contains(IList<IXPathExpression> args)
        {
            _args = args;
        }

        public object Evaluate(AutomationElement element)
        {
            return Matches(element, -1);
        }

        public bool Matches(AutomationElement element, int index)
        {
            if (_args.Count != 2)
            {
                throw new NotImplementedException($"XPath function 'contains' requires 2 parameters.");
            }

            var haystack = (_args[0] as IEvaluate).Evaluate(element);
            var needle = (_args[1] as IEvaluate).Evaluate(element);
            if (haystack is string && needle is string)
            {
                return haystack.ToString().Contains(needle.ToString());
            }

            throw new NotImplementedException($"Unexpected types of parameters of XPath function 'contains': '{haystack?.GetType().FullName}' and '{needle?.GetType().FullName}'.");
        }
    }
}
