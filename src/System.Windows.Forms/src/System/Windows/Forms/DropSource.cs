// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static System.Runtime.InteropServices.ComWrappers;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal class DropSource : Ole32.IDropSource, Ole32.IDropSourceNotify, IManagedWrapper
    {
        private readonly ISupportOleDropSource _peer;
        private readonly IComDataObject _dataObject;
        private IntPtr _lastHwndTarget;
        private GiveFeedbackEventArgs? _lastGiveFeedbacEventArgs;
        private static ComInterfaceTable? s_comInterfaceTable;

        public DropSource(ISupportOleDropSource peer, IComDataObject dataObject, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
        {
            _peer = peer.OrThrowIfNull();
            _dataObject = dataObject.OrThrowIfNull();

            if (dragImage is not null)
            {
                _lastGiveFeedbacEventArgs = new(DragDropEffects.None, useDefaultCursors: false, dragImage, cursorOffset, useDefaultDragImage);
                DragDropHelper.SetDragImage(_dataObject, _lastGiveFeedbacEventArgs);
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
            GiveFeedbackEventArgs gfbevent = _lastGiveFeedbacEventArgs is null
                ? new GiveFeedbackEventArgs((DragDropEffects)dwEffect, useDefaultCursors: true)
                : new GiveFeedbackEventArgs(
                    (DragDropEffects)dwEffect,
                    useDefaultCursors: false,
                    _lastGiveFeedbacEventArgs.DragImage,
                    _lastGiveFeedbacEventArgs.CursorOffset,
                    _lastGiveFeedbacEventArgs.UseDefaultDragImage);

            _peer.OnGiveFeedback(gfbevent);

            if (gfbevent.DragImage is not null && !gfbevent.Equals(_lastGiveFeedbacEventArgs))
            {
                _lastGiveFeedbacEventArgs = gfbevent.Clone();
                UpdateDragImage(_lastGiveFeedbacEventArgs, _dataObject, _lastHwndTarget);
            }

            if (gfbevent.UseDefaultCursors)
            {
                return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
            }

            return HRESULT.S_OK;

            void UpdateDragImage(GiveFeedbackEventArgs e, IComDataObject? dataObject, IntPtr lastHwndTarget)
            {
                if (dataObject is null)
                {
                    return;
                }

                DragDropHelper.SetDragImage(_dataObject, e);

                if (lastHwndTarget != IntPtr.Zero && (Cursor.Position is Point point))
                {
                    DragDropHelper.DragEnter(lastHwndTarget, dataObject, ref point, (Ole32.DROPEFFECT)e.Effect);
                }
            }
        }

        public HRESULT DragEnterTarget(IntPtr hwndTarget)
        {
            _lastHwndTarget = hwndTarget;
            return HRESULT.S_OK;
        }

        public HRESULT DragLeaveTarget()
        {
            if (_lastGiveFeedbacEventArgs?.DragImage is not null)
            {
                DragDropHelper.DragLeave();
            }

            return HRESULT.S_OK;
        }

        unsafe ComInterfaceTable IManagedWrapper.GetComInterfaceTable()
        {
            if (s_comInterfaceTable is null)
            {
                IDropSource.Vtbl* dropSourceVtable = (IDropSource.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDropSource.Vtbl), sizeof(IDropSource.Vtbl));
                IDropSourceNotify.Vtbl* dropSourceNotifyVtable = (IDropSourceNotify.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDropSourceNotify.Vtbl), sizeof(IDropSourceNotify.Vtbl));
                WinFormsComWrappers.PopulateIUnknownVTable((IUnknown.Vtbl*)dropSourceVtable);
                WinFormsComWrappers.PopulateIUnknownVTable((IUnknown.Vtbl*)dropSourceNotifyVtable);

                WinFormsComWrappers.IDropSourceVtbl.PopulateVTable(dropSourceVtable);
                WinFormsComWrappers.IDropSourceNotifyVtbl.PopulateVTable(dropSourceNotifyVtable);

                ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(Ole32.IDropSource), sizeof(ComInterfaceEntry) * 2);
                wrapperEntry[0].IID = *IID.Get<IDropSource>();
                wrapperEntry[0].Vtable = (nint)(void*)dropSourceVtable;
                wrapperEntry[1].IID = *IID.Get<IDropSourceNotify>();
                wrapperEntry[1].Vtable = (nint)(void*)dropSourceNotifyVtable;

                s_comInterfaceTable = new ComInterfaceTable()
                {
                    Entries = wrapperEntry,
                    Count = 2
                };
            }

            return (ComInterfaceTable)s_comInterfaceTable;
        }
    }
}
