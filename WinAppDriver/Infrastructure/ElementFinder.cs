using CodePlex.XPathParser;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.XPath;

namespace WinAppDriver.Infrastructure
{
    public class ElementFinder
    {
        private Regex _tagNameSelectorRegex = new Regex("^[A-Za-z]+$");
        private Regex _elementIdSelectorRegex = new Regex("^#[A-Za-z0-9]+$");

        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, string mechanism, string criteria, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine($"FindElements {mechanism} '{criteria}'... ({automationElement.ToDiagString()})");

            switch (mechanism)
            {
                case "id":
                    mechanism = "xpath";
                    break;
                case "css selector":
                    if (_elementIdSelectorRegex.IsMatch(criteria))
                    {
                        mechanism = "xpath";
                        criteria = $"//*[@AutomationId='{criteria}']";
                        break;
                    }
                    if (_tagNameSelectorRegex.IsMatch(criteria))
                    {
                        var controlType = criteria.AsControlType();
                        if (controlType == ControlType.Window && automationElement.Current.ControlType == controlType)
                        {
                            return new List<AutomationElement> { automationElement };
                        }

                        mechanism = "xpath";
                        criteria = $"//{criteria}";
                        break;
                    }
                    throw new InvalidSelectorException(mechanism + " " + criteria + ", only tag name and id selectors are allowed");
                case "xpath":
                    break;
                case "tag name":
                    mechanism = "xpath";
                    criteria = $"//{criteria}";
                    break;
                default:
                    throw new InvalidSelectorException(mechanism + " " + criteria);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"FindElements {mechanism} '{criteria}'...");
                var walker = new AutomationElementTreeWalker(new XPathParser<IXPathExpression>().Parse(criteria, new WalkerBuilder()));
                return walker.Find(automationElement, cancellationToken);
            }
            finally
            {
            }
        }
    }
}
