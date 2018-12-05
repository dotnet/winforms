// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.ComponentModel;

    public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
    {
        private int columnIndex, rowIndex;
        private DataGridViewCellStyle cellStyle;
        private bool formattingApplied;
    
        public DataGridViewCellFormattingEventArgs(int columnIndex,
                                                   int rowIndex,
                                                   object value,
                                                   Type desiredType,
                                                   DataGridViewCellStyle cellStyle) : base(value, desiredType)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            this.columnIndex = columnIndex;
            this.rowIndex = rowIndex;
            this.cellStyle = cellStyle;
        }

        public DataGridViewCellStyle CellStyle
        {
            get
            {
                return this.cellStyle;
            }
            set
            {
                this.cellStyle = value;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        public bool FormattingApplied
        {
            get
            {
                return this.formattingApplied;
            }
            set
            {
                this.formattingApplied = value;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
