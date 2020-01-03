// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern BOOL GetTextMetricsW(IntPtr hdc, ref TEXTMETRICW lptm);

        public static BOOL GetTextMetricsW(HandleRef hdc, ref TEXTMETRICW lptm)
        {
            BOOL result = GetTextMetricsW(hdc.Handle, ref lptm);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
