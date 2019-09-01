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
    public interface IElementFinder
    {
        IEnumerable<AutomationElement> Find(AutomationElement automationElement, CancellationToken cancellationToken);
    }

    public class ElementFinder : IElementFinder
    {
        private readonly Regex _tagNameSelectorRegex = new Regex("^[A-Za-z]+$");
        private readonly Regex _elementIdSelectorRegex = new Regex("^#[A-Za-z0-9]+$");
        private readonly string _mechanism;
        private readonly string _criteria;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ElementFinder(string mechanism, string criteria)
        {
            _criteria = criteria;
            _mechanism = mechanism;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement automationElement, CancellationToken cancellationToken)
        {
            var mechanism = _mechanism;
            var criteria = _criteria;

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
                case "accessibility id":
                    mechanism = "xpath";
                    criteria = $"//*[@AutomationId='{criteria}']";
                    break;
                case "class name":
                    mechanism = "xpath";
                    criteria = $"//*[@ClassName='{criteria}']";
                    break;
                case "tag name":
                    mechanism = "xpath";
                    criteria = $"//{criteria}";
                    break;
                case "name":
                    mechanism = "xpath";
                    criteria = $"//*[@Name='{criteria}']";
                    break;
                default:
                    throw new InvalidSelectorException(mechanism + " " + criteria);
            }

            Logger.Debug($"Find elements using {mechanism} and criteria '{criteria}' ...");
            var walker = new AutomationElementTreeWalker(new XPathParser<IXPathExpression>().Parse(criteria, new WalkerBuilder()));
            return walker.Find(automationElement, cancellationToken);
        }
    }
}
