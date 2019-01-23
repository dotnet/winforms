// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This is the overrided PaintEventArgs for painting the cell of the table
    /// It contains additional information indicating the row/column of the cell
    /// as well as the bound of the cell
    /// </devdoc>
    public class TableLayoutCellPaintEventArgs : PaintEventArgs
    {
        public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row) : base(g, clipRectangle)
        {
            CellBounds = cellBounds;
            Column = column;
            Row = row;
        }

        public Rectangle CellBounds { get; }

        public int Column { get; }

        public int Row { get; }
    }
}
