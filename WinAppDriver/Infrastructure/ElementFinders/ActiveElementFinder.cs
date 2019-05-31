using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;

namespace WinAppDriver.Infrastructure.ElementFinders
{
    public class ActiveElementFinder : IElementFinder
    {
        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, CancellationToken cancellationToken)
        {
            yield return AutomationElement.FocusedElement;
        }
    }
}
