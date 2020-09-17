// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ButtonRenderingTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public unsafe void CaptureButton()
        {
            using Button button = new Button();

            using var emf = new EmfScope();
            button.PrintToMetafile(emf);

            var types = new List<Gdi32.EMR>();
            var details = new List<string>();
            emf.Enumerate((ref EmfRecord record) =>
            {
                types.Add(record.Type);
                details.Add(record.ToString());
                return true;
            });
        }

        [WinFormsFact]
        public unsafe void Button_Default_LineDrawing()
        {
            using Button button = new Button();
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            emf.Validate(
                state,
                Validate.Repeat(Validate.SkipType(Gdi32.EMR.BITBLT), 2),
                Validate.LineTo(
                    (bounds.Right - 1, 0), (0, 0),
                    State.PenColor(SystemColors.ControlLightLight)),
                Validate.LineTo(
                    (0, 0), (0, bounds.Bottom - 1),
                    State.PenColor(SystemColors.ControlLightLight)),
                Validate.LineTo(
                    (0, bounds.Bottom - 1), (bounds.Right - 1, bounds.Bottom - 1),
                    State.PenColor(SystemColors.ControlDarkDark)),
                Validate.LineTo(
                    (bounds.Right - 1, bounds.Bottom - 1), (bounds.Right - 1, -1),
                    State.PenColor(SystemColors.ControlDarkDark)),
                Validate.LineTo(
                    (bounds.Right - 2, 1), (1, 1),
                    State.PenColor(SystemColors.Control)),
                Validate.LineTo(
                    (1, 1), (1, bounds.Bottom - 2),
                    State.PenColor(SystemColors.Control)),
                Validate.LineTo(
                    (1, bounds.Bottom - 2), (bounds.Right - 2, bounds.Bottom - 2),
                    State.PenColor(SystemColors.ControlDark)),
                Validate.LineTo(
                    (bounds.Right - 2, bounds.Bottom - 2), (bounds.Right - 2, 0),
                    State.PenColor(SystemColors.ControlDark)));
        }

        [WinFormsFact]
        public unsafe void Button_Default_WithText_LineDrawing()
        {
            using Button button = new Button { Text = "Hello" };
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            emf.Validate(
                state,
                Validate.SkipType(Gdi32.EMR.BITBLT),
                Validate.TextOut("Hello"),
                Validate.LineTo(
                    (bounds.Right - 1, 0), (0, 0),
                    State.PenColor(SystemColors.ControlLightLight)),
                Validate.LineTo(
                    (0, 0), (0, bounds.Bottom - 1),
                    State.PenColor(SystemColors.ControlLightLight)),
                Validate.LineTo(
                    (0, bounds.Bottom - 1), (bounds.Right - 1, bounds.Bottom - 1),
                    State.PenColor(SystemColors.ControlDarkDark)),
                Validate.LineTo(
                    (bounds.Right - 1, bounds.Bottom - 1), (bounds.Right - 1, -1),
                    State.PenColor(SystemColors.ControlDarkDark)),
                Validate.LineTo(
                    (bounds.Right - 2, 1), (1, 1),
                    State.PenColor(SystemColors.Control)),
                Validate.LineTo(
                    (1, 1), (1, bounds.Bottom - 2),
                    State.PenColor(SystemColors.Control)),
                Validate.LineTo(
                    (1, bounds.Bottom - 2), (bounds.Right - 2, bounds.Bottom - 2),
                    State.PenColor(SystemColors.ControlDark)),
                Validate.LineTo(
                    (bounds.Right - 2, bounds.Bottom - 2), (bounds.Right - 2, 0),
                    State.PenColor(SystemColors.ControlDark)));
        }

        [WinFormsFact]
        public unsafe void CaptureButtonOnForm()
        {
            using Form form = new Form();
            using Button button = new Button();
            form.Controls.Add(button);

            using var emf = new EmfScope();
            form.PrintToMetafile(emf);

            var details = emf.RecordsToString();
        }

        [WinFormsFact]
        public unsafe void Button_FlatStyle_WithText_Rectangle()
        {
            using Button button = new Button
            {
                Text = "Flat Style",
                FlatStyle = FlatStyle.Flat,
            };

            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            emf.Validate(
                state,
                Validate.SkipType(Gdi32.EMR.BITBLT),
                Validate.TextOut("Flat Style"),
                Validate.Rectangle(
                    new Rectangle(0, 0, button.Width - 1, button.Height - 1),
                    State.PenColor(Color.Black),
                    State.PenStyle(Gdi32.PS.ENDCAP_ROUND),
                    State.BrushStyle(Gdi32.BS.NULL),       // Regressed in https://github.com/dotnet/winforms/pull/3667
                    State.Rop2( Gdi32.R2.COPYPEN)));
        }
    }
}
