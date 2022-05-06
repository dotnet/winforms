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
        private DropImageType _lastDropImageType = DropImageType.Invalid;
        private string _lastMessage = string.Empty;
        private string _lastMessageReplacementToken = string.Empty;
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
                && toolStripTargetManager.Owner is not null
                && toolStripTargetManager.Owner is ToolStrip toolStrip
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

            DragEventArgs drgevent = new DragEventArgs(data, (int)grfKeyState, pt.X, pt.Y, (DragDropEffects)pdwEffect, _lastEffect, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
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

                if (drgevent.DropImageType > DropImageType.Invalid && drgevent.Data is IComDataObject comDataObject && _hwndTarget != IntPtr.Zero)
                {
                    DropImageType dropImageType = Enum.IsDefined(drgevent.DropImageType) ? drgevent.DropImageType : DropImageType.Invalid;
                    string message = drgevent.Message is not null ? drgevent.Message : string.Empty;
                    string messageReplacementToken = drgevent.MessageReplacementToken is not null ? drgevent.MessageReplacementToken : string.Empty;

                    if (!dropImageType.Equals(_lastDropImageType) || !message.Equals(_lastMessage) || !messageReplacementToken.Equals(_lastMessageReplacementToken))
                    {
                        _lastDropImageType = !dropImageType.Equals(_lastDropImageType) ? dropImageType : _lastDropImageType;
                        _lastMessage = !message.Equals(_lastMessage) ? message : _lastMessage;
                        _lastMessageReplacementToken = messageReplacementToken.Equals(_lastMessageReplacementToken) ? messageReplacementToken : _lastMessageReplacementToken;
                        DragDropHelper.SetDropDescription(comDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                    }

                    DragDropHelper.DragEnter(_hwndTarget, comDataObject, ref pt, pdwEffect);
                }
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
            DragEventArgs? drgevent = CreateDragEventArgs(null, grfKeyState, pt, pdwEffect);
            if (drgevent is not null)
            {
                _owner.OnDragOver(drgevent);
                pdwEffect = (uint)drgevent.Effect;
                _lastEffect = drgevent.Effect;

                if (drgevent.DropImageType > DropImageType.Invalid && drgevent.Data is IComDataObject comDataObject && _hwndTarget != IntPtr.Zero)
                {
                    DropImageType dropImageType = Enum.IsDefined(drgevent.DropImageType) ? drgevent.DropImageType : DropImageType.Invalid;
                    string message = drgevent.Message is not null ? drgevent.Message : string.Empty;
                    string messageReplacementToken = drgevent.MessageReplacementToken is not null ? drgevent.MessageReplacementToken : string.Empty;

                    if (!dropImageType.Equals(_lastDropImageType) || !message.Equals(_lastMessage) || !messageReplacementToken.Equals(_lastMessageReplacementToken))
                    {
                        _lastDropImageType = !dropImageType.Equals(_lastDropImageType) ? dropImageType : _lastDropImageType;
                        _lastMessage = !message.Equals(_lastMessage) ? message : _lastMessage;
                        _lastMessageReplacementToken = messageReplacementToken.Equals(_lastMessageReplacementToken) ? messageReplacementToken : _lastMessageReplacementToken;
                        DragDropHelper.SetDropDescription(comDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                    }

                    DragDropHelper.DragOver(ref pt, pdwEffect);
                }
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IDropTarget.DragLeave()
        {
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "OleDragLeave received");
            _owner.OnDragLeave(EventArgs.Empty);

            if (_lastDropImageType > DropImageType.Invalid && _lastDataObject is IComDataObject comDataObject)
            {
                if (!_lastDropImageType.Equals(DropImageType.Invalid) || !_lastMessage.Equals(string.Empty) || !_lastMessageReplacementToken.Equals(string.Empty))
                {
                    _lastDropImageType = !_lastDropImageType.Equals(DropImageType.Invalid) ? DropImageType.Invalid : _lastDropImageType;
                    _lastMessage = !_lastMessage.Equals(string.Empty) ? string.Empty : _lastMessage;
                    _lastMessageReplacementToken = !_lastMessageReplacementToken.Equals(string.Empty) ? string.Empty : _lastMessageReplacementToken;
                    DragDropHelper.SetDropDescription(comDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                }

                DragDropHelper.DragLeave();
            }

            return HRESULT.S_OK;
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

                if (_lastDropImageType > DropImageType.Invalid && drgevent.Data is IComDataObject comDataObject)
                {
                    if (!_lastDropImageType.Equals(DropImageType.Invalid) || !_lastMessage.Equals(string.Empty) || !_lastMessageReplacementToken.Equals(string.Empty))
                    {
                        _lastDropImageType = !_lastDropImageType.Equals(DropImageType.Invalid) ? DropImageType.Invalid : _lastDropImageType;
                        _lastMessage = !_lastMessage.Equals(string.Empty) ? string.Empty : _lastMessage;
                        _lastMessageReplacementToken = !_lastMessageReplacementToken.Equals(string.Empty) ? string.Empty : _lastMessageReplacementToken;
                        DragDropHelper.SetDropDescription(comDataObject, _lastDropImageType, _lastMessage, _lastMessageReplacementToken);
                    }

                    DragDropHelper.Drop(comDataObject, ref pt, pdwEffect);
                }
            }
            else
            {
                pdwEffect = (uint)DragDropEffects.None;
            }

            _lastEffect = DragDropEffects.None;
            _lastDataObject = null;
            return HRESULT.S_OK;
        }
    }
}
