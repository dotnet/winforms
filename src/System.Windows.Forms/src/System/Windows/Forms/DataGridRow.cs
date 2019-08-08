// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates the painting logic for a new row added to a <see cref='Forms.DataGrid'/> control.
    /// </summary>
    internal abstract class DataGridRow : MarshalByRefObject
    {
        internal protected int number;             // row number
        private bool selected;
        private int height;
        // protected DataRow   dataRow;
        private IntPtr tooltipID = new IntPtr(-1);
        private string tooltip = string.Empty;
        AccessibleObject accessibleObject;

        // for accessibility...
        //
        // internal DataGrid  dataGrid;

        // will need this for the painting information ( row header color )
        //
        protected DataGridTableStyle dgTable;

        // we will be mapping only the black color to
        // the HeaderForeColor
        //
        private static readonly ColorMap[] colorMap = new ColorMap[] { new ColorMap() };

        // bitmaps
        //
        private static Bitmap rightArrow = null;
        private static Bitmap leftArrow = null;
        private static Bitmap errorBmp = null;
        private static Bitmap pencilBmp = null;
        private static Bitmap starBmp = null;
        protected const int xOffset = 3;
        protected const int yOffset = 2;

        /// <summary>
        ///  Initializes a new instance of a <see cref='DataGridRow'/> .
        /// </summary>
        public DataGridRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
        {
            if (dataGrid == null || dgTable.DataGrid == null)
            {
                throw new ArgumentNullException(nameof(dataGrid));
            }

            if (rowNumber < 0)
            {
                throw new ArgumentException(SR.DataGridRowRowNumber, "rowNumber");
            }
            // this.dataGrid = dataGrid;
            number = rowNumber;

            // map the black color in the pictures to the DataGrid's HeaderForeColor
            //
            colorMap[0].OldColor = Color.Black;
            colorMap[0].NewColor = dgTable.HeaderForeColor;

            this.dgTable = dgTable;
            height = MinimumRowHeight(dgTable);
        }

        public AccessibleObject AccessibleObject
        {
            get
            {
                if (accessibleObject == null)
                {
                    accessibleObject = CreateAccessibleObject();
                }
                return accessibleObject;
            }
        }

        protected virtual AccessibleObject CreateAccessibleObject()
        {
            return new DataGridRowAccessibleObject(this);
        }

        internal protected virtual int MinimumRowHeight(DataGridTableStyle dgTable)
        {
            return MinimumRowHeight(dgTable.GridColumnStyles);
        }

        internal protected virtual int MinimumRowHeight(GridColumnStylesCollection columns)
        {
            int h = dgTable.IsDefault ? DataGrid.PreferredRowHeight : dgTable.PreferredRowHeight;

            try
            {
                if (dgTable.DataGrid.DataSource != null)
                {
                    int nCols = columns.Count;
                    for (int i = 0; i < nCols; ++i)
                    {
                        // if (columns[i].Visible && columns[i].PropertyDescriptor != null)
                        if (columns[i].PropertyDescriptor != null)
                        {
                            h = Math.Max(h, columns[i].GetMinimumHeight());
                        }
                    }
                }
            }
            catch
            {
            }
            return h;
        }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        /// <summary>
        ///  Gets the <see cref='Forms.DataGrid'/> control the row belongs to.
        /// </summary>
        public DataGrid DataGrid
        {
            get
            {
                return dgTable.DataGrid;
            }
        }

        internal DataGridTableStyle DataGridTableStyle
        {
            get
            {
                return dgTable;
            }
            set
            {
                dgTable = value;
            }
        }

        /*
        public DataGridTable DataGridTable {
            get {
                return dgTable;
            }
        }
        */

        /*
        public DataRow DataRow {
            get {
                return dataRow;
            }
        }
        */

        /// <summary>
        ///  Gets or sets the height of the row.
        /// </summary>
        public virtual int Height
        {
            get
            {
                return height;
            }
            set
            {
                // the height of the row should be at least 0.
                // this way, if the row has a relationship list and the user resizes the row such that
                // the new height does not accomodate the height of the relationship list
                // the row will at least show the relationship list ( and not paint on the portion of the row above this one )
                height = Math.Max(0, value);
                // when we resize the row, or when we set the PreferredRowHeigth on the
                // DataGridTableStyle, we change the height of the Row, which will cause to invalidate,
                // then the grid itself will do another invalidate call.
                dgTable.DataGrid.OnRowHeightChanged(this);
            }
        }

        /// <summary>
        ///  Gets the row's number.
        /// </summary>
        public int RowNumber
        {
            get
            {
                return number;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the row is selected.
        /// </summary>
        public virtual bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                InvalidateRow();
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        /// <summary>
        ///  Gets the bitmap associated with the row.
        /// </summary>
        protected Bitmap GetBitmap(string bitmapName)
        {
            try
            {
                return DpiHelper.GetBitmapFromIcon(typeof(DataGridCaption), bitmapName);
            }
            catch (Exception e)
            {
                Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
                throw e;
            }
        }

        /// <summary>
        ///  When overridden in a derived class, gets the <see cref='Rectangle'/>
        ///  where a cell's contents gets painted.
        /// </summary>
        public virtual Rectangle GetCellBounds(int col)
        {
            int firstVisibleCol = dgTable.DataGrid.FirstVisibleColumn;
            int cx = 0;
            Rectangle cellBounds = new Rectangle();
            GridColumnStylesCollection columns = dgTable.GridColumnStyles;
            if (columns != null)
            {
                for (int i = firstVisibleCol; i < col; i++)
                {
                    if (columns[i].PropertyDescriptor != null)
                    {
                        cx += columns[i].Width;
                    }
                }

                int borderWidth = dgTable.GridLineWidth;
                cellBounds = new Rectangle(cx,
                                     0,
                                     columns[col].Width - borderWidth,
                                     Height - borderWidth);
            }
            return cellBounds;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the <see cref='Rectangle'/> of the non-scrollable area of
        ///  the row.
        /// </summary>
        public virtual Rectangle GetNonScrollableArea()
        {
            return Rectangle.Empty;
        }

        /// <summary>
        ///  Gets or sets the bitmap displayed in the row header of a new row.
        /// </summary>
        protected Bitmap GetStarBitmap()
        {
            if (starBmp == null)
            {
                starBmp = GetBitmap("DataGridRow.star");
            }

            return starBmp;
        }

        /// <summary>
        ///  Gets or sets the bitmap displayed in the row header that indicates a row can
        ///  be edited.
        /// </summary>
        protected Bitmap GetPencilBitmap()
        {
            if (pencilBmp == null)
            {
                pencilBmp = GetBitmap("DataGridRow.pencil");
            }

            return pencilBmp;
        }

        /// <summary>
        ///  Gets or sets the bitmap displayed on a row with an error.
        /// </summary>
        protected Bitmap GetErrorBitmap()
        {
            if (errorBmp == null)
            {
                errorBmp = GetBitmap("DataGridRow.error");
            }

            return errorBmp;
        }

        protected Bitmap GetLeftArrowBitmap()
        {
            if (leftArrow == null)
            {
                leftArrow = GetBitmap("DataGridRow.left");
            }

            return leftArrow;
        }

        protected Bitmap GetRightArrowBitmap()
        {
            if (rightArrow == null)
            {
                rightArrow = GetBitmap("DataGridRow.right");
            }

            return rightArrow;
        }

        public virtual void InvalidateRow()
        {
            dgTable.DataGrid.InvalidateRow(number);
        }

        public virtual void InvalidateRowRect(Rectangle r)
        {
            dgTable.DataGrid.InvalidateRowRect(number, r);
        }

        /// <summary>
        ///  When overridden in a derived class, notifies the grid that an edit will
        ///  occur.
        /// </summary>
        public virtual void OnEdit()
        {
        }

        /// <summary>
        ///  When overridden in a derived class, called by the <see cref='Forms.DataGrid'/> control when a key press occurs on a row with focus.
        /// </summary>
        public virtual bool OnKeyPress(Keys keyData)
        {
            int currentColIndex = dgTable.DataGrid.CurrentCell.ColumnNumber;
            GridColumnStylesCollection columns = dgTable.GridColumnStyles;
            if (columns != null && currentColIndex >= 0 && currentColIndex < columns.Count)
            {
                DataGridColumnStyle currentColumn = columns[currentColIndex];
                if (currentColumn.KeyPress(RowNumber, keyData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Called by the <see cref='Forms.DataGrid'/> when a click occurs in the row's client area
        ///  specifed by the x and y coordinates and the specified <see cref='Rectangle'/>
        ///  .
        /// </summary>
        public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders)
        {
            return OnMouseDown(x, y, rowHeaders, false);
        }

        /// <summary>
        ///  When overridden in a derived class, is called by the <see cref='Forms.DataGrid'/> when a click occurs
        ///  in the row's
        ///  client area, specified by x and y coordinates.
        /// </summary>
        public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight)
        {
            // if we call base.OnMouseDown, then the row could not use this
            // mouse click at all. in that case LoseChildFocus, so the edit control
            // will become visible
            LoseChildFocus(rowHeaders, alignToRight);

            // we did not use this click at all.
            return false;
        }

        public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders)
        {
            return false;
        }

        /// <summary>
        ///  When overridden in a derived class, is called by the <see cref='Forms.DataGrid'/> when
        ///  the mouse moves within the row's client area.
        /// </summary>
        public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
        {
            return false;
        }

        public virtual void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
        {
        }

        public virtual void OnMouseLeft()
        {
        }

        /// <summary>
        ///  When overridden in a derived class, causes the RowEnter event to occur.
        /// </summary>
        public virtual void OnRowEnter() { }
        public virtual void OnRowLeave() { }

        // processes the Tab Key
        // returns TRUE if the TAB key is processed
        internal abstract bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight);

        // tells the dataGridRow that it lost the focus
        internal abstract void LoseChildFocus(Rectangle rowHeaders, bool alignToRight);

        /// <summary>
        ///  Paints the row.
        /// </summary>
        public abstract int Paint(Graphics g,
                                 Rectangle dataBounds,
                                 Rectangle rowBounds,
                                 int firstVisibleColumn,
                                 int numVisibleColumns);

        public abstract int Paint(Graphics g,
                                  Rectangle dataBounds,
                                  Rectangle rowBounds,
                                  int firstVisibleColumn,
                                  int numVisibleColumns,
                                  bool alignToRight);

        /// <summary>
        ///  Draws a border on the bottom DataGrid.GridLineWidth pixels
        ///  of the bounding rectangle passed in.
        /// </summary>
        protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth)
        {
            PaintBottomBorder(g, bounds, dataWidth, dgTable.GridLineWidth, false);
        }

        protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth, int borderWidth, bool alignToRight)
        {
            // paint bottom border
            Rectangle bottomBorder = new Rectangle(alignToRight ? bounds.Right - dataWidth : bounds.X,
                                                   bounds.Bottom - borderWidth,
                                                   dataWidth,
                                                   borderWidth);

            g.FillRectangle(dgTable.IsDefault ? DataGrid.GridLineBrush : dgTable.GridLineBrush, bottomBorder);

            // paint any exposed region to the right
            if (dataWidth < bounds.Width)
            {
                g.FillRectangle(dgTable.DataGrid.BackgroundBrush,
                                alignToRight ? bounds.X : bottomBorder.Right,
                                bottomBorder.Y,
                                bounds.Width - bottomBorder.Width,
                                borderWidth);
            }
        }

        /// <summary>
        ///  Paints the row.
        /// </summary>
        public virtual int PaintData(Graphics g,
                                     Rectangle bounds,
                                     int firstVisibleColumn,
                                     int columnCount)
        {
            return PaintData(g, bounds, firstVisibleColumn, columnCount, false);
        }

        public virtual int PaintData(Graphics g,
                                     Rectangle bounds,
                                     int firstVisibleColumn,
                                     int columnCount,
                                     bool alignToRight)
        {
            Debug.WriteLineIf(CompModSwitches.DGRowPaint.TraceVerbose, "Painting DataGridAddNewRow: bounds = " + bounds.ToString());

            Rectangle cellBounds = bounds;
            int bWidth = dgTable.IsDefault ? DataGrid.GridLineWidth : dgTable.GridLineWidth;
            int cx = 0;

            DataGridCell current = dgTable.DataGrid.CurrentCell;

            GridColumnStylesCollection columns = dgTable.GridColumnStyles;
            int nCols = columns.Count;
            for (int col = firstVisibleColumn; col < nCols; ++col)
            {
                if (cx > bounds.Width)
                {
                    break;
                }

                // if (!columns[col].Visible || columns[col].PropertyDescriptor == null)
                if (columns[col].PropertyDescriptor == null || columns[col].Width <= 0)
                {
                    continue;
                }

                cellBounds.Width = columns[col].Width - bWidth;

                if (alignToRight)
                {
                    cellBounds.X = bounds.Right - cx - cellBounds.Width;
                }
                else
                {
                    cellBounds.X = bounds.X + cx;
                }

                // Paint the data with the the DataGridColumn
                Brush backBr = BackBrushForDataPaint(ref current, columns[col], col);
                Brush foreBrush = ForeBrushForDataPaint(ref current, columns[col], col);

                PaintCellContents(g,
                                  cellBounds,
                                  columns[col],
                                  backBr,
                                  foreBrush,
                                  alignToRight);

                // Paint the border to the right of each cell
                if (bWidth > 0)
                {
                    g.FillRectangle(dgTable.IsDefault ? DataGrid.GridLineBrush : dgTable.GridLineBrush,
                                    alignToRight ? cellBounds.X - bWidth : cellBounds.Right,
                                    cellBounds.Y,
                                    bWidth,
                                    cellBounds.Height);
                }
                cx += cellBounds.Width + bWidth;
            }

            // Paint any exposed area to the right ( or left ) of the data cell area
            if (cx < bounds.Width)
            {
                g.FillRectangle(dgTable.DataGrid.BackgroundBrush,
                                alignToRight ? bounds.X : bounds.X + cx,
                                bounds.Y,
                                bounds.Width - cx,
                                bounds.Height);
            }
            return cx;
        }

        protected virtual void PaintCellContents(Graphics g, Rectangle cellBounds, DataGridColumnStyle column,
                                                 Brush backBr, Brush foreBrush)
        {
            PaintCellContents(g, cellBounds, column, backBr, foreBrush, false);
        }

        protected virtual void PaintCellContents(Graphics g, Rectangle cellBounds, DataGridColumnStyle column,
                                                 Brush backBr, Brush foreBrush, bool alignToRight)
        {
            g.FillRectangle(backBr, cellBounds);
        }

        //
        // This function will do the following: if paintIcon is set to true, then
        // will draw the image on the RowHeader. if paintIcon is set to false,
        // then this function will fill the rectangle on which otherwise will
        // have been drawn the image
        //
        // will return the rectangle that includes the Icon
        //
        protected Rectangle PaintIcon(Graphics g, Rectangle visualBounds, bool paintIcon, bool alignToRight, Bitmap bmp)
        {
            return PaintIcon(g, visualBounds, paintIcon, alignToRight, bmp,
                             dgTable.IsDefault ? DataGrid.HeaderBackBrush : dgTable.HeaderBackBrush);
        }
        protected Rectangle PaintIcon(Graphics g, Rectangle visualBounds, bool paintIcon, bool alignToRight, Bitmap bmp, Brush backBrush)
        {
            Size bmpSize = bmp.Size;
            Rectangle bmpRect = new Rectangle(alignToRight ? visualBounds.Right - xOffset - bmpSize.Width : visualBounds.X + xOffset,
                                              visualBounds.Y + yOffset,
                                              bmpSize.Width,
                                              bmpSize.Height);
            g.FillRectangle(backBrush, visualBounds);
            if (paintIcon)
            {
                colorMap[0].NewColor = dgTable.IsDefault ? DataGrid.HeaderForeColor : dgTable.HeaderForeColor;
                colorMap[0].OldColor = Color.Black;
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
                g.DrawImage(bmp, bmpRect, 0, 0, bmpRect.Width, bmpRect.Height, GraphicsUnit.Pixel, attr);
                // g.DrawImage(bmp, bmpRect);
                attr.Dispose();
            }

            return bmpRect;
        }

        // assume that the row is not aligned to right, and that the row is not dirty
        public virtual void PaintHeader(Graphics g, Rectangle visualBounds)
        {
            PaintHeader(g, visualBounds, false);
        }

        // assume that the row is not dirty
        public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight)
        {
            PaintHeader(g, visualBounds, alignToRight, false);
        }

        public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight, bool rowIsDirty)
        {
            Rectangle bounds = visualBounds;

            // paint the first part of the row header: the Arror or Pencil/Star
            Bitmap bmp;
            if (this is DataGridAddNewRow)
            {
                bmp = GetStarBitmap();
                lock (bmp)
                {
                    bounds.X += PaintIcon(g, bounds, true, alignToRight, bmp).Width + xOffset;
                }
                return;
            }
            else if (rowIsDirty)
            {
                bmp = GetPencilBitmap();
                lock (bmp)
                {
                    bounds.X += PaintIcon(g, bounds, RowNumber == DataGrid.CurrentCell.RowNumber, alignToRight, bmp).Width + xOffset;
                }
            }
            else
            {
                bmp = alignToRight ? GetLeftArrowBitmap() : GetRightArrowBitmap();
                lock (bmp)
                {
                    bounds.X += PaintIcon(g, bounds, RowNumber == DataGrid.CurrentCell.RowNumber, alignToRight, bmp).Width + xOffset;
                }
            }

            // Paint the error icon
            //
            object errorInfo = DataGrid.ListManager[number];
            if (!(errorInfo is IDataErrorInfo))
            {
                return;
            }

            string errString = ((IDataErrorInfo)errorInfo).Error;
            if (errString == null)
            {
                errString = string.Empty;
            }

            if (tooltip != errString)
            {
                if (!string.IsNullOrEmpty(tooltip))
                {
                    DataGrid.ToolTipProvider.RemoveToolTip(tooltipID);
                    tooltip = string.Empty;
                    tooltipID = new IntPtr(-1);
                }
            }

            if (string.IsNullOrEmpty(errString))
            {
                return;
            }

            // we now have an error string: paint the errorIcon and add the tooltip
            Rectangle errRect;
            bmp = GetErrorBitmap();
            lock (bmp)
            {
                errRect = PaintIcon(g, bounds, true, alignToRight, bmp);
            }
            bounds.X += errRect.Width + xOffset;

            tooltip = errString;
            tooltipID = (IntPtr)((int)DataGrid.ToolTipId++);
            DataGrid.ToolTipProvider.AddToolTip(tooltip, tooltipID, errRect);
        }

        protected Brush GetBackBrush()
        {
            Brush br = dgTable.IsDefault ? DataGrid.BackBrush : dgTable.BackBrush;
            if (DataGrid.LedgerStyle && (RowNumber % 2 == 1))
            {
                br = dgTable.IsDefault ? DataGrid.AlternatingBackBrush : dgTable.AlternatingBackBrush;
            }
            return br;
        }

        /// <summary>
        ///  Returns the BackColor and TextColor  that the Graphics object should use
        ///  for the appropriate values for a given row and column when painting the data.
        /// </summary>
        protected Brush BackBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
        {
            Brush backBr = GetBackBrush();

            if (Selected)
            {
                backBr = dgTable.IsDefault ? DataGrid.SelectionBackBrush : dgTable.SelectionBackBrush;
            }
            /*
            if (RowNumber == current.RowNumber && column == current.ColumnNumber) {
                backBr = grid.CurrentCellBackBrush;
            }
            */
            return backBr;
        }

        protected Brush ForeBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
        {
            // Brush foreBrush = gridColumn.ForeBrush;
            Brush foreBrush = dgTable.IsDefault ? DataGrid.ForeBrush : dgTable.ForeBrush;

            if (Selected)
            {
                foreBrush = dgTable.IsDefault ? DataGrid.SelectionForeBrush : dgTable.SelectionForeBrush;
            }
            /*
            if (RowNumber == current.RowNumber && column == current.ColumnNumber) {
                foreColor = grid.CurrentCellForeColor;
            }
            */
            return foreBrush;
        }

        [ComVisible(true)]
        protected class DataGridRowAccessibleObject : AccessibleObject
        {
            ArrayList cells;
            readonly DataGridRow owner = null;

            internal static string CellToDisplayString(DataGrid grid, int row, int column)
            {
                if (column < grid.myGridTable.GridColumnStyles.Count)
                {
                    return grid.myGridTable.GridColumnStyles[column].PropertyDescriptor.Converter.ConvertToString(grid[row, column]);
                }
                else
                {
                    return "";
                }
            }

            internal static object DisplayStringToCell(DataGrid grid, int row, int column, string value)
            {
                if (column < grid.myGridTable.GridColumnStyles.Count)
                {
                    return grid.myGridTable.GridColumnStyles[column].PropertyDescriptor.Converter.ConvertFromString(value);
                }
                // ignore...
                //
                return null;
            }

            public DataGridRowAccessibleObject(DataGridRow owner) : base()
            {
                Debug.Assert(owner != null, "DataGridRowAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
                DataGrid grid = DataGrid;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create row accessible object");

                EnsureChildren();
            }

            private void EnsureChildren()
            {
                if (cells == null)
                {
                    // default size... little extra for relationships...
                    //
                    cells = new ArrayList(DataGrid.myGridTable.GridColumnStyles.Count + 2);
                    AddChildAccessibleObjects(cells);
                }
            }

            protected virtual void AddChildAccessibleObjects(IList children)
            {
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create row's accessible children");
                Debug.Indent();
                GridColumnStylesCollection cols = DataGrid.myGridTable.GridColumnStyles;
                int len = cols.Count;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, len + " columns present");
                for (int i = 0; i < len; i++)
                {
                    children.Add(CreateCellAccessibleObject(i));
                }
                Debug.Unindent();
            }

            protected virtual AccessibleObject CreateCellAccessibleObject(int column)
            {
                return new DataGridCellAccessibleObject(owner, column);
            }

            public override Rectangle Bounds
            {
                get
                {
                    return DataGrid.RectangleToScreen(DataGrid.GetRowBounds(owner));
                }
            }

            public override string Name
            {
                get
                {
                    if (owner is DataGridAddNewRow)
                    {
                        return SR.AccDGNewRow;
                    }
                    else
                    {
                        return DataGridRowAccessibleObject.CellToDisplayString(DataGrid, owner.RowNumber, 0);
                    }
                }
            }

            protected DataGridRow Owner
            {
                get
                {
                    return owner;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return DataGrid.AccessibilityObject;
                }
            }

            private DataGrid DataGrid
            {
                get
                {
                    return owner.DataGrid;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Row;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (DataGrid.CurrentCell.RowNumber == owner.RowNumber)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    // Determine selected
                    //
                    if (DataGrid.CurrentRowIndex == owner.RowNumber)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    return Name;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < cells.Count)
                {
                    return (AccessibleObject)cells[index];
                }

                return null;
            }

            public override int GetChildCount()
            {
                return cells.Count;
            }

            /// <summary>
            ///  Returns the currently focused child, if any.
            ///  Returns this if the object itself is focused.
            /// </summary>
            public override AccessibleObject GetFocused()
            {
                if (DataGrid.Focused)
                {
                    DataGridCell cell = DataGrid.CurrentCell;
                    if (cell.RowNumber == owner.RowNumber)
                    {
                        return (AccessibleObject)cells[cell.ColumnNumber];
                    }
                }

                return null;
            }

            /// <summary>
            ///  Navigate to the next or previous grid entry.entry.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                switch (navdir)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber + 1);

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber - 1);

                    case AccessibleNavigation.FirstChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(0);
                        }
                        break;
                    case AccessibleNavigation.LastChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(GetChildCount() - 1);
                        }
                        break;
                }

                return null;

            }

            public override void Select(AccessibleSelection flags)
            {
                // Focus the PropertyGridView window
                //
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    DataGrid.Focus();
                }

                // Select the grid entry
                //
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    DataGrid.CurrentRowIndex = owner.RowNumber;
                }
            }

        }

        [ComVisible(true)]
        protected class DataGridCellAccessibleObject : AccessibleObject
        {
            readonly DataGridRow owner = null;
            readonly int column;

            public DataGridCellAccessibleObject(DataGridRow owner, int column) : base()
            {
                Debug.Assert(owner != null, "DataGridColumnAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
                this.column = column;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create cell accessible object");
            }

            public override Rectangle Bounds
            {
                get
                {
                    return DataGrid.RectangleToScreen(DataGrid.GetCellBounds(new DataGridCell(owner.RowNumber, column)));
                }
            }

            public override string Name
            {
                get
                {
                    return DataGrid.myGridTable.GridColumnStyles[column].HeaderText;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return owner.AccessibleObject;
                }
            }

            protected DataGrid DataGrid
            {
                get
                {
                    return owner.DataGrid;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.AccDGEdit;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (DataGrid.CurrentCell.RowNumber == owner.RowNumber
                        && DataGrid.CurrentCell.ColumnNumber == column)
                    {
                        if (DataGrid.Focused)
                        {
                            state |= AccessibleStates.Focused;
                        }
                        state |= AccessibleStates.Selected;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    if (owner is DataGridAddNewRow)
                    {
                        return null;
                    }
                    else
                    {
                        return DataGridRowAccessibleObject.CellToDisplayString(DataGrid, owner.RowNumber, column);
                    }
                }

                set
                {
                    if (!(owner is DataGridAddNewRow))
                    {
                        object realValue = DataGridRowAccessibleObject.DisplayStringToCell(DataGrid, owner.RowNumber, column, value);
                        DataGrid[owner.RowNumber, column] = realValue;
                    }
                }
            }

            public override void DoDefaultAction()
            {
                Select(AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection);
            }

            /// <summary>
            ///  Returns the currently focused child, if any.
            ///  Returns this if the object itself is focused.
            /// </summary>
            public override AccessibleObject GetFocused()
            {
                // Datagrid always returns the cell as the focused thing... so do we!
                //
                return DataGrid.AccessibilityObject.GetFocused();
            }

            /// <summary>
            ///  Navigate to the next or previous grid entry.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                switch (navdir)
                {
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        if (column < owner.AccessibleObject.GetChildCount() - 1)
                        {
                            return owner.AccessibleObject.GetChild(column + 1);
                        }
                        else
                        {
                            AccessibleObject o = DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber + 1);
                            if (o != null)
                            {
                                return o.Navigate(AccessibleNavigation.FirstChild);
                            }
                        }
                        break;
                    case AccessibleNavigation.Down:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber + 1).Navigate(AccessibleNavigation.FirstChild);
                    case AccessibleNavigation.Up:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber - 1).Navigate(AccessibleNavigation.FirstChild);
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        if (column > 0)
                        {
                            return owner.AccessibleObject.GetChild(column - 1);
                        }
                        else
                        {
                            AccessibleObject o = DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber - 1);
                            if (o != null)
                            {
                                return o.Navigate(AccessibleNavigation.LastChild);
                            }
                        }
                        break;

                    case AccessibleNavigation.FirstChild:
                    case AccessibleNavigation.LastChild:

                        break;
                }

                return null;

            }

            public override void Select(AccessibleSelection flags)
            {
                // Focus the PropertyGridView window
                //
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    DataGrid.Focus();
                }

                // Select the grid entry
                //
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    DataGrid.CurrentCell = new DataGridCell(owner.RowNumber, column);
                }
            }

        }
    }
}
