// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms;

#nullable disable
[Obsolete("DataGridRow has been deprecated.")]
internal abstract class DataGridRow : MarshalByRefObject
{
    internal protected int number;
    protected DataGridTableStyle dgTable;

    // we will be mapping only the black color to
    // the HeaderForeColor
    private static ColorMap[] colorMap = [new ColorMap()];

    // bitmaps
    private static Bitmap rightArrow;
    private static Bitmap leftArrow;
    private static Bitmap errorBmp;
    private static Bitmap pencilBmp;
    private static Bitmap starBmp;
    protected const int xOffset = 3;
    protected const int yOffset = 2;

    public DataGridRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject AccessibleObject
    {
        get
        {
            throw new PlatformNotSupportedException();
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
            if (dgTable.DataGrid.DataSource is not null)
            {
                int nCols = columns.Count;
                for (int i = 0; i < nCols; ++i)
                {
                    // if (columns[i].Visible && columns[i].PropertyDescriptor is not null)
                    if (columns[i].PropertyDescriptor is not null)
                        h = Math.Max(h, columns[i].GetMinimumHeight());
                }
            }
        }
        catch
        {
        }

        return h;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGrid DataGrid
    {
        get
        {
            throw new PlatformNotSupportedException();
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual int Height
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int RowNumber
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool Selected
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetBitmap(string bitmapName)
    {
        Bitmap b = null;
        try
        {
            b = new Bitmap(typeof(DataGridCaption), bitmapName);
            b.MakeTransparent();
        }
        catch (Exception e)
        {
            Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
            throw;
        }

        return b;
    }

    public virtual Rectangle GetCellBounds(int col)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual Rectangle GetNonScrollableArea()
    {
        throw new PlatformNotSupportedException();
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetStarBitmap()
    {
        if (starBmp is null)
            starBmp = GetBitmap("DataGridRow.star.bmp");
        return starBmp;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetPencilBitmap()
    {
        if (pencilBmp is null)
            pencilBmp = GetBitmap("DataGridRow.pencil.bmp");
        return pencilBmp;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetErrorBitmap()
    {
        if (errorBmp is null)
            errorBmp = GetBitmap("DataGridRow.error.bmp");
        errorBmp.MakeTransparent();
        return errorBmp;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetLeftArrowBitmap()
    {
        if (leftArrow is null)
            leftArrow = GetBitmap("DataGridRow.left.bmp");
        return leftArrow;
    }

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine)]
    protected Bitmap GetRightArrowBitmap()
    {
        if (rightArrow is null)
            rightArrow = GetBitmap("DataGridRow.right.bmp");
        return rightArrow;
    }

    public virtual void InvalidateRow()
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void InvalidateRowRect(Rectangle r)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void OnEdit()
    {
        throw new PlatformNotSupportedException();
    }

    public virtual bool OnKeyPress(Keys keyData)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void OnMouseLeft()
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void OnRowEnter()
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void OnRowLeave()
    {
        throw new PlatformNotSupportedException();
    }

    internal abstract bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight);

    internal abstract void LoseChildFocus(Rectangle rowHeaders, bool alignToRight);

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public abstract int Paint(Graphics g,
                             Rectangle dataBounds,
                             Rectangle rowBounds,
                             int firstVisibleColumn,
                             int numVisibleColumns);

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public abstract int Paint(Graphics g,
                              Rectangle dataBounds,
                              Rectangle rowBounds,
                              int firstVisibleColumn,
                              int numVisibleColumns,
                              bool alignToRight);

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

    public virtual int PaintData(Graphics g,
                                 Rectangle bounds,
                                 int firstVisibleColumn,
                                 int columnCount)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual int PaintData(Graphics g,
                                 Rectangle bounds,
                                 int firstVisibleColumn,
                                 int columnCount,
                                 bool alignToRight)
    {
        throw new PlatformNotSupportedException();
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
            attr.Dispose();
        }

        return bmpRect;
    }

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public virtual void PaintHeader(Graphics g, Rectangle visualBounds, bool alignToRight, bool rowIsDirty)
    {
        throw new PlatformNotSupportedException();
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

    protected Brush BackBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
    {
        Brush backBr = GetBackBrush();

        if (Selected)
        {
            backBr = dgTable.IsDefault ? DataGrid.SelectionBackBrush : dgTable.SelectionBackBrush;
        }

        return backBr;
    }

    protected Brush ForeBrushForDataPaint(ref DataGridCell current, DataGridColumnStyle gridColumn, int column)
    {
        Brush foreBrush = dgTable.IsDefault ? DataGrid.ForeBrush : dgTable.ForeBrush;

        if (Selected)
        {
            foreBrush = dgTable.IsDefault ? DataGrid.SelectionForeBrush : dgTable.SelectionForeBrush;
        }

        return foreBrush;
    }

    [ComVisible(true)]
    [Obsolete("DataGridRowAccessibleObject has been deprecated.")]
    protected class DataGridRowAccessibleObject : AccessibleObject
    {
        private ArrayList cells;

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

            return null;
        }

        public DataGridRowAccessibleObject(DataGridRow owner) : base()
        {
            throw new PlatformNotSupportedException();
        }

        private void EnsureChildren()
        {
            if (cells is null)
            {
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
            return new AccessibleObject();
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
        public override string Name
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        protected DataGridRow Owner
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

        private DataGrid DataGrid
        {
            get;
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

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }

        public override void Select(AccessibleSelection flags)
        {
            throw new PlatformNotSupportedException();
        }
    }

    [ComVisible(true)]
    [Obsolete("DataGridCellAccessibleObject has been deprecated.")]
    protected class DataGridCellAccessibleObject : AccessibleObject
    {
        public DataGridCellAccessibleObject(DataGridRow owner, int column) : base()
        {
            throw new PlatformNotSupportedException();
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

        protected DataGrid DataGrid
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
            set
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override void DoDefaultAction()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetFocused()
        {
            throw new PlatformNotSupportedException();
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
