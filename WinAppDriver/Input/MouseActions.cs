using Microsoft.Test.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Server;

namespace WinAppDriver.Input
{
    public class MouseActions
    {
        private readonly JArray _actions;
        private readonly CommandEnvironment _commandEnvironment;
        private int _x, _y;

        public MouseActions(JArray actions, CommandEnvironment commandEnvironment)
        {
            _actions = actions;
            _commandEnvironment = commandEnvironment;
        }

        public void Execute() { }

        public void Execute(out AutomationElement automationElement)
        {
            _x = 0;
            _y = 0;

            automationElement = null;

            foreach (var action in _actions)
            {
                var type = action["type"].Value<string>();
                switch (type)
                {
                    case "pointerMove":
                        Move(action, out automationElement);
                        break;
                    case "pointerDown":
                        Down(action);
                        break;
                    case "pointerUp":
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

        private void Pause(JToken action)
        {
            var duration = action.Value<int>("duration");
            if (duration > 0)
            {
                System.Threading.Thread.Sleep(duration);
            }
        }

        private void Move(JToken action, out AutomationElement element)
        {
            element = null;

            var origin = action["origin"];
            if (origin is JValue value && value.Value<string>() == "pointer") // move relatively to actual pointer position
            {
                // move relatively to inner state
                _x += action.Value<int>("x");
                _y += action.Value<int>("y");
                Mouse.MoveTo(new System.Drawing.Point(_x, _y));
            }
            else
            {
                // move pointer to an element location
                var elementId = origin.First().Last().Value<string>();
                element = _commandEnvironment.Cache.GetElement(elementId);

                if (!element.TryGetClickablePoint(out var pt))
                {
                    var boundingRect = element.Current.BoundingRectangle;
                    pt = new System.Windows.Point(boundingRect.X + (boundingRect.Height / 2), boundingRect.Y + (boundingRect.Width / 2));
                }

                // set inner state
                _x = (int)pt.X;
                _y = (int)pt.Y;

                Mouse.MoveTo(new System.Drawing.Point(_x, _y));
            }

            Pause(action);
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