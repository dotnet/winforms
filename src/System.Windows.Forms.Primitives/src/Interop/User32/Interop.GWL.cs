// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Window long values for <see cref="SetWindowLong(IntPtr, GWL, IntPtr)"/> and
        ///  <see cref="GetWindowLong(IntPtr, GWL)"/>.
        /// </summary>
        public enum GWL : int
        {
            WNDPROC = (-4),
            HWNDPARENT = (-8),
            STYLE = (-16),
            EXSTYLE = (-20),
            ID = (-12),
        }
    }
}
