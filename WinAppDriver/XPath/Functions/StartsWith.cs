using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.XPath.Functions
{
    public class StartsWith : FunctionElementBase, ICondition
    {
        private readonly IList<IXPathExpression> _args;

        public StartsWith(IList<IXPathExpression> args)
        {
            _args = args;
        }

        public bool Matches(AutomationElement element, int index)
        {
            if (_args.Count != 2)
            {
                throw new InvalidSelectorException("Function starts-with expects 2 parameters.");
            }

            var value = (_args[0] as IEvaluate).Evaluate(element, typeof(string));
            var arg = string.Empty;
            if (value is AutomationElement automationElement)
            {
                arg = automationElement.GetText();
            }

            if (value is string)
            {
                arg = value.ToString();
            }

            var v = (_args[1] as IEvaluate).Evaluate(element, typeof(string)).ToString();
            return arg.StartsWith(v);
        }
    }
}
