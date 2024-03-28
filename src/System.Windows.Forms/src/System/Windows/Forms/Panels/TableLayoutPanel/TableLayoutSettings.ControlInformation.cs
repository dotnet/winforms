// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public sealed partial class TableLayoutSettings
{
    internal struct ControlInformation
    {
        // These need careful review to convert to properties to ensure they are set on the intended ControlInformation
        // struct and not an implicit copy.

#pragma warning disable IDE1006 // Naming Styles
        internal object? Name;
        internal int Row;
        internal int Column;
        internal int RowSpan;
        internal int ColumnSpan;
#pragma warning restore IDE1006

        internal ControlInformation(
            object? name,
            int row,
            int column,
            int rowSpan,
            int columnSpan)
        {
            Name = name;
            Row = row;
            Column = column;
            RowSpan = rowSpan;
            ColumnSpan = columnSpan;
        }
    }
}
