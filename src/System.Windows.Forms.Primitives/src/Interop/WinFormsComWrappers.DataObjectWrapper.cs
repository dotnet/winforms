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
        internal sealed class DataObjectWrapper : IDataObject, IDisposable
        {
            private IntPtr _wrappedInstance;

            public DataObjectWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            ~DataObjectWrapper()
            {
                this.DisposeInternal();
            }

            public void Dispose()
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
            }

            private void DisposeInternal()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public void GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                fixed (FORMATETC* formatPtr = &format)
                {
                    STGMEDIUM_Raw mediumRaw;
                    ((delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, formatPtr, &mediumRaw).ThrowIfFailed();
                    medium = new()
                    {
                        pUnkForRelease = mediumRaw.pUnkForRelease == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease),
                        tymed = mediumRaw.tymed,
                        unionmember = mediumRaw.unionmember,
                    };
                    if (mediumRaw.pUnkForRelease != IntPtr.Zero)
                    {
                        Marshal.Release(mediumRaw.pUnkForRelease);
                    }
                }
            }

            public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                fixed (FORMATETC* formatPtr = &format)
                {
                    STGMEDIUM_Raw mediumRaw = new()
                    {
                        pUnkForRelease = medium.pUnkForRelease is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease),
                        tymed = medium.tymed,
                        unionmember = medium.unionmember,
                    };
                    ((delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                        (_wrappedInstance, formatPtr, &mediumRaw).ThrowIfFailed();
                    medium = new()
                    {
                        pUnkForRelease = mediumRaw.pUnkForRelease == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease),
                        tymed = mediumRaw.tymed,
                        unionmember = mediumRaw.unionmember,
                    };
                    if (mediumRaw.pUnkForRelease != IntPtr.Zero)
                    {
                        Marshal.Release(mediumRaw.pUnkForRelease);
                    }
                }
            }

            public int QueryGetData(ref FORMATETC format)
            {
                fixed (FORMATETC* formatPtr = &format)
                {
                    return (int)((delegate* unmanaged<IntPtr, FORMATETC*, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                        (_wrappedInstance, formatPtr);
                }
            }

            public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                fixed (FORMATETC* formatInPtr = &formatIn)
                fixed (FORMATETC* formatOutPtr = &formatOut)
                {
                    return (int)((delegate* unmanaged<IntPtr, FORMATETC*, FORMATETC*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, formatInPtr, formatOutPtr);
                }
            }

            public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
            {
                fixed (FORMATETC* formatInPtr = &formatIn)
                {
                    STGMEDIUM_Raw mediumRaw = new()
                    {
                        pUnkForRelease = medium.pUnkForRelease is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease),
                        tymed = medium.tymed,
                        unionmember = medium.unionmember,
                    };
                    ((delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, int, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, formatInPtr, &mediumRaw, release ? 1 : 0).ThrowIfFailed();
                    medium = new()
                    {
                        pUnkForRelease = mediumRaw.pUnkForRelease == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(mediumRaw.pUnkForRelease),
                        tymed = mediumRaw.tymed,
                        unionmember = mediumRaw.unionmember,
                    };
                    if (mediumRaw.pUnkForRelease != IntPtr.Zero)
                    {
                        Marshal.Release(mediumRaw.pUnkForRelease);
                    }
                }
            }

            public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
            {
                IntPtr resultPtr;
                ((delegate* unmanaged<IntPtr, DATADIR, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, direction, &resultPtr).ThrowIfFailed();
                return (IEnumFORMATETC)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(resultPtr, CreateObjectFlags.Unwrap);
            }

            public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                fixed (FORMATETC* formatPtr = &pFormatetc)
                fixed (int* connectionPtr = &connection)
                {
                    var adviseSinkPtr = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(adviseSink, CreateComInterfaceFlags.None);
                    return (int)((delegate* unmanaged<IntPtr, FORMATETC*, ADVF, IntPtr, int*, HRESULT>)(*(*(void***)_wrappedInstance + 9)))
                        (_wrappedInstance, formatPtr, advf, adviseSinkPtr, connectionPtr);
                }
            }

            public void DUnadvise(int connection)
            {
                ((delegate* unmanaged<IntPtr, int, HRESULT>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, connection).ThrowIfFailed();
            }

            public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                IntPtr enumAdvisePtr;
                var result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, &enumAdvisePtr);
                enumAdvise = result.Succeeded() ? null : (IEnumSTATDATA)Marshal.GetObjectForIUnknown(enumAdvisePtr);
                return (int)result;
            }
        }
    }
}
