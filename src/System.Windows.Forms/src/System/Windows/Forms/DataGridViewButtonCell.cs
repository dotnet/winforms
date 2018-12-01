// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.VisualStyles;
    using System.Security.Permissions;
    using System.Windows.Forms.ButtonInternal;
    using System.Globalization;

    /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell"]/*' />
    /// <devdoc>
    ///    <para>Identifies a button cell in the dataGridView.</para>
    /// </devdoc>
    public class DataGridViewButtonCell : DataGridViewCell
    {
        private static readonly int PropButtonCellFlatStyle = PropertyStore.CreateKey();
        private static readonly int PropButtonCellState = PropertyStore.CreateKey();
        private static readonly int PropButtonCellUseColumnTextForButtonValue = PropertyStore.CreateKey();
        private static readonly VisualStyleElement ButtonElement = VisualStyleElement.Button.PushButton.Normal;

        private const byte DATAGRIDVIEWBUTTONCELL_themeMargin = 100;  // used to calculate the margins required for XP theming rendering
        private const byte DATAGRIDVIEWBUTTONCELL_horizontalTextMargin = 2;
        private const byte DATAGRIDVIEWBUTTONCELL_verticalTextMargin = 1;
        private const byte DATAGRIDVIEWBUTTONCELL_textPadding = 5;

        private static Rectangle rectThemeMargins = new Rectangle(-1, -1, 0, 0);
        private static bool mouseInContentBounds = false;

        private static Type defaultFormattedValueType = typeof(System.String);
        private static Type defaultValueType = typeof(System.Object);
        private static Type cellType = typeof(DataGridViewButtonCell);

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.DataGridViewButtonCell"]/*' />
        public DataGridViewButtonCell()
        {
        }

        private ButtonState ButtonState
        {
            get
            {
                bool found;
                int buttonState = this.Properties.GetInteger(PropButtonCellState, out found);
                if (found)
                {
                    return (ButtonState)buttonState;
                }
                return ButtonState.Normal;
            }
            set
            {
                // ButtonState.Pushed is used for mouse interaction
                // ButtonState.Checked is used for keyboard interaction
                Debug.Assert((value & ~(ButtonState.Normal | ButtonState.Pushed | ButtonState.Checked)) == 0);
                if (this.ButtonState != value)
                {
                    this.Properties.SetInteger(PropButtonCellState, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.EditType"]/*' />
        public override Type EditType
        {
            get
            {
                // Buttons can't switch to edit mode
                return null;
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.FlatStyle"]/*' />
        [
            DefaultValue(FlatStyle.Standard)
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                bool found;
                int flatStyle = this.Properties.GetInteger(PropButtonCellFlatStyle, out found);
                if (found)
                {
                    return (FlatStyle)flatStyle;
                }
                return FlatStyle.Standard;
            }
            set
            {
               // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System))
                {
                     throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle)); 
                }
                if (value != this.FlatStyle)
                {
                    this.Properties.SetInteger(PropButtonCellFlatStyle, (int)value);
                    OnCommonChange();
                }
            }
        }

        internal FlatStyle FlatStyleInternal
        {
            set
            {
                Debug.Assert(value >= FlatStyle.Flat && value <= FlatStyle.System);
                if (value != this.FlatStyle)
                {
                    this.Properties.SetInteger(PropButtonCellFlatStyle, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.FormattedValueType"]/*' />
        public override Type FormattedValueType
        {
            get 
            {
                // we return string for the formatted type
                return defaultFormattedValueType;
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.UseColumnTextForButtonValue"]/*' />
        [DefaultValue(false)]
        public bool UseColumnTextForButtonValue
        {
            get
            {
                bool found;
                int useColumnTextForButtonValue = this.Properties.GetInteger(PropButtonCellUseColumnTextForButtonValue, out found);
                if (found)
                {
                    return useColumnTextForButtonValue == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != this.UseColumnTextForButtonValue)
                {
                    this.Properties.SetInteger(PropButtonCellUseColumnTextForButtonValue, value ? 1 : 0);
                    OnCommonChange();
                }
            }
        }

        internal bool UseColumnTextForButtonValueInternal
        {
            set
            {
                if (value != this.UseColumnTextForButtonValue)
                {
                    this.Properties.SetInteger(PropButtonCellUseColumnTextForButtonValue, value ? 1 : 0);
                }
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.ValueType"]/*' />
        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewButtonCell dataGridViewCell;
            Type thisType = this.GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewButtonCell();
            }
            else
            {
                dataGridViewCell = (DataGridViewButtonCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.FlatStyleInternal = this.FlatStyle;
            dataGridViewCell.UseColumnTextForButtonValueInternal = this.UseColumnTextForButtonValue;
            return dataGridViewCell;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewButtonCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.GetContentBounds"]/*' />
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null || rowIndex < 0 || this.OwningColumn == null)
            {
                return Rectangle.Empty;
            }

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                null /*formattedValue*/,            // contentBounds is independent of formattedValue
                null /*errorText*/,                 // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            object value = GetValue(rowIndex);
            Rectangle contentBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting),
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

            return contentBounds;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.GetErrorIconBounds"]/*' />
        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null ||
                rowIndex < 0 ||
                this.OwningColumn == null ||
                !this.DataGridView.ShowCellErrors ||
                String.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle errorIconBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                null /*formattedValue*/,            // errorIconBounds is independent of formattedValue
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true  /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            object value = GetValue(rowIndex);
            Rectangle errorIconBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting),
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true  /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(errorIconBoundsDebug.Equals(errorIconBounds));
#endif

            return errorIconBounds;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.GetPreferredSize"]/*' />
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize;
            Rectangle borderWidthsRect = this.StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            int marginWidths, marginHeights;
            string formattedString = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
            if (string.IsNullOrEmpty(formattedString))
            {
                formattedString = " ";
            }
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            // Adding space for text padding.
            if (this.DataGridView.ApplyVisualStylesToInnerCells)
            {
                Rectangle rectThemeMargins = DataGridViewButtonCell.GetThemeMargins(graphics);
                marginWidths = rectThemeMargins.X + rectThemeMargins.Width;
                marginHeights = rectThemeMargins.Y + rectThemeMargins.Height;
            }
            else
            {
                // Hardcoding 5 for the button borders for now.
                marginWidths = marginHeights = DATAGRIDVIEWBUTTONCELL_textPadding;
            }

            switch (freeDimension)
            {
                case DataGridViewFreeDimension.Width:
                {
                    if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1 &&
                        constraintSize.Height - borderAndPaddingHeights - marginHeights - 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin > 0)
                    {
                        preferredSize = new Size(DataGridViewCell.MeasureTextWidth(graphics, 
                                                                                   formattedString,
                                                                                   cellStyle.Font,
                                                                                   constraintSize.Height - borderAndPaddingHeights - marginHeights - 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin,
                                                                                   flags), 
                                                 0);
                    }
                    else
                    {
                        preferredSize = new Size(DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Width, 
                                                 0);
                    }
                    break;
                }
                case DataGridViewFreeDimension.Height:
                {
                    if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1 &&
                        constraintSize.Width - borderAndPaddingWidths - marginWidths - 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin > 0)
                    {
                        preferredSize = new Size(0,
                                                 DataGridViewCell.MeasureTextHeight(graphics, 
                                                                                    formattedString,
                                                                                    cellStyle.Font,
                                                                                    constraintSize.Width - borderAndPaddingWidths - marginWidths - 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin,
                                                                                    flags));
                    }
                    else
                    {
                        preferredSize = new Size(0,
                                                 DataGridViewCell.MeasureTextSize(graphics, 
                                                                                  formattedString, 
                                                                                  cellStyle.Font, 
                                                                                  flags).Height);
                    }
                    break;
                }
                default:
                {
                    if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1)
                    {
                        preferredSize = DataGridViewCell.MeasureTextPreferredSize(graphics, formattedString, cellStyle.Font, 5.0F, flags);
                    }
                    else
                    {
                        preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags);
                    }
                    break;
                }
            }

            if (freeDimension != DataGridViewFreeDimension.Height)
            {
                preferredSize.Width += borderAndPaddingWidths + marginWidths + 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin;
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += borderAndPaddingHeights + marginHeights + 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin;
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        private static Rectangle GetThemeMargins(Graphics g)
        {
            if (rectThemeMargins.X == -1)
            {
                Rectangle rectCell = new Rectangle(0, 0, DATAGRIDVIEWBUTTONCELL_themeMargin, DATAGRIDVIEWBUTTONCELL_themeMargin);
                Rectangle rectContent = DataGridViewButtonCellRenderer.DataGridViewButtonRenderer.GetBackgroundContentRectangle(g, rectCell);
                rectThemeMargins.X = rectContent.X;
                rectThemeMargins.Y = rectContent.Y;
                rectThemeMargins.Width = DATAGRIDVIEWBUTTONCELL_themeMargin - rectContent.Right;
                rectThemeMargins.Height = DATAGRIDVIEWBUTTONCELL_themeMargin - rectContent.Bottom;
            }
            return rectThemeMargins;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            if (this.UseColumnTextForButtonValue &&
                this.DataGridView != null && 
                this.DataGridView.NewRowIndex != rowIndex && 
                this.OwningColumn != null && 
                this.OwningColumn is DataGridViewButtonColumn)
            {
                return ((DataGridViewButtonColumn) this.OwningColumn).Text;
            }
            return base.GetValue(rowIndex);
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.KeyDownUnsharesRow"]/*' />
        protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.KeyUpUnsharesRow"]/*' />
        protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.MouseDownUnsharesRow"]/*' />
        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.MouseEnterUnsharesRow"]/*' />
        protected override bool MouseEnterUnsharesRow(int rowIndex)
        {
            return this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X && rowIndex == this.DataGridView.MouseDownCellAddress.Y;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.MouseLeaveUnsharesRow"]/*' />
        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return (this.ButtonState & ButtonState.Pushed) != 0;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.MouseUpUnsharesRow"]/*' />
        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnKeyDown"]/*' />
        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                UpdateButtonState(this.ButtonState | ButtonState.Checked, rowIndex);
                e.Handled = true;
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnKeyUp"]/*' />
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space)
            {
                UpdateButtonState(this.ButtonState & ~ButtonState.Checked, rowIndex);
                if (!e.Alt && !e.Control && !e.Shift)
                {
                    RaiseCellClick(new DataGridViewCellEventArgs(this.ColumnIndex, rowIndex));
                    if (this.DataGridView != null &&
                        this.ColumnIndex < this.DataGridView.Columns.Count &&
                        rowIndex < this.DataGridView.Rows.Count)
                    {
                        RaiseCellContentClick(new DataGridViewCellEventArgs(this.ColumnIndex, rowIndex));
                    }
                    e.Handled = true;
                }
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnLeave"]/*' />
        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (this.ButtonState != ButtonState.Normal)
            {
                Debug.Assert(this.RowIndex >= 0); // Cell is not in a shared row.
                UpdateButtonState(ButtonState.Normal, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnMouseDown"]/*' />
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && mouseInContentBounds)
            {
                Debug.Assert(this.DataGridView.CellMouseDownInContentBounds);
                UpdateButtonState(this.ButtonState | ButtonState.Pushed, e.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnMouseLeave"]/*' />
        protected override void OnMouseLeave(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }

            if (mouseInContentBounds)
            {
                mouseInContentBounds = false;
                if (this.ColumnIndex >= 0 &&
                    rowIndex >= 0 &&
                    (this.DataGridView.ApplyVisualStylesToInnerCells || this.FlatStyle == FlatStyle.Flat || this.FlatStyle == FlatStyle.Popup))
                {
                    this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
                }
            }

            if ((this.ButtonState & ButtonState.Pushed) != 0 &&
                this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X &&
                rowIndex == this.DataGridView.MouseDownCellAddress.Y)
            {
                UpdateButtonState(this.ButtonState & ~ButtonState.Pushed, rowIndex);
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnMouseMove"]/*' />
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            
            bool oldMouseInContentBounds = mouseInContentBounds;
            mouseInContentBounds = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);
            if (oldMouseInContentBounds != mouseInContentBounds)
            {
                if (this.DataGridView.ApplyVisualStylesToInnerCells || this.FlatStyle == FlatStyle.Flat || this.FlatStyle == FlatStyle.Popup)
                {
                    this.DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
                }

                if (e.ColumnIndex == this.DataGridView.MouseDownCellAddress.X &&
                    e.RowIndex == this.DataGridView.MouseDownCellAddress.Y &&
                    Control.MouseButtons == MouseButtons.Left)
                {
                    if ((this.ButtonState & ButtonState.Pushed) == 0 && 
                        mouseInContentBounds &&
                        this.DataGridView.CellMouseDownInContentBounds)
                    {
                        UpdateButtonState(this.ButtonState | ButtonState.Pushed, e.RowIndex);
                    }
                    else if ((this.ButtonState & ButtonState.Pushed) != 0 && !mouseInContentBounds)
                    {
                        UpdateButtonState(this.ButtonState & ~ButtonState.Pushed, e.RowIndex);
                    }
                }
            }

            base.OnMouseMove(e);
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.OnMouseUp"]/*' />
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                UpdateButtonState(this.ButtonState & ~ButtonState.Pushed, e.RowIndex);
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.Paint"]/*' />
        protected override void Paint(Graphics graphics, 
            Rectangle clipBounds,
            Rectangle cellBounds, 
            int rowIndex, 
            DataGridViewElementStates elementState, 
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

            PaintPrivate(graphics, 
                clipBounds,
                cellBounds, 
                rowIndex, 
                elementState, 
                formattedValue,
                errorText,
                cellStyle,
                advancedBorderStyle,
                paintParts,
                false /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                true  /*paint*/);
        }

        // PaintPrivate is used in three places that need to duplicate the paint code:
        // 1. DataGridViewCell::Paint method
        // 2. DataGridViewCell::GetContentBounds
        // 3. DataGridViewCell::GetErrorIconBounds
        // 
        // if computeContentBounds is true then PaintPrivate returns the contentBounds
        // else if computeErrorIconBounds is true then PaintPrivate returns the errorIconBounds
        // else it returns Rectangle.Empty;
        private Rectangle PaintPrivate(Graphics g, 
            Rectangle clipBounds,
            Rectangle cellBounds, 
            int rowIndex, 
            DataGridViewElementStates elementState,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts,
            bool computeContentBounds,
            bool computeErrorIconBounds,
            bool paint)
        {
            // Parameter checking.
            // One bit and one bit only should be turned on
            Debug.Assert(paint || computeContentBounds || computeErrorIconBounds);
            Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds);
            Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint);
            Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds);
            Debug.Assert(cellStyle != null);

            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool cellCurrent = (ptCurrentCell.X == this.ColumnIndex && ptCurrentCell.Y == rowIndex);

            Rectangle resultBounds;
            string formattedString = formattedValue as string;

            SolidBrush backBrush = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
            SolidBrush foreBrush = this.DataGridView.GetCachedBrush(cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor);

            if (paint && DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);

            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            if (valBounds.Height > 0 && valBounds.Width > 0)
            {
                if (paint && DataGridViewCell.PaintBackground(paintParts) && backBrush.Color.A == 255)
                {
                    g.FillRectangle(backBrush, valBounds);
                }

                if (cellStyle.Padding != Padding.Empty)
                {
                    if (this.DataGridView.RightToLeftInternal)
                    {
                        valBounds.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
                    }
                    else
                    {
                        valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                    }
                    valBounds.Width -= cellStyle.Padding.Horizontal;
                    valBounds.Height -= cellStyle.Padding.Vertical;
                }

                Rectangle errorBounds = valBounds;

                if (valBounds.Height > 0 && valBounds.Width > 0 && (paint || computeContentBounds))
                {
                    if (this.FlatStyle == FlatStyle.Standard || this.FlatStyle == FlatStyle.System)
                    {
                        if (this.DataGridView.ApplyVisualStylesToInnerCells)
                        {
                            if (paint && DataGridViewCell.PaintContentBackground(paintParts))
                            {
                                VisualStyles.PushButtonState pbState = VisualStyles.PushButtonState.Normal;
                                if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                                {
                                    pbState = VisualStyles.PushButtonState.Pressed;
                                }
                                else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex && 
                                         this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
                                         mouseInContentBounds)
                                {
                                    pbState = VisualStyles.PushButtonState.Hot;
                                }
                                if (DataGridViewCell.PaintFocus(paintParts) && 
                                    cellCurrent && 
                                    this.DataGridView.ShowFocusCues && 
                                    this.DataGridView.Focused)
                                {
                                    pbState |= VisualStyles.PushButtonState.Default;
                                }
                                DataGridViewButtonCellRenderer.DrawButton(g, valBounds, (int)pbState);
                            }
                            resultBounds = valBounds;
                            valBounds = DataGridViewButtonCellRenderer.DataGridViewButtonRenderer.GetBackgroundContentRectangle(g, valBounds);
                        }
                        else
                        {
                            if (paint && DataGridViewCell.PaintContentBackground(paintParts))
                            {
                                ControlPaint.DrawBorder(g, valBounds, SystemColors.Control, 
                                                        (this.ButtonState == ButtonState.Normal) ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset);
                            }
                            resultBounds = valBounds;
                            valBounds.Inflate(-SystemInformation.Border3DSize.Width, -SystemInformation.Border3DSize.Height);
                        }
                    }
                    else if (this.FlatStyle == FlatStyle.Flat)
                    {
                        // ButtonBase::PaintFlatDown and ButtonBase::PaintFlatUp paint the border in the same way
                        valBounds.Inflate(-1, -1);
                        if (paint && DataGridViewCell.PaintContentBackground(paintParts))
                        {
                            ButtonInternal.ButtonBaseAdapter.DrawDefaultBorder(g, valBounds, foreBrush.Color, true /*isDefault == true*/);

                            if (backBrush.Color.A == 255)
                            {
                                if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                                {
                                    ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintFlatRender(g,
                                                                                                           cellStyle.ForeColor,
                                                                                                           cellStyle.BackColor,
                                                                                                           this.DataGridView.Enabled).Calculate();

                                    IntPtr hdc = g.GetHdc();
                                    try {
                                        using( WindowsGraphics wg = WindowsGraphics.FromHdc(hdc)) {
                                            
                                            System.Windows.Forms.Internal.WindowsBrush windowsBrush;
                                            if (colors.options.highContrast)
                                            {
                                                windowsBrush = new System.Windows.Forms.Internal.WindowsSolidBrush(wg.DeviceContext, colors.buttonShadow);
                                            }
                                            else
                                            {
                                                windowsBrush = new System.Windows.Forms.Internal.WindowsSolidBrush(wg.DeviceContext, colors.lowHighlight);
                                            }
                                            try
                                            {
                                                ButtonInternal.ButtonBaseAdapter.PaintButtonBackground(wg, valBounds, windowsBrush);
                                            }
                                            finally
                                            {
                                                windowsBrush.Dispose();
                                            }
                                        }                                        
                                    }
                                    finally {
                                        g.ReleaseHdc();
                                    }
                                }
                                else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                         this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
                                         mouseInContentBounds)
                                {
                                    IntPtr hdc = g.GetHdc();
                                    try {
                                        using( WindowsGraphics wg = WindowsGraphics.FromHdc(hdc)) {
                                            Color mouseOverBackColor = SystemColors.ControlDark;
                                            using (System.Windows.Forms.Internal.WindowsBrush windowBrush = new System.Windows.Forms.Internal.WindowsSolidBrush(wg.DeviceContext, mouseOverBackColor))
                                            {
                                                ButtonInternal.ButtonBaseAdapter.PaintButtonBackground(wg, valBounds, windowBrush);
                                            }
                                        }
                                    }
                                    finally {
                                        g.ReleaseHdc();
                                    }
                                }
                            }
                        }
                        resultBounds = valBounds;
                    }
                    else
                    {
                        Debug.Assert(this.FlatStyle == FlatStyle.Popup, "FlatStyle.Popup is the last flat style");
                        valBounds.Inflate(-1, -1);
                        if (paint && DataGridViewCell.PaintContentBackground(paintParts))
                        {
                            if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                            {
                                // paint down
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(g,
                                                                    valBounds,
                                                                    colors.options.highContrast ? colors.windowText : colors.windowFrame,
                                                                    true /*isDefault*/);
                                ControlPaint.DrawBorder(g,
                                                        valBounds,
                                                        colors.options.highContrast ? colors.windowText : colors.buttonShadow,
                                                        ButtonBorderStyle.Solid);
                            }
                            else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                     this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
                                     mouseInContentBounds)
                            {
                                // paint over
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(g,
                                                                    valBounds,
                                                                    colors.options.highContrast ? colors.windowText : colors.buttonShadow,
                                                                    false /*isDefault*/);
                                ButtonBaseAdapter.Draw3DLiteBorder(g, valBounds, colors, true);
                            }
                            else
                            {
                                // paint up
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(g, valBounds, colors.options.highContrast ? colors.windowText : colors.buttonShadow, false /*isDefault*/);
                                ButtonBaseAdapter.DrawFlatBorder(g, valBounds, colors.options.highContrast ? colors.windowText : colors.buttonShadow);
                            }
                        }
                        resultBounds = valBounds;
                    }
                }
                else if (computeErrorIconBounds)
                {
                    if (!String.IsNullOrEmpty(errorText))
                    {
                        resultBounds = ComputeErrorIconBounds(errorBounds);
                    }
                    else
                    {
                        resultBounds = Rectangle.Empty;
                    }
                }
                else
                {
                    Debug.Assert(valBounds.Height <= 0 || valBounds.Width <= 0);
                    resultBounds = Rectangle.Empty;
                }
                
                if (paint &&
                    DataGridViewCell.PaintFocus(paintParts) &&
                    cellCurrent &&
                    this.DataGridView.ShowFocusCues && 
                    this.DataGridView.Focused &&
                    valBounds.Width > 2 * SystemInformation.Border3DSize.Width + 1 &&
                    valBounds.Height > 2 * SystemInformation.Border3DSize.Height + 1)
                {
                    // Draw focus rectangle
                    if (this.FlatStyle == FlatStyle.System || this.FlatStyle == FlatStyle.Standard)
                    {
                        ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(valBounds, -1, -1), Color.Empty, SystemColors.Control);
                    }
                    else if (this.FlatStyle == FlatStyle.Flat)
                    {
                        if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 ||
                            (this.DataGridView.CurrentCellAddress.Y == rowIndex && this.DataGridView.CurrentCellAddress.X == this.ColumnIndex))
                        {
                            ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintFlatRender(g,
                                                                                                   cellStyle.ForeColor,
                                                                                                   cellStyle.BackColor,
                                                                                                   this.DataGridView.Enabled).Calculate();
                            string text = (formattedString != null) ? formattedString : String.Empty;

                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.ButtonFlatAdapter.PaintFlatLayout(g,
                                                                                                                   true,
                                                                                                                   SystemInformation.HighContrast,
                                                                                                                   1,
                                                                                                                   valBounds,
                                                                                                                   Padding.Empty,
                                                                                                                   false,
                                                                                                                   cellStyle.Font,
                                                                                                                   text,
                                                                                                                   this.DataGridView.Enabled,
                                                                                                                   DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                                                                                                   this.DataGridView.RightToLeft);
                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();

                            ButtonInternal.ButtonBaseAdapter.DrawFlatFocus(g,
                                                                           layout.focus,
                                                                           colors.options.highContrast ? colors.windowText : colors.constrastButtonShadow);
                        }
                    }
                    else
                    {
                        Debug.Assert(this.FlatStyle == FlatStyle.Popup, "FlatStyle.Popup is the last flat style");
                        if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 || 
                            (this.DataGridView.CurrentCellAddress.Y == rowIndex && this.DataGridView.CurrentCellAddress.X == this.ColumnIndex))
                        {
                            // If we are painting the current cell, then paint the text up.
                            // If we are painting the current cell and the current cell is pressed down, then paint the text down.
                            bool paintUp = (this.ButtonState == ButtonState.Normal);
                            string text = (formattedString != null) ? formattedString : String.Empty;
                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.ButtonPopupAdapter.PaintPopupLayout(g,
                                                                                                                   paintUp,
                                                                                                                   SystemInformation.HighContrast ? 2 : 1,
                                                                                                                   valBounds,
                                                                                                                   Padding.Empty,
                                                                                                                   false,
                                                                                                                   cellStyle.Font,
                                                                                                                   text,
                                                                                                                   this.DataGridView.Enabled,
                                                                                                                   DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                                                                                                   this.DataGridView.RightToLeft);
                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();
                         

                            ControlPaint.DrawFocusRectangle(g,
                                                            layout.focus,
                                                            cellStyle.ForeColor,
                                                            cellStyle.BackColor);
                        }
                    }
                }

                if (formattedString != null && paint && DataGridViewCell.PaintContentForeground(paintParts))
                {
                    // Font independent margins
                    valBounds.Offset(DATAGRIDVIEWBUTTONCELL_horizontalTextMargin, DATAGRIDVIEWBUTTONCELL_verticalTextMargin);
                    valBounds.Width -= 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin;
                    valBounds.Height -= 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin;

                    if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 &&
                        this.FlatStyle != FlatStyle.Flat && this.FlatStyle != FlatStyle.Popup)
                    {
                        valBounds.Offset(1, 1);
                        valBounds.Width--;
                        valBounds.Height--;
                    }

                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {
                        Color textColor;
                        if (this.DataGridView.ApplyVisualStylesToInnerCells &&
                            (this.FlatStyle == FlatStyle.System || this.FlatStyle == FlatStyle.Standard))
                        {
                            textColor = DataGridViewButtonCellRenderer.DataGridViewButtonRenderer.GetColor(ColorProperty.TextColor);
                        }
                        else
                        {
                            textColor = foreBrush.Color;
                        }
                        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                        TextRenderer.DrawText(g,
                                              formattedString,
                                              cellStyle.Font,
                                              valBounds,
                                              textColor,
                                              flags);
                    }
                }

                if (this.DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
                {
                    PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
                }
            }
            else
            {
                resultBounds = Rectangle.Empty;
            }

            return resultBounds;
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCell.ToString"]/*' />
        public override string ToString()
        {
            return "DataGridViewButtonCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            if (this.ButtonState != newButtonState)
            {
                this.ButtonState = newButtonState;
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }
        }

        private class DataGridViewButtonCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewButtonCellRenderer()
            {
            }

            public static VisualStyleRenderer DataGridViewButtonRenderer
            {
                get
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ButtonElement);
                    }
                    return visualStyleRenderer;
                }
            }

            public static void DrawButton(Graphics g, Rectangle bounds, int buttonState)
            {
                DataGridViewButtonRenderer.SetParameters(ButtonElement.ClassName, ButtonElement.Part, buttonState);
                DataGridViewButtonRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }

        /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCellAccessibleObject"]/*' />
        protected class DataGridViewButtonCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCellAccessibleObject.DataGridViewButtonCellAccessibleObject"]/*' />
            public DataGridViewButtonCellAccessibleObject(DataGridViewCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    return string.Format(SR.DataGridView_AccButtonCellDefaultAction);
                }
            }

            /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                DataGridViewButtonCell dataGridViewCell = (DataGridViewButtonCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                }

                if (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            /// <include file='doc\DataGridViewButtonCell.uex' path='docs/doc[@for="DataGridViewButtonCellAccessibleObject.GetChildCount"]/*' />
            public override int GetChildCount()
            {
                return 0;
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (AccessibilityImprovements.Level2)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_ButtonControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
