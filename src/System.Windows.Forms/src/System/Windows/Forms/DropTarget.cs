// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal class DropTarget : Ole32.IDropTarget
    {
        private IDataObject? _lastDataObject;
        private DragDropEffects _lastEffect = DragDropEffects.None;
        private DragEventArgs? _lastDragEventArgs;
        private readonly IntPtr _hwndTarget;
        private readonly IDropTarget _owner;

        public DropTarget(IDropTarget owner)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget created");
            _owner = owner.OrThrowIfNull();

            if (_owner is Control control && control.IsHandleCreated)
            {
                _hwndTarget = control.Handle;
            }
            else if (_owner is ToolStripDropTargetManager toolStripTargetManager
                && toolStripTargetManager?.Owner is ToolStrip toolStrip
                && toolStrip.IsHandleCreated)
            {
                _hwndTarget = toolStrip.Handle;
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

        private DragEventArgs? CreateDragEventArgs(object? pDataObj, uint grfKeyState, Point pt, uint pdwEffect)
        {
            IDataObject? data;

            if (pDataObj is null)
            {
                data = _lastDataObject;
            }
            else
            {
                if (pDataObj is IDataObject dataObject)
                {
                    data = dataObject;
                }
                else if (pDataObj is IComDataObject)
                {
                    data = new DataObject(pDataObj);
                }
                else
                {
                    return null; // Unknown data object interface; we can't work with this so return null
                }
            }

            DragEventArgs drgevent = _lastDragEventArgs is null
                ? new DragEventArgs(data, (int)grfKeyState, pt.X, pt.Y, (DragDropEffects)pdwEffect, _lastEffect)
                : new DragEventArgs(
                    data,
                    (int)grfKeyState,
                    pt.X,
                    pt.Y,
                    (DragDropEffects)pdwEffect,
                    _lastEffect,
                    _lastDragEventArgs.DropImageType,
                    _lastDragEventArgs.Message ?? string.Empty,
                    _lastDragEventArgs.MessageReplacementToken ?? string.Empty);

            _lastDataObject = data;
            return drgevent;
        }

        HRESULT Ole32.IDropTarget.DragEnter(object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragEnter received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            Debug.Assert(pDataObj is not null, "OleDragEnter didn't give us a valid data object.");
            DragEventArgs? drgevent = CreateDragEventArgs(pDataObj, grfKeyState, pt, pdwEffect);

            if (drgevent is not null)
            {
                _owner.OnDragEnter(drgevent);
                pdwEffect = (uint)drgevent.Effect;
                _lastEffect = drgevent.Effect;

                if (drgevent.DropImageType > DropImageType.Invalid)
                {
                    UpdateDropDescription(drgevent);
                    DragDropHelper.DragEnter(_hwndTarget, drgevent);
                }
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT Ole32.IDropTarget.DragOver(uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragOver received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            DragEventArgs? drgevent = CreateDragEventArgs(null, grfKeyState, pt, pdwEffect);
            if (drgevent is not null)
            {
                _owner.OnDragOver(drgevent);
                pdwEffect = (uint)drgevent.Effect;
                _lastEffect = drgevent.Effect;

                if (drgevent.DropImageType > DropImageType.Invalid)
                {
                    UpdateDropDescription(drgevent);
                    DragDropHelper.DragOver(drgevent);
                }
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT Ole32.IDropTarget.DragLeave()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragLeave received");
            _owner.OnDragLeave(EventArgs.Empty);

            if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
            {
                ClearDropDescription();
                DragDropHelper.DragLeave();
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT Ole32.IDropTarget.Drop(object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDrop received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            DragEventArgs? drgevent = CreateDragEventArgs(pDataObj, grfKeyState, pt, pdwEffect);

            if (drgevent is not null)
            {
                _owner.OnDragDrop(drgevent);
                pdwEffect = (uint)drgevent.Effect;

                if (_lastDragEventArgs?.DropImageType > DropImageType.Invalid)
                {
                    ClearDropDescription();
                    DragDropHelper.Drop(drgevent);
                }
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            _lastEffect = DragDropEffects.None;
            _lastDataObject = null;
            return HRESULT.Values.S_OK;
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
}
