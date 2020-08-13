// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public class DataGridViewSortCompareEventArgs : HandledEventArgs
    {
        public DataGridViewSortCompareEventArgs(DataGridViewColumn dataGridViewColumn,
            object cellValue1,
            object cellValue2,
            int rowIndex1,
            int rowIndex2)
        {
            Debug.Assert(dataGridViewColumn != null);
            Debug.Assert(dataGridViewColumn.Index >= 0);
            Column = dataGridViewColumn;
            CellValue1 = cellValue1;
            CellValue2 = cellValue2;
            RowIndex1 = rowIndex1;
            RowIndex2 = rowIndex2;
        }

        public object CellValue1 { get; }

        public object CellValue2 { get; }

        public DataGridViewColumn Column { get; }

        public int RowIndex1 { get; }

        public int RowIndex2 { get; }

        public int SortResult { get; set; }
    }
}
