// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Batches child-control location changes until the scope is disposed.
/// </summary>
/// <remarks>
///  <para>
///   This scope uses the Win32 deferred window-position API when possible. If a deferred native batch
///   cannot be created or is lost, the collected changes are applied individually when the scope is disposed.
///  </para>
/// </remarks>
public readonly ref struct DeferLocationChangeScope
{
    private readonly State? _state;
    private readonly SuspendPaintingScope _paintingScope;
    private readonly SuspendRelocationScope _relocationScope;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DeferLocationChangeScope"/> struct.
    /// </summary>
    /// <param name="parent">The parent control whose child-control location changes should be deferred.</param>
    public DeferLocationChangeScope(Control parent)
        : this(parent, suppressRender: true, suspendLayout: true)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DeferLocationChangeScope"/> struct.
    /// </summary>
    /// <param name="parent">The parent control whose child-control location changes should be deferred.</param>
    /// <param name="suppressRender">
    ///  <see langword="true"/> to suppress rendering while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    public DeferLocationChangeScope(Control parent, bool suppressRender)
        : this(parent, suppressRender, suspendLayout: true)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DeferLocationChangeScope"/> struct.
    /// </summary>
    /// <param name="parent">The parent control whose child-control location changes should be deferred.</param>
    /// <param name="suppressRender">
    ///  <see langword="true"/> to suppress rendering while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    /// <param name="suspendLayout">
    ///  <see langword="true"/> to suspend layout while changes are deferred; otherwise,
    ///  <see langword="false"/>.
    /// </param>
    public DeferLocationChangeScope(Control parent, bool suppressRender, bool suspendLayout)
    {
        ArgumentNullException.ThrowIfNull(parent);

        _state = new(parent);
        _paintingScope = suppressRender
            ? new SuspendPaintingScope(parent)
            : default;
        _relocationScope = suspendLayout
            ? new SuspendRelocationScope(parent)
            : default;
    }

    /// <summary>
    ///  Defers moving a control to the specified location.
    /// </summary>
    /// <param name="control">The control to move.</param>
    /// <param name="x">The deferred x-coordinate.</param>
    /// <param name="y">The deferred y-coordinate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="control"/> is <see langword="null"/>.</exception>
    public void Defer(Control control, int x, int y)
    {
        ArgumentNullException.ThrowIfNull(control);
        Size size = control.Size;
        _state?.Defer(control, new Rectangle(x, y, size.Width, size.Height));
    }

    /// <summary>
    ///  Defers moving and resizing a control to the specified bounds.
    /// </summary>
    /// <param name="control">The control to move and resize.</param>
    /// <param name="x">The deferred x-coordinate.</param>
    /// <param name="y">The deferred y-coordinate.</param>
    /// <param name="width">The deferred width.</param>
    /// <param name="height">The deferred height.</param>
    /// <exception cref="ArgumentNullException"><paramref name="control"/> is <see langword="null"/>.</exception>
    public void Defer(Control control, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(control);
        _state?.Defer(control, new Rectangle(x, y, width, height));
    }

    /// <summary>
    ///  Defers moving and resizing a control to the specified bounds.
    /// </summary>
    /// <param name="control">The control to move and resize.</param>
    /// <param name="bounds">The deferred bounds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="control"/> is <see langword="null"/>.</exception>
    public void Defer(Control control, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(control);
        _state?.Defer(control, bounds);
    }

    /// <summary>
    ///  Applies the deferred location changes and resumes any bundled suspension scopes.
    /// </summary>
    public void Dispose()
    {
        _state?.Dispose();
        _relocationScope.Dispose();
        _paintingScope.Dispose();
    }

    /// <summary>
    ///  Holds mutable deferred-position state for <see cref="DeferLocationChangeScope"/>.
    /// </summary>
    private sealed class State
    {
        private readonly Control _parent;
        private readonly List<DeferredWindowPosition> _deferredPositions = [];
        private HDWP _hdwp;
        private bool _batchFailed;

        public State(Control parent)
        {
            _parent = parent;
            _hdwp = parent.IsHandleCreated && parent.Controls.Count > 0
                ? PInvoke.BeginDeferWindowPos(parent.Controls.Count)
                : HDWP.Null;
            _batchFailed = _hdwp.IsNull || !parent.IsHandleCreated;
        }

        public void Defer(Control control, Rectangle bounds)
        {
            DeferredWindowPosition deferredPosition = new(control, bounds);
            _deferredPositions.Add(deferredPosition);

            if (_batchFailed)
            {
                return;
            }

            if (!control.IsHandleCreated)
            {
                _batchFailed = true;

                return;
            }

            HDWP hdwp = PInvoke.DeferWindowPos(
                _hdwp,
                (HWND)control.Handle,
                HWND.Null,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);

            if (hdwp.IsNull)
            {
                _hdwp = HDWP.Null;
                _batchFailed = true;

                return;
            }

            _hdwp = hdwp;
        }

        public void Dispose()
        {
            if (!_batchFailed && !_hdwp.IsNull && PInvoke.EndDeferWindowPos(_hdwp))
            {
                return;
            }

            if (_batchFailed && !_hdwp.IsNull)
            {
                PInvoke.EndDeferWindowPos(_hdwp);
            }

            foreach (DeferredWindowPosition deferredPosition in _deferredPositions)
            {
                Control control = deferredPosition.Control;
                Rectangle bounds = deferredPosition.Bounds;
                if (control.IsHandleCreated)
                {
                    PInvoke.SetWindowPos(
                        control,
                        HWND.Null,
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height,
                        SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
                }
                else
                {
                    control.Bounds = bounds;
                }
            }

            _parent.Invalidate(invalidateChildren: true);
        }
    }

    /// <summary>
    ///  Represents a deferred window-position request.
    /// </summary>
    private readonly record struct DeferredWindowPosition(Control Control, Rectangle Bounds);
}
#endif
