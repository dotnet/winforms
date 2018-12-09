// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Windows.Forms.Internal;
    using System.Security.Permissions;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Runtime.CompilerServices;

    /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell"]/*' />
    public class DataGridViewLinkCell : DataGridViewCell
    {
        private static readonly DataGridViewContentAlignment anyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;

        private static Type defaultFormattedValueType = typeof(System.String);
        private static Type defaultValueType = typeof(System.Object);
        private static Type cellType = typeof(DataGridViewLinkCell);

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

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.DataGridViewLinkCell"]/*' />
        public DataGridViewLinkCell()
        {
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.ActiveLinkColor"]/*' />
        public Color ActiveLinkColor
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // unboxing OK here.
            ]
            get
            {
                if (this.Properties.ContainsObject(PropLinkCellActiveLinkColor))
                {
                    return (Color)this.Properties.GetObject(PropLinkCellActiveLinkColor);
                }
                else if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
                {
                    return this.HighContrastLinkColor;
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IEActiveLinkColor;
                }
            }
            set
            {
                if (!value.Equals(this.ActiveLinkColor))
                {
                    this.Properties.SetObject(PropLinkCellActiveLinkColor, value);
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

        internal Color ActiveLinkColorInternal
        {
            set
            {
                if (!value.Equals(this.ActiveLinkColor))
                {
                    this.Properties.SetObject(PropLinkCellActiveLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeActiveLinkColor()
        {
            if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
            {
                return !this.ActiveLinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.ActiveLinkColor.Equals(LinkUtilities.IEActiveLinkColor);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.EditType"]/*' />
        public override Type EditType
        {
            get
            {
                // links can't switch to edit mode
                return null;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.FormattedValueType"]/*' />
        public override Type FormattedValueType
        {
            get
            {
                return defaultFormattedValueType;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.LinkBehavior"]/*' />
        [DefaultValue(LinkBehavior.SystemDefault)]
        public LinkBehavior LinkBehavior
        {
            get
            {
                bool found;
                int linkBehavior = this.Properties.GetInteger(PropLinkCellLinkBehavior, out found);
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
                if (value != this.LinkBehavior)
                {
                    this.Properties.SetInteger(PropLinkCellLinkBehavior, (int)value);
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

        internal LinkBehavior LinkBehaviorInternal
        {
            set
            {
                Debug.Assert(value >= LinkBehavior.SystemDefault && value <= LinkBehavior.NeverUnderline);
                if (value != this.LinkBehavior)
                {
                    this.Properties.SetInteger(PropLinkCellLinkBehavior, (int)value);
                }
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.LinkColor"]/*' />
        public Color LinkColor
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // unboxing OK here.
            ]
            get
            {
                if (this.Properties.ContainsObject(PropLinkCellLinkColor))
                {
                    return (Color)this.Properties.GetObject(PropLinkCellLinkColor);
                }
                else if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
                {
                    return this.HighContrastLinkColor;
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IELinkColor;
                }
            }
            set
            {
                if (!value.Equals(this.LinkColor))
                {
                    this.Properties.SetObject(PropLinkCellLinkColor, value);
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

        internal Color LinkColorInternal
        {
            set
            {
                if (!value.Equals(this.LinkColor))
                {
                    this.Properties.SetObject(PropLinkCellLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeLinkColor()
        {
            if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
            {
                return !this.LinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.LinkColor.Equals(LinkUtilities.IELinkColor);
        }

        private LinkState LinkState
        {
            get
            {
                bool found;
                int linkState = this.Properties.GetInteger(PropLinkCellLinkState, out found);
                if (found)
                {
                    return (LinkState) linkState;
                }
                return LinkState.Normal;
            }
            set
            {
                if (this.LinkState != value)
                {
                    this.Properties.SetInteger(PropLinkCellLinkState, (int) value);
                }
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.LinkVisited"]/*' />
        public bool LinkVisited
        {
            get
            {
                if (this.linkVisitedSet)
                {
                    return this.linkVisited;
                }

                // the default is false
                return false;
            }
            set
            {
                this.linkVisitedSet = true;
                if (value != this.LinkVisited)
                {
                    this.linkVisited = value;
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

        private bool ShouldSerializeLinkVisited()
        {
            return this.linkVisitedSet = true;
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.TrackVisitedState"]/*' />
        [DefaultValue(true)]
        public bool TrackVisitedState
        {
            get
            {
                bool found;
                int trackVisitedState = this.Properties.GetInteger(PropLinkCellTrackVisitedState, out found);
                if (found)
                {
                    return trackVisitedState == 0 ? false : true;
                }
                return true;
            }
            set
            {
                if (value != this.TrackVisitedState)
                {
                    this.Properties.SetInteger(PropLinkCellTrackVisitedState, value ? 1 : 0);
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

        internal bool TrackVisitedStateInternal
        {
            set
            {
                if (value != this.TrackVisitedState)
                {
                    this.Properties.SetInteger(PropLinkCellTrackVisitedState, value ? 1 : 0);
                }
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.UseColumnTextForLinkValue"]/*' />
        [DefaultValue(false)]
        public bool UseColumnTextForLinkValue
        {
            get
            {
                bool found;
                int useColumnTextForLinkValue = this.Properties.GetInteger(PropLinkCellUseColumnTextForLinkValue, out found);
                if (found)
                {
                    return useColumnTextForLinkValue == 0 ? false : true;
                }
                return false;
            }
            set
            {
                if (value != this.UseColumnTextForLinkValue)
                {
                    this.Properties.SetInteger(PropLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
                    OnCommonChange();
                }
            }
        }

        internal bool UseColumnTextForLinkValueInternal
        {
            set
            {
                // Caller is responsible for invalidation
                if (value != this.UseColumnTextForLinkValue)
                {
                    this.Properties.SetInteger(PropLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
                }
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.VisitedLinkColor"]/*' />
        public Color VisitedLinkColor
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes") // unboxing OK here.
            ]
            get
            {
                if (this.Properties.ContainsObject(PropLinkCellVisitedLinkColor))
                {
                    return (Color)this.Properties.GetObject(PropLinkCellVisitedLinkColor);
                }
                else if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
                {
                    return this.Selected ? SystemColors.HighlightText : LinkUtilities.GetVisitedLinkColor();
                }
                else
                {
                    // return the default IE Color
                    return LinkUtilities.IEVisitedLinkColor;
                }
            }
            set
            {
                if (!value.Equals(this.VisitedLinkColor))
                {
                    this.Properties.SetObject(PropLinkCellVisitedLinkColor, value);
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

        internal Color VisitedLinkColorInternal
        {
            set
            {
                if (!value.Equals(this.VisitedLinkColor))
                {
                    this.Properties.SetObject(PropLinkCellVisitedLinkColor, value);
                }
            }
        }

        private bool ShouldSerializeVisitedLinkColor()
        {
            if (SystemInformation.HighContrast && AccessibilityImprovements.Level2)
            {
                return !this.VisitedLinkColor.Equals(SystemColors.HotTrack);
            }

            return !this.VisitedLinkColor.Equals(LinkUtilities.IEVisitedLinkColor);
        }

        private Color HighContrastLinkColor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // Selected cells have SystemColors.Highlight as a background.
                // SystemColors.HighlightText is supposed to be in contrast with SystemColors.Highlight.
                return this.Selected ? SystemColors.HighlightText : SystemColors.HotTrack;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.ValueType"]/*' />
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

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.Clone"]/*' />
        public override object Clone()
        {
            DataGridViewLinkCell dataGridViewCell;
            Type thisType = this.GetType ();

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

            if (this.Properties.ContainsObject(PropLinkCellActiveLinkColor))
            {
                dataGridViewCell.ActiveLinkColorInternal = this.ActiveLinkColor;
            }

            if (this.Properties.ContainsInteger(PropLinkCellUseColumnTextForLinkValue))
            {
                dataGridViewCell.UseColumnTextForLinkValueInternal = this.UseColumnTextForLinkValue;
            }

            if (this.Properties.ContainsInteger(PropLinkCellLinkBehavior))
            {
                dataGridViewCell.LinkBehaviorInternal = this.LinkBehavior;
            }

            if (this.Properties.ContainsObject(PropLinkCellLinkColor))
            {
                dataGridViewCell.LinkColorInternal = this.LinkColor;
            }

            if (this.Properties.ContainsInteger(PropLinkCellTrackVisitedState))
            {
                dataGridViewCell.TrackVisitedStateInternal = this.TrackVisitedState;
            }

            if (this.Properties.ContainsObject(PropLinkCellVisitedLinkColor))
            {
                dataGridViewCell.VisitedLinkColorInternal = this.VisitedLinkColor;
            }

            if (this.linkVisitedSet)
            {
                dataGridViewCell.LinkVisited = this.LinkVisited;
            }

            return dataGridViewCell;
        }

        private bool LinkBoundsContainPoint(int x, int y, int rowIndex)
        {
            Rectangle linkBounds = GetContentBounds(rowIndex);

            return linkBounds.Contains(x, y);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewLinkCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.GetContentBounds"]/*' />
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
            object formattedValue = this.GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.GetErrorIconBounds"]/*' />
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
            object formattedValue = this.GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.GetPreferredSize"]/*' />
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
            string formattedString = formattedValue as string;
            if (string.IsNullOrEmpty(formattedString))
            {
                formattedString = " ";
            }
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
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
                if (this.DataGridView.ShowCellErrors)
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
                if (this.DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.GetValue"]/*' />
        protected override object GetValue(int rowIndex)
        {
            if (this.UseColumnTextForLinkValue &&
                this.DataGridView != null && 
                this.DataGridView.NewRowIndex != rowIndex && 
                this.OwningColumn != null && 
                this.OwningColumn is DataGridViewLinkColumn)
            {
                return ((DataGridViewLinkColumn) this.OwningColumn).Text;
            }
            return base.GetValue(rowIndex);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.KeyUpUnsharesRow"]/*' />
        protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
        {
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                return this.TrackVisitedState && !this.LinkVisited;
            }
            else
            {
                return true;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.MouseDownUnsharesRow"]/*' />
        protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.MouseLeaveUnsharesRow"]/*' />
        protected override bool MouseLeaveUnsharesRow(int rowIndex)
        {
            return this.LinkState != LinkState.Normal;
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.MouseMoveUnsharesRow"]/*' />
        protected override bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                if ((this.LinkState & LinkState.Hover) == 0)
                {
                    return true;
                }
            }
            else
            {
                if ((this.LinkState & LinkState.Hover) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.MouseUpUnsharesRow"]/*' />
        protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
        {
            return this.TrackVisitedState && LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.OnKeyUp"]/*' />
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
            {
                RaiseCellClick(new DataGridViewCellEventArgs(this.ColumnIndex, rowIndex));
                if (this.DataGridView != null &&
                    this.ColumnIndex < this.DataGridView.Columns.Count &&
                    rowIndex < this.DataGridView.Rows.Count)
                {
                    RaiseCellContentClick(new DataGridViewCellEventArgs(this.ColumnIndex, rowIndex));
                    if (this.TrackVisitedState)
                    {
                        this.LinkVisited = true;
                    }
                }
                e.Handled = true;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.OnMouseDown"]/*' />
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                this.LinkState |= LinkState.Active;
                this.DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
            }

            base.OnMouseDown(e);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.OnMouseLeave"]/*' />
        protected override void OnMouseLeave(int rowIndex)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (dataGridViewCursor != null)
            {
                this.DataGridView.Cursor = dataGridViewCursor;
                dataGridViewCursor = null;
            }
            if (this.LinkState != LinkState.Normal)
            {
                this.LinkState = LinkState.Normal;
                this.DataGridView.InvalidateCell(this.ColumnIndex, rowIndex);
            }

            base.OnMouseLeave(rowIndex);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.OnMouseMove"]/*' />
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex))
            {
                if ((this.LinkState & LinkState.Hover) == 0)
                {
                    this.LinkState |= LinkState.Hover;
                    this.DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
                }

                if (dataGridViewCursor == null)
                {
                    dataGridViewCursor = this.DataGridView.UserSetCursor;
                }

                if (this.DataGridView.Cursor != Cursors.Hand)
                {
                    this.DataGridView.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if ((this.LinkState & LinkState.Hover) != 0)
                {
                    this.LinkState &= ~LinkState.Hover;
                    this.DataGridView.Cursor = dataGridViewCursor;
                    this.DataGridView.InvalidateCell(this.ColumnIndex, e.RowIndex);
                }
            }

            base.OnMouseMove(e);
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.OnMouseUp"]/*' />
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex) && this.TrackVisitedState)
            {
                this.LinkVisited = true;
            }
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.Paint"]/*' />
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

            Point ptCurrentCell = this.DataGridView.CurrentCellAddress;
            bool cellCurrent = ptCurrentCell.X == this.ColumnIndex && ptCurrentCell.Y == rowIndex;
            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
            SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255)
            {
                g.FillRectangle(br, valBounds);
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

            Rectangle errorBounds = valBounds;
            string formattedValueStr = formattedValue as string;

            if (formattedValueStr != null && (paint || computeContentBounds))
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
                LinkUtilities.EnsureLinkFonts(cellStyle.Font, this.LinkBehavior, ref linkFont, ref hoverFont);
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                // paint the focus rectangle around the link
                if (paint)
                {
                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {
                        if (cellCurrent &&
                            this.DataGridView.ShowFocusCues &&
                            this.DataGridView.Focused &&
                            DataGridViewCell.PaintFocus(paintParts))
                        {
                            Rectangle focusBounds = DataGridViewUtilities.GetTextBounds(valBounds,
                                                                                        formattedValueStr,
                                                                                        flags,
                                                                                        cellStyle,
                                                                                        this.LinkState == LinkState.Hover ? hoverFont : linkFont);
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
                        if ((this.LinkState & LinkState.Active) == LinkState.Active)
                        {
                            linkColor = this.ActiveLinkColor;
                        }
                        else if (this.LinkVisited)
                        {
                            linkColor = this.VisitedLinkColor;
                        }
                        else
                        {
                            linkColor = this.LinkColor;
                        }
                        if (DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            if ((flags & TextFormatFlags.SingleLine) != 0)
                            {
                                flags |= TextFormatFlags.EndEllipsis;
                            }
                            TextRenderer.DrawText(g,
                                                  formattedValueStr,
                                                  this.LinkState == LinkState.Hover ? hoverFont : linkFont,
                                                  valBounds,
                                                  linkColor,
                                                  flags);
                        }
                    }
                    else if (cellCurrent &&
                             this.DataGridView.ShowFocusCues &&
                             this.DataGridView.Focused &&
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
                                                                       this.LinkState == LinkState.Hover ? hoverFont : linkFont);
                }
                linkFont.Dispose();
                hoverFont.Dispose();
            }
            else if (paint || computeContentBounds)
            {
                if (cellCurrent &&
                    this.DataGridView.ShowFocusCues && 
                    this.DataGridView.Focused && 
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
                if (!String.IsNullOrEmpty(errorText))
                {
                    resultBounds = ComputeErrorIconBounds(errorBounds);
                }
            }

            if (this.DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCell.ToString"]/*' />
        public override string ToString()
        {
            return "DataGridViewLinkCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCellAccessibleObject"]/*' />
        protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
        {

            /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCellAccessibleObject.DataGridViewLinkCellAccessibleObject"]/*' />
            public DataGridViewLinkCellAccessibleObject(DataGridViewCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    return string.Format(SR.DataGridView_AccLinkCellDefaultAction);
                }
            }

            /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                DataGridViewLinkCell dataGridViewCell = (DataGridViewLinkCell)this.Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationOnSharedCell));
                }

                if (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            /// <include file='doc\DataGridViewLinkCell.uex' path='docs/doc[@for="DataGridViewLinkCellAccessibleObject.GetChildCount"]/*' />
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
                    return NativeMethods.UIA_HyperlinkControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
