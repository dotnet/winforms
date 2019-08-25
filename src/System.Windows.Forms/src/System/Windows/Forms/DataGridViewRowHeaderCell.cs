// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public class DataGridViewRowHeaderCell : DataGridViewHeaderCell
    {
        private static readonly VisualStyleElement HeaderElement = VisualStyleElement.Header.Item.Normal;

        // ColorMap used to map the black color of the resource bitmaps to the fore color in use in the row header cell
        private static readonly ColorMap[] colorMap = new ColorMap[] { new ColorMap() };

        private static Bitmap rightArrowBmp = null;
        private static Bitmap leftArrowBmp = null;
        private static Bitmap rightArrowStarBmp;
        private static Bitmap leftArrowStarBmp;
        //private static Bitmap errorBmp = null;
        private static Bitmap pencilLTRBmp = null;
        private static Bitmap pencilRTLBmp = null;
        private static Bitmap starBmp = null;

        private static readonly Type cellType = typeof(DataGridViewRowHeaderCell);

        private const byte DATAGRIDVIEWROWHEADERCELL_iconMarginWidth = 3;      // 3 pixels of margin on the left and right of icons
        private const byte DATAGRIDVIEWROWHEADERCELL_iconMarginHeight = 2;     // 2 pixels of margin on the top and bottom of icons
        private const byte DATAGRIDVIEWROWHEADERCELL_contentMarginWidth = 3;   // 3 pixels of margin on the left and right of content
        private const byte DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginLeft = 1;
        private const byte DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWROWHEADERCELL_verticalTextMargin = 1;

        public DataGridViewRowHeaderCell()
        {
        }

        private static Bitmap LeftArrowBitmap
        {
            get
            {
                if (leftArrowBmp == null)
                {
                    leftArrowBmp = GetBitmapFromIcon("DataGridViewRow.left");
                }
                return leftArrowBmp;
            }
        }

        private static Bitmap LeftArrowStarBitmap
        {
            get
            {
                if (leftArrowStarBmp == null)
                {
                    leftArrowStarBmp = GetBitmapFromIcon("DataGridViewRow.leftstar");
                }
                return leftArrowStarBmp;
            }
        }

        private static Bitmap PencilLTRBitmap
        {
            get
            {
                if (pencilLTRBmp == null)
                {
                    pencilLTRBmp = GetBitmapFromIcon("DataGridViewRow.pencil_ltr");
                }
                return pencilLTRBmp;
            }
        }

        private static Bitmap PencilRTLBitmap
        {
            get
            {
                if (pencilRTLBmp == null)
                {
                    pencilRTLBmp = GetBitmapFromIcon("DataGridViewRow.pencil_rtl");
                }
                return pencilRTLBmp;
            }
        }

        private static Bitmap RightArrowBitmap
        {
            get
            {
                if (rightArrowBmp == null)
                {
                    rightArrowBmp = GetBitmapFromIcon("DataGridViewRow.right");
                }
                return rightArrowBmp;
            }
        }

        private static Bitmap RightArrowStarBitmap
        {
            get
            {
                if (rightArrowStarBmp == null)
                {
                    rightArrowStarBmp = GetBitmapFromIcon("DataGridViewRow.rightstar");
                }
                return rightArrowStarBmp;
            }
        }

        private static Bitmap StarBitmap
        {
            get
            {
                if (starBmp == null)
                {
                    starBmp = GetBitmapFromIcon("DataGridViewRow.star");
                }
                return starBmp;
            }
        }

        public override object Clone()
        {
            DataGridViewRowHeaderCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewRowHeaderCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewRowHeaderCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = Value;
            return dataGridViewCell;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewRowHeaderCellAccessibleObject(this);
        }

        private static Bitmap GetArrowBitmap(bool rightToLeft)
        {
            return rightToLeft ? DataGridViewRowHeaderCell.LeftArrowBitmap : DataGridViewRowHeaderCell.RightArrowBitmap;
        }

        private static Bitmap GetArrowStarBitmap(bool rightToLeft)
        {
            return rightToLeft ? DataGridViewRowHeaderCell.LeftArrowStarBitmap : DataGridViewRowHeaderCell.RightArrowStarBitmap;
        }

        private static Bitmap GetBitmapFromIcon(string iconName)
        {
            Size desiredSize = new Size(iconsWidth, iconsHeight);
            Icon icon = new Icon(new Icon(typeof(DataGridViewHeaderCell), iconName), desiredSize);
            Bitmap b = icon.ToBitmap();
            icon.Dispose();

            if (DpiHelper.IsScalingRequired && (b.Size.Width != iconsWidth || b.Size.Height != iconsHeight))
            {
                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, desiredSize);
                if (scaledBitmap != null)
                {
                    b.Dispose();
                    b = scaledBitmap;
                }
            }
            return b;
        }

        protected override object GetClipboardContent(int rowIndex,
                                                      bool firstCell,
                                                      bool lastCell,
                                                      bool inFirstRow,
                                                      bool inLastRow,
                                                      string format)
        {
            if (DataGridView == null)
            {
                return null;
            }
            if (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            // Not using formatted values for header cells.
            object val = GetValue(rowIndex);
            StringBuilder sb = new StringBuilder(64);

            Debug.Assert((!DataGridView.RightToLeftInternal && firstCell) || (DataGridView.RightToLeftInternal && lastCell));

            if (string.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
            {
                if (inFirstRow)
                {
                    sb.Append("<TABLE>");
                }
                sb.Append("<TR>");
                sb.Append("<TD ALIGN=\"center\">");
                if (val != null)
                {
                    sb.Append("<B>");
                    FormatPlainTextAsHtml(val.ToString(), new StringWriter(sb, CultureInfo.CurrentCulture));
                    sb.Append("</B>");
                }
                else
                {
                    sb.Append("&nbsp;");
                }
                sb.Append("</TD>");
                if (lastCell)
                {
                    sb.Append("</TR>");
                    if (inLastRow)
                    {
                        sb.Append("</TABLE>");
                    }
                }
                return sb.ToString();
            }
            else
            {
                bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
                if (csv ||
                    string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
                {
                    if (val != null)
                    {
                        bool escapeApplied = false;
                        int insertionPoint = sb.Length;
                        FormatPlainText(val.ToString(), csv, new StringWriter(sb, CultureInfo.CurrentCulture), ref escapeApplied);
                        if (escapeApplied)
                        {
                            Debug.Assert(csv);
                            sb.Insert(insertionPoint, '"');
                        }
                    }
                    if (lastCell)
                    {
                        if (!inLastRow)
                        {
                            sb.Append((char)Keys.Return);
                            sb.Append((char)Keys.LineFeed);
                        }
                    }
                    else
                    {
                        sb.Append(csv ? ',' : (char)Keys.Tab);
                    }
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null || OwningRow == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            // the content bounds are computed on demand
            // we mimic a lot of the painting code

            // get the borders

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                null /*errorText*/,                 // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            Rectangle contentBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

            return contentBounds;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null ||
                rowIndex < 0 ||
                !DataGridView.ShowRowErrors ||
                string.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            Rectangle errorBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue /*formattedValue*/,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true /*computeErrorIconBounds*/,
                false /*paint*/);

            return errorBounds;
        }

        protected internal override string GetErrorText(int rowIndex)
        {
            if (OwningRow == null)
            {
                return base.GetErrorText(rowIndex);
            }
            else
            {
                return OwningRow.GetErrorText(rowIndex);
            }
        }

        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (DataGridView != null && (rowIndex < 0 || rowIndex >= DataGridView.Rows.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }

            if (DataGridView != null)
            {
                return DataGridView.ContextMenuStrip;
            }
            else
            {
                return null;
            }
        }

        public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
        {
            Debug.Assert(DataGridView != null);

            DataGridViewCellStyle inheritedCellStyleTmp = inheritedCellStyle ?? new DataGridViewCellStyle();

            DataGridViewCellStyle cellStyle = null;
            if (HasStyle)
            {
                cellStyle = Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowHeadersStyle = DataGridView.RowHeadersDefaultCellStyle;
            Debug.Assert(rowHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            if (includeColors)
            {
                if (cellStyle != null && !cellStyle.BackColor.IsEmpty)
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

                if (cellStyle != null && !cellStyle.ForeColor.IsEmpty)
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

                if (cellStyle != null && !cellStyle.SelectionBackColor.IsEmpty)
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

                if (cellStyle != null && !cellStyle.SelectionForeColor.IsEmpty)
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

            if (cellStyle != null && cellStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = cellStyle.Font;
            }
            else if (rowHeadersStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = rowHeadersStyle.Font;
            }
            else
            {
                inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
            }

            if (cellStyle != null && !cellStyle.IsNullValueDefault)
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

            if (cellStyle != null && !cellStyle.IsDataSourceNullValueDefault)
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

            if (cellStyle != null && cellStyle.Format.Length != 0)
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

            if (cellStyle != null && !cellStyle.IsFormatProviderDefault)
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

            if (cellStyle != null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
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

            if (cellStyle != null && cellStyle.WrapMode != DataGridViewTriState.NotSet)
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

            if (cellStyle != null && cellStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = cellStyle.Tag;
            }
            else if (rowHeadersStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = rowHeadersStyle.Tag;
            }
            else
            {
                inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
            }

            if (cellStyle != null && cellStyle.Padding != Padding.Empty)
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

        private static Bitmap GetPencilBitmap(bool rightToLeft)
        {
            return rightToLeft ? DataGridViewRowHeaderCell.PencilRTLBitmap : DataGridViewRowHeaderCell.PencilLTRBitmap;
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

            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = OwningRow.AdjustRowHeaderBorderStyle(DataGridView.AdvancedRowHeadersBorderStyle,
                dgvabsPlaceholder,
                false /*singleVerticalBorderAdded*/,
                false /*singleHorizontalBorderAdded*/,
                false /*isFirstDisplayedRow*/,
                false /*isLastVisibleRow*/);
            Rectangle borderWidthsRect = BorderWidths(dgvabsEffective);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;

            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            if (DataGridView.ApplyVisualStylesToHeaderCells)
            {
                // Add the theming margins to the borders.
                Rectangle rectThemeMargins = DataGridViewHeaderCell.GetThemeMargins(graphics);
                borderAndPaddingWidths += rectThemeMargins.Y;
                borderAndPaddingWidths += rectThemeMargins.Height;
                borderAndPaddingHeights += rectThemeMargins.X;
                borderAndPaddingHeights += rectThemeMargins.Width;
            }

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            object val = GetValue(rowIndex);
            if (!(val is string))
            {
                val = null;
            }
            return DataGridViewUtilities.GetPreferredRowHeaderSize(graphics,
                                                                   (string)val,
                                                                   cellStyle,
                                                                   borderAndPaddingWidths,
                                                                   borderAndPaddingHeights,
                                                                   DataGridView.ShowRowErrors,
                                                                   true /*showGlyph*/,
                                                                   constraintSize,
                                                                   flags);
        }

        protected override object GetValue(int rowIndex)
        {
            // We allow multiple rows to share the same row header value. The row header cell's cloning does this.
            // So here we need to allow rowIndex == -1.
            if (DataGridView != null && (rowIndex < -1 || rowIndex >= DataGridView.Rows.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            return Properties.GetObject(PropCellValue);
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
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
                cellState,
                formattedValue,
                errorText,
                cellStyle,
                advancedBorderStyle,
                paintParts,
                false /*computeContentBounds*/
                                              ,
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
        private Rectangle PaintPrivate(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates dataGridViewElementState,
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

            // If computeContentBounds == TRUE then resultBounds will be the contentBounds.
            // If computeErrorIconBounds == TRUE then resultBounds will be the error icon bounds.
            // Else resultBounds will be Rectangle.Empty;
            Rectangle resultBounds = Rectangle.Empty;

            if (paint && DataGridViewCell.PaintBorder(paintParts))
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
                    if (paint && DataGridViewCell.PaintBackground(paintParts))
                    {
                        // Theming
                        int state = (int)HeaderItemState.Normal;
                        if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                            DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
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
                        using (Bitmap bmFlipXPThemes = new Bitmap(backgroundBounds.Height, backgroundBounds.Width))
                        {
                            using (Graphics gFlip = Graphics.FromImage(bmFlipXPThemes))
                            {
                                DataGridViewRowHeaderCellRenderer.DrawHeader(gFlip, new Rectangle(0, 0, backgroundBounds.Height, backgroundBounds.Width), state);
                                bmFlipXPThemes.RotateFlip(DataGridView.RightToLeftInternal ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipY);

                                graphics.DrawImage(bmFlipXPThemes,
                                                   backgroundBounds,
                                                   new Rectangle(0, 0, backgroundBounds.Width, backgroundBounds.Height),
                                                   GraphicsUnit.Pixel);
                            }
                        }
                    }
                    // update the val bounds
                    Rectangle rectThemeMargins = DataGridViewHeaderCell.GetThemeMargins(graphics);
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
                    SolidBrush br = DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                    if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                    {
                        graphics.FillRectangle(br, valBounds);
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

            Bitmap bmp = null;

            if (valBounds.Width > 0 && valBounds.Height > 0)
            {
                Rectangle errorBounds = valBounds;
                string formattedString = formattedValue as string;
                if (!string.IsNullOrEmpty(formattedString))
                {
                    // There is text to display
                    if (valBounds.Width >= iconsWidth +
                                           2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth &&
                        valBounds.Height >= iconsHeight +
                                            2 * DATAGRIDVIEWROWHEADERCELL_iconMarginHeight)
                    {
                        if (paint && DataGridViewCell.PaintContentBackground(paintParts))
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
                                bmp = DataGridViewRowHeaderCell.StarBitmap;
                            }
                            if (bmp != null)
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
                                    PaintIcon(graphics, bmp, valBounds, iconColor);
                                }
                            }
                        }
                        if (!DataGridView.RightToLeftInternal)
                        {
                            valBounds.X += iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth;
                        }
                        valBounds.Width -= iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth;
                        Debug.Assert(valBounds.Width >= 0);
                        Debug.Assert(valBounds.Height >= 0);
                    }
                    // Second priority is text
                    // Font independent margins
                    valBounds.Offset(DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginLeft + DATAGRIDVIEWROWHEADERCELL_contentMarginWidth, DATAGRIDVIEWROWHEADERCELL_verticalTextMargin);
                    valBounds.Width -= DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginLeft + 2 * DATAGRIDVIEWROWHEADERCELL_contentMarginWidth + DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginRight;
                    valBounds.Height -= 2 * DATAGRIDVIEWROWHEADERCELL_verticalTextMargin;
                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {
                        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                        if (DataGridView.ShowRowErrors && valBounds.Width > iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth)
                        {
                            // Check if the text fits if we remove the room required for the row error icon
                            Size maxBounds = new Size(valBounds.Width - iconsWidth - 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth, valBounds.Height);
                            if (DataGridViewCell.TextFitsInBounds(graphics,
                                                                  formattedString,
                                                                  cellStyle.Font,
                                                                  maxBounds,
                                                                  flags))
                            {
                                // There is enough room for both the text and the row error icon, so use it all.
                                if (DataGridView.RightToLeftInternal)
                                {
                                    valBounds.X += iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth;
                                }
                                valBounds.Width -= iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth;
                            }
                        }

                        if (DataGridViewCell.PaintContentForeground(paintParts))
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
                                TextRenderer.DrawText(graphics,
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
                    if (errorBounds.Width >= 3 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth +
                                             2 * iconsWidth)
                    {
                        // There is enough horizontal room for the error icon and the glyph
                        if (paint && DataGridView.ShowRowErrors && DataGridViewCell.PaintErrorIcon(paintParts))
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
                    if (valBounds.Width >= iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth &&
                        valBounds.Height >= iconsHeight + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginHeight)
                    {
                        if (paint && DataGridViewCell.PaintContentBackground(paintParts))
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
                                bmp = DataGridViewRowHeaderCell.StarBitmap;
                            }
                            if (bmp != null)
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
                                    PaintIcon(graphics, bmp, valBounds, iconColor);
                                }
                            }
                        }
                    }

                    if (errorBounds.Width >= 3 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth +
                                             2 * iconsWidth)
                    {
                        // There is enough horizontal room for the error icon
                        if (paint && DataGridView.ShowRowErrors && DataGridViewCell.PaintErrorIcon(paintParts))
                        {
                            PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
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

        private void PaintIcon(Graphics g, Bitmap bmp, Rectangle bounds, Color foreColor)
        {
            Rectangle bmpRect = new Rectangle(DataGridView.RightToLeftInternal ?
                                              bounds.Right - DATAGRIDVIEWROWHEADERCELL_iconMarginWidth - iconsWidth :
                                              bounds.Left + DATAGRIDVIEWROWHEADERCELL_iconMarginWidth,
                                              bounds.Y + (bounds.Height - iconsHeight) / 2,
                                              iconsWidth,
                                              iconsHeight);
            colorMap[0].NewColor = foreColor;
            colorMap[0].OldColor = Color.Black;

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
            g.DrawImage(bmp, bmpRect, 0, 0, iconsWidth, iconsHeight, GraphicsUnit.Pixel, attr);
            attr.Dispose();
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            object originalValue = GetValue(rowIndex);
            if (value != null || Properties.ContainsObject(PropCellValue))
            {
                Properties.SetObject(PropCellValue, value);
            }
            if (DataGridView != null && originalValue != value)
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(-1, rowIndex));
            }
            return true;
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewRowHeaderCell { RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private class DataGridViewRowHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewRowHeaderCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer == null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(HeaderElement);
                    }

                    return visualStyleRenderer;
                }
            }

            public static void DrawHeader(Graphics g, Rectangle bounds, int headerState)
            {
                VisualStyleRenderer.SetParameters(HeaderElement.ClassName, HeaderElement.Part, headerState);
                VisualStyleRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }

        protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell owner) : base(owner)
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (Owner.OwningRow == null)
                    {
                        return Rectangle.Empty;
                    }

                    // use the parent row acc obj bounds
                    Rectangle rowRect = ParentPrivate.Bounds;
                    Rectangle cellRect = rowRect;
                    cellRect.Width = Owner.DataGridView.RowHeadersWidth;
                    if (Owner.DataGridView.RightToLeft == RightToLeft.Yes)
                    {
                        cellRect.X = rowRect.Right - cellRect.Width;
                    }

                    return cellRect;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        return SR.DataGridView_RowHeaderCellAccDefaultAction;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override string Name
            {
                get
                {
                    if (ParentPrivate != null)
                    {
                        return ParentPrivate.Name;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return ParentPrivate;
                }
            }

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (Owner.OwningRow == null)
                    {
                        return null;
                    }
                    else
                    {
                        return Owner.OwningRow.AccessibilityObject;
                    }
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.RowHeader;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates resultState = AccessibleStates.Selectable;

                    // get the Offscreen state from the base method.
                    AccessibleStates state = base.State;
                    if ((state & AccessibleStates.Offscreen) == AccessibleStates.Offscreen)
                    {
                        resultState |= AccessibleStates.Offscreen;
                    }

                    if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        if (Owner.OwningRow != null && Owner.OwningRow.Selected)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            public override string Value
            {
                get
                {
                    return string.Empty;
                }
            }

            public override void DoDefaultAction()
            {
                if ((Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                    Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect) &&
                    Owner.OwningRow != null)
                {
                    Owner.OwningRow.Selected = true;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                Debug.Assert(Owner.DataGridView.RowHeadersVisible, "if the rows are not visible how did you get the row headers acc obj?");
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Next:
                        if (Owner.OwningRow != null && Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return ParentPrivate.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.Down:
                        if (Owner.OwningRow == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                            {
                                return null;
                            }
                            else
                            {
                                int nextVisibleRow = Owner.DataGridView.Rows.GetNextRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);

                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return Owner.DataGridView.AccessibilityObject.GetChild(1 + actualDisplayIndex).GetChild(0);
                                }
                                else
                                {
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    case AccessibleNavigation.Previous:
                        return null;
                    case AccessibleNavigation.Up:
                        if (Owner.OwningRow == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                            {
                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // Return the top left header cell accessible object.
                                    Debug.Assert(Owner.DataGridView.TopLeftHeaderCell.AccessibilityObject == Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0));
                                    return Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                int previousVisibleRow = Owner.DataGridView.Rows.GetPreviousRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1).GetChild(0);
                                }
                                else
                                {
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    default:
                        return null;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (Owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                DataGridViewRowHeaderCell dataGridViewCell = (DataGridViewRowHeaderCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView == null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }
                if (dataGridViewCell.OwningRow != null &&
                    (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                     dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
                {
                    if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.AddSelection)) != 0)
                    {
                        dataGridViewCell.OwningRow.Selected = true;
                    }
                    else if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection)
                    {
                        dataGridViewCell.OwningRow.Selected = false;
                    }
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                if (Owner.OwningRow == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return Owner.OwningRow.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        if (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return Owner.OwningRow.AccessibilityObject.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                    default:
                        return null;
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object GetPropertyValue(int propertyId)
            {
                switch (propertyId)
                {
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_HeaderControlTypeId;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return Owner.DataGridView.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
