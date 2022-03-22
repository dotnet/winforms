// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal class DropSource : Ole32.IDropSource
    {
        private readonly ISupportOleDropSource _peer;
        private readonly IComDataObject? _dataObject;

        public DropSource(ISupportOleDropSource peer) : this(peer, null)
        {
        }

        public DropSource(ISupportOleDropSource peer, IComDataObject? dataObject)
        {
            _peer = peer.OrThrowIfNull();
            _dataObject = dataObject;
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
            var gfbevent = new GiveFeedbackEventArgs((DragDropEffects)dwEffect, true);
            _peer.OnGiveFeedback(gfbevent);

            if (gfbevent.DragImage is not null)
            {
                DragDropHelper.SetDragImage(_dataObject, gfbevent.DragImage, gfbevent.CursorOffset, gfbevent.UseDefaultDragImage);
            }

            if (gfbevent.UseDefaultCursors)
            {
                return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
            }

            return HRESULT.S_OK;
        }
    }
}
