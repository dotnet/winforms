// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        public sealed class HitTestInfo
        {
            internal DataGridViewHitTestType _type = DataGridViewHitTestType.None;
            internal DataGridViewHitTestTypeInternal _typeInternal = DataGridViewHitTestTypeInternal.None;
            internal int _row;
            internal int _col;
            internal int _adjacentRow;
            internal int _adjacentCol;
            internal int _mouseBarOffset;
            internal int _rowStart;
            internal int _colStart;

            /// <summary>
            ///  Allows the <see cref='HitTestInfo'/> object to inform you the extent of the grid.
            /// </summary>
            public static readonly HitTestInfo Nowhere = new HitTestInfo();

            internal HitTestInfo()
            {
                _type = DataGridViewHitTestType.None;
                _typeInternal = DataGridViewHitTestTypeInternal.None;
                //this.edge = DataGridViewHitTestTypeCloseEdge.None;
                _row = _col = -1;
                _rowStart = _colStart = -1;
                _adjacentRow = _adjacentCol = -1;
            }

            /// <summary>
            ///  Gets the number of the clicked column.
            /// </summary>
            public int ColumnIndex
            {
                get
                {
                    return _col;
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
                    return _row;
                }
            }

            /// <summary>
            ///  Gets the left edge of the column.
            /// </summary>
            public int ColumnX
            {
                get
                {
                    return _colStart;
                }
            }

            /// <summary>
            ///  Gets the top edge of the row.
            /// </summary>
            public int RowY
            {
                get
                {
                    return _rowStart;
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
                    return _type;
                }
            }

            /// <summary>
            ///  Indicates whether two objects are identical.
            /// </summary>
            public override bool Equals(object value)
            {
                if (value is HitTestInfo hti)
                {
                    return (_type == hti._type &&
                            _row == hti._row &&
                            _col == hti._col);
                }
                return false;
            }

            /// <summary>
            ///  Gets the hash code for the <see cref='HitTestInfo'/> instance.
            /// </summary>
            public override int GetHashCode() => HashCode.Combine(_type, _row, _col);

            /// <summary>
            ///  Gets the type, column number and row number.
            /// </summary>
            public override string ToString()
            {
                return "{ Type:" + _type.ToString() + ", Column:" + _col.ToString(CultureInfo.CurrentCulture) + ", Row:" + _row.ToString(CultureInfo.CurrentCulture) + " }";
            }
        }
    }
}
