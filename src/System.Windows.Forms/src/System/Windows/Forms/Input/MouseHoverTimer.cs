// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class MouseHoverTimer : IDisposable
{
    private readonly Timer _mouseHoverTimer = new();

    // Consider - weak reference?
    private ToolStripItem? _currentItem;

    public MouseHoverTimer()
    {
        _mouseHoverTimer.Interval = SystemInformation.MouseHoverTime;
        _mouseHoverTimer.Tick += OnTick;
    }

    public void Start(ToolStripItem? item)
    {
        if (item != _currentItem)
        {
            Cancel(_currentItem);
        }

        _currentItem = item;
        if (_currentItem is not null)
        {
            _mouseHoverTimer.Enabled = true;
        }
    }

    public void Cancel()
    {
        _mouseHoverTimer.Enabled = false;
        _currentItem = null;
    }

    /// <summary> cancels if and only if this item was the one that
    ///  requested the timer
    /// </summary>
    public void Cancel(ToolStripItem? item)
    {
        if (item == _currentItem)
        {
            Cancel();
        }
    }

    public void Dispose()
    {
        Cancel();
        _mouseHoverTimer.Dispose();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _mouseHoverTimer.Enabled = false;
        if (_currentItem is not null && !_currentItem.IsDisposed)
        {
            _currentItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseHover);
        }
    }
}
