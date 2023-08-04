// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

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
        private static TraceSwitch s_richTextDebug;

        private static TraceSwitch RichTextDebug
            => s_richTextDebug ??= new TraceSwitch("RichTextDbg", "Debug info about RichTextBox");

        internal OleCallback(RichTextBox owner) => _owner = owner;

        public HRESULT GetNewStorage(IStorage** lplpstg)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetNewStorage");

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
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetInPlaceContext");
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT ShowContainerUI(BOOL fShow)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::ShowContainerUI");
            return HRESULT.S_OK;
        }

        public unsafe HRESULT QueryInsertObject(Guid* lpclsid, IStorage* lpstg, int cp)
        {
            Debug.WriteLineIf(
                RichTextDebug.TraceVerbose,
                $"IRichTextBoxOleCallback::QueryInsertObject({(lpclsid is null ? "null" : lpclsid->ToString())})");

            HRESULT hr = PInvoke.ReadClassStg(lpstg, out Guid realClsid);
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, $"real clsid:{realClsid} (hr={hr:X})");

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

            switch (realClsid.ToString().ToUpper(CultureInfo.InvariantCulture))
            {
                case "00000315-0000-0000-C000-000000000046": // Metafile
                case "00000316-0000-0000-C000-000000000046": // DIB
                case "00000319-0000-0000-C000-000000000046": // EMF
                case "0003000A-0000-0000-C000-000000000046": // BMP
                    return HRESULT.S_OK;
                default:
                    Debug.WriteLineIf(
                        RichTextDebug.TraceVerbose,
                        $"   denying '{realClsid}' from being inserted due to security restrictions");
                    return HRESULT.S_FALSE;
            }
        }

        public HRESULT DeleteObject(IOleObject* lpoleobj)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::DeleteObject");
            return HRESULT.S_OK;
        }

        public HRESULT QueryAcceptData(Com.IDataObject* lpdataobj, ushort* lpcfFormat, RECO_FLAGS reco, BOOL fReally, HGLOBAL hMetaPict)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, $"IRichTextBoxOleCallback::QueryAcceptData(reco={reco})");
            if (reco == RECO_FLAGS.RECO_PASTE)
            {
                if (lpdataobj == null)
                {
                    return HRESULT.E_POINTER;
                }

                FORMATETC textFormat = new()
                {
                    cfFormat = (ushort)CLIPBOARD_FORMAT.CF_TEXT
                };

                FORMATETC unicodeFormat = new()
                {
                    cfFormat = (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT
                };

                return lpdataobj->QueryGetData(&textFormat).Succeeded || lpdataobj->QueryGetData(&unicodeFormat).Succeeded
                    ? HRESULT.S_OK
                    : HRESULT.E_FAIL;
            }

            return HRESULT.E_NOTIMPL;
        }

        public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::ContextSensitiveHelp");
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT GetClipboardData(CHARRANGE* lpchrg, uint reco, Com.IDataObject** lplpdataobj)
        {
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetClipboardData");
            return HRESULT.E_NOTIMPL;
        }

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
            Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetContextMenu");

            // Do nothing, we don't have ContextMenu any longer.
            if (hmenu is not null)
            {
                *hmenu = HMENU.Null;
            }

            return HRESULT.S_OK;
        }
    }
}
