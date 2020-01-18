// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    internal class CommonUnsafeNativeMethods
    {
        // These APIs are available starting Windows 10, version 1607 only.
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetThreadDpiAwarenessContext();

        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool AreDpiAwarenessContextsEqual(IntPtr dpiContextA, IntPtr dpiContextB);

        /// <summary>
        ///  Tries to compare two DPIawareness context values. Return true if they were equal.
        ///  Return false when they are not equal or underlying OS does not support this API.
        /// </summary>
        /// <returns>true/false</returns>
        public static bool TryFindDpiAwarenessContextsEqual(IntPtr dpiContextA, IntPtr dpiContextB)
        {
            if (dpiContextA == DPI_AWARENESS_CONTEXT.UNSPECIFIED && dpiContextB == DPI_AWARENESS_CONTEXT.UNSPECIFIED)
            {
                return true;
            }

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return AreDpiAwarenessContextsEqual(dpiContextA, dpiContextB);
            }

            return false;
        }

        /// <summary>
        ///  Tries to get thread dpi awareness context
        /// </summary>
        /// <returns> returns thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static IntPtr TryGetThreadDpiAwarenessContext()
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return GetThreadDpiAwarenessContext();
            }

            // legacy OS that does not have this API available.
            return DPI_AWARENESS_CONTEXT.UNSPECIFIED;
        }

        /// <summary>
        ///  Tries to set thread dpi awareness context
        /// </summary>
        /// <returns> returns old thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static IntPtr TrySetThreadDpiAwarenessContext(IntPtr dpiContext)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                if (dpiContext == DPI_AWARENESS_CONTEXT.UNSPECIFIED)
                {
                    throw new ArgumentException(nameof(dpiContext), dpiContext.ToString());
                }

                return SetThreadDpiAwarenessContext(dpiContext);
            }

            // legacy OS that does not have this API available.
            return DPI_AWARENESS_CONTEXT.UNSPECIFIED;
        }

        internal static IntPtr GetDpiAwarenessContextForWindow(IntPtr hWnd)
        {
            IntPtr dpiAwarenessContext = DPI_AWARENESS_CONTEXT.UNSPECIFIED;

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                // Works only >= Windows 10/1607
                IntPtr awarenessContext = GetWindowDpiAwarenessContext(hWnd);
                DPI_AWARENESS awareness = GetAwarenessFromDpiAwarenessContext(awarenessContext);
                dpiAwarenessContext = ConvertToDpiAwarenessContext(awareness);
            }

            return dpiAwarenessContext;
        }

        private static IntPtr ConvertToDpiAwarenessContext(DPI_AWARENESS dpiAwareness)
        {
            switch (dpiAwareness)
            {
                case DPI_AWARENESS.UNAWARE:
                    return DPI_AWARENESS_CONTEXT.UNAWARE;
                case DPI_AWARENESS.SYSTEM_AWARE:
                    return DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
                case DPI_AWARENESS.PER_MONITOR_AWARE:
                    return DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2;
                default:
                    return DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
            }
        }
    }
}
