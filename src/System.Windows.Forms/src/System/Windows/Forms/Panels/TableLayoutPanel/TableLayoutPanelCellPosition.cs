// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(TableLayoutPanelCellPositionTypeConverter))]
public struct TableLayoutPanelCellPosition : IEquatable<TableLayoutPanelCellPosition>
{
    public TableLayoutPanelCellPosition(int column, int row)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(row, -1);
        ArgumentOutOfRangeException.ThrowIfLessThan(column, -1);

        Row = row;
        Column = column;
    }

    public int Row { get; set; }

    public int Column { get; set; }

    public override readonly bool Equals(object? other)
    {
        if (other is not TableLayoutPanelCellPosition otherCellPosition)
        {
            return false;
        }

        return Equals(otherCellPosition);
    }

    public readonly bool Equals(TableLayoutPanelCellPosition other)
        => Row == other.Row && Column == other.Column;

    public static bool operator ==(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return p1.Row == p2.Row && p1.Column == p2.Column;
    }

    public static bool operator !=(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return !(p1 == p2);
    }

    public override readonly string ToString() => $"{Column},{Row}";

    public override readonly int GetHashCode() => HashCode.Combine(Row, Column);
}
