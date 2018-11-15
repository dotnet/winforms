// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
        using System;
        using System.Diagnostics;
        using System.Security.Permissions;
        using System.Security;
        using System.ComponentModel;
        
        using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

        /// <devdoc>
        /// </devdoc>
        internal class DropTarget : UnsafeNativeMethods.IOleDropTarget {
            private IDataObject lastDataObject = null;
            private DragDropEffects lastEffect = DragDropEffects.None;
            private IDropTarget owner;

            public DropTarget(IDropTarget owner) {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget created");
                this.owner = owner;
            }

#if DEBUG
            ~DropTarget() {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "DropTarget destroyed");
            }
#endif

            private DragEventArgs CreateDragEventArgs(object pDataObj, int grfKeyState, NativeMethods.POINTL pt, int pdwEffect) {
            
                IDataObject data = null;

                if (pDataObj == null) {
                    data = lastDataObject;
                }
                else {
                    if (pDataObj is IDataObject) {
                        data = (IDataObject)pDataObj;
                    }
                    else if (pDataObj is IComDataObject) {
                        data = new DataObject(pDataObj);
                    }
                    else {
                        return null; // Unknown data object interface; we can't work with this so return null
                    }
                }

                DragEventArgs drgevent = new DragEventArgs(data, grfKeyState, pt.x, pt.y, (DragDropEffects)pdwEffect, lastEffect);
                lastDataObject = data;
                return drgevent;
            }

            int UnsafeNativeMethods.IOleDropTarget.OleDragEnter(object pDataObj, int grfKeyState,
                                                          UnsafeNativeMethods.POINTSTRUCT pt,
                                                          ref int pdwEffect) {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragEnter recieved");
                NativeMethods.POINTL ptl = new NativeMethods.POINTL();
                ptl.x = pt.x;
                ptl.y = pt.y;
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (ptl.x) + "," + (ptl.y));
                Debug.Assert(pDataObj != null, "OleDragEnter didn't give us a valid data object.");
                DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, ptl, pdwEffect);
                
                if (drgevent != null) {
                    owner.OnDragEnter(drgevent);
                    pdwEffect = (int)drgevent.Effect;
                    lastEffect = drgevent.Effect;
                }
                else {
                    pdwEffect = (int)DragDropEffects.None;
                }
                return NativeMethods.S_OK;
            }
            int UnsafeNativeMethods.IOleDropTarget.OleDragOver(int grfKeyState, UnsafeNativeMethods.POINTSTRUCT pt, ref int pdwEffect) {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragOver recieved");
                NativeMethods.POINTL ptl = new NativeMethods.POINTL();
                ptl.x = pt.x;
                ptl.y = pt.y;
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (ptl.x) + "," + (ptl.y));
                DragEventArgs drgevent = CreateDragEventArgs(null, grfKeyState, ptl, pdwEffect);
                owner.OnDragOver(drgevent);
                pdwEffect = (int)drgevent.Effect;
                lastEffect = drgevent.Effect;
                return NativeMethods.S_OK;
            }
            int UnsafeNativeMethods.IOleDropTarget.OleDragLeave() {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragLeave recieved");
                owner.OnDragLeave(EventArgs.Empty);
                return NativeMethods.S_OK;
            }
            int UnsafeNativeMethods.IOleDropTarget.OleDrop(object pDataObj, int grfKeyState, UnsafeNativeMethods.POINTSTRUCT pt, ref int pdwEffect) {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDrop recieved");
                NativeMethods.POINTL ptl = new NativeMethods.POINTL();
                ptl.x = pt.x;
                ptl.y = pt.y;
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "\t" + (ptl.x) + "," + (ptl.y));
                DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, ptl, pdwEffect);
                
                if (drgevent != null) {
                    owner.OnDragDrop(drgevent);
                    pdwEffect = (int)drgevent.Effect;
                }
                else {
                    pdwEffect = (int)DragDropEffects.None;
                }
                
                lastEffect = DragDropEffects.None;
                lastDataObject = null;
                return NativeMethods.S_OK;
            }
        }


}
