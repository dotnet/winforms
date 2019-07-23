// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class fully encapsulates the painting logic for an addnew row
    ///  appearing in a DataGrid.
    /// </summary>
    internal class DataGridAddNewRow : DataGridRow
    {
        private bool dataBound = false;

        public DataGridAddNewRow(DataGrid dGrid, DataGridTableStyle gridTable, int rowNum)
            : base(dGrid, gridTable, rowNum)
        {
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        /// <summary>
        ///  Since the DataView does not return a valid DataRow for
        ///  a newly added row, the DataGrid sets this property to
        ///  true to signal that the AddNewRow can safely render
        ///  row contents and permit editing, etc because a DataRecord
        ///  exists in the cursor that created this row.
        /// </summary>
        public bool DataBound
        {
            get
            {
                return dataBound;
            }
            set
            {
                dataBound = value;
            }
        }

        public override void OnEdit()
        {
            if (!DataBound)
            {
                DataGrid.AddNewRow();
            }
        }

        public override void OnRowLeave()
        {
            if (DataBound)
            {
                DataBound = false;
            }
        }

        // the addNewRow has nothing to do with losing focus
        //
        internal override void LoseChildFocus(Rectangle rowHeader, bool alignToRight)
        {
        }

        // the newDataRow has nothing to do with TAB keys
        //
        internal override bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight)
        {
            return false;
        }

        /// <summary>
        ///  Paints the row.
        /// </summary>
        public override int Paint(Graphics g, Rectangle bounds, Rectangle trueRowBounds, int firstVisibleColumn, int columnCount)
        {
            return Paint(g, bounds, trueRowBounds, firstVisibleColumn, columnCount, false);
        }

        public override int Paint(Graphics g,
                                  Rectangle bounds,
                                  Rectangle trueRowBounds,
                                  int firstVisibleColumn,
                                  int columnCount,
                                  bool alignToRight)
        {
            Rectangle dataBounds = bounds;
            DataGridLineStyle gridStyle;
            if (dgTable.IsDefault)
            {
                gridStyle = DataGrid.GridLineStyle;
            }
            else
            {
                gridStyle = dgTable.GridLineStyle;
            }

            int bWidth = DataGrid == null ? 0 : gridStyle == DataGridLineStyle.Solid ? 1 : 0;
            dataBounds.Height -= bWidth;
            int cx = base.PaintData(g, dataBounds, firstVisibleColumn, columnCount, alignToRight);

            if (bWidth > 0)
            {
                PaintBottomBorder(g, bounds, cx, bWidth, alignToRight);
            }

            return cx;
        }

        protected override void PaintCellContents(Graphics g, Rectangle cellBounds, DataGridColumnStyle column,
                                                  Brush backBr, Brush foreBrush, bool alignToRight)
        {
            if (DataBound)
            {
                CurrencyManager listManager = DataGrid.ListManager;
                column.Paint(g, cellBounds, listManager, RowNumber, alignToRight);
            }
            else
            {
                base.PaintCellContents(g, cellBounds, column, backBr, foreBrush, alignToRight);
            }
        }
    }
}
