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
        private readonly Enumerator _enumerator;

        public DescendantIterator(AutomationElement automationElement, bool includeSelf, CancellationToken cancellationToken)
        {
            var treeScope = TreeScope.Children;
            if (includeSelf)
            {
                treeScope |= TreeScope.Element;
            }

            _enumerator = new Enumerator(automationElement, includeSelf, cancellationToken);
        }

        private class Enumerator : IEnumerator<AutomationElement>
        {
            private readonly CancellationToken _cancellationToken;
            private readonly AutomationElement _automationElement;

            private class PointComparer : IComparer<Point>
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
            private readonly bool _originalIncludeSelf;
            private bool? _includeSelf;

            public Enumerator(AutomationElement automationElement, bool includeSelf, CancellationToken cancellationToken)
            {
                _elementQueue = new Queue<AutomationElement>();
                _elementQueue.Enqueue(automationElement);

                _cancellationToken = cancellationToken;
                _automationElement = automationElement;
                _includeSelf = includeSelf;
                _originalIncludeSelf = includeSelf;
            }

            public bool MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested || _elementQueue.Count == 0)
                {
                    return false;
                }

                Current = _elementQueue.Dequeue();

                var children = Current.GetChildren(_cancellationToken)
                    .OrderBy(c => c.Current.BoundingRectangle.TopLeft, new PointComparer())
                    .ToList();

                foreach (AutomationElement automationElement in children)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    _elementQueue.Enqueue(automationElement);
                }

                if (_includeSelf.HasValue)
                {
                    // the root node should not be included
                    if (!_includeSelf.Value)
                    {
                        Current = _elementQueue.Count > 0
                            ? _elementQueue.Dequeue() // dequeue the next node in the queue (if possible)
                            : null;
                    }

                    _includeSelf = null; // never do this again
                    return Current != null; // the next node may have be
                }

                return true;
            }

            public void Reset()
            {
                _includeSelf = _originalIncludeSelf;
                _elementQueue.Clear();
                _elementQueue.Enqueue(_automationElement);
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
