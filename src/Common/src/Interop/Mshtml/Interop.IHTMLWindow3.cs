// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComVisible(true)]
        [Guid("3050f4ae-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLWindow3
        {
            int GetScreenLeft();
            int GetScreenTop();
            bool AttachEvent(string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            void DetachEvent(string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            int SetTimeout([In]ref object expression, int msec, [In] ref object language);
            int SetInterval([In]ref object expression, int msec, [In] ref object language);
            void Print();
            void SetBeforePrint(object o);
            object GetBeforePrint();
            void SetAfterPrint(object o);
            object GetAfterPrint();
            object GetClipboardData();
            object ShowModelessDialog(string url, object varArgIn, object options);
        }
    }
}
