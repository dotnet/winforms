// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IStreamVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IStream.Vtbl* vtblRaw = (IStream.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IStreamVtbl), sizeof(IStream.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<IStream*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<IStream*, uint>)fpRelease;
                vtblRaw->Read_4 = &Read;
                vtblRaw->Write_5 = &Write;
                vtblRaw->Seek_6 = &Seek;
                vtblRaw->SetSize_7 = &SetSize;
                vtblRaw->CopyTo_8 = &CopyTo;
                vtblRaw->Commit_9 = &Commit;
                vtblRaw->Revert_10 = &Revert;
                vtblRaw->LockRegion_11 = &LockRegion;
                vtblRaw->UnlockRegion_12 = &UnlockRegion;
                vtblRaw->Stat_13 = &Stat;
                vtblRaw->Clone_14 = &Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Read(IStream* @this, void* pv, uint cb, uint* pcbRead)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    instance.Read((byte*)pv, cb, pcbRead);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Write(IStream* @this, void* pv, uint cb, uint* pcbWritten)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    instance.Write((byte*)pv, cb, pcbWritten);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Seek(IStream* @this, long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    instance.Seek(dlibMove, dwOrigin, plibNewPosition);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT SetSize(IStream* @this, ulong libNewSize)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    instance.SetSize(libNewSize);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT CopyTo(IStream* @this, IStream* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.CopyTo(pstm, cb, pcbRead, pcbWritten);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Commit(IStream* @this, STGC grfCommitFlags)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.Commit(grfCommitFlags);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Revert(IStream* thisPtr)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)thisPtr);
                    return instance.Revert();
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT LockRegion(IStream* @this, ulong libOffset, ulong cb, LOCKTYPE dwLockType)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.LockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT UnlockRegion(IStream* @this, ulong libOffset, ulong cb, uint dwLockType)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.UnlockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Stat(IStream* @this, STATSTG* pstatstg, STATFLAG grfStatFlag)
            {
                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.Stat(pstatstg, grfStatFlag);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Clone(IStream* @this, IStream** ppstm)
            {
                if (ppstm is null)
                {
                    return HRESULT.STG_E_INVALIDPOINTER;
                }

                try
                {
                    IStream.Interface instance = ComInterfaceDispatch.GetInstance<IStream.Interface>((ComInterfaceDispatch*)@this);
                    return instance.Clone(ppstm);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
