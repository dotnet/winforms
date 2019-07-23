// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class fully encapsulates the painting logic for a row
    ///  appearing in a DataGrid.
    /// </summary>
    internal class DataGridRelationshipRow : DataGridRow
    {
        private const bool defaultOpen = false;
        private const int expandoBoxWidth = 14;
        private const int indentWidth = 20;
        // private const int  relationshipSpacing = 1;
        private const int triangleSize = 5;

        private bool expanded = defaultOpen;
        // private bool hasRelationships = false;
        // private Font linkFont = null;
        // private new DataGrid dataGrid; // Currently used only to obtain a Graphics object for measuring text

        // private Rectangle relationshipRect   = Rectangle.Empty;
        // private int       relationshipHeight = 0;

        // relationships
        // we should get this directly from the dgTable.
        // private ArrayList     relationships;
        // private int            focusedRelation = -1;
        // private int          focusedTextWidth;

        public DataGridRelationshipRow(DataGrid dataGrid, DataGridTableStyle dgTable, int rowNumber)
        : base(dataGrid, dgTable, rowNumber)
        {
            // this.dataGrid = dataGrid;
            // linkFont = dataGrid.LinkFont;
            // relationshipHeight = dataGrid.LinkFontHeight + this.dgTable.relationshipSpacing;

            // if (DataGrid.AllowNavigation) {
            //     hasRelationships = dgTable.RelationsList.Count > 0;
            // }
        }

        internal protected override int MinimumRowHeight(GridColumnStylesCollection cols)
        {
            /*
            if (DataGrid != null && DataGrid.LinkFontHeight + this.dgTable.relationshipSpacing != relationshipHeight) {
                relationshipRect = Rectangle.Empty;
                relationshipHeight = DataGrid.LinkFontHeight + this.dgTable.relationshipSpacing;
            }
            */

            return base.MinimumRowHeight(cols) + (expanded ? GetRelationshipRect().Height : 0);
        }

        internal protected override int MinimumRowHeight(DataGridTableStyle dgTable)
        {
            /*
            if (DataGrid != null && DataGrid.LinkFontHeight + this.dgTable.relationshipSpacing != relationshipHeight) {
                relationshipRect = Rectangle.Empty;
                relationshipHeight = DataGrid.LinkFontHeight + this.dgTable.relationshipSpacing;
            }
            */

            return base.MinimumRowHeight(dgTable) + (expanded ? GetRelationshipRect().Height : 0);
        }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        public virtual bool Expanded
        {
            get
            {
                return expanded;
            }
            set
            {
                if (expanded == value)
                {
                    return;
                }

                if (expanded)
                {
                    Collapse();
                }
                else
                {
                    Expand();
                }
            }
        }

        /*
        private Color BorderColor {
            get {
                if (DataGrid == null)
                    return Color.Empty;
                return DataGrid.GridLineColor;
            }
        }
        */

#if FALSE
        private int BorderWidth {
            get {
                DataGrid dataGrid = this.DataGrid;
                if (dataGrid == null)
                    return 0;
                // if the user set the GridLineStyle property on the dataGrid.
                // then use the value of that property
                DataGridLineStyle gridStyle;
                int gridLineWidth;
                if (this.dgTable.IsDefault) {
                    gridStyle = this.DataGrid.GridLineStyle;
                    gridLineWidth = this.DataGrid.GridLineWidth;
                } else {
                    gridStyle = this.dgTable.GridLineStyle;
                    gridLineWidth = this.dgTable.GridLineWidth;
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
                && DataGrid != null
                && dgTable != null
                && dgTable.RelationsList.Count > 0)
            {
                expanded = true;
                FocusedRelation = -1;

                // relationshipRect = Rectangle.Empty;
                DataGrid.OnRowHeightChanged(this);
            }
        }

        public override int Height
        {
            get
            {
                int height = base.Height;
                if (expanded)
                {
                    return height + GetRelationshipRect().Height;
                }
                else
                {
                    return height;
                }
            }
            set
            {
                // we should use the RelationshipRect only when the row is expanded
                if (expanded)
                {
                    base.Height = value - GetRelationshipRect().Height;
                }
                else
                {
                    base.Height = value;
                }
            }
        }

        // so the edit box will not paint under the
        // grid line of the row
        public override Rectangle GetCellBounds(int col)
        {
            Rectangle cellBounds = base.GetCellBounds(col);
            // decrement base.Height by 1, so the edit box will not
            // paint over the bottom line.
            cellBounds.Height = base.Height - 1;
            return cellBounds;
        }

        /// <summary>
        ///  Given an origin, this procedure returns
        ///  a rectangle that describes the location of an outline box.
        /// </summary>
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
            if (expanded)
            {
                return GetRelationshipRect();
            }
            else
            {
                return Rectangle.Empty;
            }
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
                relationshipRect.Y = base.Height - this.dgTable.BorderWidth;

                // Determine the width of the widest relationship name
                int longestRelationship = 0;
                for (int r = 0; r < this.dgTable.RelationsList.Count; ++r) {
                    int rwidth = (int) Math.Ceiling(g.MeasureString(((string) this.dgTable.RelationsList[r]), this.DataGrid.LinkFont).Width);
                    if (rwidth > longestRelationship)
                        longestRelationship = rwidth;
                }

                g.Dispose();

                relationshipRect.Width = longestRelationship + 5;
                relationshipRect.Width += 2; // relationshipRect border;
                relationshipRect.Height = this.dgTable.BorderWidth + relationshipHeight * this.dgTable.RelationsList.Count;
                relationshipRect.Height += 2; // relationship border
                if (this.dgTable.RelationsList.Count > 0)
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

        /// <summary>
        ///  Called by the DataGrid when a click occurs in the row's client
        ///  area.  The coordinates are normalized to the rectangle's top
        ///  left point.
        /// </summary>
        private bool PointOverPlusMinusGlyph(int x, int y, Rectangle rowHeaders, bool alignToRight)
        {
            if (dgTable == null || dgTable.DataGrid == null || !dgTable.DataGrid.AllowNavigation)
            {
                return false;
            }

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
            bool rowHeadersVisible = dgTable.IsDefault ? DataGrid.RowHeadersVisible : dgTable.RowHeadersVisible;
            if (rowHeadersVisible)
            {
                if (PointOverPlusMinusGlyph(x, y, rowHeaders, alignToRight))
                {
                    if (dgTable.RelationsList.Count == 0)
                    {
                        return false;
                    }
                    else if (expanded)
                    {
                        Collapse();
                    }
                    else
                    {
                        Expand();
                    }
                    DataGrid.OnNodeClick(EventArgs.Empty);
                    return true;
                }
            }

            if (!expanded)
            {
                return base.OnMouseDown(x, y, rowHeaders, alignToRight);
            }

            // hit test for relationships
            Rectangle relRect = GetRelationshipRectWithMirroring();

            if (relRect.Contains(x, y))
            {
                int r = RelationFromY(y);
                if (r != -1)
                {
                    // first, reset the FocusedRelation
                    FocusedRelation = -1;
                    DataGrid.NavigateTo(((string)dgTable.RelationsList[r]), this, true);
                }
                // DataGrid.OnLinkClick(EventArgs.Empty);
                return true;
            }

            return base.OnMouseDown(x, y, rowHeaders, alignToRight);
        }

        public override bool OnMouseMove(int x, int y, Rectangle rowHeaders, bool alignToRight)
        {
            if (!expanded)
            {
                return false;
            }

            Rectangle relRect = GetRelationshipRectWithMirroring();

            if (relRect.Contains(x, y))
            {
                DataGrid.Cursor = Cursors.Hand;
                return true;
            }

            DataGrid.Cursor = Cursors.Default;
            return base.OnMouseMove(x, y, rowHeaders, alignToRight);
        }

        // this function will not invalidate all of the
        // row
        public override void OnMouseLeft(Rectangle rowHeaders, bool alignToRight)
        {
            if (!expanded)
            {
                return;
            }

            Rectangle relRect = GetRelationshipRect();
            relRect.X += rowHeaders.X + dgTable.RowHeaderWidth;
            relRect.X = MirrorRelationshipRectangle(relRect, rowHeaders, alignToRight);

            if (FocusedRelation != -1)
            {
                InvalidateRowRect(relRect);
                FocusedRelation = -1;
            }
        }

        public override void OnMouseLeft()
        {
            if (!expanded)
            {
                return;
            }

            if (FocusedRelation != -1)
            {
                InvalidateRow();
                FocusedRelation = -1;
            }
            base.OnMouseLeft();
        }

        /// <summary>
        ///  Called by the DataGrid when a keypress occurs on a row with "focus."
        /// </summary>
        public override bool OnKeyPress(Keys keyData)
        {
            // ignore the shift key if it is not paired w/ the TAB key
            if ((keyData & Keys.Modifiers) == Keys.Shift && (keyData & Keys.KeyCode) != Keys.Tab)
            {
                return false;
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.F5:
                    if (dgTable == null || dgTable.DataGrid == null || !dgTable.DataGrid.AllowNavigation)
                    {
                        return false;
                    }

                    if (expanded)
                    {
                        Collapse();
                    }
                    else
                    {
                        Expand();
                    }

                    FocusedRelation = -1;
                    return true;

                // to make the gridTest run w/ the numLock key on
                //
                case Keys.NumLock:
                    if (FocusedRelation != -1)
                    {
                        return false;
                    }
                    else
                    {
                        return base.OnKeyPress(keyData);
                    }

                case Keys.Enter:
                    if (FocusedRelation != -1)
                    {
                        // somebody set the relation number up already
                        // navigate to the relation
                        DataGrid.NavigateTo(((string)dgTable.RelationsList[FocusedRelation]), this, true);

                        // now reset the FocusedRelation
                        FocusedRelation = -1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Keys.Tab:
                    return false;

                default:
                    FocusedRelation = -1;
                    return base.OnKeyPress(keyData);
            }
        }

        // will reset the FocusedRelation and will invalidate the
        // rectangle so that the linkFont is no longer shown
        internal override void LoseChildFocus(Rectangle rowHeaders, bool alignToRight)
        {
            // we only invalidate stuff if the row is expanded.
            if (FocusedRelation == -1 || !expanded)
            {
                return;
            }

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

            // if there are no relationships, this row can't do anything with the
            // key
            if (dgTable.RelationsList.Count == 0 || dgTable.DataGrid == null || !dgTable.DataGrid.AllowNavigation)
            {
                return false;
            }

            // expand the relationship box
            if (!expanded)
            {
                Expand();
            }

            if ((keyData & Keys.Shift) == Keys.Shift)
            {
                if (FocusedRelation == 0)
                {
                    // if user hits TAB-SHIFT and the focus was on the first relationship then
                    // reset FocusedRelation and let the dataGrid use the key
                    //
                    // consider: Microsoft: if the relationships box is expanded, should we collapse it on leave?
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
                {
                    // is the first time that the user focuses on this
                    // set of relationships
                    FocusedRelation = dgTable.RelationsList.Count - 1;
                }
                else
                {
                    FocusedRelation--;
                }

                return true;
            }
            else
            {
                if (FocusedRelation == dgTable.RelationsList.Count - 1)
                {
                    // if the user hits TAB and the focus was on the last relationship then
                    // reset FocusedRelation and let the dataGrid use the key
                    //
                    // consider: Microsoft: if the relationships box is expanded, should we collapse it on leave?
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

        /// <summary>
        ///  Paints the row.
        /// </summary>
        public override int Paint(Graphics g, Rectangle bounds, Rectangle trueRowBounds, int firstVisibleColumn, int numVisibleColumns)
        {
            return Paint(g, bounds, trueRowBounds, firstVisibleColumn, numVisibleColumns, false);
        }

        public override int Paint(Graphics g,
                                  Rectangle bounds,          // negative offsetted row bounds
                                  Rectangle trueRowBounds,   // real row bounds.
                                  int firstVisibleColumn,
                                  int numVisibleColumns,
                                  bool alignToRight)
        {
            if (CompModSwitches.DGRelationShpRowPaint.TraceVerbose)
            {
                Debug.WriteLine("Painting row " + RowNumber.ToString(CultureInfo.InvariantCulture) + " with bounds " + bounds.ToString());
            }

            int bWidth = dgTable.BorderWidth;

            // paint the data cells
            Rectangle dataBounds = bounds;
            dataBounds.Height = base.Height - bWidth;
            int dataWidth = PaintData(g, dataBounds, firstVisibleColumn, numVisibleColumns, alignToRight);
            int dataWidthOffsetted = dataWidth + bounds.X - trueRowBounds.X;

            dataBounds.Offset(0, bWidth);       // use bWidth, not 1
            if (bWidth > 0)
            {
                PaintBottomBorder(g, dataBounds, dataWidth, bWidth, alignToRight);
            }

            if (expanded && dgTable.RelationsList.Count > 0)
            {
                // paint the relationships
                Rectangle relationBounds = new Rectangle(trueRowBounds.X,
                                                         dataBounds.Bottom,
                                                         trueRowBounds.Width,
                                                         trueRowBounds.Height - dataBounds.Height - 2 * bWidth);
                PaintRelations(g, relationBounds, trueRowBounds, dataWidthOffsetted,
                               firstVisibleColumn, numVisibleColumns, alignToRight);
                relationBounds.Height += 1;
                if (bWidth > 0)
                {
                    PaintBottomBorder(g, relationBounds, dataWidthOffsetted, bWidth, alignToRight);
                }
            }

            return dataWidth;
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
            {
                errString = ((IDataErrorInfo)errInfo)[column.PropertyDescriptor.Name];
            }

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
                {
                    bounds.Width -= errRect.Width + xOffset;
                }
                else
                {
                    bounds.X += errRect.Width + xOffset;
                }

                DataGrid.ToolTipProvider.AddToolTip(errString, (IntPtr)(DataGrid.ToolTipId++), errRect);
            }

            column.Paint(g, bounds, listManager, RowNumber, backBr, foreBrush, alignToRight);
        }

        public override void PaintHeader(Graphics g, Rectangle bounds, bool alignToRight, bool isDirty)
        {
            DataGrid grid = DataGrid;

            Rectangle insideBounds = bounds;

            if (!grid.FlatMode)
            {
                ControlPaint.DrawBorder3D(g, insideBounds, Border3DStyle.RaisedInner);
                insideBounds.Inflate(-1, -1);
            }

            if (dgTable.IsDefault)
            {
                PaintHeaderInside(g, insideBounds, DataGrid.HeaderBackBrush, alignToRight, isDirty);
            }
            else
            {
                PaintHeaderInside(g, insideBounds, dgTable.HeaderBackBrush, alignToRight, isDirty);
            }
        }

        public void PaintHeaderInside(Graphics g, Rectangle bounds, Brush backBr, bool alignToRight, bool isDirty)
        {
            // paint the row header
            bool paintPlusMinus = dgTable.RelationsList.Count > 0 && dgTable.DataGrid.AllowNavigation;
            int rowHeaderBoundsX = MirrorRectangle(bounds.X,
                                                   bounds.Width - (paintPlusMinus ? expandoBoxWidth : 0),
                                                   bounds, alignToRight);

            if (!alignToRight)
            {
                Debug.Assert(bounds.X == rowHeaderBoundsX, "what's up doc?");
            }

            Rectangle rowHeaderBounds = new Rectangle(rowHeaderBoundsX,
                                                      bounds.Y,
                                                      bounds.Width - (paintPlusMinus ? expandoBoxWidth : 0),
                                                      bounds.Height);

            base.PaintHeader(g, rowHeaderBounds, alignToRight, isDirty);

            // Paint the expando on the right
            int expandoBoxX = MirrorRectangle(bounds.X + rowHeaderBounds.Width, expandoBoxWidth, bounds, alignToRight);

            if (!alignToRight)
            {
                Debug.Assert(rowHeaderBounds.Right == expandoBoxX, "what's up doc?");
            }

            Rectangle expandoBox = new Rectangle(expandoBoxX,
                                                 bounds.Y,
                                                 expandoBoxWidth,
                                                 bounds.Height);
            if (paintPlusMinus)
            {
                PaintPlusMinusGlyph(g, expandoBox, backBr, alignToRight);
            }

        }

        /// <summary>
        ///  Paints the relationships below the data area.
        /// </summary>
        private void PaintRelations(Graphics g, Rectangle bounds, Rectangle trueRowBounds,
                                    int dataWidth, int firstCol, int nCols, bool alignToRight)
        {
            // Calculate the relationship rect.
            // relationshipRect = Rectangle.Empty;
            Rectangle relRect = GetRelationshipRect();
            //relRect.Offset(trueRowBounds.X, trueRowBounds.Y);
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
                {
                    bWidth = DataGrid.GridLineWidth;
                }
                else
                {
                    bWidth = dgTable.GridLineWidth;
                }

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
                    {
                        br = DataGrid.GridLineBrush;
                    }
                    else
                    {
                        br = dgTable.GridLineBrush;
                    }

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
                {
                    break;
                }

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
            {
                Debug.WriteLine("PlusMinusGlyph painting in bounds    -> " + bounds.ToString());
            }

            Rectangle outline = GetOutlineRect(bounds.X, bounds.Y);

            outline = Rectangle.Intersect(bounds, outline);
            if (outline.IsEmpty)
            {
                return;
            }

            g.FillRectangle(backBr, bounds);

            if (CompModSwitches.DGRelationShpRowPaint.TraceVerbose)
            {
                Debug.WriteLine("Painting PlusMinusGlyph with outline -> " + outline.ToString());
            }
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
                {
                    break;
                }

                cy += relationshipHeight;
                relation++;
            }
            if (relation >= dgTable.RelationsList.Count)
            {
                return -1;
            }

            return relation;
        }

        // given the relRect and the rowHeader, this function will return the
        // X coordinate of the relationship rectangle as it should appear on the screen
        private int MirrorRelationshipRectangle(Rectangle relRect, Rectangle rowHeader, bool alignToRight)
        {
            if (alignToRight)
            {
                return rowHeader.X - relRect.Width;
            }
            else
            {
                return relRect.X;
            }
        }

        // given the X and Width of a rectangle R1 contained in rect,
        // this will return the X coordinate of the rectangle that corresponds to R1 in Bi-Di transformation
        private int MirrorRectangle(int x, int width, Rectangle rect, bool alignToRight)
        {
            if (alignToRight)
            {
                return rect.Right + rect.X - width - x;
            }
            else
            {
                return x;
            }
        }

        [ComVisible(true)]
        protected class DataGridRelationshipRowAccessibleObject : DataGridRowAccessibleObject
        {
            public DataGridRelationshipRowAccessibleObject(DataGridRow owner) : base(owner)
            {
            }

            protected override void AddChildAccessibleObjects(IList children)
            {
                base.AddChildAccessibleObjects(children);
                DataGridRelationshipRow row = (DataGridRelationshipRow)Owner;
                if (row.dgTable.RelationsList != null)
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

            public override string DefaultAction
            {
                get
                {
                    if (RelationshipRow.dgTable.RelationsList.Count > 0)
                    {
                        if (RelationshipRow.Expanded)
                        {
                            return SR.AccDGCollapse;
                        }
                        else
                        {
                            return SR.AccDGExpand;
                        }
                    }
                    return null;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;
                    if (RelationshipRow.dgTable.RelationsList.Count > 0)
                    {
                        if (((DataGridRelationshipRow)Owner).Expanded)
                        {
                            state |= AccessibleStates.Expanded;
                        }
                        else
                        {
                            state |= AccessibleStates.Collapsed;
                        }
                    }
                    return state;
                }
            }

            public override void DoDefaultAction()
            {
                if (RelationshipRow.dgTable.RelationsList.Count > 0)
                {
                    ((DataGridRelationshipRow)Owner).Expanded = !((DataGridRelationshipRow)Owner).Expanded;
                }
            }

            public override AccessibleObject GetFocused()
            {
                DataGridRelationshipRow row = (DataGridRelationshipRow)Owner;
                int focusRel = row.dgTable.FocusedRelation;
                if (focusRel == -1)
                {
                    return base.GetFocused();
                }
                else
                {
                    return GetChild(GetChildCount() - row.dgTable.RelationsList.Count + focusRel);
                }
            }
        }

        [ComVisible(true)]
        protected class DataGridRelationshipAccessibleObject : AccessibleObject
        {
            readonly DataGridRelationshipRow owner = null;
            readonly int relationship;

            public DataGridRelationshipAccessibleObject(DataGridRelationshipRow owner, int relationship) : base()
            {
                Debug.Assert(owner != null, "DataGridRelationshipAccessibleObject must have a valid owner DataGridRow");
                this.owner = owner;
                this.relationship = relationship;
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle rowBounds = DataGrid.GetRowBounds(owner);

                    Rectangle bounds = owner.Expanded ? owner.GetRelationshipRectWithMirroring() : Rectangle.Empty;
                    bounds.Y += owner.dgTable.RelationshipHeight * relationship;
                    bounds.Height = owner.Expanded ? owner.dgTable.RelationshipHeight : 0;      // when the row is collapsed the height of the relationship object should be 0
                    // GetRelationshipRectWithMirroring will use the row headers width
                    if (!owner.Expanded)
                    {
                        bounds.X += rowBounds.X;
                    }

                    bounds.Y += rowBounds.Y;

                    return owner.DataGrid.RectangleToScreen(bounds);
                }
            }

            public override string Name
            {
                get
                {
                    return (string)owner.dgTable.RelationsList[relationship];
                }
            }

            protected DataGridRelationshipRow Owner
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

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Link;
                }
            }

            public override AccessibleStates State
            {
                get
                {

                    DataGridRow[] dgRows = DataGrid.DataGridRows;
                    if (Array.IndexOf(dgRows, owner) == -1)
                    {
                        return AccessibleStates.Unavailable;
                    }

                    AccessibleStates state = AccessibleStates.Selectable
                        | AccessibleStates.Focusable
                        | AccessibleStates.Linked;

                    if (!owner.Expanded)
                    {
                        state |= AccessibleStates.Invisible;
                    }

                    if (DataGrid.Focused && Owner.dgTable.FocusedRelation == relationship)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    DataGridRow[] dgRows = DataGrid.DataGridRows;
                    if (Array.IndexOf(dgRows, owner) == -1)
                    {
                        return null;
                    }
                    else
                    {
                        return (string)owner.dgTable.RelationsList[relationship];
                    }
                }
                set
                {
                    // not supported
                }
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.AccDGNavigate;
                }
            }

            public override void DoDefaultAction()
            {
                ((DataGridRelationshipRow)Owner).Expanded = true;
                owner.FocusedRelation = -1;
                DataGrid.NavigateTo((string)owner.dgTable.RelationsList[relationship], owner, true);
                DataGrid.BeginInvoke(new MethodInvoker(ResetAccessibilityLayer));
            }

            private void ResetAccessibilityLayer()
            {
                ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Reorder, 0);
                ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Focus, DataGrid.CurrentCellAccIndex);
                ((DataGrid.DataGridAccessibleObject)DataGrid.AccessibilityObject).NotifyClients(AccessibleEvents.Selection, DataGrid.CurrentCellAccIndex);
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
                        if (relationship + 1 < owner.dgTable.RelationsList.Count)
                        {
                            return Parent.GetChild(Parent.GetChildCount() - owner.dgTable.RelationsList.Count + relationship + 1);
                        }
                        break;
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        if (relationship > 0)
                        {
                            return Parent.GetChild(Parent.GetChildCount() - owner.dgTable.RelationsList.Count + relationship - 1);
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

                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    Owner.FocusedRelation = relationship;
                }
            }

        }

    }
}
