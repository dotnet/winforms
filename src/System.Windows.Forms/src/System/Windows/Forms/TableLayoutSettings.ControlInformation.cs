// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public sealed partial class TableLayoutSettings
    {
        internal struct ControlInformation
        {
            internal object Name;
            internal int Row;
            internal int Column;
            internal int RowSpan;
            internal int ColumnSpan;

            internal ControlInformation(object name, int row, int column, int rowSpan, int columnSpan)
            {
                Name = name;
                Row = row;
                Column = column;
                RowSpan = rowSpan;
                ColumnSpan = columnSpan;
            }
        }
    }
}
