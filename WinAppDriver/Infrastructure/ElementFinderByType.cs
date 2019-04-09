using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Infrastructure
{
    public class BreadthFirstSearch
    {
        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, ControlType controlType, CancellationToken cancellationToken)
        {
            return new Enumerator(automationElement, controlType, cancellationToken);
        }

        public class Enumerator : IEnumerable<AutomationElement>, IEnumerator<AutomationElement>
        {
            private readonly ControlType _controlType;
            private readonly CancellationToken _cancellationToken;

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

            AutomationElement IEnumerator<AutomationElement>.Current => _current;

            object IEnumerator.Current => _current;

            private readonly Queue<AutomationElement> _elementQueue;
            private AutomationElement _current;

            public Enumerator(AutomationElement automationElement, ControlType controlType, CancellationToken cancellationToken)
            {
                _elementQueue = new Queue<AutomationElement>(); ;
                _elementQueue.Enqueue(automationElement);
                _controlType = controlType;
                _cancellationToken = cancellationToken;
            }

            bool IEnumerator.MoveNext()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                /*if (_matching.Any())
                {
                    Current = _matching.Dequeue();
                    return true;
                }*/

                if (_elementQueue.Count == 0)
                {
                    return false;
                }

                _current = _elementQueue.Dequeue();

                AutomationElement childAutomationElement = TreeWalker.ControlViewWalker.GetFirstChild(_current);

                /*var children = _current.FindAll(TreeScope.Children, Condition.TrueCondition).Cast<AutomationElement>()
                    .OrderBy(c => c.Current.BoundingRectangle.TopLeft, new PointComparer())
                    .ToList();*/

                //System.Diagnostics.Debug.WriteLine($"FindElements by type {_controlType.ProgrammaticName}, root {_current.ToDiagString()} ... ({children.Count()})");
                while (childAutomationElement != null)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    if (_controlType.CanBeNestedUnder(childAutomationElement))
                    {
                        _elementQueue.Enqueue(childAutomationElement);
                    }

                    childAutomationElement = TreeWalker.ControlViewWalker.GetNextSibling(childAutomationElement);
                }

                /*
                foreach (AutomationElement automationElement in children)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    if (!_controlType.CanBeNestedUnder(automationElement))
                    {
                        continue;
                    }

                    _elementQueue.Enqueue(automationElement);
                }*/

                return true;
            }

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                _elementQueue.Clear();
            }

            IEnumerator<AutomationElement> IEnumerable<AutomationElement>.GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
        }
    }
}
