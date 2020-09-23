// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class NumericUpDownTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void NumericUpDown_Constructor()
        {
            using var nud = new NumericUpDown();

            Assert.NotNull(nud);
            Assert.Equal("0", nud.Text);
        }

        [WinFormsFact]
        public void NumericUpDown_BasicRendering()
        {
            using var form = new Form();
            using var upDown = new NumericUpDown();

            form.Controls.Add(upDown);

            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Assert.Equal(new Rectangle(0, 0, 120, 23), upDown.Bounds);

            // The rendering here is the "fill" for the background around the child edit control, which
            // doesn't match up to the main control's bounds.
            upDown.PrintToMetafile(emf);
            emf.Validate(
                state,
                Validate.Rectangle(
                    new Rectangle(1, 1, 98, 17),
                    State.Pen(2, upDown.BackColor, Gdi32.PS.SOLID)));

            // Printing the main control doesn't get the redraw for the child controls on the first render,
            // directly hitting the up/down button subcontrol.

            using var emfButtons = new EmfScope();
            state = new DeviceContextState(emfButtons);
            upDown.Controls[0].PrintToMetafile(emfButtons);

            // This is the "fill" line under the up/down arrows
            emfButtons.Validate(
                state,
                Validate.SkipType(Gdi32.EMR.STRETCHDIBITS),
                Validate.SkipType(Gdi32.EMR.STRETCHDIBITS),
                Validate.LineTo(
                    (0, 18), (16, 18),
                    State.Pen(1, upDown.BackColor, Gdi32.PS.SOLID)));

            // Now check the disabled state

            upDown.Enabled = false;

            using var emfDisabled = new EmfScope();
            state = new DeviceContextState(emfDisabled);
            upDown.PrintToMetafile(emfDisabled);

            emfDisabled.Validate(
                state,
                Validate.Rectangle(
                    new Rectangle(0, 0, 99, 18),
                    State.Pen(1, upDown.BackColor, Gdi32.PS.SOLID)),
                Validate.Rectangle(
                    new Rectangle(1, 1, 97, 16),
                    State.Pen(1, SystemColors.Control, Gdi32.PS.SOLID)),
                Validate.SkipType(Gdi32.EMR.STRETCHDIBITS),
                Validate.SkipType(Gdi32.EMR.STRETCHDIBITS),
                Validate.LineTo(
                    (0, 18), (16, 18),
                    State.Pen(1, upDown.BackColor, Gdi32.PS.SOLID)));
        }
    }
}
