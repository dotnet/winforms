// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies responsibility for drawing a control or portion of a control.
    /// </devdoc>
    public enum DrawMode
    {
        /// <devdoc>
        /// The operating system paints the items in the control, and the items
        /// are each the same height.
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        /// The programmer explicitly paints the items in the control, and the
        /// items are each the same height.
        /// </devdoc>
        OwnerDrawFixed = 1,

        /// <devdoc>
        /// The programmer explicitly paints the items in the control manually,
        /// and they may be different heights.
        /// </devdoc>
        OwnerDrawVariable = 2,
    }
}
