using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using CodePlex.XPathParser;
using System;
using System.Reflection;
using System.ComponentModel;

namespace WinAppDriver.XPath
{
    public interface IXPathExpression
    {
        IEnumerable<AutomationElement> Find(AutomationElement root, IList<AutomationElement> collection, CancellationToken cancellationToken);
    }

    public interface IEvaluate
    {
        object Evaluate(AutomationElement element, Type expectedType);
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

        object IEvaluate.Evaluate(AutomationElement element, Type expectedType)
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

            return input.Where((e, index) => condition.Matches(e, index));
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

    public class OperatorElement : IXPathExpression, ICondition, IEvaluate
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
                throw new NotImplementedException();
            }

            if (_left is IEvaluate canGetLeftValue && _right is IEvaluate canGetRightValue)
            {
                switch (_op)
                {
                    case XPathOperator.Eq:
                    case XPathOperator.Ge:
                    case XPathOperator.Gt:
                    case XPathOperator.Le:
                    case XPathOperator.Lt:
                    case XPathOperator.Ne:
                        {
                            var left = canGetLeftValue.Evaluate(element, typeof(object));
                            var right = canGetRightValue.Evaluate(element, typeof(object));
                            return Compare(_op, left, right);
                        }
                    case XPathOperator.Or:
                    case XPathOperator.And:
                        {
                            var left = (bool)canGetLeftValue.Evaluate(element, typeof(bool));
                            var right = (bool)canGetRightValue.Evaluate(element, typeof(bool));
                            return _op == XPathOperator.And ? left && right : left || right;
                        }
                        //return (bool)left || (bool)right;
                        //case XPathOperator.And:
                        //return (bool)left && (bool)right;
                    default:
                        {
                            var left = canGetLeftValue.Evaluate(element, typeof(int));
                            var right = canGetRightValue.Evaluate(element, typeof(int));
                            var evaluated = Evaluate(element, typeof(int));
                            return index.Equals(evaluated);
                        }
                }
            }

            throw new NotImplementedException(_op.ToString());
        }

        private static bool Compare(XPathOperator xPathOperator, object left, object right)
        {
            if (!(left is IComparable comparable))
            {
                throw new Exception($"Cannot evaluate {xPathOperator} operator, value on the left side does not implement IComparable interface."); 
            }

            var result = comparable.CompareTo(right);

            switch (xPathOperator)
            {
                case XPathOperator.Eq:
                    return result == 0;
                case XPathOperator.Ge:
                    return result > 0 || result == 0;
                case XPathOperator.Gt:
                    return result > 0;
                case XPathOperator.Le:
                    return result < 0 || result == 0;
                case XPathOperator.Lt:
                    return result < 0;
                case XPathOperator.Ne:
                    return result != 0;
            }

            throw new NotImplementedException("Unknown operator " + xPathOperator);
        }

        public object Evaluate(AutomationElement element)
        {
            return Evaluate(element, typeof(object));
        }

        public object Evaluate(AutomationElement element, Type expectedType)
        {
            if (_op == XPathOperator.Union)
            {
                throw new NotImplementedException();
            }

            if (_left is IEvaluate canGetLeftValue)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(expectedType);

                var left = canGetLeftValue.Evaluate(element, expectedType);
                if (left == null)
                {
                    throw new Exception($"Cannot evaluate {_op} operator, value on the left side was evaluated as null reference.");
                }

                var l = (dynamic)converter.ConvertFromString(left.ToString());
                if (_op == XPathOperator.UnaryMinus)
                {
                    return -l;
                }

                if (_right is IEvaluate canGetRightValue)
                {
                    var right = canGetRightValue.Evaluate(element, expectedType);
                    if (right == null && expectedType != typeof(string))
                    {
                        throw new Exception($"Cannot evaluate {_op} operator, value on the right side was evaluated as null reference.");
                    }

                    var methodName = ToOperatorMethodName(_op);
                    if (methodName != null)
                    {
                        var methodInfo = expectedType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (methodInfo != null)
                        {
                            return methodInfo.Invoke(null, new object[]
                            {
                            converter.ConvertFromString(left.ToString()),
                            converter.ConvertFromString(right?.ToString())
                            });
                        }
                    }

                    var r = (dynamic)converter.ConvertFromString(right?.ToString());

                    switch (_op)
                    {
                        case XPathOperator.Divide:
                            return l / r;
                        case XPathOperator.Minus:
                            return l - r;
                        case XPathOperator.Modulo:
                            return l % r;
                        case XPathOperator.Multiply:
                            return l * r;
                        case XPathOperator.Plus:
                            return l + r;
                    }
                }
            }

            throw new NotImplementedException(_op.ToString());
        }

        private string ToOperatorMethodName(XPathOperator xPathOperator)
        {
            switch(xPathOperator)
            {
                case XPathOperator.Divide:
                    return "op_Division";
                case XPathOperator.Minus:
                    return "op_Subtraction";
                case XPathOperator.Modulo:
                    return "op_Modulus";
                case XPathOperator.Multiply:
                    return "op_Multiply";
                case XPathOperator.Plus:
                    return "op_Addition";
                case XPathOperator.UnaryMinus:
                    return "op_UnaryNegation";
            }

            return null;
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
            return Functions.FunctionElementFactory.GetFunctionElement(prefix, name, args);
        }
    }
}