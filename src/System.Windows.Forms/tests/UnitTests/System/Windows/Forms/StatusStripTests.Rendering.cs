// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public partial class StatusStripTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void StatusStrip_RendersBorderCorrectly()
        {
            using Form form = new Form();
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
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            // Render the control
            statusStrip.PrintToMetafile(emf);

            Rectangle bounds = statusStrip.Bounds;
            Rectangle bitBltBounds = new Rectangle(bounds.X, 0, bounds.Width - 1, bounds.Height - 1);
            Rectangle polylineBounds = new Rectangle(bounds.X, 0, bounds.Width - 1, 15);

            // This is the default pen style GDI+ renders polylines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT | Gdi32.PS.JOIN_MITER | Gdi32.PS.GEOMETRIC;

            emf.Validate(
               state,
               Validate.BitBltValidator(bitBltBounds, State.BrushColor(Color.Blue)),
               Validate.Polyline16(polylineBounds, null, State.Pen(16, Color.Green, penStyle)));

            var details = emf.RecordsToString();
        }

        private sealed class CustomColorTable : ProfessionalColorTable
        {
            public override Color StatusStripBorder => Color.Green;
        }
    }
}
