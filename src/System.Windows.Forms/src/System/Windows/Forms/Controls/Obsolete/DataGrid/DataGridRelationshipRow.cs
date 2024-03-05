// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Windows.Forms;

#nullable disable
[Obsolete("DataGridRelationshipRow has been deprecated.")]
internal class DataGridRelationshipRow : DataGridRow
{
    private const bool defaultOpen = false;
    private const int expandoBoxWidth = 14;
    private bool expanded = defaultOpen;

    public DataGridRelationshipRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
    : base(dataGrid, dgTable, rowNumber)
    {
        throw new PlatformNotSupportedException();
    }

    internal protected override int MinimumRowHeight(GridColumnStylesCollection cols)
    {
        return base.MinimumRowHeight(cols) + (expanded ? GetRelationshipRect().Height : 0);
    }

    internal protected override int MinimumRowHeight(DataGridTableStyle dgTable)
    {
        return base.MinimumRowHeight(dgTable) + (expanded ? GetRelationshipRect().Height : 0);
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool Expanded
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

#if FALSE
        private int BorderWidth {
            get {
                DataGrid dataGrid = DataGrid;
                if (dataGrid == null)
                    return 0;
                // if the user set the GridLineStyle property on the dataGrid.
                // then use the value of that property
                DataGridLineStyle gridStyle;
                int gridLineWidth;
                if (dgTable.IsDefault) {
                    gridStyle = DataGrid.GridLineStyle;
                    gridLineWidth = DataGrid.GridLineWidth;
                } else {
                    gridStyle = dgTable.GridLineStyle;
                    gridLineWidth = dgTable.GridLineWidth;
                }

                if (gridStyle == DataGridLineStyle.None)
                    return 0;

                return gridLineWidth;
            }
        }
#endif //FALSE

    private int FocusedRelation
    {
        get
        {
            return dgTable.FocusedRelation;
        }
        set
        {
            dgTable.FocusedRelation = value;
        }
    }

    // =------------------------------------------------------------------
    // =        Methods
    // =------------------------------------------------------------------

    private void Collapse()
    {
        Debug.Assert(dgTable.DataGrid.AllowNavigation, "how can the user collapse the relations if the grid does not allow navigation?");
        if (expanded)
        {
            expanded = false;
            // relationshipRect = Rectangle.Empty;
            FocusedRelation = -1;
            DataGrid.OnRowHeightChanged(this);
        }
    }

    protected override AccessibleObject CreateAccessibleObject()
    {
        return new DataGridRelationshipRowAccessibleObject(this);
    }

    private void Expand()
    {
        Debug.Assert(dgTable.DataGrid.AllowNavigation, "how can the user expand the relations if the grid does not allow navigation?");
        if (expanded == false
            && DataGrid is not null
            && dgTable is not null
            && dgTable.RelationsList.Count > 0)
        {
            expanded = true;
            FocusedRelation = -1;

            // relationshipRect = Rectangle.Empty;
            DataGrid.OnRowHeightChanged(this);
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override int Height
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

    // so the edit box will not paint under the
    // grid line of the row
    public override Rectangle GetCellBounds(int col)
    {
        throw new PlatformNotSupportedException();
    }

    private Rectangle GetOutlineRect(int xOrigin, int yOrigin)
    {
        Rectangle outline = new Rectangle(xOrigin + 2,
                                          yOrigin + 2,
                                          9,
                                          9);
        return outline;
    }

    public override Rectangle GetNonScrollableArea()
    {
        throw new PlatformNotSupportedException();
    }

    private Rectangle GetRelationshipRect()
    {
        Debug.Assert(expanded, "we should need this rectangle only when the row is expanded");
        Rectangle ret = dgTable.RelationshipRect;
        ret.Y = base.Height - dgTable.BorderWidth;
        return ret;
    }

#if FALSE
        private Rectangle GetRelationshipRect() {
            if (relationshipRect.IsEmpty) {
                Debug.WriteLineIf(CompModSwitches.DGRelationShpRowLayout.TraceVerbose, "GetRelationshipRect grinding away");
                if (!expanded) {
                    return(relationshipRect = new Rectangle(0,0,0,0));
                }
                Graphics g = DataGrid.CreateGraphicsInternal();
                relationshipRect = new Rectangle();
                relationshipRect.X = 0; //indentWidth;
                relationshipRect.Y = base.Height - dgTable.BorderWidth;

                // Determine the width of the widest relationship name
                int longestRelationship = 0;
                for (int r = 0; r < dgTable.RelationsList.Count; ++r) {
                    int rwidth = (int) Math.Ceiling(g.MeasureString(((string) dgTable.RelationsList[r]), DataGrid.LinkFont).Width);
                    if (rwidth > longestRelationship)
                        longestRelationship = rwidth;
                }

                g.Dispose();

                relationshipRect.Width = longestRelationship + 5;
                relationshipRect.Width += 2; // relationshipRect border;
                relationshipRect.Height = dgTable.BorderWidth + relationshipHeight * dgTable.RelationsList.Count;
                relationshipRect.Height += 2; // relationship border
                if (dgTable.RelationsList.Count > 0)
                    relationshipRect.Height += 2 * System.Windows.Forms.DataGridTableStyle.relationshipSpacing;
            }
            return relationshipRect;
        }

#endif// FALSE

    private Rectangle GetRelationshipRectWithMirroring()
    {
        Rectangle relRect = GetRelationshipRect();
        bool rowHeadersVisible = dgTable.IsDefault ? DataGrid.RowHeadersVisible : dgTable.RowHeadersVisible;
        if (rowHeadersVisible)
        {
            int rowHeaderWidth = dgTable.IsDefault ? DataGrid.RowHeaderWidth : dgTable.RowHeaderWidth;
            relRect.X += DataGrid.GetRowHeaderRect().X + rowHeaderWidth;
        }

        relRect.X = MirrorRelationshipRectangle(relRect, DataGrid.GetRowHeaderRect(), DataGrid.RightToLeft == RightToLeft.Yes);
        return relRect;
    }

    private bool PointOverPlusMinusGlyph(int x, int y, Rectangle rowHeaders, bool alignToRight)
    {
        if (dgTable is null || dgTable.DataGrid is null || !dgTable.DataGrid.AllowNavigation)
            return false;
        Rectangle insideRowHeaders = rowHeaders;
        if (!DataGrid.FlatMode)
        {
            insideRowHeaders.Inflate(-1, -1);
        }

        Rectangle outline = GetOutlineRect(insideRowHeaders.Right - expandoBoxWidth, 0);

        outline.X = MirrorRectangle(outline.X, outline.Width, insideRowHeaders, alignToRight);

        return outline.Contains(x, y);
    }

    public override bool OnMouseDown(int x, int y, Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public override bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    // this function will not invalidate all of the row
    public override void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    public override void OnMouseLeft()
    {
        throw new PlatformNotSupportedException();
    }

    public override bool OnKeyPress(Keys keyData)
    {
        throw new PlatformNotSupportedException();
    }

    // will reset the FocusedRelation and will invalidate the
    // rectangle so that the linkFont is no longer shown
    internal override void LoseChildFocus(Rectangle rowHeaders, bool alignToRight)
    {
        // we only invalidate stuff if the row is expanded.
        if (FocusedRelation == -1 || !expanded)
            return;

        FocusedRelation = -1;
        Rectangle relRect = GetRelationshipRect();
        relRect.X += rowHeaders.X + dgTable.RowHeaderWidth;
        relRect.X = MirrorRelationshipRectangle(relRect, rowHeaders, alignToRight);
        InvalidateRowRect(relRect);
    }

    // here is the logic for FOCUSED:
    //
    // first the dataGrid gets the KeyPress.
    // the dataGrid passes it to the currentRow. if it is anything other
    // than Enter or TAB, the currentRow resets the FocusedRelation variable.
    //
    // Then the dataGrid takes another look at the TAB key and if it is the case
    // it passes it to the row. If the dataRelationshipRow can become focused,
    // then it eats the TAB key, otherwise it will give it back to the dataGrid.
    //
    internal override bool ProcessTabKey(Keys keyData, Rectangle rowHeaders, bool alignToRight)
    {
        Debug.Assert((keyData & Keys.Control) != Keys.Control, "the DataGridRelationshipRow only handles TAB and TAB-SHIFT");
        Debug.Assert((keyData & Keys.Alt) != Keys.Alt, "the DataGridRelationshipRow only handles TAB and TAB-SHIFT");

        // if there are no relationships, this row can't do anything with the key
        if (dgTable.RelationsList.Count == 0 || dgTable.DataGrid is null || !dgTable.DataGrid.AllowNavigation)
            return false;

        // expand the relationship box
        if (!expanded)
            Expand();

        if ((keyData & Keys.Shift) == Keys.Shift)
        {
            if (FocusedRelation == 0)
            {
                // if user hits TAB-SHIFT and the focus was on the first relationship then
                // reset FocusedRelation and let the dataGrid use the key
                //
                // consider: DANIELHE: if the relationships box is expanded, should we collapse it on leave?
                FocusedRelation = -1;
                return false;
            }

            // we need to invalidate the relationshipRectangle, and cause the linkFont to move
            // to the next relation
            Rectangle relRect = GetRelationshipRect();
            relRect.X += rowHeaders.X + dgTable.RowHeaderWidth;
            relRect.X = MirrorRelationshipRectangle(relRect, rowHeaders, alignToRight);
            InvalidateRowRect(relRect);

            if (FocusedRelation == -1)
                // is the first time that the user focuses on this
                // set of relationships
                FocusedRelation = dgTable.RelationsList.Count - 1;
            else
                FocusedRelation--;
            return true;
        }
        else
        {
            if (FocusedRelation == dgTable.RelationsList.Count - 1)
            {
                // if the user hits TAB and the focus was on the last relationship then
                // reset FocusedRelation and let the dataGrid use the key
                //
                // consider: DANIELHE: if the relationships box is expanded, should we collapse it on leave?
                FocusedRelation = -1;
                return false;
            }

            // we need to invalidate the relationshipRectangle, and cause the linkFont to move
            // to the next relation
            Rectangle relRect = GetRelationshipRect();
            relRect.X += rowHeaders.X + dgTable.RowHeaderWidth;
            relRect.X = MirrorRelationshipRectangle(relRect, rowHeaders, alignToRight);
            InvalidateRowRect(relRect);

            FocusedRelation++;
            return true;
        }
    }

    public override int Paint(Graphics g, Rectangle bounds, Rectangle trueRowBounds, int firstVisibleColumn, int numVisibleColumns)
    {
        throw new PlatformNotSupportedException();
    }

    public override int Paint(Graphics g,
                              Rectangle bounds,          // negative offsetted row bounds
                              Rectangle trueRowBounds,   // real row bounds.
                              int firstVisibleColumn,
                              int numVisibleColumns,
                              bool alignToRight)
    {
        throw new PlatformNotSupportedException();
    }

    protected override void PaintCellContents(Graphics g, Rectangle cellBounds, DataGridColumnStyle column,
                                              Brush backBr, Brush foreBrush, bool alignToRight)
    {
        CurrencyManager listManager = DataGrid.ListManager;

        // painting the error..
        //
        string errString = string.Empty;
        Rectangle bounds = cellBounds;
        object errInfo = DataGrid.ListManager[number];
        if (errInfo is IDataErrorInfo)
            errString = ((IDataErrorInfo)errInfo)[column.PropertyDescriptor.Name];

        if (!string.IsNullOrEmpty(errString))
        {
            Bitmap bmp = GetErrorBitmap();
            Rectangle errRect;
            lock (bmp)
            {
                errRect = PaintIcon(g, bounds, true, alignToRight, bmp, backBr);
            }

            // paint the errors correctly when RTL = true
            if (alignToRight)
                bounds.Width -= errRect.Width + xOffset;
            else
                bounds.X += errRect.Width + xOffset;
            DataGridToolTip.AddToolTip(errString, (IntPtr)(DataGrid.ToolTipId++), errRect);
        }

        column.Paint(g, bounds, listManager, RowNumber, backBr, foreBrush, alignToRight);
    }

    public override void PaintHeader(Graphics g, Rectangle bounds, bool alignToRight, bool isDirty)
    {
        throw new PlatformNotSupportedException();
    }

    public void PaintHeaderInside(Graphics g, Rectangle bounds, Brush backBr, bool alignToRight, bool isDirty)
    {
        throw new PlatformNotSupportedException();
    }

    private void PaintRelations(Graphics g, Rectangle bounds, Rectangle trueRowBounds,
                                int dataWidth, int firstCol, int nCols, bool alignToRight)
    {
        // Calculate the relationship rect.
        // relationshipRect = Rectangle.Empty;
        Rectangle relRect = GetRelationshipRect();
        // relRect.Offset(trueRowBounds.X, trueRowBounds.Y);
        relRect.X = alignToRight ? bounds.Right - relRect.Width : bounds.X;
        relRect.Y = bounds.Y;
        int paintedWidth = Math.Max(dataWidth, relRect.Width);

        // Paint the stuff to the right , or left (Bi-Di) of the relationship rect.
        Region r = g.Clip;
        g.ExcludeClip(relRect);

        g.FillRectangle(GetBackBrush(),
                        alignToRight ? bounds.Right - dataWidth : bounds.X,
                        bounds.Y,
                        dataWidth,
                        bounds.Height);

        // Paint the relations' text
        g.SetClip(bounds);

        relRect.Height -= dgTable.BorderWidth;     // use bWidth not 1
        g.DrawRectangle(SystemPens.ControlText, relRect.X, relRect.Y, relRect.Width - 1, relRect.Height - 1);
        relRect.Inflate(-1, -1);

        int cy = PaintRelationText(g, relRect, alignToRight);

        if (cy < relRect.Height)
        {
            g.FillRectangle(GetBackBrush(), relRect.X, relRect.Y + cy, relRect.Width, relRect.Height - cy);
        }

        g.Clip = r;

        // paint any exposed area to the right or to the left (BI-DI)
        if (paintedWidth < bounds.Width)
        {
            int bWidth;
            if (dgTable.IsDefault)
                bWidth = DataGrid.GridLineWidth;
            else
                bWidth = dgTable.GridLineWidth;
            g.FillRectangle(DataGrid.BackgroundBrush,
                            alignToRight ? bounds.X : bounds.X + paintedWidth,
                            bounds.Y,
                            bounds.Width - paintedWidth - bWidth + 1, // + 1 cause the relationship rectangle was deflated
                            bounds.Height);

            // Paint the border to the right of each cell
            if (bWidth > 0)
            {
                Brush br;
                // if the user changed the gridLineColor on the dataGrid
                // from the defaultValue, then use that value;
                if (dgTable.IsDefault)
                    br = DataGrid.GridLineBrush;
                else
                    br = dgTable.GridLineBrush;
                g.FillRectangle(br,
                                alignToRight ? bounds.Right - bWidth - paintedWidth : bounds.X + paintedWidth - bWidth,
                                bounds.Y,
                                bWidth,
                                bounds.Height);
            }
        }
    }

    private int PaintRelationText(Graphics g, Rectangle bounds, bool alignToRight)
    {
        g.FillRectangle(GetBackBrush(), bounds.X, bounds.Y, bounds.Width, System.Windows.Forms.DataGridTableStyle.relationshipSpacing);

        int relationshipHeight = dgTable.RelationshipHeight;
        Rectangle textBounds = new Rectangle(bounds.X, bounds.Y + System.Windows.Forms.DataGridTableStyle.relationshipSpacing,
                                             bounds.Width,
                                             relationshipHeight);
        int cy = System.Windows.Forms.DataGridTableStyle.relationshipSpacing;
        for (int r = 0; r < dgTable.RelationsList.Count; ++r)
        {
            if (cy > bounds.Height)
                break;

            Brush textBrush = dgTable.IsDefault ? DataGrid.LinkBrush : dgTable.LinkBrush;

            Font textFont = DataGrid.Font;
            textBrush = dgTable.IsDefault ? DataGrid.LinkBrush : dgTable.LinkBrush;
            textFont = DataGrid.LinkFont;

            g.FillRectangle(GetBackBrush(), textBounds);

            StringFormat format = new StringFormat();
            if (alignToRight)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                format.Alignment = StringAlignment.Far;
            }

            g.DrawString(((string)dgTable.RelationsList[r]), textFont, textBrush, textBounds,
                         format);
            if (r == FocusedRelation && number == DataGrid.CurrentCell.RowNumber)
            {
                textBounds.Width = dgTable.FocusedTextWidth;
                ControlPaint.DrawFocusRectangle(g, textBounds, ((SolidBrush)textBrush).Color, ((SolidBrush)GetBackBrush()).Color);
                textBounds.Width = bounds.Width;
            }

            format.Dispose();

            textBounds.Y += relationshipHeight;
            cy += textBounds.Height;
        }

        return cy;
    }

    private void PaintPlusMinusGlyph(Graphics g, Rectangle bounds, Brush backBr, bool alignToRight)
    {
        if (CompModSwitches.DGRelationShpRowPaint.TraceVerbose)
            Debug.WriteLine("PlusMinusGlyph painting in bounds    -> " + bounds.ToString());
        Rectangle outline = GetOutlineRect(bounds.X, bounds.Y);

        outline = Rectangle.Intersect(bounds, outline);
        if (outline.IsEmpty)
            return;

        g.FillRectangle(backBr, bounds);

        if (CompModSwitches.DGRelationShpRowPaint.TraceVerbose)
            Debug.WriteLine("Painting PlusMinusGlyph with outline -> " + outline.ToString());
        // draw the +/- box
        Pen drawPen = dgTable.IsDefault ? DataGrid.HeaderForePen : dgTable.HeaderForePen;
        g.DrawRectangle(drawPen, outline.X, outline.Y, outline.Width - 1, outline.Height - 1);

        int indent = 2;
        // draw the -
        g.DrawLine(drawPen,
                   outline.X + indent, outline.Y + outline.Width / 2,
                   outline.Right - indent - 1, outline.Y + outline.Width / 2);        // -1 on the y coordinate

        if (!expanded)
        {
            // draw the vertical line to make +
            g.DrawLine(drawPen,
                       outline.X + outline.Height / 2, outline.Y + indent,
                       outline.X + outline.Height / 2, outline.Bottom - indent - 1); // -1... hinting
        }
        else
        {
            Point[] points = new Point[3];
            points[0] = new Point(outline.X + outline.Height / 2, outline.Bottom);

            points[1] = new Point(points[0].X, bounds.Y + 2 * indent + base.Height);

            points[2] = new Point(alignToRight ? bounds.X : bounds.Right,
                                  points[1].Y);
            g.DrawLines(drawPen, points);
        }
    }

    private int RelationFromY(int y)
    {
        int relation = -1;
        int relationshipHeight = dgTable.RelationshipHeight;
        Rectangle relRect = GetRelationshipRect();
        int cy = base.Height - dgTable.BorderWidth + System.Windows.Forms.DataGridTableStyle.relationshipSpacing;
        while (cy < relRect.Bottom)
        {
            if (cy > y)
                break;
            cy += relationshipHeight;
            relation++;
        }

        if (relation >= dgTable.RelationsList.Count)
            return -1;
        return relation;
    }

    // given the relRect and the rowHeader, this function will return the
    // X coordinate of the relationship rectangle as it should appear on the screen
    private int MirrorRelationshipRectangle(Rectangle relRect, Rectangle rowHeader, bool alignToRight)
    {
        if (alignToRight)
            return rowHeader.X - relRect.Width;
        else
            return relRect.X;
    }

    // given the X and Width of a rectangle R1 contained in rect,
    // this will return the X coordinate of the rectangle that corresponds to R1 in Bi-Di transformation
    private int MirrorRectangle(int x, int width, Rectangle rect, bool alignToRight)
    {
        if (alignToRight)
            return rect.Right + rect.X - width - x;
        else
            return x;
    }

    [ComVisible(true)]
    [Obsolete("DataGridRelationshipRowAccessibleObject has been deprecated.")]
    protected class DataGridRelationshipRowAccessibleObject : DataGridRowAccessibleObject
    {
        public DataGridRelationshipRowAccessibleObject(DataGridRow owner) : base(owner)
        {
            throw new PlatformNotSupportedException();
        }

        protected override void AddChildAccessibleObjects(IList children)
        {
            base.AddChildAccessibleObjects(children);
            DataGridRelationshipRow row = (DataGridRelationshipRow)Owner;
            if (row.dgTable.RelationsList is not null)
            {
                for (int i = 0; i < row.dgTable.RelationsList.Count; i++)
                {
                    children.Add(new DataGridRelationshipAccessibleObject(row, i));
                }
            }
        }

        private DataGridRelationshipRow RelationshipRow
        {
            get
            {
                return (DataGridRelationshipRow)Owner;
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
        public override AccessibleStates State
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            throw new PlatformNotSupportedException();
        }

        public override AccessibleObject GetFocused()
        {
            throw new PlatformNotSupportedException();
        }
    }

    [ComVisible(true)]
    [Obsolete("DataGridRelationshipAccessibleObject has been deprecated.")]
    protected class DataGridRelationshipAccessibleObject : AccessibleObject
    {
        public DataGridRelationshipAccessibleObject(DataGridRelationshipRow owner, int relationship) : base()
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

        protected DataGridRelationshipRow Owner
        {
            get;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        protected DataGrid DataGrid
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
            set
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

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            throw new PlatformNotSupportedException();
        }

        private void ResetAccessibilityLayer()
        {
            ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Reorder, 0);
            ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Focus, DataGrid.CurrentCellAccIndex);
            ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Selection, DataGrid.CurrentCellAccIndex);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            throw new PlatformNotSupportedException();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void Select(AccessibleSelection flags)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
