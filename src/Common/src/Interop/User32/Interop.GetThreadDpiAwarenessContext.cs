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
        /// <remarks>
        /// These APIs are available starting Windows 10, version 1607 only.
        /// </remarks>
        [DllImport(ExternDll.User32, ExactSpelling = true)]
        private static extern DpiAwarenessContext GetThreadDpiAwarenessContext();

        /// <summary>
        ///  Tries to get thread dpi awareness context
        /// </summary>
        /// <returns> returns thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static DpiAwarenessContext GetCurrentThreadDpiAwarenessContext()
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return GetThreadDpiAwarenessContext();
            }

            // legacy OS that does not have this API available.
            return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
        }
    }
}
