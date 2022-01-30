// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the font auto scaling mode used by a control.
    /// </summary>
    public enum FontAutoScale
    {
        /// <summary>
        ///  Scale only if control using system font.
        /// </summary>
        SystemOnly,

        /// <summary>
        ///  Scale if control using system or explicitly specified font.
        /// </summary>
        SystemAndExplicit,

        /// <summary>
        ///  Font scale according to their parent's scaling mode.
        ///  If there is no parent, this behaves as if <see cref="FontAutoScale.SystemOnly"/> were set.
        /// </summary>
        Inherit
    }
}
