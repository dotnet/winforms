﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public partial class ToolStripTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStrip_RendersBackgroundCorrectly()
        {
            using Form form = new Form();
            using ToolStrip toolStrip = new ToolStrip
            {
                BackColor = Color.Blue,
                Size = new Size(200, 38)
            };
            form.Controls.Add(toolStrip);

            // Force the handle creation
            Assert.NotEqual(IntPtr.Zero, form.Handle);
            Assert.NotEqual(IntPtr.Zero, toolStrip.Handle);

            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = toolStrip.Bounds;
            PaintEventArgs e = new PaintEventArgs(emf, bounds);
            toolStrip.TestAccessor().Dynamic.OnPaintBackground(e);

            Rectangle bitBltBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            RECT[] expectedRects = new RECT[]
            {
                new RECT(0, 0, 1, 1),
                new RECT(bounds.Width - 3, 0, bounds.Width, 1),
                new RECT(bounds.Width - 1, 1, bounds.Width, 3),
                new RECT(0, bounds.Height - 2, 1, bounds.Height - 1),
                new RECT(bounds.Width - 1, bounds.Height - 2, bounds.Width, bounds.Height - 1),
                new RECT(0, bounds.Height - 1, 2, bounds.Height),
                new RECT(bounds.Width - 2, bounds.Height - 1, bounds.Width, bounds.Height)
            };

            emf.Validate(
                state,
                Validate.BitBltValidator(
                    bitBltBounds,
                    State.BrushColor(Color.Blue)),
                Validate.BitBltValidator(
                    bitBltBounds,
                    State.BrushColor(form.BackColor),
                    State.Clipping(expectedRects)));

            var details = emf.RecordsToString();
        }
    }
}
