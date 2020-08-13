// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Enumeration defining the different Graphics properties to apply to a WindowsGraphics when creating it
    ///  from a Graphics object.
    /// </summary>
    [Flags]
    internal enum ApplyGraphicsProperties
    {
        None                = 0x0000_0000,

        /// <summary>
        ///  Apply clipping region.
        /// </summary>
        Clipping            = 0x0000_0001,

        /// <summary>
        ///  Apply coordinate transformation.
        /// </summary>
        TranslateTransform  = 0x0000_0002,

        /// <summary>
        ///  Apply all supported Graphics properties.
        /// </summary>
        All                 = Clipping | TranslateTransform
    }
}
