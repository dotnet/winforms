// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using Collections.Concurrent;
    using Runtime.InteropServices;
    using System;

    /// <summary>
    /// Internal APiHelper class. Helps identify if API is avaialble at runtime.
    /// </summary>
    internal static class ApiHelper
    {
        private static readonly ConcurrentDictionary<(string libName, string procName), bool> availableApis = new ConcurrentDictionary<(string libName, string procName), bool>();

        /// <summary>
        /// Checks if requested API is available in the give library that exist on the machine
        /// </summary>
        /// <param name="libName"> library name</param>
        /// <param name="procName">function name</param>
        /// <returns>return 'true' if given procName available in the installed libName(dll) </returns>
        public static bool IsApiAvailable(string libName, string procName)
        {
            bool isAvailable = false;

            if (!string.IsNullOrEmpty(libName) && !string.IsNullOrEmpty(procName))
            {
                (string libName, string procName) key = (libName, procName);

                if (availableApis.TryGetValue(key, out isAvailable))
                {
                    return isAvailable;
                }

                //load library from system path.
                IntPtr hmod = CommonUnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable(libName);
                if (hmod != IntPtr.Zero)
                {
                    IntPtr pfnProc = CommonUnsafeNativeMethods.GetProcAddress(new HandleRef(isAvailable, hmod), procName);
                    if (pfnProc != IntPtr.Zero)
                    {
                        isAvailable = true;
                    }
                }

                CommonUnsafeNativeMethods.FreeLibrary(new HandleRef(isAvailable, hmod));
                availableApis[key] = isAvailable;
            }

            return isAvailable;
        }

    }
}
