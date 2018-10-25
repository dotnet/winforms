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
    ///    Device capability indexes - See Win32 GetDeviceCaps().
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    enum DeviceCapabilities
    {  
        /* Device Parameters for GetDeviceCaps() */
        DriverVersion       = 0,     /* device driver version                    */
        Technology          = 2,     /* device classification                    */
        HorizontalSize      = 4,     /* horizontal size in millimeters           */
        VerticalSize        = 6,     /* vertical size in millimeters             */
        HorizontalResolution= 8,     /* horizontal width in pixels               */
        VerticalResolution  = 10,    /* vertical height in pixels                */
        BitsPerPixel        = 12,    /* number of bits per pixel                 */
        LogicalPixelsX      = 88,    /* Logical pixels/inch in X                 */
        LogicalPixelsY      = 90,    /* Logical pixels/inch in Y                 */

        // printing related devicecaps. these replace the appropriate escapes

        PhysicalWidth       = 110,  /* physical width in device units           */
        PhysicalHeight      = 111,  /* physical height in device units          */
        PhysicalOffsetX     = 112,  /* physical printable area x margin         */
        PhysicalOffsetY     = 113,  /* physical printable area y margin         */
        ScalingFactorX      = 114,  /* scaling factor x                         */
        ScalingFactorY      = 115,  /* scaling factor y                         */

#if CPP_DEFINITIONS
/* Device Parameters for GetDeviceCaps() */
DRIVERVERSION 0     /* Device driver version                    */
TECHNOLOGY    2     /* Device classification                    */
HORZSIZE      4     /* Horizontal size in millimeters           */
VERTSIZE      6     /* Vertical size in millimeters             */
HORZRES       8     /* Horizontal width in pixels               */
VERTRES       10    /* Vertical height in pixels                */
BITSPIXEL     12    /* Number of bits per pixel                 */
PLANES        14    /* Number of planes                         */
NUMBRUSHES    16    /* Number of brushes the device has         */
NUMPENS       18    /* Number of pens the device has            */
NUMMARKERS    20    /* Number of markers the device has         */
NUMFONTS      22    /* Number of fonts the device has           */
NUMCOLORS     24    /* Number of colors the device supports     */
PDEVICESIZE   26    /* Size required for device descriptor      */
CURVECAPS     28    /* Curve capabilities                       */
LINECAPS      30    /* Line capabilities                        */
POLYGONALCAPS 32    /* Polygonal capabilities                   */
TEXTCAPS      34    /* Text capabilities                        */
CLIPCAPS      36    /* Clipping capabilities                    */
RASTERCAPS    38    /* Bitblt capabilities                      */
ASPECTX       40    /* Length of the X leg                      */
ASPECTY       42    /* Length of the Y leg                      */
ASPECTXY      44    /* Length of the hypotenuse                 */

LOGPIXELSX    88    /* Logical pixels/inch in X                 */
LOGPIXELSY    90    /* Logical pixels/inch in Y                 */

SIZEPALETTE  104    /* Number of entries in physical palette    */
NUMRESERVED  106    /* Number of reserved entries in palette    */
COLORRES     108    /* Actual color resolution                  */

        // Printing related DeviceCaps. These replace the appropriate Escapes

PHYSICALWIDTH   110 /* Physical Width in device units           */
PHYSICALHEIGHT  111 /* Physical Height in device units          */
PHYSICALOFFSETX 112 /* Physical Printable Area x margin         */
PHYSICALOFFSETY 113 /* Physical Printable Area y margin         */
SCALINGFACTORX  114 /* Scaling factor x                         */
SCALINGFACTORY  115 /* Scaling factor y                         */

        // Display driver specific

VREFRESH        116  /* Current vertical refresh rate of the    */
        /* display device (for displays only) in Hz*/
DESKTOPVERTRES  117  /* Horizontal width of entire desktop in   */
        /* pixels                                  */
DESKTOPHORZRES  118  /* Vertical height of entire desktop in    */
        /* pixels                                  */
BLTALIGNMENT    119  /* Preferred blt alignment                 */

//#if(WINVER >= 0x0500)
SHADEBLENDCAPS  120  /* Shading and blending caps               */
COLORMGMTCAPS   121  /* Color Management caps                   */
//#endif /* WINVER >= 0x0500 */

//#ifndef NOGDICAPMASKS

#endif
    }
}
