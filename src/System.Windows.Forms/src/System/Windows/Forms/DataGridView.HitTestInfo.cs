// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        public sealed class HitTestInfo
        {
            internal DataGridViewHitTestType type = DataGridViewHitTestType.None;
            //internal DataGridViewHitTestTypeCloseEdge edge = DataGridViewHitTestTypeCloseEdge.None;
            internal DataGridViewHitTestTypeInternal typeInternal = DataGridViewHitTestTypeInternal.None;
            internal int row;
            internal int col;
            internal int adjacentRow;
            internal int adjacentCol;
            internal int mouseBarOffset;
            internal int rowStart;
            internal int colStart;

            /// <summary>
            ///  Allows the <see cref='HitTestInfo'/> object to inform you the
            ///  extent of the grid.
            /// </summary>
            public static readonly HitTestInfo Nowhere = new HitTestInfo();

            internal HitTestInfo()
            {
                type = DataGridViewHitTestType.None;
                typeInternal = DataGridViewHitTestTypeInternal.None;
                //this.edge = DataGridViewHitTestTypeCloseEdge.None;
                row = col = -1;
                rowStart = colStart = -1;
                adjacentRow = adjacentCol = -1;
            }

            /// <summary>
            ///  Gets the number of the clicked column.
            /// </summary>
            public int ColumnIndex
            {
                get
                {
                    return col;
                }
            }

            /// <summary>
            ///  Gets the
            ///  number of the clicked row.
            /// </summary>
            public int RowIndex
            {
                get
                {
                    return row;
                }
            }

            /// <summary>
            ///  Gets the left edge of the column.
            /// </summary>
            public int ColumnX
            {
                get
                {
                    return colStart;
                }
            }

            /// <summary>
            ///  Gets the top edge of the row.
            /// </summary>
            public int RowY
            {
                get
                {
                    return rowStart;
                }
            }

            /// <summary>
            ///  Gets the part of the <see cref='DataGridView'/> control, other than the row or column, that was
            ///  clicked.
            /// </summary>
            public DataGridViewHitTestType Type
            {
                get
                {
                    return type;
                }
            }

            /// <summary>
            ///  Indicates whether two objects are identical.
            /// </summary>
            public override bool Equals(object value)
            {
                if (value is HitTestInfo hti)
                {
                    return (type == hti.type &&
                            row == hti.row &&
                            col == hti.col);
                }
                return false;
            }

            /// <summary>
            ///  Gets the hash code for the <see cref='HitTestInfo'/> instance.
            /// </summary>
            public override int GetHashCode() => HashCode.Combine(type, row, col);

            /// <summary>
            ///  Gets the type, column number and row number.
            /// </summary>
            public override string ToString()
            {
                return "{ Type:" + type.ToString() + ", Column:" + col.ToString(CultureInfo.CurrentCulture) + ", Row:" + row.ToString(CultureInfo.CurrentCulture) + " }";
            }
        }
    }
}
