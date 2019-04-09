using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.XPath.Iterators;
using CodePlex.XPathParser;

namespace WinAppDriver.XPath
{   
    public class AxisElement : IXPathExpression, IEvaluate
    {
        private readonly string _prefix, _name;
        private readonly System.Xml.XPath.XPathNodeType _nodeType;
        private readonly XPathAxis _xpathAxis;

        public AxisElement(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            _xpathAxis = xpathAxis;
            _nodeType = nodeType;
            _prefix = prefix;
            _name = name;
        }

        public string Name => _name;

        public XPathAxis Axis => _xpathAxis;

        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new List<AutomationElement>();
            }

            switch (_xpathAxis)
            {
                case XPathAxis.Child:
                    // TODO window
                    if (Name != null && Name.Equals("window", System.StringComparison.InvariantCultureIgnoreCase) &&
                        automationElement.Current.ControlType == ControlType.Window)
                    {
                        return new List<AutomationElement> { automationElement };
                    }
                    return new ChildIterator(Name, automationElement, cancellationToken);
                case XPathAxis.Descendant:
                    return new DescendantIterator(automationElement, false, cancellationToken);
                case XPathAxis.DescendantOrSelf:
                    return new DescendantIterator(automationElement, true, cancellationToken);
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
                    throw new System.NotSupportedException(_xpathAxis.ToString());
            }
        }

        object IEvaluate.Evaluate(AutomationElement automationElement)
        {
            switch (_xpathAxis)
            {
                case XPathAxis.Attribute:
                    var attributeValue = automationElement.GetAutomationElementPropertyValue(_name);
                    System.Diagnostics.Debug.WriteLine($"{_xpathAxis}: {automationElement.Current.LocalizedControlType}#{automationElement.Current.AutomationId} @{_name} => {attributeValue}");
                    return attributeValue;
                case XPathAxis.Self:
                    return automationElement;
                default:
                    throw new System.NotSupportedException(_xpathAxis.ToString());
            }
        }
    }
}
