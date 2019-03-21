using WinAppDriver.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using System.Threading;

namespace WinAppDriver.XPath.Iterators
{
    public class ChildIterator : IEnumerable<AutomationElement>
    {
        private IList<AutomationElement> _children;

        public ChildIterator(string name, AutomationElement automationElement, CancellationToken cancellationToken)
        {
            var treeScope = TreeScope.Children;
            var condition = Condition.TrueCondition;
            if (name != null)
            {
                var controlType = name.AsControlType();
                condition = new PropertyCondition(AutomationElement.ControlTypeProperty, controlType);
            }

            _children = automationElement.FindAll(treeScope, condition).Cast<AutomationElement>().ToList();

            System.Diagnostics.Debug.WriteLine($"Child: {automationElement.Current.LocalizedControlType}#{automationElement.Current.AutomationId} {name ?? "any"} => {_children.Count}");
        }

        public IEnumerator<AutomationElement> GetEnumerator()
        {
            return _children.Cast<AutomationElement>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}
