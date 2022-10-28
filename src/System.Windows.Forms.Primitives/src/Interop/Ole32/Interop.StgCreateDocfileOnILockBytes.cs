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
        private static extern IStorage.Interface StgCreateDocfileOnILockBytes(IntPtr iLockBytes, STGM grfMode, uint reserved);

        public static IStorage.Interface StgCreateDocfileOnILockBytes(WinFormsComWrappers.LockBytesWrapper iLockBytes, STGM grfMode)
        {
            return StgCreateDocfileOnILockBytes(iLockBytes.Instance, grfMode, 0);
        }
    }
}
