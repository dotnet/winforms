// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.ComponentModel;

    public class DataGridViewCellPaintingEventArgs : HandledEventArgs
    {
        private DataGridView dataGridView;
        private Graphics graphics;
        private Rectangle clipBounds;
        private Rectangle cellBounds;
        private int rowIndex, columnIndex;
        private DataGridViewElementStates cellState;
        private object value;
        private object formattedValue;
        private string errorText;
        private DataGridViewCellStyle cellStyle;
        private DataGridViewAdvancedBorderStyle advancedBorderStyle;
        private DataGridViewPaintParts paintParts;

        public DataGridViewCellPaintingEventArgs(DataGridView dataGridView,
                                                 Graphics graphics, 
                                                 Rectangle clipBounds,
                                                 Rectangle cellBounds, 
                                                 int rowIndex, 
                                                 int columnIndex, 
                                                 DataGridViewElementStates cellState,
                                                 object value,
                                                 object formattedValue,
                                                 string errorText,
                                                 DataGridViewCellStyle cellStyle,
                                                 DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                                 DataGridViewPaintParts paintParts)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }
            if ((paintParts & ~DataGridViewPaintParts.All) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, "paintParts"));
            }
            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.cellBounds = cellBounds;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.cellState = cellState;
            this.value = value;
            this.formattedValue = formattedValue;
            this.errorText = errorText;
            this.cellStyle = cellStyle;
            this.advancedBorderStyle = advancedBorderStyle;
            this.paintParts = paintParts;
        }

        internal DataGridViewCellPaintingEventArgs(DataGridView dataGridView)
        {
            Debug.Assert(dataGridView != null);
            this.dataGridView = dataGridView;
        }

        public DataGridViewAdvancedBorderStyle AdvancedBorderStyle
        {
            get
            {
                return this.advancedBorderStyle;
            }
        }

        public Rectangle CellBounds
        {
            get
            {
                return this.cellBounds;
            }
        }

        public DataGridViewCellStyle CellStyle
        {
            get
            {
                return this.cellStyle;
            }
        }

        public Rectangle ClipBounds
        {
            get
            {
                return this.clipBounds;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }

        public object FormattedValue
        {
            get
            {
                return this.formattedValue;
            }
        }

        public Graphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }

        public DataGridViewPaintParts PaintParts
        {
            get
            {
                return this.paintParts;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        public DataGridViewElementStates State 
        {
            get 
            {
                return this.cellState;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
        }

        public void Paint(Rectangle clipBounds, DataGridViewPaintParts paintParts)
        {
            if (this.rowIndex < -1 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            if (this.columnIndex < -1 || this.columnIndex >= this.dataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange));
            }
            this.dataGridView.GetCellInternal(this.columnIndex, this.rowIndex).PaintInternal(this.graphics,
                                                                                             clipBounds,
                                                                                             this.cellBounds,
                                                                                             this.rowIndex,
                                                                                             this.cellState,
                                                                                             this.value,
                                                                                             this.formattedValue,
                                                                                             this.errorText,
                                                                                             this.cellStyle,
                                                                                             this.advancedBorderStyle,
                                                                                             paintParts);
        }

        public void PaintBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
        {
            if (this.rowIndex < -1 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            if (this.columnIndex < -1 || this.columnIndex >= this.dataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange));
            }
            DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
            if (cellsPaintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            this.dataGridView.GetCellInternal(this.columnIndex, this.rowIndex).PaintInternal(this.graphics,
                                                                                             clipBounds,
                                                                                             this.cellBounds,
                                                                                             this.rowIndex,
                                                                                             this.cellState,
                                                                                             this.value,
                                                                                             this.formattedValue,
                                                                                             this.errorText,
                                                                                             this.cellStyle,
                                                                                             this.advancedBorderStyle,
                                                                                             paintParts);
        }

        public void PaintContent(Rectangle clipBounds)
        {
            if (this.rowIndex < -1 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            if (this.columnIndex < -1 || this.columnIndex >= this.dataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange));
            }
            this.dataGridView.GetCellInternal(this.columnIndex, this.rowIndex).PaintInternal(this.graphics,
                                                                                             clipBounds,
                                                                                             this.cellBounds,
                                                                                             this.rowIndex,
                                                                                             this.cellState,
                                                                                             this.value,
                                                                                             this.formattedValue,
                                                                                             this.errorText,
                                                                                             this.cellStyle,
                                                                                             this.advancedBorderStyle,
                                                                                             DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
        }

        internal void SetProperties(Graphics graphics,
                                    Rectangle clipBounds,
                                    Rectangle cellBounds, 
                                    int rowIndex, 
                                    int columnIndex, 
                                    DataGridViewElementStates cellState,
                                    object value,
                                    object formattedValue,
                                    string errorText,
                                    DataGridViewCellStyle cellStyle,
                                    DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                    DataGridViewPaintParts paintParts)
        {
            Debug.Assert(graphics != null);
            Debug.Assert(cellStyle != null);

            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.cellBounds = cellBounds;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.cellState = cellState;
            this.value = value;
            this.formattedValue = formattedValue;
            this.errorText = errorText;
            this.cellStyle = cellStyle;
            this.advancedBorderStyle = advancedBorderStyle;
            this.paintParts = paintParts;
            this.Handled = false;
        }
    }
}
