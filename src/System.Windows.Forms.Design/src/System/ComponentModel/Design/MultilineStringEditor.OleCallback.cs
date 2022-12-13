// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.ComponentModel.Design
{
    public sealed partial class MultilineStringEditor
    {
        // I used the visual basic 6 RichText (REOleCB.CPP) as a guide for this
        private class OleCallback : Richedit.IRichEditOleCallback
        {
            private readonly RichTextBox _owner;
            private static TraceSwitch s_richTextDebug;

            private static TraceSwitch RichTextDebug
                => s_richTextDebug ??= new TraceSwitch("RichTextDbg", "Debug info about RichTextBox");

            internal OleCallback(RichTextBox owner) => _owner = owner;

            public unsafe HRESULT GetNewStorage(out IStorage* storage)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetNewStorage");
                using ComScope<ILockBytes> pLockBytes = new(null);
                PInvoke.CreateILockBytesOnHGlobal(0, true, pLockBytes);

                fixed (IStorage** pStorage = &storage)
                {
                    HRESULT result = PInvoke.StgCreateDocfileOnILockBytes(
                    pLockBytes,
                    STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE,
                    reserved: 0,
                    pStorage);

                    Debug.Assert(result.Succeeded, "storage is NULL!");

                    return result;
                }
            }

            public HRESULT GetInPlaceContext(IntPtr lplpFrame, IntPtr lplpDoc, IntPtr lpFrameInfo)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetInPlaceContext");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ShowContainerUI(BOOL fShow)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::ShowContainerUI");
                return HRESULT.S_OK;
            }

            public HRESULT QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, $"IRichTextBoxOleCallback::QueryInsertObject({lpclsid})");

                HRESULT hr = Ole32.ReadClassStg(lpstg, out Guid realClsid);
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, $"real clsid:{realClsid} (hr={hr:X})");

                if (!hr.Succeeded)
                {
                    return HRESULT.S_FALSE;
                }

                if (realClsid == Guid.Empty)
                {
                    realClsid = lpclsid;
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
                            $"   denying '{lpclsid}' from being inserted due to security restrictions");
                        return HRESULT.S_FALSE;
                }
            }

            public HRESULT DeleteObject(IntPtr lpoleobj)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::DeleteObject");
                return HRESULT.S_OK;
            }

            public HRESULT QueryAcceptData(IComDataObject lpdataobj, IntPtr lpcfFormat, Richedit.RECO reco, BOOL fReally, IntPtr hMetaPict)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, $"IRichTextBoxOleCallback::QueryAcceptData(reco={reco})");
                if (reco == Richedit.RECO.PASTE)
                {
                    DataObject dataObj = new DataObject(lpdataobj);
                    if (dataObj.GetDataPresent(DataFormats.Text) || dataObj.GetDataPresent(DataFormats.UnicodeText))
                    {
                        return HRESULT.S_OK;
                    }

                    return HRESULT.E_FAIL;
                }

                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::ContextSensitiveHelp");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetClipboardData(ref Richedit.CHARRANGE lpchrg, Richedit.RECO reco, IntPtr lplpdataobj)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetClipboardData");
                return HRESULT.E_NOTIMPL;
            }

            public unsafe HRESULT GetDragDropEffect(BOOL fDrag, User32.MK grfKeyState, Ole32.DROPEFFECT* pdwEffect)
            {
                if (pdwEffect is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pdwEffect = Ole32.DROPEFFECT.NONE;
                return HRESULT.S_OK;
            }

            public HRESULT GetContextMenu(short seltype, IntPtr lpoleobj, ref Richedit.CHARRANGE lpchrg, out IntPtr hmenu)
            {
                Debug.WriteLineIf(RichTextDebug.TraceVerbose, "IRichTextBoxOleCallback::GetContextMenu");

                // Do nothing, we don't have ContextMenu any longer.
                hmenu = IntPtr.Zero;
                return HRESULT.S_OK;
            }
        }
    }
}
