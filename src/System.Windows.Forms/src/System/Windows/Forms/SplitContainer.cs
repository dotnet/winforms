// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Collections;
    using System.Drawing.Drawing2D;
    using System.Globalization;

    /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer"]/*' />
    /// <devdoc>
    ///    A SplitContainer is a ContainerControl with 2 panels separated with a splitter
    ///    in the middle. This is a composite control. The user can drag and drop this control from Toolbox.
    ///    Controls can be added to the right panel and the left panel. The Orientation can be either Horizontal or Vertical.
    ///    The Controls inside the Panels would be redrawn with the new Orientation.
    ///    With this control the user need be aware of docking, z-order of the controls. The controls get parented when thry are
    ///    dropped on the SpitContainer.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(SplitterMoved)),
    Docking(DockingBehavior.AutoDock),
    Designer("System.Windows.Forms.Design.SplitContainerDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionSplitContainer))
    ]
    public class SplitContainer : ContainerControl, ISupportInitialize
    {

        //
        // CONTANTS USED DURING DRAWING SPLITTER MOOVEMENTS
        //
        private const int DRAW_START = 1;
        private const int DRAW_MOVE = 2;
        private const int DRAW_END = 3;
        private const int rightBorder = 5;
        private const int leftBorder = 2;

        private int BORDERSIZE = 0;


        //
        // SplitContainer private Cached copies of public properties...
        //
        private Orientation orientation = Orientation.Vertical;
        private SplitterPanel panel1 = null;
        private SplitterPanel panel2 = null;
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;
        private FixedPanel fixedPanel = FixedPanel.None;

        private int panel1MinSize = 25; //Panel1 Minimum Size
        private int newPanel1MinSize = 25; //New panel1 Minimum Size used for ISupportInitialize
        private int panel2MinSize = 25; //Panel2 Minimum Size
        private int newPanel2MinSize = 25; //New panel2 Minimum Size used for ISupportInitialize
        private bool tabStop = true;
        //
        //Fixed panel width or height;
        //
        private int panelSize;

        //
        //SPLITTER PROPERTIES
        //
        private Rectangle splitterRect;
        private int splitterInc = 1;
        private bool splitterFixed;
        private int splitterDistance = 50; //default splitter distance;
        private int splitterWidth = 4;
        private int newSplitterWidth = 4; //New splitter width used for ISupportInitialize
        private int splitDistance = 50;

        //
        //Properties used using DRAWING a MOVING SPLITTER
        //
        private int lastDrawSplit =1;
        private int initialSplitterDistance;
        private Rectangle initialSplitterRectangle;
        private Point anchor = Point.Empty;
        private bool splitBegin = false;
        private bool splitMove = false;
        private bool splitBreak = false;

        //
        // Split Cursor
        //
        Cursor overrideCursor = null;

        //
        //Needed For Tabbing
        //
        Control nextActiveControl = null;
        private bool callBaseVersion;
        private bool splitterFocused;

        //
        //Required to keep track of Splitter movements....
        //
        private bool splitterClick;
        private bool splitterDrag;

        //
        // FixedPanel.None require us
        // to keep the Width/Height Ratio Depending on SplitContainer.Orientation
        //

        double ratioWidth = 0.0f;
        double ratioHeight = 0.0f;
        bool resizeCalled = false;
        bool splitContainerScaling = false;
        bool setSplitterDistance = false;

        //
        //Events
        //
        private static readonly object EVENT_MOVING = new object();
        private static readonly object EVENT_MOVED = new object();

        // private IMessageFilter implementation
        SplitContainerMessageFilter splitContainerMessageFilter = null;

		//This would avoid re-entrant code into SelectNextControl.
		private bool selectNextControl = false;

        // Initialization flag for ISupportInitialize
        private bool initializing = false;

        //
        // Constructor
        //
        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitContainer"]/*' />
        public SplitContainer()
        {
            // either the left or top panel - LTR
            // either the right or top panel - RTL
            panel1 = new SplitterPanel(this);
            // either the right or bottom panel - LTR
            // either the left or bottom panel - RTL
            panel2 = new SplitterPanel(this);
            splitterRect = new Rectangle();
            
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ((WindowsFormsUtils.TypedControlCollection)this.Controls).AddInternal(panel1);
            ((WindowsFormsUtils.TypedControlCollection)this.Controls).AddInternal(panel2);
            UpdateSplitter();

        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                         //
        //PROPERTIES START IN ALPHABETICAL ORDER                                                   //
        //                                                                                         //
        /////////////////////////////////////////////////////////////////////////////////////////////




        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.AutoScroll"]/*' />
        /// <devdoc>
        ///     This property is overridden to allow the AutoScroll to be set on all the panels when
        ///     The autoScroll on SplitContainer is shown.
        ///     Here we dont set the base value ... but set autoscroll for panels.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.FormAutoScrollDescr)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
        ]
        public override bool AutoScroll {
            get {
                //Always return false ... as Splitcontainer doesnt support AutoScroll
                return false;
            }

            set {
                base.AutoScroll = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DefaultValue(typeof(Point), "0, 0")
        ]
        public override Point AutoScrollOffset {
            get {
               return base.AutoScrollOffset;
            }
            set {
                base.AutoScrollOffset = value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.AutoScrollMinSize"]/*' />
        /// <devdoc>
        ///    Override AutoScrollMinSize to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public new Size AutoScrollMinSize {
            get {
                return base.AutoScrollMinSize;
            }
            set {
                base.AutoScrollMinSize = value;
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.AutoScrollMargin"]/*' />
        /// <devdoc>
        ///    Override AutoScrollMargin to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public new Size AutoScrollMargin {
            get {
                return base.AutoScrollMargin;
            }
            set {
                base.AutoScrollMargin = value;
            }
        }


        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormAutoScrollPositionDescr))        
        ]
        public new Point AutoScrollPosition {
            get {
                return base.AutoScrollPosition;
            }

            set {
                base.AutoScrollPosition = value;
            }
        }

        /// <devdoc>
        ///     <para>Hide AutoSize, as it can mean more than one thing and might confuse users</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BackgroundImage"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BindingContext"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       The binding manager for the container control.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        SRDescription(nameof(SR.ContainerControlBindingContextDescr)),
        SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")
        ]
        public override BindingContext BindingContext {
            get {
                return BindingContextInternal;
            }
            set {
                BindingContextInternal = value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BorderStyle"]/*' />
        /// <devdoc>
        ///     Indicates what type of border the Splitter control has.  This value
        ///     comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </devdoc>
        [
        DefaultValue(BorderStyle.None),
        SRCategory(nameof(SR.CatAppearance)),
        System.Runtime.InteropServices.DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.SplitterBorderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value) {
                    borderStyle = value;
                    Invalidate();
                    SetInnerMostBorder(this);
                    if (this.ParentInternal != null) {
                        if (this.ParentInternal is SplitterPanel) {
                            SplitContainer sc = (SplitContainer)((SplitterPanel)this.ParentInternal).Owner;
                            sc.SetInnerMostBorder(sc);
                        }
                    }
                }

                switch (BorderStyle)
                {
                    case BorderStyle.None: 
                        BORDERSIZE = 0;
                        break;
                    case BorderStyle.FixedSingle:
                        BORDERSIZE = 1;
                        break;
                    case BorderStyle.Fixed3D:
                        BORDERSIZE = 4;
                        break;
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Controls"]/*' />
        /// <devdoc>
        ///     Controls Collection...
        ///     This is overriden so that the Controls.Add ( ) is not Code Gened...
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Control.ControlCollection Controls {
            get {
                return base.Controls;
            }
        }

        /// <include file='doc\WinBar.uex' path='docs/doc[@for="SplitContainer.ControlAdded"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlAdded {
            add {
                base.ControlAdded += value;
            }
            remove {
                base.ControlAdded -= value;
            }
        }
        /// <include file='doc\WinBar.uex' path='docs/doc[@for="SplitContainer.ControlRemoved"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlRemoved {
             add {
                 base.ControlRemoved += value;
             }
             remove {
                 base.ControlRemoved -= value;
             }
        }
        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Dock"]/*' />
        /// <devdoc>
        ///     The dock property. The dock property controls to which edge
        ///     of the container this control is docked to. For example, when docked to
        ///     the top of the container, the control will be displayed flush at the
        ///     top of the container, extending the length of the container.
        /// </devdoc>
        public new DockStyle Dock {
            get {
                return base.Dock;
            }
            set {
                base.Dock = value;
                if (this.ParentInternal != null) {
                    if (this.ParentInternal is SplitterPanel) {
                        SplitContainer sc = (SplitContainer)((SplitterPanel)this.ParentInternal).Owner;
                        sc.SetInnerMostBorder(sc);
                    }
                }
                ResizeSplitContainer();
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(150, 100);
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.FixedPanel"]/*' />
        /// <devdoc>
        ///     Indicates what type of border the Splitter control has.  This value
        ///     comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </devdoc>
        [
        DefaultValue(FixedPanel.None),
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.SplitContainerFixedPanelDescr))
        ]
        public FixedPanel FixedPanel {
            get {
                return fixedPanel;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FixedPanel.None, (int)FixedPanel.Panel2)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FixedPanel));
                }

                if (fixedPanel != value) {
                    fixedPanel = value;
                    // UpdatePanelSize !!
                    switch (fixedPanel) {
                        case FixedPanel.Panel2:
                            if (Orientation == Orientation.Vertical) {
                                panelSize  = Width - SplitterDistanceInternal - SplitterWidthInternal;
                            }
                            else {
                                panelSize  = Height - SplitterDistanceInternal - SplitterWidthInternal;
                            }
                            break;
                        default:
                            panelSize  =   SplitterDistanceInternal;
                            break;
                    }
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.IsSplitterFixed"]/*' />
        /// <devdoc>
        /// This property determines whether the the splitter can move.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.SplitContainerIsSplitterFixedDescr))
        ]

        public bool IsSplitterFixed {
            get {
                return splitterFixed;
            }
            set {
                splitterFixed = value;
            }
        }

        //Private property used to check whether the splitter can be moved by the user.
        private bool IsSplitterMovable {
            get {
                 if (Orientation == Orientation.Vertical) {
                     return (Width >= Panel1MinSize + SplitterWidthInternal + Panel2MinSize);
                 }
                 else {
                     return (Height >= Panel1MinSize + SplitterWidthInternal + Panel2MinSize);
                 }

            }
        }

        // Refer to IsContainerControl property on Control for more details.
        internal override bool IsContainerControl
        {
            get
            {
                return true;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Orientation"]/*' />
        /// <devdoc>
        /// This Property sets or gets if the splitter is vertical or horizontal.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(Orientation.Vertical),
        Localizable(true),
        SRDescription(nameof(SR.SplitContainerOrientationDescr))
        ]
        public Orientation Orientation {
            get { return  orientation; }
            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Orientation.Horizontal, (int)Orientation.Vertical)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Orientation));
                }
                if (orientation != value) {
                    orientation = value;
					//update the splitterDistance to validate it w.r.t the new Orientation.
					splitDistance = 0;
					SplitterDistance = SplitterDistanceInternal;
                    UpdateSplitter();
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OverrideCursor"]/*' />
        private Cursor OverrideCursor {
            get {
                return overrideCursor;
            }
            set {
                if (overrideCursor != value) {
                    overrideCursor = value;

                    if (IsHandleCreated) {
                        // We want to instantly change the cursor if the mouse is within our bounds.
                        NativeMethods.POINT p = new NativeMethods.POINT();
                        NativeMethods.RECT r = new NativeMethods.RECT();
                        UnsafeNativeMethods.GetCursorPos(p);
                        UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);
                        if ((r.left <= p.x && p.x < r.right && r.top <= p.y && p.y < r.bottom) || UnsafeNativeMethods.GetCapture() == Handle)
                            SendMessage(NativeMethods.WM_SETCURSOR, Handle, NativeMethods.HTCLIENT);
                    }
                }
            }
        }

        ///<devdoc>
        /// Indicates if either panel is collapsed
        ///</devdoc>
        private bool CollapsedMode {
            get {
                return Panel1Collapsed || Panel2Collapsed;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel1"]/*' />
        /// <devdoc>
        /// The Left or Top panel in the SplitContainer.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.SplitContainerPanel1Descr)),
        Localizable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public SplitterPanel Panel1 {
            get {
                return this.panel1;
            }
        }

        ///<devdoc>
        ///     Collapses or restores the given panel
        ///</devdoc>
        private void CollapsePanel(SplitterPanel p, bool collapsing) {
            p.Collapsed = collapsing;
            if (collapsing) {
                p.Visible = false;

            }
            else {
                // restore panel
                p.Visible = true;
            }
            UpdateSplitter();
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add { base.PaddingChanged += value; }
            remove { base.PaddingChanged -= value; }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel1"]/*' />
        /// <devdoc>
        /// Collapses or restores panel1
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(false),
        SRDescription(nameof(SR.SplitContainerPanel1CollapsedDescr))
        ]
        public bool Panel1Collapsed {
            get {
                return panel1.Collapsed;
            }
            set {
                if (value != panel1.Collapsed) {
                    if (value && panel2.Collapsed) {
                        CollapsePanel(panel2, false);
                    }
                    CollapsePanel(panel1, value);
                }
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel1"]/*' />
        /// <devdoc>
        /// Collapses or restores panel2
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(false),
        SRDescription(nameof(SR.SplitContainerPanel2CollapsedDescr))
        ]
        public bool Panel2Collapsed {
            get {
                return panel2.Collapsed;
            }
            set {
                if (value != panel2.Collapsed) {
                    if (value && panel1.Collapsed) {
                        CollapsePanel(panel1, false);
                    }
                    CollapsePanel(panel2, value);
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel1MinSize"]/*' />
        /// <devdoc>
        /// This property determines the minimum distance of pixels of the splitter from the left or the top edge of Panel1.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(25),
        Localizable(true),
        SRDescription(nameof(SR.SplitContainerPanel1MinSizeDescr)),
        RefreshProperties(RefreshProperties.All)       
        ]
        public int Panel1MinSize {
            get {
                return panel1MinSize;
            }
            set {
                newPanel1MinSize = value;
                if (value != Panel1MinSize && !initializing) {
                    ApplyPanel1MinSize(value);
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel2"]/*' />
        /// <devdoc>
        /// This is the Right or Bottom panel in the SplitContainer.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.SplitContainerPanel2Descr)),
        Localizable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public SplitterPanel Panel2 {
            get {
                return this.panel2;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Panel2MinSize"]/*' />
        /// <devdoc>
        /// This property determines the minimum distance of pixels of the splitter from the right or the bottom edge of Panel2
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(25),
        Localizable(true),
        SRDescription(nameof(SR.SplitContainerPanel2MinSizeDescr)),
        RefreshProperties(RefreshProperties.All)
        ]
        public int Panel2MinSize {
            get {
                return panel2MinSize;
            }
            set {
                newPanel2MinSize = value;
                if (value != Panel2MinSize && !initializing) {
                    ApplyPanel2MinSize(value);
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterDistance"]/*' />
        /// <devdoc>
        /// This property determines pixel distance of the splitter from the left or top edge.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SettingsBindable(true),
        SRDescription(nameof(SR.SplitContainerSplitterDistanceDescr)),
        DefaultValue(50)
        ]
        public int SplitterDistance 
        {
            get
            {
                return splitDistance;
            }
            set
            {
                if (value != SplitterDistance)
                {
                    if (value < 0) {
                        throw new ArgumentOutOfRangeException(nameof(SplitterDistance), string.Format(SR.InvalidLowBoundArgument, "SplitterDistance", (value).ToString(CultureInfo.CurrentCulture), "0"));
                    }
                    

                    try
                    {
                        setSplitterDistance = true;
                    
                        if (Orientation == Orientation.Vertical) {

                            if (value  < Panel1MinSize)
                            {
                                value = Panel1MinSize;
                            }
                            if (value + SplitterWidthInternal > this.Width - Panel2MinSize)
                            {
                                value = this.Width - Panel2MinSize - SplitterWidthInternal;
                            }
                            if (value < 0 ) {
                                throw new InvalidOperationException(SR.SplitterDistanceNotAllowed);
                            }
                            splitDistance = value;
                            splitterDistance = value;
                            panel1.WidthInternal = SplitterDistance;

                        }
                        else {

                            if (value < Panel1MinSize)
                            {
                                value = Panel1MinSize;
                            }

                            if (value + SplitterWidthInternal > this.Height - Panel2MinSize)
                            {
                                value = this.Height - Panel2MinSize - SplitterWidthInternal;
                            }
                            if (value < 0 ) {
                                throw new InvalidOperationException(SR.SplitterDistanceNotAllowed);
                            }
                            splitDistance = value;
                            splitterDistance = value;
                            panel1.HeightInternal = SplitterDistance;
                        }

                        switch (fixedPanel) {
                            case FixedPanel.Panel1:
                                panelSize  =   SplitterDistance;
                                break;
                            case FixedPanel.Panel2:
                                if (Orientation == Orientation.Vertical) {
                                    panelSize  = Width - SplitterDistance - SplitterWidthInternal;
                                }
                                else {
                                    panelSize  = Height - SplitterDistance - SplitterWidthInternal;
                                }
                                break;
                        }
                        UpdateSplitter();
                    }
                    finally
                    {
                        setSplitterDistance = false;
                    }
                    OnSplitterMoved(new SplitterEventArgs(SplitterRectangle.X + SplitterRectangle.Width/2, SplitterRectangle.Y + SplitterRectangle.Height/2, SplitterRectangle.X, SplitterRectangle.Y));
                }
            }
        }

        private int SplitterDistanceInternal {

            get {
                return splitterDistance;
            }
            set {
                SplitterDistance = value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterIncrement"]/*' />
        /// <devdoc>
        /// This determines the number of pixels the splitter moves in increments.This is defaulted to 1.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(1),
        Localizable(true),
        SRDescription(nameof(SR.SplitContainerSplitterIncrementDescr))
        ]
        public int SplitterIncrement {
            get {
                return splitterInc;
            }
            set {

                if (value < 1 ) {
                    throw new ArgumentOutOfRangeException(nameof(SplitterIncrement), string.Format(SR.InvalidLowBoundArgumentEx, "SplitterIncrement", (value).ToString(CultureInfo.CurrentCulture), "1"));
                }

                splitterInc = value;
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterRectangle"]/*' />
        /// <devdoc>
        /// This property determines the rectangle bounds of the splitter.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.SplitContainerSplitterRectangleDescr)),
        Browsable(false)
        ]
        public Rectangle SplitterRectangle {
            get {
                Rectangle r = splitterRect;
                r.X = splitterRect.X - Left;
                r.Y = splitterRect.Y - Top;
                return r;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterWidth"]/*' />
        /// <devdoc>
        /// This property determines the thickness of the splitter.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.SplitContainerSplitterWidthDescr)),
        Localizable(true),
        DefaultValue(4)
        ]
        public int SplitterWidth {
            get {
                return splitterWidth;
            }
            set {
                newSplitterWidth = value;
                if (value != SplitterWidth && !initializing) {
                    ApplySplitterWidth(value);
                }
            }
        }

        /// <devdoc>
        ///    We need to have a internal Property for the SplitterWidth which returns zero if we are in collapased mode.
        ///    This property is used to Layout SplitContainer.
        /// </devdoc>
        private int SplitterWidthInternal {
            get {
                // if CollapsedMode then splitterwidth == 0;
                return (CollapsedMode) ? 0 : splitterWidth;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.TabStop"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the user can give the focus to this control using the TAB
        ///       key. This property is read-only.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        DispId(NativeMethods.ActiveX.DISPID_TABSTOP),
        SRDescription(nameof(SR.ControlTabStopDescr))
        ]
        public new bool TabStop {
            get {
                return tabStop;
            }
            set {
                if (TabStop != value) {
                    tabStop = value;
                    OnTabStopChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Text"]/*' />
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //END PROPERTIES                                                              //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //Start PUBLIC FUNCTIONS                                                      //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BeginInit"]/*' />
        /// <devdoc>
        ///     ISupportInitialize support. Disables splitter panel min size and splitter width
        ///     validation during initialization.
        /// </devdoc>
        public void BeginInit() {
            initializing = true;
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.EndInit"]/*' />
        /// <devdoc>
        ///     ISupportInitialize support. Enables splitter panel min size and splitter width
        ///     validation after initialization.
        /// </devdoc>
        public void EndInit() {
            initializing = false;

            // validate and apply new value
            if (newPanel1MinSize != panel1MinSize) {
                ApplyPanel1MinSize(newPanel1MinSize);
            }
            if (newPanel2MinSize != panel2MinSize) {
                ApplyPanel2MinSize(newPanel2MinSize);
            }
            if (newSplitterWidth != splitterWidth) {
                ApplySplitterWidth(newSplitterWidth);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //End PUBLIC FUNCTIONS                                                        //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //Start EVENT HANDLERS                                                        //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterMoving"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovingDescr))]
        public event SplitterCancelEventHandler SplitterMoving {
            add {
                Events.AddHandler(EVENT_MOVING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOVING, value);
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitterMoved"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovedDescr))]
        public event SplitterEventHandler SplitterMoved {
            add {
                Events.AddHandler(EVENT_MOVED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOVED, value);
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //End EVENT HANDLERS                                                          //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //start EVENT Delegates                                                       //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnGotFocus"]/*' />
        /// <devdoc>
        ///     Overides the Control.OnGotFocus to Invalidate...
        /// </devdoc>
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            Invalidate();

        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnKeyDown"]/*' />
        /// <devdoc>
        ///     Overrides the Control.OnKeydown for implementing splitter movements.
        /// </devdoc>
        protected override void OnKeyDown(KeyEventArgs e) {
            Debug.Assert(Enabled, "SplitContainer.OnKeyDown should not be called if the button is disabled");
            base.OnKeyDown(e);
            //If the Panel1MinSize + Panel2MinSize < SplitContainer.Size then carry on the splitter move...
            if (IsSplitterMovable && !IsSplitterFixed) {
                if (e.KeyData == Keys.Escape && splitBegin) {
                    splitBegin = false;
                    splitBreak = true;
                    return;
                }
                //valid Keys that move the splitter...
                if (e.KeyData == Keys.Right || e.KeyData == Keys.Down ||
                    e.KeyData == Keys.Left || e.KeyData == Keys.Up
                    && splitterFocused) {
                    if (splitBegin) {
                        splitMove = true;
                    }

                    //left OR up
                    if (e.KeyData == Keys.Left || e.KeyData == Keys.Up && splitterFocused) {
                        splitterDistance -= SplitterIncrement;
                        splitterDistance = (splitterDistance < Panel1MinSize) ? splitterDistance + SplitterIncrement : Math.Max(splitterDistance, BORDERSIZE);
                    }
                    //right OR down
                    if (e.KeyData == Keys.Right || e.KeyData == Keys.Down && splitterFocused) {
                        splitterDistance += SplitterIncrement;
                        if (Orientation == Orientation.Vertical) {
                            splitterDistance = (splitterDistance + SplitterWidth > Width - Panel2MinSize -BORDERSIZE) ? splitterDistance - SplitterIncrement : splitterDistance;
                        }
                        else {
                            splitterDistance = (splitterDistance + SplitterWidth > Height - Panel2MinSize - BORDERSIZE) ? splitterDistance - SplitterIncrement : splitterDistance;
                        }

                    }

                    if (!splitBegin) {
                        splitBegin = true;
                    }
                    //draw Helper start
                    if (splitBegin && !splitMove) {
                        initialSplitterDistance = SplitterDistanceInternal;
                        DrawSplitBar(DRAW_START);
                    }
                    else { //draw helper move
                        DrawSplitBar(DRAW_MOVE);
                        //Moving by mouse .....gives the origin of the splitter..
                        //
                        Rectangle r = CalcSplitLine(splitterDistance, 0);
                        int xSplit = r.X;
                        int ySplit = r.Y;
                        SplitterCancelEventArgs se = new SplitterCancelEventArgs(this.Left + SplitterRectangle.X + SplitterRectangle.Width/2, this.Top + SplitterRectangle.Y + SplitterRectangle.Height/2, xSplit, ySplit);
                        OnSplitterMoving(se);
                        if (se.Cancel) {
                            SplitEnd(false);
                        }
                    }
                } //End Valid Keys....
            } //End SplitterFixed Check...
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnKeyUp"]/*' />
        /// <devdoc>
        ///     Overrides the Control.OnKeydown for implementing splitter movements.
        /// </devdoc>
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (splitBegin && IsSplitterMovable) {
                if (e.KeyData == Keys.Right || e.KeyData == Keys.Down ||
                    e.KeyData == Keys.Left || e.KeyData == Keys.Up
                    && splitterFocused) {
                    DrawSplitBar(DRAW_END);
                    ApplySplitterDistance();
                    splitBegin = false;
                    splitMove = false;
                }
            }
            if (splitBreak) {
                splitBreak = false;
                SplitEnd(false);
            }
            //problem with the Focus rect after Keyup ....
            //Focus rect and reverible lines leave a trace behind on the splitter...
            using (Graphics g = CreateGraphicsInternal()) {
                if (BackgroundImage == null) {
                    using (SolidBrush brush = new SolidBrush(this.BackColor)) {
                        g.FillRectangle(brush, SplitterRectangle);
                    }
                }
                DrawFocus(g, SplitterRectangle);
            }
            
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnLayout"]/*' />
        /// <devdoc>
        ///     Overrides the Control.OnLayout.
        /// </devdoc>
        protected override void OnLayout(LayoutEventArgs e) {
            SetInnerMostBorder(this);
            
            if (IsSplitterMovable && !setSplitterDistance) {
                ResizeSplitContainer();
            }
            base.OnLayout(e);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnLostFocus"]/*' />
        /// <devdoc>
        ///     Overrides the Control.OnLostFocus to Invalidate.
        /// </devdoc>
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            Invalidate();
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnMouseMove"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.SplitContainer.MouseMove'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (!IsSplitterFixed && IsSplitterMovable) {

                //change cursor if default and user hasnt changed the cursor.
                if (Cursor == DefaultCursor && SplitterRectangle.Contains(e.Location))
                {
                    if (Orientation == Orientation.Vertical) {
                        OverrideCursor = Cursors.VSplit;
                    }
                    else {
                        OverrideCursor = Cursors.HSplit;
                    }
                }
                else {
                    OverrideCursor = null;;
                }

                if (splitterClick) {
                    int x = e.X ;
                    int y = e.Y ;
                    splitterDrag = true;
                    SplitMove(x, y);
                    if (Orientation == Orientation.Vertical) {
                        x = Math.Max(Math.Min(x, Width - Panel2MinSize), Panel1MinSize);
                        y = Math.Max(y, 0);
                    }
                    else {
                        y = Math.Max(Math.Min(y, Height - Panel2MinSize), Panel1MinSize);
                        x = Math.Max(x, 0);
                    }
                    Rectangle r = CalcSplitLine(GetSplitterDistance(e.X, e.Y), 0);
                    int xSplit = r.X;
                    int ySplit = r.Y;
                    SplitterCancelEventArgs se = new SplitterCancelEventArgs(x, y, xSplit, ySplit);
                    OnSplitterMoving(se);
                    if (se.Cancel) {
                        SplitEnd(false);

                    }
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnMouseLeave"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.SplitContainer.OnMouseLeave'/> event.</para>
        /// </devdoc>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (!Enabled) {
                return;
            }
            OverrideCursor = null;
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnMouseDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.SplitContainer.OnMouseDown'/> event.</para>
        /// </devdoc>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            //If the Panel1MinSize + Panel2MinSize < SplitContainer.Size then carry on the splitter move...
            if (IsSplitterMovable && SplitterRectangle.Contains(e.Location)) {
                if (!Enabled) {
                    return;
                }
                if (e.Button == MouseButtons.Left && e.Clicks == 1 && !IsSplitterFixed) {
                    // Focus the current splitter OnMouseDown.
                    splitterFocused = true;
                    IContainerControl c = this.ParentInternal.GetContainerControlInternal();
                    if (c != null) {
                        ContainerControl cc = c as ContainerControl;
                        if (cc == null) {
                            c.ActiveControl = this;
                        }
                        else {
                            cc.SetActiveControlInternal(this);
                        }
                    }
                    SetActiveControlInternal(null);
                    nextActiveControl = panel2;

                    SplitBegin(e.X, e.Y);
                    splitterClick  = true;
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnMouseUp"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.SplitContainer.OnMouseUp'/> event.</para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (!Enabled) {
                return;
            }
            if (!IsSplitterFixed && IsSplitterMovable && splitterClick) {
                CaptureInternal = false;
                
                if (splitterDrag) {
                    CalcSplitLine(GetSplitterDistance(e.X, e.Y), 0);
                    SplitEnd(true);
                }
                else {
                    SplitEnd(false);

                }
                splitterClick  = false;
                splitterDrag = false;
            }
        }

	/// <devdoc>
	///     Overrides the Control.OnMove() to synchronize the 
	///     splitterRect with the position of the SplitContainer.
	/// </devdoc>
	protected override void OnMove(EventArgs e)
	{
	    base.OnMove(e);
	    SetSplitterRect(this.Orientation == Orientation.Vertical);
	}
		

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnPaint"]/*' />
        /// <devdoc>
        ///     Overrides the Control.OnPaint() to focus the Splitter.
        /// </devdoc>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (Focused) {
                DrawFocus(e.Graphics,SplitterRectangle);
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnSplitterMoving"]/*' />
        /// <devdoc>
        ///     Inherriting classes should override this method to respond to the
        ///     splitterMoving event. This event occurs while the splitter is
        ///     being moved by the user.
        /// </devdoc>
        public void OnSplitterMoving(SplitterCancelEventArgs e) {
            SplitterCancelEventHandler handler = (SplitterCancelEventHandler)Events[EVENT_MOVING];
            if (handler != null) handler(this, e);

        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnSplitterMoved"]/*' />
        /// <devdoc>
        ///     Inherriting classes should override this method to respond to the
        ///     splitterMoved event. This event occurs when the user finishes
        ///     moving the splitter.
        /// </devdoc>
        public void OnSplitterMoved(SplitterEventArgs e) {
            SplitterEventHandler handler = (SplitterEventHandler)Events[EVENT_MOVED];
            if (handler != null) handler(this, e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                            //
        ///END DELEGATES                                                                              //
        //                                                                                            //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.OnRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnRightToLeftChanged(EventArgs e) {
            base.OnRightToLeftChanged(e);
            // pass the RightToLeft value to the Parent.
            this.panel1.RightToLeft = this.RightToLeft;
            this.panel2.RightToLeft = this.RightToLeft;
            UpdateSplitter();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                            //
        ///START PRIVATE FUNCTIONS                                                                    //
        //                                                                                            //
        ////////////////////////////////////////////////////////////////////////////////////////////////


        /// <devdoc>
        ///     Validate and set the minimum size for Panel1.
        /// </devdoc>
        private void ApplyPanel1MinSize(int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(Panel1MinSize), string.Format(SR.InvalidLowBoundArgument, "Panel1MinSize", (value).ToString(CultureInfo.CurrentCulture), "0"));
            }

            if (Orientation== Orientation.Vertical) {
                if (DesignMode && Width != DefaultSize.Width && value + Panel2MinSize + SplitterWidth > Width) {
                    throw new ArgumentOutOfRangeException(nameof(Panel1MinSize), string.Format(SR.InvalidArgument, "Panel1MinSize", (value).ToString(CultureInfo.CurrentCulture)));
                }
            }
            else if (Orientation == Orientation.Horizontal) {
                if (DesignMode && Height != DefaultSize.Height && value + Panel2MinSize + SplitterWidth > Height) {
                    throw new ArgumentOutOfRangeException(nameof(Panel1MinSize), string.Format(SR.InvalidArgument, "Panel1MinSize", (value).ToString(CultureInfo.CurrentCulture)));
                }
            }

            panel1MinSize = value;
            if (value > SplitterDistanceInternal) {
                SplitterDistanceInternal = value;  //Set the Splitter Distance to the end of Panel1
            }
        }

        /// <devdoc>
        ///     Validate and set the minimum size for Panel2.
        /// </devdoc>
        private void ApplyPanel2MinSize(int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(Panel2MinSize), string.Format(SR.InvalidLowBoundArgument, "Panel2MinSize", (value).ToString(CultureInfo.CurrentCulture), "0"));
            }
            if (Orientation == Orientation.Vertical) {
                if (DesignMode && Width != DefaultSize.Width && value + Panel1MinSize + SplitterWidth > Width) {
                    throw new ArgumentOutOfRangeException(nameof(Panel2MinSize), string.Format(SR.InvalidArgument, "Panel2MinSize", (value).ToString(CultureInfo.CurrentCulture)));
                }

            }
            else if (Orientation == Orientation.Horizontal) {
                if (DesignMode && Height != DefaultSize.Height && value + Panel1MinSize + SplitterWidth > Height) {
                    throw new ArgumentOutOfRangeException(nameof(Panel2MinSize), string.Format(SR.InvalidArgument, "Panel2MinSize", (value).ToString(CultureInfo.CurrentCulture)));
                }
            }
            panel2MinSize = value;
            if (value > Panel2.Width) {
                SplitterDistanceInternal = Panel2.Width + SplitterWidthInternal;  //Set the Splitter Distance to the start of Panel2
            }
        }

        /// <devdoc>
        ///     Validate and set the splitter width.
        /// </devdoc>
        private void ApplySplitterWidth(int value) {
            if (value < 1) {
                throw new ArgumentOutOfRangeException(nameof(SplitterWidth), string.Format(SR.InvalidLowBoundArgumentEx, "SplitterWidth", (value).ToString(CultureInfo.CurrentCulture), "1"));
            }
            if (Orientation == Orientation.Vertical) {
                if (DesignMode && value + Panel1MinSize + Panel2MinSize > Width) {
                    throw new ArgumentOutOfRangeException(nameof(SplitterWidth), string.Format(SR.InvalidArgument, "SplitterWidth", (value).ToString(CultureInfo.CurrentCulture)));
                }

            }
            else if (Orientation == Orientation.Horizontal) {
                if (DesignMode && value + Panel1MinSize + Panel2MinSize > Height) {
                    throw new ArgumentOutOfRangeException(nameof(SplitterWidth), string.Format(SR.InvalidArgument, "SplitterWidth", (value).ToString(CultureInfo.CurrentCulture)));
                }
            }
            splitterWidth = value;
            UpdateSplitter();
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.ApplySplitPosition"]/*' />
        /// <devdoc>
        ///     Sets the split position to be the current split size. This is called
        ///     by splitEdit
        /// </devdoc>
        /// <internalonly/>
        private void ApplySplitterDistance() {

            using (new System.Windows.Forms.Layout.LayoutTransaction(this, this, "SplitterDistance", false)) {
                SplitterDistanceInternal = splitterDistance;
            }

            // We need to invalidate when we have transparent backgournd.
            if (this.BackColor == Color.Transparent) {
                // the panel1 retains the focus rect... so Invalidate the rect ...
                Invalidate();
            }

            if (Orientation == Orientation.Vertical) {
                
                if (RightToLeft == RightToLeft.No) {
                    splitterRect.X = this.Location.X + SplitterDistanceInternal;
                }
                else {
                    splitterRect.X = this.Right - SplitterDistanceInternal - SplitterWidthInternal;
                }
            }
            else {
                splitterRect.Y = this.Location.Y + SplitterDistanceInternal;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.CalcSplitLine"]/*' />
        /// <devdoc>
        ///     Calculates the bounding rect of the split line. minWeight refers
        ///     to the minimum height or width of the splitline.
        /// </devdoc>
        private Rectangle CalcSplitLine(int splitSize, int minWeight) {

           Rectangle r = new Rectangle();
            switch (Orientation) {
                case Orientation.Vertical:
                    r.Width =  SplitterWidthInternal ;
                    r.Height = Height;
                    if (r.Width < minWeight) {
                        r.Width = minWeight;
                    }
                    
                    if (RightToLeft == RightToLeft.No) {
                        r.X = panel1.Location.X + splitSize;
                    }
                    else {
                        r.X = Width - splitSize - SplitterWidthInternal;
                    }
                    
                    break;


                case Orientation.Horizontal:
                    r.Width = Width;
                    r.Height = SplitterWidthInternal;
                    if (r.Width < minWeight) {
                        r.Width = minWeight;
                    }
                    r.Y = panel1.Location.Y + splitSize;
                    break;
            }
            return r;
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.DrawSplitBar"]/*' />
        /// <devdoc>
        ///     Draws the splitter bar at the current location. Will automatically
        ///     cleanup anyplace the splitter was drawn previously.
        /// </devdoc>
        /// <internalonly/>
        private void DrawSplitBar(int mode) {
            if (mode != DRAW_START && lastDrawSplit != -1) {
                DrawSplitHelper(lastDrawSplit);
                lastDrawSplit = -1;
            }
            // Bail if drawing with no old point...
            //
            else if (mode != DRAW_START && lastDrawSplit == -1) {
                return;
            }

            if (mode != DRAW_END) {
                if (splitMove || splitBegin) { // Splitter is moved by keys and not by mouse
                        DrawSplitHelper(splitterDistance);
                        lastDrawSplit = splitterDistance;
                }
                else {
                    DrawSplitHelper(splitterDistance);
                    lastDrawSplit = splitterDistance;
                }

            }
            else {
                if (lastDrawSplit != -1) {
                    DrawSplitHelper(lastDrawSplit);
                }
                lastDrawSplit = -1;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Draws the focus rectangle if the control has focus.
        ///
        ///    </para>
        /// </devdoc>
        private void DrawFocus(Graphics g, Rectangle r) {
            r.Inflate (-1, -1);
            ControlPaint.DrawFocusRectangle(g, r, this.ForeColor, this.BackColor);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.DrawSplitHelper"]/*' />
        /// <devdoc>
        ///     Draws the splitter line at the requested location. Should only be called
        ///     by drawSpltBar.
        /// </devdoc>
        /// <internalonly/>
        private void DrawSplitHelper(int splitSize) {

            Rectangle r = CalcSplitLine(splitSize, 3);
            IntPtr parentHandle = this.Handle;
            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(this, parentHandle), NativeMethods.NullHandleRef, NativeMethods.DCX_CACHE | NativeMethods.DCX_LOCKWINDOWUPDATE);
            IntPtr halftone = ControlPaint.CreateHalftoneHBRUSH();
            IntPtr saveBrush = SafeNativeMethods.SelectObject(new HandleRef(this, dc), new HandleRef(null, halftone));
            SafeNativeMethods.PatBlt(new HandleRef(this, dc), r.X, r.Y, r.Width, r.Height, NativeMethods.PATINVERT);
            SafeNativeMethods.SelectObject(new HandleRef(this, dc), new HandleRef(null, saveBrush));
            SafeNativeMethods.DeleteObject(new HandleRef(null, halftone));
            UnsafeNativeMethods.ReleaseDC(new HandleRef(this, parentHandle), new HandleRef(null, dc));
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.GetSplitSize"]/*' />
        /// <devdoc>
        ///     Calculates the split size based on the mouse position (x, y).
        /// </devdoc>
        /// <internalonly/>
        private int GetSplitterDistance(int x, int y) {
            int delta;
            if (Orientation == Orientation.Vertical) {
                delta = x - anchor.X;
            }
            else {
                delta = y - anchor.Y;
            }

            // Negative delta - moving to the left
            // Positive delta - moving to the right
            
            int size = 0;
            switch (Orientation) {
                case Orientation.Vertical:
                    if (RightToLeft == RightToLeft.No) {
                        size = Math.Max(panel1.Width + delta, BORDERSIZE);
                    }
                    else {
                        // In RTL negative delta actually means increasing the size....
                        size = Math.Max(panel1.Width - delta, BORDERSIZE);
                    }
                    break;
                case Orientation.Horizontal:
                    size = Math.Max(panel1.Height + delta, BORDERSIZE);
                    break;
            }
            if (Orientation == Orientation.Vertical) {
                return Math.Max(Math.Min(size, Width - Panel2MinSize), Panel1MinSize);
            }
            else {
                return Math.Max(Math.Min(size, Height - Panel2MinSize), Panel1MinSize);
            }
        }

        /// <devdoc>
        ///     Process an arrowKey press by selecting the next control in the group
        ///     that the activeControl belongs to.
        /// </devdoc>
        /// <internalonly/>
        private bool ProcessArrowKey(bool forward) {
            Control group = this;
            if (ActiveControl != null) {
                group = ActiveControl.ParentInternal;
            }
            return group.SelectNextControl(ActiveControl, forward, false, false, true);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.RepaintSplitterRect"]/*' />
        /// <devdoc>
        ///     Re paint SplitterRect for SplitContainer
        /// </devdoc>
        /// <internalonly/>
        private void RepaintSplitterRect()
        {
            if (IsHandleCreated) {
                Graphics g = this.CreateGraphicsInternal();
                if (BackgroundImage != null) {

                    using (TextureBrush textureBrush = new TextureBrush(BackgroundImage,WrapMode.Tile)) {
                        g.FillRectangle(textureBrush, ClientRectangle);
                    }
                }
                else{
                    using (SolidBrush solidBrush = new SolidBrush(this.BackColor)) {
                        g.FillRectangle(solidBrush, splitterRect);
                    }
                }
                g.Dispose();
            }
        }


        private void SetSplitterRect(bool vertical) {
            if (vertical)
            {
                splitterRect.X = ((RightToLeft == RightToLeft.Yes) ? this.Width - splitterDistance - SplitterWidthInternal : this.Location.X + splitterDistance);
                splitterRect.Y = this.Location.Y;
                splitterRect.Width = SplitterWidthInternal;
                splitterRect.Height = this.Height;
            }
            else
            {
                splitterRect.X = this.Location.X;
                splitterRect.Y = this.Location.Y + SplitterDistanceInternal;
                splitterRect.Width = this.Width;
                splitterRect.Height = SplitterWidthInternal;
            }
        }
        
        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.ResizeSplitContainer"]/*' />
        /// <devdoc>
        ///     Reize SplitContainer
        /// </devdoc>
        /// <internalonly/>
        private void ResizeSplitContainer()
        {
            if (splitContainerScaling)
            {
                return;
            }

            panel1.SuspendLayout();
            panel2.SuspendLayout();
                        
            if (this.Width == 0) {         // Set the correct Width iif the WIDTH has changed to ZERO.
                panel1.Size = new Size(0, panel1.Height);
                panel2.Size = new Size(0, panel2.Height);
            }
            else if (this.Height == 0) {   // Set the correct Height iif the HEIGHT has changed to ZERO.
                panel1.Size = new Size(panel1.Width, 0);
                panel2.Size = new Size(panel2.Width, 0);
            }
            else 
            {
                if (Orientation == Orientation.Vertical) 
                {
                    // If no panel is collapsed then do the default ...
                    if (!CollapsedMode) 
                    {
                        if (this.FixedPanel == FixedPanel.Panel1) {
                            panel1.Size = new Size(panelSize, Height);
                            panel2.Size = new Size(Math.Max(Width - panelSize - SplitterWidthInternal, Panel2MinSize), Height);
                        }
                        if (this.FixedPanel == FixedPanel.Panel2) {
                            panel2.Size = new Size(panelSize, Height);
                            splitterDistance = Math.Max(Width - panelSize - SplitterWidthInternal, Panel1MinSize);
                            panel1.WidthInternal = splitterDistance;
                            panel1.HeightInternal = Height;
                        }
                        if (this.FixedPanel == FixedPanel.None) {
                            if (ratioWidth != 0.0) {
                                splitterDistance = Math.Max((int)(Math.Floor(this.Width / ratioWidth)), Panel1MinSize);
                            }
                            panel1.WidthInternal = splitterDistance; //Default splitter distance from left or top.
                            panel1.HeightInternal = Height;
                            panel2.Size = new Size(Math.Max(Width - splitterDistance - SplitterWidthInternal, Panel2MinSize), Height);
                        }
                        if (RightToLeft == RightToLeft.No) {
                            panel2.Location = new Point(panel1.WidthInternal + SplitterWidthInternal, 0);
                        }
                        else {
                            panel1.Location = new Point(Width - panel1.WidthInternal, 0);
                        }
                        RepaintSplitterRect();
                        SetSplitterRect(true);
                    }
                    else
                    {
                        if (Panel1Collapsed) {
                            panel2.Size = this.Size;
                            panel2.Location = new Point(0,0);
                        }
                        else if (Panel2Collapsed) {
                            panel1.Size = this.Size;
                            panel1.Location = new Point(0,0);
                        }
                    }

                }
                else if (Orientation == Orientation.Horizontal) {
                    // If no panel is collapsed then do the default ...
                    if (!CollapsedMode) 
                    {
                        if (this.FixedPanel == FixedPanel.Panel1) {

                            //Default splitter distance from left or top.
                            panel1.Size = new Size(Width, panelSize);
                            int panel2Start = panelSize + SplitterWidthInternal;
                            panel2.Size = new Size(Width, Math.Max(Height - panel2Start, Panel2MinSize));
                            panel2.Location = new Point(0,panel2Start);
                        }
                        if (this.FixedPanel == FixedPanel.Panel2) {

                            panel2.Size = new Size(Width, panelSize);
                            splitterDistance = Math.Max(Height - Panel2.Height - SplitterWidthInternal, Panel1MinSize);
                            panel1.HeightInternal = splitterDistance;
                            panel1.WidthInternal = Width;
                            int panel2Start = splitterDistance + SplitterWidthInternal;
                            panel2.Location = new Point(0, panel2Start);
                        }
                        if (this.FixedPanel == FixedPanel.None) {
                            //NO PANEL FIXED !!
                            if (ratioHeight != 0.0)
                            {
                                splitterDistance = Math.Max((int)(Math.Floor(this.Height / ratioHeight )), Panel1MinSize);
                            }
                            panel1.HeightInternal = splitterDistance; //Default splitter distance from left or top.
                            panel1.WidthInternal = Width;
                            int panel2Start = splitterDistance + SplitterWidthInternal;
                            panel2.Size = new Size(Width,Math.Max(Height - panel2Start, Panel2MinSize));
                            panel2.Location = new Point(0,panel2Start);
                            

                        }
                        RepaintSplitterRect();
                        SetSplitterRect(false);
                    }
                    else
                    {
                        if (Panel1Collapsed) {
                            panel2.Size = this.Size;
                            panel2.Location = new Point(0,0);
                        }
                        else if (Panel2Collapsed) {
                            panel1.Size = this.Size;
                            panel1.Location = new Point(0,0);
                        }
                    }
                }
                try {
                    resizeCalled = true;
                    ApplySplitterDistance();
                }
                finally {
                    resizeCalled = false;
                }
            }
            panel1.ResumeLayout();
            panel2.ResumeLayout();
        }


        /// <devdoc>
        ///     Scales an individual control's location, size, padding and margin.
        ///     If the control is top level, this will not scale the control's location.
        ///     This does not scale children or the size of auto sized controls.  You can
        ///     omit scaling in any direction by changing BoundsSpecified.
        ///
        ///     After the control is scaled the RequiredScaling property is set to
        ///     BoundsSpecified.None.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            try
            {
                splitContainerScaling = true;
                base.ScaleControl(factor, specified);

                float scale;
                if (orientation == Orientation.Vertical) {
                    scale = factor.Width;
                }
                else {
                    scale = factor.Height;
                }
                SplitterWidth = (int)Math.Round((float)SplitterWidth * scale);
            }
            finally
            {
                splitContainerScaling = false;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.Select"]/*' />
        protected override void Select(bool directed, bool forward) {
            // avoid re-entrant code.
            // SelectNextControl can call back on us.. and we might end up infinitely recursing.
            if (selectNextControl)
            {
                return;
            }
            // continue selection iff panels have controls or tabstop is true.
            if ((this.Panel1.Controls.Count > 0 || this.Panel2.Controls.Count > 0) || TabStop) {
                SelectNextControlInContainer(this, forward, true, true, false);
            }
            else { //If this SplitContainer cannot be selected let the parent select the next in line
                try {
                    Control parent = this.ParentInternal;
                    selectNextControl = true;
                    while (parent != null) {
                        if (parent.SelectNextControl(this, forward, true, true, parent.ParentInternal == null)) {
                            break;
                        }
                        parent = parent.ParentInternal;
                    }
                }
                finally {
                    selectNextControl = false;
                }
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SelectNextControl"]/*' />
        /// <devdoc>
        ///     Selects the next control following ctl.
        /// </devdoc>
        private bool SelectNextControlInContainer(Control ctl, bool forward, bool tabStopOnly,
                                      bool nested, bool wrap) {
            if (!Contains(ctl) || !nested && ctl.ParentInternal != this) ctl = null;
            Control start = ctl;
            SplitterPanel firstPanel = null;
            do {
                ctl = GetNextControl(ctl, forward);

                SplitterPanel panel = ctl as SplitterPanel;
                if (panel != null && panel.Visible) {
                    //We have crossed over to the second Panel...
                    if (firstPanel != null) {
                        break;
                    }
                    firstPanel = panel;
                }
                if (!forward && firstPanel != null && ctl.ParentInternal != firstPanel) {
                    //goback to start correct re-ordering ....
                    ctl = firstPanel;
                    break;
                }
                if (ctl == null) {
                    break;
                }
                else {
                    if (ctl.CanSelect && ctl.TabStop) {
                        if (ctl is SplitContainer)
                        {
                            ((SplitContainer)ctl).Select(forward, forward);
                        }
                        else
                        {
                            SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
                        }
                        return true;
                    }
                }
            } while (ctl != null);
            if (ctl != null && this.TabStop) {
                //we are on Splitter.....Focus it
                splitterFocused = true;
                IContainerControl c = this.ParentInternal.GetContainerControlInternal();
                if (c != null) {
                    ContainerControl cc = c as ContainerControl;
                    if (cc == null) {
                        c.ActiveControl = this;
                    }
                    else {
                        IntSecurity.ModifyFocus.Demand();
                        cc.SetActiveControlInternal(this);
                    }
                }
                SetActiveControlInternal(null);
                nextActiveControl = ctl;
                return true;
            }
            else
            { 
                // If the splitter cannot be selected select the next control in the splitter
                bool selected = SelectNextControlInPanel(ctl, forward, tabStopOnly, nested, wrap);
                if (!selected)
                {
                    Control parent = this.ParentInternal;
                    if (parent != null)
                    {
                        try
                        {
                            selectNextControl = true;
                            parent.SelectNextControl(this, forward, true, true, true);
                        }
                        finally
                        {
                            selectNextControl = false;
                        }
                    }
                }
            }
            return false;
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SelectNextControl"]/*' />
        /// <devdoc>
        ///     Selects the next control following ctl.
        /// </devdoc>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private bool SelectNextControlInPanel(Control ctl, bool forward, bool tabStopOnly,
                                      bool nested, bool wrap) {

            if (!Contains(ctl) || !nested && ctl.ParentInternal != this) ctl = null;
            Control start = ctl;
            do {
                ctl = GetNextControl(ctl, forward);
                if (ctl == null || (ctl is SplitterPanel && ctl.Visible)) {
                    break;
                }
                else {
                    if (ctl.CanSelect && (!tabStopOnly || ctl.TabStop)) {
                        if (ctl is SplitContainer)
                        {
                            ((SplitContainer)ctl).Select(forward, forward);
                        }
                        else
                        {
                            SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
                        }
                        return true;
                    }
                }
            } while (ctl != null);

            //If CTL == null .. we r out of the Current SplitContainer...
            if (ctl == null || (ctl is SplitterPanel && !ctl.Visible)) {
                callBaseVersion = true;

            }
            //IF the CTL == typeof(SpliterPanel) find the NEXT Control... so that we know
            // we can focus the NEXT control within this SPLITCONTAINER....
            else
            {
                ctl = GetNextControl(ctl, forward);
                if (forward) {
                    nextActiveControl = panel2;
                }
                else {
                    if (ctl == null || !(ctl.ParentInternal.Visible)) {
                        callBaseVersion = true;
                    }
                    else
                        nextActiveControl = panel2;
                }
            }
            return false;
        }

        // This will select the correct active control in the containerControl (if the passed in control is a containerControl)
        private static void SelectNextActiveControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            ContainerControl container = ctl as ContainerControl;
            if (container != null)
            {
                bool correctParentActiveControl = true;
                if (container.ParentInternal != null)
                {
                    IContainerControl c = container.ParentInternal.GetContainerControlInternal();
                    if (c != null)
                    {
                        c.ActiveControl = container;
                        correctParentActiveControl = (c.ActiveControl == container);
                    }
                }
                if (correctParentActiveControl)
                {
                    ctl.SelectNextControl(null, forward, tabStopOnly, nested, wrap);
                }
            }
            else
            {
                ctl.Select();
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SetInnerMostBorder"]/*' />
        /// <devdoc>
        ///     Selects the innermost PANEL.
        /// </devdoc>
        private void SetInnerMostBorder(SplitContainer sc) {
            foreach(Control ctl in sc.Controls) {
                bool foundChildSplitContainer = false;
                if (ctl is SplitterPanel) {
                    foreach (Control c in ctl.Controls) {
                        SplitContainer c1 = c as SplitContainer;
                        if (c1 != null && c1.Dock == DockStyle.Fill) {
                           // We need to Overlay borders
                           // if the Children have matching BorderStyles ...
                           if (c1.BorderStyle != BorderStyle) {
                               break;
                           }
                           ((SplitterPanel)ctl).BorderStyle = BorderStyle.None;
                           SetInnerMostBorder(c1);
                           foundChildSplitContainer = true;
                        }
                    }
                    if (!foundChildSplitContainer) {
                        ((SplitterPanel)ctl).BorderStyle = BorderStyle;
                    }
                }
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SetBoundsCore"]/*' />
        /// <devdoc>
        ///     This protected override allows us to check is an unvalid value is set for Width and Height.
        ///     The SplitContainer would not throw on invalid Size (i.e Width and Height) settings, but would correct the error like Form
        ///     Say, the Panel1MinSize == 150 , Panel2MinSize == 50 and SplitterWidth == 4 and the user tries
        ///     to set SplitContainer.Width = 50 ... then this function would try to correct the value to 204.. instead of throwing.
        /// </devdoc>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {

            // If we are changing Height, check if its greater than minimun else ... make it equal to the minimum
            if ((specified & BoundsSpecified.Height) != BoundsSpecified.None && Orientation == Orientation.Horizontal) {
                if (height < Panel1MinSize + SplitterWidthInternal + Panel2MinSize)
                {
                    height = Panel1MinSize + SplitterWidthInternal + Panel2MinSize;
                }
            }

            // If we are changing Width, check if its greater than minimun else ... make it equal to the minimum
            if ((specified & BoundsSpecified.Width) != BoundsSpecified.None && Orientation == Orientation.Vertical) {
                if (width < Panel1MinSize + SplitterWidthInternal + Panel2MinSize)
                {
                    width = Panel1MinSize + SplitterWidthInternal + Panel2MinSize;
                }
            }
            
            base.SetBoundsCore(x, y, width, height, specified);

            SetSplitterRect(this.Orientation == Orientation.Vertical);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitBegin"]/*' />
        /// <devdoc>
        ///     Begins the splitter moving.
        /// </devdoc>
        /// <internalonly/>
        private void SplitBegin(int x, int y) {
            anchor = new Point(x, y);
            splitterDistance = GetSplitterDistance(x, y);
            initialSplitterDistance = splitterDistance;
            initialSplitterRectangle = SplitterRectangle;

            // 






            IntSecurity.UnmanagedCode.Assert();
            try {
                if (splitContainerMessageFilter == null)
                {
                   splitContainerMessageFilter = new SplitContainerMessageFilter(this);
                }
                Application.AddMessageFilter(splitContainerMessageFilter);
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }
            CaptureInternal = true;
            DrawSplitBar(DRAW_START);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitMove"]/*' />
        /// <devdoc>
        ///     The split movement.
        /// </devdoc>
        /// <internalonly/>
        private void SplitMove(int x, int y) {
            int size = GetSplitterDistance(x, y);
            int delta = size - initialSplitterDistance;
            int mod = delta % SplitterIncrement;
            if (splitterDistance != size) {
                if (Orientation == Orientation.Vertical)
                {
                	if (size + SplitterWidthInternal <= this.Width - Panel2MinSize - BORDERSIZE)
                	{
                		splitterDistance = size - mod;
                	}
                }
                else
                {
                	if (size + SplitterWidthInternal <= this.Height - Panel2MinSize - BORDERSIZE)
                	{
                		splitterDistance = size - mod;
                	}
                }
            }
            DrawSplitBar(DRAW_MOVE);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.SplitEnd"]/*' />
        /// <devdoc>
        ///     Finishes the split movement.
        /// </devdoc>
        /// <internalonly/>
        private void SplitEnd(bool accept) {
            DrawSplitBar(DRAW_END);
            if (splitContainerMessageFilter != null)
            {
                Application.RemoveMessageFilter(splitContainerMessageFilter);    
                splitContainerMessageFilter = null;
            }

            if (accept) {
                ApplySplitterDistance();
                
            }
            else if (splitterDistance != initialSplitterDistance) {
                splitterClick = false;
                splitterDistance = SplitterDistanceInternal = initialSplitterDistance;
            }
            anchor = Point.Empty;

        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.UpdateSplitter"]/*' />
        /// <devdoc>
        ///     Update Splitter
        /// </devdoc>
        /// <internalonly/>
        private void UpdateSplitter() {
            if (splitContainerScaling)
            {
                return;
            }
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            if (Orientation == Orientation.Vertical) {
                bool isRTL = RightToLeft == RightToLeft.Yes;

                //NO PANEL FIXED !!
                if (!CollapsedMode) {

                    panel1.HeightInternal = Height;
                    panel1.WidthInternal = splitterDistance; //Default splitter distance from left or top.
                    panel2.Size = new Size(Width - splitterDistance - SplitterWidthInternal, Height);
                    
                    if (!isRTL) {
                        panel1.Location = new Point(0,0);
                        panel2.Location = new Point(splitterDistance + SplitterWidthInternal, 0);
                    }
                    else {
                        panel1.Location = new Point(Width - splitterDistance, 0);
                        panel2.Location = new Point(0, 0);
                    }

                    RepaintSplitterRect();
                    SetSplitterRect(true /*Vertical*/);
                    if (!resizeCalled) {
                        ratioWidth =  ((double)(this.Width) / (double)(panel1.Width) > 0) ? (double)(this.Width) / (double)(panel1.Width) : ratioWidth;
                    }
                    
                        
                }
                else {
                    if (Panel1Collapsed) {
                        panel2.Size = this.Size;
                        panel2.Location = new Point(0,0);
                    }
                    else if (Panel2Collapsed) {
                        panel1.Size = this.Size;
                        panel1.Location = new Point(0,0);
                    }
                    // Update Ratio when the splitContainer is in CollapsedMode.
                    if (!resizeCalled)
                    {
                    	ratioWidth = ((double)(this.Width) / (double)(splitterDistance) > 0) ? (double)(this.Width) / (double)(splitterDistance) : ratioWidth;
                    }
                }
            }
            else {
                //NO PANEL FIXED !!
                if (!CollapsedMode) {
                    panel1.Location = new Point(0,0);
                    panel1.WidthInternal = Width;

                    panel1.HeightInternal = SplitterDistanceInternal; //Default splitter distance from left or top.
                    int panel2Start = splitterDistance + SplitterWidthInternal;
                    panel2.Size = new Size(Width, Height - panel2Start);
                    panel2.Location = new Point(0,panel2Start);

                    RepaintSplitterRect();
                    SetSplitterRect(false/*Horizontal*/);
                    
                    if (!resizeCalled) {
                        ratioHeight =  ((double)(this.Height) / (double)(panel1.Height) > 0) ? (double)(this.Height) / (double)(panel1.Height) : ratioHeight;
                    }
                }
                else {
                    if (Panel1Collapsed) {
                        panel2.Size = this.Size;
                        panel2.Location = new Point(0,0);
                    }
                    else if (Panel2Collapsed) {
                        panel1.Size = this.Size;
                        panel1.Location = new Point(0,0);
                    }

                    // Update Ratio when the splitContainer is in CollapsedMode.
                    if (!resizeCalled)
                    {
                    	ratioHeight = ((double)(this.Height) / (double)(splitterDistance) > 0) ? (double)(this.Height) / (double)(splitterDistance) : ratioHeight;
                    }
                }
            }
            panel1.ResumeLayout();
            panel2.ResumeLayout(); 
         }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.WmSetCursor"]/*' />
        /// <devdoc>
        ///     Handles the WM_SETCURSOR message
        /// </devdoc>
        /// <internalonly/>
        private void WmSetCursor(ref Message m) {

            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            //
            if (m.WParam == InternalHandle && ((int)m.LParam & 0x0000FFFF) == NativeMethods.HTCLIENT) {
                if (OverrideCursor != null) {
                    Cursor.CurrentInternal = OverrideCursor;
                }
                else {
                    Cursor.CurrentInternal = Cursor;
                }
            }
            else {
                DefWndProc(ref m);
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                               //
        // END PRIVATE FUNCTIONS ...                                                                     //
        //                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////




        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                               //
        // Start PROTECTED OVERRIDE FUNCTIONS                                                            //
        //                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        internal override Rectangle GetToolNativeScreenRectangle() {
            // Return splitter rectangle instead of the whole container rectangle to be consistent with the mouse ToolTip
            Rectangle containerRectangle = base.GetToolNativeScreenRectangle();
            Rectangle splitterRectangle = this.SplitterRectangle;
            return new Rectangle(containerRectangle.X + splitterRectangle.X, containerRectangle.Y + splitterRectangle.Y, splitterRectangle.Width, splitterRectangle.Height);
        }

        internal override void AfterControlRemoved(Control control, Control oldParent) {
            base.AfterControlRemoved(control, oldParent);
            if (control is SplitContainer && control.Dock == DockStyle.Fill)
            {
               SetInnerMostBorder(this); 
            }
            
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.ProcessDialogKey"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Processes a dialog key. Overrides Control.processDialogKey(). This
        ///    method implements handling of the TAB, LEFT, RIGHT, UP, and DOWN
        ///    keys in dialogs.
        ///    The method performs no processing on keys that include the ALT or
        ///    CONTROL modifiers. For the TAB key, the method selects the next control
        ///    on the form. For the arrow keys,
        ///    !!!
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData) {
#if DEBUG
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessDialogKey [" + keyData.ToString() + "]");
#endif
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None) {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                switch (keyCode) {
                    case Keys.Tab:
                        if (ProcessTabKey((keyData & Keys.Shift) == Keys.None)) return true;
                        break;
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        if (!splitterFocused) {
                            if (ProcessArrowKey(keyCode == Keys.Right ||
                                            keyCode == Keys.Down)) return true;
                        }
                        else
                            return false;
                        break;

                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.ProcessDialogKey"]/*' />
        /// /// <devdoc>
        ///   This will process the TabKey for the SplitContainer. The Focus needs to Shift from controls to the Left of the Splitter
        ///   to the splitter and then to the controls on the right of the splitter. This override implements this Logic.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessTabKey(bool forward) {
            //Dont Focus the Splitter if TabStop == False or if the Splitter is Fixed !!
            if (!TabStop || IsSplitterFixed) {
                return base.ProcessTabKey(forward);
            }

            if (nextActiveControl != null) {
                SetActiveControlInternal(nextActiveControl);
                nextActiveControl = null;
            }

            if (SelectNextControlInPanel(ActiveControl, forward, true, true, true)) {
                nextActiveControl = null;
                splitterFocused = false;
                return true;
            }
            else {
                if (callBaseVersion) {
                    callBaseVersion = false;
                    return base.ProcessTabKey(forward);
                }
                else {
                    //We are om Splitter ......
                    splitterFocused = true;
                    IContainerControl c = this.ParentInternal.GetContainerControlInternal();
                    if (c != null) {
                        ContainerControl cc = c as ContainerControl;
                        if (cc == null) {
                            c.ActiveControl = this;
                        }
                        else {
                            cc.SetActiveControlInternal(this);
                        }
                    }
                    SetActiveControlInternal(null);
                    return true;
                }
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e) {
            base.OnMouseCaptureChanged(e);
            if (splitContainerMessageFilter != null)
            {
                Application.RemoveMessageFilter(splitContainerMessageFilter);
                splitContainerMessageFilter = null;
            }
        }

        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.WndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message msg) {
            switch (msg.Msg) {
                case NativeMethods.WM_SETCURSOR:
                    WmSetCursor(ref msg);
                    break;
                case NativeMethods.WM_SETFOCUS:
                    splitterFocused = true;
                    base.WndProc(ref msg);
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    splitterFocused = false;
                    base.WndProc(ref msg);
                    break;

                default:
                    base.WndProc(ref msg);
                    break;
            }
        }


        /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.CreateControlsInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance() {
            return new SplitContainerTypedControlCollection(this, typeof(SplitterPanel), /*isReadOnly*/true);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                 //
        // End   PROTECTED OVERRIDE FUNCTIONS                                              //
        //                                                                                 //
        ///////////////////////////////////////////////////////////////////////////////////////////////////


        private class SplitContainerMessageFilter : IMessageFilter 
        {
            private SplitContainer owner = null;

            public SplitContainerMessageFilter(SplitContainer splitContainer)
            {
                this.owner = splitContainer;
            }
            
            /// <include file='doc\SplitContainer.uex' path='docs/doc[@for="SplitContainer.PreFilterMessage"]/*' />
            /// <devdoc>
            /// </devdoc>
            /// <internalonly/>
            [
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
            ]
            bool IMessageFilter.PreFilterMessage(ref Message m) {
                if (m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST) {
                    if ((m.Msg == NativeMethods.WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape)
                        || (m.Msg == NativeMethods.WM_SYSKEYDOWN)) {
                        //Notify that splitMOVE was reverted ..
                        //this is used in ONKEYUP!!
                        owner.splitBegin = false;
                        owner.SplitEnd(false);
                        owner.splitterClick  = false;
                        owner.splitterDrag = false;
                    }
                    return true;
                }
                return false;
            }
        }

         /// <devdoc>
         /// This control collection only allows a specific type of control
         /// into the controls collection.  It optionally supports readonlyness.
         /// </devdoc>
         internal class SplitContainerTypedControlCollection : WindowsFormsUtils.TypedControlCollection {
            SplitContainer owner;

            public SplitContainerTypedControlCollection(Control c, Type type, bool isReadOnly): base(c, type, isReadOnly)
            {
              this.owner = c as SplitContainer;
            }

            public override void Remove(Control value) {
                if (value is SplitterPanel) {
                    if (!owner.DesignMode) {
                        if (IsReadOnly) throw new NotSupportedException(SR.ReadonlyControlsCollection);
                    }
                }
                base.Remove(value);
            }

            internal override void SetChildIndexInternal(Control child, int newIndex)
            {
               if (child is SplitterPanel) {
                   if (!owner.DesignMode) {
                       if (IsReadOnly) {
                           throw new NotSupportedException(SR.ReadonlyControlsCollection);
                       }
                   }
                   else {
                       // just no-op it at DT.
                       return;
                   }
               }
               base.SetChildIndexInternal(child, newIndex);
            }

        }

    }
 }
