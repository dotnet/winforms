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
        public static extern TA GetTextAlign(IntPtr hdc);

        public static TA GetTextAlign(IHandle hdc)
        {
            TA result = GetTextAlign(hdc.Handle);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
