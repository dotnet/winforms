﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [LibraryImport(Libraries.Ole32)]
        private static partial HRESULT CreateILockBytesOnHGlobal(IntPtr hGlobal, BOOL fDeleteOnRelease, out IntPtr pplkbyt);

        public static WinFormsComWrappers.LockBytesWrapper CreateILockBytesOnHGlobal(IntPtr hGlobal, BOOL fDeleteOnRelease)
        {
            CreateILockBytesOnHGlobal(hGlobal, fDeleteOnRelease, out var ptr).ThrowIfFailed();
            return (WinFormsComWrappers.LockBytesWrapper)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ptr, CreateObjectFlags.None);
        }
    }
}