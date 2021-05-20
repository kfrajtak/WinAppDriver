using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using WinAppDriver.Server;

namespace WinAppDriver.Strategies.SendKeys
{
    public enum SendKeyStrategyType
    {
        OneByOne,
        Grouped,
        SetValue
    }

    public interface ISendKeyStrategy
    {
        Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken);
    }
}
