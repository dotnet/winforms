// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Security.Permissions;
    using System.Reflection;
    using System.Windows.Forms.Layout;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using Microsoft.Win32;
    
    /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Defines a base class for controls that support auto-scrolling behavior.
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.ScrollableControlDesigner, " + AssemblyRef.SystemDesign)
    ]
    public class ScrollableControl : Control, IArrangedElement {
#if DEBUG        
        internal static readonly TraceSwitch AutoScrolling = new TraceSwitch("AutoScrolling", "Debug autoscrolling logic");
#else
        internal static readonly TraceSwitch AutoScrolling;
#endif

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollStateAutoScrolling"]/*' />
        /// <internalonly/>
        protected const int ScrollStateAutoScrolling     =  0x0001;
        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollStateHScrollVisible"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected const int ScrollStateHScrollVisible    =  0x0002;
        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollStateVScrollVisible"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected const int ScrollStateVScrollVisible    =  0x0004;
        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollStateUserHasScrolled"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected const int ScrollStateUserHasScrolled   =  0x0008;
        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollStateFullDrag"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected const int ScrollStateFullDrag          =  0x0010;

        
        private Size       userAutoScrollMinSize = System.Drawing.Size.Empty;
        /// <devdoc>
        ///     Current size of the displayRect.
        /// </devdoc>
        /// <internalonly/>
        private Rectangle   displayRect = Rectangle.Empty;
        /// <devdoc>
        ///     Current margins for autoscrolling.
        /// </devdoc>
        /// <internalonly/>
        private Size       scrollMargin = System.Drawing.Size.Empty;
        /// <devdoc>
        ///     User requested margins for autoscrolling.
        /// </devdoc>
        /// <internalonly/>
        private Size       requestedScrollMargin = System.Drawing.Size.Empty;
        /// <devdoc>
        ///     User requested autoscroll position - used for form creation only.
        /// </devdoc>
        /// <internalonly/>
        internal Point       scrollPosition = Point.Empty;

        private DockPaddingEdges dockPadding = null;

        private int         scrollState;
        
        VScrollProperties verticalScroll = null;
        HScrollProperties horizontalScroll = null;
        private static readonly object EVENT_SCROLL = new object();


        // Used to figure out what the horizontal scroll value should be set to when 
        // the horizontal scrollbar is first shown
        private bool resetRTLHScrollValue = false;

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollableControl"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ScrollableControl'/> class.
        ///    </para>
        /// </devdoc>
        public ScrollableControl()
        : base() {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            SetScrollState(ScrollStateAutoScrolling, false);
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.AutoScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value
        ///       indicating whether the container will allow the user to scroll to any
        ///       controls placed outside of its visible boundaries.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.FormAutoScrollDescr))
        ]
        public virtual bool AutoScroll {
            get {
                return GetScrollState(ScrollStateAutoScrolling);
            }

            set {
                if (value) {
                    UpdateFullDrag();
                }

                SetScrollState(ScrollStateAutoScrolling, value);                
                LayoutTransaction.DoLayout(this, this, PropertyNames.AutoScroll);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.AutoScrollMargin"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets the size of the auto-scroll
        ///       margin.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.FormAutoScrollMarginDescr))
        ]
        public Size AutoScrollMargin {
            get {
                return requestedScrollMargin;
            }

            set {
                if (value.Width < 0 || value.Height < 0) {
                    throw new ArgumentOutOfRangeException(nameof(AutoScrollMargin), string.Format(SR.InvalidArgument, "AutoScrollMargin", value.ToString()));
                }
                SetAutoScrollMargin(value.Width, value.Height);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.AutoScrollPosition"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the location of the auto-scroll position.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormAutoScrollPositionDescr))        
        ]
        public Point AutoScrollPosition {
            get {
                Rectangle rect = GetDisplayRectInternal();
                return new Point(rect.X, rect.Y);
            }

            set {
                if (Created) {
                    SetDisplayRectLocation(-value.X, -value.Y);
                    SyncScrollbars(true);
                }

                scrollPosition = value;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.AutoScrollMinSize"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the mimimum size of the auto-scroll.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.FormAutoScrollMinSizeDescr))
        ]
        public Size AutoScrollMinSize {
            get {
                return userAutoScrollMinSize;
            }

            set {
                if (value != userAutoScrollMinSize) {
                    userAutoScrollMinSize = value;
                    AutoScroll = true;
                    PerformLayout();
                }
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Retrieves the CreateParams used to create the window.
        ///       If a subclass overrides this function, it must call the base implementation.
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;

                if (HScroll || HorizontalScroll.Visible) {
                    cp.Style |= NativeMethods.WS_HSCROLL;
                }
                else {
                    cp.Style &= (~NativeMethods.WS_HSCROLL);
                }
                if (VScroll || VerticalScroll.Visible) {
                    cp.Style |= NativeMethods.WS_VSCROLL;
                }
                else {
                    cp.Style &= (~NativeMethods.WS_VSCROLL);
                }

                return cp;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DisplayRectangle"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Retreives the current display rectangle. The display rectangle
        ///       is the virtual display area that is used to layout components.
        ///       The position and dimensions of the Form's display rectangle
        ///       change during autoScroll.
        ///    </para>
        /// </devdoc>
        public override Rectangle DisplayRectangle {
            [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
            get {
                Rectangle rect = base.ClientRectangle;
                if (!displayRect.IsEmpty) {
                    rect.X = displayRect.X;
                    rect.Y = displayRect.Y;
                    if (HScroll) {
                        rect.Width = displayRect.Width;
                    }
                    if (VScroll) {
                        rect.Height = displayRect.Height;
                    }
                }
                return LayoutUtils.DeflateRect(rect, Padding);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.IArrangedElement.DisplayRectangle"]/*' />
        /// <internalonly/>
        Rectangle IArrangedElement.DisplayRectangle {
            get {
                Rectangle displayRectangle = this.DisplayRectangle;
                // V7#79 : Controls anchored the bottom of their container may disappear (be scrunched)
                //       when scrolling is used.
                if(AutoScrollMinSize.Width != 0 && AutoScrollMinSize.Height != 0) {
                    displayRectangle.Width = Math.Max(displayRectangle.Width, AutoScrollMinSize.Width);
                    displayRectangle.Height = Math.Max(displayRectangle.Height, AutoScrollMinSize.Height);
                }
                return displayRectangle;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.HScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets a value indicating whether the horizontal scroll bar is visible.
        ///    </para>
        /// </devdoc>
        protected bool HScroll {
            get {
                return GetScrollState(ScrollStateHScrollVisible);
            }
            set { 
                SetScrollState(ScrollStateHScrollVisible, value);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.HorizontalScroll"]/*' />
        /// <devdoc>
        ///    <para>Gets the Horizontal Scroll bar for this ScrollableControl.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ScrollableControlHorizontalScrollDescr)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public HScrollProperties HorizontalScroll {
            get {
                if (horizontalScroll == null) {
                    horizontalScroll = new HScrollProperties(this);
                }
                return  horizontalScroll;
            }
        }


       


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.VScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets a value indicating whether the vertical scroll bar is visible.
        ///    </para>
        /// </devdoc>
        protected bool VScroll {
            get { 
                return GetScrollState(ScrollStateVScrollVisible);
            }
            set {
                SetScrollState(ScrollStateVScrollVisible, value);
            }
        }


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.VerticalScroll"]/*' />
        /// <devdoc>
        ///    <para>Gets the Veritcal Scroll bar for this ScrollableControl.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ScrollableControlVerticalScrollDescr)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public VScrollProperties VerticalScroll {
            get {
                if (verticalScroll == null) {
                    verticalScroll = new VScrollProperties(this);
                }
                return  verticalScroll;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPadding"]/*' />
        /// <devdoc>
        ///    <para>Gets the dock padding settings for all
        ///       edges of the control.</para>
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public DockPaddingEdges DockPadding {
            get {
                if (dockPadding == null) {
                    dockPadding = new DockPaddingEdges(this);
                }
                return dockPadding;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.AdjustFormScrollbars"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Adjusts
        ///       the auto-scroll bars on the container based on the current control
        ///       positions and the control currently selected.
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void AdjustFormScrollbars(bool displayScrollbars) {
            bool needLayout = false;

            Rectangle display = GetDisplayRectInternal();

            if (!displayScrollbars && (HScroll || VScroll)) {
                needLayout = SetVisibleScrollbars(false, false);
            }

            if (!displayScrollbars) {
                Rectangle client = ClientRectangle;
                display.Width = client.Width;
                display.Height = client.Height;
            }
            else {
                needLayout |= ApplyScrollbarChanges(display);
            }

            if (needLayout) {
                LayoutTransaction.DoLayout(this, this, PropertyNames.DisplayRectangle);
            }
        }
        
        private bool ApplyScrollbarChanges(Rectangle display) {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, GetType().Name + "::ApplyScrollbarChanges(" + display + ") {");
            Debug.Indent();

            bool needLayout = false;
            bool needHscroll = false;
            bool needVscroll = false;
            Rectangle currentClient = ClientRectangle;
            Rectangle fullClient = currentClient;
            Rectangle minClient = fullClient;
            if (HScroll) {
                fullClient.Height += SystemInformation.HorizontalScrollBarHeight;
            }
            else {
                minClient.Height -= SystemInformation.HorizontalScrollBarHeight;
            }
            
            if (VScroll) {
                fullClient.Width += SystemInformation.VerticalScrollBarWidth;
            }
            else {
                minClient.Width -= SystemInformation.VerticalScrollBarWidth;
            }
            
            int maxX = minClient.Width;
            int maxY = minClient.Height;

            if (Controls.Count != 0) {

                // Compute the actual scroll margins (take into account docked
                // things.)
                //
                scrollMargin = requestedScrollMargin;
                
                if (dockPadding != null) {
                    scrollMargin.Height += Padding.Bottom;
                    scrollMargin.Width += Padding.Right;
                }
                
                for (int i=0; i<Controls.Count; i++) {
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
                    //
                    if (current != null && current.GetState(STATE_VISIBLE)) {
                        switch (((Control)current).Dock) {
                            case DockStyle.Bottom:
                                scrollMargin.Height += current.Size.Height;
                                break;
                            case DockStyle.Right:
                                scrollMargin.Width += current.Size.Width;
                                break;
                        }
                    }
                }
            }

            if (!userAutoScrollMinSize.IsEmpty) {
                maxX = userAutoScrollMinSize.Width + scrollMargin.Width;
                maxY = userAutoScrollMinSize.Height + scrollMargin.Height;
                needHscroll = true;
                needVscroll = true;
            }


            bool defaultLayoutEngine = (LayoutEngine == DefaultLayout.Instance);
            
            if (!defaultLayoutEngine && CommonProperties.HasLayoutBounds(this)) {

                Size layoutBounds = CommonProperties.GetLayoutBounds(this);

                if (layoutBounds.Width > maxX) {
                    needHscroll = true;
                    maxX = layoutBounds.Width;
                }
                if (layoutBounds.Height > maxY) {
                    needVscroll = true;
                    maxY = layoutBounds.Height;
                }                
            }
            else if (Controls.Count != 0) {
                
                // Compute the dimensions of the display rect
                //
                for (int i=0; i < Controls.Count; i++) {
                    bool watchHoriz = true;
                    bool watchVert = true;

                    Control current = Controls[i];

                    // Same logic as the margin calc - you need to see if the
                    // control *will* be visible... 
                    //
                    if (current != null && current.GetState(STATE_VISIBLE)) {
                        if (defaultLayoutEngine) {
                            Control richCurrent = (Control)current;

                            switch (richCurrent.Dock) {
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
                                    if ((anchor & AnchorStyles.Right) == AnchorStyles.Right) {
                                        watchHoriz = false;
                                    }
                                    if ((anchor & AnchorStyles.Left) != AnchorStyles.Left) {
                                        watchHoriz = false;
                                    }
                                    if ((anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom) {
                                        watchVert = false;
                                    }
                                    if ((anchor & AnchorStyles.Top) != AnchorStyles.Top) {
                                        watchVert = false;
                                    }
                                    break;
                            }
                        }

                        if (watchHoriz || watchVert) {
                            Rectangle bounds = current.Bounds;
                            int ctlRight = -display.X + bounds.X + bounds.Width + scrollMargin.Width;
                            int ctlBottom = -display.Y + bounds.Y + bounds.Height + scrollMargin.Height;

                            if (!defaultLayoutEngine)
                            {
                                ctlRight += current.Margin.Right;
                                ctlBottom += current.Margin.Bottom;
                            }                            

                            if (ctlRight > maxX && watchHoriz) {
                                needHscroll = true;
                                maxX = ctlRight;
                            }
                            if (ctlBottom > maxY && watchVert) {
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
            //
            if (maxX <= fullClient.Width) {
                needHscroll = false;
            }
            if (maxY <= fullClient.Height) {
                needVscroll = false;
            }
            Rectangle clientToBe = fullClient;
            if (needHscroll) {
                clientToBe.Height -= SystemInformation.HorizontalScrollBarHeight;
            }
            if (needVscroll) {
                clientToBe.Width -= SystemInformation.VerticalScrollBarWidth;
            }
            if (needHscroll && maxY > clientToBe.Height) {
                needVscroll = true;
            }
            if (needVscroll && maxX > clientToBe.Width) {
                needHscroll = true;
            }
            if (!needHscroll) {
                maxX = clientToBe.Width;
            }
            if (!needVscroll) {
                maxY = clientToBe.Height;
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "Current scrollbars(" + HScroll + ", " + VScroll + ")");
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "Needed  scrollbars(" + needHscroll + ", " + needVscroll + ")");

            // Show the needed scrollbars
            //
            needLayout = (SetVisibleScrollbars(needHscroll, needVscroll) || needLayout);

            // If needed, adjust the size...
            //
            if (HScroll || VScroll) {
                needLayout = (SetDisplayRectangleSize(maxX, maxY) || needLayout);
            }
            // Else just update the display rect size... this keeps it as big as the client
            // area in a resize scenario
            //
            else {
                SetDisplayRectangleSize(maxX, maxY);
            }

            // Sync up the scrollbars
            //
            SyncScrollbars(true);

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, needLayout ? "Need layout" : "No layout changes");
            Debug.Unindent();
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "}");
            return needLayout;
        }

        private Rectangle GetDisplayRectInternal() {
            if (displayRect.IsEmpty) {
                displayRect = ClientRectangle;
            }
            if (!AutoScroll && this.HorizontalScroll.visible == true) {
                displayRect = new Rectangle(displayRect.X, displayRect.Y, this.HorizontalScroll.Maximum, this.displayRect.Height); 
            }
            if (!AutoScroll && this.VerticalScroll.visible == true) {
                displayRect = new Rectangle(displayRect.X, displayRect.Y, this.displayRect.Width, this.VerticalScroll.Maximum); 
            }
            return displayRect;
        }


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.GetScrollState"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Tests a given scroll state bit to determine if it is set.
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected bool GetScrollState(int bit) {
            return(bit & scrollState) == bit;
        }
      

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.OnLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Forces the layout of any docked or anchored child controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLayout(LayoutEventArgs levent) {

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
            // isn't any noticible flicker, but this could be a perf problem...
            //
            if (levent.AffectedControl != null && AutoScroll) {
                base.OnLayout(levent);
            }
			
            AdjustFormScrollbars(AutoScroll);
            base.OnLayout(levent);
            
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.OnMouseWheel"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     Handles mouse wheel processing for our scrollbars.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseWheel(MouseEventArgs e) {

            // Favor the vertical scroll bar, since it's the most
            // common use.  However, if there isn't a vertical 
            // scroll and the horizontal is on, then wheel it around.
            //
            if (VScroll) {
                Rectangle client = ClientRectangle;
                int pos = -displayRect.Y;
                int maxPos = -(client.Height - displayRect.Height);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(displayRect.X, -pos);
                SyncScrollbars(AutoScroll);
                if (e is HandledMouseEventArgs)
                {
                    ((HandledMouseEventArgs)e).Handled = true;
                }
            }
            else if (HScroll) {
                Rectangle client = ClientRectangle;
                int pos = -displayRect.X;
                int maxPos = -(client.Width - displayRect.Width);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(-pos, displayRect.Y);
                SyncScrollbars(AutoScroll);
                if (e is HandledMouseEventArgs)
                {
                    ((HandledMouseEventArgs)e).Handled = true;
                }
            }

            // The base implementation should be called before the implementation above,
            // but changing the order in Whidbey would be too much of a breaking change
            // for this particular class.
            base.OnMouseWheel(e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnRightToLeftChanged(EventArgs e) {
            base.OnRightToLeftChanged(e);
            resetRTLHScrollValue = true;
            // When the page becomes visible, we need to call OnLayout to adjust the scrollbars.
            LayoutTransaction.DoLayout(this, this, PropertyNames.RightToLeft);
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.OnPaintBackground"]/*' />
        protected override void OnPaintBackground (PaintEventArgs e) {
            if ((HScroll || VScroll) &&
                BackgroundImage != null && 
                (BackgroundImageLayout == ImageLayout.Zoom || BackgroundImageLayout == ImageLayout.Stretch || BackgroundImageLayout == ImageLayout.Center)) {
                    if (ControlPaint.IsImageTransparent(BackgroundImage)) {
                        PaintTransparentBackground(e, displayRect);
                    }
                    ControlPaint.DrawBackgroundImage(e.Graphics, BackgroundImage, BackColor, BackgroundImageLayout, displayRect, displayRect, displayRect.Location);
            }
            else {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaddingChanged(EventArgs e) {
            // DockPaddingEdges compat.  
            // dont call base in this instance - for App compat we should not fire Invalidate when 
            // the padding has changed.
            EventHandler handler = (EventHandler)Events[Control.EventPaddingChanged];
            if (handler != null) handler(this, e);
        }


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.OnVisibleChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnVisibleChanged(EventArgs e) {
            if (Visible) {
                // When the page becomes visible, we need to call OnLayout to adjust the scrollbars.
                LayoutTransaction.DoLayout(this, this, PropertyNames.Visible);
            }
            base.OnVisibleChanged(e);
        }

        // internal for Form to call
        //
        internal void ScaleDockPadding(float dx, float dy) {
            if (dockPadding != null) {
                dockPadding.Scale(dx, dy);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScaleCore"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy) {
            ScaleDockPadding(dx, dy);
            base.ScaleCore(dx, dy);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ScaleControl"]/*' />
        /// <devdoc>
        ///     Scale this form.  Form overrides this to enforce a maximum / minimum size.
        /// </devdoc>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            ScaleDockPadding(factor.Width, factor.Height);
            base.ScaleControl(factor, specified);
        }

        // internal for the respective scroll bars to call so that the Display Rectangle is set to 
        // enable the visual scroll effect.
        //
        internal void SetDisplayFromScrollProps(int x, int y)
        {
          Rectangle display = GetDisplayRectInternal();
          ApplyScrollbarChanges(display);
          SetDisplayRectLocation(x, y);
        }
        
        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.SetDisplayRectLocation"]/*' />
        /// <devdoc>
        ///    Adjusts the displayRect to be at the offset x, y. The contents of the
        ///    Form is scrolled using Windows.ScrollWindowEx.
        /// </devdoc>
        //
        // 

        protected void SetDisplayRectLocation(int x, int y) {
            int xDelta = 0;
            int yDelta = 0;


            Rectangle client = ClientRectangle;
            // The DisplayRect property modifies
            // the returned rect to include padding.  We don't want to
            // include this padding in our adjustment of the DisplayRect
            // because it interferes with the scrolling.
            Rectangle displayRectangle = displayRect;
            int minX = Math.Min(client.Width - displayRectangle.Width, 0);
            int minY = Math.Min(client.Height - displayRectangle.Height, 0);

            if (x > 0) {
                x = 0;
            }
            if (y > 0) {
                y = 0;
            }
            if (x < minX) {
                x = minX;
            }
            if (y < minY) {
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
			displayRect.X = x;
			displayRect.Y = y;

            if (xDelta != 0 || yDelta != 0 && IsHandleCreated) {
                Rectangle cr = ClientRectangle;
                NativeMethods.RECT rcClip = NativeMethods.RECT.FromXYWH(cr.X, cr.Y, cr.Width, cr.Height);
                NativeMethods.RECT rcUpdate = NativeMethods.RECT.FromXYWH(cr.X, cr.Y, cr.Width, cr.Height);
                SafeNativeMethods.ScrollWindowEx(new HandleRef(this, Handle), xDelta, yDelta, 
                                                 null,
                                                 ref rcClip, 
                                                 NativeMethods.NullHandleRef, 
                                                 ref rcUpdate,
                                                 NativeMethods.SW_INVALIDATE
                                                 | NativeMethods.SW_ERASE
                                                 | NativeMethods.SW_SCROLLCHILDREN);
            }

			

            // Force child controls to update bounds.
            //
            for (int i=0; i<Controls.Count; i++) {
                Control ctl = Controls[i];
                if (ctl != null && ctl.IsHandleCreated) {
                    ctl.UpdateBounds();
                }
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.ScrollControlIntoView"]/*' />
        /// <devdoc>
        ///    Scrolls the currently active control into view if we are an AutoScroll
        ///    Form that has the Horiz or Vert scrollbar displayed...
        /// </devdoc>
        public void ScrollControlIntoView(Control activeControl) {
            Debug.WriteLineIf(ScrollableControl.AutoScrolling.TraceVerbose, "ScrollControlIntoView(" + activeControl.GetType().FullName + ")");
            Debug.Indent();

            Rectangle client = ClientRectangle;
          
            if (IsDescendant(activeControl)
                && AutoScroll
                && (HScroll || VScroll)
                && activeControl != null
                && (client.Width > 0 && client.Height > 0)) {

                Debug.WriteLineIf(ScrollableControl.AutoScrolling.TraceVerbose, "Calculating...");
            

                Point scrollLocation = ScrollToControl(activeControl);
                

                SetScrollState(ScrollStateUserHasScrolled, false);
                SetDisplayRectLocation(scrollLocation.X, scrollLocation.Y);
                SyncScrollbars(true);
            }

            Debug.Unindent();
        }


        /// <devdoc> Allow containers to tweak autoscrolling.  when you tab between controls contained in the scrollable control
        /// this allows you to set the scroll location.  This would allow you to scroll to the middle of a control, where as the default is 
        /// the top of the control.

        ///
        /// Additionally there is a new AutoScrollOffset property on the child controls themselves.  This lets them control where they want to 
        /// be scrolled to.  E.g. In SelectedIndexChanged for a ListBox, you could do:
        /// 
        /// listBox1.AutoScrollOffset = parent.AutoScrollPosition; 
        ///
        /// </devdoc>
        protected virtual Point ScrollToControl(Control activeControl) {

            Rectangle client = ClientRectangle;
            int xCalc = displayRect.X;
            int yCalc = displayRect.Y;
            int xMargin = scrollMargin.Width;
            int yMargin = scrollMargin.Height;

            Rectangle bounds = activeControl.Bounds;
            if (activeControl.ParentInternal != this) {
                Debug.WriteLineIf(ScrollableControl.AutoScrolling.TraceVerbose, "not direct child, original bounds: " + bounds);
                bounds = this.RectangleToClient(activeControl.ParentInternal.RectangleToScreen(bounds));
            }
            Debug.WriteLineIf(ScrollableControl.AutoScrolling.TraceVerbose, "adjusted bounds: " + bounds);

            if (bounds.X < xMargin) {
                xCalc = displayRect.X + xMargin - bounds.X;
            }
            else if (bounds.X + bounds.Width + xMargin > client.Width) {

                xCalc = client.Width - (bounds.X + bounds.Width + xMargin - displayRect.X);

                if (bounds.X + xCalc - displayRect.X < xMargin) {
                    xCalc = displayRect.X + xMargin - bounds.X;
                }
            }

            if (bounds.Y < yMargin) {
                yCalc = displayRect.Y + yMargin - bounds.Y;
            }
            else if (bounds.Y + bounds.Height + yMargin > client.Height) {

                yCalc = client.Height - (bounds.Y + bounds.Height + yMargin - displayRect.Y);

                if (bounds.Y + yCalc - displayRect.Y < yMargin) {
                    yCalc = displayRect.Y + yMargin - bounds.Y;
                }
            }

            xCalc += activeControl.AutoScrollOffset.X;
            yCalc += activeControl.AutoScrollOffset.Y;

            return new Point(xCalc, yCalc);

        }
        private int ScrollThumbPosition(int fnBar) {
            NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
            si.fMask = NativeMethods.SIF_TRACKPOS;
            SafeNativeMethods.GetScrollInfo(new HandleRef(this, Handle), fnBar, si);
            return si.nTrackPos;
        }


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.Scroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the scroll box has been moved by either a mouse or keyboard action.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ScrollBarOnScrollDescr))]
        public event ScrollEventHandler Scroll {
            add {
                Events.AddHandler(EVENT_SCROLL, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SCROLL, value);
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.OnScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.ScrollBar.OnScroll'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnScroll(ScrollEventArgs se) {
            ScrollEventHandler handler = (ScrollEventHandler)Events[EVENT_SCROLL];
            if (handler != null) handler(this,se);
        }

	    private void ResetAutoScrollMargin() {
		    AutoScrollMargin = Size.Empty;
		}

	    private void ResetAutoScrollMinSize() {
		    AutoScrollMinSize = Size.Empty;
		}

        private void ResetScrollProperties(ScrollProperties scrollProperties) {
            // Set only these two values as when the ScrollBars are not visible ...
            // there is no meaning of the "value" property.
            scrollProperties.visible = false;
            scrollProperties.value = 0;
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.SetAutoScrollMargin"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the size
        ///       of the auto-scroll margins.
        ///    </para>
        /// </devdoc>
        public void SetAutoScrollMargin(int x, int y) {
            // Make sure we're not setting the margins to negative numbers
            if (x < 0) {
                x = 0;
            }
            if (y < 0) {
                y = 0;
            }

            if (x != requestedScrollMargin.Width
                || y != requestedScrollMargin.Height) {

                requestedScrollMargin = new Size(x, y);
                if (AutoScroll) {
                    PerformLayout();
                }
            }
        }


        /// <devdoc>
        ///     Actually displays or hides the horiz and vert autoscrollbars. This will
        ///     also adjust the values of formState to reflect the new state
        /// </devdoc>
        private bool SetVisibleScrollbars(bool horiz, bool vert) {
            bool needLayout = false;

            if (!horiz && HScroll
                || horiz && !HScroll
                || !vert && VScroll
                || vert && !VScroll) {

                needLayout = true;
            }

            // If we are about to show the horizontal scrollbar, then
            // set this flag, so that we can set the right initial value
            // based on whether we are right to left.
            if (horiz && !HScroll && (RightToLeft == RightToLeft.Yes)) {
                resetRTLHScrollValue = true;
            }

            
            if (needLayout) {
                int x = displayRect.X;
                int y = displayRect.Y;
                if (!horiz) {
                    x = 0;
                }
                if (!vert) {
                    y = 0;
                }
                SetDisplayRectLocation(x, y);
                SetScrollState(ScrollStateUserHasScrolled, false);
                HScroll = horiz;
                VScroll = vert;
                //Update the visible member of ScrollBars....
                if (horiz)
                {
                    HorizontalScroll.visible = true;
                }
                else
                {
                    ResetScrollProperties(HorizontalScroll);
                }
                if (vert)
                {
                    VerticalScroll.visible = true;
                }
                else
                {
                    ResetScrollProperties(VerticalScroll);
                }
                UpdateStyles();
            }
            return needLayout;
        }

        /// <devdoc>
        ///     Sets the width and height of the virtual client area used in
        ///     autoscrolling. This will also adjust the x and y location of the
        ///     virtual client area if the new size forces it.
        /// </devdoc>
        /// <internalonly/>
        private bool SetDisplayRectangleSize(int width, int height) {
            bool needLayout = false;

            if (displayRect.Width != width
                || displayRect.Height != height) {

                displayRect.Width = width;
                displayRect.Height = height;
                needLayout = true;
            }

            int minX = ClientRectangle.Width - width;
            int minY = ClientRectangle.Height - height;

            if (minX > 0) minX = 0;
            if (minY > 0) minY = 0;

            int x = displayRect.X;
            int y = displayRect.Y;

            if (!HScroll) {
                x = 0;
            }
            if (!VScroll) {
                y = 0;
            }

            if (x < minX) {
                x = minX;
            }
            if (y < minY) {
                y = minY;
            }
            SetDisplayRectLocation(x, y);

            return needLayout;
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.SetScrollState"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets a given scroll state bit.
        ///    </para>
        /// </devdoc>
        protected void SetScrollState(int bit, bool value) {
            if (value) {
                scrollState |= bit;
            }
            else {
                scrollState &= (~bit);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the <see cref='System.Windows.Forms.ScrollableControl.AutoScrollPosition'/>
        ///       property should be persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeAutoScrollPosition() {
            if (AutoScroll) {
                Point pt = AutoScrollPosition;
                if (pt.X != 0 || pt.Y != 0) {
                    return true;
                }
            }
            return false;
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the <see cref='System.Windows.Forms.ScrollableControl.AutoScrollMargin'/> property should be persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeAutoScrollMargin() {
            return !AutoScrollMargin.Equals(new Size(0,0));
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the <see cref='System.Windows.Forms.ScrollableControl.AutoScrollMinSize'/>
        ///       property should be persisted.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeAutoScrollMinSize() {
            return !AutoScrollMinSize.Equals(new Size(0,0));
        }


        /// <devdoc>
        ///     Updates the value of the autoscroll scrollbars based on the current form
        ///     state. This is a one-way sync, updating the scrollbars only.
        /// </devdoc>
        /// <internalonly/>
        private void SyncScrollbars(bool autoScroll) {
            Rectangle displayRect = this.displayRect;

            if (autoScroll) {
                if (!IsHandleCreated) {
                    return;
                }
                
                if (HScroll) {
                    if (!HorizontalScroll.maximumSetExternally) {
                        HorizontalScroll.maximum = displayRect.Width-1;
                    }
                    if (!HorizontalScroll.largeChangeSetExternally) {
                        HorizontalScroll.largeChange = ClientRectangle.Width;
                    }
                    if (!HorizontalScroll.smallChangeSetExternally) {
                        HorizontalScroll.smallChange = 5;
                    }
                    if (resetRTLHScrollValue && !IsMirrored) {
                        resetRTLHScrollValue = false;
                        BeginInvoke(new EventHandler(this.OnSetScrollPosition));
                    }
                    else if(-displayRect.X >= HorizontalScroll.minimum && -displayRect.X < HorizontalScroll.maximum) {
                        HorizontalScroll.value = -displayRect.X;
                    }                    
                    HorizontalScroll.UpdateScrollInfo ();                    
                }
                if (VScroll) {
                    if (!VerticalScroll.maximumSetExternally) {
                        VerticalScroll.maximum = displayRect.Height-1;
                    }
                    if (!VerticalScroll.largeChangeSetExternally) {
                        VerticalScroll.largeChange = ClientRectangle.Height;
                    }
                    if (!VerticalScroll.smallChangeSetExternally) {
                        VerticalScroll.smallChange = 5;
                    }
                    if (-displayRect.Y >= VerticalScroll.minimum && -displayRect.Y < VerticalScroll.maximum) {
                        VerticalScroll.value = -displayRect.Y;
                    }
                    VerticalScroll.UpdateScrollInfo ();                    
                }
            }
            else {
                if (this.HorizontalScroll.Visible) {
                    HorizontalScroll.Value = -displayRect.X;
                }
                else {
                    ResetScrollProperties(HorizontalScroll);
                }
                if (this.VerticalScroll.Visible) {
                    VerticalScroll.Value = -displayRect.Y;
                }
                else {
                    ResetScrollProperties(VerticalScroll);
                }
            }
        }

        private void OnSetScrollPosition(object sender, EventArgs e) {
            if (!IsMirrored) {
                SendMessage(NativeMethods.WM_HSCROLL, 
                            NativeMethods.Util.MAKELPARAM((RightToLeft == RightToLeft.Yes) ? NativeMethods.SB_RIGHT : NativeMethods.SB_LEFT,0), 0);
            }
        }
        

        /// <devdoc>
        ///     Queries the system to determine the users preference for full drag
        ///     of windows.
        /// </devdoc>
        private void UpdateFullDrag() {
            SetScrollState(ScrollStateFullDrag, SystemInformation.DragFullWindows);
        }

        /// <devdoc>
        ///     WM_VSCROLL handler
        /// </devdoc>
        /// <internalonly/>
        private void WmVScroll(ref Message m) {

            // The lparam is handle of the sending scrollbar, or NULL when
            // the scrollbar sending the message is the "form" scrollbar...
            //
            if (m.LParam != IntPtr.Zero) {
                base.WndProc(ref m);
                return;
            }

            Rectangle client = ClientRectangle;
            bool thumbTrack = NativeMethods.Util.LOWORD(m.WParam) != NativeMethods.SB_THUMBTRACK;
            int pos = -displayRect.Y;
            int oldValue = pos;

            int maxPos = -(client.Height - displayRect.Height);
            if (!AutoScroll) {
                maxPos = this.VerticalScroll.Maximum;
            }            
            
            switch (NativeMethods.Util.LOWORD(m.WParam)) {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    pos = ScrollThumbPosition(NativeMethods.SB_VERT);                    
                    break;
                case NativeMethods.SB_LINEUP:
                    if (pos > 0) {
                        pos-=VerticalScroll.SmallChange;
                    }
                    else {
                        pos = 0;
                    }
                    break;
                case NativeMethods.SB_LINEDOWN:
                    if (pos < maxPos-VerticalScroll.SmallChange) {
                        pos+=VerticalScroll.SmallChange;
                    }
                    else {
                        pos = maxPos;
                    }
                    break;
                case NativeMethods.SB_PAGEUP:
                    if (pos > VerticalScroll.LargeChange) {
                        pos-=VerticalScroll.LargeChange;
                    }
                    else {
                        pos = 0;
                    }
                    break;
                case NativeMethods.SB_PAGEDOWN:
                    if (pos < maxPos-VerticalScroll.LargeChange) {
                        pos+=VerticalScroll.LargeChange;
                    }
                    else {
                        pos = maxPos;
                    }
                    break;
                case NativeMethods.SB_TOP:
                    pos = 0;
                    break;
                case NativeMethods.SB_BOTTOM:
                    pos = maxPos;
                    break;
            }
            // This bugs reproes on all those machine which have  SystemInformation.DragFullWindows set to false
            // "thumbTrack" was incorrectly used... the usage should be identical to WnHScroll which follows.
            if (GetScrollState(ScrollStateFullDrag) || thumbTrack ) {
                SetScrollState(ScrollStateUserHasScrolled, true);
                SetDisplayRectLocation(displayRect.X, -pos);
                SyncScrollbars(AutoScroll);
            }
            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.VerticalScroll);
        }

        /// <devdoc>
        ///     WM_HSCROLL handler
        /// </devdoc>
        /// <internalonly/>
        private void WmHScroll(ref Message m) {

            // The lparam is handle of the sending scrollbar, or NULL when
            // the scrollbar sending the message is the "form" scrollbar...
            //
            if (m.LParam != IntPtr.Zero) {
                base.WndProc(ref m);
                return;
            }

            Rectangle client = ClientRectangle;
            
            int pos = -displayRect.X;
            int oldValue = pos;
            int maxPos = -(client.Width - displayRect.Width);
            if (!AutoScroll) {
                maxPos = this.HorizontalScroll.Maximum;
            }

            switch (NativeMethods.Util.LOWORD(m.WParam)) {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    pos = ScrollThumbPosition(NativeMethods.SB_HORZ);
                    break;
                case NativeMethods.SB_LINEUP:
                    if (pos > HorizontalScroll.SmallChange) {
                        pos-=HorizontalScroll.SmallChange;
                    }
                    else {
                        pos = 0;
                    }
                    break;
                case NativeMethods.SB_LINEDOWN:
                    if (pos < maxPos-HorizontalScroll.SmallChange) {
                        pos+=HorizontalScroll.SmallChange;
                    }
                    else {
                        pos = maxPos;
                    }
                    break;
                case NativeMethods.SB_PAGEUP:
                    if (pos > HorizontalScroll.LargeChange) {
                        pos-=HorizontalScroll.LargeChange;
                    }
                    else {
                        pos = 0;
                    }
                    break;
                case NativeMethods.SB_PAGEDOWN:
                    if (pos < maxPos-HorizontalScroll.LargeChange) {
                        pos+=HorizontalScroll.LargeChange;
                    }
                    else {
                        pos = maxPos;
                    }
                    break;
                case NativeMethods.SB_LEFT:
                    pos = 0;
                    break;
                case NativeMethods.SB_RIGHT:
                    pos = maxPos;
                    break;
            }
            if (GetScrollState(ScrollStateFullDrag) || NativeMethods.Util.LOWORD(m.WParam) != NativeMethods.SB_THUMBTRACK) {
                SetScrollState(ScrollStateUserHasScrolled, true);
                SetDisplayRectLocation(-pos, displayRect.Y);
                SyncScrollbars(AutoScroll);
            }
            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.HorizontalScroll);
        }

        /// <devdoc>
        ///     This function gets called which populates the eventArgs and fires the OnScroll( ) event passing
        ///     the appropriate scroll event and scroll bar.
        /// </devdoc>
        /// <internalonly/>
        private void WmOnScroll(ref Message m, int oldValue, int value, ScrollOrientation scrollOrientation) {
            ScrollEventType type = (ScrollEventType)NativeMethods.Util.LOWORD(m.WParam);
            if (type != ScrollEventType.EndScroll) { 
                ScrollEventArgs se = new ScrollEventArgs(type, oldValue, value, scrollOrientation);
                OnScroll(se);
            }
        }

        private void WmSettingChange(ref Message m) {
            base.WndProc(ref m);
            UpdateFullDrag();
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.WndProc"]/*' />
        /// <devdoc>
        ///    The button's window procedure.  Inheriting classes can override this
        ///    to add extra functionality, but should not forget to call
        ///    base.wndProc(m); to ensure the button continues to function properly.
        /// </devdoc>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_VSCROLL:
                    WmVScroll(ref m);
                    break;
                case NativeMethods.WM_HSCROLL:
                    WmHScroll(ref m);
                    break;
                case NativeMethods.WM_SETTINGCHANGE:
                    WmSettingChange(ref m);
                    break;
                default:
                    base.WndProc(ref m);
            break;
            }
        }

        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges"]/*' />
        /// <devdoc>
        ///    <para>Determines the border padding for
        ///       docked controls.</para>
        /// </devdoc>
        [TypeConverterAttribute(typeof(DockPaddingEdgesConverter))]
        public class DockPaddingEdges : ICloneable {
            private ScrollableControl owner;
            private int               left;
            private int               right;
            private int               top;
            private int               bottom;

            /// <devdoc>
            ///     Creates a new DockPaddingEdges. The specified owner will
            ///     be notified when the values are changed.
            /// </devdoc>
            internal DockPaddingEdges(ScrollableControl owner) {
                this.owner = owner;
            }

            internal DockPaddingEdges(int left, int right, int top, int bottom) {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.All"]/*' />
            /// <devdoc>
            ///    <para>Gets
            ///       or
            ///       sets the padding width for all edges of a docked control.</para>
            /// </devdoc>
            [
            RefreshProperties(RefreshProperties.All),
            SRDescription(nameof(SR.PaddingAllDescr))
            ]
            public int All {
                get { 
                    if (owner == null) {
                        if (left == right && top == bottom && left == top) {
                            return left;
                        }
                        else {
                            return 0;
                        }
                    }
                    else {
                        // The Padding struct uses -1 to indicate that LRTB are in disagreement.
                        // For backwards compatibility, we need to remap -1 to 0, but we need
                        // to be careful because it could be that they explicitly set All to -1.
                        if (owner.Padding.All == -1
                            && 
                                (owner.Padding.Left != -1
                                || owner.Padding.Top != -1
                                || owner.Padding.Right != -1
                                || owner.Padding.Bottom != -1)) {
                            return 0;
                        }
                        return owner.Padding.All; 
                    }
                }
                set { 
                    if (owner == null) {
                        left = value;
                        top = value;
                        right = value;
                        bottom = value;
                    }
                    else {
                        owner.Padding = new Padding(value);
                    }
                }
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.Bottom"]/*' />
            /// <devdoc>
            ///    <para>Gets
            ///       or
            ///       sets the padding width for the bottom edge of a docked control.</para>
            /// </devdoc>
            [
            RefreshProperties(RefreshProperties.All),
            SRDescription(nameof(SR.PaddingBottomDescr))
            ]
            public int Bottom {
                get { 
                    if (owner == null) {
                        return bottom;
                    }
                    else {
                        return owner.Padding.Bottom; 
                    }
                }
                set {
                    if (owner == null) {
                        bottom = value;
                    }
                    else {
                        Padding padding = owner.Padding;
                        padding.Bottom = value;
                        owner.Padding = padding;
                    }
                }
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.Left"]/*' />
            /// <devdoc>
            ///    <para>Gets
            ///       or sets the padding width for the left edge of a docked control.</para>
            /// </devdoc>
            [
            RefreshProperties(RefreshProperties.All),
            SRDescription(nameof(SR.PaddingLeftDescr))
            ]
            public int Left {
                 get { 
                    if (owner == null) {
                        return left;
                    }
                    else {
                        return owner.Padding.Left; 
                    }
                }
                set {
                    if (owner == null) {
                        left = value;
                    }
                    else {
                        Padding padding = owner.Padding;
                        padding.Left = value;
                        owner.Padding = padding;
                    }
                }
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.Right"]/*' />
            /// <devdoc>
            ///    <para>Gets
            ///       or sets the padding width for the right edge of a docked control.</para>
            /// </devdoc>
            [
            RefreshProperties(RefreshProperties.All),
            SRDescription(nameof(SR.PaddingRightDescr))
            ]
            public int Right {
                 get { 
                    if (owner == null) {
                        return right;
                    }
                    else {
                        return owner.Padding.Right; 
                    }
                }
                set {
                    if (owner == null) {
                        right = value;
                    }
                    else {
                        Padding padding = owner.Padding;
                        padding.Right = value;
                        owner.Padding = padding;
                    }
                }
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.Top"]/*' />
            /// <devdoc>
            ///    <para>Gets
            ///       or sets the padding width for the top edge of a docked control.</para>
            /// </devdoc>
            [
            RefreshProperties(RefreshProperties.All),
            SRDescription(nameof(SR.PaddingTopDescr))
            ]
            public int Top {
                 get { 
                    if (owner == null) {
                        return bottom;
                    }
                    else {
                        return owner.Padding.Top; 
                    }
                }
                set {
                    if (owner == null) {
                        top = value;
                    }
                    else {
                        Padding padding = owner.Padding;
                        padding.Top = value;
                        owner.Padding = padding;
                    }
                }
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.Equals"]/*' />
            /// <internalonly/>
            public override bool Equals(object other) {
                DockPaddingEdges dpeOther = other as DockPaddingEdges;

                if (dpeOther != null) {
                    return this.owner.Padding.Equals(dpeOther.owner.Padding);
                }
                return false;
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.GetHashCode"]/*' />
            /// <internalonly/>
            public override int GetHashCode() {
                return base.GetHashCode();
            }


            /// <internalonly/>
            private void ResetAll() {
                All = 0;
            }

            /// <internalonly/>
            private void ResetBottom() {
                Bottom = 0;
            }

            /// <internalonly/>
            private void ResetLeft() {
                Left = 0;
            }

            /// <internalonly/>
            private void ResetRight() {
                Right = 0;
            }

            /// <internalonly/>
            private void ResetTop() {
                Top = 0;
            }

            internal void Scale(float dx, float dy) {
                this.owner.Padding.Scale(dx, dy);
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdges.ToString"]/*' />
            /// <internalonly/>
            public override string ToString() {
                return "";      // used to say "(DockPadding)" but that's useless
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="DockPaddingEdges.ICloneable.Clone"]/*' />
            /// <internalonly/>
            object ICloneable.Clone() {
                DockPaddingEdges dpe = new DockPaddingEdges(Left, Right, Top, Bottom);
                return dpe;
            }
        }


        /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdgesConverter"]/*' />
        public class DockPaddingEdgesConverter : TypeConverter {
            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdgesConverter.GetProperties"]/*' />
            /// <devdoc>
            ///    Retrieves the set of properties for this type.  By default, a type has
            ///    does not return any properties.  An easy implementation of this method
            ///    can just call TypeDescriptor.GetProperties for the correct data type.
            /// </devdoc>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(DockPaddingEdges), attributes);
                return props.Sort(new string[] {"All", "Left", "Top", "Right", "Bottom"});
            }

            /// <include file='doc\ScrollableControl.uex' path='docs/doc[@for="ScrollableControl.DockPaddingEdgesConverter.GetPropertiesSupported"]/*' />
            /// <devdoc>
            ///    Determines if this object supports properties.  By default, this
            ///    is false.
            /// </devdoc>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
                return true;
            }
        }
    }
}
