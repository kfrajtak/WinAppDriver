using Microsoft.Test.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using WinAppDriver.Server;

namespace WinAppDriver.Input
{
    public class MouseActions
    {
        private readonly JArray _actions;
        private readonly CommandEnvironment _commandEnvironment;

        public MouseActions(JArray actions, CommandEnvironment commandEnvironment)
        {
            _actions = actions;
            _commandEnvironment = commandEnvironment;
        }

        public void Execute()
        {
            foreach (var action in _actions)
            {
                var type = action["type"].Value<string>();
                System.Diagnostics.Trace.WriteLine(type);
                switch (type)
                {
                    case "pointerMove":
                        Move(action);
                        break;
                    case "pointerDown":
                        Down(action);
                        break;
                    case "pointerUp":
                        Up(action);
                        break;
                    default:
                        throw new NotImplementedException("mouse-" + type);
                }
            }
        }

        private void Move(JToken action)
        {
            var elementId = action["origin"].First().Last().Value<string>();
            var element = _commandEnvironment.Cache.GetElement(elementId);

            if (!element.TryGetClickablePoint(out var pt))
            {
                var boundingRect = element.Current.BoundingRectangle;
                pt = new System.Windows.Point(boundingRect.X + boundingRect.Height / 2, boundingRect.Y + boundingRect.Width / 2);
            }

            Mouse.MoveTo(new System.Drawing.Point((int)pt.X, (int)pt.Y));

            System.Threading.Thread.Sleep(action["duration"].Value<int>());
            return;

        }

        private void Down(JToken action)
        {
            var button = action["button"].Value<int>();
            Mouse.Down((MouseButton)button);
        }

        private void Up(JToken action)
        {
            var button = action["button"].Value<int>();
            Mouse.Up((MouseButton)button);
        }
    }
}