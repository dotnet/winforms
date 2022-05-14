// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal class DropSource : Ole32.IDropSource, Ole32.IDropSourceNotify
    {
        private readonly ISupportOleDropSource _peer;
        private readonly IComDataObject? _dataObject;
        private IntPtr _lastHwndTarget;
        private Bitmap? _lastDragImage;
        private Point _lastCursorOffset;
        private bool _lastUseDefaultDragImage;

        public DropSource(ISupportOleDropSource peer) : this(peer, dataObject: null, dragImage: null, cursorOffset: default, useDefaultDragImage: false)
        {
        }

        public DropSource(ISupportOleDropSource peer, IComDataObject? dataObject, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
        {
            _peer = peer.OrThrowIfNull();

            if (dragImage is not null && dataObject is not null)
            {
                _dataObject = dataObject;
                _lastDragImage = dragImage;
                _lastCursorOffset = cursorOffset;
                _lastUseDefaultDragImage = useDefaultDragImage;
                DragDropHelper.SetDragImage(_dataObject, dragImage, cursorOffset, useDefaultDragImage);
            }
        }

        private void UpdateDragImage(GiveFeedbackEventArgs gfbevent)
        {
            if (_dataObject is null)
            {
                return;
            }

            if (!gfbevent.DragImage.Equals(_lastDragImage) || !gfbevent.CursorOffset.Equals(_lastCursorOffset) || !gfbevent.UseDefaultDragImage.Equals(_lastUseDefaultDragImage))
            {
                _lastDragImage = gfbevent.DragImage;
                _lastCursorOffset = gfbevent.CursorOffset;
                _lastUseDefaultDragImage = gfbevent.UseDefaultDragImage;
                DragDropHelper.SetDragImage(_dataObject, _lastDragImage, _lastCursorOffset, _lastUseDefaultDragImage);

                if (!_lastHwndTarget.Equals(IntPtr.Zero) && (Cursor.Position is Point point))
                {
                    DragDropHelper.DragEnter(_lastHwndTarget, _dataObject, ref point, (uint)gfbevent.Effect);
                }
            }
        }

        public HRESULT QueryContinueDrag(BOOL fEscapePressed, User32.MK grfKeyState)
        {
            bool escapePressed = fEscapePressed != 0;
            DragAction action = DragAction.Continue;
            if (escapePressed)
            {
                action = DragAction.Cancel;
            }
            else if ((grfKeyState & User32.MK.LBUTTON) == 0
                     && (grfKeyState & User32.MK.RBUTTON) == 0
                     && (grfKeyState & User32.MK.MBUTTON) == 0)
            {
                action = DragAction.Drop;
            }

            QueryContinueDragEventArgs qcdevent = new QueryContinueDragEventArgs((int)grfKeyState, escapePressed, action);
            _peer.OnQueryContinueDrag(qcdevent);

            switch (qcdevent.Action)
            {
                case DragAction.Drop:
                    return HRESULT.DRAGDROP_S_DROP;
                case DragAction.Cancel:
                    return HRESULT.DRAGDROP_S_CANCEL;
                default:
                    return HRESULT.S_OK;
            }
        }

        public HRESULT GiveFeedback(Ole32.DROPEFFECT dwEffect)
        {
            var gfbevent = _lastDragImage is null
                ? new GiveFeedbackEventArgs((DragDropEffects)dwEffect, true)
                : new GiveFeedbackEventArgs((DragDropEffects)dwEffect, false, _lastDragImage, _lastCursorOffset, _lastUseDefaultDragImage);
            _peer.OnGiveFeedback(gfbevent);

            if (gfbevent.DragImage is not null)
            {
                UpdateDragImage(gfbevent);
            }

            if (gfbevent.UseDefaultCursors)
            {
                return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
            }

            return HRESULT.S_OK;
        }

        public HRESULT DragEnterTarget(IntPtr hwndTarget)
        {
            _lastHwndTarget = hwndTarget;
            return HRESULT.S_OK;
        }

        public HRESULT DragLeaveTarget()
        {
            if (_lastDragImage is not null)
            {
                DragDropHelper.DragLeave();
            }

            return HRESULT.S_OK;
        }
    }
}
