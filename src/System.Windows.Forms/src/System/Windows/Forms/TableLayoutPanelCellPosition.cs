// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(TableLayoutPanelCellPositionTypeConverter))]
public struct TableLayoutPanelCellPosition : IEquatable<TableLayoutPanelCellPosition>
{
    public TableLayoutPanelCellPosition(int column, int row)
    {
        if (row < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row, string.Format(SR.InvalidArgument, nameof(row), row));
        }

        if (column < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(column), column, string.Format(SR.InvalidArgument, nameof(column), column));
        }

        Row = row;
        Column = column;
    }

    public int Row { get; set; }

    public int Column { get; set; }

    public override bool Equals(object? other)
    {
        if (other is not TableLayoutPanelCellPosition otherCellPosition)
        {
            return false;
        }

        return Equals(otherCellPosition);
    }

    public bool Equals(TableLayoutPanelCellPosition other)
        => Row == other.Row && Column == other.Column;

    public static bool operator ==(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return p1.Row == p2.Row && p1.Column == p2.Column;
    }

    public static bool operator !=(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return !(p1 == p2);
    }

    public override string ToString() => $"{Column},{Row}";

    public override int GetHashCode() => HashCode.Combine(Row, Column);
}
