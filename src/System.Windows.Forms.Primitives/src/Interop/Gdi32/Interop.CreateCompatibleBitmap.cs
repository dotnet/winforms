// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        public static IntPtr CreateCompatibleBitmap(HandleRef hdc, int cx, int cy)
        {
            IntPtr result = CreateCompatibleBitmap(hdc.Handle, cx, cy);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
