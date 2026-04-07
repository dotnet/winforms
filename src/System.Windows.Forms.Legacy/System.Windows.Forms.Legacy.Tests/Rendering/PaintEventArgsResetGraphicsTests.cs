// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Legacy.Tests;

/// <summary>
///  Regression tests for the <c>PaintEventArgs.ResetGraphics</c> FailFast crash.
/// </summary>
/// <remarks>
///  <para>
///   When a <see cref="PaintEventArgs"/> is created from an <see cref="System.Drawing.Graphics"/> HDC
///   (non-double-buffered path with <c>DrawingEventFlags.SaveState</c>), the graphics state must be
///   saved no matter which property — <see cref="PaintEventArgs.Graphics"/> (public) or the internal
///   <c>GraphicsInternal</c> — first causes the underlying <see cref="System.Drawing.Graphics"/> object
///   to be created. If state is not saved, <c>ResetGraphics()</c> silently skips the
///   <c>graphics.Restore()</c> call, causing any clip or transform applied in <c>OnPaintBackground</c>
///   to bleed into <c>OnPaint</c>. In DEBUG builds this also fires <c>Debug.Fail</c>, which in
///   .NET 10 escalates to <c>Environment.FailFast</c> and terminates the process.
///  </para>
///  <para>
///   IMPORTANT: <c>Debug.Fail</c> is decorated with <c>[Conditional("DEBUG")]</c> and is therefore
///   a compile-time no-op in release/test builds. Tests based purely on "no exception was thrown"
///   would pass even without the fix. Instead, these tests assert the <em>observable behavioural
///   invariant</em>: a clip applied during <c>OnPaintBackground</c> must not be visible during
///   <c>OnPaint</c>, which is only true when <c>ResetGraphics()</c> has correctly restored state.
///  </para>
///  <para>
///   See <c>painteventargs-resetgraphics-failfast.md</c> for the full analysis.
///  </para>
/// </remarks>
public class PaintEventArgsResetGraphicsTests
{
    /// <summary>
    ///  A tiny clip rectangle applied during <c>OnPaintBackground</c> to a region that does not
    ///  include the centre of the control. If <c>ResetGraphics()</c> fails to restore state, this
    ///  clip will still be active in <c>OnPaint</c>, and the centre point will appear clipped.
    /// </summary>
    private static readonly Rectangle s_backgroundClip = new(0, 0, 10, 10);

    /// <summary>
    ///  <para>A control that:</para>
    ///  <list type="bullet">
    ///   <item>In <see cref="Control.OnPaintBackground"/>: accesses the public <c>Graphics</c>
    ///   property first (triggering the bug path), then applies a small clip in the top-left corner.</item>
    ///   <item>In <see cref="Control.OnPaint"/>: records whether the centre of the control is
    ///   inside the current clip region. After a correct <c>ResetGraphics()</c> it must be visible;
    ///   if state was not restored, the narrow clip from background painting bleeds through and
    ///   the centre point appears clipped.</item>
    ///  </list>
    /// </summary>
    private sealed class ClipIsolationControl : Control
    {
        internal ClipIsolationControl(bool optimizedDoubleBuffer)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, optimizedDoubleBuffer);
            SetStyle(ControlStyles.UserPaint, true);
        }

        /// <summary>
        ///  Set to <see langword="true"/> when <see cref="OnPaint"/> confirms the centre of the
        ///  control is visible (i.e. not still clipped by the background-paint clip region).
        /// </summary>
        internal bool CentrePointVisibleInOnPaint { get; private set; }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Access the public Graphics property FIRST so the Graphics object is created
            // without the SaveStateIfNeeded callback — this is the exact trigger for the bug.
            Graphics g = e.Graphics;

            // Narrow the clip to a small corner region that excludes the centre of the control.
            // Without the fix, ResetGraphics() skips Restore(), so this clip persists into OnPaint.
            g.SetClip(s_backgroundClip);

            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // If ResetGraphics() correctly restored state, the clip here should be the full
            // client rectangle, so the centre point is visible.
            // If state was NOT restored, the narrow background clip is still active and the
            // centre is clipped out.
            Point centre = new(Width / 2, Height / 2);
            CentrePointVisibleInOnPaint = e.Graphics.IsVisible(centre);
        }
    }

    /// <summary>
    ///  Verifies that a clip applied via the public <see cref="PaintEventArgs.Graphics"/> property
    ///  in <see cref="Control.OnPaintBackground"/> is properly isolated from <see cref="Control.OnPaint"/>
    ///  on the non-double-buffered (<c>AllPaintingInWmPaint</c>) code path.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This test catches the bug in all build configurations. Without the fix,
    ///   <c>ResetGraphics()</c> silently skips <c>graphics.Restore()</c> (because
    ///   <c>_savedGraphicsState</c> is <see langword="null"/>), and the background clip bleeds into
    ///   <c>OnPaint</c>, causing <c>CentrePointVisibleInOnPaint</c> to be <see langword="false"/>.
    ///  </para>
    /// </remarks>
    [StaFact]
    public void ClipAppliedInOnPaintBackground_IsNotVisible_InOnPaint_NonDoubleBuffered()
    {
        // AllPaintingInWmPaint without OptimizedDoubleBuffer is the non-double-buffered HDC path
        // that creates PaintEventArgs with DrawingEventFlags.SaveState — where the bug lives.
        using ClipIsolationControl control = new(optimizedDoubleBuffer: false)
        {
            Size = new Size(100, 100)
        };

        using Form form = new()
        {
            Size = new Size(200, 200)
        };

        form.Controls.Add(control);

        // Show the form so WmPaint is dispatched and the full paint sequence runs:
        //   WmPaint → OnPaintBackground (applies clip via public Graphics)
        //   → ResetGraphics() → OnPaint (checks whether clip was restored)
        form.Show();
        control.Refresh();

        // The centre of the control must be visible in OnPaint — proving that
        // ResetGraphics() correctly restored the graphics state after OnPaintBackground.
        Assert.True(control.CentrePointVisibleInOnPaint,
            "The clip set in OnPaintBackground bled into OnPaint. " +
            "ResetGraphics() did not restore the graphics state, which means " +
            "_savedGraphicsState was null when it was called (the bug).");
    }

    /// <summary>
    ///  Verifies the same clip-isolation invariant with double-buffering enabled, ensuring
    ///  the change to the <c>Graphics</c> getter does not break the double-buffered path.
    /// </summary>
    [StaFact]
    public void ClipAppliedInOnPaintBackground_IsNotVisible_InOnPaint_DoubleBuffered()
    {
        using ClipIsolationControl control = new(optimizedDoubleBuffer: true)
        {
            Size = new Size(100, 100)
        };

        using Form form = new()
        {
            Size = new Size(200, 200)
        };

        form.Controls.Add(control);
        form.Show();
        control.Refresh();

        Assert.True(control.CentrePointVisibleInOnPaint,
            "The clip set in OnPaintBackground bled into OnPaint on the double-buffered path.");
    }
}
