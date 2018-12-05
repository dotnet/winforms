// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using System.Diagnostics;
    using System.Security.Permissions;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell"]/*' />
    public class DataGridViewImageCell : DataGridViewCell
    {
        private static ColorMap[] colorMap = new ColorMap[] { new ColorMap() };
        private static readonly int PropImageCellDescription = PropertyStore.CreateKey();
        private static readonly int PropImageCellLayout = PropertyStore.CreateKey();
        private static Type defaultTypeImage = typeof(System.Drawing.Image);
        private static Type defaultTypeIcon = typeof(System.Drawing.Icon);
        private static Type cellType = typeof(DataGridViewImageCell);
        private static Bitmap errorBmp = null;
        private static Icon errorIco = null;

        private const byte DATAGRIDVIEWIMAGECELL_valueIsIcon = 0x01;

        private byte flags;  // see DATAGRIDVIEWIMAGECELL_ consts above

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.DataGridViewImageCell1"]/*' />
        public DataGridViewImageCell() : this(false /*valueIsIcon*/)
        {
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.DataGridViewImageCell2"]/*' />
        public DataGridViewImageCell(bool valueIsIcon)
        {
            if (valueIsIcon)
            {
                this.flags = DATAGRIDVIEWIMAGECELL_valueIsIcon;
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.DefaultNewRowValue"]/*' />
        public override object DefaultNewRowValue
        {
            get
            {
                if (defaultTypeImage.IsAssignableFrom(this.ValueType))
                {
                    return ErrorBitmap;
                }
                else if (defaultTypeIcon.IsAssignableFrom(this.ValueType))
                {
                    return ErrorIcon;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.Description"]/*' />
        [
            DefaultValue("")
        ]
        public string Description
        { 
            get
            {
                object description = this.Properties.GetObject(PropImageCellDescription);
                if (description != null)
                {
                    return (string) description;
                }
                return string.Empty;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) || this.Properties.ContainsObject(PropImageCellDescription))
                {
                    this.Properties.SetObject(PropImageCellDescription, value);
                }
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.EditType"]/*' />
        public override Type EditType
        {
            get
            {
                // Image cells don't have an editor
                return null;
            }
        }

        static internal Bitmap ErrorBitmap
        {
            get
            {
                if (errorBmp == null)
                {
                    errorBmp = new Bitmap(typeof(DataGridView), "ImageInError.bmp");
                }
                return errorBmp;
            }
        }

        static internal Icon ErrorIcon
        {
            get
            {
                if (errorIco == null)
                {
                    errorIco = new Icon(typeof(DataGridView), "IconInError.ico");
                }
                return errorIco;
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.FormattedValueType"]/*' />
        public override Type FormattedValueType
        {
            get
            {
                if (this.ValueIsIcon)
                {
                    return defaultTypeIcon;
                }
                else
                {
                    return defaultTypeImage;
                }
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.ImageLayout"]/*' />
        [
            DefaultValue(DataGridViewImageCellLayout.NotSet)
        ]
        public DataGridViewImageCellLayout ImageLayout
        {
            get
            {
                bool found;
                int imageLayout = this.Properties.GetInteger(PropImageCellLayout, out found);
                if (found)
                {
                    return (DataGridViewImageCellLayout)imageLayout;
                }
                return DataGridViewImageCellLayout.Normal;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewImageCellLayout.NotSet, (int)DataGridViewImageCellLayout.Zoom))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewImageCellLayout)); 
                }
                if (this.ImageLayout != value)
                {
                    this.Properties.SetInteger(PropImageCellLayout, (int)value);
                    OnCommonChange();
                }
            }
        }

        internal DataGridViewImageCellLayout ImageLayoutInternal
        {
            set
            {
                Debug.Assert(value >= DataGridViewImageCellLayout.NotSet && value <= DataGridViewImageCellLayout.Zoom);
                if (this.ImageLayout != value)
                {
                    this.Properties.SetInteger(PropImageCellLayout, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.ValueIsIcon"]/*' />
        [
            DefaultValue(false)
        ]
        public bool ValueIsIcon
        {
            get
            {
                return ((this.flags & DATAGRIDVIEWIMAGECELL_valueIsIcon) != 0x00);
            }
            set
            {
                if (this.ValueIsIcon != value)
                {
                    this.ValueIsIconInternal = value;
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

        internal bool ValueIsIconInternal
        {
            set
            {
                if (this.ValueIsIcon != value)
                {
                    if (value)
                    {
                        this.flags |= (byte) DATAGRIDVIEWIMAGECELL_valueIsIcon;
                    }
                    else
                    {
                        this.flags = (byte) (this.flags & ~DATAGRIDVIEWIMAGECELL_valueIsIcon);
                    }
                    if (this.DataGridView != null && 
                        this.RowIndex != -1 && 
                        this.DataGridView.NewRowIndex == this.RowIndex && 
                        !this.DataGridView.VirtualMode)
                    {
                        Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                        // We automatically update the content of the new row's cell based on the new ValueIsIcon value.
                        if ((value && this.Value == ErrorBitmap) || (!value && this.Value == ErrorIcon))
                        {
                            this.Value = this.DefaultNewRowValue;
                        }
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.ValueType"]/*' />
        public override Type ValueType
        {
            get
            {
                Type baseValueType = base.ValueType;

                if (baseValueType != null)
                {
                    return baseValueType;
                }

                if (this.ValueIsIcon)
                {
                    return defaultTypeIcon;
                }
                else
                {
                    return defaultTypeImage;
                }
            }
            set
            {
                base.ValueType = value;
                this.ValueIsIcon = (value != null && defaultTypeIcon.IsAssignableFrom(value));
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewImageCell dataGridViewCell;
            Type thisType = this.GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewImageCell();
            }
            else
            {
                // 

                dataGridViewCell = (DataGridViewImageCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.ValueIsIconInternal = this.ValueIsIcon;
            dataGridViewCell.Description = this.Description;
            dataGridViewCell.ImageLayoutInternal = this.ImageLayout;
            return dataGridViewCell;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewImageCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.GetContentBounds"]/*' />
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

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle imgBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                null /*errorText*/,                 // imgBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            Rectangle imgBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(imgBoundsDebug.Equals(imgBounds));
#endif

            return imgBounds;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.GetErrorIconBounds"]/*' />
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

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

            Rectangle errBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true  /*computeErrorIconBounds*/,
                false /*paint*/);

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
                false /*computeContentBounds*/,
                true  /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(errBoundsDebug.Equals(errBounds));
#endif

            return errBounds;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.GetFormattedValue"]/*' />
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        protected override object GetFormattedValue(object value,
                                                    int rowIndex,
                                                    ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter,
                                                    TypeConverter formattedValueTypeConverter,
                                                    DataGridViewDataErrorContexts context)
        {
            if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
            {
                return this.Description;
            }

            object formattedValue = base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            if (formattedValue == null && cellStyle.NullValue == null)
            {
                return null;
            }
            if (this.ValueIsIcon)
            {
                Icon ico = formattedValue as Icon;
                if (ico == null)
                {
                    ico = ErrorIcon;
                }
                return ico;
            }
            else
            {
                Image img = formattedValue as Image;
                if (img == null)
                {
                    img = ErrorBitmap;
                }
                return img;
            }
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.GetPreferredSize"]/*' />
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
            object formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize);
            Image img = formattedValue as Image;
            Icon ico = null;
            if (img == null)
            {
                ico = formattedValue as Icon;
            }

            if (freeDimension == DataGridViewFreeDimension.Height &&
                this.ImageLayout == DataGridViewImageCellLayout.Zoom)
            {
                if (img != null || ico != null)
                {
                    if (img != null)
                    {
                        int imgWidthAllowed = constraintSize.Width - borderAndPaddingWidths;
                        if (imgWidthAllowed <= 0 || img.Width == 0)
                        {
                            preferredSize = Size.Empty;
                        }
                        else
                        {
                            preferredSize = new Size(0, Math.Min(img.Height, Decimal.ToInt32((decimal)img.Height * imgWidthAllowed / img.Width)));
                        }
                    }
                    else
                    {
                        int icoWidthAllowed = constraintSize.Width - borderAndPaddingWidths;
                        if (icoWidthAllowed <= 0 || ico.Width == 0)
                        {
                            preferredSize = Size.Empty;
                        }
                        else
                        {
                            preferredSize = new Size(0, Math.Min(ico.Height, Decimal.ToInt32((decimal)ico.Height * icoWidthAllowed / ico.Width)));
                        }
                    }
                }
                else
                {
                    preferredSize = new Size(0, 1);
                }
            }
            else if (freeDimension == DataGridViewFreeDimension.Width &&
                     this.ImageLayout == DataGridViewImageCellLayout.Zoom)
            {
                if (img != null || ico != null)
                {
                    if (img != null)
                    {
                        int imgHeightAllowed = constraintSize.Height - borderAndPaddingHeights;
                        if (imgHeightAllowed <= 0 || img.Height == 0)
                        {
                            preferredSize = Size.Empty;
                        }
                        else
                        {
                            preferredSize = new Size(Math.Min(img.Width, Decimal.ToInt32((decimal)img.Width * imgHeightAllowed / img.Height)), 0);
                        }
                    }
                    else
                    {
                        int icoHeightAllowed = constraintSize.Height - borderAndPaddingHeights;
                        if (icoHeightAllowed <= 0 || ico.Height == 0)
                        {
                            preferredSize = Size.Empty;
                        }
                        else
                        {
                            preferredSize = new Size(Math.Min(ico.Width, Decimal.ToInt32((decimal)ico.Width * icoHeightAllowed / ico.Height)), 0);
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
                if (img != null)
                {
                    preferredSize = new Size(img.Width, img.Height);
                }
                else if (ico != null)
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
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += borderAndPaddingHeights;
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            object valueBase = base.GetValue(rowIndex);
            if (valueBase == null)
            {
                DataGridViewImageColumn owningImageColumn = this.OwningColumn as DataGridViewImageColumn;
                if (owningImageColumn != null)
                {
                    if (defaultTypeImage.IsAssignableFrom(this.ValueType))
                    {
                        Image image = owningImageColumn.Image;
                        if (image != null)
                        {
                            return image;
                        }
                    }
                    else if (defaultTypeIcon.IsAssignableFrom(this.ValueType))
                    {
                        Icon icon = owningImageColumn.Icon;
                        if (icon != null)
                        {
                            return icon;
                        }
                    }
                }
            }
            return valueBase;
        }

        private Rectangle ImgBounds(Rectangle bounds, int imgWidth, int imgHeight, DataGridViewImageCellLayout imageLayout, DataGridViewCellStyle cellStyle)
        {
            // when the imageLayout == stretch there is nothing to do
            Debug.Assert(imageLayout != DataGridViewImageCellLayout.Stretch);

            Rectangle imgBounds = Rectangle.Empty;

            switch (imageLayout)
            {
                case DataGridViewImageCellLayout.Normal:
                case DataGridViewImageCellLayout.NotSet:
                    imgBounds = new Rectangle(bounds.X, bounds.Y, imgWidth, imgHeight);
                    break;
                case DataGridViewImageCellLayout.Zoom:
                    // we have to determine which side will be fully filled: the height or the width
                    if (imgWidth * bounds.Height < imgHeight * bounds.Width)
                    {
                        // we fill the height
                        imgBounds = new Rectangle(bounds.X, bounds.Y, Decimal.ToInt32((decimal)imgWidth * bounds.Height / imgHeight), bounds.Height);
                    }
                    else
                    {
                        // we fill the width
                        imgBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, Decimal.ToInt32((decimal)imgHeight * bounds.Width / imgWidth));
                    }
                    break;
                default:
                    break;
            }

            // now use the alignment on the cellStyle to determine the final bounds
            if (this.DataGridView.RightToLeftInternal)
            {
                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopRight:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.TopLeft:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                    case DataGridViewContentAlignment.MiddleRight:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.MiddleLeft:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                    case DataGridViewContentAlignment.BottomRight:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.BottomLeft:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                }
            }
            else
            {
                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.TopRight:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                    case DataGridViewContentAlignment.MiddleLeft:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.MiddleRight:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                    case DataGridViewContentAlignment.BottomLeft:
                        imgBounds.X = bounds.X;
                        break;
                    case DataGridViewContentAlignment.BottomRight:
                        imgBounds.X = bounds.Right - imgBounds.Width;
                        break;
                }
            }

            switch (cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    imgBounds.X = bounds.X + (bounds.Width - imgBounds.Width) / 2;
                    break;
            }

            switch (cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.TopRight:
                    imgBounds.Y = bounds.Y;
                    break;

                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.MiddleRight:
                    imgBounds.Y = bounds.Y + (bounds.Height - imgBounds.Height) / 2;
                    break;

                case DataGridViewContentAlignment.BottomLeft:
                case DataGridViewContentAlignment.BottomCenter:
                case DataGridViewContentAlignment.BottomRight:
                    imgBounds.Y = bounds.Bottom - imgBounds.Height;
                    break;

                default:
                    Debug.Assert(cellStyle.Alignment == DataGridViewContentAlignment.NotSet, "this is the only alignment left");
                    break;
            }
            return imgBounds;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.Paint"]/*' />
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
                true /*paint*/);
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

            if (paint && DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle resultBounds;
            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            if (valBounds.Width > 0 && valBounds.Height > 0 && (paint || computeContentBounds))
            {
                Rectangle imgBounds = valBounds;
                if (cellStyle.Padding != Padding.Empty)
                {
                    if (this.DataGridView.RightToLeftInternal)
                    {
                        imgBounds.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
                    }
                    else
                    {
                        imgBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                    }
                    imgBounds.Width -= cellStyle.Padding.Horizontal;
                    imgBounds.Height -= cellStyle.Padding.Vertical;
                }

                bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
                SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);

                if (imgBounds.Width > 0 && imgBounds.Height > 0)
                {
                    Image img = formattedValue as Image;
                    Icon ico = null;
                    if (img == null)
                    {
                        ico = formattedValue as Icon;
                    }
                    if (ico != null || img != null)
                    {
                        DataGridViewImageCellLayout imageLayout = this.ImageLayout;
                        if (imageLayout == DataGridViewImageCellLayout.NotSet)
                        {
                            if (this.OwningColumn is DataGridViewImageColumn)
                            {
                                imageLayout = ((DataGridViewImageColumn) this.OwningColumn).ImageLayout;
                                Debug.Assert(imageLayout != DataGridViewImageCellLayout.NotSet);
                            }
                            else
                            {
                                imageLayout = DataGridViewImageCellLayout.Normal;
                            }
                        }

                        if (imageLayout == DataGridViewImageCellLayout.Stretch)
                        {
                            if (paint)
                            {
                                if (DataGridViewCell.PaintBackground(paintParts))
                                {
                                    DataGridViewCell.PaintPadding(g, valBounds, cellStyle, br, this.DataGridView.RightToLeftInternal);
                                }
                                if (DataGridViewCell.PaintContentForeground(paintParts))
                                {
                                    if (img != null)
                                    {
                                        // 

                                        ImageAttributes attr = new ImageAttributes();

                                        attr.SetWrapMode(WrapMode.TileFlipXY);
                                        g.DrawImage(img, imgBounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
                                        attr.Dispose();
                                    }
                                    else
                                    {
                                        g.DrawIcon(ico, imgBounds);
                                    }
                                }
                            }

                            resultBounds = imgBounds;
                        }
                        else
                        {
                            Rectangle imgBounds2 = ImgBounds(imgBounds, (img == null) ? ico.Width : img.Width, (img == null) ? ico.Height : img.Height, imageLayout, cellStyle);
                            resultBounds = imgBounds2;

                            if (paint)
                            {
                                if (DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                                {
                                    g.FillRectangle(br, valBounds);
                                }
                                if (DataGridViewCell.PaintContentForeground(paintParts))
                                {
                                    //paint the image
                                    Region reg = g.Clip;
                                    g.SetClip(Rectangle.Intersect(Rectangle.Intersect(imgBounds2, imgBounds), Rectangle.Truncate(g.VisibleClipBounds)));
                                    if (img != null)
                                    {
                                        g.DrawImage(img, imgBounds2);
                                    }
                                    else
                                    {
                                        g.DrawIconUnstretched(ico, imgBounds2);
                                    }
                                    g.Clip = reg;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                        {
                            g.FillRectangle(br, valBounds);
                        }
                        resultBounds = Rectangle.Empty;
                    }
                }
                else
                {
                    if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                    {
                        g.FillRectangle(br, valBounds);
                    }
                    resultBounds = Rectangle.Empty;
                }

                Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
                if (paint && 
                    DataGridViewCell.PaintFocus(paintParts) && 
                    ptCurrentCell.X == this.ColumnIndex && 
                    ptCurrentCell.Y == rowIndex &&
                    this.DataGridView.ShowFocusCues && 
                    this.DataGridView.Focused)
                {
                    // Draw focus rectangle
                    ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, br.Color);
                }

                if (this.DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
                {
                    PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, valBounds, errorText);
                }
            }
            else if (computeErrorIconBounds)
            {
                if (!String.IsNullOrEmpty(errorText))
                {
                    resultBounds = ComputeErrorIconBounds(valBounds);
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

            return resultBounds;
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCell.ToString"]/*' />
        public override string ToString()
        {
            return "DataGridViewImageCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject"]/*' />
        protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
        {

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.DataGridViewImageCellAccessibleObject"]/*' />
            public DataGridViewImageCellAccessibleObject(DataGridViewCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    return String.Empty;
                }
            }

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.Description"]/*' />
            public override string Description
            {
                get
                {
                    DataGridViewImageCell imageCell = this.Owner as DataGridViewImageCell;
                    if (imageCell != null)
                    {
                        return imageCell.Description;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get 
                {
                    return base.Value;
                }

                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                set
                {
                    // do nothing.
                }
            }

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                // do nothing if Level < 3

                if (AccessibilityImprovements.Level3)
                {
                    DataGridViewImageCell dataGridViewCell = (DataGridViewImageCell)this.Owner;
                    DataGridView dataGridView = dataGridViewCell.DataGridView;

                    if (dataGridView != null && dataGridViewCell.RowIndex != -1 &&
                        dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                    {
                        dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                    }
                }
            }

            /// <include file='doc\DataGridViewImageCell.uex' path='docs/doc[@for="DataGridViewImageCellAccessibleObject.GetChildCount"]/*' />
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
                    return NativeMethods.UIA_ImageControlTypeId;
                }

                if (AccessibilityImprovements.Level3 && propertyID == NativeMethods.UIA_IsInvokePatternAvailablePropertyId)
                {
                    return true;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
