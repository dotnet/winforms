// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

    public ToolStrip Owner => _owner;

    public ToolStripDropTargetManager(ToolStrip owner)
    {
        _owner = owner.OrThrowIfNull();
    }

    public void EnsureRegistered()
    {
        SetAcceptDrops(true);
    }

    public void EnsureUnRegistered()
    {
        for (int i = 0; i < _owner.Items.Count; i++)
        {
            if (_owner.Items[i].AllowDrop)
            {
                // Can't unregister this as a drop target unless everyone is done.
                return;
            }
        }

        if (_owner.AllowDrop || _owner.AllowItemReorder)
        {
            // Can't unregister this as a drop target if ToolStrip is still accepting drops.
            return;
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
        // If we are supporting Item Reordering
        // and this is a ToolStripItem - snitch it.
        if (_owner.AllowItemReorder && e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            _lastDropTarget = _owner.ItemReorderDropTarget;
        }
        else
        {
            ToolStripItem? item = FindItemAtPoint(e.X, e.Y);

            if ((item is not null) && (item.AllowDrop))
            {
                // The item wants this event.
                _lastDropTarget = item;
            }
            else if (_owner.AllowDrop)
            {
                // The ToolStrip wants this event.
                _lastDropTarget = _owner;
            }
            else
            {
                // There could be one item that says "AllowDrop == true" which would turn
                // on this drop target manager. If we're not over that particular item - then
                // just null out the last drop target manager.

                // The other valid reason for being here is that we've done an AllowItemReorder
                // and we don't have a ToolStripItem contain within the data (say someone drags a link
                // from IE over the ToolStrip).

                _lastDropTarget = null;
            }
        }

        _lastDropTarget?.OnDragEnter(e);
    }

    public void OnDragOver(DragEventArgs e)
    {
        IDropTarget? newDropTarget;

        // If we are supporting Item Reordering
        // and this is a ToolStripItem - snitch it.
        if (_owner.AllowItemReorder && e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            newDropTarget = _owner.ItemReorderDropTarget;
        }
        else
        {
            ToolStripItem? item = FindItemAtPoint(e.X, e.Y);

            if ((item is not null) && (item.AllowDrop))
            {
                // The item wants this event.
                newDropTarget = item;
            }
            else if (_owner.AllowDrop)
            {
                // The ToolStrip wants this event.
                newDropTarget = _owner;
            }
            else
            {
                newDropTarget = null;
            }
        }

        // If we've switched drop targets then we need to create drag enter and leave events.
        if (newDropTarget != _lastDropTarget)
        {
            UpdateDropTarget(newDropTarget, e);
        }

        _lastDropTarget?.OnDragOver(e);
    }

    public void OnDragLeave(EventArgs e)
    {
        _lastDropTarget?.OnDragLeave(e);
        _lastDropTarget = null;
    }

    public void OnDragDrop(DragEventArgs e)
    {
        if (_lastDropTarget is not null)
        {
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

            // Register
            HRESULT hr = PInvoke.RegisterDragDrop(_owner, new DropTarget(this));
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
                // tell the drag image manager you've left
                if (e.DropImageType > DropImageType.Invalid)
                {
                    DragDropHelper.ClearDropDescription(e.Data);
                    DragDropHelper.DragLeave();
                }

                OnDragLeave(EventArgs.Empty);
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
