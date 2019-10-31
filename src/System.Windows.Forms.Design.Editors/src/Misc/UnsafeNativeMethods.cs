// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Design
{
    internal class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(HandleRef hWnd, ref Point lpPoint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ScreenToClient(HandleRef hWnd, ref Point lpPoint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport(ExternDll.Ole32)]
        public static extern int ReadClassStg(HandleRef pStg, ref Guid pclsid);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);
    }
}
