// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        public unsafe static extern uint GetDlgItemInt(IntPtr hDlg, int nIDDlgItem, BOOL* lpTranslated, BOOL bSigned);
    }
}
