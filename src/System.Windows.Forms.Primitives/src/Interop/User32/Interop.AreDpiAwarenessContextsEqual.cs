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
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "AreDpiAwarenessContextsEqual", SetLastError = true)]
        private static extern BOOL AreDpiAwarenessContextsEqualInternal(IntPtr dpiContextA, IntPtr dpiContextB);

        /// <summary>
        ///  Tries to compare two DPIawareness context values. Return true if they were equal.
        ///  Return false when they are not equal or underlying OS does not support this API.
        /// </summary>
        /// <returns>true/false</returns>
        public static bool AreDpiAwarenessContextsEqual(IntPtr dpiContextA, IntPtr dpiContextB)
        {
            if (dpiContextA == UNSPECIFIED_DPI_AWARENESS_CONTEXT && dpiContextB == UNSPECIFIED_DPI_AWARENESS_CONTEXT)
            {
                return true;
            }

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return AreDpiAwarenessContextsEqualInternal(dpiContextA, dpiContextB).IsTrue();
            }

            return false;
        }
    }
}
