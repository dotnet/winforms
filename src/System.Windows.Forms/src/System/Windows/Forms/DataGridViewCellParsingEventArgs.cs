// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.ComponentModel;

    public class DataGridViewCellParsingEventArgs : ConvertEventArgs
    {
        private int rowIndex, columnIndex;
        private DataGridViewCellStyle inheritedCellStyle;
        private bool parsingApplied;
    
        public DataGridViewCellParsingEventArgs(int rowIndex,
                                         int columnIndex,
                                         object value,
                                         Type desiredType,
                                         DataGridViewCellStyle inheritedCellStyle) : base(value, desiredType)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.inheritedCellStyle = inheritedCellStyle;
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        public DataGridViewCellStyle InheritedCellStyle
        {
            get
            {
                return this.inheritedCellStyle;
            }
            set
            {
                this.inheritedCellStyle = value;
            }
        }

        public bool ParsingApplied
        {
            get
            {
                return this.parsingApplied;
            }
            set
            {
                this.parsingApplied = value;
            }
        }
    }
}
