using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using CodePlex.XPathParser;

namespace WinAppDriver.XPath
{
    public interface IXPathExpression
    {
        IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken);        
    }

    public interface IEvaluate
    {
        object Evaluate(AutomationElement element);
    }

    public class ValueElement<T> : IXPathExpression, ICondition, IEvaluate
    {
        private readonly T _value;

        public ValueElement(T value)
        {
            _value = value;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            return collection;
        }

        bool ICondition.Matches(AutomationElement element, int index)
        {
            if (int.TryParse(_value.ToString(), out var i) && i == index)
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, _value);
        }

        object IEvaluate.Evaluate(AutomationElement element)
        {
            return _value;
        }
    }

    public interface ICondition
    {
        bool Matches(AutomationElement element, int index);
    }

    public class PredicateElement : IXPathExpression
    {
        private readonly bool _reversed;
        private readonly IXPathExpression _element;
        private readonly IXPathExpression _condition;

        public PredicateElement(bool reversed, IXPathExpression element, IXPathExpression condition)
        {
            _reversed = reversed;
            _element = element;
            _condition = condition;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement element, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            var input = _element.Find(element, null, cancellationToken);
            if (_reversed)
            {
                input = input.Reverse();
            }

            var condition = _condition as ICondition ?? throw new System.Exception($"PredicateElement condition ({_condition.GetType().Name}) does not implement ICondition.");

            return input.Where((e, index) =>
            {
                var matches = condition.Matches(e, index);
                System.Diagnostics.Debug.WriteLine($"PredicateElement: {e.ToDiagString()} at {index}: match? {matches}");
                return matches;
            });
        }
    }

    public class VariableElement : IXPathExpression
    {
        private readonly string _prefix, _name;

        public VariableElement(string prefix, string name)
        {
            _prefix = prefix;
            _name = name;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            return collection;
        }
    }

    public class OperatorElement : IXPathExpression, ICondition
    {
        private readonly IXPathExpression _left, _right;
        private readonly XPathOperator _op;

        public OperatorElement(XPathOperator op, IXPathExpression left, IXPathExpression right)
        {
            _op = op;
            _left = left;
            _right = right;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            return collection;
        }

        public bool Matches(AutomationElement element, int index)
        {
            if (_op == XPathOperator.Union)
            {
                throw new System.NotImplementedException();
            }

            IEvaluate canGetValue = _left as IEvaluate;
            var left = canGetValue.Evaluate(element);

            switch (_op)
            {
                case XPathOperator.Eq:
                    return _right.Equals(left);
                case XPathOperator.Ge:
                case XPathOperator.Gt:
                case XPathOperator.Le:
                case XPathOperator.Lt:
                case XPathOperator.Ne:
                    break;
                default:
                    throw new System.Exception("Not a relational operator " + _op);
            }

            throw new System.NotImplementedException();
        }
    }    

    public class JoinStepElement : IXPathExpression
    {
        private readonly IXPathExpression _left, _right;

        public JoinStepElement(IXPathExpression left, IXPathExpression right)
        {
            _left = left;
            _right = right;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken)
        {
            //System.Diagnostics.Debug.WriteLine(GetHashCode() + " JoinStep " + collection.Count);
            return _left.Find(root, collection, cancellationToken).SelectMany(e => _right.Find(e, null, cancellationToken));/*
            //System.Diagnostics.Debug.WriteLine(GetHashCode() + " JoinStep >> " + left.Count());
            var right = new List<AutomationElement>();
            foreach(var l in left)
            {
                var r = _right.Find(l, left.ToList());
                if (r.Count() > 0)
                {
                    right.AddRange(r);
                }
            }
            
            System.Diagnostics.Debug.WriteLine(GetHashCode() + " JoinStep >> " + right.Count);
            return right;*/
        }
    }        

    public class AutomationElementTreeWalker
    {
        private readonly IXPathExpression _xPathExpresion;

        public AutomationElementTreeWalker(string xPath)
        {
            _xPathExpresion = new XPathParser<IXPathExpression>().Parse(xPath, new WalkerBuilder());
        }

        public AutomationElementTreeWalker(IXPathExpression xPathExpresion)
        {
            _xPathExpresion = xPathExpresion;
        }

        public IEnumerable<AutomationElement> Find(AutomationElement root, CancellationToken cancellationToken)
        {
            var collection = new List<AutomationElement>();
            return _xPathExpresion.Find(root, collection, cancellationToken);
        }
    }

    public class WalkerBuilder : IXPathBuilder<IXPathExpression>
    {
        public void StartBuild() { }

        public IXPathExpression EndBuild(IXPathExpression result)
        {
            return result;
        }

        public IXPathExpression String(string value)
        {
            return new ValueElement<string>(value);
        }

        public IXPathExpression Number(string value)
        {
            return new ValueElement<string>(value);
        }

        public IXPathExpression Operator(XPathOperator op, IXPathExpression left, IXPathExpression right)
        {
            return new OperatorElement(op, left, right);
        }

        public IXPathExpression Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            return new AxisElement(xpathAxis, nodeType, prefix, name);
        }

        public IXPathExpression JoinStep(IXPathExpression left, IXPathExpression right)
        {
            return new JoinStepElement(left, right);
        }

        public IXPathExpression Predicate(IXPathExpression node, IXPathExpression condition, bool reverseStep)
        {
            return new PredicateElement(reverseStep, node, condition);
        }

        public IXPathExpression Variable(string prefix, string name)
        {
            return new VariableElement(prefix, name);
        }

        public IXPathExpression Function(string prefix, string name, IList<IXPathExpression> args)
        {
            return new FunctionElement(prefix, name, args);
        }
    }
}