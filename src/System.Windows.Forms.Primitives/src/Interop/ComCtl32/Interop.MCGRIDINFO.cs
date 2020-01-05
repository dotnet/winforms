// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using static System.Windows.Forms.NativeMethods;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// MonthCalendar grid info structure.
        /// Copied form CommCtrl.h
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public unsafe struct MCGRIDINFO
        {
            public uint cbSize;
            public MCGIP dwPart;
            public MCGIF dwFlags;
            public int iCalendar;
            public int iRow;
            public int iCol;
            public bool bSelected;
            public Kernel32.SYSTEMTIME stStart;
            public Kernel32.SYSTEMTIME stEnd;
            public RECT rc;
            public string pszName;
            public uint cchName;
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, [In, Out] ref MCGRIDINFO gridInfo);
    }
}
