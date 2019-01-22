// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using Runtime.InteropServices;
    using Runtime.Versioning;
    using System;

    internal class DpiUnsafeNativeMethods
    {

        // This section could go to Nativemethods.cs or Safenativemethods.cs but we have separate copies of them in each library (System.winforms, System.Design and System.Drawing).
        // Keeping them here will reduce duplicating code but may have to take care of security warnings (if any).
        // These APIs are available starting Windows 10, version 1607 only.

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern DpiAwarenessContext GetThreadDpiAwarenessContext();

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern DpiAwarenessContext SetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AreDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern DpiAwarenessContext GetWindowDpiAwarenessContext(IntPtr hwnd);

        /// <summary>
        /// Tries to compare two DPIawareness context values. Return true if they were equal. 
        /// Return false when they are not equal or underlying OS does not support this API.
        /// </summary>
        /// <returns>true/false</returns>
        public static bool TryFindDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, DpiAwarenessContext dpiContextB)
        {
            if(dpiContextA == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED && dpiContextB == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
            {
                return true;
            }
            if (ApiHelper.IsApiAvailable(ExternDll.User32, nameof(DpiUnsafeNativeMethods.AreDpiAwarenessContextsEqual)))
            {
                return AreDpiAwarenessContextsEqual(dpiContextA, dpiContextB);
            }

            return false;
        }

        /// <summary>
        /// Tries to get thread dpi awareness context
        /// </summary>
        /// <returns> returns thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static DpiAwarenessContext TryGetThreadDpiAwarenessContext()
        {
            if (ApiHelper.IsApiAvailable(ExternDll.User32, nameof(DpiUnsafeNativeMethods.GetThreadDpiAwarenessContext)))
            {
                return GetThreadDpiAwarenessContext();
            }
            else
            {
                // legacy OS that does not have this API available.
                return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
            }
        }

        /// <summary>
        /// Tries to set thread dpi awareness context
        /// </summary>
        /// <returns> returns old thread dpi awareness context if API is available in this version of OS. otherwise, return IntPtr.Zero.</returns>
        public static DpiAwarenessContext TrySetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext)
        {
            if (ApiHelper.IsApiAvailable(ExternDll.User32, nameof(DpiUnsafeNativeMethods.SetThreadDpiAwarenessContext)))
            {
                if (dpiContext == DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
                {
                    throw new ArgumentException(nameof(dpiContext), dpiContext.ToString());
                }
                return SetThreadDpiAwarenessContext(dpiContext);
            }
            else
            {
                // legacy OS that does not have this API available.
                return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
            }
        }

        /// <summary>
        /// Tries to get window dpi awareness context
        /// </summary>
        /// <returns> returns window dpi awareness context if API is available in this version of OS. otherwise, return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED.</returns>
        public static DpiAwarenessContext TryGetWindowDpiAwarenessContext(IntPtr hwnd)
        {
            if (ApiHelper.IsApiAvailable(ExternDll.User32, nameof(DpiUnsafeNativeMethods.GetWindowDpiAwarenessContext)))
            {
                return GetWindowDpiAwarenessContext(hwnd);
            }
            else
            {
                // legacy OS that does not have this API available.
                return DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;
            }
        }
    }
}