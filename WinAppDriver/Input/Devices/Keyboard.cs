using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
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

        public void Execute(CommandEnvironment commandEnvironment)
        {
            AutomationElement.FromHandle(commandEnvironment.WindowHandle).SetFocus();

            var actions = GetActions();

            new KeyboardActions(new JArray(actions.ToArray()), commandEnvironment).Execute();
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

        public static bool AllKeysAreNumbersOrChars(JArray actions, out string value)
        {
            value = null;
            var chars = actions
                .Where(a => a["type"].Value<string>() != "pause")
                .Where(a => a["type"].Value<string>() != "keyUp")
                .Select(a => a["value"]?.Value<string>()[0] ?? '\0');
            if (chars.All(a => char.IsLetterOrDigit(a)))
            {
                value = new string(chars.ToArray());
                return true;
            }

            return false;
        }
    }
}
