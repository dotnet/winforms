// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    public enum EnumProperty
    {
        BackgroundType = 4001,
        BorderType = 4002,
        FillType = 4003,
        SizingType = 4004,
        HorizontalAlignment = 4005,
        ContentAlignment = 4006,
        VerticalAlignment = 4007,
        OffsetType = 4008,
        IconEffect = 4009,
        TextShadowType = 4010,
        ImageLayout = 4011,
        GlyphType = 4012,
        ImageSelectType = 4013,
        GlyphFontSizingType = 4014,
        TrueSizeScalingType = 4015
        //		TM_PROP(4001, TMT, BGTYPE,           ENUM)        // basic drawing type for each part
        //		TM_PROP(4002, TMT, BORDERTYPE,       ENUM)        // type of border for BorderFill parts
        //		TM_PROP(4003, TMT, FILLTYPE,         ENUM)        // fill shape for BorderFill parts
        //		TM_PROP(4004, TMT, SIZINGTYPE,       ENUM)        // how to size ImageFile parts
        //		TM_PROP(4005, TMT, HALIGN,           ENUM)        // horizontal alignment for TRUESIZE parts & glyphs
        //		TM_PROP(4006, TMT, CONTENTALIGNMENT, ENUM)        // custom window prop: how text is aligned in caption
        //		TM_PROP(4007, TMT, VALIGN,           ENUM)        // horizontal alignment for TRUESIZE parts & glyphs
        //		TM_PROP(4008, TMT, OFFSETTYPE,       ENUM)        // how window part should be placed
        //		TM_PROP(4009, TMT, ICONEFFECT,       ENUM)        // type of effect to use with DrawThemeIcon
        //		TM_PROP(4010, TMT, TEXTSHADOWTYPE,   ENUM)        // type of shadow to draw with text
        //		TM_PROP(4011, TMT, IMAGELAYOUT,      ENUM)        // how multiple images are arranged (horz. or vert.)
        //		TM_PROP(4012, TMT, GLYPHTYPE,             ENUM)   // controls type of glyph in imagefile objects
        //		TM_PROP(4013, TMT, IMAGESELECTTYPE,       ENUM)   // controls when to select from IMAGEFILE1...IMAGEFILE5
        //		TM_PROP(4014, TMT, GLYPHFONTSIZINGTYPE,   ENUM)   // controls when to select a bigger/small glyph font size
        //		TM_PROP(4015, TMT, TRUESIZESCALINGTYPE,   ENUM)   // controls how TrueSize image is scaled
    }
}
