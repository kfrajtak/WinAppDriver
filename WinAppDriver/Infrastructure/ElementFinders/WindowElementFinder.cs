using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.XPath;

namespace WinAppDriver.Infrastructure.ElementFinders
{
    public class WindowElementFinder : IElementFinder
    {
        private readonly AutomationElement _rootElement;
        private readonly string _criteria;

        public WindowElementFinder(string criteria, AutomationElement rootElement)
        {
            _criteria = criteria.Replace("//Window", "self::node()");
            _rootElement = rootElement;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, CancellationToken cancellationToken)
        {
            var walker = new AutomationElementTreeWalker(new XPathParser<IXPathExpression>().Parse(_criteria, new WalkerBuilder()));
            return new BreadthFirstSearch()
                .Find(_rootElement, ControlType.Window, cancellationToken)
                .Where(w => w.Current.ControlType == ControlType.Window)
                .SelectMany(window => walker.Find(window, cancellationToken));
        }
    }
}
