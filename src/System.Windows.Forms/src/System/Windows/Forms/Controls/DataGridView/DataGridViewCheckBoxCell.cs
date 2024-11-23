// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Identifies a checkbox cell in the DataGridView.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public partial class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
{
    private const DataGridViewContentAlignment AnyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
    private const DataGridViewContentAlignment AnyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
    private const DataGridViewContentAlignment AnyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;
    private const DataGridViewContentAlignment AnyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;
    private const DataGridViewContentAlignment AnyMiddle = DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.MiddleLeft;

    private static readonly VisualStyleElement s_checkBoxElement = VisualStyleElement.Button.CheckBox.UncheckedNormal;
    private static readonly int s_propButtonCellState = PropertyStore.CreateKey();
    private static readonly int s_propTrueValue = PropertyStore.CreateKey();
    private static readonly int s_propFalseValue = PropertyStore.CreateKey();
    private static readonly int s_propFlatStyle = PropertyStore.CreateKey();
    private static readonly int s_propIndeterminateValue = PropertyStore.CreateKey();
    private static Bitmap? s_checkImage;

    private const byte DATAGRIDVIEWCHECKBOXCELL_margin = 2;  // horizontal and vertical margins for preferred sizes

    private DataGridViewCheckBoxCellFlags _flags;
    private static bool s_mouseInContentBounds;
    private static readonly Type s_defaultCheckStateType = typeof(CheckState);
    private static readonly Type s_defaultBooleanType = typeof(bool);
    private static readonly Type s_cellType = typeof(DataGridViewCheckBoxCell);

    public DataGridViewCheckBoxCell()
        : this(threeState: false)
    {
    }

    public DataGridViewCheckBoxCell(bool threeState)
    {
        if (threeState)
        {
            _flags = DataGridViewCheckBoxCellFlags.ThreeState;
        }
    }

    public virtual object? EditingCellFormattedValue
    {
        get => GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting);
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

            if (value is CheckState valueAsCheckState)
            {
                if (valueAsCheckState == CheckState.Checked)
                {
                    _flags |= DataGridViewCheckBoxCellFlags.Checked;
                    _flags &= ~DataGridViewCheckBoxCellFlags.Indeterminate;
                }
                else if (valueAsCheckState == CheckState.Indeterminate)
                {
                    _flags |= DataGridViewCheckBoxCellFlags.Indeterminate;
                    _flags &= ~DataGridViewCheckBoxCellFlags.Checked;
                }
                else
                {
                    _flags &= ~DataGridViewCheckBoxCellFlags.Checked;
                    _flags &= ~DataGridViewCheckBoxCellFlags.Indeterminate;
                }
            }
            else if (value is bool valueAsBool)
            {
                if (valueAsBool)
                {
                    _flags |= DataGridViewCheckBoxCellFlags.Checked;
                }
                else
                {
                    _flags &= ~DataGridViewCheckBoxCellFlags.Checked;
                }

                _flags &= ~DataGridViewCheckBoxCellFlags.Indeterminate;
            }
            else
            {
                throw new ArgumentException(SR.DataGridViewCheckBoxCell_InvalidValueType);
            }
        }
    }

    public virtual bool EditingCellValueChanged
    {
        get => _flags.HasFlag(DataGridViewCheckBoxCellFlags.ValueChanged);
        set
        {
            if (value)
            {
                _flags |= DataGridViewCheckBoxCellFlags.ValueChanged;
            }
            else
            {
                _flags &= ~DataGridViewCheckBoxCellFlags.ValueChanged;
            }
        }
    }

    public virtual object? GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
    {
        if (FormattedValueType is null)
        {
            throw new InvalidOperationException(SR.DataGridViewCell_FormattedValueTypeNull);
        }

        if (FormattedValueType.IsAssignableFrom(s_defaultCheckStateType))
        {
            if (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Checked))
            {
                if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                {
                    return SR.DataGridViewCheckBoxCell_ClipboardChecked;
                }

                return CheckState.Checked;
            }
            else if (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Indeterminate))
            {
                if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                {
                    return SR.DataGridViewCheckBoxCell_ClipboardIndeterminate;
                }

                return CheckState.Indeterminate;
            }
            else
            {
                if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
                {
                    return SR.DataGridViewCheckBoxCell_ClipboardUnchecked;
                }

                return CheckState.Unchecked;
            }
        }
        else if (FormattedValueType.IsAssignableFrom(s_defaultBooleanType))
        {
            bool ret = _flags.HasFlag(DataGridViewCheckBoxCellFlags.Checked);
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
        get => Properties.GetValueOrDefault(s_propButtonCellState, ButtonState.Normal);
        set
        {
            // ButtonState.Pushed is used for mouse interaction
            // ButtonState.Checked is used for keyboard interaction
            Debug.Assert((value & ~(ButtonState.Normal | ButtonState.Pushed | ButtonState.Checked)) == 0);
            if (ButtonState != value)
            {
                Properties.AddOrRemoveValue(s_propButtonCellState, value, defaultValue: ButtonState.Normal);
            }
        }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    public override Type? EditType
    {
        get
        {
            // Check boxes can't switch to edit mode
            // This cell type must implement the IEditingCell interface
            return null;
        }
    }

    [DefaultValue(null)]
    public object? FalseValue
    {
        get => Properties.GetValueOrDefault<object?>(s_propFalseValue);
        set
        {
            Properties.AddOrRemoveValue(s_propFalseValue, value);
            if (DataGridView is not null)
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

    internal object? FalseValueInternal
    {
        set => Properties.AddOrRemoveValue(s_propFalseValue, value);
    }

    [DefaultValue(FlatStyle.Standard)]
    public FlatStyle FlatStyle
    {
        get => Properties.GetValueOrDefault(s_propFlatStyle, FlatStyle.Standard);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            FlatStyle previous = Properties.AddOrRemoveValue(s_propFlatStyle, value, defaultValue: FlatStyle.Standard);
            if (value != previous)
            {
                OnCommonChange();
            }
        }
    }

    internal FlatStyle FlatStyleInternal
    {
        set
        {
            Debug.Assert(value is >= FlatStyle.Flat and <= FlatStyle.System);
            if (value != FlatStyle)
            {
                Properties.AddOrRemoveValue(s_propFlatStyle, value, defaultValue: FlatStyle.Standard);
            }
        }
    }

    public override Type FormattedValueType => ThreeState ? s_defaultCheckStateType : s_defaultBooleanType;

    [DefaultValue(null)]
    public object? IndeterminateValue
    {
        get => Properties.GetValueOrDefault<object?>(s_propIndeterminateValue);
        set
        {
            Properties.AddOrRemoveValue(s_propIndeterminateValue, value);
            if (DataGridView is not null)
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

    internal object? IndeterminateValueInternal
    {
        set => Properties.AddOrRemoveValue(s_propIndeterminateValue, value);
    }

    [DefaultValue(false)]
    public bool ThreeState
    {
        get => _flags.HasFlag(DataGridViewCheckBoxCellFlags.ThreeState);
        set
        {
            if (ThreeState != value)
            {
                ThreeStateInternal = value;
                if (DataGridView is not null)
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
                    _flags |= DataGridViewCheckBoxCellFlags.ThreeState;
                }
                else
                {
                    _flags &= ~DataGridViewCheckBoxCellFlags.ThreeState;
                }
            }
        }
    }

    private CheckState CheckState
    {
        get
        {
            // When the CheckBoxCell has not been focused, the flags variable is not up to date,
            // so in this case, we use EditingCellValueChanged && FormattedValue to determine CheckState.
            // When users change the CheckBoxCell's CheckState but don't commit the value,
            // FormattedValue is not updated, but flags are, so in this case, we use flags to determine CheckState.
            if ((!EditingCellValueChanged && FormattedValue is CheckState checkState && checkState == CheckState.Indeterminate) ||
                (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Indeterminate)))
            {
                return CheckState.Indeterminate;
            }

            if ((!EditingCellValueChanged && FormattedValue is CheckState checkState2 && checkState2 == CheckState.Checked) ||
                (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Checked)))
            {
                return CheckState.Checked;
            }

            if ((!EditingCellValueChanged && FormattedValue is bool boolValue && boolValue) ||
                (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Checked)))
            {
                return CheckState.Checked;
            }

            return CheckState.Unchecked;
        }
    }

    [DefaultValue(null)]
    public object? TrueValue
    {
        get => Properties.GetValueOrDefault<object?>(s_propTrueValue);
        set
        {
            if (value is not null || Properties.ContainsKey(s_propTrueValue))
            {
                Properties.AddOrRemoveValue(s_propTrueValue, value);
                if (DataGridView is not null)
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

    internal object? TrueValueInternal
    {
        set => Properties.AddOrRemoveValue(s_propTrueValue, value);
    }

    public override Type? ValueType
    {
        get
        {
            Type? valueType = base.ValueType;
            if (valueType is not null)
            {
                return valueType;
            }

            return ThreeState ? s_defaultCheckStateType : s_defaultBooleanType;
        }
        set
        {
            base.ValueType = value;
            ThreeState = (value is not null && s_defaultCheckStateType.IsAssignableFrom(value));
        }
    }

    public override object Clone()
    {
        DataGridViewCheckBoxCell dataGridViewCell;
        Type thisType = GetType();
        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewCheckBoxCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewCheckBoxCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);
        dataGridViewCell.ThreeStateInternal = ThreeState;
        dataGridViewCell.TrueValueInternal = TrueValue;
        dataGridViewCell.FalseValueInternal = FalseValue;
        dataGridViewCell.IndeterminateValueInternal = IndeterminateValue;
        dataGridViewCell.FlatStyleInternal = FlatStyle;
        return dataGridViewCell;
    }

    private bool CommonContentClickUnsharesRow(DataGridViewCellEventArgs e)
    {
        Debug.Assert(DataGridView is not null);
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        return ptCurrentCell.X == ColumnIndex &&
               ptCurrentCell.Y == e.RowIndex &&
               DataGridView.IsCurrentCellInEditMode;
    }

    protected override bool ContentClickUnsharesRow(DataGridViewCellEventArgs e) => CommonContentClickUnsharesRow(e);

    protected override bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e) => CommonContentClickUnsharesRow(e);

    protected override AccessibleObject CreateAccessibilityInstance() => new DataGridViewCheckBoxCellAccessibleObject(this);

    protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
        {
            return Rectangle.Empty;
        }

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle checkBoxBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue: null,   // checkBoxBounds is independent of formattedValue
            errorText: null,    // checkBoxBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);

#if DEBUG
        object? value = GetValue(rowIndex);
        Rectangle checkBoxBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            GetEditedFormattedValue(value, rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting),
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);
        Debug.Assert(checkBoxBoundsDebug.Equals(checkBoxBounds));
#endif

        return checkBoxBounds;
    }

    private protected override string? GetDefaultToolTipText()
    {
        if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
        {
            return SR.DataGridViewCheckBoxCell_ClipboardFalse;
        }

        return null;
    }

    protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

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

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle errorIconBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue: null,   // errorIconBounds is independent of formattedValue
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: false,
            computeErrorIconBounds: true,
            paint: false);

        return errorIconBounds;
    }

    protected override object? GetFormattedValue(
        object? value,
        int rowIndex,
        ref DataGridViewCellStyle cellStyle,
        TypeConverter? valueTypeConverter,
        TypeConverter? formattedValueTypeConverter,
        DataGridViewDataErrorContexts context)
    {
        if (value is int intValue)
        {
            if (ThreeState)
            {
                value = (CheckState)intValue switch
                {
                    CheckState.Checked => CheckState.Checked,
                    CheckState.Unchecked => CheckState.Unchecked,
                    CheckState.Indeterminate => CheckState.Indeterminate,
                    _ => value
                };
            }
            else
            {
                value = intValue != 0;
            }
        }
        else if (value is not null)
        {
            if (value.Equals(TrueValue))
            {
                value = CheckState.Checked;
            }
            else if (value.Equals(FalseValue))
            {
                value = CheckState.Unchecked;
            }
            else if (ThreeState && value.Equals(IndeterminateValue))
            {
                value = CheckState.Indeterminate;
            }
        }

        object? ret = base.GetFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            valueTypeConverter,
            formattedValueTypeConverter,
            context);

        if (ret is not null && (context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
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

    protected override Size GetPreferredSize(
        Graphics graphics,
        DataGridViewCellStyle cellStyle,
        int rowIndex,
        Size constraintSize)
    {
        if (DataGridView is null)
        {
            return new Size(-1, -1);
        }

        ArgumentNullException.ThrowIfNull(cellStyle);

        DataGridViewFreeDimension freeDimension = GetFreeDimensionFromConstraint(constraintSize);
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
            int checkBoxSize = FlatStyle switch
            {
                FlatStyle.Flat => CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal).Width - 3,
                FlatStyle.Popup => CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal).Width - 2,
                // FlatStyle.Standard || FlatStyle.System
                _ => SystemInformation.Border3DSize.Width * 2 + 9 + 2 * DATAGRIDVIEWCHECKBOXCELL_margin,
            };

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

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out _,
            out _);

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

    protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex) =>
        e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift;

    protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex) => e.KeyCode == Keys.Space;

    protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) => e.Button == MouseButtons.Left;

    protected override bool MouseEnterUnsharesRow(int rowIndex)
    {
        Debug.Assert(DataGridView is not null);
        return ColumnIndex == DataGridView.MouseDownCellAddress.X && rowIndex == DataGridView.MouseDownCellAddress.Y;
    }

    protected override bool MouseLeaveUnsharesRow(int rowIndex) => ButtonState.HasFlag(ButtonState.Pushed);

    protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) => e.Button == MouseButtons.Left;

    private void NotifyDataGridViewOfValueChange()
    {
        _flags |= DataGridViewCheckBoxCellFlags.ValueChanged;
        Debug.Assert(DataGridView is not null);
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

    protected override void OnContentClick(DataGridViewCellEventArgs e) => OnCommonContentClick(e);

    protected override void OnContentDoubleClick(DataGridViewCellEventArgs e) => OnCommonContentClick(e);

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

        if (e.Button == MouseButtons.Left && s_mouseInContentBounds)
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

        if (s_mouseInContentBounds)
        {
            s_mouseInContentBounds = false;
            if (ColumnIndex >= 0 &&
                rowIndex >= 0 &&
                (DataGridView.ApplyVisualStylesToInnerCells || FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
            {
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }

        if (ButtonState.HasFlag(ButtonState.Pushed) &&
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

        bool oldMouseInContentBounds = s_mouseInContentBounds;
        s_mouseInContentBounds = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);
        if (oldMouseInContentBounds != s_mouseInContentBounds)
        {
            if (DataGridView.ApplyVisualStylesToInnerCells || FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)
            {
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
            }

            if (e.ColumnIndex == DataGridView.MouseDownCellAddress.X &&
                e.RowIndex == DataGridView.MouseDownCellAddress.Y &&
                Control.MouseButtons == MouseButtons.Left)
            {
                if (!ButtonState.HasFlag(ButtonState.Pushed) &&
                    s_mouseInContentBounds &&
                    DataGridView.CellMouseDownInContentBounds)
                {
                    UpdateButtonState(ButtonState | ButtonState.Pushed, e.RowIndex);
                }
                else if (ButtonState.HasFlag(ButtonState.Pushed) && !s_mouseInContentBounds)
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
        if (IsParentAccessibilityObjectCreated)
        {
            string cellName = AccessibilityObject.Name ?? string.Empty;
            string notificationText = CheckState switch
            {
                CheckState.Checked => string.Format(SR.DataGridViewCheckBoxCellCheckedStateDescription, cellName),
                CheckState.Unchecked => string.Format(SR.DataGridViewCheckBoxCellUncheckedStateDescription, cellName),
                _ => string.Format(SR.DataGridViewCheckBoxCellIndeterminateStateDescription, cellName),
            };
            AccessibilityObject.InternalRaiseAutomationNotification(
                Automation.AutomationNotificationKind.Other,
                Automation.AutomationNotificationProcessing.MostRecent,
                notificationText);
        }
    }

    private void NotifyMSAAClient(int columnIndex, int rowIndex)
    {
        Debug.Assert(DataGridView is not null);
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

        if (DataGridView.IsAccessibilityObjectCreated && DataGridView.AccessibilityObject is Control.ControlAccessibleObject accessibleObject)
        {
            accessibleObject.NotifyClients(AccessibleEvents.StateChange, objectID, childID);
        }
    }

    protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates elementState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        PaintPrivate(
            graphics,
            clipBounds,
            cellBounds,
            rowIndex,
            elementState,
            formattedValue,
            errorText,
            cellStyle,
            advancedBorderStyle,
            paintParts,
            computeContentBounds: false,
            computeErrorIconBounds: false,
            paint: true);
    }

    // PaintPrivate is used in three places that need to duplicate the paint code:
    // 1. DataGridViewCell::Paint method
    // 2. DataGridViewCell::GetContentBounds
    // 3. DataGridViewCell::GetErrorIconBounds
    //
    // if computeContentBounds is true then PaintPrivate returns the contentBounds
    // else if computeErrorIconBounds is true then PaintPrivate returns the errorIconBounds
    // else it returns Rectangle.Empty;
    private Rectangle PaintPrivate(
        Graphics g,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates elementState,
        object? formattedValue,
        string? errorText,
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
        Debug.Assert(DataGridView is not null);
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        if (ptCurrentCell.X == ColumnIndex &&
            ptCurrentCell.Y == rowIndex && DataGridView.IsCurrentCellInEditMode)
        {
            drawErrorText = false;
        }

        if (formattedValue is not null and CheckState state)
        {
            checkState = state;
            bs = (checkState == CheckState.Unchecked) ? ButtonState.Normal : ButtonState.Checked;
            drawAsMixedCheckBox = (checkState == CheckState.Indeterminate);
        }
        else if (formattedValue is bool formattedValueAsBool)
        {
            if (formattedValueAsBool)
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
            ControlPaint.DrawFocusRectangle(g, valBounds, cellStyle.BackColor, cellStyle.ForeColor);
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
                    && s_mouseInContentBounds);

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
                if (FlatStyle is FlatStyle.System or FlatStyle.Standard)
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

                    Rectangle checkBounds = new(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);

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
                            s_mouseInContentBounds)
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
                            Rectangle fullSize = new(checkBoxX - 1, checkBoxY - 1, checkBoxSize.Width + 3, checkBoxSize.Height + 3);
                            fullSize.Width++;
                            fullSize.Height++;

                            if (s_checkImage is null || s_checkImage.Width != fullSize.Width || s_checkImage.Height != fullSize.Height)
                            {
                                if (s_checkImage is not null)
                                {
                                    s_checkImage.Dispose();
                                    s_checkImage = null;
                                }

                                // We draw the checkmark slightly off center to eliminate 3-D border artifacts,
                                // and compensate below
                                RECT rcCheck = new Rectangle(0, 0, fullSize.Width, fullSize.Height);
                                Bitmap bitmap = new(fullSize.Width, fullSize.Height);
                                using (Graphics offscreen = Graphics.FromImage(bitmap))
                                {
                                    offscreen.Clear(Color.Transparent);
                                    using DeviceContextHdcScope hdc = new(offscreen);
                                    PInvoke.DrawFrameControl(
                                        hdc,
                                        ref rcCheck,
                                        DFC_TYPE.DFC_MENU,
                                        DFCS_STATE.DFCS_MENUCHECK);
                                }

                                bitmap.MakeTransparent();
                                s_checkImage = bitmap;
                            }

                            fullSize.Y--;
                            ControlPaint.DrawImageColorized(
                                g,
                                s_checkImage,
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

                    Rectangle checkBounds = new(checkBoxX, checkBoxY, checkBoxSize.Width - 1, checkBoxSize.Height - 1);

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
                            cellStyle.Font!,
                            string.Empty,
                            DataGridView.Enabled,
                            DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                            DataGridView.RightToLeft);

                        options.DotNetOneButtonCompat = false;
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
                                layout.CheckBounds,
                                colors.ButtonFace,
                                disabledColors: true);
                            CheckBoxBaseAdapter.DrawPopupBorder(g, layout.CheckBounds, colors);
                            CheckBoxBaseAdapter.DrawCheckOnly(
                                checkBoxSize.Width,
                                checkState is CheckState.Checked or CheckState.Indeterminate,
                                DataGridView.Enabled,
                                checkState,
                                g,
                                layout,
                                colors,
                                colors.WindowText);
                        }

                        resultBounds = layout.CheckBounds;
                    }
                    else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex
                        && DataGridView.MouseEnteredCellAddress.X == ColumnIndex
                        && s_mouseInContentBounds)
                    {
                        // paint over

                        ButtonBaseAdapter.LayoutOptions options = CheckBoxPopupAdapter.PaintPopupLayout(
                            show3D: true,
                            checkBoxSize.Width,
                            checkBounds,
                            Padding.Empty,
                            isDefault: false,
                            cellStyle.Font!,
                            string.Empty,
                            DataGridView.Enabled,
                            DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                            DataGridView.RightToLeft);

                        options.DotNetOneButtonCompat = false;
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
                                layout.CheckBounds,
                                colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight,
                                disabledColors: true);

                            CheckBoxBaseAdapter.DrawPopupBorder(g, layout.CheckBounds, colors);
                            CheckBoxBaseAdapter.DrawCheckOnly(
                                checkBoxSize.Width,
                                checkState is CheckState.Checked or CheckState.Indeterminate,
                                DataGridView.Enabled,
                                checkState,
                                g,
                                layout,
                                colors,
                                colors.WindowText);
                        }

                        resultBounds = layout.CheckBounds;
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
                            cellStyle.Font!,
                            string.Empty,
                            DataGridView.Enabled,
                            DataGridViewUtilities.ComputeDrawingContentAlignmentForCellStyleAlignment(cellStyle.Alignment),
                            DataGridView.RightToLeft);

                        options.DotNetOneButtonCompat = false;
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
                                layout.CheckBounds,
                                colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight,
                                disabledColors: true);

                            ControlPaint.DrawBorderSimple(g, layout.CheckBounds, colors.ButtonShadow);
                            CheckBoxBaseAdapter.DrawCheckOnly(
                                checkBoxSize.Width,
                                checkState is CheckState.Checked or CheckState.Indeterminate,
                                DataGridView.Enabled,
                                checkState,
                                g,
                                layout,
                                colors,
                                colors.WindowText);
                        }

                        resultBounds = layout.CheckBounds;
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

    public override object? ParseFormattedValue(
        object? formattedValue,
        DataGridViewCellStyle cellStyle,
        TypeConverter? formattedValueTypeConverter,
        TypeConverter? valueTypeConverter)
    {
        Debug.Assert(formattedValue is null || FormattedValueType is null || FormattedValueType.IsAssignableFrom(formattedValue.GetType()));

        if (formattedValue is not null)
        {
            if (formattedValue is bool boolean)
            {
                if (boolean)
                {
                    if (TrueValue is not null)
                    {
                        return TrueValue;
                    }
                    else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultBooleanType))
                    {
                        return true;
                    }
                    else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultCheckStateType))
                    {
                        return CheckState.Checked;
                    }
                }
                else
                {
                    if (FalseValue is not null)
                    {
                        return FalseValue;
                    }
                    else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultBooleanType))
                    {
                        return false;
                    }
                    else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultCheckStateType))
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
                        if (TrueValue is not null)
                        {
                            return TrueValue;
                        }
                        else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultBooleanType))
                        {
                            return true;
                        }
                        else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultCheckStateType))
                        {
                            return CheckState.Checked;
                        }

                        break;
                    case CheckState.Unchecked:
                        if (FalseValue is not null)
                        {
                            return FalseValue;
                        }
                        else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultBooleanType))
                        {
                            return false;
                        }
                        else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultCheckStateType))
                        {
                            return CheckState.Unchecked;
                        }

                        break;
                    case CheckState.Indeterminate:
                        if (IndeterminateValue is not null)
                        {
                            return IndeterminateValue;
                        }
                        else if (ValueType is not null && ValueType.IsAssignableFrom(s_defaultCheckStateType))
                        {
                            return CheckState.Indeterminate;
                        }

                        /* case where this.ValueType.IsAssignableFrom(defaultBooleanType) is treated in base.ParseFormattedValue */
                        break;
                }
            }
        }

        return base.ParseFormattedValue(
            formattedValue,
            cellStyle,
            formattedValueTypeConverter,
            valueTypeConverter);
    }

    private bool SwitchFormattedValue()
    {
        if (FormattedValueType is null)
        {
            return false;
        }

        DataGridViewCheckBoxCell editingCell = this;
        if (FormattedValueType.IsAssignableFrom(typeof(CheckState)))
        {
            if (_flags.HasFlag(DataGridViewCheckBoxCellFlags.Checked))
            {
                editingCell.EditingCellFormattedValue = CheckState.Indeterminate;
            }
            else if ((_flags.HasFlag(DataGridViewCheckBoxCellFlags.Indeterminate)))
            {
                editingCell.EditingCellFormattedValue = CheckState.Unchecked;
            }
            else
            {
                editingCell.EditingCellFormattedValue = CheckState.Checked;
            }
        }
        else if (FormattedValueType.IsAssignableFrom(s_defaultBooleanType))
        {
            editingCell.EditingCellFormattedValue = !((bool)editingCell.GetEditingCellFormattedValue(DataGridViewDataErrorContexts.Formatting)!);
        }

        return true;
    }

    /// <summary>
    ///  Gets the row Index and column Index of the cell.
    /// </summary>
    public override string ToString() =>
        $"DataGridViewCheckBoxCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";

    private void UpdateButtonState(ButtonState newButtonState, int rowIndex)
    {
        Debug.Assert(DataGridView is not null);
        ButtonState = newButtonState;
        DataGridView.InvalidateCell(ColumnIndex, rowIndex);
    }
}
