// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

public partial class DataGridViewLinkCell : DataGridViewCell
{
    private const DataGridViewContentAlignment AnyLeft = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.MiddleLeft | DataGridViewContentAlignment.BottomLeft;
    private const DataGridViewContentAlignment AnyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
    private const DataGridViewContentAlignment AnyBottom = DataGridViewContentAlignment.BottomRight | DataGridViewContentAlignment.BottomCenter | DataGridViewContentAlignment.BottomLeft;

    private static readonly Type s_defaultFormattedValueType = typeof(string);
    private static readonly Type s_defaultValueType = typeof(object);
    private static readonly Type s_cellType = typeof(DataGridViewLinkCell);

    private static readonly int s_propLinkCellActiveLinkColor = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellLinkBehavior = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellLinkColor = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellLinkState = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellTrackVisitedState = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellUseColumnTextForLinkValue = PropertyStore.CreateKey();
    private static readonly int s_propLinkCellVisitedLinkColor = PropertyStore.CreateKey();

    private const byte HorizontalTextMarginLeft = 1;
    private const byte HorizontalTextMarginRight = 2;
    private const byte VerticalTextMarginTop = 1;
    private const byte VerticalTextMarginBottom = 1;

    // we cache LinkVisited because it will be set multiple times
    private bool _linkVisited;
    private bool _linkVisitedSet;

    private static Cursor? s_dataGridViewCursor;

    public DataGridViewLinkCell()
    {
    }

    public Color ActiveLinkColor
    {
        get
        {
            if (Properties.TryGetObject(s_propLinkCellActiveLinkColor, out Color color))
            {
                return color;
            }
            else if (SystemInformation.HighContrast)
            {
                return HighContrastLinkColor;
            }
            else
            {
                // return the default IE Color if cell is not not selected
                return Selected ? SystemColors.HighlightText : LinkUtilities.IEActiveLinkColor;
            }
        }
        set
        {
            if (!value.Equals(ActiveLinkColor))
            {
                Properties.SetObject(s_propLinkCellActiveLinkColor, value);
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

    internal Color ActiveLinkColorInternal
    {
        set
        {
            if (!value.Equals(ActiveLinkColor))
            {
                Properties.SetObject(s_propLinkCellActiveLinkColor, value);
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

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]
    public override Type? EditType
    {
        get
        {
            // links can't switch to edit mode
            return null;
        }
    }

    public override Type? FormattedValueType => s_defaultFormattedValueType;

    [DefaultValue(LinkBehavior.SystemDefault)]
    public LinkBehavior LinkBehavior
    {
        get
        {
            int linkBehavior = Properties.GetInteger(s_propLinkCellLinkBehavior, out bool found);
            if (found)
            {
                return (LinkBehavior)linkBehavior;
            }

            return LinkBehavior.SystemDefault;
        }
        set
        {
            // Sequential enum.  Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (value != LinkBehavior)
            {
                Properties.SetInteger(s_propLinkCellLinkBehavior, (int)value);
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

    internal LinkBehavior LinkBehaviorInternal
    {
        set
        {
            Debug.Assert(value is >= LinkBehavior.SystemDefault and <= LinkBehavior.NeverUnderline);
            if (value != LinkBehavior)
            {
                Properties.SetInteger(s_propLinkCellLinkBehavior, (int)value);
            }
        }
    }

    public Color LinkColor
    {
        get
        {
            if (Properties.TryGetObject(s_propLinkCellLinkColor, out Color color))
            {
                return color;
            }
            else if (SystemInformation.HighContrast)
            {
                return HighContrastLinkColor;
            }
            else
            {
                // return the default IE Color when cell is not selected
                return Selected ? SystemColors.HighlightText : LinkUtilities.IELinkColor;
            }
        }
        set
        {
            if (!value.Equals(LinkColor))
            {
                Properties.SetObject(s_propLinkCellLinkColor, value);
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

    internal Color LinkColorInternal
    {
        set
        {
            if (!value.Equals(LinkColor))
            {
                Properties.SetObject(s_propLinkCellLinkColor, value);
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
            int linkState = Properties.GetInteger(s_propLinkCellLinkState, out bool found);
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
                Properties.SetInteger(s_propLinkCellLinkState, (int)value);
            }
        }
    }

    public bool LinkVisited
    {
        get
        {
            if (_linkVisitedSet)
            {
                return _linkVisited;
            }

            // the default is false
            return false;
        }
        set
        {
            _linkVisitedSet = true;
            if (value != LinkVisited)
            {
                _linkVisited = value;
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

    private bool ShouldSerializeLinkVisited() => _linkVisitedSet = true;

    [DefaultValue(true)]
    public bool TrackVisitedState
    {
        get
        {
            int trackVisitedState = Properties.GetInteger(s_propLinkCellTrackVisitedState, out bool found);
            return !found || trackVisitedState != 0;
        }
        set
        {
            if (value != TrackVisitedState)
            {
                Properties.SetInteger(s_propLinkCellTrackVisitedState, value ? 1 : 0);
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

    internal bool TrackVisitedStateInternal
    {
        set
        {
            if (value != TrackVisitedState)
            {
                Properties.SetInteger(s_propLinkCellTrackVisitedState, value ? 1 : 0);
            }
        }
    }

    [DefaultValue(false)]
    public bool UseColumnTextForLinkValue
    {
        get
        {
            int useColumnTextForLinkValue = Properties.GetInteger(s_propLinkCellUseColumnTextForLinkValue, out bool found);
            return found && useColumnTextForLinkValue != 0;
        }
        set
        {
            if (value != UseColumnTextForLinkValue)
            {
                Properties.SetInteger(s_propLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
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
                Properties.SetInteger(s_propLinkCellUseColumnTextForLinkValue, value ? 1 : 0);
            }
        }
    }

    public Color VisitedLinkColor
    {
        get
        {
            if (Properties.TryGetObject(s_propLinkCellVisitedLinkColor, out Color color))
            {
                return color;
            }
            else if (SystemInformation.HighContrast)
            {
                return Selected ? SystemColors.HighlightText : LinkUtilities.GetVisitedLinkColor();
            }
            else
            {
                // return the default IE Color if cell is not not selected
                return Selected ? SystemColors.HighlightText : LinkUtilities.IEVisitedLinkColor;
            }
        }
        set
        {
            if (!value.Equals(VisitedLinkColor))
            {
                Properties.SetObject(s_propLinkCellVisitedLinkColor, value);
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

    internal Color VisitedLinkColorInternal
    {
        set
        {
            if (!value.Equals(VisitedLinkColor))
            {
                Properties.SetObject(s_propLinkCellVisitedLinkColor, value);
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
            // Selected cells have Application.SystemColors.Highlight as a background.
            // Application.SystemColors.HighlightText is supposed to be in contrast with Application.SystemColors.Highlight.
            return Selected ? SystemColors.HighlightText : SystemColors.HotTrack;
        }
    }

    public override Type ValueType
    {
        get
        {
            Type? valueType = base.ValueType;
            if (valueType is not null)
            {
                return valueType;
            }

            return s_defaultValueType;
        }
    }

    public override object Clone()
    {
        DataGridViewLinkCell dataGridViewCell;
        Type thisType = GetType();

        if (thisType == s_cellType) // performance improvement
        {
            dataGridViewCell = new DataGridViewLinkCell();
        }
        else
        {
            dataGridViewCell = (DataGridViewLinkCell)Activator.CreateInstance(thisType)!;
        }

        CloneInternal(dataGridViewCell);

        if (Properties.ContainsObject(s_propLinkCellActiveLinkColor))
        {
            dataGridViewCell.ActiveLinkColorInternal = ActiveLinkColor;
        }

        if (Properties.ContainsInteger(s_propLinkCellUseColumnTextForLinkValue))
        {
            dataGridViewCell.UseColumnTextForLinkValueInternal = UseColumnTextForLinkValue;
        }

        if (Properties.ContainsInteger(s_propLinkCellLinkBehavior))
        {
            dataGridViewCell.LinkBehaviorInternal = LinkBehavior;
        }

        if (Properties.ContainsObject(s_propLinkCellLinkColor))
        {
            dataGridViewCell.LinkColorInternal = LinkColor;
        }

        if (Properties.ContainsInteger(s_propLinkCellTrackVisitedState))
        {
            dataGridViewCell.TrackVisitedStateInternal = TrackVisitedState;
        }

        if (Properties.ContainsObject(s_propLinkCellVisitedLinkColor))
        {
            dataGridViewCell.VisitedLinkColorInternal = VisitedLinkColor;
        }

        if (_linkVisitedSet)
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

        Rectangle linkBounds = PaintPrivate(
            graphics,
            cellBounds,
            cellBounds,
            rowIndex,
            cellState,
            formattedValue,
            errorText: null,    // linkBounds is independent of errorText
            cellStyle,
            dgvabsEffective,
            DataGridViewPaintParts.ContentForeground,
            computeContentBounds: true,
            computeErrorIconBounds: false,
            paint: false);

#if DEBUG
        Rectangle linkBoundsDebug = PaintPrivate(
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
        Debug.Assert(linkBoundsDebug.Equals(linkBounds));
#endif

        return linkBounds;
    }

    private protected override string? GetDefaultToolTipText()
    {
        if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
        {
            return SR.DefaultDataGridViewLinkCellTollTipText;
        }

        return null;
    }

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

        Rectangle errorIconBounds = PaintPrivate(
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

        return errorIconBounds;
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
        string? formattedString = formattedValue as string;
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
                        int maxHeight = constraintSize.Height - borderAndPaddingHeights - VerticalTextMarginTop - VerticalTextMarginBottom;
                        if ((cellStyle.Alignment & AnyBottom) != 0)
                        {
                            maxHeight--;
                        }

                        preferredSize = new Size(
                            MeasureTextWidth(
                                graphics,
                                formattedString,
                                cellStyle.Font,
                                Math.Max(1, maxHeight),
                                flags),
                            0);
                        break;
                    }

                case DataGridViewFreeDimension.Height:
                    {
                        preferredSize = new Size(
                            0,
                            MeasureTextHeight(
                                graphics,
                                formattedString,
                                cellStyle.Font,
                                Math.Max(1, constraintSize.Width - borderAndPaddingWidths - HorizontalTextMarginLeft - HorizontalTextMarginRight),
                                flags));
                        break;
                    }

                default:
                    {
                        preferredSize = MeasureTextPreferredSize(
                            graphics,
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
                        preferredSize = new Size(
                            MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Width,
                            0);
                        break;
                    }

                case DataGridViewFreeDimension.Height:
                    {
                        preferredSize = new Size(
                            0,
                            MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Height);
                        break;
                    }

                default:
                    {
                        preferredSize = MeasureTextSize(
                            graphics,
                            formattedString,
                            cellStyle.Font,
                            flags);
                        break;
                    }
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Height)
        {
            preferredSize.Width += HorizontalTextMarginLeft + HorizontalTextMarginRight + borderAndPaddingWidths;
            if (DataGridView.ShowCellErrors)
            {
                // Making sure that there is enough room for the potential error icon
                preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + IconMarginWidth * 2 + s_iconsWidth);
            }
        }

        if (freeDimension != DataGridViewFreeDimension.Width)
        {
            preferredSize.Height += VerticalTextMarginTop + VerticalTextMarginBottom + borderAndPaddingHeights;
            if ((cellStyle.Alignment & AnyBottom) != 0)
            {
                preferredSize.Height += VerticalTextMarginBottom;
            }

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
        if (UseColumnTextForLinkValue &&
            DataGridView is not null &&
            DataGridView.NewRowIndex != rowIndex &&
            OwningColumn is DataGridViewLinkColumn dataGridViewLinkColumn)
        {
            return dataGridViewLinkColumn.Text;
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

    protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) =>
        LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);

    protected override bool MouseLeaveUnsharesRow(int rowIndex) =>
        LinkState != LinkState.Normal;

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

    protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) =>
        TrackVisitedState && LinkBoundsContainPoint(e.X, e.Y, e.RowIndex);

    protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (e.KeyCode == Keys.Space && !e.Alt && !e.Control && !e.Shift)
        {
            RaiseCellClick(new DataGridViewCellEventArgs(ColumnIndex, rowIndex));
            if (DataGridView is not null &&
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
        if (DataGridView is null)
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
        if (DataGridView is null)
        {
            return;
        }

        if (s_dataGridViewCursor is not null)
        {
            DataGridView.Cursor = s_dataGridViewCursor;
            s_dataGridViewCursor = null;
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
        if (DataGridView is null)
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

            s_dataGridViewCursor ??= DataGridView.UserSetCursor;

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
                DataGridView.Cursor = s_dataGridViewCursor;
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
            }
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
    {
        if (DataGridView is null)
        {
            return;
        }

        if (LinkBoundsContainPoint(e.X, e.Y, e.RowIndex) && TrackVisitedState)
        {
            LinkVisited = true;
        }
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
        Graphics g,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
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

        if (paint && PaintBorder(paintParts))
        {
            PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
        }

        Rectangle resultBounds = Rectangle.Empty;

        Rectangle borderWidths = BorderWidths(advancedBorderStyle);
        Rectangle valBounds = cellBounds;
        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;

        Point ptCurrentCell = DataGridView!.CurrentCellAddress;
        bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
        bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
        Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
            ? cellStyle.SelectionBackColor
            : cellStyle.BackColor;

        if (paint && PaintBackground(paintParts) && !brushColor.HasTransparency())
        {
            using var brush = brushColor.GetCachedSolidBrushScope();
            g.FillRectangle(brush, valBounds);
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
            valBounds.Offset(HorizontalTextMarginLeft, VerticalTextMarginTop);
            valBounds.Width -= HorizontalTextMarginLeft + HorizontalTextMarginRight;
            valBounds.Height -= VerticalTextMarginTop + VerticalTextMarginBottom;
            if ((cellStyle.Alignment & AnyBottom) != 0)
            {
                valBounds.Height -= VerticalTextMarginBottom;
            }

            Font? getLinkFont = null;
            Font? getHoverFont = null;
            bool isActive = (LinkState & LinkState.Active) == LinkState.Active;

            LinkUtilities.EnsureLinkFonts(cellStyle.Font, LinkBehavior, ref getLinkFont, ref getHoverFont, isActive);
            using Font linkFont = getLinkFont;
            using Font hoverFont = getHoverFont;

            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(
                DataGridView.RightToLeftInternal,
                cellStyle.Alignment,
                cellStyle.WrapMode);

            // Paint the focus rectangle around the link
            if (!paint)
            {
                Debug.Assert(computeContentBounds);
                resultBounds = DataGridViewUtilities.GetTextBounds(
                    valBounds,
                    formattedValueStr,
                    flags,
                    cellStyle,
                    LinkState == LinkState.Hover ? hoverFont : linkFont);
            }
            else
            {
                if (valBounds.Width > 0 && valBounds.Height > 0)
                {
                    if (cellCurrent &&
                        DataGridView.ShowFocusCues &&
                        DataGridView.Focused &&
                        PaintFocus(paintParts))
                    {
                        Rectangle focusBounds = DataGridViewUtilities.GetTextBounds(
                            valBounds,
                            formattedValueStr,
                            flags,
                            cellStyle,
                            LinkState == LinkState.Hover ? hoverFont : linkFont);

                        if ((cellStyle.Alignment & AnyLeft) != 0)
                        {
                            focusBounds.X--;
                            focusBounds.Width++;
                        }
                        else if ((cellStyle.Alignment & AnyRight) != 0)
                        {
                            focusBounds.X++;
                            focusBounds.Width++;
                        }

                        focusBounds.Height += 2;
                        ControlPaint.DrawFocusRectangle(g, focusBounds, Color.Empty, brushColor);
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

                    if (PaintContentForeground(paintParts))
                    {
                        if ((flags & TextFormatFlags.SingleLine) != 0)
                        {
                            flags |= TextFormatFlags.EndEllipsis;
                        }

                        TextRenderer.DrawText(
                            g,
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
                    PaintFocus(paintParts) &&
                    errorBounds.Width > 0 &&
                    errorBounds.Height > 0)
                {
                    // Draw focus rectangle
                    ControlPaint.DrawFocusRectangle(g, errorBounds, Color.Empty, brushColor);
                }
            }
        }
        else if (paint || computeContentBounds)
        {
            if (cellCurrent &&
                DataGridView.ShowFocusCues &&
                DataGridView.Focused &&
                PaintFocus(paintParts) &&
                paint &&
                valBounds.Width > 0 &&
                valBounds.Height > 0)
            {
                // Draw focus rectangle
                ControlPaint.DrawFocusRectangle(g, valBounds, Color.Empty, brushColor);
            }
        }
        else if (computeErrorIconBounds)
        {
            if (!string.IsNullOrEmpty(errorText))
            {
                resultBounds = ComputeErrorIconBounds(errorBounds);
            }
        }

        if (DataGridView.ShowCellErrors && paint && PaintErrorIcon(paintParts))
        {
            PaintErrorIcon(g, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
        }

        return resultBounds;
    }

    public override string ToString() =>
        $"DataGridViewLinkCell {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";
}
