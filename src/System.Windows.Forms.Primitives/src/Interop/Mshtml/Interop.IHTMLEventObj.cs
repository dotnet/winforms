// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050F32D-98B5-11CF-BB82-00AA00BDCE0B")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLEventObj
        {
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetSrcElement();
            bool GetAltKey();
            bool GetCtrlKey();
            bool GetShiftKey();
            void SetReturnValue(object p);
            object GetReturnValue();
            void SetCancelBubble(bool p);
            bool GetCancelBubble();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetFromElement();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElement GetToElement();
            void SetKeyCode([In] int p);
            int GetKeyCode();
            int GetButton();
            string GetEventType();
            string GetQualifier();
            int GetReason();
            int GetX();
            int GetY();
            int GetClientX();
            int GetClientY();
            int GetOffsetX();
            int GetOffsetY();
            int GetScreenX();
            int GetScreenY();
            object GetSrcFilter();
        }
    }
}
