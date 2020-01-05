// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        // Packing is defined as 1 in CommCtrl.h ("pack(1)").
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TASKDIALOGCONFIG
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
            public IntPtr pszWindowTitle;
            public IntPtr mainIconUnion;
            public IntPtr pszMainInstruction;
            public IntPtr pszContent;
            public uint cButtons;
            public IntPtr pButtons;
            public int nDefaultButton;
            public uint cRadioButtons;
            public IntPtr pRadioButtons;
            public int nDefaultRadioButton;
            public IntPtr pszVerificationText;
            public IntPtr pszExpandedInformation;
            public IntPtr pszExpandedControlText;
            public IntPtr pszCollapsedControlText;
            public IntPtr footerIconUnion;
            public IntPtr pszFooter;
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
