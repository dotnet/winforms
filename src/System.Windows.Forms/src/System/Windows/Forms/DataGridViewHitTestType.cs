// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the part of the <see cref='DataGridView'/>
    ///  control where the mouse is.
    /// </summary>
    public enum DataGridViewHitTestType
    {
        None = 0,
        Cell = 1,
        ColumnHeader = 2,
        RowHeader = 3,
        TopLeftHeader = 4,
        HorizontalScrollBar = 5,
        VerticalScrollBar = 6
    }
}
