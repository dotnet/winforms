// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    internal class DataGridParentRows
    {
        // siting
        //
        private readonly DataGrid dataGrid;

        // ui
        //
        // private Color backColor = DataGrid.defaultParentRowsBackColor;
        // private Color foreColor = DataGrid.defaultParentRowsForeColor;

        private SolidBrush backBrush = DataGrid.DefaultParentRowsBackBrush;
        private SolidBrush foreBrush = DataGrid.DefaultParentRowsForeBrush;

        private readonly int borderWidth = 1;
        // private Color borderColor = SystemColors.WindowFrame;
        private Brush borderBrush = new SolidBrush(SystemColors.WindowFrame);

        private static Bitmap rightArrow = null;
        private static Bitmap leftArrow = null;

        private readonly ColorMap[] colorMap = new ColorMap[] { new ColorMap() };

        // private bool gridLineDots = false;
        // private Color gridLineColor = SystemColors.Control;
        // private Brush gridLineBrush = SystemBrushes.Control;
        private readonly Pen gridLinePen = SystemPens.Control;

        private int totalHeight = 0;
        private int textRegionHeight = 0;

        // now that we have left and right arrows, we also have layout
        private readonly Layout layout = new Layout();

        // mouse info
        //
        // private bool overLeftArrow = false;
        // private bool overRightArrow = false;
        private bool downLeftArrow = false;
        private bool downRightArrow = false;

        // a horizOffset of 0 means that the layout for the parent
        // rows is left aligned.
        // a horizOffset of 1 means that the leftmost unit of information ( let it be a
        // table name, a column name or a column value ) is not visible.
        // a horizOffset of 2 means that the leftmost 2 units of information are not visible, and so on
        //
        private int horizOffset = 0;

        // storage for parent row states
        //
        private readonly ArrayList parents = new ArrayList();
        private int parentsCount = 0;
        private readonly ArrayList rowHeights = new ArrayList();
        AccessibleObject accessibleObject;

        internal DataGridParentRows(DataGrid dataGrid)
        {
            colorMap[0].OldColor = Color.Black;
            this.dataGrid = dataGrid;
            // UpdateGridLinePen();
        }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        public AccessibleObject AccessibleObject
        {
            get
            {
                if (accessibleObject == null)
                {
                    accessibleObject = new DataGridParentRowsAccessibleObject(this);
                }
                return accessibleObject;
            }
        }

        internal Color BackColor
        {
            get
            {
                return backBrush.Color;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.DataGridEmptyColor, "Parent Rows BackColor"));
                }

                if (value != backBrush.Color)
                {
                    backBrush = new SolidBrush(value);
                    Invalidate();
                }
            }
        }

        internal SolidBrush BackBrush
        {
            get
            {
                return backBrush;
            }
            set
            {
                if (value != backBrush)
                {
                    CheckNull(value, "BackBrush");
                    backBrush = value;
                    Invalidate();
                }
            }
        }

        internal SolidBrush ForeBrush
        {
            get
            {
                return foreBrush;
            }
            set
            {
                if (value != foreBrush)
                {
                    CheckNull(value, "BackBrush");
                    foreBrush = value;
                    Invalidate();
                }
            }
        }

        // since the layout of the parentRows is computed on every paint message,
        // we can actually return true ClientRectangle coordinates
        internal Rectangle GetBoundsForDataGridStateAccesibility(DataGridState dgs)
        {
            Rectangle ret = Rectangle.Empty;
            int rectY = 0;
            for (int i = 0; i < parentsCount; i++)
            {
                int height = (int)rowHeights[i];
                if (parents[i] == dgs)
                {
                    ret.X = layout.leftArrow.IsEmpty ? layout.data.X : layout.leftArrow.Right;
                    ret.Height = height;
                    ret.Y = rectY;
                    ret.Width = layout.data.Width;
                    return ret;
                }
                rectY += height;
            }
            return ret;
        }

        /*
        internal Color BorderColor {
            get {
                return borderColor;
            }
            set {
                if (value != borderColor) {
                    borderColor = value;
                    Invalidate();
                }
            }
        }
        */

        internal Brush BorderBrush
        {
            get
            {
                return borderBrush;
            }
            set
            {
                if (value != borderBrush)
                {
                    borderBrush = value;
                    Invalidate();
                }
            }
        }

        /*
        internal Brush GridLineBrush {
            get {
                return gridLineBrush;
            }
            set {
                if (value != gridLineBrush) {
                    gridLineBrush = value;
                    UpdateGridLinePen();
                    Invalidate();
                }
            }
        }

        internal bool GridLineDots {
            get {
                return gridLineDots;
            }
            set {
                if (gridLineDots != value) {
                    gridLineDots = value;
                    UpdateGridLinePen();
                    Invalidate();
                }
            }
        }
        */

        internal int Height
        {
            get
            {
                return totalHeight;
            }
        }

        internal Color ForeColor
        {
            get
            {
                return foreBrush.Color;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.DataGridEmptyColor, "Parent Rows ForeColor"));
                }

                if (value != foreBrush.Color)
                {
                    foreBrush = new SolidBrush(value);
                    Invalidate();
                }
            }
        }

        internal bool Visible
        {
            get
            {
                return dataGrid.ParentRowsVisible;
            }
            set
            {
                dataGrid.ParentRowsVisible = value;
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        /// <summary>
        ///  Adds a DataGridState object to the top of the list of parents.
        /// </summary>
        internal void AddParent(DataGridState dgs)
        {
            CurrencyManager childDataSource = (CurrencyManager)dataGrid.BindingContext[dgs.DataSource, dgs.DataMember];
            parents.Add(dgs);
            SetParentCount(parentsCount + 1);
            Debug.Assert(GetTopParent() != null, "we should have a parent at least");
        }

        internal void Clear()
        {
            for (int i = 0; i < parents.Count; i++)
            {
                DataGridState dgs = parents[i] as DataGridState;
                dgs.RemoveChangeNotification();
            }
            parents.Clear();
            rowHeights.Clear();
            totalHeight = 0;
            SetParentCount(0);
        }

        internal void SetParentCount(int count)
        {
            parentsCount = count;
            dataGrid.Caption.BackButtonVisible = (parentsCount > 0) && (dataGrid.AllowNavigation);
        }

        internal void CheckNull(object value, string propName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(propName));
            }
        }

        internal void Dispose()
        {
            gridLinePen.Dispose();
        }

        /// <summary>
        ///  Retrieves the top most parent in the list of parents.
        /// </summary>
        internal DataGridState GetTopParent()
        {
            if (parentsCount < 1)
            {
                return null;
            }
            return (DataGridState)(((ICloneable)(parents[parentsCount - 1])).Clone());
        }

        /// <summary>
        ///  Determines if there are any parent rows contained in this object.
        /// </summary>
        internal bool IsEmpty()
        {
            return parentsCount == 0;
        }

        /// <summary>
        ///  Similar to GetTopParent() but also removes it.
        /// </summary>
        internal DataGridState PopTop()
        {
            if (parentsCount < 1)
            {
                return null;
            }

            SetParentCount(parentsCount - 1);
            DataGridState ret = (DataGridState)parents[parentsCount];
            ret.RemoveChangeNotification();
            parents.RemoveAt(parentsCount);
            return ret;
        }

        internal void Invalidate()
        {
            if (dataGrid != null)
            {
                dataGrid.InvalidateParentRows();
            }
        }

        internal void InvalidateRect(Rectangle rect)
        {
            if (dataGrid != null)
            {
                Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width + borderWidth, rect.Height + borderWidth);
                dataGrid.InvalidateParentRowsRect(r);
            }
        }

        // called from DataGrid::OnLayout
        internal void OnLayout()
        {
            if (parentsCount == rowHeights.Count)
            {
                return;
            }

            int height = 0;
            if (totalHeight == 0)
            {
                totalHeight += 2 * borderWidth;
            }

            // figure out how tall each row's text will be
            //
            textRegionHeight = (int)dataGrid.Font.Height + 2;

            // make the height of the Column.Font count for the height
            // of the parentRows;
            //
            // if the user wants to programatically
            // navigate to a relation in the constructor of the form
            // ( ie, when the form does not process PerformLayout )
            // the grid will receive an OnLayout message when there is more
            // than one parent in the grid
            if (parentsCount > rowHeights.Count)
            {
                Debug.Assert(parentsCount == rowHeights.Count + 1 || rowHeights.Count == 0, "see comment above for more info");
                int rowHeightsCount = rowHeights.Count;
                for (int i = rowHeightsCount; i < parentsCount; i++)
                {
                    DataGridState dgs = (DataGridState)parents[i];
                    GridColumnStylesCollection cols = dgs.GridColumnStyles;

                    int colsHeight = 0;

                    for (int j = 0; j < cols.Count; j++)
                    {
                        colsHeight = Math.Max(colsHeight, cols[j].GetMinimumHeight());
                    }
                    height = Math.Max(colsHeight, textRegionHeight);

                    // the height of the bottom border
                    height++;
                    rowHeights.Add(height);

                    totalHeight += height;
                }
            }
            else
            {
                Debug.Assert(parentsCount == rowHeights.Count - 1, "we do layout only for push/popTop");
                if (parentsCount == 0)
                {
                    totalHeight = 0;
                }
                else
                {
                    totalHeight -= (int)rowHeights[rowHeights.Count - 1];
                }

                rowHeights.RemoveAt(rowHeights.Count - 1);
            }
        }

        private int CellCount()
        {
            int cellCount = 0;
            cellCount = ColsCount();

            if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.TableName ||
                dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
            {
                cellCount++;
            }

            return cellCount;
        }

        private void ResetMouseInfo()
        {
            // overLeftArrow = false;
            // overRightArrow = false;
            downLeftArrow = false;
            downRightArrow = false;
        }

        private void LeftArrowClick(int cellCount)
        {
            if (horizOffset > 0)
            {
                ResetMouseInfo();
                horizOffset -= 1;
                Invalidate();
            }
            else
            {
                ResetMouseInfo();
                InvalidateRect(layout.leftArrow);
            }
        }

        private void RightArrowClick(int cellCount)
        {
            if (horizOffset < cellCount - 1)
            {
                ResetMouseInfo();
                horizOffset += 1;
                Invalidate();
            }
            else
            {
                ResetMouseInfo();
                InvalidateRect(layout.rightArrow);
            }
        }

        // the only mouse clicks that are handled are
        // the mouse clicks on the LeftArrow and RightArrow
        //
        internal void OnMouseDown(int x, int y, bool alignToRight)
        {
            if (layout.rightArrow.IsEmpty)
            {
                Debug.Assert(layout.leftArrow.IsEmpty, "we can't have the leftArrow w/o the rightArrow");
                return;
            }

            int cellCount = CellCount();

            if (layout.rightArrow.Contains(x, y))
            {
                // draw a nice sunken border around the right arrow area
                // we want to keep a cell on the screen

                downRightArrow = true;

                if (alignToRight)
                {
                    LeftArrowClick(cellCount);
                }
                else
                {
                    RightArrowClick(cellCount);
                }
            }
            else if (layout.leftArrow.Contains(x, y))
            {
                downLeftArrow = true;

                if (alignToRight)
                {
                    RightArrowClick(cellCount);
                }
                else
                {
                    LeftArrowClick(cellCount);
                }
            }
            else
            {
                if (downLeftArrow)
                {
                    downLeftArrow = false;
                    InvalidateRect(layout.leftArrow);
                }
                if (downRightArrow)
                {
                    downRightArrow = false;
                    InvalidateRect(layout.rightArrow);
                }
            }
        }

        internal void OnMouseLeave()
        {
            if (downLeftArrow)
            {
                downLeftArrow = false;
                InvalidateRect(layout.leftArrow);
            }
            if (downRightArrow)
            {
                downRightArrow = false;
                InvalidateRect(layout.rightArrow);
            }
        }

        internal void OnMouseMove(int x, int y)
        {
            /*
            if (!layout.leftArrow.IsEmpty && layout.leftArrow.Contains(x,y))
            {
                ResetMouseInfo();
                overLeftArrow = true;
                InvalidateRect(layout.leftArrow);
                return;
            }
            if (!layout.rightArrow.IsEmpty && layout.rightArrow.Contains(x,y))
            {
                ResetMouseInfo();
                overRightArrow = true;
                InvalidateRect(layout.rightArrow);
                return;
            }
            */

            if (downLeftArrow)
            {
                downLeftArrow = false;
                InvalidateRect(layout.leftArrow);
            }
            if (downRightArrow)
            {
                downRightArrow = false;
                InvalidateRect(layout.rightArrow);
            }
        }

        internal void OnMouseUp(int x, int y)
        {
            ResetMouseInfo();
            if (!layout.rightArrow.IsEmpty && layout.rightArrow.Contains(x, y))
            {
                InvalidateRect(layout.rightArrow);
                return;
            }
            if (!layout.leftArrow.IsEmpty && layout.leftArrow.Contains(x, y))
            {
                InvalidateRect(layout.leftArrow);
                return;
            }
        }

        internal void OnResize(Rectangle oldBounds)
        {
            Invalidate();
        }

        /// <summary>
        ///  Paints the parent rows
        /// </summary>
        internal void Paint(Graphics g, Rectangle visualbounds, bool alignRight)
        {
            Rectangle bounds = visualbounds;
            // Paint the border around our bounds
            if (borderWidth > 0)
            {
                PaintBorder(g, bounds);
                bounds.Inflate(-borderWidth, -borderWidth);
            }

            PaintParentRows(g, bounds, alignRight);
        }

        private void PaintBorder(Graphics g, Rectangle bounds)
        {
            Rectangle border = bounds;

            // top
            border.Height = borderWidth;
            g.FillRectangle(borderBrush, border);

            // bottom
            border.Y = bounds.Bottom - borderWidth;
            g.FillRectangle(borderBrush, border);

            // left
            border = new Rectangle(bounds.X, bounds.Y + borderWidth,
                                   borderWidth, bounds.Height - 2 * borderWidth);
            g.FillRectangle(borderBrush, border);

            // right
            border.X = bounds.Right - borderWidth;
            g.FillRectangle(borderBrush, border);
        }

        // will return the width of the text box that will fit all the
        // tables names
        private int GetTableBoxWidth(Graphics g, Font font)
        {
            // try to make the font BOLD
            Font textFont = font;
            try
            {
                textFont = new Font(font, FontStyle.Bold);
            }
            catch
            {
            }
            int width = 0;
            for (int row = 0; row < parentsCount; row++)
            {
                DataGridState dgs = (DataGridState)parents[row];
                // Graphics.MeasureString(...) returns different results for ": " than for " :"
                //
                string displayTableName = dgs.ListManager.GetListName() + " :";
                int size = (int)g.MeasureString(displayTableName, textFont).Width;
                width = Math.Max(size, width);
            }

            return width;
        }

        // will return the width of the text box that will
        // fit all the column names
        private int GetColBoxWidth(Graphics g, Font font, int colNum)
        {
            int width = 0;

            for (int row = 0; row < parentsCount; row++)
            {
                DataGridState dgs = (DataGridState)parents[row];
                GridColumnStylesCollection columns = dgs.GridColumnStyles;
                if (colNum < columns.Count)
                {
                    // Graphics.MeasureString(...) returns different results for ": " than for " :"
                    //
                    string colName = columns[colNum].HeaderText + " :";
                    int size = (int)g.MeasureString(colName, font).Width;
                    width = Math.Max(size, width);
                }
            }

            return width;
        }

        // will return the width of the best fit for the column
        //
        private int GetColDataBoxWidth(Graphics g, int colNum)
        {
            int width = 0;
            for (int row = 0; row < parentsCount; row++)
            {
                DataGridState dgs = (DataGridState)parents[row];
                GridColumnStylesCollection columns = dgs.GridColumnStyles;
                if (colNum < columns.Count)
                {
                    object value = columns[colNum].GetColumnValueAtRow((CurrencyManager)dataGrid.BindingContext[dgs.DataSource, dgs.DataMember],
                                                                        dgs.LinkingRow.RowNumber);
                    int size = columns[colNum].GetPreferredSize(g, value).Width;
                    width = Math.Max(size, width);
                }
            }
            return width;
        }

        // will return the count of the table with the largest number of columns
        private int ColsCount()
        {
            int colNum = 0;
            for (int row = 0; row < parentsCount; row++)
            {
                DataGridState dgs = (DataGridState)parents[row];
                colNum = Math.Max(colNum, dgs.GridColumnStyles.Count);
            }
            return colNum;
        }

        // will return the total width required to paint the parentRows
        private int TotalWidth(int tableNameBoxWidth, int[] colsNameWidths, int[] colsDataWidths)
        {
            int totalWidth = 0;
            totalWidth += tableNameBoxWidth;
            Debug.Assert(colsNameWidths.Length == colsDataWidths.Length, "both arrays are as long as the largest column count in dgs");
            for (int i = 0; i < colsNameWidths.Length; i++)
            {
                totalWidth += colsNameWidths[i];
                totalWidth += colsDataWidths[i];
            }

            // let 3 pixels in between datacolumns
            // see DonnaWa
            totalWidth += 3 * (colsNameWidths.Length - 1);
            return totalWidth;
        }

        // computes the layout for the parent rows
        //
        private void ComputeLayout(Rectangle bounds, int tableNameBoxWidth, int[] colsNameWidths, int[] colsDataWidths)
        {
            int totalWidth = TotalWidth(tableNameBoxWidth, colsNameWidths, colsDataWidths);
            if (totalWidth > bounds.Width)
            {
                layout.leftArrow = new Rectangle(bounds.X, bounds.Y, 15, bounds.Height);
                layout.data = new Rectangle(layout.leftArrow.Right, bounds.Y, bounds.Width - 30, bounds.Height);
                layout.rightArrow = new Rectangle(layout.data.Right, bounds.Y, 15, bounds.Height);
            }
            else
            {
                layout.data = bounds;
                layout.leftArrow = Rectangle.Empty;
                layout.rightArrow = Rectangle.Empty;
            }
        }

        private void PaintParentRows(Graphics g, Rectangle bounds, bool alignToRight)
        {
            // variables needed for aligning the table and column names
            int tableNameBoxWidth = 0;
            int numCols = ColsCount();
            int[] colsNameWidths = new int[numCols];
            int[] colsDataWidths = new int[numCols];

            // compute the size of the box that will contain the tableName
            //
            if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.TableName ||
                dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
            {
                tableNameBoxWidth = GetTableBoxWidth(g, dataGrid.Font);
            }

            // initialiaze the arrays that contain the column names and the column size
            //
            for (int i = 0; i < numCols; i++)
            {
                if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.ColumnName ||
                    dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
                {
                    colsNameWidths[i] = GetColBoxWidth(g, dataGrid.Font, i);
                }
                else
                {
                    colsNameWidths[i] = 0;
                }
                colsDataWidths[i] = GetColDataBoxWidth(g, i);
            }

            // compute the layout
            //
            ComputeLayout(bounds, tableNameBoxWidth, colsNameWidths, colsDataWidths);

            // paint the navigation arrows, if necessary
            //
            if (!layout.leftArrow.IsEmpty)
            {
                g.FillRectangle(BackBrush, layout.leftArrow);
                PaintLeftArrow(g, layout.leftArrow, alignToRight);
            }

            // paint the parent rows:
            //
            Rectangle rowBounds = layout.data;
            for (int row = 0; row < parentsCount; ++row)
            {
                rowBounds.Height = (int)rowHeights[row];
                if (rowBounds.Y > bounds.Bottom)
                {
                    break;
                }

                int paintedWidth = PaintRow(g, rowBounds, row, dataGrid.Font, alignToRight, tableNameBoxWidth, colsNameWidths, colsDataWidths);
                if (row == parentsCount - 1)
                {
                    break;
                }

                // draw the grid line below
                g.DrawLine(gridLinePen, rowBounds.X, rowBounds.Bottom,
                           rowBounds.X + paintedWidth,
                           rowBounds.Bottom);
                rowBounds.Y += rowBounds.Height;
            }

            if (!layout.rightArrow.IsEmpty)
            {
                g.FillRectangle(BackBrush, layout.rightArrow);
                PaintRightArrow(g, layout.rightArrow, alignToRight);
            }
        }

        private Bitmap GetBitmap(string bitmapName)
        {
            try
            {
                return DpiHelper.GetBitmapFromIcon(typeof(DataGridParentRows), bitmapName);
            }
            catch (Exception e)
            {
                Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
                return null;
            }
        }

        private Bitmap GetRightArrowBitmap()
        {
            if (rightArrow == null)
            {
                rightArrow = GetBitmap("DataGridParentRows.RightArrow");
            }

            return rightArrow;
        }

        private Bitmap GetLeftArrowBitmap()
        {
            if (leftArrow == null)
            {
                leftArrow = GetBitmap("DataGridParentRows.LeftArrow");
            }

            return leftArrow;
        }

        private void PaintBitmap(Graphics g, Bitmap b, Rectangle bounds)
        {
            // center the bitmap in the bounds:
            int bmpX = bounds.X + (bounds.Width - b.Width) / 2;
            int bmpY = bounds.Y + (bounds.Height - b.Height) / 2;
            Rectangle bmpRect = new Rectangle(bmpX, bmpY, b.Width, b.Height);

            g.FillRectangle(BackBrush, bmpRect);

            // now draw the bitmap
            ImageAttributes attr = new ImageAttributes();
            colorMap[0].NewColor = ForeColor;
            attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
            g.DrawImage(b, bmpRect, 0, 0, bmpRect.Width, bmpRect.Height, GraphicsUnit.Pixel, attr);
            attr.Dispose();
        }

        /*
        private void PaintOverButton(Graphics g, Rectangle bounds)
        {
        }
        */

        private void PaintDownButton(Graphics g, Rectangle bounds)
        {
            g.DrawLine(Pens.Black, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);  // the top
            g.DrawLine(Pens.White, bounds.X + bounds.Width, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);  // the right side
            g.DrawLine(Pens.White, bounds.X + bounds.Width, bounds.Y + bounds.Height, bounds.X, bounds.Y + bounds.Height);  // the right side
            g.DrawLine(Pens.Black, bounds.X, bounds.Y + bounds.Height, bounds.X, bounds.Y);  // the left side
        }

        private void PaintLeftArrow(Graphics g, Rectangle bounds, bool alignToRight)
        {
            Bitmap bmp = GetLeftArrowBitmap();
            // paint the border around this bitmap if this is the case
            //
            /*
            if (overLeftArrow)
            {
                Debug.Assert(!downLeftArrow, "can both of those happen?");
                PaintOverButton(g, bounds);
                layout.leftArrow.Inflate(-1,-1);
            }
            */
            if (downLeftArrow)
            {
                PaintDownButton(g, bounds);
                layout.leftArrow.Inflate(-1, -1);
                lock (bmp)
                {
                    PaintBitmap(g, bmp, bounds);
                }
                layout.leftArrow.Inflate(1, 1);
            }
            else
            {
                lock (bmp)
                {
                    PaintBitmap(g, bmp, bounds);
                }
            }
        }

        private void PaintRightArrow(Graphics g, Rectangle bounds, bool alignToRight)
        {
            Bitmap bmp = GetRightArrowBitmap();
            // paint the border around this bitmap if this is the case
            //
            /*
            if (overRightArrow)
            {
                Debug.Assert(!downRightArrow, "can both of those happen?");
                PaintOverButton(g, bounds);
                layout.rightArrow.Inflate(-1,-1);
            }
            */
            if (downRightArrow)
            {
                PaintDownButton(g, bounds);
                layout.rightArrow.Inflate(-1, -1);
                lock (bmp)
                {
                    PaintBitmap(g, bmp, bounds);
                }
                layout.rightArrow.Inflate(1, 1);
            }
            else
            {
                lock (bmp)
                {
                    PaintBitmap(g, bmp, bounds);
                }
            }
        }

        private int PaintRow(Graphics g, Rectangle bounds, int row, Font font, bool alignToRight,
                             int tableNameBoxWidth, int[] colsNameWidths, int[] colsDataWidths)
        {
            DataGridState dgs = (DataGridState)parents[row];
            Rectangle paintBounds = bounds;
            Rectangle rowBounds = bounds;
            paintBounds.Height = (int)rowHeights[row];
            rowBounds.Height = (int)rowHeights[row];

            int paintedWidth = 0;
            // used for scrolling: when paiting, we will skip horizOffset cells in the dataGrid ParentRows
            int skippedCells = 0;

            // paint the table name
            if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.TableName ||
                 dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
            {
                if (skippedCells < horizOffset)
                {
                    // skip this
                    skippedCells++;
                }
                else
                {
                    paintBounds.Width = Math.Min(paintBounds.Width, tableNameBoxWidth);
                    paintBounds.X = MirrorRect(bounds, paintBounds, alignToRight);
                    string displayTableName = dgs.ListManager.GetListName() + ": ";
                    PaintText(g, paintBounds, displayTableName, font, true, alignToRight);      // true is for painting bold
                    paintedWidth += paintBounds.Width;
                }
            }

            if (paintedWidth >= bounds.Width)
            {
                return bounds.Width;        // we painted everything
            }

            rowBounds.Width -= paintedWidth;
            rowBounds.X += alignToRight ? 0 : paintedWidth;
            paintedWidth += PaintColumns(g, rowBounds, dgs, font, alignToRight, colsNameWidths, colsDataWidths, skippedCells);

            // paint the possible space left after columns
            if (paintedWidth < bounds.Width)
            {
                paintBounds.X = bounds.X + paintedWidth;
                paintBounds.Width = bounds.Width - paintedWidth;
                paintBounds.X = MirrorRect(bounds, paintBounds, alignToRight);
                g.FillRectangle(BackBrush, paintBounds);
            }
            return paintedWidth;
        }

        private int PaintColumns(Graphics g, Rectangle bounds, DataGridState dgs, Font font, bool alignToRight,
                                 int[] colsNameWidths, int[] colsDataWidths, int skippedCells)
        {
            Rectangle paintBounds = bounds;
            Rectangle rowBounds = bounds;
            GridColumnStylesCollection cols = dgs.GridColumnStyles;
            int cx = 0;

            for (int i = 0; i < cols.Count; i++)
            {
                if (cx >= bounds.Width)
                {
                    break;
                }

                // paint the column name, if we have to
                if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.ColumnName ||
                    dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
                {
                    if (skippedCells < horizOffset)
                    {
                        // skip this column
                    }
                    else
                    {
                        paintBounds.X = bounds.X + cx;
                        paintBounds.Width = Math.Min(bounds.Width - cx, colsNameWidths[i]);
                        paintBounds.X = MirrorRect(bounds, paintBounds, alignToRight);

                        string colName = cols[i].HeaderText + ": ";
                        PaintText(g, paintBounds, colName, font, false, alignToRight);      // false is for not painting bold

                        cx += paintBounds.Width;
                    }
                }

                if (cx >= bounds.Width)
                {
                    break;
                }

                if (skippedCells < horizOffset)
                {
                    // skip this cell
                    skippedCells++;
                }
                else
                {
                    // paint the cell contents
                    paintBounds.X = bounds.X + cx;
                    paintBounds.Width = Math.Min(bounds.Width - cx, colsDataWidths[i]);
                    paintBounds.X = MirrorRect(bounds, paintBounds, alignToRight);

                    // when we paint the data grid parent rows, we want to paint the data at the position
                    // stored in the currency manager.
                    cols[i].Paint(g, paintBounds, (CurrencyManager)dataGrid.BindingContext[dgs.DataSource, dgs.DataMember],
                                    dataGrid.BindingContext[dgs.DataSource, dgs.DataMember].Position, BackBrush, ForeBrush, alignToRight);

                    cx += paintBounds.Width;

                    // draw the line to the right (or left, according to alignRight)
                    //
                    g.DrawLine(new Pen(SystemColors.ControlDark),
                               alignToRight ? paintBounds.X : paintBounds.Right,
                               paintBounds.Y,
                               alignToRight ? paintBounds.X : paintBounds.Right,
                               paintBounds.Bottom);

                    // this is how wide the line is....
                    cx++;

                    // put 3 pixels in between columns
                    // see DonnaWa
                    //
                    if (i < cols.Count - 1)
                    {
                        paintBounds.X = bounds.X + cx;
                        paintBounds.Width = Math.Min(bounds.Width - cx, 3);
                        paintBounds.X = MirrorRect(bounds, paintBounds, alignToRight);

                        g.FillRectangle(BackBrush, paintBounds);
                        cx += 3;
                    }
                }
            }

            return cx;
        }

        /// <summary>
        ///  Draws on the screen the text. It is used only to paint the Table Name and the column Names
        ///  Returns the width of bounding rectangle that was passed in
        /// </summary>
        private int PaintText(Graphics g, Rectangle textBounds, string text, Font font, bool bold, bool alignToRight)
        {
            Font textFont = font;
            if (bold)
            {
                try
                {
                    textFont = new Font(font, FontStyle.Bold);
                }
                catch { }
            }
            else
            {
                textFont = font;
            }

            // right now, we paint the entire box, cause it will be used anyway
            g.FillRectangle(BackBrush, textBounds);
            StringFormat format = new StringFormat();
            if (alignToRight)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                format.Alignment = StringAlignment.Far;
            }
            format.FormatFlags |= StringFormatFlags.NoWrap;
            // part 1, section 3: put the table and the column name in the
            // parent rows at the same height as the dataGridTextBoxColumn draws the string
            //
            textBounds.Offset(0, 2);
            textBounds.Height -= 2;
            g.DrawString(text, textFont, ForeBrush, textBounds, format);
            format.Dispose();
            return textBounds.Width;

        }

        // will return the X coordinate of the containedRect mirrored within the surroundingRect
        // according to the value of alignToRight
        private int MirrorRect(Rectangle surroundingRect, Rectangle containedRect, bool alignToRight)
        {
            Debug.Assert(containedRect.X >= surroundingRect.X && containedRect.Right <= surroundingRect.Right, "containedRect is not contained in surroundingRect");
            if (alignToRight)
            {
                return surroundingRect.Right - containedRect.Right + surroundingRect.X;
            }
            else
            {
                return containedRect.X;
            }
        }

        private class Layout
        {
            public Rectangle data;
            public Rectangle leftArrow;
            public Rectangle rightArrow;

            public Layout()
            {
                data = Rectangle.Empty;
                leftArrow = Rectangle.Empty;
                rightArrow = Rectangle.Empty;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(200);
                sb.Append("ParentRows Layout: \n");
                sb.Append("data = ");
                sb.Append(data.ToString());
                sb.Append("\n leftArrow = ");
                sb.Append(leftArrow.ToString());
                sb.Append("\n rightArrow = ");
                sb.Append(rightArrow.ToString());
                sb.Append("\n");

                return sb.ToString();
            }
        }

        [ComVisible(true)]
        protected internal class DataGridParentRowsAccessibleObject : AccessibleObject
        {
            readonly DataGridParentRows owner = null;

            public DataGridParentRowsAccessibleObject(DataGridParentRows owner) : base()
            {
                Debug.Assert(owner != null, "DataGridParentRowsAccessibleObject must have a valid owner");
                this.owner = owner;
            }

            internal DataGridParentRows Owner
            {
                get
                {
                    return owner;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    return owner.dataGrid.RectangleToScreen(owner.dataGrid.ParentRowsBounds);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.AccDGNavigateBack;
                }
            }

            public override string Name
            {
                get
                {
                    return SR.AccDGParentRows;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return owner.dataGrid.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.List;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.ReadOnly;

                    if (owner.parentsCount == 0)
                    {
                        state |= AccessibleStates.Invisible;
                    }
                    if (owner.dataGrid.ParentRowsVisible)
                    {
                        state |= AccessibleStates.Expanded;
                    }
                    else
                    {
                        state |= AccessibleStates.Collapsed;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    return null;
                }
            }

            public override void DoDefaultAction()
            {
                owner.dataGrid.NavigateBack();
            }

            public override AccessibleObject GetChild(int index)
            {
                return ((DataGridState)owner.parents[index]).ParentRowAccessibleObject;
            }

            public override int GetChildCount()
            {
                return owner.parentsCount;
            }

            /// <summary>
            ///  Returns the currently focused child, if any.
            ///  Returns this if the object itself is focused.
            /// </summary>
            public override AccessibleObject GetFocused()
            {
                return null;
            }

            internal AccessibleObject GetNext(AccessibleObject child)
            {
                int children = GetChildCount();
                bool hit = false;

                for (int i = 0; i < children; i++)
                {
                    if (hit)
                    {
                        return GetChild(i);
                    }
                    if (GetChild(i) == child)
                    {
                        hit = true;
                    }
                }

                return null;
            }

            internal AccessibleObject GetPrev(AccessibleObject child)
            {
                int children = GetChildCount();
                bool hit = false;

                for (int i = children - 1; i >= 0; i--)
                {
                    if (hit)
                    {
                        return GetChild(i);
                    }
                    if (GetChild(i) == child)
                    {
                        hit = true;
                    }
                }

                return null;
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
                    case AccessibleNavigation.Down:
                        return Parent.GetChild(1);
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return Parent.GetChild(GetChildCount() - 1);
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
            }
        }
    }
}

