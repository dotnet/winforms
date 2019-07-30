// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComVisible(true)]
        [Guid("3050f5da-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDOMNode
        {
            long GetNodeType();
            IHTMLDOMNode GetParentNode();
            bool HasChildNodes();
            object GetChildNodes();
            object GetAttributes();
            IHTMLDOMNode InsertBefore(IHTMLDOMNode newChild, object refChild);
            IHTMLDOMNode RemoveChild(IHTMLDOMNode oldChild);
            IHTMLDOMNode ReplaceChild(IHTMLDOMNode newChild, IHTMLDOMNode oldChild);
            IHTMLDOMNode CloneNode(bool fDeep);
            IHTMLDOMNode RemoveNode(bool fDeep);
            IHTMLDOMNode SwapNode(IHTMLDOMNode otherNode);
            IHTMLDOMNode ReplaceNode(IHTMLDOMNode replacement);
            IHTMLDOMNode AppendChild(IHTMLDOMNode newChild);
            string NodeName();
            void SetNodeValue(object v);
            object GetNodeValue();
            IHTMLDOMNode FirstChild();
            IHTMLDOMNode LastChild();
            IHTMLDOMNode PreviousSibling();
            IHTMLDOMNode NextSibling();
        }
    }
}
