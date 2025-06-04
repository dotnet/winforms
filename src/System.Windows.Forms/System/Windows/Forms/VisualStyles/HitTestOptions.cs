// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

[Flags]
public enum HitTestOptions
{
    BackgroundSegment = 0x0000,
    FixedBorder = 0x0002,
    Caption = 0x0004,
    ResizingBorderLeft = 0x0010,
    ResizingBorderTop = 0x0020,
    ResizingBorderRight = 0x0040,
    ResizingBorderBottom = 0x0080,
    ResizingBorder = ResizingBorderLeft | ResizingBorderTop | ResizingBorderRight | ResizingBorderBottom,
    SizingTemplate = 0x0100,
    SystemSizingMargins = 0x0200

    // Theme background segment hit test flag (default). possible return values are:
    //  HTCLIENT: hit test succeeded in the middle background segment
    //  HTTOP, HTLEFT, HTTOPLEFT, etc:  // hit test succeeded in the respective theme background segment.
    // #define HTTB_BACKGROUNDSEG          0x0000

    // Fixed border hit test option. possible return values are:
    //  HTCLIENT: hit test succeeded in the middle background segment
    //  HTBORDER: hit test succeeded in any other background segment
    // #define HTTB_FIXEDBORDER            0x0002  // Return code may be either HTCLIENT or HTBORDER.

    // Caption hit test option. Possible return values are:
    //  HTCAPTION: hit test succeeded in the top, top left, or top right background segments
    //  HTNOWHERE or another return code, depending on absence or presence of accompanying flags, resp.
    // #define HTTB_CAPTION                0x0004

    // Resizing border hit test flags. Possible return values are:
    //  HTCLIENT: hit test succeeded in middle background segment
    //  HTTOP, HTTOPLEFT, HTLEFT, HTRIGHT, etc:    hit test succeeded in the respective system resizing zone
    //  HTBORDER: hit test failed in middle segment and resizing zones, but succeeded in a background border segment
    // #define HTTB_RESIZINGBORDER_LEFT    0x0010  // Hit test left resizing border,
    // #define HTTB_RESIZINGBORDER_TOP     0x0020  // Hit test top resizing border
    // #define HTTB_RESIZINGBORDER_RIGHT   0x0040  // Hit test right resizing border
    // #define HTTB_RESIZINGBORDER_BOTTOM  0x0080  // Hit test bottom resizing border

    // #define HTTB_RESIZINGBORDER         (HTTB_RESIZINGBORDER_LEFT|HTTB_RESIZINGBORDER_TOP|\
    //      HTTB_RESIZINGBORDER_RIGHT|HTTB_RESIZINGBORDER_BOTTOM)

    // Resizing border is specified as a template, not just window edges.
    // This option is mutually exclusive with HTTB_SYSTEMSIZINGWIDTH; HTTB_SIZINGTEMPLATE takes precedence
    // #define HTTB_SIZINGTEMPLATE      0x0100

    // Use system resizing border width rather than theme content margins.
    // This option is mutually exclusive with HTTB_SIZINGTEMPLATE, which takes precedence.
    // #define HTTB_SYSTEMSIZINGMARGINS 0x0200
}
