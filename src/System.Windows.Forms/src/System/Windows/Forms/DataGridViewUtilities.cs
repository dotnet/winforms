// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    internal class DataGridViewUtilities
    {
        private const byte IconMarginWidth = 3;      // 3 pixels of margin on the left and right of icons
        private const byte IconMarginHeight = 2;     // 2 pixels of margin on the top and bottom of icons
        private const byte ContentMarginWidth = 3;   // 3 pixels of margin on the left and right of content
        private const byte IconsWidth = 12;          // all icons are 12 pixels wide - make sure that is stays that way
        private const byte IconsHeight = 11;         // all icons are 11 pixels tall - make sure that is stays that way

        private const byte HorizontalTextMarginLeft = 1;
        private const byte HorizontalTextMarginRight = 2;
        private const byte VerticalTextMargin = 1;

        internal static ContentAlignment ComputeDrawingContentAlignmentForCellStyleAlignment(DataGridViewContentAlignment alignment)
        {
            // Why isn't the DataGridView using System.Drawing.ContentAlignment?
            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                    return System.Drawing.ContentAlignment.TopLeft;
                case DataGridViewContentAlignment.TopCenter:
                    return System.Drawing.ContentAlignment.TopCenter;
                case DataGridViewContentAlignment.TopRight:
                    return System.Drawing.ContentAlignment.TopRight;
                case DataGridViewContentAlignment.MiddleLeft:
                    return System.Drawing.ContentAlignment.MiddleLeft;
                case DataGridViewContentAlignment.MiddleCenter:
                    return System.Drawing.ContentAlignment.MiddleCenter;
                case DataGridViewContentAlignment.MiddleRight:
                    return System.Drawing.ContentAlignment.MiddleRight;
                case DataGridViewContentAlignment.BottomLeft:
                    return System.Drawing.ContentAlignment.BottomLeft;
                case DataGridViewContentAlignment.BottomCenter:
                    return System.Drawing.ContentAlignment.BottomCenter;
                case DataGridViewContentAlignment.BottomRight:
                    return System.Drawing.ContentAlignment.BottomRight;
                default:
                    return System.Drawing.ContentAlignment.MiddleCenter;
            }
        }

        internal static TextFormatFlags ComputeTextFormatFlagsForCellStyleAlignment(bool rightToLeft,
                                                                                    DataGridViewContentAlignment alignment,
                                                                                    DataGridViewTriState wrapMode)
        {
            TextFormatFlags tff;
            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                    tff = TextFormatFlags.Top;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    break;
                case DataGridViewContentAlignment.TopCenter:
                    tff = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.TopRight:
                    tff = TextFormatFlags.Top;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    break;
                case DataGridViewContentAlignment.MiddleLeft:
                    tff = TextFormatFlags.VerticalCenter;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    break;
                case DataGridViewContentAlignment.MiddleCenter:
                    tff = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.MiddleRight:
                    tff = TextFormatFlags.VerticalCenter;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                    tff = TextFormatFlags.Bottom;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    break;
                case DataGridViewContentAlignment.BottomCenter:
                    tff = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.BottomRight:
                    tff = TextFormatFlags.Bottom;
                    if (rightToLeft)
                    {
                        tff |= TextFormatFlags.Left;
                    }
                    else
                    {
                        tff |= TextFormatFlags.Right;
                    }
                    break;
                default:
                    tff = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    break;
            }
            if (wrapMode == DataGridViewTriState.False)
            {
                tff |= TextFormatFlags.SingleLine;
            }
            else
            {
                //tff |= TextFormatFlags.NoFullWidthCharacterBreak;
                tff |= TextFormatFlags.WordBreak;
            }
            tff |= TextFormatFlags.NoPrefix;
            tff |= TextFormatFlags.PreserveGraphicsClipping;
            if (rightToLeft)
            {
                tff |= TextFormatFlags.RightToLeft;
            }
            return tff;
        }

        internal static Size GetPreferredRowHeaderSize(Graphics graphics,
                                                       string val,
                                                       DataGridViewCellStyle cellStyle,
                                                       int borderAndPaddingWidths,
                                                       int borderAndPaddingHeights,
                                                       bool showRowErrors,
                                                       bool showGlyph,
                                                       Size constraintSize,
                                                       TextFormatFlags flags)
        {
            Size preferredSize;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);

            switch (freeDimension)
            {
                case DataGridViewFreeDimension.Width:
                    {
                        int preferredWidth = 0, allowedHeight = constraintSize.Height - borderAndPaddingHeights;
                        if (!string.IsNullOrEmpty(val))
                        {
                            int maxHeight = allowedHeight - 2 * VerticalTextMargin;
                            if (maxHeight > 0)
                            {
                                if (cellStyle.WrapMode == DataGridViewTriState.True)
                                {
                                    preferredWidth = DataGridViewCell.MeasureTextWidth(graphics, val, cellStyle.Font, maxHeight, flags);
                                }
                                else
                                {
                                    preferredWidth = DataGridViewCell.MeasureTextSize(graphics, val, cellStyle.Font, flags).Width;
                                }
                                preferredWidth += 2 * ContentMarginWidth + HorizontalTextMarginLeft + HorizontalTextMarginRight;
                            }
                        }
                        if (allowedHeight >= IconsHeight + 2 * IconMarginHeight)
                        {
                            if (showGlyph)
                            {
                                preferredWidth += IconsWidth + 2 * IconMarginWidth;
                            }
                            if (showRowErrors)
                            {
                                preferredWidth += IconsWidth + 2 * IconMarginWidth;
                            }
                        }
                        preferredWidth = Math.Max(preferredWidth, 1);
                        preferredWidth += borderAndPaddingWidths;
                        return new Size(preferredWidth, 0);
                    }
                case DataGridViewFreeDimension.Height:
                    {
                        int minHeightIcon = 1, minHeightContent = 1;
                        int allowedWidth = constraintSize.Width - borderAndPaddingWidths;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (showGlyph && allowedWidth >= 2 * IconMarginWidth + IconsWidth)
                            {
                                // There is enough room for the status icon
                                minHeightIcon = IconsHeight + 2 * IconMarginHeight;
                                // Status icon takes priority
                                allowedWidth -= 2 * IconMarginWidth + IconsWidth;
                            }
                            if (showRowErrors && allowedWidth >= 2 * IconMarginWidth + IconsWidth)
                            {
                                // There is enough room for the error icon
                                minHeightIcon = IconsHeight + 2 * IconMarginHeight;
                                // There is enough room for both the status and error icons
                                allowedWidth -= 2 * IconMarginWidth + IconsWidth;
                            }
                            if (allowedWidth > 2 * ContentMarginWidth +
                                               HorizontalTextMarginLeft +
                                               HorizontalTextMarginRight)
                            {
                                // There is enough room for text
                                allowedWidth -= 2 * ContentMarginWidth +
                                                HorizontalTextMarginLeft +
                                                HorizontalTextMarginRight;
                                if (cellStyle.WrapMode == DataGridViewTriState.True)
                                {
                                    minHeightContent = DataGridViewCell.MeasureTextHeight(graphics, val, cellStyle.Font, allowedWidth, flags);
                                }
                                else
                                {
                                    minHeightContent = DataGridViewCell.MeasureTextSize(graphics, val, cellStyle.Font, flags).Height;
                                }
                                minHeightContent += 2 * VerticalTextMargin;
                            }
                        }
                        else
                        {
                            if ((showGlyph || showRowErrors) && allowedWidth >= 2 * IconMarginWidth + IconsWidth)
                            {
                                minHeightIcon = IconsHeight + 2 * IconMarginHeight;
                            }
                        }
                        return new Size(0, Math.Max(minHeightIcon, minHeightContent) + borderAndPaddingHeights);
                    }
                default:
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (cellStyle.WrapMode == DataGridViewTriState.True)
                            {
                                preferredSize = DataGridViewCell.MeasureTextPreferredSize(graphics, val, cellStyle.Font, 5.0F, flags);
                            }
                            else
                            {
                                preferredSize = DataGridViewCell.MeasureTextSize(graphics, val, cellStyle.Font, flags);
                            }
                            preferredSize.Width += 2 * ContentMarginWidth +
                                                   HorizontalTextMarginLeft +
                                                   HorizontalTextMarginRight;
                            preferredSize.Height += 2 * VerticalTextMargin;
                        }
                        else
                        {
                            preferredSize = new Size(0, 1);
                        }
                        if (showGlyph)
                        {
                            preferredSize.Width += IconsWidth + 2 * IconMarginWidth;
                        }
                        if (showRowErrors)
                        {
                            preferredSize.Width += IconsWidth + 2 * IconMarginWidth;
                        }
                        if (showGlyph || showRowErrors)
                        {
                            preferredSize.Height = Math.Max(preferredSize.Height,
                                                            IconsHeight + 2 * IconMarginHeight);
                        }
                        preferredSize.Width += borderAndPaddingWidths;
                        preferredSize.Height += borderAndPaddingHeights;
                        return preferredSize;
                    }
            }
        }

        internal static Rectangle GetTextBounds(Rectangle cellBounds,
                                                string text,
                                                TextFormatFlags flags,
                                                DataGridViewCellStyle cellStyle)
        {
            return GetTextBounds(cellBounds, text, flags, cellStyle, cellStyle.Font);
        }

        internal static Rectangle GetTextBounds(Rectangle cellBounds,
                                                string text,
                                                TextFormatFlags flags,
                                                DataGridViewCellStyle cellStyle,
                                                Font font)
        {
            if ((flags & TextFormatFlags.SingleLine) != 0)
            {
                Size sizeRequired = TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), flags);
                if (sizeRequired.Width > cellBounds.Width)
                {
                    flags |= TextFormatFlags.EndEllipsis;
                }
            }

            Size sizeCell = new Size(cellBounds.Width, cellBounds.Height);
            Size sizeConstraint = TextRenderer.MeasureText(text, font, sizeCell, flags);
            if (sizeConstraint.Width > sizeCell.Width)
            {
                sizeConstraint.Width = sizeCell.Width;
            }
            if (sizeConstraint.Height > sizeCell.Height)
            {
                sizeConstraint.Height = sizeCell.Height;
            }
            if (sizeConstraint == sizeCell)
            {
                return cellBounds;
            }
            return new Rectangle(GetTextLocation(cellBounds, sizeConstraint, flags, cellStyle), sizeConstraint);
        }

        internal static Point GetTextLocation(Rectangle cellBounds,
                                              Size sizeText,
                                              TextFormatFlags flags,
                                              DataGridViewCellStyle cellStyle)
        {
            Point ptTextLocation = new Point(0, 0);

            // now use the alignment on the cellStyle to determine the final text location
            DataGridViewContentAlignment alignment = cellStyle.Alignment;
            if ((flags & TextFormatFlags.RightToLeft) != 0)
            {
                switch (alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                        alignment = DataGridViewContentAlignment.TopRight;
                        break;

                    case DataGridViewContentAlignment.TopRight:
                        alignment = DataGridViewContentAlignment.TopLeft;
                        break;

                    case DataGridViewContentAlignment.MiddleLeft:
                        alignment = DataGridViewContentAlignment.MiddleRight;
                        break;

                    case DataGridViewContentAlignment.MiddleRight:
                        alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;

                    case DataGridViewContentAlignment.BottomLeft:
                        alignment = DataGridViewContentAlignment.BottomRight;
                        break;

                    case DataGridViewContentAlignment.BottomRight:
                        alignment = DataGridViewContentAlignment.BottomLeft;
                        break;
                }
            }

            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                    ptTextLocation.X = cellBounds.X;
                    ptTextLocation.Y = cellBounds.Y;
                    break;

                case DataGridViewContentAlignment.TopCenter:
                    ptTextLocation.X = cellBounds.X + (cellBounds.Width - sizeText.Width) / 2;
                    ptTextLocation.Y = cellBounds.Y;
                    break;

                case DataGridViewContentAlignment.TopRight:
                    ptTextLocation.X = cellBounds.Right - sizeText.Width;
                    ptTextLocation.Y = cellBounds.Y;
                    break;

                case DataGridViewContentAlignment.MiddleLeft:
                    ptTextLocation.X = cellBounds.X;
                    ptTextLocation.Y = cellBounds.Y + (cellBounds.Height - sizeText.Height) / 2;
                    break;

                case DataGridViewContentAlignment.MiddleCenter:
                    ptTextLocation.X = cellBounds.X + (cellBounds.Width - sizeText.Width) / 2;
                    ptTextLocation.Y = cellBounds.Y + (cellBounds.Height - sizeText.Height) / 2;
                    break;

                case DataGridViewContentAlignment.MiddleRight:
                    ptTextLocation.X = cellBounds.Right - sizeText.Width;
                    ptTextLocation.Y = cellBounds.Y + (cellBounds.Height - sizeText.Height) / 2;
                    break;

                case DataGridViewContentAlignment.BottomLeft:
                    ptTextLocation.X = cellBounds.X;
                    ptTextLocation.Y = cellBounds.Bottom - sizeText.Height;
                    break;

                case DataGridViewContentAlignment.BottomCenter:
                    ptTextLocation.X = cellBounds.X + (cellBounds.Width - sizeText.Width) / 2;
                    ptTextLocation.Y = cellBounds.Bottom - sizeText.Height;
                    break;

                case DataGridViewContentAlignment.BottomRight:
                    ptTextLocation.X = cellBounds.Right - sizeText.Width;
                    ptTextLocation.Y = cellBounds.Bottom - sizeText.Height;
                    break;

                default:
                    Debug.Assert(cellStyle.Alignment == DataGridViewContentAlignment.NotSet, "this is the only alignment left");
                    break;
            }
            return ptTextLocation;
        }

        internal static bool ValidTextFormatFlags(TextFormatFlags flags)
        {
            return (flags & ~(TextFormatFlags.Bottom |
                              TextFormatFlags.Default |
                              TextFormatFlags.EndEllipsis |
                              TextFormatFlags.ExpandTabs |
                              TextFormatFlags.ExternalLeading |
                              TextFormatFlags.HidePrefix |
                              TextFormatFlags.HorizontalCenter |
                              TextFormatFlags.Internal |
                              TextFormatFlags.Left |
                              TextFormatFlags.ModifyString |
                              TextFormatFlags.NoClipping |
                              TextFormatFlags.NoFullWidthCharacterBreak |
                              TextFormatFlags.NoPrefix |
                              TextFormatFlags.PathEllipsis |
                              TextFormatFlags.PrefixOnly |
                              TextFormatFlags.PreserveGraphicsClipping |
                              TextFormatFlags.PreserveGraphicsTranslateTransform |
                              TextFormatFlags.Right |
                              TextFormatFlags.RightToLeft |
                              TextFormatFlags.SingleLine |
                              TextFormatFlags.TextBoxControl |
                              TextFormatFlags.Top |
                              TextFormatFlags.VerticalCenter |
                              TextFormatFlags.WordBreak |
                              TextFormatFlags.WordEllipsis)) == 0;
        }
    }
}
