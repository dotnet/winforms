// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.SystemServices;
using Com = Windows.Win32.System.Com;
using Ole = Windows.Win32.System.Ole;

namespace System.Windows.Forms;

internal unsafe class DropTarget : Ole.IDropTarget.Interface, IManagedWrapper<Ole.IDropTarget>
{
    private IDataObject? _lastDataObject;
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
    private static IDataObject? CreateWinFormsDataObjectForOutgoingDropData(Com.IDataObject* nativeDataObject)
    {
        using var unknown = ComScope<IUnknown>.QueryFrom(nativeDataObject);
        if (ComWrappers.TryGetObject(unknown, out object? obj) && obj is IDataObject dataObject)
        {
            // If the original data object implemented IDataObject, we might've wrapped it. We need to give the original back out.
            return dataObject is DataObject winFormsDataObject && winFormsDataObject.OriginalIDataObject is { } originalDataObject
                ? originalDataObject
                : dataObject;
        }

        return new DataObject(nativeDataObject);
    }

    private DragEventArgs? CreateDragEventArgs(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT pdwEffect)
    {
        IDataObject? data;

        if (pDataObj is null)
        {
            data = _lastDataObject;
        }
        else
        {
            data = CreateWinFormsDataObjectForOutgoingDropData(pDataObj);
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

    HRESULT Ole.IDropTarget.Interface.DragEnter(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT* pdwEffect)
    {
        Debug.Assert(pDataObj is not null, "OleDragEnter didn't give us a valid data object.");

        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(pDataObj, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            _owner.OnDragEnter(dragEvent);
            *pdwEffect = (Ole.DROPEFFECT)dragEvent.Effect;
            _lastEffect = dragEvent.Effect;

            if (dragEvent.DropImageType > DropImageType.Invalid)
            {
                UpdateDropDescription(dragEvent);
                DragDropHelper.DragEnter(_hwndTarget, dragEvent);
            }
        }
        else
        {
            *pdwEffect = Ole.DROPEFFECT.DROPEFFECT_NONE;
        }

        return HRESULT.S_OK;
    }

    HRESULT Ole.IDropTarget.Interface.DragOver(MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT* pdwEffect)
    {
        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(null, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            _owner.OnDragOver(dragEvent);
            *pdwEffect = (Ole.DROPEFFECT)dragEvent.Effect;
            _lastEffect = dragEvent.Effect;

            if (dragEvent.DropImageType > DropImageType.Invalid)
            {
                UpdateDropDescription(dragEvent);
                DragDropHelper.DragOver(dragEvent);
            }
        }
        else
        {
            *pdwEffect = Ole.DROPEFFECT.DROPEFFECT_NONE;
        }

        return HRESULT.S_OK;
    }

    HRESULT Ole.IDropTarget.Interface.DragLeave()
    {
        if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
        {
            ClearDropDescription();
            DragDropHelper.DragLeave();
        }

        _owner.OnDragLeave(EventArgs.Empty);

        return HRESULT.S_OK;
    }

    HRESULT Ole.IDropTarget.Interface.Drop(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT* pdwEffect)
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
            *pdwEffect = (Ole.DROPEFFECT)dragEvent.Effect;
        }
        else
        {
            *pdwEffect = Ole.DROPEFFECT.DROPEFFECT_NONE;
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
