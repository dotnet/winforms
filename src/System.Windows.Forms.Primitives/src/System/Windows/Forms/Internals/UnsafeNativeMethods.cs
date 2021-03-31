// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal static class UnsafeNativeMethods
    {
        [DllImport(Libraries.User32, CharSet = CharSet.Unicode)]
        public static extern unsafe int GetClassName(HandleRef hwnd, char* lpClassName, int nMaxCount);

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT PrintDlgEx([In, Out] NativeMethods.PRINTDLGEX lppdex);

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern unsafe int GetModuleFileName(HandleRef hModule, char* buffer, int length);

        public static unsafe string GetModuleFileNameLongPath(HandleRef hModule)
        {
            Span<char> buffer = new char[Kernel32.MAX_PATH];
            int noOfTimes = 1;
            int length;
            int error = 0;

            // Iterating by allocating chunk of memory each time we find the length is not sufficient.
            // Performance should not be an issue for current MAX_PATH length due to this change.
            while (((length = GetModuleFileName(hModule, buffer)) == buffer.Length)
                && (error = Marshal.GetLastWin32Error()) == ERROR.INSUFFICIENT_BUFFER
                && buffer.Length < Kernel32.MAX_UNICODESTRING_LEN)
            {
                noOfTimes += 2; // Increasing buffer size by 520 in each iteration.
                int capacity = noOfTimes * Kernel32.MAX_PATH < Kernel32.MAX_UNICODESTRING_LEN ? noOfTimes * Kernel32.MAX_PATH : Kernel32.MAX_UNICODESTRING_LEN;
                buffer = new char[capacity];
            }

            if (error == ERROR.INSUFFICIENT_BUFFER)
            {
                // probably should localize this.
                throw new Win32Exception("Error getting the module file names.");
            }

            return buffer.Slice(0, length).SliceAtFirstNull().ToString();
        }

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateStdAccessibleObject(HandleRef hWnd, int objID, ref Guid refiid, [In, Out, MarshalAs(UnmanagedType.Interface)] ref object? pAcc);

        private static unsafe int GetModuleFileName(HandleRef hModule, Span<char> buffer)
        {
            fixed (char* pBuffer = buffer)
            {
                return GetModuleFileName(hModule, pBuffer, buffer.Length);
            }
        }
    }
}
