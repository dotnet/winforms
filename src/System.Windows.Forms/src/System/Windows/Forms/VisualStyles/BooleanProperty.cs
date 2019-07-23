// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    public enum BooleanProperty
    {
        Transparent = 2201,
        AutoSize = 2202,
        BorderOnly = 2203,
        Composited = 2204,
        BackgroundFill = 2205,
        GlyphTransparent = 2206,
        GlyphOnly = 2207,
        AlwaysShowSizingBar = 2208,
        MirrorImage = 2209,
        UniformSizing = 2210,
        IntegralSizing = 2211,
        SourceGrow = 2212,
        SourceShrink = 2213

        // TM_PROP(2201, TMT, TRANSPARENT,   BOOL)       // image has transparent areas (see TransparentColor)
        // TM_PROP(2202, TMT, AUTOSIZE,      BOOL)       // if TRUE, nonclient caption width varies with text extent
        // TM_PROP(2203, TMT, BORDERONLY,    BOOL)       // only draw the border area of the image
        // TM_PROP(2204, TMT, COMPOSITED,    BOOL)       // control will handle the composite drawing
        // TM_PROP(2205, TMT, BGFILL,        BOOL)       // if TRUE, TRUESIZE images should be drawn on bg fill
        // TM_PROP(2206, TMT, GLYPHTRANSPARENT,  BOOL)   // glyph has transparent areas (see GlyphTransparentColor)
        // TM_PROP(2207, TMT, GLYPHONLY,         BOOL)   // only draw glyph (not background)
        // TM_PROP(2208, TMT, ALWAYSSHOWSIZINGBAR, BOOL)
        // TM_PROP(2209, TMT, MIRRORIMAGE,         BOOL) // default=TRUE means image gets mirrored in RTL (Mirror) windows
        // TM_PROP(2210, TMT, UNIFORMSIZING,       BOOL) // if TRUE, height & width must be uniformly sized
        // TM_PROP(2211, TMT, INTEGRALSIZING,      BOOL) // for TRUESIZE and Border sizing; if TRUE, factor must be integer
        // TM_PROP(2212, TMT, SOURCEGROW,          BOOL) // if TRUE, will scale up src image when needed
        // TM_PROP(2213, TMT, SOURCESHRINK,        BOOL) // if TRUE, will scale down src image when needed
    }
}
