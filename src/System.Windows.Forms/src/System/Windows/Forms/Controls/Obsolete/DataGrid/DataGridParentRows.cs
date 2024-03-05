// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace System.Windows.Forms;

#nullable disable
[Obsolete("DataGridParentRows has been deprecated.")]
internal class DataGridParentRows
{
    // siting
    private DataGrid dataGrid;

    private SolidBrush backBrush = DataGrid.DefaultParentRowsBackBrush;
    private SolidBrush foreBrush = DataGrid.DefaultParentRowsForeBrush;

    private int borderWidth = 1;
    // private Color borderColor = SystemColors.WindowFrame;
    private Brush borderBrush = new SolidBrush(SystemColors.WindowFrame);

    private static Bitmap rightArrow;
    private static Bitmap leftArrow;

    private ColorMap[] colorMap = [new ColorMap()];

    private Pen gridLinePen = SystemPens.Control;

    private int totalHeight;
    private int textRegionHeight;

    // now that we have left and right arrows, we also have layout
    private Layout layout = new Layout();
    private bool downLeftArrow;
    private bool downRightArrow;

    private int horizOffset;

    // storage for parent row states
    //
    private ArrayList parents = new ArrayList();
    private int parentsCount;
    private ArrayList rowHeights = new ArrayList();

    internal DataGridParentRows(DataGrid dataGrid)
    {
        colorMap[0].OldColor = Color.Black;
        this.dataGrid = dataGrid;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject AccessibleObject
    {
        get
        {
            throw new PlatformNotSupportedException();
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
                throw new ArgumentException("SR.GetString(SR.DataGridEmptyColor)");
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
                throw new ArgumentException("SR.GetString(SR.DataGridEmptyColor)");
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

    internal void AddParent(DataGridState dgs)
    {
        CurrencyManager childDataSource = (CurrencyManager)dataGrid.BindingContext[dgs.DataSource, dgs.DataMember];
        parents.Add(dgs);
        SetParentCount(parentsCount + 1);
        Debug.Assert(GetTopParent() is not null, "we should have a parent at least");
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
        if (value is null)
            throw new ArgumentNullException();
    }

    internal void Dispose()
    {
        gridLinePen.Dispose();
    }

    internal DataGridState GetTopParent()
    {
        if (parentsCount < 1)
        {
            return null;
        }

        return (DataGridState)(((ICloneable)(parents[parentsCount - 1])).Clone());
    }

    internal bool IsEmpty()
    {
        return parentsCount == 0;
    }

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
        if (dataGrid is not null)
            dataGrid.InvalidateParentRows();
    }

    internal void InvalidateRect(Rectangle rect)
    {
        if (dataGrid is not null)
        {
            Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width + borderWidth, rect.Height + borderWidth);
            dataGrid.InvalidateParentRowsRect(r);
        }
    }

    internal void OnLayout()
    {
        if (parentsCount == rowHeights.Count)
            return;

        int height = 0;
        if (totalHeight == 0)
        {
            totalHeight += 2 * borderWidth;
        }

        textRegionHeight = (int)dataGrid.Font.Height + 2;

        if (parentsCount > rowHeights.Count)
        {
            Debug.Assert(parentsCount == rowHeights.Count + 1 || rowHeights.Count == 0, "see bug 82808 for more info, or the comment above");
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
                totalHeight = 0;
            else
                totalHeight -= (int)rowHeights[rowHeights.Count - 1];
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
            downRightArrow = true;

            if (alignToRight)
                LeftArrowClick(cellCount);
            else
                RightArrowClick(cellCount);
        }
        else if (layout.leftArrow.Contains(x, y))
        {
            downLeftArrow = true;

            if (alignToRight)
                RightArrowClick(cellCount);
            else
                LeftArrowClick(cellCount);
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
                string colName = columns[colNum].HeaderText + " :";
                int size = (int)g.MeasureString(colName, font).Width;
                width = Math.Max(size, width);
            }
        }

        return width;
    }

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

        totalWidth += 3 * (colsNameWidths.Length - 1);
        return totalWidth;
    }

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
        int tableNameBoxWidth = 0;
        int numCols = ColsCount();
        int[] colsNameWidths = new int[numCols];
        int[] colsDataWidths = new int[numCols];

        if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.TableName ||
            dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
        {
            tableNameBoxWidth = GetTableBoxWidth(g, dataGrid.Font);
        }

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

        ComputeLayout(bounds, tableNameBoxWidth, colsNameWidths, colsDataWidths);

        if (!layout.leftArrow.IsEmpty)
        {
            g.FillRectangle(BackBrush, layout.leftArrow);
            PaintLeftArrow(g, layout.leftArrow, alignToRight);
        }

        Rectangle rowBounds = layout.data;
        for (int row = 0; row < parentsCount; ++row)
        {
            rowBounds.Height = (int)rowHeights[row];
            if (rowBounds.Y > bounds.Bottom)
                break;
            int paintedWidth = PaintRow(g, rowBounds, row, dataGrid.Font, alignToRight, tableNameBoxWidth, colsNameWidths, colsDataWidths);
            if (row == parentsCount - 1)
                break;

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

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    private Bitmap GetBitmap(string bitmapName, Color transparentColor)
    {
        Bitmap b = null;
        try
        {
            b = new Bitmap(typeof(DataGridParentRows), bitmapName);
            b.MakeTransparent(transparentColor);
        }
        catch (Exception e)
        {
            Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
        }

        return b;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    private Bitmap GetRightArrowBitmap()
    {
        if (rightArrow is null)
            rightArrow = GetBitmap("DataGridParentRows.RightArrow.bmp", Color.White);
        return rightArrow;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    private Bitmap GetLeftArrowBitmap()
    {
        if (leftArrow is null)
            leftArrow = GetBitmap("DataGridParentRows.LeftArrow.bmp", Color.White);
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
            return bounds.Width;        // we painted everything

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
                break;

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
                break;

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

    private int PaintText(Graphics g, Rectangle textBounds, string text, Font font, bool bold, bool alignToRight)
    {
        Font textFont = font;
        if (bold)
            try
            {
                textFont = new Font(font, FontStyle.Bold);
            }
            catch { }
        else
            textFont = font;

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
        textBounds.Offset(0, 2);
        textBounds.Height -= 2;
        g.DrawString(text, textFont, ForeBrush, textBounds, format);
        format.Dispose();
        return textBounds.Width;
    }

    private int MirrorRect(Rectangle surroundingRect, Rectangle containedRect, bool alignToRight)
    {
        Debug.Assert(containedRect.X >= surroundingRect.X && containedRect.Right <= surroundingRect.Right, "containedRect is not contained in surroundingRect");
        if (alignToRight)
            return surroundingRect.Right - containedRect.Right + surroundingRect.X;
        else
            return containedRect.X;
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
            sb.Append('\n');

            return sb.ToString();
        }
    }

    [ComVisible(true)]
    [Obsolete("DataGridParentRowsAccessibleObject has been deprecated.")]
    protected internal class DataGridParentRowsAccessibleObject : AccessibleObject
    {
        public DataGridParentRowsAccessibleObject(DataGridParentRows owner) : base()
        {
            throw new PlatformNotSupportedException();
        }

        internal DataGridParentRows Owner
        {
            get;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultAction
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleStates State
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override void DoDefaultAction()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetChild(int index)
        {
            throw new PlatformNotSupportedException();
        }

        public override int GetChildCount()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetFocused()
        {
            throw new PlatformNotSupportedException();
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

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }

        public override void Select(AccessibleSelection flags)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
