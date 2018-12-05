// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the enums defining various ThemeData Types and Properties.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {
using System.Diagnostics.CodeAnalysis;

    public enum BackgroundType 
    {
    	ImageFile = 0,
    	BorderFill = 1,
    	None = 2,
    //		TM_ENUM(0, BT, IMAGEFILE)
    //		TM_ENUM(1, BT, BORDERFILL)
    //		TM_ENUM(2, BT, NONE)
    }

    public enum BorderType 
    {
    	Rectangle = 0,
    	RoundedRectangle = 1,
    	Ellipse = 2,
    //		TM_ENUM(0, BT, RECT)
    //		TM_ENUM(1, BT, ROUNDRECT)
    //		TM_ENUM(2, BT, ELLIPSE)
    }
    
    public enum ImageOrientation 
    {
    	Vertical = 0,
    	Horizontal = 1,
    //		TM_ENUM(0, IL, VERTICAL)
    //		TM_ENUM(1, IL, HORIZONTAL)
    }
    
    public enum SizingType 
    {
    	FixedSize = 0,
    	Stretch = 1,
    	Tile = 2,
    //		TM_ENUM(0, ST, TRUESIZE)
    //		TM_ENUM(1, ST, STRETCH)
    //		TM_ENUM(2, ST, TILE)
    }
    
    public enum FillType 
    {
    	Solid = 0,
    	VerticalGradient = 1,
    	HorizontalGradient = 2,
    	RadialGradient = 3,
    	TileImage = 4,
    //		TM_ENUM(0, FT, SOLID)
    //		TM_ENUM(1, FT, VERTGRADIENT)
    //		TM_ENUM(2, FT, HORZGRADIENT)
    //		TM_ENUM(3, FT, RADIALGRADIENT)
    //		TM_ENUM(4, FT, TILEIMAGE)
    }
    
    public enum HorizontalAlign 
    {
    	Left = 0,
    	Center = 1,
    	Right = 2,
    //		TM_ENUM(0, HA, LEFT)
    //		TM_ENUM(1, HA, CENTER)
    //		TM_ENUM(2, HA, RIGHT)
    }
    
    public enum ContentAlignment 
    {
    	Left = 0,
    	Center = 1,
    	Right = 2,
    //		TM_ENUM(0, CA, LEFT)
    //		TM_ENUM(1, CA, CENTER)
    //		TM_ENUM(2, CA, RIGHT)
    }
    
    public enum VerticalAlignment 
    {
    	Top = 0,
    	Center = 1,
    	Bottom = 2,
    //		TM_ENUM(0, VA, TOP)
    //		TM_ENUM(1, VA, CENTER)
    //		TM_ENUM(2, VA, BOTTOM)
    }
    
    public enum OffsetType 
    {
    	TopLeft = 0,
    	TopRight = 1,
    	TopMiddle = 2,
    	BottomLeft = 3,
    	BottomRight = 4,
    	BottomMiddle = 5,
    	MiddleLeft = 6,
    	MiddleRight = 7,
    	LeftOfCaption = 8,
    	RightOfCaption = 9,
    	LeftOfLastButton = 10,
    	RightOfLastButton = 11,
    	AboveLastButton = 12,
    	BelowLastButton = 13,
    //		TM_ENUM(0, OT, TOPLEFT)
    //		TM_ENUM(1, OT, TOPRIGHT)
    //		TM_ENUM(2, OT, TOPMIDDLE)
    //		TM_ENUM(3, OT, BOTTOMLEFT)
    //		TM_ENUM(4, OT, BOTTOMRIGHT)
    //		TM_ENUM(5, OT, BOTTOMMIDDLE)
    //		TM_ENUM(6, OT, MIDDLELEFT)
    //		TM_ENUM(7, OT, MIDDLERIGHT)
    //		TM_ENUM(8, OT, LEFTOFCAPTION)
    //		TM_ENUM(9, OT, RIGHTOFCAPTION)
    //		TM_ENUM(10, OT, LEFTOFLASTBUTTON)
    //		TM_ENUM(11, OT, RIGHTOFLASTBUTTON)
    //		TM_ENUM(12, OT, ABOVELASTBUTTON)
    //		TM_ENUM(13, OT, BELOWLASTBUTTON)
    }
    
    public enum IconEffect 
    {
    	None = 0,
    	Glow = 1,
    	Shadow = 2,
    	Pulse = 3,
    	Alpha = 4,
    //		TM_ENUM(0, ICE, NONE)
    //		TM_ENUM(1, ICE, GLOW)
    //		TM_ENUM(2, ICE, SHADOW)
    //		TM_ENUM(3, ICE, PULSE)
    //		TM_ENUM(4, ICE, ALPHA)
    }
    
    public enum TextShadowType 
    {
    	None = 0,
    	Single = 1,
    	Continuous = 2,
    //		TM_ENUM(0, TST, NONE)
    //		TM_ENUM(1, TST, SINGLE)
    //		TM_ENUM(2, TST, CONTINUOUS)
    }
    
    public enum GlyphType 
    {
    	None = 0,
    	ImageGlyph = 1,
    	FontGlyph = 2,
    //		TM_ENUM(0, GT, NONE)
    //		TM_ENUM(1, GT, IMAGEGLYPH)
    //		TM_ENUM(2, GT, FONTGLYPH)
    }
    
    public enum ImageSelectType 
    {
    	None = 0,
    	Size = 1,
    	Dpi = 2,
    //		TM_ENUM(0, IST, NONE)
    //		TM_ENUM(1, IST, SIZE)
    //		TM_ENUM(2, IST, DPI)
    }
    
    public enum TrueSizeScalingType 
    {
    	None = 0,
    	Size = 1,
    	Dpi = 2,
    //		TM_ENUM(0, TSST, NONE)
    //		TM_ENUM(1, TSST, SIZE)
    //		TM_ENUM(2, TSST, DPI)
    }
    
    public enum GlyphFontSizingType 
    {
    	None = 0,
    	Size = 1,
    	Dpi = 2,
    //		TM_ENUM(0, GFST, NONE)
    //		TM_ENUM(1, GFST, SIZE)
    //		TM_ENUM(2, GFST, DPI)
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum ColorProperty 
    {
    	BorderColor = 3801,
    	FillColor = 3802,
    	TextColor = 3803,
    	EdgeLightColor = 3804,
    	EdgeHighlightColor = 3805,
    	EdgeShadowColor = 3806,
    	EdgeDarkShadowColor = 3807,
    	EdgeFillColor = 3808,
    	TransparentColor = 3809,
    	GradientColor1 = 3810,
    	GradientColor2 = 3811,
    	GradientColor3 = 3812,
    	GradientColor4 = 3813,
    	GradientColor5 = 3814,
    	ShadowColor = 3815,
    	GlowColor = 3816,
    	TextBorderColor = 3817,
    	TextShadowColor = 3818,
    	GlyphTextColor = 3819,
    	GlyphTransparentColor = 3820,
    	FillColorHint = 3821,
    	BorderColorHint = 3822,
    	AccentColorHint = 3823
    //		TM_PROP(3801, TMT, BORDERCOLOR,      COLOR)       // color of borders for BorderFill 
    //		TM_PROP(3802, TMT, FILLCOLOR,        COLOR)       // color of bg fill 
    //		TM_PROP(3803, TMT, TEXTCOLOR,        COLOR)       // color text is drawn in
    //		TM_PROP(3804, TMT, EDGELIGHTCOLOR,     COLOR)     // edge color
    //		TM_PROP(3805, TMT, EDGEHIGHLIGHTCOLOR, COLOR)     // edge color
    //		TM_PROP(3806, TMT, EDGESHADOWCOLOR,    COLOR)     // edge color
    //		TM_PROP(3807, TMT, EDGEDKSHADOWCOLOR,  COLOR)     // edge color
    //		TM_PROP(3808, TMT, EDGEFILLCOLOR,  COLOR)         // edge color
    //		TM_PROP(3809, TMT, TRANSPARENTCOLOR, COLOR)       // color of pixels that are treated as transparent (not drawn)
    //		TM_PROP(3810, TMT, GRADIENTCOLOR1,   COLOR)       // first color in gradient
    //		TM_PROP(3811, TMT, GRADIENTCOLOR2,   COLOR)       // second color in gradient
    //		TM_PROP(3812, TMT, GRADIENTCOLOR3,   COLOR)       // third color in gradient
    //		TM_PROP(3813, TMT, GRADIENTCOLOR4,   COLOR)       // forth color in gradient
    //		TM_PROP(3814, TMT, GRADIENTCOLOR5,   COLOR)       // fifth color in gradient
    //		TM_PROP(3815, TMT, SHADOWCOLOR,      COLOR)       // color of text shadow
    //		TM_PROP(3816, TMT, GLOWCOLOR,        COLOR)       // color of glow produced by DrawThemeIcon
    //		TM_PROP(3817, TMT, TEXTBORDERCOLOR,  COLOR)       // color of text border
    //		TM_PROP(3818, TMT, TEXTSHADOWCOLOR,  COLOR)       // color of text shadow
    //		TM_PROP(3819, TMT, GLYPHTEXTCOLOR,        COLOR)  // color that font-based glyph is drawn with
    //		TM_PROP(3820, TMT, GLYPHTRANSPARENTCOLOR, COLOR)  // color of transparent pixels in GlyphImageFile
    //		TM_PROP(3821, TMT, FILLCOLORHINT, COLOR)          // hint about fill color used (for custom controls)
    //		TM_PROP(3822, TMT, BORDERCOLORHINT, COLOR)        // hint about border color used (for custom controls)
    //		TM_PROP(3823, TMT, ACCENTCOLORHINT, COLOR)        // hint about accent color used (for custom controls)
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // EnumProperty maps to native enum.
    ]
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
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum FilenameProperty 
    {
    	ImageFile = 3001,
    	ImageFile1 = 3002,
    	ImageFile2 = 3003,
    	ImageFile3 = 3004,
    	ImageFile4 = 3005,
    	ImageFile5 = 3006,
    	StockImageFile = 3007,
    	GlyphImageFile = 3008
    //		TM_PROP(3001, TMT, IMAGEFILE,         FILENAME)   // the filename of the image (or basename, for mult. images)
    //		TM_PROP(3002, TMT, IMAGEFILE1,        FILENAME)   // multiresolution image file
    //		TM_PROP(3003, TMT, IMAGEFILE2,        FILENAME)   // multiresolution image file
    //		TM_PROP(3004, TMT, IMAGEFILE3,        FILENAME)   // multiresolution image file
    //		TM_PROP(3005, TMT, IMAGEFILE4,        FILENAME)   // multiresolution image file
    //		TM_PROP(3006, TMT, IMAGEFILE5,        FILENAME)   // multiresolution image file
    //		TM_PROP(3007, TMT, STOCKIMAGEFILE,    FILENAME)   // These are the only images that you can call GetThemeBitmap on
    //		TM_PROP(3008, TMT, GLYPHIMAGEFILE,    FILENAME)   // the filename for the glyph image
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum FontProperty 
    {
    	GlyphFont = 2601
    //		 TM_PROP(2601, TMT, GLYPHFONT,         FONT)   // the font that the glyph is drawn with
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum IntegerProperty 
    {
    	ImageCount = 2401,
    	AlphaLevel = 2402,
    	BorderSize = 2403,
    	RoundCornerWidth = 2404,
    	RoundCornerHeight = 2405,
    	GradientRatio1 = 2406,
    	GradientRatio2 = 2407,
    	GradientRatio3 = 2408,
    	GradientRatio4 = 2409,
    	GradientRatio5 = 2410,
    	ProgressChunkSize = 2411,
    	ProgressSpaceSize = 2412,
    	Saturation = 2413,
    	TextBorderSize = 2414,
    	AlphaThreshold = 2415,
    	Width = 2416,
    	Height = 2417,
    	GlyphIndex = 2418,
    	TrueSizeStretchMark = 2419,
    	MinDpi1 = 2420,
    	MinDpi2 = 2421,
    	MinDpi3 = 2422,
    	MinDpi4 = 2423,
    	MinDpi5 = 2424
    //		TM_PROP(2401, TMT, IMAGECOUNT,        INT)    // the number of state images in an imagefile
    //		TM_PROP(2402, TMT, ALPHALEVEL,        INT)    // (0-255) alpha value for an icon (DrawThemeIcon part)
    //		TM_PROP(2403, TMT, BORDERSIZE,        INT)    // the size of the border line for bgtype=BorderFill
    //		TM_PROP(2404, TMT, ROUNDCORNERWIDTH,  INT)    // (0-100) % of roundness for rounded rects
    //		TM_PROP(2405, TMT, ROUNDCORNERHEIGHT, INT)    // (0-100) % of roundness for rounded rects
    //		TM_PROP(2406, TMT, GRADIENTRATIO1,    INT)    // (0-255) - amt of gradient color 1 to use (all must total=255)
    //		TM_PROP(2407, TMT, GRADIENTRATIO2,    INT)    // (0-255) - amt of gradient color 2 to use (all must total=255)
    //		TM_PROP(2408, TMT, GRADIENTRATIO3,    INT)    // (0-255) - amt of gradient color 3 to use (all must total=255)
    //		TM_PROP(2409, TMT, GRADIENTRATIO4,    INT)    // (0-255) - amt of gradient color 4 to use (all must total=255)
    //		TM_PROP(2410, TMT, GRADIENTRATIO5,    INT)    // (0-255) - amt of gradient color 5 to use (all must total=255)
    //		TM_PROP(2411, TMT, PROGRESSCHUNKSIZE, INT)    // size of progress control chunks
    //		TM_PROP(2412, TMT, PROGRESSSPACESIZE, INT)    // size of progress control spaces
    //		TM_PROP(2413, TMT, SATURATION,        INT)    // (0-255) amt of saturation for DrawThemeIcon() part
    //		TM_PROP(2414, TMT, TEXTBORDERSIZE,    INT)    // size of border around text chars
    //		TM_PROP(2415, TMT, ALPHATHRESHOLD,    INT)    // (0-255) the min. alpha value of a pixel that is solid
    //		TM_PROP(2416, TMT, WIDTH,             SIZE)   // custom window prop: size of part (min. window)
    //		TM_PROP(2417, TMT, HEIGHT,            SIZE)   // custom window prop: size of part (min. window)
    //		TM_PROP(2418, TMT, GLYPHINDEX,        INT)    // for font-based glyphs, the char index into the font
    //		TM_PROP(2419, TMT, TRUESIZESTRETCHMARK, INT)  // stretch TrueSize image when target exceeds source by this percent
    //		TM_PROP(2420, TMT, MINDPI1,         INT)      // min DPI ImageFile1 was designed for
    //		TM_PROP(2421, TMT, MINDPI2,         INT)      // min DPI ImageFile1 was designed for
    //		TM_PROP(2422, TMT, MINDPI3,         INT)      // min DPI ImageFile1 was designed for
    //		TM_PROP(2423, TMT, MINDPI4,         INT)      // min DPI ImageFile1 was designed for
    //		TM_PROP(2424, TMT, MINDPI5,         INT)      // min DPI ImageFile1 was designed for
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum PointProperty 
    {
    	Offset = 3401,
    	TextShadowOffset = 3402,
    	MinSize = 3403,
    	MinSize1 = 3404,
    	MinSize2 = 3405,
    	MinSize3 = 3406,
    	MinSize4 = 3407,
    	MinSize5 = 3408
    //		TM_PROP(3401, TMT, OFFSET,            POSITION)   // for window part layout
    //		TM_PROP(3402, TMT, TEXTSHADOWOFFSET,  POSITION)   // where char shadows are drawn, relative to orig. chars
    //		TM_PROP(3403, TMT, MINSIZE,           POSITION)   // min dest rect than ImageFile was designed for
    //		TM_PROP(3404, TMT, MINSIZE1,          POSITION)   // min dest rect than ImageFile1 was designed for
    //		TM_PROP(3405, TMT, MINSIZE2,          POSITION)   // min dest rect than ImageFile2 was designed for
    //		TM_PROP(3406, TMT, MINSIZE3,          POSITION)   // min dest rect than ImageFile3 was designed for
    //		TM_PROP(3407, TMT, MINSIZE4,          POSITION)   // min dest rect than ImageFile4 was designed for
    //		TM_PROP(3408, TMT, MINSIZE5,          POSITION)   // min dest rect than ImageFile5 was designed for
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum MarginProperty 
    {
    	SizingMargins = 3601,
    	ContentMargins = 3602,
    	CaptionMargins = 3603
    //		TM_PROP(3601, TMT, SIZINGMARGINS,     MARGINS)    // margins used for 9-grid sizing
    //		TM_PROP(3602, TMT, CONTENTMARGINS,    MARGINS)    // margins that define where content can be placed
    //		TM_PROP(3603, TMT, CAPTIONMARGINS,    MARGINS)    // margins that define where caption text can be placed
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum StringProperty 
    {
    	Text = 3201   
    	//TM_PROP(3201, TMT, TEXT,              STRING)
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
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

    // Some other misc enums
    
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum Edges 
    {
        Left = 0x0001,
        Top = 0x0002,
        Right = 0x0004,
        Bottom = 0x0008,
        Diagonal = 0x0010,
        
    //		#define BF_LEFT         0x0001
    //		#define BF_TOP          0x0002
    //		#define BF_RIGHT        0x0004
    //		#define BF_BOTTOM       0x0008
    //
    //		#define BF_TOPLEFT      (BF_TOP | BF_LEFT)
    //		#define BF_TOPRIGHT     (BF_TOP | BF_RIGHT)
    //		#define BF_BOTTOMLEFT   (BF_BOTTOM | BF_LEFT)
    //		#define BF_BOTTOMRIGHT  (BF_BOTTOM | BF_RIGHT)
    //		#define BF_RECT         (BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM)
    //
    //		#define BF_DIAGONAL     0x0010
    
    //		// For diagonal lines, the BF_RECT flags specify the end point of the
    //		// vector bounded by the rectangle parameter.
    //		#define BF_DIAGONAL_ENDTOPRIGHT     (BF_DIAGONAL | BF_TOP | BF_RIGHT)
    //		#define BF_DIAGONAL_ENDTOPLEFT      (BF_DIAGONAL | BF_TOP | BF_LEFT)
    //		#define BF_DIAGONAL_ENDBOTTOMLEFT   (BF_DIAGONAL | BF_BOTTOM | BF_LEFT)
    //		#define BF_DIAGONAL_ENDBOTTOMRIGHT  (BF_DIAGONAL | BF_BOTTOM | BF_RIGHT)
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum EdgeStyle 
    {
        Raised = 0x0001 | 0x0004,
    	Sunken = 0x0002 | 0x0008,
        Etched = 0x0002 | 0x0004,
        Bump = 0x0001 | 0x0008
    	
        //		#define BDR_RAISEDOUTER 0x0001
    	//		#define BDR_SUNKENOUTER 0x0002
    	//		#define BDR_RAISEDINNER 0x0004
    	//		#define BDR_SUNKENINNER 0x0008
    	//		#define EDGE_RAISED     (BDR_RAISEDOUTER | BDR_RAISEDINNER)
    	//		#define EDGE_SUNKEN     (BDR_SUNKENOUTER | BDR_SUNKENINNER)
    	//		#define EDGE_ETCHED     (BDR_SUNKENOUTER | BDR_RAISEDINNER)
    	//		#define EDGE_BUMP       (BDR_RAISEDOUTER | BDR_SUNKENINNER)
    }
    
    [Flags]
    public enum EdgeEffects 
    {
        None = 0,
        FillInterior = 0x0800,
    	Flat = 0x1000,
    	Soft = 0x4000,
    	Mono = 0x8000,
    //	#define BF_SOFT         0x1000  /* For softer buttons */
    //	#define BF_FLAT         0x4000  /* For flat rather than 3D borders */
    //	#define BF_MONO         0x8000  /* For monochrome borders */
    }
    
	public struct TextMetrics
	{
		
		private int height; 
		private int ascent; 
		private int descent; 
		private int internalLeading; 
		private int externalLeading; 
		private int aveCharWidth; 
		private int maxCharWidth; 
		private int weight; 
		private int overhang; 
		private int digitizedAspectX; 
		private int digitizedAspectY; 
		private char firstChar; 
		private char lastChar; 
		private char defaultChar; 
		private char breakChar; 
		private bool italic; 
		private bool underlined; 
		private bool struckOut; 
		private TextMetricsPitchAndFamilyValues pitchAndFamily; 
		private TextMetricsCharacterSet charSet; 

		public int Height 
		{ 
			get
			{
				return height;
			} set 
			  { 
				  height = value;
			  }
		} 

		public int Ascent 
		{ 
			get
			{
				return ascent;
			} 
			set 
			{ 
				ascent = value;
			}
		} 
		public int Descent 
		{ 
			get
			{
				return descent;
			} 
			set 
			{ 
				descent = value;
			}
		} 

		public int InternalLeading 
		{ 
			get
			{
				return internalLeading;
			} 
			set 
			{ 
				internalLeading = value;
			}
		} 

		public int ExternalLeading 
		{ 
			get
			{
				return externalLeading;
			} 
			set 
			{ 
				externalLeading = value;
			}
		} 

		public int AverageCharWidth 
		{ 
			get
			{
				return aveCharWidth;
			} 
			set 
			{ 
				aveCharWidth = value;
			}
		} 

		public int MaxCharWidth 
		{ 
			get
			{
				return maxCharWidth;
			} 
			set 
			{ 
				maxCharWidth = value;
			}
		} 

		public int Weight 
		{ 
			get
			{
				return weight;
			} 
			set 
			{ 
				weight = value;
			}
		} 

		public int Overhang 
		{ 
			get
			{
				return overhang;
			} 
			set 
			{ 
				overhang = value;
			}
		} 

		public int DigitizedAspectX 
		{ 
			get
			{
				return digitizedAspectX;
			} 
			set 
			{ 
				digitizedAspectX = value;
			}
		} 

		public int DigitizedAspectY 
		{ 
			get
			{
				return digitizedAspectY;
			} 
			set 
			{ 
				digitizedAspectY = value;
			}
		} 

		public char FirstChar 
		{ 
			get
			{
				return firstChar;
			} 
			set 
			{ 
				firstChar = value;
			}
		} 

		public char LastChar 
		{ 
			get
			{
				return lastChar;
			} 
			set 
			{ 
				lastChar = value;
			}
		} 

		public char DefaultChar 
		{ 
			get
			{
				return defaultChar;
			} 
			set 
			{ 
				defaultChar = value;
			}
		} 

		public char BreakChar 
		{ 
			get
			{
				return breakChar;
			} 
			set 
			{ 
				breakChar = value;
			}
		} 

		public bool Italic 
		{ 
			get
			{
				return italic;
			} 
			set 
			{ 
				italic = value;
			}
		} 

		public bool Underlined 
		{ 
			get
			{
				return underlined;
			} 
			set 
			{ 
				underlined = value;
			}
		} 

		public bool StruckOut 
		{ 
			get
			{
				return struckOut;
			} 
			set 
			{ 
				struckOut = value;
			}
		} 

		public TextMetricsPitchAndFamilyValues PitchAndFamily 
		{ 
			get
			{
				return pitchAndFamily;
			} 
			set 
			{ 
				pitchAndFamily = value;
			}
		} 

		public TextMetricsCharacterSet CharSet 
		{ 
			get
			{
				return charSet;
			} 
			set 
			{ 
				charSet = value;
			}
		}
	}
    
    [Flags]
    public enum TextMetricsPitchAndFamilyValues 
    {
    	FixedPitch = 0x01,
    	Vector = 0x02,
        TrueType = 0x04,
    	Device = 0x08
    	
    //		#define TMPF_FIXED_PITCH    0x01
    //		#define TMPF_VECTOR             0x02
    //		#define TMPF_DEVICE             0x08
    //		#define TMPF_TRUETYPE       0x04
    }
    
    public enum TextMetricsCharacterSet 
    {
    	Ansi = 0,
    	Baltic = 186,
    	ChineseBig5 = 136,
    	Default = 1,
    	EastEurope = 238,
    	Gb2312 = 134,
    	Greek = 161,
    	Hangul = 129,
    	Mac = 77,
    	Oem = 255,
    	Russian = 204,
    	ShiftJis = 128,
    	Symbol = 2,
    	Turkish = 162,
    	Vietnamese = 163,
    	Johab = 130,
    	Arabic = 178,
    	Hebrew = 177,
    	Thai = 222,
    	//		#define ANSI_CHARSET            0
    	//		#define BALTIC_CHARSET          186
    	//		#define CHINESEBIG5_CHARSET     136
    	//		#define DEFAULT_CHARSET         1
    	//		#define EASTEUROPE_CHARSET      238
    	//		#define GB2312_CHARSET          134
    	//		#define GREEK_CHARSET           161
    	//		#define HANGUL_CHARSET          129
    	//		#define MAC_CHARSET             77
    	//		#define OEM_CHARSET             255
    	//		#define RUSSIAN_CHARSET         204
    	//		#define SHIFTJIS_CHARSET        128
    	//		#define SYMBOL_CHARSET          2
    	//		#define TURKISH_CHARSET         162
    	//		#define VIETNAMESE_CHARSET      163
    
    	// Korean
    	//		#define JOHAB_CHARSET           130
    
    	// Middle East
    	//		#define ARABIC_CHARSET          178
    	//		#define HEBREW_CHARSET          177
    
    	// Thai
    	//		#define THAI_CHARSET            222
    }
    
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
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
    
    	//  Theme background segment hit test flag (default). possible return values are:
    	//  HTCLIENT: hit test succeeded in the middle background segment
    	//  HTTOP, HTLEFT, HTTOPLEFT, etc:  // hit test succeeded in the the respective theme background segment.
    	//#define HTTB_BACKGROUNDSEG          0x0000  
    
    	//  Fixed border hit test option.  possible return values are:
    	//  HTCLIENT: hit test succeeded in the middle background segment
    	//  HTBORDER: hit test succeeded in any other background segment
    	//#define HTTB_FIXEDBORDER            0x0002  // Return code may be either HTCLIENT or HTBORDER. 
    
    	//  Caption hit test option.  Possible return values are:
    	//  HTCAPTION: hit test succeeded in the top, top left, or top right background segments
    	//  HTNOWHERE or another return code, depending on absence or presence of accompanying flags, resp.
    	//#define HTTB_CAPTION                0x0004  
    
    	//  Resizing border hit test flags.  Possible return values are:
    	//  HTCLIENT: hit test succeeded in middle background segment
    	//  HTTOP, HTTOPLEFT, HTLEFT, HTRIGHT, etc:    hit test succeeded in the respective system resizing zone
    	//  HTBORDER: hit test failed in middle segment and resizing zones, but succeeded in a background border segment
    	//#define HTTB_RESIZINGBORDER_LEFT    0x0010  // Hit test left resizing border, 
    	//#define HTTB_RESIZINGBORDER_TOP     0x0020  // Hit test top resizing border
    	//#define HTTB_RESIZINGBORDER_RIGHT   0x0040  // Hit test right resizing border
    	//#define HTTB_RESIZINGBORDER_BOTTOM  0x0080  // Hit test bottom resizing border
    
    	//#define HTTB_RESIZINGBORDER         (HTTB_RESIZINGBORDER_LEFT|HTTB_RESIZINGBORDER_TOP|\
    	//		HTTB_RESIZINGBORDER_RIGHT|HTTB_RESIZINGBORDER_BOTTOM)
    
    	// Resizing border is specified as a template, not just window edges.
    	// This option is mutually exclusive with HTTB_SYSTEMSIZINGWIDTH; HTTB_SIZINGTEMPLATE takes precedence  
    	//#define HTTB_SIZINGTEMPLATE      0x0100
    
    	// Use system resizing border width rather than theme content margins.   
    	// This option is mutually exclusive with HTTB_SIZINGTEMPLATE, which takes precedence.
    	//#define HTTB_SYSTEMSIZINGMARGINS 0x0200
    }
    
    public enum HitTestCode 
    {
    	Nowhere = 0,
    	Client = 1,
    	Left = 10,
    	Right = 11,
    	Top = 12,
    	Bottom = 15,
    	TopLeft = 13,
    	TopRight = 14,
    	BottomLeft = 16,
    	BottomRight = 17
    //		#define HTNOWHERE           0
    //		#define HTCLIENT            1
    //		#define HTLEFT              10
    //		#define HTRIGHT             11
    //		#define HTTOP               12
    //		#define HTTOPLEFT           13
    //		#define HTTOPRIGHT          14
    //		#define HTBOTTOM            15
    //		#define HTBOTTOMLEFT        16
    //		#define HTBOTTOMRIGHT       17
    
    }

    public enum ThemeSizeType {
        Minimum = 0,
        True = 1,
        Draw = 2
    }

    // Internal enums for VisualStyleInformation

    internal struct VisualStyleDocProperty
    {
        internal static string DisplayName = "DisplayName";
        internal static string Company = "Company";
        internal static string Author = "Author";
        internal static string Copyright = "Copyright";
        internal static string Url = "Url";
        internal static string Version = "Version";
        internal static string Description = "Description";
    }

    internal struct VisualStyleSystemProperty
    {
        internal static int SupportsFlatMenus = 1001;
        internal static int MinimumColorDepth = 1301;
    }
}
