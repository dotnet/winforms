// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL EndDialog(IntPtr hDlg, IntPtr nResult);

        public static BOOL EndDialog(HandleRef hDlg, IntPtr nResult)
        {
            BOOL result = EndDialog(hDlg.Handle, nResult);
            GC.KeepAlive(hDlg.Wrapper);
            return result;
        }
    }
}
