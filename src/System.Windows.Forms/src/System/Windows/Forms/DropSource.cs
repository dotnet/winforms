// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
	using System;

	/// </devdoc>
        internal class DropSource : UnsafeNativeMethods.IOleDropSource {

            private const int DragDropSDrop    = 0x00040100;
            private const int DragDropSCancel  = 0x00040101;
            private const int DragDropSUseDefaultCursors = 0x00040102;
    
            private ISupportOleDropSource peer;

            public DropSource(ISupportOleDropSource peer ) {
                if (peer == null)
                    throw new ArgumentNullException(nameof(peer));
                this.peer = peer;
            }

            public int OleQueryContinueDrag(int fEscapePressed, int grfKeyState) {
                QueryContinueDragEventArgs qcdevent = null;
                bool escapePressed = (fEscapePressed != 0);
                DragAction action = DragAction.Continue;
                if (escapePressed) {
                    action = DragAction.Cancel;
                }
                else if ((grfKeyState & NativeMethods.MK_LBUTTON) == 0
                         && (grfKeyState & NativeMethods.MK_RBUTTON) == 0
                         && (grfKeyState & NativeMethods.MK_MBUTTON) == 0) {
                    action = DragAction.Drop;
                }

                qcdevent = new QueryContinueDragEventArgs(grfKeyState,escapePressed, action);
                peer.OnQueryContinueDrag(qcdevent);

                int hr = 0;

                switch (qcdevent.Action) {
                    case DragAction.Drop:
                        hr = DragDropSDrop;
                        break;
                    case DragAction.Cancel:
                        hr = DragDropSCancel;
                        break;
                }

                return hr;
            }

            public int OleGiveFeedback(int dwEffect) {
                GiveFeedbackEventArgs gfbevent = new GiveFeedbackEventArgs((DragDropEffects) dwEffect, true);
                peer.OnGiveFeedback(gfbevent);
                if (gfbevent.UseDefaultCursors) {
                    return DragDropSUseDefaultCursors;
                }
                return 0;
            }
        }
}
