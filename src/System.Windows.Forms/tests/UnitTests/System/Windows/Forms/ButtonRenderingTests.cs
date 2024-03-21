// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public class ButtonRenderingTests : AbstractButtonBaseTests
{
    [WinFormsFact]
    public unsafe void CaptureButton()
    {
        using Button button = (Button)CreateButton();
        using EmfScope emf = new();
        button.PrintToMetafile(emf);

        List<ENHANCED_METAFILE_RECORD_TYPE> types = [];
        List<string> details = [];
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

        using Button button = (Button)CreateButton();
        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = button.Bounds;

        button.PrintToMetafile(emf);

        emf.Validate(
            state,
            Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT), 1),
            Validate.LineTo(
                new(bounds.Right - 1, 0), new(0, 0),
                State.PenColor(SystemColors.ControlLightLight)),
            Validate.LineTo(
                new(0, 0), new(0, bounds.Bottom - 1),
                State.PenColor(SystemColors.ControlLightLight)),
            Validate.LineTo(
                new(0, bounds.Bottom - 1), new(bounds.Right - 1, bounds.Bottom - 1),
                State.PenColor(SystemColors.ControlDarkDark)),
            Validate.LineTo(
                new(bounds.Right - 1, bounds.Bottom - 1), new(bounds.Right - 1, -1),
                State.PenColor(SystemColors.ControlDarkDark)),
            Validate.LineTo(
                new(bounds.Right - 2, 1), new(1, 1),
                State.PenColor(SystemColors.Control)),
            Validate.LineTo(
                new(1, 1), new(1, bounds.Bottom - 2),
                State.PenColor(SystemColors.Control)),
            Validate.LineTo(
                new(1, bounds.Bottom - 2), new(bounds.Right - 2, bounds.Bottom - 2),
                State.PenColor(SystemColors.ControlDark)),
            Validate.LineTo(
                new(bounds.Right - 2, bounds.Bottom - 2), new(bounds.Right - 2, 0),
                State.PenColor(SystemColors.ControlDark)));
    }

    [WinFormsFact]
    public unsafe void Button_VisualStyles_on_Default_LineDrawing()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using Button button = (Button)CreateButton();
        using EmfScope emf = new();
        DeviceContextState state = new(emf);
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

        using Button button = (Button)CreateButton();
        button.Text = "Hello";
        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = button.Bounds;

        button.PrintToMetafile(emf);

        emf.Validate(
            state,
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT),
            Validate.TextOut("Hello"),
            Validate.LineTo(
                new(bounds.Right - 1, 0), new(0, 0),
                State.PenColor(SystemColors.ControlLightLight)),
            Validate.LineTo(
                new(0, 0), new(0, bounds.Bottom - 1),
                State.PenColor(SystemColors.ControlLightLight)),
            Validate.LineTo(
                new(0, bounds.Bottom - 1), new(bounds.Right - 1, bounds.Bottom - 1),
                State.PenColor(SystemColors.ControlDarkDark)),
            Validate.LineTo(
                new(bounds.Right - 1, bounds.Bottom - 1), new(bounds.Right - 1, -1),
                State.PenColor(SystemColors.ControlDarkDark)),
            Validate.LineTo(
                new(bounds.Right - 2, 1), new(1, 1),
                State.PenColor(SystemColors.Control)),
            Validate.LineTo(
                new(1, 1), new(1, bounds.Bottom - 2),
                State.PenColor(SystemColors.Control)),
            Validate.LineTo(
                new(1, bounds.Bottom - 2), new(bounds.Right - 2, bounds.Bottom - 2),
                State.PenColor(SystemColors.ControlDark)),
            Validate.LineTo(
                new(bounds.Right - 2, bounds.Bottom - 2), new(bounds.Right - 2, 0),
                State.PenColor(SystemColors.ControlDark)));
    }

    [WinFormsFact]
    public unsafe void Button_VisualStyles_on_Default_WithText_LineDrawing()
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using Button button = (Button)CreateButton();
        button.Text = "Hello";
        using EmfScope emf = new();
        DeviceContextState state = new(emf);
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
        using Form form = new();
        using Button button = (Button)CreateButton();
        form.Controls.Add(button);

        using EmfScope emf = new();
        form.PrintToMetafile(emf);

        string details = emf.RecordsToString();
    }

    [WinFormsFact]
    public unsafe void Button_FlatStyle_WithText_Rectangle()
    {
        using Button button = (Button)CreateButton();
        button.Text = "Flat Style";
        button.FlatStyle = FlatStyle.Flat;

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = button.Bounds;

        button.PrintToMetafile(emf);

        emf.Validate(
            state,
            Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_BITBLT),
            Validate.TextOut("Flat Style"),
            Validate.Rectangle(
                new Rectangle(0, 0, button.Width - 1, button.Height - 1),
                State.PenColor(Color.Black),
                State.PenStyle(PEN_STYLE.PS_ENDCAP_ROUND),
                State.BrushStyle(BRUSH_STYLE.BS_NULL),       // Regressed in https://github.com/dotnet/winforms/pull/3667
                State.Rop2(R2_MODE.R2_COPYPEN)));
    }

    protected override ButtonBase CreateButton() => new Button();
}
