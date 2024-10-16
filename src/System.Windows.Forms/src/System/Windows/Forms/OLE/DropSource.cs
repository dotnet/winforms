// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.SystemServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms;

internal class DropSource : IDropSource.Interface, IDropSourceNotify.Interface, IManagedWrapper<IDropSource, IDropSourceNotify>
{
    private readonly ISupportOleDropSource _peer;
    private readonly IComDataObject _dataObject;
    private HWND _lastHwndTarget;
    private uint _lastHwndTargetThreadId;
    private GiveFeedbackEventArgs? _lastGiveFeedbackEventArgs;

    public DropSource(ISupportOleDropSource peer, IComDataObject dataObject, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
    {
        _peer = peer.OrThrowIfNull();
        _dataObject = dataObject.OrThrowIfNull();

        if (dragImage is not null)
        {
            _lastGiveFeedbackEventArgs = new(DragDropEffects.None, useDefaultCursors: false, dragImage, cursorOffset, useDefaultDragImage);
            DragDropHelper.SetDragImage(_dataObject, _lastGiveFeedbackEventArgs);
        }
    }

    public HRESULT QueryContinueDrag(BOOL fEscapePressed, MODIFIERKEYS_FLAGS grfKeyState)
    {
        DragAction action = DragAction.Continue;
        if (fEscapePressed)
        {
            action = DragAction.Cancel;
        }
        else if (
            !grfKeyState.HasFlag(MODIFIERKEYS_FLAGS.MK_LBUTTON)
            && !grfKeyState.HasFlag(MODIFIERKEYS_FLAGS.MK_RBUTTON)
            && !grfKeyState.HasFlag(MODIFIERKEYS_FLAGS.MK_MBUTTON))
        {
            action = DragAction.Drop;
        }

        QueryContinueDragEventArgs qcdEvent = new((int)grfKeyState, fEscapePressed, action);
        _peer.OnQueryContinueDrag(qcdEvent);

        return qcdEvent.Action switch
        {
            DragAction.Drop => HRESULT.DRAGDROP_S_DROP,
            DragAction.Cancel => HRESULT.DRAGDROP_S_CANCEL,
            _ => HRESULT.S_OK,
        };
    }

    public HRESULT GiveFeedback(DROPEFFECT dwEffect)
    {
        GiveFeedbackEventArgs gfbEvent = _lastGiveFeedbackEventArgs is null
            ? new((DragDropEffects)dwEffect, useDefaultCursors: true)
            : new(
                (DragDropEffects)dwEffect,
                useDefaultCursors: false,
                _lastGiveFeedbackEventArgs.DragImage,
                _lastGiveFeedbackEventArgs.CursorOffset,
                _lastGiveFeedbackEventArgs.UseDefaultDragImage);

        _peer.OnGiveFeedback(gfbEvent);

        if (IsDropTargetWindowInCurrentThread() && gfbEvent.DragImage is not null && !gfbEvent.Equals(_lastGiveFeedbackEventArgs))
        {
            _lastGiveFeedbackEventArgs = gfbEvent.Clone();
            UpdateDragImage(_lastGiveFeedbackEventArgs, _dataObject, _lastHwndTarget);
        }

        if (gfbEvent.UseDefaultCursors)
        {
            return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        return HRESULT.S_OK;

        void UpdateDragImage(GiveFeedbackEventArgs e, IComDataObject? dataObject, HWND lastHwndTarget)
        {
            if (dataObject is null)
            {
                return;
            }

            DragDropHelper.SetDragImage(_dataObject, e);

            if (!lastHwndTarget.IsNull && (Cursor.Position is Point point))
            {
                DragDropHelper.DragEnter(lastHwndTarget, dataObject, ref point, (DROPEFFECT)e.Effect);
            }
        }
    }

    public unsafe HRESULT DragEnterTarget(HWND hwndTarget)
    {
        _lastHwndTarget = hwndTarget;
        _lastHwndTargetThreadId = PInvoke.GetWindowThreadProcessId(hwndTarget, lpdwProcessId: null);
        return HRESULT.S_OK;
    }

    public HRESULT DragLeaveTarget()
    {
        if (IsDropTargetWindowInCurrentThread() && _lastGiveFeedbackEventArgs?.DragImage is not null)
        {
            DragDropHelper.DragLeave();
        }

        _lastHwndTarget = default;
        _lastHwndTargetThreadId = default;
        return HRESULT.S_OK;
    }

    private bool IsDropTargetWindowInCurrentThread() => _lastHwndTargetThreadId == PInvokeCore.GetCurrentThreadId();
}
