// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public partial class TextBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TextBox_PlaceholderText_RendersBackgroundCorrectly()
        {
            using Form form = new Form();

            // This adds a placeholder text with only whitespace so we can test the background of the placeholder text
            using TextBox textBox = new TextBox
            {
                BackColor = Color.Blue,
                Size = new Size(80, 23),
                PlaceholderText = "                        ",
                Enabled = false
            };
            form.Controls.Add(textBox);

            // Force the handle creation
            _ = form.Handle;
            _ = textBox.Handle;
            form.PerformLayout();

            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = textBox.Bounds;
            PaintEventArgs e = new PaintEventArgs(emf, bounds);
            textBox.TestAccessor().Dynamic.OnPaintBackground(e);

            Rectangle bitBltBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 5, bounds.Height - 5);

            emf.Validate(state,
                Validate.BitBltValidator(
                    bitBltBounds,
                    State.BrushColor(Color.Blue)));

            var details = emf.RecordsToString();
        }
    }
}
