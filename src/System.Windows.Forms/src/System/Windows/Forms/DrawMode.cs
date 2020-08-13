// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies responsibility for drawing a control or portion of a control.
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        ///  The operating system paints the items in the control, and the items
        ///  are each the same height.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  The programmer explicitly paints the items in the control, and the
        ///  items are each the same height.
        /// </summary>
        OwnerDrawFixed = 1,

        /// <summary>
        ///  The programmer explicitly paints the items in the control manually,
        ///  and they may be different heights.
        /// </summary>
        OwnerDrawVariable = 2,
    }
}
