// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public partial class ToolStripTests
{
    [WinFormsFact]
    public void ToolStrip_RendersBackgroundCorrectly()
    {
        using Form form = new();
        using ToolStrip toolStrip = new ToolStrip
        {
            BackColor = Color.Blue,
            Size = new Size(200, 38)
        };
        form.Controls.Add(toolStrip);

        // Force the handle creation
        Assert.NotEqual(IntPtr.Zero, form.Handle);
        Assert.NotEqual(IntPtr.Zero, toolStrip.Handle);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Rectangle bounds = toolStrip.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStrip.TestAccessor().Dynamic.OnPaintBackground(e);

        Rectangle bitBltBounds = new(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

        RECT[] expectedRects = [
            new(0, 0, 1, 1),
            new(bounds.Width - 3, 0, bounds.Width, 1),
            new(bounds.Width - 1, 1, bounds.Width, 3),
            new(0, bounds.Height - 2, 1, bounds.Height - 1),
            new(bounds.Width - 1, bounds.Height - 2, bounds.Width, bounds.Height - 1),
            new(0, bounds.Height - 1, 2, bounds.Height),
            new(bounds.Width - 2, bounds.Height - 1, bounds.Width, bounds.Height)
        ];

        emf.Validate(
            state,
            Validate.BitBltValidator(
                bitBltBounds,
                State.BrushColor(Color.Blue)),
            Validate.BitBltValidator(
                bitBltBounds,
                State.BrushColor(form.BackColor),
                State.Clipping(expectedRects)));

        string details = emf.RecordsToString();
    }
}
