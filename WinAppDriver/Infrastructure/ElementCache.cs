using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;

namespace WinAppDriver.Infrastructure
{
    public class ElementCache : IDisposable
    {
        private int _counter = 1;
        private readonly ElementCache _parentCache;

        public ElementCache(IntPtr hwnd, AutomationElement automationElement) : this(automationElement)
        {
            Handle = hwnd;
        }

        public ElementCache(AutomationElement automationElement)
        {
            AutomationElement = automationElement;
            Cache(null);
        }

        public ElementCache(string elementId, AutomationElement automationElement, ElementCache parentCache)
        {
            AutomationElement = automationElement;
            _parentCache = parentCache;
            Cache(elementId);
        }

        private void Cache(string elementId)
        {
            Handle = AutomationElement.NativeElement.CurrentNativeWindowHandle;

            AddToCache(new Tuple<string, AutomationElement>(elementId ?? GetNextElementId(), AutomationElement));
        }

        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Handle of the previous/parent window.
        /// </summary>
        public IntPtr PrevWindowsHandle { get; set; }

        private Dictionary<string, AutomationElement> _cache = new Dictionary<string, AutomationElement>();
        private Dictionary<AutomationElement, string> _cacheReversed = new Dictionary<AutomationElement, string>();
        private IList<IDisposable> _handlers = new List<IDisposable>();

        public AutomationElement AutomationElement { get; }

        public IEnumerable<Tuple<string, AutomationElement>> FindElements(IElementFinder finder, CancellationToken cancellationToken)
        {
            return FindElements(AutomationElement, finder, cancellationToken);
        }

        public IEnumerable<Tuple<string, AutomationElement>> FindElements(AutomationElement automationElement, IElementFinder finder, CancellationToken cancellationToken)
        {
            return finder.Find(automationElement, cancellationToken)
                .Distinct()
                .Select(found =>
                {
                    if (_cacheReversed.TryGetValue(found, out var id))
                    {
                        return new Tuple<string, AutomationElement>(id, found);
                    }

                    return new Tuple<string, AutomationElement>(GetNextElementId(), found);
                });
        }

        private string GetNextElementId() => $"{Handle}_{_counter++}";

        public void AddToCache(params Tuple<string, AutomationElement>[] automationElements)
        {
            foreach (var tuple in automationElements)
            {
                var id = tuple.Item1;
                if (!_cacheReversed.TryGetValue(tuple.Item2, out id))
                {
                    id = tuple.Item1;
                }

                System.Diagnostics.Debug.WriteLine($"{tuple.Item2.Current.AutomationId}: {string.Join(", ", tuple.Item2.GetSupportedPatterns().Select(p => p.ProgrammaticName))}");

                if (!_cache.ContainsKey(id))
                {
                    _cache.Add(id, tuple.Item2);
                }

                if (!_cacheReversed.ContainsKey(tuple.Item2))
                {
                    _cacheReversed.Add(tuple.Item2, id);
                }
            }
        }

        public void AddHandler(object p)
        {
            _handlers.Add((IDisposable)p);
        }

        public T GetHandler<T>()
        {
            return _handlers.OfType<T>().FirstOrDefault();
        }

        internal void Clear()
        {
            _cache.Clear();
        }

        public AutomationElement GetElement(object id)
        {
            if (_cache.TryGetValue(id.ToString(), out var element))
            {
                return element;
            }

            if (_parentCache != null)
            {
                return _parentCache.GetElement(id);
            }

            return null;
        }

        public bool TryGetElementKey(AutomationElement element, out string key)
        {
            foreach (var entry in _cache)
            {
                if (entry.Value == element)
                {
                    key = entry.Key;
                    return true;
                }
            }

            if (_parentCache != null)
            {
                return _parentCache.TryGetElementKey(element, out key);
            }

            key = null;
            return false;
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
