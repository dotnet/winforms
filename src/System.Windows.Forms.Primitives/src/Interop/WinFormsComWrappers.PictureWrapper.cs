// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class PictureWrapper : IPicture.Interface, IPersistStream.Interface, IDisposable
        {
            private IPicture* _wrappedInstance;

            public PictureWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = (IPicture*)wrappedInstance.OrThrowIfZero();
            }

            public void Dispose()
            {
                _wrappedInstance->Release();
                _wrappedInstance = null;
            }

            public HRESULT get_Handle(uint* pHandle)
                => pHandle is null ? HRESULT.E_POINTER : _wrappedInstance->get_Handle(pHandle).ThrowOnFailure();

            public HRESULT get_hPal(uint* phPal)
                =>phPal is null ? HRESULT.E_POINTER : _wrappedInstance->get_hPal(phPal).ThrowOnFailure();

            public HRESULT get_Type(short* pType)
                => pType is null ? HRESULT.E_POINTER : _wrappedInstance->get_Type(pType).ThrowOnFailure();

            public HRESULT get_Width(int* pWidth)
                => pWidth is null ? HRESULT.E_POINTER : _wrappedInstance->get_Width(pWidth).ThrowOnFailure();

            public HRESULT get_Height(int* pHeight)
                => pHeight is null ? HRESULT.E_POINTER : _wrappedInstance->get_Height(pHeight).ThrowOnFailure();

            public HRESULT GetClassID(Guid* pClassID) => HRESULT.E_NOTIMPL;

            public HRESULT IsDirty() => HRESULT.E_NOTIMPL;

            public HRESULT Load(IStream* pstm)
            {
                Guid persistedStreamIID = IPersistStream.Guid;
                IPersistStream* pPersistStream = null;
                try
                {
                    int errorCode = Marshal.QueryInterface((nint)_wrappedInstance, ref persistedStreamIID, out nint temp);
                    pPersistStream = (IPersistStream*)temp;
                    return pPersistStream->Load(pstm).ThrowOnFailure();
                }
                finally
                {
                    ComHelpers.Release(pstm);
                    ComHelpers.Release(pPersistStream);
                }
            }

            public HRESULT Save(IStream* pstm, BOOL fClearDirty) => HRESULT.E_NOTIMPL;

            public HRESULT GetSizeMax(ulong* pcbSize) => HRESULT.E_NOTIMPL;

            public HRESULT Render(HDC hDC, int x, int y, int cx, int cy, int xSrc, int ySrc, int cxSrc, int cySrc, RECT* pRcWBounds)
                => HRESULT.E_NOTIMPL;

            public HRESULT set_hPal(uint hPal) => HRESULT.E_NOTIMPL;

            public HRESULT get_CurDC(HDC* phDC) => HRESULT.E_NOTIMPL;

            public HRESULT SelectPicture(HDC hDCIn, HDC* phDCOut, uint* phBmpOut) => HRESULT.E_NOTIMPL;

            public HRESULT get_KeepOriginalFormat(BOOL* pKeep) => HRESULT.E_NOTIMPL;

            public HRESULT put_KeepOriginalFormat(BOOL keep) => HRESULT.E_NOTIMPL;

            public HRESULT PictureChanged() => HRESULT.E_NOTIMPL;

            public HRESULT SaveAsFile(IStream* pStream, BOOL fSaveMemCopy, int* pCbSize) => HRESULT.E_NOTIMPL;

            public HRESULT get_Attributes(uint* pDwAttr) => HRESULT.E_NOTIMPL;
        }
    }
}
