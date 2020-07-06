// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Hhctl
    {
        [DllImport(Libraries.Hhctrl, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int HtmlHelpW(IntPtr hwndCaller, string pszFile, HH uCommand, IntPtr dwData);

        public static int HtmlHelpW(HandleRef hwndCaller, string pszFile, HH uCommand, IntPtr dwData)
        {
            int result = HtmlHelpW(hwndCaller.Handle, pszFile, uCommand, dwData);
            GC.KeepAlive(hwndCaller.Wrapper);
            return result;
        }

        public static unsafe int HtmlHelpW(HandleRef hwndCaller, string pszFile, HH uCommand, string data)
        {
            fixed (char* dwData = data)
            {
                return HtmlHelpW(hwndCaller, pszFile, uCommand, (IntPtr)(void*)dwData);
            }
        }

        public static unsafe int HtmlHelpW<T>(HandleRef hwndCaller, string pszFile, HH uCommand, ref T data) where T : unmanaged
        {
            fixed (void* dwData = &data)
            {
                return HtmlHelpW(hwndCaller, pszFile, uCommand, (IntPtr)dwData);
            }
        }
    }
}
