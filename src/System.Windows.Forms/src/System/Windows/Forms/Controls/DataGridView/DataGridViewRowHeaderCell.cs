// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewRowHeaderCell : DataGridViewHeaderCell
{
    private static readonly VisualStyleElement s_headerElement = VisualStyleElement.Header.Item.Normal;

    private static Bitmap? s_rightArrowBmp;
    private static Bitmap? s_leftArrowBmp;
    private static Bitmap? s_rightArrowStarBmp;
    private static Bitmap? s_leftArrowStarBmp;
    private static Bitmap? s_pencilLTRBmp;
    private static Bitmap? s_pencilRTLBmp;
    private static Bitmap? s_starBmp;

    private static readonly Type s_cellType = typeof(DataGridViewRowHeaderCell);

    private const byte RowHeaderIconMarginWidth = 3;    // 3 pixels of margin on the left and right of icons
    private const byte RowHeaderIconMarginHeight = 2;   // 2 pixels of margin on the top and bottom of icons
    private const byte ContentMarginWidth = 3;          // 3 pixels of margin on the left and right of content
    private const byte HorizontalTextMarginLeft = 1;
    private const byte HorizontalTextMarginRight = 2;
    private const byte VerticalTextMargin = 1;

    public DataGridViewRowHeaderCell()
    {
    }

    private static Bitmap LeftArrowBitmap => s_leftArrowBmp ??= GetBitmapFromIcon("DataGridViewRow.left");

    private static Bitmap LeftArrowStarBitmap => s_leftArrowStarBmp ??= GetBitmapFromIcon("DataGridViewRow.leftstar");

    private static Bitmap PencilLTRBitmap => s_pencilLTRBmp ??= GetBitmapFromIcon("DataGridViewRow.pencil_ltr");

    private static Bitmap PencilRTLBitmap => s_pencilRTLBmp ??= GetBitmapFromIcon("DataGridViewRow.pencil_rtl");

    private static Bitmap RightArrowBitmap => s_rightArrowBmp ??= GetBitmapFromIcon("DataGridViewRow.right");

    private static Bitmap RightArrowStarBitmap => s_rightArrowStarBmp ??= GetBitmapFromIcon("DataGridViewRow.rightstar");

    private static Bitmap StarBitmap => s_starBmp ??= GetBitmapFromIcon("DataGridViewRow.star");

    public override object Clone()
    {
        DataGridViewRowHeaderCell dataGridViewCell;
        Type thisType = GetType();

        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewRowHeaderCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewRowHeaderCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);
        dataGridViewCell.Value = Value;
        return dataGridViewCell;
    }

    protected override AccessibleObject CreateAccessibilityInstance() =>
        new DataGridViewRowHeaderCellAccessibleObject(this);

    private static Bitmap GetArrowBitmap(bool rightToLeft) =>
        rightToLeft
            ? LeftArrowBitmap
            : RightArrowBitmap;

    private static Bitmap GetArrowStarBitmap(bool rightToLeft) =>
        rightToLeft
            ? LeftArrowStarBitmap
            : RightArrowStarBitmap;

    private static Bitmap GetBitmapFromIcon(string iconName) =>
        ScaleHelper.GetIconResourceAsBitmap(typeof(DataGridViewHeaderCell), iconName, new Size(s_iconsWidth, s_iconsHeight));

    protected override object? GetClipboardContent(
        int rowIndex,
        bool firstCell,
        bool lastCell,
        bool inFirstRow,
        bool inLastRow,
        string format)
    {
        if (DataGridView is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);

        // Not using formatted values for header cells.
        object? val = GetValue(rowIndex);
        StringBuilder stringBuilder = new(64);

        Debug.Assert((!DataGridView.RightToLeftInternal && firstCell) || (DataGridView.RightToLeftInternal && lastCell));

        if (string.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
        {
            if (inFirstRow)
            {
                stringBuilder.Append("<TABLE>");
            }

            stringBuilder.Append("<TR>");
            stringBuilder.Append("<TD ALIGN=\"center\">");
            if (val is not null)
            {
                stringBuilder.Append("<B>");
                using StringWriter sw = new(stringBuilder, CultureInfo.CurrentCulture);
                FormatPlainTextAsHtml(val.ToString(), sw);
                stringBuilder.Append("</B>");
            }
            else
            {
                stringBuilder.Append("&nbsp;");
            }

            stringBuilder.Append("</TD>");
            if (lastCell)
            {
                stringBuilder.Append("</TR>");
                if (inLastRow)
                {
                    stringBuilder.Append("</TABLE>");
                }
            }

            return stringBuilder.ToString();
        }
        else
        {
            bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
            if (csv ||
                string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
            {
                if (val is not null)
                {
                    bool escapeApplied = false;
                    int insertionPoint = stringBuilder.Length;
                    using StringWriter sw = new(stringBuilder, CultureInfo.CurrentCulture);
                    FormatPlainText(val.ToString(), csv, sw, ref escapeApplied);
                    if (escapeApplied)
                    {
                        Debug.Assert(csv);
                        stringBuilder.Insert(insertionPoint, '"');
                    }
                }

                if (lastCell)
                {
                    if (!inLastRow)
                    {
                        stringBuilder.Append((char)Keys.Return);
                        stringBuilder.Append((char)Keys.LineFeed);
                    }
                }
                else
                {
                    stringBuilder.Append(csv ? ',' : (char)Keys.Tab);
                }

                return stringBuilder.ToString();
            }
            else
            {
                return null;
            }
        }
    }

    protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null || OwningRow is null)
        {
            return Rectangle.Empty;
        }

        object? value = GetValue(rowIndex);

        // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
        // the content bounds are computed on demand
        // we mimic a lot of the painting code

        // get the borders

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        Rectangle contentBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            value,
            errorText: null,    // contentBounds is independent of errorText
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
        ArgumentNullException.ThrowIfNull(cellStyle);

        if (DataGridView is null ||
            rowIndex < 0 ||
            !DataGridView.ShowRowErrors ||
            string.IsNullOrEmpty(GetErrorText(rowIndex)))
        {
            return Rectangle.Empty;
        }

        ComputeBorderStyleCellStateAndCellBounds(
            rowIndex,
            out DataGridViewAdvancedBorderStyle dgvabsEffective,
            out DataGridViewElementStates cellState,
            out Rectangle cellBounds);

        object? value = GetValue(rowIndex);
        object? formattedValue = GetFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            valueTypeConverter: null,
            formattedValueTypeConverter: null,
            DataGridViewDataErrorContexts.Formatting);

        Rectangle errorBounds = PaintPrivate(
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
            computeContentBounds: false,
            computeErrorIconBounds: true,
            paint: false);

        return errorBounds;
    }

    protected internal override string GetErrorText(int rowIndex) =>
        OwningRow is null
            ? base.GetErrorText(rowIndex)
            : OwningRow.GetErrorText(rowIndex);

    public override ContextMenuStrip? GetInheritedContextMenuStrip(int rowIndex)
    {
        if (DataGridView is not null)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);
        }

        ContextMenuStrip? contextMenuStrip = GetContextMenuStrip(rowIndex);

        return contextMenuStrip ?? DataGridView?.ContextMenuStrip;
    }

    public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle? inheritedCellStyle, int rowIndex, bool includeColors)
    {
        Debug.Assert(DataGridView is not null);

        DataGridViewCellStyle inheritedCellStyleTmp = inheritedCellStyle ?? new DataGridViewCellStyle();

        DataGridViewCellStyle? cellStyle = null;
        if (HasStyle)
        {
            cellStyle = Style;
            Debug.Assert(cellStyle is not null);
        }

        DataGridViewCellStyle rowHeadersStyle = DataGridView.RowHeadersDefaultCellStyle;
        Debug.Assert(rowHeadersStyle is not null);

        DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
        Debug.Assert(dataGridViewStyle is not null);

        if (includeColors)
        {
            if (cellStyle is not null && !cellStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = cellStyle.BackColor;
            }
            else if (!rowHeadersStyle.BackColor.IsEmpty)
            {
                inheritedCellStyleTmp.BackColor = rowHeadersStyle.BackColor;
            }
            else
            {
                inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
            }

            if (cellStyle is not null && !cellStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = cellStyle.ForeColor;
            }
            else if (!rowHeadersStyle.ForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.ForeColor = rowHeadersStyle.ForeColor;
            }
            else
            {
                inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
            }

            if (cellStyle is not null && !cellStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = cellStyle.SelectionBackColor;
            }
            else if (!rowHeadersStyle.SelectionBackColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionBackColor = rowHeadersStyle.SelectionBackColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
            }

            if (cellStyle is not null && !cellStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = cellStyle.SelectionForeColor;
            }
            else if (!rowHeadersStyle.SelectionForeColor.IsEmpty)
            {
                inheritedCellStyleTmp.SelectionForeColor = rowHeadersStyle.SelectionForeColor;
            }
            else
            {
                inheritedCellStyleTmp.SelectionForeColor = dataGridViewStyle.SelectionForeColor;
            }
        }

        if (cellStyle is not null && cellStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = cellStyle.Font;
        }
        else if (rowHeadersStyle.Font is not null)
        {
            inheritedCellStyleTmp.Font = rowHeadersStyle.Font;
        }
        else
        {
            inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
        }

        if (cellStyle is not null && !cellStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = cellStyle.NullValue;
        }
        else if (!rowHeadersStyle.IsNullValueDefault)
        {
            inheritedCellStyleTmp.NullValue = rowHeadersStyle.NullValue;
        }
        else
        {
            inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
        }

        if (cellStyle is not null && !cellStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = cellStyle.DataSourceNullValue;
        }
        else if (!rowHeadersStyle.IsDataSourceNullValueDefault)
        {
            inheritedCellStyleTmp.DataSourceNullValue = rowHeadersStyle.DataSourceNullValue;
        }
        else
        {
            inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
        }

        if (cellStyle is not null && cellStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = cellStyle.Format;
        }
        else if (rowHeadersStyle.Format.Length != 0)
        {
            inheritedCellStyleTmp.Format = rowHeadersStyle.Format;
        }
        else
        {
            inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
        }

        if (cellStyle is not null && !cellStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = cellStyle.FormatProvider;
        }
        else if (!rowHeadersStyle.IsFormatProviderDefault)
        {
            inheritedCellStyleTmp.FormatProvider = rowHeadersStyle.FormatProvider;
        }
        else
        {
            inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
        }

        if (cellStyle is not null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = cellStyle.Alignment;
        }
        else if (rowHeadersStyle.Alignment != DataGridViewContentAlignment.NotSet)
        {
            inheritedCellStyleTmp.AlignmentInternal = rowHeadersStyle.Alignment;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.Alignment != DataGridViewContentAlignment.NotSet);
            inheritedCellStyleTmp.AlignmentInternal = dataGridViewStyle.Alignment;
        }

        if (cellStyle is not null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = cellStyle.WrapMode;
        }
        else if (rowHeadersStyle.WrapMode != DataGridViewTriState.NotSet)
        {
            inheritedCellStyleTmp.WrapModeInternal = rowHeadersStyle.WrapMode;
        }
        else
        {
            Debug.Assert(dataGridViewStyle.WrapMode != DataGridViewTriState.NotSet);
            inheritedCellStyleTmp.WrapModeInternal = dataGridViewStyle.WrapMode;
        }

        if (cellStyle is not null && cellStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = cellStyle.Tag;
        }
        else if (rowHeadersStyle.Tag is not null)
        {
            inheritedCellStyleTmp.Tag = rowHeadersStyle.Tag;
        }
        else
        {
            inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
        }

        if (cellStyle is not null && cellStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = cellStyle.Padding;
        }
        else if (rowHeadersStyle.Padding != Padding.Empty)
        {
            inheritedCellStyleTmp.PaddingInternal = rowHeadersStyle.Padding;
        }
        else
        {
            inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
        }

        return inheritedCellStyleTmp;
    }

    private static Bitmap GetPencilBitmap(bool rightToLeft) =>
        rightToLeft
            ? PencilRTLBitmap
            : PencilLTRBitmap;

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

        DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new();

        Debug.Assert(OwningRow is not null);
        DataGridViewAdvancedBorderStyle dgvabsEffective = OwningRow.AdjustRowHeaderBorderStyle(
            DataGridView.AdvancedRowHeadersBorderStyle,
            dgvabsPlaceholder,
            singleVerticalBorderAdded: false,
            singleHorizontalBorderAdded: false,
            isFirstDisplayedRow: false,
            isLastVisibleRow: false);
        Rectangle borderWidthsRect = BorderWidths(dgvabsEffective);
        int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
        int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;

        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

        if (DataGridView.ApplyVisualStylesToHeaderCells)
        {
            // Add the theming margins to the borders.
            Rectangle rectThemeMargins = GetThemeMargins(graphics);
            borderAndPaddingWidths += rectThemeMargins.Y;
            borderAndPaddingWidths += rectThemeMargins.Height;
            borderAndPaddingHeights += rectThemeMargins.X;
            borderAndPaddingHeights += rectThemeMargins.Width;
        }

        // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
        string? val = GetValue(rowIndex) as string;

        return DataGridViewUtilities.GetPreferredRowHeaderSize(
            graphics,
            val,
            cellStyle,
            borderAndPaddingWidths,
            borderAndPaddingHeights,
            DataGridView.ShowRowErrors,
            showGlyph: true,
            constraintSize,
            flags);
    }

    protected override object? GetValue(int rowIndex)
    {
        // We allow multiple rows to share the same row header value. The row header cell's cloning does this.
        // So here we need to allow rowIndex == -1.
        if (DataGridView is not null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(rowIndex, DataGridView.Rows.Count);
        }

        return Properties.GetValueOrDefault<object?>(s_propCellValue);
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
        DataGridViewElementStates dataGridViewElementState,
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

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        Rectangle valBounds = cellBounds;
        Rectangle borderWidths = BorderWidths(advancedBorderStyle);

        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;
        Rectangle backgroundBounds = valBounds;

        bool cellSelected = (dataGridViewElementState & DataGridViewElementStates.Selected) != 0;

        Debug.Assert(DataGridView is not null);
        if (DataGridView.ApplyVisualStylesToHeaderCells)
        {
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

            if (backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
            {
                if (paint && PaintBackground(paintParts))
                {
                    // Theming
                    int state = (int)HeaderItemState.Normal;
                    if (DataGridView.SelectionMode is DataGridViewSelectionMode.FullRowSelect or DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        if (ButtonState != ButtonState.Normal)
                        {
                            Debug.Assert(ButtonState == ButtonState.Pushed);
                            state = (int)HeaderItemState.Pressed;
                        }
                        else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                 DataGridView.MouseEnteredCellAddress.X == -1)
                        {
                            state = (int)HeaderItemState.Hot;
                        }
                        else if (cellSelected)
                        {
                            state = (int)HeaderItemState.Pressed;
                        }
                    }

                    // Flip the column header background
                    using Bitmap bmFlipXPThemes = new(backgroundBounds.Height, backgroundBounds.Width);
                    using Graphics gFlip = Graphics.FromImage(bmFlipXPThemes);
                    DataGridViewRowHeaderCellRenderer.DrawHeader(gFlip, new Rectangle(0, 0, backgroundBounds.Height, backgroundBounds.Width), state);
                    bmFlipXPThemes.RotateFlip(DataGridView.RightToLeftInternal ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipY);

                    graphics.DrawImage(
                        bmFlipXPThemes,
                        backgroundBounds,
                        new Rectangle(0, 0, backgroundBounds.Width, backgroundBounds.Height),
                        GraphicsUnit.Pixel);
                }

                // update the val bounds
                Rectangle rectThemeMargins = GetThemeMargins(graphics);
                if (DataGridView.RightToLeftInternal)
                {
                    valBounds.X += rectThemeMargins.Height;
                }
                else
                {
                    valBounds.X += rectThemeMargins.Y;
                }

                valBounds.Width -= rectThemeMargins.Y + rectThemeMargins.Height;
                valBounds.Height -= rectThemeMargins.X + rectThemeMargins.Width;
                valBounds.Y += rectThemeMargins.X;
            }
        }
        else
        {
            // No visual style applied
            if (valBounds.Width > 0 && valBounds.Height > 0)
            {
                Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
                    ? cellStyle.SelectionBackColor
                    : cellStyle.BackColor;

                if (paint && PaintBackground(paintParts) && !brushColor.HasTransparency())
                {
                    using var brush = brushColor.GetCachedSolidBrushScope();
                    graphics.FillRectangle(brush, valBounds);
                }
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
        }

        Bitmap? bmp = null;

        if (valBounds.Width > 0 && valBounds.Height > 0)
        {
            Rectangle errorBounds = valBounds;
            string? formattedString = formattedValue as string;
            if (!string.IsNullOrEmpty(formattedString))
            {
                // There is text to display
                if (valBounds.Width >= s_iconsWidth + 2 * RowHeaderIconMarginWidth
                    && valBounds.Height >= s_iconsHeight + 2 * RowHeaderIconMarginHeight)
                {
                    if (paint && PaintContentBackground(paintParts))
                    {
                        // There is enough room for the potential glyph which is the first priority
                        if (DataGridView.CurrentCellAddress.Y == rowIndex)
                        {
                            if (DataGridView.VirtualMode)
                            {
                                if (DataGridView.IsCurrentRowDirty && DataGridView.ShowEditingIcon)
                                {
                                    bmp = GetPencilBitmap(DataGridView.RightToLeftInternal);
                                }
                                else if (DataGridView.NewRowIndex == rowIndex)
                                {
                                    bmp = GetArrowStarBitmap(DataGridView.RightToLeftInternal);
                                }
                                else
                                {
                                    bmp = GetArrowBitmap(DataGridView.RightToLeftInternal);
                                }
                            }
                            else
                            {
                                if (DataGridView.IsCurrentCellDirty && DataGridView.ShowEditingIcon)
                                {
                                    bmp = GetPencilBitmap(DataGridView.RightToLeftInternal);
                                }
                                else if (DataGridView.NewRowIndex == rowIndex)
                                {
                                    bmp = GetArrowStarBitmap(DataGridView.RightToLeftInternal);
                                }
                                else
                                {
                                    bmp = GetArrowBitmap(DataGridView.RightToLeftInternal);
                                }
                            }
                        }
                        else if (DataGridView.NewRowIndex == rowIndex)
                        {
                            bmp = StarBitmap;
                        }

                        if (bmp is not null)
                        {
                            Color iconColor;
                            if (DataGridView.ApplyVisualStylesToHeaderCells)
                            {
                                iconColor = DataGridViewRowHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                            }
                            else
                            {
                                iconColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                            }

                            lock (bmp)
                            {
                                PaintIcon(graphics, bmp, valBounds, iconColor, cellStyle.BackColor);
                            }
                        }
                    }

                    if (!DataGridView.RightToLeftInternal)
                    {
                        valBounds.X += s_iconsWidth + 2 * RowHeaderIconMarginWidth;
                    }

                    valBounds.Width -= s_iconsWidth + 2 * RowHeaderIconMarginWidth;
                    Debug.Assert(valBounds.Width >= 0);
                    Debug.Assert(valBounds.Height >= 0);
                }

                // Second priority is text
                // Font independent margins
                valBounds.Offset(HorizontalTextMarginLeft + ContentMarginWidth, VerticalTextMargin);
                valBounds.Width -= HorizontalTextMarginLeft + 2 * ContentMarginWidth + HorizontalTextMarginRight;
                valBounds.Height -= 2 * VerticalTextMargin;
                if (valBounds.Width > 0 && valBounds.Height > 0)
                {
                    TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                    if (DataGridView.ShowRowErrors && valBounds.Width > s_iconsWidth + 2 * RowHeaderIconMarginWidth)
                    {
                        // Check if the text fits if we remove the room required for the row error icon
                        Size maxBounds = new(valBounds.Width - s_iconsWidth - 2 * RowHeaderIconMarginWidth, valBounds.Height);
                        if (TextFitsInBounds(
                            graphics,
                            formattedString,
                            cellStyle.Font!,
                            maxBounds,
                            flags))
                        {
                            // There is enough room for both the text and the row error icon, so use it all.
                            if (DataGridView.RightToLeftInternal)
                            {
                                valBounds.X += s_iconsWidth + 2 * RowHeaderIconMarginWidth;
                            }

                            valBounds.Width -= s_iconsWidth + 2 * RowHeaderIconMarginWidth;
                        }
                    }

                    if (PaintContentForeground(paintParts))
                    {
                        if (paint)
                        {
                            Color textColor;
                            if (DataGridView.ApplyVisualStylesToHeaderCells)
                            {
                                textColor = DataGridViewRowHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                            }
                            else
                            {
                                textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                            }

                            if ((flags & TextFormatFlags.SingleLine) != 0)
                            {
                                flags |= TextFormatFlags.EndEllipsis;
                            }

                            TextRenderer.DrawText(
                                graphics,
                                formattedString,
                                cellStyle.Font,
                                valBounds,
                                textColor,
                                flags);
                        }
                        else if (computeContentBounds)
                        {
                            resultBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedString, flags, cellStyle);
                        }
                    }
                }

                // Third priority is the row error icon, which may be painted on top of text
                if (errorBounds.Width >= 3 * RowHeaderIconMarginWidth + 2 * s_iconsWidth)
                {
                    // There is enough horizontal room for the error icon and the glyph
                    if (paint && DataGridView.ShowRowErrors && PaintErrorIcon(paintParts))
                    {
                        PaintErrorIcon(graphics, clipBounds, errorBounds, errorText);
                    }
                    else if (computeErrorIconBounds)
                    {
                        if (!string.IsNullOrEmpty(errorText))
                        {
                            resultBounds = ComputeErrorIconBounds(errorBounds);
                        }
                    }
                }
            }
            else
            {
                // There is no text to display
                if (valBounds.Width >= s_iconsWidth + 2 * RowHeaderIconMarginWidth &&
                    valBounds.Height >= s_iconsHeight + 2 * RowHeaderIconMarginHeight)
                {
                    if (paint && PaintContentBackground(paintParts))
                    {
                        // There is enough room for the potential icon
                        if (DataGridView.CurrentCellAddress.Y == rowIndex)
                        {
                            if (DataGridView.VirtualMode)
                            {
                                if (DataGridView.IsCurrentRowDirty && DataGridView.ShowEditingIcon)
                                {
                                    bmp = GetPencilBitmap(DataGridView.RightToLeftInternal);
                                }
                                else if (DataGridView.NewRowIndex == rowIndex)
                                {
                                    bmp = GetArrowStarBitmap(DataGridView.RightToLeftInternal);
                                }
                                else
                                {
                                    bmp = GetArrowBitmap(DataGridView.RightToLeftInternal);
                                }
                            }
                            else
                            {
                                if (DataGridView.IsCurrentCellDirty && DataGridView.ShowEditingIcon)
                                {
                                    bmp = GetPencilBitmap(DataGridView.RightToLeftInternal);
                                }
                                else if (DataGridView.NewRowIndex == rowIndex)
                                {
                                    bmp = GetArrowStarBitmap(DataGridView.RightToLeftInternal);
                                }
                                else
                                {
                                    bmp = GetArrowBitmap(DataGridView.RightToLeftInternal);
                                }
                            }
                        }
                        else if (DataGridView.NewRowIndex == rowIndex)
                        {
                            bmp = StarBitmap;
                        }

                        if (bmp is not null)
                        {
                            lock (bmp)
                            {
                                Color iconColor;
                                if (DataGridView.ApplyVisualStylesToHeaderCells)
                                {
                                    iconColor = DataGridViewRowHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                                }
                                else
                                {
                                    iconColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                                }

                                PaintIcon(graphics, bmp, valBounds, iconColor, cellStyle.BackColor);
                            }
                        }
                    }
                }

                if (errorBounds.Width >= 3 * RowHeaderIconMarginWidth +
                                         2 * s_iconsWidth)
                {
                    // There is enough horizontal room for the error icon
                    if (paint && DataGridView.ShowRowErrors && PaintErrorIcon(paintParts))
                    {
                        PaintErrorIcon(
                            graphics,
                            cellStyle,
                            rowIndex,
                            cellBounds,
                            errorBounds,
                            errorText);
                    }
                    else if (computeErrorIconBounds)
                    {
                        if (!string.IsNullOrEmpty(errorText))
                        {
                            resultBounds = ComputeErrorIconBounds(errorBounds);
                        }
                    }
                }
            }
        }

        // else no room for content or error icon, resultBounds = Rectangle.Empty

        return resultBounds;
    }

    private void PaintIcon(
        Graphics g,
        Bitmap bmp,
        Rectangle bounds,
        Color foreColor,
        Color backColor)
    {
        Debug.Assert(DataGridView is not null);
        int width = DataGridView.RightToLeftInternal
            ? bounds.Right - RowHeaderIconMarginWidth - s_iconsWidth
            : bounds.Left + RowHeaderIconMarginWidth;
        int height = bounds.Y + (bounds.Height - s_iconsHeight) / 2;
        Rectangle bmpRect = new(width, height, s_iconsWidth, s_iconsHeight);

        (Color OldColor, Color NewColor) map = new(Color.Black, foreColor);
        using ImageAttributes attr = new();
        attr.SetRemapTable(ColorAdjustType.Bitmap, new ReadOnlySpan<(Color OldColor, Color NewColor)>(ref map));

        if (SystemInformation.HighContrast &&
            // We can't replace black with white and vice versa as in other cases due to the colors of images are
            // not exactly black and white. Also, we can't make a decision of inverting every pixel by comparing
            // it with a background because it causes artifacts in the image.
            // Because the primary color of all images provided to this method is similar to
            // black (brightness almost zero), the decision to invert color may be made by checking
            // the background color's brightness.
            ControlPaint.IsDark(backColor))
        {
            using Bitmap invertedBitmap = ControlPaint.CreateBitmapWithInvertedForeColor(bmp, backColor);
            g.DrawImage(invertedBitmap, bmpRect, 0, 0, s_iconsWidth, s_iconsHeight, GraphicsUnit.Pixel, attr);
        }
        else
        {
            g.DrawImage(bmp, bmpRect, 0, 0, s_iconsWidth, s_iconsHeight, GraphicsUnit.Pixel, attr);
        }
    }

    protected override bool SetValue(int rowIndex, object? value)
    {
        object? originalValue = GetValue(rowIndex);
        Properties.AddOrRemoveValue(s_propCellValue, value);

        if (DataGridView is not null && originalValue != value)
        {
            RaiseCellValueChanged(new DataGridViewCellEventArgs(-1, rowIndex));
        }

        return true;
    }

    /// <summary>
    /// </summary>
    public override string ToString() =>
        $"DataGridViewRowHeaderCell {{ RowIndex={RowIndex} }}";
}
