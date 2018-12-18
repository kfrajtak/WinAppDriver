using WinAppDriver.Server;
using WinAppDriver.Server.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class GetAlertTextCommandHandler : CommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
            var windows = environment.Cache.AutomationElement.FindAll(TreeScope.Children, condition);
            var modalWindow = default(AutomationElement);
            foreach (AutomationElement automationElement in windows)
            {
                if (!automationElement.IsTopMostWindow())
                {
                    continue;
                }

                if (automationElement.IsModalWindow())
                {
                    modalWindow = automationElement;
                    break;
                }
            }

            if (modalWindow == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoAlertPresent, string.Empty);
            }

            /*var text = string.Join(Environment.NewLine, 
                modalWindow.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "65535"))
            if (text != null)
            {
                System.Console.WriteLine(text.Current.Name);
                */
            return Response.CreateSuccessResponse("Blabla");
        }
    }
}
