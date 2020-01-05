// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("0000000B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IStorage
        {
            IStream CreateStream(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                STGM grfMode,
                uint reserved1,
                uint reserved2);

            IStream OpenStream(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                IntPtr reserved1,
                STGM grfMode,
                uint reserved2);

            IStorage CreateStorage(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                STGM grfMode,
                uint reserved1,
                uint reserved2);

            IStorage OpenStorage(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                IntPtr pstgPriority,
                STGM grfMode,
                IntPtr snbExclude,
                uint reserved);

            void CopyTo(
                uint ciidExclude,
                Guid[] pIIDExclude,
                IntPtr snbExclude,
                IStorage stgDest);

            void MoveElementTo(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                IStorage stgDest,
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName,
                uint grfFlags);

            void Commit(uint grfCommitFlags);

            void Revert();

            void EnumElements(
                uint reserved1,
                IntPtr reserved2,
                uint reserved3,
                out object ppVal);

            void DestroyElement([MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

            void RenameElement(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName,
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName);

            // pctime, patime and pmtime are optional
            void SetElementTimes(
                [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
                FILETIME *pctime,
                FILETIME *patime,
                FILETIME *pmtime);

            void SetClass(ref Guid clsid);

            void SetStateBits(uint grfStateBits, uint grfMask);

            void Stat(out STATSTG pStatStg, STATFLAG grfStatFlag);
        }
    }
}
