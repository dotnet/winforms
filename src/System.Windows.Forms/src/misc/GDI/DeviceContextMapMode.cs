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
    enum DeviceContextMapMode
    {   
        Text             = 1, // MM_TEXT
        LoMetric         = 2,
        HiMetric         = 3,
        LoEnglish        = 4,
        HiEnglish        = 5,
        Twips            = 6,
        Isotropic        = 7,
        Anisotropic      = 8

        /*
        Mapping Mode        Logical Unit        x-axis      y-axis 
        MM_TEXT             Pixel               Right       Down 
        MM_LOMETRIC         0.1 mm              Right       Up 
        MM_HIMETRIC         0.01 mm             Right       Up 
        MM_LOENGLISH        0.01 in.            Right       Up 
        MM_HIENGLISH        0.001 in.           Right       Up 
        MM_TWIPS            1/1440 in.          Right       Up 
        MM_ISOTROPIC        Arbitrary (x = y)   Selectable  Selectable 
        MM_ANISOTROPIC      Arbitrary (x !=y)   Selectable  Selectable 
        */

    }
}
