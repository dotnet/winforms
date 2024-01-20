// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.System.Com;
using Windows.Win32.System.SystemServices;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
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
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget created");
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

#if DEBUG
    ~DropTarget()
    {
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget destroyed");
    }
#endif

    private void ClearDropDescription()
    {
        _lastDragEventArgs = null;
        DragDropHelper.ClearDropDescription(_lastDataObject);
    }

    /// <summary>
    ///  Prepares the drag data to be passed out.
    ///  <paramref name="dataObj"/> should implement <see cref="IDataObject"/> to be passed out as is.
    ///  Otherwise, the data will be wrapped in a <see cref="DataObject"/>.
    /// </summary>
    private static IDataObject? PrepareOutgoingDropData(object dataObj)
    {
        if (dataObj is IDataObject dataObject)
        {
            return dataObject;
        }
        else if (dataObj is ComTypes.IDataObject nativeDataObject)
        {
            return new DataObject(nativeDataObject);
        }

        return null; // Unknown data object interface; we can't work with this so return null
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
            data = PrepareOutgoingDropData(ComHelpers.GetObjectForIUnknown((IUnknown*)pDataObj));
            if (data is null)
            {
                return null;
            }
        }

        DragEventArgs drgevent = _lastDragEventArgs is null
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
        return drgevent;
    }

    HRESULT Ole.IDropTarget.Interface.DragEnter(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT* pdwEffect)
    {
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragEnter received");
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"\t{pt.x},{pt.y}");
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
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragOver received");
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"\t{pt.x},{pt.y}");

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
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragLeave received");
        _owner.OnDragLeave(EventArgs.Empty);

        if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
        {
            ClearDropDescription();
            DragDropHelper.DragLeave();
        }

        return HRESULT.S_OK;
    }

    HRESULT Ole.IDropTarget.Interface.Drop(Com.IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, Ole.DROPEFFECT* pdwEffect)
    {
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDrop received");
        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"\t{pt.x},{pt.y}");

        if (pdwEffect is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (CreateDragEventArgs(pDataObj, grfKeyState, pt, *pdwEffect) is { } dragEvent)
        {
            _owner.OnDragDrop(dragEvent);
            *pdwEffect = (Ole.DROPEFFECT)dragEvent.Effect;

            if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
            {
                ClearDropDescription();
                DragDropHelper.Drop(dragEvent);
            }
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
