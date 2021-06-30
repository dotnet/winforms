// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    /// <summary>
    /// The ComWrappers implementation for System.Drawing.Common's COM interop usages.
    ///
    /// Supports IStream COM interface.
    /// </summary>
    internal unsafe class WinFormsComWrappers : ComWrappers
    {
        private const int S_OK = (int)Interop.HRESULT.S_OK;
        private static readonly ComInterfaceEntry* s_wrapperEntry = InitializeComInterfaceEntry();
        private static readonly Guid IID_IStream = new Guid(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
        internal static readonly Guid IID_IPicture = new Guid(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        internal static WinFormsComWrappers Instance { get; } = new WinFormsComWrappers();

        private WinFormsComWrappers() { }

        private static ComInterfaceEntry* InitializeComInterfaceEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInteface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iStreamVtbl = IStreamVtbl.Create(fpQueryInteface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID_IStream;
            wrapperEntry->Vtable = iStreamVtbl;
            return wrapperEntry;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            Debug.Assert(obj is Interop.Ole32.IStream);
            Debug.Assert(s_wrapperEntry != null);

            // Always return the same table mappings.
            count = 1;
            return s_wrapperEntry;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance);

            Guid pictureIID = IID_IPicture;
            int hr = Marshal.QueryInterface(externalComObject, ref pictureIID, out IntPtr comObject);
            if (hr == S_OK)
            {
                return new PictureWrapper(comObject);
            }

            throw new NotImplementedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }

        internal static class IStreamVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInteface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IStreamVtbl), IntPtr.Size * 14);
                vtblRaw[0] = fpQueryInteface;
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
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Read(pv, cb, pcbRead);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Write(IntPtr thisPtr, byte* pv, uint cb, uint* pcbWritten)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Write(pv, cb, pcbWritten);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Seek(IntPtr thisPtr, long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Seek(dlibMove, dwOrigin, plibNewPosition);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int SetSize(IntPtr thisPtr, ulong libNewSize)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.SetSize(libNewSize);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int CopyTo(IntPtr thisPtr, IntPtr pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    Interop.Ole32.IStream pstmStream = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)pstm);

                    inst.CopyTo(pstmStream, cb, pcbRead, pcbWritten);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Commit(IntPtr thisPtr, uint grfCommitFlags)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Commit((Interop.Ole32.STGC)grfCommitFlags);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Revert(IntPtr thisPtr)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Revert();
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int LockRegion(IntPtr thisPtr, ulong libOffset, ulong cb, uint dwLockType)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    return (int)inst.LockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int UnlockRegion(IntPtr thisPtr, ulong libOffset, ulong cb, uint dwLockType)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    return (int)inst.UnlockRegion(libOffset, cb, dwLockType);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Stat(IntPtr thisPtr, Interop.Ole32.STATSTG* pstatstg, Interop.Ole32.STATFLAG grfStatFlag)
            {
                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);
                    inst.Stat(out *pstatstg, grfStatFlag);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Clone(IntPtr thisPtr, IntPtr* ppstm)
            {
                if (ppstm == null)
                {
                    return (int)Interop.HRESULT.STG_E_INVALIDPOINTER;
                }

                try
                {
                    Interop.Ole32.IStream inst = ComInterfaceDispatch.GetInstance<Interop.Ole32.IStream>((ComInterfaceDispatch*)thisPtr);

                    *ppstm = Instance.GetOrCreateComInterfaceForObject(inst.Clone(), CreateComInterfaceFlags.None);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return S_OK;
            }
        }

        private class PictureWrapper : Ole32.IPicture, Ole32.IPersistStream
        {
            private readonly IntPtr _wrappedInstance;

            public PictureWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance;
            }

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
            }

            public int Handle
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 3 /* IPicture.get_Handle slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int hPal
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 4 /* IPicture.get_hPal slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public short Type
            {
                get
                {
                    short retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, short*, int>)(*(*(void***)_wrappedInstance + 5 /* IPicture.get_Type slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int Width
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 6 /* IPicture.get_Width slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public int Height
            {
                get
                {
                    int retVal;
                    int errorCode = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 7 /* IPicture.get_Height slot */)))
                        (_wrappedInstance, &retVal);
                    if (errorCode < 0)
                    {
                        Marshal.ThrowExceptionForHR(errorCode);
                    }

                    return retVal;
                }
            }

            public HRESULT GetClassID(
                Guid* pClassID)
            {
                throw new NotImplementedException();
            }

            public HRESULT IsDirty()
            {
                throw new NotImplementedException();
            }

            public void Load(Interop.Ole32.IStream pstm)
            {
                Guid streamIID = IID_IStream;
                IntPtr streamPtr = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(pstm, CreateComInterfaceFlags.None);
                Marshal.ThrowExceptionForHR(Marshal.QueryInterface(streamPtr, ref streamIID, out IntPtr pstmImpl));

                try
                {
                    Marshal.ThrowExceptionForHR(((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 5 /* IPersistStream.Load slot */)))
                        (_wrappedInstance, pstmImpl));
                }
                finally
                {
                    Marshal.Release(pstmImpl);
                }
            }

            public void Save(Interop.Ole32.IStream pstm, Interop.BOOL fClearDirty)
            {
                throw new NotImplementedException();
            }

            public long GetSizeMax()
            {
                throw new NotImplementedException();
            }
        }
    }
}
