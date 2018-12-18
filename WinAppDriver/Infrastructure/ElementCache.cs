using WinAppDriver.Behaviors;
using WinAppDriver.Exceptions;
using WinAppDriver.Extensions;
using WinAppDriver.Server;
using WinAppDriver.XPath;
using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace WinAppDriver.Infrastructure
{
    public class ElementCache : IDisposable
    {
        private readonly AutomationElement _automationElement;        

        public ElementCache(AutomationElement automationElement)
        {
            Automation.RemoveAllEventHandlers();

            _automationElement = automationElement;

            AddToCache(new Tuple<string, AutomationElement>((_counter++).ToString(), _automationElement));            
        }

        public IntPtr Handle { get; set; }

        private Dictionary<string, AutomationElement> _cache = new Dictionary<string, AutomationElement>();
        private Dictionary<AutomationElement, string> _cacheReversed = new Dictionary<AutomationElement, string>();
        private IList<IDisposable> _handlers = new List<IDisposable>();

        public AutomationElement AutomationElement => _automationElement;

        public IEnumerable<Tuple<string, AutomationElement>> FindElements(string mechanism, string criteria, CancellationToken cancellationToken)
        {
            return FindElements(_automationElement, mechanism, criteria, cancellationToken);
        }

        public IEnumerable<Tuple<string, AutomationElement>> FindElements(AutomationElement automationElement, string mechanism, string criteria, CancellationToken cancellationToken)
        {
            return new ElementFinder().Find(automationElement, mechanism, criteria, cancellationToken)
                .Distinct()
                .Select(found =>
                {
                    if (_cacheReversed.TryGetValue(found, out var id))
                    {
                        return new Tuple<string, AutomationElement>(id, found);
                    }

                    return new Tuple<string, AutomationElement>((_counter++).ToString(), found);
                });
        }

        private int _counter = 1;

        public void AddToCache(params Tuple<string, AutomationElement>[] automationElements)
        {
            foreach (var tuple in automationElements)
            {
                var id = tuple.Item1;
                if (!_cacheReversed.TryGetValue(tuple.Item2, out id))
                {
                    id = tuple.Item1;
                }

                if (!_cache.ContainsKey(id))
                {
                    _cache.Add(id, tuple.Item2);
                }

                if (!_cacheReversed.ContainsKey(tuple.Item2))
                {
                    _cacheReversed.Add(tuple.Item2, id);
                }

                if (tuple.Item2.Current.ControlType == ControlType.Window)
                {
                    //_handlers.Add(UnexpectedAlertBehavior.CreateHandler(tuple.Item2));
                }
            }
        }

        internal void AddHandler(object p)
        {
            _handlers.Add((IDisposable)p);
        }

        internal void Clear()
        {
            _cache.Clear();
        }

        public AutomationElement GetElement(object id)
        {
            return _cache[id.ToString()];
        }

        public void Dispose()
        {
            foreach (var item in _handlers)
            {
                item.Dispose();
            }

            _handlers.Clear();
            _cache.Clear();
            _cacheReversed.Clear();
        }
    }
}
