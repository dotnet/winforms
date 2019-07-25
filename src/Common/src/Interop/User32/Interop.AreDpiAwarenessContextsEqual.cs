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
        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static extern bool AreDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB);

        /// <summary>
        /// Tries to compare two DPIawareness context values. Return true if they were equal.
        /// Return false when they are not equal or underlying OS does not support this API.
        /// </summary>
        /// <returns>true/false</returns>
        public static bool DpiAwarenessContextsEquals(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return AreDpiAwarenessContextsEqual(dpiContextA, dpiContextB);
            }

            return false;
        }
    }
}
