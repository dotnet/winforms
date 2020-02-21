// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Specifies identifiers to use to indicate the style of the selection frame of a component.
    /// </summary>
    [Flags]
    internal enum SelectionStyles
    {
        /// <summary>
        ///  The component is not currently selected.
        /// </summary>
        None = 0,
        /// <summary>
        ///  A component is selected and may be dragged around
        /// </summary>
        Selected = 0x01,
        /// <summary>
        ///  An alternative selection border, indicating that a component is in active editing mode and that clicking and dragging on the component affects the component itself, not its position in the designer.
        /// </summary>
        Active = 0x02,
    }
}
