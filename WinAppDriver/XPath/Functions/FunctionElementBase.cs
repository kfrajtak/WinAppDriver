using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;

namespace WinAppDriver.XPath.Functions
{
    public class FunctionElementBase : IXPathExpression
    {
        IEnumerable<AutomationElement> IXPathExpression.Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            return collection;
        }
    }
}
