// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, PreserveSig = false, ExactSpelling = true)]
        private static extern IntPtr GetHGlobalFromILockBytes(IntPtr pLkbyt);

        public static IntPtr GetHGlobalFromILockBytes(WinFormsComWrappers.LockBytesComWrapper pLkbyt)
        {
            return GetHGlobalFromILockBytes(pLkbyt.Instance);
        }
    }
}
