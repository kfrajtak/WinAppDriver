using WinAppDriver.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Linq;
using System.Windows;

namespace WinAppDriver.XPath.Iterators
{
    public class DescendantIterator : IEnumerable<AutomationElement>
    {
        private Enumerator _enumerator;

        public DescendantIterator(AutomationElement automationElement, bool includeSelf, CancellationToken cancellationToken)
        {
            var treeScope = TreeScope.Children;
            if (includeSelf)
            {
                treeScope |= TreeScope.Element;
            }

            var elementQueue = new Queue<AutomationElement>();
            var automationElementCollection = automationElement.FindAll(treeScope, Condition.TrueCondition);
            foreach (AutomationElement childElement in automationElementCollection)
            {
                elementQueue.Enqueue(childElement);

            }

            _enumerator = new Enumerator(elementQueue, cancellationToken);
        }

        public class Enumerator : IEnumerator<AutomationElement>
        {
            private readonly CancellationToken _cancellationToken;

            private class PointComparer : IComparer<System.Windows.Point>
            {
                public int Compare(Point a, Point b)
                {
                    if (a.Y < b.Y)
                    {
                        return -1;
                    }

                    if (a.Y > b.Y)
                    {
                        return 1;
                    }

                    if (a.X == b.X)
                    {
                        return 0;
                    }

                    return a.X < b.X ? -1 : 1;
                }
            }

            public AutomationElement Current { get; private set; }

            object IEnumerator.Current => Current;

            private readonly Queue<AutomationElement> _elementQueue;
            public Enumerator(Queue<AutomationElement> queue, CancellationToken cancellationToken)
            {
                _elementQueue = queue;
                _cancellationToken = cancellationToken;
            }

            public bool MoveNext()
            {
                if (_elementQueue.Count == 0 || _cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                Current = _elementQueue.Dequeue();
                System.Diagnostics.Debug.WriteLine("Current " + Current.ToDiagString());

                /*var condition = Condition.TrueCondition;
                if (!Current.ControlType.CanBeNestedUnder(automationElement.Current.ControlType))
                {
                    condition = Condition.FalseCondition;
                }*/

                var children = Current.FindAll(TreeScope.Children, Condition.TrueCondition).Cast<AutomationElement>()
                    .OrderBy(c => c.Current.BoundingRectangle.TopLeft, new PointComparer())
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Children: {string.Join("\n", children.Select(c => c.ToDiagString()))}");

                foreach (AutomationElement automationElement in children)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    _elementQueue.Enqueue(automationElement);
                }

                return true;
            }

            public void Reset()
            {
                throw new System.NotImplementedException();
            }

            public void Dispose()
            {
                _elementQueue.Clear();
            }
        }

        public IEnumerator<AutomationElement> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }
}
