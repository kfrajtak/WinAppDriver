using WinAppDriver.Server;
using System;
using System.Collections.Concurrent;

namespace WinAppDriver.Infrastructure
{
    public class CacheStore
    {
        private static Lazy<ConcurrentDictionary<string, ElementCache>> _cacheStore =
            new Lazy<ConcurrentDictionary<string, ElementCache>>(
                () => new ConcurrentDictionary<string, ElementCache>(), 
                true);

        private static Lazy<ConcurrentDictionary<string, CommandEnvironment>> _commandtore =
            new Lazy<ConcurrentDictionary<string, CommandEnvironment>>(
                () => new ConcurrentDictionary<string, CommandEnvironment>(),
                true);

        public static ConcurrentDictionary<string, ElementCache> Store => _cacheStore.Value;

        public static ConcurrentDictionary<string, CommandEnvironment> CommandStore => _commandtore.Value;
    }
}
