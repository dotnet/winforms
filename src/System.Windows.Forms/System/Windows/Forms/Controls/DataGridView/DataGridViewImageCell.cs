// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Windows.Forms;

public partial class DataGridViewImageCell : DataGridViewCell
{
    private static readonly int s_propImageCellDescription = PropertyStore.CreateKey();
    private static readonly int s_propImageCellLayout = PropertyStore.CreateKey();
    private static readonly Type s_defaultTypeImage = typeof(Image);
    private static readonly Type s_defaultTypeIcon = typeof(Icon);
    private static readonly Type s_cellType = typeof(DataGridViewImageCell);
    private static Bitmap? s_errorBitmap;
    private static Icon? s_errorIcon;

    private const byte CellValueIsIcon = 0x01;

    private byte _flags;  // see DATAGRIDVIEWIMAGECELL_ constants above

    public DataGridViewImageCell()
        : this(valueIsIcon: false)
    {
    }

    public DataGridViewImageCell(bool valueIsIcon)
    {
        if (valueIsIcon)
        {
            _flags = CellValueIsIcon;
        }
    }

    public override object? DefaultNewRowValue
    {
        get
        {
            if (s_defaultTypeImage.IsAssignableFrom(ValueType))
            {
                return ErrorBitmap;
            }
            else if (s_defaultTypeIcon.IsAssignableFrom(ValueType))
            {
                return ErrorIcon;
            }
            else
            {
                return null;
            }
        }
    }

    [DefaultValue("")]
    [AllowNull]
    public string Description
    {
        get => Properties.GetStringOrEmptyString(s_propImageCellDescription);
        set => Properties.AddOrRemoveString(s_propImageCellDescription, value);
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    public override Type? EditType
    {
        get
        {
            // Image cells don't have an editor
            return null;
        }
    }

    internal static Bitmap ErrorBitmap =>
        s_errorBitmap ??= ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(DataGridView), "ImageInError");

    internal static Icon ErrorIcon => s_errorIcon ??= new Icon(typeof(DataGridView), "IconInError");

    public override Type FormattedValueType
    {
        get
        {
            if (ValueIsIcon)
            {
                return s_defaultTypeIcon;
            }
            else
            {
                return s_defaultTypeImage;
            }
        }
    }

    [DefaultValue(DataGridViewImageCellLayout.NotSet)]
    public DataGridViewImageCellLayout ImageLayout
    {
        get => Properties.GetValueOrDefault(s_propImageCellLayout, DataGridViewImageCellLayout.Normal);
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (ImageLayout != value)
            {
                Properties.AddOrRemoveValue(s_propImageCellLayout, value, defaultValue: DataGridViewImageCellLayout.Normal);
                OnCommonChange();
            }
        }
    }

    internal DataGridViewImageCellLayout ImageLayoutInternal
    {
        set
        {
            Debug.Assert(value is >= DataGridViewImageCellLayout.NotSet and <= DataGridViewImageCellLayout.Zoom);
            Properties.AddOrRemoveValue(s_propImageCellLayout, value, defaultValue: DataGridViewImageCellLayout.Normal);
        }
    }

    [DefaultValue(false)]
    public bool ValueIsIcon
    {
        get => ((_flags & CellValueIsIcon) != 0x00);
        set
        {
            if (ValueIsIcon != value)
            {
                ValueIsIconInternal = value;
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

    internal bool ValueIsIconInternal
    {
        set
        {
            if (ValueIsIcon != value)
            {
                if (value)
                {
                    _flags |= CellValueIsIcon;
                }
                else
                {
                    _flags = (byte)(_flags & ~CellValueIsIcon);
                }

                if (DataGridView is not null &&
                    RowIndex != -1 &&
                    DataGridView.NewRowIndex == RowIndex &&
                    !DataGridView.VirtualMode)
                {
                    Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                    // We automatically update the content of the new row's cell based on the new ValueIsIcon value.
                    if ((value && Value == ErrorBitmap) || (!value && Value == ErrorIcon))
                    {
                        Value = DefaultNewRowValue;
                    }
                }
            }
        }
    }

    public override Type? ValueType
    {
        get
        {
            Type? baseValueType = base.ValueType;

            if (baseValueType is not null)
            {
                return baseValueType;
            }

            if (ValueIsIcon)
            {
                return s_defaultTypeIcon;
            }
            else
            {
                return s_defaultTypeImage;
            }
        }
        set
        {
            base.ValueType = value;
            ValueIsIcon = (value is not null && s_defaultTypeIcon.IsAssignableFrom(value));
        }
    }

    public override object Clone()
    {
        DataGridViewImageCell dataGridViewCell;
        Type thisType = GetType();

        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewImageCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewImageCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);
        dataGridViewCell.ValueIsIconInternal = ValueIsIcon;
        dataGridViewCell.Description = Description;
        dataGridViewCell.ImageLayoutInternal = ImageLayout;
        return dataGridViewCell;
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new DataGridViewImageCellAccessibleObject(this);

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

        Rectangle imgBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            errorText: null,    // imgBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);

#if DEBUG
        Rectangle imgBoundsDebug = PaintPrivate(
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
        Debug.Assert(imgBoundsDebug.Equals(imgBounds));
#endif

        return imgBounds;
    }

    private protected override string GetDefaultToolTipText() => SR.DefaultDataGridViewImageCellToolTipText;

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

        Rectangle errBounds = PaintPrivate(
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

#if DEBUG
        Rectangle errBoundsDebug = PaintPrivate(graphics,
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
        Debug.Assert(errBoundsDebug.Equals(errBounds));
#endif

        return errBounds;
    }

    protected override object? GetFormattedValue(
        object? value,
        int rowIndex,
        ref DataGridViewCellStyle cellStyle,
        TypeConverter? valueTypeConverter,
        TypeConverter? formattedValueTypeConverter,
        DataGridViewDataErrorContexts context)
    {
        if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
        {
            return Description;
        }

        object? formattedValue = base.GetFormattedValue(
            value,
            rowIndex,
            ref cellStyle,
            valueTypeConverter,
            formattedValueTypeConverter,
            context);
        if (formattedValue is null && cellStyle.NullValue is null)
        {
            return null;
        }

        if (ValueIsIcon)
        {
            if (formattedValue is not Icon ico)
            {
                ico = ErrorIcon;
            }

            return ico;
        }
        else
        {
            if (formattedValue is not Image img)
            {
                img = ErrorBitmap;
            }

            return img;
        }
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
        Image? img = formattedValue as Image;
        Icon? ico = null;
        if (img is null)
        {
            ico = formattedValue as Icon;
        }

        if (freeDimension == DataGridViewFreeDimension.Height &&
            ImageLayout == DataGridViewImageCellLayout.Zoom)
        {
            if (img is not null || ico is not null)
            {
                if (img is not null)
                {
                    int imgWidthAllowed = constraintSize.Width - borderAndPaddingWidths;
                    if (imgWidthAllowed <= 0 || img.Width == 0)
                    {
                        preferredSize = Size.Empty;
                    }
                    else
                    {
                        preferredSize = new Size(0, Math.Min(img.Height, decimal.ToInt32((decimal)img.Height * imgWidthAllowed / img.Width)));
                    }
                }
                else
                {
                    int icoWidthAllowed = constraintSize.Width - borderAndPaddingWidths;
                    if (icoWidthAllowed <= 0 || ico!.Width == 0)
                    {
                        preferredSize = Size.Empty;
                    }
                    else
                    {
                        preferredSize = new Size(0, Math.Min(ico.Height, decimal.ToInt32((decimal)ico.Height * icoWidthAllowed / ico.Width)));
                    }
                }
            }
            else
            {
                preferredSize = new Size(0, 1);
            }
        }
        else if (freeDimension == DataGridViewFreeDimension.Width &&
                 ImageLayout == DataGridViewImageCellLayout.Zoom)
        {
            if (img is not null || ico is not null)
            {
                if (img is not null)
                {
                    int imgHeightAllowed = constraintSize.Height - borderAndPaddingHeights;
                    if (imgHeightAllowed <= 0 || img.Height == 0)
                    {
                        preferredSize = Size.Empty;
                    }
                    else
                    {
                        preferredSize = new Size(Math.Min(img.Width, decimal.ToInt32((decimal)img.Width * imgHeightAllowed / img.Height)), 0);
                    }
                }
                else
                {
                    int icoHeightAllowed = constraintSize.Height - borderAndPaddingHeights;
                    if (icoHeightAllowed <= 0 || ico!.Height == 0)
                    {
                        preferredSize = Size.Empty;
                    }
                    else
                    {
                        preferredSize = new Size(Math.Min(ico.Width, decimal.ToInt32((decimal)ico.Width * icoHeightAllowed / ico.Height)), 0);
                    }
                }
            }
            else
            {
                preferredSize = new Size(1, 0);
            }
        }
        else
        {
            if (img is not null)
            {
                preferredSize = new Size(img.Width, img.Height);
            }
            else if (ico is not null)
            {
                preferredSize = new Size(ico.Width, ico.Height);
            }
            else
            {
                preferredSize = new Size(1, 1);
            }

            if (freeDimension == DataGridViewFreeDimension.Height)
            {
                preferredSize.Width = 0;
            }
            else if (freeDimension == DataGridViewFreeDimension.Width)
            {
                preferredSize.Height = 0;
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Height)
        {
            preferredSize.Width += borderAndPaddingWidths;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + IconMarginWidth * 2 + s_iconsWidth);
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Width)
        {
            preferredSize.Height += borderAndPaddingHeights;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + IconMarginHeight * 2 + s_iconsHeight);
            }
        }

        return preferredSize;
    }

    protected override object? GetValue(int rowIndex)
    {
        object? valueBase = base.GetValue(rowIndex);
        if (valueBase is null)
        {
            if (OwningColumn is DataGridViewImageColumn owningImageColumn)
            {
                if (s_defaultTypeImage.IsAssignableFrom(ValueType))
                {
                    Image? image = owningImageColumn.Image;
                    if (image is not null)
                    {
                        return image;
                    }
                }
                else if (s_defaultTypeIcon.IsAssignableFrom(ValueType))
                {
                    Icon? icon = owningImageColumn.Icon;
                    if (icon is not null)
                    {
                        return icon;
                    }
                }
            }
        }

        return valueBase;
    }

    private Rectangle ImageBounds(
        Rectangle bounds,
        int imgWidth,
        int imgHeight,
        DataGridViewImageCellLayout imageLayout,
        DataGridViewCellStyle cellStyle)
    {
        // When the imageLayout == stretch there is nothing to do
        Debug.Assert(imageLayout != DataGridViewImageCellLayout.Stretch);

        Rectangle imageBounds = Rectangle.Empty;

        switch (imageLayout)
        {
            case DataGridViewImageCellLayout.Normal:
            case DataGridViewImageCellLayout.NotSet:
                imageBounds = new Rectangle(bounds.X, bounds.Y, imgWidth, imgHeight);
                break;
            case DataGridViewImageCellLayout.Zoom:
                // We have to determine which side will be fully filled: the height or the width
                if (imgWidth * bounds.Height < imgHeight * bounds.Width)
                {
                    // We fill the height
                    imageBounds = new Rectangle(
                        bounds.X,
                        bounds.Y,
                        decimal.ToInt32((decimal)imgWidth * bounds.Height / imgHeight),
                        bounds.Height);
                }
                else
                {
                    // we fill the width
                    imageBounds = new Rectangle(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        decimal.ToInt32((decimal)imgHeight * bounds.Width / imgWidth));
                }

                break;
        }

        // Now use the alignment on the cellStyle to determine the final bounds
        if (DataGridView!.RightToLeftInternal)
        {
            imageBounds.X = cellStyle.Alignment switch
            {
                DataGridViewContentAlignment.TopRight => bounds.X,
                DataGridViewContentAlignment.TopLeft => bounds.Right - imageBounds.Width,
                DataGridViewContentAlignment.MiddleRight => bounds.X,
                DataGridViewContentAlignment.MiddleLeft => bounds.Right - imageBounds.Width,
                DataGridViewContentAlignment.BottomRight => bounds.X,
                DataGridViewContentAlignment.BottomLeft => bounds.Right - imageBounds.Width,
                _ => imageBounds.X
            };
        }
        else
        {
            imageBounds.X = cellStyle.Alignment switch
            {
                DataGridViewContentAlignment.TopRight => bounds.Right - imageBounds.Width,
                DataGridViewContentAlignment.TopLeft => bounds.X,
                DataGridViewContentAlignment.MiddleRight => bounds.Right - imageBounds.Width,
                DataGridViewContentAlignment.MiddleLeft => bounds.X,
                DataGridViewContentAlignment.BottomRight => bounds.Right - imageBounds.Width,
                DataGridViewContentAlignment.BottomLeft => bounds.X,
                _ => imageBounds.X
            };
        }

        switch (cellStyle.Alignment)
        {
            case DataGridViewContentAlignment.TopCenter:
            case DataGridViewContentAlignment.MiddleCenter:
            case DataGridViewContentAlignment.BottomCenter:
                imageBounds.X = bounds.X + (bounds.Width - imageBounds.Width) / 2;
                break;
        }

        switch (cellStyle.Alignment)
        {
            case DataGridViewContentAlignment.TopLeft:
            case DataGridViewContentAlignment.TopCenter:
            case DataGridViewContentAlignment.TopRight:
                imageBounds.Y = bounds.Y;
                break;

            case DataGridViewContentAlignment.MiddleLeft:
            case DataGridViewContentAlignment.MiddleCenter:
            case DataGridViewContentAlignment.MiddleRight:
                imageBounds.Y = bounds.Y + (bounds.Height - imageBounds.Height) / 2;
                break;

            case DataGridViewContentAlignment.BottomLeft:
            case DataGridViewContentAlignment.BottomCenter:
            case DataGridViewContentAlignment.BottomRight:
                imageBounds.Y = bounds.Bottom - imageBounds.Height;
                break;

            default:
                Debug.Assert(
                    cellStyle.Alignment == DataGridViewContentAlignment.NotSet,
                    "this is the only alignment left");
                break;
        }

        return imageBounds;
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
        // Parameter checking. One bit and one bit only should be turned on.
        Debug.Assert(paint || computeContentBounds || computeErrorIconBounds);
        Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds);
        Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint);
        Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds);
        Debug.Assert(cellStyle is not null);

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        Rectangle resultBounds = Rectangle.Empty;
        Rectangle valBounds = cellBounds;
        Rectangle borderWidths = BorderWidths(advancedBorderStyle);
        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;

        if (valBounds.Width <= 0 || valBounds.Height <= 0 || (!paint && !computeContentBounds))
        {
            if (computeErrorIconBounds)
            {
                return string.IsNullOrEmpty(errorText)
                    ? Rectangle.Empty
                    : ComputeErrorIconBounds(valBounds);
            }
            else
            {
                Debug.Assert(valBounds.Height <= 0 || valBounds.Width <= 0);
                return Rectangle.Empty;
            }
        }

        Rectangle imageBounds = valBounds;
        if (cellStyle.Padding != Padding.Empty)
        {
            imageBounds.Offset(
                DataGridView!.RightToLeftInternal ? cellStyle.Padding.Right : cellStyle.Padding.Left,
                cellStyle.Padding.Top);
            imageBounds.Width -= cellStyle.Padding.Horizontal;
            imageBounds.Height -= cellStyle.Padding.Vertical;
        }

        bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
        Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
            ? cellStyle.SelectionBackColor
            : cellStyle.BackColor;

        using var brush = paint ? brushColor.GetCachedSolidBrushScope() : default;

        Image? image = formattedValue as Image;
        Icon? icon = image is null ? formattedValue as Icon : null;

        if (imageBounds.Width <= 0 || imageBounds.Height <= 0 || (icon is null && image is null))
        {
            if (paint && !brushColor.HasTransparency() && PaintBackground(paintParts))
            {
                g.FillRectangle(brush!, valBounds);
            }

            if (!paint)
            {
                return Rectangle.Empty;
            }
        }
        else
        {
            DataGridViewImageCellLayout imageLayout = ImageLayout;
            if (imageLayout == DataGridViewImageCellLayout.NotSet)
            {
                if (OwningColumn is DataGridViewImageColumn column)
                {
                    imageLayout = column.ImageLayout;
                    Debug.Assert(imageLayout != DataGridViewImageCellLayout.NotSet);
                }
                else
                {
                    imageLayout = DataGridViewImageCellLayout.Normal;
                }
            }

            if (imageLayout == DataGridViewImageCellLayout.Stretch)
            {
                resultBounds = imageBounds;
                if (!paint)
                {
                    return resultBounds;
                }

                if (PaintBackground(paintParts))
                {
                    PaintPadding(g, valBounds, cellStyle, brush!, DataGridView!.RightToLeftInternal);
                }

                if (PaintContentForeground(paintParts))
                {
                    if (image is not null)
                    {
                        using ImageAttributes attr = new();
                        attr.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(image, imageBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
                    }
                    else
                    {
                        g.DrawIcon(icon!, imageBounds);
                    }
                }
            }
            else
            {
                Rectangle imageBounds2 = ImageBounds(
                    imageBounds,
                    image is null ? icon!.Width : image.Width,
                    image is null ? icon!.Height : image.Height,
                    imageLayout,
                    cellStyle);

                resultBounds = imageBounds2;

                if (!paint)
                {
                    return resultBounds;
                }

                if (PaintBackground(paintParts) && !brushColor.HasTransparency())
                {
                    g.FillRectangle(brush!, valBounds);
                }

                if (PaintContentForeground(paintParts))
                {
                    // Paint the image
                    using Region originalClip = g.Clip;
                    try
                    {
                        g.SetClip(Rectangle.Intersect(
                            Rectangle.Intersect(imageBounds2, imageBounds),
                            Rectangle.Truncate(g.VisibleClipBounds)));

                        if (image is not null)
                        {
                            g.DrawImage(image, imageBounds2);
                        }
                        else
                        {
                            g.DrawIconUnstretched(icon!, imageBounds2);
                        }
                    }
                    finally
                    {
                        g.Clip = originalClip;
                    }
                }
            }
        }

        Point ptCurrentCell = DataGridView!.CurrentCellAddress;
        if (paint
            && PaintFocus(paintParts)
            && ptCurrentCell.X == ColumnIndex
            && ptCurrentCell.Y == rowIndex
            && DataGridView.ShowFocusCues
            && DataGridView.Focused)
        {
            // Draw focus rectangle
            ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, brushColor);
        }

        if (paint && DataGridView.ShowCellErrors && PaintErrorIcon(paintParts))
        {
            PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, valBounds, errorText);
        }

        return resultBounds;
    }

    public override string ToString() => $"DataGridViewImageCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";
}
