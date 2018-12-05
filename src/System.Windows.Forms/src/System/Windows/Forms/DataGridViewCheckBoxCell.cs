// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms.VisualStyles;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms.Internal;
    using System.Windows.Forms.ButtonInternal;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell"]/*' />
    /// <devdoc>
    ///    <para>Identifies a checkbox cell in the DataGridView.</para>
    /// </devdoc>
    public class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
    {
        private static readonly DataGridViewContentAlignment anyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;
        private static readonly DataGridViewContentAlignment anyBottom  = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;
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
        private static Type defaultCheckStateType = typeof(System.Windows.Forms.CheckState);
        private static Type defaultBooleanType = typeof(System.Boolean);
        private static Type cellType = typeof(DataGridViewCheckBoxCell);

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.DataGridViewCheckBoxCell"]/*' />
        public DataGridViewCheckBoxCell() : this(false /*threeState*/)
        {
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.DataGridViewCheckBoxCell2"]/*' />
        public DataGridViewCheckBoxCell(bool threeState)
        {
            if (threeState)
            {
                this.flags = DATAGRIDVIEWCHECKBOXCELL_threeState;
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.IDataGridViewEditingCell.EditingCellFormattedValue"]/*' />
        public virtual object EditingCellFormattedValue
        {
            get
            {
                return GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                if (this.FormattedValueType == null)
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewCell_FormattedValueTypeNull));
                }
                if (value == null || !this.FormattedValueType.IsAssignableFrom(value.GetType()))
                {
                    // Assigned formatted value may not be of the good type, in cases where the app
                    // is feeding wrong values to the cell in virtual / databound mode.
                    throw new ArgumentException(string.Format(SR.DataGridViewCheckBoxCell_InvalidValueType));
                }
                if (value is System.Windows.Forms.CheckState)
                {
                    if (((System.Windows.Forms.CheckState)value) == System.Windows.Forms.CheckState.Checked)
                    {
                        this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_checked;
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                    }
                    else if (((System.Windows.Forms.CheckState)value) == System.Windows.Forms.CheckState.Indeterminate)
                    {
                        this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_indeterminate;
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                    }
                }
                else if (value is System.Boolean)
                {
                    if ((bool)value)
                    {
                        this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_checked;
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_checked);
                    }

                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_indeterminate);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewCheckBoxCell_InvalidValueType));
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.IDataGridViewEditingCell.EditingCellValueChanged"]/*' />
        public virtual bool EditingCellValueChanged
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWCHECKBOXCELL_valueChanged) != 0x00);
            }
            set
            {
                if (value)
                {
                    this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_valueChanged;
                }
                else
                {
                    this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_valueChanged);
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.IDataGridViewEditingCell.GetEditingCellFormattedValue"]/*' />
        public virtual object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
        {
            if (this.FormattedValueType == null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCell_FormattedValueTypeNull));
            }
            if (this.FormattedValueType.IsAssignableFrom(defaultCheckStateType))
            {
                if ((this.flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00)
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return string.Format(SR.DataGridViewCheckBoxCell_ClipboardChecked);
                    }
                    return System.Windows.Forms.CheckState.Checked;
                }
                else if ((this.flags & DATAGRIDVIEWCHECKBOXCELL_indeterminate) != 0x00)
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return string.Format(SR.DataGridViewCheckBoxCell_ClipboardIndeterminate);
                    }
                    return System.Windows.Forms.CheckState.Indeterminate;
                }
                else
                {
                    if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                    {
                        return string.Format(SR.DataGridViewCheckBoxCell_ClipboardUnchecked);
                    }
                    return System.Windows.Forms.CheckState.Unchecked;
                }
            }
            else if (this.FormattedValueType.IsAssignableFrom(defaultBooleanType))
            {
                bool ret = (bool)((this.flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00);
                if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                {
                    return string.Format(ret ? SR.DataGridViewCheckBoxCell_ClipboardTrue : SR.DataGridViewCheckBoxCell_ClipboardFalse);
                }
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.IDataGridViewEditingCell.PrepareEditingCellForEdit"]/*' />
        public virtual void PrepareEditingCellForEdit(bool selectAll)
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.EditType"]/*' />
        public override Type EditType
        {
            get
            {
                // Check boxes can't switch to edit mode
                // This cell type must implement the IEditingCell interface
                return null;
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.FalseValue"]/*' />
        [DefaultValue(null)]
        public object FalseValue
        {
            get
            {
                return this.Properties.GetObject(PropFalseValue);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropFalseValue))
                {
                    this.Properties.SetObject(PropFalseValue, value);
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object FalseValueInternal
        {
            set
            {
                if (value != null || this.Properties.ContainsObject(PropFalseValue))
                {
                    this.Properties.SetObject(PropFalseValue, value);
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.FlatStyle"]/*' />
        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get
            {
                bool found;
                int flatStyle = this.Properties.GetInteger(PropFlatStyle, out found);
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
                    this.Properties.SetInteger(PropFlatStyle, (int)value);
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
                    this.Properties.SetInteger(PropFlatStyle, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.FormattedValueType"]/*' />
        public override Type FormattedValueType
        {
            get
            {
                if (this.ThreeState)
                {
                    return defaultCheckStateType;
                }
                else
                {
                    return defaultBooleanType;
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.IndeterminateValue"]/*' />
        [DefaultValue(null)]
        public object IndeterminateValue
        {
            get
            {
                return this.Properties.GetObject(PropIndeterminateValue);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropIndeterminateValue))
                {
                    this.Properties.SetObject(PropIndeterminateValue, value);
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object IndeterminateValueInternal
        {
            set
            {
                if (value != null || this.Properties.ContainsObject(PropIndeterminateValue))
                {
                    this.Properties.SetObject(PropIndeterminateValue, value);
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ThreeState"]/*' />
        [DefaultValue(false)]
        public bool ThreeState
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWCHECKBOXCELL_threeState) != 0x00);
            }
            set
            {
                if (this.ThreeState != value)
                {
                    this.ThreeStateInternal = value;
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
                        }
                    }
                }
            }
        }

        internal bool ThreeStateInternal
        {
            set
            {
                if (this.ThreeState != value)
                {
                    if (value)
                    {
                        this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_threeState;
                    }
                    else
                    {
                        this.flags = (byte)(this.flags & ~DATAGRIDVIEWCHECKBOXCELL_threeState);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.TrueValue"]/*' />
        [DefaultValue(null)]
        public object TrueValue
        {
            get
            {
                return this.Properties.GetObject(PropTrueValue);
            }
            set
            {
                if (value != null || this.Properties.ContainsObject(PropTrueValue))
                {
                    this.Properties.SetObject(PropTrueValue, value);
                    if (this.DataGridView != null)
                    {
                        if (this.RowIndex != -1)
                        {
                            this.DataGridView.InvalidateCell(this);
                        }
                        else
                        {
                            this.DataGridView.InvalidateColumnInternal(this.ColumnIndex);
                        }
                    }
                }
            }
        }

        internal object TrueValueInternal
        {
            set
            {
                if (value != null || this.Properties.ContainsObject(PropTrueValue))
                {
                    this.Properties.SetObject(PropTrueValue, value);
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ValueType"]/*' />
        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }

                if (this.ThreeState)
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
                this.ThreeState = (value != null && defaultCheckStateType.IsAssignableFrom(value));
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewCheckBoxCell dataGridViewCell;
            Type thisType = this.GetType();
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
            dataGridViewCell.ThreeStateInternal = this.ThreeState;
            dataGridViewCell.TrueValueInternal = this.TrueValue;
            dataGridViewCell.FalseValueInternal = this.FalseValue;
            dataGridViewCell.IndeterminateValueInternal = this.IndeterminateValue;
            dataGridViewCell.FlatStyleInternal = this.FlatStyle;
            return dataGridViewCell;
        }

        private bool CommonContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            return ptCurrentCell.X == this.ColumnIndex &&
                   ptCurrentCell.Y == e.RowIndex &&
                   this.DataGridView.IsCurrentCellInEditMode;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ContentClickUnsharesRow"]/*' />
        protected override bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return CommonContentClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ContentDoubleClickUnsharesRow"]/*' />
        protected override bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
        {
            return CommonContentClickUnsharesRow(e);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewCheckBoxCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.GetContentBounds"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.GetErrorIconBounds"]/*' />
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
            
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == this.ColumnIndex &&
                ptCurrentCell.Y == rowIndex && this.DataGridView.IsCurrentCellInEditMode)
            {
                // PaintPrivate does not paint the error icon if this is the current cell.
                // So don't set the ErrorIconBounds either.
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
                true  /*computeErrorIconBound*/,
                false /*paint*/);

            return errorIconBounds;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.GetFormattedValue"]/*' />
        protected override object GetFormattedValue(object value,
                                                    int rowIndex,
                                                    ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            if (value != null)
            {
                if (this.ThreeState)
                {
                    if (value.Equals(this.TrueValue) ||
                        (value is int && (int)value == (int)CheckState.Checked))
                    {
                        value = CheckState.Checked;
                    }
                    else if (value.Equals(this.FalseValue) ||
                             (value is int && (int)value == (int)CheckState.Unchecked))
                    {
                        value = CheckState.Unchecked;
                    }
                    else if (value.Equals(this.IndeterminateValue) ||
                             (value is int && (int)value == (int)CheckState.Indeterminate))
                    {
                        value = CheckState.Indeterminate;
                    }
                }
                else
                {
                    if (value.Equals(this.TrueValue) ||
                        (value is int && (int)value != 0))
                    {
                        value = true;
                    }
                    else if (value.Equals(this.FalseValue) ||
                             (value is int && (int)value == 0))
                    {
                        value = false;
                    }
                }
            }

            object ret = base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            if (ret != null && (context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
            {
                if (ret is bool)
                {
                    bool retBool = (bool) ret;
                    if (retBool)
                    {
                        return string.Format(this.ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardChecked : SR.DataGridViewCheckBoxCell_ClipboardTrue);
                    }
                    else
                    {
                        return string.Format(this.ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardUnchecked : SR.DataGridViewCheckBoxCell_ClipboardFalse);
                    }
                }
                else if (ret is CheckState)
                {
                    CheckState retCheckState = (CheckState) ret;
                    if (retCheckState == CheckState.Checked)
                    {
                        return string.Format(this.ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardChecked : SR.DataGridViewCheckBoxCell_ClipboardTrue);
                    }
                    else if (retCheckState == CheckState.Unchecked)
                    {
                        return string.Format(this.ThreeState ? SR.DataGridViewCheckBoxCell_ClipboardUnchecked : SR.DataGridViewCheckBoxCell_ClipboardFalse);
                    }
                    else
                    {
                        Debug.Assert(retCheckState == CheckState.Indeterminate);
                        return string.Format(SR.DataGridViewCheckBoxCell_ClipboardIndeterminate);
                    }
                }
            }
            return ret;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.GetPreferredSize"]/*' />
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

            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            Rectangle borderWidthsRect = this.StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            Size preferredSize;
            if (this.DataGridView.ApplyVisualStylesToInnerCells)
            {
                // Assuming here that all checkbox states use the same size. We should take the largest of the state specific sizes.
                Size checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
                switch (this.FlatStyle)
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
                switch (this.FlatStyle)
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
            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);
            Rectangle borderWidths = BorderWidths(dgvabsEffective);
            preferredSize.Width += borderWidths.X;
            preferredSize.Height += borderWidths.Y;

            if (this.DataGridView.ShowCellErrors)
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.KeyDownUnsharesRow"]/*' />
        protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.KeyUpUnsharesRow"]/*' />
        protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            return e.KeyCode == Keys.Space;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.MouseDownUnsharesRow"]/*' />
        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.MouseEnterUnsharesRow"]/*' />
        protected override bool MouseEnterUnsharesRow(int rowIndex)
        {
            return this.ColumnIndex == this.DataGridView.MouseDownCellAddress.X && rowIndex == this.DataGridView.MouseDownCellAddress.Y;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.MouseLeaveUnsharesRow"]/*' />
        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return (this.ButtonState & ButtonState.Pushed) != 0;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.MouseUpUnsharesRow"]/*' />
        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        private void NotifyDataGridViewOfValueChange()
        {
            this.flags |= (byte)DATAGRIDVIEWCHECKBOXCELL_valueChanged;
            this.DataGridView.NotifyCurrentCellDirty(true);
        }

        private void OnCommonContentClick(DataGridViewCellEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == this.ColumnIndex &&
                ptCurrentCell.Y == e.RowIndex &&
                this.DataGridView.IsCurrentCellInEditMode)
            {
                if (SwitchFormattedValue())
                {
                    NotifyDataGridViewOfValueChange();
                }
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnContentClick"]/*' />
        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            OnCommonContentClick(e);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnContentDoubleClick"]/*' />
        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            OnCommonContentClick(e);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnKeyDown"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnKeyUp"]/*' />
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
                NotifyMASSClient(new Point(this.ColumnIndex, rowIndex));
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnLeave"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnMouseDown"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnMouseLeave"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnMouseMove"]/*' />
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

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.OnMouseUp"]/*' />
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                UpdateButtonState(this.ButtonState & ~ButtonState.Pushed, e.RowIndex);
                NotifyMASSClient(new Point(e.ColumnIndex, e.RowIndex));
            }
        }

        private void NotifyMASSClient(Point position)
        {
            Debug.Assert((position.X >= 0) && (position.X < this.DataGridView.Columns.Count));
            Debug.Assert((position.Y >= 0) && (position.Y < this.DataGridView.Rows.Count));

            int visibleRowIndex = this.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, position.Y);
            int visibleColumnIndex = this.DataGridView.Columns.ColumnIndexToActualDisplayIndex(position.X, DataGridViewElementStates.Visible);

            int topHeaderRowIncrement = this.DataGridView.ColumnHeadersVisible ? 1 : 0;
            int rowHeaderIncrement = this.DataGridView.RowHeadersVisible ? 1 : 0;

            int objectID = visibleRowIndex + topHeaderRowIncrement  // + 1 because the top header row acc obj is at index 0
                                           + 1;                     // + 1 because objectID's need to be positive and non-zero

            int childID = visibleColumnIndex + rowHeaderIncrement;  // + 1 because the column header cell is at index 0 in top header row acc obj
                                                                    //     same thing for the row header cell in the data grid view row acc obj

            (this.DataGridView.AccessibilityObject as Control.ControlAccessibleObject).NotifyClients(AccessibleEvents.StateChange, objectID, childID);
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.Paint"]/*' />
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
            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == this.ColumnIndex && 
                ptCurrentCell.Y == rowIndex && this.DataGridView.IsCurrentCellInEditMode)
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
            if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
            {
                bs |= ButtonState.Pushed;
            }
            SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
            {
                g.FillRectangle(br, valBounds);
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

            if (paint && 
                DataGridViewCell.PaintFocus(paintParts) &&
                this.DataGridView.ShowFocusCues && 
                this.DataGridView.Focused && 
                ptCurrentCell.X == this.ColumnIndex && 
                ptCurrentCell.Y == rowIndex)
            {
                // Draw focus rectangle
                ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, br.Color);
            }

            Rectangle errorBounds = valBounds;

            valBounds.Inflate(-DATAGRIDVIEWCHECKBOXCELL_margin, -DATAGRIDVIEWCHECKBOXCELL_margin);
            
            Size checkBoxSize;

            CheckBoxState themeCheckBoxState = CheckBoxState.UncheckedNormal;

            if (this.DataGridView.ApplyVisualStylesToInnerCells)
            {
                themeCheckBoxState = CheckBoxRenderer.ConvertFromButtonState(bs, drawAsMixedCheckBox, 
                    this.DataGridView.MouseEnteredCellAddress.Y == rowIndex && 
                    this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex && 
                    mouseInContentBounds);
                checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, themeCheckBoxState);
                switch (this.FlatStyle)
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
                switch (this.FlatStyle)
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
                if ((!this.DataGridView.RightToLeftInternal && (cellStyle.Alignment & anyRight) != 0) ||
                    (this.DataGridView.RightToLeftInternal && (cellStyle.Alignment & anyLeft) != 0))
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

                if (this.DataGridView.ApplyVisualStylesToInnerCells && this.FlatStyle != FlatStyle.Flat && this.FlatStyle != FlatStyle.Popup)
                {
                    if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                    {
                        DataGridViewCheckBoxCellRenderer.DrawCheckBox(g, 
                            new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height), 
                            (int) themeCheckBoxState);
                    }

                    resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);
                }
                else
                {
                    if (this.FlatStyle == FlatStyle.System || this.FlatStyle == FlatStyle.Standard)
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
                    else if (this.FlatStyle == FlatStyle.Flat)
                    {
                        // CheckBox::Paint will only paint the check box differently when in FlatStyle.Flat
                        // this code is copied from CheckBox::DrawCheckFlat. it was a lot of trouble making this function static

                        Rectangle checkBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);

                        SolidBrush foreBrush = null;
                        SolidBrush backBrush = null;
                        Color highlight = Color.Empty;

                        if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            foreBrush = this.DataGridView.GetCachedBrush(cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor);
                            backBrush = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                            highlight = ControlPaint.LightLight(backBrush.Color);

                            if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
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
                                g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Right-1, checkBounds.Top);
                                g.DrawLine(pen, checkBounds.Left, checkBounds.Top, checkBounds.Left, checkBounds.Bottom-1);
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
                                Rectangle fullSize = new Rectangle(checkBoxX-1, checkBoxY-1, checkBoxSize.Width+3, checkBoxSize.Height+3);
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
                                    NativeMethods.RECT rcCheck = NativeMethods.RECT.FromXYWH(0, 0, fullSize.Width, fullSize.Height);
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
                        Debug.Assert(this.FlatStyle == FlatStyle.Popup);

                        Rectangle checkBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width - 1, checkBoxSize.Height - 1);

                        // The CheckBoxAdapter code moves the check box down about 3 pixels so we have to take that into account
                        checkBounds.Y -= 3;

                        if ((this.ButtonState & (ButtonState.Pushed | ButtonState.Checked)) != 0)
                        {
                            // paint down
                            ButtonBaseAdapter.LayoutOptions options = ButtonInternal.CheckBoxPopupAdapter.PaintPopupLayout(g,
                                                                                                                        true /*show3D*/,
                                                                                                                        checkBoxSize.Width,
                                                                                                                        checkBounds,
                                                                                                                        Padding.Empty,
                                                                                                                        false,
                                                                                                                        cellStyle.Font,
                                                                                                                        String.Empty,
                                                                                                                        this.DataGridView.Enabled,
                                                                                                                        DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                                                                                                        this.DataGridView.RightToLeft);
                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();
                            
                            
                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(this.DataGridView.Enabled,
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
                                                                    this.DataGridView.Enabled,
                                                                    checkState,
                                                                    g,
                                                                    layout,
                                                                    colors,
                                                                    colors.windowText,
                                                                    colors.buttonFace);
                            }
                            resultBounds = layout.checkBounds;
                        }
                        else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                 this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex &&
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
                                                                                                                        String.Empty,
                                                                                                                        this.DataGridView.Enabled,
                                                                                                                        DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                                                                                                        this.DataGridView.RightToLeft);
                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();

                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(this.DataGridView.Enabled,
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
                                                                    this.DataGridView.Enabled,
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
                                                                                                                       String.Empty,
                                                                                                                       this.DataGridView.Enabled,
                                                                                                                       DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                                                                                                                       this.DataGridView.RightToLeft);

                            options.everettButtonCompat = false;
                            ButtonBaseAdapter.LayoutData layout = options.Layout();
                            
                            if (paint && DataGridViewCell.PaintContentForeground(paintParts))
                            {
                                ButtonBaseAdapter.ColorData colors = ButtonBaseAdapter.PaintPopupRender(g,
                                                                                                        cellStyle.ForeColor,
                                                                                                        cellStyle.BackColor,
                                                                                                        this.DataGridView.Enabled).Calculate();
                                CheckBoxBaseAdapter.DrawCheckBackground(this.DataGridView.Enabled,
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
                                                                  this.DataGridView.Enabled,
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
                Debug.Assert(valBounds.Width < checkBoxSize.Width || valBounds.Height < checkBoxSize.Height, "the bounds are empty");
                resultBounds = Rectangle.Empty;
            }

            if (paint && DataGridViewCell.PaintErrorIcon(paintParts) && drawErrorText && this.DataGridView.ShowCellErrors)
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ParseFormattedValue"]/*' />
        public override object ParseFormattedValue(object formattedValue,
                                                   DataGridViewCellStyle cellStyle,
                                                   TypeConverter formattedValueTypeConverter,
                                                   TypeConverter valueTypeConverter)
        {
            Debug.Assert(formattedValue == null || this.FormattedValueType == null || this.FormattedValueType.IsAssignableFrom(formattedValue.GetType()));

            if (formattedValue != null)
            {
                if (formattedValue is bool)
                {
                    if ((bool) formattedValue)
                    {
                        if (this.TrueValue != null)
                        {
                            return this.TrueValue;
                        }
                        else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultBooleanType))
                        {
                            return true;
                        }
                        else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultCheckStateType))
                        {
                            return CheckState.Checked;
                        }
                    }
                    else
                    {
                        if (this.FalseValue != null)
                        {
                            return this.FalseValue;
                        }
                        else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultBooleanType))
                        {
                            return false;
                        }
                        else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultCheckStateType))
                        {
                            return CheckState.Unchecked;
                        }
                    }
                }
                else if (formattedValue is CheckState)
                {
                    switch ((CheckState) formattedValue)
                    {
                        case CheckState.Checked:
                            if (this.TrueValue != null)
                            {
                                return this.TrueValue;
                            }
                            else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultBooleanType))
                            {
                                return true;
                            }
                            else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultCheckStateType))
                            {
                                return CheckState.Checked;
                            }
                            break;
                        case CheckState.Unchecked:
                            if (this.FalseValue != null)
                            {
                                return this.FalseValue;
                            }
                            else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultBooleanType))
                            {
                                return false;
                            }
                            else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultCheckStateType))
                            {
                                return CheckState.Unchecked;
                            }
                            break;
                        case CheckState.Indeterminate:
                            if (this.IndeterminateValue != null)
                            {
                                return this.IndeterminateValue;
                            }
                            else if (this.ValueType != null && this.ValueType.IsAssignableFrom(defaultCheckStateType))
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

        [
            SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // no much choice here.
        ]
        private bool SwitchFormattedValue()
        {
            if (this.FormattedValueType == null)
            {
                return false;
            }
            IDataGridViewEditingCell editingCell = (IDataGridViewEditingCell)this;
            if (this.FormattedValueType.IsAssignableFrom(typeof(System.Windows.Forms.CheckState)))
            {
                if ((this.flags & DATAGRIDVIEWCHECKBOXCELL_checked) != 0x00)
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Indeterminate;
                }
                else if ((this.flags & DATAGRIDVIEWCHECKBOXCELL_indeterminate) != 0x00)
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Unchecked;
                }
                else
                {
                    editingCell.EditingCellFormattedValue = System.Windows.Forms.CheckState.Checked;
                }
            }
            else if (this.FormattedValueType.IsAssignableFrom(defaultBooleanType))
            {
                editingCell.EditingCellFormattedValue = !((bool)editingCell.GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting));
            }
            return true;
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCell.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the row Index and column Index of the cell.
        ///    </para>
        /// </devdoc>
        public override string ToString() {
            return "DataGridViewCheckBoxCell { ColumnIndex=" + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + this.RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
        {
            this.ButtonState = newButtonState;
            this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
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
                CheckBoxRenderer.SetParameters(CheckBoxElement.ClassName, CheckBoxElement.Part, (int) state);
                CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }

        /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject"]/*' />
        protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation

            /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject.DataGridViewCheckBoxCellAccessibleObject"]/*' />
            public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject.State"]/*' />
            public override AccessibleStates State
            {
                get
                {
                    if (((DataGridViewCheckBoxCell)Owner).EditedFormattedValue is CheckState)
                    {
                        CheckState state = (CheckState)(((DataGridViewCheckBoxCell)Owner).EditedFormattedValue);
                        switch (state)
                        {
                            case CheckState.Checked:
                                return AccessibleStates.Checked | base.State;
                            case CheckState.Indeterminate:
                                return AccessibleStates.Indeterminate | base.State;
                        }
                    }
                    else if (((DataGridViewCheckBoxCell)Owner).EditedFormattedValue is Boolean)
                    {
                        Boolean state = (Boolean)(((DataGridViewCheckBoxCell)Owner).EditedFormattedValue);
                        if (state)
                        {
                            return AccessibleStates.Checked | base.State;
                        }
                    }
                    return base.State;
                }
            }

            /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    if (!this.Owner.ReadOnly)
                    {
                        // determine if we switch to Checked/Unchecked value
                        bool switchToCheckedState = true;

                        object formattedValue = this.Owner.FormattedValue;

                        if (formattedValue is System.Windows.Forms.CheckState)
                        {
                            switchToCheckedState = ((CheckState) formattedValue) == CheckState.Unchecked;
                        }
                        else if (formattedValue is bool)
                        {
                            switchToCheckedState = !((bool) formattedValue);
                        }

                        if (switchToCheckedState)
                        {
                            return string.Format(SR.DataGridView_AccCheckBoxCellDefaultActionCheck);
                        }
                        else
                        {
                            return string.Format(SR.DataGridView_AccCheckBoxCellDefaultActionUncheck);
                        }
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                DataGridViewCheckBoxCell dataGridViewCell = (DataGridViewCheckBoxCell) this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
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
                            DataGridViewCheckBoxCell checkBoxCell = Owner as DataGridViewCheckBoxCell;
                            if (checkBoxCell != null)
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

            /// <include file='doc\DataGridViewCheckBoxCell.uex' path='docs/doc[@for="DataGridViewCheckBoxCellAccessibleObject.GetChildCount"]/*' />
            public override int GetChildCount()
            {
                return 0;
            }

            internal override bool IsIAccessibleExSupported() {
                if (AccessibilityImprovements.Level1) {
                    return true;
                }
                else {
                    return false;
                }
            }

            internal override int[] RuntimeId {
                get {
                    if (runtimeId == null) {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_IsTogglePatternAvailablePropertyId) {
                    return (Object)IsPatternSupported(NativeMethods.UIA_TogglePatternId);
                }
                else if (propertyID == NativeMethods.UIA_ControlTypePropertyId && AccessibilityImprovements.Level2)
                {
                    return NativeMethods.UIA_CheckBoxControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId) {
                if (patternId == NativeMethods.UIA_TogglePatternId) {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void Toggle() {
                DoDefaultAction();
            }

            internal override UnsafeNativeMethods.ToggleState ToggleState {
                get {
                    bool toggledState = true;
                    object formattedValue = this.Owner.FormattedValue;

                    if (formattedValue is System.Windows.Forms.CheckState) {
                        toggledState = ((CheckState)formattedValue) == CheckState.Checked;
                    }
                    else if (formattedValue is bool) {
                        toggledState = ((bool)formattedValue);
                    }
                    else {
                        return UnsafeNativeMethods.ToggleState.ToggleState_Indeterminate;
                    }

                    return toggledState ? UnsafeNativeMethods.ToggleState.ToggleState_On : UnsafeNativeMethods.ToggleState.ToggleState_Off;
                }
            }
        }
    }
}
