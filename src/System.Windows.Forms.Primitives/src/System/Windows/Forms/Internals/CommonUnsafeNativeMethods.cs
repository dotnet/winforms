// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal class CommonUnsafeNativeMethods
    {
        #region PInvoke DpiRelated
        // This section could go to Nativemethods.cs or Safenativemethods.cs but we have separate copies of them in each library (System.winforms, System.Design and System.Drawing).
        // These APIs are available starting Windows 10, version 1607 only.
        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern DpiAwarenessContext GetThreadDpiAwarenessContext();

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern DpiAwarenessContext SetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool AreDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB);

        /// <summary>
        ///  Tries to compare two DPIawareness context values. Return true if they were equal.
        ///  Return false when they are not equal or underlying OS does not support this API.
        /// </summary>
        /// <returns>true/false</returns>
        public static bool TryFindDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB)
        {
            if (dpiContextA == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED && dpiContextB == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
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
        public static DpiAwarenessContext TryGetThreadDpiAwarenessContext()
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return GetThreadDpiAwarenessContext();
            }

            // legacy OS that does not have this API available.
            return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
        }

        /// <summary>
        ///  Tries to set thread dpi awareness context
        /// </summary>
        /// <returns> returns old thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static DpiAwarenessContext TrySetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext)
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
        /*
                // Dpi awareness context values. Matching windows values.
                public static readonly DPI_AWARENESS_CONTEXT DPI_AWARENESS_CONTEXT_UNAWARE = (-1);
                public static readonly DPI_AWARENESS_CONTEXT DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = (-2);
                public static readonly DPI_AWARENESS_CONTEXT DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = (-3);
                public static readonly DPI_AWARENESS_CONTEXT DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = (-4);
                public static readonly DPI_AWARENESS_CONTEXT DPI_AWARENESS_CONTEXT_UNSPECIFIED = (0);*/

        internal static DpiAwarenessContext GetDpiAwarenessContextForWindow(IntPtr hWnd)
        {
            DpiAwarenessContext dpiAwarenessContext = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                // Works only >= Windows 10/1607
                IntPtr awarenessContext = User32.GetWindowDpiAwarenessContext(hWnd);
                User32.DPI_AWARENESS awareness = User32.GetAwarenessFromDpiAwarenessContext(awarenessContext);
                dpiAwarenessContext = ConvertToDpiAwarenessContext(awareness);
            }

            return dpiAwarenessContext;
        }

        #endregion

        #region Private Methods

        private static DpiAwarenessContext ConvertToDpiAwarenessContext(User32.DPI_AWARENESS dpiAwareness)
        {
            switch (dpiAwareness)
            {
                case User32.DPI_AWARENESS.UNAWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE;
                case User32.DPI_AWARENESS.SYSTEM_AWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
                case User32.DPI_AWARENESS.PER_MONITOR_AWARE:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2;
                default:
                    return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
            }
        }

        #endregion
    }
}
