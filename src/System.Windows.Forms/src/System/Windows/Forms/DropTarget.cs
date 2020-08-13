// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal class DropTarget : Ole32.IDropTarget
    {
        private IDataObject lastDataObject;
        private DragDropEffects lastEffect = DragDropEffects.None;
        private readonly IDropTarget owner;

        public DropTarget(IDropTarget owner)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget created");
            this.owner = owner;
        }

#if DEBUG
        ~DropTarget()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget destroyed");
        }
#endif

        private DragEventArgs CreateDragEventArgs(object pDataObj, uint grfKeyState, Point pt, uint pdwEffect)
        {
            IDataObject data = null;

            if (pDataObj is null)
            {
                data = lastDataObject;
            }
            else
            {
                if (pDataObj is IDataObject)
                {
                    data = (IDataObject)pDataObj;
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

            DragEventArgs drgevent = new DragEventArgs(data, (int)grfKeyState, pt.X, pt.Y, (DragDropEffects)pdwEffect, lastEffect);
            lastDataObject = data;
            return drgevent;
        }

        HRESULT Ole32.IDropTarget.DragEnter(object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragEnter received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            Debug.Assert(pDataObj != null, "OleDragEnter didn't give us a valid data object.");
            DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, pt, pdwEffect);

            if (drgevent != null)
            {
                owner.OnDragEnter(drgevent);
                pdwEffect = (uint)drgevent.Effect;
                lastEffect = drgevent.Effect;
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IDropTarget.DragOver(uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragOver received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            DragEventArgs drgevent = CreateDragEventArgs(null, grfKeyState, pt, pdwEffect);
            owner.OnDragOver(drgevent);
            pdwEffect = (uint)drgevent.Effect;
            lastEffect = drgevent.Effect;
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IDropTarget.DragLeave()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragLeave received");
            owner.OnDragLeave(EventArgs.Empty);
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IDropTarget.Drop(object pDataObj, uint grfKeyState, Point pt, ref uint pdwEffect)
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDrop received");
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (pt.X) + "," + (pt.Y));
            DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, pt, pdwEffect);

            if (drgevent != null)
            {
                owner.OnDragDrop(drgevent);
                pdwEffect = (uint)drgevent.Effect;
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            lastEffect = DragDropEffects.None;
            lastDataObject = null;
            return HRESULT.S_OK;
        }
    }
}
