// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Com
{
    internal unsafe partial struct IStream : IVTable<IStream, IStream.Vtbl>
    {
        static void IVTable<IStream, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
        {
            vtable->Read_4 = &Read;
            vtable->Write_5 = &Write;
            vtable->Seek_6 = &Seek;
            vtable->SetSize_7 = &SetSize;
            vtable->CopyTo_8 = &CopyTo;
            vtable->Commit_9 = &Commit;
            vtable->Revert_10 = &Revert;
            vtable->LockRegion_11 = &LockRegion;
            vtable->UnlockRegion_12 = &UnlockRegion;
            vtable->Stat_13 = &Stat;
            vtable->Clone_14 = &Clone;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Read(IStream* @this, void* pv, uint cb, uint* pcbRead)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Read(pv, cb, pcbRead));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Write(IStream* @this, void* pv, uint cb, uint* pcbWritten)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Write(pv, cb, pcbWritten));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Seek(IStream* @this, long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Seek(dlibMove, dwOrigin, plibNewPosition));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT SetSize(IStream* @this, ulong libNewSize)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.SetSize(libNewSize));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT CopyTo(IStream* @this, IStream* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.CopyTo(pstm, cb, pcbRead, pcbWritten));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Commit(IStream* @this, STGC grfCommitFlags)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Commit(grfCommitFlags));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Revert(IStream* @this)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Revert());

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT LockRegion(IStream* @this, ulong libOffset, ulong cb, LOCKTYPE dwLockType)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.LockRegion(libOffset, cb, dwLockType));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT UnlockRegion(IStream* @this, ulong libOffset, ulong cb, uint dwLockType)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.UnlockRegion(libOffset, cb, dwLockType));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Stat(IStream* @this, STATSTG* pstatstg, STATFLAG grfStatFlag)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Stat(pstatstg, grfStatFlag));

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static HRESULT Clone(IStream* @this, IStream** ppstm)
            => ComWrappers.UnwrapAndInvoke<IStream, Interface>(@this, o => o.Clone(ppstm));
    }
}
