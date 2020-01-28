// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies if a TableLayoutPanel will gain additional rows or columns once
    ///  its existing cells become full.  If the value is 'None' then the
    ///  TableLayoutPanel will throw an exception when the TableLayoutPanel is
    ///  over-filled.
    /// </summary>
    public enum TableLayoutPanelGrowStyle
    {
        /// <summary>
        ///  The TableLayoutPanel will not allow additional rows or columns once
        ///  it is full.
        /// </summary>
        FixedSize = 0,

        /// <summary>
        ///  The TableLayoutPanel will gain additional rows once it becomes full.
        /// </summary>
        AddRows = 1,

        /// <summary>
        ///  The TableLayoutPanel will gain additional columns once it becomes full.
        /// </summary>
        AddColumns = 2
    }
}
