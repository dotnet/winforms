// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[SRDescription(nameof(SR.DescriptionStatusStrip))]
public partial class StatusStrip : ToolStrip
{
    private const AnchorStyles AllAnchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
    private const AnchorStyles HorizontalAnchor = AnchorStyles.Left | AnchorStyles.Right;
    private const AnchorStyles VerticalAnchor = AnchorStyles.Top | AnchorStyles.Bottom;

    private BitVector32 _state;

    private static readonly int s_stateSizingGrip = BitVector32.CreateMask();
    private static readonly int s_stateCalledSpringTableLayout = BitVector32.CreateMask(s_stateSizingGrip);

    private const int GripWidth = 12;
    private RightToLeftLayoutGrip? _rtlLayoutGrip;
    private Orientation _lastOrientation = Orientation.Horizontal;

    public StatusStrip()
    {
        SuspendLayout();
        CanOverflow = false;
        LayoutStyle = ToolStripLayoutStyle.Table;

        // Default changed for DarkMode from System to ManagerRenderMode.
        // Also to be consistent to the MenuStrip.
        // TODO: We'd need to quirk that!
        RenderMode = ToolStripRenderMode.ManagerRenderMode;
        GripStyle = ToolStripGripStyle.Hidden;

        SetStyle(ControlStyles.ResizeRedraw, true);
        Stretch = true;
        _state[s_stateSizingGrip] = true;
        ResumeLayout(true);
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripCanOverflowDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    public new bool CanOverflow
    {
        get => base.CanOverflow;
        set => base.CanOverflow = value;
    }

    protected override bool DefaultShowItemToolTips
    {
        get
        {
            return false;
        }
    }

    protected override Size DefaultSize
    {
        get
        {
            return new Size(200, 22);
        }
    }

    protected override Padding DefaultPadding
    {
        get
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (RightToLeft == RightToLeft.No)
                {
                    return new Padding(1, 0, 14, 0);
                }
                else
                {
                    return new Padding(14, 0, 1, 0);
                }
            }
            else
            {
                // vertical
                // the difference in symmetry here is that the grip does not actually rotate, it remains the same height it
                // was before, so the DisplayRectangle needs to shrink up by its height.
                return new Padding(1, 3, 1, DefaultSize.Height);
            }
        }
    }

    protected override DockStyle DefaultDock
    {
        get
        {
            return DockStyle.Bottom;
        }
    }

    [DefaultValue(DockStyle.Bottom)]
    public override DockStyle Dock
    {
        get => base.Dock;
        set => base.Dock = value;
    }

    [DefaultValue(ToolStripGripStyle.Hidden)]
    public new ToolStripGripStyle GripStyle
    {
        get => base.GripStyle;
        set => base.GripStyle = value;
    }

    [DefaultValue(ToolStripLayoutStyle.Table)]
    public new ToolStripLayoutStyle LayoutStyle
    {
        get => base.LayoutStyle;
        set => base.LayoutStyle = value;
    }

    // we do some custom stuff with padding to accomodate size grip.
    // changing this is not supported at DT
    [Browsable(false)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    private Control RTLGrip
    {
        get
        {
            _rtlLayoutGrip ??= new RightToLeftLayoutGrip();

            return _rtlLayoutGrip;
        }
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripShowItemToolTipsDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    public new bool ShowItemToolTips
    {
        get => base.ShowItemToolTips;
        set => base.ShowItemToolTips = value;
    }

    // return whether we should paint the sizing grip.
    private bool ShowSizingGrip
    {
        get
        {
            if (SizingGrip && IsHandleCreated)
            {
                if (DesignMode)
                {
                    return true;  // we don't care about the state of VS.
                }

                HWND rootHwnd = PInvoke.GetAncestor(this, GET_ANCESTOR_FLAGS.GA_ROOT);
                if (!rootHwnd.IsNull)
                {
                    return !PInvoke.IsZoomed(rootHwnd);
                }
            }

            return false;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.StatusStripSizingGripDescr))]
    public bool SizingGrip
    {
        get
        {
            return _state[s_stateSizingGrip];
        }
        set
        {
            if (value != _state[s_stateSizingGrip])
            {
                _state[s_stateSizingGrip] = value;
                EnsureRightToLeftGrip();
                Invalidate(true);
            }
        }
    }

    [Browsable(false)]
    public Rectangle SizeGripBounds
    {
        get
        {
            if (SizingGrip)
            {
                Size statusStripSize = Size;
                // we can't necessarily make this the height of the status strip, as
                // the orientation could change.
                int gripHeight = Math.Min(DefaultSize.Height, statusStripSize.Height);

                if (RightToLeft == RightToLeft.Yes)
                {
                    return new Rectangle(0, statusStripSize.Height - gripHeight, GripWidth, gripHeight);
                }
                else
                {
                    return new Rectangle(statusStripSize.Width - GripWidth, statusStripSize.Height - gripHeight, GripWidth, gripHeight);
                }
            }

            return Rectangle.Empty;
        }
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ToolStripStretchDescr))]
    public new bool Stretch
    {
        get => base.Stretch;
        set => base.Stretch = value;
    }

    private TableLayoutSettings TableLayoutSettings
    {
        get { return (TableLayoutSettings)LayoutSettings!; }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new StatusStripAccessibleObject(this);
    }

    protected internal override ToolStripItem CreateDefaultItem(string? text, Image? image, EventHandler? onClick)
    {
        return new ToolStripStatusLabel(text, image, onClick);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_rtlLayoutGrip is not null)
            {
                _rtlLayoutGrip.Dispose();
                _rtlLayoutGrip = null;
            }
        }

        base.Dispose(disposing);
    }

    // in RTL, we parent a transparent control over the grip to support mirroring.
    private void EnsureRightToLeftGrip()
    {
        if (SizingGrip && RightToLeft == RightToLeft.Yes)
        {
            RTLGrip.Bounds = SizeGripBounds;
            if (!Controls.Contains(RTLGrip))
            {
                if (Controls is ReadOnlyControlCollection controlCollection)
                {
                    controlCollection.AddInternal(RTLGrip);
                }
            }
        }
        else if (_rtlLayoutGrip is not null)
        {
            if (Controls.Contains(_rtlLayoutGrip))
            {
                if (Controls is ReadOnlyControlCollection controlCollection)
                {
                    controlCollection.RemoveInternal(_rtlLayoutGrip);
                }

                _rtlLayoutGrip.Dispose();
                _rtlLayoutGrip = null;
            }
        }
    }

    internal override Size GetPreferredSizeCore(Size proposedSize)
    {
        if (LayoutStyle == ToolStripLayoutStyle.Table)
        {
            if (proposedSize.Width == 1)
            {
                proposedSize.Width = int.MaxValue;
            }

            if (proposedSize.Height == 1)
            {
                proposedSize.Height = int.MaxValue;
            }

            if (Orientation == Orientation.Horizontal)
            {
                return GetPreferredSizeHorizontal(this, proposedSize) + Padding.Size;
            }
            else
            {
                return GetPreferredSizeVertical(this) + Padding.Size;
            }
        }

        return base.GetPreferredSizeCore(proposedSize);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        if (ShowSizingGrip)
        {
            Renderer.DrawStatusStripSizingGrip(new ToolStripRenderEventArgs(e.Graphics, this));
        }
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        _state[s_stateCalledSpringTableLayout] = false;
        bool inDisplayedItemCollection = false;
        ToolStripItem? item = levent.AffectedComponent as ToolStripItem;
        int itemCount = DisplayedItems.Count;
        if (item is not null)
        {
            inDisplayedItemCollection = DisplayedItems.Contains(item);
        }

        if (LayoutStyle == ToolStripLayoutStyle.Table)
        {
            OnSpringTableLayoutCore();
        }

        base.OnLayout(levent);

        if (itemCount != DisplayedItems.Count || (item is not null && (inDisplayedItemCollection != DisplayedItems.Contains(item))))
        {
            // calling OnLayout has changed the displayed items collection
            // the SpringTableLayoutCore requires the count of displayed items to
            // be accurate.
            // - so we need to perform layout again.
            if (LayoutStyle == ToolStripLayoutStyle.Table)
            {
                OnSpringTableLayoutCore();
                base.OnLayout(levent);
            }
        }

        EnsureRightToLeftGrip();
    }

    internal override bool SupportsUiaProviders => true;

    protected override void SetDisplayedItems()
    {
        if (_state[s_stateCalledSpringTableLayout])
        {
            // shove all items that don't fit one pixel outside the displayed region
            Rectangle displayRect = DisplayRectangle;
            Point noMansLand = displayRect.Location;
            noMansLand.X += ClientSize.Width + 1;
            noMansLand.Y += ClientSize.Height + 1;
            bool overflow = false;
            Rectangle lastItemBounds = Rectangle.Empty;

            ToolStripItem? lastItem = null;
            for (int i = 0; i < Items.Count; i++)
            {
                ToolStripItem item = Items[i];

                // using spring layout we can get into a situation where there's extra items which arent
                // visible.
                if (overflow || ((IArrangedElement)item).ParticipatesInLayout)
                {
                    if (overflow || (SizingGrip && item.Bounds.IntersectsWith(SizeGripBounds)))
                    {
                        // if the item collides with the size grip, set the location to nomansland.
                        SetItemLocation(item, noMansLand);
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }
                }
                else if (lastItem is not null && (lastItemBounds.IntersectsWith(item.Bounds)))
                {
                    // if it overlaps the previous element, set the location to nomansland.
                    SetItemLocation(item, noMansLand);
                    item.SetPlacement(ToolStripItemPlacement.None);
                }
                else if (item.Bounds.Width == 1)
                {
                    if (item is ToolStripStatusLabel panel && panel.Spring)
                    {
                        // once we get down to one pixel, there can always be a one pixel
                        // distribution problem with the TLP - there's usually a spare one around.
                        // so set this off to nomansland as well.
                        SetItemLocation(item, noMansLand);
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }
                }

                if (item.Bounds.Location != noMansLand)
                {
                    // set the next item to inspect for collisions
                    lastItem = item;
                    lastItemBounds = lastItem.Bounds;
                }
                else
                {
                    // we can't fit an item, everything else after it should not be displayed
                    if (((IArrangedElement)item).ParticipatesInLayout)
                    {
                        overflow = true;
                    }
                }
            }
        }

        base.SetDisplayedItems();
    }

    internal override void ResetRenderMode()
    {
        RenderMode = ToolStripRenderMode.System;
    }

    internal override bool ShouldSerializeRenderMode()
    {
        // We should NEVER serialize custom.
        return (RenderMode is not ToolStripRenderMode.System and not ToolStripRenderMode.Custom);
    }

    /// <summary>
    ///  Override this function if you want to do custom table layouts for the
    ///  StatusStrip.  The default layoutstyle is tablelayout, and we need to play
    ///  with the row/column styles
    /// </summary>
    protected virtual void OnSpringTableLayoutCore()
    {
        if (LayoutStyle == ToolStripLayoutStyle.Table)
        {
            _state[s_stateCalledSpringTableLayout] = true;

            SuspendLayout();

            if (_lastOrientation != Orientation)
            {
                TableLayoutSettings settings = TableLayoutSettings;
                settings.RowCount = 0;
                settings.ColumnCount = 0;
                settings.ColumnStyles.Clear();
                settings.RowStyles.Clear();
            }

            _lastOrientation = Orientation;

            if (Orientation == Orientation.Horizontal)
            {
                //
                // Horizontal layout
                //
                TableLayoutSettings.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;

                int originalColumnCount = TableLayoutSettings.ColumnStyles.Count;

                // iterate through the elements which are going to be displayed.
                for (int i = 0; i < DisplayedItems.Count; i++)
                {
                    if (i >= originalColumnCount)
                    {
                        // add if it's necessary.
                        TableLayoutSettings.ColumnStyles.Add(new ColumnStyle());
                    }

                    // determine if we "spring" or "autosize" the column
                    bool spring = (DisplayedItems[i] is ToolStripStatusLabel panel && panel.Spring);
                    DisplayedItems[i].Anchor = (spring) ? AllAnchor : VerticalAnchor;

                    // spring is achieved by using 100% as the column style
                    ColumnStyle colStyle = TableLayoutSettings.ColumnStyles[i];
                    colStyle.Width = 100; // this width is ignored in AutoSize.
                    colStyle.SizeType = (spring) ? SizeType.Percent : SizeType.AutoSize;
                }

                if (TableLayoutSettings.RowStyles.Count is > 1 or 0)
                {
                    TableLayoutSettings.RowStyles.Clear();
                    TableLayoutSettings.RowStyles.Add(new RowStyle());
                }

                TableLayoutSettings.RowCount = 1;

                TableLayoutSettings.RowStyles[0].SizeType = SizeType.Absolute;
                TableLayoutSettings.RowStyles[0].Height = Math.Max(0, DisplayRectangle.Height);
                TableLayoutSettings.ColumnCount = DisplayedItems.Count + 1; // add an extra cell so it fills the remaining space

                // don't remove the extra column styles, just set them back to autosize.
                for (int i = DisplayedItems.Count; i < TableLayoutSettings.ColumnStyles.Count; i++)
                {
                    TableLayoutSettings.ColumnStyles[i].SizeType = SizeType.AutoSize;
                }
            }
            else
            {
                //
                // Vertical layout
                //

                TableLayoutSettings.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

                int originalRowCount = TableLayoutSettings.RowStyles.Count;

                // iterate through the elements which are going to be displayed.
                for (int i = 0; i < DisplayedItems.Count; i++)
                {
                    if (i >= originalRowCount)
                    {
                        // add if it's necessary.
                        TableLayoutSettings.RowStyles.Add(new RowStyle());
                    }

                    // determine if we "spring" or "autosize" the row
                    bool spring = (DisplayedItems[i] is ToolStripStatusLabel panel && panel.Spring);
                    DisplayedItems[i].Anchor = (spring) ? AllAnchor : HorizontalAnchor;

                    // spring is achieved by using 100% as the row style
                    RowStyle rowStyle = TableLayoutSettings.RowStyles[i];
                    rowStyle.Height = 100; // this width is ignored in AutoSize.
                    rowStyle.SizeType = (spring) ? SizeType.Percent : SizeType.AutoSize;
                }

                TableLayoutSettings.ColumnCount = 1;

                if (TableLayoutSettings.ColumnStyles.Count is > 1 or 0)
                {
                    TableLayoutSettings.ColumnStyles.Clear();
                    TableLayoutSettings.ColumnStyles.Add(new ColumnStyle());
                }

                TableLayoutSettings.ColumnCount = 1;
                TableLayoutSettings.ColumnStyles[0].SizeType = SizeType.Absolute;
                TableLayoutSettings.ColumnStyles[0].Width = Math.Max(0, DisplayRectangle.Width);

                TableLayoutSettings.RowCount = DisplayedItems.Count + 1; // add an extra cell so it fills the remaining space

                // don't remove the extra column styles, just set them back to autosize.
                for (int i = DisplayedItems.Count; i < TableLayoutSettings.RowStyles.Count; i++)
                {
                    TableLayoutSettings.RowStyles[i].SizeType = SizeType.AutoSize;
                }
            }

            ResumeLayout(false);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if ((m.Msg == (int)PInvoke.WM_NCHITTEST) && SizingGrip)
        {
            // if we're within the grip bounds tell windows
            // that we're the bottom right of the window.
            Rectangle sizeGripBounds = SizeGripBounds;

            if (sizeGripBounds.Contains(PointToClient(PARAM.ToPoint(m.LParamInternal))))
            {
                HWND rootHwnd = PInvoke.GetAncestor(this, GET_ANCESTOR_FLAGS.GA_ROOT);

                // if the main window isn't maximized - we should paint a resize grip.
                // double check that we're at the bottom right hand corner of the window.
                if (!rootHwnd.IsNull && !PInvoke.IsZoomed(rootHwnd))
                {
                    // get the client area of the topmost window.  If we're next to the edge then
                    // the sizing grip is valid.
                    PInvokeCore.GetClientRect(rootHwnd, out RECT rootHwndClientArea);

                    // map the size grip FROM statusStrip coords TO the toplevel window coords.
                    Point gripLocation;
                    if (RightToLeft == RightToLeft.Yes)
                    {
                        gripLocation = new Point(SizeGripBounds.Left, SizeGripBounds.Bottom);
                    }
                    else
                    {
                        gripLocation = new Point(SizeGripBounds.Right, SizeGripBounds.Bottom);
                    }

                    PInvoke.MapWindowPoints(this, rootHwnd, ref gripLocation);

                    int deltaBottomEdge = Math.Abs(rootHwndClientArea.bottom - gripLocation.Y);
                    int deltaRightEdge = Math.Abs(rootHwndClientArea.right - gripLocation.X);

                    if (RightToLeft != RightToLeft.Yes)
                    {
                        if ((deltaRightEdge + deltaBottomEdge) < 2)
                        {
                            m.ResultInternal = (LRESULT)(nint)PInvoke.HTBOTTOMRIGHT;
                            return;
                        }
                    }
                }
            }
        }

        base.WndProc(ref m);
    }

    // special transparent mirrored window which says it's the bottom left of the form.
    private class RightToLeftLayoutGrip : Control
    {
        public RightToLeftLayoutGrip()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvoke.WM_NCHITTEST)
            {
                if (ClientRectangle.Contains(PointToClient(PARAM.ToPoint(m.LParamInternal))))
                {
                    m.ResultInternal = (LRESULT)(nint)PInvoke.HTBOTTOMLEFT;
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
