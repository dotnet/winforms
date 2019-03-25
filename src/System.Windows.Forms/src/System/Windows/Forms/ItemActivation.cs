﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how the user activates items and the appearance of items as the
    /// mouse cursor moves over them.
    /// </devdoc>
    public enum ItemActivation
    {
        /// <devdoc>
        /// Activate items with a double-click.
        /// Items do not change appearance.
        /// </devdoc>
        Standard = 0,

        /// <devdoc>
        /// Activate items with a single click. The cursor changes shape and the
        /// item text changes color.
        /// </devdoc>
        OneClick = 1,

        /// <devdoc>
        /// Activate items with a double click. The item text changes color.
        /// </devdoc>
        TwoClick = 2,
    }
}
