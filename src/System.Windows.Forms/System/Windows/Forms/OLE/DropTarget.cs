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

        // Some drag sources only support async operations. Notably, Chromium-based applications with file drop (the
        // new Outlook is one example). The async interface is primarily a feature check and ref counting mechanism.
        // To enable applications to accept filenames from these sources we use the interface when available and just
        // do the operation synchronously. When we add new async API we would defer to the async interface.
        //
        // While initial investigations show that this is not a problem, we'll still provide a way to opt out should
        // this prove blocking for some unknown scenario.
        //
        // https://learn.microsoft.com/windows/win32/shell/datascenarios#dragging-and-dropping-shell-objects-asynchronously

        IDataObjectAsyncCapability* asyncCapability = null;
        HRESULT result = HRESULT.S_OK;

        bool enableSyncOverAsync = !CoreAppContextSwitches.DragDropDisableSyncOverAsync;
#pragma warning disable WFO5003 // Type is for evaluation purposes only
        IAsyncDropTarget? asyncDropTarget = _owner as IAsyncDropTarget;
#pragma warning restore WFO5003
        if (asyncDropTarget is not null || enableSyncOverAsync)
        {
            result = pDataObj->QueryInterface(out asyncCapability);
            if (result.Succeeded
                && asyncCapability is not null
                && asyncCapability->GetAsyncMode(out BOOL isAsync).Succeeded
                && isAsync)
            {
                result = asyncCapability->StartOperation();
                if (result.Failed)
                {
                    return result;
                }
            }
        }

        *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;

        try
        {
            if (CreateDragEventArgs(pDataObj, grfKeyState, pt, *pdwEffect) is { } dragEvent)
            {
                if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
                {
                    ClearDropDescription();
                    DragDropHelper.Drop(dragEvent);
                }

                result = HandleOnDragDrop(dragEvent, asyncCapability, pdwEffect);
                asyncCapability = null;
            }

            _lastEffect = DragDropEffects.None;
            _lastDataObject = null;
        }
        finally
        {
            if (asyncCapability is not null)
            {
                // We weren't successful in completing the operation, so we need to end it with no drop effect.
                // There isn't clear guidance on expected errors here, so we'll just use E_UNEXPECTED.
                result = asyncCapability->EndOperation(HRESULT.E_UNEXPECTED, null, (uint)DROPEFFECT.DROPEFFECT_NONE);
                asyncCapability->Release();
            }
        }

        return result;
    }

    private HRESULT HandleOnDragDrop(DragEventArgs e, IDataObjectAsyncCapability* asyncCapability, DROPEFFECT* pdwEffect)
    {
#pragma warning disable WFO5003 // Type is for evaluation purposes only
        if (asyncCapability is not null && _owner is IAsyncDropTarget asyncDropTarget)
#pragma warning restore WFO5003
        {
            // We have an implemented IAsyncDropTarget and the drag source supports async operations, push to a
            // worker thread to allow the drop to complete without blocking the UI thread.
            Task.Run(() =>
            {
                DROPEFFECT effect = DROPEFFECT.DROPEFFECT_NONE;

                try
                {
                    asyncDropTarget.OnAsyncDragDrop(e);
                    effect = (DROPEFFECT)e.Effect;
                }
                finally
                {
                    HRESULT result = asyncCapability->EndOperation(HRESULT.S_OK, null, (uint)effect);
                    asyncCapability->Release();
                }
            });

            // It isn't clear what we're supposed to do with the effect here as the actual result comes from
            // EndOperation. Perhaps DROPEFFECT_COPY would be a better default?
            *pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
            return HRESULT.S_OK;
        }

        // We don't have the IAsyncDropTarget or the drag source doesn't support async operations, so just call
        // the normal OnDragDrop.

        DROPEFFECT effect = DROPEFFECT.DROPEFFECT_NONE;

        try
        {
            _owner.OnDragDrop(e);
            *pdwEffect = effect = (DROPEFFECT)e.Effect;
        }
        finally
        {
            if (asyncCapability is not null)
            {
                HRESULT result = asyncCapability->EndOperation(HRESULT.S_OK, null, (uint)effect);
                asyncCapability->Release();
            }
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
