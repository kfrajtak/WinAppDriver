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

            AutomationElement IEnumerator<AutomationElement>.Current => _current;

            object IEnumerator.Current => _current;

            private readonly Queue<AutomationElement> _elementQueue;
            private AutomationElement _current;

            public Enumerator(AutomationElement automationElement, ControlType controlType, CancellationToken cancellationToken)
            {
                _elementQueue = new Queue<AutomationElement>();
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

                if (_elementQueue.Count == 0)
                {
                    return false;
                }

                _current = _elementQueue.Dequeue();

                AutomationElement childAutomationElement = TreeWalker.ControlViewWalker.GetFirstChild(_current, _cancellationToken);

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

                    // sometimes the app can stop responding when performing long operation thanks to poor designed 
                    // try to wait for the response in a loop 
                    while (true)
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return false;
                        }

                        try
                        {
                            childAutomationElement = TreeWalker.ControlViewWalker.GetNextSibling(childAutomationElement);
                            break;
                        }
                        catch (System.Runtime.InteropServices.COMException comEx) when (comEx.IsTimeout())
                        {
                            // retry after a short sleep
                            Thread.Sleep(TimeSpan.FromMilliseconds(250));
                            // childAutomationElement = TreeWalker.ControlViewWalker.GetNextSibling(childAutomationElement);
                        }
                    }
                }

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
