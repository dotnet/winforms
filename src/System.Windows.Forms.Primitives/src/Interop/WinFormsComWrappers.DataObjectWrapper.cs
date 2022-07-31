// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class DataObjectWrapper : IDataObject
        {
            private IntPtr _wrappedInstance;

            public DataObjectWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public void GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                fixed (FORMATETC* pFormat = &format)
                {
                    IDataObjectVtbl.STGMEDIUM_Raw mediumRaw;
                    ((delegate* unmanaged<IntPtr, FORMATETC*, IDataObjectVtbl.STGMEDIUM_Raw*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, pFormat, &mediumRaw).ThrowIfFailed();
                    medium.pUnkForRelease = Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease);
                    medium.tymed = mediumRaw.tymed;
                    medium.unionmember = mediumRaw.unionmember;
                }
            }

            public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                fixed (FORMATETC* pFormat = &format)
                {
                    IDataObjectVtbl.STGMEDIUM_Raw mediumRaw = new()
                    {
                        pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease),
                        tymed = medium.tymed,
                        unionmember = medium.unionmember,
                    };
                    ((delegate* unmanaged<IntPtr, FORMATETC*, IDataObjectVtbl.STGMEDIUM_Raw*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                        (_wrappedInstance, pFormat, &mediumRaw).ThrowIfFailed();
                    medium.pUnkForRelease = Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease);
                    medium.tymed = mediumRaw.tymed;
                    medium.unionmember = mediumRaw.unionmember;
                }
            }

            public int QueryGetData(ref FORMATETC format)
            {
                fixed (FORMATETC* pFormat = &format)
                {
                    return ((delegate* unmanaged<IntPtr, FORMATETC*, int>)(*(*(void***)_wrappedInstance + 5)))
                        (_wrappedInstance, pFormat);
                }
            }

            public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                fixed (FORMATETC* pFormatIn = &formatIn)
                fixed (FORMATETC* pFormatOut = &formatOut)
                {
                    return ((delegate* unmanaged<IntPtr, FORMATETC*, FORMATETC*, int>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, pFormatIn, pFormatOut);
                }
            }

            public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
            {
                fixed (FORMATETC* pFormatIn = &formatIn)
                {
                    IDataObjectVtbl.STGMEDIUM_Raw mediumRaw = new()
                    {
                        pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease),
                        tymed = medium.tymed,
                        unionmember = medium.unionmember,
                    };
                    ((delegate* unmanaged<IntPtr, FORMATETC*, IDataObjectVtbl.STGMEDIUM_Raw*, int, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, pFormatIn, &mediumRaw, release ? 1 : 0).ThrowIfFailed();
                    medium.pUnkForRelease = Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease);
                    medium.tymed = mediumRaw.tymed;
                    medium.unionmember = mediumRaw.unionmember;
                }
            }

            public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
            {
                IntPtr retVal;
                ((delegate* unmanaged<IntPtr, DATADIR, IntPtr*, int>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, direction, &retVal);
                return (IEnumFORMATETC)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(retVal, CreateObjectFlags.Unwrap);
            }

            public int DAdvise(ref FORMATETC format, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                fixed (FORMATETC* pFormat = &format)
                fixed (int* connectionPtr = &connection)
                {
                    IntPtr adviseSinkPtr = Marshal.GetComInterfaceForObject<IAdviseSink, IAdviseSink>(adviseSink);
                    try
                    {
                        return ((delegate* unmanaged<IntPtr, FORMATETC*, ADVF, IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 9)))
                            (_wrappedInstance, pFormat, advf, adviseSinkPtr, connectionPtr);
                    }
                    finally
                    {
                        Marshal.Release(adviseSinkPtr);
                    }
                }
            }

            public void DUnadvise(int connection)
            {
                ((delegate* unmanaged<IntPtr, int, HRESULT>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, connection).ThrowIfFailed();
            }

            public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                IntPtr retVal;
                int result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, &retVal);
                enumAdvise = retVal == IntPtr.Zero ? null : (IEnumSTATDATA)Marshal.GetObjectForIUnknown(retVal);
                return result;
            }
        }
    }
}
