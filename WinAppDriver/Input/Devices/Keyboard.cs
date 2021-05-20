using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Server;

namespace WinAppDriver.Input.Devices
{
    public class Keyboard
    {
        private JArray _array;
        private List<string> _keysHeld;

        public Keyboard(JArray array)
        {
            _array = array;
            _keysHeld = new List<string>();
        }

        public void Execute(CommandEnvironment commandEnvironment, System.Threading.CancellationToken cancellationToken)
        {
            AutomationElement.FromHandle(commandEnvironment.WindowHandle).SetFocus();

            var actions = GetActions();
            var strategy = new Strategies.SendKeys.OneByOne();
            strategy.Execute(commandEnvironment, new Dictionary<string, object> { { "actions", actions } }, cancellationToken);
        }

        private IEnumerable<JObject> GetActions()
        {
            foreach (var t in _array)
            {
                var keyCode = t.Value<string>();
                var tmp = ProcessKey(keyCode);
                foreach (var tm in tmp)
                {
                    yield return tm;
                }
            }

            foreach (var heldKey in _keysHeld)
            {
                yield return new JObject
                {
                    ["type"] = "keyUp",
                    ["value"] = heldKey
                };
            }

            yield break;
        }

        private JObject[] ProcessKey(string keyCode)
        {
            if (/*!KeyboardActions.TryGetKey(keyCode[0], out var key) || */!KeyboardActions.IsSpecial(keyCode))
            {
                return new[] {
                    new JObject
                    {
                        ["type"] = "keyDown",
                        ["value"] = keyCode
                    },
                    new JObject
                    {
                        ["type"] = "keyUp",
                        ["value"] = keyCode
                    }
                };
            }

            if (_keysHeld.Contains(keyCode))
            {
                _keysHeld.Remove(keyCode);
                return new[] {
                    new JObject
                    {
                        ["type"] = "keyUp",
                        ["value"] = keyCode
                    }
                };
            }

            _keysHeld.Add(keyCode);

            return new[] {
                new JObject
                {
                    ["type"] = "keyDown",
                    ["value"] = keyCode
                }
            };
        }
    }
}
