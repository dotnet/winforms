// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050F485-98B5-11CF-BB82-00AA00BDCE0B")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDocument3
        {
            void ReleaseCapture();
            void Recalc([In] bool fForce);
            object CreateTextNode([In] string text);
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetDocumentElement();
            string GetUniqueID();
            bool AttachEvent([In] string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            void DetachEvent([In] string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            void SetOnrowsdelete([In] object p);
            object GetOnrowsdelete();
            void SetOnrowsinserted([In] object p);
            object GetOnrowsinserted();
            void SetOncellchange([In] object p);
            object GetOncellchange();
            void SetOndatasetchanged([In] object p);
            object GetOndatasetchanged();
            void SetOndataavailable([In] object p);
            object GetOndataavailable();
            void SetOndatasetcomplete([In] object p);
            object GetOndatasetcomplete();
            void SetOnpropertychange([In] object p);
            object GetOnpropertychange();
            void SetDir([In] string p);
            string GetDir();
            void SetOncontextmenu([In] object p);
            object GetOncontextmenu();
            void SetOnstop([In] object p);
            object GetOnstop();
            object CreateDocumentFragment();
            object GetParentDocument();
            void SetEnableDownload([In] bool p);
            bool GetEnableDownload();
            void SetBaseUrl([In] string p);
            string GetBaseUrl();
            [return: MarshalAs(UnmanagedType.IDispatch)] object GetChildNodes();
            void SetInheritStyleSheets([In] bool p);
            bool GetInheritStyleSheets();
            void SetOnbeforeeditfocus([In] object p);
            object GetOnbeforeeditfocus();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElementCollection GetElementsByName([In] string v);
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetElementById([In] string v);
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElementCollection GetElementsByTagName([In] string v);
        }
    }
}
