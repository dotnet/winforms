// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell"]/*' />
    /// <devdoc>
    ///    <para>Identifies a cell in the dataGridView.</para>
    /// </devdoc>
    /// 
    public class DataGridViewHeaderCell : DataGridViewCell
    {
        private const byte DATAGRIDVIEWHEADERCELL_themeMargin = 100;  // used to calculate the margins required for XP theming rendering

        private static Type defaultFormattedValueType = typeof(System.String);
        private static Type defaultValueType = typeof(System.Object);
        private static Type cellType = typeof(DataGridViewHeaderCell);
        private static Rectangle rectThemeMargins = new Rectangle(-1, -1, 0, 0);
        private static readonly int PropValueType = PropertyStore.CreateKey();
        private static readonly int PropButtonState = PropertyStore.CreateKey();
        private static readonly int PropFlipXPThemesBitmap = PropertyStore.CreateKey();
        private const string AEROTHEMEFILENAME = "Aero.msstyles";

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.DataGridViewHeaderCell"]/*' />
        public DataGridViewHeaderCell()
        {
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.ButtonState"]/*' />
        protected ButtonState ButtonState
        {
            get
            {
                bool found;
                int buttonState = this.Properties.GetInteger(PropButtonState, out found);
                if (found)
                {
                    return (ButtonState) buttonState;
                }
                return ButtonState.Normal;
            }
        }

        private ButtonState ButtonStatePrivate
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible") // Enum.IsDefined is OK here. Only specific flag combinations are allowed, and it's debug only anyways.
            ]
            set
            {
                Debug.Assert(Enum.IsDefined(typeof(ButtonState), value));
                if (this.ButtonState != value)
                {
                    this.Properties.SetInteger(PropButtonState, (int) value);
                }
            }
        }

        protected override void Dispose(bool disposing) {
             if (FlipXPThemesBitmap != null && disposing) {
                  FlipXPThemesBitmap.Dispose();
             }
             base.Dispose(disposing);
                             
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Displayed"]/*' />
        [
            Browsable(false)
        ]
        public override bool Displayed
        {
            get
            {
                if (this.DataGridView == null || !this.DataGridView.Visible)
                {
                    // No detached or invisible element is displayed.
                    return false;
                }

                if (this.OwningRow != null)
                {
                    // row header cell
                    return this.DataGridView.RowHeadersVisible && this.OwningRow.Displayed;
                }

                if (this.OwningColumn != null)
                {
                    // column header cell
                    return this.DataGridView.ColumnHeadersVisible && this.OwningColumn.Displayed;
                }

                // top left header cell
                Debug.Assert(!this.DataGridView.LayoutInfo.dirty);
                return this.DataGridView.LayoutInfo.TopLeftHeader != Rectangle.Empty;
            }
        }

        internal Bitmap FlipXPThemesBitmap
        {
            get
            {
                return (Bitmap)this.Properties.GetObject(PropFlipXPThemesBitmap);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropFlipXPThemesBitmap))
                {
                    this.Properties.SetObject(PropFlipXPThemesBitmap, value);
                }
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.FormattedValueType"]/*' />
        public override Type FormattedValueType
        {
            get
            {
                return defaultFormattedValueType;
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Frozen"]/*' />
        [
            Browsable(false)
        ]
        public override bool Frozen
        {
            get
            {
                if (this.OwningRow != null)
                {
                    // row header cell
                    return this.OwningRow.Frozen;
                }

                if (this.OwningColumn != null)
                {
                    // column header cell
                    return this.OwningColumn.Frozen;
                }

                if (this.DataGridView != null)
                {
                    // top left header cell
                    return true;
                }

                // detached header cell
                return false;
            }
        }

        internal override bool HasValueType
        {
            get
            {
                return this.Properties.ContainsObject(PropValueType) && this.Properties.GetObject(PropValueType) != null;
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.ReadOnly"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool ReadOnly
        {
            get
            {
                return true;
            }
            set
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_HeaderCellReadOnlyProperty, "ReadOnly"));
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Resizable"]/*' />
        [
            Browsable(false)
        ]
        public override bool Resizable
        {
            get
            {
                if (this.OwningRow != null)
                {
                    // must be a row header cell
                    return (this.OwningRow.Resizable == DataGridViewTriState.True) || (this.DataGridView != null && this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing);
                }

                if (this.OwningColumn != null)
                {
                    // must be a column header cell
                    return (this.OwningColumn.Resizable == DataGridViewTriState.True) ||
                           (this.DataGridView != null && this.DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing);
                }

                // must be the top left header cell
                return this.DataGridView != null && (this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || this.DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing);
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Selected"]/*' />
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool Selected
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_HeaderCellReadOnlyProperty, "Selected"));
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.ValueType"]/*' />
        public override Type ValueType
        {
            get
            {
                Type valueType = (Type) this.Properties.GetObject(PropValueType);
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropValueType))
                {
                    this.Properties.SetObject(PropValueType, value);
                }
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Visible"]/*' />
        [
            Browsable(false)
        ]
        public override bool Visible
        {
            get
            {
                if (this.OwningRow != null)
                {
                    // row header cell
                    return this.OwningRow.Visible &&
                            (this.DataGridView == null || this.DataGridView.RowHeadersVisible);
                }

                if (this.OwningColumn != null)
                {
                    // column header cell
                    return this.OwningColumn.Visible && 
                            (this.DataGridView == null || this.DataGridView.ColumnHeadersVisible);
                }

                if (this.DataGridView != null)
                {
                    // top left header cell
                    return this.DataGridView.RowHeadersVisible && this.DataGridView.ColumnHeadersVisible;
                }

                return false;
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewHeaderCell dataGridViewCell;
            Type thisType = this.GetType();
            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewHeaderCell();
            }
            else
            {
                // 

                dataGridViewCell = (DataGridViewHeaderCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = this.Value;
            return dataGridViewCell;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.GetInheritedContextMenuStrip"]/*' />
        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }

            if (this.DataGridView != null)
            {
                return this.DataGridView.ContextMenuStrip;
            }
            else
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.GetInheritedState"]/*' />
        public override DataGridViewElementStates GetInheritedState(int rowIndex)
        {
            DataGridViewElementStates state = DataGridViewElementStates.ResizableSet | DataGridViewElementStates.ReadOnly;

            if (this.OwningRow != null)
            {
                // row header cell
                if ((this.DataGridView == null && rowIndex != -1) || 
                    (this.DataGridView != null && (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)))
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
                }
                if (this.DataGridView != null && this.DataGridView.Rows.SharedRow(rowIndex) != this.OwningRow)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
                }
                state |= (this.OwningRow.GetState(rowIndex) & DataGridViewElementStates.Frozen);
                if (this.OwningRow.GetResizable(rowIndex) == DataGridViewTriState.True || (this.DataGridView != null && this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing))
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (this.OwningRow.GetVisible(rowIndex) && (this.DataGridView == null || this.DataGridView.RowHeadersVisible))
                {
                    state |= DataGridViewElementStates.Visible;
                    if (this.OwningRow.GetDisplayed(rowIndex))
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }
            else if (this.OwningColumn != null)
            {
                // column header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                state |= (this.OwningColumn.State & DataGridViewElementStates.Frozen);
                if (this.OwningColumn.Resizable == DataGridViewTriState.True ||
                    (this.DataGridView != null && this.DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing))
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (this.OwningColumn.Visible && (this.DataGridView == null || this.DataGridView.ColumnHeadersVisible))
                {
                    state |= DataGridViewElementStates.Visible;
                    if (this.OwningColumn.Displayed)
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }
            else if (this.DataGridView != null)
            {
                // top left header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                state |= DataGridViewElementStates.Frozen;
                if (this.DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || this.DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing)
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (this.DataGridView.RowHeadersVisible && this.DataGridView.ColumnHeadersVisible)
                {
                    state |= DataGridViewElementStates.Visible;
                    if (this.DataGridView.LayoutInfo.TopLeftHeader != Rectangle.Empty)
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }

#if DEBUG
            if (this.OwningRow == null || this.OwningRow.Index != -1)
            {
                DataGridViewElementStates stateDebug = DataGridViewElementStates.ResizableSet;
                if (this.Displayed)
                {
                    stateDebug |= DataGridViewElementStates.Displayed;
                }
                if (this.Frozen)
                {
                    stateDebug |= DataGridViewElementStates.Frozen;
                }
                if (this.ReadOnly)
                {
                    stateDebug |= DataGridViewElementStates.ReadOnly;
                }
                if (this.Resizable)
                {
                    stateDebug |= DataGridViewElementStates.Resizable;
                }
                if (this.Selected)
                {
                    stateDebug |= DataGridViewElementStates.Selected;
                }
                if (this.Visible)
                {
                    stateDebug |= DataGridViewElementStates.Visible;
                }
                Debug.Assert(state == stateDebug);
            }
#endif

            return state;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.GetSize"]/*' />
        protected override Size GetSize(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                // detached cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(-1, -1);
            }
            if (this.OwningColumn != null)
            {
                // must be a column header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(this.OwningColumn.Thickness, this.DataGridView.ColumnHeadersHeight);
            }
            else if (this.OwningRow != null)
            {
                // must be a row header cell
                if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (this.DataGridView.Rows.SharedRow(rowIndex) != this.OwningRow)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "rowIndex", rowIndex.ToString(CultureInfo.CurrentCulture)));
                }
                return new Size(this.DataGridView.RowHeadersWidth, this.OwningRow.GetHeight(rowIndex));
            }
            else
            {
                // must be the top left header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(this.DataGridView.RowHeadersWidth, this.DataGridView.ColumnHeadersHeight);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static Rectangle GetThemeMargins(Graphics g)
        {
            if (rectThemeMargins.X == -1)
            {
                Rectangle rectCell = new Rectangle(0, 0, DATAGRIDVIEWHEADERCELL_themeMargin, DATAGRIDVIEWHEADERCELL_themeMargin);
                Rectangle rectContent = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetBackgroundContentRectangle(g, rectCell);
                rectThemeMargins.X = rectContent.X;
                rectThemeMargins.Y = rectContent.Y;
                rectThemeMargins.Width = DATAGRIDVIEWHEADERCELL_themeMargin - rectContent.Right;
                rectThemeMargins.Height = DATAGRIDVIEWHEADERCELL_themeMargin - rectContent.Bottom;
                // On WinXP, the theming margins for a header are unexpectedly (3, 0, 0, 0) when you'd expect something like (0, 0, 2, 3)
                if (rectThemeMargins.X == 3 &&
                    rectThemeMargins.Y + rectThemeMargins.Width + rectThemeMargins.Height == 0)
                {
                    rectThemeMargins = new Rectangle(0, 0, 2, 3);
                }
                else
                {
                    // On Vista, the theming margins for a header are unexpectedly (0, 0, 0, 0) when you'd expect something like (2, 1, 0, 2)
                    // Padding themePadding = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetMargins(g, MarginProperty.ContentMargins); /* or MarginProperty.SizingMargins */
                    // does not work either at this time. It AVs -So we hard code the margins for now.
                    try
                    {
                        string themeFilename = System.IO.Path.GetFileName(System.Windows.Forms.VisualStyles.VisualStyleInformation.ThemeFilename);
                        if (String.Equals(themeFilename, AEROTHEMEFILENAME, StringComparison.OrdinalIgnoreCase))
                        {
                            rectThemeMargins = new Rectangle(2, 1, 0, 2);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return rectThemeMargins;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            return this.Properties.GetObject(PropCellValue);
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.MouseDownUnsharesRow"]/*' />
        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left && this.DataGridView.ApplyVisualStylesToHeaderCells;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.MouseEnterUnsharesRow"]/*' />
        protected override bool MouseEnterUnsharesRow(int rowIndex)
        {
            return this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X &&
                   rowIndex == this.DataGridView.MouseDownCellAddress.Y &&
                   this.DataGridView.ApplyVisualStylesToHeaderCells;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.MouseLeaveUnsharesRow"]/*' />
        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return this.ButtonState != ButtonState.Normal && this.DataGridView.ApplyVisualStylesToHeaderCells;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.MouseUpUnsharesRow"]/*' />
        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left && this.DataGridView.ApplyVisualStylesToHeaderCells;
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.OnMouseDown"]/*' />
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && 
                this.DataGridView.ApplyVisualStylesToHeaderCells &&
                !this.DataGridView.ResizingOperationAboutToStart)
            {
                UpdateButtonState(ButtonState.Pushed, e.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.OnMouseEnter"]/*' />
        protected override void OnMouseEnter(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X &&
                    rowIndex == this.DataGridView.MouseDownCellAddress.Y &&
                    this.ButtonState == ButtonState.Normal &&
                    Control.MouseButtons == MouseButtons.Left &&
                    !this.DataGridView.ResizingOperationAboutToStart)
                {
                    UpdateButtonState(ButtonState.Pushed, rowIndex);
                }
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.OnMouseLeave"]/*' />
        protected override void OnMouseLeave(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (this.ButtonState != ButtonState.Normal)
                {
                    Debug.Assert(this.ButtonState == ButtonState.Pushed);
                    Debug.Assert(this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X);
                    Debug.Assert(rowIndex == this.DataGridView.MouseDownCellAddress.Y);
                    UpdateButtonState(ButtonState.Normal, rowIndex);
                }
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.OnMouseUp"]/*' />
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                UpdateButtonState(ButtonState.Normal, e.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.Paint"]/*' />
        protected override void Paint(Graphics graphics, 
                                      Rectangle clipBounds,
                                      Rectangle cellBounds, 
                                      int rowIndex, 
                                      DataGridViewElementStates dataGridViewElementState, 
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            if (DataGridViewCell.PaintBackground(paintParts))
            {
                Rectangle valBounds = cellBounds;
                Rectangle borderWidths = BorderWidths(advancedBorderStyle);

                valBounds.Offset(borderWidths.X, borderWidths.Y);
                valBounds.Width -= borderWidths.Right;
                valBounds.Height -= borderWidths.Bottom;

                bool cellSelected = (dataGridViewElementState & DataGridViewElementStates.Selected) != 0;
                SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                if (br.Color.A == 255)
                {
                    graphics.FillRectangle(br, valBounds);
                }
            }
        }

        /// <include file='doc\DataGridViewHeaderCell.uex' path='docs/doc[@for="DataGridViewHeaderCell.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the row Index and column Index of the cell.
        ///    </para>
        /// </devdoc>
        public override string ToString()
        {
            return "DataGridViewHeaderCell { ColumnIndex=" + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + this.RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            Debug.Assert(this.DataGridView != null);
            this.ButtonStatePrivate = newButtonState;
            this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
        }

        private class DataGridViewHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewHeaderCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
                    }

                    return visualStyleRenderer;
                }
            }
        }
    }
}
