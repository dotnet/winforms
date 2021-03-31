// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal static class UnsafeNativeMethods
    {
        [DllImport(Libraries.User32)]
        public static extern unsafe int GetClassName(HandleRef hwnd, char* lpClassName, int nMaxCount);

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HRESULT PrintDlgEx([In, Out] NativeMethods.PRINTDLGEX lppdex);

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern unsafe int GetModuleFileName(HandleRef hModule, char* buffer, int length);

        public static unsafe string GetModuleFileNameLongPath(HandleRef hModule)
        {
            Span<char> buf = stackalloc char[Kernel32.MAX_UNICODESTRING_LEN];
            string result;
            fixed (char* valueChars = buf)
            {
                // Allocating a bigger buffer each time the function fails by MAX_PATH after starting
                // with MAX_PATH is not possible with stack allocations, as such we must allocate exactly
                // Kernel32.MAX_UNICODESTRING_LEN of memory so that way we never fail in the first place.
                _ = GetModuleFileName(hModule, valueChars, buf.Length);
                if (Marshal.GetLastWin32Error() == ERROR.INSUFFICIENT_BUFFER)
                {
                    // failure getting the module file names. Throw???
                }

                result = buf.SliceAtFirstNull().ToString();
            }

            return result;
        }

        [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(Libraries.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateStdAccessibleObject(HandleRef hWnd, int objID, ref Guid refiid, [In, Out, MarshalAs(UnmanagedType.Interface)] ref object? pAcc);
    }
}
