using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.XPath.Iterators;
using CodePlex.XPathParser;
using WinAppDriver.Infrastructure;
using System.Linq;

namespace WinAppDriver.XPath
{   
    public class AxisElement : IXPathExpression, IEvaluate
    {
        private readonly string _prefix;
        private readonly System.Xml.XPath.XPathNodeType _nodeType;

        public AxisElement(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            Axis = xpathAxis;
            _nodeType = nodeType;
            _prefix = prefix;
            Name = name;
        }

        public string Name { get; }

        public XPathAxis Axis { get; }

        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new List<AutomationElement>();
            }

            var isWindow = Name?.Equals("window", System.StringComparison.InvariantCultureIgnoreCase) == true;

            switch (Axis)
            {
                case XPathAxis.Child:
                    // TODO window
                    if (isWindow && automationElement.Current.ControlType == ControlType.Window)
                    {
                        return new List<AutomationElement> { automationElement };
                    }
                    return new ChildIterator(Name, automationElement, cancellationToken);
                case XPathAxis.Descendant:
                case XPathAxis.DescendantOrSelf:
                    return new DescendantIterator(automationElement, Axis == XPathAxis.DescendantOrSelf, cancellationToken);
                case XPathAxis.Root:
                    return new List<AutomationElement> { automationElement };
                case XPathAxis.Parent:
                    if (automationElement.CachedParent != null)
                    {
                        return new List<AutomationElement> { automationElement.CachedParent };
                    }
                    return new List<AutomationElement>();
                case XPathAxis.Self:
                    return new List<AutomationElement> { automationElement };
                default:
                    throw new System.NotSupportedException(Axis.ToString());
            }
        }

        object IEvaluate.Evaluate(AutomationElement automationElement, System.Type expectedType)
        {
            switch (Axis)
            {
                case XPathAxis.Attribute:
                    return automationElement.GetAutomationElementPropertyValue(Name);
                case XPathAxis.Self:
                    return automationElement;
                default:
                    throw new System.NotSupportedException(Axis.ToString());
            }
        }
    }
}
