// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class MouseHoverTimer : IDisposable
{
    private readonly Timer _mouseHoverTimer = new();
    private readonly WeakReference<ToolStripItem?> _currentItem = new(null);

    public MouseHoverTimer()
    {
        _mouseHoverTimer.Interval = SystemInformation.MouseHoverTime;
        _mouseHoverTimer.Tick += OnTick;
    }

    public void Start(ToolStripItem? item)
    {
        _currentItem.TryGetTarget(out var currentItem);
        if (item != currentItem)
        {
            Cancel();
            _currentItem.SetTarget(item);
        }

        if (item is not null)
        {
            _mouseHoverTimer.Enabled = true;
        }
    }

    /// <summary>
    /// Cancels if and only if this <paramref name="item"/> was the one that requested the timer.
    /// </summary>
    public void Cancel(ToolStripItem? item)
    {
        _currentItem.TryGetTarget(out var currentItem);
        if (item == currentItem)
        {
            Cancel();
        }
    }

    public void Dispose()
    {
        Cancel();
        _mouseHoverTimer.Dispose();
    }

    private void Cancel()
    {
        _mouseHoverTimer.Enabled = false;
        _currentItem.SetTarget(null);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _mouseHoverTimer.Enabled = false;
        if (_currentItem.TryGetTarget(out var currentItem) && !currentItem.IsDisposed)
        {
            currentItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseHover);
        }
    }
}
