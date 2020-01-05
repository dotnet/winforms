// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [DllImport(Libraries.Comdlg32, ExactSpelling = true)]
        public unsafe static extern BOOL ChooseColorW(ref CHOOSECOLORW lppsd);

        public unsafe struct CHOOSECOLORW
        {
            public uint lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public int rgbResult;
            public IntPtr lpCustColors;
            public CC Flags;
            public IntPtr lCustData;
            public User32.WNDPROCINT lpfnHook;
            public char* lpTemplateName;
        }
    }
}
