// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class implements our design time document. This is the outer window that encompasses a designer.
///  It maintains a control hierarchy that looks like this:
///
///     - DesignerFrame
///         - ScrollableControl
///             - Designer
///         - Splitter
///         - ScrollableControl
///             - Component Tray
///
///  The splitter and second scrollable control are created on demand when a tray is added.
/// </summary>
internal class DesignerFrame : Control, IOverlayService, ISplitWindowService, IContainsThemedScrollbarWindows
{
    private readonly ISite _designerSite;
    private readonly OverlayControl _designerRegion;
    private Splitter? _splitter;
    private Control? _designer;
    private BehaviorService? _behaviorService;
    private readonly IUIService? _uiService;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DesignerFrame"/> class.
    /// </summary>
    public DesignerFrame(ISite site)
    {
        Text = "DesignerFrame";
        _designerSite = site;
        _designerRegion = new OverlayControl(site);
        _uiService = _designerSite.GetService<IUIService>();
        if (_uiService?.Styles["ArtboardBackground"] is Color color)
        {
            BackColor = color;
        }

        Controls.Add(_designerRegion);
        // Now we must configure our designer to be at the correct location, and setup the autoscrolling for its container.
        _designerRegion.AutoScroll = true;
        _designerRegion.Dock = DockStyle.Fill;
    }

    /// <summary>
    ///  Returns the scroll offset for the scrollable control that manages all overlays. This is needed by the
    ///  BehaviorService so we can correctly invalidate our AdornerWindow based on scrollposition.
    /// </summary>
    internal Point AutoScrollPosition => _designerRegion.AutoScrollPosition;

    /// <summary>
    ///  Demand creates a ptr to the BehaviorService - we do this so we can route keyboard message to it.
    /// </summary>
    private BehaviorService? BehaviorService => _behaviorService ??= _designerSite.GetService<BehaviorService>();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_designer is not null)
            {
                Control designerHolder = _designer;
                _designer = null;
                designerHolder.Visible = false;
                designerHolder.Parent = null;
            }

            if (_splitter is not null)
            {
                _splitter.SplitterMoved -= OnSplitterMoved;
            }
        }

        base.Dispose(disposing);
    }

    private unsafe void ForceDesignerRedraw(bool focus)
    {
        if (_designer is not null && _designer.IsHandleCreated)
        {
            PInvokeCore.SendMessage(_designer, PInvokeCore.WM_NCACTIVATE, (WPARAM)(BOOL)focus);
            PInvoke.RedrawWindow(_designer, lprcUpdate: null, HRGN.Null, REDRAW_WINDOW_FLAGS.RDW_FRAME);
        }
    }

    /// <summary>
    ///  Initializes this frame with the given designer view.
    /// </summary>
    public void Initialize(Control view)
    {
        _designer = view;
        if (_designer is Form form)
        {
            form.TopLevel = false;
        }

        _designerRegion.Controls.Add(_designer);
        SyncDesignerUI();
        _designer.Visible = true;
        _designer.Enabled = true;
    }

    /// <summary>
    ///  When we get an lose focus, we need to make sure the form designer knows about it so it'll paint it's caption right.
    /// </summary>
    protected override void OnGotFocus(EventArgs e)
    {
        ForceDesignerRedraw(true);
        ISelectionService? selSvc = _designerSite.GetService<ISelectionService>();
        if (selSvc?.PrimarySelection is Control { IsDisposed: false } ctrl)
        {
            PInvoke.NotifyWinEvent(
                (uint)AccessibleEvents.Focus,
                ctrl,
                (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                (int)PInvoke.CHILDID_SELF);
        }
    }

    /// <summary>
    ///  When we get an lose focus, we need to make sure the form designer knows about it so it'll paint it's caption right.
    /// </summary>
    protected override void OnLostFocus(EventArgs e)
    {
        ForceDesignerRedraw(false);
    }

    private void OnSplitterMoved(object? sender, SplitterEventArgs e)
    {
        // Dirty the designer.
        if (_designerSite.TryGetService(out IComponentChangeService? changeService))
        {
            try
            {
                changeService.OnComponentChanging(_designerSite.Component);
                changeService.OnComponentChanged(_designerSite.Component);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }
        }
    }

    /// <summary>
    ///  We override this to do nothing. Otherwise, all the nice keyboard messages we want would get run through
    ///  the Form's keyboard handling procedure.
    /// </summary>
    protected override bool ProcessDialogKey(Keys keyData)
    {
        return false;
    }

    private void SyncDesignerUI()
    {
        Size selectionSize = DesignerUtils.GetAdornmentDimensions(AdornmentType.Maximum);
        _designerRegion.AutoScrollMargin = selectionSize;
        _designer!.Location = new Point(selectionSize.Width, selectionSize.Height);
        BehaviorService?.SyncSelection();
    }

    /// <summary>
    ///  Base wndProc. All messages are sent to wndProc after getting filtered through the preProcessMessage function.
    ///  Inheriting controls should call base.wndProc for any messages that they don't handle.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            // Provide MouseWheel access for scrolling
            case PInvokeCore.WM_MOUSEWHEEL:
                // Send a message to ourselves to scroll
                if (!_designerRegion._messageMouseWheelProcessed)
                {
                    _designerRegion._messageMouseWheelProcessed = true;
                    PInvokeCore.SendMessage(_designerRegion, PInvokeCore.WM_MOUSEWHEEL, m.WParamInternal, m.LParamInternal);
                    return;
                }

                break;
            // Provide keyboard access for scrolling
            case PInvokeCore.WM_KEYDOWN:
                Keys keycode = (Keys)(m.WParamInternal & 0xFFFF);
                (SCROLLBAR_COMMAND wScrollNotify, MessageId msg) = keycode switch
                {
                    Keys.Up => (SCROLLBAR_COMMAND.SB_LINEUP, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.Down => (SCROLLBAR_COMMAND.SB_LINEDOWN, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.PageUp => (SCROLLBAR_COMMAND.SB_PAGEUP, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.PageDown => (SCROLLBAR_COMMAND.SB_PAGEDOWN, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.Home => (SCROLLBAR_COMMAND.SB_TOP, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.End => (SCROLLBAR_COMMAND.SB_BOTTOM, (MessageId)PInvokeCore.WM_VSCROLL),
                    Keys.Left => (SCROLLBAR_COMMAND.SB_LINEUP, (MessageId)PInvokeCore.WM_HSCROLL),
                    Keys.Right => (SCROLLBAR_COMMAND.SB_LINEDOWN, (MessageId)PInvokeCore.WM_HSCROLL),
                    _ => ((SCROLLBAR_COMMAND)0, (MessageId)PInvokeCore.WM_NULL)
                };

                if (msg == PInvokeCore.WM_VSCROLL || msg == PInvokeCore.WM_HSCROLL)
                {
                    // Send a message to ourselves to scroll
                    PInvokeCore.SendMessage(_designerRegion, msg, (WPARAM)(int)wScrollNotify);
                    return;
                }

                break;
            case PInvokeCore.WM_CONTEXTMENU:
                PInvokeCore.SendMessage(_designer!, m.MsgInternal, m.WParamInternal, m.LParamInternal);
                return;
        }

        base.WndProc(ref m);
    }

    /// <summary>
    ///  Pushes the given control on top of the overlay list. This is a "push" operation, meaning that it forces
    ///  this control to the top of the existing overlay list.
    /// </summary>
    int IOverlayService.PushOverlay(Control control) => _designerRegion.PushOverlay(control);

    /// <summary>
    ///  Removes the given control from the overlay list. Unlike pushOverlay, this can remove a control from the
    ///  middle of the overlay list.
    /// </summary>
    void IOverlayService.RemoveOverlay(Control control)
    {
        _designerRegion.RemoveOverlay(control);
    }

    /// <summary>
    ///  Inserts the overlay.
    /// </summary>
    void IOverlayService.InsertOverlay(Control control, int index)
    {
        _designerRegion.InsertOverlay(control, index);
    }

    /// <summary>
    ///  Invalidate child overlays
    /// </summary>
    void IOverlayService.InvalidateOverlays(Rectangle screenRectangle)
    {
        _designerRegion.InvalidateOverlays(screenRectangle);
    }

    /// <summary>
    ///  Invalidate child overlays
    /// </summary>
    void IOverlayService.InvalidateOverlays(Region screenRegion)
    {
        _designerRegion.InvalidateOverlays(screenRegion);
    }

    /// <summary>
    ///  Requests the service to add a window 'pane'.
    /// </summary>
    void ISplitWindowService.AddSplitWindow(Control window)
    {
        if (_splitter is null)
        {
            _splitter = new Splitter();
            if (_uiService?.Styles["HorizontalResizeGrip"] is Color color)
            {
                _splitter.BackColor = color;
            }
            else
            {
                _splitter.BackColor = SystemColors.Control;
            }

            _splitter.BorderStyle = BorderStyle.Fixed3D;
            _splitter.Height = 7;
            _splitter.Dock = DockStyle.Bottom;
            _splitter.SplitterMoved += OnSplitterMoved;
        }

        SuspendLayout();
        window.Dock = DockStyle.Bottom;

        // Compute a minimum height for this window.
        int minHeight = 80;
        if (window.Height < minHeight)
        {
            window.Height = minHeight;
        }

        Controls.Add(_splitter);
        Controls.Add(window);
        ResumeLayout();
    }

    /// <summary>
    ///  Requests the service to remove a window 'pane'.
    /// </summary>
    void ISplitWindowService.RemoveSplitWindow(Control window)
    {
        SuspendLayout();
        Controls.Remove(window);
        Controls.Remove(_splitter);
        ResumeLayout();
    }

    /// <summary>
    ///  Returns IEnumerable of all windows which need to be themed when running inside VS We don't know how to do
    ///  theming here but we know which windows need to be themed. The two ScrollableControls that hold the designer
    ///  and the tray need to be themed, all of the children of the designed form should not be themed. The tray
    ///  contains only controls which are not visible in the user app but are visible inside VS. As a result, we
    ///  want to theme all windows within the tray but only the top window for the designer pane.
    /// </summary>
    IEnumerable IContainsThemedScrollbarWindows.ThemedScrollbarWindows()
    {
        List<ThemedScrollbarWindow> windows = new(Controls.Count);
        foreach (Control c in Controls)
        {
            ThemedScrollbarWindow windowInfo = new() { Handle = c.Handle };
            if (c is OverlayControl)
            {
                windowInfo.Mode = ThemedScrollbarMode.OnlyTopLevel;
            }
            else
            {
                windowInfo.Mode = ThemedScrollbarMode.All;
            }

            windows.Add(windowInfo);
        }

        return windows;
    }

    /// <summary>
    ///  This is a scrollable control that supports additional floating overlay controls.
    /// </summary>
    private class OverlayControl : ScrollableControl
    {
        private readonly List<Control> _overlayList;
        private readonly IServiceProvider _provider;
        internal bool _messageMouseWheelProcessed;
        private BehaviorService? _behaviorService;

        /// <summary>
        ///  Creates a new overlay control.
        /// </summary>
        public OverlayControl(IServiceProvider provider)
        {
            _provider = provider;
            _overlayList = [];
            AutoScroll = true;
            Text = "OverlayControl";
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new OverlayControlAccessibleObject(this);
        }

        /// <summary>
        ///  Demand creates a ptr to the BehaviorService
        /// </summary>
        private BehaviorService? BehaviorService => _behaviorService ??= _provider.GetService<BehaviorService>();

        /// <summary>
        ///  At handle creation time we request the designer's handle and parent it.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // Loop through all of the overlays, create them, and hook them up
            foreach (Control c in _overlayList)
            {
                ParentOverlay(c);
            }

            // We've reparented everything, which means that our selection UI is probably out of sync. Ask it to sync.
            BehaviorService?.SyncSelection();
        }

        /// <summary>
        ///  We override onLayout to provide our own custom layout functionality. This just overlaps all of the controls.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            Rectangle client = DisplayRectangle;

            // Loop through all of the overlays and size them. Also make sure that they are still on top of the
            // Z-order, because a handle recreate could have changed this.
            foreach (Control c in _overlayList)
            {
                c.Bounds = client;
            }
        }

        /// <summary>
        ///  Called to parent an overlay window into our document. This assumes that we call in reverse stack
        ///  order, as it always pushes to the top of the z-order.
        /// </summary>
        private void ParentOverlay(Control control)
        {
            PInvoke.SetParent(control, this);
            PInvoke.SetWindowPos(
                control,
                HWND.HWND_TOP,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
        }

        /// <summary>
        ///  Pushes the given control on top of the overlay list. This is a "push" operation, meaning that it
        ///  forces this control to the top of the existing overlay list.
        /// </summary>
        public int PushOverlay(Control control)
        {
            Debug.Assert(_overlayList.IndexOf(control) == -1, $"Duplicate overlay in overlay service: {control.GetType().FullName}");
            _overlayList.Add(control);
            // We need to have these components parented, but we don't want them to effect our layout.
            if (IsHandleCreated)
            {
                ParentOverlay(control);
                control.Bounds = DisplayRectangle;
            }

            return _overlayList.IndexOf(control);
        }

        /// <summary>
        ///  Removes the given control from the overlay list. Unlike pushOverlay, this can remove a control from
        ///  the middle of the overlay list.
        /// </summary>
        public void RemoveOverlay(Control control)
        {
            Debug.Assert(_overlayList.IndexOf(control) != -1, $"Control is not in overlay service: {control.GetType().FullName}");
            _overlayList.Remove(control);
            control.Visible = false;
            control.Parent = null;
        }

        /// <summary>
        ///  Inserts Overlay.
        /// </summary>
        public void InsertOverlay(Control control, int index)
        {
            Debug.Assert(_overlayList.IndexOf(control) == -1, $"Duplicate overlay in overlay service: {control.GetType().FullName}");
            Control c = _overlayList[index];
            RemoveOverlay(c);
            PushOverlay(control);
            PushOverlay(c);
            c.Visible = true;
        }

        /// <summary>
        ///  Invalidates overlays that intersect with the given section of the screen;
        /// </summary>
        public void InvalidateOverlays(Rectangle screenRectangle)
        {
            // paint in inverse order so that things at the front paint last.
            for (int i = _overlayList.Count - 1; i >= 0; i--)
            {
                Control overlayControl = _overlayList[i];
                Rectangle invalidateRect = new(overlayControl.PointToClient(screenRectangle.Location), screenRectangle.Size);
                if (overlayControl.ClientRectangle.IntersectsWith(invalidateRect))
                {
                    overlayControl.Invalidate(invalidateRect);
                }
            }
        }

        /// <summary>
        ///  Invalidates overlays that intersect with the given section of the screen;
        /// </summary>
        public void InvalidateOverlays(Region screenRegion)
        {
            // paint in inverse order so that things at the front paint last.
            for (int i = _overlayList.Count - 1; i >= 0; i--)
            {
                Control overlayControl = _overlayList[i];
                Rectangle overlayControlScreenBounds = overlayControl.Bounds;
                overlayControlScreenBounds.Location = overlayControl.PointToScreen(overlayControl.Location);
                using Region intersectionRegion = screenRegion.Clone();
                // get the intersection of everything on the screen that's invalidating and the overlaycontrol
                intersectionRegion.Intersect(overlayControlScreenBounds);
                // translate this down to overlay control coordinates.
                intersectionRegion.Translate(-overlayControlScreenBounds.X, -overlayControlScreenBounds.Y);
                overlayControl.Invalidate(intersectionRegion);
            }
        }

        /// <summary>
        ///  Need to know when child windows are created so we can properly set the Z-order
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.MsgInternal == PInvokeCore.WM_PARENTNOTIFY && m.WParamInternal.LOWORD == PInvokeCore.WM_CREATE)
            {
                bool ourWindow = false;
                foreach (Control c in _overlayList)
                {
                    if (c.IsHandleCreated && m.LParamInternal == c.Handle)
                    {
                        ourWindow = true;
                        break;
                    }
                }

                if (!ourWindow)
                {
                    foreach (Control c in _overlayList)
                    {
                        PInvoke.SetWindowPos(
                            c,
                            HWND.HWND_TOP,
                            0, 0, 0, 0,
                            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
                    }
                }
            }
            else if (m.Msg is (int)PInvokeCore.WM_VSCROLL or (int)PInvokeCore.WM_HSCROLL && BehaviorService is not null)
            {
                BehaviorService.SyncSelection();
            }
            else if ((m.Msg == (int)PInvokeCore.WM_MOUSEWHEEL))
            {
                _messageMouseWheelProcessed = false;
                BehaviorService?.SyncSelection();
            }
        }

        public class OverlayControlAccessibleObject : ControlAccessibleObject
        {
            public OverlayControlAccessibleObject(OverlayControl owner) : base(owner)
            {
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                // Since the SelectionUIOverlay in first in the z-order, it normally gets
                // returned from accHitTest. But we'd rather expose the form that is being
                // designed.
                //
                foreach (Control c in Owner!.Controls)
                {
                    AccessibleObject cao = c.AccessibilityObject;
                    if (cao.Bounds.Contains(x, y))
                    {
                        return cao;
                    }
                }

                return base.HitTest(x, y);
            }
        }
    }
}
