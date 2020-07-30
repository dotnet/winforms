// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewColumnHeaderCell : DataGridViewHeaderCell
    {
        private static readonly VisualStyleElement s_headerElement = VisualStyleElement.Header.Item.Normal;

        private const byte SortGlyphSeparatorWidth = 2;     // additional 2 pixels between caption and glyph
        private const byte SortGlyphHorizontalMargin = 4;   // 4 pixels on left & right of glyph
        private const byte SortGlyphWidth = 9;              // glyph is 9 pixels wide by default
        private const byte SortGlyphHeight = 7;             // glyph is 7 pixels high by default (includes 1 blank line on top and 1 at the bottom)
        private const byte HorizontalTextMarginLeft = 2;
        private const byte HorizontalTextMarginRight = 2;
        private const byte VerticalMargin = 1;              // 1 pixel on top & bottom of glyph and text

        private static bool s_isScalingInitialized;
        private static byte s_sortGlyphSeparatorWidth = SortGlyphSeparatorWidth;
        private static byte s_sortGlyphHorizontalMargin = SortGlyphHorizontalMargin;
        private static byte s_sortGlyphWidth = SortGlyphWidth;
        private static byte s_sortGlyphHeight = SortGlyphHeight;

        private static readonly Type s_cellType = typeof(DataGridViewColumnHeaderCell);

        private SortOrder _sortGlyphDirection;

        public DataGridViewColumnHeaderCell()
        {
            if (!s_isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    s_sortGlyphSeparatorWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(SortGlyphSeparatorWidth);
                    s_sortGlyphHorizontalMargin = (byte)DpiHelper.LogicalToDeviceUnitsX(SortGlyphHorizontalMargin);
                    s_sortGlyphWidth = (byte)DpiHelper.LogicalToDeviceUnitsX(SortGlyphWidth);
                    // make sure that the width of the base of the arrow is odd, otherwise the tip of the arrow is one pixel off to the side
                    if ((s_sortGlyphWidth % 2) == 0)
                    {
                        s_sortGlyphWidth++;
                    }
                    s_sortGlyphHeight = (byte)DpiHelper.LogicalToDeviceUnitsY(SortGlyphHeight);
                }
                s_isScalingInitialized = true;
            }

            _sortGlyphDirection = SortOrder.None;
        }

        internal bool ContainsLocalValue
        {
            get
            {
                return Properties.ContainsObject(s_propCellValue);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortOrder SortGlyphDirection
        {
            get
            {
                return _sortGlyphDirection;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SortOrder.None, (int)SortOrder.Descending))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SortOrder));
                }
                if (OwningColumn is null || DataGridView is null)
                {
                    throw new InvalidOperationException(SR.DataGridView_CellDoesNotYetBelongToDataGridView);
                }
                if (value != _sortGlyphDirection)
                {
                    if (OwningColumn.SortMode == DataGridViewColumnSortMode.NotSortable && value != SortOrder.None)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewColumnHeaderCell_SortModeAndSortGlyphDirectionClash, (value).ToString()));
                    }
                    _sortGlyphDirection = value;
                    DataGridView.OnSortGlyphDirectionChanged(this);
                }
            }
        }

        internal SortOrder SortGlyphDirectionInternal
        {
            set
            {
                Debug.Assert(value >= SortOrder.None && value <= SortOrder.Descending);
                _sortGlyphDirection = value;
            }
        }

        public override object Clone()
        {
            DataGridViewColumnHeaderCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == s_cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewColumnHeaderCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewColumnHeaderCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.Value = Value;
            return dataGridViewCell;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewColumnHeaderCellAccessibleObject(this);
        }

        protected override object GetClipboardContent(int rowIndex,
                                                      bool firstCell,
                                                      bool lastCell,
                                                      bool inFirstRow,
                                                      bool inLastRow,
                                                      string format)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null)
            {
                return null;
            }

            // Not using formatted values for header cells.
            object val = GetValue(rowIndex);
            StringBuilder sb = new StringBuilder(64);

            Debug.Assert(inFirstRow);

            if (string.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
            {
                if (firstCell)
                {
                    sb.Append("<TABLE>");
                    sb.Append("<THEAD>");
                }
                sb.Append("<TH>");
                if (val != null)
                {
                    using var sw = new StringWriter(sb, CultureInfo.CurrentCulture);
                    FormatPlainTextAsHtml(val.ToString(), sw);
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
                bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
                if (csv ||
                    string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
                {
                    if (val != null)
                    {
                        bool escapeApplied = false;
                        int insertionPoint = sb.Length;
                        using var sw = new StringWriter(sb, CultureInfo.CurrentCulture);
                        FormatPlainText(val.ToString(), csv, sw, ref escapeApplied);
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
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null || OwningColumn is null)
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

        public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            ContextMenuStrip contextMenuStrip = GetContextMenuStrip(-1);
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
            if (DataGridView is null)
            {
                throw new InvalidOperationException(SR.DataGridView_CellNeedsDataGridViewForInheritedStyle);
            }
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            DataGridViewCellStyle inheritedCellStyleTmp = inheritedCellStyle ?? new DataGridViewCellStyle();

            DataGridViewCellStyle cellStyle = null;
            if (HasStyle)
            {
                cellStyle = Style;
                Debug.Assert(cellStyle != null);
            }

            DataGridViewCellStyle columnHeadersStyle = DataGridView.ColumnHeadersDefaultCellStyle;
            Debug.Assert(columnHeadersStyle != null);

            DataGridViewCellStyle dataGridViewStyle = DataGridView.DefaultCellStyle;
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

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            DataGridViewAdvancedBorderStyle dgvabsPlaceholder = new DataGridViewAdvancedBorderStyle(), dgvabsEffective;
            dgvabsEffective = DataGridView.AdjustColumnHeaderBorderStyle(DataGridView.AdvancedColumnHeadersBorderStyle,
                dgvabsPlaceholder,
                false /*isFirstDisplayedColumn*/,
                false /*isLastVisibleColumn*/);
            Rectangle borderWidthsRect = BorderWidths(dgvabsEffective);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

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
                                                                                           Math.Max(1, constraintSize.Height - borderAndPaddingHeights - 2 * VerticalMargin),
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
                        if (constraintSize.Height - borderAndPaddingHeights - 2 * VerticalMargin > s_sortGlyphHeight &&
                            OwningColumn != null &&
                            OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            preferredSize.Width += s_sortGlyphWidth +
                                                   2 * s_sortGlyphHorizontalMargin;
                            if (!string.IsNullOrEmpty(valStr))
                            {
                                preferredSize.Width += s_sortGlyphSeparatorWidth;
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

                        if (allowedWidth >= s_sortGlyphWidth + 2 * s_sortGlyphHorizontalMargin &&
                            OwningColumn != null &&
                            OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            glyphSize = new Size(s_sortGlyphWidth + 2 * s_sortGlyphHorizontalMargin,
                                                 s_sortGlyphHeight);
                        }
                        else
                        {
                            glyphSize = Size.Empty;
                        }

                        if (allowedWidth - HorizontalTextMarginLeft - HorizontalTextMarginRight > 0 &&
                            !string.IsNullOrEmpty(valStr))
                        {
                            if (cellStyle.WrapMode == DataGridViewTriState.True)
                            {
                                if (glyphSize.Width > 0 &&
                                    allowedWidth -
                                    HorizontalTextMarginLeft -
                                    HorizontalTextMarginRight -
                                    s_sortGlyphSeparatorWidth -
                                    glyphSize.Width > 0)
                                {
                                    preferredSize = new Size(0,
                                                             DataGridViewCell.MeasureTextHeight(graphics,
                                                                                                valStr,
                                                                                                cellStyle.Font,
                                                                                                allowedWidth -
                                                                                                HorizontalTextMarginLeft -
                                                                                                HorizontalTextMarginRight -
                                                                                                s_sortGlyphSeparatorWidth -
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
                                                                                                HorizontalTextMarginLeft -
                                                                                                HorizontalTextMarginRight,
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
                        if (OwningColumn != null &&
                            OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            preferredSize.Width += s_sortGlyphWidth +
                                                   2 * s_sortGlyphHorizontalMargin;
                            if (!string.IsNullOrEmpty(valStr))
                            {
                                preferredSize.Width += s_sortGlyphSeparatorWidth;
                            }
                            preferredSize.Height = Math.Max(preferredSize.Height, s_sortGlyphHeight);
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
                    preferredSize.Width += HorizontalTextMarginLeft + HorizontalTextMarginRight;
                }
                preferredSize.Width += borderAndPaddingWidths;
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += 2 * VerticalMargin + borderAndPaddingHeights;
            }
            if (DataGridView.ApplyVisualStylesToHeaderCells)
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

        protected override object GetValue(int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (ContainsLocalValue)
            {
                return Properties.GetObject(s_propCellValue);
            }
            else
            {
                if (OwningColumn != null)
                {
                    return OwningColumn.Name;
                }
                else
                {
                    return null;
                }
            }
        }

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
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
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

            if (DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (cellStyle.Padding != Padding.Empty)
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
                }
                // Theming
                if (paint && DataGridViewCell.PaintBackground(paintParts) && backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
                {
                    int state = (int)HeaderItemState.Normal;

                    // Set the state to Pressed/Hot only if the column can be sorted or selected
#pragma warning disable SA1408 // Conditional expressions should declare precedence
                    if (OwningColumn != null && OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable ||
#pragma warning restore SA1408 // Conditional expressions should declare precedence
                        DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        if (ButtonState != ButtonState.Normal)
                        {
                            Debug.Assert(ButtonState == ButtonState.Pushed);
                            state = (int)HeaderItemState.Pressed;
                        }
                        else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex &&
                                 DataGridView.MouseEnteredCellAddress.X == ColumnIndex)
                        {
                            state = (int)HeaderItemState.Hot;
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

                    // Even though Windows provides support for theming the sort glyph,
                    // we rely on our own implementation for painting the sort glyph
                    if (DataGridView.RightToLeftInternal)
                    {
                        // Flip the column header background
                        Bitmap bmFlipXPThemes = FlipXPThemesBitmap;
                        if (bmFlipXPThemes is null ||
                            bmFlipXPThemes.Width < backgroundBounds.Width || bmFlipXPThemes.Width > 2 * backgroundBounds.Width ||
                            bmFlipXPThemes.Height < backgroundBounds.Height || bmFlipXPThemes.Height > 2 * backgroundBounds.Height)
                        {
                            bmFlipXPThemes = FlipXPThemesBitmap = new Bitmap(backgroundBounds.Width, backgroundBounds.Height);
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
                if (DataGridView.RightToLeftInternal)
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
                if (paint && PaintBackground(paintParts) && backgroundBounds.Width > 0 && backgroundBounds.Height > 0)
                {
                    Color brushColor = (PaintSelectionBackground(paintParts) && cellSelected) || IsHighlighted()
                        ? cellStyle.SelectionBackColor : cellStyle.BackColor;

                    if (!brushColor.HasTransparency())
                    {
                        using var brush = brushColor.GetCachedSolidBrushScope();
                        g.FillRectangle(brush, backgroundBounds);
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

            bool displaySortGlyph = false;
            Point sortGlyphLocation = new Point(0, 0);
            string formattedValueStr = formattedValue as string;

            // Font independent margins
            valBounds.Y += VerticalMargin;
            valBounds.Height -= 2 * VerticalMargin;

            if (valBounds.Width - HorizontalTextMarginLeft - HorizontalTextMarginRight > 0 &&
                valBounds.Height > 0 &&
                !string.IsNullOrEmpty(formattedValueStr))
            {
                valBounds.Offset(HorizontalTextMarginLeft, 0);
                valBounds.Width -= HorizontalTextMarginLeft + HorizontalTextMarginRight;

                Color textColor;
                if (DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    textColor = DataGridViewColumnHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else
                {
                    textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                }

                if (OwningColumn != null && OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    // Is there enough room to show the glyph?
                    int width = valBounds.Width -
                        s_sortGlyphSeparatorWidth -
                        s_sortGlyphWidth -
                        2 * s_sortGlyphHorizontalMargin;
                    if (width > 0)
                    {
                        int preferredHeight = DataGridViewCell.GetPreferredTextHeight(g, DataGridView.RightToLeftInternal, formattedValueStr, cellStyle, width, out bool widthTruncated);
                        if (preferredHeight <= valBounds.Height && !widthTruncated)
                        {
                            displaySortGlyph = (SortGlyphDirection != SortOrder.None);
                            valBounds.Width -= s_sortGlyphSeparatorWidth +
                                               s_sortGlyphWidth +
                                               2 * s_sortGlyphHorizontalMargin;
                            if (DataGridView.RightToLeftInternal)
                            {
                                valBounds.X += s_sortGlyphSeparatorWidth +
                                               s_sortGlyphWidth +
                                               2 * s_sortGlyphHorizontalMargin;
                                sortGlyphLocation = new Point(valBounds.Left -
                                                              HorizontalTextMarginLeft -
                                                              s_sortGlyphSeparatorWidth -
                                                              s_sortGlyphHorizontalMargin -
                                                              s_sortGlyphWidth,
                                                              valBounds.Top +
                                                              (valBounds.Height - s_sortGlyphHeight) / 2);
                            }
                            else
                            {
                                sortGlyphLocation = new Point(valBounds.Right +
                                                              HorizontalTextMarginRight +
                                                              s_sortGlyphSeparatorWidth +
                                                              s_sortGlyphHorizontalMargin,
                                                              valBounds.Top +
                                                              (valBounds.Height - s_sortGlyphHeight) / 2);
                            }
                        }
                    }
                }
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
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
                if (paint && SortGlyphDirection != SortOrder.None &&
                    valBounds.Width >= s_sortGlyphWidth + 2 * s_sortGlyphHorizontalMargin &&
                    valBounds.Height >= s_sortGlyphHeight)
                {
                    displaySortGlyph = true;
                    sortGlyphLocation = new Point(valBounds.Left + (valBounds.Width - s_sortGlyphWidth) / 2,
                                                    valBounds.Top + (valBounds.Height - s_sortGlyphHeight) / 2);
                }
            }

            if (paint && displaySortGlyph && PaintContentBackground(paintParts))
            {
                (Color darkColor, Color lightColor) = GetContrastedColors(cellStyle.BackColor);
                using var penControlDark = darkColor.GetCachedPenScope();
                using var penControlLightLight = lightColor.GetCachedPenScope();

                if (SortGlyphDirection == SortOrder.Ascending)
                {
                    switch (advancedBorderStyle.Right)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.Outset:
                            // Sunken look
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 3,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            // Raised look
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 3,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 2);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            break;

                        default:
                            // Flat look
                            for (int line = 0; line < s_sortGlyphWidth / 2; line++)
                            {
                                g.DrawLine(penControlDark,
                                    sortGlyphLocation.X + line,
                                    sortGlyphLocation.Y + s_sortGlyphHeight - line - 1,
                                    sortGlyphLocation.X + s_sortGlyphWidth - line - 1,
                                    sortGlyphLocation.Y + s_sortGlyphHeight - line - 1);
                            }
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - s_sortGlyphWidth / 2);
                            break;
                    }
                }
                else
                {
                    Debug.Assert(SortGlyphDirection == SortOrder.Descending);
                    switch (advancedBorderStyle.Right)
                    {
                        case DataGridViewAdvancedCellBorderStyle.OutsetPartial:
                        case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
                        case DataGridViewAdvancedCellBorderStyle.Outset:
                            // Sunken look
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + 1,
                                sortGlyphLocation.Y + 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + 1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 3,
                                sortGlyphLocation.Y + 1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y);
                            break;

                        case DataGridViewAdvancedCellBorderStyle.Inset:
                            // Raised look
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y + 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            g.DrawLine(penControlLightLight,
                                sortGlyphLocation.X + 1,
                                sortGlyphLocation.Y + 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2 - 1,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y + 1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphHeight - 1,
                                sortGlyphLocation.X + s_sortGlyphWidth - 3,
                                sortGlyphLocation.Y + 1);
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X,
                                sortGlyphLocation.Y,
                                sortGlyphLocation.X + s_sortGlyphWidth - 2,
                                sortGlyphLocation.Y);
                            break;

                        default:
                            // Flat look
                            for (int line = 0; line < s_sortGlyphWidth / 2; line++)
                            {
                                g.DrawLine(penControlDark,
                                    sortGlyphLocation.X + line,
                                    sortGlyphLocation.Y + line + 2,
                                    sortGlyphLocation.X + s_sortGlyphWidth - line - 1,
                                    sortGlyphLocation.Y + line + 2);
                            }
                            g.DrawLine(penControlDark,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphWidth / 2 + 1,
                                sortGlyphLocation.X + s_sortGlyphWidth / 2,
                                sortGlyphLocation.Y + s_sortGlyphWidth / 2 + 2);
                            break;
                    }
                }
            }

            return contentBounds;
        }

        private bool IsHighlighted()
        {
            return DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                DataGridView.CurrentCell != null && DataGridView.CurrentCell.Selected &&
                DataGridView.CurrentCell.OwningColumn == OwningColumn;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            object originalValue = GetValue(rowIndex);
            Properties.SetObject(s_propCellValue, value);
            if (DataGridView != null && originalValue != value)
            {
                RaiseCellValueChanged(new DataGridViewCellEventArgs(ColumnIndex, -1));
            }
            return true;
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return "DataGridViewColumnHeaderCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }
    }
}
