using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.XPath
{
    public class FunctionElement : IXPathExpression, ICondition, IEvaluate
    {
        private readonly string _prefix, _name;
        private readonly IList<IXPathExpression> _args;

        public FunctionElement(string prefix, string name, IList<IXPathExpression> args)
        {
            _prefix = prefix;
            _name = name;
            _args = args;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            return collection;
        }

        bool ICondition.Matches(AutomationElement element, int index)
        {
            switch (_name)
            {
                case "contains":
                    if (_args.Count != 2)
                    {
                        throw new System.NotImplementedException($"XPath function '{_name}' requires 2 parameters.");
                    }
                    var haystack = (_args[0] as IEvaluate).Evaluate(element);
                    var needle = (_args[1] as IEvaluate).Evaluate(element);
                    if (haystack is string && needle is string)
                    {
                        return haystack.ToString().Contains(needle.ToString());
                    }

                    throw new System.NotImplementedException($"Unexpected types of parameters of XPath function '{_name}': '{haystack?.GetType().FullName}' and '{needle?.GetType().FullName}'.");
            }

            throw new System.NotImplementedException($"XPath function '{_name}' cannot be used for predicate (yet).");
        }

        object IEvaluate.Evaluate(AutomationElement element)
        {
            switch (_name)
            {
                // The normalize-space function strips leading and trailing white-space from a string, 
                // replaces sequences of whitespace characters by a single space
                case "normalize-space":
                    var value = (_args[0] as IEvaluate).Evaluate(element);
                    var arg = string.Empty;
                    if (value is AutomationElement automationElement)
                    {
                        arg = automationElement.GetText();
                    }

                    return System.Text.RegularExpressions.Regex.Replace(arg.ToString().Trim(), @"\s+", " ");
            }

            throw new System.NotImplementedException($"XPath function '{_name}' is not implemented (yet).");
        }
    }
}
