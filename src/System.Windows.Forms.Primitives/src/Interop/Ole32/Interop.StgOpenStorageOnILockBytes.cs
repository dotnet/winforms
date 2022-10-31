// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com.StructuredStorage;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, PreserveSig = false, ExactSpelling = true)]
        private static extern IStorage.Interface StgOpenStorageOnILockBytes(IntPtr iLockBytes, IStorage.Interface? pStgPriority, STGM grfMode, IntPtr snbExclude, uint reserved);

        public static IStorage.Interface StgOpenStorageOnILockBytes(WinFormsComWrappers.LockBytesWrapper iLockBytes, IStorage.Interface? pStgPriority, STGM grfMode, IntPtr snbExclude)
        {
            return StgOpenStorageOnILockBytes(iLockBytes.Instance, pStgPriority, grfMode, snbExclude, 0);
        }
    }
}
