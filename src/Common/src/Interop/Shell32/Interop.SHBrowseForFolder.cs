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
        [DllImport(Libraries.Shell32, EntryPoint = "SHBrowseForFolderW", ExactSpelling = true)]
        public static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);

        public delegate int BrowseCallbackProc(IntPtr hwnd, int msg,  IntPtr lParam,  IntPtr lpData);

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public class BROWSEINFO 
        {
            [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr hwndOwner;

            [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr pidlRoot;
    
            [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr pszDisplayName;
    
            public string lpszTitle;

            public int ulFlags;

            public BrowseCallbackProc lpfn;

            [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr lParam;

            public int iImage;
        }
    }
}
