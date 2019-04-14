// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(ExternDll.Shell32, EntryPoint = "SHGetPathFromIDListEx", ExactSpelling = true)]
        private static extern bool SHGetPathFromIDListEx(IntPtr pidl, IntPtr pszPath, int cchPath, int flags);

        public static bool SHGetPathFromIDListLongPath(IntPtr pidl, ref IntPtr pszPath)
        {
            int noOfTimes = 1;
            int length = Kernel32.MAX_PATH;
            bool result = false;

            // SHGetPathFromIDListEx returns false in case of insufficient buffer.
            // This method does not distinguish between insufficient memory and an error. Until we get a proper solution,
            // this logic would work. In the worst case scenario, loop exits when length reaches unicode string length.
            while (!(result = SHGetPathFromIDListEx(pidl, pszPath, length, 0)) && length < Kernel32.MAX_UNICODESTRING_LEN)
            {
                string path = Marshal.PtrToStringAuto(pszPath);
                if (path.Length != 0 && path.Length < length)
                {
                    break;
                }

                noOfTimes += 2; //520 chars capacity increase in each iteration.
                length = noOfTimes * length >= Kernel32.MAX_UNICODESTRING_LEN
                    ? Kernel32.MAX_UNICODESTRING_LEN : noOfTimes * length;
                pszPath = Marshal.ReAllocHGlobal(pszPath, (IntPtr)((length + 1) * sizeof(char)));
            }

            return result;
        }
    }
}
