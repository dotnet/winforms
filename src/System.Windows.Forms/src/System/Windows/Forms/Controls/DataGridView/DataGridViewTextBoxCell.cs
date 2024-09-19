// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Identifies a cell in the dataGridView.
/// </summary>
public partial class DataGridViewTextBoxCell : DataGridViewCell
{
    private static readonly int s_propTextBoxCellMaxInputLength = PropertyStore.CreateKey();
    private static readonly int s_propTextBoxCellEditingTextBox = PropertyStore.CreateKey();

    private const byte DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick = 0x01;
    private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft = 3;
    private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight = 4;
    private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft = 0;
    private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight = 0;
    private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop = 2;
    private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetBottom = 1;
    private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping = 1;
    private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping = 2;
    private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom = 1;

    private const int MaxInputLengthDefault = 32767;

    private byte _flagsState;  // see DATAGRIDVIEWTEXTBOXCELL_ constants above

    private static readonly Type s_defaultFormattedValueType = typeof(string);
    private static readonly Type s_defaultValueType = typeof(object);
    private static readonly Type s_cellType = typeof(DataGridViewTextBoxCell);

    public DataGridViewTextBoxCell()
    {
    }

    /// <summary>
    ///  Creates a new AccessibleObject for this DataGridViewTextBoxCell instance.
    ///  The AccessibleObject instance returned by this method supports ControlType UIA property.
    /// </summary>
    /// <returns>
    ///  AccessibleObject for this DataGridViewTextBoxCell instance.
    /// </returns>
    protected override AccessibleObject CreateAccessibilityInstance() => new DataGridViewTextBoxCellAccessibleObject(this);

    private DataGridViewTextBoxEditingControl? EditingTextBox
    {
        get => Properties.GetValueOrDefault<DataGridViewTextBoxEditingControl?>(s_propTextBoxCellEditingTextBox);
        set => Properties.AddOrRemoveValue(s_propTextBoxCellEditingTextBox, value);
    }

    public override Type FormattedValueType
    {
        get
        {
            // we return string for the formatted type
            return s_defaultFormattedValueType;
        }
    }

    [DefaultValue(MaxInputLengthDefault)]
    public virtual int MaxInputLength
    {
        get => Properties.GetValueOrDefault(s_propTextBoxCellMaxInputLength, MaxInputLengthDefault);
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            Properties.AddOrRemoveValue(s_propTextBoxCellMaxInputLength, value, defaultValue: MaxInputLengthDefault);
            if (OwnsEditingTextBox(RowIndex))
            {
                EditingTextBox.MaxLength = value;
            }
        }
    }

    public override Type ValueType
    {
        get
        {
            Type? valueType = base.ValueType;
            if (valueType is not null)
            {
                return valueType;
            }

            return s_defaultValueType;
        }
    }

    // Called when the row that owns the editing control gets un-shared.
    internal override void CacheEditingControl()
    {
        EditingTextBox = DataGridView!.EditingControl as DataGridViewTextBoxEditingControl;
    }

    public override object Clone()
    {
        DataGridViewTextBoxCell dataGridViewCell;
        Type thisType = GetType();
        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewTextBoxCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewTextBoxCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);
        dataGridViewCell.MaxInputLength = MaxInputLength;
        return dataGridViewCell;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override void DetachEditingControl()
    {
        DataGridView? dataGridView = DataGridView;
        if (dataGridView?.EditingControl is null)
        {
            throw new InvalidOperationException();
        }

        if (dataGridView.EditingControl is TextBox textBox)
        {
            textBox.ClearUndo();
        }

        EditingTextBox = null;

        base.DetachEditingControl();
    }

    private Rectangle GetAdjustedEditingControlBounds(Rectangle editingControlBounds, DataGridViewCellStyle cellStyle)
    {
        Debug.Assert(cellStyle.WrapMode != DataGridViewTriState.NotSet);
        Debug.Assert(DataGridView is not null);

        int originalWidth = editingControlBounds.Width;
        if (DataGridView.EditingControl is TextBox txtEditingControl)
        {
            switch (cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.BottomLeft:
                    // Add 3 pixels on the left of the editing control to match non-editing text position
                    if (DataGridView.RightToLeftInternal)
                    {
                        editingControlBounds.X += 1;
                        editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft - 2);
                    }
                    else
                    {
                        editingControlBounds.X += DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft;
                        editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft - 1);
                    }

                    break;

                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    editingControlBounds.X += 1;
                    editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - 3);
                    break;

                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.BottomRight:
                    // Shorten the editing control by 5 pixels to match non-editing text position
                    if (DataGridView.RightToLeftInternal)
                    {
                        editingControlBounds.X += DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight - 1;
                        editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight);
                    }
                    else
                    {
                        editingControlBounds.X += 1;
                        editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight - 1);
                    }

                    break;
            }

            switch (cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.TopRight:
                    editingControlBounds.Y += DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop;
                    editingControlBounds.Height = Math.Max(0, editingControlBounds.Height - DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop);
                    break;

                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.MiddleRight:
                    editingControlBounds.Height++;
                    break;

                case DataGridViewContentAlignment.BottomLeft:
                case DataGridViewContentAlignment.BottomCenter:
                case DataGridViewContentAlignment.BottomRight:
                    editingControlBounds.Height = Math.Max(0, editingControlBounds.Height - DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetBottom);
                    break;
            }

            int preferredHeight;
            if (cellStyle.WrapMode == DataGridViewTriState.False)
            {
                preferredHeight = txtEditingControl.PreferredSize.Height;
            }
            else
            {
                string editedFormattedValue = (string)((IDataGridViewEditingControl)txtEditingControl).GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
                if (string.IsNullOrEmpty(editedFormattedValue))
                {
                    editedFormattedValue = " ";
                }

                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

                using var screen = GdiCache.GetScreenDCGraphics();
                preferredHeight = MeasureTextHeight(screen, editedFormattedValue, cellStyle.Font!, originalWidth, flags);
            }

            if (preferredHeight < editingControlBounds.Height)
            {
                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.TopRight:
                        // Single pixel move - leave it as is for now
                        break;
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.MiddleRight:
                        editingControlBounds.Y += (editingControlBounds.Height - preferredHeight) / 2;
                        break;
                    case DataGridViewContentAlignment.BottomLeft:
                    case DataGridViewContentAlignment.BottomCenter:
                    case DataGridViewContentAlignment.BottomRight:
                        editingControlBounds.Y += editingControlBounds.Height - preferredHeight;
                        break;
                }
            }
        }

        return editingControlBounds;
    }

    protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
        {
            return Rectangle.Empty;
        }

        object? value = GetValue(rowIndex);
        object? formattedValue = GetFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            valueTypeConverter: null,
            formattedValueTypeConverter: null,
            DataGridViewDataErrorContexts.Formatting);

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle textBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            errorText: null,    // textBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);

#if DEBUG
        Rectangle textBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);
        Debug.Assert(textBoundsDebug.Equals(textBounds));
#endif

        return textBounds;
    }

    private protected override string? GetDefaultToolTipText()
    {
        if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
        {
            return SR.DefaultDataGridViewTextBoxCellTollTipText;
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

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle errorBounds = PaintPrivate(graphics,
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

#if DEBUG
        object? value = GetValue(rowIndex);
        object? formattedValue = GetFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            valueTypeConverter: null,
            formattedValueTypeConverter: null,
            DataGridViewDataErrorContexts.Formatting);

        Rectangle errorBoundsDebug = PaintPrivate(graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: false,
            computeErrorIconBounds: true,
            paint: false);
        Debug.Assert(errorBoundsDebug.Equals(errorBounds));
#endif

        return errorBounds;
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

        Size preferredSize;
        Rectangle borderWidthsRect = StdBorderWidths;
        int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
        int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
        DataGridViewFreeDimension freeDimension = GetFreeDimensionFromConstraint(constraintSize);
        object? formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize);
        string? formattedString = formattedValue as string;
        if (string.IsNullOrEmpty(formattedString))
        {
            formattedString = " ";
        }

        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
        if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1)
        {
            switch (freeDimension)
            {
                case DataGridViewFreeDimension.Width:
                    {
                        preferredSize = new Size(
                            MeasureTextWidth(
                                graphics,
                                formattedString,
                                cellStyle.Font!,
                                Math.Max(1, constraintSize.Height - borderAndPaddingHeights - DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping - DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom),
                                flags),
                            0);
                        break;
                    }

                case DataGridViewFreeDimension.Height:
                    {
                        preferredSize = new Size(
                            0,
                            MeasureTextHeight(
                                graphics,
                                formattedString,
                                cellStyle.Font!,
                                Math.Max(1, constraintSize.Width - borderAndPaddingWidths - DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft - DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight),
                                flags));
                        break;
                    }

                default:
                    {
                        preferredSize = MeasureTextPreferredSize(
                            graphics,
                            formattedString,
                            cellStyle.Font!,
                            5.0F,
                            flags);
                        break;
                    }
            }
        }
        else
        {
            switch (freeDimension)
            {
                case DataGridViewFreeDimension.Width:
                    {
                        preferredSize = new Size(
                            MeasureTextSize(graphics, formattedString, cellStyle.Font!, flags).Width,
                            0);
                        break;
                    }

                case DataGridViewFreeDimension.Height:
                    {
                        preferredSize = new Size(
                            0,
                            MeasureTextSize(graphics, formattedString, cellStyle.Font!, flags).Height);
                        break;
                    }

                default:
                    {
                        preferredSize = MeasureTextSize(graphics, formattedString, cellStyle.Font!, flags);
                        break;
                    }
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Height)
        {
            preferredSize.Width += DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft + DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight + borderAndPaddingWidths;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + IconMarginWidth * 2 + s_iconsWidth);
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Width)
        {
            int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping : DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping;
            preferredSize.Height += verticalTextMarginTop + DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom + borderAndPaddingHeights;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + IconMarginHeight * 2 + s_iconsHeight);
            }
        }

        return preferredSize;
    }

    public override void InitializeEditingControl(int rowIndex, object? initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
        Debug.Assert(DataGridView is not null &&
                     DataGridView.EditingControl is not null);
        Debug.Assert(!ReadOnly);
        base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
        if (DataGridView.EditingControl is TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.None;
            textBox.AcceptsReturn = textBox.Multiline = dataGridViewCellStyle.WrapMode == DataGridViewTriState.True;
            textBox.MaxLength = MaxInputLength;
            if (initialFormattedValue is not string initialFormattedValueStr)
            {
                textBox.Text = string.Empty;
            }
            else
            {
                textBox.Text = initialFormattedValueStr;
            }

            EditingTextBox = DataGridView.EditingControl as DataGridViewTextBoxEditingControl;
        }
    }

    public override bool KeyEntersEditMode(KeyEventArgs e)
    {
        if (((char.IsLetterOrDigit((char)e.KeyCode) && !(e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)) ||
             (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide) ||
             (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.Oem102) ||
             (e.KeyCode == Keys.Space && !e.Shift)) &&
            !e.Alt &&
            !e.Control)
        {
            return true;
        }

        return base.KeyEntersEditMode(e);
    }

    protected override void OnEnter(int rowIndex, bool throughMouseClick)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (throughMouseClick)
        {
            _flagsState |= DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick;
        }
    }

    protected override void OnLeave(int rowIndex, bool throughMouseClick)
    {
        if (DataGridView is null)
        {
            return;
        }

        _flagsState = (byte)(_flagsState & ~DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick);
    }

    protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        Debug.Assert(e.ColumnIndex == ColumnIndex);
        Point ptCurrentCell = DataGridView.CurrentCellAddress;
        if (ptCurrentCell.X == e.ColumnIndex && ptCurrentCell.Y == e.RowIndex && e.Button == MouseButtons.Left)
        {
            if ((_flagsState & DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick) != 0x00)
            {
                _flagsState = (byte)(_flagsState & ~DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick);
            }
            else if (DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
            {
                DataGridView.BeginEdit(selectAll: true);
            }
        }
    }

    [MemberNotNullWhen(true, nameof(EditingTextBox))]
    private bool OwnsEditingTextBox(int rowIndex) =>
        rowIndex != -1 &&
        EditingTextBox is not null &&
        rowIndex == ((IDataGridViewEditingControl)EditingTextBox).EditingControlRowIndex;

    protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
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
            cellState,
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
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts,
        bool computeContentBounds,
        bool computeErrorIconBounds,
        bool paint)
    {
        // Parameter checking. One bit and one bit only should be turned on.
        Debug.Assert(paint || computeContentBounds || computeErrorIconBounds);
        Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds);
        Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint);
        Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds);
        Debug.Assert(cellStyle is not null);

        // If computeContentBounds == TRUE then resultBounds will be the contentBounds.
        // If computeErrorIconBounds == TRUE then resultBounds will be the error icon bounds.
        // Else resultBounds will be Rectangle.Empty;
        Rectangle resultBounds = Rectangle.Empty;

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        Rectangle borderWidths = BorderWidths(advancedBorderStyle);
        Rectangle valBounds = cellBounds;
        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;

        Point ptCurrentCell = DataGridView!.CurrentCellAddress;
        bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
        bool cellEdited = cellCurrent && DataGridView.EditingControl is not null;
        bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
        bool notCollapsed = valBounds.Width > 0 && valBounds.Height > 0;

        Color brushColor = PaintSelectionBackground(paintParts) && cellSelected && !cellEdited
            ? cellStyle.SelectionBackColor
            : cellStyle.BackColor;

        if (paint && PaintBackground(paintParts) && !brushColor.HasTransparency() && notCollapsed)
        {
            using var brush = brushColor.GetCachedSolidBrushScope();
            graphics.FillRectangle(brush, valBounds);
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

        if (paint && cellCurrent && !cellEdited)
        {
            // Draw focus rectangle
            if (PaintFocus(paintParts) && DataGridView.ShowFocusCues && DataGridView.Focused && notCollapsed)
            {
                ControlPaint.DrawFocusRectangle(graphics, valBounds, Color.Empty, cellStyle.ForeColor);
            }
        }

        Rectangle errorBounds = valBounds;
        string? formattedString = formattedValue as string;

        if (formattedString is not null && ((paint && !cellEdited) || computeContentBounds))
        {
            // Font independent margins
            int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True
                ? DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping
                : DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping;
            valBounds.Offset(DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft, verticalTextMarginTop);
            valBounds.Width -= DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft + DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight;
            valBounds.Height -= verticalTextMarginTop + DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom;
            if (valBounds.Width > 0 && valBounds.Height > 0)
            {
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(
                    DataGridView.RightToLeftInternal,
                    cellStyle.Alignment,
                    cellStyle.WrapMode);

                if (paint)
                {
                    if (PaintContentForeground(paintParts))
                    {
                        if ((flags & TextFormatFlags.SingleLine) != 0)
                        {
                            flags |= TextFormatFlags.EndEllipsis;
                        }

                        TextRenderer.DrawText(graphics,
                            formattedString,
                            cellStyle.Font,
                            valBounds,
                            cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor,
                            flags);
                    }
                }
                else
                {
                    resultBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedString, flags, cellStyle);
                }
            }
        }
        else if (computeErrorIconBounds)
        {
            if (!string.IsNullOrEmpty(errorText))
            {
                resultBounds = ComputeErrorIconBounds(errorBounds);
            }
        }
        else
        {
            Debug.Assert(cellEdited || formattedString is null);
            Debug.Assert(paint || computeContentBounds);
        }

        if (DataGridView.ShowCellErrors && paint && PaintErrorIcon(paintParts))
        {
            PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
        }

        return resultBounds;
    }

    public override void PositionEditingControl(
        bool setLocation,
        bool setSize,
        Rectangle cellBounds,
        Rectangle cellClip,
        DataGridViewCellStyle cellStyle,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedColumn,
        bool isFirstDisplayedRow)
    {
        Rectangle editingControlBounds = PositionEditingPanel(
            cellBounds,
            cellClip,
            cellStyle,
            singleVerticalBorderAdded,
            singleHorizontalBorderAdded,
            isFirstDisplayedColumn,
            isFirstDisplayedRow);
        editingControlBounds = GetAdjustedEditingControlBounds(editingControlBounds, cellStyle);
        DataGridView!.EditingControl!.Location = new Point(editingControlBounds.X, editingControlBounds.Y);
        DataGridView.EditingControl.Size = new Size(editingControlBounds.Width, editingControlBounds.Height);
    }

    public override string ToString()
        => $"DataGridViewTextBoxCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";
}
