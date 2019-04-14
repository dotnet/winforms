// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, EntryPoint = "SHGetKnownFolderPath", PreserveSig = true, ExactSpelling = true)]
        private static extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        public static int SHGetFolderPathEx(ref Guid rfid, uint dwFlags, IntPtr hToken, StringBuilder pszPath)
        {
            int result = SHGetKnownFolderPath(ref rfid, dwFlags, hToken, out IntPtr path);
            if (result == HRESULT.S_OK)
            {
                pszPath.Append(Marshal.PtrToStringAuto(path));
                Marshal.FreeCoTaskMem(path);
            }

            return result;
        }
    }
}
