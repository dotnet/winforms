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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
                    Interop.Ole32.IStream pstmStream = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)pstm);

                    instance.CopyTo(pstmStream, cb, pcbRead, pcbWritten);
                    return HRESULT.S_OK;
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
                    instance.Commit((Interop.Ole32.STGC)grfCommitFlags);
                    return HRESULT.S_OK;
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Revert();
                    return HRESULT.S_OK;
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
                    return instance.LockRegion(libOffset, cb, (uint)dwLockType);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);
                    instance.Stat(out *((Ole32.STATSTG*)pstatstg), (Ole32.STATFLAG)grfStatFlag);
                    return HRESULT.S_OK;
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
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)@this);

                    *ppstm = (IStream*)Instance.GetOrCreateComInterfaceForObject(instance.Clone(), CreateComInterfaceFlags.None);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
