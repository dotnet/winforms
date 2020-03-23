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
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetThreadDpiAwarenessContext", SetLastError = true)]
        private static extern IntPtr SetThreadDpiAwarenessContextInternal(IntPtr dpiContext);

        /// <summary>
        ///  Tries to set thread dpi awareness context
        /// </summary>
        /// <returns>Returns old thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                if (dpiContext == UNSPECIFIED_DPI_AWARENESS_CONTEXT)
                {
                    throw new ArgumentException(nameof(dpiContext), dpiContext.ToString());
                }

                return SetThreadDpiAwarenessContextInternal(dpiContext);
            }

            // legacy OS that does not have this API available.
            return UNSPECIFIED_DPI_AWARENESS_CONTEXT;
        }
    }
}
