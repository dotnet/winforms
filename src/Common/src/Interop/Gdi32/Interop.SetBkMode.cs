// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern BKMODE SetBkMode(IntPtr hdc, BKMODE mode);

        public static BKMODE SetBkMode(IHandle hdc, BKMODE mode)
        {
            BKMODE result = SetBkMode(hdc.Handle, mode);
            GC.KeepAlive(hdc);
            return result;
        }

        public static BKMODE SetBkMode(HandleRef hdc, BKMODE mode)
        {
            BKMODE result = SetBkMode(hdc.Handle, mode);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
