// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
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
}
