// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Specifies the layout of a device context.
    /// </summary>
    [Flags]
    internal enum DeviceContextLayout
    {
        /// <summary>
        ///  Right to left.
        /// </summary>
        Normal = 0x00000000,

        /// <summary>
        ///  Right to left. LAYOUT_RTL
        /// </summary>
        RightToLeft = 0x00000001,

        /// <summary>
        ///  Bottom to top. LAYOUT_BTT
        /// </summary>
        BottomToTop = 0x00000002,

        /// <summary>
        ///  Vertical before horizontal. LAYOUT_VBH
        /// </summary>
        VerticalBeforeHorizontal = 0x00000004,

        /// <summary>
        ///  Disables any reflection during BitBlt and StretchBlt operations. LAYOUT_BITMAPORIENTATIONPRESERVED
        /// </summary>
        BitmapOrientationPreserved = 0x00000008
    }
}
