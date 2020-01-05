// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("0000000A-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ILockBytes
        {
            // pcbRead is optional so it must be a pointer
            void ReadAt(ulong ulOffset, out IntPtr pv, uint cb, uint* pcbRead);

            // pcbWritten is optional so it must be a pointer
            void WriteAt(ulong ulOffset, IntPtr pv, uint cb, uint* pcbWritten);

            void Flush();

            void SetSize(ulong cb);

            void LockRegion(ulong libOffset, ulong cb, uint dwLockType);

            void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);

            void Stat(out STATSTG pstatstg, STATFLAG grfStatFlag);
        }
    }
}
