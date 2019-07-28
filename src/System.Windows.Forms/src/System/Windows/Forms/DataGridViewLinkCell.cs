// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    public class DataGridViewLinkCell : DataGridViewCell
    {
        private static readonly DataGridViewContentAlignment anyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;

        private static readonly Type defaultFormattedValueType = typeof(string);
        private static readonly Type defaultValueType = typeof(object);
        private static readonly Type cellType = typeof(DataGridViewLinkCell);

        private static readonly int PropLinkCellActiveLinkColor = PropertyStore.CreateKey();
        private static readonly int PropLinkCellLinkBehavior = PropertyStore.CreateKey();
        private static readonly int PropLinkCellLinkColor = PropertyStore.CreateKey();
        private static readonly int PropLinkCellLinkState = PropertyStore.CreateKey();
        private static readonly int PropLinkCellTrackVisitedState = PropertyStore.CreateKey();
        private static readonly int PropLinkCellUseColumnTextForLinkValue = PropertyStore.CreateKey();
        private static readonly int PropLinkCellVisitedLinkColor = PropertyStore.CreateKey();

        private const byte DATAGRIDVIEWLINKCELL_horizontalTextMarginLeft = 1;
        private const byte DATAGRIDVIEWLINKCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWLINKCELL_verticalTextMarginTop = 1;
        private const byte DATAGRIDVIEWLINKCELL_verticalTextMarginBottom = 1;

        // we cache LinkVisited because it will be set multiple times
        private bool linkVisited = false;
        private bool linkVisitedSet = false;

        private static Cursor dataGridViewCursor = null;

        public DataGridViewLinkCell()
        {
        }

        public Color ActiveLinkColor
        {
            get
            {
                if (Properties.ContainsObject(PropLinkCellActiveLinkColor))
                {
                    return (Color)Properties.GetObject(PropLinkCellActiveLinkColor);
                }
                else if (SystemInformation.HighContrast)
                {
                    return HighContrastLinkColor;
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IEActiveLinkColor;
                }
            }
            set
            {
                if (!value.Equals(ActiveLinkColor))
                {
                    Properties.SetObject(PropLinkCellActiveLinkColor, value);
                    if (DataGridView != null)
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

        internal Color ActiveLinkColorInternal
        {
            set
            {
                if (!value.Equals(ActiveLinkColor))
                {
                    Properties.SetObject(PropLinkCellActiveLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeActiveLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !ActiveLinkColor.Equals(SystemColors.HotTrack);
            }

            return !ActiveLinkColor.Equals(LinkUtilities.IEActiveLinkColor);
        }

        public override Type EditType
        {
            get
            {
                // links can't switch to edit mode
                return null;
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return defaultFormattedValueType;
            }
        }

        [DefaultValue(LinkBehavior.SystemDefault)]
        public LinkBehavior LinkBehavior
        {
            get
            {
                int linkBehavior = Properties.GetInteger(PropLinkCellLinkBehavior, out bool found);
                if (found)
                {
                    return (LinkBehavior)linkBehavior;
                }
                return LinkBehavior.SystemDefault;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LinkBehavior.SystemDefault, (int)LinkBehavior.NeverUnderline))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LinkBehavior));
                }
                if (value != LinkBehavior)
                {
                    Properties.SetInteger(PropLinkCellLinkBehavior, (int)value);
                    if (DataGridView != null)
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

        internal LinkBehavior LinkBehaviorInternal
        {
            set
            {
                Debug.Assert(value >= LinkBehavior.SystemDefault && value <= LinkBehavior.NeverUnderline);
                if (value != LinkBehavior)
                {
                    Properties.SetInteger(PropLinkCellLinkBehavior, (int)value);
                }
            }
        }

        public Color LinkColor
        {
            get
            {
                if (Properties.ContainsObject(PropLinkCellLinkColor))
                {
                    return (Color)Properties.GetObject(PropLinkCellLinkColor);
                }
                else if (SystemInformation.HighContrast)
                {
                    return HighContrastLinkColor;
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IELinkColor;
                }
            }
            set
            {
                if (!value.Equals(LinkColor))
                {
                    Properties.SetObject(PropLinkCellLinkColor, value);
                    if (DataGridView != null)
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

        internal Color LinkColorInternal
        {
            set
            {
                if (!value.Equals(LinkColor))
                {
                    Properties.SetObject(PropLinkCellLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !LinkColor.Equals(SystemColors.HotTrack);
            }

            return !LinkColor.Equals(LinkUtilities.IELinkColor);
        }

        private LinkState LinkState
        {
            get
            {
                int linkState = Properties.GetInteger(PropLinkCellLinkState, out bool found);
                if (found)
                {
                    return (LinkState)linkState;
                }
                return LinkState.Normal;
            }
            set
            {
                if (LinkState != value)
                {
                    Properties.SetInteger(PropLinkCellLinkState, (int)value);
                }
            }
        }

        public bool LinkVisited
        {
            get
            {
                if (linkVisitedSet)
                {
                    return linkVisited;
                }

                // the default is false
                return false;
            }
            set
            {
                linkVisitedSet = true;
                if (value != LinkVisited)
                {
                    linkVisited = value;
                    if (DataGridView != null)
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

        private bool ShouldSerializeLinkVisited()
        {
            return linkVisitedSet = true;
        }

        [DefaultValue(true)]
        public bool TrackVisitedState
        {
            get
            {
                int trackVisitedState = Properties.GetInteger(PropLinkCellTrackVisitedState, out bool found);
                if (found)
                {
                    return trackVisitedState == 0 ? false : true;
                }
                return true;
            }
            set
            {
                if (value != TrackVisitedState)
                {
                    Properties.SetInteger(PropLinkCellTrackVisitedState, value ? 1 : 0);
                    if (DataGridView != null)
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

        internal bool TrackVisitedStateInternal
        {
            set
            {
                if (value != TrackVisitedState)
                {
                    Properties.SetInteger(PropLinkCellTrackVisitedState, value ? 1 : 0);
                }
            }
        }

        [DefaultValue(false)]
        public bool UseColumnTextForLinkValue
        {
            get
            {
                int useColumnTextForLinkValue = Properties.GetInteger(PropLinkCellUseColumnTextForLinkValue, out bool found);
                if (found)
                {
                    return useColumnTextForLinkValue == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != UseColumnTextForLinkValue)
                {
                    Properties.SetInteger(PropLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
                    OnCommonChange();
                }
            }
        }

        internal bool UseColumnTextForLinkValueInternal
        {
            set
            {
                // Caller is responsible for invalidation
                if (value != UseColumnTextForLinkValue)
                {
                    Properties.SetInteger(PropLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
                }
            }
        }

        public Color VisitedLinkColor
        {
            get
            {
                if (Properties.ContainsObject(PropLinkCellVisitedLinkColor))
                {
                    return (Color)Properties.GetObject(PropLinkCellVisitedLinkColor);
                }
                else if (SystemInformation.HighContrast)
                {
                    return Selected ? SystemColors.HighlightText : LinkUtilities.GetVisitedLinkColor();
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IEVisitedLinkColor;
                }
            }
            set
            {
                if (!value.Equals(VisitedLinkColor))
                {
                    Properties.SetObject(PropLinkCellVisitedLinkColor, value);
                    if (DataGridView != null)
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

        internal Color VisitedLinkColorInternal
        {
            set
            {
                if (!value.Equals(VisitedLinkColor))
                {
                    Properties.SetObject(PropLinkCellVisitedLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeVisitedLinkColor()
        {
            if (SystemInformation.HighContrast)
            {
                return !VisitedLinkColor.Equals(SystemColors.HotTrack);
            }

            return !VisitedLinkColor.Equals(LinkUtilities.IEVisitedLinkColor);
        }

        private Color HighContrastLinkColor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // Selected cells have SystemColors.Highlight as a background.
                // SystemColors.HighlightText is supposed to be in contrast with SystemColors.Highlight.
                return Selected ? SystemColors.HighlightText : SystemColors.HotTrack;
            }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
        }

        public override object Clone()
        {
            DataGridViewLinkCell dataGridViewCell;
            Type thisType = GetType();

            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewLinkCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewLinkCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);

            if (Properties.ContainsObject(PropLinkCellActiveLinkColor))
            {
                dataGridViewCell.ActiveLinkColorInternal = ActiveLinkColor;
            }

            if (Properties.ContainsInteger(PropLinkCellUseColumnTextForLinkValue))
            {
                dataGridViewCell.UseColumnTextForLinkValueInternal = UseColumnTextForLinkValue;
            }

            if (Properties.ContainsInteger(PropLinkCellLinkBehavior))
            {
                dataGridViewCell.LinkBehaviorInternal = LinkBehavior;
            }

            if (Properties.ContainsObject(PropLinkCellLinkColor))
            {
                dataGridViewCell.LinkColorInternal = LinkColor;
            }

            if (Properties.ContainsInteger(PropLinkCellTrackVisitedState))
            {
                dataGridViewCell.TrackVisitedStateInternal = TrackVisitedState;
            }

            if (Properties.ContainsObject(PropLinkCellVisitedLinkColor))
            {
                dataGridViewCell.VisitedLinkColorInternal = VisitedLinkColor;
            }

            if (linkVisitedSet)
            {
                dataGridViewCell.LinkVisited = LinkVisited;
            }

            return dataGridViewCell;
        }

        private bool LinkBoundsContainPoint(int x, int y, int rowIndex)
        {
            Rectangle linkBounds = GetContentBounds(rowIndex);

            return linkBounds.Contains(x, y);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewLinkCellAccessibleObject(this);
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null || rowIndex < 0 || OwningColumn == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle linkBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                null /*errorText*/,                 // linkBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true  /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            Rectangle linkBoundsDebug = PaintPrivate(graphics,
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
            Debug.Assert(linkBoundsDebug.Equals(linkBounds));
#endif

            return linkBounds;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null ||
                rowIndex < 0 ||
                OwningColumn == null ||
                !DataGridView.ShowCellErrors ||
                string.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle errorIconBounds = PaintPrivate(graphics,
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

            return errorIconBounds;
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

            Size preferredSize;
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            object formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize);
            string formattedString = formattedValue as string;
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
                            int maxHeight = constraintSize.Height - borderAndPaddingHeights - DATAGRIDVIEWLINKCELL_verticalTextMarginTop - DATAGRIDVIEWLINKCELL_verticalTextMarginBottom;
                            if ((cellStyle.Alignment & anyBottom) != 0)
                            {
                                maxHeight--;
                            }
                            preferredSize = new Size(DataGridViewCell.MeasureTextWidth(graphics,
                                                                                       formattedString,
                                                                                       cellStyle.Font,
                                                                                       Math.Max(1, maxHeight),
                                                                                       flags),
                                                     0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0,
                                                     DataGridViewCell.MeasureTextHeight(graphics,
                                                                                        formattedString,
                                                                                        cellStyle.Font,
                                                                                        Math.Max(1, constraintSize.Width - borderAndPaddingWidths - DATAGRIDVIEWLINKCELL_horizontalTextMarginLeft - DATAGRIDVIEWLINKCELL_horizontalTextMarginRight),
                                                                                        flags));
                            break;
                        }
                    default:
                        {
                            preferredSize = DataGridViewCell.MeasureTextPreferredSize(graphics,
                                                                                      formattedString,
                                                                                      cellStyle.Font,
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
                            preferredSize = new Size(DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Width,
                                                     0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0,
                                                     DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Height);
                            break;
                        }
                    default:
                        {
                            preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags);
                            break;
                        }
                }
            }

            if (freeDimension != DataGridViewFreeDimension.Height)
            {
                preferredSize.Width += DATAGRIDVIEWLINKCELL_horizontalTextMarginLeft + DATAGRIDVIEWLINKCELL_horizontalTextMarginRight + borderAndPaddingWidths;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                preferredSize.Height += DATAGRIDVIEWLINKCELL_verticalTextMarginTop + DATAGRIDVIEWLINKCELL_verticalTextMarginBottom + borderAndPaddingHeights;
                if ((cellStyle.Alignment & anyBottom) != 0)
                {
                    preferredSize.Height += DATAGRIDVIEWLINKCELL_verticalTextMarginBottom;
                }
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        protected override object GetValue(int rowIndex)
        {
            if (UseColumnTextForLinkValue &&
                DataGridView != null &&
                DataGridView.NewRowIndex != rowIndex &&
                OwningColumn != null &&
                OwningColumn is DataGridViewLinkColumn)
            {
                return ((DataGridViewLinkColumn)OwningColumn).Text;
            }
            return base.GetValue(rowIndex);
        }

        protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                return TrackVisitedState && !LinkVisited;
            }
            else
            {
                return true;
            }
        }

        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);
        }

        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return LinkState != LinkState.Normal;
        }

        protected override bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                if ((LinkState & LinkState.Hover) == 0)
                {
                    return true;
                }
            }
            else
            {
                if ((LinkState & LinkState.Hover) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return TrackVisitedState && LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);
        }

        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                RaiseCellClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
                if (DataGridView != null &&
                    ColumnIndex < DataGridView.Columns.Count &&
                    rowIndex < DataGridView.Rows.Count)
                {
                    RaiseCellContentClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
                    if (TrackVisitedState)
                    {
                        LinkVisited = true;
                    }
                }
                e.Handled = true;
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                LinkState |= LinkState.Active;
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (dataGridViewCursor != null)
            {
                DataGridView.Cursor = dataGridViewCursor;
                dataGridViewCursor = null;
            }
            if (LinkState != LinkState.Normal)
            {
                LinkState = LinkState.Normal;
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }

            base.OnMouseLeave(rowIndex);
        }

        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                if ((LinkState & LinkState.Hover) == 0)
                {
                    LinkState |= LinkState.Hover;
                    DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
                }

                if (dataGridViewCursor == null)
                {
                    dataGridViewCursor = DataGridView.UserSetCursor;
                }

                if (DataGridView.Cursor != Cursors.Hand)
                {
                    DataGridView.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if ((LinkState & LinkState.Hover) != 0)
                {
                    LinkState &= ~LinkState.Hover;
                    DataGridView.Cursor = dataGridViewCursor;
                    DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex) && TrackVisitedState)
            {
                LinkVisited = true;
            }
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
            DataGridViewElementStates cellState,
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

            Rectangle resultBounds = Rectangle.Empty;

            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            Rectangle valBounds = cellBounds;
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
            SolidBrush br = DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
            {
                g.FillRectangle(br, valBounds);
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

            Rectangle errorBounds = valBounds;

            if (formattedValue is string formattedValueStr && (paint || computeContentBounds))
            {
                // Font independent margins
                valBounds.Offset(DATAGRIDVIEWLINKCELL_horizontalTextMarginLeft, DATAGRIDVIEWLINKCELL_verticalTextMarginTop);
                valBounds.Width -= DATAGRIDVIEWLINKCELL_horizontalTextMarginLeft + DATAGRIDVIEWLINKCELL_horizontalTextMarginRight;
                valBounds.Height -= DATAGRIDVIEWLINKCELL_verticalTextMarginTop + DATAGRIDVIEWLINKCELL_verticalTextMarginBottom;
                if ((cellStyle.Alignment & anyBottom) != 0)
                {
                    valBounds.Height -= DATAGRIDVIEWLINKCELL_verticalTextMarginBottom;
                }

                Font linkFont = null;
                Font hoverFont = null;
                LinkUtilities.EnsureLinkFonts(cellStyle.Font, LinkBehavior, ref linkFont, ref hoverFont);
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                // paint the focus rectangle around the link
                if (paint)
                {
                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {
                        if (cellCurrent &&
                            DataGridView.ShowFocusCues &&
                            DataGridView.Focused &&
                            DataGridViewCell.PaintFocus(paintParts))
                        {
                            Rectangle focusBounds = DataGridViewUtilities.GetTextBounds(valBounds,
                                                                                        formattedValueStr,
                                                                                        flags,
                                                                                        cellStyle,
                                                                                        LinkState == LinkState.Hover ? hoverFont : linkFont);
                            if ((cellStyle.Alignment & anyLeft) != 0)
                            {
                                focusBounds.X--;
                                focusBounds.Width++;
                            }
                            else if ((cellStyle.Alignment & anyRight) != 0)
                            {
                                focusBounds.X++;
                                focusBounds.Width++;
                            }
                            focusBounds.Height += 2;
                            ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, br.Color);
                        }
                        Color linkColor;
                        if ((LinkState & LinkState.Active) == LinkState.Active)
                        {
                            linkColor = ActiveLinkColor;
                        }
                        else if (LinkVisited)
                        {
                            linkColor = VisitedLinkColor;
                        }
                        else
                        {
                            linkColor = LinkColor;
                        }
                        if (DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            if ((flags & TextFormatFlags.SingleLine) != 0)
                            {
                                flags |= TextFormatFlags.EndEllipsis;
                            }
                            TextRenderer.DrawText(g,
                                                  formattedValueStr,
                                                  LinkState == LinkState.Hover ? hoverFont : linkFont,
                                                  valBounds,
                                                  linkColor,
                                                  flags);
                        }
                    }
                    else if (cellCurrent &&
                             DataGridView.ShowFocusCues &&
                             DataGridView.Focused &&
                             DataGridViewCell.PaintFocus(paintParts) &&
                             errorBounds.Width > 0 &&
                             errorBounds.Height > 0)
                    {
                        // Draw focus rectangle
                        ControlPaint.DrawFocusRectangle(g, errorBounds, Color.Empty, br.Color);
                    }
                }
                else
                {
                    Debug.Assert(computeContentBounds);
                    resultBounds = DataGridViewUtilities.GetTextBounds(valBounds,
                                                                       formattedValueStr,
                                                                       flags,
                                                                       cellStyle,
                                                                       LinkState == LinkState.Hover ? hoverFont : linkFont);
                }
                linkFont.Dispose();
                hoverFont.Dispose();
            }
            else if (paint || computeContentBounds)
            {
                if (cellCurrent &&
                    DataGridView.ShowFocusCues &&
                    DataGridView.Focused &&
                    DataGridViewCell.PaintFocus(paintParts) &&
                    paint &&
                    valBounds.Width > 0 &&
                    valBounds.Height > 0)
                {
                    // Draw focus rectangle
                    ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, br.Color);
                }
            }
            else if (computeErrorIconBounds)
            {
                if (!string.IsNullOrEmpty(errorText))
                {
                    resultBounds = ComputeErrorIconBounds(errorBounds);
                }
            }

            if (DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        public override string ToString()
        {
            return "DataGridViewLinkCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewLinkCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.DataGridView_AccLinkCellDefaultAction;
                }
            }

            public override void DoDefaultAction()
            {
                DataGridViewLinkCell dataGridViewCell = (DataGridViewLinkCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                if (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            public override int GetChildCount()
            {
                return 0;
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_HyperlinkControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
