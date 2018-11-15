// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the enums defining various ThemeData Types and Properties.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {
using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BackgroundType"]/*' />
    public enum BackgroundType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BackgroundType.ImageFile"]/*' />
    	ImageFile = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BackgroundType.BorderFill"]/*' />
    	BorderFill = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BackgroundType.None"]/*' />
    	None = 2,
    //		TM_ENUM(0, BT, IMAGEFILE)
    //		TM_ENUM(1, BT, BORDERFILL)
    //		TM_ENUM(2, BT, NONE)
    }

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BorderType"]/*' />
    public enum BorderType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BorderType.Rectangle"]/*' />
    	Rectangle = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BorderType.RoundedRectangle"]/*' />
    	RoundedRectangle = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BorderType.Ellipse"]/*' />
    	Ellipse = 2,
    //		TM_ENUM(0, BT, RECT)
    //		TM_ENUM(1, BT, ROUNDRECT)
    //		TM_ENUM(2, BT, ELLIPSE)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageOrientation"]/*' />
    public enum ImageOrientation 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageOrientation.Vertical"]/*' />
    	Vertical = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageOrientation.Horizontal"]/*' />
    	Horizontal = 1,
    //		TM_ENUM(0, IL, VERTICAL)
    //		TM_ENUM(1, IL, HORIZONTAL)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="SizingType"]/*' />
    public enum SizingType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="SizingType.FixedSize"]/*' />
    	FixedSize = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="SizingType.Stretch"]/*' />
    	Stretch = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="SizingType.Tile"]/*' />
    	Tile = 2,
    //		TM_ENUM(0, ST, TRUESIZE)
    //		TM_ENUM(1, ST, STRETCH)
    //		TM_ENUM(2, ST, TILE)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType"]/*' />
    public enum FillType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType.Solid"]/*' />
    	Solid = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType.VerticalGradient"]/*' />
    	VerticalGradient = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType.HorizontalGradient"]/*' />
    	HorizontalGradient = 2,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType.RadialGradient"]/*' />
    	RadialGradient = 3,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FillType.TileImage"]/*' />
    	TileImage = 4,
    //		TM_ENUM(0, FT, SOLID)
    //		TM_ENUM(1, FT, VERTGRADIENT)
    //		TM_ENUM(2, FT, HORZGRADIENT)
    //		TM_ENUM(3, FT, RADIALGRADIENT)
    //		TM_ENUM(4, FT, TILEIMAGE)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HorizontalAlignment"]/*' />
    public enum HorizontalAlign 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HorizontalAlignment.Left"]/*' />
    	Left = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HorizontalAlignment.Center"]/*' />
    	Center = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HorizontalAlignment.Right"]/*' />
    	Right = 2,
    //		TM_ENUM(0, HA, LEFT)
    //		TM_ENUM(1, HA, CENTER)
    //		TM_ENUM(2, HA, RIGHT)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ContentAlignment"]/*' />
    public enum ContentAlignment 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ContentAlignment.Left"]/*' />
    	Left = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ContentAlignment.Center"]/*' />
    	Center = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ContentAlignment.Right"]/*' />
    	Right = 2,
    //		TM_ENUM(0, CA, LEFT)
    //		TM_ENUM(1, CA, CENTER)
    //		TM_ENUM(2, CA, RIGHT)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="VerticalAlignment"]/*' />
    public enum VerticalAlignment 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="VerticalAlignment.Top"]/*' />
    	Top = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="VerticalAlignment.Center"]/*' />
    	Center = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="VerticalAlignment.Bottom"]/*' />
    	Bottom = 2,
    //		TM_ENUM(0, VA, TOP)
    //		TM_ENUM(1, VA, CENTER)
    //		TM_ENUM(2, VA, BOTTOM)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType"]/*' />
    public enum OffsetType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.TopLeft"]/*' />
    	TopLeft = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.TopRight"]/*' />
    	TopRight = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.TopMiddle"]/*' />
    	TopMiddle = 2,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.BottomLeft"]/*' />
    	BottomLeft = 3,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.BottomRight"]/*' />
    	BottomRight = 4,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.BottomMiddle"]/*' />
    	BottomMiddle = 5,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.MiddleLeft"]/*' />
    	MiddleLeft = 6,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.MiddleRight"]/*' />
    	MiddleRight = 7,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.LeftOfCaption"]/*' />
    	LeftOfCaption = 8,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.RightOfCaption"]/*' />
    	RightOfCaption = 9,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.LeftOfLastButton"]/*' />
    	LeftOfLastButton = 10,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.RightOfLastButton"]/*' />
    	RightOfLastButton = 11,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.AboveLastButton"]/*' />
    	AboveLastButton = 12,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="OffsetType.BelowLastButton"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect"]/*' />
    public enum IconEffect 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect.Glow"]/*' />
    	Glow = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect.Shadow"]/*' />
    	Shadow = 2,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect.Pulse"]/*' />
    	Pulse = 3,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IconEffect.Alpha"]/*' />
    	Alpha = 4,
    //		TM_ENUM(0, ICE, NONE)
    //		TM_ENUM(1, ICE, GLOW)
    //		TM_ENUM(2, ICE, SHADOW)
    //		TM_ENUM(3, ICE, PULSE)
    //		TM_ENUM(4, ICE, ALPHA)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextShadowType"]/*' />
    public enum TextShadowType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextShadowType.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextShadowType.Single"]/*' />
    	Single = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextShadowType.Continuous"]/*' />
    	Continuous = 2,
    //		TM_ENUM(0, TST, NONE)
    //		TM_ENUM(1, TST, SINGLE)
    //		TM_ENUM(2, TST, CONTINUOUS)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphType"]/*' />
    public enum GlyphType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphType.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphType.ImageGlyph"]/*' />
    	ImageGlyph = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphType.FontGlyph"]/*' />
    	FontGlyph = 2,
    //		TM_ENUM(0, GT, NONE)
    //		TM_ENUM(1, GT, IMAGEGLYPH)
    //		TM_ENUM(2, GT, FONTGLYPH)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageSelectType"]/*' />
    public enum ImageSelectType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageSelectType.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageSelectType.Size"]/*' />
    	Size = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ImageSelectType.Dpi"]/*' />
    	Dpi = 2,
    //		TM_ENUM(0, IST, NONE)
    //		TM_ENUM(1, IST, SIZE)
    //		TM_ENUM(2, IST, DPI)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TrueSizeScalingType"]/*' />
    public enum TrueSizeScalingType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TrueSizeScalingType.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TrueSizeScalingType.Size"]/*' />
    	Size = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TrueSizeScalingType.Dpi"]/*' />
    	Dpi = 2,
    //		TM_ENUM(0, TSST, NONE)
    //		TM_ENUM(1, TSST, SIZE)
    //		TM_ENUM(2, TSST, DPI)
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphFontSizingType"]/*' />
    public enum GlyphFontSizingType 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphFontSizingType.None"]/*' />
    	None = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphFontSizingType.Size"]/*' />
    	Size = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="GlyphFontSizingType.Dpi"]/*' />
    	Dpi = 2,
    //		TM_ENUM(0, GFST, NONE)
    //		TM_ENUM(1, GFST, SIZE)
    //		TM_ENUM(2, GFST, DPI)
    }

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum ColorProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.BorderColor"]/*' />
    	BorderColor = 3801,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.FillColor"]/*' />
    	FillColor = 3802,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.TextColor"]/*' />
    	TextColor = 3803,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.EdgeLightColor"]/*' />
    	EdgeLightColor = 3804,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.EdgeHighlightColor"]/*' />
    	EdgeHighlightColor = 3805,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.EdgeShadowColor"]/*' />
    	EdgeShadowColor = 3806,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.EdgeDarkShadowColor"]/*' />
    	EdgeDarkShadowColor = 3807,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.EdgeFillColor"]/*' />
    	EdgeFillColor = 3808,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.TransparentColor"]/*' />
    	TransparentColor = 3809,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GradientColor1"]/*' />
    	GradientColor1 = 3810,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GradientColor2"]/*' />
    	GradientColor2 = 3811,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GradientColor3"]/*' />
    	GradientColor3 = 3812,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GradientColor4"]/*' />
    	GradientColor4 = 3813,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GradientColor5"]/*' />
    	GradientColor5 = 3814,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.ShadowColor"]/*' />
    	ShadowColor = 3815,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GlowColor"]/*' />
    	GlowColor = 3816,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.TextBorderColor"]/*' />
    	TextBorderColor = 3817,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.TextShadowColor"]/*' />
    	TextShadowColor = 3818,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GlyphTextColor"]/*' />
    	GlyphTextColor = 3819,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.GlyphTransparentColor"]/*' />
    	GlyphTransparentColor = 3820,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.FillColorHint"]/*' />
    	FillColorHint = 3821,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.BorderColorHint"]/*' />
    	BorderColorHint = 3822,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ColorProperty.AccentColorHint"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // EnumProperty maps to native enum.
    ]
    public enum EnumProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.BackgroundType"]/*' />
    	BackgroundType = 4001,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.BorderType"]/*' />
    	BorderType = 4002,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.FillType"]/*' />
    	FillType = 4003,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.SizingType"]/*' />
    	SizingType = 4004,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.HorizontalAlignment"]/*' />
    	HorizontalAlignment = 4005,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.ContentAlignment"]/*' />
        ContentAlignment = 4006,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.VerticalAlignment"]/*' />
    	VerticalAlignment = 4007,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.OffsetType"]/*' />
    	OffsetType = 4008,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.IconEffect"]/*' />
    	IconEffect = 4009,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.TextShadowType"]/*' />
    	TextShadowType = 4010,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.ImageLayout"]/*' />
    	ImageLayout = 4011,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.GlyphType"]/*' />
    	GlyphType = 4012,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.ImageSelectType"]/*' />
    	ImageSelectType = 4013,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.GlyphFontSizingType"]/*' />
    	GlyphFontSizingType = 4014,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EnumProperty.TrueSizeScalingType"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum FilenameProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile"]/*' />
    	ImageFile = 3001,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile1"]/*' />
    	ImageFile1 = 3002,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile2"]/*' />
    	ImageFile2 = 3003,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile3"]/*' />
    	ImageFile3 = 3004,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile4"]/*' />
    	ImageFile4 = 3005,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.ImageFile5"]/*' />
    	ImageFile5 = 3006,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.StockImageFile"]/*' />
    	StockImageFile = 3007,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FilenameProperty.GlyphImageFile"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FontProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum FontProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="FontProperty.GlyphFont"]/*' />
    	GlyphFont = 2601
    //		 TM_PROP(2601, TMT, GLYPHFONT,         FONT)   // the font that the glyph is drawn with
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum IntegerProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.ImageCount"]/*' />
    	ImageCount = 2401,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.AlphaLevel"]/*' />
    	AlphaLevel = 2402,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.BorderSize"]/*' />
    	BorderSize = 2403,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.RoundCornerWidth"]/*' />
    	RoundCornerWidth = 2404,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.RoundCornerHeight"]/*' />
    	RoundCornerHeight = 2405,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GradientRatio1"]/*' />
    	GradientRatio1 = 2406,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GradientRatio2"]/*' />
    	GradientRatio2 = 2407,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GradientRatio3"]/*' />
    	GradientRatio3 = 2408,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GradientRatio4"]/*' />
    	GradientRatio4 = 2409,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GradientRatio5"]/*' />
    	GradientRatio5 = 2410,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.ProgressChunkSize"]/*' />
    	ProgressChunkSize = 2411,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.ProgressSpaceSize"]/*' />
    	ProgressSpaceSize = 2412,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.Saturation"]/*' />
    	Saturation = 2413,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.TextBorderSize"]/*' />
    	TextBorderSize = 2414,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.AlphaThreshold"]/*' />
    	AlphaThreshold = 2415,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.Width"]/*' />
    	Width = 2416,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.Height"]/*' />
    	Height = 2417,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.GlyphIndex"]/*' />
    	GlyphIndex = 2418,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.TrueSizeStretchMark"]/*' />
    	TrueSizeStretchMark = 2419,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.MinDpi1"]/*' />
    	MinDpi1 = 2420,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.MinDpi2"]/*' />
    	MinDpi2 = 2421,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.MinDpi3"]/*' />
    	MinDpi3 = 2422,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.MinDpi4"]/*' />
    	MinDpi4 = 2423,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="IntegerProperty.MinDpi5"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum PointProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.Offset"]/*' />
    	Offset = 3401,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.TextShadowOffset"]/*' />
    	TextShadowOffset = 3402,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize"]/*' />
    	MinSize = 3403,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize1"]/*' />
    	MinSize1 = 3404,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize2"]/*' />
    	MinSize2 = 3405,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize3"]/*' />
    	MinSize3 = 3406,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize4"]/*' />
    	MinSize4 = 3407,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="PointProperty.MinSize5"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="MarginProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum MarginProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="MarginProperty.SizingMargins"]/*' />
    	SizingMargins = 3601,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="MarginProperty.ContentMargins"]/*' />
    	ContentMargins = 3602,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="MarginProperty.CaptionMargins"]/*' />
    	CaptionMargins = 3603
    //		TM_PROP(3601, TMT, SIZINGMARGINS,     MARGINS)    // margins used for 9-grid sizing
    //		TM_PROP(3602, TMT, CONTENTMARGINS,    MARGINS)    // margins that define where content can be placed
    //		TM_PROP(3603, TMT, CAPTIONMARGINS,    MARGINS)    // margins that define where caption text can be placed
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="StringProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum StringProperty 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="StringProperty.Text"]/*' />
    	Text = 3201   
    	//TM_PROP(3201, TMT, TEXT,              STRING)
    }

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum BooleanProperty
    {
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.Transparent"]/*' />
        Transparent = 2201,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.AutoSize"]/*' />
        AutoSize = 2202,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.BorderOnly"]/*' />
        BorderOnly = 2203,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.Composited"]/*' />
        Composited = 2204,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.BackgroundFill"]/*' />
        BackgroundFill = 2205,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.GlyphTransparent"]/*' />
        GlyphTransparent = 2206,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.GlyphOnly"]/*' />
        GlyphOnly = 2207,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.AlwaysShowSizingBar"]/*' />
        AlwaysShowSizingBar = 2208,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.MirrorImage"]/*' />
        MirrorImage = 2209,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.UniformSizing"]/*' />
        UniformSizing = 2210,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.IntegralSizing"]/*' />
        IntegralSizing = 2211,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.SourceGrow"]/*' />
        SourceGrow = 2212,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="BooleanProperty.SourceShrink"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges"]/*' />
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum Edges 
    {
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges.Left"]/*' />
        Left = 0x0001,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges.Top"]/*' />
        Top = 0x0002,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges.Right"]/*' />
        Right = 0x0004,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges.Bottom"]/*' />
        Bottom = 0x0008,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="Edges.Diagonal"]/*' />
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

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeStyle"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum EdgeStyle 
    {
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeStyle.Raised"]/*' />
        Raised = 0x0001 | 0x0004,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeStyle.Sunken"]/*' />
    	Sunken = 0x0002 | 0x0008,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeStyle.Etched"]/*' />
        Etched = 0x0002 | 0x0004,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeStyle.Bump"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects"]/*' />
    [Flags]
    public enum EdgeEffects 
    {
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects.None"]/*' />
        None = 0,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects.FillInterior"]/*' />
        FillInterior = 0x0800,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects.Flat"]/*' />
    	Flat = 0x1000,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects.Soft"]/*' />
    	Soft = 0x4000,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="EdgeEffects.Mono"]/*' />
    	Mono = 0x8000,
    //	#define BF_SOFT         0x1000  /* For softer buttons */
    //	#define BF_FLAT         0x4000  /* For flat rather than 3D borders */
    //	#define BF_MONO         0x8000  /* For monochrome borders */
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Height;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Ascent;"]/*' />
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
		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Descent;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.InternalLeading;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.ExternalLeading;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.AveCharWidth;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.MaxCharWidth;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Weight;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Overhang;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DigitizedAspectX;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DigitizedAspectY;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.FirstChar;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.LastChar;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DefaultChar;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.BreakChar;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Italic;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Underlined;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.StruckOut;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.PitchAndFamily;"]/*' />
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

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.CharSet;"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricPitchAndFamilyValues"]/*' />
    [Flags]
    public enum TextMetricsPitchAndFamilyValues 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricPitchAndFamilyValues.FixedPitch"]/*' />
    	FixedPitch = 0x01,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricPitchAndFamilyValues.Vector"]/*' />
    	Vector = 0x02,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricPitchAndFamilyValues.TrueType"]/*' />
        TrueType = 0x04,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricPitchAndFamilyValues.Device"]/*' />
    	Device = 0x08
    	
    //		#define TMPF_FIXED_PITCH    0x01
    //		#define TMPF_VECTOR             0x02
    //		#define TMPF_DEVICE             0x08
    //		#define TMPF_TRUETYPE       0x04
    }
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet"]/*' />
    public enum TextMetricsCharacterSet 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Ansi"]/*' />
    	Ansi = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Baltic"]/*' />
    	Baltic = 186,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.ChineseBig5"]/*' />
    	ChineseBig5 = 136,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Default"]/*' />
    	Default = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.EastEurope"]/*' />
    	EastEurope = 238,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Gb2312"]/*' />
    	Gb2312 = 134,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Greek"]/*' />
    	Greek = 161,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Hangul"]/*' />
    	Hangul = 129,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Mac"]/*' />
    	Mac = 77,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Oem"]/*' />
    	Oem = 255,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Russian"]/*' />
    	Russian = 204,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.ShiftJis"]/*' />
    	ShiftJis = 128,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Symbol"]/*' />
    	Symbol = 2,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Turkish"]/*' />
    	Turkish = 162,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Vietnamese"]/*' />
    	Vietnamese = 163,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Johab"]/*' />
    	Johab = 130,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Arabic"]/*' />
    	Arabic = 178,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Hebrew"]/*' />
    	Hebrew = 177,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetricCharacterSet.Thai"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions"]/*' />
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum HitTestOptions 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.BackgroundSegment"]/*' />
    	BackgroundSegment = 0x0000,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.FixedBorder"]/*' />
    	FixedBorder = 0x0002,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.Caption"]/*' />
    	Caption = 0x0004,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.ResizingBorderLeft"]/*' />
    	ResizingBorderLeft = 0x0010,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.ResizingBorderTop"]/*' />
    	ResizingBorderTop = 0x0020,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.ResizingBorderRight"]/*' />
    	ResizingBorderRight = 0x0040,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.ResizingBorderBottom"]/*' />
    	ResizingBorderBottom = 0x0080,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.ResizingBorder"]/*' />
    	ResizingBorder = ResizingBorderLeft | ResizingBorderTop | ResizingBorderRight | ResizingBorderBottom,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.SizingTemplate"]/*' />
    	SizingTemplate = 0x0100,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestOptions.SystemSizingMargins"]/*' />
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
    
    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode"]/*' />
    public enum HitTestCode 
    {
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Nowhere"]/*' />
    	Nowhere = 0,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Client"]/*' />
    	Client = 1,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Left"]/*' />
    	Left = 10,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Right"]/*' />
    	Right = 11,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Top"]/*' />
    	Top = 12,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.Bottom"]/*' />
    	Bottom = 15,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.TopLeft"]/*' />
    	TopLeft = 13,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.TopRight"]/*' />
    	TopRight = 14,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.BottomLeft"]/*' />
    	BottomLeft = 16,
    	/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="HitTestCode.BottomRight"]/*' />
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

    /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ThemeSizeType"]/*' />
    public enum ThemeSizeType {
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ThemeSizeType.Minimum"]/*' />
        Minimum = 0,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ThemeSizeType.True"]/*' />
        True = 1,
        /// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="ThemeSizeType.Draw"]/*' />
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
