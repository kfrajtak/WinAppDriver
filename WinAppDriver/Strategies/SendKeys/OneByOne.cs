using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using WinAppDriver.Input;
using WinAppDriver.Server;

namespace WinAppDriver.Strategies.SendKeys
{
    public class OneByOne : ISendKeyStrategy
    {
        public Response Execute(CommandEnvironment commandEnvironment, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("actions", out object o))
            {
                return Response.CreateMissingParametersResponse("actions");
            }

            var actions = (JArray)o;

            foreach (var block in actions)
            {
                var blockActions = (JArray)block["actions"];
                var type = block["type"].Value<string>();

                switch (type)
                {
                    case "pointer":
                        new MouseActions(blockActions, commandEnvironment).Execute();
                        break;
                    case "key":
                        new KeyboardActions(blockActions).Execute();
                        break;
                }
            }

            return Response.CreateSuccessResponse();
        }
    }
}
