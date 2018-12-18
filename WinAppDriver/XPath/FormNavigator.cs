using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace WinAppDriver.XPath
{
    public class ControlXPathNavigator : XPathNavigator
    {
        private Control _control;
        public ControlXPathNavigator(Control control)
        {
            _control = control;
        }

        public override XmlNameTable NameTable => throw new NotImplementedException();

        public override XPathNodeType NodeType => throw new NotImplementedException();

        public override string LocalName => null;

        public override string Name => null;

        public override string NamespaceURI => string.Empty;

        public override string Prefix => string.Empty;

        public override string BaseURI => string.Empty;

        public override bool IsEmptyElement => _control == null;

        public override string Value => throw new NotImplementedException();

        public override XPathNavigator Clone()
        {
            return new ControlXPathNavigator(_control);
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            throw new NotImplementedException();
        }

        public override bool MoveTo(XPathNavigator other)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstChild()
        {
            if (_control.HasChildren)
            {
                _control = _control.Controls[0];
            }

            return _control.HasChildren;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToId(string id)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNext()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToParent()
        {
            if (_control.Parent == null)
            {
                return false;
            }

            _control = _control.Parent;
            return true;
        }

        public override bool MoveToPrevious()
        {
            throw new NotImplementedException();
        }
    }
}
