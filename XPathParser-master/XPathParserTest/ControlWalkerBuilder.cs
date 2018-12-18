using System.Collections.Generic;
using System.Xml.Linq;
using CodePlex.XPathParser;

namespace XPathParserTest
{
    public interface IControlWalkerElement
    {

    }

    public class ValueElement<T> : IControlWalkerElement
    {
        private readonly T _value;

        public ValueElement(T value)
        {
            _value = value;
        }
    }

    public class PredicateElement : IControlWalkerElement
    {
        private readonly bool _reversed;
        private readonly IControlWalkerElement _element;
        private readonly IControlWalkerElement _condition;

        public PredicateElement(bool reversed, IControlWalkerElement element, IControlWalkerElement condition)
        {
            _reversed = reversed;
            _element = element;
            _condition = condition;
        }
    }

    public class VariableElement : IControlWalkerElement
    {
        private readonly string _prefix, _name;

        public VariableElement(string prefix, string name)
        {
            _prefix = prefix;
            _name = name;
        }
    }

    public class OperatorElement : IControlWalkerElement
    {
        private readonly IControlWalkerElement _left, _right;
        private readonly XPathOperator _op;

        public OperatorElement(XPathOperator op, IControlWalkerElement left, IControlWalkerElement right)
        {
            _op = op;
            _left = left;
            _right = right;
        }
    }

    public class AxisElement : IControlWalkerElement
    {
        private readonly string _prefix, _name;
        private readonly System.Xml.XPath.XPathNodeType _nodeType;
        private readonly XPathAxis _xpathAxis;

        public AxisElement(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            _xpathAxis = xpathAxis;
            _nodeType = nodeType;
            _prefix = prefix;
            _name = name;
        }
    }

    public class JoinStepElement : IControlWalkerElement
    {
        private readonly IControlWalkerElement _left, _right;

        public JoinStepElement(IControlWalkerElement left, IControlWalkerElement right)
        {
            _left = left;
            _right = right;
        }
    }

    public class FunctionElement : IControlWalkerElement
    {
        private readonly string _prefix, _name;
        private readonly IList<IControlWalkerElement> _args;

        public FunctionElement(string prefix, string name, IList<IControlWalkerElement> args)
        {
            _prefix = prefix;
            _name = name;
            _args = args;
        }
    }

    class ControlWalkerBuilder : IXPathBuilder<IControlWalkerElement>
    {
        public void StartBuild() { }

        public IControlWalkerElement EndBuild(IControlWalkerElement result)
        {
            return result;
        }

        public IControlWalkerElement String(string value)
        {
            return new ValueElement<string>(value);
        }

        public IControlWalkerElement Number(string value)
        {
            return new ValueElement<string>(value);
        }

        public IControlWalkerElement Operator(XPathOperator op, IControlWalkerElement left, IControlWalkerElement right)
        {
            return new OperatorElement(op, left, right);
        }

        public IControlWalkerElement Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            return new AxisElement(xpathAxis, nodeType, prefix, name);
        }

        public IControlWalkerElement JoinStep(IControlWalkerElement left, IControlWalkerElement right)
        {
            return new JoinStepElement(left, right);
        }

        public IControlWalkerElement Predicate(IControlWalkerElement node, IControlWalkerElement condition, bool reverseStep)
        {
            return new PredicateElement(reverseStep, node, condition);
        }

        public IControlWalkerElement Variable(string prefix, string name)
        {
            return new VariableElement(prefix, name);
        }

        public IControlWalkerElement Function(string prefix, string name, IList<IControlWalkerElement> args)
        {
            return new FunctionElement(prefix, name, args);
        }
    }
}