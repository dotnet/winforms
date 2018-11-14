//------------------------------------------------------------------------------
// <copyright file="ApiHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
        private static ConcurrentDictionary<Tuple<string, string>, bool> availableApis = new ConcurrentDictionary<Tuple<string, string>, bool>();

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
                var key = new Tuple<string, string>(libName, procName);

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
