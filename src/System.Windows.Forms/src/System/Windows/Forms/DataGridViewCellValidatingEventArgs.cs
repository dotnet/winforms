// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    public class DataGridViewCellValidatingEventArgs : CancelEventArgs
    {
        internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object formattedValue)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            FormattedValue = formattedValue;
        }

        public int ColumnIndex { get; }

        public int RowIndex { get; }

        public object FormattedValue { get; }
    }
}
