// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.System.LibraryLoader;
using static Interop;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public unsafe static IntPtr LoadComctl32(string startupPath)
        {
            // NOTE: we don't look for the loaded module!

            if (!string.IsNullOrWhiteSpace(startupPath))
            {
                string customPath = Path.Join(startupPath, Libraries.Comctl32);
                Debug.Assert(Path.IsPathFullyQualified(customPath));

                fixed (char* lpLibFileName = customPath)
                {
                    if (Path.IsPathFullyQualified(customPath))
                    {
                        // OS will validate the path for us
                        IntPtr result = LoadLibraryEx(lpLibFileName, (HANDLE)0, 0);
                        if (result != IntPtr.Zero)
                        {
                            return result;
                        }
                    }
                }
            }

            // Load the system default
            return LoadLibraryFromSystemPathIfAvailable(Libraries.Comctl32);
        }

        /// <summary>
        /// Loads the requested <paramref name="libraryName"/> from the default system path.
        /// If the module is already loaded, return the handle to it.
        /// </summary>
        /// <param name="libraryName">The assembly name to load.</param>
        /// <returns>A handle to the loaded module, if successful; <see cref="IntPtr.Zero"/> otherwise.</returns>
        public unsafe static nint LoadLibraryFromSystemPathIfAvailable(string libraryName)
        {
            if (GetModuleHandle(Libraries.Kernel32) == 0)
            {
                return 0;
            }

            // LOAD_LIBRARY_SEARCH_SYSTEM32 was introduced in KB2533623. Check for its presence
            // to preserve compat with Windows 7 SP1 without this patch.
            fixed (char* lpLibFileName = libraryName)
            {
                nint result = LoadLibraryEx(lpLibFileName, (HANDLE)0, LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_SEARCH_SYSTEM32);
                if (result != 0)
                {
                    return result;
                }

                // Load without this flag.
                if (Marshal.GetLastWin32Error() != ERROR.INVALID_PARAMETER)
                {
                    return 0;
                }

                return LoadLibraryEx(lpLibFileName, (HANDLE)0, 0);
            }
        }
    }
}

