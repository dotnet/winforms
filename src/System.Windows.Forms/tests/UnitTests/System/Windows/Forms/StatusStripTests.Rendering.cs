// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public partial class StatusStripTests
{
    [WinFormsFact]
    public void StatusStrip_RendersBorderCorrectly()
    {
        using Form form = new();
        using StatusStrip statusStrip = new StatusStrip
        {
            BackColor = Color.Blue,
            SizingGrip = false,
            Renderer = new ToolStripProfessionalRenderer(new CustomColorTable()),
            Size = new Size(200, 38)
        };
        form.Controls.Add(statusStrip);

        // Force the handle creation
        Assert.NotEqual(IntPtr.Zero, form.Handle);
        Assert.NotEqual(IntPtr.Zero, statusStrip.Handle);

        // Create an Enhance Metafile into which we will render the control
        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        // Render the control
        statusStrip.PrintToMetafile(emf);

        Rectangle bounds = statusStrip.Bounds;
        Rectangle bitBltBounds = new(bounds.X, 0, bounds.Width - 1, bounds.Height - 1);
        Rectangle polylineBounds = new(bounds.X, 0, bounds.Width - 1, 15);

        // This is the default pen style GDI+ renders polylines with
        PEN_STYLE penStyle = PEN_STYLE.PS_SOLID | PEN_STYLE.PS_JOIN_ROUND | PEN_STYLE.PS_COSMETIC |
            PEN_STYLE.PS_ENDCAP_FLAT | PEN_STYLE.PS_JOIN_MITER | PEN_STYLE.PS_GEOMETRIC;

        emf.Validate(
           state,
           Validate.BitBltValidator(bitBltBounds, State.BrushColor(Color.Blue)),
           Validate.Polyline16(polylineBounds, null, State.Pen(16, Color.Green, penStyle)));

        string details = emf.RecordsToString();
    }

    private sealed class CustomColorTable : ProfessionalColorTable
    {
        public override Color StatusStripBorder => Color.Green;
    }
}
