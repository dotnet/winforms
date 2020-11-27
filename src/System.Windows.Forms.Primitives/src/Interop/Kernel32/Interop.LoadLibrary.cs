// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        private const int LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr LoadLibraryExW(string lpModuleName, IntPtr hFile, uint dwFlags);

        /// <summary>
        /// Loads comctl32.dll from either the <paramref name="startupPath"/>, if it is supplied;
        /// or from the default system path.
        /// </summary>
        /// <param name="startupPath">The application start up path, where a custom comctl32.dll could be found.</param>
        /// <returns>A </returns>
        public static IntPtr LoadComctl32(string startupPath)
        {
            if (!string.IsNullOrWhiteSpace(startupPath))
            {
                string customPath = Path.Combine(startupPath, Libraries.Comctl32);
                if (File.Exists(customPath))
                {
                    IntPtr result = LoadLibraryExW(customPath, IntPtr.Zero, 0);
                    if (result != IntPtr.Zero)
                    {
                        return result;
                    }
                }
            }

            // Load the system default
            return LoadLibraryFromSystemPathIfAvailable(Libraries.Comctl32);
        }

        public static IntPtr LoadLibraryFromSystemPathIfAvailable(string libraryName)
        {
            IntPtr kernel32 = GetModuleHandleW(Libraries.Kernel32);
            if (kernel32 == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            // LOAD_LIBRARY_SEARCH_SYSTEM32 was introduced in KB2533623. Check for its presence
            // to preserve compat with Windows 7 SP1 without this patch.
            IntPtr result = LoadLibraryExW(libraryName, IntPtr.Zero, LOAD_LIBRARY_SEARCH_SYSTEM32);
            if (result != IntPtr.Zero)
            {
                return result;
            }

            // Load without this flag.
            if (Marshal.GetLastWin32Error() != ERROR.INVALID_PARAMETER)
            {
                return IntPtr.Zero;
            }

            return LoadLibraryExW(libraryName, IntPtr.Zero, 0);
        }
    }
}
