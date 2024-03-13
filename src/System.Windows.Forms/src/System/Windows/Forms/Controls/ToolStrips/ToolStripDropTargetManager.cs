// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  RegisterDropTarget requires an HWND to back it's IDropTargets. Since some ToolStripItems
///  do not have HWNDS, this guy's got to figure out who the event was really supposed
///  to go to and pass it on to it.
/// </summary>
internal class ToolStripDropTargetManager : IDropTarget
{
    private IDropTarget? _lastDropTarget;
    private readonly ToolStrip _owner;

#if DEBUG
    private bool _dropTargetIsEntered;
#endif

#if DEBUG
    internal static TraceSwitch DragDropDebug { get; } = new("DragDropDebug", "Debug ToolStrip DragDrop code");
#else
    internal static TraceSwitch? DragDropDebug { get; }
#endif

    public ToolStrip Owner
    {
        get => _owner;
    }

    public ToolStripDropTargetManager(ToolStrip owner)
    {
        _owner = owner.OrThrowIfNull();
    }

    public void EnsureRegistered()
    {
        DragDropDebug.TraceVerbose("Ensuring drop target registered");
        SetAcceptDrops(true);
    }

    public void EnsureUnRegistered()
    {
        DragDropDebug.TraceVerbose("Attempting to unregister droptarget");
        for (int i = 0; i < _owner.Items.Count; i++)
        {
            if (_owner.Items[i].AllowDrop)
            {
                DragDropDebug.TraceVerbose("An item still has allowdrop set to true - can't unregister");
                return; // can't unregister this as a drop target unless everyone is done.
            }
        }

        if (_owner.AllowDrop || _owner.AllowItemReorder)
        {
            DragDropDebug.TraceVerbose("The ToolStrip has AllowDrop or AllowItemReorder set to true - can't unregister");
            return;  // can't unregister this as a drop target if ToolStrip is still accepting drops
        }

        SetAcceptDrops(false);
        _owner.DropTargetManager = null;
    }

    /// <summary>
    ///  Takes a screen point and converts it into an item. May return null.
    /// </summary>
    private ToolStripItem? FindItemAtPoint(int x, int y)
    {
        return _owner.GetItemAt(_owner.PointToClient(new Point(x, y)));
    }

    public void OnDragEnter(DragEventArgs e)
    {
        DragDropDebug.TraceVerbose("[DRAG ENTER] ==============");

        // If we are supporting Item Reordering
        // and this is a ToolStripItem - snitch it.
        if (_owner.AllowItemReorder && e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            DragDropDebug.TraceVerbose("ItemReorderTarget taking this...");
            _lastDropTarget = _owner.ItemReorderDropTarget;
        }
        else
        {
            ToolStripItem? item = FindItemAtPoint(e.X, e.Y);

            if ((item is not null) && (item.AllowDrop))
            {
                // the item wants this event
                DragDropDebug.TraceVerbose($"ToolStripItem taking this: {item}");

                _lastDropTarget = item;
            }
            else if (_owner.AllowDrop)
            {
                // the ToolStrip wants this event
                DragDropDebug.TraceVerbose("ToolStrip taking this because AllowDrop set to true.");
                _lastDropTarget = _owner;
            }
            else
            {
                // There could be one item that says "AllowDrop == true" which would turn
                // on this drop target manager.  If we're not over that particular item - then
                // just null out the last drop target manager.

                // the other valid reason for being here is that we've done an AllowItemReorder
                // and we don't have a ToolStripItem contain within the data (say someone drags a link
                // from IE over the ToolStrip)

                DragDropDebug.TraceVerbose("No one wanted it.");

                _lastDropTarget = null;
            }
        }

        if (_lastDropTarget is not null)
        {
            DragDropDebug.TraceVerbose("Calling OnDragEnter on target...");
#if DEBUG
            _dropTargetIsEntered = true;
#endif
            _lastDropTarget.OnDragEnter(e);
        }
    }

    public void OnDragOver(DragEventArgs e)
    {
        DragDropDebug.TraceVerbose("[DRAG OVER] ==============");

        IDropTarget? newDropTarget = null;

        // If we are supporting Item Reordering
        // and this is a ToolStripItem - snitch it.
        if (_owner.AllowItemReorder && e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            DragDropDebug.TraceVerbose("ItemReorderTarget taking this...");
            newDropTarget = _owner.ItemReorderDropTarget;
        }
        else
        {
            ToolStripItem? item = FindItemAtPoint(e.X, e.Y);

            if ((item is not null) && (item.AllowDrop))
            {
                // the item wants this event
                DragDropDebug.TraceVerbose($"ToolStripItem taking this: {item}");
                newDropTarget = item;
            }
            else if (_owner.AllowDrop)
            {
                // the ToolStrip wants this event
                DragDropDebug.TraceVerbose("ToolStrip taking this because AllowDrop set to true.");
                newDropTarget = _owner;
            }
            else
            {
                DragDropDebug.TraceVerbose("No one wanted it.");
                newDropTarget = null;
            }
        }

        // if we've switched drop targets - then
        // we need to create drag enter and leave events
        if (newDropTarget != _lastDropTarget)
        {
            DragDropDebug.TraceVerbose("NEW DROP TARGET!");
            UpdateDropTarget(newDropTarget, e);
        }

        // now call drag over
        if (_lastDropTarget is not null)
        {
            DragDropDebug.TraceVerbose("Calling OnDragOver on target...");
            _lastDropTarget.OnDragOver(e);
        }
    }

    public void OnDragLeave(EventArgs e)
    {
        DragDropDebug.TraceVerbose("[DRAG LEAVE] ==============");

        if (_lastDropTarget is not null)
        {
            DragDropDebug.TraceVerbose("Calling OnDragLeave on current target...");
#if DEBUG
            _dropTargetIsEntered = false;
#endif
            _lastDropTarget.OnDragLeave(e);
        }
#if DEBUG
        else
        {
            Debug.Assert(!_dropTargetIsEntered, "Why do we have an entered droptarget and NO lastDropTarget?");
        }
#endif
        _lastDropTarget = null;
    }

    public void OnDragDrop(DragEventArgs e)
    {
        DragDropDebug.TraceVerbose("[DRAG DROP] ==============");

        if (_lastDropTarget is not null)
        {
            DragDropDebug.TraceVerbose("Calling OnDragDrop on current target...");

            _lastDropTarget.OnDragDrop(e);
        }
        else
        {
            Debug.Assert(false, "Why is lastDropTarget null?");
        }

        _lastDropTarget = null;
    }

    /// <summary>
    ///  Used to actually register the control as a drop target.
    /// </summary>
    private void SetAcceptDrops(bool accept)
    {
        if (!accept || !_owner.IsHandleCreated)
        {
            return;
        }

        try
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"Registering as drop target: {_owner.Handle}");

            // Register
            HRESULT hr = PInvoke.RegisterDragDrop(_owner, new DropTarget(this));
            Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"   ret:{hr}");
            if (hr.Failed && hr != HRESULT.DRAGDROP_E_ALREADYREGISTERED)
            {
                throw Marshal.GetExceptionForHR((int)hr)!;
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(SR.DragDropRegFailed, e);
        }
    }

    /// <summary>
    ///  If we have a new active item, fire drag leave and enter. This corresponds to the case
    ///  where you are dragging between items and haven't actually left the ToolStrip's client area.
    /// </summary>
    private void UpdateDropTarget(IDropTarget? newTarget, DragEventArgs e)
    {
        if (newTarget != _lastDropTarget)
        {
            // tell the last drag target you've left
            if (_lastDropTarget is not null)
            {
                OnDragLeave(EventArgs.Empty);

                // tell the drag image manager you've left
                if (e.DropImageType > DropImageType.Invalid)
                {
                    DragDropHelper.ClearDropDescription(e.Data);
                    DragDropHelper.DragLeave();
                }
            }

            _lastDropTarget = newTarget;
            if (newTarget is not null)
            {
                DragEventArgs dragEnterArgs = new(e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect, e.DropImageType, e.Message, e.MessageReplacementToken)
                {
                    Effect = DragDropEffects.None,
                    DropImageType = DropImageType.Invalid,
                    Message = string.Empty,
                    MessageReplacementToken = string.Empty
                };

                // tell the next drag target you've entered
                OnDragEnter(dragEnterArgs);

                // tell the drag image manager you've entered
                if (dragEnterArgs.DropImageType > DropImageType.Invalid && _owner is ToolStrip toolStrip && toolStrip.IsHandleCreated)
                {
                    DragDropHelper.SetDropDescription(dragEnterArgs);
                    DragDropHelper.DragEnter(toolStrip.HWND, dragEnterArgs);
                }
            }
        }
    }
}
