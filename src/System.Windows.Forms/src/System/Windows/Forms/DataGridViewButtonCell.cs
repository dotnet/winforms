﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a button cell in the dataGridView.
    /// </summary>
    public partial class DataGridViewButtonCell : DataGridViewCell
    {
        private static readonly int PropButtonCellFlatStyle = PropertyStore.CreateKey();
        private static readonly int PropButtonCellState = PropertyStore.CreateKey();
        private static readonly int PropButtonCellUseColumnTextForButtonValue = PropertyStore.CreateKey();
        private static readonly VisualStyleElement ButtonElement = VisualStyleElement.Button.PushButton.Normal;

        private const byte DATAGRIDVIEWBUTTONCELL_themeMargin = 100; // Used to calculate the margins required for theming rendering
        private const byte DATAGRIDVIEWBUTTONCELL_horizontalTextMargin = 2;
        private const byte DATAGRIDVIEWBUTTONCELL_verticalTextMargin = 1;
        private const byte DATAGRIDVIEWBUTTONCELL_textPadding = 5;

        private static Rectangle rectThemeMargins = new Rectangle(-1, -1, 0, 0);
        private static bool mouseInContentBounds;

        private static readonly Type defaultFormattedValueType = typeof(string);
        private static readonly Type defaultValueType = typeof(object);
        private static readonly Type cellType = typeof(DataGridViewButtonCell);

        public DataGridViewButtonCell()
        {
        }

        private ButtonState ButtonState
        {
            get
            {
                int buttonState = Properties.GetInteger(PropButtonCellState, out bool found);
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
                if (ButtonState != value)
                {
                    Properties.SetInteger(PropButtonCellState, (int)value);
                }
            }
        }

        public override Type EditType
        {
            get
            {
                // Buttons can't switch to edit mode
                return null;
            }
        }

        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                int flatStyle = Properties.GetInteger(PropButtonCellFlatStyle, out bool found);
                if (found)
                {
                    return (FlatStyle)flatStyle;
                }
                return FlatStyle.Standard;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                SourceGenerated.EnumValidator.Validate(value);
                if (value != FlatStyle)
                {
                    Properties.SetInteger(PropButtonCellFlatStyle, (int)value);
                    OnCommonChange();
                }
            }
        }

        internal FlatStyle FlatStyleInternal
        {
            set
            {
                Debug.Assert(value >= FlatStyle.Flat && value <= FlatStyle.System);
                if (value != FlatStyle)
                {
                    Properties.SetInteger(PropButtonCellFlatStyle, (int)value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                // we return string for the formatted type
                return defaultFormattedValueType;
            }
        }

        [DefaultValue(false)]
        public bool UseColumnTextForButtonValue
        {
            get
            {
                int useColumnTextForButtonValue = Properties.GetInteger(PropButtonCellUseColumnTextForButtonValue, out bool found);
                if (found)
                {
                    return useColumnTextForButtonValue == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != UseColumnTextForButtonValue)
                {
                    Properties.SetInteger(PropButtonCellUseColumnTextForButtonValue, value ? 1 : 0);
                    OnCommonChange();
                }
            }
        }

        internal bool UseColumnTextForButtonValueInternal
        {
            set
            {
                if (value != UseColumnTextForButtonValue)
                {
                    Properties.SetInteger(PropButtonCellUseColumnTextForButtonValue, value ? 1 : 0);
                }
            }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType is not null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
        }

        public override object Clone()
        {
            DataGridViewButtonCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewButtonCell();
            }
            else
            {
                dataGridViewCell = (DataGridViewButtonCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.FlatStyleInternal = FlatStyle;
            dataGridViewCell.UseColumnTextForButtonValueInternal = UseColumnTextForButtonValue;
            return dataGridViewCell;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewButtonCellAccessibleObject(this);
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
            {
                return Rectangle.Empty;
            }

            ComputeBorderStyleCellStateAndCellBounds(
                rowIndex,
                out DataGridViewAdvancedBorderStyle dgvabsEffective,
                out DataGridViewElementStates cellState,
                out Rectangle cellBounds);

            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue: null,            // contentBounds is independent of formattedValue
                errorText: null,                 // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                computeContentBounds: true,
                computeErrorIconBounds: false,
                paint: false);

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

        private protected override string GetDefaultToolTipText()
        {
            if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
            {
                return SR.DefaultDataGridViewButtonCellTollTipText;
            }

            return null;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView is null ||
                rowIndex < 0 ||
                OwningColumn is null ||
                !DataGridView.ShowCellErrors ||
                string.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

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

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (DataGridView is null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize;
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            int marginWidths, marginHeights;
            string formattedString = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize) as string;
            if (string.IsNullOrEmpty(formattedString))
            {
                formattedString = " ";
            }
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            // Adding space for text padding.
            if (DataGridView.ApplyVisualStylesToInnerCells)
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
                            preferredSize = new Size(
                                MeasureTextWidth(
                                    graphics,
                                    formattedString,
                                    cellStyle.Font,
                                    constraintSize.Height - borderAndPaddingHeights - marginHeights - 2
                                        * DATAGRIDVIEWBUTTONCELL_verticalTextMargin,
                                    flags),
                                0);
                        }
                        else
                        {
                            preferredSize = new Size(
                                MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Width,
                                0);
                        }
                        break;
                    }
                case DataGridViewFreeDimension.Height:
                    {
                        if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1 &&
                            constraintSize.Width - borderAndPaddingWidths - marginWidths - 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin > 0)
                        {
                            preferredSize = new Size(
                                0,
                                MeasureTextHeight(
                                    graphics,
                                    formattedString,
                                    cellStyle.Font,
                                    constraintSize.Width - borderAndPaddingWidths - marginWidths - 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin,
                                    flags));
                        }
                        else
                        {
                            preferredSize = new Size(
                                0,
                                MeasureTextSize(
                                    graphics,
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
                            preferredSize = MeasureTextPreferredSize(graphics, formattedString, cellStyle.Font, 5.0F, flags);
                        }
                        else
                        {
                            preferredSize = MeasureTextSize(graphics, formattedString, cellStyle.Font, flags);
                        }
                        break;
                    }
            }

            if (freeDimension != DataGridViewFreeDimension.Height)
            {
                preferredSize.Width += borderAndPaddingWidths + marginWidths + 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + IconMarginWidth * 2 + s_iconsWidth);
                }
            }

            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += borderAndPaddingHeights + marginHeights + 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + IconMarginHeight * 2 + s_iconsHeight);
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

        protected override object GetValue(int rowIndex)
        {
            if (UseColumnTextForButtonValue &&
                DataGridView is not null &&
                DataGridView.NewRowIndex != rowIndex &&
                OwningColumn is not null &&
                OwningColumn is DataGridViewButtonColumn)
            {
                return ((DataGridViewButtonColumn)OwningColumn).Text;
            }
            return base.GetValue(rowIndex);
        }

        protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift;
        }

        protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space;
        }

        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        protected override bool MouseEnterUnsharesRow(int rowIndex)
        {
            return ColumnIndex == DataGridView.MouseDownCellAddress.X && rowIndex == DataGridView.MouseDownCellAddress.Y;
        }

        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return (ButtonState & ButtonState.Pushed) != 0;
        }

        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                UpdateButtonState(ButtonState | ButtonState.Checked, rowIndex);
                e.Handled = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space)
            {
                UpdateButtonState(ButtonState & ~ButtonState.Checked, rowIndex);
                if (!e.Alt && !e.Control && !e.Shift)
                {
                    RaiseCellClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
                    if (DataGridView is not null &&
                        ColumnIndex < DataGridView.Columns.Count &&
                        rowIndex < DataGridView.Rows.Count)
                    {
                        RaiseCellContentClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
                    }
                    e.Handled = true;
                }
            }
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (ButtonState != ButtonState.Normal)
            {
                Debug.Assert(RowIndex >= 0); // Cell is not in a shared row.
                UpdateButtonState(ButtonState.Normal, rowIndex);
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && mouseInContentBounds)
            {
                Debug.Assert(DataGridView.CellMouseDownInContentBounds);
                UpdateButtonState(ButtonState | ButtonState.Pushed, e.RowIndex);
            }
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            if (DataGridView is null)
            {
                return;
            }

            if (mouseInContentBounds)
            {
                mouseInContentBounds = false;
                if (ColumnIndex >= 0 &&
                    rowIndex >= 0 &&
                    (DataGridView.ApplyVisualStylesToInnerCells || FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                {
                    DataGridView.InvalidateCell(ColumnIndex, rowIndex);
                }
            }

            if ((ButtonState & ButtonState.Pushed) != 0 &&
                ColumnIndex == DataGridView.MouseDownCellAddress.X &&
                rowIndex == DataGridView.MouseDownCellAddress.Y)
            {
                UpdateButtonState(ButtonState & ~ButtonState.Pushed, rowIndex);
            }
        }

        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }

            bool oldMouseInContentBounds = mouseInContentBounds;
            mouseInContentBounds = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);
            if (oldMouseInContentBounds != mouseInContentBounds)
            {
                if (DataGridView.ApplyVisualStylesToInnerCells || FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)
                {
                    DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
                }

                if (e.ColumnIndex == DataGridView.MouseDownCellAddress.X &&
                    e.RowIndex == DataGridView.MouseDownCellAddress.Y &&
                    Control.MouseButtons == MouseButtons.Left)
                {
                    if ((ButtonState & ButtonState.Pushed) == 0 &&
                        mouseInContentBounds &&
                        DataGridView.CellMouseDownInContentBounds)
                    {
                        UpdateButtonState(ButtonState | ButtonState.Pushed, e.RowIndex);
                    }
                    else if ((ButtonState & ButtonState.Pushed) != 0 && !mouseInContentBounds)
                    {
                        UpdateButtonState(ButtonState & ~ButtonState.Pushed, e.RowIndex);
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                UpdateButtonState(ButtonState & ~ButtonState.Pushed, e.RowIndex);
            }
        }

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
            if (cellStyle is null)
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
            Debug.Assert(cellStyle is not null);

            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool cellCurrent = (ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex);

            Rectangle resultBounds;
            string formattedString = formattedValue as string;

            Color backBrushColor = PaintSelectionBackground(paintParts) && cellSelected
                ? cellStyle.SelectionBackColor
                : cellStyle.BackColor;
            Color foreBrushColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;

            if (paint && PaintBorder(paintParts))
            {
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);

            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            if (valBounds.Height <= 0 || valBounds.Width <= 0)
            {
                return Rectangle.Empty;
            }

            if (paint && PaintBackground(paintParts) && !backBrushColor.HasTransparency())
            {
                using var backBrush = backBrushColor.GetCachedSolidBrushScope();
                g.FillRectangle(backBrush, valBounds);
            }

            if (cellStyle.Padding != Padding.Empty)
            {
                if (DataGridView.RightToLeftInternal)
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
                switch (FlatStyle)
                {
                    case FlatStyle.Standard:
                    case FlatStyle.System:
                        if (DataGridView.ApplyVisualStylesToInnerCells)
                        {
                            if (paint && PaintContentBackground(paintParts))
                            {
                                PushButtonState pbState = VisualStyles.PushButtonState.Normal;
                                if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                                {
                                    pbState = PushButtonState.Pressed;
                                }
                                else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                    DataGridView.MouseEnteredCellAddress.X == ColumnIndex && mouseInContentBounds)
                                {
                                    pbState = PushButtonState.Hot;
                                }

                                if (PaintFocus(paintParts) && cellCurrent && DataGridView.ShowFocusCues && DataGridView.Focused)
                                {
                                    pbState |= PushButtonState.Default;
                                }

                                DataGridViewButtonCellRenderer.DrawButton(g, valBounds, (int)pbState);
                            }

                            resultBounds = valBounds;
                            valBounds = DataGridViewButtonCellRenderer.DataGridViewButtonRenderer.GetBackgroundContentRectangle(g, valBounds);
                        }
                        else
                        {
                            if (paint && PaintContentBackground(paintParts))
                            {
                                ControlPaint.DrawBorder(
                                    g,
                                    valBounds,
                                    SystemColors.Control,
                                    (ButtonState == ButtonState.Normal) ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset);
                            }
                            resultBounds = valBounds;
                            valBounds.Inflate(-SystemInformation.Border3DSize.Width, -SystemInformation.Border3DSize.Height);
                        }

                        break;

                    case FlatStyle.Flat:
                        // ButtonBase::PaintFlatDown and ButtonBase::PaintFlatUp paint the border in the same way
                        valBounds.Inflate(-1, -1);
                        if (paint && PaintContentBackground(paintParts))
                        {
                            ButtonBaseAdapter.DrawDefaultBorder(g, valBounds, backBrushColor, isDefault: true);

                            if (!backBrushColor.HasTransparency())
                            {
                                if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                                {
                                    ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintFlatRender(
                                        g,
                                        cellStyle.ForeColor,
                                        cellStyle.BackColor,
                                        DataGridView.Enabled).Calculate();

                                    using var hdc = new DeviceContextHdcScope(g);
                                    using var hbrush = new Gdi32.CreateBrushScope(
                                        colors.Options.HighContrast ? colors.ButtonShadow : colors.LowHighlight);
                                    hdc.FillRectangle(valBounds, hbrush);
                                }
                                else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                    DataGridView.MouseEnteredCellAddress.X == ColumnIndex && mouseInContentBounds)
                                {
                                    using var hdc = new DeviceContextHdcScope(g);
                                    using var hbrush = new Gdi32.CreateBrushScope(SystemColors.ControlDark);
                                    hdc.FillRectangle(valBounds, hbrush);
                                }
                            }
                        }

                        resultBounds = valBounds;
                        break;

                    default:
                        Debug.Assert(FlatStyle == FlatStyle.Popup, "FlatStyle.Popup is the last flat style");
                        valBounds.Inflate(-1, -1);
                        if (paint && PaintContentBackground(paintParts))
                        {
                            if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                            {
                                // paint down
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(
                                    g,
                                    valBounds,
                                    colors.Options.HighContrast ? colors.WindowText : colors.WindowFrame,
                                    isDefault: true);
                                ControlPaint.DrawBorder(
                                    g,
                                    valBounds,
                                    colors.Options.HighContrast ? colors.WindowText : colors.ButtonShadow,
                                    ButtonBorderStyle.Solid);
                            }
                            else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                        DataGridView.MouseEnteredCellAddress.X == ColumnIndex &&
                                        mouseInContentBounds)
                            {
                                // paint over
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(
                                    g,
                                    valBounds,
                                    colors.Options.HighContrast ? colors.WindowText : colors.ButtonShadow,
                                    isDefault: false);
                                ButtonBaseAdapter.Draw3DLiteBorder(g, valBounds, colors, true);
                            }
                            else
                            {
                                // paint up
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                ButtonBaseAdapter.DrawDefaultBorder(
                                    g,
                                    valBounds,
                                    colors.Options.HighContrast ? colors.WindowText : colors.ButtonShadow,
                                    isDefault: false);
                                ControlPaint.DrawBorderSimple(
                                    g,
                                    valBounds,
                                    colors.Options.HighContrast ? colors.WindowText : colors.ButtonShadow);
                            }
                        }

                        resultBounds = valBounds;
                        break;
                }
            }
            else if (computeErrorIconBounds)
            {
                if (!string.IsNullOrEmpty(errorText))
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
                PaintFocus(paintParts) &&
                cellCurrent &&
                DataGridView.ShowFocusCues &&
                DataGridView.Focused &&
                valBounds.Width > 2 * SystemInformation.Border3DSize.Width + 1 &&
                valBounds.Height > 2 * SystemInformation.Border3DSize.Height + 1)
            {
                // Draw focus rectangle
                if (FlatStyle == FlatStyle.System || FlatStyle == FlatStyle.Standard)
                {
                    ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(valBounds, -1, -1), Color.Empty, SystemColors.Control);
                }
                else if (FlatStyle == FlatStyle.Flat)
                {
                    if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 ||
                        (DataGridView.CurrentCellAddress.Y == rowIndex && DataGridView.CurrentCellAddress.X == ColumnIndex))
                    {
                        ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintFlatRender(
                            g,
                            cellStyle.ForeColor,
                            cellStyle.BackColor,
                            DataGridView.Enabled).Calculate();

                        string text = formattedString ?? string.Empty;

                        ButtonBaseAdapter.LayoutOptions options = ButtonFlatAdapter.PaintFlatLayout(
                            true,
                            SystemInformation.HighContrast,
                            1,
                            valBounds,
                            Padding.Empty,
                            false,
                            cellStyle.Font,
                            text,
                            DataGridView.Enabled,
                            DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                            DataGridView.RightToLeft);
                        options.DotNetOneButtonCompat = false;
                        ButtonBaseAdapter.LayoutData layout = options.Layout();

                        ButtonBaseAdapter.DrawFlatFocus(
                            g,
                            layout.Focus,
                            colors.Options.HighContrast ? colors.WindowText : colors.ConstrastButtonShadow);
                    }
                }
                else
                {
                    Debug.Assert(FlatStyle == FlatStyle.Popup, "FlatStyle.Popup is the last flat style");
                    if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 ||
                        (DataGridView.CurrentCellAddress.Y == rowIndex && DataGridView.CurrentCellAddress.X == ColumnIndex))
                    {
                        // If we are painting the current cell, then paint the text up.
                        // If we are painting the current cell and the current cell is pressed down, then paint the text down.
                        bool paintUp = (ButtonState == ButtonState.Normal);
                        string text = formattedString ?? string.Empty;
                        ButtonBaseAdapter.LayoutOptions options = ButtonPopupAdapter.PaintPopupLayout(
                            paintUp,
                            SystemInformation.HighContrast ? 2 : 1,
                            valBounds,
                            Padding.Empty,
                            false,
                            cellStyle.Font,
                            text,
                            DataGridView.Enabled,
                            DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                            DataGridView.RightToLeft);
                        options.DotNetOneButtonCompat = false;
                        ButtonBaseAdapter.LayoutData layout = options.Layout();

                        ControlPaint.DrawFocusRectangle(
                            g,
                            layout.Focus,
                            cellStyle.ForeColor,
                            cellStyle.BackColor);
                    }
                }
            }

            if (formattedString is not null && paint && PaintContentForeground(paintParts))
            {
                // Font independent margins
                valBounds.Offset(DATAGRIDVIEWBUTTONCELL_horizontalTextMargin, DATAGRIDVIEWBUTTONCELL_verticalTextMargin);
                valBounds.Width -= 2 * DATAGRIDVIEWBUTTONCELL_horizontalTextMargin;
                valBounds.Height -= 2 * DATAGRIDVIEWBUTTONCELL_verticalTextMargin;

                if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0 &&
                    FlatStyle != FlatStyle.Flat && FlatStyle != FlatStyle.Popup)
                {
                    valBounds.Offset(1, 1);
                    valBounds.Width--;
                    valBounds.Height--;
                }

                if (valBounds.Width > 0 && valBounds.Height > 0)
                {
                    Color textColor;
                    if (DataGridView.ApplyVisualStylesToInnerCells &&
                        (FlatStyle == FlatStyle.System || FlatStyle == FlatStyle.Standard))
                    {
                        textColor = DataGridViewButtonCellRenderer.DataGridViewButtonRenderer.GetColor(ColorProperty.TextColor);
                    }
                    else
                    {
                        textColor = foreBrushColor;
                    }

                    TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(
                        DataGridView.RightToLeftInternal,
                        cellStyle.Alignment,
                        cellStyle.WrapMode);

                    TextRenderer.DrawText(
                        g,
                        formattedString,
                        cellStyle.Font,
                        valBounds,
                        textColor,
                        flags);
                }
            }

            if (DataGridView.ShowCellErrors && paint && PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        public override string ToString()
        {
            return "DataGridViewButtonCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            if (ButtonState != newButtonState)
            {
                ButtonState = newButtonState;
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }
    }
}
