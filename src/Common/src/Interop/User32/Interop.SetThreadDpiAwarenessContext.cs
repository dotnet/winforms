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
        private static extern DpiAwarenessContext SetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext);

        /// <summary>
        /// Tries to set thread dpi awareness context
        /// </summary>
        /// <returns> returns old thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static DpiAwarenessContext SetCurrentThreadDpiAwarenessContext(DpiAwarenessContext dpiContext)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                if (dpiContext == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
                {
                    throw new ArgumentException(nameof(dpiContext), dpiContext.ToString());
                }

                return SetThreadDpiAwarenessContext(dpiContext);
            }

            // legacy OS that does not have this API available.
            return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
        }
    }
}
