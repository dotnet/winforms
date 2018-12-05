// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms.VisualStyles;
    using System.ComponentModel;
    using System.Windows.Forms.Internal;
    using System.Security.Permissions;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell"]/*' />
    /// <devdoc>
    ///    <para></para>
    /// </devdoc>
    public class DataGridViewRowHeaderCell : DataGridViewHeaderCell
    {
        private static readonly VisualStyleElement HeaderElement = VisualStyleElement.Header.Item.Normal;

        // ColorMap used to map the black color of the resource bitmaps to the fore color in use in the row header cell
        private static ColorMap[] colorMap = new ColorMap[] {new ColorMap()};

        private static Bitmap rightArrowBmp = null;
        private static Bitmap leftArrowBmp = null;
        private static Bitmap rightArrowStarBmp;
        private static Bitmap leftArrowStarBmp;
        //private static Bitmap errorBmp = null;
        private static Bitmap pencilLTRBmp = null;
        private static Bitmap pencilRTLBmp = null;
        private static Bitmap starBmp = null;

        private static Type cellType = typeof(DataGridViewRowHeaderCell);

        private const byte DATAGRIDVIEWROWHEADERCELL_iconMarginWidth = 3;      // 3 pixels of margin on the left and right of icons
        private const byte DATAGRIDVIEWROWHEADERCELL_iconMarginHeight = 2;     // 2 pixels of margin on the top and bottom of icons
        private const byte DATAGRIDVIEWROWHEADERCELL_contentMarginWidth = 3;   // 3 pixels of margin on the left and right of content
        private const byte DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginLeft = 1;
        private const byte DATAGRIDVIEWROWHEADERCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWROWHEADERCELL_verticalTextMargin = 1;
 
        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.DataGridViewRowHeaderCell"]/*' />
        public DataGridViewRowHeaderCell()
        {
        }

        /* Unused for now.
        private static Bitmap ErrorBitmap
        {
            get
            {
                if (errorBmp == null)
                {
                    errorBmp = GetBitmap("DataGridViewRow.error.bmp");
                }
                return errorBmp;
            }
        }
        */

        private static Bitmap LeftArrowBitmap
        {
            get
            {
                if (leftArrowBmp == null)
                {
                    leftArrowBmp = GetBitmapFromIcon("DataGridViewRow.left.ico");
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
                    leftArrowStarBmp = GetBitmapFromIcon("DataGridViewRow.leftstar.ico");
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
                    pencilLTRBmp = GetBitmapFromIcon("DataGridViewRow.pencil_ltr.ico");
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
                    pencilRTLBmp = GetBitmapFromIcon("DataGridViewRow.pencil_rtl.ico");
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
                    rightArrowBmp = GetBitmapFromIcon("DataGridViewRow.right.ico");
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
                    rightArrowStarBmp = GetBitmapFromIcon("DataGridViewRow.rightstar.ico");
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
                    starBmp = GetBitmapFromIcon("DataGridViewRow.star.ico");
                }
                return starBmp;
            }
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewRowHeaderCell dataGridViewCell;
            Type thisType = this.GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewRowHeaderCell();
            }
            else
            {
                // 

                dataGridViewCell = (DataGridViewRowHeaderCell) System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = this.Value;
            return dataGridViewCell;
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.CreateAccessibilityInstance"]/*' />
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

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private static Bitmap GetBitmapFromIcon(string iconName)
        {
            Size desiredSize = new Size(iconsWidth, iconsHeight);
            Icon icon = new Icon(BitmapSelector.GetResourceStream(typeof(DataGridViewRowHeaderCell), iconName), desiredSize);
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

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetClipboardContent"]/*' />
        protected override object GetClipboardContent(int rowIndex,
                                                      bool firstCell,
                                                      bool lastCell,
                                                      bool inFirstRow,
                                                      bool inLastRow,
                                                      string format)
        {
            if (this.DataGridView == null)
            {
                return null;
            }
            if (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            // Not using formatted values for header cells.
            object val = GetValue(rowIndex);
            StringBuilder sb = new StringBuilder(64);

            Debug.Assert((!this.DataGridView.RightToLeftInternal && firstCell) || (this.DataGridView.RightToLeftInternal && lastCell));

            if (String.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
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
                bool csv = String.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
                if (csv ||
                    String.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
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
                            sb.Append((char) Keys.Return);
                            sb.Append((char) Keys.LineFeed);
                        }
                    }
                    else
                    {
                        sb.Append(csv ? ',' : (char) Keys.Tab);
                    }
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetContentBounds"]/*' />
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null || this.OwningRow == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            // the content bounds are computed on demand
            // we mimic a lot of the painting code

            // get the borders
            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetErrorIconBounds"]/*' />
        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (this.DataGridView == null ||
                rowIndex < 0 ||
                !this.DataGridView.ShowRowErrors ||
                String.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetErrorText"]/*' />
        protected internal override string GetErrorText(int rowIndex)
        {
            if (this.OwningRow == null)
            {
                return base.GetErrorText(rowIndex);
            }
            else
            {
                return this.OwningRow.GetErrorText(rowIndex);
            }
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetInheritedContextMenuStrip"]/*' />
        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (this.DataGridView != null && (rowIndex < 0 || rowIndex >= this.DataGridView.Rows.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(rowIndex);
            if (contextMenuStrip != null)
            {
                return contextMenuStrip;
            }

            if (this.DataGridView != null)
            {
                return this.DataGridView.ContextMenuStrip;
            }
            else
            {
                return null;
            }
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetInheritedStyle"]/*' />
        public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
        {
            Debug.Assert(this.DataGridView != null);

            DataGridViewCellStyle inheritedCellStyleTmp = (inheritedCellStyle == null) ? new DataGridViewCellStyle() : inheritedCellStyle;

            DataGridViewCellStyle cellStyle = null;
            if (this.HasStyle)
            {
                cellStyle = this.Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle rowHeadersStyle = this.DataGridView.RowHeadersDefaultCellStyle;
            Debug.Assert(rowHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
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

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetPreferredSize"]/*' />
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

            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = this.OwningRow.AdjustRowHeaderBorderStyle(this.DataGridView.AdvancedRowHeadersBorderStyle,
                dgvabsPlaceholder,
                false /*singleVerticalBorderAdded*/,
                false /*singleHorizontalBorderAdded*/,
                false /*isFirstDisplayedRow*/,
                false /*isLastVisibleRow*/);
            Rectangle borderWidthsRect = BorderWidths(dgvabsEffective);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;

            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                // Add the XP theming margins to the borders.
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
                                                                   (string) val, 
                                                                   cellStyle, 
                                                                   borderAndPaddingWidths, 
                                                                   borderAndPaddingHeights,
                                                                   this.DataGridView.ShowRowErrors,
                                                                   true /*showGlyph*/,
                                                                   constraintSize, 
                                                                   flags);
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            // We allow multiple rows to share the same row header value. The row header cell's cloning does this.
            // So here we need to allow rowIndex == -1.
            if (this.DataGridView != null && (rowIndex < -1 || rowIndex >= this.DataGridView.Rows.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            return this.Properties.GetObject(PropCellValue);
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.Paint"]/*' />
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

            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
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

                if (backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
                {
                    if (paint && DataGridViewCell.PaintBackground(paintParts))
                    {
                        // XP Theming
                        int state = (int)HeaderItemState.Normal;
                        if (this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                            this.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                        {
                            if (this.ButtonState != ButtonState.Normal)
                            {
                                Debug.Assert(this.ButtonState == ButtonState.Pushed);
                                state = (int)HeaderItemState.Pressed;
                            }
                            else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                     this.DataGridView.MouseEnteredCellAddress.X == -1)
                            {
                                state = (int)HeaderItemState.Hot;
                            }
                            else if (cellSelected)
                            {
                                state = (int)HeaderItemState.Pressed;
                            }
                        }
                        // Flip the column header background
                        using (Bitmap bmFlipXPThemes = new Bitmap(backgroundBounds.Height, backgroundBounds.Width)) {
                            using (Graphics gFlip = Graphics.FromImage(bmFlipXPThemes)) {
                            DataGridViewRowHeaderCellRenderer.DrawHeader(gFlip, new Rectangle(0, 0, backgroundBounds.Height, backgroundBounds.Width), state);
                            bmFlipXPThemes.RotateFlip(this.DataGridView.RightToLeftInternal ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipY);

                            graphics.DrawImage(bmFlipXPThemes,
                                               backgroundBounds,
                                               new Rectangle(0, 0, backgroundBounds.Width, backgroundBounds.Height),
                                               GraphicsUnit.Pixel);
                            }
                        }
                    }
                    // update the val bounds
                    Rectangle rectThemeMargins = DataGridViewHeaderCell.GetThemeMargins(graphics);
                    if (this.DataGridView.RightToLeftInternal)
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
                    SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                    if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
                    {
                        graphics.FillRectangle(br, valBounds);
                    }
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
            }

            Bitmap bmp = null;

            if (valBounds.Width > 0 && valBounds.Height > 0)
            {
                Rectangle errorBounds = valBounds;
                string formattedString = formattedValue as string;
                if (!String.IsNullOrEmpty(formattedString))
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
                            if (this.DataGridView.CurrentCellAddress.Y == rowIndex)
                            {
                                if (this.DataGridView.VirtualMode)
                                {
                                    if (this.DataGridView.IsCurrentRowDirty && this.DataGridView.ShowEditingIcon)
                                    {
                                        bmp = GetPencilBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else if (this.DataGridView.NewRowIndex == rowIndex)
                                    {
                                        bmp = GetArrowStarBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else
                                    {
                                        bmp = GetArrowBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                }
                                else
                                {
                                    if (this.DataGridView.IsCurrentCellDirty && this.DataGridView.ShowEditingIcon)
                                    {
                                        bmp = GetPencilBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else if (this.DataGridView.NewRowIndex == rowIndex)
                                    {
                                        bmp = GetArrowStarBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else
                                    {
                                        bmp = GetArrowBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                }
                            }
                            else if (this.DataGridView.NewRowIndex == rowIndex)
                            {
                                bmp = DataGridViewRowHeaderCell.StarBitmap;
                            }
                            if (bmp != null)
                            {
                                Color iconColor;
                                if (this.DataGridView.ApplyVisualStylesToHeaderCells)
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
                        if (!this.DataGridView.RightToLeftInternal)
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
                        TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                        if (this.DataGridView.ShowRowErrors && valBounds.Width > iconsWidth + 2 * DATAGRIDVIEWROWHEADERCELL_iconMarginWidth)
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
                                if (this.DataGridView.RightToLeftInternal)
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
                                if (this.DataGridView.ApplyVisualStylesToHeaderCells)
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
                        if (paint && this.DataGridView.ShowRowErrors && DataGridViewCell.PaintErrorIcon(paintParts))
                        {
                            PaintErrorIcon(graphics, clipBounds, errorBounds, errorText);
                        }
                        else if (computeErrorIconBounds)
                        {
                            if (!String.IsNullOrEmpty(errorText))
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
                            if (this.DataGridView.CurrentCellAddress.Y == rowIndex)
                            {
                                if (this.DataGridView.VirtualMode)
                                {
                                    if (this.DataGridView.IsCurrentRowDirty && this.DataGridView.ShowEditingIcon)
                                    {
                                        bmp = GetPencilBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else if (this.DataGridView.NewRowIndex == rowIndex)
                                    {
                                        bmp = GetArrowStarBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else
                                    {
                                        bmp = GetArrowBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                }
                                else
                                {
                                    if (this.DataGridView.IsCurrentCellDirty && this.DataGridView.ShowEditingIcon)
                                    {
                                        bmp = GetPencilBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else if (this.DataGridView.NewRowIndex == rowIndex)
                                    {
                                        bmp = GetArrowStarBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                    else
                                    {
                                        bmp = GetArrowBitmap(this.DataGridView.RightToLeftInternal);
                                    }
                                }
                            }
                            else if (this.DataGridView.NewRowIndex == rowIndex)
                            {
                                bmp = DataGridViewRowHeaderCell.StarBitmap;
                            }
                            if (bmp != null)
                            {
                                lock (bmp)
                                {
                                    Color iconColor;
                                    if (this.DataGridView.ApplyVisualStylesToHeaderCells)
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
                        if (paint && this.DataGridView.ShowRowErrors && DataGridViewCell.PaintErrorIcon(paintParts))
                        {
                            PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
                        }
                        else if (computeErrorIconBounds)
                        {
                            if (!String.IsNullOrEmpty(errorText))
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
            Rectangle bmpRect = new Rectangle(this.DataGridView.RightToLeftInternal ? 
                                              bounds.Right - DATAGRIDVIEWROWHEADERCELL_iconMarginWidth - iconsWidth :
                                              bounds.Left + DATAGRIDVIEWROWHEADERCELL_iconMarginWidth,
                                              bounds.Y + (bounds.Height - iconsHeight)/2,
                                              iconsWidth,
                                              iconsHeight);
            colorMap[0].NewColor = foreColor;
            colorMap[0].OldColor = Color.Black;

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
            g.DrawImage(bmp, bmpRect, 0, 0, iconsWidth, iconsHeight, GraphicsUnit.Pixel, attr);
            attr.Dispose();
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.SetValue"]/*' />
        protected override bool SetValue(int rowIndex, object value)
        {
            object originalValue = GetValue(rowIndex);
            if (value != null || this.Properties.ContainsObject(PropCellValue)) 
            {
                this.Properties.SetObject(PropCellValue, value);
            }
            if (this.DataGridView != null && originalValue != value)
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(-1, rowIndex));
            }
            return true;
        }

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCell.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
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

        /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject"]/*' />
        protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.DataGridViewRowHeaderCellAccessibleObject"]/*' />
            public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell owner) : base(owner)
            {
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds
            {
                get
                {
                    if (this.Owner.OwningRow == null)
                    {
                        return Rectangle.Empty;
                    }

                    // use the parent row acc obj bounds
                    Rectangle rowRect = this.ParentPrivate.Bounds;
                    rowRect.Width = this.Owner.DataGridView.RowHeadersWidth;
                    return rowRect;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    if (this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        return string.Format(SR.DataGridView_RowHeaderCellAccDefaultAction);
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Name"]/*' />
            public override string Name
            {
                get
                {
                    if (this.ParentPrivate != null)
                    {
                        return this.ParentPrivate.Name;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Parent"]/*' />
            public override AccessibleObject Parent
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.ParentPrivate;
                }
            }

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (this.Owner.OwningRow == null)
                    {
                        return null;
                    }
                    else
                    {
                        return this.Owner.OwningRow.AccessibilityObject;
                    }
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Role"]/*' />
            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.RowHeader;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.State"]/*' />
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

                    if (this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        if (this.Owner.OwningRow != null && this.Owner.OwningRow.Selected)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return String.Empty;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                if ((this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                    this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect ) &&
                    this.Owner.OwningRow != null)
                {
                    this.Owner.OwningRow.Selected = true;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                Debug.Assert(this.Owner.DataGridView.RowHeadersVisible, "if the rows are not visible how did you get the row headers acc obj?");
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Next:
                        if (this.Owner.OwningRow != null && this.Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return this.ParentPrivate.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.Down:
                        if (this.Owner.OwningRow == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (this.Owner.OwningRow.Index == this.Owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                            {
                                return null;
                            }
                            else
                            {
                                int nextVisibleRow = this.Owner.DataGridView.Rows.GetNextRow(this.Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = this.Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);

                                if (this.Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return this.Owner.DataGridView.AccessibilityObject.GetChild(1 + actualDisplayIndex).GetChild(0);
                                }
                                else
                                {
                                    return this.Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    case AccessibleNavigation.Previous:
                        return null;
                    case AccessibleNavigation.Up:
                        if (this.Owner.OwningRow == null)
                        {
                            return null;
                        }
                        else
                        {
                            if (this.Owner.OwningRow.Index == this.Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                            {
                                if (this.Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // Return the top left header cell accessible object.
                                    Debug.Assert(this.Owner.DataGridView.TopLeftHeaderCell.AccessibilityObject == this.Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0));
                                    return this.Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                int previousVisibleRow = this.Owner.DataGridView.Rows.GetPreviousRow(this.Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = this.Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                                if (this.Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return this.Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1).GetChild(0);
                                }
                                else
                                {
                                    return this.Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    default:
                        return null;
                }
            }

            /// <include file='doc\DataGridViewRowHeaderCell.uex' path='docs/doc[@for="DataGridViewRowHeaderCellAccessibleObject.Select"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags)
            {
                if (this.Owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }
                
                DataGridViewRowHeaderCell dataGridViewCell = (DataGridViewRowHeaderCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView == null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.FocusInternal();
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
                if (this.Owner.OwningRow == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return this.Owner.OwningRow.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        if (this.Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return this.Owner.OwningRow.AccessibilityObject.GetChild(1);
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
                if (AccessibilityImprovements.Level3)
                {
                    switch (propertyId)
                    {
                        case NativeMethods.UIA_NamePropertyId:
                            return this.Name;
                        case NativeMethods.UIA_ControlTypePropertyId:
                            return NativeMethods.UIA_HeaderControlTypeId;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return Owner.DataGridView.Enabled;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return this.Help ?? string.Empty;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return (this.State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        case NativeMethods.UIA_IsPasswordPropertyId:
                            return false;
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return (this.State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return string.Empty;
                    }
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
