using WinAppDriver.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Threading;

namespace WinAppDriver.XPath.Iterators
{
    public class ChildIterator : IEnumerable<AutomationElement>, IEnumerator<AutomationElement>
    {
        private readonly ControlType controlType;
        private readonly bool _empty;
        private readonly AutomationElement _automationElement;
        private readonly CancellationToken _cancellationToken;
        private readonly TreeWalker _treeWalker;
        private bool _started;

        public ChildIterator(string name, AutomationElement automationElement, CancellationToken cancellationToken)
        {
            _automationElement = automationElement;
            _cancellationToken = cancellationToken;
            // tree walker is used instead of FindAll, FindAll cannot be "interrupted"
            _treeWalker = TreeWalker.ControlViewWalker;
            /* using "plain" ControlViewWalker because the advanced one with conditions is taking too long to complete
             * and sometimes it returns elements that are coming from a different subtree - which can be avoided by comparing parent 
             * of the element with automationElement, but that will reduce speed
             * overall the reason is unknown
             * new TreeWalker(new AndCondition(
            Automation.ControlViewCondition,
            // treewalker was jumping around to another windows
            new PropertyCondition(AutomationElement.ProcessIdProperty, automationElement.Current.ProcessId),
            // return only the elements that are visible
            //new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
            condition));*
             */
            var automationId = automationElement.Current.AutomationId;
            var condition = Condition.TrueCondition;
            if (name != null)
            {
                controlType = name.AsControlType();
                condition = new PropertyCondition(AutomationElement.ControlTypeProperty, controlType);

                if (!controlType.CanBeNestedUnder(automationElement.Current.ControlType))
                {
                    condition = Condition.FalseCondition;
                    _empty = true;
                }
            }
        }

        public AutomationElement Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_cancellationToken.IsCancellationRequested || _empty)
            {
                return false;
            }

            if (!_started)
            {
                _started = true;
                Current = _treeWalker.GetFirstChild(_automationElement);
            }
            else
            {
                Current = _treeWalker.GetNextSibling(Current);
            }

            if (controlType != null)
            {
                while (Current != null && Current.Current.ControlType != controlType)
                {
                    Current = _treeWalker.GetNextSibling(Current);
                }
            }

            return Current != null;
        }

        public void Reset()
        {
            _started = false;
        }

        public IEnumerator<AutomationElement> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
