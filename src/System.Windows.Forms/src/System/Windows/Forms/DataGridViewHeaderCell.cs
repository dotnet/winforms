// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a cell in the dataGridView.
    /// </summary>
    public class DataGridViewHeaderCell : DataGridViewCell
    {
        private const byte ThemeMargin = 100; // Used to calculate the margins required for theming rendering

        private static readonly Type s_defaultFormattedValueType = typeof(string);
        private static readonly Type s_defaultValueType = typeof(object);
        private static readonly Type s_cellType = typeof(DataGridViewHeaderCell);
        private static Rectangle s_rectThemeMargins = new Rectangle(-1, -1, 0, 0);
        private static readonly int s_propValueType = PropertyStore.CreateKey();
        private static readonly int s_propButtonState = PropertyStore.CreateKey();
        private static readonly int s_propFlipXPThemesBitmap = PropertyStore.CreateKey();
        private const string AeroThemeFileName = "Aero.msstyles";

        public DataGridViewHeaderCell()
        {
        }

        protected ButtonState ButtonState
        {
            get
            {
                int buttonState = Properties.GetInteger(s_propButtonState, out bool found);
                if (found)
                {
                    return (ButtonState)buttonState;
                }
                return ButtonState.Normal;
            }
        }

        private ButtonState ButtonStatePrivate
        {
            set
            {
                Debug.Assert(Enum.IsDefined(typeof(ButtonState), value));
                if (ButtonState != value)
                {
                    Properties.SetInteger(s_propButtonState, (int)value);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (FlipXPThemesBitmap != null && disposing)
            {
                FlipXPThemesBitmap.Dispose();
            }
            base.Dispose(disposing);
        }

        [Browsable(false)]
        public override bool Displayed
        {
            get
            {
                if (DataGridView is null || !DataGridView.Visible)
                {
                    // No detached or invisible element is displayed.
                    return false;
                }

                if (OwningRow != null)
                {
                    // row header cell
                    return DataGridView.RowHeadersVisible && OwningRow.Displayed;
                }

                if (OwningColumn != null)
                {
                    // column header cell
                    return DataGridView.ColumnHeadersVisible && OwningColumn.Displayed;
                }

                // top left header cell
                return DataGridView.LayoutInfo.TopLeftHeader != Rectangle.Empty;
            }
        }

        internal Bitmap FlipXPThemesBitmap
        {
            get
            {
                return (Bitmap)Properties.GetObject(s_propFlipXPThemesBitmap);
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propFlipXPThemesBitmap))
                {
                    Properties.SetObject(s_propFlipXPThemesBitmap, value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return s_defaultFormattedValueType;
            }
        }

        [Browsable(false)]
        public override bool Frozen
        {
            get
            {
                if (OwningRow != null)
                {
                    // row header cell
                    return OwningRow.Frozen;
                }

                if (OwningColumn != null)
                {
                    // column header cell
                    return OwningColumn.Frozen;
                }

                if (DataGridView != null)
                {
                    // top left header cell
                    return true;
                }

                // detached header cell
                return false;
            }
        }

        private protected override bool HasValueType
        {
            get => Properties.ContainsObject(s_propValueType) && Properties.GetObject(s_propValueType) != null;
        }

        [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [Browsable(false)]
        public override bool Resizable
        {
            get
            {
                if (OwningRow != null)
                {
                    // must be a row header cell
                    return (OwningRow.Resizable == DataGridViewTriState.True) || (DataGridView != null && DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing);
                }

                if (OwningColumn != null)
                {
                    // must be a column header cell
                    return (OwningColumn.Resizable == DataGridViewTriState.True) ||
                           (DataGridView != null && DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing);
                }

                // must be the top left header cell
                return DataGridView != null && (DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing);
            }
        }

        [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        public override Type ValueType
        {
            get
            {
                Type valueType = (Type)Properties.GetObject(s_propValueType);
                if (valueType != null)
                {
                    return valueType;
                }
                return s_defaultValueType;
            }
            set
            {
                if (value != null || Properties.ContainsObject(s_propValueType))
                {
                    Properties.SetObject(s_propValueType, value);
                }
            }
        }

        [Browsable(false)]
        public override bool Visible
        {
            get
            {
                if (OwningRow != null)
                {
                    // row header cell
                    return OwningRow.Visible &&
                            (DataGridView is null || DataGridView.RowHeadersVisible);
                }

                if (OwningColumn != null)
                {
                    // column header cell
                    return OwningColumn.Visible &&
                            (DataGridView is null || DataGridView.ColumnHeadersVisible);
                }

                if (DataGridView != null)
                {
                    // top left header cell
                    return DataGridView.RowHeadersVisible && DataGridView.ColumnHeadersVisible;
                }

                return false;
            }
        }

        public override object Clone()
        {
            DataGridViewHeaderCell dataGridViewCell;
            Type thisType = GetType();
            if (thisType == s_cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewHeaderCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewHeaderCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = Value;
            return dataGridViewCell;
        }

        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }

            if (DataGridView != null)
            {
                return DataGridView.ContextMenuStrip;
            }
            else
            {
                return null;
            }
        }

        public override DataGridViewElementStates GetInheritedState(int rowIndex)
        {
            DataGridViewElementStates state = DataGridViewElementStates.ResizableSet | DataGridViewElementStates.ReadOnly;

            if (OwningRow != null)
            {
                // row header cell
                if ((DataGridView is null && rowIndex != -1) ||
                    (DataGridView != null && (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)))
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex), nameof(rowIndex));
                }
                if (DataGridView != null && DataGridView.Rows.SharedRow(rowIndex) != OwningRow)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex), nameof(rowIndex));
                }
                state |= (OwningRow.GetState(rowIndex) & DataGridViewElementStates.Frozen);
                if (OwningRow.GetResizable(rowIndex) == DataGridViewTriState.True || (DataGridView != null && DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing))
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (OwningRow.GetVisible(rowIndex) && (DataGridView is null || DataGridView.RowHeadersVisible))
                {
                    state |= DataGridViewElementStates.Visible;
                    if (OwningRow.GetDisplayed(rowIndex))
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }
            else if (OwningColumn != null)
            {
                // column header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                state |= (OwningColumn.State & DataGridViewElementStates.Frozen);
                if (OwningColumn.Resizable == DataGridViewTriState.True ||
                    (DataGridView != null && DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing))
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (OwningColumn.Visible && (DataGridView is null || DataGridView.ColumnHeadersVisible))
                {
                    state |= DataGridViewElementStates.Visible;
                    if (OwningColumn.Displayed)
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }
            else if (DataGridView != null)
            {
                // top left header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                state |= DataGridViewElementStates.Frozen;
                if (DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || DataGridView.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing)
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (DataGridView.RowHeadersVisible && DataGridView.ColumnHeadersVisible)
                {
                    state |= DataGridViewElementStates.Visible;
                    if (DataGridView.LayoutInfo.TopLeftHeader != Rectangle.Empty)
                    {
                        state |= DataGridViewElementStates.Displayed;
                    }
                }
            }

#if DEBUG
            if (OwningRow is null || OwningRow.Index != -1)
            {
                DataGridViewElementStates stateDebug = DataGridViewElementStates.ResizableSet;
                if (Displayed)
                {
                    stateDebug |= DataGridViewElementStates.Displayed;
                }
                if (Frozen)
                {
                    stateDebug |= DataGridViewElementStates.Frozen;
                }
                if (ReadOnly)
                {
                    stateDebug |= DataGridViewElementStates.ReadOnly;
                }
                if (Resizable)
                {
                    stateDebug |= DataGridViewElementStates.Resizable;
                }
                if (Selected)
                {
                    stateDebug |= DataGridViewElementStates.Selected;
                }
                if (Visible)
                {
                    stateDebug |= DataGridViewElementStates.Visible;
                }
                Debug.Assert(state == stateDebug);
            }
#endif

            return state;
        }

        protected override Size GetSize(int rowIndex)
        {
            if (DataGridView is null)
            {
                // detached cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(-1, -1);
            }
            if (OwningColumn != null)
            {
                // must be a column header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(OwningColumn.Thickness, DataGridView.ColumnHeadersHeight);
            }
            else if (OwningRow != null)
            {
                // must be a row header cell
                if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (DataGridView.Rows.SharedRow(rowIndex) != OwningRow)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex), nameof(rowIndex));
                }
                return new Size(DataGridView.RowHeadersWidth, OwningRow.GetHeight(rowIndex));
            }
            else
            {
                // must be the top left header cell
                if (rowIndex != -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                return new Size(DataGridView.RowHeadersWidth, DataGridView.ColumnHeadersHeight);
            }
        }

        internal static Rectangle GetThemeMargins(Graphics g)
        {
            if (s_rectThemeMargins.X == -1)
            {
                Rectangle rectCell = new Rectangle(0, 0, ThemeMargin, ThemeMargin);
                Rectangle rectContent = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetBackgroundContentRectangle(g, rectCell);
                s_rectThemeMargins.X = rectContent.X;
                s_rectThemeMargins.Y = rectContent.Y;
                s_rectThemeMargins.Width = ThemeMargin - rectContent.Right;
                s_rectThemeMargins.Height = ThemeMargin - rectContent.Bottom;
                // On older platforms, the theming margins for a header are unexpectedly (3, 0, 0, 0) when you'd expect something like (0, 0, 2, 3)
                if (s_rectThemeMargins.X == 3 &&
                    s_rectThemeMargins.Y + s_rectThemeMargins.Width + s_rectThemeMargins.Height == 0)
                {
                    s_rectThemeMargins = new Rectangle(0, 0, 2, 3);
                }
                else
                {
                    // On some platforms, the theming margins for a header are unexpectedly (0, 0, 0, 0) when you'd expect something like (2, 1, 0, 2)
                    // Padding themePadding = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetMargins(g, MarginProperty.ContentMargins); /* or MarginProperty.SizingMargins */
                    // does not work either at this time. It AVs -So we hard code the margins for now.
                    try
                    {
                        string themeFilename = System.IO.Path.GetFileName(System.Windows.Forms.VisualStyles.VisualStyleInformation.ThemeFilename);
                        if (string.Equals(themeFilename, AeroThemeFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            s_rectThemeMargins = new Rectangle(2, 1, 0, 2);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return s_rectThemeMargins;
        }

        protected override object GetValue(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            return Properties.GetObject(s_propCellValue);
        }

        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left && DataGridView.ApplyVisualStylesToHeaderCells;
        }

        protected override bool MouseEnterUnsharesRow(int rowIndex)
        {
            return ColumnIndex == DataGridView.MouseDownCellAddress.X &&
                   rowIndex == DataGridView.MouseDownCellAddress.Y &&
                   DataGridView.ApplyVisualStylesToHeaderCells;
        }

        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return ButtonState != ButtonState.Normal && DataGridView.ApplyVisualStylesToHeaderCells;
        }

        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left && DataGridView.ApplyVisualStylesToHeaderCells;
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left &&
                DataGridView.ApplyVisualStylesToHeaderCells &&
                !DataGridView.ResizingOperationAboutToStart)
            {
                UpdateButtonState(ButtonState.Pushed, e.RowIndex);
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (ColumnIndex == DataGridView.MouseDownCellAddress.X &&
                    rowIndex == DataGridView.MouseDownCellAddress.Y &&
                    ButtonState == ButtonState.Normal &&
                    Control.MouseButtons == MouseButtons.Left &&
                    !DataGridView.ResizingOperationAboutToStart)
                {
                    UpdateButtonState(ButtonState.Pushed, rowIndex);
                }
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (ButtonState != ButtonState.Normal)
                {
                    Debug.Assert(ButtonState == ButtonState.Pushed);
                    Debug.Assert(ColumnIndex == DataGridView.MouseDownCellAddress.X);
                    Debug.Assert(rowIndex == DataGridView.MouseDownCellAddress.Y);
                    UpdateButtonState(ButtonState.Normal, rowIndex);
                }
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && DataGridView.ApplyVisualStylesToHeaderCells)
            {
                UpdateButtonState(ButtonState.Normal, e.RowIndex);
            }
        }

        protected override void Paint(
            Graphics graphics,
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
            if (cellStyle is null)
                throw new ArgumentNullException(nameof(cellStyle));

            if (PaintBorder(paintParts))
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            if (PaintBackground(paintParts))
            {
                Rectangle bounds = cellBounds;
                Rectangle borderWidths = BorderWidths(advancedBorderStyle);

                bounds.Offset(borderWidths.X, borderWidths.Y);
                bounds.Width -= borderWidths.Right;
                bounds.Height -= borderWidths.Bottom;

                bool cellSelected = (dataGridViewElementState & DataGridViewElementStates.Selected) != 0;
                Color backColor = PaintSelectionBackground(paintParts) && cellSelected
                    ? cellStyle.SelectionBackColor : cellStyle.BackColor;

                if (!backColor.HasTransparency())
                {
                    using var brush = backColor.GetCachedSolidBrushScope();
                    graphics.FillRectangle(brush, bounds);
                }
            }
        }

        /// <summary>
        ///  Gets the row Index and column Index of the cell.
        /// </summary>
        public override string ToString()
            => $"DataGridViewHeaderCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            Debug.Assert(DataGridView != null);
            ButtonStatePrivate = newButtonState;
            DataGridView.InvalidateCell(ColumnIndex, rowIndex);
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
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
                    }

                    return visualStyleRenderer;
                }
            }
        }
    }
}
