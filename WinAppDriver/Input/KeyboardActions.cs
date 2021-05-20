using Microsoft.Test.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WinAppDriver.Server;

namespace WinAppDriver.Input
{
    public class KeyboardActions
    {
        private readonly JArray _actions;

        private static Dictionary<int, Key> _map = new Dictionary<int, Key>
        {
            //{ 57344, Key.Null },
            { 57345, Key.Cancel },
            { 57346, Key.Help },
            { 57347, Key.Back },
            { 57348, Key.Tab },
            { 57349, Key.Clear },
            { 57350, Key.Return },
            { 57351, Key.Enter },
            { 57352, Key.Shift },
            { 57353, Key.LeftCtrl },
            { 57354, Key.LeftAlt },
            { 57355, Key.Pause },
            { 57356, Key.Escape },
            { 57357, Key.Space },
            { 57358, Key.PageUp },
            { 57359, Key.PageDown },
            { 57360, Key.End },
            { 57361, Key.Home },
            { 57362, Key.Left },
            { 57363, Key.Up },
            { 57364, Key.Right },
            { 57365, Key.Down },
            { 57366, Key.Insert },
            { 57367, Key.Delete },
            { 57368, Key.OemSemicolon },
            //{ 57369, Key. },
            { 57370, Key.NumPad0 },
            { 57371, Key.NumPad1 },
            { 57372, Key.NumPad2 },
            { 57373, Key.NumPad3 },
            { 57374, Key.NumPad4 },
            { 57375, Key.NumPad5 },
            { 57376, Key.NumPad6 },
            { 57377, Key.NumPad7 },
            { 57378, Key.NumPad8 },
            { 57379, Key.NumPad9 },
            { 57380, Key.Multiply },
            { 57381, Key.Add },
            { 57382, Key.Separator },
            { 57383, Key.Subtract },
            { 57384, Key.Decimal },
            { 57385, Key.Divide },
            { 57393, Key.F1 },
            { 57394, Key.F2 },
            { 57395, Key.F3 },
            { 57396, Key.F4 },
            { 57397, Key.F5 },
            { 57398, Key.F6 },
            { 57399, Key.F7 },
            { 57400, Key.F8 },
            { 57401, Key.F9 },
            { 57402, Key.F10 },
            { 57403, Key.F11 },
            { 57404, Key.F12 }
        };

        internal static bool IsSpecial(string keyCode)
        {
            return TryGetKey(keyCode[0], out var key) && _specialKeys.Contains(key);
        }

        private static readonly IList<Key> _specialKeys = new List<Key> 
        { 
            Key.RightAlt, Key.LeftAlt, 
            Key.Insert, Key.Delete,
            Key.Ctrl, Key.LeftCtrl, Key.RightCtrl,
            Key.Shift, Key.LeftShift, Key.RightShift,
            Key.Left, Key.Right, Key.Down, Key.Up,
            Key.Home, Key.End,
            Key.PageUp, Key.PageDown,
            Key.LWin, Key.RWin
        };

        public KeyboardActions(JArray actions)
        {
            _actions = actions;
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
                        throw new NotImplementedException("key-" + type);
                }
            }
        }

        public static bool TryGetKey(char keyCode, out Key key)
        {
            // 8-bit char
            if (keyCode > 0 && keyCode <= 255)
            {
                key = Key.None;
                return false;
            }

            // NOTE hack, 0-9 were not sent to controls
            if (keyCode >= '0' && keyCode <= '9')
            {
                key = Key.D0 + (keyCode - '0');
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
                Keyboard.Press(key);
                return;
            }

            Keyboard.Type(keyCode);
        }

        private void Up(JToken action)
        {
            var keyCode = action["value"].Value<string>();
            if (TryGetKey(keyCode[0], out var key))
            {
                Keyboard.Release(key);
                return;
            }
        }
    }
}
