// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms;

internal static class NativeMethods
{
    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class PRINTDLGEX
    {
        public int lStructSize;

        public IntPtr hwndOwner;
        public IntPtr hDevMode;
        public IntPtr hDevNames;
        public IntPtr hDC;

        public PRINTDLGEX_FLAGS Flags;
        public int Flags2;

        public int ExclusionFlags;

        public int nPageRanges;
        public int nMaxPageRanges;

        public IntPtr pageRanges;

        public int nMinPage;
        public int nMaxPage;
        public int nCopies;

        public IntPtr hInstance;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? lpPrintTemplateName;

        public WndProc? lpCallback;

        public int nPropertyPages;

        public IntPtr lphPropertyPages;

        public int nStartPage;
        public uint dwResultAction;
    }

    public static class ActiveX
    {
        public const int ALIGN_MIN = 0x0;
        public const int ALIGN_NO_CHANGE = 0x0;
        public const int ALIGN_TOP = 0x1;
        public const int ALIGN_BOTTOM = 0x2;
        public const int ALIGN_LEFT = 0x3;
        public const int ALIGN_RIGHT = 0x4;
        public const int ALIGN_MAX = 0x4;
    }
}
