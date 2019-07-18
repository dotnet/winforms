// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class Shell32
    {
        [DllImport(ExternDll.Shell32, EntryPoint = "SHGetPathFromIDListEx", ExactSpelling = true)]
        private static extern bool SHGetPathFromIDListEx(IntPtr pidl, IntPtr pszPath, int cchPath, int flags);

        public static bool SHGetPathFromIDListLongPath(IntPtr pidl, out string path)
        {
            IntPtr pszPath = Marshal.AllocHGlobal((Interop.Kernel32.MAX_PATH + 1) * sizeof(char));
            int length = Interop.Kernel32.MAX_PATH;
            try
            {
                if (!SHGetPathFromIDListLongPath(pidl, ref pszPath, length))
                {
                    path = null;
                    return false;
                }

                path = Marshal.PtrToStringAuto(pszPath);
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(pszPath);
            }
        }

        public unsafe static bool SHGetPathFromIDListLongPath(IntPtr pidl, ref IntPtr pszPath, int length)
        {
            // SHGetPathFromIDListEx is basically a helper to get IShellFolder.DisplayNameOf() with some
            // extra functionally built in if the various flags are set.

            // SHGetPathFromIDListEx copies into the ouput buffer using StringCchCopyW, which truncates
            // when there isn't enough space (with a terminating null) and fails. Long paths can be
            // extracted by simply increasing the buffer size whenever the buffer is full.

            // To get the equivalent functionality we could call SHBindToParent on the PIDL to get IShellFolder
            // and then invoke IShellFolder.DisplayNameOf directly. This would avoid long path contortions as
            // we could directly convert from STRRET, calling CoTaskMemFree manually. (Presuming the type is
            // STRRET_WSTR, of course. Otherwise we can just fall back to StrRetToBufW and give up for > MAX_PATH.
            // Presumption is that we shouldn't be getting back ANSI results, and if we are they are likely
            // some very old component that won't have a > MAX_PATH string.)

            // While we could avoid contortions and avoid intermediate buffers by invoking IShellFolder directly,
            // it isn't without cost as we'd be initializing a COM wrapper (RCW) for IShellFolder. Presumably
            // this is much less overhead then looping and copying to intermediate buffers before creating a string.
            // Additionally, implementing this would allow us to short circuit the one caller (FolderBrowserDialog)
            // who doesn't care about the path, but just wants to know that we have an IShellFolder.
            while (SHGetPathFromIDListEx(pidl, pszPath, length, 0) == false)
            {
                if (length >= Kernel32.MAX_UNICODESTRING_LEN
                    || *(char*)pszPath.ToPointer() == '\0')
                {
                    // Already at the maximum size string, or no data was copied in. Fail.
                    return false;
                }

                // Try giving the API a larger buffer
                length *= 2;
                if (length > Kernel32.MAX_UNICODESTRING_LEN)
                {
                    length = Kernel32.MAX_UNICODESTRING_LEN;
                }

                pszPath = Marshal.ReAllocHGlobal(pszPath, (IntPtr)(length * sizeof(char)));
            }

            return true;
        }
    }
}
