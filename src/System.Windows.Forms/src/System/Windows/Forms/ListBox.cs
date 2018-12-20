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
    using System.Security;
    using System.Security.Permissions;
    using System.Globalization;
    using System.Windows.Forms.Layout;

    using System.Drawing.Design;
    using System.ComponentModel;
    using System.Windows.Forms.ComponentModel;
    using System.Windows.Forms.VisualStyles;

    using System.Collections;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Text;

    /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox"]/*' />
    /// <devdoc>
    ///
    ///     This is a control that presents a list of items to the user.  They may be
    ///     navigated using the keyboard, or the scrollbar on the right side of the
    ///     control.  One or more items may be selected as well.
    /// <para>
    ///
    ///     The preferred way to add items is to set them all via an array at once,
    ///     which is definitely the most efficient way.  The following is an example
    ///     of this:
    /// </para>
    /// <code>
    ///     ListBox lb = new ListBox();
    ///     //     set up properties on the listbox here.
    ///     lb.Items.All = new String [] {
    ///     "A",
    ///     "B",
    ///     "C",
    ///     "D"};
    /// </code>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.ListBoxDesigner, " + AssemblyRef.SystemDesign),
    DefaultEvent(nameof(SelectedIndexChanged)),
    DefaultProperty(nameof(Items)),
    DefaultBindingProperty(nameof(SelectedValue)),
    SRDescription(nameof(SR.DescriptionListBox))
    ]
    public class ListBox : ListControl {
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.NoMatches"]/*' />
        /// <devdoc>
        ///     while doing a search, if no matches are found, this is returned
        /// </devdoc>
        public const int NoMatches = NativeMethods.LB_ERR;
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.DefaultItemHeight"]/*' />
        /// <devdoc>
        ///     The default item height for an owner-draw ListBox.
        /// </devdoc>
        public const int DefaultItemHeight = 13; // 13 == listbox's non-ownerdraw item height.  That's with Win2k and
        // the default font; on other platforms and with other fonts, it may be different.

        private static readonly object EVENT_SELECTEDINDEXCHANGED = new object();
        private static readonly object EVENT_DRAWITEM             = new object();
        private static readonly object EVENT_MEASUREITEM          = new object();

        static bool checkedOS = false;
        static bool runningOnWin2K = true;

        SelectedObjectCollection selectedItems;
        SelectedIndexCollection selectedIndices;
        ObjectCollection itemsCollection;

        int itemHeight = DefaultItemHeight;
        int columnWidth;
        int requestedHeight;
        int topIndex;
        int horizontalExtent = 0;
        int maxWidth = -1;
        int updateCount = 0;

        bool sorted = false;
        bool scrollAlwaysVisible = false;
        bool integralHeight = true;
        bool integralHeightAdjust = false;
        bool multiColumn = false;
        bool horizontalScrollbar = false;
        bool useTabStops = true;
        bool useCustomTabOffsets = false;
        bool fontIsChanged = false;
        bool doubleClickFired = false;
        bool selectedValueChangedFired = false;

        DrawMode drawMode = System.Windows.Forms.DrawMode.Normal;
        BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        SelectionMode selectionMode = System.Windows.Forms.SelectionMode.One;
               
        SelectionMode cachedSelectionMode = System.Windows.Forms.SelectionMode.One;
        //We need to know that we are in middle of handleRecreate through Setter of SelectionMode. 
        //In this case we set a bool denoting that we are changing SelectionMode and 
        //in this case we should always use the cachedValue instead of the currently set value. 
        //We need to change this in the count as well as SelectedIndex code where we access the SelectionMode.
        private bool selectionModeChanging = false;

        /// <devdoc>
        ///     This value stores the array of custom tabstops in the listbox. the array should be populated by
        ///     integers in a ascending order.
        /// </devdoc>
        private IntegerCollection customTabOffsets;

        /// <summary>
        /// Default start position of items in the checked list box
        /// </summary>
        private const int defaultListItemStartPos = 1;

        /// <summary>
        /// Borders are 1 pixel height.
        /// </summary>
        private const int defaultListItemBorderHeight = 1;

        /// <summary>
        /// Borders are 1 pixel width and a pixel buffer 
        /// </summary>
        private const int defaultListItemPaddingBuffer = 3;


        internal int scaledListItemStartPosition = defaultListItemStartPos;
        internal int scaledListItemBordersHeight = 2 * defaultListItemBorderHeight;
        internal int scaledListItemPaddingBuffer = defaultListItemPaddingBuffer;


        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ListBox"]/*' />
        /// <devdoc>
        ///     Creates a basic win32 list box with default values for everything.
        /// </devdoc>
        public ListBox() : base() {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.StandardClick | 
                     ControlStyles.UseTextForAccessibility, false);

            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  

            SetBounds(0, 0, 120, 96);

            requestedHeight = Height;

            PrepareForDrawing();
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            PrepareForDrawing();
        }

        private void PrepareForDrawing() {
            // Scale paddings
            if (DpiHelper.IsScalingRequirementMet) {
                scaledListItemStartPosition = LogicalToDeviceUnits(defaultListItemStartPos);

                // height inlude 2 borders ( top and bottom). we are using multiplication by 2 instead of scaling doubled value to get an even number 
                // that might helps us in positioning control in the center for list items.
                scaledListItemBordersHeight = 2 * LogicalToDeviceUnits(defaultListItemBorderHeight);
                scaledListItemPaddingBuffer = LogicalToDeviceUnits(defaultListItemPaddingBuffer);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BackColor"]/*' />
        public override Color BackColor {
            get {
                if (ShouldSerializeBackColor()) {
                    return base.BackColor;
                }
                else {
                    return SystemColors.Window;
                }
            }
            set {
                base.BackColor = value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BackgroundImageChanged"]/*' />
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

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BorderStyle"]/*' />
        /// <devdoc>
        ///     Retrieves the current border style.  Values for this are taken from
        ///     The System.Windows.Forms.BorderStyle enumeration.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.ListBoxBorderDescr))
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

                if (value != borderStyle) {
                    borderStyle = value;
                    RecreateHandle();
                    // Avoid the listbox and textbox behavior in Collection editors
                    //
                    integralHeightAdjust = true;
                    try {
                        Height = requestedHeight;
                    }
                    finally {
                        integralHeightAdjust = false;
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ColumnWidth"]/*' />
        /// <devdoc>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(0),
        SRDescription(nameof(SR.ListBoxColumnWidthDescr))
        ]
        public int ColumnWidth {
            get {
                return columnWidth;
            }

            set {
                if (value < 0) {
                    throw new ArgumentException(string.Format(SR.InvalidLowBoundArgumentEx, "value",
                                                             (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                if (columnWidth != value) {
                    columnWidth = value;
                    // if it's zero, we need to reset, and only way to do
                    // that is to recreate the handle.
                    if (columnWidth == 0) {
                        RecreateHandle();
                    }
                    else if (IsHandleCreated) {
                        SendMessage(NativeMethods.LB_SETCOLUMNWIDTH, columnWidth, 0);
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.CreateParams"]/*' />
        /// <devdoc>
        ///     Retrieves the parameters needed to create the handle.  Inheriting classes
        ///     can override this to provide extra functionality.  They should not,
        ///     however, forget to call base.getCreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "LISTBOX";

                cp.Style |= NativeMethods.WS_VSCROLL | NativeMethods.LBS_NOTIFY | NativeMethods.LBS_HASSTRINGS;
                if (scrollAlwaysVisible) cp.Style |= NativeMethods.LBS_DISABLENOSCROLL;
                if (!integralHeight) cp.Style |= NativeMethods.LBS_NOINTEGRALHEIGHT;
                if (useTabStops) cp.Style |= NativeMethods.LBS_USETABSTOPS;

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }

                if (multiColumn) {
                    cp.Style |= NativeMethods.LBS_MULTICOLUMN | NativeMethods.WS_HSCROLL;
                }
                else if (horizontalScrollbar) {
                    cp.Style |= NativeMethods.WS_HSCROLL;
                }

                switch (selectionMode) {
                    case SelectionMode.None:
                        cp.Style |= NativeMethods.LBS_NOSEL;
                        break;
                    case SelectionMode.MultiSimple:
                        cp.Style |= NativeMethods.LBS_MULTIPLESEL;
                        break;
                    case SelectionMode.MultiExtended:
                        cp.Style |= NativeMethods.LBS_EXTENDEDSEL;
                        break;
                    case SelectionMode.One:
                        break;
                }

                switch (drawMode) {
                    case DrawMode.Normal:
                        break;
                    case DrawMode.OwnerDrawFixed:
                        cp.Style |= NativeMethods.LBS_OWNERDRAWFIXED;
                        break;
                    case DrawMode.OwnerDrawVariable:
                        cp.Style |= NativeMethods.LBS_OWNERDRAWVARIABLE;
                        break;
                }

                return cp;
            }
        }


        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.UseCustomTabOffsets"]/*' />
        /// <devdoc>
        ///     Enables a list box to recognize and expand tab characters when drawing
        ///     its strings using the CustomTabOffsets integer array.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Browsable(false)
        ]
        public bool UseCustomTabOffsets {
            get {
                return useCustomTabOffsets;
            }
            set {
                if (useCustomTabOffsets != value) {
                    useCustomTabOffsets = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return new Size(120, 96);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.DrawMode"]/*' />
        /// <devdoc>
        ///     Retrieves the style of the listbox.  This will indicate if the system
        ///     draws it, or if the user paints each item manually.  It also indicates
        ///     whether or not items have to be of the same height.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DrawMode.Normal),
        SRDescription(nameof(SR.ListBoxDrawModeDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public virtual DrawMode DrawMode {
            get {
                return drawMode;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DrawMode.Normal, (int)DrawMode.OwnerDrawVariable))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DrawMode));
                }
                if (drawMode != value) {
                    if (MultiColumn && value == DrawMode.OwnerDrawVariable) {
                        throw new ArgumentException(SR.ListBoxVarHeightMultiCol, "value");
                    }
                    drawMode = value;
                    RecreateHandle();
                    if (drawMode == DrawMode.OwnerDrawVariable) {
                        // Force a layout after RecreateHandle() completes because now
                        // the LB is definitely fully populated and can report a preferred size accurately.
                        LayoutTransaction.DoLayoutIf(AutoSize, this.ParentInternal, this, PropertyNames.DrawMode);
                    }
                }
            }
        }

        // Used internally to find the currently focused item
        //
        internal int FocusedIndex {
            get {
                if (IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.LB_GETCARETINDEX, 0, 0));
                }

                return -1;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Font"]/*' />
        // The scroll bars don't display properly when the IntegralHeight == false
        // and the control is resized before the font size is change and the new font size causes
        // the height of all the items to exceed the new height of the control. This is a bug in
        // the control, but can be easily worked around by removing and re-adding all the items.

        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;

                if (false == integralHeight) {
                    // Refresh the list to force the scroll bars to display
                    // when the integral height is false.
                    RefreshItems();
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ForeColor"]/*' />
        public override Color ForeColor {
            get {
                if (ShouldSerializeForeColor()) {
                    return base.ForeColor;
                }
                else {
                    return SystemColors.WindowText;
                }
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.HorizontalExtent"]/*' />
        /// <devdoc>
        ///     Indicates the width, in pixels, by which a list box can be scrolled horizontally (the scrollable width).
        ///     This property will only have an effect if HorizontalScrollbars is true.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxHorizontalExtentDescr))
        ]
        public int HorizontalExtent {
            get {
                return horizontalExtent;
            }

            set {
                if (value != horizontalExtent) {
                    horizontalExtent = value;
                    UpdateHorizontalExtent();
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.HorizontalScrollbar"]/*' />
        /// <devdoc>
        ///     Indicates whether or not the ListBox should display a horizontal scrollbar
        ///     when the items extend beyond the right edge of the ListBox.
        ///     If true, the scrollbar will automatically set its extent depending on the length
        ///     of items in the ListBox. The exception is if the ListBox is owner-draw, in
        ///     which case HorizontalExtent will need to be explicitly set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxHorizontalScrollbarDescr))
        ]
        public bool HorizontalScrollbar {
            get {
                return horizontalScrollbar;
            }

            set {
                if (value != horizontalScrollbar) {
                    horizontalScrollbar = value;

                    // There seems to be a bug in the native ListBox in that the addition
                    // of the horizontal scroll bar does not get reflected in the control
                    // rightaway. So, we refresh the items here.

                    RefreshItems();

                    // Only need to recreate the handle if not MultiColumn
                    // (HorizontalScrollbar has no effect on a MultiColumn listbox)
                    //
                    if (!MultiColumn) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegralHeight"]/*' />
        /// <devdoc>
        ///     Indicates if the listbox should avoid showing partial Items.  If so,
        ///     then only full items will be displayed, and the listbox will be resized
        ///     to prevent partial items from being shown.  Otherwise, they will be
        ///     shown
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxIntegralHeightDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public bool IntegralHeight {
            get {
                return integralHeight;
            }

            set {
                if (integralHeight != value) {
                    integralHeight = value;
                    RecreateHandle();
                    // Avoid the listbox and textbox behaviour in Collection editors
                    //

                    integralHeightAdjust = true;
                    try {
                        Height = requestedHeight;
                    }
                    finally {
                        integralHeightAdjust = false;
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ItemHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns
        ///       the height of an item in an owner-draw list box.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DefaultItemHeight),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxItemHeightDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public virtual int ItemHeight {
            get {
                if (drawMode == DrawMode.OwnerDrawFixed ||
                    drawMode == DrawMode.OwnerDrawVariable) {
                    return itemHeight;
                }

                return GetItemHeight(0);
            }

            set {
                if (value < 1 || value > 255) {
                    throw new ArgumentOutOfRangeException(nameof(ItemHeight), string.Format(SR.InvalidExBoundArgument, "ItemHeight", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture), "256"));
                }
                if (itemHeight != value) {
                    itemHeight = value;
                    if (drawMode == DrawMode.OwnerDrawFixed && IsHandleCreated) {
                        BeginUpdate();
                        SendMessage(NativeMethods.LB_SETITEMHEIGHT, 0, value);

                        // Changing the item height might require a resize for IntegralHeight list boxes
                        //
                        if (IntegralHeight) {
                            Size oldSize = Size;
                            Size = new Size(oldSize.Width + 1, oldSize.Height);
                            Size = oldSize;
                        }

                        EndUpdate();
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Items"]/*' />
        /// <devdoc>
        ///     Collection of items in this listbox.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxItemsDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        MergableProperty(false)
        ]
        public ObjectCollection Items {
            get {
                if (itemsCollection == null) {
                    itemsCollection = CreateItemCollection();
                }
                return itemsCollection;
            }
        }

        // Computes the maximum width of all items in the ListBox
        //
        internal virtual int MaxItemWidth {
            get {

                if (horizontalExtent > 0) {
                    return horizontalExtent;
                }

                if (DrawMode != DrawMode.Normal) {
                    return -1;
                }

                // Return cached maxWidth if available
                //
                if (maxWidth > -1) {
                    return maxWidth;
                }

                // Compute maximum width
                //
                maxWidth = ComputeMaxItemWidth(maxWidth);

                return maxWidth;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.MultiColumn"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates if the listbox is multi-column
        ///       or not.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListBoxMultiColumnDescr))
        ]
        public bool MultiColumn {
            get {
                return multiColumn;
            }
            set {
                if (multiColumn != value) {
                    if (value && drawMode == DrawMode.OwnerDrawVariable) {
                        throw new ArgumentException(SR.ListBoxVarHeightMultiCol, "value");
                    }
                    multiColumn = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.PreferredHeight"]/*' />
        /// <devdoc>
        ///     The total height of the items in the list box.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxPreferredHeightDescr))
        ]
        public int PreferredHeight {
            get {
                int height = 0;

                if (drawMode == DrawMode.OwnerDrawVariable) {
                    // don't try to get item heights from the LB when items haven't been
                    // added to the LB yet. Just return current height.
                    if (RecreatingHandle || GetState(STATE_CREATINGHANDLE)) {
                        height = this.Height;
                    }
                    else {
                        if (itemsCollection != null) {
                            int cnt = itemsCollection.Count;
                            for (int i = 0; i < cnt; i++) {
                                height += GetItemHeight(i);
                            }
                        }
                    }
                }
                else {
                    //When the list is empty, we don't want to multiply by 0 here.
                    int cnt = (itemsCollection == null || itemsCollection.Count == 0) ? 1 : itemsCollection.Count;
                    height = GetItemHeight(0) * cnt;
                }

                if (borderStyle != BorderStyle.None) {
                    height += SystemInformation.BorderSize.Height * 4 + 3;
                }

                return height;
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            int height = PreferredHeight;
            int width;

            // Convert with a dummy height to add space required for borders
            // PreferredSize should return either the new
            // size of the control, or the default size if the handle has not been
            // created
            if (IsHandleCreated)
            {
                width = SizeFromClientSize(new Size(MaxItemWidth, height)).Width;
                width += SystemInformation.VerticalScrollBarWidth + 4;
            }
            else
            {
                return DefaultSize;
            }
            return new Size(width, height) + Padding.Size;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.RightToLeft"]/*' />
        public override RightToLeft RightToLeft {
            get {
                if (!RunningOnWin2K) {
                    return RightToLeft.No;
                }
                return base.RightToLeft;
            }
            set {
                base.RightToLeft = value;
            }
        }

        static bool RunningOnWin2K {
            get {
                if (!checkedOS) {
                    if (Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                        Environment.OSVersion.Version.Major < 5) {
                        runningOnWin2K = false;
                        checkedOS = true;
                    }
                }
                return runningOnWin2K;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ScrollAlwaysVisible"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets whether the scrollbar is shown at all times.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxScrollIsVisibleDescr))
        ]
        public bool ScrollAlwaysVisible {
            get {
                return scrollAlwaysVisible;
            }
            set {
                if (scrollAlwaysVisible != value) {
                    scrollAlwaysVisible = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.AllowSelection"]/*' />
        /// <devdoc>
        ///    Indicates whether list currently allows selection of list items.
        ///    For ListBox, this returns true unless SelectionMode is SelectionMode.None.
        /// </devdoc>
        protected override bool AllowSelection {
            get {
                return selectionMode != SelectionMode.None;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndex"]/*' />
        /// <devdoc>
        ///     The index of the currently selected item in the list, if there
        ///     is one.  If the value is -1, there is currently no selection.  If the
        ///     value is 0 or greater, than the value is the index of the currently
        ///     selected item.  If the MultiSelect property on the ListBox is true,
        ///     then a non-zero value for this property is the index of the first
        ///     selection
        /// </devdoc>
        [
        Browsable(false),
        Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxSelectedIndexDescr))
        ]
        public override int SelectedIndex {
            get {

                SelectionMode current = (selectionModeChanging) ? cachedSelectionMode : selectionMode;
                
                if (current == SelectionMode.None) {
                    return -1;
                }

                if (current == SelectionMode.One && IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.LB_GETCURSEL, 0, 0));
                }

                if (itemsCollection != null && SelectedItems.Count > 0) {
                    return Items.IndexOfIdentifier(SelectedItems.GetObjectAt(0));
                }

                return -1;
            }
            set {

                int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;

                if (value < -1 || value >= itemCount) {
                    throw new ArgumentOutOfRangeException(nameof(SelectedIndex), string.Format(SR.InvalidArgument, "SelectedIndex", (value).ToString(CultureInfo.CurrentCulture)));
                }

                if (selectionMode == SelectionMode.None) {
                    throw new ArgumentException(SR.ListBoxInvalidSelectionMode, "SelectedIndex");
                }

                if (selectionMode == SelectionMode.One && value != -1) {

                    // Single select an individual value.
                    int currentIndex = SelectedIndex;

                    if (currentIndex != value) {
                        if (currentIndex != -1) {
                            SelectedItems.SetSelected(currentIndex, false);
                        }
                        SelectedItems.SetSelected(value, true);

                        if (IsHandleCreated) {
                            NativeSetSelected(value, true);
                        }

                        OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
                else if (value == -1) {
                    if (SelectedIndex != -1) {
                        ClearSelected();
                        // ClearSelected raises OnSelectedIndexChanged for us
                    }
                }
                else {
                    if (!SelectedItems.GetSelected(value)) {

                        // Select this item while keeping any previously selected items selected.
                        //
                        SelectedItems.SetSelected(value, true);
                        if (IsHandleCreated) {
                            NativeSetSelected(value, true);
                        }
                        OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndices"]/*' />
        /// <devdoc>
        ///     A collection of the indices of the selected items in the
        ///     list box. If there are no selected items in the list box, the result is
        ///     an empty collection.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxSelectedIndicesDescr))
        ]
        public SelectedIndexCollection SelectedIndices {
            get {
                if (selectedIndices == null) {
                    selectedIndices = new SelectedIndexCollection(this);
                }
                return selectedIndices;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedItem"]/*' />
        /// <devdoc>
        ///     The value of the currently selected item in the list, if there
        ///     is one.  If the value is null, there is currently no selection.  If the
        ///     value is non-null, then the value is that of the currently selected
        ///     item. If the MultiSelect property on the ListBox is true, then a
        ///     non-null return value for this method is the value of the first item
        ///     selected
        /// </devdoc>
        [
        Browsable(false),
        Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxSelectedItemDescr))
        ]
        public object SelectedItem {
            get {
                if (SelectedItems.Count > 0) {
                    return SelectedItems[0];
                }

                return null;
            }
            set {
                if (itemsCollection != null) {
                    if (value != null) {
                        int index = itemsCollection.IndexOf(value);
                        if (index != -1) {
                            SelectedIndex = index;
                        }
                    }
                    else {
                        SelectedIndex = -1;
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedItems"]/*' />
        /// <devdoc>
        ///     The collection of selected items.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxSelectedItemsDescr))
        ]
        public SelectedObjectCollection SelectedItems {
            get {
                if (selectedItems == null) {
                    selectedItems = new SelectedObjectCollection(this);
                }
                return selectedItems;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectionMode"]/*' />
        /// <devdoc>
        ///     Controls how many items at a time can be selected in the listbox. Valid
        ///     values are from the System.Windows.Forms.SelectionMode enumeration.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(SelectionMode.One),
        SRDescription(nameof(SR.ListBoxSelectionModeDescr))
        ]
        public virtual SelectionMode SelectionMode {
            get {
                return selectionMode;
            }
            set {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SelectionMode.None, (int)SelectionMode.MultiExtended))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SelectionMode));
                }

                if (selectionMode != value) {
                    SelectedItems.EnsureUpToDate();
                    selectionMode = value;
                    try
                    {
                        selectionModeChanging = true;
                        RecreateHandle();
                    }
                    finally
                    {
                        selectionModeChanging = false;
                        cachedSelectionMode = selectionMode;
                        // update the selectedItems list and SelectedItems index collection
                        if (IsHandleCreated)
                        {
                            NativeUpdateSelection();
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Sorted"]/*' />
        /// <devdoc>
        ///     Indicates if the ListBox is sorted or not.  'true' means that strings in
        ///     the list will be sorted alphabetically
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListBoxSortedDescr))
        ]
        public bool Sorted {
            get {
                return sorted;
            }
            set {
                if (sorted != value) {
                    sorted = value;

                    if (sorted && itemsCollection != null && itemsCollection.Count >= 1) {
                        Sort();
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Text"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Bindable(false)
        ]
        public override string Text {
            get {
                if (SelectionMode != SelectionMode.None && SelectedItem != null) {
                    if (FormattingEnabled) {
                        return GetItemText(SelectedItem);
                    } else {
                        return FilterItemOnProperty(SelectedItem).ToString();
                    }
                }
                else {
                    return base.Text;
                }
            }
            set {
                base.Text = value;

                // Scan through the list items looking for the supplied text string.  If we find it,
                // select it.
                //
                if (SelectionMode != SelectionMode.None && value != null && (SelectedItem == null || !value.Equals(GetItemText(SelectedItem)))) {

                    int cnt = Items.Count;
                    for (int index=0; index < cnt; ++index) {
                        if (String.Compare(value, GetItemText(Items[index]), true, CultureInfo.CurrentCulture) == 0) {
                            SelectedIndex = index;
                            return;
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.TopIndex"]/*' />
        /// <devdoc>
        ///     The index of the first visible item in a list box. Initially
        ///     the item with index 0 is at the top of the list box, but if the list
        ///     box contents have been scrolled another item may be at the top.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListBoxTopIndexDescr))
        ]
        public int TopIndex {
            get {
                if (IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.LB_GETTOPINDEX, 0, 0));
                }
                else {
                    return topIndex;
                }
            }
            set {
                if (IsHandleCreated) {
                    SendMessage(NativeMethods.LB_SETTOPINDEX, value, 0);
                }
                else {
                    topIndex = value;
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.UseTabStops"]/*' />
        /// <devdoc>
        ///     Enables a list box to recognize and expand tab characters when drawing
        ///     its strings.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListBoxUseTabStopsDescr))
        ]
        public bool UseTabStops {
            get {
                return useTabStops;
            }
            set {
                if (useTabStops != value) {
                    useTabStops = value;
                    RecreateHandle();
                }
            }
        }
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.CustomTabOffsets"]/*' />
        /// <devdoc>
        ///     Allows to set the width of the tabs between the items in the list box.
        ///     The integer array should have the tab spaces in the ascending order.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ListBoxCustomTabOffsetsDescr)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Browsable(false)
        ]
        public IntegerCollection CustomTabOffsets {
            get {
                if (customTabOffsets == null) {
                    customTabOffsets = new IntegerCollection(this);
                }
                return customTabOffsets;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.AddItemsCore"]/*' />
        /// <devdoc>
        ///     Performs the work of adding the specified items to the Listbox
        /// </devdoc>
        [Obsolete("This method has been deprecated.  There is no replacement.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected virtual void AddItemsCore(object[] value) {
            int count = value == null? 0: value.Length;
            if (count == 0) {
                return;
            }

            Items.AddRangeInternal(value);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Click"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler Click {
            add {
                base.Click += value;
            }
            remove {
                base.Click -= value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.MouseClick"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event MouseEventHandler MouseClick {
            add {
                base.MouseClick += value;
            }
            remove {
                base.MouseClick -= value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Padding"]/*' />
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

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnPaint"]/*' />
        /// <devdoc>
        ///     ListBox / CheckedListBox Onpaint.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.DrawItem"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem {
            add {
                Events.AddHandler(EVENT_DRAWITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DRAWITEM, value);
            }
        }


        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.MeasureItem"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.measureItemEventDescr))]
        public event MeasureItemEventHandler MeasureItem {
            add {
                Events.AddHandler(EVENT_MEASUREITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MEASUREITEM, value);
            }
        }


        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged {
            add {
                Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.BeginUpdate"]/*' />
        /// <devdoc>
        ///     While the preferred way to insert items is to set Items.All,
        ///     and set all the items at once, there are times when you may wish to
        ///     insert each item one at a time.  To help with the performance of this,
        ///     it is desirable to prevent the ListBox from painting during these
        ///     operations.  This method, along with EndUpdate, is the preferred
        ///     way of doing this.  Don't forget to call EndUpdate when you're done,
        ///     or else the ListBox won't paint properly afterwards.
        /// </devdoc>
        public void BeginUpdate() {
            BeginUpdateInternal();
            updateCount++;
        }

        private void CheckIndex(int index) {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.IndexOutOfRange, index.ToString(CultureInfo.CurrentCulture)));
        }

        private void CheckNoDataSource() {
            if (DataSource != null)
                throw new ArgumentException(SR.DataSourceLocksItems);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.CreateItemCollection"]/*' />
        protected virtual ObjectCollection CreateItemCollection() {
            return new ObjectCollection(this);
        }

        internal virtual int ComputeMaxItemWidth(int oldMax) {
            // pass LayoutUtils the collection of strings
            string[] strings = new string[this.Items.Count];

            for (int i = 0; i < Items.Count; i ++) {
                strings[i] = GetItemText(Items[i]);
            }

            Size textSize = LayoutUtils.OldGetLargestStringSizeInCollection(Font, strings);
            return Math.Max(oldMax, textSize.Width);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ClearSelected"]/*' />
        /// <devdoc>
        ///     Unselects all currently selected items.
        /// </devdoc>
        public void ClearSelected() {

            bool hadSelection = false;

            int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;
            for (int x = 0; x < itemCount;x++) {
                if (SelectedItems.GetSelected(x)) {
                    hadSelection = true;
                    SelectedItems.SetSelected(x, false);
                    if (IsHandleCreated) {
                        NativeSetSelected(x, false);
                    }
                }
            }

            if (hadSelection) {
                OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.EndUpdate"]/*' />
        /// <devdoc>
        ///     While the preferred way to insert items is to set Items.All,
        ///     and set all the items at once, there are times when you may wish to
        ///     insert each item one at a time.  To help with the performance of this,
        ///     it is desirable to prevent the ListBox from painting during these
        ///     operations.  This method, along with BeginUpdate, is the preferred
        ///     way of doing this.  BeginUpdate should be called first, and this method
        ///     should be called when you want the control to start painting again.
        /// </devdoc>
        public void EndUpdate() {
            EndUpdateInternal();
            --updateCount;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.FindString"]/*' />
        /// <devdoc>
        ///     Finds the first item in the list box that starts with the given string.
        ///     The search is not case sensitive.
        /// </devdoc>
        public int FindString(string s) {
            return FindString(s, -1);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.FindString1"]/*' />
        /// <devdoc>
        ///     Finds the first item after the given index which starts with the given
        ///     string. The search is not case sensitive.
        /// </devdoc>
        public int FindString(string s, int startIndex) {
            if (s == null) return -1;

            int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;

            if (itemCount == 0) {
                return -1;
            }

            // The last item in the list is still a valid starting point for a search.
            if (startIndex < -1 || startIndex >= itemCount) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // Always use the managed FindStringInternal instead of LB_FINDSTRING.
            // The managed version correctly handles Turkish I.
            return FindStringInternal(s, Items, startIndex, false);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.FindStringExact"]/*' />
        /// <devdoc>
        ///     Finds the first item in the list box that matches the given string.
        ///     The strings must match exactly, except for differences in casing.
        /// </devdoc>
        public int FindStringExact(string s) {
            return FindStringExact(s, -1);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.FindStringExact1"]/*' />
        /// <devdoc>
        ///     Finds the first item after the given index that matches the given
        ///     string. The strings must match excatly, except for differences in
        ///     casing.
        /// </devdoc>
        public int FindStringExact(string s, int startIndex) {
            if (s == null) return -1;

            int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;

            if (itemCount == 0) {
                return -1;
            }

            // The last item in the list is still a valid starting point for a search.
            if (startIndex < -1 || startIndex >= itemCount) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // Always use the managed FindStringInternal instead of LB_FINDSTRING.
            // The managed version correctly handles Turkish I.
            //
            return FindStringInternal(s, Items, startIndex, true);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.GetItemHeight"]/*' />
        /// <devdoc>
        ///     Returns the height of the given item in a list box. The index parameter
        ///     is ignored if drawMode is not OwnerDrawVariable.
        /// </devdoc>
        public int GetItemHeight(int index) {
            int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;

            // Note: index == 0 is OK even if the ListBox currently has
            // no items.
            //
            if (index < 0 || (index > 0 && index >= itemCount))
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

            if (drawMode != DrawMode.OwnerDrawVariable) index = 0;

            if (IsHandleCreated) {
                int h = unchecked( (int) (long)SendMessage(NativeMethods.LB_GETITEMHEIGHT, index, 0));
                if (h == -1)
                    throw new Win32Exception();
                return h;
            }

            return itemHeight;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.GetItemRectangle"]/*' />
        /// <devdoc>
        ///     Retrieves a Rectangle object which describes the bounding rectangle
        ///     around an item in the list.  If the item in question is not visible,
        ///     the rectangle will be outside the visible portion of the control.
        /// </devdoc>
        public Rectangle GetItemRectangle(int index) {
            CheckIndex(index);
            NativeMethods.RECT rect = new NativeMethods.RECT();
            SendMessage(NativeMethods.LB_GETITEMRECT, index, ref rect);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <devdoc>
        ///     List box overrides GetScaledBounds to ensure we always scale the requested
        ///     height, not the current height.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {
            // update bounds' height to use the requested height, not the current height.  These
            // can be different if integral height is turned on.
            bounds.Height = requestedHeight;
            return base.GetScaledBounds(bounds, factor, specified);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.GetSelected"]/*' />
        /// <devdoc>
        ///     Tells you whether or not the item at the supplied index is selected
        ///     or not.
        /// </devdoc>
        public bool GetSelected(int index) {
            CheckIndex(index);
            return GetSelectedInternal(index);
        }

        private bool GetSelectedInternal(int index) {
            if (IsHandleCreated) {
                int sel = unchecked( (int) (long)SendMessage(NativeMethods.LB_GETSEL, index, 0));
                if (sel == -1) {
                    throw new Win32Exception();
                }
                return sel > 0;
            }
            else {
                if (itemsCollection != null && SelectedItems.GetSelected(index)) {
                    return true;
                }
                return false;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IndexFromPoint"]/*' />
        /// <devdoc>
        ///     Retrieves the index of the item at the given coordinates.
        /// </devdoc>
        public int IndexFromPoint(Point p) {
            return IndexFromPoint(p.X, p.Y);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IndexFromPoint1"]/*' />
        /// <devdoc>
        ///     Retrieves the index of the item at the given coordinates.
        /// </devdoc>
        public int IndexFromPoint(int x, int y) {
            //NT4 SP6A : SendMessage Fails. So First check whether the point is in Client Co-ordinates and then
            //call Sendmessage.
            //
            NativeMethods.RECT r = new NativeMethods.RECT();
            UnsafeNativeMethods.GetClientRect(new HandleRef(this, Handle), ref r);
            if (r.left <= x && x < r.right && r.top <= y && y < r.bottom) {
                int index = unchecked( (int) (long)SendMessage(NativeMethods.LB_ITEMFROMPOINT, 0, unchecked( (int) (long)NativeMethods.Util.MAKELPARAM(x, y))));
                if (NativeMethods.Util.HIWORD(index) == 0) {
                    // Inside ListBox client area
                    return NativeMethods.Util.LOWORD(index);
                }
            }

            return NoMatches;
        }

        /// <devdoc>
        ///     Adds the given item to the native combo box.  This asserts if the handle hasn't been
        ///     created.
        /// </devdoc>
        private int NativeAdd(object item) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked( (int) (long)SendMessage(NativeMethods.LB_ADDSTRING, 0, GetItemText(item)));

            if (insertIndex == NativeMethods.LB_ERRSPACE) {
                throw new OutOfMemoryException();
            }

            if (insertIndex == NativeMethods.LB_ERR) {
                // On some platforms (e.g. Win98), the ListBox control
                // appears to return LB_ERR if there are a large number (>32000)
                // of items. It doesn't appear to set error codes appropriately,
                // so we'll have to assume that LB_ERR corresponds to item
                // overflow.
                //
                throw new OutOfMemoryException(SR.ListBoxItemOverflow);
            }

            return insertIndex;
        }

        /// <devdoc>
        ///     Clears the contents of the combo box.
        /// </devdoc>
        private void NativeClear() {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            SendMessage(NativeMethods.LB_RESETCONTENT, 0, 0);
        }

        /// <devdoc>
        ///     Get the text stored by the native control for the specified list item.
        /// </devdoc>
        internal string NativeGetItemText(int index) {
            int len = unchecked( (int) (long)SendMessage(NativeMethods.LB_GETTEXTLEN, index, 0));
            StringBuilder sb = new StringBuilder(len + 1);
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LB_GETTEXT, index, sb);
            return sb.ToString();
        }

        /// <devdoc>
        ///     Inserts the given item to the native combo box at the index.  This asserts if the handle hasn't been
        ///     created or if the resulting insert index doesn't match the passed in index.
        /// </devdoc>
        private int NativeInsert(int index, object item) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked( (int) (long)SendMessage(NativeMethods.LB_INSERTSTRING, index, GetItemText(item)));

            if (insertIndex == NativeMethods.LB_ERRSPACE) {
                throw new OutOfMemoryException();
            }

            if (insertIndex == NativeMethods.LB_ERR) {
                // On some platforms (e.g. Win98), the ListBox control
                // appears to return LB_ERR if there are a large number (>32000)
                // of items. It doesn't appear to set error codes appropriately,
                // so we'll have to assume that LB_ERR corresponds to item
                // overflow.
                //
                throw new OutOfMemoryException(SR.ListBoxItemOverflow);
            }

            Debug.Assert(insertIndex == index, "NativeListBox inserted at " + insertIndex + " not the requested index of " + index);
            return insertIndex;
        }

        /// <devdoc>
        ///     Removes the native item from the given index.
        /// </devdoc>
        private void NativeRemoveAt(int index) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");

            bool selected = (unchecked( (int) (long)SendMessage(NativeMethods.LB_GETSEL, (IntPtr)index, IntPtr.Zero)) > 0);
            SendMessage(NativeMethods.LB_DELETESTRING, index, 0);

            //If the item currently selected is removed then we should fire a Selectionchanged event...
            //as the next time selected index returns -1...

            if (selected) {
                OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        /// <devdoc>
        ///     Sets the selection of the given index to the native window.  This does not change
        ///     the collection; you must update the collection yourself.
        /// </devdoc>
        private void NativeSetSelected(int index, bool value) {
            Debug.Assert(IsHandleCreated, "Should only call Native methods after the handle has been created");
            Debug.Assert(selectionMode != SelectionMode.None, "Guard against setting selection for None selection mode outside this code.");

            if (selectionMode == SelectionMode.One) {
                SendMessage(NativeMethods.LB_SETCURSEL, (value ? index : -1), 0);
            }
            else {
                SendMessage(NativeMethods.LB_SETSEL, value? -1: 0, index);
            }
        }

        /// <devdoc>
        ///     This is called by the SelectedObjectCollection in response to the first
        ///     query on that collection after we have called Dirty().  Dirty() is called
        ///     when we receive a LBN_SELCHANGE message.
        /// </devdoc>
        private void NativeUpdateSelection() {
            Debug.Assert(IsHandleCreated, "Should only call native methods if handle is created");

            // Clear the selection state.
            //
            int cnt = Items.Count;
            for (int i = 0; i < cnt; i++) {
                SelectedItems.SetSelected(i, false);
            }

            int[] result = null;

            switch (selectionMode) {

                case SelectionMode.One:
                    int index = unchecked( (int) (long)SendMessage(NativeMethods.LB_GETCURSEL, 0, 0));
                    if (index >= 0) result = new int[] {index};
                    break;

                case SelectionMode.MultiSimple:
                case SelectionMode.MultiExtended:
                    int count = unchecked( (int) (long)SendMessage(NativeMethods.LB_GETSELCOUNT, 0, 0));
                    if (count > 0) {
                        result = new int[count];
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LB_GETSELITEMS, count, result);
                    }
                    break;
            }

            // Now set the selected state on the appropriate items.
            //
            if (result != null) {
                foreach(int i in result) {
                    SelectedItems.SetSelected(i, true);
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnChangeUICues"]/*' />
        protected override void OnChangeUICues(UICuesEventArgs e) {

            // ListBox seems to get a bit confused when the UI cues change for the first
            // time - it draws the focus rect when it shouldn't and vice-versa. So when
            // the UI cues change, we just do an extra invalidate to get it into the
            // right state.
            //
            Invalidate();

            base.OnChangeUICues(e);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnDrawItem"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the drawItem event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler yourself for this event].  They should,
        ///     however, remember to call base.onDrawItem(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected virtual void OnDrawItem(DrawItemEventArgs e) {
            DrawItemEventHandler handler = (DrawItemEventHandler)Events[EVENT_DRAWITEM];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     We need to know when the window handle has been created so we can
        ///     set up a few things, like column width, etc!  Inheriting classes should
        ///     not forget to call base.OnHandleCreated().
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);

            
            //for getting the current Locale to set the Scrollbars...
            //
            SendMessage(NativeMethods.LB_SETLOCALE, CultureInfo.CurrentCulture.LCID, 0);

            if (columnWidth != 0) {
                SendMessage(NativeMethods.LB_SETCOLUMNWIDTH, columnWidth, 0);
            }
            if (drawMode == DrawMode.OwnerDrawFixed) {
                SendMessage(NativeMethods.LB_SETITEMHEIGHT, 0, ItemHeight);
            }

            if (topIndex != 0) {
                SendMessage(NativeMethods.LB_SETTOPINDEX, topIndex, 0);
            }

            if (UseCustomTabOffsets && CustomTabOffsets != null) {
                int wpar = CustomTabOffsets.Count;
                int[] offsets = new int[wpar];
                CustomTabOffsets.CopyTo(offsets, 0);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LB_SETTABSTOPS, wpar, offsets);
            }

            if (itemsCollection != null) {

                int count = itemsCollection.Count;

                for(int i = 0; i < count; i++) {
                    NativeAdd(itemsCollection[i]);

                    if (selectionMode != SelectionMode.None) {
                        if (selectedItems != null) {
                            selectedItems.PushSelectionIntoNativeListBox(i);
                        }
                    }
                }
            }
            if (selectedItems != null) {
                if (selectedItems.Count > 0 && selectionMode == SelectionMode.One) {
                    SelectedItems.Dirty();
                    SelectedItems.EnsureUpToDate();
                }
            }
            UpdateHorizontalExtent();
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnHandleDestroyed"]/*' />
        /// <devdoc>
        ///     Overridden to make sure that we set up and clear out items
        ///     correctly.  Inheriting controls should not forget to call
        ///     base.OnHandleDestroyed()
        /// </devdoc>
        protected override void OnHandleDestroyed(EventArgs e) {
            SelectedItems.EnsureUpToDate();
            if (Disposing) {
                itemsCollection = null;
            }
            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnMeasureItem"]/*' />
        protected virtual void OnMeasureItem(MeasureItemEventArgs e) {
            MeasureItemEventHandler handler = (MeasureItemEventHandler)Events[EVENT_MEASUREITEM];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);

            // Changing the font causes us to resize, always rounding down.
            // Make sure we do this after base.OnPropertyChanged, which sends the WM_SETFONT message

            // Avoid the listbox and textbox behaviour in Collection editors
            //
            UpdateFontCache();
        }

       
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnParentChanged"]/*' />
        /// <devdoc>
        ///    <para>We override this so we can re-create the handle if the parent has changed.</para>
        /// </devdoc>
        protected override void OnParentChanged(EventArgs e) {
            base.OnParentChanged(e);
            //No need to RecreateHandle if we are removing the Listbox from controls collection...
            //so check the parent before recreating the handle...
            if (this.ParentInternal != null) {
                RecreateHandle();
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnResize"]/*' />
        protected override void OnResize(EventArgs e) {

            base.OnResize(e);

            // There are some repainting issues for RightToLeft - so invalidate when we resize.
            //
            if (RightToLeft == RightToLeft.Yes || this.HorizontalScrollbar) {
                Invalidate();
            }

        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnSelectedIndexChanged"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the selectedIndexChanged event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected override void OnSelectedIndexChanged(EventArgs e) {
            base.OnSelectedIndexChanged(e);

            // set the position in the dataSource, if there is any
            // we will only set the position in the currencyManager if it is different
            // from the SelectedIndex. Setting CurrencyManager::Position (even w/o changing it)
            // calls CurrencyManager::EndCurrentEdit, and that will pull the dataFrom the controls
            // into the backEnd. We do not need to do that.
            //
            if (this.DataManager != null && DataManager.Position != SelectedIndex) {
                //read this as "if everett or   (whidbey and selindex is valid)"
                if (!FormattingEnabled || this.SelectedIndex != -1)
                {
                    // Don't change dataManager position if we simply unselected everything.
                    // (Doing so would cause the first LB item to be selected...)
                    this.DataManager.Position = this.SelectedIndex;
                }
            }

            // Call the handler after updating the DataManager's position so that
            // the DataManager's selected index will be correct in an event handler.
            EventHandler handler = (EventHandler)Events[EVENT_SELECTEDINDEXCHANGED];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnSelectedValueChanged"]/*' />
        protected override void OnSelectedValueChanged(EventArgs e) {
            base.OnSelectedValueChanged(e);
            selectedValueChangedFired = true;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnDataSourceChanged"]/*' />
        protected override void OnDataSourceChanged(EventArgs e) {
            if (DataSource == null)
            {
                BeginUpdate();
                SelectedIndex = -1;
                Items.ClearInternal();
                EndUpdate();
            }
            base.OnDataSourceChanged(e);
            RefreshItems();
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.OnDisplayMemberChanged"]/*' />
        protected override void OnDisplayMemberChanged(EventArgs e) {
            base.OnDisplayMemberChanged(e);

            // we want to use the new DisplayMember even if there is no data source
            RefreshItems();

            if (SelectionMode != SelectionMode.None && this.DataManager != null)
                this.SelectedIndex = this.DataManager.Position;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Refresh"]/*' />
        /// <devdoc>
        ///     Forces the ListBox to invalidate and immediately
        ///     repaint itself and any children if OwnerDrawVariable.
        /// </devdoc>
        public override void Refresh() {
            if (drawMode == DrawMode.OwnerDrawVariable) {
                //Fire MeasureItem for Each Item in the Listbox...
                int cnt = Items.Count;
                Graphics graphics = CreateGraphicsInternal();

                try
                {
                    for (int i = 0; i < cnt; i++) {
                        MeasureItemEventArgs mie = new MeasureItemEventArgs(graphics, i, ItemHeight);
                        OnMeasureItem(mie);
                    }
                }
                finally {
                    graphics.Dispose();
                }

            }
            base.Refresh();
        }
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.RefreshItems"]/*' />
        /// <devdoc>
        /// Reparses the objects, getting new text strings for them.
        /// </devdoc>
        /// <internalonly/>
        protected override void RefreshItems() {

            // Store the currently selected object collection.
            //
            ObjectCollection savedItems = itemsCollection;

            // Clear the items.
            //
            itemsCollection = null;
            selectedIndices = null;

            if (IsHandleCreated) {
                NativeClear();
            }

            object[] newItems = null;

            // if we have a dataSource and a DisplayMember, then use it
            // to populate the Items collection
            //
            if (this.DataManager != null && this.DataManager.Count != -1) {
                newItems = new object[this.DataManager.Count];
                for(int i = 0; i < newItems.Length; i++) {
                    newItems[i] = this.DataManager[i];
                }
            }
            else if (savedItems != null) {
                newItems = new object[savedItems.Count];
                savedItems.CopyTo(newItems, 0);
            }

            // Store the current list of items
            //
            if (newItems != null) {
                Items.AddRangeInternal(newItems);
            }

            // Restore the selected indices if SelectionMode allows it.
            //
            if (SelectionMode != SelectionMode.None) {
                if (this.DataManager != null) {
                    // put the selectedIndex in sync w/ the position in the dataManager
                    this.SelectedIndex = this.DataManager.Position;
                }
                else {
                    if (savedItems != null) {
                        int cnt = savedItems.Count;
                        for(int index = 0; index < cnt; index++) {
                            if (savedItems.InnerArray.GetState(index, SelectedObjectCollection.SelectedObjectMask)) {
                                SelectedItem = savedItems[index];
                            }
                        }
                    }
                }
            }

        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.RefreshItem"]/*' />
        /// <devdoc>
        /// Reparses the object at the given index, getting new text string for it.
        /// </devdoc>
        /// <internalonly/>
        protected override void RefreshItem(int index) {
            Items.SetItemInternal(index, Items[index]);
        }

        public override void ResetBackColor() {
            base.ResetBackColor();
        }

        public override void ResetForeColor() {
            base.ResetForeColor();
        }


        private void ResetItemHeight() {
            itemHeight = DefaultItemHeight;
        }
      
       [SuppressMessage("Microsoft.Portability", "CA1902:AvoidTestingForFloatingPointEquality")]
       protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
           
           if (factor.Width != 1F && factor.Height != 1F) {
               UpdateFontCache();
           }
           base.ScaleControl(factor, specified);
       }


        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SetBoundsCore"]/*' />
        /// <devdoc>
        ///     Overrides Control.SetBoundsCore to remember the requestedHeight.
        /// </devdoc>
        /// <internalonly/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {

            // Avoid the listbox and textbox behaviour in Collection editors
            //


            if (!integralHeightAdjust && height != Height)
                requestedHeight = height;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SetItemsCore"]/*' />
        /// <devdoc>
        ///     Performs the work of setting the specified items into the ListBox.
        /// </devdoc>
        protected override void SetItemsCore(IList value) {
                BeginUpdate();
                Items.ClearInternal();
                Items.AddRangeInternal(value);

                this.SelectedItems.Dirty();

                // if the list changed, we want to keep the same selected index
                // CurrencyManager will provide the PositionChanged event
                // it will be provided before changing the list though...
                if (this.DataManager != null) {
                    if (this.DataSource is ICurrencyManagerProvider) {
                        // Everett ListControl's had a 





                        this.selectedValueChangedFired = false;
                    }

                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.LB_SETCURSEL, DataManager.Position, 0);
                    }

                    // if the list changed and we still did not fire the
                    // onselectedChanged event, then fire it now;
                    if (!selectedValueChangedFired) {
                        OnSelectedValueChanged(EventArgs.Empty);
                        selectedValueChangedFired = false;
                    }
                }
                EndUpdate();
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SetItemCore"]/*' />
        protected override void SetItemCore(int index, object value) {
            Items.SetItemInternal(index, value);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SetSelected"]/*' />
        /// <devdoc>
        ///     Allows the user to set an item as being selected or not.  This should
        ///     only be used with ListBoxes that allow some sort of multi-selection.
        /// </devdoc>
        public void SetSelected(int index, bool value) {
            int itemCount = (itemsCollection == null) ? 0: itemsCollection.Count;
            if (index < 0 || index >= itemCount)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

            if (selectionMode == SelectionMode.None)
                throw new InvalidOperationException(SR.ListBoxInvalidSelectionMode);

            SelectedItems.SetSelected(index, value);
            if (IsHandleCreated) {
                NativeSetSelected(index, value);
            }
            SelectedItems.Dirty();
            OnSelectedIndexChanged(EventArgs.Empty);
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.Sort"]/*' />
        /// <devdoc>
        ///     Sorts the items in the listbox.
        /// </devdoc>
        protected virtual void Sort() {
            // This will force the collection to add each item back to itself
            // if sorted is now true, then the add method will insert the item
            // into the correct position
            //
            CheckNoDataSource();

            SelectedObjectCollection currentSelections = SelectedItems;
            currentSelections.EnsureUpToDate();

            if (sorted && itemsCollection != null) {
                itemsCollection.InnerArray.Sort();

                // Now that we've sorted, update our handle
                // if it has been created.
                if (IsHandleCreated) {
                    NativeClear();
                    int count = itemsCollection.Count;
                    for(int i = 0; i < count; i++) {
                        NativeAdd(itemsCollection[i]);
                        if (currentSelections.GetSelected(i)) {
                            NativeSetSelected(i, true);
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            if (itemsCollection != null) {
                s += ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
                if (Items.Count > 0) {
                    string z = GetItemText(Items[0]);
                    string txt = (z.Length > 40) ? z.Substring(0, 40) : z;
                    s += ", Items[0]: " + txt;
                }
            }
            return s;
        }
        private void UpdateFontCache() {
            fontIsChanged = true;
            integralHeightAdjust = true;
            try {
                Height = requestedHeight;
            }
            finally {
                integralHeightAdjust = false;
            }
            maxWidth = -1;
            UpdateHorizontalExtent();
            // clear the preferred size cache.
            CommonProperties.xClearPreferredSizeCache(this);

        }

        private void UpdateHorizontalExtent() {
            if (!multiColumn && horizontalScrollbar && IsHandleCreated) {
                int width = horizontalExtent;
                if (width == 0) {
                    width = MaxItemWidth;
                }
                SendMessage(NativeMethods.LB_SETHORIZONTALEXTENT, width, 0);
            }
        }

        // Updates the cached max item width
        //
        private void UpdateMaxItemWidth(object item, bool removing) {

            // We shouldn't be caching maxWidth if we don't have horizontal scrollbars,
            // or horizontal extent has been set
            //
            if (!horizontalScrollbar || horizontalExtent > 0) {
                maxWidth = -1;
                return;
            }

            // Only update if we are currently caching maxWidth
            //
            if (maxWidth > -1) {

                // Compute item width
                //
                int width;
                using (Graphics graphics = CreateGraphicsInternal()) {
                    width = (int)(Math.Ceiling(graphics.MeasureString(GetItemText(item), this.Font).Width));
                }

                if (removing) {
                    // We're removing this item, so if it's the longest
                    // in the list, reset the cache
                    //
                    if (width >= maxWidth) {
                        maxWidth = -1;
                    }
                }
                else {
                    // We're adding or inserting this item - update the cache
                    //
                    if (width > maxWidth) {
                        maxWidth = width;
                    }
                }
            }
        }

        // Updates the Custom TabOffsets
        //

        private  void UpdateCustomTabOffsets() {
            if (IsHandleCreated && UseCustomTabOffsets && CustomTabOffsets != null) {
                int wpar = CustomTabOffsets.Count;
                int[] offsets = new int[wpar];
                CustomTabOffsets.CopyTo(offsets, 0);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LB_SETTABSTOPS, wpar, offsets);
                Invalidate();
            }
        }

        private void WmPrint(ref Message m) {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & (int)m.LParam) != 0 && Application.RenderWithVisualStyles && this.BorderStyle == BorderStyle.Fixed3D) {
                IntSecurity.UnmanagedCode.Assert();
                try {
                    using (Graphics g = Graphics.FromHdc(m.WParam)) {
                        Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
                        using (Pen pen = new Pen(VisualStyleInformation.TextControlBorder)) {
                            g.DrawRectangle(pen, rect);
                        }
                        rect.Inflate(-1, -1);
                        g.DrawRectangle(SystemPens.Window, rect);
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.WmReflectCommand"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [
        System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode),
        System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual void WmReflectCommand(ref Message m) {
            switch (NativeMethods.Util.HIWORD(m.WParam)) {
                case NativeMethods.LBN_SELCHANGE:
                    if (selectedItems != null) {
                        selectedItems.Dirty();
                    }
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case NativeMethods.LBN_DBLCLK:
                    // Handle this inside WM_LBUTTONDBLCLK
                    // OnDoubleClick(EventArgs.Empty);
                    break;
            }
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.WmReflectDrawItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectDrawItem(ref Message m) {
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));
            IntPtr dc = dis.hDC;
            IntPtr oldPal = SetUpPalette(dc, false /*force*/, false /*realize*/);
            try {
                Graphics g = Graphics.FromHdcInternal(dc);

                try {
                    Rectangle bounds = Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom);
                    
                    if (HorizontalScrollbar) {
                        if (MultiColumn) {
                            bounds.Width = Math.Max(ColumnWidth, bounds.Width);
                        }
                        else {
                            bounds.Width = Math.Max(MaxItemWidth, bounds.Width);
                        }
                    }
                    

                    OnDrawItem(new DrawItemEventArgs(g, Font, bounds, dis.itemID, (DrawItemState)dis.itemState, ForeColor, BackColor));
                }
                finally {
                    g.Dispose();
                }
            }
            finally {
                if (oldPal != IntPtr.Zero) {
                    SafeNativeMethods.SelectPalette(new HandleRef(null, dc), new HandleRef(null, oldPal), 0);
                }
            }
            m.Result = (IntPtr)1;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.WmReflectMeasureItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        // This method is only called if in owner draw mode
        private void WmReflectMeasureItem(ref Message m) {

            NativeMethods.MEASUREITEMSTRUCT mis = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));

            if (drawMode == DrawMode.OwnerDrawVariable && mis.itemID >= 0) {
                Graphics graphics = CreateGraphicsInternal();
                MeasureItemEventArgs mie = new MeasureItemEventArgs(graphics, mis.itemID, ItemHeight);
                try {
                    OnMeasureItem(mie);
                    mis.itemHeight = mie.ItemHeight;
                }
                finally {
                    graphics.Dispose();
                }
            }
            else {
                mis.itemHeight = ItemHeight;
            }
            Marshal.StructureToPtr(mis, m.LParam, false);
            m.Result = (IntPtr)1;
        }

        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.WndProc"]/*' />
        /// <devdoc>
        ///     The list's window procedure.  Inheriting classes can override this
        ///     to add extra functionality, but should not forget to call
        ///     base.wndProc(m); to ensure the list continues to function properly.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND:
                    WmReflectCommand(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_DRAWITEM:
                    WmReflectDrawItem(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_MEASUREITEM:
                    WmReflectMeasureItem(ref m);
                    break;
                case NativeMethods.WM_PRINT:
                    WmPrint(ref m);
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                    if (selectedItems != null) {
                        selectedItems.Dirty();
                    }
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    // Get the mouse location
                    //
                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    Point pt = new Point(x,y);
                    pt = PointToScreen(pt);
                    bool captured = Capture;
                    if (captured && UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {


                        if (!doubleClickFired && !ValidationCancelled) {
                             OnClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                             OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

                        }
                        else {
                            doubleClickFired = false;
                            // WM_COMMAND is only fired if the user double clicks an item,
                            // so we can't use that as a double-click substitute
                            if (!ValidationCancelled) {
                                OnDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                                OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

                            }
                       }
                    }

                    //
                    // If this control has been disposed in the user's event handler, then we need to ignore the WM_LBUTTONUP
                    // message to avoid exceptions thrown as a result of handle re-creation.
                    // We handle this situation here and not at the top of the window procedure since this is the only place
                    // where we can get disposed as an effect of external code (form.Close() for instance) and then pass the
                    // message to the base class.
                    //
                    if (GetState(STATE_DISPOSED))
                    {
                        base.DefWndProc(ref m);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }

                    doubleClickFired = false;
                    break;

                case NativeMethods.WM_RBUTTONUP:
                    // Get the mouse location
                    //
                    int rx = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int ry = NativeMethods.Util.SignedHIWORD(m.LParam);
                    Point rpt = new Point(rx,ry);
                    rpt = PointToScreen(rpt);
                    bool rCaptured = Capture;
                    if (rCaptured && UnsafeNativeMethods.WindowFromPoint(rpt.X, rpt.Y) == Handle) {
                        if (selectedItems != null) {
                            selectedItems.Dirty();
                        }
                    }
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_LBUTTONDBLCLK:
                    //the Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP...
                    //sequence for doubleclick...
                    //the first WM_LBUTTONUP, resets the flag for Doubleclick
                    //So its necessary for us to set it again...
                    doubleClickFired = true;
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_WINDOWPOSCHANGED:
                    base.WndProc(ref m);
                    if (integralHeight && fontIsChanged) {
                        Height = Math.Max(Height,ItemHeight);
                        fontIsChanged = false;
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <devdoc>
        ///     This is similar to ArrayList except that it also
        ///     mantains a bit-flag based state element for each item
        ///     in the array.
        ///
        ///     The methods to enumerate, count and get data support
        ///     virtualized indexes.  Indexes are virtualized according
        ///     to the state mask passed in.  This allows ItemArray
        ///     to be the backing store for one read-write "master"
        ///     collection and serveral read-only collections based
        ///     on masks.  ItemArray supports up to 31 masks.
        /// </devdoc>
        internal class ItemArray : IComparer {

            private static int lastMask = 1;

            private ListControl listControl;
            private Entry[]     entries;
            private int         count;
            private int         version;

            public ItemArray(ListControl listControl) {
                this.listControl = listControl;
            }

            /// <devdoc>
            ///     The version of this array.  This number changes with each
            ///     change to the item list.
            /// </devdoc>
            public int Version {
                get {
                    return version;
                }
            }

            /// <devdoc>
            ///     Adds the given item to the array.  The state is initially
            ///     zero.
            /// </devdoc>
            public object Add(object item) {
                EnsureSpace(1);
                version++;
                entries[count] = new Entry(item);
                return entries[count++];
            }

            /// <devdoc>
            ///     Adds the given collection of items to the array.
            /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void AddRange(ICollection items) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSpace(items.Count);
                foreach(object i in items) {
                    entries[count++] = new Entry(i);
                }
                version++;
            }

            /// <devdoc>
            ///     Clears this array.
            /// </devdoc>
            public void Clear() {
                if (count > 0) {
                    Array.Clear(entries, 0, count);
                }

                count = 0;
                version++;
            }

            /// <devdoc>
            ///     Allocates a new bitmask for use.
            /// </devdoc>
            public static int CreateMask() {
                int mask = lastMask;
                lastMask = lastMask << 1;
                Debug.Assert(lastMask > mask, "We have overflowed our state mask.");
                return mask;
            }

            /// <devdoc>
            ///     Ensures that our internal array has space for
            ///     the requested # of elements.
            /// </devdoc>
            private void EnsureSpace(int elements) {
                if (entries == null) {
                    entries = new Entry[Math.Max(elements, 4)];
                }
                else if (count + elements >= entries.Length) {
                    int newLength = Math.Max(entries.Length * 2, entries.Length + elements);
                    Entry[] newEntries = new Entry[newLength];
                    entries.CopyTo(newEntries, 0);
                    entries = newEntries;
                }
            }

            /// <devdoc>
            ///     Turns a virtual index into an actual index.
            /// </devdoc>
            public int GetActualIndex(int virtualIndex, int stateMask) {
                if (stateMask == 0) {
                    return virtualIndex;
                }

                // More complex; we must compute this index.
                int calcIndex = -1;
                for(int i = 0; i < count; i++) {
                    if ((entries[i].state & stateMask) != 0) {
                        calcIndex++;
                        if (calcIndex == virtualIndex) {
                            return i;
                        }
                    }
                }

                return -1;
            }

            /// <devdoc>
            ///     Gets the count of items matching the given mask.
            /// </devdoc>
            public int GetCount(int stateMask) {
                // If mask is zero, then just give the main count
                if (stateMask == 0) {
                    return count;
                }

                // more complex:  must provide a count of items
                // based on a mask.

                int filteredCount = 0;

                for(int i = 0; i < count; i++) {
                    if ((entries[i].state & stateMask) != 0) {
                        filteredCount++;
                    }
                }

                return filteredCount;
            }

            /// <devdoc>
            ///     Retrieves an enumerator that will enumerate based on
            ///     the given mask.
            /// </devdoc>
            public IEnumerator GetEnumerator(int stateMask) {
                return GetEnumerator(stateMask, false);
            }

            /// <devdoc>
            ///     Retrieves an enumerator that will enumerate based on
            ///     the given mask.
            /// </devdoc>
            public IEnumerator GetEnumerator(int stateMask, bool anyBit) {
                return new EntryEnumerator(this, stateMask, anyBit);
            }

            /// <devdoc>
            ///     Gets the item at the given index.  The index is
            ///     virtualized against the given mask value.
            /// </devdoc>
            public object GetItem(int virtualIndex, int stateMask) {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1) {
                    throw new IndexOutOfRangeException();
                }

                return entries[actualIndex].item;
            }
            /// <devdoc>
            ///     Gets the item at the given index.  The index is
            ///     virtualized against the given mask value.
            /// </devdoc>
            internal object GetEntryObject(int virtualIndex, int stateMask) {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1) {
                    throw new IndexOutOfRangeException();
                }

                return entries[actualIndex];
            }
            /// <devdoc>
            ///     Returns true if the requested state mask is set.
            ///     The index is the actual index to the array.
            /// </devdoc>
            public bool GetState(int index, int stateMask) {
                return ((entries[index].state & stateMask) == stateMask);
            }

            /// <devdoc>
            ///     Returns the virtual index of the item based on the
            ///     state mask.
            /// </devdoc>
            public int IndexOf(object item, int stateMask) {

                int virtualIndex = -1;

                for(int i = 0; i < count; i++) {
                    if (stateMask == 0 || (entries[i].state & stateMask) != 0) {
                        virtualIndex++;
                        if (entries[i].item.Equals(item)) {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <devdoc>
            ///     Returns the virtual index of the item based on the
            ///     state mask. Uses reference equality to identify the
            ///     given object in the list.
            /// </devdoc>
            public int IndexOfIdentifier(object identifier, int stateMask) {
                int virtualIndex = -1;

                for(int i = 0; i < count; i++) {
                    if (stateMask == 0 || (entries[i].state & stateMask) != 0) {
                        virtualIndex++;
                        if (entries[i] == identifier) {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <devdoc>
            ///     Inserts item at the given index.  The index
            ///     is not virtualized.
            /// </devdoc>
            public void Insert(int index, object item) {
                EnsureSpace(1);

                if (index < count) {
                    System.Array.Copy(entries, index, entries, index + 1, count - index);
                }

                entries[index] = new Entry(item);
                count++;
                version++;
            }

            /// <devdoc>
            ///     Removes the given item from the array.  If
            ///     the item is not in the array, this does nothing.
            /// </devdoc>
            public void Remove(object item) {
                int index = IndexOf(item, 0);

                if (index != -1) {
                    RemoveAt(index);
                }
            }

            /// <devdoc>
            ///     Removes the item at the given index.
            /// </devdoc>
            public void RemoveAt(int index) {
                count--;
                for (int i = index; i < count; i++) {
                    entries[i] = entries[i+1];
                }
                entries[count] = null;
                version++;
            }

            /// <devdoc>
            ///     Sets the item at the given index to a new value.
            /// </devdoc>
            public void SetItem(int index, object item) {
                entries[index].item = item;
            }

            /// <devdoc>
            ///     Sets the state data for the given index.
            /// </devdoc>
            public void SetState(int index, int stateMask, bool value) {
                if (value) {
                    entries[index].state |= stateMask;
                }
                else {
                    entries[index].state &= ~stateMask;
                }
                version++;
            }

            /// <devdoc>
            ///     Find element in sorted array. If element is not found returns a binary complement of index for inserting
            /// </devdoc>
            public int BinarySearch(object element)
            {
                return Array.BinarySearch(entries, 0, count, element, this);
            }


            /// <devdoc>
            ///     Sorts our array.
            /// </devdoc>
            public void Sort() {
                Array.Sort(entries, 0, count, this);
            }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void Sort(Array externalArray) {
                Array.Sort(externalArray, this);
            }

            int IComparer.Compare(object item1, object item2) {
                if (item1 == null) {
                    if (item2 == null)
                        return 0; //both null, then they are equal

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                    return 1; //item2 is null, so item 1 is greater

                if (item1 is Entry) {
                    item1 = ((Entry)item1).item;
                }

                if (item2 is Entry) {
                    item2 = ((Entry)item2).item;
                }

                String itemName1 = listControl.GetItemText(item1);
                String itemName2 = listControl.GetItemText(item2);

                CompareInfo compInfo = (Application.CurrentCulture).CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }

            /// <devdoc>
            ///     This is a single entry in our item array.
            /// </devdoc>
            private class Entry {
                public object item;
                public int state;

                public Entry(object item) {
                    this.item = item;
                    this.state = 0;
                }
            }

            /// <devdoc>
            ///     EntryEnumerator is an enumerator that will enumerate over
            ///     a given state mask.
            /// </devdoc>
            private class EntryEnumerator : IEnumerator {
                private ItemArray items;
                private bool anyBit;
                private int state;
                private int current;
                private int version;

                /// <devdoc>
                ///     Creates a new enumerator that will enumerate over the given state.
                /// </devdoc>
                public EntryEnumerator(ItemArray items, int state, bool anyBit) {
                    this.items = items;
                    this.state = state;
                    this.anyBit = anyBit;
                    this.version = items.version;
                    this.current = -1;
                }

                /// <devdoc>
                ///     Moves to the next element, or returns false if at the end.
                /// </devdoc>
                bool IEnumerator.MoveNext() {
                    if(version != items.version) throw new InvalidOperationException(SR.ListEnumVersionMismatch);

                    while(true) {
                        if (current < items.count - 1) {
                                            current++;
                            if (anyBit) {
                                if ((items.entries[current].state & state) != 0) {
                                    return true;
                                }
                            }
                            else {
                                if ((items.entries[current].state & state) == state) {
                                    return true;
                                }
                            }
                        }
                        else {
                            current = items.count;
                            return false;
                        }
                    }
                }

                /// <devdoc>
                ///     Resets the enumeration back to the beginning.
                /// </devdoc>
                void IEnumerator.Reset() {
                    if(version != items.version) throw new InvalidOperationException(SR.ListEnumVersionMismatch);
                    current = -1;
                }

                /// <devdoc>
                ///     Retrieves the current value in the enumerator.
                /// </devdoc>
                object IEnumerator.Current {
                    get {
                        if (current == -1 || current == items.count) {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items.entries[current].item;
                    }
                }
            }
        }

        // Items
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection"]/*' />
        /// <devdoc>
        ///     <para>
        ///       A collection that stores objects.
        ///    </para>
        /// </devdoc>
        [ListBindable(false)]
        public class ObjectCollection : IList {

            private ListBox owner;
            private ItemArray items;

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.ObjectCollection"]/*' />
            public ObjectCollection(ListBox owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.ObjectCollection1"]/*' />
            /// <devdoc>
            ///     <para>
            ///       Initializes a new instance of ListBox.ObjectCollection based on another ListBox.ObjectCollection.
            ///    </para>
            /// </devdoc>
            public ObjectCollection(ListBox owner, ObjectCollection value) {
                this.owner = owner;
                this.AddRange(value);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.ObjectCollection2"]/*' />
            /// <devdoc>
            ///     <para>
            ///       Initializes a new instance of ListBox.ObjectCollection containing any array of objects.
            ///    </para>
            /// </devdoc>
            public ObjectCollection(ListBox owner, object[] value) {
                this.owner = owner;
                this.AddRange(value);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Count"]/*' />
            /// <devdoc>
            ///     Retrieves the number of items.
            /// </devdoc>
            public int Count {
                get {
                    return InnerArray.GetCount(0);
                }
            }

            /// <devdoc>
            ///     Internal access to the actual data store.
            /// </devdoc>
            internal ItemArray InnerArray {
                get {
                    if (items == null) {
                        items = new ItemArray(owner);
                    }
                    return items;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ObjectCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds an item to the List box. For an unsorted List box, the item is
            ///     added to the end of the existing list of items. For a sorted List box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's toString() method is called to obtain the string that is
            ///     displayed in the combo box.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>

            public int Add(object item)
            {
                owner.CheckNoDataSource();
                int index = AddInternal(item);
                owner.UpdateHorizontalExtent();
                return index;
            }


            private int AddInternal(object item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                int index = -1;
                if (!owner.sorted)
                {
                    InnerArray.Add(item);
                }
                else
                {
                    if (Count > 0)
                    {
                        index = InnerArray.BinarySearch(item);
                        if (index < 0)
                        {
                            index = ~index; // getting the index of the first element that is larger than the search value
                                            //this index will be used for insert
                        }
                    }
                    else
                        index = 0;

                    Debug.Assert(index >= 0 && index <= Count, "Wrong index for insert");
                    InnerArray.Insert(index, item);
                }
                bool successful = false;

                try
                {
                    if (owner.sorted)
                    {
                        if (owner.IsHandleCreated)
                        {
                            owner.NativeInsert(index, item);
                            owner.UpdateMaxItemWidth(item, false);
                            if (owner.selectedItems != null)
                            {
                                // Sorting may throw the LB contents and the selectedItem array out of synch.
                                owner.selectedItems.Dirty();
                            }
                        }
                    }
                    else
                    {
                        index = Count - 1;
                        if (owner.IsHandleCreated)
                        {
                            owner.NativeAdd(item);
                            owner.UpdateMaxItemWidth(item, false);
                        }
                    }
                    successful = true;
                }
                finally
                {
                    if (!successful)
                    {
                        InnerArray.Remove(item);
                    }
                }
              
                return index;
            }

 
            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ObjectCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object item) {
                return Add(item);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.AddRange1"]/*' />
            public void AddRange(ObjectCollection value) {
                owner.CheckNoDataSource();
                AddRangeInternal((ICollection)value);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.AddRange"]/*' />
            public void AddRange(object[] items) {
                owner.CheckNoDataSource();
                AddRangeInternal((ICollection)items);
            }
           
            internal void AddRangeInternal(ICollection items) {

                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                owner.BeginUpdate();
                try
                {
                    foreach (object item in items)
                    {
                        // adding items one-by-one for performance 
                        // not using sort because after the array is sorted index of each newly added item will need to be found
                        // AddInternal is based on BinarySearch and finds index without any additional cost
                        AddInternal(item);
                    }
                }
                finally
                {
                    owner.UpdateHorizontalExtent();
                    owner.EndUpdate();
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the item with the specified index.
            /// </devdoc>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual object this[int index] {
                get {
                    if (index < 0 || index >= InnerArray.GetCount(0)) {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    return InnerArray.GetItem(index, 0);
                }
                set {
                    owner.CheckNoDataSource();
                    SetItemInternal(index, value);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all items from the ListBox.
            /// </devdoc>
            public virtual void Clear() {
                owner.CheckNoDataSource();
                ClearInternal();
            }

            /// <devdoc>
            ///     Removes all items from the ListBox.  Bypasses the data source check.
            /// </devdoc>
            internal void ClearInternal() {

                //update the width.. to reset Scrollbars..
                // Clear the selection state.
                //
                int cnt = owner.Items.Count;
                for (int i = 0; i < cnt; i++) {
                    owner.UpdateMaxItemWidth(InnerArray.GetItem(i, 0), true);
                }


                if (owner.IsHandleCreated) {
                    owner.NativeClear();
                }
                InnerArray.Clear();
                owner.maxWidth = -1;
                owner.UpdateHorizontalExtent();
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Contains"]/*' />
            public bool Contains(object value) {
                return IndexOf(value) != -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.CopyTo"]/*' />
            /// <devdoc>
            ///     Copies the ListBox Items collection to a destination array.
            /// </devdoc>
            public void CopyTo(object[] destination, int arrayIndex) {
                int count = InnerArray.GetCount(0);
                for(int i = 0; i < count; i++) {
                    destination[i + arrayIndex] = InnerArray.GetItem(i, 0);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array destination, int index) {
                int count = InnerArray.GetCount(0);
                for(int i = 0; i < count; i++) {
                    destination.SetValue(InnerArray.GetItem(i, 0), i + index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///     Returns an enumerator for the ListBox Items collection.
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                return InnerArray.GetEnumerator(0);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.IndexOf"]/*' />
            public int IndexOf(object value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOf(value,0);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.IndexOfIdentifier"]/*' />
            /// <internalonly/>
            internal int IndexOfIdentifier(object value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOfIdentifier(value,0);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Insert"]/*' />
            /// <devdoc>
            ///     Adds an item to the combo box. For an unsorted combo box, the item is
            ///     added to the end of the existing list of items. For a sorted combo box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's toString() method is called to obtain the string that is
            ///     displayed in the combo box.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>
            public void Insert(int index, object item) {
                owner.CheckNoDataSource();

                if (index < 0 || index > InnerArray.GetCount(0)) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                if (item == null) {
                    throw new ArgumentNullException(nameof(item));
                }

                // If the combo box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (owner.sorted) {
                    Add(item);
                }
                else {
                    InnerArray.Insert(index, item);
                    if (owner.IsHandleCreated) {

                        bool successful = false;

                        try {
                            owner.NativeInsert(index, item);
                            owner.UpdateMaxItemWidth(item, false);
                            successful = true;
                        }
                        finally {
                            if (!successful) {
                                InnerArray.RemoveAt(index);
                            }
                        }
                    }
                }
                owner.UpdateHorizontalExtent();
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes the given item from the ListBox, provided that it is
            ///     actually in the list.
            /// </devdoc>
            public void Remove(object value) {

                int index = InnerArray.IndexOf(value, 0);

                if (index != -1) {
                    RemoveAt(index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.ObjectCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     Removes an item from the ListBox at the given index.
            /// </devdoc>
            public void RemoveAt(int index) {
                owner.CheckNoDataSource();

                if (index < 0 || index >= InnerArray.GetCount(0)) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                owner.UpdateMaxItemWidth(InnerArray.GetItem(index, 0), true);

                // Update InnerArray before calling NativeRemoveAt to ensure that when
                // SelectedIndexChanged is raised (by NativeRemoveAt), InnerArray's state matches wrapped LB state.
                InnerArray.RemoveAt(index);

                if (owner.IsHandleCreated) {
                    owner.NativeRemoveAt(index);
                }

                owner.UpdateHorizontalExtent();
            }

            internal void SetItemInternal(int index, object value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                if (index < 0 || index >= InnerArray.GetCount(0)) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                owner.UpdateMaxItemWidth(InnerArray.GetItem(index, 0), true);
                InnerArray.SetItem(index, value);

                // If the native control has been created, and the display text of the new list item object
                // is different to the current text in the native list item, recreate the native list item...
                if (owner.IsHandleCreated) {
                    bool selected = (owner.SelectedIndex == index);
                    if (String.Compare(this.owner.GetItemText(value), this.owner.NativeGetItemText(index), true, CultureInfo.CurrentCulture) != 0) {
                        owner.NativeRemoveAt(index);
                        owner.SelectedItems.SetSelected(index, false);
                        owner.NativeInsert(index, value);
                        owner.UpdateMaxItemWidth(value, false);
                        if (selected) {
                            owner.SelectedIndex = index;
                        }
                    }
                    else {
                        // FOR COMPATIBILITY REASONS
                        if (selected) {
                            owner.OnSelectedIndexChanged(EventArgs.Empty); //will fire selectedvaluechanged
                        }
                    }
                }
                owner.UpdateHorizontalExtent();
            }
        } // end ObjectCollection

        //******************************************************************************************
        // IntegerCollection
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection"]/*' />
        public class IntegerCollection : IList {
            private ListBox owner;
            private int[] innerArray;
            private int count=0;

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.IntegerCollection"]/*' />
            public IntegerCollection(ListBox owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.Count"]/*' />
            /// <devdoc>
            ///    <para>Number of current selected items.</para>
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    return count;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.IsReadOnly"]/*' />
            bool IList.IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.Contains"]/*' />
            public bool Contains(int item) {
                return IndexOf(item) != -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object item) {
                if (item is Int32) {
                    return Contains((int)item);
                }
                else {
                    return false;
                }
            }

            public void Clear()
            {
                count = 0;
                innerArray = null;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.IndexOf"]/*' />
            public int IndexOf(int item) {
                int index = -1;

                if (innerArray != null) {
                    index = Array.IndexOf(innerArray, item);

                    // We initialize innerArray with more elements than needed in the method EnsureSpace, 
                    // and we don't actually remove element from innerArray in the method RemoveAt,
                    // so there maybe some elements which are not actually in innerArray will be found
                    // and we need to filter them out
                    if (index >= count) {
                        index = -1;
                    }
                }

                return index;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object item) {
                if (item is Int32) {
                    return IndexOf((int)item);
                }
                else {
                    return -1;
                }
            }


            /// <devdoc>
            ///     Add a unique integer to the collection in sorted order.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>
            private int AddInternal(int item) {

                EnsureSpace(1);

                int index = IndexOf(item);
                if (index == -1) {
                    innerArray[count++] = item;
                    Array.Sort(innerArray,0,count);
                    index = IndexOf(item);
                }
               return index;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds a unique integer to the collection in sorted order.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>
            public int Add(int item) {
                int index = AddInternal(item);
                owner.UpdateCustomTabOffsets();

                return index;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.Add"]/*' />
            /// <internalonly/>
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "item" is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            int IList.Add(object item) {
                if (!(item is int)) {
                    throw new ArgumentException(nameof(item));
                }
                return Add((int)item);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.AddRange2"]/*' />
            public void AddRange(int[] items) {
                AddRangeInternal((ICollection)items);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.AddRange1"]/*' />
            public void AddRange(IntegerCollection value) {
                AddRangeInternal((ICollection)value);
            }

            /// <devdoc>
            ///     Add range that bypasses the data source check.
            /// </devdoc>
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "item" is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            private void AddRangeInternal(ICollection items) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                owner.BeginUpdate();
                try
                {
                    EnsureSpace(items.Count);
                    foreach(object item in items) {
                        if (!(item is int)) {
                            throw new ArgumentException(nameof(item));
                        }
                        else {
                            AddInternal((int)item);
                        }
                    }
                    owner.UpdateCustomTabOffsets();
                }
                finally
                {
                    owner.EndUpdate();
                }
            }


            /// <devdoc>
            ///     Ensures that our internal array has space for
            ///     the requested # of elements.
            /// </devdoc>
            private void EnsureSpace(int elements) {
                if (innerArray == null) {
                    innerArray = new int[Math.Max(elements, 4)];
                }
                else if (count + elements >= innerArray.Length) {
                    int newLength = Math.Max(innerArray.Length * 2, innerArray.Length + elements);
                    int[] newEntries = new int[newLength];
                    innerArray.CopyTo(newEntries, 0);
                    innerArray = newEntries;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                Clear();
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException(SR.ListBoxCantInsertIntoIntegerCollection);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.Remove"]/*' />
            /// <internalonly/>            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "value" is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            void IList.Remove(object value) {
                if (!(value is int)) {
                    throw new ArgumentException(nameof(value));
                }
                Remove((int)value);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                RemoveAt(index);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.Remove1"]/*' />
            /// <devdoc>
            ///     Removes the given item from the array.  If
            ///     the item is not in the array, this does nothing.
            /// </devdoc>
            public void Remove(int item) {

                int index = IndexOf(item);

                if (index != -1) {
                    RemoveAt(index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.RemoveAt1"]/*' />
            /// <devdoc>
            ///     Removes the item at the given index.
            /// </devdoc>
            public void RemoveAt(int index) {
                if (index < 0 || index >= count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                count--;
                for (int i = index; i < count; i++) {
                    innerArray[i] = innerArray[i+1];
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the specified selected item.
            /// </devdoc>
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "index" is the name of the param passed in.
                                                                                                            // So we don't have to localize it.
            ]
            public int this[int index] {
                get {
                    return innerArray[index];
                }
                [
                    SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")     // This exception already shipped.
                                                                                                            // We can't change its text.
                ]
                set {

                    if (index < 0 || index >= count) {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }
                    innerArray[index] = (int)value;
                    owner.UpdateCustomTabOffsets();


                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="IntegerCollection.IList.this"]/*' />
            /// <internalonly/>            
            [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
            object IList.this[int index] {
                get {
                    return this[index];
                }
                [
                    SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters"),    // "value" is the name of the param.
                                                                                                                    // So we don't have to localize it.
                    SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")             // This exception already shipped.
                                                                                                                    // We can't change its text.
                ]
                set {
                    if (!(value is int)) {
                        throw new ArgumentException(nameof(value));
                    }
                    else {
                        this[index] = (int)value;
                    }

                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.CopyTo"]/*' />
            public void CopyTo(Array destination, int index) {
                int cnt = Count;
                for (int i = 0; i < cnt; i++) {
                    destination.SetValue(this[i], i + index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.IntegerCollection.GetEnumerator"]/*' />
            IEnumerator IEnumerable.GetEnumerator() {
                return new CustomTabOffsetsEnumerator(this);
            }

            /// <devdoc>
            ///     EntryEnumerator is an enumerator that will enumerate over
            ///     a given state mask.
            /// </devdoc>
            private class CustomTabOffsetsEnumerator : IEnumerator {
                private IntegerCollection items;
                private int current;

                /// <devdoc>
                ///     Creates a new enumerator that will enumerate over the given state.
                /// </devdoc>
                public CustomTabOffsetsEnumerator(IntegerCollection items) {
                    this.items = items;
                    this.current = -1;
                }

                /// <devdoc>
                ///     Moves to the next element, or returns false if at the end.
                /// </devdoc>
                bool IEnumerator.MoveNext() {

                    if (current < items.Count - 1) {
                        current++;
                        return true;
                    }
                    else {
                        current = items.Count;
                        return false;
                    }
                }

                /// <devdoc>
                ///     Resets the enumeration back to the beginning.
                /// </devdoc>
                void IEnumerator.Reset() {
                    current = -1;
                }

                /// <devdoc>
                ///     Retrieves the current value in the enumerator.
                /// </devdoc>
                object IEnumerator.Current {
                    get {
                        if (current == -1 || current == items.Count) {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items[current];
                    }
                }
            }
        }

        //******************************************************************************************

        // SelectedIndices
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection"]/*' />
        public class SelectedIndexCollection : IList {
            private ListBox owner;

            /* C#r: protected */
            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.SelectedIndexCollection"]/*' />
            public SelectedIndexCollection(ListBox owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.Count"]/*' />
            /// <devdoc>
            ///    <para>Number of current selected items.</para>
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    return owner.SelectedItems.Count;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.Contains"]/*' />
            public bool Contains(int selectedIndex) {
                return IndexOf(selectedIndex) != -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object selectedIndex) {
                if (selectedIndex is Int32) {
                    return Contains((int)selectedIndex);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.IndexOf"]/*' />
            public int IndexOf(int selectedIndex) {

                // Just what does this do?  The selectedIndex parameter above is the index into the
                // main object collection.  We look at the state of that item, and if the state indicates
                // that it is selected, we get back the virtualized index into this collection.  Indexes on
                // this collection match those on the SelectedObjectCollection.
                if (selectedIndex >= 0 &&
                    selectedIndex < InnerArray.GetCount(0) &&
                    InnerArray.GetState(selectedIndex, SelectedObjectCollection.SelectedObjectMask)) {

                    return InnerArray.IndexOf(InnerArray.GetItem(selectedIndex, 0), SelectedObjectCollection.SelectedObjectMask);
                }

                return -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object selectedIndex) {
                if (selectedIndex is Int32) {
                    return IndexOf((int)selectedIndex);
                }
                else {
                    return -1;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the specified selected item.
            /// </devdoc>
            public int this[int index] {
                get {
                    object identifier = InnerArray.GetEntryObject(index, SelectedObjectCollection.SelectedObjectMask);
                    return InnerArray.IndexOfIdentifier(identifier, 0);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedIndexCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
                }
            }

            /// <devdoc>
            ///     This is the item array that stores our data.  We share this backing store
            ///     with the main object collection.
            /// </devdoc>
            private ItemArray InnerArray {
                get {
                    owner.SelectedItems.EnsureUpToDate();
                    return ((ObjectCollection)owner.Items).InnerArray;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.CopyTo"]/*' />
            public void CopyTo(Array destination, int index) {
                int cnt = Count;
                for (int i = 0; i < cnt; i++) {
                    destination.SetValue(this[i], i + index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.ClearSelected"]/*' />
            public void Clear() {
                if (owner != null) {
                    owner.ClearSelected();
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.Add"]/*' />
            public void Add(int index) {
                if (owner != null) {
                    ObjectCollection items = owner.Items;
                    if (items != null) {
                        if (index != -1 && !Contains(index)) {
                            owner.SetSelected(index, true);
                        }
                    }
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.Remove"]/*' />
            public void Remove(int index) {
                if (owner != null) {
                    ObjectCollection items = owner.Items;
                    if (items != null) {
                        if (index != -1 && Contains(index)) {
                            owner.SetSelected(index, false);
                        }
                    }
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedIndexCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                return new SelectedIndexEnumerator(this);
            }

            /// <devdoc>
            ///     EntryEnumerator is an enumerator that will enumerate over
            ///     a given state mask.
            /// </devdoc>
            private class SelectedIndexEnumerator : IEnumerator {
                private SelectedIndexCollection items;
                private int current;

                /// <devdoc>
                ///     Creates a new enumerator that will enumerate over the given state.
                /// </devdoc>
                public SelectedIndexEnumerator(SelectedIndexCollection items) {
                    this.items = items;
                    this.current = -1;
                }

                /// <devdoc>
                ///     Moves to the next element, or returns false if at the end.
                /// </devdoc>
                bool IEnumerator.MoveNext() {

                    if (current < items.Count - 1) {
                        current++;
                        return true;
                    }
                    else {
                        current = items.Count;
                        return false;
                    }
                }

                /// <devdoc>
                ///     Resets the enumeration back to the beginning.
                /// </devdoc>
                void IEnumerator.Reset() {
                    current = -1;
                }

                /// <devdoc>
                ///     Retrieves the current value in the enumerator.
                /// </devdoc>
                object IEnumerator.Current {
                    get {
                        if (current == -1 || current == items.Count) {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items[current];
                    }
                }
            }
        }

        // Should be "ObjectCollection", except we already have one of those.
        /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection"]/*' />
        public class SelectedObjectCollection : IList {

            // This is the bitmask used within ItemArray to identify selected objects.
            internal static int SelectedObjectMask = ItemArray.CreateMask();

            private ListBox owner;
            private bool    stateDirty;
            private int     lastVersion;
            private int     count;

            /* C#r: protected */
            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.SelectedObjectCollection"]/*' />
            public SelectedObjectCollection(ListBox owner) {
                this.owner = owner;
                this.stateDirty = true;
                this.lastVersion = -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.Count"]/*' />
            /// <devdoc>
            ///     Number of current selected items.
            /// </devdoc>
            public int Count {
                get {
                    if (owner.IsHandleCreated) {
                        SelectionMode current = (owner.selectionModeChanging) ? owner.cachedSelectionMode : owner.selectionMode;
                        switch (current) {

                            case SelectionMode.None:
                                return 0;

                            case SelectionMode.One:
                                int index = owner.SelectedIndex;
                                if (index >= 0) {
                                    return 1;
                                }
                                return 0;

                            case SelectionMode.MultiSimple:
                            case SelectionMode.MultiExtended:
                                return unchecked( (int) (long)owner.SendMessage(NativeMethods.LB_GETSELCOUNT, 0, 0));
                        }

                        return 0;
                    }

                    // If the handle hasn't been created, we must do this the hard way.
                    // Getting the count when using a mask is expensive, so cache it.
                    //
                    if (lastVersion != InnerArray.Version) {
                        lastVersion = InnerArray.Version;
                        count = InnerArray.GetCount(SelectedObjectMask);
                    }

                    return count;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return true;
                }
            }

            /// <devdoc>
            ///     Called by the list box to dirty the selected item state.
            /// </devdoc>
            internal void Dirty() {
                stateDirty = true;
            }

            /// <devdoc>
            ///     This is the item array that stores our data.  We share this backing store
            ///     with the main object collection.
            /// </devdoc>
            private ItemArray InnerArray {
                get {
                    EnsureUpToDate();
                    return ((ObjectCollection)owner.Items).InnerArray;
                }
            }


            /// <devdoc>
            ///     This is the function that Ensures that the selections are uptodate with
            ///     current listbox handle selections.
            /// </devdoc>
            internal void EnsureUpToDate() {
                if (stateDirty) {
                     stateDirty = false;
                     if (owner.IsHandleCreated) {
                         owner.NativeUpdateSelection();
                     }
                }
            }


            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.Contains"]/*' />
            public bool Contains(object selectedObject) {
                return IndexOf(selectedObject) != -1;
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.IndexOf"]/*' />
            public int IndexOf(object selectedObject) {
                return InnerArray.IndexOf(selectedObject, SelectedObjectMask);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
            }

            // A new internal method used in SelectedIndex getter...
            // For a Multi select ListBox there can be two items with the same name ...
            // and hence a object comparison is required...
            // This method returns the "object" at the passed index rather than the "item" ...
            // this "object" is then compared in the IndexOf( ) method of the itemsCollection.
            //
            /// <include file='doc\ListBox.uex' path='docs/doc[@for="SelectedObjectCollection.IList.GetObjectAt"]/*' />
            /// <internalonly/>
            internal object GetObjectAt(int index) {
               return InnerArray.GetEntryObject(index, SelectedObjectCollection.SelectedObjectMask);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the specified selected item.
            /// </devdoc>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public object this[int index] {
                get {
                    return InnerArray.GetItem(index, SelectedObjectMask);
                }
                set {
                    throw new NotSupportedException(SR.ListBoxSelectedObjectCollectionIsReadOnly);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.CopyTo"]/*' />
            public void CopyTo(Array destination, int index) {
                int cnt = InnerArray.GetCount(SelectedObjectMask);
                for (int i = 0; i < cnt; i++) {
                    destination.SetValue(InnerArray.GetItem(i, SelectedObjectMask), i + index);
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                return InnerArray.GetEnumerator(SelectedObjectMask);
            }

            /// <devdoc>
            ///     This method returns if the actual item index is selected.  The index is the index to the MAIN
            ///     collection, not this one.
            /// </devdoc>
            internal bool GetSelected(int index) {
                return InnerArray.GetState(index, SelectedObjectMask);
            }

            // when SelectedObjectsCollection::ItemArray is accessed we push the selection from Native ListBox into our .Net ListBox - see EnsureUpToDate()
            // when we create the handle we need to be able to do the opposite : push the selection from .Net ListBox into Native ListBox
            internal void PushSelectionIntoNativeListBox(int index) {
                // we can't use ItemArray accessor because this will wipe out our Selection collection
                bool selected = ((ObjectCollection)owner.Items).InnerArray.GetState(index, SelectedObjectMask);
                // push selection only if the item is actually selected
                // this also takes care of the case where owner.SelectionMode == SelectionMode.One
                if (selected) {
                    this.owner.NativeSetSelected(index, true /*we signal selection to the native listBox only if the item is actually selected*/);
                }
            }

            /// <devdoc>
            ///     Same thing for GetSelected.
            /// </devdoc>
            internal void SetSelected(int index, bool value) {
                InnerArray.SetState(index, SelectedObjectMask, value);
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.ClearSelected"]/*' />
            public void Clear() {
                if (owner != null) {
                    owner.ClearSelected();
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.Add"]/*' />
            public void Add(object value)  {
                if (owner != null) {
                    ObjectCollection items = owner.Items;
                    if (items != null && value != null) {
                        int index = items.IndexOf(value);
                        if (index != -1 && !GetSelected(index)) {
                            owner.SelectedIndex = index;
                        }
                    }
                }
            }

            /// <include file='doc\ListBox.uex' path='docs/doc[@for="ListBox.SelectedObjectCollection.Remove"]/*' />
            public void Remove(object value) {
                if (owner != null) {
                    ObjectCollection items = owner.Items;
                    if (items != null & value != null) {
                        int index = items.IndexOf(value);
                        if (index != -1 && GetSelected(index)) {
                            owner.SetSelected(index, false);
                        }
                    }
                }
            }
        }
    }
}

