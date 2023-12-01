// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Ole;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.Controls.RichEdit;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public partial class RichTextBox
{
    // I used the visual basic 6 RichText (REOleCB.CPP) as a guide for this
    private unsafe class OleCallback : IRichEditOleCallback.Interface
    {
        private readonly RichTextBox _owner;
        private IDataObject? _lastDataObject;
        private DragDropEffects _lastEffect;
        private DragEventArgs? _lastDragEventArgs;

        internal OleCallback(RichTextBox owner) => _owner = owner;

        private void ClearDropDescription()
        {
            _lastDragEventArgs = null;
            DragDropHelper.ClearDropDescription(_lastDataObject);
        }

        public HRESULT GetNewStorage(IStorage** lplpstg)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::GetNewStorage");
            if (lplpstg is null)
            {
                return HRESULT.E_POINTER;
            }

            *lplpstg = null;
            if (!_owner.AllowOleObjects)
            {
                return HRESULT.E_FAIL;
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
                0,
                lplpstg);

            Debug.Assert(hr.Succeeded);

            return hr;
        }

        public HRESULT GetInPlaceContext(
            IOleInPlaceFrame** lplpFrame,
            IOleInPlaceUIWindow** lplpDoc,
            OLEINPLACEFRAMEINFO* lpFrameInfo)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::GetInPlaceContext");
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT ShowContainerUI(BOOL fShow)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::ShowContainerUI");
            return HRESULT.S_OK;
        }

        public HRESULT QueryInsertObject(Guid* lpclsid, IStorage* lpstg, int cp)
        {
            RichTextDbg.TraceVerbose($"IRichEditOleCallback::QueryInsertObject({(lpclsid is null ? "null" : lpclsid->ToString())})");
            return HRESULT.S_OK;
        }

        public HRESULT DeleteObject(IOleObject* lpoleobj)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::DeleteObject");
            return HRESULT.S_OK;
        }

        /// <inheritdoc cref="IRichEditOleCallback.QueryAcceptData(Com.IDataObject*, ushort*, RECO_FLAGS, BOOL, HGLOBAL)"/>
        public HRESULT QueryAcceptData(Com.IDataObject* lpdataobj, ushort* lpcfFormat, RECO_FLAGS reco, BOOL fReally, HGLOBAL hMetaPict)
        {
            RichTextDbg.TraceVerbose($"IRichEditOleCallback::QueryAcceptData(reco={reco})");

            if (reco != RECO_FLAGS.RECO_DROP)
            {
                return HRESULT.E_NOTIMPL;
            }

            if (!_owner.AllowDrop && !_owner.EnableAutoDragDrop)
            {
                RichTextDbg.TraceVerbose("\tCancel data, allowdrop == false");
                _lastDataObject = null;
                return HRESULT.E_FAIL;
            }

            MouseButtons mouseButtons = MouseButtons;
            Keys modifierKeys = ModifierKeys;

            MODIFIERKEYS_FLAGS keyState = 0;

            // Due to the order in which we get called, we have to set up the keystate here.
            // First GetDragDropEffect is called with grfKeyState == 0, and then
            // QueryAcceptData is called. Since this is the time we want to fire
            // OnDragEnter, but we have yet to get the keystate, we set it up ourselves.

            if ((mouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                keyState |= MODIFIERKEYS_FLAGS.MK_LBUTTON;
            }

            if ((mouseButtons & MouseButtons.Right) == MouseButtons.Right)
            {
                keyState |= MODIFIERKEYS_FLAGS.MK_RBUTTON;
            }

            if ((mouseButtons & MouseButtons.Middle) == MouseButtons.Middle)
            {
                keyState |= MODIFIERKEYS_FLAGS.MK_MBUTTON;
            }

            if ((modifierKeys & Keys.Control) == Keys.Control)
            {
                keyState |= MODIFIERKEYS_FLAGS.MK_CONTROL;
            }

            if ((modifierKeys & Keys.Shift) == Keys.Shift)
            {
                keyState |= MODIFIERKEYS_FLAGS.MK_SHIFT;
            }

            _lastDataObject = DataObject.FromComPointer(lpdataobj);

            if (!_owner.EnableAutoDragDrop)
            {
                _lastEffect = DragDropEffects.None;
            }

            DragEventArgs e = _lastDragEventArgs is null
                ? new DragEventArgs(_lastDataObject,
                    (int)keyState,
                    MousePosition.X,
                    MousePosition.Y,
                    DragDropEffects.All,
                    _lastEffect)
                : new DragEventArgs(_lastDataObject,
                    (int)keyState,
                    MousePosition.X,
                    MousePosition.Y,
                    DragDropEffects.All,
                    _lastEffect,
                    _lastDragEventArgs.DropImageType,
                    _lastDragEventArgs.Message ?? string.Empty,
                    _lastDragEventArgs.MessageReplacementToken ?? string.Empty);

            if (!fReally)
            {
                // We are just querying

                e.DropImageType = DropImageType.Invalid;
                e.Message = string.Empty;
                e.MessageReplacementToken = string.Empty;

                // We can get here without GetDragDropEffects actually being called first.
                // This happens when you drag/drop between two rtb's. Say you drag from rtb1 to rtb2.
                // GetDragDropEffects will first be called for rtb1, then QueryAcceptData for rtb1 just
                // like in the local drag case. Then you drag into rtb2. rtb2 will first be called in this method,
                // and not GetDragDropEffects. Now lastEffect is initialized to None for rtb2, so we would not allow
                // the drag. Thus we need to set the effect here as well.
                e.Effect = keyState.HasFlag(MODIFIERKEYS_FLAGS.MK_CONTROL) ? DragDropEffects.Copy : DragDropEffects.Move;
                _owner.OnDragEnter(e);

                if ((e.DropImageType > DropImageType.Invalid) && _owner.IsHandleCreated)
                {
                    UpdateDropDescription(e);
                    DragDropHelper.DragEnter(_owner.Handle, e);
                }
            }
            else
            {
                _owner.OnDragDrop(e);

                if (e.DropImageType > DropImageType.Invalid)
                {
                    ClearDropDescription();
                    DragDropHelper.Drop(e);
                    DragDropHelper.DragLeave();
                }

                _lastDataObject = null;
            }

            _lastEffect = e.Effect;
            if (e.Effect == DragDropEffects.None)
            {
                RichTextDbg.TraceVerbose("\tCancel data");
                return HRESULT.E_FAIL;
            }
            else
            {
                RichTextDbg.TraceVerbose("\tAccept data");
                return HRESULT.S_OK;
            }
        }

        public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::ContextSensitiveHelp");
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT GetClipboardData(CHARRANGE* lpchrg, uint reco, Com.IDataObject** lplpdataobj)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::GetClipboardData");
            return HRESULT.E_NOTIMPL;
        }

        public unsafe HRESULT GetDragDropEffect(BOOL fDrag, MODIFIERKEYS_FLAGS grfKeyState, DROPEFFECT* pdwEffect)
        {
            if (pdwEffect is null)
            {
                return HRESULT.E_POINTER;
            }

            RichTextDbg.TraceVerbose("IRichEditOleCallback::GetDragDropEffect");

            if (!_owner.AllowDrop && !_owner.EnableAutoDragDrop)
            {
                *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
                return HRESULT.S_OK;
            }

            if (fDrag && grfKeyState == default)
            {
                // This is the very first call we receive in a Drag-Drop operation,
                // so we will let the control know what we support.

                // Note that we haven't gotten any data yet, so we will let QueryAcceptData
                // do the OnDragEnter. Note too, that grfKeyState does not yet reflect the
                // current keystate
                _lastEffect = _owner.EnableAutoDragDrop
                    ? DragDropEffects.All | DragDropEffects.None
                    : DragDropEffects.None;
            }
            else
            {
                // We are either dragging over or dropping

                // The below is the complete reverse of what the docs on MSDN suggest,
                // but if we follow the docs, we would be firing OnDragDrop all the
                // time instead of OnDragOver (see

                // drag - fDrag = false, grfKeyState != 0
                // drop - fDrag = false, grfKeyState = 0
                // We only care about the drag.
                //
                // When we drop, lastEffect will have the right state
                if (!fDrag && _lastDataObject is not null && grfKeyState != default)
                {
                    DragEventArgs e = _lastDragEventArgs is null
                        ? new DragEventArgs(_lastDataObject,
                            (int)grfKeyState,
                            MousePosition.X,
                            MousePosition.Y,
                            DragDropEffects.All,
                            _lastEffect)
                        : new DragEventArgs(_lastDataObject,
                            (int)grfKeyState,
                            MousePosition.X,
                            MousePosition.Y,
                            DragDropEffects.All,
                            _lastEffect,
                            _lastDragEventArgs.DropImageType,
                            _lastDragEventArgs.Message ?? string.Empty,
                            _lastDragEventArgs.MessageReplacementToken ?? string.Empty);

                    // Now tell which of the allowable effects we want to use, but only if we are not already none
                    if (_lastEffect != DragDropEffects.None)
                    {
                        e.Effect = grfKeyState.HasFlag(MODIFIERKEYS_FLAGS.MK_CONTROL)
                            ? DragDropEffects.Copy
                            : DragDropEffects.Move;
                    }

                    _owner.OnDragOver(e);
                    _lastEffect = e.Effect;

                    if (e.DropImageType > DropImageType.Invalid)
                    {
                        UpdateDropDescription(e);
                        DragDropHelper.DragOver(e);
                    }
                }
            }

            *pdwEffect = (DROPEFFECT)_lastEffect;

            return HRESULT.S_OK;
        }

        public HRESULT GetContextMenu(
            RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE seltype,
            IOleObject* lpoleobj,
            CHARRANGE* lpchrg,
            HMENU* hmenu)
        {
            RichTextDbg.TraceVerbose("IRichEditOleCallback::GetContextMenu");

            // Do nothing, we don't have ContextMenu any longer
            if (hmenu is not null)
            {
                *hmenu = HMENU.Null;
            }

            return HRESULT.S_OK;
        }

        private void UpdateDropDescription(DragEventArgs e)
        {
            if (!e.Equals(_lastDragEventArgs))
            {
                _lastDragEventArgs = e.Clone();
                DragDropHelper.SetDropDescription(_lastDragEventArgs);
            }
        }
    }
}
