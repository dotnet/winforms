// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public class NumericUpDownTests
{
    [WinFormsFact]
    public void NumericUpDown_Constructor()
    {
        using NumericUpDown nud = new();
        Assert.NotNull(nud);
        Assert.Equal("0", nud.Text);
    }

    [WinFormsFact]
    public void NumericUpDown_VisualStyles_off_BasicRendering_ControlEnabled()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Assert.Equal(new Rectangle(0, 0, 120, 23), upDown.Bounds);

        // The rendering here is the "fill" for the background around the child edit control, which
        // doesn't match up to the main control's bounds.
        upDown.PrintToMetafile(emf);
        emf.Validate(
            state,
            Validate.Rectangle(
                new Rectangle(1, 1, 98, 17),
                State.Pen(2, upDown.BackColor, PEN_STYLE.PS_SOLID)));

        // Printing the main control doesn't get the redraw for the child controls on the first render,
        // directly hitting the up/down button subcontrol.
        using EmfScope emfButtons = new();
        state = new DeviceContextState(emfButtons);
        upDown.Controls[0].PrintToMetafile(emfButtons);

        // This is the "fill" line under the up/down arrows
        emfButtons.Validate(
            state,
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.LineTo(
                new(0, 18), new(16, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)));
    }

    [WinFormsFact]
    public void NumericUpDown_VisualStyles_off_BasicRendering_ControlDisabled()
    {
        if (Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        // Check the disabled state
        upDown.Enabled = false;

        using EmfScope emfDisabled = new();
        DeviceContextState state = new(emfDisabled);
        upDown.PrintToMetafile(emfDisabled);

        emfDisabled.Validate(
            state,
            Validate.Rectangle(
                new Rectangle(0, 0, 99, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)),
            Validate.Rectangle(
                new Rectangle(1, 1, 97, 16),
                State.Pen(1, SystemColors.Control, PEN_STYLE.PS_SOLID)),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS),
            Validate.LineTo(
                new(0, 18), new(16, 18),
                State.Pen(1, upDown.BackColor, PEN_STYLE.PS_SOLID)));
    }

    [WinFormsFact(Skip = "TODO, refer to https://github.com/dotnet/winforms/issues/4212")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/4212")]
    public void NumericUpDown_VisualStyles_on_BasicRendering()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using Form form = new();
        using NumericUpDown upDown = new();

        form.Controls.Add(upDown);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        Assert.Equal(new Rectangle(0, 0, 120, 23), upDown.Bounds);

        // The rendering here is the "fill" for the background around the child edit control, which
        // doesn't match up to the main control's bounds.
        upDown.PrintToMetafile(emf);

        /*

        [ENHMETAHEADER] Bounds: {0, 0, 119, 22 (LTRB)} Device Size: {Width=3840, Height=2160} Header Size: 108
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 0, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 0, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 0, 119, 0 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 22, 119, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSETBKCOLOR] Color: [R=171, G=173, B=179]
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 0, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {119, 22, 119, 22 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 22, 119, 22 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMREXTTEXTOUTW] Bounds: {0, 0, -1, -1 (LTRB)} Text: ''
        [EMRSETBKCOLOR] Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRCREATEPEN] Index: 1 Style: ENDCAP_ROUND Width: {X=1,Y=0} Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSELECTOBJECT] Index: 1
        [EMRSETROP2] Mode: R2_COPYPEN
        [EMRSELECTOBJECT] StockObject: NULL_BRUSH
        [EMRRECTANGLE] RECT: {1, 1, 102, 21 (LTRB)}
        [EMRSELECTOBJECT] StockObject: WHITE_BRUSH
        [EMRSELECTOBJECT] StockObject: BLACK_PEN
        [EMRDELETEOBJECT] Index: 1
        [EMREOF]

         */

        // Printing the main control doesn't get the redraw for the child controls on the first render,
        // directly hitting the up/down button subcontrol.

        using EmfScope emfButtons = new();
        state = new DeviceContextState(emfButtons);
        upDown.Controls[0].PrintToMetafile(emfButtons);

        /*

        [ENHMETAHEADER] Bounds: {0, 0, 16, 20 (LTRB)} Device Size: {Width=3840, Height=2160} Header Size: 108
        [EMRINTERSECTCLIPRECT] RECT: {0, 0, 16, 10 (LTRB)}
        [EMRBITBLT] Bounds: {0, 0, 15, 9 (LTRB)} Destination: {0, 0, 16, 10 (LTRB)} Rop: SRCCOPY Source DC Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {4, 2, 11, 8 (LTRB)}
        [EMRALPHABLEND]
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {0, 10, 16, 20 (LTRB)}
        [EMRBITBLT] Bounds: {0, 10, 15, 19 (LTRB)} Destination: {0, 10, 16, 20 (LTRB)} Rop: SRCCOPY Source DC Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRINTERSECTCLIPRECT] RECT: {4, 12, 11, 18 (LTRB)}
        [EMRALPHABLEND]
        [EMREXTSELECTCLIPRGN] Mode: COPY Bounds: {0, 0, 16, 21 (LTRB)} Rects: 1 Rect index 0: {0, 0, 16, 21 (LTRB)}
        [EMRSETROP2] Mode: R2_COPYPEN
        [EMRSETBKMODE] Mode: BKMODE_TRANSPARENT
        [EMRCREATEPEN] Index: 1 Style: ENDCAP_ROUND Width: {X=1,Y=0} Color: [R=255, G=255, B=255] (COLOR_WINDOW, COLOR_HIGHLIGHTTEXT, COLOR_BTNHIGHLIGHT)
        [EMRSELECTOBJECT] Index: 1
        [EMRMOVETOEX] Point: {X=0,Y=20}
        [EMRLINETO] Point: {X=16,Y=20}
        [EMRMOVETOEX] Point: {X=0,Y=0}
        [EMRSELECTOBJECT] StockObject: BLACK_PEN
        [EMRSETBKMODE] Mode: BKMODE_OPAQUE
        [EMRDELETEOBJECT] Index: 1
        [EMREOF]

        */
    }
}
