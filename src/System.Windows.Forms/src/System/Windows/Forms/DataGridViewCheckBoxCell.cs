// Licensed to the .NET Foundation under one or more agreements.
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
    ///  Identifies a checkbox cell in the DataGridView.
    /// </summary>
    public partial class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
    {
        private const DataGridViewContentAlignment AnyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
        private const DataGridViewContentAlignment AnyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private const DataGridViewContentAlignment AnyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;
        private const DataGridViewContentAlignment AnyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;
        private const DataGridViewContentAlignment AnyMiddle = DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.MiddleLeft;

        private static readonly VisualStyleElement CheckBoxElement = VisualStyleElement.Button.CheckBox.UncheckedNormal;
        private static readonly int PropButtonCellState = PropertyStore.CreateKey();
        private static readonly int PropTrueValue = PropertyStore.CreateKey();
        private static readonly int PropFalseValue = PropertyStore.CreateKey();
        private static readonly int PropFlatStyle = PropertyStore.CreateKey();
        private static readonly int PropIndeterminateValue = PropertyStore.CreateKey();
        private static Bitmap checkImage;

        private const byte DATAGRIDVIEWCHECKBOXCELL_threeState = 0x01;
        private const byte DATAGRIDVIEWCHECKBOXCELL_valueChanged = 0x02;
        private const byte DATAGRIDVIEWCHECKBOXCELL_checked = 0x10;
        private const byte DATAGRIDVIEWCHECKBOXCELL_indeterminate = 0x20;

        private const byte DATAGRIDVIEWCHECKBOXCELL_margin = 2;  // horizontal and vertical margins for preferred sizes

        private byte flags;  // see DATAGRIDVIEWCHECKBOXCELL_ consts above
        private static bool mouseInContentBounds;
        private static readonly Type defaultCheckStateType = typeof(CheckState);
        private static readonly Type defaultBooleanType = typeof(bool);
        private static readonly Type cellType = typeof(DataGridViewCheckBoxCell);

        public DataGridViewCheckBoxCell() : this(false /*threeState*/)
        {
        }

        public DataGridViewCheckBoxCell(bool threeState)
        {
            if (threeState)
            {
                flags = DATAGRIDVIEWCHECKBOXCELL_threeState;
            }
        }

        public virtual object EditingCellFormattedValue
        {
            get
            {
                return GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                if (FormattedValueType is null)
                {
                    throw new ArgumentException(SR.DataGridViewCell_FormattedValueTypeNull);
                }
                if (value is null || !FormattedValueType.IsAssignableFrom(value.GetType()))
                {
                    // Assigned formatted value may not be of the good type, in cases where the app
                    // is feeding wrong values to the cell in virtual / databound mode.
                    throw new ArgumentException(SR.DataGridViewCheckBoxCell_InvalidValueType);
                }
                if (value is CheckState)
                {
                    if (((CheckState)value) == System.Windows.Forms.CheckState.Checked)
                    {
                        flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_checked;
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                    }
                    else if (((CheckState)value) == System.Windows.Forms.CheckState.Indeterminate)
                    {
                        flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_indeterminate;
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                    }
                }
                else if (value is bool)
                {
                    if ((bool)value)
                    {
                        flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_checked;
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                    }

                    flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                }
                else
                {
                    throw new ArgumentException(SR.DataGridViewCheckBoxCell_InvalidValueType);
                }
            }
        }

        public virtual bool EditingCellValueChanged
        {
            get
            {
                return ((flags & DATAGRIDVIEWCHECKBOXCELL_valueChanged) != 0x00);
            }
            set
            {
                if (value)
                {
                    flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_valueChanged;
                }
                else
                {
                    flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_valueChanged);
                }
            }
        }

        public virtual object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
        {
            if (FormattedValueType is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCell_FormattedValueTypeNull);
            }
            if (FormattedValueType.IsAssignableFrom(defaultCheckStateType))
            {
                if ((flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00)
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return SR.DataGridViewCheckBoxCell_ClipboardChecked;
                    }
                    return System.Windows.Forms.CheckState.Checked;
                }
                else if ((flags & DATAGRIDVIEWCHECKBOXCELL_indeterminate) != 0x00)
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return SR.DataGridViewCheckBoxCell_ClipboardIndeterminate;
                    }
                    return System.Windows.Forms.CheckState.Indeterminate;
                }
                else
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return SR.DataGridViewCheckBoxCell_ClipboardUnchecked;
                    }
                    return System.Windows.Forms.CheckState.Unchecked;
                }
            }
            else if (FormattedValueType.IsAssignableFrom(defaultBooleanType))
            {
                bool ret = (bool)((flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00);
                if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                {
                    return ret ? SR.DataGridViewCheckBoxCell_ClipboardTrue : SR.DataGridViewCheckBoxCell_ClipboardFalse;
                }
                return ret;
            }
            else
            {
                return null;
            }
        }

        public virtual void PrepareEditingCellForEdit(bool selectAll)
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
                // Check boxes can't switch to edit mode
                // This cell type must implement the IEditingCell interface
                return null;
            }
        }

        [DefaultValue(null)]
        public object FalseValue
        {
            get
            {
                return Properties.GetObject(PropFalseValue);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropFalseValue))
                {
                    Properties.SetObject(PropFalseValue, value);
                    if (DataGridView != null)
                    {
                        if (RowIndex != -1)
                        {
                            DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            DataGridView.InvalidateColumnInternal(ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object FalseValueInternal
        {
            set
            {
                if (value != null || Properties.ContainsObject(PropFalseValue))
                {
                    Properties.SetObject(PropFalseValue, value);
                }
            }
        }

        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                int flatStyle = Properties.GetInteger(PropFlatStyle, out bool found);
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
                if (value != FlatStyle)
                {
                    Properties.SetInteger(PropFlatStyle, (int)value);
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
                    Properties.SetInteger(PropFlatStyle, (int)value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                if (ThreeState)
                {
                    return defaultCheckStateType;
                }
                else
                {
                    return defaultBooleanType;
                }
            }
        }

        [DefaultValue(null)]
        public object IndeterminateValue
        {
            get
            {
                return Properties.GetObject(PropIndeterminateValue);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropIndeterminateValue))
                {
                    Properties.SetObject(PropIndeterminateValue, value);
                    if (DataGridView != null)
                    {
                        if (RowIndex != -1)
                        {
                            DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            DataGridView.InvalidateColumnInternal(ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object IndeterminateValueInternal
        {
            set
            {
                if (value != null || Properties.ContainsObject(PropIndeterminateValue))
                {
                    Properties.SetObject(PropIndeterminateValue, value);
                }
            }
        }

        [DefaultValue(false)]
        public bool ThreeState
        {
            get
            {
                return ((flags & DATAGRIDVIEWCHECKBOXCELL_threeState) != 0x00);
            }
            set
            {
                if (ThreeState != value)
                {
                    ThreeStateInternal = value;
                    if (DataGridView != null)
                    {
                        if (RowIndex != -1)
                        {
                            DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            DataGridView.InvalidateColumnInternal(ColumnIndex);
                        }
                    }
                }
            }
        }

        internal bool ThreeStateInternal
        {
            set
            {
                if (ThreeState != value)
                {
                    if (value)
                    {
                        flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_threeState;
                    }
                    else
                    {
                        flags = (byte)(flags & ~DATAGRIDVIEWCHECKBOXCELL_threeState);
                    }
                }
            }
        }

        [DefaultValue(null)]
        public object TrueValue
        {
            get
            {
                return Properties.GetObject(PropTrueValue);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropTrueValue))
                {
                    Properties.SetObject(PropTrueValue, value);
                    if (DataGridView != null)
                    {
                        if (RowIndex != -1)
                        {
                            DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            DataGridView.InvalidateColumnInternal(ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object TrueValueInternal
        {
            set
            {
                if (value != null || Properties.ContainsObject(PropTrueValue))
                {
                    Properties.SetObject(PropTrueValue, value);
                }
            }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }

                if (ThreeState)
                {
                    return defaultCheckStateType;
                }
                else
                {
                    return defaultBooleanType;
                }
            }
            set
            {
                base.ValueType = value;
                ThreeState = (value != null && defaultCheckStateType.IsAssignableFrom(value));
            }
        }

        public override object Clone()
        {
            DataGridViewCheckBoxCell dataGridViewCell;
            Type thisType = GetType();
            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewCheckBoxCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewCheckBoxCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.ThreeStateInternal = ThreeState;
            dataGridViewCell.TrueValueInternal = TrueValue;
            dataGridViewCell.FalseValueInternal = FalseValue;
            dataGridViewCell.IndeterminateValueInternal = IndeterminateValue;
            dataGridViewCell.FlatStyleInternal = FlatStyle;
            return dataGridViewCell;
        }

        private bool CommonContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            return ptCurrentCell.X == ColumnIndex &&
                   ptCurrentCell.Y == e.RowIndex &&
                   DataGridView.IsCurrentCellInEditMode;
        }

        protected override bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return CommonContentClickUnsharesRow(e);
        }

        protected override bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return CommonContentClickUnsharesRow(e);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewCheckBoxCellAccessibleObject(this);
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

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle checkBoxBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                null /*formattedValue*/,            // checkBoxBounds is independent of formattedValue
                null /*errorText*/,                 // checkBoxBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            object value = GetValue(rowIndex);
            Rectangle checkBoxBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting),
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(checkBoxBoundsDebug.Equals(checkBoxBounds));
#endif

            return checkBoxBounds;
        }

        private protected override string GetDefaultToolTipText()
        {
            if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
            {
                return SR.DataGridViewCheckBoxCell_ClipboardFalse;
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

            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == rowIndex && DataGridView.IsCurrentCellInEditMode)
            {
                // PaintPrivate does not paint the error icon if this is the current cell.
                // So don't set the ErrorIconBounds either.
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
                true  /*computeErrorIconBound*/,
                false /*paint*/);

            return errorIconBounds;
        }

        protected override object GetFormattedValue(object value,
                                                    int rowIndex,
                                                    ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            if (value != null)
            {
                if (ThreeState)
                {
                    if (value.Equals(TrueValue) ||
                        (value is int && (int)value == (int)CheckState.Checked))
                    {
                        value = CheckState.Checked;
                    }
                    else if (value.Equals(FalseValue) ||
                             (value is int && (int)value == (int)CheckState.Unchecked))
                    {
                        value = CheckState.Unchecked;
                    }
                    else if (value.Equals(IndeterminateValue) ||
                             (value is int && (int)value == (int)CheckState.Indeterminate))
                    {
                        value = CheckState.Indeterminate;
                    }
                }
                else
                {
                    if (value.Equals(TrueValue) ||
                        (value is int && (int)value != 0))
                    {
                        value = true;
                    }
                    else if (value.Equals(FalseValue) ||
                             (value is int && (int)value == 0))
                    {
                        value = false;
                    }
                }
            }

            object ret = base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            if (ret != null && (context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
            {
                if (ret is bool retBool)
                {
                    if (retBool)
                    {
                        return ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardChecked : SR.DataGridViewCheckBoxCell_ClipboardTrue;
                    }
                    else
                    {
                        return ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardUnchecked : SR.DataGridViewCheckBoxCell_ClipboardFalse;
                    }
                }
                else if (ret is CheckState retCheckState)
                {
                    if (retCheckState == CheckState.Checked)
                    {
                        return ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardChecked : SR.DataGridViewCheckBoxCell_ClipboardTrue;
                    }
                    else if (retCheckState == CheckState.Unchecked)
                    {
                        return ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardUnchecked : SR.DataGridViewCheckBoxCell_ClipboardFalse;
                    }
                    else
                    {
                        Debug.Assert(retCheckState == CheckState.Indeterminate);
                        return SR.DataGridViewCheckBoxCell_ClipboardIndeterminate;
                    }
                }
            }
            return ret;
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

            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            Size preferredSize;
            if (DataGridView.ApplyVisualStylesToInnerCells)
            {
                // Assuming here that all checkbox states use the same size. We should take the largest of the state specific sizes.
                Size checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
                switch (FlatStyle)
                {
                    case FlatStyle.Standard:
                    case FlatStyle.System:
                        break;
                    case FlatStyle.Flat:
                        checkBoxSize.Width -= 3;
                        checkBoxSize.Height -= 3;
                        break;
                    case FlatStyle.Popup:
                        checkBoxSize.Width -= 2;
                        checkBoxSize.Height -= 2;
                        break;
                }

                switch (freeDimension)
                {
                    case DataGridViewFreeDimension.Width:
                        {
                            preferredSize = new Size(checkBoxSize.Width + borderAndPaddingWidths + 2 * DATAGRIDVIEWCHECKBOXCELL_margin, 0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0, checkBoxSize.Height + borderAndPaddingHeights + 2 * DATAGRIDVIEWCHECKBOXCELL_margin);
                            break;
                        }
                    default:
                        {
                            preferredSize = new Size(checkBoxSize.Width + borderAndPaddingWidths + 2 * DATAGRIDVIEWCHECKBOXCELL_margin,
                                                     checkBoxSize.Height + borderAndPaddingHeights + 2 * DATAGRIDVIEWCHECKBOXCELL_margin);
                            break;
                        }
                }
            }
            else
            {
                int checkBoxSize;
                switch (FlatStyle)
                {
                    case FlatStyle.Flat:
                        checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal).Width - 3;
                        break;
                    case FlatStyle.Popup:
                        checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal).Width - 2;
                        break;
                    default: // FlatStyle.Standard || FlatStyle.System
                        checkBoxSize = SystemInformation.Border3DSize.Width * 2 + 9 + 2 * DATAGRIDVIEWCHECKBOXCELL_margin;
                        break;
                }

                switch (freeDimension)
                {
                    case DataGridViewFreeDimension.Width:
                        {
                            preferredSize = new Size(checkBoxSize + borderAndPaddingWidths, 0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0, checkBoxSize + borderAndPaddingHeights);
                            break;
                        }
                    default:
                        {
                            preferredSize = new Size(checkBoxSize + borderAndPaddingWidths, checkBoxSize + borderAndPaddingHeights);
                            break;
                        }
                }
            }

            // We should consider the border size when calculating the preferred size.

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);
            Rectangle borderWidths = BorderWidths(dgvabsEffective);
            preferredSize.Width += borderWidths.X;
            preferredSize.Height += borderWidths.Y;

            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                if (freeDimension != DataGridViewFreeDimension.Height)
                {
                    preferredSize.Width = Math.Max(preferredSize.Width,
                                                   borderAndPaddingWidths + IconMarginWidth * 2 + s_iconsWidth);
                }
                if (freeDimension != DataGridViewFreeDimension.Width)
                {
                    preferredSize.Height = Math.Max(preferredSize.Height,
                                                    borderAndPaddingHeights + IconMarginHeight * 2 + s_iconsHeight);
                }
            }
            return preferredSize;
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

        private void NotifyDataGridViewOfValueChange()
        {
            flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_valueChanged;
            DataGridView.NotifyCurrentCellDirty(true);
        }

        private void OnCommonContentClick(DataGridViewCellEventArgs e)
        {
            if (DataGridView is null)
            {
                return;
            }
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == e.RowIndex &&
                DataGridView.IsCurrentCellInEditMode)
            {
                if (SwitchFormattedValue())
                {
                    NotifyDataGridViewOfValueChange();
                    NotifyUiaClient();
                }
            }
        }

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            OnCommonContentClick(e);
        }

        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            OnCommonContentClick(e);
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
                    if (DataGridView != null &&
                        ColumnIndex < DataGridView.Columns.Count &&
                        rowIndex < DataGridView.Rows.Count)
                    {
                        RaiseCellContentClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
                    }
                    e.Handled = true;
                }

                NotifyMSAAClient(ColumnIndex, rowIndex);
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
                NotifyMSAAClient(e.ColumnIndex, e.RowIndex);
            }
        }

        private void NotifyUiaClient()
        {
            bool isCheckboxChecked = false;
            switch (EditingCellFormattedValue)
            {
                case string stringValue:
                    isCheckboxChecked = stringValue == SR.DataGridViewCheckBoxCell_ClipboardChecked;
                    break;
                case CheckState checkStateValue:
                    isCheckboxChecked = checkStateValue == CheckState.Checked;
                    break;
                case bool boolValue:
                    isCheckboxChecked = boolValue;
                    break;
            }

            var cellName = AccessibilityObject.Name ?? string.Empty;
            AccessibilityObject.InternalRaiseAutomationNotification(
                Automation.AutomationNotificationKind.Other,
                Automation.AutomationNotificationProcessing.MostRecent,
                isCheckboxChecked
                    ? string.Format(SR.DataGridViewCheckBoxCellCheckedStateDescription, cellName)
                    : string.Format(SR.DataGridViewCheckBoxCellUncheckedStateDescription, cellName));
        }

        private void NotifyMSAAClient(int columnIndex, int rowIndex)
        {
            Debug.Assert((columnIndex >= 0) && (columnIndex < DataGridView.Columns.Count));
            Debug.Assert((rowIndex >= 0) && (rowIndex < DataGridView.Rows.Count));

            int visibleRowIndex = DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, rowIndex);
            int visibleColumnIndex = DataGridView.Columns.ColumnIndexToActualDisplayIndex(columnIndex, DataGridViewElementStates.Visible);

            int topHeaderRowIncrement = DataGridView.ColumnHeadersVisible ? 1 : 0;
            int rowHeaderIncrement = DataGridView.RowHeadersVisible ? 1 : 0;

            int objectID = visibleRowIndex + topHeaderRowIncrement  // + 1 because the top header row acc obj is at index 0
                                           + 1;                     // + 1 because objectID's need to be positive and non-zero

            int childID = visibleColumnIndex + rowHeaderIncrement;  // + 1 because the column header cell is at index 0 in top header row acc obj
                                                                    //     same thing for the row header cell in the data grid view row acc obj

            if (DataGridView.AccessibilityObject is Control.ControlAccessibleObject accessibleObject)
            {
                accessibleObject.NotifyClients(AccessibleEvents.StateChange, objectID, childID);
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
            Debug.Assert(cellStyle != null);

            Rectangle resultBounds;

            if (paint && PaintBorder(paintParts))
            {
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool drawAsMixedCheckBox = false, drawErrorText = true;
            CheckState checkState;
            ButtonState bs;
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == rowIndex && DataGridView.IsCurrentCellInEditMode)
            {
                drawErrorText = false;
            }

            if (formattedValue != null && formattedValue is CheckState)
            {
                checkState = (CheckState)formattedValue;
                bs = (checkState == CheckState.Unchecked) ? ButtonState.Normal : ButtonState.Checked;
                drawAsMixedCheckBox = (checkState == CheckState.Indeterminate);
            }
            else if (formattedValue != null && formattedValue is bool)
            {
                if ((bool)formattedValue)
                {
                    checkState = CheckState.Checked;
                    bs = ButtonState.Checked;
                }
                else
                {
                    checkState = CheckState.Unchecked;
                    bs = ButtonState.Normal;
                }
            }
            else
            {
                // The provided formatted value has a wrong type. We raised a DataError event while formatting.
                bs = ButtonState.Normal; // Default rendering of the checkbox with wrong formatted value type.
                checkState = CheckState.Unchecked;
            }

            if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
            {
                bs |= ButtonState.Pushed;
            }

            Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
                ? cellStyle.SelectionBackColor
                : cellStyle.BackColor;

            if (paint && PaintBackground(paintParts) && !brushColor.HasTransparency())
            {
                using var brush = brushColor.GetCachedSolidBrushScope();
                g.FillRectangle(brush, valBounds);
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

            if (paint &&
                PaintFocus(paintParts) &&
                DataGridView.ShowFocusCues &&
                DataGridView.Focused &&
                ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == rowIndex)
            {
                // Draw focus rectangle
                ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, brushColor);
            }

            Rectangle errorBounds = valBounds;

            valBounds.Inflate(-DATAGRIDVIEWCHECKBOXCELL_margin, -DATAGRIDVIEWCHECKBOXCELL_margin);

            Size checkBoxSize;

            CheckBoxState themeCheckBoxState = CheckBoxState.UncheckedNormal;

            if (DataGridView.ApplyVisualStylesToInnerCells)
            {
                themeCheckBoxState = CheckBoxRenderer.ConvertFromButtonState(
                    bs,
                    drawAsMixedCheckBox,
                    isHot: DataGridView.MouseEnteredCellAddress.Y == rowIndex
                        && DataGridView.MouseEnteredCellAddress.X == ColumnIndex
                        && mouseInContentBounds);

                checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, themeCheckBoxState);
                switch (FlatStyle)
                {
                    case FlatStyle.Standard:
                    case FlatStyle.System:
                        break;
                    case FlatStyle.Flat:
                        checkBoxSize.Width -= 3;
                        checkBoxSize.Height -= 3;
                        break;
                    case FlatStyle.Popup:
                        checkBoxSize.Width -= 2;
                        checkBoxSize.Height -= 2;
                        break;
                }
            }
            else
            {
                switch (FlatStyle)
                {
                    case FlatStyle.Flat:
                        checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
                        checkBoxSize.Width -= 3;
                        checkBoxSize.Height -= 3;
                        break;
                    case FlatStyle.Popup:
                        checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
                        checkBoxSize.Width -= 2;
                        checkBoxSize.Height -= 2;
                        break;
                    default: // FlatStyle.Standard || FlatStyle.System
                        checkBoxSize = new Size(SystemInformation.Border3DSize.Width * 2 + 9, SystemInformation.Border3DSize.Width * 2 + 9);
                        break;
                }
            }

            if (valBounds.Width >= checkBoxSize.Width && valBounds.Height >= checkBoxSize.Height && (paint || computeContentBounds))
            {
                int checkBoxY;
                int checkBoxX;

                if ((!DataGridView.RightToLeftInternal && (cellStyle.Alignment & AnyRight) != 0) ||
                    (DataGridView.RightToLeftInternal && (cellStyle.Alignment & AnyLeft) != 0))
                {
                    checkBoxX = valBounds.Right - checkBoxSize.Width;
                }
                else if ((cellStyle.Alignment & AnyCenter) != 0)
                {
                    checkBoxX = valBounds.Left + (valBounds.Width - checkBoxSize.Width) / 2;
                }
                else
                {
                    checkBoxX = valBounds.Left;
                }

                if ((cellStyle.Alignment & AnyBottom) != 0)
                {
                    checkBoxY = valBounds.Bottom - checkBoxSize.Height;
                }
                else if ((cellStyle.Alignment & AnyMiddle) != 0)
                {
                    checkBoxY = valBounds.Top + (valBounds.Height - checkBoxSize.Height) / 2;
                }
                else
                {
                    checkBoxY = valBounds.Top;
                }

                if (DataGridView.ApplyVisualStylesToInnerCells && FlatStyle != FlatStyle.Flat && FlatStyle != FlatStyle.Popup)
                {
                    if (paint && PaintContentForeground(paintParts))
                    {
                        DataGridViewCheckBoxCellRenderer.DrawCheckBox(
                            g,
                            new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height),
                            (int)themeCheckBoxState);
                    }

                    resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);
                }
                else
                {
                    if (FlatStyle == FlatStyle.System || FlatStyle == FlatStyle.Standard)
                    {
                        if (paint && PaintContentForeground(paintParts))
                        {
                            if (drawAsMixedCheckBox)
                            {
                                ControlPaint.DrawMixedCheckBox(
                                    g,
                                    checkBoxX,
                                    checkBoxY,
                                    checkBoxSize.Width,
                                    checkBoxSize.Height,
                                    bs);
                            }
                            else
                            {
                                ControlPaint.DrawCheckBox(
                                    g,
                                    checkBoxX,
                                    checkBoxY,
                                    checkBoxSize.Width,
                                    checkBoxSize.Height,
                                    bs);
                            }
                        }

                        resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);
                    }
                    else if (FlatStyle == FlatStyle.Flat)
                    {
                        // CheckBox::Paint will only paint the check box differently when in FlatStyle.Flat
                        // this code is copied from CheckBox::DrawCheckFlat. it was a lot of trouble making this function static

                        Rectangle checkBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);

                        Color foreBrushColor = default;
                        Color backBrushColor = default;
                        Color highlight = default;

                        if (paint && PaintContentForeground(paintParts))
                        {
                            foreBrushColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                            backBrushColor = PaintSelectionBackground(paintParts) && cellSelected
                                ? cellStyle.SelectionBackColor
                                : cellStyle.BackColor;

                            highlight = ControlPaint.LightLight(backBrushColor);

                            if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                DataGridView.MouseEnteredCellAddress.X == ColumnIndex &&
                                mouseInContentBounds)
                            {
                                const float lowlight = .1f;
                                float adjust = 1 - lowlight;
                                if (highlight.GetBrightness() < .5)
                                {
                                    adjust = 1 + lowlight * 2;
                                }

                                highlight = Color.FromArgb(
                                    ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.R),
                                    ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.G),
                                    ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.B));
                            }

                            highlight = g.FindNearestColor(highlight);

                            using var pen = foreBrushColor.GetCachedPenScope();
                            g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Right - 1, checkBounds.Top);
                            g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Left, checkBounds.Bottom - 1);
                        }

                        checkBounds.Inflate(-1, -1);
                        checkBounds.Width++;
                        checkBounds.Height++;

                        if (paint && PaintContentForeground(paintParts))
                        {
                            if (checkState == CheckState.Indeterminate)
                            {
                                ButtonBaseAdapter.DrawDitheredFill(g, backBrushColor, highlight, checkBounds);
                            }
                            else
                            {
                                using var highBrush = highlight.GetCachedSolidBrushScope();
                                g.FillRectangle(highBrush, checkBounds);
                            }

                            // draw the check box
                            if (checkState != CheckState.Unchecked)
                            {
                                Rectangle fullSize = new Rectangle(checkBoxX - 1, checkBoxY - 1, checkBoxSize.Width + 3, checkBoxSize.Height + 3);
                                fullSize.Width++;
                                fullSize.Height++;

                                if (checkImage is null || checkImage.Width != fullSize.Width || checkImage.Height != fullSize.Height)
                                {
                                    if (checkImage != null)
                                    {
                                        checkImage.Dispose();
                                        checkImage = null;
                                    }

                                    // We draw the checkmark slightly off center to eliminate 3-D border artifacts,
                                    // and compensate below
                                    RECT rcCheck = new Rectangle(0, 0, fullSize.Width, fullSize.Height);
                                    Bitmap bitmap = new Bitmap(fullSize.Width, fullSize.Height);
                                    using (Graphics offscreen = Graphics.FromImage(bitmap))
                                    {
                                        offscreen.Clear(Color.Transparent);
                                        using var hdc = new DeviceContextHdcScope(offscreen);
                                        User32.DrawFrameControl(
                                            hdc,
                                            ref rcCheck,
                                            User32.DFC.MENU,
                                            User32.DFCS.MENUCHECK);
                                    }
                                    bitmap.MakeTransparent();
                                    checkImage = bitmap;
                                }

                                fullSize.Y--;
                                ControlPaint.DrawImageColorized(
                                    g,
                                    checkImage,
                                    fullSize,
                                    checkState == CheckState.Indeterminate
                                        ? ControlPaint.LightLight(foreBrushColor)
                                        : foreBrushColor);
                            }
                        }

                        resultBounds = checkBounds;
                    }
                    else
                    {
                        Debug.Assert(FlatStyle == FlatStyle.Popup);

                        Rectangle checkBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width - 1, checkBoxSize.Height - 1);

                        // The CheckBoxAdapter code moves the check box down about 3 pixels so we have to take that into account
                        checkBounds.Y -= 3;

                        if ((ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                        {
                            // paint down
                            ButtonBaseAdapter.LayoutOptions options = CheckBoxPopupAdapter.PaintPopupLayout(
                                show3D: true,
                                checkBoxSize.Width,
                                checkBounds,
                                Padding.Empty,
                                isDefault: false,
                                cellStyle.Font,
                                string.Empty,
                                DataGridView.Enabled,
                                DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                DataGridView.RightToLeft);

                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();

                            if (paint && PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout.checkBounds,
                                    colors.buttonFace,
                                    disabledColors: true);
                                CheckBoxBaseAdapter.DrawPopupBorder(g, layout.checkBounds, colors);
                                CheckBoxBaseAdapter.DrawCheckOnly(
                                    checkBoxSize.Width,
                                    checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout,
                                    colors,
                                    colors.windowText);
                            }

                            resultBounds = layout.checkBounds;
                        }
                        else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex
                            && DataGridView.MouseEnteredCellAddress.X == ColumnIndex
                            && mouseInContentBounds)
                        {
                            // paint over

                            ButtonBaseAdapter.LayoutOptions options = CheckBoxPopupAdapter.PaintPopupLayout(
                                show3D: true,
                                checkBoxSize.Width,
                                checkBounds,
                                Padding.Empty,
                                isDefault: false,
                                cellStyle.Font,
                                string.Empty,
                                DataGridView.Enabled,
                                DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                DataGridView.RightToLeft);

                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();

                            if (paint && PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout.checkBounds,
                                    colors.options.HighContrast ? colors.buttonFace : colors.highlight,
                                    disabledColors: true);

                                CheckBoxBaseAdapter.DrawPopupBorder(g, layout.checkBounds, colors);
                                CheckBoxBaseAdapter.DrawCheckOnly(
                                    checkBoxSize.Width,
                                    checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout,
                                    colors,
                                    colors.windowText);
                            }
                            resultBounds = layout.checkBounds;
                        }
                        else
                        {
                            // paint up
                            ButtonBaseAdapter.LayoutOptions options = CheckBoxPopupAdapter.PaintPopupLayout(
                                show3D: false,
                                checkBoxSize.Width,
                                checkBounds,
                                Padding.Empty,
                                false,
                                cellStyle.Font,
                                string.Empty,
                                DataGridView.Enabled,
                                DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                DataGridView.RightToLeft);

                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();

                            if (paint && PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(
                                    g,
                                    cellStyle.ForeColor,
                                    cellStyle.BackColor,
                                    DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout.checkBounds,
                                    colors.options.HighContrast ? colors.buttonFace : colors.highlight,
                                    disabledColors: true);

                                ControlPaint.DrawBorderSimple(g, layout.checkBounds, colors.buttonShadow);
                                CheckBoxBaseAdapter.DrawCheckOnly(
                                    checkBoxSize.Width,
                                    checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                    DataGridView.Enabled,
                                    checkState,
                                    g,
                                    layout,
                                    colors,
                                    colors.windowText);
                            }

                            resultBounds = layout.checkBounds;
                        }
                    }
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
                Debug.Assert(valBounds.Width < checkBoxSize.Width || valBounds.Height < checkBoxSize.Height, "the bounds are empty");
                resultBounds = Rectangle.Empty;
            }

            if (paint && PaintErrorIcon(paintParts) && drawErrorText && DataGridView.ShowCellErrors)
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        public override object ParseFormattedValue(
            object formattedValue,
            DataGridViewCellStyle cellStyle,
            TypeConverter formattedValueTypeConverter,
            TypeConverter valueTypeConverter)
        {
            Debug.Assert(formattedValue is null || FormattedValueType is null || FormattedValueType.IsAssignableFrom(formattedValue.GetType()));

            if (formattedValue != null)
            {
                if (formattedValue is bool boolean)
                {
                    if (boolean)
                    {
                        if (TrueValue != null)
                        {
                            return TrueValue;
                        }
                        else if (ValueType != null && ValueType.IsAssignableFrom(defaultBooleanType))
                        {
                            return true;
                        }
                        else if (ValueType != null && ValueType.IsAssignableFrom(defaultCheckStateType))
                        {
                            return CheckState.Checked;
                        }
                    }
                    else
                    {
                        if (FalseValue != null)
                        {
                            return FalseValue;
                        }
                        else if (ValueType != null && ValueType.IsAssignableFrom(defaultBooleanType))
                        {
                            return false;
                        }
                        else if (ValueType != null && ValueType.IsAssignableFrom(defaultCheckStateType))
                        {
                            return CheckState.Unchecked;
                        }
                    }
                }
                else if (formattedValue is CheckState state)
                {
                    switch (state)
                    {
                        case CheckState.Checked:
                            if (TrueValue != null)
                            {
                                return TrueValue;
                            }
                            else if (ValueType != null && ValueType.IsAssignableFrom(defaultBooleanType))
                            {
                                return true;
                            }
                            else if (ValueType != null && ValueType.IsAssignableFrom(defaultCheckStateType))
                            {
                                return CheckState.Checked;
                            }
                            break;
                        case CheckState.Unchecked:
                            if (FalseValue != null)
                            {
                                return FalseValue;
                            }
                            else if (ValueType != null && ValueType.IsAssignableFrom(defaultBooleanType))
                            {
                                return false;
                            }
                            else if (ValueType != null && ValueType.IsAssignableFrom(defaultCheckStateType))
                            {
                                return CheckState.Unchecked;
                            }
                            break;
                        case CheckState.Indeterminate:
                            if (IndeterminateValue != null)
                            {
                                return IndeterminateValue;
                            }
                            else if (ValueType != null && ValueType.IsAssignableFrom(defaultCheckStateType))
                            {
                                return CheckState.Indeterminate;
                            }
                            /* case where this.ValueType.IsAssignableFrom(defaultBooleanType) is treated in base.ParseFormattedValue */
                            break;
                    }
                }
            }
            return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }

        private bool SwitchFormattedValue()
        {
            if (FormattedValueType is null)
            {
                return false;
            }
            IDataGridViewEditingCell editingCell = (IDataGridViewEditingCell)this;
            if (FormattedValueType.IsAssignableFrom(typeof(CheckState)))
            {
                if ((flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00)
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Indeterminate;
                }
                else if ((flags & DATAGRIDVIEWCHECKBOXCELL_indeterminate) != 0x00)
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Unchecked;
                }
                else
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Checked;
                }
            }
            else if (FormattedValueType.IsAssignableFrom(defaultBooleanType))
            {
                editingCell.EditingCellFormattedValue = !((bool)editingCell.GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting));
            }
            return true;
        }

        /// <summary>
        ///  Gets the row Index and column Index of the cell.
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewCheckBoxCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            ButtonState = newButtonState;
            DataGridView.InvalidateCell(ColumnIndex, rowIndex);
        }
    }
}
