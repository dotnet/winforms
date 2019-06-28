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
            /// <para>Allows the <see cref='System.Windows.Forms.DataGridView.HitTestInfo'/> object to inform you the 
            ///    extent of the grid.</para>
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
            ///    <para>Gets the number of the clicked column.</para>
            /// </summary>
            public int ColumnIndex
            {
                get
                {
                    return col;
                }
            }

            /// <summary>
            ///    <para>Gets the
            ///       number of the clicked row.</para>
            /// </summary>
            public int RowIndex
            {
                get
                {
                    return row;
                }
            }

            /// <summary>
            ///    <para>Gets the left edge of the column.</para>
            /// </summary>
            public int ColumnX
            {
                get
                {
                    return colStart;
                }
            }

            /// <summary>
            ///    <para>Gets the top edge of the row.</para>
            /// </summary>
            public int RowY
            {
                get
                {
                    return rowStart;
                }
            }

            /// <summary>
            /// <para>Gets the part of the <see cref='System.Windows.Forms.DataGridView'/> control, other than the row or column, that was 
            ///    clicked.</para>
            /// </summary>
            public DataGridViewHitTestType Type
            {
                get
                {
                    return type;
                }
            }

            /// <summary>
            ///    <para>Indicates whether two objects are identical.</para>
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
            /// <para>Gets the hash code for the <see cref='System.Windows.Forms.DataGridView.HitTestInfo'/> instance.</para>
            /// </summary>
            public override int GetHashCode() => HashCode.Combine(type, row, col);

            /// <summary>
            ///    <para>Gets the type, column number and row number.</para>
            /// </summary>
            public override string ToString()
            {
                return "{ Type:" + type.ToString() + ", Column:" + col.ToString(CultureInfo.CurrentCulture) + ", Row:" + row.ToString(CultureInfo.CurrentCulture) + " }";
            }
        }
    }
}
