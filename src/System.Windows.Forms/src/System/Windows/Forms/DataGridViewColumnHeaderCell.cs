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
    using System.ComponentModel;
    using System.Windows.Forms.VisualStyles;
    using System.Security.Permissions;
    using System.Windows.Forms.Internal;
    using System.Globalization;

    /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell"]/*' />
    public class DataGridViewColumnHeaderCell : DataGridViewHeaderCell
    {
        private static readonly VisualStyleElement HeaderElement = VisualStyleElement.Header.Item.Normal;

        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphSeparatorWidth = 2;    // additional 2 pixels between caption and glyph
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHorizontalMargin = 4;  // 4 pixels on left & right of glyph
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphWidth = 9;   // glyph is 9 pixels wide by default
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHeight = 7;  // glyph is 7 pixels high by default (includes 1 blank line on top and 1 at the bottom)
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft = 2;
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin = 1;  // 1 pixel on top & bottom of glyph and text

        private static bool isScalingInitialized = false;
        private static byte sortGlyphSeparatorWidth = DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphSeparatorWidth;
        private static byte sortGlyphHorizontalMargin = DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHorizontalMargin;
        private static byte sortGlyphWidth = DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphWidth;
        private static byte sortGlyphHeight = DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHeight; 

        private static Type cellType = typeof(DataGridViewColumnHeaderCell);

        private SortOrder sortGlyphDirection;

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.DataGridViewColumnHeaderCell"]/*' />
        public DataGridViewColumnHeaderCell()
        {
            if (!isScalingInitialized) {
                if (DpiHelper.IsScalingRequired) {
                    sortGlyphSeparatorWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphSeparatorWidth);
                    sortGlyphHorizontalMargin = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHorizontalMargin);
                    sortGlyphWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphWidth);
                    // make sure that the width of the base of the arrow is odd, otherwise the tip of the arrow is one pixel off to the side
                    if ((sortGlyphWidth % 2) == 0) {
                        sortGlyphWidth++;
                    }
                    sortGlyphHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(DATAGRIDVIEWCOLUMNHEADERCELL_sortGlyphHeight);
                }
                isScalingInitialized = true;
            }

            this.sortGlyphDirection = SortOrder.None;
        }

        internal bool ContainsLocalValue
        {
            get
            {
                return this.Properties.ContainsObject(PropCellValue);
            }
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.SortGlyphDirection"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortOrder SortGlyphDirection
        {
            get
            {
                return this.sortGlyphDirection;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SortOrder.None, (int)SortOrder.Descending))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(SortOrder)); 
                }
                if (this.OwningColumn == null || this.DataGridView == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_CellDoesNotYetBelongToDataGridView));
                }
                if (value != this.sortGlyphDirection)
                {
                    if (this.OwningColumn.SortMode == DataGridViewColumnSortMode.NotSortable && value != SortOrder.None)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewColumnHeaderCell_SortModeAndSortGlyphDirectionClash, (value).ToString()));
                    }
                    this.sortGlyphDirection = value;
                    this.DataGridView.OnSortGlyphDirectionChanged(this);
                }
            }
        }

        internal SortOrder SortGlyphDirectionInternal
        {
            set
            {
                Debug.Assert(value >= SortOrder.None && value <= SortOrder.Descending);
                this.sortGlyphDirection = value;
            }
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewColumnHeaderCell dataGridViewCell;
            Type thisType = this.GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewColumnHeaderCell();
            }
            else
            {
                // 

                dataGridViewCell = (DataGridViewColumnHeaderCell) System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = this.Value;
            return dataGridViewCell;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewColumnHeaderCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetClipboardContent"]/*' />
        protected override object GetClipboardContent(int rowIndex,
                                                      bool firstCell,
                                                      bool lastCell,
                                                      bool inFirstRow,
                                                      bool inLastRow,
                                                      string format)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (this.DataGridView == null)
            {
                return null;
            }

            // Not using formatted values for header cells.
            object val = GetValue(rowIndex);
            StringBuilder sb = new StringBuilder(64);

            Debug.Assert(inFirstRow);

            if (String.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
            {
                if (firstCell)
                {
                    sb.Append("<TABLE>");
                    sb.Append("<THEAD>");
                }
                sb.Append("<TH>");
                if (val != null)
                {
                    FormatPlainTextAsHtml(val.ToString(), new StringWriter(sb, CultureInfo.CurrentCulture));
                }
                else
                {
                    sb.Append("&nbsp;");
                }
                sb.Append("</TH>");
                if (lastCell)
                {
                    sb.Append("</THEAD>");
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

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetContentBounds"]/*' />
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException("cellStyle");
            }

            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }
            
            if (this.DataGridView == null || this.OwningColumn == null)
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
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*paint*/);

#if DEBUG
            Rectangle contentBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*paint*/);
            Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

            return contentBounds;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetInheritedContextMenuStrip"]/*' />
        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(-1);
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

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetInheritedStyle"]/*' />
        public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            Debug.Assert(this.DataGridView != null);

            DataGridViewCellStyle inheritedCellStyleTmp = (inheritedCellStyle == null) ? new DataGridViewCellStyle() : inheritedCellStyle;

            DataGridViewCellStyle cellStyle = null;
            if (this.HasStyle)
            {
                cellStyle = this.Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle columnHeadersStyle = this.DataGridView.ColumnHeadersDefaultCellStyle;
            Debug.Assert(columnHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = this.DataGridView.DefaultCellStyle;
            Debug.Assert(dataGridViewStyle != null);

            if (includeColors)
            {
                if (cellStyle != null && !cellStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = cellStyle.BackColor;
                } 
                else if (!columnHeadersStyle.BackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.BackColor = columnHeadersStyle.BackColor;
                }
                else
                {
                    inheritedCellStyleTmp.BackColor = dataGridViewStyle.BackColor;
                }

                if (cellStyle != null && !cellStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = cellStyle.ForeColor;
                } 
                else if (!columnHeadersStyle.ForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.ForeColor = columnHeadersStyle.ForeColor;
                }
                else
                {
                    inheritedCellStyleTmp.ForeColor = dataGridViewStyle.ForeColor;
                }

                if (cellStyle != null && !cellStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = cellStyle.SelectionBackColor;
                } 
                else if (!columnHeadersStyle.SelectionBackColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionBackColor = columnHeadersStyle.SelectionBackColor;
                }
                else
                {
                    inheritedCellStyleTmp.SelectionBackColor = dataGridViewStyle.SelectionBackColor;
                }

                if (cellStyle != null && !cellStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = cellStyle.SelectionForeColor;
                } 
                else if (!columnHeadersStyle.SelectionForeColor.IsEmpty)
                {
                    inheritedCellStyleTmp.SelectionForeColor = columnHeadersStyle.SelectionForeColor;
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
            else if (columnHeadersStyle.Font != null)
            {
                inheritedCellStyleTmp.Font = columnHeadersStyle.Font;
            }
            else
            {
                inheritedCellStyleTmp.Font = dataGridViewStyle.Font;
            }

            if (cellStyle != null && !cellStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = cellStyle.NullValue;
            }
            else if (!columnHeadersStyle.IsNullValueDefault)
            {
                inheritedCellStyleTmp.NullValue = columnHeadersStyle.NullValue;
            }
            else
            {
                inheritedCellStyleTmp.NullValue = dataGridViewStyle.NullValue;
            }

            if (cellStyle != null && !cellStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = cellStyle.DataSourceNullValue;
            }
            else if (!columnHeadersStyle.IsDataSourceNullValueDefault)
            {
                inheritedCellStyleTmp.DataSourceNullValue = columnHeadersStyle.DataSourceNullValue;
            }
            else
            {
                inheritedCellStyleTmp.DataSourceNullValue = dataGridViewStyle.DataSourceNullValue;
            }

            if (cellStyle != null && cellStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = cellStyle.Format;
            } 
            else if (columnHeadersStyle.Format.Length != 0)
            {
                inheritedCellStyleTmp.Format = columnHeadersStyle.Format;
            }
            else
            {
                inheritedCellStyleTmp.Format = dataGridViewStyle.Format;
            }

            if (cellStyle != null && !cellStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = cellStyle.FormatProvider;
            }
            else if (!columnHeadersStyle.IsFormatProviderDefault)
            {
                inheritedCellStyleTmp.FormatProvider = columnHeadersStyle.FormatProvider;
            }
            else
            {
                inheritedCellStyleTmp.FormatProvider = dataGridViewStyle.FormatProvider;
            }

            if (cellStyle != null && cellStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = cellStyle.Alignment;
            } 
            else if (columnHeadersStyle.Alignment != DataGridViewContentAlignment.NotSet)
            {
                inheritedCellStyleTmp.AlignmentInternal = columnHeadersStyle.Alignment;
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
            else if (columnHeadersStyle.WrapMode != DataGridViewTriState.NotSet)
            {
                inheritedCellStyleTmp.WrapModeInternal = columnHeadersStyle.WrapMode;
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
            else if (columnHeadersStyle.Tag != null)
            {
                inheritedCellStyleTmp.Tag = columnHeadersStyle.Tag;
            }
            else
            {
                inheritedCellStyleTmp.Tag = dataGridViewStyle.Tag;
            }

            if (cellStyle != null && cellStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = cellStyle.Padding;
            }
            else if (columnHeadersStyle.Padding != Padding.Empty)
            {
                inheritedCellStyleTmp.PaddingInternal = columnHeadersStyle.Padding;
            }
            else
            {
                inheritedCellStyleTmp.PaddingInternal = dataGridViewStyle.Padding;
            }

            return inheritedCellStyleTmp;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetPreferredSize"]/*' />
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException("cellStyle");
            }

            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = this.DataGridView.AdjustColumnHeaderBorderStyle(this.DataGridView.AdvancedColumnHeadersBorderStyle,
                dgvabsPlaceholder,
                false /*isFirstDisplayedColumn*/,
                false /*isLastVisibleColumn*/);
            Rectangle borderWidthsRect = BorderWidths(dgvabsEffective);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            Size preferredSize;
            // approximate preferred sizes
            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            string valStr = GetValue(rowIndex) as string;

            switch (freeDimension)
            {
                case DataGridViewFreeDimension.Width:
                {
                    preferredSize = new Size(0, 0);
                    if (!string.IsNullOrEmpty(valStr))
                    {
                        if (cellStyle.WrapMode == DataGridViewTriState.True)
                        {
                            preferredSize = new Size(DataGridViewCell.MeasureTextWidth(graphics, 
                                                                                       valStr, 
                                                                                       cellStyle.Font,
                                                                                       Math.Max(1, constraintSize.Height - borderAndPaddingHeights - 2 * DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin),
                                                                                       flags), 
                                                     0);
                        }
                        else
                        {
                            preferredSize = new Size(DataGridViewCell.MeasureTextSize(graphics, 
                                                                                      valStr, 
                                                                                      cellStyle.Font, 
                                                                                      flags).Width, 
                                                     0);
                        }
                    }
                    if (constraintSize.Height - borderAndPaddingHeights - 2 * DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin > sortGlyphHeight &&
                        this.OwningColumn != null && 
                        this.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                    {
                        preferredSize.Width += sortGlyphWidth + 
                                               2 * sortGlyphHorizontalMargin;
                        if (!string.IsNullOrEmpty(valStr))
                        {
                            preferredSize.Width += sortGlyphSeparatorWidth;
                        }
                    }
                    preferredSize.Width = Math.Max(preferredSize.Width, 1);
                    break;
                }
                case DataGridViewFreeDimension.Height:
                {
                    int allowedWidth = constraintSize.Width - borderAndPaddingWidths;
                    Size glyphSize;
                    preferredSize = new Size(0, 0);

                    if (allowedWidth >= sortGlyphWidth + 2 * sortGlyphHorizontalMargin &&
                        this.OwningColumn != null && 
                        this.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                    {
                        glyphSize = new Size(sortGlyphWidth + 2 * sortGlyphHorizontalMargin,
                                             sortGlyphHeight);
                    }
                    else
                    {
                        glyphSize = Size.Empty;
                    }

                    if (allowedWidth - DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft - DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight > 0 &&
                        !string.IsNullOrEmpty(valStr))
                    {
                        if (cellStyle.WrapMode == DataGridViewTriState.True)
                        {
                            if (glyphSize.Width > 0 &&
                                allowedWidth -
                                DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft -
                                DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight -
                                sortGlyphSeparatorWidth -
                                glyphSize.Width > 0)
                            {
                                preferredSize = new Size(0,
                                                         DataGridViewCell.MeasureTextHeight(graphics, 
                                                                                            valStr, 
                                                                                            cellStyle.Font,
                                                                                            allowedWidth -
                                                                                            DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft -
                                                                                            DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight -
                                                                                            sortGlyphSeparatorWidth -
                                                                                            glyphSize.Width,
                                                                                            flags));
                            }
                            else
                            {
                                preferredSize = new Size(0,
                                                         DataGridViewCell.MeasureTextHeight(graphics, 
                                                                                            valStr, 
                                                                                            cellStyle.Font,
                                                                                            allowedWidth -
                                                                                            DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft -
                                                                                            DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight,
                                                                                            flags));
                            }
                        }
                        else
                        {
                            preferredSize = new Size(0,
                                                     DataGridViewCell.MeasureTextSize(graphics, 
                                                                                      valStr, 
                                                                                      cellStyle.Font, 
                                                                                      flags).Height);
                        }
                    }
                    preferredSize.Height = Math.Max(preferredSize.Height, glyphSize.Height);
                    preferredSize.Height = Math.Max(preferredSize.Height, 1);
                    break;
                }
                default:
                {
                    if (!string.IsNullOrEmpty(valStr))
                    {
                        if (cellStyle.WrapMode == DataGridViewTriState.True)
                        {
                            preferredSize = DataGridViewCell.MeasureTextPreferredSize(graphics, valStr, cellStyle.Font, 5.0F, flags);
                        }
                        else
                        {
                            preferredSize = DataGridViewCell.MeasureTextSize(graphics, valStr, cellStyle.Font, flags);
                        }
                    }
                    else
                    {
                        preferredSize = new Size(0, 0);
                    }
                    if (this.OwningColumn != null &&
                        this.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                    {
                        preferredSize.Width += sortGlyphWidth +
                                               2 * sortGlyphHorizontalMargin;
                        if (!string.IsNullOrEmpty(valStr))
                        {
                            preferredSize.Width += sortGlyphSeparatorWidth;
                        }
                        preferredSize.Height = Math.Max(preferredSize.Height, sortGlyphHeight);
                    }
                    preferredSize.Width = Math.Max(preferredSize.Width, 1);
                    preferredSize.Height = Math.Max(preferredSize.Height, 1);
                    break;
                }
            }

            if (freeDimension != DataGridViewFreeDimension.Height)
            {
                if (!string.IsNullOrEmpty(valStr))
                {
                    preferredSize.Width += DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft + DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight;
                }
                preferredSize.Width += borderAndPaddingWidths;
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += 2 * DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin + borderAndPaddingHeights;
            }
            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                Rectangle rectThemeMargins = DataGridViewHeaderCell.GetThemeMargins(graphics);
                if (freeDimension != DataGridViewFreeDimension.Height)
                {
                    preferredSize.Width += rectThemeMargins.X + rectThemeMargins.Width;
                }
                if (freeDimension != DataGridViewFreeDimension.Width)
                {
                    preferredSize.Height += rectThemeMargins.Y + rectThemeMargins.Height;
                }
            }
            return preferredSize;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }
            if (this.ContainsLocalValue)
            {
                return this.Properties.GetObject(PropCellValue);
            }
            else
            {
                if (this.OwningColumn != null)
                {
                    return this.OwningColumn.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.Paint"]/*' />
        protected override void Paint(Graphics graphics, 
            Rectangle clipBounds,
            Rectangle cellBounds, 
            int rowIndex, 
            DataGridViewElementStates dataGridViewElementState, 
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException("cellStyle");
            }

            PaintPrivate(graphics, 
                         clipBounds,
                         cellBounds, 
                         rowIndex, 
                         dataGridViewElementState, 
                         formattedValue,
                         cellStyle, 
                         advancedBorderStyle,
                         paintParts,
                         true /*paint*/);
        }


        // PaintPrivate is used in two places that need to duplicate the paint code:
        // 1. DataGridViewCell::Paint method
        // 2. DataGridViewCell::GetContentBounds
        // PaintPrivate returns the content bounds;
        private Rectangle PaintPrivate(Graphics g,
            Rectangle clipBounds,
            Rectangle cellBounds, 
            int rowIndex, 
            DataGridViewElementStates dataGridViewElementState,
            object formattedValue,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts,
            bool paint)
        {
            Debug.Assert(cellStyle != null);
            Rectangle contentBounds = Rectangle.Empty;

            if (paint && DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);

            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;
            Rectangle backgroundBounds = valBounds;

            bool cellSelected = (dataGridViewElementState & DataGridViewElementStates.Selected) != 0;
            SolidBrush br;

            if (this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (cellStyle.Padding != Padding.Empty)
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
                }
                // XP Theming
                if (paint && DataGridViewCell.PaintBackground(paintParts) && backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
                {
                    int state = (int) HeaderItemState.Normal;

                    // Set the state to Pressed/Hot only if the column can be sorted or selected
                    if (this.OwningColumn != null && this.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable ||
                        this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        this.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        if (this.ButtonState != ButtonState.Normal)
                        {
                            Debug.Assert(this.ButtonState == ButtonState.Pushed);
                            state = (int) HeaderItemState.Pressed;
                        }
                        else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex && 
                                 this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex)
                        {
                            state = (int) HeaderItemState.Hot;
                        }
                        else if (cellSelected)
                        {
                            state = (int)HeaderItemState.Pressed;
                        }
                    }

                    if (IsHighlighted())
                    {
                        state = (int)HeaderItemState.Pressed;
                    }

                    // Microsoft: even though XP provides support for theming the sort glyph, 
                    // we rely on our own implementation for painting the sort glyph
                    if (this.DataGridView.RightToLeftInternal)
                    {
                        // Flip the column header background
                        Bitmap bmFlipXPThemes = this.FlipXPThemesBitmap;
                        if (bmFlipXPThemes == null || 
                            bmFlipXPThemes.Width < backgroundBounds.Width || bmFlipXPThemes.Width > 2 * backgroundBounds.Width ||
                            bmFlipXPThemes.Height < backgroundBounds.Height || bmFlipXPThemes.Height > 2 * backgroundBounds.Height)
                        {
                            bmFlipXPThemes = this.FlipXPThemesBitmap = new Bitmap(backgroundBounds.Width, backgroundBounds.Height);
                        }
                        Graphics gFlip = Graphics.FromImage(bmFlipXPThemes);
                        DataGridViewColumnHeaderCellRenderer.DrawHeader(gFlip, new Rectangle(0, 0, backgroundBounds.Width, backgroundBounds.Height), state);
                        bmFlipXPThemes.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        g.DrawImage(bmFlipXPThemes, backgroundBounds, new Rectangle(bmFlipXPThemes.Width - backgroundBounds.Width, 0, backgroundBounds.Width, backgroundBounds.Height), GraphicsUnit.Pixel); 
                    }
                    else
                    {
                        DataGridViewColumnHeaderCellRenderer.DrawHeader(g, backgroundBounds, state);
                    }
                }
                // update the value bounds
                Rectangle rectThemeMargins = DataGridViewHeaderCell.GetThemeMargins(g);
                valBounds.Y += rectThemeMargins.Y;
                valBounds.Height -= rectThemeMargins.Y + rectThemeMargins.Height;
                if (this.DataGridView.RightToLeftInternal)
                {
                    valBounds.X += rectThemeMargins.Width;
                    valBounds.Width -= rectThemeMargins.X + rectThemeMargins.Width;
                }
                else
                {
                    valBounds.X += rectThemeMargins.X;
                    valBounds.Width -= rectThemeMargins.X + rectThemeMargins.Width;
                }
            }
            else
            {
                if (paint && DataGridViewCell.PaintBackground(paintParts) && backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
                {
                    br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) || IsHighlighted() ? 
                        cellStyle.SelectionBackColor : cellStyle.BackColor);
                    if (br.Color.A == 255)
                    {
                        g.FillRectangle(br, backgroundBounds);
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

            bool displaySortGlyph = false;
            Point sortGlyphLocation = new Point(0, 0);
            string formattedValueStr = formattedValue as string;

            // Font independent margins
            valBounds.Y += DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin;
            valBounds.Height -= 2 * DATAGRIDVIEWCOLUMNHEADERCELL_verticalMargin;

            if (valBounds.Width - DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft - DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight > 0 &&
                valBounds.Height > 0 &&
                !String.IsNullOrEmpty(formattedValueStr))
            {
                valBounds.Offset(DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft, 0);
                valBounds.Width -= DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft + DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight;

                Color textColor;
                if (this.DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    textColor = DataGridViewColumnHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else
                {
                    textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                }

                if (this.OwningColumn != null && this.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    // Is there enough room to show the glyph?
                    int width = valBounds.Width -
                        sortGlyphSeparatorWidth -
                        sortGlyphWidth -
                        2 * sortGlyphHorizontalMargin;
                    if (width > 0)
                    {
                        bool widthTruncated;
                        int preferredHeight = DataGridViewCell.GetPreferredTextHeight(g, this.DataGridView.RightToLeftInternal, formattedValueStr, cellStyle, width, out widthTruncated);
                        if (preferredHeight <= valBounds.Height && !widthTruncated)
                        {
                            displaySortGlyph = (this.SortGlyphDirection != SortOrder.None);
                            valBounds.Width -= sortGlyphSeparatorWidth +
                                               sortGlyphWidth +
                                               2 * sortGlyphHorizontalMargin;
                            if (this.DataGridView.RightToLeftInternal)
                            {
                                valBounds.X += sortGlyphSeparatorWidth +
                                               sortGlyphWidth +
                                               2 * sortGlyphHorizontalMargin;
                                sortGlyphLocation = new Point(valBounds.Left -
                                                              DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginLeft -
                                                              sortGlyphSeparatorWidth -
                                                              sortGlyphHorizontalMargin -
                                                              sortGlyphWidth,
                                                              valBounds.Top + 
                                                              (valBounds.Height-sortGlyphHeight)/2);
                            }
                            else
                            {
                                sortGlyphLocation = new Point(valBounds.Right +
                                                              DATAGRIDVIEWCOLUMNHEADERCELL_horizontalTextMarginRight +
                                                              sortGlyphSeparatorWidth + 
                                                              sortGlyphHorizontalMargin,
                                                              valBounds.Top + 
                                                              (valBounds.Height-sortGlyphHeight)/2);
                            }
                        }
                    }
                }
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                if (paint)
                {
                    if (DataGridViewCell.PaintContentForeground(paintParts))
                    {
                        if ((flags & TextFormatFlags.SingleLine) != 0)
                        {
                            flags |= TextFormatFlags.EndEllipsis;
                        }
                        TextRenderer.DrawText(g,
                                              formattedValueStr,
                                              cellStyle.Font,
                                              valBounds,
                                              textColor,
                                              flags);
                    }
                }
                else
                {
                    contentBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedValueStr, flags, cellStyle);
                }
            }
            else
            {
                if (paint && this.SortGlyphDirection != SortOrder.None &&
                    valBounds.Width >= sortGlyphWidth + 2 * sortGlyphHorizontalMargin &&
                    valBounds.Height >= sortGlyphHeight)
                {
                    displaySortGlyph = true;
                    sortGlyphLocation = new Point(valBounds.Left + (valBounds.Width-sortGlyphWidth)/2,
                                                    valBounds.Top + (valBounds.Height-sortGlyphHeight)/2);
                }
            }

            if (paint && displaySortGlyph && DataGridViewCell.PaintContentBackground(paintParts))
            {
                Pen penControlDark = null, penControlLightLight = null;
                GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);

                if (this.SortGlyphDirection == SortOrder.Ascending)
                {
                    switch (advancedBorderStyle.Right)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.Outset:
                            // Sunken look
                            g.DrawLine(penControlDark, 
                                sortGlyphLocation.X, 
                                sortGlyphLocation.Y + sortGlyphHeight-2, 
                                sortGlyphLocation.X + sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlDark, 
                                sortGlyphLocation.X+1, 
                                sortGlyphLocation.Y + sortGlyphHeight-2, 
                                sortGlyphLocation.X + sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + sortGlyphWidth/2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + sortGlyphWidth-2,
                                sortGlyphLocation.Y + sortGlyphHeight-2);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + sortGlyphWidth/2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + sortGlyphWidth-3,
                                sortGlyphLocation.Y + sortGlyphHeight-2);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + sortGlyphHeight-1,
                                sortGlyphLocation.X + sortGlyphWidth-2,
                                sortGlyphLocation.Y + sortGlyphHeight-1);
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            // Raised look
                            g.DrawLine(penControlLightLight, 
                                sortGlyphLocation.X, 
                                sortGlyphLocation.Y + sortGlyphHeight - 2, 
                                sortGlyphLocation.X + sortGlyphWidth / 2 - 1, 
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlLightLight, 
                                sortGlyphLocation.X+1, 
                                sortGlyphLocation.Y + sortGlyphHeight - 2, 
                                sortGlyphLocation.X + sortGlyphWidth / 2 - 1, 
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + sortGlyphWidth - 2,
                                sortGlyphLocation.Y + sortGlyphHeight - 2);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + sortGlyphWidth - 3,
                                sortGlyphLocation.Y + sortGlyphHeight - 2);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + sortGlyphHeight - 1,
                                sortGlyphLocation.X + sortGlyphWidth - 2,
                                sortGlyphLocation.Y + sortGlyphHeight - 1);
                            break;

                        default:
                            // Flat look
                            for (int line = 0; line < sortGlyphWidth / 2; line++)
                            {
                                g.DrawLine(penControlDark,
                                    sortGlyphLocation.X + line,
                                    sortGlyphLocation.Y + sortGlyphHeight - line - 1,
                                    sortGlyphLocation.X + sortGlyphWidth - line - 1,
                                    sortGlyphLocation.Y + sortGlyphHeight - line - 1);
                            }
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y + sortGlyphHeight - sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y + sortGlyphHeight - sortGlyphWidth / 2);
                            break;
                    }
                }
                else
                {
                    Debug.Assert(this.SortGlyphDirection == SortOrder.Descending);
                    switch (advancedBorderStyle.Right)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.Outset:
                            // Sunken look
                            g.DrawLine(penControlDark, 
                                sortGlyphLocation.X, 
                                sortGlyphLocation.Y+1, 
                                sortGlyphLocation.X+sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y+sortGlyphHeight-1);
                            g.DrawLine(penControlDark, 
                                sortGlyphLocation.X+1, 
                                sortGlyphLocation.Y+1, 
                                sortGlyphLocation.X+sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y+sortGlyphHeight-1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X+sortGlyphWidth/2,
                                sortGlyphLocation.Y+sortGlyphHeight-1,
                                sortGlyphLocation.X+sortGlyphWidth-2,
                                sortGlyphLocation.Y+1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X+sortGlyphWidth/2,
                                sortGlyphLocation.Y+sortGlyphHeight-1,
                                sortGlyphLocation.X+sortGlyphWidth-3,
                                sortGlyphLocation.Y+1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X+sortGlyphWidth-2,
                                sortGlyphLocation.Y);
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            // Raised look
                            g.DrawLine(penControlLightLight, 
                                sortGlyphLocation.X, 
                                sortGlyphLocation.Y+1, 
                                sortGlyphLocation.X+sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y+sortGlyphHeight-1);
                            g.DrawLine(penControlLightLight, 
                                sortGlyphLocation.X+1, 
                                sortGlyphLocation.Y+1, 
                                sortGlyphLocation.X+sortGlyphWidth/2-1, 
                                sortGlyphLocation.Y+sortGlyphHeight-1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X+sortGlyphWidth/2,
                                sortGlyphLocation.Y+sortGlyphHeight-1,
                                sortGlyphLocation.X+sortGlyphWidth-2,
                                sortGlyphLocation.Y+1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X+sortGlyphWidth/2,
                                sortGlyphLocation.Y+sortGlyphHeight-1,
                                sortGlyphLocation.X+sortGlyphWidth-3,
                                sortGlyphLocation.Y+1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X+sortGlyphWidth-2,
                                sortGlyphLocation.Y);
                            break;

                        default:
                            // Flat look
                            for (int line = 0; line < sortGlyphWidth / 2; line++)
                            {
                                g.DrawLine(penControlDark,
                                    sortGlyphLocation.X + line,
                                    sortGlyphLocation.Y + line + 2,
                                    sortGlyphLocation.X + sortGlyphWidth - line - 1,
                                    sortGlyphLocation.Y + line + 2);
                            }
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y + sortGlyphWidth / 2 + 1,
                                sortGlyphLocation.X + sortGlyphWidth / 2,
                                sortGlyphLocation.Y + sortGlyphWidth / 2 + 2);
                            break;
                    }
                }
            }

            return contentBounds;
        }

        private bool IsHighlighted()
        {
            return this.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect && 
                this.DataGridView.CurrentCell != null && this.DataGridView.CurrentCell.Selected &&
                this.DataGridView.CurrentCell.OwningColumn == this.OwningColumn &&
                AccessibilityImprovements.Level2;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.SetValue"]/*' />
        protected override bool SetValue(int rowIndex, object value)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            object originalValue = GetValue(rowIndex);
            this.Properties.SetObject(PropCellValue, value);
            if (this.DataGridView != null && originalValue != value)
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(this.ColumnIndex, -1));
            }
            return true;
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCell.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
        public override string ToString()
        {
            return "DataGridViewColumnHeaderCell { ColumnIndex=" + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        private class DataGridViewColumnHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewColumnHeaderCellRenderer()
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
                Rectangle rectClip = Rectangle.Truncate(g.ClipBounds);
                if ((int) HeaderItemState.Hot == headerState)
                {
                    // Workaround for a 
                    VisualStyleRenderer.SetParameters(HeaderElement);
                    Rectangle cornerClip = new Rectangle(bounds.Left, bounds.Bottom-2, 2, 2);
                    cornerClip.Intersect(rectClip);
                    VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
                    cornerClip = new Rectangle(bounds.Right-2, bounds.Bottom-2, 2, 2);
                    cornerClip.Intersect(rectClip);
                    VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
                }
                VisualStyleRenderer.SetParameters(HeaderElement.ClassName, HeaderElement.Part, headerState);
                VisualStyleRenderer.DrawBackground(g, bounds, rectClip);
            }
        }

        /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject"]/*' />
        protected class DataGridViewColumnHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewColumnHeaderCellAccessibleObject(DataGridViewColumnHeaderCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds
            {
                get
                {
                    return this.GetAccessibleObjectBounds(this.ParentPrivate);
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    if (this.Owner.OwningColumn != null)
                    {
                        if (this.Owner.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                        {
                            return string.Format(SR.DataGridView_AccColumnHeaderCellDefaultAction);
                        }
                        else if (this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect||
                                 this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            return string.Format(SR.DataGridView_AccColumnHeaderCellSelectDefaultAction);
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Name"]/*' />
            public override string Name
            {
                get
                {
                    if (this.Owner.OwningColumn != null)
                    {
                        return this.Owner.OwningColumn.HeaderText;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Parent"]/*' />
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
                    // return the top header row accessible object
                    return this.Owner.DataGridView.AccessibilityObject.GetChild(0);
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Role"]/*' />
            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ColumnHeader;
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.State"]/*' />
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

                    if (this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        this.Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        if (this.Owner.OwningColumn != null && this.Owner.OwningColumn.Selected)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.Name;
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                DataGridViewColumnHeaderCell dataGridViewCell = (DataGridViewColumnHeaderCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridViewCell.OwningColumn != null)
                {
                    if (dataGridViewCell.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        ListSortDirection listSortDirection = ListSortDirection.Ascending;
                        if (dataGridView.SortedColumn == dataGridViewCell.OwningColumn && dataGridView.SortOrder == SortOrder.Ascending)
                        {
                            listSortDirection = ListSortDirection.Descending;
                        }
                        dataGridView.Sort(dataGridViewCell.OwningColumn, listSortDirection);
                    }
                    else if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                             dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        dataGridViewCell.OwningColumn.Selected = true;
                    }
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (this.Owner.OwningColumn == null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateForward();
                        }
                        else
                        {
                            return NavigateBackward();
                        }
                    case AccessibleNavigation.Next:
                        return NavigateForward();
                    case AccessibleNavigation.Left:
                        if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateBackward();
                        }
                        else
                        {
                            return NavigateForward();
                        }
                    case AccessibleNavigation.Previous:
                        return NavigateBackward();
                    case AccessibleNavigation.FirstChild:
                        // return the top left header cell accessible object
                        return this.Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0);
                    case AccessibleNavigation.LastChild:
                        // return the last column header cell in the top row header accessible object
                        AccessibleObject topRowHeaderAccessibleObject = this.Owner.DataGridView.AccessibilityObject.GetChild(0);
                        return topRowHeaderAccessibleObject.GetChild(topRowHeaderAccessibleObject.GetChildCount() - 1);
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateBackward()
            {
                if (this.Owner.OwningColumn == this.Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    if (this.Owner.DataGridView.RowHeadersVisible)
                    {
                        // return the row header cell accessible object for the current row
                        return this.Parent.GetChild(0);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    int previousVisibleColumnIndex = this.Owner.DataGridView.Columns.GetPreviousColumn(this.Owner.OwningColumn,
                                                                                                                DataGridViewElementStates.Visible,
                                                                                                                DataGridViewElementStates.None).Index;
                    int actualDisplayIndex = this.Owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(previousVisibleColumnIndex,
                                                                                                             DataGridViewElementStates.Visible);
                    if (this.Owner.DataGridView.RowHeadersVisible)
                    {
                        return this.Parent.GetChild(actualDisplayIndex + 1);
                    }
                    else
                    {
                        return this.Parent.GetChild(actualDisplayIndex);
                    }
                }
            }

            private AccessibleObject NavigateForward()
            {
                if (this.Owner.OwningColumn == this.Owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                                      DataGridViewElementStates.None))
                {
                    return null;
                }
                else
                {
                    int nextVisibleColumnIndex = this.Owner.DataGridView.Columns.GetNextColumn(this.Owner.OwningColumn,
                                                                                                        DataGridViewElementStates.Visible,
                                                                                                        DataGridViewElementStates.None).Index;
                    int actualDisplayIndex = this.Owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(nextVisibleColumnIndex,
                                                                                                             DataGridViewElementStates.Visible);

                    if (this.Owner.DataGridView.RowHeadersVisible)
                    {
                        // + 1 because the top header row accessible object has the top left header cell accessible object at the beginning
                        return this.Parent.GetChild(actualDisplayIndex + 1);
                    }
                    else
                    {
                        return this.Parent.GetChild(actualDisplayIndex);
                    }
                }
            }

            /// <include file='doc\DataGridViewColumnHeaderCell.uex' path='docs/doc[@for="DataGridViewColumnHeaderCellAccessibleObject.Select"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags)
            {
                if (this.Owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                DataGridViewColumnHeaderCell dataGridViewCell = (DataGridViewColumnHeaderCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView == null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.FocusInternal();
                }
                if (dataGridViewCell.OwningColumn != null &&
                    (dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                     dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                {
                    if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.AddSelection)) != 0)
                    {
                        dataGridViewCell.OwningColumn.Selected = true;
                    }
                    else if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection)
                    {
                        dataGridViewCell.OwningColumn.Selected = false;
                    }
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                if (this.Owner.OwningColumn == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return Parent;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        return NavigateForward();
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        return NavigateBackward();
                    default:
                        return null;
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(int patternId)
            {
                return patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId) ||
                    patternId.Equals(NativeMethods.UIA_InvokePatternId);
            }

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
