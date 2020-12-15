// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DateTimePickerRenderingTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public unsafe void Capture_DateTimePicker_On_Form()
        {
            // create control and form
            using Form form = new Form();
            using DateTimePicker control = new DateTimePicker();
            form.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, form.Handle);

            using var emf = new EmfScope();
            control.PrintToMetafile(emf);

            var details = emf.RecordsToString();

            /*
            [ENHMETAHEADER] Bounds: {0, 0, 199, 22 (LTRB)} Device Size: {Width=2560, Height=1600} Header Size: 108
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 200, 23 (LTRB)}
            [EMRBITBLT] Bounds: {0, 0, 199, 22 (LTRB)} Destination: {0, 0, 200, 23 (LTRB)} Rop: SRCCOPY Source DC Color: [R=0, G=120, B=215] (COLOR_HIGHLIGHT)
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 200, 23 (LTRB)} Rects: 1
	            Rect index 0: {0, 0, 200, 23 (LTRB)}
            [EMREOF]
             */
        }

        [WinFormsFact]
        public unsafe void DateTimePicker_VisualStyles_Enabled_Border()
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }

            using Form form = new Form();
            using DateTimePicker control = new DateTimePicker { };
            form.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, form.Handle);

            using var emf = new EmfScope();

            control.PrintToMetafile(emf);

            DeviceContextState state = new DeviceContextState(emf);
            emf.Validate(
                state,
                Validate.BitBltValidator(
                    new Rectangle(0, 0, control.Width - 1, control.Height - 1)
                )
            );
        }
    }
}
