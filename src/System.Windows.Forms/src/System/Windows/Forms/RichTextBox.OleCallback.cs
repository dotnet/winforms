// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;
using static Interop.Richedit;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    public partial class RichTextBox
    {
        // I used the visual basic 6 RichText (REOleCB.CPP) as a guide for this
        private class OleCallback : Richedit.IRichEditOleCallback
        {
            private readonly RichTextBox owner;
            private IDataObject? lastDataObject;
            private DragDropEffects lastEffect;
            private IComDataObject? _lastComDataObject;
            private DropImageType _lastDropImageType = DropImageType.Invalid;
            private string _lastMessage = string.Empty;
            private string _lastMessageReplacementToken = string.Empty;

            internal OleCallback(RichTextBox owner)
            {
                this.owner = owner;
            }

            public HRESULT GetNewStorage(out Ole32.IStorage? storage)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetNewStorage");
                if (!owner.AllowOleObjects)
                {
                    storage = null;
                    return HRESULT.E_FAIL;
                }

                WinFormsComWrappers.LockBytesWrapper pLockBytes = Ole32.CreateILockBytesOnHGlobal(IntPtr.Zero, BOOL.TRUE);
                storage = Ole32.StgCreateDocfileOnILockBytes(
                    pLockBytes,
                    Ole32.STGM.SHARE_EXCLUSIVE | Ole32.STGM.CREATE | Ole32.STGM.READWRITE);
                Debug.Assert(storage is not null, "storage is NULL!");

                return HRESULT.S_OK;
            }

            public HRESULT GetInPlaceContext(IntPtr lplpFrame,
                                         IntPtr lplpDoc,
                                         IntPtr lpFrameInfo)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetInPlaceContext");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ShowContainerUI(BOOL fShow)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::ShowContainerUI");
                // Do nothing
                return HRESULT.S_OK;
            }

            public HRESULT QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::QueryInsertObject(" + lpclsid.ToString() + ")");
                return HRESULT.S_OK;
            }

            public HRESULT DeleteObject(IntPtr lpoleobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::DeleteObject");
                // Do nothing
                return HRESULT.S_OK;
            }

            public HRESULT QueryAcceptData(IComDataObject lpdataobj, IntPtr lpcfFormat, RECO reco, BOOL fReally, IntPtr hMetaPict)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::QueryAcceptData(reco=" + reco + ")");

                if (reco == RECO.DROP)
                {
                    if (owner.AllowDrop || owner.EnableAutoDragDrop)
                    {
                        MouseButtons b = Control.MouseButtons;
                        Keys k = Control.ModifierKeys;

                        User32.MK keyState = 0;

                        // Due to the order in which we get called, we have to set up the keystate here.
                        // First GetDragDropEffect is called with grfKeyState == 0, and then
                        // QueryAcceptData is called. Since this is the time we want to fire
                        // OnDragEnter, but we have yet to get the keystate, we set it up ourselves.

                        if ((b & MouseButtons.Left) == MouseButtons.Left)
                        {
                            keyState |= User32.MK.LBUTTON;
                        }

                        if ((b & MouseButtons.Right) == MouseButtons.Right)
                        {
                            keyState |= User32.MK.RBUTTON;
                        }

                        if ((b & MouseButtons.Middle) == MouseButtons.Middle)
                        {
                            keyState |= User32.MK.MBUTTON;
                        }

                        if ((k & Keys.Control) == Keys.Control)
                        {
                            keyState |= User32.MK.CONTROL;
                        }

                        if ((k & Keys.Shift) == Keys.Shift)
                        {
                            keyState |= User32.MK.SHIFT;
                        }

                        lastDataObject = new DataObject(lpdataobj);
                        _lastComDataObject = lpdataobj;

                        if (!owner.EnableAutoDragDrop)
                        {
                            lastEffect = DragDropEffects.None;
                        }

                        var e = new DragEventArgs(lastDataObject,
                                                  (int)keyState,
                                                  Control.MousePosition.X,
                                                  Control.MousePosition.Y,
                                                  DragDropEffects.All,
                                                  lastEffect,
                                                  _lastDropImageType,
                                                  _lastMessage,
                                                  _lastMessageReplacementToken);
                        if (fReally == 0)
                        {
                            // we are just querying

                            e.DropImageType = _lastDropImageType = DropImageType.Invalid;
                            e.Message = _lastMessage = string.Empty;
                            e.MessageReplacementToken = _lastMessageReplacementToken = string.Empty;

                            // We can get here without GetDragDropEffects actually being called first.
                            // This happens when you drag/drop between two rtb's. Say you drag from rtb1 to rtb2.
                            // GetDragDropEffects will first be called for rtb1, then QueryAcceptData for rtb1 just
                            // like in the local drag case. Then you drag into rtb2. rtb2 will first be called in this method,
                            // and not GetDragDropEffects. Now lastEffect is initialized to None for rtb2, so we would not allow
                            // the drag. Thus we need to set the effect here as well.
                            e.Effect = ((keyState & User32.MK.CONTROL) == User32.MK.CONTROL) ? DragDropEffects.Copy : DragDropEffects.Move;
                            owner.OnDragEnter(e);

                            if ((e.DropImageType > DropImageType.Invalid) && (_lastComDataObject is not null) && owner.IsHandleCreated)
                            {
                                DropImageType dropImageType = Enum.IsDefined(e.DropImageType) ? e.DropImageType : DropImageType.Invalid;
                                string message = e.Message ?? string.Empty;
                                string messageReplacementToken = e.MessageReplacementToken ?? string.Empty;

                                if (!dropImageType.Equals(_lastDropImageType) || !message.Equals(_lastMessage) || !messageReplacementToken.Equals(_lastMessageReplacementToken))
                                {
                                    _lastDropImageType = !dropImageType.Equals(_lastDropImageType) ? dropImageType : _lastDropImageType;
                                    _lastMessage = !message.Equals(_lastMessage) ? message : _lastMessage;
                                    _lastMessageReplacementToken = messageReplacementToken.Equals(_lastMessageReplacementToken) ? messageReplacementToken : _lastMessageReplacementToken;
                                    DragDropHelper.SetDropDescription(_lastComDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                                }

                                Point pt = new(e.X, e.Y);
                                DragDropHelper.DragEnter(owner.Handle, _lastComDataObject, ref pt, (uint)e.Effect);
                            }
                        }
                        else
                        {
                            owner.OnDragDrop(e);

                            if ((_lastDropImageType > DropImageType.Invalid) && (_lastComDataObject is not null))
                            {
                                if (!_lastDropImageType.Equals(DropImageType.Invalid) || !_lastMessage.Equals(string.Empty) || !_lastMessageReplacementToken.Equals(string.Empty))
                                {
                                    _lastDropImageType = !_lastDropImageType.Equals(DropImageType.Invalid) ? DropImageType.Invalid : _lastDropImageType;
                                    _lastMessage = !_lastMessage.Equals(string.Empty) ? string.Empty : _lastMessage;
                                    _lastMessageReplacementToken = !_lastMessageReplacementToken.Equals(string.Empty) ? string.Empty : _lastMessageReplacementToken;
                                    DragDropHelper.SetDropDescription(_lastComDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                                }

                                Point pt = new(e.X, e.Y);
                                DragDropHelper.Drop(_lastComDataObject, ref pt, (uint)e.Effect);
                                DragDropHelper.DragLeave();
                            }

                            _lastComDataObject = null;
                            lastDataObject = null;
                        }

                        lastEffect = e.Effect;
                        if (e.Effect == DragDropEffects.None)
                        {
                            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tCancel data");
                            return HRESULT.E_FAIL;
                        }
                        else
                        {
                            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tAccept data");
                            return HRESULT.S_OK;
                        }
                    }
                    else
                    {
                        Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tCancel data, allowdrop == false");
                        _lastComDataObject = null;
                        lastDataObject = null;
                        return HRESULT.E_FAIL;
                    }
                }
                else
                {
                    return HRESULT.E_NOTIMPL;
                }
            }

            public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::ContextSensitiveHelp");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetClipboardData(ref Richedit.CHARRANGE lpchrg, RECO reco, IntPtr lplpdataobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetClipboardData");
                return HRESULT.E_NOTIMPL;
            }

            public unsafe HRESULT GetDragDropEffect(BOOL fDrag, User32.MK grfKeyState, Ole32.DROPEFFECT* pdwEffect)
            {
                if (pdwEffect is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetDragDropEffect");

                if (owner.AllowDrop || owner.EnableAutoDragDrop)
                {
                    if (fDrag.IsTrue() && grfKeyState == (User32.MK)0)
                    {
                        // This is the very first call we receive in a Drag-Drop operation,
                        // so we will let the control know what we support.

                        // Note that we haven't gotten any data yet, so we will let QueryAcceptData
                        // do the OnDragEnter. Note too, that grfKeyState does not yet reflect the
                        // current keystate
                        if (owner.EnableAutoDragDrop)
                        {
                            lastEffect = (DragDropEffects.All | DragDropEffects.None);
                        }
                        else
                        {
                            lastEffect = DragDropEffects.None;
                        }
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
                        if (fDrag.IsFalse() && lastDataObject is not null && grfKeyState != (User32.MK)0)
                        {
                            DragEventArgs e = new DragEventArgs(lastDataObject,
                                                                (int)grfKeyState,
                                                                Control.MousePosition.X,
                                                                Control.MousePosition.Y,
                                                                DragDropEffects.All,
                                                                lastEffect,
                                                                _lastDropImageType,
                                                                _lastMessage,
                                                                _lastMessageReplacementToken);

                            // Now tell which of the allowable effects we want to use, but only if we are not already none
                            if (lastEffect != DragDropEffects.None)
                            {
                                e.Effect = ((grfKeyState & User32.MK.CONTROL) == User32.MK.CONTROL) ? DragDropEffects.Copy : DragDropEffects.Move;
                            }

                            owner.OnDragOver(e);
                            lastEffect = e.Effect;

                            if ((e.DropImageType > DropImageType.Invalid) && (_lastComDataObject is not null) && owner.IsHandleCreated)
                            {
                                DropImageType dropImageType = Enum.IsDefined(e.DropImageType) ? e.DropImageType : DropImageType.Invalid;
                                string message = e.Message ?? string.Empty;
                                string messageReplacementToken = e.MessageReplacementToken ?? string.Empty;

                                if (!dropImageType.Equals(_lastDropImageType) || !message.Equals(_lastMessage) || !messageReplacementToken.Equals(_lastMessageReplacementToken))
                                {
                                    _lastDropImageType = !dropImageType.Equals(_lastDropImageType) ? dropImageType : _lastDropImageType;
                                    _lastMessage = !message.Equals(_lastMessage) ? message : _lastMessage;
                                    _lastMessageReplacementToken = messageReplacementToken.Equals(_lastMessageReplacementToken) ? messageReplacementToken : _lastMessageReplacementToken;
                                    DragDropHelper.SetDropDescription(_lastComDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                                }

                                Point pt = new(e.X, e.Y);
                                DragDropHelper.DragOver(ref pt, (uint)e.Effect);
                            }
                        }
                    }

                    *pdwEffect = (Ole32.DROPEFFECT)lastEffect;
                }
                else
                {
                    *pdwEffect = Ole32.DROPEFFECT.NONE;
                }

                return HRESULT.S_OK;
            }

            public HRESULT GetContextMenu(short seltype, IntPtr lpoleobj, ref Richedit.CHARRANGE lpchrg, out IntPtr hmenu)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetContextMenu");

                // do nothing, we don't have ContextMenu any longer
                hmenu = IntPtr.Zero;
                return HRESULT.S_OK;
            }
        }
    }
}
