// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("332C4427-26CB-11D0-B483-00C04FD90119")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLWindow2
        {
            [return: MarshalAs(UnmanagedType.IDispatch)] object Item([In] ref object pvarIndex);
            int GetLength();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLFramesCollection2 GetFrames();
            void SetDefaultStatus([In] string p);
            string GetDefaultStatus();
            void SetStatus([In] string p);
            string GetStatus();
            int SetTimeout([In] string expression, [In] int msec, [In] ref object language);
            void ClearTimeout([In] int timerID);
            void Alert([In] string message);
            bool Confirm([In] string message);
            [return: MarshalAs(UnmanagedType.Struct)] object Prompt([In] string message, [In] string defstr);
            object GetImage();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLLocation GetLocation();
            [return: MarshalAs(UnmanagedType.Interface)] IOmHistory GetHistory();
            void Close();
            void SetOpener([In] object p);
            [return: MarshalAs(UnmanagedType.IDispatch)] object GetOpener();
            [return: MarshalAs(UnmanagedType.Interface)] IOmNavigator GetNavigator();
            void SetName([In] string p);
            string GetName();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLWindow2 GetParent();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLWindow2 Open([In] string URL, [In] string name, [In] string features, [In] bool replace);
            object GetSelf();
            object GetTop();
            object GetWindow();
            void Navigate([In] string URL);
            void SetOnfocus([In] object p);
            object GetOnfocus();
            void SetOnblur([In] object p);
            object GetOnblur();
            void SetOnload([In] object p);
            object GetOnload();
            void SetOnbeforeunload(object p);
            object GetOnbeforeunload();
            void SetOnunload([In] object p);
            object GetOnunload();
            void SetOnhelp(object p);
            object GetOnhelp();
            void SetOnerror([In] object p);
            object GetOnerror();
            void SetOnresize([In] object p);
            object GetOnresize();
            void SetOnscroll([In] object p);
            object GetOnscroll();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLDocument2 GetDocument();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLEventObj GetEvent();
            object Get_newEnum();
            object ShowModalDialog([In] string dialog, [In] ref object varArgIn, [In] ref object varOptions);
            void ShowHelp([In] string helpURL, [In] object helpArg, [In] string features);
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLScreen GetScreen();
            object GetOption();
            void Focus();
            bool GetClosed();
            void Blur();
            void Scroll([In] int x, [In] int y);
            object GetClientInformation();
            int SetInterval([In] string expression, [In] int msec, [In] ref object language);
            void ClearInterval([In] int timerID);
            void SetOffscreenBuffering([In] object p);
            object GetOffscreenBuffering();
            [return: MarshalAs(UnmanagedType.Struct)] object ExecScript([In] string code, [In] string language);
            string toString();
            void ScrollBy([In] int x, [In] int y);
            void ScrollTo([In] int x, [In] int y);
            void MoveTo([In] int x, [In] int y);
            void MoveBy([In] int x, [In] int y);
            void ResizeTo([In] int x, [In] int y);
            void ResizeBy([In] int x, [In] int y);
            object GetExternal();
        }
    }
}
