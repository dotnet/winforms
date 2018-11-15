// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    /// <devdoc>
    ///    Specifies the map-mode of a device context.
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    enum DeviceContextTextAlignment
    {   
        BaseLine     = 24,  // TA_BASELINE The reference point is on the base line of the text. 
        Bottom       = 8,   // TA_BOTTOM The reference point is on the bottom edge of the bounding rectangle. 
        Top          = 0,   // TA_TOP The reference point is on the top edge of the bounding rectangle. 
        Center       = 6,   // TA_CENTER The reference point is aligned horizontally with the center of the bounding rectangle. 
        Default      = 0,   // Top, Left, NoUpdateCP
        Left         = 0,   // TA_LEFT The reference point is on the left edge of the bounding rectangle. 
        Right        = 2,   // TA_RIGHT The reference point is on the right edge of the bounding rectangle. 
        RtlReading   = 256, // TA_RTLREADING Middle East language edition of Windows: The text is laid out in right to left reading order, as opposed to the default left to right order. This only applies when the font selected into the device context is either Hebrew or Arabic. 
        NoUpdateCP   = 0,   // TA_NOUPDATECP The current position is not updated after each text output call. 
        UpdateCP     = 1,   // TA_UPDATECP The current position is updated after each text output call. 

        // When the current font has a vertical default base line (as with Kanji), the following values are used instead of TA_BASELINE and TA_CENTER. 
        // Value Meaning 
        VerticalBaseLine, // VTA_BASELINE The reference point is on the base line of the text. 
        VerticalCenter    // VTA_CENTER The reference point is aligned vertically with the center of the bounding rectangle. 

        /*
        TA_NOUPDATECP                0
        TA_UPDATECP                  1

        TA_LEFT                      0
        TA_RIGHT                     2
        TA_CENTER                    6

        TA_TOP                       0
        TA_BOTTOM                    8
        TA_BASELINE                  24
        //WINVER >= 0x0400)
        TA_RTLREADING                256
        */
    }
}
