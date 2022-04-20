// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, PreserveSig = false, EntryPoint = "CreateILockBytesOnHGlobal")]
        private static extern IntPtr CreateILockBytesOnHGlobalRaw(IntPtr hGlobal, BOOL fDeleteOnRelease);

        public static WinFormsComWrappers.LockBytesComWrapper? CreateILockBytesOnHGlobal(IntPtr hGlobal, BOOL fDeleteOnRelease)
        {
            var ptr = CreateILockBytesOnHGlobalRaw(hGlobal, fDeleteOnRelease);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return (WinFormsComWrappers.LockBytesComWrapper)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ptr, CreateObjectFlags.None);
        }
    }
}
