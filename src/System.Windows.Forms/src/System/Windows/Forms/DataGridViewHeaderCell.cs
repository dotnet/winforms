// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a cell in the dataGridView.
    /// </summary>
    public class DataGridViewHeaderCell : DataGridViewCell
    {
        private const byte DATAGRIDVIEWHEADERCELL_themeMargin = 100; // Used to calculate the margins required for theming rendering

        private static readonly Type defaultFormattedValueType = typeof(string);
        private static readonly Type defaultValueType = typeof(object);
        private static readonly Type cellType = typeof(DataGridViewHeaderCell);
        private static Rectangle rectThemeMargins = new Rectangle(-1, -1, 0, 0);
        private static readonly int PropValueType = PropertyStore.CreateKey();
        private static readonly int PropButtonState = PropertyStore.CreateKey();
        private static readonly int PropFlipXPThemesBitmap = PropertyStore.CreateKey();
        private const string AEROTHEMEFILENAME = "Aero.msstyles";

        public DataGridViewHeaderCell()
        {
        }

        protected ButtonState ButtonState
        {
            get
            {
                int buttonState = Properties.GetInteger(PropButtonState, out bool found);
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
                    Properties.SetInteger(PropButtonState, (int)value);
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

        [
            Browsable(false)
        ]
        public override bool Displayed
        {
            get
            {
                if (DataGridView == null || !DataGridView.Visible)
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
                Debug.Assert(!DataGridView.LayoutInfo.dirty);
                return DataGridView.LayoutInfo.TopLeftHeader != Rectangle.Empty;
            }
        }

        internal Bitmap FlipXPThemesBitmap
        {
            get
            {
                return (Bitmap)Properties.GetObject(PropFlipXPThemesBitmap);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropFlipXPThemesBitmap))
                {
                    Properties.SetObject(PropFlipXPThemesBitmap, value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return defaultFormattedValueType;
            }
        }

        [
            Browsable(false)
        ]
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
            get => Properties.ContainsObject(PropValueType) && Properties.GetObject(PropValueType) != null;
        }

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

        [
            Browsable(false)
        ]
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

        public override Type ValueType
        {
            get
            {
                Type valueType = (Type)Properties.GetObject(PropValueType);
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropValueType))
                {
                    Properties.SetObject(PropValueType, value);
                }
            }
        }

        [
            Browsable(false)
        ]
        public override bool Visible
        {
            get
            {
                if (OwningRow != null)
                {
                    // row header cell
                    return OwningRow.Visible &&
                            (DataGridView == null || DataGridView.RowHeadersVisible);
                }

                if (OwningColumn != null)
                {
                    // column header cell
                    return OwningColumn.Visible &&
                            (DataGridView == null || DataGridView.ColumnHeadersVisible);
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
                if ((DataGridView == null && rowIndex != -1) ||
                    (DataGridView != null && (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)))
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
                }
                if (DataGridView != null && DataGridView.Rows.SharedRow(rowIndex) != OwningRow)
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
                }
                state |= (OwningRow.GetState(rowIndex) & DataGridViewElementStates.Frozen);
                if (OwningRow.GetResizable(rowIndex) == DataGridViewTriState.True || (DataGridView != null && DataGridView.RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing))
                {
                    state |= DataGridViewElementStates.Resizable;
                }
                if (OwningRow.GetVisible(rowIndex) && (DataGridView == null || DataGridView.RowHeadersVisible))
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
                if (OwningColumn.Visible && (DataGridView == null || DataGridView.ColumnHeadersVisible))
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
            if (OwningRow == null || OwningRow.Index != -1)
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
            if (DataGridView == null)
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
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(rowIndex), rowIndex));
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
            if (rectThemeMargins.X == -1)
            {
                Rectangle rectCell = new Rectangle(0, 0, DATAGRIDVIEWHEADERCELL_themeMargin, DATAGRIDVIEWHEADERCELL_themeMargin);
                Rectangle rectContent = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetBackgroundContentRectangle(g, rectCell);
                rectThemeMargins.X = rectContent.X;
                rectThemeMargins.Y = rectContent.Y;
                rectThemeMargins.Width = DATAGRIDVIEWHEADERCELL_themeMargin - rectContent.Right;
                rectThemeMargins.Height = DATAGRIDVIEWHEADERCELL_themeMargin - rectContent.Bottom;
                // On older platforms, the theming margins for a header are unexpectedly (3, 0, 0, 0) when you'd expect something like (0, 0, 2, 3)
                if (rectThemeMargins.X == 3 &&
                    rectThemeMargins.Y + rectThemeMargins.Width + rectThemeMargins.Height == 0)
                {
                    rectThemeMargins = new Rectangle(0, 0, 2, 3);
                }
                else
                {
                    // On some platforms, the theming margins for a header are unexpectedly (0, 0, 0, 0) when you'd expect something like (2, 1, 0, 2)
                    // Padding themePadding = DataGridViewHeaderCellRenderer.VisualStyleRenderer.GetMargins(g, MarginProperty.ContentMargins); /* or MarginProperty.SizingMargins */
                    // does not work either at this time. It AVs -So we hard code the margins for now.
                    try
                    {
                        string themeFilename = System.IO.Path.GetFileName(System.Windows.Forms.VisualStyles.VisualStyleInformation.ThemeFilename);
                        if (string.Equals(themeFilename, AEROTHEMEFILENAME, StringComparison.OrdinalIgnoreCase))
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

        protected override object GetValue(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            return Properties.GetObject(PropCellValue);
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && DataGridView.ApplyVisualStylesToHeaderCells)
            {
                UpdateButtonState(ButtonState.Normal, e.RowIndex);
            }
        }

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
                SolidBrush br = DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                if (br.Color.A == 255)
                {
                    graphics.FillRectangle(br, valBounds);
                }
            }
        }

        /// <summary>
        ///  Gets the row Index and column Index of the cell.
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewHeaderCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

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
