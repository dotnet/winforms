// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

internal partial class Interop
{
    internal static partial class Comdlg32
    {
        [DllImport(Libraries.Comdlg32, ExactSpelling = true)]
        public static extern unsafe BOOL ChooseColorW(ref CHOOSECOLORW lppsd);

        public unsafe struct CHOOSECOLORW
        {
            public uint lStructSize;
            public HWND hwndOwner;
            public HINSTANCE hInstance;
            public int rgbResult;
            public IntPtr lpCustColors;
            public CHOOSECOLOR_FLAGS Flags;
            public IntPtr lCustData;
            public void* lpfnHook;
            public char* lpTemplateName;
        }
    }
}
