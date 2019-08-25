// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a checkbox cell in the DataGridView.
    /// </summary>
    public class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
    {
        private static readonly DataGridViewContentAlignment anyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;
        private static readonly DataGridViewContentAlignment anyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;
        private static readonly DataGridViewContentAlignment anyMiddle = DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.MiddleLeft;

        private static readonly VisualStyleElement CheckBoxElement = VisualStyleElement.Button.CheckBox.UncheckedNormal;
        private static readonly int PropButtonCellState = PropertyStore.CreateKey();
        private static readonly int PropTrueValue = PropertyStore.CreateKey();
        private static readonly int PropFalseValue = PropertyStore.CreateKey();
        private static readonly int PropFlatStyle = PropertyStore.CreateKey();
        private static readonly int PropIndeterminateValue = PropertyStore.CreateKey();
        private static Bitmap checkImage = null;

        private const byte DATAGRIDVIEWCHECKBOXCELL_threeState = 0x01;
        private const byte DATAGRIDVIEWCHECKBOXCELL_valueChanged = 0x02;
        private const byte DATAGRIDVIEWCHECKBOXCELL_checked = 0x10;
        private const byte DATAGRIDVIEWCHECKBOXCELL_indeterminate = 0x20;

        private const byte DATAGRIDVIEWCHECKBOXCELL_margin = 2;  // horizontal and vertical margins for preferred sizes

        private byte flags;  // see DATAGRIDVIEWCHECKBOXCELL_ consts above
        private static bool mouseInContentBounds = false;
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
                if (FormattedValueType == null)
                {
                    throw new ArgumentException(SR.DataGridViewCell_FormattedValueTypeNull);
                }
                if (value == null || !FormattedValueType.IsAssignableFrom(value.GetType()))
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
            if (FormattedValueType == null)
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
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null || rowIndex < 0 || OwningColumn == null)
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

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null ||
                rowIndex < 0 ||
                OwningColumn == null ||
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
            if (DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
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
                                                   borderAndPaddingWidths + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
                if (freeDimension != DataGridViewFreeDimension.Width)
                {
                    preferredSize.Height = Math.Max(preferredSize.Height,
                                                    borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
                NotifyMASSClient(new Point(ColumnIndex, rowIndex));
            }
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
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
            if (DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                UpdateButtonState(ButtonState & ~ButtonState.Pushed, e.RowIndex);
                NotifyMASSClient(new Point(e.ColumnIndex, e.RowIndex));
            }
        }

        private void NotifyMASSClient(Point position)
        {
            Debug.Assert((position.X >= 0) && (position.X < DataGridView.Columns.Count));
            Debug.Assert((position.Y >= 0) && (position.Y < DataGridView.Rows.Count));

            int visibleRowIndex = DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, position.Y);
            int visibleColumnIndex = DataGridView.Columns.ColumnIndexToActualDisplayIndex(position.X, DataGridViewElementStates.Visible);

            int topHeaderRowIncrement = DataGridView.ColumnHeadersVisible ? 1 : 0;
            int rowHeaderIncrement = DataGridView.RowHeadersVisible ? 1 : 0;

            int objectID = visibleRowIndex + topHeaderRowIncrement  // + 1 because the top header row acc obj is at index 0
                                           + 1;                     // + 1 because objectID's need to be positive and non-zero

            int childID = visibleColumnIndex + rowHeaderIncrement;  // + 1 because the column header cell is at index 0 in top header row acc obj
                                                                    //     same thing for the row header cell in the data grid view row acc obj

            (DataGridView.AccessibilityObject as Control.ControlAccessibleObject).NotifyClients(AccessibleEvents.StateChange, objectID, childID);
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

            Rectangle resultBounds;

            if (paint && DataGridViewCell.PaintBorder(paintParts))
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
            SolidBrush br = DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
            {
                g.FillRectangle(br, valBounds);
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
                DataGridViewCell.PaintFocus(paintParts) &&
                DataGridView.ShowFocusCues &&
                DataGridView.Focused &&
                ptCurrentCell.X == ColumnIndex &&
                ptCurrentCell.Y == rowIndex)
            {
                // Draw focus rectangle
                ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, br.Color);
            }

            Rectangle errorBounds = valBounds;

            valBounds.Inflate(-DATAGRIDVIEWCHECKBOXCELL_margin, -DATAGRIDVIEWCHECKBOXCELL_margin);

            Size checkBoxSize;

            CheckBoxState themeCheckBoxState = CheckBoxState.UncheckedNormal;

            if (DataGridView.ApplyVisualStylesToInnerCells)
            {
                themeCheckBoxState = CheckBoxRenderer.ConvertFromButtonState(bs, drawAsMixedCheckBox,
                    DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                    DataGridView.MouseEnteredCellAddress.X == ColumnIndex &&
                    mouseInContentBounds);
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
                int checkBoxX = 0, checkBoxY = 0;
                if ((!DataGridView.RightToLeftInternal && (cellStyle.Alignment & anyRight) != 0) ||
                    (DataGridView.RightToLeftInternal && (cellStyle.Alignment & anyLeft) != 0))
                {
                    checkBoxX = valBounds.Right - checkBoxSize.Width;
                }
                else if ((cellStyle.Alignment & anyCenter) != 0)
                {
                    checkBoxX = valBounds.Left + (valBounds.Width - checkBoxSize.Width) / 2;
                }
                else
                {
                    checkBoxX = valBounds.Left;
                }

                if ((cellStyle.Alignment & anyBottom) != 0)
                {
                    checkBoxY = valBounds.Bottom - checkBoxSize.Height;
                }
                else if ((cellStyle.Alignment & anyMiddle) != 0)
                {
                    checkBoxY = valBounds.Top + (valBounds.Height - checkBoxSize.Height) / 2;
                }
                else
                {
                    checkBoxY = valBounds.Top;
                }

                if (DataGridView.ApplyVisualStylesToInnerCells && FlatStyle != FlatStyle.Flat && FlatStyle != FlatStyle.Popup)
                {
                    if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                    {
                        DataGridViewCheckBoxCellRenderer.DrawCheckBox(g,
                            new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height),
                            (int)themeCheckBoxState);
                    }

                    resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);
                }
                else
                {
                    if (FlatStyle == FlatStyle.System || FlatStyle == FlatStyle.Standard)
                    {
                        if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            if (drawAsMixedCheckBox)
                            {
                                ControlPaint.DrawMixedCheckBox(g, checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height, bs);
                            }
                            else
                            {
                                ControlPaint.DrawCheckBox(g, checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height, bs);
                            }
                        }

                        resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);
                    }
                    else if (FlatStyle == FlatStyle.Flat)
                    {
                        // CheckBox::Paint will only paint the check box differently when in FlatStyle.Flat
                        // this code is copied from CheckBox::DrawCheckFlat. it was a lot of trouble making this function static

                        Rectangle checkBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);

                        SolidBrush foreBrush = null;
                        SolidBrush backBrush = null;
                        Color highlight = Color.Empty;

                        if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            foreBrush = DataGridView.GetCachedBrush(cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor);
                            backBrush = DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                            highlight = ControlPaint.LightLight(backBrush.Color);

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
                                highlight = Color.FromArgb(ButtonInternal.ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.R),
                                                           ButtonInternal.ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.G),
                                                           ButtonInternal.ButtonBaseAdapter.ColorOptions.Adjust255(adjust, highlight.B));
                            }
                            highlight = g.GetNearestColor(highlight);

                            using (Pen pen = new Pen(foreBrush.Color))
                            {
                                g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Right - 1, checkBounds.Top);
                                g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Left, checkBounds.Bottom - 1);
                            }
                        }

                        checkBounds.Inflate(-1, -1);
                        checkBounds.Width++;
                        checkBounds.Height++;

                        if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            if (checkState == CheckState.Indeterminate)
                            {
                                ButtonInternal.ButtonBaseAdapter.DrawDitheredFill(g, backBrush.Color, highlight, checkBounds);
                            }
                            else
                            {
                                using (SolidBrush highBrush = new SolidBrush(highlight))
                                {
                                    g.FillRectangle(highBrush, checkBounds);
                                }
                            }

                            // draw the check box
                            if (checkState != CheckState.Unchecked)
                            {
                                Rectangle fullSize = new Rectangle(checkBoxX - 1, checkBoxY - 1, checkBoxSize.Width + 3, checkBoxSize.Height + 3);
                                fullSize.Width++;
                                fullSize.Height++;

                                if (checkImage == null || checkImage.Width != fullSize.Width || checkImage.Height != fullSize.Height)
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
                                        IntPtr dc = offscreen.GetHdc();
                                        try
                                        {
                                            SafeNativeMethods.DrawFrameControl(new HandleRef(offscreen, dc), ref rcCheck,
                                                                               NativeMethods.DFC_MENU, NativeMethods.DFCS_MENUCHECK);
                                        }
                                        finally
                                        {
                                            offscreen.ReleaseHdcInternal(dc);
                                        }
                                    }
                                    bitmap.MakeTransparent();
                                    checkImage = bitmap;
                                }
                                fullSize.Y--;
                                ControlPaint.DrawImageColorized(g, checkImage, fullSize, checkState == CheckState.Indeterminate ? ControlPaint.LightLight(foreBrush.Color) : foreBrush.Color);
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
                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.CheckBoxPopupAdapter.PaintPopupLayout(g,
                                                                                                                        true /*show3D*/,
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

                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(DataGridView.Enabled,
                                                                        checkState,
                                                                        g,
                                                                        layout.checkBounds,
                                                                        colors.windowText,
                                                                        colors.buttonFace,
                                                                        true /*disabledColors*/,
                                                                        colors);
                                CheckBoxBaseAdapter.DrawPopupBorder(g, layout.checkBounds, colors);
                                CheckBoxBaseAdapter.DrawCheckOnly(checkBoxSize.Width,
                                                                    checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                                                    DataGridView.Enabled,
                                                                    checkState,
                                                                    g,
                                                                    layout,
                                                                    colors,
                                                                    colors.windowText,
                                                                    colors.buttonFace);
                            }
                            resultBounds = layout.checkBounds;
                        }
                        else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                 DataGridView.MouseEnteredCellAddress.X == ColumnIndex &&
                                 mouseInContentBounds)
                        {
                            // paint over

                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.CheckBoxPopupAdapter.PaintPopupLayout(g,
                                                                                                                        true /*show3D*/,
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

                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(DataGridView.Enabled,
                                                                        checkState,
                                                                        g,
                                                                        layout.checkBounds,
                                                                        colors.windowText,
                                                                        colors.options.highContrast ? colors.buttonFace : colors.highlight,
                                                                        true /*disabledColors*/,
                                                                        colors);
                                CheckBoxBaseAdapter.DrawPopupBorder(g, layout.checkBounds, colors);
                                CheckBoxBaseAdapter.DrawCheckOnly(checkBoxSize.Width,
                                                                    checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                                                    DataGridView.Enabled,
                                                                    checkState,
                                                                    g,
                                                                    layout,
                                                                    colors,
                                                                    colors.windowText,
                                                                    colors.highlight);
                            }
                            resultBounds = layout.checkBounds;
                        }
                        else
                        {
                            // paint up
                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.CheckBoxPopupAdapter.PaintPopupLayout(g,
                                                                                                                       false /*show3D*/,
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

                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(DataGridView.Enabled,
                                                                        checkState,
                                                                        g,
                                                                        layout.checkBounds,
                                                                        colors.windowText,
                                                                        colors.options.highContrast ? colors.buttonFace : colors.highlight,
                                                                        true /*disabledColors*/,
                                                                        colors);
                                ButtonBaseAdapter.DrawFlatBorder(g, layout.checkBounds, colors.buttonShadow);
                                CheckBoxBaseAdapter.DrawCheckOnly(checkBoxSize.Width,
                                                                  checkState == CheckState.Checked || checkState == CheckState.Indeterminate,
                                                                  DataGridView.Enabled,
                                                                  checkState,
                                                                  g,
                                                                  layout,
                                                                  colors,
                                                                  colors.windowText,
                                                                  colors.highlight);
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

            if (paint && DataGridViewCell.PaintErrorIcon(paintParts) && drawErrorText && DataGridView.ShowCellErrors)
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        public override object ParseFormattedValue(object formattedValue,
                                                   DataGridViewCellStyle cellStyle,
                                                   TypeConverter formattedValueTypeConverter,
                                                   TypeConverter valueTypeConverter)
        {
            Debug.Assert(formattedValue == null || FormattedValueType == null || FormattedValueType.IsAssignableFrom(formattedValue.GetType()));

            if (formattedValue != null)
            {
                if (formattedValue is bool)
                {
                    if ((bool)formattedValue)
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
                else if (formattedValue is CheckState)
                {
                    switch ((CheckState)formattedValue)
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
            if (FormattedValueType == null)
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

        private class DataGridViewCheckBoxCellRenderer
        {
            static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewCheckBoxCellRenderer()
            {
            }

            public static VisualStyleRenderer CheckBoxRenderer
            {
                get
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(CheckBoxElement);
                    }
                    return visualStyleRenderer;
                }
            }

            public static void DrawCheckBox(Graphics g, Rectangle bounds, int state)
            {
                CheckBoxRenderer.SetParameters(CheckBoxElement.ClassName, CheckBoxElement.Part, (int)state);
                CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }

        protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation

            public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            public override AccessibleStates State
            {
                get
                {
                    if ((Owner as DataGridViewCheckBoxCell).EditedFormattedValue is CheckState state)
                    {
                        switch (state)
                        {
                            case CheckState.Checked:
                                return AccessibleStates.Checked | base.State;
                            case CheckState.Indeterminate:
                                return AccessibleStates.Indeterminate | base.State;
                        }
                    }
                    else if ((Owner as DataGridViewCheckBoxCell).EditedFormattedValue is bool stateAsBool)
                    {
                        if (stateAsBool)
                        {
                            return AccessibleStates.Checked | base.State;
                        }
                    }
                    return base.State;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (!Owner.ReadOnly)
                    {
                        // determine if we switch to Checked/Unchecked value
                        bool switchToCheckedState = true;

                        object formattedValue = Owner.FormattedValue;

                        if (formattedValue is CheckState)
                        {
                            switchToCheckedState = ((CheckState)formattedValue) == CheckState.Unchecked;
                        }
                        else if (formattedValue is bool)
                        {
                            switchToCheckedState = !((bool)formattedValue);
                        }

                        if (switchToCheckedState)
                        {
                            return SR.DataGridView_AccCheckBoxCellDefaultActionCheck;
                        }
                        else
                        {
                            return SR.DataGridView_AccCheckBoxCellDefaultActionUncheck;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override void DoDefaultAction()
            {
                DataGridViewCheckBoxCell dataGridViewCell = (DataGridViewCheckBoxCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                if (!dataGridViewCell.ReadOnly && dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.CurrentCell = dataGridViewCell;
                    bool endEditMode = false;
                    if (!dataGridView.IsCurrentCellInEditMode)
                    {
                        endEditMode = true;
                        dataGridView.BeginEdit(false /*selectAll*/);
                    }
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        if (dataGridViewCell.SwitchFormattedValue())
                        {
                            dataGridViewCell.NotifyDataGridViewOfValueChange();
                            dataGridView.InvalidateCell(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex);

                            // notify MSAA clients that the default action changed
                            if (Owner is DataGridViewCheckBoxCell checkBoxCell)
                            {
                                checkBoxCell.NotifyMASSClient(new Point(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                            }
                        }
                        if (endEditMode)
                        {
                            dataGridView.EndEdit();
                        }
                    }
                }
            }

            public override int GetChildCount()
            {
                return 0;
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_IsTogglePatternAvailablePropertyId)
                {
                    return (object)IsPatternSupported(NativeMethods.UIA_TogglePatternId);
                }
                else if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_CheckBoxControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_TogglePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void Toggle()
            {
                DoDefaultAction();
            }

            internal override UnsafeNativeMethods.ToggleState ToggleState
            {
                get
                {
                    bool toggledState = true;
                    object formattedValue = Owner.FormattedValue;

                    if (formattedValue is CheckState)
                    {
                        toggledState = ((CheckState)formattedValue) == CheckState.Checked;
                    }
                    else if (formattedValue is bool)
                    {
                        toggledState = ((bool)formattedValue);
                    }
                    else
                    {
                        return UnsafeNativeMethods.ToggleState.ToggleState_Indeterminate;
                    }

                    return toggledState ? UnsafeNativeMethods.ToggleState.ToggleState_On : UnsafeNativeMethods.ToggleState.ToggleState_Off;
                }
            }
        }
    }
}
