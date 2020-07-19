// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        // Packing is defined as 1 in CommCtrl.h ("pack(1)").
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe partial struct TASKDIALOGCONFIG
        {
            public uint cbSize;
            /// <summary>
            ///   "incorrectly named, this is the owner window, not a parent."
            /// </summary>
            public IntPtr hwndParent;
            /// <summary>
            ///   "used for MAKEINTRESOURCE() strings"
            /// </summary>
            public IntPtr hInstance;
            public TDF dwFlags;
            public TDCBF dwCommonButtons;
            public char* pszWindowTitle;
            public IconUnion mainIcon;
            public char* pszMainInstruction;
            public char* pszContent;
            public uint cButtons;
            public TASKDIALOG_BUTTON* pButtons;
            public int nDefaultButton;
            public uint cRadioButtons;
            public TASKDIALOG_BUTTON* pRadioButtons;
            public int nDefaultRadioButton;
            public char* pszVerificationText;
            public char* pszExpandedInformation;
            public char* pszExpandedControlText;
            public char* pszCollapsedControlText;
            public IconUnion footerIcon;
            public char* pszFooter;
            public IntPtr pfCallback;
            public IntPtr lpCallbackData;
            /// <summary>
            ///   "width of the Task Dialog's client area in DLU's. If 0, Task Dialog
            ///   will calculate the ideal width."
            /// </summary>
            public uint cxWidth;
        }
    }
}
