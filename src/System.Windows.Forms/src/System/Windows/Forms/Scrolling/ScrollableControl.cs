// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  Defines a base class for controls that support auto-scrolling behavior.
/// </summary>
[Designer($"System.Windows.Forms.Design.ScrollableControlDesigner, {AssemblyRef.SystemDesign}")]
public partial class ScrollableControl : Control, IArrangedElement
{
    protected const int ScrollStateAutoScrolling = 0x0001;
    protected const int ScrollStateHScrollVisible = 0x0002;
    protected const int ScrollStateVScrollVisible = 0x0004;
    protected const int ScrollStateUserHasScrolled = 0x0008;
    protected const int ScrollStateFullDrag = 0x0010;

    private Size _userAutoScrollMinSize = Size.Empty;

    /// <summary>
    ///  Current size of the displayRect.
    /// </summary>
    private Rectangle _displayRect = Rectangle.Empty;

    /// <summary>
    ///  Current margins for auto-scrolling.
    /// </summary>
    private Size _scrollMargin = Size.Empty;

    /// <summary>
    ///  User requested margins for autoscrolling.
    /// </summary>
    private Size _requestedScrollMargin = Size.Empty;

    private DockPaddingEdges? _dockPadding;

    private int _scrollState;

    private VScrollProperties? _verticalScroll;

    private HScrollProperties? _horizontalScroll;

    private static readonly object s_scrollEvent = new();

    /// <summary>
    ///  Used to figure out what the horizontal scroll value should be set to when the horizontal
    ///  scrollbar is first shown.
    /// </summary>
    private bool _resetRTLHScrollValue;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ScrollableControl"/> class.
    /// </summary>
    public ScrollableControl() : base()
    {
        SetStyle(ControlStyles.ContainerControl, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, false);
        SetScrollState(ScrollStateAutoScrolling, false);

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        SetStyle(ControlStyles.ApplyThemingImplicitly, true);
#pragma warning restore WFO5001
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the container will allow the user to
    ///  scroll to any controls placed outside of its visible boundaries.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FormAutoScrollDescr))]
    public virtual bool AutoScroll
    {
        get => GetScrollState(ScrollStateAutoScrolling);
        set
        {
            if (value)
            {
                UpdateFullDrag();
            }

            SetScrollState(ScrollStateAutoScrolling, value);
            LayoutTransaction.DoLayout(this, this, PropertyNames.AutoScroll);
        }
    }

    /// <summary>
    ///  Gets or sets the size of the auto-scroll margin.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.FormAutoScrollMarginDescr))]
    public Size AutoScrollMargin
    {
        get => _requestedScrollMargin;
        set
        {
            if (value.Width < 0 || value.Height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(AutoScrollMargin), value));
            }

            SetAutoScrollMargin(value.Width, value.Height);
        }
    }

    /// <summary>
    ///  Gets or sets the location of the auto-scroll position.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.FormAutoScrollPositionDescr))]
    public Point AutoScrollPosition
    {
        get
        {
            Rectangle rect = GetDisplayRectInternal();
            return new Point(rect.X, rect.Y);
        }
        set
        {
            if (Created)
            {
                SetDisplayRectLocation(-value.X, -value.Y);
                SyncScrollbars(true);
            }
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.FormAutoScrollMinSizeDescr))]
    public Size AutoScrollMinSize
    {
        get => _userAutoScrollMinSize;
        set
        {
            if (value != _userAutoScrollMinSize)
            {
                _userAutoScrollMinSize = value;
                AutoScroll = true;
                PerformLayout();
            }
        }
    }

    /// <summary>
    ///  Retrieves the CreateParams used to create the window.
    ///  If a subclass overrides this function, it must call the base implementation.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;

            if (HScroll || HorizontalScroll.Visible)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_HSCROLL;
            }
            else
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_HSCROLL;
            }

            if (VScroll || VerticalScroll.Visible)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_VSCROLL;
            }
            else
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_VSCROLL;
            }

            return cp;
        }
    }

    /// <summary>
    ///  Retrieves the current display rectangle. The display rectangle is the virtual
    ///  display area that is used to layout components. The position and dimensions of
    ///  the Form's display rectangle change during autoScroll.
    /// </summary>
    public override Rectangle DisplayRectangle
    {
        get
        {
            Rectangle rect = ClientRectangle;
            if (!_displayRect.IsEmpty)
            {
                rect.X = _displayRect.X;
                rect.Y = _displayRect.Y;
                if (HScroll)
                {
                    rect.Width = _displayRect.Width;
                }

                if (VScroll)
                {
                    rect.Height = _displayRect.Height;
                }
            }

            return LayoutUtils.DeflateRect(rect, Padding);
        }
    }

    Rectangle IArrangedElement.DisplayRectangle
    {
        get
        {
            Rectangle displayRectangle = DisplayRectangle;
            // Controls anchored the bottom of their container may disappear (be scrunched)
            // when scrolling is used.
            if (AutoScrollMinSize.Width != 0 && AutoScrollMinSize.Height != 0)
            {
                displayRectangle.Width = Math.Max(displayRectangle.Width, AutoScrollMinSize.Width);
                displayRectangle.Height = Math.Max(displayRectangle.Height, AutoScrollMinSize.Height);
            }

            return displayRectangle;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the horizontal scroll bar is visible.
    /// </summary>
    protected bool HScroll
    {
        get => GetScrollState(ScrollStateHScrollVisible);
        set => SetScrollState(ScrollStateHScrollVisible, value);
    }

    /// <summary>
    ///  Gets the Horizontal Scroll bar for this ScrollableControl.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ScrollableControlHorizontalScrollDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public HScrollProperties HorizontalScroll => _horizontalScroll ??= new HScrollProperties(this);

    /// <summary>
    ///  Gets or sets a value indicating whether the vertical scroll bar is visible.
    /// </summary>
    protected bool VScroll
    {
        get => GetScrollState(ScrollStateVScrollVisible);
        set => SetScrollState(ScrollStateVScrollVisible, value);
    }

    /// <summary>
    ///  Gets the Vertical Scroll bar for this ScrollableControl.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ScrollableControlVerticalScrollDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public VScrollProperties VerticalScroll => _verticalScroll ??= new VScrollProperties(this);

    /// <summary>
    ///  Gets the dock padding settings for all edges of the control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DockPaddingEdges DockPadding => _dockPadding ??= new DockPaddingEdges(this);

    /// <summary>
    ///  Adjusts the auto-scroll bars on the container based on the current control
    ///  positions and the control currently selected.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void AdjustFormScrollbars(bool displayScrollbars)
    {
        bool needLayout = false;
        Rectangle display = GetDisplayRectInternal();
        if (!displayScrollbars && (HScroll || VScroll))
        {
            needLayout = SetVisibleScrollbars(false, false);
        }

        if (!displayScrollbars)
        {
            Rectangle client = ClientRectangle;
            display.Width = client.Width;
            display.Height = client.Height;
        }
        else
        {
            needLayout |= ApplyScrollbarChanges(display);
        }

        if (needLayout)
        {
            LayoutTransaction.DoLayout(this, this, PropertyNames.DisplayRectangle);
        }
    }

    private bool ApplyScrollbarChanges(Rectangle display)
    {
        bool needLayout = false;
        bool needHscroll = false;
        bool needVscroll = false;
        Rectangle currentClient = ClientRectangle;
        Rectangle fullClient = currentClient;
        Rectangle minClient = fullClient;
        if (HScroll)
        {
            fullClient.Height += SystemInformation.HorizontalScrollBarHeight;
        }
        else
        {
            minClient.Height -= SystemInformation.HorizontalScrollBarHeight;
        }

        if (VScroll)
        {
            fullClient.Width += SystemInformation.VerticalScrollBarWidth;
        }
        else
        {
            minClient.Width -= SystemInformation.VerticalScrollBarWidth;
        }

        int maxX = minClient.Width;
        int maxY = minClient.Height;

        if (Controls.Count != 0)
        {
            // Compute the actual scroll margins (take into account docked
            // things.)
            _scrollMargin = _requestedScrollMargin;

            if (_dockPadding is not null)
            {
                _scrollMargin.Height += Padding.Bottom;
                _scrollMargin.Width += Padding.Right;
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                Control current = Controls[i];

                // Since Control.Visible checks the parent visibility, we
                // want to see if this control will be visible if we
                // become visible. This prevents a nasty painting issue
                // if we suddenly update windows styles in response
                // to a WM_SHOWWINDOW.
                //
                // In addition, this is the more correct thing, because
                // we want to layout the children with respect to their
                // "local" visibility, not the hierarchy.
                if (current is not null && current.DesiredVisibility)
                {
                    switch (current.Dock)
                    {
                        case DockStyle.Bottom:
                            _scrollMargin.Height += current.Size.Height;
                            break;
                        case DockStyle.Right:
                            _scrollMargin.Width += current.Size.Width;
                            break;
                    }
                }
            }
        }

        if (!_userAutoScrollMinSize.IsEmpty)
        {
            maxX = _userAutoScrollMinSize.Width + _scrollMargin.Width;
            maxY = _userAutoScrollMinSize.Height + _scrollMargin.Height;
            needHscroll = true;
            needVscroll = true;
        }

        bool defaultLayoutEngine = (LayoutEngine == DefaultLayout.Instance);
        if (!defaultLayoutEngine && CommonProperties.HasLayoutBounds(this))
        {
            Size layoutBounds = CommonProperties.GetLayoutBounds(this);

            if (layoutBounds.Width > maxX)
            {
                needHscroll = true;
                maxX = layoutBounds.Width;
            }

            if (layoutBounds.Height > maxY)
            {
                needVscroll = true;
                maxY = layoutBounds.Height;
            }
        }
        else if (Controls.Count != 0)
        {
            // Compute the dimensions of the display rect
            for (int i = 0; i < Controls.Count; i++)
            {
                bool watchHoriz = true;
                bool watchVert = true;

                Control current = Controls[i];

                // Same logic as the margin calc - you need to see if the
                // control *will* be visible...
                if (current is not null && current.DesiredVisibility)
                {
                    if (defaultLayoutEngine)
                    {
                        Control richCurrent = current;

                        switch (richCurrent.Dock)
                        {
                            case DockStyle.Top:
                                watchHoriz = false;
                                break;
                            case DockStyle.Left:
                                watchVert = false;
                                break;
                            case DockStyle.Bottom:
                            case DockStyle.Fill:
                            case DockStyle.Right:
                                watchHoriz = false;
                                watchVert = false;
                                break;
                            default:
                                AnchorStyles anchor = richCurrent.Anchor;
                                if ((anchor & AnchorStyles.Right) == AnchorStyles.Right)
                                {
                                    watchHoriz = false;
                                }

                                if ((anchor & AnchorStyles.Left) != AnchorStyles.Left)
                                {
                                    watchHoriz = false;
                                }

                                if ((anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
                                {
                                    watchVert = false;
                                }

                                if ((anchor & AnchorStyles.Top) != AnchorStyles.Top)
                                {
                                    watchVert = false;
                                }

                                break;
                        }
                    }

                    if (watchHoriz || watchVert)
                    {
                        Rectangle bounds = current.Bounds;
                        int ctlRight = -display.X + bounds.X + bounds.Width + _scrollMargin.Width;
                        int ctlBottom = -display.Y + bounds.Y + bounds.Height + _scrollMargin.Height;

                        if (!defaultLayoutEngine)
                        {
                            ctlRight += current.Margin.Right;
                            ctlBottom += current.Margin.Bottom;
                        }

                        if (ctlRight > maxX && watchHoriz)
                        {
                            needHscroll = true;
                            maxX = ctlRight;
                        }

                        if (ctlBottom > maxY && watchVert)
                        {
                            needVscroll = true;
                            maxY = ctlBottom;
                        }
                    }
                }
            }
        }

        // Check maxX/maxY against the clientRect, we must compare it to the
        // clientRect without any scrollbars, and then we can check it against
        // the clientRect with the "new" scrollbars. This will make the
        // scrollbars show and hide themselves correctly at the boundaries.
        if (maxX <= fullClient.Width)
        {
            needHscroll = false;
        }

        if (maxY <= fullClient.Height)
        {
            needVscroll = false;
        }

        Rectangle clientToBe = fullClient;
        if (needHscroll)
        {
            clientToBe.Height -= SystemInformation.HorizontalScrollBarHeight;
        }

        if (needVscroll)
        {
            clientToBe.Width -= SystemInformation.VerticalScrollBarWidth;
        }

        if (needHscroll && maxY > clientToBe.Height)
        {
            needVscroll = true;
        }

        if (needVscroll && maxX > clientToBe.Width)
        {
            needHscroll = true;
        }

        if (!needHscroll)
        {
            maxX = clientToBe.Width;
        }

        if (!needVscroll)
        {
            maxY = clientToBe.Height;
        }

        // Show the needed scrollbars
        needLayout = (SetVisibleScrollbars(needHscroll, needVscroll) || needLayout);

        // If needed, adjust the size.
        if (HScroll || VScroll)
        {
            needLayout = (SetDisplayRectangleSize(maxX, maxY) || needLayout);
        }
        else
        {
            // Else just update the display rect size. This keeps it as big as the client
            // area in a resize scenario.
            SetDisplayRectangleSize(maxX, maxY);
        }

        // Sync up the scrollbars
        SyncScrollbars(true);
        return needLayout;
    }

    private Rectangle GetDisplayRectInternal()
    {
        if (_displayRect.IsEmpty)
        {
            _displayRect = ClientRectangle;
        }

        if (!AutoScroll && HorizontalScroll._visible)
        {
            _displayRect = new Rectangle(_displayRect.X, _displayRect.Y, HorizontalScroll.Maximum, _displayRect.Height);
        }

        if (!AutoScroll && VerticalScroll._visible)
        {
            _displayRect = new Rectangle(_displayRect.X, _displayRect.Y, _displayRect.Width, VerticalScroll.Maximum);
        }

        return _displayRect;
    }

    /// <summary>
    ///  Tests a given scroll state bit to determine if it is set.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool GetScrollState(int bit) => (bit & _scrollState) == bit;

    /// <summary>
    ///  Forces the layout of any docked or anchored child controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnLayout(LayoutEventArgs levent)
    {
        // We get into a problem when you change the docking of a control
        // with autosizing on. Since the control (affectedControl) has
        // already had the dock property changed, adjustFormScrollbars
        // treats it as a docked control. However, since base.onLayout
        // hasn't been called yet, the bounds of the control haven't been
        // changed.
        //
        // We can't just call base.onLayout() once in this case, since
        // adjusting the scrollbars COULD adjust the display area, and
        // thus require a new layout. The result is that when you
        // affect a control's layout, we are forced to layout twice. There
        // isn't any noticeable flicker, but this could be a perf problem...
        if (levent is not null && levent.AffectedControl is not null && AutoScroll)
        {
            base.OnLayout(levent);
        }

        AdjustFormScrollbars(AutoScroll);

        // Because the code has been like that since long time, we assume that levent is not null.
        base.OnLayout(levent!);
    }

    /// <summary>
    ///  Handles mouse wheel processing for our scrollbars.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        // Favor the vertical scroll bar, since it's the most common use. However, if
        // there isn't a vertical scroll and the horizontal is on, then wheel it around.
        if (VScroll)
        {
            Rectangle client = ClientRectangle;
            int pos = -_displayRect.Y;
            int maxPos = -(client.Height - _displayRect.Height);

            pos = Math.Max(pos - e.Delta, 0);
            pos = Math.Min(pos, maxPos);

            SetDisplayRectLocation(_displayRect.X, -pos);
            SyncScrollbars(AutoScroll);
            if (e is HandledMouseEventArgs args)
            {
                args.Handled = true;
            }
        }
        else if (HScroll)
        {
            Rectangle client = ClientRectangle;
            int pos = -_displayRect.X;
            int maxPos = -(client.Width - _displayRect.Width);

            pos = Math.Max(pos - e.Delta, 0);
            pos = Math.Min(pos, maxPos);

            SetDisplayRectLocation(-pos, _displayRect.Y);
            SyncScrollbars(AutoScroll);
            if (e is HandledMouseEventArgs args)
            {
                args.Handled = true;
            }
        }

        // The base implementation should be called before the implementation above,
        // but changing the order in Whidbey would be too much of a breaking change
        // for this particular class.
        base.OnMouseWheel(e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        _resetRTLHScrollValue = true;
        // When the page becomes visible, we need to call OnLayout to adjust the scrollbars.
        LayoutTransaction.DoLayout(this, this, PropertyNames.RightToLeft);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if ((HScroll || VScroll) &&
            BackgroundImage is not null &&
            (BackgroundImageLayout == ImageLayout.Zoom || BackgroundImageLayout == ImageLayout.Stretch || BackgroundImageLayout == ImageLayout.Center))
        {
            if (ControlPaint.IsImageTransparent(BackgroundImage))
            {
                PaintTransparentBackground(e, _displayRect);
            }

            ControlPaint.DrawBackgroundImage(
                e.Graphics,
                BackgroundImage,
                BackColor,
                BackgroundImageLayout,
                _displayRect,
                _displayRect,
                _displayRect.Location);
        }
        else
        {
            base.OnPaintBackground(e);
        }
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        // Don't call base in this instance - for App compat we should not fire Invalidate
        // when  the padding has changed.
        ((EventHandler?)Events[s_paddingChangedEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnVisibleChanged(EventArgs e)
    {
        if (Visible)
        {
            // When the page becomes visible, we need to call OnLayout to adjust the scrollbars.
            LayoutTransaction.DoLayout(this, this, PropertyNames.Visible);
        }

        base.OnVisibleChanged(e);
    }

    internal void ScaleDockPadding(float dx, float dy)
    {
        _dockPadding?.Scale(dx, dy);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void ScaleCore(float dx, float dy)
    {
        ScaleDockPadding(dx, dy);
        base.ScaleCore(dx, dy);
    }

    /// <summary>
    ///  Scale this form. Form overrides this to enforce a maximum / minimum size.
    /// </summary>
    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        ScaleDockPadding(factor.Width, factor.Height);
        base.ScaleControl(factor, specified);
    }

    /// <summary>
    ///  Allows to set the <see cref="DisplayRectangle" /> to enable the visual scroll effect.
    /// </summary>
    internal void SetDisplayFromScrollProps(int x, int y)
    {
        Rectangle display = GetDisplayRectInternal();
        ApplyScrollbarChanges(display);
        SetDisplayRectLocation(x, y);
    }

    /// <summary>
    ///  Adjusts the displayRect to be at the offset x, y. The contents of the
    ///  Form is scrolled using Windows.ScrollWindowEx.
    /// </summary>
    protected unsafe void SetDisplayRectLocation(int x, int y)
    {
        int xDelta = 0;
        int yDelta = 0;

        Rectangle client = ClientRectangle;
        // The DisplayRect property modifies
        // the returned rect to include padding. We don't want to
        // include this padding in our adjustment of the DisplayRect
        // because it interferes with the scrolling.
        Rectangle displayRectangle = _displayRect;
        int minX = Math.Min(client.Width - displayRectangle.Width, 0);
        int minY = Math.Min(client.Height - displayRectangle.Height, 0);

        if (x > 0)
        {
            x = 0;
        }

        if (y > 0)
        {
            y = 0;
        }

        if (x < minX)
        {
            x = minX;
        }

        if (y < minY)
        {
            y = minY;
        }

        if (displayRectangle.X != x)
        {
            xDelta = x - displayRectangle.X;
        }

        if (displayRectangle.Y != y)
        {
            yDelta = y - displayRectangle.Y;
        }

        _displayRect.X = x;
        _displayRect.Y = y;

        if (IsHandleCreated && (xDelta != 0 || yDelta != 0))
        {
            Debug.Assert(IsHandleCreated, "Handle is not created");

            RECT rcClip = ClientRectangle;
            RECT rcUpdate = ClientRectangle;
            PInvoke.ScrollWindowEx(
                this,
                xDelta,
                yDelta,
                null,
                &rcClip,
                HRGN.Null,
                &rcUpdate,
                SCROLL_WINDOW_FLAGS.SW_INVALIDATE | SCROLL_WINDOW_FLAGS.SW_ERASE | SCROLL_WINDOW_FLAGS.SW_SCROLLCHILDREN);
        }

        // Force child controls to update bounds.
        for (int i = 0; i < Controls.Count; i++)
        {
            Control ctl = Controls[i];
            if (ctl is not null && ctl.IsHandleCreated)
            {
                ctl.UpdateBounds();
            }
        }
    }

    /// <summary>
    ///  Scrolls the currently active control into view if we are an AutoScroll
    ///  Form that has the Horiz or Vert scrollbar displayed...
    /// </summary>
    public void ScrollControlIntoView(Control? activeControl)
    {
        if (activeControl is null)
        {
            return;
        }

        Rectangle client = ClientRectangle;

        if (IsDescendant(activeControl)
            && AutoScroll
            && (HScroll || VScroll)
            && (client.Width > 0 && client.Height > 0))
        {
            Point scrollLocation = ScrollToControl(activeControl);
            SetScrollState(ScrollStateUserHasScrolled, false);
            SetDisplayRectLocation(scrollLocation.X, scrollLocation.Y);
            SyncScrollbars(true);
        }
    }

    /// <summary>
    ///  Allow containers to tweak AutoScrolling. when you tab between controls contained in the scrollable control
    ///  this allows you to set the scroll location. This would allow you to scroll to the middle of a control,
    ///  where as the default is the top of the control.
    ///  Additionally there is a new AutoScrollOffset property on the child controls themselves. This lets them control
    ///  where they want to be scrolled to. E.g. In SelectedIndexChanged for a ListBox, you could do:
    ///  listBox1.AutoScrollOffset = parent.AutoScrollPosition;
    /// </summary>
    protected virtual Point ScrollToControl(Control activeControl)
    {
        Rectangle client = ClientRectangle;
        int xCalc = _displayRect.X;
        int yCalc = _displayRect.Y;
        int xMargin = _scrollMargin.Width;
        int yMargin = _scrollMargin.Height;

        Rectangle bounds = activeControl.Bounds;
        if (activeControl.ParentInternal != this)
        {
            if (activeControl.ParentInternal is null)
            {
                throw new InvalidOperationException(SR.ScrollableControlActiveControlParentNull);
            }

            bounds = RectangleToClient(activeControl.ParentInternal.RectangleToScreen(bounds));
        }

        if (bounds.X < xMargin)
        {
            xCalc = _displayRect.X + xMargin - bounds.X;
        }
        else if (bounds.X + bounds.Width + xMargin > client.Width)
        {
            xCalc = client.Width - (bounds.X + bounds.Width + xMargin - _displayRect.X);

            if (bounds.X + xCalc - _displayRect.X < xMargin)
            {
                xCalc = _displayRect.X + xMargin - bounds.X;
            }
        }

        if (bounds.Y < yMargin)
        {
            yCalc = _displayRect.Y + yMargin - bounds.Y;
        }
        else if (bounds.Y + bounds.Height + yMargin > client.Height)
        {
            yCalc = client.Height - (bounds.Y + bounds.Height + yMargin - _displayRect.Y);
            if (bounds.Y + yCalc - _displayRect.Y < yMargin)
            {
                yCalc = _displayRect.Y + yMargin - bounds.Y;
            }
        }

        xCalc += activeControl.AutoScrollOffset.X;
        yCalc += activeControl.AutoScrollOffset.Y;

        return new Point(xCalc, yCalc);
    }

    private unsafe int ScrollThumbPosition(SCROLLBAR_CONSTANTS fnBar)
    {
        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_TRACKPOS
        };

        PInvoke.GetScrollInfo(this, fnBar, ref si);
        return si.nTrackPos;
    }

    /// <summary>
    ///  Occurs when the scroll box has been moved by either a mouse or keyboard action.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ScrollBarOnScrollDescr))]
    public event ScrollEventHandler? Scroll
    {
        add => Events.AddHandler(s_scrollEvent, value);
        remove => Events.RemoveHandler(s_scrollEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="ScrollBar.OnScroll"/> event.
    /// </summary>
    protected virtual void OnScroll(ScrollEventArgs se)
    {
        ((ScrollEventHandler?)Events[s_scrollEvent])?.Invoke(this, se);
    }

    private void ResetAutoScrollMargin() => AutoScrollMargin = Size.Empty;

    private void ResetAutoScrollMinSize() => AutoScrollMinSize = Size.Empty;

    private static void ResetScrollProperties(ScrollProperties scrollProperties)
    {
        // Set only these two values as when the ScrollBars are not visible ...
        // there is no meaning of the "value" property.
        scrollProperties._visible = false;
        scrollProperties._value = 0;
    }

    /// <summary>
    ///  Sets the size of the auto-scroll margins.
    /// </summary>
    public void SetAutoScrollMargin(int x, int y)
    {
        // Make sure we're not setting the margins to negative numbers
        if (x < 0)
        {
            x = 0;
        }

        if (y < 0)
        {
            y = 0;
        }

        if (x != _requestedScrollMargin.Width || y != _requestedScrollMargin.Height)
        {
            _requestedScrollMargin = new Size(x, y);
            if (AutoScroll)
            {
                PerformLayout();
            }
        }
    }

    /// <summary>
    ///  Actually displays or hides the horiz and vert autoscrollbars. This will
    ///  also adjust the values of formState to reflect the new state
    /// </summary>
    private bool SetVisibleScrollbars(bool horiz, bool vert)
    {
        bool needLayout = false;

        if ((!horiz && HScroll)
            || (horiz && !HScroll)
            || (!vert && VScroll)
            || (vert && !VScroll))
        {
            needLayout = true;
        }

        // If we are about to show the horizontal scrollbar, then
        // set this flag, so that we can set the right initial value
        // based on whether we are right to left.
        if (horiz && !HScroll && (RightToLeft == RightToLeft.Yes))
        {
            _resetRTLHScrollValue = true;
        }

        if (needLayout)
        {
            int x = _displayRect.X;
            int y = _displayRect.Y;
            if (!horiz)
            {
                x = 0;
            }

            if (!vert)
            {
                y = 0;
            }

            SetDisplayRectLocation(x, y);
            SetScrollState(ScrollStateUserHasScrolled, false);
            HScroll = horiz;
            VScroll = vert;

            // Update the visible member of ScrollBars.
            if (horiz)
            {
                HorizontalScroll._visible = true;
            }
            else
            {
                ResetScrollProperties(HorizontalScroll);
            }

            if (vert)
            {
                VerticalScroll._visible = true;
            }
            else
            {
                ResetScrollProperties(VerticalScroll);
            }

            UpdateStyles();
        }

        return needLayout;
    }

    /// <summary>
    ///  Sets the width and height of the virtual client area used in autoscrolling.
    ///  This will also adjust the x and y location of the virtual client area if the
    ///  new size forces it.
    /// </summary>
    private bool SetDisplayRectangleSize(int width, int height)
    {
        bool needLayout = false;
        if (_displayRect.Width != width || _displayRect.Height != height)
        {
            _displayRect.Width = width;
            _displayRect.Height = height;
            needLayout = true;
        }

        int minX = ClientRectangle.Width - width;
        int minY = ClientRectangle.Height - height;
        if (minX > 0)
        {
            minX = 0;
        }

        if (minY > 0)
        {
            minY = 0;
        }

        int x = _displayRect.X;
        int y = _displayRect.Y;

        if (!HScroll)
        {
            x = 0;
        }

        if (!VScroll)
        {
            y = 0;
        }

        if (x < minX)
        {
            x = minX;
        }

        if (y < minY)
        {
            y = minY;
        }

        SetDisplayRectLocation(x, y);
        return needLayout;
    }

    /// <summary>
    ///  Sets a given scroll state bit.
    /// </summary>
    protected void SetScrollState(int bit, bool value)
    {
        if (value)
        {
            _scrollState |= bit;
        }
        else
        {
            _scrollState &= (~bit);
        }
    }

    /// <summary>
    ///  Indicates whether the <see cref="AutoScrollPosition"/> property should
    ///  be persisted.
    /// </summary>
    private bool ShouldSerializeAutoScrollPosition()
    {
        if (AutoScroll)
        {
            Point pt = AutoScrollPosition;
            if (pt.X != 0 || pt.Y != 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Indicates whether the <see cref="AutoScrollMargin"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeAutoScrollMargin() => !AutoScrollMargin.Equals(Size.Empty);

    /// <summary>
    ///  Indicates whether the <see cref="AutoScrollMinSize"/>
    ///  property should be persisted.
    /// </summary>
    private bool ShouldSerializeAutoScrollMinSize() => !AutoScrollMinSize.Equals(Size.Empty);

    /// <summary>
    ///  Updates the value of the autoscroll scrollbars based on the current form
    ///  state. This is a one-way sync, updating the scrollbars only.
    /// </summary>
    private void SyncScrollbars(bool autoScroll)
    {
        Rectangle displayRect = _displayRect;

        if (autoScroll)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            if (HScroll)
            {
                if (!HorizontalScroll._maximumSetExternally)
                {
                    HorizontalScroll._maximum = displayRect.Width - 1;
                }

                if (!HorizontalScroll._largeChangeSetExternally)
                {
                    HorizontalScroll._largeChange = ClientRectangle.Width;
                }

                if (!HorizontalScroll._smallChangeSetExternally)
                {
                    HorizontalScroll._smallChange = 5;
                }

                if (_resetRTLHScrollValue && !IsMirrored)
                {
                    _resetRTLHScrollValue = false;
                    BeginInvoke(new EventHandler(OnSetScrollPosition));
                }
                else if (-displayRect.X >= HorizontalScroll._minimum && -displayRect.X < HorizontalScroll._maximum)
                {
                    HorizontalScroll._value = -displayRect.X;
                }

                HorizontalScroll.UpdateScrollInfo();
            }

            if (VScroll)
            {
                if (!VerticalScroll._maximumSetExternally)
                {
                    VerticalScroll._maximum = displayRect.Height - 1;
                }

                if (!VerticalScroll._largeChangeSetExternally)
                {
                    VerticalScroll._largeChange = ClientRectangle.Height;
                }

                if (!VerticalScroll._smallChangeSetExternally)
                {
                    VerticalScroll._smallChange = 5;
                }

                if (-displayRect.Y >= VerticalScroll._minimum && -displayRect.Y < VerticalScroll._maximum)
                {
                    VerticalScroll._value = -displayRect.Y;
                }

                VerticalScroll.UpdateScrollInfo();
            }
        }
        else
        {
            if (HorizontalScroll.Visible)
            {
                HorizontalScroll.Value = -displayRect.X;
            }
            else
            {
                ResetScrollProperties(HorizontalScroll);
            }

            if (VerticalScroll.Visible)
            {
                VerticalScroll.Value = -displayRect.Y;
            }
            else
            {
                ResetScrollProperties(VerticalScroll);
            }
        }
    }

    private void OnSetScrollPosition(object? sender, EventArgs e)
    {
        if (!IsMirrored)
        {
            PInvokeCore.SendMessage(
                this,
                PInvokeCore.WM_HSCROLL,
                (WPARAM)(RightToLeft == RightToLeft.Yes ? (int)SCROLLBAR_COMMAND.SB_RIGHT : (int)SCROLLBAR_COMMAND.SB_LEFT),
                0);
        }
    }

    /// <summary>
    ///  Queries the system to determine the users preference for full drag
    ///  of windows.
    /// </summary>
    private void UpdateFullDrag()
    {
        SetScrollState(ScrollStateFullDrag, SystemInformation.DragFullWindows);
    }

    /// <summary>
    ///  WM_VSCROLL handler
    /// </summary>
    private void WmVScroll(ref Message m)
    {
        // The lparam is handle of the sending scrollbar, or NULL when
        // the scrollbar sending the message is the "form" scrollbar.
        if (m.LParamInternal != 0)
        {
            base.WndProc(ref m);
            return;
        }

        Rectangle client = ClientRectangle;
        SCROLLBAR_COMMAND loWord = (SCROLLBAR_COMMAND)m.WParamInternal.LOWORD;
        bool thumbTrack = loWord != SCROLLBAR_COMMAND.SB_THUMBTRACK;
        int pos = -_displayRect.Y;
        int oldValue = pos;

        int maxPos = -(client.Height - _displayRect.Height);
        if (!AutoScroll)
        {
            maxPos = VerticalScroll.Maximum;
        }

        switch (loWord)
        {
            case SCROLLBAR_COMMAND.SB_THUMBPOSITION:
            case SCROLLBAR_COMMAND.SB_THUMBTRACK:
                pos = ScrollThumbPosition(SCROLLBAR_CONSTANTS.SB_VERT);
                break;
            case SCROLLBAR_COMMAND.SB_LINEUP:
                if (pos > 0)
                {
                    pos -= VerticalScroll.SmallChange;
                }
                else
                {
                    pos = 0;
                }

                break;
            case SCROLLBAR_COMMAND.SB_LINEDOWN:
                if (pos < maxPos - VerticalScroll.SmallChange)
                {
                    pos += VerticalScroll.SmallChange;
                }
                else
                {
                    pos = maxPos;
                }

                break;
            case SCROLLBAR_COMMAND.SB_PAGEUP:
                if (pos > VerticalScroll.LargeChange)
                {
                    pos -= VerticalScroll.LargeChange;
                }
                else
                {
                    pos = 0;
                }

                break;
            case SCROLLBAR_COMMAND.SB_PAGEDOWN:
                if (pos < maxPos - VerticalScroll.LargeChange)
                {
                    pos += VerticalScroll.LargeChange;
                }
                else
                {
                    pos = maxPos;
                }

                break;
            case SCROLLBAR_COMMAND.SB_TOP:
                pos = 0;
                break;
            case SCROLLBAR_COMMAND.SB_BOTTOM:
                pos = maxPos;
                break;
        }

        // If  SystemInformation.DragFullWindows set is to false the usage should be
        // identical to WnHScroll which follows.
        if (GetScrollState(ScrollStateFullDrag) || thumbTrack)
        {
            SetScrollState(ScrollStateUserHasScrolled, true);
            SetDisplayRectLocation(_displayRect.X, -pos);
            SyncScrollbars(AutoScroll);
        }

        WmOnScroll(ref m, oldValue, pos, ScrollOrientation.VerticalScroll);
    }

    /// <summary>
    ///  WM_HSCROLL handler
    /// </summary>
    private void WmHScroll(ref Message m)
    {
        // The lparam is handle of the sending scrollbar, or NULL when
        // the scrollbar sending the message is the "form" scrollbar.
        if (m.LParamInternal != 0)
        {
            base.WndProc(ref m);
            return;
        }

        Rectangle client = ClientRectangle;

        int pos = -_displayRect.X;
        int oldValue = pos;
        int maxPos = -(client.Width - _displayRect.Width);
        if (!AutoScroll)
        {
            maxPos = HorizontalScroll.Maximum;
        }

        SCROLLBAR_COMMAND loWord = (SCROLLBAR_COMMAND)m.WParamInternal.LOWORD;
        switch (loWord)
        {
            case SCROLLBAR_COMMAND.SB_THUMBPOSITION:
            case SCROLLBAR_COMMAND.SB_THUMBTRACK:
                pos = ScrollThumbPosition(SCROLLBAR_CONSTANTS.SB_HORZ);
                break;
            case SCROLLBAR_COMMAND.SB_LINELEFT:
                if (pos > HorizontalScroll.SmallChange)
                {
                    pos -= HorizontalScroll.SmallChange;
                }
                else
                {
                    pos = 0;
                }

                break;
            case SCROLLBAR_COMMAND.SB_LINERIGHT:
                if (pos < maxPos - HorizontalScroll.SmallChange)
                {
                    pos += HorizontalScroll.SmallChange;
                }
                else
                {
                    pos = maxPos;
                }

                break;
            case SCROLLBAR_COMMAND.SB_PAGELEFT:
                if (pos > HorizontalScroll.LargeChange)
                {
                    pos -= HorizontalScroll.LargeChange;
                }
                else
                {
                    pos = 0;
                }

                break;
            case SCROLLBAR_COMMAND.SB_PAGERIGHT:
                if (pos < maxPos - HorizontalScroll.LargeChange)
                {
                    pos += HorizontalScroll.LargeChange;
                }
                else
                {
                    pos = maxPos;
                }

                break;
            case SCROLLBAR_COMMAND.SB_LEFT:
                pos = 0;
                break;
            case SCROLLBAR_COMMAND.SB_RIGHT:
                pos = maxPos;
                break;
        }

        if (GetScrollState(ScrollStateFullDrag) || loWord != SCROLLBAR_COMMAND.SB_THUMBTRACK)
        {
            SetScrollState(ScrollStateUserHasScrolled, true);
            SetDisplayRectLocation(-pos, _displayRect.Y);
            SyncScrollbars(AutoScroll);
        }

        WmOnScroll(ref m, oldValue, pos, ScrollOrientation.HorizontalScroll);
    }

    /// <summary>
    ///  This function gets called which populates the eventArgs and fires the OnScroll( ) event passing
    ///  the appropriate scroll event and scroll bar.
    /// </summary>
    private void WmOnScroll(ref Message m, int oldValue, int value, ScrollOrientation scrollOrientation)
    {
        ScrollEventType type = (ScrollEventType)m.WParamInternal.LOWORD;
        if (type != ScrollEventType.EndScroll)
        {
            ScrollEventArgs se = new(type, oldValue, value, scrollOrientation);
            OnScroll(se);
        }
    }

    private void WmSettingChange(ref Message m)
    {
        base.WndProc(ref m);
        UpdateFullDrag();
    }

    /// <summary>
    ///  The button's window procedure. Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the button continues to function properly.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_VSCROLL:
                WmVScroll(ref m);
                break;
            case PInvokeCore.WM_HSCROLL:
                WmHScroll(ref m);
                break;
            case PInvokeCore.WM_SETTINGCHANGE:
                WmSettingChange(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
