// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true)]
        public static extern CoTaskMemSafeHandle SHBrowseForFolderW(ref BROWSEINFO lpbi);

        public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

        public static class BrowseInfoFlags
        {
            public const uint BIF_RETURNONLYFSDIRS = 0x00000001;
            public const uint BIF_DONTGOBELOWDOMAIN = 0x00000002;
            public const uint BIF_RETURNFSANCESTORS = 0x00000008;
            public const uint BIF_EDITBOX = 0x00000010;
            public const uint BIF_NEWDIALOGSTYLE = 0x00000040;
            public const uint BIF_NONEWFOLDERBUTTON = 0x00000200;

            public const uint BIF_BROWSEFORCOMPUTER = 0x00001000;
            public const uint BIF_BROWSEFORPRINTER = 0x00002000;
            public const uint BIF_BROWSEFOREVERYTHING = 0x00004000;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public unsafe struct BROWSEINFO
        {
            public IntPtr hwndOwner;

            public CoTaskMemSafeHandle? pidlRoot;

            public char* pszDisplayName;

            public string? lpszTitle;

            public uint ulFlags;

            public BrowseCallbackProc? lpfn;

            public IntPtr lParam;

            public int iImage;
        }
    }
}
