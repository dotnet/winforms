// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050F1FF-98B5-11CF-BB82-00AA00BDCE0B")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement
        {
            void SetAttribute(string attributeName, object attributeValue, int lFlags);
            object GetAttribute(string attributeName, int lFlags);
            bool RemoveAttribute(string strAttributeName, int lFlags);
            void SetClassName(string p);
            string GetClassName();
            void SetId(string p);
            string GetId();
            string GetTagName();
            IHTMLElement GetParentElement();
            IHTMLStyle GetStyle();
            void SetOnhelp(object p);
            object GetOnhelp();
            void SetOnclick(object p);
            object GetOnclick();
            void SetOndblclick(object p);
            object GetOndblclick();
            void SetOnkeydown(object p);
            object GetOnkeydown();
            void SetOnkeyup(object p);
            object GetOnkeyup();
            void SetOnkeypress(object p);
            object GetOnkeypress();
            void SetOnmouseout(object p);
            object GetOnmouseout();
            void SetOnmouseover(object p);
            object GetOnmouseover();
            void SetOnmousemove(object p);
            object GetOnmousemove();
            void SetOnmousedown(object p);
            object GetOnmousedown();
            void SetOnmouseup(object p);
            object GetOnmouseup();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLDocument2 GetDocument();
            void SetTitle(string p);
            string GetTitle();
            void SetLanguage(string p);
            string GetLanguage();
            void SetOnselectstart(object p);
            object GetOnselectstart();
            void ScrollIntoView(object varargStart);
            bool Contains(IHTMLElement pChild);
            int GetSourceIndex();
            object GetRecordNumber();
            void SetLang(string p);
            string GetLang();
            int GetOffsetLeft();
            int GetOffsetTop();
            int GetOffsetWidth();
            int GetOffsetHeight();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetOffsetParent();
            void SetInnerHTML(string p);
            string GetInnerHTML();
            void SetInnerText(string p);
            string GetInnerText();
            void SetOuterHTML(string p);
            string GetOuterHTML();
            void SetOuterText(string p);
            string GetOuterText();
            void InsertAdjacentHTML(string @where,
                string html);
            void InsertAdjacentText(string @where,
                string text);
            IHTMLElement GetParentTextEdit();
            bool GetIsTextEdit();
            void Click();
            [return: MarshalAs(UnmanagedType.Interface)] object GetFilters();
            void SetOndragstart(object p);
            object GetOndragstart();
            string toString();
            void SetOnbeforeupdate(object p);
            object GetOnbeforeupdate();
            void SetOnafterupdate(object p);
            object GetOnafterupdate();
            void SetOnerrorupdate(object p);
            object GetOnerrorupdate();
            void SetOnrowexit(object p);
            object GetOnrowexit();
            void SetOnrowenter(object p);
            object GetOnrowenter();
            void SetOndatasetchanged(object p);
            object GetOndatasetchanged();
            void SetOndataavailable(object p);
            object GetOndataavailable();
            void SetOndatasetcomplete(object p);
            object GetOndatasetcomplete();
            void SetOnfilterchange(object p);
            object GetOnfilterchange();
            [return: MarshalAs(UnmanagedType.IDispatch)] object GetChildren();
            [return: MarshalAs(UnmanagedType.IDispatch)] object GetAll();
        }
    }
}
