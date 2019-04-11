using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WinAppDriver.Server;

namespace WinAppDriver.Input
{
    public class KeyboardActions
    {
        private readonly JArray _actions;
        private readonly CommandEnvironment _commandEnvironment;

        private static Dictionary<int, Microsoft.Test.Input.Key> _map = new Dictionary<int, Microsoft.Test.Input.Key>
        {
            //{ 57344, Microsoft.Test.Input.Key.Null },
            { 57345, Microsoft.Test.Input.Key.Cancel },
            { 57346, Microsoft.Test.Input.Key.Help },
            { 57347, Microsoft.Test.Input.Key.Back },
            { 57348, Microsoft.Test.Input.Key.Tab },
            { 57349, Microsoft.Test.Input.Key.Clear },
            { 57350, Microsoft.Test.Input.Key.Return },
            { 57351, Microsoft.Test.Input.Key.Enter },
            { 57352, Microsoft.Test.Input.Key.Shift },
            { 57353, Microsoft.Test.Input.Key.LeftCtrl },
            { 57354, Microsoft.Test.Input.Key.LeftAlt },
            { 57355, Microsoft.Test.Input.Key.Pause },
            { 57356, Microsoft.Test.Input.Key.Escape },
            { 57357, Microsoft.Test.Input.Key.Space },
            { 57358, Microsoft.Test.Input.Key.PageUp },
            { 57359, Microsoft.Test.Input.Key.PageDown },
            { 57360, Microsoft.Test.Input.Key.End },
            { 57361, Microsoft.Test.Input.Key.Home },
            { 57362, Microsoft.Test.Input.Key.Left },
            { 57363, Microsoft.Test.Input.Key.Up },
            { 57364, Microsoft.Test.Input.Key.Right },
            { 57365, Microsoft.Test.Input.Key.Down },
            { 57366, Microsoft.Test.Input.Key.Insert },
            { 57367, Microsoft.Test.Input.Key.Delete },
            { 57368, Microsoft.Test.Input.Key.OemSemicolon },
            //{ 57369, Microsoft.Test.Input.Key. },
            { 57370, Microsoft.Test.Input.Key.NumPad0 },
            { 57371, Microsoft.Test.Input.Key.NumPad1 },
            { 57372, Microsoft.Test.Input.Key.NumPad2 },
            { 57373, Microsoft.Test.Input.Key.NumPad3 },
            { 57374, Microsoft.Test.Input.Key.NumPad4 },
            { 57375, Microsoft.Test.Input.Key.NumPad5 },
            { 57376, Microsoft.Test.Input.Key.NumPad6 },
            { 57377, Microsoft.Test.Input.Key.NumPad7 },
            { 57378, Microsoft.Test.Input.Key.NumPad8 },
            { 57379, Microsoft.Test.Input.Key.NumPad9 },
            { 57380, Microsoft.Test.Input.Key.Multiply },
            { 57381, Microsoft.Test.Input.Key.Add },
            { 57382, Microsoft.Test.Input.Key.Separator },
            { 57383, Microsoft.Test.Input.Key.Subtract },
            { 57384, Microsoft.Test.Input.Key.Decimal },
            { 57385, Microsoft.Test.Input.Key.Divide },
            { 57393, Microsoft.Test.Input.Key.F1 },
            { 57394, Microsoft.Test.Input.Key.F2 },
            { 57395, Microsoft.Test.Input.Key.F3 },
            { 57396, Microsoft.Test.Input.Key.F4 },
            { 57397, Microsoft.Test.Input.Key.F5 },
            { 57398, Microsoft.Test.Input.Key.F6 },
            { 57399, Microsoft.Test.Input.Key.F7 },
            { 57400, Microsoft.Test.Input.Key.F8 },
            { 57401, Microsoft.Test.Input.Key.F9 },
            { 57402, Microsoft.Test.Input.Key.F10 },
            { 57403, Microsoft.Test.Input.Key.F11 },
            { 57404, Microsoft.Test.Input.Key.F12 }
        };

        public KeyboardActions(JArray actions, CommandEnvironment commandEnvironment)
        {
            _actions = actions;
            _commandEnvironment = commandEnvironment;
        }

        public void Execute()
        {
            // TODO implement a state diagram to speed up typing - isolate group of keypresses (down & up) and create a string for typing (Type method)
            foreach (var action in _actions)
            {
                var type = action["type"].Value<string>();
                switch (type)
                {
                    case "keyDown":
                        Down(action);
                        break;
                    case "keyUp":
                        Up(action);
                        break;
                    case "pause":
                        Pause(action);
                        break;
                    default:
                        throw new NotImplementedException("mouse-" + type);
                }
            }
        }

        private bool TryGetKey(char keyCode, out Microsoft.Test.Input.Key key)
        {
            // NOTE hack, 0-9 were not sent to controls
            if (keyCode >= '0' && keyCode <= '9')
            {
                key = Microsoft.Test.Input.Key.D0 + (keyCode - '0');
                return true;
            }

            if (_map.TryGetValue(keyCode, out key))
            {
                return true;
            }

            if (Enum.TryParse(keyCode.ToString(), out key))
            {
                return true;
            }

            return false;
        }

        private void Pause(JToken action)
        {
            var duration = action.Value<int>("duration");
            if (duration > 0)
            {
                System.Threading.Thread.Sleep(duration);
            }
        }

        private void Down(JToken action)
        {
            var keyCode = action["value"].Value<string>();
            if (TryGetKey(keyCode[0], out var key))
            {
                Microsoft.Test.Input.Keyboard.Press(key);
                return;
            }

            Microsoft.Test.Input.Keyboard.Type(keyCode);
        }

        private void Up(JToken action)
        {
            var keyCode = action["value"].Value<string>();
            if (TryGetKey(keyCode[0], out var key))
            {
                Microsoft.Test.Input.Keyboard.Release(key);
                return;
            }            
        }
    }
}
