// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
{
    private static readonly VisualStyleElement s_headerElement = VisualStyleElement.Header.Item.Normal;

    private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft = 1;
    private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginRight = 2;
    private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin = 1;

    public DataGridViewTopLeftHeaderCell()
    {
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new DataGridViewTopLeftHeaderCellAccessibleObject(this);
    }

    protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        ArgumentOutOfRangeException.ThrowIfNotEqual(rowIndex, -1);

        if (DataGridView is null)
        {
            return Rectangle.Empty;
        }

        object? value = GetValue(rowIndex);

        // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
        // the content bounds are computed on demand
        // we mimic a lot of the painting code

        // get the borders

        ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

        Rectangle contentBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
            errorText: null, // contentBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);

#if DEBUG
        Rectangle contentBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);
        Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

        return contentBounds;
    }

    protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(rowIndex, -1);

        if (DataGridView is null)
        {
            return Rectangle.Empty;
        }

        ArgumentNullException.ThrowIfNull(cellStyle);

        ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

        Rectangle errorBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue: null, // errorIconBounds is independent of formattedValue
            GetErrorText(rowIndex),
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: false,
            computeErrorIconBounds: true,
            paint: false);

#if DEBUG
        object? value = GetValue(rowIndex);

        Rectangle errorBoundsDebug = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
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

    protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(rowIndex, -1);

        if (DataGridView is null)
        {
            return new Size(-1, -1);
        }

        ArgumentNullException.ThrowIfNull(cellStyle);

        Rectangle borderWidthsRect = BorderWidths(DataGridView.AdjustedTopLeftHeaderBorderStyle);
        int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
        int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

        // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
        string? val = GetValue(rowIndex) as string;

        return DataGridViewUtilities.GetPreferredRowHeaderSize(
            graphics,
            val,
            cellStyle,
            borderAndPaddingWidths,
            borderAndPaddingHeights,
            DataGridView.ShowCellErrors,
            showGlyph: false,
            constraintSize,
            flags);
    }

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
        // Parameter checking.
        // One bit and one bit only should be turned on
        Debug.Assert(paint || computeContentBounds || computeErrorIconBounds);
        Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds);
        Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint);
        Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds);
        Debug.Assert(cellStyle is not null);

        // If computeContentBounds == TRUE then resultBounds will be the contentBounds.
        // If computeErrorIconBounds == TRUE then resultBounds will be the error icon bounds.
        // Else resultBounds will be Rectangle.Empty;
        Rectangle resultBounds = Rectangle.Empty;

        Rectangle valBounds = cellBounds;
        Rectangle borderWidths = BorderWidths(advancedBorderStyle);

        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;

        bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;

        if (paint && PaintBackground(paintParts))
        {
            if (DataGridView!.ApplyVisualStylesToHeaderCells)
            {
                // Theming
                int state = (int)HeaderItemState.Normal;

                if (ButtonState != ButtonState.Normal)
                {
                    Debug.Assert(ButtonState == ButtonState.Pushed);
                    state = (int)HeaderItemState.Pressed;
                }
                else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex && DataGridView.MouseEnteredCellAddress.X == ColumnIndex)
                {
                    state = (int)HeaderItemState.Hot;
                }

                valBounds.Inflate(16, 16);
                DataGridViewTopLeftHeaderCellRenderer.DrawHeader(graphics, valBounds, state);
                valBounds.Inflate(-16, -16);
            }
            else
            {
                Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
                    ? cellStyle.SelectionBackColor
                    : cellStyle.BackColor;

                if (!brushColor.HasTransparency())
                {
                    using var brush = brushColor.GetCachedSolidBrushScope();
                    graphics.FillRectangle(brush, valBounds);
                }
            }
        }

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        if (cellStyle.Padding != Padding.Empty)
        {
            if (DataGridView!.RightToLeftInternal)
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
        string? formattedValueStr = formattedValue as string;

        // Font independent margins
        valBounds.Offset(DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft, DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin);
        valBounds.Width -= DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft + DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginRight;
        valBounds.Height -= 2 * DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin;
        if (valBounds.Width > 0 &&
            valBounds.Height > 0 &&
            !string.IsNullOrEmpty(formattedValueStr) &&
            (paint || computeContentBounds))
        {
            Color textColor;
            if (DataGridView!.ApplyVisualStylesToHeaderCells)
            {
                textColor = DataGridViewTopLeftHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
            }

            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
            if (paint)
            {
                if (PaintContentForeground(paintParts))
                {
                    if ((flags & TextFormatFlags.SingleLine) != 0)
                    {
                        flags |= TextFormatFlags.EndEllipsis;
                    }

                    TextRenderer.DrawText(
                        graphics,
                        formattedValueStr,
                        cellStyle.Font,
                        valBounds,
                        textColor,
                        flags);
                }
            }
            else
            {
                Debug.Assert(computeContentBounds);
                resultBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedValueStr, flags, cellStyle);
            }
        }
        else if (computeErrorIconBounds && !string.IsNullOrEmpty(errorText))
        {
            resultBounds = ComputeErrorIconBounds(errorBounds);
        }

        if (DataGridView!.ShowCellErrors && paint && PaintErrorIcon(paintParts))
        {
            PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
        }

        return resultBounds;
    }

    protected override void PaintBorder(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle bounds,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle)
    {
        if (DataGridView is null)
        {
            return;
        }

        base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);

        if (!DataGridView.RightToLeftInternal &&
            DataGridView.ApplyVisualStylesToHeaderCells)
        {
            (Color darkColor, Color lightColor) = GetContrastedColors(cellStyle.BackColor);

            if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Inset)
            {
                using var penControlDark = darkColor.GetCachedPenScope();
                graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
            }
            else if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Outset)
            {
                using var penControlLightLight = lightColor.GetCachedPenScope();
                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
            }
            else if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                using var penControlDark = darkColor.GetCachedPenScope();
                graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Bottom - 1);
                graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.Right - 1, bounds.Y + 1);
            }
        }
    }

    public override string ToString() => "DataGridViewTopLeftHeaderCell";
}
