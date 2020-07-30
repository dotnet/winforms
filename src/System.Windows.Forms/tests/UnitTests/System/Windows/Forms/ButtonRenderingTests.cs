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
                Validate.LineTo((bounds.Right - 1, 0), (0, 0), SystemColors.ControlLightLight),
                Validate.LineTo((0, 0), (0, bounds.Bottom - 1), SystemColors.ControlLightLight),
                Validate.LineTo((0, bounds.Bottom - 1), (bounds.Right - 1, bounds.Bottom - 1), SystemColors.ControlDarkDark),
                Validate.LineTo((bounds.Right - 1, bounds.Bottom - 1), (bounds.Right - 1, -1), SystemColors.ControlDarkDark),
                Validate.LineTo((bounds.Right - 2, 1), (1, 1), SystemColors.Control),
                Validate.LineTo((1, 1), (1, bounds.Bottom - 2), SystemColors.Control),
                Validate.LineTo((1, bounds.Bottom - 2), (bounds.Right - 2, bounds.Bottom - 2), SystemColors.ControlDark),
                Validate.LineTo((bounds.Right - 2, bounds.Bottom - 2), (bounds.Right - 2, 0), SystemColors.ControlDark));
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
                Validate.LineTo((bounds.Right - 1, 0), (0, 0), SystemColors.ControlLightLight),
                Validate.LineTo((0, 0), (0, bounds.Bottom - 1), SystemColors.ControlLightLight),
                Validate.LineTo((0, bounds.Bottom - 1), (bounds.Right - 1, bounds.Bottom - 1), SystemColors.ControlDarkDark),
                Validate.LineTo((bounds.Right - 1, bounds.Bottom - 1), (bounds.Right - 1, -1), SystemColors.ControlDarkDark),
                Validate.LineTo((bounds.Right - 2, 1), (1, 1), SystemColors.Control),
                Validate.LineTo((1, 1), (1, bounds.Bottom - 2), SystemColors.Control),
                Validate.LineTo((1, bounds.Bottom - 2), (bounds.Right - 2, bounds.Bottom - 2), SystemColors.ControlDark),
                Validate.LineTo((bounds.Right - 2, bounds.Bottom - 2), (bounds.Right - 2, 0), SystemColors.ControlDark));
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
    }
}
