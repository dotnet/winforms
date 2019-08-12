// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class implements our design time document. This is the outer window that encompases a designer. It maintains a control hierarchy that looks like this:
    ///  DesignerFrame
    ///  ScrollableControl
    ///  Designer
    ///  Splitter
    ///  ScrollableControl
    ///  Component Tray
    ///  The splitter and second scrollable control are created on demand when a tray is added.
    /// </summary>
    internal class DesignerFrame : Control, IOverlayService, ISplitWindowService, IContainsThemedScrollbarWindows
    {
        private readonly ISite _designerSite;
        private readonly OverlayControl _designerRegion;
        private Splitter _splitter;
        private Control _designer;
        private BehaviorService _behaviorService;
        private readonly IUIService _uiService;

        /// <summary>
        ///  Initializes a new instance of the <see cref='System.Windows.Forms.Design.DesignerFrame'/> class.
        /// </summary>
        public DesignerFrame(ISite site)
        {
            Text = "DesignerFrame";
            _designerSite = site;
            _designerRegion = new OverlayControl(site);
            _uiService = _designerSite.GetService(typeof(IUIService)) as IUIService;
            if (_uiService != null)
            {
                if (_uiService.Styles["ArtboardBackground"] is Color)
                {
                    BackColor = (Color)_uiService.Styles["ArtboardBackground"];
                }
            }
            Controls.Add(_designerRegion);
            // Now we must configure our designer to be at the correct location, and setup the autoscrolling for its container.
            _designerRegion.AutoScroll = true;
            _designerRegion.Dock = DockStyle.Fill;
        }

        /// <summary>
        ///  Returns the scroll offset for the scrollable control that manages all overlays.  This is needed by the BehaviorService so we can correctly invalidate our AdornerWindow based on scrollposition.
        /// </summary>
        internal Point AutoScrollPosition
        {
            get => _designerRegion.AutoScrollPosition;
        }

        /// <summary>
        ///  Demand creates a ptr to the BehaviorService - we do this so we can route keyboard message to it.
        /// </summary>
        private BehaviorService BehaviorService
        {
            get
            {
                if (_behaviorService == null)
                {
                    _behaviorService = _designerSite.GetService(typeof(BehaviorService)) as BehaviorService;
                }
                return _behaviorService;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_designer != null)
                {
                    Control designerHolder = _designer;
                    _designer = null;
                    designerHolder.Visible = false;
                    designerHolder.Parent = null;
                }
                if (_splitter != null)
                {
                    _splitter.SplitterMoved -= new SplitterEventHandler(OnSplitterMoved);
                }
            }
            base.Dispose(disposing);
        }

        private void ForceDesignerRedraw(bool focus)
        {
            if (_designer != null && _designer.IsHandleCreated)
            {
                NativeMethods.SendMessage(_designer.Handle, WindowMessages.WM_NCACTIVATE, focus ? 1 : 0, 0);
                SafeNativeMethods.RedrawWindow(_designer.Handle, null, IntPtr.Zero, NativeMethods.RDW_FRAME);
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
            ISelectionService selSvc = (ISelectionService)_designerSite.GetService(typeof(ISelectionService));
            if (selSvc != null)
            {
                if (selSvc.PrimarySelection is Control ctrl && !ctrl.IsDisposed)
                {
                    UnsafeNativeMethods.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(ctrl, ctrl.Handle), NativeMethods.OBJID_CLIENT, 0);
                }
            }
        }

        /// <summary>
        ///  When we get an lose focus, we need to make sure the form designer knows about it so it'll paint it's caption right.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            ForceDesignerRedraw(false);
        }

        void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            // Dirty the designer.
            if (_designerSite.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                try
                {
                    cs.OnComponentChanging(_designerSite.Component, null);
                    cs.OnComponentChanged(_designerSite.Component, null, null, null);
                }
                catch
                {
                }
            }
        }

        void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Window && _designer != null)
            {
                SyncDesignerUI();
            }
        }

        /// <summary>
        ///  We override this to do nothing.  Otherwise, all the nice keyboard messages we want would get run through the Form's keyboard handling procedure.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            return false;
        }

        void SyncDesignerUI()
        {
            Size selectionSize = DesignerUtils.GetAdornmentDimensions(AdornmentType.Maximum);
            _designerRegion.AutoScrollMargin = selectionSize;
            _designer.Location = new Point(selectionSize.Width, selectionSize.Height);
            if (BehaviorService != null)
            {
                BehaviorService.SyncSelection();
            }
        }

        /// <summary>
        ///  Base wndProc. All messages are sent to wndProc after getting filtered through the preProcessMessage function. Inheriting controls should call base.wndProc for any messages that they don't handle.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Provide MouseWheel access for scrolling
                case WindowMessages.WM_MOUSEWHEEL:
                    // Send a message to ourselves to scroll
                    if (!_designerRegion._messageMouseWheelProcessed)
                    {
                        _designerRegion._messageMouseWheelProcessed = true;
                        NativeMethods.SendMessage(_designerRegion.Handle, WindowMessages.WM_MOUSEWHEEL, m.WParam, m.LParam);
                        return;
                    }
                    break;
                // Provide keyboard access for scrolling
                case WindowMessages.WM_KEYDOWN:
                    int wScrollNotify = 0;
                    int msg = 0;
                    int keycode = unchecked((int)(long)m.WParam) & 0xFFFF;
                    switch ((Keys)keycode)
                    {
                        case Keys.Up:
                            wScrollNotify = NativeMethods.SB_LINEUP;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.Down:
                            wScrollNotify = NativeMethods.SB_LINEDOWN;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.PageUp:
                            wScrollNotify = NativeMethods.SB_PAGEUP;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.PageDown:
                            wScrollNotify = NativeMethods.SB_PAGEDOWN;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.Home:
                            wScrollNotify = NativeMethods.SB_TOP;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.End:
                            wScrollNotify = NativeMethods.SB_BOTTOM;
                            msg = WindowMessages.WM_VSCROLL;
                            break;
                        case Keys.Left:
                            wScrollNotify = NativeMethods.SB_LINEUP;
                            msg = WindowMessages.WM_HSCROLL;
                            break;
                        case Keys.Right:
                            wScrollNotify = NativeMethods.SB_LINEDOWN;
                            msg = WindowMessages.WM_HSCROLL;
                            break;
                    }
                    if ((msg == WindowMessages.WM_VSCROLL)
                        || (msg == WindowMessages.WM_HSCROLL))
                    {
                        // Send a message to ourselves to scroll
                        NativeMethods.SendMessage(_designerRegion.Handle, msg, NativeMethods.Util.MAKELONG(wScrollNotify, 0), 0);
                        return;
                    }
                    break;
                case WindowMessages.WM_CONTEXTMENU:
                    NativeMethods.SendMessage(_designer.Handle, m.Msg, m.WParam, m.LParam);
                    return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        ///  Pushes the given control on top of the overlay list.  This is a "push" operation, meaning that it forces this control to the top of the existing overlay list.
        /// </summary>
        int IOverlayService.PushOverlay(Control control) => _designerRegion.PushOverlay(control);

        /// <summary>
        ///  Removes the given control from the overlay list.  Unlike pushOverlay, this can remove a control from the middle of the overlay list.
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
            if (_splitter == null)
            {
                _splitter = new Splitter();
                if (_uiService != null && _uiService.Styles["HorizontalResizeGrip"] is Color)
                {
                    _splitter.BackColor = (Color)_uiService.Styles["HorizontalResizeGrip"];
                }
                else
                {
                    _splitter.BackColor = SystemColors.Control;
                }
                _splitter.BorderStyle = BorderStyle.Fixed3D;
                _splitter.Height = 7;
                _splitter.Dock = DockStyle.Bottom;
                _splitter.SplitterMoved += new SplitterEventHandler(OnSplitterMoved);
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
        ///  Returns IEnumerable of all windows which need to be themed when running inside VS We don't know how to do theming here but we know which windows need to be themed.  The two ScrollableControls that hold the designer and the tray need to be themed, all of the children of the designed form should not be themed. The tray contains only conrols which are not visible in the user app but are visible inside VS. As a result, we want to theme all windows within the tray but only the top window for the designer pane.
        /// </summary>
        IEnumerable IContainsThemedScrollbarWindows.ThemedScrollbarWindows()
        {
            ArrayList windows = new ArrayList();
            foreach (Control c in Controls)
            {
                ThemedScrollbarWindow windowInfo = new ThemedScrollbarWindow { Handle = c.Handle };
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
            private readonly ArrayList _overlayList;
            private readonly IServiceProvider _provider;
            internal bool _messageMouseWheelProcessed;
            private BehaviorService _behaviorService;

            /// <summary>
            ///  Creates a new overlay control.
            /// </summary>
            public OverlayControl(IServiceProvider provider)
            {
                _provider = provider;
                _overlayList = new ArrayList();
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
            private BehaviorService BehaviorService
            {
                get
                {
                    if (_behaviorService == null)
                    {
                        _behaviorService = _provider.GetService(typeof(BehaviorService)) as BehaviorService;
                    }
                    return _behaviorService;
                }
            }

            /// <summary>
            ///  At handle creation time we request the designer's handle and parent it.
            /// </summary>
            protected override void OnCreateControl()
            {
                base.OnCreateControl();
                // Loop through all of the overlays, create them, and hook them up
                if (_overlayList != null)
                {
                    foreach (Control c in _overlayList)
                    {
                        ParentOverlay(c);
                    }
                }

                // We've reparented everything, which means that our selection UI is probably out of sync.  Ask it to sync.
                if (BehaviorService != null)
                {
                    BehaviorService.SyncSelection();
                }
            }

            /// <summary>
            ///  We override onLayout to provide our own custom layout functionality. This just overlaps all of the controls.
            /// </summary>
            protected override void OnLayout(LayoutEventArgs e)
            {
                base.OnLayout(e);
                Rectangle client = DisplayRectangle;

                // Loop through all of the overlays and size them.  Also make sure that they are still on top of the zorder, because a handle recreate could have changed this.
                if (_overlayList != null)
                {
                    foreach (Control c in _overlayList)
                    {
                        c.Bounds = client;
                    }
                }
            }

            /// <summary>
            ///  Called to parent an overlay window into our document.  This assumes that we call in reverse stack order, as it always pushes to the top of the z-order.
            /// </summary>
            private void ParentOverlay(Control control)
            {
                NativeMethods.SetParent(control.Handle, Handle);
                SafeNativeMethods.SetWindowPos(control.Handle, (IntPtr)NativeMethods.HWND_TOP, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
            }

            /// <summary>
            ///  Pushes the given control on top of the overlay list.  This is a "push" operation, meaning that it forces this control to the top of the existing overlay list.
            /// </summary>
            public int PushOverlay(Control control)
            {
                Debug.Assert(_overlayList.IndexOf(control) == -1, "Duplicate overlay in overlay service :" + control.GetType().FullName);
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
            ///  Removes the given control from the overlay list.  Unlike pushOverlay, this can remove a control from the middle of the overlay list.
            /// </summary>
            public void RemoveOverlay(Control control)
            {
                Debug.Assert(_overlayList.IndexOf(control) != -1, "Control is not in overlay service :" + control.GetType().FullName);
                _overlayList.Remove(control);
                control.Visible = false;
                control.Parent = null;
            }

            /// <summary>
            ///  Inserts Overlay.
            /// </summary>
            public void InsertOverlay(Control control, int index)
            {
                Debug.Assert(_overlayList.IndexOf(control) == -1, "Duplicate overlay in overlay service :" + control.GetType().FullName);
                Control c = (Control)_overlayList[index];
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
                    if (_overlayList[i] is Control overlayControl)
                    {
                        Rectangle invalidateRect = new Rectangle(overlayControl.PointToClient(screenRectangle.Location), screenRectangle.Size);
                        if (overlayControl.ClientRectangle.IntersectsWith(invalidateRect))
                        {
                            overlayControl.Invalidate(invalidateRect);
                        }
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
                    if (_overlayList[i] is Control overlayControl)
                    {
                        Rectangle overlayControlScreenBounds = overlayControl.Bounds;
                        overlayControlScreenBounds.Location = overlayControl.PointToScreen(overlayControl.Location);
                        using (Region intersectionRegion = screenRegion.Clone())
                        {
                            // get the intersection of everything on the screen that's invalidating and the overlaycontrol
                            intersectionRegion.Intersect(overlayControlScreenBounds);
                            // translate this down to overlay control coordinates.
                            intersectionRegion.Translate(-overlayControlScreenBounds.X, -overlayControlScreenBounds.Y);
                            overlayControl.Invalidate(intersectionRegion);
                        }
                    }
                }
            }
            /// <summary>
            ///  Need to know when child windows are created so we can properly set the Z-order
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == WindowMessages.WM_PARENTNOTIFY && NativeMethods.Util.LOWORD(unchecked((int)(long)m.WParam)) == (short)WindowMessages.WM_CREATE)
                {
                    if (_overlayList != null)
                    {
                        bool ourWindow = false;
                        foreach (Control c in _overlayList)
                        {
                            if (c.IsHandleCreated && m.LParam == c.Handle)
                            {
                                ourWindow = true;
                                break;
                            }
                        }

                        if (!ourWindow)
                        {
                            foreach (Control c in _overlayList)
                            {
                                SafeNativeMethods.SetWindowPos(c.Handle, (IntPtr)NativeMethods.HWND_TOP, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
                            }
                        }
                    }
                }
                else if ((m.Msg == WindowMessages.WM_VSCROLL || m.Msg == WindowMessages.WM_HSCROLL) && BehaviorService != null)
                {
                    BehaviorService.SyncSelection();
                }
                else if ((m.Msg == WindowMessages.WM_MOUSEWHEEL))
                {
                    _messageMouseWheelProcessed = false;
                    if (BehaviorService != null)
                    {
                        BehaviorService.SyncSelection();
                    }
                }
            }

            public class OverlayControlAccessibleObject : Control.ControlAccessibleObject
            {
                public OverlayControlAccessibleObject(OverlayControl owner) : base(owner)
                {
                }

                public override AccessibleObject HitTest(int x, int y)
                {
                    // Since the SelectionUIOverlay in first in the z-order, it normally gets
                    // returned from accHitTest. But we'd rather expose the form that is being
                    // designed.
                    //
                    foreach (Control c in Owner.Controls)
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
}
