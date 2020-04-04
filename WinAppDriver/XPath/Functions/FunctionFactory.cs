using System.Collections.Generic;

namespace WinAppDriver.XPath.Functions
{
    public class FunctionElementFactory
    {
        public static IXPathExpression GetFunctionElement(string prefix, string name, IList<IXPathExpression> args)
        {
            switch (name)
            {
                case "contains":
                    return new Contains(args);
                case "starts-with":
                    return new StartsWith(args);
            }

            return new FunctionElement(prefix, name, args);
        }
    }
}
