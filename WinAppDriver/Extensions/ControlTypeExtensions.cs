using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WinAppDriver.Extensions
{
    public static class ControlTypeExtensions
    {
        public static bool CanBeNestedUnder(this ControlType controlType, AutomationElement automationElement)
        {
            var r = CanBeNestedUnder(controlType, automationElement.Current.ControlType);
            if (!r)
            {
                return false;
            }

            if (controlType.ProgrammaticName == ControlType.Window.ProgrammaticName)
            {
                if (automationElement.Current.ControlType == ControlType.Custom && automationElement.Current.Name.StartsWith("Row "))
                {
                    // "ControlType.Custom" "Row 27" 
                    return false;
                }
            }

            return true;
        }

        public static bool CanBeNestedUnder(this ControlType controlType, ControlType ct)
        {
            if (controlType.ProgrammaticName == ControlType.Window.ProgrammaticName)
            {
                return new List<ControlType>
                {
                    ControlType.Separator,
                    ControlType.SplitButton,
                    ControlType.Tab,
                    ControlType.TabItem,
                    ControlType.Table,
                    ControlType.TitleBar,
                    ControlType.Window,
                    ControlType.Custom,
                    ControlType.Document,
                    ControlType.Pane,
                    ControlType.Group,
                    ControlType.Header
                }.Contains(ct);
            }

            return true;
        }
    }

    /*
     * ControlType.Button,
ControlType.Separator,
ControlType.Slider,
ControlType.Spinner,
ControlType.SplitButton,
ControlType.StatusBar,
ControlType.Tab,
ControlType.TabItem,
ControlType.ScrollBar,
ControlType.Table,
ControlType.Thumb,
ControlType.TitleBar,
ControlType.ToolBar,
ControlType.ToolTip,
ControlType.Tree,
ControlType.TreeItem,
ControlType.Window,
ControlType.Text,
ControlType.ProgressBar,
ControlType.RadioButton,
ControlType.MenuItem,
ControlType.Calendar,
ControlType.CheckBox,
ControlType.ComboBox,
ControlType.Custom,
ControlType.DataGrid,
ControlType.DataItem,
ControlType.Document,
ControlType.Pane,
ControlType.Group,
ControlType.Edit,
ControlType.HeaderItem,
ControlType.Hyperlink,
ControlType.Image,
ControlType.List,
ControlType.ListItem,
ControlType.Menu,
ControlType.MenuBar,
ControlType.Header,*/
}
