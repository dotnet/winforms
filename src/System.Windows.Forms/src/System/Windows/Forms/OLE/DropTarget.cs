// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.SystemServices;
using Com = Windows.Win32.System.Com;
using DROPEFFECT = Windows.Win32.System.Ole.DROPEFFECT;
using OleIDropTarget = Windows.Win32.System.Ole.IDropTarget;

namespace System.Windows.Forms;

internal unsafe class DropTarget : OleIDropTarget.Interface, IManagedWrapper<OleIDropTarget>
{
    private DataObject? _lastDataObject;
    private DragDropEffects _lastEffect = DragDropEffects.None;
    private DragEventArgs? _lastDragEventArgs;
    private readonly HWND _hwndTarget;
    private readonly IDropTarget _owner;

    public DropTarget(IDropTarget owner)
    {
        _owner = owner.OrThrowIfNull();

        if (_owner is Control control && control.IsHandleCreated)
        {
            _hwndTarget = control.HWND;
        }
        else if (_owner is ToolStripDropTargetManager toolStripTargetManager
            && toolStripTargetManager?.Owner is ToolStrip toolStrip
            && toolStrip.IsHandleCreated)
        {
            _hwndTarget = toolStrip.HWND;
        }
    }

    private void ClearDropDescription()
    {
        _lastDragEventArgs = null;
        DragDropHelper.ClearDropDescription(_lastDataObject);
    }

    /// <summary>
    ///  Creates <see cref="IDataObject"/> to be passed out as data for drag/drop operation
    ///  <paramref name="nativeDataObject"/> should have associated ComWrappers created wrapper that implements
    ///  <see cref="IDataObject"/> to be passed out as is. Otherwise, the data will be wrapped in a <see cref="DataObject"/>.
    /// </summary>
    private static DataObject? CreateManagedDataObjectForOutgoingDropData(Com.IDataObject* nativeDataObject)
    {
        DataObject? dataObject = null;

        using var unknown = ComScope<IUnknown>.QueryFrom(nativeDataObject);

        if (ComWrappers.TryGetObject(unknown, out object? obj) && obj is IDataObject iDataObject)
        {
            // If the original data object implemented IDataObject, we might've wrapped it.
            // We need to give the original back out.

            dataObject = iDataObject as DataObject;

            if (dataObject is not null
                && dataObject.TryUnwrapUserDataObject(out IDataObject? originalIDataObject)
                && originalIDataObject is DataObject originalDataObject)
            {
                dataObject = originalDataObject;
            }
        }

        return dataObject ?? new DataObject(nativeDataObject);
    }

    private DragEventArgs? CreateDragEventArgs(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT pdwEffect)
    {
        DataObject? data;

        if (pDataObj is null)
        {
            data = _lastDataObject;
        }
        else
        {
            data = CreateManagedDataObjectForOutgoingDropData(pDataObj);
            if (data is null)
            {
                return null;
            }
        }

        DragEventArgs dragEvent = _lastDragEventArgs is null
            ? new DragEventArgs(data, (int)grfKeyState, pt.x, pt.y, (DragDropEffects)pdwEffect, _lastEffect)
            : new DragEventArgs(
                data,
                (int)grfKeyState,
                pt.x,
                pt.y,
                (DragDropEffects)pdwEffect,
                _lastEffect,
                _lastDragEventArgs.DropImageType,
                _lastDragEventArgs.Message ?? string.Empty,
                _lastDragEventArgs.MessageReplacementToken ?? string.Empty);

        _lastDataObject = data;
        return dragEvent;
    }

    HRESULT OleIDropTarget.Interface.DragEnter(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        Debug.Assert(pDataObj is not null, "OleDragEnter didn't give us a valid data object.");

        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(pDataObj, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            _owner.OnDragEnter(dragEvent);
            *pdwEffect = (DROPEFFECT)dragEvent.Effect;
            _lastEffect = dragEvent.Effect;

            if (dragEvent.DropImageType > DropImageType.Invalid)
            {
                UpdateDropDescription(dragEvent);
                DragDropHelper.DragEnter(_hwndTarget, dragEvent);
            }
        }
        else
        {
            *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        }

        return HRESULT.S_OK;
    }

    HRESULT OleIDropTarget.Interface.DragOver(MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(null, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            _owner.OnDragOver(dragEvent);
            *pdwEffect = (DROPEFFECT)dragEvent.Effect;
            _lastEffect = dragEvent.Effect;

            if (dragEvent.DropImageType > DropImageType.Invalid)
            {
                UpdateDropDescription(dragEvent);
                DragDropHelper.DragOver(dragEvent);
            }
        }
        else
        {
            *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        }

        return HRESULT.S_OK;
    }

    HRESULT OleIDropTarget.Interface.DragLeave()
    {
        if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
        {
            ClearDropDescription();
            DragDropHelper.DragLeave();
        }

        _owner.OnDragLeave(EventArgs.Empty);

        return HRESULT.S_OK;
    }

    HRESULT OleIDropTarget.Interface.Drop(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(pDataObj, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
            {
                ClearDropDescription();
                DragDropHelper.Drop(dragEvent);
            }

            _owner.OnDragDrop(dragEvent);
            *pdwEffect = (DROPEFFECT)dragEvent.Effect;
        }
        else
        {
            *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        }

        _lastEffect = DragDropEffects.None;
        _lastDataObject = null;
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
