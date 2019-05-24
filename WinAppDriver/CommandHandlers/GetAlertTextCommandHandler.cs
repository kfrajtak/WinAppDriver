using System.Linq;
using System.Collections.Generic;
using WinAppDriver.XPath.Iterators;
using System.Windows.Automation;
using System;
using WinAppDriver.Extensions;
using WinAppDriver.Utils;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class GetAlertTextCommandHandler : CommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var modalWindow = environment.GetModalWindow();
            if (modalWindow == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoAlertPresent, string.Empty);
            }

            var enumerable = new DescendantIterator(modalWindow, false, environment.GetCancellationToken()).Cast<AutomationElement>()
                .Where(el => el.Current.ControlType == ControlType.Text || el.Current.ControlType == ControlType.Edit)
                .OrderBy(el => el.Current.BoundingRectangle.TopLeft, new PointComparer());

            var alertText = string.Join(Environment.NewLine, enumerable.Select(el => el.GetText()));
            
            return Response.CreateSuccessResponse(alertText);
        }
    }
}