// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public class DataGridViewSortCompareEventArgs : HandledEventArgs
    {
        private readonly DataGridViewColumn dataGridViewColumn;
        private readonly object cellValue1, cellValue2;
        private int sortResult;
        private readonly int rowIndex1;
        private readonly int rowIndex2;

        public DataGridViewSortCompareEventArgs(DataGridViewColumn dataGridViewColumn,
            object cellValue1,
            object cellValue2,
            int rowIndex1,
            int rowIndex2)
        {
            Debug.Assert(dataGridViewColumn != null);
            Debug.Assert(dataGridViewColumn.Index >= 0);
            this.dataGridViewColumn = dataGridViewColumn;
            this.cellValue1 = cellValue1;
            this.cellValue2 = cellValue2;
            this.rowIndex1 = rowIndex1;
            this.rowIndex2 = rowIndex2;
        }

        public object CellValue1
        {
            get
            {
                return cellValue1;
            }
        }

        public object CellValue2
        {
            get
            {
                return cellValue2;
            }
        }

        public DataGridViewColumn Column
        {
            get
            {
                return dataGridViewColumn;
            }
        }

        public int RowIndex1
        {
            get
            {
                return rowIndex1;
            }
        }

        public int RowIndex2
        {
            get
            {
                return rowIndex2;
            }
        }

        public int SortResult
        {
            get
            {
                return sortResult;
            }
            set
            {
                sortResult = value;
            }
        }
    }
}
