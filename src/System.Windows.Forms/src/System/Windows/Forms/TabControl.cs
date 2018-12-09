// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.





namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Configuration.Assemblies;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;
    using System.Globalization;

    /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl"]/*' />
    /// <devdoc>
    ///     The TabControl.  This control has a lot of the functionality of a TabStrip
    ///     but manages a list of TabPages which are the 'pages' that appear on each tab.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(TabPages)),
    DefaultEvent(nameof(SelectedIndexChanged)),
    Designer("System.Windows.Forms.Design.TabControlDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionTabControl))
    ]
    public class TabControl : Control {

        private static readonly Size DEFAULT_ITEMSIZE = Size.Empty;
        private static readonly Point DEFAULT_PADDING = new Point(6, 3);

        //properties
        private TabPageCollection tabCollection;
        private TabAlignment alignment         = TabAlignment.Top;
        private TabDrawMode  drawMode          = TabDrawMode.Normal;
        private ImageList imageList                = null;
        private Size itemSize                     = DEFAULT_ITEMSIZE;
        private Point padding                      = DEFAULT_PADDING;
        private TabSizeMode   sizeMode         = TabSizeMode.Normal;
        private TabAppearance appearance       = TabAppearance.Normal;
        private Rectangle cachedDisplayRect        = Rectangle.Empty;
        private bool currentlyScaling          = false;
        private int selectedIndex = -1;
        private Size cachedSize = Size.Empty;
        private string controlTipText = String.Empty;
        private bool handleInTable;

        //events
        private EventHandler onSelectedIndexChanged;
        private DrawItemEventHandler onDrawItem;

        //events
        private static readonly object EVENT_DESELECTING = new object();
        private static readonly object EVENT_DESELECTED = new object();
        private static readonly object EVENT_SELECTING = new object();
        private static readonly object EVENT_SELECTED = new object();
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED = new object();


        private const int   TABCONTROLSTATE_hotTrack                        = 0x00000001;
        private const int   TABCONTROLSTATE_multiline                       = 0x00000002;
        private const int   TABCONTROLSTATE_showToolTips                    = 0x00000004;
        private const int   TABCONTROLSTATE_getTabRectfromItemSize          = 0x00000008;
        private const int   TABCONTROLSTATE_fromCreateHandles               = 0x00000010;
        private const int   TABCONTROLSTATE_UISelection                     = 0x00000020;
        private const int   TABCONTROLSTATE_selectFirstControl              = 0x00000040;
        private const int   TABCONTROLSTATE_insertingItem                   = 0x00000080;
        private const int   TABCONTROLSTATE_autoSize                        = 0x00000100;
        

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32  tabControlState; // see TABCONTROLSTATE_ consts above



        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.tabBaseReLayoutMessage"]/*' />
        /// <devdoc>
        ///     This message is posted by the control to itself after a TabPage is
        ///     added to it.  On certain occasions, after items are added to a
        ///     TabControl in quick succession, TCM_ADJUSTRECT calls return the wrong
        ///     display rectangle.  When the message is received, the control calls
        ///     updateTabSelection() to layout the TabPages correctly.
        /// </devdoc>
        /// <internalonly/>
        private readonly int tabBaseReLayoutMessage = SafeNativeMethods.RegisterWindowMessage(Application.WindowMessagesVersion + "_TabBaseReLayout");

        //state
        private TabPage[] tabPages;
        private int tabPageCount;
        private int lastSelection;

        private bool             rightToLeftLayout = false;
        private bool             skipUpdateSize;

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabControl"]/*' />
        /// <devdoc>
        ///     Constructs a TabBase object, usually as the base class for a TabStrip or TabControl.
        /// </devdoc>
        public TabControl()
        : base() {

            tabControlState = new System.Collections.Specialized.BitVector32(0x00000000);

            tabCollection = new TabPageCollection(this);
            SetStyle(ControlStyles.UserPaint, false);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Alignment"]/*' />
        /// <devdoc>
        ///     Returns on what area of the control the tabs reside on (A TabAlignment value).
        ///     The possibilities are Top (the default), Bottom, Left, and Right.  When alignment
        ///     is left or right, the Multiline property is ignored and Multiline is implicitly on.
        ///     If the alignment is anything other than top, TabAppearance.FlatButtons degenerates
        ///     to TabAppearance.Buttons.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(TabAlignment.Top),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.TabBaseAlignmentDescr))
        ]
        public TabAlignment Alignment {
            get {
                return alignment;
            }

            set {
                if (this.alignment != value) {
                    //valid values are 0x0 to 0x3
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabAlignment.Top, (int)TabAlignment.Right)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabAlignment));
                    }

                    this.alignment = value;
                    if (this.alignment == TabAlignment.Left || this.alignment == TabAlignment.Right)
                        Multiline = true;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Appearance"]/*' />
        /// <devdoc>
        ///     Indicates whether the tabs in the tabstrip look like regular tabs, or if they look
        ///     like buttons as seen in the Windows 95 taskbar.
        ///     If the alignment is anything other than top, TabAppearance.FlatButtons degenerates
        ///     to TabAppearance.Buttons.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(TabAppearance.Normal),
        SRDescription(nameof(SR.TabBaseAppearanceDescr))
        ]
        public TabAppearance Appearance {
            get {
                if (appearance == TabAppearance.FlatButtons && alignment != TabAlignment.Top) {
                    return TabAppearance.Buttons;
                }
                else {
                    return appearance;
                }
            }

            set {
                if (this.appearance != value) {
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabAppearance.Normal, (int)TabAppearance.FlatButtons)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabAppearance));
                    }

                    this.appearance = value;
                    RecreateHandle();

                    //Fire OnStyleChanged(EventArgs.Empty) here since we are no longer calling UpdateStyles( ) but always reCreating the Handle.
                    OnStyleChanged(EventArgs.Empty);
                }
            }
        }
       

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor {
            get {
                //The tab control can only be rendered in 1 color: System's Control color.
                //So, always return this value... otherwise, we're inheriting the forms backcolor
                //and passing it on to the pab pages.
                return SystemColors.Control;
            }
            set {
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged {
            add {
                base.BackColorChanged += value;
            }
            remove {
                base.BackColorChanged -= value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(200, 100);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DoubleBuffered"]/*' />
        /// <devdoc>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                base.DoubleBuffered = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ForeColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.CreateParams"]/*' />
        /// <devdoc>
        ///     Returns the parameters needed to create the handle.  Inheriting classes
        ///     can override this to provide extra functionality.  They should not,
        ///     however, forget to call base.getCreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_TABCONTROL;

                // set up window styles
                //
                if (Multiline == true) cp.Style |= NativeMethods.TCS_MULTILINE;
                if (drawMode == TabDrawMode.OwnerDrawFixed) cp.Style |= NativeMethods.TCS_OWNERDRAWFIXED;
                if (ShowToolTips && !DesignMode) {
                    cp.Style |= NativeMethods.TCS_TOOLTIPS;
                }

                if (alignment == TabAlignment.Bottom ||
                    alignment == TabAlignment.Right)
                    cp.Style |= NativeMethods.TCS_BOTTOM;

                if (alignment == TabAlignment.Left ||
                    alignment == TabAlignment.Right)
                    cp.Style |= NativeMethods.TCS_VERTICAL | NativeMethods.TCS_MULTILINE;

                if (tabControlState[TABCONTROLSTATE_hotTrack]) {
                    cp.Style |= NativeMethods.TCS_HOTTRACK;
                }

                if (appearance == TabAppearance.Normal)
                    cp.Style |= NativeMethods.TCS_TABS;
                else {
                    cp.Style |= NativeMethods.TCS_BUTTONS;
                    if (appearance == TabAppearance.FlatButtons && alignment == TabAlignment.Top)
                        cp.Style |= NativeMethods.TCS_FLATBUTTONS;
                }

                switch (sizeMode) {
                    case TabSizeMode.Normal:
                        cp.Style |= NativeMethods.TCS_RAGGEDRIGHT;
                        break;
                    case TabSizeMode.FillToRight:
                        cp.Style |= NativeMethods.TCS_RIGHTJUSTIFY;
                        break;
                    case TabSizeMode.Fixed:
                        cp.Style |= NativeMethods.TCS_FIXEDWIDTH;
                        break;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true) {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL | NativeMethods.WS_EX_NOINHERITLAYOUT;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DisplayRectangle"]/*' />
        /// <devdoc>
        ///     The rectangle that represents the Area of the tab strip not
        ///     taken up by the tabs, borders, or anything else owned by the Tab.  This
        ///     is typically the rectangle you want to use to place the individual
        ///     children of the tab strip.
        /// </devdoc>
        public override Rectangle DisplayRectangle {
            get {

                
                // Null out cachedDisplayRect whenever we do anything to change it...
                //
                if (!cachedDisplayRect.IsEmpty)
                    return cachedDisplayRect;

                Rectangle bounds = Bounds;
                NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(bounds.X, bounds.Y, bounds.Width, bounds.Height);

                
                
                // We force a handle creation here, because otherwise the DisplayRectangle will be wildly inaccurate
                //
                if (!IsDisposed)
                {
                    // Since this is called thru the OnResize (and Layout) which is triggered by SetExtent if the TabControl is hosted as
                    // a ActiveX control, so check if this is ActiveX and dont force Handle Creation here as the native code breaks in this case.
                    if (!IsActiveX)
                    {
                        if (!IsHandleCreated) {
                            CreateHandle();
                        }
                    }
                    if (IsHandleCreated)
                    {
                        SendMessage(NativeMethods.TCM_ADJUSTRECT, 0, ref rect);
                    }
                }

                Rectangle r = Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

                Point p = this.Location;
                r.X -= p.X;
                r.Y -= p.Y;

                cachedDisplayRect = r;
                return r;
            }
        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DrawMode"]/*' />
        /// <devdoc>
        ///     The drawing mode of the tabs in the tab strip.  This will indicate
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(TabDrawMode.Normal),
        SRDescription(nameof(SR.TabBaseDrawModeDescr))
        ]
        public TabDrawMode DrawMode {
            get {
                return drawMode;
            }

            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabDrawMode.Normal, (int)TabDrawMode.OwnerDrawFixed)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabDrawMode));
                }

                if (drawMode != value) {
                    drawMode = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.HotTrack"]/*' />
        /// <devdoc>
        ///     Indicates whether the tabs visually change when the mouse passes over them.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TabBaseHotTrackDescr))
        ]
        public bool HotTrack {
            get {
                return tabControlState[TABCONTROLSTATE_hotTrack];
            }

            set {
                if (HotTrack != value) {
                    tabControlState[TABCONTROLSTATE_hotTrack] = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ImageList"]/*' />
        /// <devdoc>
        ///     Returns the imageList the control points at.  This is where tabs that have imageIndex
        ///     set will get there images from.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(null),
        SRDescription(nameof(SR.TabBaseImageListDescr))
        ]
        public ImageList ImageList {
            get {
                return imageList;
            }
            set {
                if (this.imageList != value) {
                    EventHandler recreateHandler = new EventHandler(ImageListRecreateHandle);
                    EventHandler disposedHandler = new EventHandler(DetachImageList);

                    if (imageList != null) {
                        imageList.RecreateHandle -= recreateHandler;
                        imageList.Disposed -= disposedHandler;
                    }

                    this.imageList = value;
                    IntPtr handle = (value != null) ? value.Handle : IntPtr.Zero;
                    if (IsHandleCreated)
                        SendMessage(NativeMethods.TCM_SETIMAGELIST, IntPtr.Zero, handle);

                    // Update the image list in the tab pages.
                    foreach (TabPage tabPage in TabPages) {
                        tabPage.ImageIndexer.ImageList = value;
                    }


                    if (value != null) {
                        value.RecreateHandle += recreateHandler;
                        value.Disposed += disposedHandler;
                    }
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ItemSize"]/*' />
        /// <devdoc>
        ///     By default, tabs will automatically size themselves to fit their icon, if any, and their label.
        ///     However, the tab size can be explicity set by setting this property.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.TabBaseItemSizeDescr))
        ]
        public Size ItemSize {
            get {
                if (itemSize.IsEmpty) {

                    // Obtain the current itemsize of the first tab from the winctl control
                    //
                    if (IsHandleCreated) {
                        tabControlState[TABCONTROLSTATE_getTabRectfromItemSize] = true;
                        return GetTabRect(0).Size;
                    }
                    else {
                        return DEFAULT_ITEMSIZE;
                    }
                }
                else {
                    return itemSize;
                }
            }

            set {
                if (value.Width < 0 || value.Height < 0) {
                    throw new ArgumentOutOfRangeException(nameof(ItemSize), string.Format(SR.InvalidArgument, "ItemSize", value.ToString()));
                }
                itemSize = value;
                ApplyItemSize();
                UpdateSize();
                Invalidate();
            }
        }

        /// <devdoc>
        ///     This private property is set by the TabPageCollection when the user calls "InsertItem".
        ///     The problem is when InsertItem is called then we add this item to the ControlsCollection (in addition to the TabPageCollection)
        ///     to keep both the collections is sync. But the controlCollection.Add is overriden to again ADD the item to the TabPageCollection.
        ///     So we keep this flag in order to aviod repeatd addition (only during insert)
        ///     When the Add ends ... we reset this flag.
        /// </devdoc>
        private bool InsertingItem {
            get {
                return (bool)tabControlState[TABCONTROLSTATE_insertingItem];
            }
            set {
                tabControlState[TABCONTROLSTATE_insertingItem] = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Multiline"]/*' />
        /// <devdoc>
        ///     Indicates if there can be more than one row of tabs.  By default [when
        ///     this property is false], if there are more tabs than available display
        ///     space, arrows are shown to let the user navigate between the extra
        ///     tabs, but only one row is shown.  If this property is set to true, then
        ///     Windows spills extra tabs over on to second rows.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TabBaseMultilineDescr))
        ]
        public bool Multiline {
            get {
                return tabControlState[TABCONTROLSTATE_multiline];
            }
            set {
                if (Multiline != value) {
                    tabControlState[TABCONTROLSTATE_multiline] = value;
                    if (Multiline == false && (this.alignment == TabAlignment.Left || this.alignment == TabAlignment.Right))
                        this.alignment = TabAlignment.Top;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Padding"]/*' />
        /// <devdoc>
        ///     The amount of padding around the items in the individual tabs.
        ///     You can specify both horizontal and vertical padding.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.TabBasePaddingDescr))
        ]
        public new Point Padding {
            get {
                return padding;
            }
            set {
                //do some validation checking here, against min & max GridSize
                //
                if ( value.X < 0 || value.Y < 0 )
                    throw new ArgumentOutOfRangeException(nameof(Padding), string.Format(SR.InvalidArgument, "Padding", value.ToString()));

                if (padding != value) {
                    padding = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.RightToLeftLayout"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout {
            get {

                return rightToLeftLayout;
            }

            set {
                if (value != rightToLeftLayout) {
                    rightToLeftLayout = value;
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout)) {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }



        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.RowCount"]/*' />
        /// <devdoc>
        ///     The number of rows currently being displayed in
        ///     the tab strip.  This is most commonly used when the Multline property
        ///     is 'true' and you want to know how many rows the tabs are currently
        ///     taking up.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TabBaseRowCountDescr))
        ]
        public int RowCount {
            get {
                int n;
                n = unchecked( (int) (long)SendMessage(NativeMethods.TCM_GETROWCOUNT, 0, 0));
                return n;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectedIndex"]/*' />
        /// <devdoc>
        ///     The index of the currently selected tab in the strip, if there
        ///     is one.  If the value is -1, there is currently no selection.  If the
        ///     value is 0 or greater, than the value is the index of the currently
        ///     selected tab.
        /// </devdoc>
        [
        Browsable(false),
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(-1),
        SRDescription(nameof(SR.selectedIndexDescr))
        ]
        public int SelectedIndex {
            get {
                if (IsHandleCreated) {
                    int n;
                    n = unchecked( (int) (long)SendMessage(NativeMethods.TCM_GETCURSEL, 0, 0));
                    return n;
                }
                else {
                    return selectedIndex;
                }
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException(nameof(SelectedIndex), string.Format(SR.InvalidLowBoundArgumentEx, "SelectedIndex", value.ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
                }

                if (SelectedIndex != value) {
                    if (IsHandleCreated) {
                        // Guard Against CreateHandle ..
                        // And also if we are setting SelectedIndex ourselves from SelectNextTab..
                        if (!tabControlState[TABCONTROLSTATE_fromCreateHandles] && !tabControlState[TABCONTROLSTATE_selectFirstControl]) {
                            tabControlState[TABCONTROLSTATE_UISelection] = true;
                            // Fire Deselecting .. Deselected on currently selected TabPage...
                            if (WmSelChanging()) {
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                return;
                            }
                            if(ValidationCancelled) {
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                return;
                            }
                        }

                        SendMessage(NativeMethods.TCM_SETCURSEL, value, 0);

                        if (!tabControlState[TABCONTROLSTATE_fromCreateHandles] && !tabControlState[TABCONTROLSTATE_selectFirstControl]) {
                            // Fire Selecting & Selected .. Also if Selecting is Canceled..
                            // then retuern as we do not change the SelectedIndex...
                            tabControlState[TABCONTROLSTATE_selectFirstControl] = true;
                            if (WmSelChange ()) {
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                tabControlState[TABCONTROLSTATE_selectFirstControl] = false;
                                return;
                            }
                            else
                            {
                                tabControlState[TABCONTROLSTATE_selectFirstControl] = false;
                            }
                        }
                    }
                    else {
                        selectedIndex = value;
                    }
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectedTab"]/*' />
        /// <devdoc>
        ///      The selection to the given tab, provided it .equals a tab in the
        ///      list.  The return value is the index of the tab that was selected,
        ///      or -1 if no tab was selected.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TabControlSelectedTabDescr))
        ]
        public TabPage SelectedTab {
            get {
                return SelectedTabInternal;
            }
            set {
                SelectedTabInternal = value;
            }
        }

        internal TabPage SelectedTabInternal {
            get {
                int index = SelectedIndex;
                if (index == -1) {
                    return null;
                }
                else {
                    Debug.Assert(0 <= index && index < tabPages.Length, "SelectedIndex returned an invalid index");
                    return tabPages[index];
                }
            }
            set {
                int index = FindTabPage(value);
                SelectedIndex = index;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SizeMode"]/*' />
        /// <devdoc>
        ///     By default, tabs are big enough to display their text, and any space
        ///     on the right of the strip is left as such.  However, you can also
        ///     set it such that the tabs are stretched to fill out the right extent
        ///     of the strip, if necessary, or you can set it such that all tabs
        ///     the same width.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(TabSizeMode.Normal),
        SRDescription(nameof(SR.TabBaseSizeModeDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public TabSizeMode SizeMode {
            get {
                return sizeMode;
            }
            set {
                if (sizeMode == value) return;

                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabSizeMode.Normal, (int)TabSizeMode.Fixed)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabSizeMode));
                }

                sizeMode = value;
                RecreateHandle();
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ShowToolTips"]/*' />
        /// <devdoc>
        ///     Indicates whether tooltips are being shown for tabs that have tooltips set on
        ///     them.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.TabBaseShowToolTipsDescr))
        ]
        public bool ShowToolTips {
            get {
                return tabControlState[TABCONTROLSTATE_showToolTips];
            }
            set {
                if (ShowToolTips != value) {
                    tabControlState[TABCONTROLSTATE_showToolTips] = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabCount"]/*' />
        /// <devdoc>
        ///     Returns the number of tabs in the strip
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TabBaseTabCountDescr))
        ]
        public int TabCount {
            get { return tabPageCount;}
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPages"]/*' />
        /// <devdoc>
        ///     Returns the Collection of TabPages.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TabControlTabsDescr)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Editor("System.Windows.Forms.Design.TabPageCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        MergableProperty(false)
        ]
        public TabPageCollection TabPages {
            get {
                return tabCollection;
            }
        }
        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Text"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TextChanged"]/*' />
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

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DrawItem"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem {
            add {
                onDrawItem += value;
            }
            remove {
                onDrawItem -= value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftLayoutChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectedIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged {
            add {
                onSelectedIndexChanged += value;
            }
            remove {
                onSelectedIndexChanged -= value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Selecting"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs before a tabpage is selected as the top tabPage.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.TabControlSelectingEventDescr))
        ]
        public event TabControlCancelEventHandler Selecting {
            add {
                Events.AddHandler(EVENT_SELECTING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTING, value);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Selected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs after a tabpage is selected as the top tabPage.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.TabControlSelectedEventDescr))
        ]
        public event TabControlEventHandler Selected {
            add {
                Events.AddHandler(EVENT_SELECTED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTED, value);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Deselecting"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs before the visible property of the top tabpage is set to false.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.TabControlDeselectingEventDescr))
        ]
        public event TabControlCancelEventHandler Deselecting {
            add {
                Events.AddHandler(EVENT_DESELECTING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DESELECTING, value);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Deselected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs after the visible property of the top tabpage is set to false.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.TabControlDeselectedEventDescr))
        ]
        public event TabControlEventHandler Deselected {
            add {
                Events.AddHandler(EVENT_DESELECTED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DESELECTED, value);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnPaint"]/*' />
        /// <devdoc>
        ///     TabControl Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.AddItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal int AddTabPage(TabPage tabPage, NativeMethods.TCITEM_T tcitem) {
            int index = AddNativeTabPage(tcitem);
            if (index >= 0) {
                Insert(index, tabPage);

            }
            return index;
        }

        internal int AddNativeTabPage(NativeMethods.TCITEM_T tcitem) {
            int index = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TCM_INSERTITEM, tabPageCount + 1, tcitem);
            UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), tabBaseReLayoutMessage, IntPtr.Zero, IntPtr.Zero);
            return index;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ApplySize"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void ApplyItemSize() {
            if (IsHandleCreated && ShouldSerializeItemSize()) {
                SendMessage(NativeMethods.TCM_SETITEMSIZE, 0, (int)NativeMethods.Util.MAKELPARAM(itemSize.Width, itemSize.Height));
            }
            cachedDisplayRect = Rectangle.Empty;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.BeginUpdate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void BeginUpdate() {
            BeginUpdateInternal();
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.CreateControlsInstance"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override Control.ControlCollection CreateControlsInstance() {
            return new ControlCollection(this);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.CreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void CreateHandle() {
            if (!RecreatingHandle) {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                try {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);
                }
                finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        private void DetachImageList(object sender, EventArgs e) {
            ImageList = null;
        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DeselectTab"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be hidden.
        /// </devdoc>
        public void DeselectTab(int index) {
            TabPage t = GetTabPage(index);
            if (SelectedTab == t) {
                if (0 <= index && index < TabPages.Count -1) {
                    SelectedTab =  GetTabPage(++index);
                }
                else {
                    SelectedTab = GetTabPage(0);
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DeselectTab1"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the tabpage in Tabcontrol.TabPageCollection  to be hidden.
        /// </devdoc>
        public void DeselectTab(TabPage tabPage) {
            if (tabPage == null) {
                throw new ArgumentNullException(nameof(tabPage));

            }
            int index = FindTabPage(tabPage);
            DeselectTab(index);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.DeselectTab2"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be hidden.
        /// </devdoc>
        public void DeselectTab(string tabPageName) {
            if (tabPageName == null) {
                throw new ArgumentNullException(nameof(tabPageName));

            }
            TabPage tabPage = TabPages[tabPageName];
            DeselectTab(tabPage);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (imageList != null) {
                    imageList.Disposed -= new EventHandler(this.DetachImageList);
                }
            }
            base.Dispose(disposing);
        }

        internal void EndUpdate() {
            EndUpdate(true);
        }

        internal void EndUpdate(bool invalidate) {
            EndUpdateInternal(invalidate);
        }

        internal int FindTabPage(TabPage tabPage) {
            if (tabPages != null) {
                for (int i = 0; i < tabPageCount; i++) {
                    if (tabPages[i].Equals(tabPage)) {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.GetControl"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        public Control GetControl(int index) {
            return(Control) GetTabPage(index);
        }

        internal TabPage GetTabPage(int index) {

            if (index < 0 || index >= tabPageCount) {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            }
            return tabPages[index];
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.GetItems"]/*' />
        /// <devdoc>
        ///     This has package scope so that TabStrip and TabControl can call it.
        /// </devdoc>
        /// <internalonly/>
        protected virtual object[] GetItems() {
            TabPage[] result = new TabPage[tabPageCount];
            if (tabPageCount > 0) Array.Copy(tabPages, 0, result, 0, tabPageCount);
            return result;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.GetItems1"]/*' />
        /// <devdoc>
        ///     This has package scope so that TabStrip and TabControl can call it.
        /// </devdoc>
        /// <internalonly/>
        protected virtual object[] GetItems(Type baseType) {
            object[] result = (object[]) Array.CreateInstance(baseType, tabPageCount);
            if (tabPageCount > 0) Array.Copy(tabPages, 0, result, 0, tabPageCount);
            return result;
        }

        internal TabPage[] GetTabPages() {
            return (TabPage[])GetItems();
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.GetTabRect"]/*' />
        /// <devdoc>
        ///     Retrieves the bounding rectangle for the given tab in the tab strip.
        /// </devdoc>
        public Rectangle GetTabRect(int index) {
            if (index < 0 || (index >= tabPageCount && !tabControlState[TABCONTROLSTATE_getTabRectfromItemSize])) {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            }
            tabControlState[TABCONTROLSTATE_getTabRectfromItemSize] = false ;
            NativeMethods.RECT rect = new NativeMethods.RECT();

            // normally, we would not want to create the handle for this, but since
            // it is dependent on the actual physical display, we simply must.
            if (!IsHandleCreated)
                CreateHandle();

            SendMessage(NativeMethods.TCM_GETITEMRECT, index, ref rect);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.GetToolTipText"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected string GetToolTipText(object item) {
            return((TabPage)item).ToolTipText;
        }

        private void ImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated)
                SendMessage(NativeMethods.TCM_SETIMAGELIST, 0, ImageList.Handle);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.Insert"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void Insert(int index, TabPage tabPage) {
            if (tabPages == null) {
                tabPages = new TabPage[4];
            }
            else if (tabPages.Length == tabPageCount) {
                TabPage[] newTabPages = new TabPage[tabPageCount * 2];
                Array.Copy(tabPages, 0, newTabPages, 0, tabPageCount);
                tabPages = newTabPages;
            }
            if (index < tabPageCount) {
                Array.Copy(tabPages, index, tabPages, index + 1, tabPageCount - index);
            }
            tabPages[index] = tabPage;
            tabPageCount++;
            cachedDisplayRect = Rectangle.Empty;
            ApplyItemSize();
            if (Appearance == TabAppearance.FlatButtons) {
                Invalidate();
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.InsertItem"]/*' />
        /// <devdoc>
        ///     This function is used by the Insert Logic to insert a tabPage in the current TabPage in the TabPageCollection.
        /// </devdoc>
        /// <internalonly/>
        private void InsertItem(int index, TabPage tabPage) {

            if (index < 0 || ((tabPages != null) && index > tabPageCount))
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));


            int retIndex;
            if (IsHandleCreated) {
                NativeMethods.TCITEM_T tcitem = tabPage.GetTCITEM();
                retIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TCM_INSERTITEM, index, tcitem);
                if (retIndex >= 0) Insert(retIndex, tabPage);
            }

        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.IsInputKey"]/*' />
        /// <devdoc>
        ///      Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if ((keyData & Keys.Alt) == Keys.Alt) return false;
            switch (keyData & Keys.KeyCode) {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     This is a notification that the handle has been created.
        ///     We do some work here to configure the handle.
        ///     Overriders should call base.OnHandleCreated()
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {

            //Add the handle to hashtable for Ids ..
            NativeWindow.AddWindowToIDTable(this, this.Handle);
            handleInTable = true;

            // Set the padding BEFORE setting the control's font (as done
            // in base.OnHandleCreated()) so that the tab control will honor both the
            // horizontal and vertical dimensions of the padding rectangle.
            if (!padding.IsEmpty) {
                SendMessage(NativeMethods.TCM_SETPADDING, 0, NativeMethods.Util.MAKELPARAM(padding.X, padding.Y));
            }

            base.OnHandleCreated(e);
            cachedDisplayRect = Rectangle.Empty;
            ApplyItemSize();
            if (imageList != null) {
                SendMessage(NativeMethods.TCM_SETIMAGELIST, 0, imageList.Handle);
            }

            if (ShowToolTips) {
                IntPtr tooltipHwnd;
                tooltipHwnd = SendMessage(NativeMethods.TCM_GETTOOLTIPS, 0, 0);
                if (tooltipHwnd != IntPtr.Zero) {
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, tooltipHwnd),
                                         NativeMethods.HWND_TOPMOST,
                                         0, 0, 0, 0,
                                         NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                         NativeMethods.SWP_NOACTIVATE);
                }
            }

            // Add the pages
            //
            foreach(TabPage page in TabPages) {
                AddNativeTabPage(page.GetTCITEM());
            }

            // Resize the pages
            //
            ResizePages();

            if (selectedIndex != -1) {
                try {
                    tabControlState[TABCONTROLSTATE_fromCreateHandles] = true;
                    SelectedIndex = selectedIndex;
                }
                finally {
                    tabControlState[TABCONTROLSTATE_fromCreateHandles] = false;
                }
                selectedIndex = -1;
            }
            UpdateTabSelection(false);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnHandleDestroyed"]/*' />
        protected override void OnHandleDestroyed(EventArgs e) {
            if (!Disposing) {
                selectedIndex = SelectedIndex;
            }
            //Remove the Handle from NativewIndow....

            // Don't try to remove the Handle if we've already done so
            if (handleInTable)
            {
                handleInTable = false;
                NativeWindow.RemoveWindowFromIDTable(this.Handle);
            }

            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnDrawItem"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the OnDrawItem event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.onDrawItem(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected virtual void OnDrawItem(DrawItemEventArgs e) {
            if (onDrawItem != null) onDrawItem(this, e);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnEnter"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the OnLeave event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnLeave(e); to ensure the event is
        ///     still fired to external listeners
        ///     This listener is overidden so that we can fire SAME ENTER and LEAVE
        ///     events on the TabPage.
        ///     TabPage should fire enter when the focus is on the TABPAGE and not when the control
        ///     within the TabPage gets Focused.
        ///     Similary the Leave event should fire when the TabControl (and hence the TabPage) looses
        ///     Focus. To be Backward compatible we have added new bool which can be set to true
        ///     to the get the NEW SANE ENTER-LEAVE EVENTS ON THE TABPAGE.
        /// </devdoc>
        protected override void OnEnter(EventArgs e) {
            base.OnEnter (e);
            if (SelectedTab != null) {
                SelectedTab.FireEnter(e);
            }

        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnLeave"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the OnLeave event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnLeave(e); to ensure the event is
        ///     still fired to external listeners
        ///     This listener is overidden so that we can fire SAME ENTER and LEAVE
        ///     events on the TabPage.
        ///     TabPage should fire enter when the focus is on the TABPAGE and not when the control
        ///     within the TabPage gets Focused.
        ///     Similary the Leave event  should fire when the TabControl (and hence the TabPage) looses
        ///     Focus. To be Backward compatible we have added new bool which can be set to true
        ///     to the get the NEW SANE ENTER-LEAVE EVENTS ON THE TABPAGE.
        /// </devdoc>
        protected override void OnLeave(EventArgs e) {
            if (SelectedTab != null) {
                SelectedTab.FireLeave(e);
            }
            base.OnLeave(e);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnKeyDown"]/*' />
        /// <devdoc>
        ///     We override this to get tabbing functionality.
        ///     If overriding this, remember to call base.onKeyDown.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyDown(KeyEventArgs ke) {
            if (ke.KeyCode == Keys.Tab && (ke.KeyData & Keys.Control) !=0) {
                bool forward = (ke.KeyData & Keys.Shift) == 0;
                SelectNextTab(ke, forward);
            }
            if (ke.KeyCode == Keys.PageDown && (ke.KeyData & Keys.Control) !=0) {
                SelectNextTab(ke, true);
            }
            if (ke.KeyCode == Keys.PageUp && (ke.KeyData & Keys.Control) !=0) {
                SelectNextTab(ke, false);
            }

            base.OnKeyDown(ke);
        }

        internal override void OnParentHandleRecreated()
        {
            // Avoid temporarily resizing the TabControl while the parent 
            // recreates its handle to avoid 
            this.skipUpdateSize = true;
            try {
                base.OnParentHandleRecreated();
            }
            finally {
                this.skipUpdateSize = false;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnResize"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(false);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftLayoutChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandle();
            }

            EventHandler eh = Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnSelectedIndexChanged"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the onSelectedIndexChanged event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.onSelectedIndexChanged(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected virtual void OnSelectedIndexChanged(EventArgs e) {
            int index = SelectedIndex;
            cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(tabControlState[TABCONTROLSTATE_UISelection]);
            tabControlState[TABCONTROLSTATE_UISelection] = false;
            if (onSelectedIndexChanged != null) onSelectedIndexChanged(this, e);

        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnSelecting"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.TabControl.OnSelecting'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnSelecting(TabControlCancelEventArgs e) {
            TabControlCancelEventHandler handler = (TabControlCancelEventHandler)Events[EVENT_SELECTING];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnSelected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.TabControl.OnSelected'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnSelected(TabControlEventArgs e) {
            TabControlEventHandler handler = (TabControlEventHandler)Events[EVENT_SELECTED];
            if (handler != null) handler(this, e);

            // Raise the enter event for this tab.
            if (SelectedTab != null) {
                SelectedTab.FireEnter(EventArgs.Empty);
            }

        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnDeselecting"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.TabControl.OnDeselecting'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnDeselecting(TabControlCancelEventArgs e) {
            TabControlCancelEventHandler handler = (TabControlCancelEventHandler)Events[EVENT_DESELECTING];
            if (handler != null) handler(this, e);


        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnDeselected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.TabControl.OnDeselected'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnDeselected(TabControlEventArgs e) {
            TabControlEventHandler handler = (TabControlEventHandler)Events[EVENT_DESELECTED];
            if (handler != null) handler(this, e);

            // Raise the Leave event for this tab.
            if (SelectedTab != null) {
                SelectedTab.FireLeave(EventArgs.Empty);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ProcessKeyPreview"]/*' />
        /// <devdoc>
        ///     We override this to get the Ctrl and Ctrl-Shift Tab functionality.
        /// </devdoc>
        /// <internalonly/>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected override bool ProcessKeyPreview(ref Message m) {
            if (ProcessKeyEventArgs(ref m)) return true;
            return base.ProcessKeyPreview(ref m);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.UpdateSize"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void UpdateSize() {
            if (this.skipUpdateSize) {
                return;
            }
            // the spin control (left right arrows) won't update without resizing.
            // the most correct thing would be to recreate the handle, but this works
            // and is cheaper.
            //
            BeginUpdate();
            Size size = Size;
            Size = new Size(size.Width + 1, size.Height);
            Size = size;
            EndUpdate();
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            cachedDisplayRect = Rectangle.Empty;
            UpdateSize();
        }

        internal override void RecreateHandleCore() {
            // 
            TabPage[] tabPages = GetTabPages();

            int index = ((tabPages.Length > 0) && (SelectedIndex == -1)) ? 0: SelectedIndex;

            // We don't actually want to remove the windows forms Tabpages - we only
            // want to remove the corresponding TCITEM structs.
            // So, no RemoveAll()
            if (IsHandleCreated) {
                SendMessage(NativeMethods.TCM_DELETEALLITEMS, 0, 0);
            }
            this.tabPages = null;
            tabPageCount = 0;

            base.RecreateHandleCore();

            for (int i = 0; i < tabPages.Length; i++) {
                TabPages.Add(tabPages[i]);
            }
            try {
                tabControlState[TABCONTROLSTATE_fromCreateHandles] = true;
                SelectedIndex = index;
            }
            finally {
                tabControlState[TABCONTROLSTATE_fromCreateHandles] = false;
            }

            // The comctl32 TabControl seems to have some painting glitches. Briefly
            // resizing the control seems to fix these.
            //
            UpdateSize();
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.RemoveAll"]/*' />
        protected void RemoveAll() {
            this.Controls.Clear();

            SendMessage(NativeMethods.TCM_DELETEALLITEMS, 0, 0);
            tabPages = null;
            tabPageCount = 0;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.RemoveItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void RemoveTabPage(int index) {
            if (index < 0 || index >= tabPageCount)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            tabPageCount--;
            if (index < tabPageCount) {
                Array.Copy(tabPages, index + 1, tabPages, index, tabPageCount - index);
            }
            tabPages[tabPageCount] = null;
            if (IsHandleCreated) {
                SendMessage(NativeMethods.TCM_DELETEITEM, index, 0);
            }
            cachedDisplayRect = Rectangle.Empty;
        }

        private void ResetItemSize() {
            ItemSize = DEFAULT_ITEMSIZE;
        }

        private void ResetPadding() {
            Padding = DEFAULT_PADDING;

        }

        private void ResizePages() {
            Rectangle rect = DisplayRectangle;
            TabPage[] pages = GetTabPages();
            for (int i = 0; i < pages.Length; i++) {
                pages[i].Bounds = rect;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SetToolTip"]/*' />
        /// <devdoc>
        ///     Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </devdoc>
        /// <internalonly/>
        internal void SetToolTip(ToolTip toolTip, string controlToolTipText) {
            UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TCM_SETTOOLTIPS, new HandleRef(toolTip, toolTip.Handle), 0);
            controlTipText = controlToolTipText;

        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SetItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void SetTabPage(int index, TabPage tabPage, NativeMethods.TCITEM_T tcitem) {
            if (index < 0 || index >= tabPageCount)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));
            if (IsHandleCreated)
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TCM_SETITEM, index, tcitem);
            // Make the Updated tab page the currently selected tab page
            if (DesignMode && IsHandleCreated) {
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TCM_SETCURSEL, (IntPtr)index, IntPtr.Zero);
            }
            tabPages[index] = tabPage;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectTab"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be shown.
        /// </devdoc>
        public void SelectTab(int index) {
            TabPage t = GetTabPage(index);
            if(t != null)
            {
                SelectedTab = t;
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectTab1"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the tabpage in Tabcontrol.TabPageCollection  to be shown.
        /// </devdoc>
        public void SelectTab(TabPage tabPage) {
            if (tabPage == null) {
                throw new ArgumentNullException(nameof(tabPage));

            }
            int index = FindTabPage(tabPage);
            SelectTab(index);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectTab2"]/*' />
        /// <devdoc>
        ///     Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be shown.
        /// </devdoc>
        public void SelectTab(string tabPageName) {
            if (tabPageName == null) {
                throw new ArgumentNullException(nameof(tabPageName));

            }
            TabPage tabPage = TabPages[tabPageName];
            SelectTab(tabPage);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.SelectNextTab"]/*' />
        /// <devdoc>
        ///     This is called by TabControl in response to the KeyDown event to override the selection of tabpages
        ///     for different key combinations.
        ///     Control + Tab selects the next tabpage.
        ///     Control + Shift + Tab selects the previous tabpage.
        ///     Control + PageDown selects the next tabpage.
        ///     Control + PageUp selects the previous tabpage.
        /// </devdoc>
        /// <internalonly/>
        private void SelectNextTab(KeyEventArgs ke, bool forward) {
            // WmSelChanging actually changes focus to cause validations.
            // So cache in the Focused value so that we can reuse it later
            bool focused = Focused;
            
            // Fire Deselecting .. Deselected on currently selected TabPage...
            if (WmSelChanging()) {
                tabControlState[TABCONTROLSTATE_UISelection] = false;
                return;
            }
            if(ValidationCancelled) {
                tabControlState[TABCONTROLSTATE_UISelection] = false;
                return;
            }
            else {

                int sel = SelectedIndex;
                if (sel != -1) {
                    int count = TabCount;
                    if (forward)
                        sel = (sel + 1) % count;
                    else
                        sel = (sel + count - 1) % count;
                    

                    // this is special casing..
                    // this function is called from OnKeyDown( ) which selects the NEXT TABPAGE
                    // But now we call the WmSelChanging( ) to Focus the tab page
                    // This breaks the logic in UpdateTabSelection (which is called
                    // thru SET of SelectedIndex) to Select the First control
                    // So adding this new Flag to select the first control.
                    try
                    {
                        tabControlState[TABCONTROLSTATE_UISelection] = true;
                        tabControlState[TABCONTROLSTATE_selectFirstControl] = true;
                        SelectedIndex = sel;
                        // This is required so that we select the first control if the TabControl is not current focused.
                        tabControlState[TABCONTROLSTATE_selectFirstControl] = !focused;
                        // Fire Selecting .. Selected on newly selected TabPage...
                        WmSelChange();
                        
                    }
                    finally
                    {
                        // tabControlState[TABCONTROLSTATE_selectFirstControl] can be true if the TabControl is not focussed
                        // But at the end of this function reset the state !!
                        tabControlState[TABCONTROLSTATE_selectFirstControl] = false;
                        ke.Handled = true;
                    }

                }
            }
        }

        // TabControl overrides this method to return true.
        internal override bool ShouldPerformContainerValidation() {
            return true;
        }

        private bool ShouldSerializeItemSize() {
            return !itemSize.Equals(DEFAULT_ITEMSIZE);
        }

        private new bool ShouldSerializePadding() {
            return !padding.Equals(DEFAULT_PADDING);
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            if (TabPages != null) {
                s += ", TabPages.Count: " + TabPages.Count.ToString(CultureInfo.CurrentCulture);
                if (TabPages.Count > 0)
                    s += ", TabPages[0]: " + TabPages[0].ToString();
            }
            return s;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy) {
            currentlyScaling = true;
            base.ScaleCore(dx, dy);
            currentlyScaling = false;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.UpdateTabSelection"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        /// <devdoc>
        ///     Set the panel selections appropriately
        /// </devdoc>
        /// <internalonly/>
        protected void UpdateTabSelection(bool updateFocus) {
            if (IsHandleCreated) {
                int index = SelectedIndex;

                // make current panel invisible
                TabPage[] tabPages = GetTabPages();
                if (index != -1) {
                    // Changing the bounds of the tabPage during scaling
                    // will force a layout to occur.  After this layout
                    // the tabpage will then be scaled again resulting
                    // in incorrect sizes.  Suspend Layout in this case.
                    if (currentlyScaling) {
                        tabPages[index].SuspendLayout();
                    }
                    tabPages[index].Bounds = DisplayRectangle;

                    // After changing the Bounds of TabPages, we need to
                    // make TabPages Redraw.
                    // Use Invalidate directly here has no performance
                    // issue, since ReSize is calling low frequence.
                    tabPages[index].Invalidate();

                    if (currentlyScaling) {
                        tabPages[index].ResumeLayout(false);
                    }

                    tabPages[index].Visible = true;
                    if (updateFocus) {
                        if (!Focused || tabControlState[TABCONTROLSTATE_selectFirstControl]) {
                            tabControlState[TABCONTROLSTATE_UISelection] = false;
                            bool selectNext = false;
                            // 


                            IntSecurity.ModifyFocus.Assert();
                            try {
                                selectNext = tabPages[index].SelectNextControl(null, true, true, false, false);
                            }
                            finally {
                                CodeAccessPermission.RevertAssert();
                            }

                            if (selectNext) {
                                if (!ContainsFocus) {
                                    IContainerControl c = GetContainerControlInternal();
                                    if (c != null) {
                                        while (c.ActiveControl is ContainerControl) {
                                            c = (IContainerControl) c.ActiveControl;
                                        }
                                        if (c.ActiveControl != null)
                                        {
                                            c.ActiveControl.FocusInternal();
                                        }
                                    }
                                }
                            }
                            else {
                                IContainerControl c = GetContainerControlInternal();
                                if (c != null && !DesignMode) {
                                    if (c is ContainerControl) {
                                        ((ContainerControl)c).SetActiveControlInternal(this);
                                    }
                                    else {
                                        // 


                                        IntSecurity.ModifyFocus.Assert();
                                        try {
                                            c.ActiveControl = this;
                                        }
                                        finally {
                                            CodeAccessPermission.RevertAssert();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < tabPages.Length; i++) {
                    if (i != SelectedIndex) {
                        tabPages[i].Visible = false;
                    }
                }
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.OnStyleChanged"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnStyleChanged(EventArgs e) {
            base.OnStyleChanged(e);
            cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(false);
        }



        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.UpdateTab"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void UpdateTab(TabPage tabPage) {
            int index = FindTabPage(tabPage);
            SetTabPage(index, tabPage, tabPage.GetTCITEM());

            // It's possible that changes to this TabPage will change the DisplayRectangle of the
            // TabControl, so invalidate and resize the size of this page.
            //
            cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(false);
        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WmNeedText"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmNeedText(ref Message m) {
           
            NativeMethods.TOOLTIPTEXT ttt = (NativeMethods.TOOLTIPTEXT) m.GetLParam(typeof(NativeMethods.TOOLTIPTEXT));

            int commandID = (int)ttt.hdr.idFrom;

            string tipText = GetToolTipText(GetTabPage(commandID));
            if (!string.IsNullOrEmpty(tipText))
                ttt.lpszText = tipText;
            else
                ttt.lpszText = controlTipText;

            ttt.hinst = IntPtr.Zero;

            // RightToLeft reading order
            //
            if (RightToLeft == RightToLeft.Yes) {
                ttt.uFlags |= NativeMethods.TTF_RTLREADING;
            }

            Marshal.StructureToPtr(ttt, m.LParam, false);
           
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WmReflectDrawItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectDrawItem(ref Message m) {

            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));
            IntPtr oldPal = SetUpPalette(dis.hDC, false /*force*/, false /*realize*/);
            using (Graphics g = Graphics.FromHdcInternal(dis.hDC)) {
                OnDrawItem(new DrawItemEventArgs(g, Font, Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom), dis.itemID, (DrawItemState)dis.itemState));
            }
            if (oldPal != IntPtr.Zero) {
                SafeNativeMethods.SelectPalette(new HandleRef(null, dis.hDC), new HandleRef(null, oldPal), 0);
            }
            m.Result = (IntPtr)1;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WmSelChange"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private bool WmSelChange() {
            TabControlCancelEventArgs tcc = new TabControlCancelEventArgs(this.SelectedTab, this.SelectedIndex, false, TabControlAction.Selecting);
            OnSelecting(tcc);
            if (!tcc.Cancel) {
                OnSelected (new TabControlEventArgs (this.SelectedTab, this.SelectedIndex, TabControlAction.Selected));
                OnSelectedIndexChanged(EventArgs.Empty);
            }
            else {
                // user Cancelled the Selection of the new Tab.
                SendMessage(NativeMethods.TCM_SETCURSEL, lastSelection, 0);
                UpdateTabSelection(true);
            }
            return tcc.Cancel;
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WmSelChanging"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private bool WmSelChanging() {
            IContainerControl c = GetContainerControlInternal();
            if (c != null && !DesignMode) {
                if (c is ContainerControl) {
                    ((ContainerControl)c).SetActiveControlInternal(this);
                }
                else {
                    // 


                    IntSecurity.ModifyFocus.Assert();
                    try {
                        c.ActiveControl = this;
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }
            }
            // Fire DeSelecting .... on the current Selected Index...
            // Set the return value to a global
            // if 'cancelled' return from here else..
            // fire Deselected.
            lastSelection = SelectedIndex;
            TabControlCancelEventArgs tcc = new TabControlCancelEventArgs(this.SelectedTab, this.SelectedIndex, false, TabControlAction.Deselecting);
            OnDeselecting(tcc);
            if (!tcc.Cancel) {
                OnDeselected(new TabControlEventArgs(this.SelectedTab, this.SelectedIndex, TabControlAction.Deselected));
            }
            return tcc.Cancel;

        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WmTabBaseReLayout"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmTabBaseReLayout(ref Message m) {
            BeginUpdate();
            cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(false);
            EndUpdate();
            Invalidate(true);

            // Remove other TabBaseReLayout messages from the message queue
            NativeMethods.MSG msg = new NativeMethods.MSG();
            IntPtr hwnd = Handle;
            while (UnsafeNativeMethods.PeekMessage(ref msg, new HandleRef(this, hwnd),
                                       tabBaseReLayoutMessage,
                                       tabBaseReLayoutMessage,
                                       NativeMethods.PM_REMOVE)) {
                ; // NULL loop
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.WndProc"]/*' />
        /// <devdoc>
        ///     The tab's window procedure.  Inheritng classes can override this
        ///     to add extra functionality, but should not forget to call
        ///     base.wndProc(m); to ensure the tab continues to function properly.
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {

            switch (m.Msg) {
                case NativeMethods.WM_REFLECT + NativeMethods.WM_DRAWITEM:
                    WmReflectDrawItem(ref m);
                    break;

                case NativeMethods.WM_REFLECT + NativeMethods.WM_MEASUREITEM:
                    // We use TCM_SETITEMSIZE instead
                    break;

                case NativeMethods.WM_NOTIFY:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                    NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR) m.GetLParam(typeof(NativeMethods.NMHDR));
                    switch (nmhdr.code) {
                        // new switch added to prevent the TabControl from changing to next TabPage ...
                        //in case of validation cancelled...
                        //Turn  tabControlState[TABCONTROLSTATE_UISelection] = false and Return So that no WmSelChange() gets fired.
                        //If validation not cancelled then tabControlState[TABCONTROLSTATE_UISelection] is turned ON to set the focus on to the ...
                        //next TabPage..

                    case NativeMethods.TCN_SELCHANGING:
                            if (WmSelChanging()) {
                                m.Result = (IntPtr)1;
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                return;
                            }

                            if(ValidationCancelled) {
                                m.Result = (IntPtr)1;
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                return;
                            }
                            else {
                                tabControlState[TABCONTROLSTATE_UISelection] = true;
                            }
                            break;
                        case NativeMethods.TCN_SELCHANGE:
                            if (WmSelChange ()) {
                                m.Result = (IntPtr)1;
                                tabControlState[TABCONTROLSTATE_UISelection] = false;
                                return;
                            }
                            else {
                                tabControlState[TABCONTROLSTATE_UISelection] = true;
                            }
                            break;
                        case NativeMethods.TTN_GETDISPINFO:
                            // MSDN:
                            // Setting the max width has the added benefit of enabling Multiline
                            // tool tips!
                            //
                            UnsafeNativeMethods.SendMessage(new HandleRef(nmhdr, nmhdr.hwndFrom), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                            WmNeedText(ref m);
                            m.Result = (IntPtr)1;
                            return;
                    }
                    break;
            }
            if (m.Msg == tabBaseReLayoutMessage) {
                WmTabBaseReLayout(ref m);
                return;
            }
            base.WndProc(ref m);
        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection"]/*' />
        public class TabPageCollection : IList {
            private TabControl owner;
            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.TabPageCollection"]/*' />
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public TabPageCollection( TabControl owner ) {
                if (owner == null) {
                    throw new ArgumentNullException(nameof(owner));
                }
                this.owner = owner;
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.this"]/*' />
            public virtual TabPage this[int index] {
                get {
                    return owner.GetTabPage(index);
                }
                set {
                    owner.SetTabPage(index, value, value.GetTCITEM());
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }                
                [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
                [
                    SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // value is the name of the param passed in.
                                                                                                                // So we don't have to localize it.
                ]
                set {
                    if (value is TabPage) {
                        this[index] = (TabPage)value;
                    }
                    else {
                        throw new ArgumentException(nameof(value));
                    }
                }
            }
            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual TabPage  this[string key] {
                get {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key)){
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index)) {
                        return this[index];
                    }
                    else {
                        return null;
                    }

                }
            }


            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Count"]/*' />
            [Browsable(false)]
            public int Count {
                get {
                    return owner.tabPageCount;
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Add"]/*' />
            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public void Add(TabPage value) {

                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                owner.Controls.Add(value);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.Add"]/*' />
            /// <internalonly/>
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // value is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            int IList.Add(object value) {
                if (value is TabPage) {
                    Add((TabPage)value);
                    return IndexOf((TabPage)value);
                }
                else {
                    throw new ArgumentException(nameof(value));
                }
            }

            // <-- NEW ADD OVERLOADS FOR WHIDBEY

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Add1"]/*' />
            public void Add(string text) {
                TabPage page = new TabPage();
                page.Text = text;
                Add(page);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Add2"]/*' />
            public void Add(string key, string text) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Add(page);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Add3"]/*' />
            public void Add(string key, string text, int imageIndex) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                page.ImageIndex = imageIndex;
                Add(page);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Add4"]/*' />
            public void Add(string key, string text, string imageKey) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                page.ImageKey = imageKey;
                Add(page);
            }

            // END - NEW ADD OVERLOADS FOR WHIDBEY -->

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.AddRange"]/*' />
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public void AddRange(TabPage[] pages) {
                if (pages == null) {
                    throw new ArgumentNullException(nameof(pages));
                }
                foreach(TabPage page in pages) {
                    Add(page);
                }
             }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Contains"]/*' />
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public bool Contains(TabPage page) {

                //check for the page not to be null
                if (page == null)
                    throw new ArgumentNullException(nameof(page));
                //end check

                return IndexOf(page) != -1;
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object page) {
                if (page is TabPage) {
                    return Contains((TabPage)page);
                }
                else {
                    return false;
                }
            }

           /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.ContainsKey"]/*' />
           /// <devdoc>
           ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
           /// </devdoc>
           public virtual bool ContainsKey(string key) {
                return IsValidIndex(IndexOfKey(key));
           }


            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.IndexOf"]/*' />
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public int IndexOf(TabPage page) {

                //check for the page not to be null
                if (page == null)
                    throw new ArgumentNullException(nameof(page));
                //end check

                for(int index=0; index < Count; ++index) {
                    if (this[index] == page) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object page) {
                if (page is TabPage) {
                    return IndexOf((TabPage)page);
                }
                else {
                    return -1;
                }
            }
            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.this"]/*' />
            /// <devdoc>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
            /// </devdoc>
            public virtual int  IndexOfKey(String key) {
                  // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key)){
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true)) {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < this.Count; i ++) {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true)) {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
           }


            /// <include file='doc\TabControl.uex' path='docs/doc[@for="ToolBar.TabPageCollection.Insert"]/*' />
            /// <devdoc>
            ///    <para>Inserts the supplied Tabpage at the given index.</para>
            /// </devdoc>
            public void Insert(int index, TabPage tabPage) {
                owner.InsertItem(index, tabPage);
                try {
                    // See InsertingItem property
                    owner.InsertingItem = true;
                    owner.Controls.Add(tabPage);
                }
                finally {
                    owner.InsertingItem = false;
                }
                owner.Controls.SetChildIndex(tabPage, index);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.Insert"]/*' />
            /// <internalonly/>            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // tabPage is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            void IList.Insert(int index, object tabPage) {
                if (tabPage is TabPage) {
                    Insert(index, (TabPage)tabPage);
                }
                else {
                    throw new ArgumentException(nameof(tabPage));
                }
            }

            // <-- NEW INSERT OVERLOADS FOR WHIDBEY

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Insert1"]/*' />
            public void Insert(int index, string text) {
                TabPage page = new TabPage();
                page.Text = text;
                Insert(index, page);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Insert2"]/*' />
            public void Insert(int index, string key, string text) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Insert(index, page);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Insert3"]/*' />
            public void Insert(int index, string key, string text, int imageIndex) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Insert(index,page);
        		// ImageKey and ImageIndex require parenting...
        		page.ImageIndex = imageIndex;
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Insert4"]/*' />
            public void Insert(int index, string key, string text, string imageKey) {
                TabPage page = new TabPage();
                page.Name = key;
                page.Text = text;
                Insert(index, page);
        		// ImageKey and ImageIndex require parenting...
        		page.ImageKey = imageKey;
            }

            // END - NEW INSERT OVERLOADS FOR WHIDBEY -->


           /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.IsValidIndex"]/*' />
           /// <devdoc>
           ///     <para>Determines if the index is valid for the collection.</para>
           /// </devdoc>
           /// <internalonly/>
           private bool IsValidIndex(int index) {
              return ((index >= 0) && (index < this.Count));
           }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Clear"]/*' />
            public virtual void Clear() {
                owner.RemoveAll();
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                if (Count > 0) {
                    System.Array.Copy(owner.GetTabPages(), 0, dest, index, Count);
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                TabPage[] tabPages = owner.GetTabPages();
                if (tabPages != null) {
                    return tabPages.GetEnumerator();
                }
                else {
                    return new TabPage[0].GetEnumerator();
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.Remove"]/*' />
            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            public void Remove(TabPage value) {

                //check for the value not to be null
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                //end check
                owner.Controls.Remove(value);
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabPageCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                if (value is TabPage) {
                    Remove((TabPage)value);
                }
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.RemoveAt"]/*' />
            public void RemoveAt(int index) {
                owner.Controls.RemoveAt(index);
            }


            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.TabPageCollection.RemoveByKey"]/*' />
            /// <devdoc>
            ///     <para>Removes the child control with the specified key.</para>
            /// </devdoc>
            public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index);
                 }
            }

        }


        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ControlCollection"]/*' />
        /// <devdoc>
        ///     Collection of controls...
        /// </devdoc>
        [ComVisible(false)]
        public new class ControlCollection : Control.ControlCollection {

            private TabControl owner;

            /*C#r: protected*/

            /// <internalonly/>
            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ControlCollection.ControlCollection"]/*' />
            public ControlCollection(TabControl owner)
            : base(owner) {
                this.owner = owner;
            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ControlCollection.Add"]/*' />
            public override void Add(Control value) {
                if (!(value is TabPage)) {
                    throw new ArgumentException(string.Format(SR.TabControlInvalidTabPageType, value.GetType().Name));
                }

                TabPage tabPage = (TabPage)value;

                // See InsertingItem property
                if (!owner.InsertingItem)
                {
                    if (owner.IsHandleCreated) {
                        owner.AddTabPage(tabPage, tabPage.GetTCITEM());
                    }
                    else {
                        owner.Insert(owner.TabCount, tabPage);
                    }
                }

                base.Add(tabPage);
                tabPage.Visible = false;

                // Without this check, we force handle creation on the tabcontrol
                // which is not good at all of there are any OCXs on it.
                //
                if (owner.IsHandleCreated) {
                    tabPage.Bounds = owner.DisplayRectangle;
                }

                // site the tabPage if necessary.
                ISite site = owner.Site;
                if (site != null) {
                    ISite siteTab = tabPage.Site;
                    if (siteTab == null) {
                        IContainer container = site.Container;
                        if (container != null)
                        {
                            container.Add(tabPage);
                        }
                    }
                }
                owner.ApplyItemSize();
                owner.UpdateTabSelection(false);

            }

            /// <include file='doc\TabControl.uex' path='docs/doc[@for="TabControl.ControlCollection.Remove"]/*' />
            public override void Remove(Control value) {
                base.Remove(value);
                if (!(value is TabPage)) {
                    return;
                }
                int index = owner.FindTabPage((TabPage)value);
                int curSelectedIndex = owner.SelectedIndex;

                if (index != -1) {
                    owner.RemoveTabPage(index);
                    if (index == curSelectedIndex)
                    {
                        owner.SelectedIndex = 0; //Always select the first tabPage is the Selected TabPage is removed.
                    }
                }
                owner.UpdateTabSelection(false);
            }

        }


    }
}
