﻿// Licensed to the .NET Foundation under one or more agreements.
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
        public unsafe void Button_VisualStyles_off_Default_LineDrawing()
        {
            if (Application.RenderWithVisualStyles)
            {
                return;
            }

            using Button button = new Button();
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            emf.Validate(
                state,
                Validate.Repeat(Validate.SkipType(Gdi32.EMR.BITBLT), 1),
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

        [WinFormsFact(Skip = "TODO, refer to https://github.com/dotnet/winforms/issues/4212")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/4212")]
        public unsafe void Button_VisualStyles_on_Default_LineDrawing()
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }

            using Button button = new Button();
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            /*

            [ENHMETAHEADER] Bounds: {0, 0, 74, 22 (LTRB)} Device Size: {Width=1024, Height=768} Header Size: 108
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 75, 23 (LTRB)}
            [EMRSETVIEWPORTORGEX] Point: {X=0,Y=0}
            [EMRCREATEBRUSHINDIRECT] Index: 1 Style: SOLID Color: [R=240, G=240, B=240] (COLOR_MENU, COLOR_BTNFACE, COLOR_MENUBAR)
            [EMRSELECTOBJECT] Index: 1
            [EMRBITBLT] Bounds: {0, 0, -1, -1 (LTRB)} Destination: {0, 0, 120, 0 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRDELETEOBJECT] Index: 1
            [EMRCREATEBRUSHINDIRECT] Index: 1 Style: SOLID Color: [R=240, G=240, B=240] (COLOR_MENU, COLOR_BTNFACE, COLOR_MENUBAR)
            [EMRSELECTOBJECT] Index: 1
            [EMRBITBLT] Bounds: {0, 0, -1, -1 (LTRB)} Destination: {0, 0, 120, 0 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRDELETEOBJECT] Index: 1
            [EMRSETVIEWPORTORGEX] Point: {X=0,Y=0}
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 75, 23 (LTRB)} Rects: 1 Rect index 0: {0, 0, 75, 23 (LTRB)}
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 75, 23 (LTRB)}
            [EMRALPHABLEND]
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 75, 23 (LTRB)} Rects: 1 Rect index 0: {0, 0, 75, 23 (LTRB)}
            [EMREOF]

            */
        }

        [WinFormsFact]
        public unsafe void Button_VisualStyles_off_Default_WithText_LineDrawing()
        {
            if (Application.RenderWithVisualStyles)
            {
                return;
            }

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
        public unsafe void Button_VisualStyles_on_Default_WithText_LineDrawing()
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }

            using Button button = new Button { Text = "Hello" };
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            Rectangle bounds = button.Bounds;

            button.PrintToMetafile(emf);

            /*

            [ENHMETAHEADER] Bounds: {0, 0, 74, 22 (LTRB)} Device Size: {Width=3840, Height=2160} Header Size: 108
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 75, 23 (LTRB)}
            [EMRSETVIEWPORTORGEX] Point: {X=0,Y=0}
            [EMRCREATEBRUSHINDIRECT] Index: 1 Style: SOLID Color: [R=240, G=240, B=240] (COLOR_MENU, COLOR_BTNFACE, COLOR_MENUBAR)
            [EMRSELECTOBJECT] Index: 1
            [EMRBITBLT] Bounds: {0, 0, -1, -1 (LTRB)} Destination: {0, 0, 120, 0 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRDELETEOBJECT] Index: 1
            [EMRCREATEBRUSHINDIRECT] Index: 1 Style: SOLID Color: [R=240, G=240, B=240] (COLOR_MENU, COLOR_BTNFACE, COLOR_MENUBAR)
            [EMRSELECTOBJECT] Index: 1
            [EMRBITBLT] Bounds: {0, 0, -1, -1 (LTRB)} Destination: {0, 0, 120, 0 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRDELETEOBJECT] Index: 1
            [EMRSETVIEWPORTORGEX] Point: {X=0,Y=0}
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 75, 23 (LTRB)} Rects: 1 Rect index 0: {0, 0, 75, 23 (LTRB)}
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 75, 23 (LTRB)}
            [EMRALPHABLEND]
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 75, 23 (LTRB)} Rects: 1 Rect index 0: {0, 0, 75, 23 (LTRB)}
            [EMRSAVEDC]
            [EMRSETICMMODE] Mode: ICM_OFF
            [EMREXTSELECTCLIPRGN] Mode: Set Default
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSETTEXTALIGN] Mode: TA_BASELINE
            [EMREXTCREATEFONTINDIRECTW] Index: 1 FaceName: 'Segoe UI' Height: -12 Weight: FW_NORMAL
            [EMRSELECTOBJECT] Index: 1
            [EMREXTTEXTOUTW] Bounds: {24, 4, 51, 18 (LTRB)} Text: 'Hello'
            [EMRSELECTOBJECT] StockObject: SYSTEM_FONT
            [EMRRESTOREDC] Index: -1
            [EMRDELETEOBJECT] Index: 1
            [EMREOF]

            */
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
