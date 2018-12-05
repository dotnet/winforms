// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs"]/*' />
    public class DataGridViewRowPostPaintEventArgs : EventArgs
    {
        private DataGridView dataGridView;
        private Graphics graphics;
        private Rectangle clipBounds;
        private Rectangle rowBounds;
        private DataGridViewCellStyle inheritedRowStyle;
        private int rowIndex;
        private DataGridViewElementStates rowState;
        private string errorText;
        private bool isFirstDisplayedRow;
        private bool isLastVisibleRow;

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.DataGridViewRowPostPaintEventArgs"]/*' />
        public DataGridViewRowPostPaintEventArgs(DataGridView dataGridView,
                                                 Graphics graphics, 
                                                 Rectangle clipBounds, 
                                                 Rectangle rowBounds,
                                                 int rowIndex,
                                                 DataGridViewElementStates rowState,
                                                 string errorText,
                                                 DataGridViewCellStyle inheritedRowStyle,
                                                 bool isFirstDisplayedRow,
                                                 bool isLastVisibleRow)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (inheritedRowStyle == null)
            {
                throw new ArgumentNullException(nameof(inheritedRowStyle));
            }
            this.dataGridView = dataGridView;
            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.rowBounds = rowBounds;
            this.rowIndex = rowIndex;
            this.rowState = rowState;
            this.errorText = errorText;
            this.inheritedRowStyle = inheritedRowStyle;
            this.isFirstDisplayedRow = isFirstDisplayedRow;
            this.isLastVisibleRow = isLastVisibleRow;
        }

        internal DataGridViewRowPostPaintEventArgs(DataGridView dataGridView)
        {
            Debug.Assert(dataGridView != null);
            this.dataGridView = dataGridView;
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.ClipBounds"]/*' />
        public Rectangle ClipBounds
        {
            get
            {
                return this.clipBounds;
            }
            set
            {
                this.clipBounds = value;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.ErrorText"]/*' />
        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.Graphics"]/*' />
        public Graphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.InheritedRowStyle"]/*' />
        public DataGridViewCellStyle InheritedRowStyle
        {
            get
            {
                return this.inheritedRowStyle;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.IsFirstDisplayedRow"]/*' />
        public bool IsFirstDisplayedRow
        {
            get
            {
                return this.isFirstDisplayedRow;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.IsLastVisibleRow"]/*' />
        public bool IsLastVisibleRow
        {
            get
            {
                return this.isLastVisibleRow;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.RowBounds"]/*' />
        public Rectangle RowBounds
        {
            get
            {
                return this.rowBounds;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.State"]/*' />
        public DataGridViewElementStates State
        {
            get
            {
                return this.rowState;
            }
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.DrawFocus"]/*' />
        public void DrawFocus(Rectangle bounds, bool cellsPaintSelectionBackground)
        {
            if (this.rowIndex < 0 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            this.dataGridView.Rows.SharedRow(rowIndex).DrawFocus(this.graphics, 
                                                                 this.clipBounds, 
                                                                 bounds, 
                                                                 this.rowIndex, 
                                                                 this.rowState,
                                                                 this.inheritedRowStyle,
                                                                 cellsPaintSelectionBackground);
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.PaintCells"]/*' />
        public void PaintCells(Rectangle clipBounds, DataGridViewPaintParts paintParts)
        {
            if (this.rowIndex < 0 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            this.dataGridView.Rows.SharedRow(rowIndex).PaintCells(this.graphics,
                                                                  clipBounds,
                                                                  this.rowBounds,
                                                                  this.rowIndex,
                                                                  this.rowState,
                                                                  this.isFirstDisplayedRow,
                                                                  this.isLastVisibleRow,
                                                                  paintParts);
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.PaintCellsBackground"]/*' />
        public void PaintCellsBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
        {
            if (this.rowIndex < 0 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
            if (cellsPaintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            this.dataGridView.Rows.SharedRow(rowIndex).PaintCells(this.graphics,
                                                                  clipBounds,
                                                                  this.rowBounds,
                                                                  this.rowIndex,
                                                                  this.rowState,
                                                                  this.isFirstDisplayedRow,
                                                                  this.isLastVisibleRow,
                                                                  paintParts);
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.PaintCellsContent"]/*' />
        public void PaintCellsContent(Rectangle clipBounds)
        {
            if (this.rowIndex < 0 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            this.dataGridView.Rows.SharedRow(rowIndex).PaintCells(this.graphics,
                                                                  clipBounds,
                                                                  this.rowBounds,
                                                                  this.rowIndex,
                                                                  this.rowState,
                                                                  this.isFirstDisplayedRow,
                                                                  this.isLastVisibleRow,
                                                                  DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.PaintHeader1"]/*' />
        public void PaintHeader(bool paintSelectionBackground)
        {
            DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon;
            if (paintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            PaintHeader(paintParts);
        }

        /// <include file='doc\DataGridViewRowPostPaintEventArgs.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventArgs.PaintHeader2"]/*' />
        public void PaintHeader(DataGridViewPaintParts paintParts)
        {
            if (this.rowIndex < 0 || this.rowIndex >= this.dataGridView.Rows.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange));
            }
            this.dataGridView.Rows.SharedRow(rowIndex).PaintHeader(this.graphics,
                                                                   this.clipBounds,
                                                                   this.rowBounds,
                                                                   this.rowIndex,
                                                                   this.rowState,
                                                                   this.isFirstDisplayedRow,
                                                                   this.isLastVisibleRow,
                                                                   paintParts);
        }

        internal void SetProperties(Graphics graphics,
                                    Rectangle clipBounds, 
                                    Rectangle rowBounds, 
                                    int rowIndex, 
                                    DataGridViewElementStates rowState, 
                                    string errorText,
                                    DataGridViewCellStyle inheritedRowStyle,
                                    bool isFirstDisplayedRow, 
                                    bool isLastVisibleRow)
        {
            Debug.Assert(graphics != null);
            Debug.Assert(inheritedRowStyle != null);

            this.graphics = graphics;
            this.clipBounds = clipBounds;
            this.rowBounds = rowBounds;
            this.rowIndex = rowIndex;
            this.rowState = rowState;
            this.errorText = errorText;
            this.inheritedRowStyle = inheritedRowStyle;
            this.isFirstDisplayedRow = isFirstDisplayedRow;
            this.isLastVisibleRow = isLastVisibleRow;
        }
    }
}
