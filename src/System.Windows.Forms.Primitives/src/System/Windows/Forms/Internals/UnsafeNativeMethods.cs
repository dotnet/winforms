// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Text;
using static Interop;

namespace System.Windows.Forms
{
    internal static class UnsafeNativeMethods
    {
        [DllImport(Libraries.User32)]
#pragma warning disable CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
        public static extern int GetClassName(HandleRef hwnd, StringBuilder lpClassName, int nMaxCount);
#pragma warning restore CA1838 // Avoid 'StringBuilder' parameters for P/Invokes

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT PrintDlgEx([In, Out] NativeMethods.PRINTDLGEX lppdex);

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
#pragma warning disable CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
        public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
#pragma warning restore CA1838 // Avoid 'StringBuilder' parameters for P/Invokes

        public static StringBuilder GetModuleFileNameLongPath(HandleRef hModule)
        {
            StringBuilder buffer = new StringBuilder(Kernel32.MAX_PATH);
            int noOfTimes = 1;
            int length = 0;
            // Iterating by allocating chunk of memory each time we find the length is not sufficient.
            // Performance should not be an issue for current MAX_PATH length due to this change.
            while (((length = GetModuleFileName(hModule, buffer, buffer.Capacity)) == buffer.Capacity)
                && Marshal.GetLastWin32Error() == ERROR.INSUFFICIENT_BUFFER
                && buffer.Capacity < Kernel32.MAX_UNICODESTRING_LEN)
            {
                noOfTimes += 2; // Increasing buffer size by 520 in each iteration.
                int capacity = noOfTimes * Kernel32.MAX_PATH < Kernel32.MAX_UNICODESTRING_LEN ? noOfTimes * Kernel32.MAX_PATH : Kernel32.MAX_UNICODESTRING_LEN;
                buffer.EnsureCapacity(capacity);
            }
            buffer.Length = length;
            return buffer;
        }

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Auto)]
#pragma warning disable CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
        public static extern void GetTempFileName(string tempDirName, string prefixName, int unique, StringBuilder sb);
#pragma warning restore CA1838 // Avoid 'StringBuilder' parameters for P/Invokes

        [DllImport(Libraries.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateStdAccessibleObject(HandleRef hWnd, int objID, ref Guid refiid, [In, Out, MarshalAs(UnmanagedType.Interface)] ref object? pAcc);
    }
}
