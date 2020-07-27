// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is the overrided PaintEventArgs for painting the cell of the table. It contains additional information
    ///  indicating the row/column of the cell as well as the bounds of the cell.
    /// </summary>
    public class TableLayoutCellPaintEventArgs : PaintEventArgs
    {
        public TableLayoutCellPaintEventArgs(
            Graphics g,
            Rectangle clipRectangle,
            Rectangle cellBounds,
            int column,
            int row) : base(g, clipRectangle)
        {
            CellBounds = cellBounds;
            Column = column;
            Row = row;
        }

        internal TableLayoutCellPaintEventArgs(
            PaintEventArgs e,
            Rectangle clipRectangle,
            Rectangle cellBounds,
            int column,
            int row) : base(e, clipRectangle)
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
