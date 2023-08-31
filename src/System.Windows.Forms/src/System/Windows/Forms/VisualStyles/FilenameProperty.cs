// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

public enum FilenameProperty
{
    ImageFile = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE,
    ImageFile1 = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE1,
    ImageFile2 = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE2,
    ImageFile3 = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE3,
    ImageFile4 = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE4,
    ImageFile5 = (int)THEME_PROPERTY_SYMBOL_ID.TMT_IMAGEFILE5,
    // This value isn't used so it isn't in the Win32 metadata (TMT_STOCKIMAGEFILE)
    StockImageFile = 3007,
    GlyphImageFile = (int)THEME_PROPERTY_SYMBOL_ID.TMT_GLYPHIMAGEFILE
}
