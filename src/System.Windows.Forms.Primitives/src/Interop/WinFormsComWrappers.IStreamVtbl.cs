// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IStreamVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IStreamVtbl), IntPtr.Size * 14);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, byte*, uint, uint*, int>)&Read;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, byte*, uint, uint*, int>)&Write;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, long, SeekOrigin, ulong*, int>)&Seek;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, ulong, int>)&SetSize;
                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, ulong, ulong*, ulong*, int>)&CopyTo;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, uint, int>)&Commit;
                vtblRaw[9] = (IntPtr)(delegate* unmanaged<IntPtr, int>)&Revert;
                vtblRaw[10] = (IntPtr)(delegate* unmanaged<IntPtr, ulong, ulong, uint, int>)&LockRegion;
                vtblRaw[11] = (IntPtr)(delegate* unmanaged<IntPtr, ulong, ulong, uint, int>)&UnlockRegion;
                vtblRaw[12] = (IntPtr)(delegate* unmanaged<IntPtr, Interop.Ole32.STATSTG*, Interop.Ole32.STATFLAG, int>)&Stat;
                vtblRaw[13] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, int>)&Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static int Read(IntPtr thisPtr, byte* pv, uint cb, uint* pcbRead)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Read(pv, cb, pcbRead);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Write(IntPtr thisPtr, byte* pv, uint cb, uint* pcbWritten)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Write(pv, cb, pcbWritten);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Seek(IntPtr thisPtr, long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Seek(dlibMove, dwOrigin, plibNewPosition);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int SetSize(IntPtr thisPtr, ulong libNewSize)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.SetSize(libNewSize);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int CopyTo(IntPtr thisPtr, IntPtr pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    Interop.Ole32.IStream pstmStream = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)pstm);

                    instance.CopyTo(pstmStream, cb, pcbRead, pcbWritten);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Commit(IntPtr thisPtr, uint grfCommitFlags)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Commit((Interop.Ole32.STGC)grfCommitFlags);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Revert(IntPtr thisPtr)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Revert();
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int LockRegion(IntPtr thisPtr, ulong libOffset, ulong cb, uint dwLockType)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    return (int)instance.LockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int UnlockRegion(IntPtr thisPtr, ulong libOffset, ulong cb, uint dwLockType)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    return (int)instance.UnlockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Stat(IntPtr thisPtr, Interop.Ole32.STATSTG* pstatstg, Interop.Ole32.STATFLAG grfStatFlag)
            {
                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    instance.Stat(out *pstatstg, grfStatFlag);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Clone(IntPtr thisPtr, IntPtr* ppstm)
            {
                if (ppstm is null)
                {
                    return (int)HRESULT.Values.STG_E_INVALIDPOINTER;
                }

                try
                {
                    Interop.Ole32.IStream instance = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);

                    *ppstm = Instance.GetOrCreateComInterfaceForObject(instance.Clone(), CreateComInterfaceFlags.None);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return ex.HResult;
                }
            }
        }
    }
}
