using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace WinAppDriver.Extensions
{
    public static class AutomationExtensions
    {
        public static string GetText(this AutomationElement element)
        {
            object patternObj;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                var valuePattern = (ValuePattern)patternObj;
                return valuePattern.Current.Value;
            }
            else if (element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r'); // often there is an extra '\r' hanging off the end.
            }

            if (element.IsTabItem() || element.IsListItem())
            {
                return element.Current.Name;
            }

            var current = element.Current;
            if (current.ControlType == ControlType.Text ||
                current.ControlType == ControlType.Button ||
                current.ControlType == ControlType.RadioButton ||
                current.ControlType == ControlType.Window)
            {
                return current.Name?.ToString().Trim();
            }

            throw new NotSupportedException("GetText for " + element.ToDiagString());
        }

        public static string GetWindowCaption(this AutomationElement element)
        {
            if (element == null)
            {
                return null;
            }

            TreeWalker walker = new TreeWalker(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TitleBar));
            AutomationElement elementNode = walker.GetFirstChild(element);
            if (elementNode != null)
            {
                return elementNode.GetText();
            }

            return element.GetText();
        }

        public static void SetText(this AutomationElement element, string value)
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/ui-automation/add-content-to-a-text-box-using-ui-automation
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // A series of basic checks prior to attempting an insertion.
            //
            // Check #1: Is control enabled?
            // An alternative to testing for static or read-only controls 
            // is to filter using 
            // PropertyCondition(AutomationElement.IsEnabledProperty, true) 
            // and exclude all read-only text controls from the collection.
            if (!element.Current.IsEnabled)
            {
                throw new InvalidOperationException($"The control with an AutomationID of {element.Current.AutomationId} is not enabled.");
            }

            // Check #2: Are there styles that prohibit us 
            //           from sending text to this control?
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new InvalidOperationException($"The control with an AutomationID of {element.Current.AutomationId} is read-only.");
            }

            // Once you have an instance of an AutomationElement,  
            // check if it supports the ValuePattern pattern.

            // Control does not support the ValuePattern pattern 
            // so use keyboard input to insert content.
            //
            // NOTE: Elements that support TextPattern 
            //       do not support ValuePattern and TextPattern
            //       does not support setting the text of 
            //       multi-line edit or document controls.
            //       For this reason, text input must be simulated
            //       using one of the following methods.
            //       
            if (!element.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePattern))
            {
                // Set focus for input functionality and begin.
                element.SetFocus();

                // Pause before sending keyboard input.
                Thread.Sleep(100);

                // Delete existing content in the control and insert new content.
                SendKeys.SendWait("^{HOME}");   // Move to start of control
                SendKeys.SendWait("^+{END}");   // Select everything
                SendKeys.SendWait("{DEL}");     // Delete selection
                SendKeys.SendWait(value);
            }
            // Control supports the ValuePattern pattern so we can 
            // use the SetValue method to insert content.
            else
            {
                // Set focus for input functionality and begin.
                element.SetFocus();

                ((ValuePattern)valuePattern).SetValue(value);
            }
        }

        public static string ToDiagString(this AutomationElement automationElement)
        {
            return $"\"{automationElement.Current.ControlType.ProgrammaticName}\" \"{automationElement.Current.Name}\" {automationElement.Current.AutomationId} ({automationElement.Current.LocalizedControlType}/{automationElement.Current.ClassName}) [{automationElement.Current.BoundingRectangle.TopLeft}]";
        }

        public static object GetAutomationElementPropertyValue(this AutomationElement element, AutomationProperty property)
        {
            return element.GetCurrentPropertyValue(property);
        }

        public static object GetAutomationElementPropertyValue(this AutomationElement element, string propertyName)
        {
            var name = $"AutomationElementIdentifiers.{propertyName}Property";
            var supportedProperies = element.GetSupportedProperties().ToList();
            var prop = supportedProperies.SingleOrDefault(p => p.ProgrammaticName == name);
            if (prop == null)
            {
                var propsInfo = string.Join(", ",
                    supportedProperies.Select(p => p.ProgrammaticName.Replace("AutomationElementIdentifiers.", string.Empty).Replace("Property", string.Empty))
                    .OrderBy(p => p));
                throw new NotSupportedException($"Unknown or unsupported attribute name '{propertyName}' by {element.Current.ControlType.ProgrammaticName}, only these properties are supported: {propsInfo}");
            }

            return element.GetCurrentPropertyValue(prop);
        }

        public static bool IsTabItem(this AutomationElement automationElement)
        {
            return automationElement.Current.ControlType == ControlType.TabItem;
        }

        public static bool IsListItem(this AutomationElement automationElement)
        {
            return automationElement.Current.ControlType == ControlType.ListItem;
        }

        /// <summary>
        /// Retrieves the top-level window that contains the specified UI Automation element.
        /// </summary>
        /// <param name="element">The contained element.</param>
        /// <returns>The containing top-level window element.</returns>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.automation.treewalker.getparent?view=netframework-4.8#System_Windows_Automation_TreeWalker_GetParent_System_Windows_Automation_AutomationElement_System_Windows_Automation_CacheRequest_
        public static AutomationElement GetTopLevelWindow(this AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement node = element;
            
            do
            {
                if (node == AutomationElement.RootElement)
                {
                    return node;
                }

                if (node.Current.ControlType != ControlType.Window)
                {
                    node = walker.GetParent(node);
                    continue;
                }

                return node;
            }
            while (true);
        }
    }
}
