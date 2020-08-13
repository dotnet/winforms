// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern Gdi32.HBRUSH GetSysColorBrush(COLOR nIndex);

        public static Gdi32.HBRUSH GetSysColorBrush(Color systemColor)
        {
            Debug.Assert(systemColor.IsSystemColor);

            // Extract the COLOR value
            return GetSysColorBrush((COLOR)(ColorTranslator.ToOle(systemColor) & 0xFF));
        }
    }
}
