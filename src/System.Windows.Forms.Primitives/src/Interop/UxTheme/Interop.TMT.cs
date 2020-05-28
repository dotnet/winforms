// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        // The values for this enum can be found in vssym32.h.
        // Because it has many values, we only have the ones that we are currently using.
        [Flags]
        public enum TMT
        {
            FONT = 210,

            FLATMENUS = 1001,
            MINCOLORDEPTH = 1301,

            /// <summary>
            /// Image has transparent areas (see TransparentColor).
            /// </summary>
            TRANSPARENT = 2201,

            /// <summary>
            /// If TRUE, nonclient caption width varies with text extent.
            /// </summary>
            AUTOSIZE = 2202,

            /// <summary>
            /// Only draw the border area of the image.
            /// </summary>
            BORDERONLY = 2203,

            /// <summary>
            /// Control will handle the composite drawing.
            /// </summary>
            COMPOSITED = 2204,

            /// <summary>
            /// If TRUE, TRUESIZE images should be drawn on bg fill
            /// </summary>
            BGFILL = 2205,

            /// <summary>
            /// Glyph has transparent areas (see GlyphTransparentColor).
            /// </summary>
            GLYPHTRANSPARENT = 2206,

            /// <summary>
            /// Only draw glyph (not background).
            /// </summary>
            GLYPHONLY = 2207,

            /// <summary>
            /// Default=TRUE means image gets mirrored in RTL (Mirror) windows.
            /// </summary>
            ALWAYSSHOWSIZINGBAR = 2208,

            /// <summary>
            /// If TRUE, height &amp; width must be uniformly sized.
            /// </summary>
            MIRRORIMAGE = 2209,

            /// <summary>
            /// For TRUESIZE and Border sizing; if TRUE, factor must be integer.
            /// </summary>
            UNIFORMSIZING = 2210,

            /// <summary>
            /// If TRUE, will scale up src image when needed.
            /// </summary>
            INTEGRALSIZING = 2211,

            /// <summary>
            /// If TRUE, will scale down src image when needed.
            /// </summary>
            SOURCEGROW = 2212,
            SOURCESHRINK = 2213,

            /// <summary>
            /// The number of state images in an imagefile.
            /// </summary>
            IMAGECOUNT = 2401,

            /// <summary>
            /// (0-255) alpha value for an icon (DrawThemeIcon part).
            /// </summary>
            ALPHALEVEL = 2402,

            /// <summary>
            /// The size of the border line for bgtype=BorderFill.
            /// </summary>
            BORDERSIZE = 2403,

            /// <summary>
            /// (0-100) % of roundness for rounded rects.
            /// </summary>
            ROUNDCORNERWIDTH = 2404,

            /// <summary>
            /// (0-100) % of roundness for rounded rects.
            /// </summary>
            ROUNDCORNERHEIGHT = 2405,

            /// <summary>
            /// (0-255) - amt of gradient color 1 to use (all must total=255).
            /// </summary>
            GRADIENTRATIO1 = 2406,

            /// <summary>
            /// (0-255) - amt of gradient color 2 to use (all must total=255).
            /// </summary>
            GRADIENTRATIO2 = 2407,

            /// <summary>
            /// (0-255) - amt of gradient color 3 to use (all must total=255).
            /// </summary>
            GRADIENTRATIO3 = 2408,

            /// <summary>
            /// (0-255) - amt of gradient color 4 to use (all must total=255).
            /// </summary>
            GRADIENTRATIO4 = 2409,

            /// <summary>
            /// (0-255) - amt of gradient color 5 to use (all must total=255).
            /// </summary>
            GRADIENTRATIO5 = 2410,

            /// <summary>
            /// Size of progress control chunks.
            /// </summary>
            PROGRESSCHUNKSIZE = 2411,

            /// <summary>
            /// Size of progress control spaces.
            /// </summary>
            PROGRESSSPACESIZE = 2412,

            /// <summary>
            /// (0-255) amt of saturation for DrawThemeIcon() part.
            /// </summary>
            SATURATION = 2413,

            /// <summary>
            /// Size of border around text chars.
            /// </summary>
            TEXTBORDERSIZE = 2414,

            /// <summary>
            /// (0-255) the min. alpha value of a pixel that is solid.
            /// </summary>
            ALPHATHRESHOLD = 2415,

            /// <summary>
            /// Custom window prop: size of part (min. window).
            /// </summary>
            WIDTH = 2416,

            /// <summary>
            /// Custom window prop: size of part (min. window)
            /// </summary>
            HEIGHT = 2417,

            /// <summary>
            /// For font-based glyphs, the char index into the font.
            /// </summary>
            GLYPHINDEX = 2418,

            /// <summary>
            /// Stretch TrueSize image when target exceeds source by this percent.
            /// </summary>
            TRUESIZESTRETCHMARK = 2419,

            /// <summary>
            /// Min DPI ImageFile1 was designed for.
            /// </summary>
            MINDPI1 = 2420,

            /// <summary>
            /// Min DPI ImageFile2 was designed for.
            /// </summary>
            MINDPI2 = 2421,

            /// <summary>
            /// Min DPI ImageFile3 was designed for.
            /// </summary>
            MINDPI3 = 2422,

            /// <summary>
            /// Min DPI ImageFile4 was designed for.
            /// </summary>
            MINDPI4 = 2423,

            /// <summary>
            /// Min DPI ImageFile5 was designed for.
            /// </summary>
            MINDPI5 = 2424,

            /// <summary>
            /// The font that the glyph is drawn with.
            /// </summary>
            GLYPHFONT = 2601,

            /// <summary>
            /// The filename of the image (or basename, for mult. images).
            /// </summary>
            IMAGEFILE = 3001,

            /// <summary>
            /// Multiresolution image file.
            /// </summary>
            IMAGEFILE1 = 3002,

            /// <summary>
            /// Multiresolution image file.
            /// </summary>
            IMAGEFILE2 = 3003,

            /// <summary>
            /// Multiresolution image file.
            /// </summary>
            IMAGEFILE3 = 3004,

            /// <summary>
            /// Multiresolution image file.
            /// </summary>
            IMAGEFILE4 = 3005,

            /// <summary>
            /// Multiresolution image file.
            /// </summary>
            IMAGEFILE5 = 3006,

            /// <summary>
            /// The filename for the glyph image
            /// </summary>
            STOCKIMAGEFILE = 3007,

            /// <summary>
            /// The filename for the glyph image
            /// </summary>
            GLYPHIMAGEFILE = 3008,

            TEXT = 3201,

            /// <summary>
            /// For window part layout.
            /// </summary>
            OFFSET = 3401,

            /// <summary>
            ///  Where char shadows are drawn, relative to orig. chars.
            /// </summary>
            TEXTSHADOWOFFSET = 3402,

            /// <summary>
            /// Min dest rect than ImageFile was designed for.
            /// </summary>
            MINSIZE = 3403,

            /// <summary>
            /// Min dest rect than ImageFile1 was designed for.
            /// </summary>
            MINSIZE1 = 3404,

            /// <summary>
            /// Min dest rect than ImageFile2 was designed for.
            /// </summary>
            MINSIZE2 = 3405,

            /// <summary>
            /// Min dest rect than ImageFile3 was designed for.
            /// </summary>
            MINSIZE3 = 3406,

            /// <summary>
            /// Min dest rect than ImageFile4 was designed for.
            /// </summary>
            MINSIZE4 = 3407,

            /// <summary>
            /// Min dest rect than ImageFile5 was designed for.
            /// </summary>
            MINSIZE5 = 3408,

            /// <summary>
            /// margins used for 9-grid sizing.
            /// </summary>
            SIZINGMARGINS = 3601,

            /// <summary>
            /// margins that define where content can be placed.
            /// </summary>
            CONTENTMARGINS = 3602,

            /// <summary>
            /// margins that define where caption text can be place
            /// </summary>
            CAPTIONMARGINS = 3603,

            /// <summary>
            /// Color of borders for BorderFill.
            /// </summary>
            BORDERCOLOR = 3801,

            /// <summary>
            /// Color of bg fill.
            /// </summary>
            FILLCOLOR = 3802,

            /// <summary>
            /// Color text is drawn in.
            /// </summary>
            TEXTCOLOR = 3803,

            /// <summary>
            /// Edge light color.
            /// </summary>
            EDGELIGHTCOLOR = 3804,

            /// <summary>
            /// Edge highlight color.
            /// </summary>
            EDGEHIGHLIGHTCOLOR = 3805,

            /// <summary>
            /// Edge shadow color.
            /// </summary>
            EDGESHADOWCOLOR = 3806,

            /// <summary>
            /// Edge dark shadow color.
            /// </summary>
            EDGEDKSHADOWCOLOR = 3807,

            /// <summary>
            /// Edge file color.
            /// </summary>
            EDGEFILLCOLOR = 3808,

            /// <summary>
            /// Color of pixels that are treated as transparent (not drawn)
            /// </summary>
            TRANSPARENTCOLOR = 3809,

            /// <summary>
            /// First color in gradient.
            /// </summary>
            GRADIENTCOLOR1 = 3810,

            /// <summary>
            /// Second color in gradient.
            /// </summary>
            GRADIENTCOLOR2 = 3811,

            /// <summary>
            /// Third color in gradient.
            /// </summary>
            GRADIENTCOLOR3 = 3812,

            /// <summary>
            /// Forth color in gradient.
            /// </summary>
            GRADIENTCOLOR4 = 3813,

            /// <summary>
            /// Fifth color in gradient.
            /// </summary>
            GRADIENTCOLOR5 = 3814,

            /// <summary>
            /// Color of text shadow.
            /// </summary>
            SHADOWCOLOR = 3815,

            /// <summary>
            /// Color of glow produced by DrawThemeIcon.
            /// </summary>
            GLOWCOLOR = 3816,

            /// <summary>
            /// Color of text border.
            /// </summary>
            TEXTBORDERCOLOR = 3817,

            /// <summary>
            /// Color of text shadow.
            /// </summary>
            TEXTSHADOWCOLOR = 3818,

            /// <summary>
            /// Color that font-based glyph is drawn with.
            /// </summary>
            GLYPHTEXTCOLOR = 3819,

            /// <summary>
            /// Color of transparent pixels in GlyphImageFile.
            /// </summary>
            GLYPHTRANSPARENTCOLOR = 3820,

            /// <summary>
            /// Hint about fill color used (for custom controls).
            /// </summary>
            FILLCOLORHINT = 3821,

            /// <summary>
            /// Hint about border color used (for custom controls).
            /// </summary>
            BORDERCOLORHINT = 3822,

            /// <summary>
            /// Hint about accent color used (for custom controls).
            /// </summary>
            ACCENTCOLORHINT = 3823,

            /// <summary>
            /// Basic drawing type for each part.
            /// </summary>
            BGTYPE = 4001,

            /// <summary>
            /// Type of border for BorderFill parts.
            /// </summary>
            BORDERTYPE = 4002,

            /// <summary>
            /// Fill shape for BorderFill parts.
            /// </summary>
            FILLTYPE = 4003,

            /// <summary>
            /// How to size ImageFile parts.
            /// </summary>
            SIZINGTYPE = 4004,

            /// <summary>
            /// Horizontal alignment for TRUESIZE parts &amp; glyphs.
            /// </summary>
            HALIGN = 4005,

            /// <summary>
            /// Custom window prop: how text is aligned in caption.
            /// </summary>
            CONTENTALIGNMENT = 4006,

            /// <summary>
            /// Horizontal alignment for TRUESIZE parts &amp; glyphs.
            /// </summary>
            VALIGN = 4007,

            /// <summary>
            /// How window part should be placed.
            /// </summary>
            OFFSETTYPE = 4008,

            /// <summary>
            /// Type of effect to use with DrawThemeIcon.
            /// </summary>
            ICONEFFECT = 4009,

            /// <summary>
            /// Type of shadow to draw with text.
            /// </summary>
            TEXTSHADOWTYPE = 4010,

            /// <summary>
            /// How multiple images are arranged (horz. or vert.).
            /// </summary>
            IMAGELAYOUT = 4011,

            /// <summary>
            /// Controls type of glyph in imagefile objects.
            /// </summary>
            GLYPHTYPE = 4012,

            /// <summary>
            /// Controls when to select from IMAGEFILE1...IMAGEFILE5.
            /// </summary>
            IMAGESELECTTYPE = 4013,

            /// <summary>
            /// Controls when to select a bigger/small glyph font size.
            /// </summary>
            GLYPHFONTSIZINGTYPE = 4014,

            /// <summary>
            /// Controls how TrueSize image is scaled
            /// </summary>
            TRUESIZESCALINGTYPE = 4015
        }
    }
}
