// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Ole;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.Controls.RichEdit;
using Com = Windows.Win32.System.Com;

namespace System.ComponentModel.Design;

public sealed partial class MultilineStringEditor
{
    private unsafe class OleCallback : IRichEditOleCallback.Interface
    {
        private readonly RichTextBox _owner;

        internal OleCallback(RichTextBox owner) => _owner = owner;

        public HRESULT GetNewStorage(IStorage** lplpstg)
        {
            if (lplpstg is null)
            {
                return HRESULT.E_POINTER;
            }

            using ComScope<ILockBytes> pLockBytes = new(null);
            HRESULT hr = PInvoke.CreateILockBytesOnHGlobal(default, fDeleteOnRelease: true, pLockBytes);
            if (hr.Failed)
            {
                return hr;
            }

            hr = PInvoke.StgCreateDocfileOnILockBytes(
                pLockBytes,
                STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE,
                reserved: 0,
                lplpstg);

            Debug.Assert(hr.Succeeded);

            return hr;
        }

        public HRESULT GetInPlaceContext(
            IOleInPlaceFrame** lplpFrame,
            IOleInPlaceUIWindow** lplpDoc,
            OLEINPLACEFRAMEINFO* lpFrameInfo)
        {
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT ShowContainerUI(BOOL fShow)
        {
            return HRESULT.S_OK;
        }

        public unsafe HRESULT QueryInsertObject(Guid* lpclsid, IStorage* lpstg, int cp)
        {
            HRESULT hr = PInvoke.ReadClassStg(lpstg, out Guid realClsid);

            if (!hr.Succeeded)
            {
                return HRESULT.S_FALSE;
            }

            if (realClsid == Guid.Empty)
            {
                if (lpclsid is null)
                {
                    return HRESULT.E_POINTER;
                }

                realClsid = *lpclsid;
            }

            return realClsid.ToString().ToUpper(CultureInfo.InvariantCulture) switch
            {
                // Metafile
                "00000315-0000-0000-C000-000000000046"
                    or "00000316-0000-0000-C000-000000000046"
                    or "00000319-0000-0000-C000-000000000046"
                    or "0003000A-0000-0000-C000-000000000046" => HRESULT.S_OK,
                _ => HRESULT.S_FALSE,
            };
        }

        public HRESULT DeleteObject(IOleObject* lpoleobj) => HRESULT.S_OK;

        public HRESULT QueryAcceptData(Com.IDataObject* lpdataobj, ushort* lpcfFormat, RECO_FLAGS reco, BOOL fReally, HGLOBAL hMetaPict)
        {
            if (reco == RECO_FLAGS.RECO_PASTE)
            {
                if (lpdataobj == null)
                {
                    return HRESULT.E_POINTER;
                }

                FORMATETC textFormat = new()
                {
                    cfFormat = (ushort)CLIPBOARD_FORMAT.CF_TEXT,
                    dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)(TYMED.TYMED_HGLOBAL | TYMED.TYMED_ISTREAM | TYMED.TYMED_GDI)
                };

                FORMATETC unicodeFormat = new()
                {
                    cfFormat = (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT,
                    dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)(TYMED.TYMED_HGLOBAL | TYMED.TYMED_ISTREAM | TYMED.TYMED_GDI)
                };

                bool success = lpdataobj->QueryGetData(&textFormat).Succeeded || lpdataobj->QueryGetData(&unicodeFormat).Succeeded;
                Debug.Assert(success);
                return success ? HRESULT.S_OK : HRESULT.E_FAIL;
            }

            return HRESULT.E_NOTIMPL;
        }

        public HRESULT ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.E_NOTIMPL;

        public HRESULT GetClipboardData(CHARRANGE* lpchrg, uint reco, Com.IDataObject** lplpdataobj) => HRESULT.E_NOTIMPL;

        public unsafe HRESULT GetDragDropEffect(BOOL fDrag, MODIFIERKEYS_FLAGS grfKeyState, DROPEFFECT* pdwEffect)
        {
            if (pdwEffect is null)
            {
                return HRESULT.E_POINTER;
            }

            *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
            return HRESULT.S_OK;
        }

        public HRESULT GetContextMenu(
            RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE seltype,
            IOleObject* lpoleobj,
            CHARRANGE* lpchrg,
            HMENU* hmenu)
        {
            // Do nothing, we don't have ContextMenu any longer.
            if (hmenu is not null)
            {
                *hmenu = HMENU.Null;
            }

            return HRESULT.S_OK;
        }
    }
}
