// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.Runtime.Versioning;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Runtime.InteropServices;
    
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Security.Permissions;
    using Microsoft.Win32;
    using System.Collections;

    /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow"]/*' />
    /// <devdoc>
    ///    <para>Encapsulates the painting logic for a new row added to a 
    ///    <see cref='System.Windows.Forms.DataGrid'/> 
    ///    control.</para>
    /// </devdoc>
    internal abstract class DataGridRow : MarshalByRefObject {
        internal protected int       number;             // row number
        private bool      selected;
        private int       height;
        // protected DataRow   dataRow;
        private IntPtr       tooltipID = new IntPtr(-1);
        private string    tooltip = String.Empty;
        AccessibleObject  accessibleObject;

        // for accessibility...
        //
        // internal DataGrid  dataGrid;

        // will need this for the painting information ( row header color )
        // 
        protected DataGridTableStyle dgTable;

        // we will be mapping only the black color to 
        // the HeaderForeColor
        //
        private static ColorMap[] colorMap = new ColorMap[] {new ColorMap()};

        // bitmaps
        //
        private static Bitmap rightArrow = null;
        private static Bitmap leftArrow = null;
        private static Bitmap errorBmp = null;
        private static Bitmap pencilBmp = null;
        private static Bitmap starBmp = null;
        protected const int xOffset = 3;
        protected const int yOffset = 2;


        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGridRow"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of a <see cref='System.Windows.Forms.DataGridRow'/> . </para>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // This class and its derived classes are internal.
                                                                                                    // So this is not a security back door.
        ]
        public DataGridRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber) {
            if (dataGrid == null || dgTable.DataGrid == null)
                throw new ArgumentNullException(nameof(dataGrid));
            if (rowNumber < 0)
                throw new ArgumentException(SR.DataGridRowRowNumber, "rowNumber");
            // this.dataGrid = dataGrid;
            this.number = rowNumber;

            // map the black color in the pictures to the DataGrid's HeaderForeColor
            //
            colorMap[0].OldColor = Color.Black;
            colorMap[0].NewColor = dgTable.HeaderForeColor;

            this.dgTable = dgTable;
            height = MinimumRowHeight(dgTable);
        }

        public AccessibleObject AccessibleObject {
            get {
                if (accessibleObject == null) {
                    accessibleObject = CreateAccessibleObject();
                }
                return accessibleObject;
            }
        }

        protected virtual AccessibleObject CreateAccessibleObject() {
            return new DataGridRowAccessibleObject(this);
        }

        internal protected virtual int MinimumRowHeight(DataGridTableStyle dgTable) {
            return MinimumRowHeight(dgTable.GridColumnStyles);
        }

        internal protected virtual int MinimumRowHeight(GridColumnStylesCollection columns) {
            int h = dgTable.IsDefault ? this.DataGrid.PreferredRowHeight : dgTable.PreferredRowHeight;

            try {
                if (this.dgTable.DataGrid.DataSource != null) {
                    int nCols = columns.Count;
                    for (int i = 0; i < nCols; ++i) {
                        // if (columns[i].Visible && columns[i].PropertyDescriptor != null)
                        if (columns[i].PropertyDescriptor != null)
                            h = Math.Max(h, columns[i].GetMinimumHeight());
                    }
                }
            }
            catch {
            }
            return h;
        }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGrid"]/*' />
        /// <devdoc>
        /// <para>Gets the <see cref='System.Windows.Forms.DataGrid'/> control the row belongs to.</para>
        /// </devdoc>
        public DataGrid DataGrid {
            get {
                return this.dgTable.DataGrid;
            }
        }

        internal DataGridTableStyle DataGridTableStyle {
            get {
                return this.dgTable;
            }
            set {
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

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.Height"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the height of the row.</para>
        /// </devdoc>
        public virtual int Height {
            get {
                return height;
            }
            set {
                // the height of the row should be at least 0.
                // this way, if the row has a relationship list and the user resizes the row such that
                // the new height does not accomodate the height of the relationship list
                // the row will at least show the relationship list ( and not paint on the portion of the row above this one )
                height = Math.Max(0, value);
                // when we resize the row, or when we set the PreferredRowHeigth on the
                // DataGridTableStyle, we change the height of the Row, which will cause to invalidate,
                // then the grid itself will do another invalidate call.
                this.dgTable.DataGrid.OnRowHeightChanged(this);
            }
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.RowNumber"]/*' />
        /// <devdoc>
        ///    <para>Gets the row's number.</para>
        /// </devdoc>
        public int RowNumber {
            get {
                return this.number;
            }
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.Selected"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the row is selected.</para>
        /// </devdoc>
        public virtual bool Selected {
            get {
                return selected;
            }
            set {
                selected = value;
                InvalidateRow();
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetBitmap"]/*' />
        /// <devdoc>
        ///    <para>Gets the bitmap associated with the row.</para>
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetBitmap(string bitmapName) {
            Bitmap b = null;
            try {
                b = new Bitmap(typeof(DataGridCaption), bitmapName);
                b.MakeTransparent();
            }
            catch (Exception e) {
                Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
                throw e;
            }
            return b;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetCellBounds"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, gets the <see cref='System.Drawing.Rectangle'/> 
        /// where a cell's contents gets painted.</para>
        /// </devdoc>
        public virtual Rectangle GetCellBounds(int col) {
            int firstVisibleCol = this.dgTable.DataGrid.FirstVisibleColumn;
            int cx = 0;
            Rectangle cellBounds = new Rectangle();
            GridColumnStylesCollection columns = this.dgTable.GridColumnStyles;
            if (columns != null) {
                for (int i = firstVisibleCol; i < col; i++)
                    if (columns[i].PropertyDescriptor != null)
                        cx += columns[i].Width;

                int borderWidth = this.dgTable.GridLineWidth;
                cellBounds = new Rectangle(cx,
                                     0,
                                     columns[col].Width - borderWidth,
                                     Height - borderWidth);
            }
            return cellBounds;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetNonScrollableArea"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, gets the <see cref='System.Drawing.Rectangle'/> of the non-scrollable area of 
        ///    the row.</para>
        /// </devdoc>
        public virtual Rectangle GetNonScrollableArea() {
            return Rectangle.Empty;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetStarBitmap"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the bitmap displayed in the row header of a new row.</para>
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetStarBitmap() {
            if (starBmp == null)
                starBmp = GetBitmap("DataGridRow.star.bmp");
            return starBmp;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetPencilBitmap"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the bitmap displayed in the row header that indicates a row can 
        ///       be edited.</para>
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetPencilBitmap() {
            if (pencilBmp == null)
                pencilBmp = GetBitmap("DataGridRow.pencil.bmp");
            return pencilBmp;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.GetErrorBitmap"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the bitmap displayed on a row with an error.</para>
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetErrorBitmap() {
            if (errorBmp == null)
                errorBmp = GetBitmap("DataGridRow.error.bmp");
            errorBmp.MakeTransparent();
            return errorBmp;
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetLeftArrowBitmap() {
            if (leftArrow == null)
                leftArrow = GetBitmap("DataGridRow.left.bmp");
            return leftArrow;
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected Bitmap GetRightArrowBitmap() {
            if (rightArrow == null)
                rightArrow = GetBitmap("DataGridRow.right.bmp");
            return rightArrow;
        }

        public virtual void InvalidateRow() {
            this.dgTable.DataGrid.InvalidateRow(number);
        }

        public virtual void InvalidateRowRect(Rectangle r) {
            this.dgTable.DataGrid.InvalidateRowRect(number, r);
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnEdit"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, notifies the grid that an edit will 
        ///       occur.</para>
        /// </devdoc>
        public virtual void OnEdit() {
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnKeyPress"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, called by the <see cref='System.Windows.Forms.DataGrid'/> control when a key press occurs on a row with focus.</para>
        /// </devdoc>
        public virtual bool OnKeyPress(Keys keyData) {
            int currentColIndex = this.dgTable.DataGrid.CurrentCell.ColumnNumber;
            GridColumnStylesCollection columns = this.dgTable.GridColumnStyles;
            if (columns != null && currentColIndex >= 0 && currentColIndex < columns.Count) {
                DataGridColumnStyle currentColumn = columns[currentColIndex];
                if (currentColumn.KeyPress(this.RowNumber, keyData)) {
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnMouseDown"]/*' />
        /// <devdoc>
        /// <para> Called by the <see cref='System.Windows.Forms.DataGrid'/> when a click occurs in the row's client area 
        ///    specifed by the x and y coordinates and the specified <see cref='System.Drawing.Rectangle'/>
        ///    .</para>
        /// </devdoc>
        public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders)
        {
            return OnMouseDown(x,y,rowHeaders, false);
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnMouseDown1"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, is called by the <see cref='System.Windows.Forms.DataGrid'/> when a click occurs 
        ///    in the row's
        ///    client area, specified by x and y coordinates.</para>
        /// </devdoc>
        public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight) {
            // if we call base.OnMouseDown, then the row could not use this 
            // mouse click at all. in that case LoseChildFocus, so the edit control 
            // will become visible
            LoseChildFocus(rowHeaders, alignToRight);

            // we did not use this click at all.
            return false;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnMouseMove"]/*' />
        /// <devdoc>
        /// </devdoc>
        public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders) {
            return false;
        }
        
        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnMouseMove1"]/*' />
        /// <devdoc>
        /// <para>When overridden in a derived class, is called by the <see cref='System.Windows.Forms.DataGrid'/> when 
        ///    the mouse moves within the row's client area.</para>
        /// </devdoc>
        public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight) {
            return false;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnMouseLeft"]/*' />
        /// <devdoc>
        /// </devdoc>
        public virtual void OnMouseLeft(Rectangle rowHeaders, bool alignToRight) {
        }

        public virtual void OnMouseLeft() {
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.OnRowEnter"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, causes the RowEnter event to occur.</para>
        /// </devdoc>
        public virtual void OnRowEnter() {}
        public virtual void OnRowLeave() {}

        // processes the Tab Key
        // returns TRUE if the TAB key is processed
        internal abstract bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight);

        // tells the dataGridRow that it lost the focus
        internal abstract void LoseChildFocus(Rectangle rowHeaders, bool alignToRight);

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.Paint"]/*' />
        /// <devdoc>
        ///      Paints the row.
        /// </devdoc>
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

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.PaintBottomBorder"]/*' />
        /// <devdoc>
        ///      Draws a border on the bottom DataGrid.GridLineWidth pixels
        ///      of the bounding rectangle passed in.
        /// </devdoc>
        protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth)
        {
            PaintBottomBorder(g, bounds, dataWidth, this.dgTable.GridLineWidth, false);
        }

        protected virtual void PaintBottomBorder(Graphics g, Rectangle bounds, int dataWidth, int borderWidth, bool alignToRight) {
            // paint bottom border
            Rectangle bottomBorder = new Rectangle(alignToRight ? bounds.Right - dataWidth : bounds.X,
                                                   bounds.Bottom - borderWidth,
                                                   dataWidth,
                                                   borderWidth);

            g.FillRectangle(this.dgTable.IsDefault ? this.DataGrid.GridLineBrush : this.dgTable.GridLineBrush, bottomBorder);

            // paint any exposed region to the right
            if (dataWidth < bounds.Width) {
                g.FillRectangle(this.dgTable.DataGrid.BackgroundBrush,
                                alignToRight ? bounds.X: bottomBorder.Right,
                                bottomBorder.Y,
                                bounds.Width - bottomBorder.Width,
                                borderWidth);
            }
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.PaintData"]/*' />
        /// <devdoc>
        ///      Paints the row.
        /// </devdoc>
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
                                     bool alignToRight) {
            Debug.WriteLineIf(CompModSwitches.DGRowPaint.TraceVerbose, "Painting DataGridAddNewRow: bounds = " + bounds.ToString());

            Rectangle cellBounds   = bounds;
            int       bWidth       = this.dgTable.IsDefault ? this.DataGrid.GridLineWidth : this.dgTable.GridLineWidth;
            int       cx           = 0;

            DataGridCell current = this.dgTable.DataGrid.CurrentCell;

            GridColumnStylesCollection columns = dgTable.GridColumnStyles;
            int nCols = columns.Count;
            for (int col = firstVisibleColumn; col < nCols; ++col) {
                if (cx > bounds.Width)
                    break;

                // if (!columns[col].Visible || columns[col].PropertyDescriptor == null)
                if (columns[col].PropertyDescriptor == null || columns[col].Width <= 0)
                    continue;

                cellBounds.Width = columns[col].Width - bWidth;

                if (alignToRight)
                    cellBounds.X = bounds.Right - cx - cellBounds.Width;
                else
                    cellBounds.X = bounds.X + cx;

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
                if (bWidth > 0) {
                    g.FillRectangle(this.dgTable.IsDefault ? this.DataGrid.GridLineBrush : this.dgTable.GridLineBrush,
                                    alignToRight ? cellBounds.X - bWidth : cellBounds.Right,
                                    cellBounds.Y,
                                    bWidth,
                                    cellBounds.Height);
                }
                cx += cellBounds.Width + bWidth;
            }

            // Paint any exposed area to the right ( or left ) of the data cell area
            if (cx < bounds.Width) {
                g.FillRectangle(this.dgTable.DataGrid.BackgroundBrush,
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
                                                 Brush backBr, Brush foreBrush, bool alignToRight) {
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
        protected Rectangle PaintIcon(Graphics g, Rectangle visualBounds, bool paintIcon, bool alignToRight, Bitmap bmp) {
            return PaintIcon(g, visualBounds, paintIcon, alignToRight, bmp, 
                             this.dgTable.IsDefault ? this.DataGrid.HeaderBackBrush : this.dgTable.HeaderBackBrush);
        }
        protected Rectangle PaintIcon(Graphics g, Rectangle visualBounds, bool paintIcon, bool alignToRight, Bitmap bmp, Brush backBrush) {
            Size bmpSize = bmp.Size;
            Rectangle bmpRect = new Rectangle(alignToRight ? visualBounds.Right - xOffset - bmpSize.Width : visualBounds.X + xOffset,
                                              visualBounds.Y + yOffset,
                                              bmpSize.Width,
                                              bmpSize.Height);
            g.FillRectangle(backBrush, visualBounds);
            if (paintIcon)
            {
                colorMap[0].NewColor = this.dgTable.IsDefault ? this.DataGrid.HeaderForeColor : this.dgTable.HeaderForeColor;
                colorMap[0].OldColor = Color.Black;
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
                g.DrawImage(bmp, bmpRect, 0, 0, bmpRect.Width, bmpRect.Height,GraphicsUnit.Pixel, attr);
                // g.DrawImage(bmp, bmpRect);
                attr.Dispose();
            }

            return bmpRect;
        }

        // assume that the row is not aligned to right, and that the row is not dirty
        public virtual void PaintHeader(Graphics g, Rectangle visualBounds) {
            PaintHeader(g, visualBounds, false);
        }

        // assume that the row is not dirty
        public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight) {
            PaintHeader(g,visualBounds, alignToRight, false);
        }

        public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight, bool rowIsDirty) {
            Rectangle bounds = visualBounds;

            // paint the first part of the row header: the Arror or Pencil/Star
            Bitmap bmp;
            if (this is DataGridAddNewRow)
            {
                bmp = GetStarBitmap();
                lock (bmp) {
                    bounds.X += PaintIcon(g, bounds, true, alignToRight, bmp).Width + xOffset;
                }
                return;
            }
            else if (rowIsDirty)
            {
                bmp = GetPencilBitmap();
                lock (bmp) {
                    bounds.X += PaintIcon(g, bounds, RowNumber == this.DataGrid.CurrentCell.RowNumber, alignToRight, bmp).Width + xOffset;
                }
            }
            else
            {
                bmp = alignToRight ? GetLeftArrowBitmap() : GetRightArrowBitmap();
                lock (bmp) {
                    bounds.X += PaintIcon(g, bounds, RowNumber == this.DataGrid.CurrentCell.RowNumber, alignToRight, bmp).Width + xOffset;
                }
            }

            // Paint the error icon
            //
            object errorInfo = DataGrid.ListManager[this.number];
            if (!(errorInfo is IDataErrorInfo))
                return;

            string errString = ((IDataErrorInfo) errorInfo).Error;
            if (errString == null)
                errString = String.Empty;

            if (tooltip != errString) {
                if (!String.IsNullOrEmpty(tooltip)) {
                    DataGrid.ToolTipProvider.RemoveToolTip(tooltipID);
                    tooltip = String.Empty;
                    tooltipID = new IntPtr(-1);
                }
            }

            if (String.IsNullOrEmpty(errString))
                return;

            // we now have an error string: paint the errorIcon and add the tooltip
            Rectangle errRect;
            bmp = GetErrorBitmap();
            lock (bmp) {
                errRect = PaintIcon(g, bounds, true, alignToRight, bmp);
            }
            bounds.X += errRect.Width + xOffset;

            tooltip = errString;
            tooltipID = (IntPtr)((int)DataGrid.ToolTipId++);
            DataGrid.ToolTipProvider.AddToolTip(tooltip, tooltipID, errRect);
        }

        protected Brush GetBackBrush() {
            Brush br = this.dgTable.IsDefault ? DataGrid.BackBrush : this.dgTable.BackBrush;
            if (DataGrid.LedgerStyle && (RowNumber % 2 == 1)) {
                br = this.dgTable.IsDefault ? this.DataGrid.AlternatingBackBrush : this.dgTable.AlternatingBackBrush;
            }
            return br;
        }

        /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.BackBrushForDataPaint"]/*' />
        /// <devdoc>
        ///      Returns the BackColor and TextColor  that the Graphics object should use
        ///      for the appropriate values for a given row and column when painting the data.
        ///
        /// </devdoc>
        protected Brush BackBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column) {
            Brush backBr = this.GetBackBrush();

            if (Selected) {
                backBr = this.dgTable.IsDefault ? this.DataGrid.SelectionBackBrush : this.dgTable.SelectionBackBrush;
            }            
            /*
            if (RowNumber == current.RowNumber && column == current.ColumnNumber) {
                backBr = grid.CurrentCellBackBrush;
            }
            */
            return backBr;
        }

        protected Brush ForeBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column) {
            // Brush foreBrush = gridColumn.ForeBrush;
            Brush foreBrush = this.dgTable.IsDefault ? this.DataGrid.ForeBrush : this.dgTable.ForeBrush;

            if (Selected) {
                foreBrush = this.dgTable.IsDefault ? this.DataGrid.SelectionForeBrush : this.dgTable.SelectionForeBrush;
            }
            /*
            if (RowNumber == current.RowNumber && column == current.ColumnNumber) {
                foreColor = grid.CurrentCellForeColor;
            }
            */
            return foreBrush;
        }

        [ComVisible(true)]
        protected class DataGridRowAccessibleObject : AccessibleObject {
            ArrayList cells;

            DataGridRow owner = null;

            internal static string CellToDisplayString(DataGrid grid, int row, int column) {
                if (column < grid.myGridTable.GridColumnStyles.Count) {
                    return grid.myGridTable.GridColumnStyles[column].PropertyDescriptor.Converter.ConvertToString(grid[row, column]);
                }
                else {
                    return "";
                }
            }

            internal static object DisplayStringToCell(DataGrid grid, int row, int column, string value) {
                if (column < grid.myGridTable.GridColumnStyles.Count) {
                    return grid.myGridTable.GridColumnStyles[column].PropertyDescriptor.Converter.ConvertFromString(value);
                }
                // ignore...
                //
                return null;
            }

            [
                SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // This class and its derived classes are internal.
                                                                                                        // So this is not a security back door.
            ]
            public DataGridRowAccessibleObject(DataGridRow owner) : base() {
                Debug.Assert(owner != null, "DataGridRowAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
                DataGrid grid = DataGrid;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create row accessible object");

                EnsureChildren();
            }

            private void EnsureChildren() {
                if (cells == null) {
                    // default size... little extra for relationships...
                    //
                    cells = new ArrayList(DataGrid.myGridTable.GridColumnStyles.Count + 2);
                    AddChildAccessibleObjects(cells);
                }
            }

            protected virtual void AddChildAccessibleObjects(IList children) {
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create row's accessible children");
                Debug.Indent();
                GridColumnStylesCollection cols = DataGrid.myGridTable.GridColumnStyles;
                int len = cols.Count;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, len + " columns present");
                for (int i=0; i<len; i++) {
                    children.Add(CreateCellAccessibleObject(i));
                }
                Debug.Unindent();
            }

            protected virtual AccessibleObject CreateCellAccessibleObject(int column) {
                return new DataGridCellAccessibleObject(owner, column);
            }

            public override Rectangle Bounds {
                get {
                    return DataGrid.RectangleToScreen(DataGrid.GetRowBounds(owner));
                }
            }

            public override string Name {
                get {
                    if (owner is DataGridAddNewRow) {
                        return SR.AccDGNewRow;
                    }
                    else {
                        return DataGridRowAccessibleObject.CellToDisplayString(DataGrid, owner.RowNumber, 0);
                    }
                }
            }

            protected DataGridRow Owner {
                get {
                    return owner;
                }
            }

            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return DataGrid.AccessibilityObject;
                }
            }

            private DataGrid DataGrid {
                get {
                    return owner.DataGrid;
                }
            }

            public override AccessibleRole Role {
                get {
                    return AccessibleRole.Row;
                }
            }

            public override AccessibleStates State {
                get {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (DataGrid.CurrentCell.RowNumber == owner.RowNumber) {
                        state |= AccessibleStates.Focused;
                    }

                    // Determine selected
                    //
                    if (DataGrid.CurrentRowIndex == owner.RowNumber) {
                        state |= AccessibleStates.Selected;
                    }

                    return state;
                }
            }

            public override string Value {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return Name;
                }
            }
            
            public override AccessibleObject GetChild(int index) {
                if (index < cells.Count) {
                    return (AccessibleObject)cells[index];
                }

                return null;
            }

            public override int GetChildCount() {
                return cells.Count;
            }

            /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGridRowAccessibleObject.GetFocused"]/*' />
            /// <devdoc>
            ///      Returns the currently focused child, if any.
            ///      Returns this if the object itself is focused.
            /// </devdoc>
            public override AccessibleObject GetFocused() {
                if (DataGrid.Focused) {
                    DataGridCell cell = DataGrid.CurrentCell;
                    if (cell.RowNumber == owner.RowNumber) {
                        return (AccessibleObject)cells[cell.ColumnNumber];
                    }
                }

                return null;
            }


            /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGridRowAccessibleObject.Navigate"]/*' />
            /// <devdoc>
            ///      Navigate to the next or previous grid entry.entry.
            /// </devdoc>
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navdir) {
                switch (navdir) {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber + 1);

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber - 1);

                    case AccessibleNavigation.FirstChild:
                        if (GetChildCount() > 0) {
                            return GetChild(0);
                        }
                        break;
                    case AccessibleNavigation.LastChild:
                        if (GetChildCount() > 0) {
                            return GetChild(GetChildCount() - 1);
                        }
                        break;
                }

                return null;

            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags) {
                // Focus the PropertyGridView window
                //
                if ( (flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus) {
                    DataGrid.Focus();
                }

                // Select the grid entry
                //
                if ( (flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection) {
                    DataGrid.CurrentRowIndex = owner.RowNumber;
                }
            }

        }

        [ComVisible(true)]
        protected class DataGridCellAccessibleObject : AccessibleObject {
            DataGridRow owner = null;
            int column;

            public DataGridCellAccessibleObject(DataGridRow owner, int column) : base() {
                Debug.Assert(owner != null, "DataGridColumnAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
                this.column = column;
                Debug.WriteLineIf(DataGrid.DataGridAcc.TraceVerbose, "Create cell accessible object");
            }

            public override Rectangle Bounds {
                get {
                    return DataGrid.RectangleToScreen(DataGrid.GetCellBounds(new DataGridCell(owner.RowNumber, column)));
                }
            }

            public override string Name {
                get {
                    return DataGrid.myGridTable.GridColumnStyles[column].HeaderText;
                }
            }

            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return owner.AccessibleObject;
                }
            }

            protected DataGrid DataGrid {
                get {
                    return owner.DataGrid;
                }
            }

            public override string DefaultAction {
                get {
                    return SR.AccDGEdit;
                }
            }

            public override AccessibleRole Role {
                get {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State {
                get {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (DataGrid.CurrentCell.RowNumber == owner.RowNumber
                        && DataGrid.CurrentCell.ColumnNumber == column) {
                        if (DataGrid.Focused) {
                            state |= AccessibleStates.Focused;
                        }
                        state |= AccessibleStates.Selected;
                    }

                    return state;
                }
            }

            public override string Value {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    if (owner is DataGridAddNewRow) {
                        return null;
                    }
                    else {
                        return DataGridRowAccessibleObject.CellToDisplayString(DataGrid, owner.RowNumber, column);
                    }
                }

                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                set {
                    if (!(owner is DataGridAddNewRow)) {
                        object realValue =  DataGridRowAccessibleObject.DisplayStringToCell(DataGrid, owner.RowNumber, column, value);
                        DataGrid[owner.RowNumber, column] = realValue;
                    }
                }
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                Select(AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection);
            }

            /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGridCellAccessibleObject.GetFocused"]/*' />
            /// <devdoc>
            ///      Returns the currently focused child, if any.
            ///      Returns this if the object itself is focused.
            /// </devdoc>
            public override AccessibleObject GetFocused() {
                // Datagrid always returns the cell as the focused thing... so do we!
                //
                return DataGrid.AccessibilityObject.GetFocused();
            }


            /// <include file='doc\DataGridRow.uex' path='docs/doc[@for="DataGridRow.DataGridCellAccessibleObject.Navigate"]/*' />
            /// <devdoc>
            ///      Navigate to the next or previous grid entry.
            /// </devdoc>
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navdir) {
                switch (navdir) {
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        if (column < owner.AccessibleObject.GetChildCount() - 1) {
                            return owner.AccessibleObject.GetChild(column + 1);
                        }
                        else {
                            AccessibleObject o = DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber + 1);
                            if (o != null) {
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
                        if (column > 0) {
                            return owner.AccessibleObject.GetChild(column - 1);
                        }
                        else {
                            AccessibleObject o = DataGrid.AccessibilityObject.GetChild(1 + owner.dgTable.GridColumnStyles.Count + owner.RowNumber - 1);
                            if (o != null) {
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

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags) {
                // Focus the PropertyGridView window
                //
                if ( (flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus) {
                    DataGrid.Focus();
                }

                // Select the grid entry
                //
                if ( (flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection) {
                    DataGrid.CurrentCell = new DataGridCell(owner.RowNumber, column);
                }
            }

        }

    }
}
