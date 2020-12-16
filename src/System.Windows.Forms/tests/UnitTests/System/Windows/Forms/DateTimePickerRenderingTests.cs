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
        public unsafe void DateTimePicker_VisualStyles_On_Capture()
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }
            // create control and form
            using Form form = new Form();
            using DateTimePicker control = new DateTimePicker();
            form.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, form.Handle);

            using var emf = new EmfScope();
            control.PrintToMetafile(emf);

            var details = emf.RecordsToString();

            #region details content
            /*
            [ENHMETAHEADER] Bounds: {0, 0, 199, 22 (LTRB)} Device Size: {Width=2560, Height=1600} Header Size: 108
            [EMRINTERSECTCLIPRECT] RECT: {0, 0, 200, 23 (LTRB)}
            [EMRBITBLT] Bounds: {0, 0, 199, 22 (LTRB)} Destination: {0, 0, 200, 23 (LTRB)} Rop: SRCCOPY Source DC Color: [R=0, G=120, B=215] (COLOR_HIGHLIGHT)
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 200, 23 (LTRB)} Rects: 1
               Rect index 0: {0, 0, 200, 23 (LTRB)}
            [EMREOF]
             */
            #endregion
        }

        [WinFormsFact]
        public unsafe void DateTimePicker_VisualStyles_Off_Capture()
        {
            if (Application.RenderWithVisualStyles)
            {
                return;
            }

            // create control and form
            using Form form = new Form();
            using DateTimePicker control = new DateTimePicker();
            form.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, form.Handle);

            using var emf = new EmfScope();
            control.PrintToMetafile(emf);

            var details = emf.RecordsToString();

            #region details content
            /*
            [ENHMETAHEADER] Bounds: {5, 0, 195, 18 (LTRB)} Device Size: {Width=2560, Height=1600} Header Size: 108
            [EMREXTCREATEFONTINDIRECTW] Index: 1 FaceName: 'Segoe UI' Height: -12 Weight: FW_NORMAL
            [EMRSELECTOBJECT] Index: 1
            [EMRINTERSECTCLIPRECT] RECT: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKCOLOR] Color: [R=0, G=120, B=215] (COLOR_HIGHLIGHT)
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {2, 2, 55, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {5, 2, 51, 16 (LTRB)} Text: 'mercredi'
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {55, 2, 58, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {55, 2, 57, 16 (LTRB)} Text: ' '
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {58, 2, 70, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {58, 2, 69, 16 (LTRB)} Text: '16'
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {70, 2, 73, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {70, 2, 72, 16 (LTRB)} Text: ' '
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {73, 2, 129, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {74, 2, 126, 16 (LTRB)} Text: 'décembre'
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {129, 2, 132, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {129, 2, 131, 16 (LTRB)} Text: ' '
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRINTERSECTCLIPRECT] RECT: {132, 2, 156, 17 (LTRB)}
            [EMRSETTEXTALIGN] Mode: TA_UPDATECP
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTTEXTOUTW] Bounds: {132, 2, 155, 16 (LTRB)} Text: '2020'
            [EMRMOVETOEX] Point: {X=0,Y=0}
            [EMRSETTEXTALIGN] Mode: TA_LEFT
            [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {2, 0, 178, 19 (LTRB)} Rects: 1
                Rect index 0: {2, 0, 178, 19 (LTRB)}
            [EMREXTSELECTCLIPRGN] Mode: Set Default
            [EMRSELECTOBJECT] StockObject: SYSTEM_FONT
            [EMRCREATEBRUSHINDIRECT] Index: 2 Style: SOLID Color: [R=105, G=105, B=105] (COLOR_3DDKSHADOW)
            [EMRSELECTOBJECT] Index: 2
            [EMRBITBLT] Bounds: {195, 0, 195, 18 (LTRB)} Destination: {195, 0, 196, 19 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] Index: 2
            [EMRBITBLT] Bounds: {179, 18, 194, 18 (LTRB)} Destination: {179, 18, 195, 19 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRCREATEBRUSHINDIRECT] Index: 3 Style: SOLID Color: [R=227, G=227, B=227] (COLOR_3DLIGHT)
            [EMRSELECTOBJECT] Index: 3
            [EMRBITBLT] Bounds: {179, 0, 179, 17 (LTRB)} Destination: {179, 0, 180, 18 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] Index: 3
            [EMRBITBLT] Bounds: {180, 0, 194, 0 (LTRB)} Destination: {180, 0, 195, 1 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRCREATEBRUSHINDIRECT] Index: 4 Style: SOLID Color: [R=160, G=160, B=160] (COLOR_BTNSHADOW)
            [EMRSELECTOBJECT] Index: 4
            [EMRBITBLT] Bounds: {194, 1, 194, 17 (LTRB)} Destination: {194, 1, 195, 18 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] Index: 4
            [EMRBITBLT] Bounds: {180, 17, 193, 17 (LTRB)} Destination: {180, 17, 194, 18 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRCREATEBRUSHINDIRECT] Index: 5 Style: SOLID Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
            [EMRSELECTOBJECT] Index: 5
            [EMRBITBLT] Bounds: {180, 1, 180, 16 (LTRB)} Destination: {180, 1, 181, 17 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] Index: 5
            [EMRBITBLT] Bounds: {181, 1, 193, 1 (LTRB)} Destination: {181, 1, 194, 2 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRCREATEBRUSHINDIRECT] Index: 6 Style: SOLID Color: [R=240, G=240, B=240] (COLOR_MENU, COLOR_BTNFACE, COLOR_MENUBAR)
            [EMRSELECTOBJECT] Index: 6
            [EMRBITBLT] Bounds: {181, 2, 193, 16 (LTRB)} Destination: {181, 2, 194, 17 (LTRB)} Rop: PATCOPY Source DC Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMREXTCREATEFONTINDIRECTW] Index: 7 FaceName: 'Marlett' Height: 13 Weight: FW_NORMAL
            [EMRSELECTOBJECT] Index: 7
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMREXTTEXTOUTW] Bounds: {181, 3, 193, 15 (LTRB)} Text: '6'
            [EMRSETTEXTCOLOR] Color: [R=0, G=0, B=0] (COLOR_BACKGROUND, COLOR_MENUTEXT, COLOR_WINDOWTEXT, COLOR_CAPTIONTEXT, COLOR_BTNTEXT, COLOR_INACTIVECAPTIONTEXT, COLOR_INFOTEXT)
            [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
            [EMRSELECTOBJECT] StockObject: SYSTEM_FONT
            [EMRDELETEOBJECT] Index: 7
            [EMREOF]
             */
            #endregion
        }

        [WinFormsFact]
        public unsafe void DateTimePicker_VisualStyles_On_Enabled_Border()
        {
            if (!Application.RenderWithVisualStyles)
            {
                return;
            }

            using Form form = new Form();
            using DateTimePicker control = new DateTimePicker();
            form.Controls.Add(control);
            Assert.NotEqual(IntPtr.Zero, form.Handle);

            using var emf = new EmfScope();

            control.PrintToMetafile(emf);

            DeviceContextState state = new DeviceContextState(emf);
            emf.Validate(
                state
                // TODO Add validation
            );
        }
    }
}
