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

            /// <devdoc>
            /// <para>Allows the <see cref='System.Windows.Forms.DataGridView.HitTestInfo'/> object to inform you the
            ///    extent of the grid.</para>
            /// </devdoc>
            public static readonly HitTestInfo Nowhere = new HitTestInfo();

            internal HitTestInfo()
            {
                this.type = DataGridViewHitTestType.None;
                this.typeInternal = DataGridViewHitTestTypeInternal.None;
                //this.edge = DataGridViewHitTestTypeCloseEdge.None;
                this.row = this.col = -1;
                this.rowStart = this.colStart = -1;
                this.adjacentRow = this.adjacentCol = -1;
            }

            /// <devdoc>
            ///    <para>Gets the number of the clicked column.</para>
            /// </devdoc>
            public int ColumnIndex
            {
                get
                {
                    return this.col;
                }
            }

            /// <devdoc>
            ///    <para>Gets the
            ///       number of the clicked row.</para>
            /// </devdoc>
            public int RowIndex
            {
                get
                {
                    return this.row;
                }
            }

            /// <devdoc>
            ///    <para>Gets the left edge of the column.</para>
            /// </devdoc>
            public int ColumnX
            {
                get
                {
                    return this.colStart;
                }
            }

            /// <devdoc>
            ///    <para>Gets the top edge of the row.</para>
            /// </devdoc>
            public int RowY
            {
                get
                {
                    return this.rowStart;
                }
            }

            /// <devdoc>
            /// <para>Gets the part of the <see cref='System.Windows.Forms.DataGridView'/> control, other than the row or column, that was
            ///    clicked.</para>
            /// </devdoc>
            public DataGridViewHitTestType Type
            {
                get
                {
                    return this.type;
                }
            }

            /// <devdoc>
            ///    <para>Indicates whether two objects are identical.</para>
            /// </devdoc>
            public override bool Equals(object value)
            {
                HitTestInfo hti = value as HitTestInfo;
                if (hti != null)
                {
                    return (this.type == hti.type &&
                            this.row  == hti.row &&
                            this.col  == hti.col);
                }
                return false;
            }

            /// <devdoc>
            /// <para>Gets the hash code for the <see cref='System.Windows.Forms.DataGridView.HitTestInfo'/> instance.</para>
            /// </devdoc>
            public override int GetHashCode() => HashCode.Combine(type, row, col);

            /// <devdoc>
            ///    <para>Gets the type, column number and row number.</para>
            /// </devdoc>
            public override string ToString()
            {
                return "{ Type:" + type.ToString() + ", Column:" + col.ToString(CultureInfo.CurrentCulture) + ", Row:" + row.ToString(CultureInfo.CurrentCulture) + " }";
            }
        }
    }
}
