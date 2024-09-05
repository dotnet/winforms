// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class MenuTimer
{
    private readonly Timer _autoMenuExpandTimer = new();
    private ToolStripMenuItem? _fromItem;
    private bool _inTransition;

    private readonly int _quickShow = 1;

    private readonly int _slowShow;

    public MenuTimer()
    {
        // MenuShowDelay can be set to 0. In this case, set to something low so it's imperceptible.
        _autoMenuExpandTimer.Tick += OnTick;

        // since MenuShowDelay is registry tweakable we've gotta make sure we've got some sort
        // of interval
        _slowShow = Math.Max(_quickShow, SystemInformation.MenuShowDelay);
    }

    // The current item to autoexpand.
    private ToolStripMenuItem? CurrentItem { get; set; }

    public bool InTransition
    {
        get { return _inTransition; }
        set { _inTransition = value; }
    }

    public void Start(ToolStripMenuItem item)
    {
        if (InTransition)
        {
            return;
        }

        StartCore(item);
    }

    private void StartCore(ToolStripMenuItem? item)
    {
        if (item != CurrentItem)
        {
            Cancel(CurrentItem);
        }

        CurrentItem = item;
        if (item is not null)
        {
            CurrentItem = item;
            _autoMenuExpandTimer.Interval = item.IsOnDropDown ? _slowShow : _quickShow;
            _autoMenuExpandTimer.Enabled = true;
        }
    }

    public void Transition(ToolStripMenuItem fromItem, ToolStripMenuItem? toItem)
    {
        if (toItem is null && InTransition)
        {
            Cancel();

            // In this case we're likely to have hit an item that's not a menu item or nothing is selected.
            EndTransition(forceClose: true);
            return;
        }

        if (_fromItem != fromItem)
        {
            _fromItem = fromItem;
            CancelCore();
            StartCore(toItem);
        }

        // Set up the current item to be the toItem so it will be auto expanded when complete.
        CurrentItem = toItem;
        InTransition = true;
    }

    public void Cancel()
    {
        if (InTransition)
        {
            return;
        }

        CancelCore();
    }

    /// <summary> cancels if and only if this item was the one that
    ///  requested the timer
    /// </summary>
    public void Cancel(ToolStripMenuItem? item)
    {
        if (InTransition)
        {
            return;
        }

        if (item == CurrentItem)
        {
            CancelCore();
        }
    }

    private void CancelCore()
    {
        _autoMenuExpandTimer.Enabled = false;
        CurrentItem = null;
    }

    private void EndTransition(bool forceClose)
    {
        ToolStripMenuItem? lastSelected = _fromItem;
        _fromItem = null; // immediately clear BEFORE we call user code.
        if (InTransition)
        {
            InTransition = false;

            // we should rollup if the current item has changed and is selected.
            bool rollup = forceClose || (CurrentItem is not null && CurrentItem != lastSelected && CurrentItem.Selected);
            if (rollup && lastSelected is not null && lastSelected.HasDropDownItems)
            {
                lastSelected.HideDropDown();
            }
        }
    }

    internal void HandleToolStripMouseLeave(ToolStrip toolStrip)
    {
        if (InTransition && toolStrip == _fromItem?.ParentInternal)
        {
            // restore the selection back to CurrentItem.
            // we're about to fall off the edge of the toolstrip, something should be selected
            // at all times while we're InTransition mode - otherwise it looks really funny
            // to have an auto expanded item
            CurrentItem?.Select();
        }
        else
        {
            // because we've split up selected/pressed, we need to make sure
            // that onmouseleave we make sure there's a selected menu item.
            if (toolStrip.IsDropDown && toolStrip.ActiveDropDowns.Count > 0)
            {
                ToolStripMenuItem? menuItem = toolStrip.ActiveDropDowns[0].OwnerItem as ToolStripMenuItem;
                if (menuItem is not null && menuItem.Pressed)
                {
                    menuItem.Select();
                }
            }
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _autoMenuExpandTimer.Enabled = false;

        if (CurrentItem is null)
        {
            return;
        }

        EndTransition(forceClose: false);
        if (CurrentItem is not null && !CurrentItem.IsDisposed && CurrentItem.Selected && CurrentItem.Enabled && ToolStripManager.ModalMenuFilter.InMenuMode)
        {
            CurrentItem.OnMenuAutoExpand();
        }
    }
}
