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
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetWindowDpiAwarenessContext(IntPtr hWnd);

#pragma warning disable CA1712 // Do not prefix enum values with type name
        private enum DPI_AWARENESS
        {
            DPI_AWARENESS_INVALID = -1,
            DPI_AWARENESS_UNAWARE = 0,
            DPI_AWARENESS_SYSTEM_AWARE = 1,
            DPI_AWARENESS_PER_MONITOR_AWARE = 2
        }
#pragma warning restore CA1712 // Do not prefix enum values with type name

        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        private static extern DPI_AWARENESS GetAwarenessFromDpiAwarenessContext(IntPtr dpiAwarenessContext);
        
        public static DpiAwarenessContext GetDpiAwarenessContextForWindow(IntPtr hWnd)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                // Works only >= Windows 10/1607
                IntPtr awarenessContext = GetWindowDpiAwarenessContext(hWnd);
                DPI_AWARENESS awareness = GetAwarenessFromDpiAwarenessContext(awarenessContext);
                return ConvertToDpiAwarenessContext(awareness);
            }

            return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
        }

        private static DpiAwarenessContext ConvertToDpiAwarenessContext(DPI_AWARENESS dpiAwareness)
        {
            switch (dpiAwareness)
            {
                case DPI_AWARENESS.DPI_AWARENESS_UNAWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE;

                case DPI_AWARENESS.DPI_AWARENESS_SYSTEM_AWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;

                case DPI_AWARENESS.DPI_AWARENESS_PER_MONITOR_AWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2;

                default:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
            }
        }
    }
}
