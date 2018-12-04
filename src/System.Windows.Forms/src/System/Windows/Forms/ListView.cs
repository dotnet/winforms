// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Security;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.ComponentModel.Design;
    using System.Collections;
    using Microsoft.Win32;
    using System.Globalization;
    using System.Windows.Forms.Layout;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays a list of items in one of four
    ///       views. Each item displays a caption and optionally an image.
    ///
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Docking(DockingBehavior.Ask),
    Designer("System.Windows.Forms.Design.ListViewDesigner, " + AssemblyRef.SystemDesign),
    DefaultProperty(nameof(Items)),
    DefaultEvent(nameof(SelectedIndexChanged)),
    SRDescription(nameof(SR.DescriptionListView))
    ]
    public class ListView : Control {

        //members
        private const int MASK_HITTESTFLAG = 0x00F7;

        private static readonly object EVENT_CACHEVIRTUALITEMS                 = new object();
        private static readonly object EVENT_COLUMNREORDERED                   = new object();
        private static readonly object EVENT_COLUMNWIDTHCHANGED                = new object();
        private static readonly object EVENT_COLUMNWIDTHCHANGING               = new object();
        private static readonly object EVENT_DRAWCOLUMNHEADER                  = new object();
        private static readonly object EVENT_DRAWITEM                          = new object();
        private static readonly object EVENT_DRAWSUBITEM                       = new object();
        private static readonly object EVENT_ITEMSELECTIONCHANGED              = new object();
        private static readonly object EVENT_RETRIEVEVIRTUALITEM               = new object();
        private static readonly object EVENT_SEARCHFORVIRTUALITEM              = new object();
        private static readonly object EVENT_SELECTEDINDEXCHANGED              = new object();
        private static readonly object EVENT_VIRTUALITEMSSELECTIONRANGECHANGED = new object();
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED           = new object();


        private ItemActivation activation     = ItemActivation.Standard;
        private ListViewAlignment alignStyle  = ListViewAlignment.Top;
        private BorderStyle borderStyle       = System.Windows.Forms.BorderStyle.Fixed3D;
        private ColumnHeaderStyle headerStyle = ColumnHeaderStyle.Clickable;
        private SortOrder sorting             = SortOrder.None;
        private View viewStyle                = System.Windows.Forms.View.LargeIcon;
        private string toolTipCaption = String.Empty;


        private const int   LISTVIEWSTATE_ownerDraw                           = 0x00000001;
        private const int   LISTVIEWSTATE_allowColumnReorder                  = 0x00000002;
        private const int   LISTVIEWSTATE_autoArrange                         = 0x00000004;
        private const int   LISTVIEWSTATE_checkBoxes                          = 0x00000008;
        private const int   LISTVIEWSTATE_fullRowSelect                       = 0x00000010;
        private const int   LISTVIEWSTATE_gridLines                           = 0x00000020;
        private const int   LISTVIEWSTATE_hideSelection                       = 0x00000040;
        private const int   LISTVIEWSTATE_hotTracking                         = 0x00000080;
        private const int   LISTVIEWSTATE_labelEdit                           = 0x00000100;
        private const int   LISTVIEWSTATE_labelWrap                           = 0x00000200;
        private const int   LISTVIEWSTATE_multiSelect                         = 0x00000400;
        private const int   LISTVIEWSTATE_scrollable                          = 0x00000800;
        private const int   LISTVIEWSTATE_hoverSelection                      = 0x00001000;
        private const int   LISTVIEWSTATE_nonclickHdr                         = 0x00002000;
        private const int   LISTVIEWSTATE_inLabelEdit                         = 0x00004000;
        private const int   LISTVIEWSTATE_showItemToolTips                    = 0x00008000;
        private const int   LISTVIEWSTATE_backgroundImageTiled                = 0x00010000;
        private const int   LISTVIEWSTATE_columnClicked                       = 0x00020000;
        private const int   LISTVIEWSTATE_doubleclickFired                    = 0x00040000;
        private const int   LISTVIEWSTATE_mouseUpFired                        = 0x00080000;
        private const int   LISTVIEWSTATE_expectingMouseUp                    = 0x00100000;
        private const int   LISTVIEWSTATE_comctlSupportsVisualStyles          = 0x00200000;
        private const int   LISTVIEWSTATE_comctlSupportsVisualStylesTested    = 0x00400000;
        private const int   LISTVIEWSTATE_showGroups                          = 0x00800000;
        private const int   LISTVIEWSTATE_handleDestroyed                     = 0x01000000; // while we are recreating the handle we want to know if we can still get data from the handle
        private const int   LISTVIEWSTATE_virtualMode                         = 0x02000000;
        private const int   LISTVIEWSTATE_headerControlTracking               = 0x04000000;
        private const int   LISTVIEWSTATE_itemCollectionChangedInMouseDown    = 0x08000000;
        private const int   LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon     = 0x10000000;
        private const int   LISTVIEWSTATE_headerDividerDblClick               = 0x20000000;
        private const int   LISTVIEWSTATE_columnResizeCancelled               = 0x40000000;

        private const int   LISTVIEWSTATE1_insertingItemsNatively             = 0x00000001;
        private const int   LISTVIEWSTATE1_cancelledColumnWidthChanging       = 0x00000002;
        private const int   LISTVIEWSTATE1_disposingImageLists                = 0x00000004;
        private const int   LISTVIEWSTATE1_useCompatibleStateImageBehavior        = 0x00000008;
        private const int   LISTVIEWSTATE1_selectedIndexChangedSkipped        = 0x00000010;

        private const int   LVTOOLTIPTRACKING = 0x30;
        private const int   MAXTILECOLUMNS = 20;

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32      listViewState; // see LISTVIEWSTATE_ consts above
        private System.Collections.Specialized.BitVector32      listViewState1;// see LISTVIEWSTATE1_ consts above



        // Ownerdraw data caches...  Only valid inside WM_PAINT.
        // 

        private Color odCacheForeColor = SystemColors.WindowText;
        private Color odCacheBackColor = SystemColors.Window;
        private Font odCacheFont = null;
        private IntPtr odCacheFontHandle = IntPtr.Zero;
        private FontHandleWrapper odCacheFontHandleWrapper = null;

        private ImageList imageListLarge;
        private ImageList imageListSmall;
        private ImageList imageListState;

        private MouseButtons downButton;
        private int itemCount;
        private int columnIndex = 0;
        private int topIndex;          
        private bool hoveredAlready  = false;

        private bool             rightToLeftLayout = false;


        // member variables which are used for VirtualMode
        private int virtualListSize = 0;

        private ListViewGroup defaultGroup = null;

        // Invariant: the table always contains all Items in the ListView, and maps IDs -> Items.
        // listItemsArray is null if the handle is created; otherwise, it contains all Items.
        // We do not try to sort listItemsArray as items are added, but during a handle recreate
        // we will make sure we get the items in the same order the ListView displays them.
        private Hashtable listItemsTable = new Hashtable(); // elements are ListViewItem's
        private ArrayList listItemsArray = new ArrayList(); // elements are ListViewItem's

        private Size tileSize = Size.Empty;


        // when we are in delayed update mode (that is when BeginUpdate has been called, we want to cache the items to
        // add until EndUpdate is called. To do that, we push in an array list into our PropertyStore
        // under this key.  When Endupdate is fired, we process the items all at once.
        //
        private static readonly int PropDelayedUpdateItems          = PropertyStore.CreateKey();

        private int  updateCounter = 0; // the counter we use to track how many BeginUpdate/EndUpdate calls there have been.

        private ColumnHeader[] columnHeaders;
        private ListViewItemCollection listItemCollection;
        private ColumnHeaderCollection columnHeaderCollection;
        private CheckedIndexCollection checkedIndexCollection;
        private CheckedListViewItemCollection checkedListViewItemCollection;
        private SelectedListViewItemCollection selectedListViewItemCollection;
        private SelectedIndexCollection selectedIndexCollection;
        private ListViewGroupCollection groups;
        private ListViewInsertionMark insertionMark;
        private LabelEditEventHandler onAfterLabelEdit;
        private LabelEditEventHandler onBeforeLabelEdit;
        private ColumnClickEventHandler onColumnClick;
        private EventHandler onItemActivate;
        private ItemCheckedEventHandler onItemChecked;
        private ItemDragEventHandler onItemDrag;
        private ItemCheckEventHandler onItemCheck;
        private ListViewItemMouseHoverEventHandler onItemMouseHover;

        // IDs for identifying ListViewItem's
        private int nextID = 0;


        // We save selected and checked items between handle creates.
        private List<ListViewItem> savedSelectedItems;
        private List<ListViewItem> savedCheckedItems;

        // Sorting
        private IComparer listItemSorter = null;

        private ListViewItem prevHoveredItem = null;

        // Background image stuff
        // Because we have to create a temporary file and the OS does not clean up the temporary files from the machine
        // we have to do that ourselves
        string backgroundImageFileName = string.Empty;

        // it *seems* that if the user changes the background image then the win32 listView will hang on to the previous
        // background image until it gets the first WM_PAINT message -  I use words like *seems* because nothing is guaranteed
        // when it comes to win32 listView.
        // so our wrapper has to hang on to the previousBackgroundImageFileNames and destroy them after it gets the first WM_PAINT message
        
        int bkImgFileNamesCount = -1;
        string[] bkImgFileNames = null;
        private const int BKIMGARRAYSIZE = 8;

        // If the user clicked on the column divider, the native ListView fires HDN_ITEMCHANGED on each mouse up event.
        // This means that even if the user did not change the column width our wrapper will still think
        // that the column header width changed.
        // We need to make our ListView wrapper more robust in face of this limitation inside ComCtl ListView.
        // columnHeaderClicked will be set in HDN_BEGINTRACK and reset in HDN_ITEMCHANGED.
        ColumnHeader columnHeaderClicked;
        int columnHeaderClickedWidth;

        // The user cancelled the column width changing event.
        // We cache the NewWidth supplied by the user and use it on HDN_ENDTRACK to set the final column width.
        int newWidthForColumnWidthChangingCancelled = -1;

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListView"]/*' />
        /// <devdoc>
        ///     Creates an empty ListView with default styles.
        /// </devdoc>
        public ListView() : base() {

            int listViewStateFlags = LISTVIEWSTATE_scrollable |
                                     LISTVIEWSTATE_multiSelect |
                                     LISTVIEWSTATE_labelWrap |
                                     LISTVIEWSTATE_autoArrange |
                                     LISTVIEWSTATE_showGroups;
            if (!AccessibilityImprovements.Level3) {
                // Show grey rectangle around the selected list item if the list view is out of focus.
                listViewStateFlags |= LISTVIEWSTATE_hideSelection;
            }

            listViewState = new System.Collections.Specialized.BitVector32(listViewStateFlags);

            listViewState1 = new System.Collections.Specialized.BitVector32(LISTVIEWSTATE1_useCompatibleStateImageBehavior);
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);

            odCacheFont = Font;
            odCacheFontHandle = FontHandle;
            SetBounds(0,0,121,97);

            listItemCollection = new ListViewItemCollection(new ListViewNativeItemCollection(this));
            columnHeaderCollection = new ColumnHeaderCollection(this);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Activation"]/*' />
        /// <devdoc>
        ///     The activation style specifies what kind of user action is required to
        ///     activate an item.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(ItemActivation.Standard),
        SRDescription(nameof(SR.ListViewActivationDescr))
        ]
        public ItemActivation Activation {
            get {
                return activation;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ItemActivation.Standard, (int)ItemActivation.TwoClick)){
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ItemActivation));
                }

                if (this.HotTracking && value != ItemActivation.OneClick) {
                    throw new ArgumentException(SR.ListViewActivationMustBeOnWhenHotTrackingIsOn, "value");
                }

                if (activation != value) {
                    activation = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Alignment"]/*' />
        /// <devdoc>
        ///     The alignment style specifies which side of the window items are aligned
        ///     to by default
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(ListViewAlignment.Top),
        Localizable(true),
        SRDescription(nameof(SR.ListViewAlignmentDescr))
        ]
        public ListViewAlignment Alignment {
            get {
                return alignStyle;
            }

            set {
                // using this as ListViewAlignment has discontiguous values.
                if (!ClientUtils.IsEnumValid_NotSequential(value,
                                                (int)value,
                                                (int)ListViewAlignment.Default,
                                                (int)ListViewAlignment.Top,
                                                (int)ListViewAlignment.Left,
                                                (int)ListViewAlignment.SnapToGrid))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ListViewAlignment));
                }
                if (alignStyle != value) {
                    alignStyle = value;
                    RecreateHandleInternal();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.AllowColumnReorder"]/*' />
        /// <devdoc>
        ///     Specifies whether the user can drag column headers to
        ///     other column positions, thus changing the order of displayed columns.
        ///     This property is only meaningful in Details view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewAllowColumnReorderDescr))
        ]
        public bool AllowColumnReorder {
            get {
                return listViewState[LISTVIEWSTATE_allowColumnReorder];
            }

            set {
                if (AllowColumnReorder != value) {
                    listViewState[LISTVIEWSTATE_allowColumnReorder] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.AutoArrange"]/*' />
        /// <devdoc>
        ///     If AutoArrange is true items are automatically arranged according to
        ///     the alignment property.  Items are also kept snapped to grid.
        ///     This property is only meaningful in Large Icon or Small Icon views.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListViewAutoArrangeDescr))
        ]
        public bool AutoArrange {
            get {
                return listViewState[LISTVIEWSTATE_autoArrange];
            }

            set {
                if (AutoArrange != value) {
                    listViewState[LISTVIEWSTATE_autoArrange] = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BackColor"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
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
                if (IsHandleCreated) {
                    SendMessage(NativeMethods.LVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(BackColor));
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BackgroundImageLayout"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BackgroundImageTiled"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewBackgroundImageTiledDescr))
        ]
        public bool BackgroundImageTiled {
            get {
                return listViewState[LISTVIEWSTATE_backgroundImageTiled];
            }
            set {
                if (BackgroundImageTiled != value) {
                    listViewState[LISTVIEWSTATE_backgroundImageTiled] = value;
                    if (IsHandleCreated && BackgroundImage != null) {
                        // Don't call SetBackgroundImage because SetBackgroundImage deletes the existing image
                        // We don't need to delete it and this causes BAD problems w/ the Win32 list view control.
                        NativeMethods.LVBKIMAGE lvbkImage = new NativeMethods.LVBKIMAGE();
                        lvbkImage.xOffset = 0;
                        lvbkImage.yOffset = 0;

                        if (BackgroundImageTiled)
                            lvbkImage.ulFlags = NativeMethods.LVBKIF_STYLE_TILE;
                        else
                            lvbkImage.ulFlags = NativeMethods.LVBKIF_STYLE_NORMAL;

                        lvbkImage.ulFlags  |= NativeMethods.LVBKIF_SOURCE_URL;
                        lvbkImage.pszImage = this.backgroundImageFileName;
                        lvbkImage.cchImageMax = this.backgroundImageFileName.Length + 1;

                        UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETBKIMAGE, 0, lvbkImage);
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BorderStyle"]/*' />
        /// <devdoc>
        ///     Describes the border style of the window.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.borderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value) {
                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckBoxes"]/*' />
        /// <devdoc>
        ///     If CheckBoxes is true, every item will display a checkbox next
        ///     to it.  The user can change the state of the item by clicking the checkbox.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewCheckBoxesDescr))
        ]
        public bool CheckBoxes {
            get {
                return listViewState[LISTVIEWSTATE_checkBoxes];
            }

            set {
                if (this.UseCompatibleStateImageBehavior) {
                    if (CheckBoxes != value) {
                        
                        if (value && this.View == View.Tile) {
                            throw new NotSupportedException(SR.ListViewCheckBoxesNotSupportedInTileView);
                        }

                        if (CheckBoxes) {
                            // Save away the checked items just in case we re-activate checkboxes
                            //
                            savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                            ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                            CheckedItems.CopyTo(items, 0);
                            for (int i = 0; i < items.Length; i ++) {
                                savedCheckedItems.Add(items[i]);
                            }
                        }
                    
                        listViewState[LISTVIEWSTATE_checkBoxes] = value;
                        UpdateExtendedStyles();
                        
                        if (CheckBoxes && savedCheckedItems != null) {
                            // Check the saved checked items.
                            //
                            if (savedCheckedItems.Count > 0) {
                                foreach(ListViewItem item in savedCheckedItems) {
                                    item.Checked = true;
                                }
                            }
                            savedCheckedItems = null;
                        }                                       
                                           
                        // Comctl should handle auto-arrange for us, but doesn't
                        if (AutoArrange)
                            ArrangeIcons(Alignment);
                    }
                } else {
                    if (CheckBoxes != value) {

                        if (value && this.View == View.Tile) {
                            throw new NotSupportedException(SR.ListViewCheckBoxesNotSupportedInTileView);
                        }

                        if (CheckBoxes) {
                            // Save away the checked items just in case we re-activate checkboxes
                            //
                            savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                            ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                            CheckedItems.CopyTo(items, 0);
                            for (int i = 0; i < items.Length; i ++) {
                                savedCheckedItems.Add(items[i]);
                            }
                        }

                        listViewState[LISTVIEWSTATE_checkBoxes] = value;

                        if ((!value && this.StateImageList != null && this.IsHandleCreated) ||
                            (!value && this.Alignment == ListViewAlignment.Left && this.IsHandleCreated) ||
                            (value && this.View == View.List && this.IsHandleCreated) ||
                            (value && (this.View == View.SmallIcon || this.View == View.LargeIcon) && this.IsHandleCreated)) {
                            // we have to recreate the handle when we are going from CheckBoxes == true to CheckBoxes == false
                            // if we want to have the bitmaps from the StateImageList on the items.

                            /**
                            ***  there are a LOT of issues with setting CheckBoxes to TRUE when in View.List, View.SmallIcon or View.LargeIcon:
                            ***  
                            ***
                            ***  these are caused by the fact that the win32 ListView control does not resize its column width
                            ***  when CheckBoxes changes from FALSE to TRUE.
                            ***  we need to recreate the handle when we set CheckBoxes to TRUE
                            **/
                            RecreateHandleInternal();
                        } else {
                            UpdateExtendedStyles();

                        }

                        if (CheckBoxes && savedCheckedItems != null) {
                            // Check the saved checked items.
                            //
                            if (savedCheckedItems.Count > 0) {
                                foreach(ListViewItem item in savedCheckedItems) {
                                    item.Checked = true;
                                }
                            }
                            savedCheckedItems = null;
                        }

                        // Setting the LVS_CHECKBOXES window style also causes the ListView to display the default checkbox
                        // images rather than the user specified StateImageList.  We send a LVM_SETIMAGELIST to restore the
                        // user's images.
                        if (IsHandleCreated && imageListState != null) {
                            if (CheckBoxes) { // we want custom checkboxes
                                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, imageListState.Handle);
                            } else {
                                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, IntPtr.Zero);
                            }
                        }

                        // Comctl should handle auto-arrange for us, but doesn't
                        if (AutoArrange) {
                            ArrangeIcons(Alignment);
                        }

                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndices"]/*' />
        /// <devdoc>
        ///     The indices of the currently checked list items.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public CheckedIndexCollection CheckedIndices {
            get {
                if (checkedIndexCollection == null) {
                    checkedIndexCollection = new CheckedIndexCollection(this);
                }
                return checkedIndexCollection;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedItems"]/*' />
        /// <devdoc>
        ///     The currently checked list items.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public CheckedListViewItemCollection CheckedItems {
            get {
                if (checkedListViewItemCollection == null) {
                    checkedListViewItemCollection = new CheckedListViewItemCollection(this);
                }
                return checkedListViewItemCollection;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Columns"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, " + AssemblyRef.SystemDesign,typeof(UITypeEditor)),
        SRDescription(nameof(SR.ListViewColumnsDescr)),
        Localizable(true),
        MergableProperty(false)
        ]
        public ColumnHeaderCollection Columns {
            get {
                return columnHeaderCollection;
            }
        }

        /// <devdoc>
        ///     Actually we are using this to indicate whether ComCtl supports
        ///     the new listview features. This is true for ComCtl 6 and above, same as
        ///     the versions that support visual styles.
        /// </devdoc>
        private bool ComctlSupportsVisualStyles {
             get {
                  if(!listViewState[LISTVIEWSTATE_comctlSupportsVisualStylesTested])  {
                      listViewState[LISTVIEWSTATE_comctlSupportsVisualStylesTested] = true;
                      listViewState[LISTVIEWSTATE_comctlSupportsVisualStyles] = Application.ComCtlSupportsVisualStyles;
                  }
                  return listViewState[LISTVIEWSTATE_comctlSupportsVisualStyles];
              }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CreateParams"]/*' />
        /// <devdoc>
        ///     Computes the handle creation parameters for the ListView control.
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;

                cp.ClassName = NativeMethods.WC_LISTVIEW;

                // Keep the scrollbar if we are just updating styles...
                //
                if (IsHandleCreated) {
                    int currentStyle = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                    cp.Style |= (currentStyle & (NativeMethods.WS_HSCROLL | NativeMethods.WS_VSCROLL));
                }

                cp.Style |= NativeMethods.LVS_SHAREIMAGELISTS;

                switch (alignStyle) {
                    case ListViewAlignment.Top:
                        cp.Style |= NativeMethods.LVS_ALIGNTOP;
                        break;
                    case ListViewAlignment.Left:
                        cp.Style |= NativeMethods.LVS_ALIGNLEFT;
                        break;
                }

                if (AutoArrange)
                    cp.Style |= NativeMethods.LVS_AUTOARRANGE;

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }

                switch (headerStyle) {
                    case ColumnHeaderStyle.None:
                        cp.Style |= NativeMethods.LVS_NOCOLUMNHEADER;
                        break;
                    case ColumnHeaderStyle.Nonclickable:
                        cp.Style |= NativeMethods.LVS_NOSORTHEADER;
                        break;
                }

                if (LabelEdit)
                    cp.Style |= NativeMethods.LVS_EDITLABELS;

                if (!LabelWrap)
                    cp.Style |= NativeMethods.LVS_NOLABELWRAP;

                if (!HideSelection)
                    cp.Style |= NativeMethods.LVS_SHOWSELALWAYS;

                if (!MultiSelect)
                    cp.Style |= NativeMethods.LVS_SINGLESEL;

                if (listItemSorter == null) {
                    switch (sorting) {
                        case SortOrder.Ascending:
                            cp.Style |= NativeMethods.LVS_SORTASCENDING;
                            break;
                        case SortOrder.Descending:
                            cp.Style |= NativeMethods.LVS_SORTDESCENDING;
                            break;
                    }
                }

                if (VirtualMode)
                    cp.Style |= NativeMethods.LVS_OWNERDATA;

                // We can do this 'cuz the viewStyle enums are the same values as the actual LVS styles
                // this new check since the value for LV_VIEW_TILE == LVS_SINGLESEL; so dont OR that value since
                // LV_VIEW_TILE is not a STYLE but should be Send via a SENDMESSAGE.
                if (viewStyle != View.Tile ) {
                    cp.Style |= (int)viewStyle;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true) {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }
                return cp;
            }
        }

        internal ListViewGroup DefaultGroup {
            get
            {
                if (defaultGroup == null) {
                    defaultGroup = new ListViewGroup(string.Format(SR.ListViewGroupDefaultGroup, "1"));
                }
                return defaultGroup;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(121, 97);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DoubleBuffered"]/*' />
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                if (this.DoubleBuffered != value)
                {
                    base.DoubleBuffered = value;
                    UpdateExtendedStyles();
                }
            }
        }

        internal bool ExpectingMouseUp {
            get {
                return this.listViewState[LISTVIEWSTATE_expectingMouseUp];
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FocusedItem"]/*' />
        /// <devdoc>
        ///     Retreives the item which currently has the user focus.  This is the
        ///     item that's drawn with the dotted focus rectangle around it.
        ///     Returns null if no item is currently focused.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListViewFocusedItemDescr))
        ]
        public ListViewItem FocusedItem {
            get {
                if (IsHandleCreated) {
                    int displayIndex = unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETNEXTITEM, -1, NativeMethods.LVNI_FOCUSED));
                    if (displayIndex > -1)
                        return Items[displayIndex];
                }
                return null;
            }
            set {
                if (IsHandleCreated && value != null) {
                    value.Focused = true;
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ForeColor"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override Color ForeColor
        {
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
                if (IsHandleCreated) {
                    SendMessage(NativeMethods.LVM_SETTEXTCOLOR, 0, ColorTranslator.ToWin32(ForeColor));
                }
            }
        }

        private bool FlipViewToLargeIconAndSmallIcon {
            get {
                // it never hurts to check that our house is in order
                Debug.Assert(!this.listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || this.View == View.SmallIcon, "we need this bit only in SmallIcon view");
                Debug.Assert(!this.listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || this.ComctlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

                return this.listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon];
            }
            set {
                // it never hurts to check that our house is in order
                Debug.Assert(!value || this.View == View.SmallIcon, "we need this bit only in SmallIcon view");
                Debug.Assert(!value || this.ComctlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

                this.listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] = value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FullRowSelect"]/*' />
        /// <devdoc>
        ///     Specifies whether a click on an item will select the entire row instead
        ///     of just the item itself.
        ///     This property is only meaningful in Details view
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewFullRowSelectDescr))
        ]
        public bool FullRowSelect {
            get {
                return listViewState[LISTVIEWSTATE_fullRowSelect];
            }
            set {
                if (FullRowSelect != value) {
                    listViewState[LISTVIEWSTATE_fullRowSelect] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GridLines"]/*' />
        /// <devdoc>
        ///     If true, draws grid lines between items and subItems.
        ///     This property is only meaningful in Details view
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewGridLinesDescr))
        ]
        public bool GridLines {
            get {
                return listViewState[LISTVIEWSTATE_gridLines];
            }

            set {
                if (GridLines != value) {
                    listViewState[LISTVIEWSTATE_gridLines] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Groups"]/*' />
        /// <devdoc>
        ///     The collection of groups belonging to this ListView
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        Editor("System.Windows.Forms.Design.ListViewGroupCollectionEditor, " + AssemblyRef.SystemDesign,typeof(UITypeEditor)),
        SRDescription(nameof(SR.ListViewGroupsDescr)),
        MergableProperty(false)
        ]
        public ListViewGroupCollection Groups {
            get
            {
                if (groups == null) {
                    groups = new ListViewGroupCollection(this);
                }
                return groups;
            }
        }

        // this essentially means that the version of CommCtl supports list view grouping
        // and that the user wants to make use of list view groups
        internal bool GroupsEnabled
        {
            get {
                return this.ShowGroups && groups != null && groups.Count > 0 && ComctlSupportsVisualStyles && !VirtualMode;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HeaderStyle"]/*' />
        /// <devdoc>
        ///     Column headers can either be invisible, clickable, or non-clickable.
        ///     This property is only meaningful in Details view
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(ColumnHeaderStyle.Clickable),
        SRDescription(nameof(SR.ListViewHeaderStyleDescr))
        ]
        public ColumnHeaderStyle HeaderStyle {
            get { return headerStyle;}
            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ColumnHeaderStyle.None, (int)ColumnHeaderStyle.Clickable))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ColumnHeaderStyle));
                }
                if (headerStyle != value) {
                    // We can switch between NONE and either *one* of the other styles without
                    // recreating the handle, but if we change from CLICKABLE to NONCLICKABLE
                    // or vice versa, with or without an intervening setting of NONE, then
                    // the handle needs to be recreated.
                    headerStyle = value;
                    if ((listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Clickable) ||
                        (!listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Nonclickable)) {
                        listViewState[LISTVIEWSTATE_nonclickHdr] = !listViewState[LISTVIEWSTATE_nonclickHdr];
                        RecreateHandleInternal();
                    }
                    else
                        UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HideSelection"]/*' />
        /// <devdoc>
        ///     If false, selected items will still be highlighted (in a
        ///     different color) when focus is moved away from the ListView.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListViewHideSelectionDescr))
        ]
        public bool HideSelection {
            get {
                return listViewState[LISTVIEWSTATE_hideSelection];
            }

            set {
                if (HideSelection != value) {
                    listViewState[LISTVIEWSTATE_hideSelection] = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HotTracking"]/*' />
        /// <devdoc>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewHotTrackingDescr))
        ]
        public bool HotTracking {
            get {
                return listViewState[LISTVIEWSTATE_hotTracking];
            }
            set {
                if (HotTracking != value) {

                    listViewState[LISTVIEWSTATE_hotTracking] = value;
                    if (value) {
                        this.HoverSelection = true;
                        this.Activation = ItemActivation.OneClick;
                    }
                    UpdateExtendedStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HoverSelection"]/*' />
        /// <devdoc>
        ///     Determines whether items can be selected by hovering over them with the mouse.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewHoverSelectDescr))
        ]
        public bool HoverSelection {
            get {
                return listViewState[LISTVIEWSTATE_hoverSelection];
            }

            set {
                if (HoverSelection != value) {
                    if (HotTracking && !value) {
                        throw new ArgumentException(SR.ListViewHoverMustBeOnWhenHotTrackingIsOn, "value");
                    }

                    listViewState[LISTVIEWSTATE_hoverSelection] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        internal bool InsertingItemsNatively
        {
            get
            {
                return this.listViewState1[LISTVIEWSTATE1_insertingItemsNatively];
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.InsertionMark"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListViewInsertionMarkDescr))
        ]
        public ListViewInsertionMark InsertionMark {
            get
            {
                if(insertionMark == null) {
                    insertionMark = new ListViewInsertionMark(this);
                }
                return insertionMark;
            }
        }

        private bool ItemCollectionChangedInMouseDown {
            get {
                return this.listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown];
            }
            set {
                this.listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown] = value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Items"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, " + AssemblyRef.SystemDesign,typeof(UITypeEditor)),
        SRDescription(nameof(SR.ListViewItemsDescr)),
        MergableProperty(false)
        ]
        public ListViewItemCollection Items {
            get {
                return listItemCollection;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.LabelEdit"]/*' />
        /// <devdoc>
        ///     Tells whether the EditLabels style is currently set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewLabelEditDescr))
        ]
        public bool LabelEdit {
            get {
                return listViewState[LISTVIEWSTATE_labelEdit];
            }
            set {
                if (LabelEdit != value) {
                    listViewState[LISTVIEWSTATE_labelEdit] = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.LabelWrap"]/*' />
        /// <devdoc>
        ///     Tells whether the LabelWrap style is currently set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ListViewLabelWrapDescr))
        ]
        public bool LabelWrap {
            get {
                return listViewState[LISTVIEWSTATE_labelWrap];
            }
            set {
                if (LabelWrap != value) {
                    listViewState[LISTVIEWSTATE_labelWrap] = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.LargeImageList"]/*' />
        /// <devdoc>
        ///     The Currently set ImageList for Large Icon mode.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ListViewLargeImageListDescr))
        ]
        public ImageList LargeImageList {
            get {
                return imageListLarge;
            }
            set {
                if (value != imageListLarge) {

                    EventHandler recreateHandler = new EventHandler(LargeImageListRecreateHandle);
                    EventHandler disposedHandler = new EventHandler(DetachImageList);
                    EventHandler changeHandler = new EventHandler(LargeImageListChangedHandle);

                    if (imageListLarge != null) {
                        imageListLarge.RecreateHandle -= recreateHandler;
                        imageListLarge.Disposed -= disposedHandler;
                        imageListLarge.ChangeHandle -= changeHandler;
                    }

                    imageListLarge = value;

                    if (value != null) {
                        value.RecreateHandle += recreateHandler;
                        value.Disposed += disposedHandler;
                        value.ChangeHandle += changeHandler;
                    }

                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.LVM_SETIMAGELIST, (IntPtr)NativeMethods.LVSIL_NORMAL, value == null ? IntPtr.Zero: value.Handle);
                        if (AutoArrange && !listViewState1[LISTVIEWSTATE1_disposingImageLists]) {
                            UpdateListViewItemsLocations();
                        }
                    }
                }
            }
        }

        /// <devdoc>
        /// Returns the current LISTVIEWSTATE_handleDestroyed value so that this
        /// value can be accessed from child classes.
        /// </devdoc>
        internal bool ListViewHandleDestroyed {
            get {
                return listViewState[LISTVIEWSTATE_handleDestroyed];
            }
            set {
                listViewState[LISTVIEWSTATE_handleDestroyed] = value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemSorter"]/*' />
        /// <devdoc>
        ///     The sorting comparer for this ListView.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListViewItemSorterDescr))
        ]
        public IComparer ListViewItemSorter {
            get {
                return listItemSorter;
            }
            set {
                if (listItemSorter != value) {
                    listItemSorter = value;

                    if (!this.VirtualMode) {
                        Sort();
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.MultiSelect"]/*' />
        /// <devdoc>
        ///     Tells whether the MultiSelect style is currently set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListViewMultiSelectDescr))
        ]
        public bool MultiSelect {
            get {
                return listViewState[LISTVIEWSTATE_multiSelect];
            }
            set {
                if (MultiSelect != value) {
                    listViewState[LISTVIEWSTATE_multiSelect] = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OwnerDraw"]/*' />
        /// <devdoc>
        ///     Indicates whether the list view items (and sub-items in the Details view) will be
        ///     drawn by the system or the user. This includes the column header when item index = -1.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewOwnerDrawDescr))
        ]
        public bool OwnerDraw {
            get {
                return listViewState[LISTVIEWSTATE_ownerDraw];
            }

            set {
                if (OwnerDraw != value)
                {
                   listViewState[LISTVIEWSTATE_ownerDraw] = value;
                   Invalidate(true);
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RightToLeftLayout"]/*' />
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftLayoutChanged"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
        }


        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Scrollable"]/*' />
        /// <devdoc>
        ///     Tells whether the ScrollBars are visible or not.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListViewScrollableDescr))
        ]
        public bool Scrollable {
            get {
                return listViewState[LISTVIEWSTATE_scrollable];
            }
            set {
                if (Scrollable != value) {
                    listViewState[LISTVIEWSTATE_scrollable] = value;
                    RecreateHandleInternal();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndices"]/*' />
        /// <devdoc>
        ///     The indices of the currently selected list items.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public SelectedIndexCollection SelectedIndices {
            get {
                if (selectedIndexCollection == null) {
                    selectedIndexCollection = new SelectedIndexCollection(this);
                }
                return selectedIndexCollection;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedItems"]/*' />
        /// <devdoc>
        ///     The currently selected list items.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListViewSelectedItemsDescr))
        ]
        public SelectedListViewItemCollection SelectedItems {
            get {
                if (selectedListViewItemCollection == null) {
                    selectedListViewItemCollection = new SelectedListViewItemCollection(this);
                }
                return selectedListViewItemCollection;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ShowGroups"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.ListViewShowGroupsDescr))
        ]
        public bool ShowGroups {
            get {
                return this.listViewState[LISTVIEWSTATE_showGroups];
            }
            set {
                if (value != this.ShowGroups) {
                    this.listViewState[LISTVIEWSTATE_showGroups] = value;
                    if (IsHandleCreated) {
                        UpdateGroupView();
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SmallImageList"]/*' />
        /// <devdoc>
        ///     The currently set SmallIcon image list.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ListViewSmallImageListDescr))
        ]
        public ImageList SmallImageList {
            get {
                return imageListSmall;
            }
            set {
                if (imageListSmall != value) {

                    EventHandler recreateHandler = new EventHandler(SmallImageListRecreateHandle);
                    EventHandler disposedHandler = new EventHandler(DetachImageList);

                    if (imageListSmall != null) {
                        imageListSmall.RecreateHandle -= recreateHandler;
                        imageListSmall.Disposed -= disposedHandler;
                    }
                    imageListSmall = value;
                    if (value != null) {
                        value.RecreateHandle += recreateHandler;
                        value.Disposed += disposedHandler;
                    }

                    if (IsHandleCreated) {

                        SendMessage(NativeMethods.LVM_SETIMAGELIST, (IntPtr)NativeMethods.LVSIL_SMALL, value == null ? IntPtr.Zero: value.Handle);

                        if (this.View == View.SmallIcon) {
                            this.View = View.LargeIcon;
                            this.View = View.SmallIcon;
                        } else if (!listViewState1[LISTVIEWSTATE1_disposingImageLists]) {
                            UpdateListViewItemsLocations();
                        }

                        if (this.View == View.Details) {
                            this.Invalidate(true /*invalidateChildren*/);
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ShowItemToolTips"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ListViewShowItemToolTipsDescr))
        ]
        public bool ShowItemToolTips {
            get {
                return listViewState[LISTVIEWSTATE_showItemToolTips];
            }
            set {
                if (ShowItemToolTips != value) {
                    listViewState[LISTVIEWSTATE_showItemToolTips] = value;
                    RecreateHandleInternal();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Sorting"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(SortOrder.None),
        SRDescription(nameof(SR.ListViewSortingDescr))
        ]
        public SortOrder Sorting {
            get {
                return sorting;
            }
            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SortOrder.None, (int)SortOrder.Descending))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(SortOrder));
                }
                if (sorting != value) {
                    sorting = value;
                    if (this.View == View.LargeIcon || this.View == View.SmallIcon) {
                        if (listItemSorter == null) {
                            listItemSorter = new IconComparer(sorting);
                        }
                        else if (listItemSorter is IconComparer) {
                           ((IconComparer)listItemSorter).SortOrder = sorting;
                        }

                    } else if (value == SortOrder.None) {
                        listItemSorter = null;
                    }

                    // If we're changing to No Sorting, no need to recreate the handle
                    // because none of the existing items need to be rearranged.
                    if (value == SortOrder.None)
                        UpdateStyles();
                    else
                        RecreateHandleInternal();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.StateImageList"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ListViewStateImageListDescr))
        ]
        public ImageList StateImageList {
            get {
                return imageListState;
            }
            set {
                if (this.UseCompatibleStateImageBehavior) {
                    if (imageListState != value) {
                        
                        EventHandler recreateHandler = new EventHandler(StateImageListRecreateHandle);
                        EventHandler disposedHandler = new EventHandler(DetachImageList);

                        if (imageListState != null) {
                            imageListState.RecreateHandle -= recreateHandler;
                            imageListState.Disposed -= disposedHandler;
                        }
                        imageListState = value;
                        if (value != null) {
                            value.RecreateHandle += recreateHandler;
                            value.Disposed += disposedHandler;
                        }
                        
                        if (IsHandleCreated)
                            SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, value == null ? IntPtr.Zero: value.Handle);
                    }
                } else {
                    if (imageListState != value) {

                        EventHandler recreateHandler = new EventHandler(StateImageListRecreateHandle);
                        EventHandler disposedHandler = new EventHandler(DetachImageList);

                        if (imageListState != null) {
                            imageListState.RecreateHandle -= recreateHandler;
                            imageListState.Disposed -= disposedHandler;
                        }

                        if (this.IsHandleCreated && imageListState != null && this.CheckBoxes) {
                            // 
                            // If CheckBoxes are set to true, then we will have to recreate the handle.
                            // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native listView destroys
                            // the state imageList.
                            // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
                            // So we make the native listView forget about its StateImageList just before we recreate the handle.
                            SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, IntPtr.Zero);
                        }

                        imageListState = value;

                        if (value != null) {
                            value.RecreateHandle += recreateHandler;
                            value.Disposed += disposedHandler;
                        }

                        if (IsHandleCreated) {
                            if (CheckBoxes) {
                                // need to recreate to get the new images pushed in.
                                RecreateHandleInternal();
                            } else {
                                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, (imageListState == null || imageListState.Images.Count == 0) ? IntPtr.Zero : imageListState.Handle);
                            }

                            // Comctl should handle auto-arrange for us, but doesn't
                            if (!listViewState1[LISTVIEWSTATE1_disposingImageLists]) {
                                UpdateListViewItemsLocations();
                            }
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Text"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
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

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.TextChanged"]/*' />
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

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.TileSize"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
            SRCategory(nameof(SR.CatAppearance)),
            Browsable(true),
            SRDescription(nameof(SR.ListViewTileSizeDescr)),
        ]
        public Size TileSize {
            get {
                if (tileSize.IsEmpty) {
                    if (this.IsHandleCreated) {
                        // Get the default value from the ListView
                        //
                        NativeMethods.LVTILEVIEWINFO tileViewInfo = new NativeMethods.LVTILEVIEWINFO();
                        tileViewInfo.dwMask = NativeMethods.LVTVIM_TILESIZE;
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_GETTILEVIEWINFO, 0, tileViewInfo);
                        return new Size(tileViewInfo.sizeTile.cx, tileViewInfo.sizeTile.cy);
                    } else {
                        return Size.Empty;
                    }
                } else {
                    return tileSize;
                }
            }
            set {
                if (tileSize != value) {
                    if (value.IsEmpty || value.Height <= 0 || value.Width <= 0) {
                        throw new ArgumentOutOfRangeException("TileSize", SR.ListViewTileSizeMustBePositive);
                    }

                    tileSize = value;
                    if (this.IsHandleCreated) {
                        NativeMethods.LVTILEVIEWINFO tileViewInfo = new NativeMethods.LVTILEVIEWINFO();
                        tileViewInfo.dwMask = NativeMethods.LVTVIM_TILESIZE;
                        tileViewInfo.dwFlags = NativeMethods.LVTVIF_FIXEDSIZE;
                        tileViewInfo.sizeTile = new NativeMethods.SIZE(tileSize.Width, tileSize.Height);
                        bool retval = UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETTILEVIEWINFO, 0, tileViewInfo);
                        Debug.Assert(retval, "LVM_SETTILEVIEWINFO failed");
                        if (this.AutoArrange) {
                            this.UpdateListViewItemsLocations();
                        }
                    }
                }
            }
        }

        private bool ShouldSerializeTileSize() {
            return !this.tileSize.Equals(Size.Empty);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.TopItem"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListViewTopItemDescr))
        ]
        public ListViewItem TopItem {
            get {
                if (viewStyle == View.LargeIcon || viewStyle == View.SmallIcon || viewStyle == View.Tile)
                    throw new InvalidOperationException(SR.ListViewGetTopItem);

                if (!IsHandleCreated) {
                    if (Items.Count > 0) {
                        return Items[0];
                    }
                    else {
                        return null;
                    }
                }
                topIndex = unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETTOPINDEX, 0, 0));
                if (topIndex >= 0 && topIndex < Items.Count)
                    return Items[topIndex];

                return null;
            }
            set {
                if (viewStyle == View.LargeIcon || viewStyle == View.SmallIcon || viewStyle == View.Tile)
                    throw new InvalidOperationException(SR.ListViewSetTopItem);

                if (value == null)
                    return;
                if (value.ListView != this)
                    return;
                if (!IsHandleCreated) {
                    CreateHandle();
                }
                if (value == TopItem)
                    return;
                EnsureVisible(value.Index);
                ListViewItem topItem = TopItem;

                if ((topItem == null) && (topIndex == Items.Count)) // 
                {                                                   // There's a 
                    topItem = value;                                // a single item.  Result of the 
                    if (Scrollable)                                 // message is the number of items in the list rather than an index of an item in the list.
                    {                                               // This causes TopItem to return null.  A side issue is that EnsureVisible doesn't do too well
                        EnsureVisible(0);                           // here either, because it causes the listview to go blank rather than displaying anything useful.
                        Scroll(0, value.Index);                     // To work around this, we force the listbox to display the first item, then scroll down to the item
                    }                                               // user is setting as the top item.
                    return;                                         //
                }                                                   // 

                if (value.Index == topItem.Index)
                    return;
                if (Scrollable)
                    Scroll(topItem.Index, value.Index);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.UseCompatibleStateImageBehavior"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced),
        DefaultValue(true)
        ]
        public bool UseCompatibleStateImageBehavior {
            get {
                return this.listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior];
            }
            set
            {
                this.listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior] = value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.View"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(View.LargeIcon),
        SRDescription(nameof(SR.ListViewViewDescr))
        ]
        public View View {
            get {
                return viewStyle;
            }
            set {
                if (value == View.Tile && this.CheckBoxes) {
                    throw new NotSupportedException(SR.ListViewTileViewDoesNotSupportCheckBoxes);
                }

                this.FlipViewToLargeIconAndSmallIcon = false;

                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)View.LargeIcon, (int)View.Tile))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(View));
                }

                if (value == View.Tile && VirtualMode) {
                    throw new NotSupportedException(SR.ListViewCantSetViewToTileViewInVirtualMode);
                }

                if (viewStyle != value) {
                    viewStyle = value;
                    if (IsHandleCreated && ComctlSupportsVisualStyles) {
                        SendMessage(NativeMethods.LVM_SETVIEW,(int)viewStyle,0);
                        UpdateGroupView();

                        // if we switched to Tile view we should update the win32 list view tile view info
                        if (viewStyle == View.Tile) {
                            UpdateTileView();
                        }
                    }
                    else {
                        UpdateStyles();
                    }

                    UpdateListViewItemsLocations();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.VirtualListSize"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ListViewVirtualListSizeDescr))
        ]
        public int VirtualListSize {
            get {
                return this.virtualListSize;
            }
            set {
                if (value < 0)
                    throw new System.ArgumentException(string.Format(SR.ListViewVirtualListSizeInvalidArgument, "value", (value.ToString(CultureInfo.CurrentCulture))));
                if (value == virtualListSize)
                    return;
                bool keepTopItem = this.IsHandleCreated && VirtualMode && this.View == View.Details && !this.DesignMode;
                int topIndex = -1;
                if (keepTopItem) {
                    topIndex = unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETTOPINDEX, 0, 0));
                }

                virtualListSize = value;

                if (IsHandleCreated && VirtualMode && !DesignMode)
                    SendMessage(NativeMethods.LVM_SETITEMCOUNT, virtualListSize, 0);

                if (keepTopItem) {
                    topIndex = Math.Min(topIndex, this.VirtualListSize - 1);
                    // After setting the virtual list size ComCtl makes the first item the top item.
                    // So we set the top item only if it wasn't the first item to begin with.
                    if (topIndex > 0) {
                        ListViewItem lvItem = this.Items[topIndex];
                        this.TopItem = lvItem;
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.VirtualMode"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ListViewVirtualModeDescr))
        ]
        public bool VirtualMode {
            get {
                return listViewState[LISTVIEWSTATE_virtualMode];
            }
            set {
                if (value == VirtualMode)
                    return;

                if (value && Items.Count > 0)
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoItems);

                if (value && CheckedItems.Count > 0) {
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoCheckedItems);
                }

                if (value && SelectedItems.Count > 0) {
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoSelectedItems);
                }

                // Tile view does not work w/ VirtualMode.
                if (value && View == View.Tile) {
                    throw new NotSupportedException(SR.ListViewCantSetVirtualModeWhenInTileView);
                }

                listViewState[LISTVIEWSTATE_virtualMode] = value;

                RecreateHandleInternal();
            }
        }


        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.AfterLabelEdit"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewAfterLabelEditDescr))]
        public event LabelEditEventHandler AfterLabelEdit {
            add {
                onAfterLabelEdit += value;
            }
            remove {
                onAfterLabelEdit -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BeforeLabelEdit"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewBeforeLabelEditDescr))]
        public event LabelEditEventHandler BeforeLabelEdit {
            add {
                onBeforeLabelEdit += value;
            }
            remove {
                onBeforeLabelEdit -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CacheVirtualItems"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewCacheVirtualItemsEventDescr))]
        public event CacheVirtualItemsEventHandler CacheVirtualItems {
            add {
                Events.AddHandler(EVENT_CACHEVIRTUALITEMS, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CACHEVIRTUALITEMS, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnClick"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewColumnClickDescr))]
        public event ColumnClickEventHandler ColumnClick {
            add {
                onColumnClick += value;
            }
            remove {
                onColumnClick -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnReordered"]/*' />
        /// <devdoc>
        ///     Tell the user that the column headers are being rearranged
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListViewColumnReorderedDscr))]
        public event ColumnReorderedEventHandler ColumnReordered {
            add {
                Events.AddHandler(EVENT_COLUMNREORDERED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_COLUMNREORDERED, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnWidthChanged"]/*' />
        /// <devdoc>
        ///     Tell the user that the column width changed
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListViewColumnWidthChangedDscr))]
        public event ColumnWidthChangedEventHandler ColumnWidthChanged {
            add {
                Events.AddHandler(EVENT_COLUMNWIDTHCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_COLUMNWIDTHCHANGED, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnWidthChanging"]/*' />
        /// <devdoc>
        ///     Tell the user that the column width is being changed
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListViewColumnWidthChangingDscr))]
        public event ColumnWidthChangingEventHandler ColumnWidthChanging {
            add {
                Events.AddHandler(EVENT_COLUMNWIDTHCHANGING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_COLUMNWIDTHCHANGING, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DrawColumnHeader"]/*' />
        /// <devdoc>
        ///    <para>Fires in owner draw + Details mode when a column header needs to be drawn.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewDrawColumnHeaderEventDescr))]
        public event DrawListViewColumnHeaderEventHandler DrawColumnHeader {
            add {
                Events.AddHandler(EVENT_DRAWCOLUMNHEADER, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DRAWCOLUMNHEADER, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DrawItem"]/*' />
        /// <devdoc>
        ///    <para>Fires in owner draw mode when a ListView item needs to be drawn.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewDrawItemEventDescr))]
        public event DrawListViewItemEventHandler DrawItem {
            add {
                Events.AddHandler(EVENT_DRAWITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DRAWITEM, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DrawSubItem"]/*' />
        /// <devdoc>
        ///      <para>Fires in owner draw mode and Details view when a ListView sub-item needs to be drawn.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewDrawSubItemEventDescr))]
        public event DrawListViewSubItemEventHandler DrawSubItem {
            add {
                Events.AddHandler(EVENT_DRAWSUBITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DRAWSUBITEM, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ItemActivate"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewItemClickDescr))]
        public event EventHandler ItemActivate {
            add {
                onItemActivate += value;
            }
            remove {
                onItemActivate -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ItemCheck"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.CheckedListBoxItemCheckDescr))]
        public event ItemCheckEventHandler ItemCheck {
            add {
                onItemCheck += value;
            }
            remove {
                onItemCheck -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ItemChecked"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewItemCheckedDescr))]
        public event ItemCheckedEventHandler ItemChecked {
            add {
                onItemChecked += value;
            }
            remove {
                onItemChecked -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ItemDrag"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewItemDragDescr))]
        public event ItemDragEventHandler ItemDrag {
            add {
                onItemDrag += value;
            }
            remove {
                onItemDrag -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.NodeMouseHover"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewItemMouseHoverDescr))]
        public event ListViewItemMouseHoverEventHandler ItemMouseHover {
            add {
                onItemMouseHover += value;
            }
            remove {
                onItemMouseHover -= value;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewItemSelectionChangedDescr))]
        public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged {
            add {
                Events.AddHandler(EVENT_ITEMSELECTIONCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_ITEMSELECTIONCHANGED, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Padding"]/*' />
        /// <devdoc>
        ///    <para>
        ///    <para>[To be supplied.]</para>
        ///    </para>
        /// </devdoc>
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

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnPaint"]/*' />
        /// <devdoc>
        ///     ListView Onpaint.
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

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RetrieveVirtualItem"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewRetrieveVirtualItemEventDescr))]
        public event RetrieveVirtualItemEventHandler RetrieveVirtualItem {
            add {
                Events.AddHandler(EVENT_RETRIEVEVIRTUALITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RETRIEVEVIRTUALITEM, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SearchForVirtualItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewSearchForVirtualItemDescr))]
        public event SearchForVirtualItemEventHandler SearchForVirtualItem {
            add {
                Events.AddHandler(EVENT_SEARCHFORVIRTUALITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SEARCHFORVIRTUALITEM, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexChanged"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewSelectedIndexChangedDescr))]
        public event EventHandler SelectedIndexChanged {
            add {
                Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.VirtualItemsSelectionRangeChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ListViewVirtualItemsSelectionRangeChangedDescr))]
        public event ListViewVirtualItemsSelectionRangeChangedEventHandler VirtualItemsSelectionRangeChanged {
            add {
                Events.AddHandler(EVENT_VIRTUALITEMSSELECTIONRANGECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_VIRTUALITEMSSELECTIONRANGECHANGED, value);
            }
        }


        /// <devdoc>
        ///     Called to add any delayed update items we have to the list view.  We do this because
        ///     we have optimnized the case where a user is only adding items within a beginupdate/endupdate
        ///     block.  If they do any other operations (get the count, remove, insert, etc.), we push in the
        ///     cached up items first, then do the requested operation.  This keeps it simple so we don't have to
        ///     try to maintain parellel state of the cache during a begin update end update.
        /// </devdoc>
        private void ApplyUpdateCachedItems() {
            // first check if there is a delayed update array
            //
            ArrayList newItems = (ArrayList)Properties.GetObject(PropDelayedUpdateItems);
            if (newItems != null) {
                // if there is, clear it and push the items in.
                //
                Properties.SetObject(PropDelayedUpdateItems, null);
                ListViewItem[] items = (ListViewItem[])newItems.ToArray(typeof(ListViewItem));
                if (items.Length > 0) {
                    InsertItems(itemCount, items, false /*checkHosting*/);
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ArrangeIcons"]/*' />
        /// <devdoc>
        ///     In Large Icon or Small Icon view, arranges the items according to one
        ///     of the following behaviors:
        /// </devdoc>
        public void ArrangeIcons(ListViewAlignment value) {
            // LVM_ARRANGE only work in SmallIcon view
            if (viewStyle != View.SmallIcon) return;

            switch ((int)value) {
                case NativeMethods.LVA_DEFAULT:
                case NativeMethods.LVA_ALIGNLEFT:
                case NativeMethods.LVA_ALIGNTOP:
                case NativeMethods.LVA_SNAPTOGRID:
                    if (IsHandleCreated) {
                        UnsafeNativeMethods.PostMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_ARRANGE, (int) value, 0);
                    }
                    break;

                default:
                    throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                              "value",
                                                              ((value).ToString())));
            }

            if (!VirtualMode && sorting != SortOrder.None) {
                Sort();
            }

        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ArrangeIcons1"]/*' />
        /// <devdoc>
        ///     In Large Icon or Small Icon view, arranges items according to the ListView's
        ///     current alignment style.
        /// </devdoc>
        public void ArrangeIcons() {
            ArrangeIcons((ListViewAlignment)NativeMethods.LVA_DEFAULT);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.AutoResizeColumns"]/*' />
        public void AutoResizeColumns(ColumnHeaderAutoResizeStyle headerAutoResize) {
            if (!this.IsHandleCreated) {
                this.CreateHandle();
            }
            UpdateColumnWidths(headerAutoResize);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.AutoResizeColumn"]/*' />
        public void AutoResizeColumn(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize) {
            if (!this.IsHandleCreated) {
                this.CreateHandle();
            }
            SetColumnWidth(columnIndex, headerAutoResize);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.BeginUpdate"]/*' />
        /// <devdoc>
        ///     Prevents the ListView from redrawing itself until EndUpdate is called.
        ///     Calling this method before individually adding or removing a large number of Items
        ///     will improve performance and reduce flicker on the ListView as items are
        ///     being updated.  Always call EndUpdate immediately after the last item is updated.
        /// </devdoc>
        public void BeginUpdate() {
            BeginUpdateInternal();

            // if this is the first BeginUpdate call, push an ArrayList into the PropertyStore so
            // we can cache up any items that have been added while this is active.
            //
            if (updateCounter++ == 0 && null == Properties.GetObject(PropDelayedUpdateItems)) {
                Properties.SetObject(PropDelayedUpdateItems, new ArrayList());
            }
        }

        internal void CacheSelectedStateForItem(ListViewItem lvi, bool selected) {
            if (selected) {
                if (this.savedSelectedItems == null) {
                    this.savedSelectedItems = new List<ListViewItem>();
                }
                if (!this.savedSelectedItems.Contains(lvi)) {
                    this.savedSelectedItems.Add(lvi);
                }
            } else {
                if (this.savedSelectedItems != null && this.savedSelectedItems.Contains(lvi)) {
                    this.savedSelectedItems.Remove(lvi);
                }
            }
        }

#if DEBUG
        private void CheckDisplayIndices() {
            // sanity check
            // all the column headers should have a displayIndex between 0 and this.Columns.Count - 1;
            // DisplayIndex should be different for different column headers
            int sumOfDisplayIndices = 0;
            for (int i = 0; i < this.Columns.Count; i ++) {
                sumOfDisplayIndices += this.Columns[i].DisplayIndex;
                Debug.Assert(this.Columns[i].DisplayIndex > -1 && this.Columns[i].DisplayIndex < this.Columns.Count, "display indices out of whack");
            }
            int colsCount = this.Columns.Count;
            Debug.Assert(sumOfDisplayIndices == (colsCount - 1) * colsCount / 2, "display indices out of whack");
        }
#endif

        private void CleanPreviousBackgroundImageFiles() {
            if (this.bkImgFileNames == null) {
                return;
            }

            // 

            FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
            fiop.Assert();
            try {
                System.IO.FileInfo fi;
                for (int i = 0; i <= this.bkImgFileNamesCount; i ++) {
                    fi = new System.IO.FileInfo(this.bkImgFileNames[i]);
                    if (fi.Exists) {
                        // 
                        // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                        // I could not find any resources which explain in detail when the IImgCtx objects
                        // release the temporary file. So if we get a FileIO when we delete the temporary file
                        // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                        try {
                            fi.Delete();
                        } catch (System.IO.IOException){}
                    }
                }
            } finally {
                System.Security.PermissionSet.RevertAssert();
            }

            this.bkImgFileNames = null;
            this.bkImgFileNamesCount = -1;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Clear"]/*' />
        /// <devdoc>
        ///     Removes all items and columns from the ListView.
        /// </devdoc>
        public void Clear() {
            Items.Clear();
            Columns.Clear();
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CompareFunc"]/*' />
        /// <devdoc>
        ///      This is the sorting callback function called by the system ListView control.
        /// </devdoc>
        /// <internalonly/>
        private int CompareFunc(IntPtr lparam1, IntPtr lparam2, IntPtr lparamSort) {

            Debug.Assert(listItemSorter != null, "null sorter!");
            if (listItemSorter != null) {
                return listItemSorter.Compare(listItemsTable[(int)lparam1], listItemsTable[(int)lparam2]);
            }
            else {
                return 0;
            }
        }

        private int CompensateColumnHeaderResize(Message m, bool columnResizeCancelled) {
            if (this.ComctlSupportsVisualStyles &&
                this.View == View.Details &&
                !columnResizeCancelled &&
                this.Items.Count > 0) {
                NativeMethods.NMHEADER header = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));

                return CompensateColumnHeaderResize(header.iItem, columnResizeCancelled);
            } else {
                return 0;
            }
        }

        private int CompensateColumnHeaderResize(int columnIndex, bool columnResizeCancelled) {
            // We need to compensate padding only when ComCtl60 is loaded.
            // We need to compensate padding only if the list view is in Details mode.
            // We need to compensate padding only if the user did not cancel ColumnWidthChanging event.
            // We need to compensate padding only if the list view contains items.
            // We need to compensate padding if the user resizes the first column.
            // If the list view has a small image list then:
            //  1. if there is a list view item w/ ImageIndex > -1 then we don't have to resize the first column.
            //  2. Otherwise, we need to add 18 pixels.
            // If the list view does not have a small image list then we need to add 2 pixels.

            if (this.ComctlSupportsVisualStyles &&
                this.View == View.Details &&
                !columnResizeCancelled &&
                this.Items.Count > 0) {
                // The user resized the first column.
                if (columnIndex == 0) {
                    ColumnHeader col = (this.columnHeaders != null && this.columnHeaders.Length > 0) ? this.columnHeaders[0] : null;
                    if (col != null) {
                        if (this.SmallImageList == null) {
                            return 2;
                        } else {
                            // If the list view contains an item w/ a non-negative ImageIndex then we don't need to
                            // add extra padding.
                            bool addPadding = true;
                            for (int i = 0; i < this.Items.Count; i ++) {
                                if (this.Items[i].ImageIndexer.ActualIndex > -1) {
                                    addPadding = false;
                                    break;
                                }
                            }

                            if (addPadding) {
                                // 18 = 16 + 2.
                                // 16 = size of small image list.
                                // 2 is the padding we add when there is no small image list.
                                return 18;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void CreateHandle() {
            if (!RecreatingHandle) {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                        
                try {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_LISTVIEW_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);
                }
                finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();

            // image location
            if (this.BackgroundImage != null)
                SetBackgroundImage();
        }

        //



        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CustomDraw"]/*' />
        /// <devdoc>
        ///     Handles custom drawing of list items - for individual item font/color changes.
        ///
        ///        If OwnerDraw is true, we fire the OnDrawItem and OnDrawSubItem (in Details view)
        ///        events and let the user do the drawing.
        /// </devdoc>
        /// <internalonly/>
        unsafe void CustomDraw(ref Message m) {

            bool dontmess = false;
            bool itemDrawDefault = false;

            try
            {
                NativeMethods.NMLVCUSTOMDRAW* nmcd = (NativeMethods.NMLVCUSTOMDRAW*)m.LParam;
                // Find out which stage we're drawing
                switch (nmcd->nmcd.dwDrawStage)
                {
                    case NativeMethods.CDDS_PREPAINT:
                        if (OwnerDraw)
                        {
                            m.Result = (IntPtr)(NativeMethods.CDRF_NOTIFYITEMDRAW);
                            return;
                        }
                        // We want custom draw for this paint cycle
                        m.Result = (IntPtr)(NativeMethods.CDRF_NOTIFYSUBITEMDRAW | NativeMethods.CDRF_NEWFONT);
                        // refresh the cache of the current color & font settings for this paint cycle
                        odCacheBackColor = this.BackColor;
                        odCacheForeColor = this.ForeColor;
                        odCacheFont = this.Font;
                        odCacheFontHandle = this.FontHandle;

                        // If preparing to paint a group item, make sure its bolded.
                        if (nmcd->dwItemType == NativeMethods.LVCDI_GROUP)
                        {
                            if (odCacheFontHandleWrapper != null)
                            {
                                odCacheFontHandleWrapper.Dispose();
                            }
                            odCacheFont = new Font(odCacheFont, FontStyle.Bold);
                            odCacheFontHandleWrapper = new Control.FontHandleWrapper(odCacheFont);
                            odCacheFontHandle = odCacheFontHandleWrapper.Handle;
                            SafeNativeMethods.SelectObject(new HandleRef(nmcd->nmcd, nmcd->nmcd.hdc), new HandleRef(odCacheFontHandleWrapper, odCacheFontHandleWrapper.Handle));
                            m.Result = (IntPtr)NativeMethods.CDRF_NEWFONT;
                        }
                        return;

                    //We have to return a NOTIFYSUBITEMDRAW (called NOTIFYSUBITEMREDRAW in the docs) here to
                    //get it to enter "change all subitems instead of whole rows" mode.

                    //HOWEVER... we only want to do this for report styles...

                    case NativeMethods.CDDS_ITEMPREPAINT:

                        int itemIndex = (int)nmcd->nmcd.dwItemSpec;
                        // The following call silently returns Rectangle.Empty if no corresponding
                        // item was found. We do this because the native listview, under some circumstances, seems
                        // to send spurious custom draw notifications. The check below does the rest.
                        Rectangle itemBounds = GetItemRectOrEmpty(itemIndex);

                        if (!ClientRectangle.IntersectsWith(itemBounds))
                        {
                            // we don't need to bother painting this one.
                            return;
                        }

                        // If OwnerDraw is true, fire the onDrawItem event.
                        if (OwnerDraw)
                        {

                            Graphics g = Graphics.FromHdcInternal(nmcd->nmcd.hdc);

#if DEBUGGING
                            Rectangle r = itemBounds;
                            Rectangle r2 = Rectangle.FromLTRB(nmcd->nmcd.rc.left, nmcd->nmcd.rc.top, nmcd->nmcd.rc.right, nmcd->nmcd.rc.bottom);
                            Debug.WriteLine("ClipBounds      : l {0} t {1} r {2} b {3}", g.ClipBounds.Left, g.ClipBounds.Top, g.ClipBounds.Right, g.ClipBounds.Bottom);
                            Debug.WriteLine("Rect (Send Msg) : l {0} t {1} r {2} b {3}", r.Left, r.Top, r.Right, r.Bottom);
                            Debug.WriteLine("Rect (NM)       : l {0} t {1} r {2} b {3}", r2.Left, r2.Top, r2.Right, r2.Bottom);
#endif
                            DrawListViewItemEventArgs e = null;
                            try
                            {
                                e = new DrawListViewItemEventArgs(g,
                                       Items[(int)nmcd->nmcd.dwItemSpec],
                                       itemBounds,
                                       (int)nmcd->nmcd.dwItemSpec,
                                       (ListViewItemStates)(nmcd->nmcd.uItemState));

                                OnDrawItem(e);
                            }
                            finally
                            {
                                g.Dispose();
                            }

                            itemDrawDefault = e.DrawDefault;

                            // For the Details view, we send a SKIPDEFAULT when we get a sub-item drawing notification.
                            // For other view styles, we do it here.
                            if (viewStyle == View.Details)
                            {
                                m.Result = (IntPtr)(NativeMethods.CDRF_NOTIFYSUBITEMDRAW);
                            }
                            else
                            {
                                if (!e.DrawDefault)
                                {
                                    m.Result = (IntPtr)(NativeMethods.CDRF_SKIPDEFAULT);
                                }
                            }

                            if (!e.DrawDefault)
                            {
                                return;   // skip our regular drawing code
                            }
                        }

                        if (viewStyle == View.Details || viewStyle == View.Tile)
                        {
                            m.Result = (IntPtr)(NativeMethods.CDRF_NOTIFYSUBITEMDRAW | NativeMethods.CDRF_NEWFONT);
                            dontmess = true; // don't mess with our return value!

                            //ITEMPREPAINT is used to work out the rect for the first column!!! GAH!!!
                            //(which means we can't just do our color/font work on SUBITEM|ITEM_PREPAINT)
                            //so fall through... and tell the end of SUBITEM|ITEM_PREPAINT not to mess
                            //with our return value...

                        }

                        //If it's not a report, we fall through and change the main item's styles

                        goto case (NativeMethods.CDDS_SUBITEM | NativeMethods.CDDS_ITEMPREPAINT);

                    case (NativeMethods.CDDS_SUBITEM | NativeMethods.CDDS_ITEMPREPAINT):

                        itemIndex = (int)nmcd->nmcd.dwItemSpec;
                        // The following call silently returns Rectangle.Empty if no corresponding
                        // item was found. We do this because the native listview, under some circumstances, seems
                        // to send spurious custom draw notifications. The check below does the rest.
                        itemBounds = GetItemRectOrEmpty(itemIndex);

                        if (!ClientRectangle.IntersectsWith(itemBounds))
                        {
                            // we don't need to bother painting this one.
                            return;
                        }

                        // If OwnerDraw is true, fire the onDrawSubItem event.
                        if (OwnerDraw && !itemDrawDefault)
                        {

                            Graphics g = Graphics.FromHdcInternal(nmcd->nmcd.hdc);
                            DrawListViewSubItemEventArgs e = null;

                            // by default, we want to skip the customDrawCode
                            bool skipCustomDrawCode = true;
                            try
                            {

                                //The ListView will send notifications for every column, even if no
                                //corresponding subitem exists for a particular item. We shouldn't
                                //fire events in such cases.
                                if (nmcd->iSubItem < Items[itemIndex].SubItems.Count)
                                {
                                    Rectangle subItemBounds = GetSubItemRect(itemIndex, nmcd->iSubItem);

                                    // For the first sub-item, the rectangle corresponds to the whole item.
                                    // We need to handle this case separately.
                                    if (nmcd->iSubItem == 0 && Items[itemIndex].SubItems.Count > 1)
                                    {
                                        // Use the width for the first column header.
                                        subItemBounds.Width = this.columnHeaders[0].Width;
                                    }

                                    if (this.ClientRectangle.IntersectsWith(subItemBounds))
                                    {
                                        e = new DrawListViewSubItemEventArgs(g,
                                                  subItemBounds,
                                                  Items[itemIndex],
                                                  Items[itemIndex].SubItems[nmcd->iSubItem],
                                                  itemIndex,
                                                  nmcd->iSubItem,
                                                  columnHeaders[nmcd->iSubItem],
                                                  (ListViewItemStates)(nmcd->nmcd.uItemState));
                                        OnDrawSubItem(e);

                                        // the customer still wants to draw the default.
                                        // Don't skip the custom draw code then
                                        skipCustomDrawCode = !e.DrawDefault;
                                    }
                                }
                            }
                            finally
                            {
                                g.Dispose();
                            }

                            if (skipCustomDrawCode)
                            {
                                m.Result = (IntPtr)(NativeMethods.CDRF_SKIPDEFAULT);
                                return; // skip our custom draw code
                            }
                        }

                        // get the node
                        ListViewItem item = Items[(int)(nmcd->nmcd.dwItemSpec)];
                        // if we're doing the whole row in one style, change our result!
                        if (dontmess && item.UseItemStyleForSubItems)
                        {
                            m.Result = (IntPtr)NativeMethods.CDRF_NEWFONT;
                        }
                        Debug.Assert(item != null, "Item was null in ITEMPREPAINT");

                        int state = nmcd->nmcd.uItemState;
                        // There is a known and documented problem in the ListView winctl control -
                        // if the LVS_SHOWSELALWAYS style is set, then the item state will have
                        // the CDIS_SELECTED bit set for all items. So we need to verify with the
                        // real item state to be sure.
                        if (!HideSelection)
                        {
                            int realState = GetItemState((int)(nmcd->nmcd.dwItemSpec));
                            if ((realState & NativeMethods.LVIS_SELECTED) == 0)
                            {
                                state &= ~NativeMethods.CDIS_SELECTED;
                            }
                        }

                        // subitem is invalid if the flag isn't set -- and we also use this code in
                        // cases where subitems aren't visible (ie. non-Details modes), so if subitem
                        // is invalid, point it at the main item's render info

                        int subitem = ((nmcd->nmcd.dwDrawStage & NativeMethods.CDDS_SUBITEM) != 0) ? nmcd->iSubItem : 0;

                        // Work out the style in which to render this item
                        //
                        Font subItemFont = null;
                        Color subItemForeColor = Color.Empty;
                        Color subItemBackColor = Color.Empty;
                        bool haveRenderInfo = false;
                        bool disposeSubItemFont = false;
                        if (item != null && subitem < item.SubItems.Count)
                        {
                            haveRenderInfo = true;
                            if (subitem == 0 && (state & NativeMethods.CDIS_HOT) != 0 && HotTracking)
                            {
                                disposeSubItemFont = true;
                                subItemFont = new Font(item.SubItems[0].Font, FontStyle.Underline);
                            }
                            else
                            {
                                subItemFont = item.SubItems[subitem].Font;
                            }

                            if (subitem > 0 || (state & (NativeMethods.CDIS_SELECTED | NativeMethods.CDIS_GRAYED | NativeMethods.CDIS_HOT | NativeMethods.CDIS_DISABLED)) == 0)
                            {
                                // we only propogate colors if we're displaying things normally
                                // the user can override this method to do all kinds of other bad things if they
                                // want to though - but we don't support that.
                                subItemForeColor = item.SubItems[subitem].ForeColor;
                                subItemBackColor = item.SubItems[subitem].BackColor;
                            }
                        }

                        // We always have to set font and color data, because of comctl design

                        //




                        Color riFore = Color.Empty;
                        Color riBack = Color.Empty;

                        if (haveRenderInfo)
                        {
                            riFore = subItemForeColor;
                            riBack = subItemBackColor;
                        }

                        bool changeColor = true;
                        if (!Enabled)
                        {
                            changeColor = false;
                        }
                        else if ((activation == ItemActivation.OneClick)
                              || (activation == ItemActivation.TwoClick))
                        {
                            if ((state & (NativeMethods.CDIS_SELECTED
                                        | NativeMethods.CDIS_GRAYED
                                        | NativeMethods.CDIS_HOT
                                        | NativeMethods.CDIS_DISABLED)) != 0)
                            {
                                changeColor = false;
                            }
                        }

                        if (changeColor)
                        {
                            if (!haveRenderInfo || riFore.IsEmpty)
                            {
                                nmcd->clrText = ColorTranslator.ToWin32(odCacheForeColor);
                            }
                            else
                            {
                                nmcd->clrText = ColorTranslator.ToWin32(riFore);
                            }

                            // Work-around for a comctl quirk where,
                            // if clrText is the same as SystemColors.HotTrack,
                            // the subitem's color is not changed to nmcd->clrText.
                            //
                            // Try to tweak the blue component of clrText first, then green, then red.
                            // Basically, if the color component is 0xFF, subtract 1 from it
                            // (adding 1 will overflow), else add 1 to it. If the color component is 0,
                            // skip it and go to the next color (unless it is our last option).
                            if (nmcd->clrText == ColorTranslator.ToWin32(SystemColors.HotTrack))
                            {
                                int totalshift = 0;
                                bool clrAdjusted = false;
                                int mask = 0xFF0000;
                                do
                                {
                                    int C = nmcd->clrText & mask;
                                    if (C != 0 || (mask == 0x0000FF)) // The color is not 0
                                    // or this is the last option
                                    {
                                        int n = 16 - totalshift;
                                        // Make sure the value doesn't overflow
                                        if (C == mask)
                                        {
                                            C = ((C >> n) - 1) << n;
                                        }
                                        else
                                        {
                                            C = ((C >> n) + 1) << n;
                                        }
                                        // Copy the adjustment into nmcd->clrText
                                        nmcd->clrText = (nmcd->clrText & (~mask)) | C;
                                        clrAdjusted = true;
                                    }
                                    else
                                    {
                                        mask >>= 8; // Try the next color.
                                        // We try adjusting Blue, Green, Red in that order,
                                        // since 0x0000FF is the most likely value of
                                        // SystemColors.HotTrack
                                        totalshift += 8;
                                    }
                                } while (!clrAdjusted);
                            }

                            if (!haveRenderInfo || riBack.IsEmpty)
                            {
                                nmcd->clrTextBk = ColorTranslator.ToWin32(odCacheBackColor);
                            }
                            else
                            {
                                nmcd->clrTextBk = ColorTranslator.ToWin32(riBack);
                            }
                        }

                        if (!haveRenderInfo || subItemFont == null)
                        {
                            // safety net code just in case
                            if (odCacheFont != null)
                            {
                                SafeNativeMethods.SelectObject(new HandleRef(nmcd->nmcd, nmcd->nmcd.hdc), new HandleRef(null, odCacheFontHandle));
                            }
                        }
                        else
                        {
                            if (odCacheFontHandleWrapper != null)
                            {
                                odCacheFontHandleWrapper.Dispose();
                            }
                            odCacheFontHandleWrapper = new Control.FontHandleWrapper(subItemFont);
                            SafeNativeMethods.SelectObject(new HandleRef(nmcd->nmcd, nmcd->nmcd.hdc), new HandleRef(odCacheFontHandleWrapper, odCacheFontHandleWrapper.Handle));
                        }

                        if (!dontmess)
                        {
                            m.Result = (IntPtr)NativeMethods.CDRF_NEWFONT;
                        }
                        if (disposeSubItemFont)
                        {
                            subItemFont.Dispose();
                        }
                        return;

                    default:
                        m.Result = (IntPtr)NativeMethods.CDRF_DODEFAULT;
                        return;
                }
            }
            catch (Exception e)
            {
                Debug.Fail("Exception occurred attempting to setup custom draw. Disabling custom draw for this control", e.ToString());
                m.Result = (IntPtr)NativeMethods.CDRF_DODEFAULT;
            }
        }

        private void DeleteFileName(string fileName) {
            if (!String.IsNullOrEmpty(fileName)) {
                // the list view needs the FileIOPermission when the app runs on an UNC share
                // and the list view creates / destroys temporary files for its background image
                
                // 

                FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
                fiop.Assert();
                try {
                    System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
                    if (fi.Exists) {
                        // 
                        // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                        // I could not find any resources which explain in detail when the IImgCtx objects
                        // release the temporary file. So if we get a FileIO when we delete the temporary file
                        // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                        try {
                            fi.Delete();
                        } catch (System.IO.IOException){}
                    }
                } finally {
                    System.Security.PermissionSet.RevertAssert();
                }
            }
        }

        private void DestroyLVGROUP(NativeMethods.LVGROUP lvgroup) {
            if(lvgroup.pszHeader != IntPtr.Zero) {
                Marshal.FreeHGlobal(lvgroup.pszHeader);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.DetachImageList"]/*' />
        /// <devdoc>
        ///     Resets the imageList to null.  We wire this method up to the imageList's
        ///     Dispose event, so that we don't hang onto an imageList that's gone away.
        /// </devdoc>
        private void DetachImageList(object sender, EventArgs e) {
            listViewState1[LISTVIEWSTATE1_disposingImageLists] = true;
            try {
                #if DEBUG
                if (sender != imageListSmall && sender != imageListState && sender != imageListLarge)
                {
                    Debug.Fail("ListView sunk dispose event from unknown component");
                }
                #endif // DEBUG
                if (sender == imageListSmall)
                    SmallImageList = null;
                if (sender == imageListLarge)
                    LargeImageList = null;
                if (sender == imageListState)
                    StateImageList = null;
            }
            finally {
                listViewState1[LISTVIEWSTATE1_disposingImageLists] = false;
            }

            UpdateListViewItemsLocations();

            
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Dispose"]/*' />
        /// <devdoc>
        ///     Disposes of the component.  Call dispose when the component is no longer needed.
        ///     This method removes the component from its container (if the component has a site)
        ///     and triggers the dispose event.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                // Remove any event sinks we have hooked up to imageLists
                if (imageListSmall != null) {
                    imageListSmall.Disposed -= new EventHandler(this.DetachImageList);
                    imageListSmall = null;
                }
                if (imageListLarge != null) {
                    imageListLarge.Disposed -= new EventHandler(this.DetachImageList);
                    imageListLarge = null;
                }
                if (imageListState != null) {
                    imageListState.Disposed -= new EventHandler(this.DetachImageList);
                    imageListState = null;
                }

                // Remove any ColumnHeaders contained in this control
                if (columnHeaders != null) {
                    for (int colIdx = columnHeaders.Length-1; colIdx >= 0; colIdx--) {
                        columnHeaders[colIdx].OwnerListview = null;
                        columnHeaders[colIdx].Dispose();
                    }
                    columnHeaders = null;
                }

                // Remove any items we have
                Items.Clear();

                if (odCacheFontHandleWrapper != null)
                {
                    odCacheFontHandleWrapper.Dispose();
                    odCacheFontHandleWrapper = null;
                }

                if (!String.IsNullOrEmpty(this.backgroundImageFileName) || this.bkImgFileNames != null) {
                    // we need the fileIoPermission when the app runs on an UNC share and
                    // the list view creates/deletes temporary files for its background image
                    
                    // 

                    FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
                    fiop.Assert();

                    try {
                        System.IO.FileInfo fi;
                        if (!String.IsNullOrEmpty(this.backgroundImageFileName)) {
                            fi = new System.IO.FileInfo(this.backgroundImageFileName);
                            Debug.Assert(fi.Exists, "who deleted our temp file?");
                            // 
                            // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                            // I could not find any resources which explain in detail when the IImgCtx objects
                            // release the temporary file. So if we get a FileIO when we delete the temporary file
                            // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                            try {
                                fi.Delete();
                            } catch (System.IO.IOException){}
                            this.backgroundImageFileName = String.Empty;
                        }
                        for (int i = 0; i <= this.bkImgFileNamesCount; i++) {
                            fi = new System.IO.FileInfo(this.bkImgFileNames[i]);
                            Debug.Assert(fi.Exists, "who deleted our temp file?");
                            // 
                            // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                            // I could not find any resources which explain in detail when the IImgCtx objects
                            // release the temporary file. So if we get a FileIO when we delete the temporary file
                            // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                            try {
                                fi.Delete();
                            } catch (System.IO.IOException){}
                        }

                        this.bkImgFileNames = null;
                        this.bkImgFileNamesCount = -1;
                    } finally {
                        System.Security.PermissionSet.RevertAssert();
                    }
                }

            }

            base.Dispose(disposing);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.EndUpdate"]/*' />
        /// <devdoc>
        ///     Cancels the effect of BeginUpdate.
        /// </devdoc>
        public void EndUpdate() {

            // On the final EndUpdate, check to see if we've got any cached items.
            // If we do, insert them as normal, then turn off the painting freeze.
            //
            if (--updateCounter == 0 && null != Properties.GetObject(PropDelayedUpdateItems)) {
                ApplyUpdateCachedItems();
            }
            EndUpdateInternal();
        }

        private void EnsureDefaultGroup() {
            if (IsHandleCreated && ComctlSupportsVisualStyles && GroupsEnabled) {
                if (SendMessage(NativeMethods.LVM_HASGROUP, DefaultGroup.ID, 0) == IntPtr.Zero) {
                    UpdateGroupView();
                    InsertGroupNative(0, DefaultGroup);
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.EnsureVisible"]/*' />
        /// <devdoc>
        ///     Ensure that the item is visible, scrolling the view as necessary.
        ///     @index  Index of item to scroll into view
        /// </devdoc>
        public void EnsureVisible(int index) {
            if (index < 0 || index >= Items.Count) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            if (IsHandleCreated)
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_ENSUREVISIBLE, index, 0);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FindItemWithText1"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewItem FindItemWithText(string text) {
            // if the user does not use the FindItemWithText overloads that specify a StartIndex and the listView is empty then return null
            if (this.Items.Count == 0) {
                return null;
            }

            return FindItemWithText(text, true, 0, true);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FindItemWithText2"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex) {
            return FindItemWithText(text, includeSubItemsInSearch, startIndex, true);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FindItemWithText3"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch) {
            if (startIndex < 0 || startIndex >= this.Items.Count)
            {
                throw new ArgumentOutOfRangeException("startIndex", string.Format(SR.InvalidArgument, "startIndex", (startIndex).ToString(CultureInfo.CurrentCulture)));
            }
            return FindItem(true, text, isPrefixSearch, new Point(0,0), SearchDirectionHint.Down, startIndex, includeSubItemsInSearch);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FindNearestItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewItem FindNearestItem(SearchDirectionHint dir, Point point) {
            return FindNearestItem(dir, point.X, point.Y);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.FindNearestItem2"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewItem FindNearestItem(SearchDirectionHint searchDirection, int x, int y)
        {
            if (this.View != View.SmallIcon && this.View != View.LargeIcon) {
                throw new InvalidOperationException(SR.ListViewFindNearestItemWorksOnlyInIconView);
            }

            if ( searchDirection < SearchDirectionHint.Left || searchDirection > SearchDirectionHint.Down) {
                throw new ArgumentOutOfRangeException("searchDirection", string.Format(SR.InvalidArgument, "searchDirection", (searchDirection).ToString()));
            }

            // the win32 ListView::FindNearestItem does some pretty weird things to determine the nearest item.
            // simply passing the (x,y) coordinates will cause problems when we call FindNearestItem for a point inside an item.
            // so we have to do some special processing when (x,y) falls inside an item;
            // 
            ListViewItem lvi = this.GetItemAt(x,y);

            if (lvi != null) {
                Rectangle itemBounds = lvi.Bounds;
                // LVM_FINDITEM is a nightmare
                // LVM_FINDITEM will use the top left corner of icon rectangle to determine the closest item
                // What happens if there is no icon for this item? then the top left corner of the icon rectangle falls INSIDE the item label (???)
                //

                Rectangle iconBounds = this.GetItemRect(lvi.Index, ItemBoundsPortion.Icon);

                switch (searchDirection) {
                    case SearchDirectionHint.Up:
                        y = Math.Max(itemBounds.Top, iconBounds.Top) - 1;
                        break;
                    case SearchDirectionHint.Down:
                        y = Math.Max(itemBounds.Top, iconBounds.Top) + 1;
                        break;
                    case SearchDirectionHint.Left:
                        x = Math.Max(itemBounds.Left, iconBounds.Left) - 1;
                        break;
                    case SearchDirectionHint.Right:
                        x = Math.Max(itemBounds.Left, iconBounds.Left) + 1;
                        break;
                    default:
                        Debug.Assert(false, "these are all the search directions");
                        break;
                 }
            }

            return FindItem(false, String.Empty, false, new Point(x, y), searchDirection, -1, false);
        }

        private ListViewItem FindItem(bool isTextSearch, string text, bool isPrefixSearch, Point pt, SearchDirectionHint dir, int startIndex, bool includeSubItemsInSearch) {
            if (this.Items.Count == 0) {
                return null;
            }

            if (!IsHandleCreated)
                CreateHandle();
            if (VirtualMode) {
                SearchForVirtualItemEventArgs sviEvent = new SearchForVirtualItemEventArgs(isTextSearch, isPrefixSearch, includeSubItemsInSearch, text, pt, dir, startIndex);

                OnSearchForVirtualItem(sviEvent);
                // NOTE: this will cause a RetrieveVirtualItem event w/o a corresponding cache hint event.
                if (sviEvent.Index != -1) {
                    return this.Items[sviEvent.Index];
                } else {
                    return null;
                }
            } else {
                NativeMethods.LVFINDINFO lvFindInfo = new NativeMethods.LVFINDINFO();
                if (isTextSearch) {
                    lvFindInfo.flags = NativeMethods.LVFI_STRING;
                    lvFindInfo.flags |= (isPrefixSearch ? NativeMethods.LVFI_PARTIAL : 0);
                    lvFindInfo.psz = text;
                } else {
                    lvFindInfo.flags = NativeMethods.LVFI_NEARESTXY;
                    lvFindInfo.ptX = pt.X;
                    lvFindInfo.ptY = pt.Y;
                    // we can do this because SearchDirectionHint is set to the VK_*
                    lvFindInfo.vkDirection = (int)dir;
                }
                lvFindInfo.lParam = IntPtr.Zero;
                int index = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle),
                                                                 NativeMethods.LVM_FINDITEM,
                                                                 startIndex-1,                      // decrement startIndex so that the search is 0-based
                                                                 ref lvFindInfo);
                if (index >= 0) {
                    return Items[index];
                } else if (isTextSearch && includeSubItemsInSearch) {
                    // win32 listView control can't search inside sub items
                    for (int i = startIndex; i < this.Items.Count; i ++) {
                        ListViewItem lvi = this.Items[i];
                        for (int j = 0; j < lvi.SubItems.Count; j ++) {
                            ListViewItem.ListViewSubItem lvsi = lvi.SubItems[j];
                            // the win32 list view search for items w/ text is case insensitive
                            // do the same for sub items
                            // because we are comparing user defined strings we have to do the slower String search
                            // ie, use String.Compare(string, string, case sensitive, CultureInfo)
                            // instead of new Whidbey String.Equals overload
                            // String.Equals(string, string, StringComparison.OrdinalIgnoreCase
                            if (String.Equals(text,lvsi.Text, StringComparison.OrdinalIgnoreCase)) {
                                return lvi;
                            } else if (isPrefixSearch && CultureInfo.CurrentCulture.CompareInfo.IsPrefix(lvsi.Text, text, CompareOptions.IgnoreCase)) {
                                return lvi;
                            }
                        }
                    }
                    return null;
                } else {
                    return null;
                }
            }
        }

        private void ForceCheckBoxUpdate() {
            // Force ListView to update its checkbox bitmaps.
            //
            if (CheckBoxes && IsHandleCreated) {
                SendMessage(NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, NativeMethods.LVS_EX_CHECKBOXES, 0);
                SendMessage(NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, NativeMethods.LVS_EX_CHECKBOXES, NativeMethods.LVS_EX_CHECKBOXES);

                // Comctl should handle auto-arrange for us, but doesn't
                if (AutoArrange) {
                    ArrangeIcons(Alignment);
                }
            }
        }

        private string GenerateRandomName() {
            Debug.Assert(this.BackgroundImage != null, "we need to generate random numbers only when saving the background image to disk");
            Bitmap bm = new Bitmap(this.BackgroundImage);
            int handle = 0;

            try {
                handle = unchecked ((int) (long) bm.GetHicon());
            } catch {
                bm.Dispose();
            }

            Random rnd;
            if (handle == 0) {
                // there was a problem when we got the icon handle
                // use DateTime.Now to seed the randomizer
                rnd = new Random((int)System.DateTime.Now.Ticks);
            } else {
                rnd = new Random(handle);
            }
            return rnd.Next().ToString(CultureInfo.InvariantCulture);
        }

        // IDs for identifying ListViewItem's
        private int GenerateUniqueID() {
            // Okay, if someone adds several billion items to the list and doesn't remove all of them,
            // we can reuse the same ID, but I'm willing to take that risk.  We are even tracking IDs
            // on a per-list view basis to reduce the problem.
            int result = nextID++;
            if (result == -1) {// leave -1 as a "no such value" ID
                result = 0;
                nextID = 1;
            }
            return result;
        }

        /// <devdoc>
        ///     Gets the real index for the given item.  lastIndex is the last return
        ///     value from GetDisplayIndex, or -1 if you don't know.  If provided,
        ///     the search for the index can be greatly improved.
        /// </devdoc>
        internal int GetDisplayIndex(ListViewItem item, int lastIndex) {

            Debug.Assert(item.listView == this, "Can't GetDisplayIndex if the list item doesn't belong to us");
            Debug.Assert(item.ID != -1, "ListViewItem has no ID yet");

            ApplyUpdateCachedItems();
            if (IsHandleCreated && !ListViewHandleDestroyed) {
                Debug.Assert(listItemsArray == null, "listItemsArray not null, even though handle created");

                NativeMethods.LVFINDINFO info = new NativeMethods.LVFINDINFO();
                info.lParam = (IntPtr) item.ID;
                info.flags = NativeMethods.LVFI_PARAM;

                int displayIndex = -1;

                if (lastIndex != -1) {
                    displayIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_FINDITEM, lastIndex - 1, ref info);
                }

                if (displayIndex == -1) {
                    displayIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_FINDITEM, -1 /* beginning */, ref info);
                }
                Debug.Assert(displayIndex != -1, "This item is in the list view -- why can't we find a display index for it?");
                return displayIndex;
            }
            else {
                // PERF: The only reason we should ever call this before the handle is created
                // is if the user calls ListViewItem.Index.
                Debug.Assert(listItemsArray != null, "listItemsArray is null, but the handle isn't created");

                int index = 0;
                foreach (object o in listItemsArray) {
                    if (o == item)
                        return index;
                    index++;
                }
                return -1;
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetColumnIndex"]/*' />
        /// <devdoc>
        ///     Called by ColumnHeader objects to determine their position
        ///     in the ListView
        /// </devdoc>
        /// <internalonly/>
        internal int GetColumnIndex(ColumnHeader ch) {
            if (columnHeaders == null)
                return -1;

            for (int i = 0; i < columnHeaders.Length; i++) {
                if (columnHeaders[i] == ch)
                    return i;
            }

            return -1;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetItemAt"]/*' />
        /// <devdoc>
        ///     Returns the current ListViewItem corresponding to the specific
        ///     x,y co-ordinate.
        /// </devdoc>
        public ListViewItem GetItemAt(int x, int y) {
            NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();

            lvhi.pt_x = x;
            lvhi.pt_y = y;

            int displayIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_HITTEST, 0, lvhi);

            ListViewItem li = null;
            if (displayIndex >= 0 && ((lvhi.flags & NativeMethods.LVHT_ONITEM) != 0))
                li = Items[displayIndex];

            return li;
        }

        internal int GetNativeGroupId(ListViewItem item) {
            item.UpdateGroupFromName();

            if (item.Group != null && Groups.Contains(item.Group)) {
                return item.Group.ID;
            }
            else {
                EnsureDefaultGroup();
                return DefaultGroup.ID;
            }
        }

        internal void GetSubItemAt(int x, int y, out int iItem, out int iSubItem) {
            NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();

            lvhi.pt_x = x;
            lvhi.pt_y = y;

            int index = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SUBITEMHITTEST, 0, lvhi);

            if (index > -1) {
                iItem = lvhi.iItem;
                iSubItem = lvhi.iSubItem;
            } else {
                iItem = -1;
                iSubItem = -1;
            }
        }

        internal Point GetItemPosition(int index) {
            NativeMethods.POINT pt = new NativeMethods.POINT();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_GETITEMPOSITION, index, pt);

            return new Point(pt.x, pt.y);
        }

        internal int GetItemState(int index) {
            return GetItemState(index, NativeMethods.LVIS_FOCUSED | NativeMethods.LVIS_SELECTED | NativeMethods.LVIS_CUT |
                                NativeMethods.LVIS_DROPHILITED | NativeMethods.LVIS_OVERLAYMASK |
                                NativeMethods.LVIS_STATEIMAGEMASK);
        }

        internal int GetItemState(int index, int mask) {
            if (index < 0 || ((this.VirtualMode && index >= this.VirtualListSize) || (!this.VirtualMode && index >= itemCount))) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            Debug.Assert(IsHandleCreated, "How did we add items without a handle?");

            return unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETITEMSTATE, index, mask));
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetItemRect"]/*' />
        /// <devdoc>
        ///     Returns a list item's bounding rectangle, including subitems.
        /// </devdoc>
        public Rectangle GetItemRect(int index) {
            return GetItemRect(index, 0);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetItemRect1"]/*' />
        /// <devdoc>
        ///     Returns a specific portion of a list item's bounding rectangle.
        /// </devdoc>
        public Rectangle GetItemRect(int index, ItemBoundsPortion portion) {
            if (index < 0 || index >= this.Items.Count) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(portion, (int)portion, (int)ItemBoundsPortion.Entire, (int)ItemBoundsPortion.ItemOnly)){
                throw new InvalidEnumArgumentException("portion", (int)portion, typeof(ItemBoundsPortion));
            }

            if (this.View == View.Details && this.Columns.Count == 0) {
                return Rectangle.Empty;
            }


            NativeMethods.RECT itemrect = new NativeMethods.RECT();
            itemrect.left = (int)portion;
            if (unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETITEMRECT, index, ref itemrect)) == 0)
                throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                          "index",
                                                          (index).ToString(CultureInfo.CurrentCulture)));

            return Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);
        }

        /// <devdoc>
        ///     Private version of GetItemRect that fails silently. We use this instead of catching
        ///     exceptions thrown by GetItemRect, to avoid first chance exceptions confusing the user.
        /// </devdoc>
        private Rectangle GetItemRectOrEmpty(int index) {
            if (index < 0 || index >= this.Items.Count)
                return Rectangle.Empty;

            if (this.View == View.Details && this.Columns.Count == 0) {
                return Rectangle.Empty;
            }


            NativeMethods.RECT itemrect = new NativeMethods.RECT();
            itemrect.left = 0;
            if (unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETITEMRECT, index, ref itemrect)) == 0)
                return Rectangle.Empty;

            return Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);
        }

        private NativeMethods.LVGROUP GetLVGROUP(ListViewGroup group)
        {
            NativeMethods.LVGROUP lvgroup = new NativeMethods.LVGROUP();
            lvgroup.mask = NativeMethods.LVGF_HEADER | NativeMethods.LVGF_GROUPID | NativeMethods.LVGF_ALIGN;

            // Header
            //
            String header = group.Header;
            lvgroup.pszHeader = Marshal.StringToHGlobalAuto(header);
            lvgroup.cchHeader = header.Length;

            // Group ID
            //
            lvgroup.iGroupId = group.ID;

            // Alignment
            //
            switch(group.HeaderAlignment) {
                case HorizontalAlignment.Left:
                    lvgroup.uAlign = NativeMethods.LVGA_HEADER_LEFT;
                    break;
                case HorizontalAlignment.Right:
                    lvgroup.uAlign = NativeMethods.LVGA_HEADER_RIGHT;
                    break;
                case HorizontalAlignment.Center:
                    lvgroup.uAlign = NativeMethods.LVGA_HEADER_CENTER;
                    break;
            }
            return lvgroup;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetSubItemRect"]/*' />
        /// <devdoc>
        ///     Returns a listview sub-item's bounding rectangle.
        /// </devdoc>
        internal Rectangle GetSubItemRect(int itemIndex, int subItemIndex) {
            return GetSubItemRect(itemIndex, subItemIndex, 0);
        }

        internal Rectangle GetSubItemRect(int itemIndex, int subItemIndex, ItemBoundsPortion portion) {
            // it seems that getting the rectangle for a sub item only works for list view which are in Details view
            if (this.View != View.Details) {
                return Rectangle.Empty;
            }
            if (itemIndex < 0 || itemIndex >= this.Items.Count) {
                throw new ArgumentOutOfRangeException("itemIndex", string.Format(SR.InvalidArgument, "itemIndex", (itemIndex).ToString(CultureInfo.CurrentCulture)));
            }
            int subItemCount = Items[itemIndex].SubItems.Count;

            if (subItemIndex < 0 || subItemIndex >= subItemCount) {
                throw new ArgumentOutOfRangeException("subItemIndex", string.Format(SR.InvalidArgument, "subItemIndex", (subItemIndex).ToString(CultureInfo.CurrentCulture)));
            }
            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(portion, (int)portion, (int)ItemBoundsPortion.Entire, (int)ItemBoundsPortion.ItemOnly))
            {
                throw new InvalidEnumArgumentException("portion", (int)portion, typeof(ItemBoundsPortion));
            }

            if (this.Columns.Count == 0) {
                return Rectangle.Empty;
            }

            NativeMethods.RECT itemrect = new NativeMethods.RECT();
            itemrect.left = (int)portion;
            itemrect.top = subItemIndex;
            if (unchecked( (int) (long)SendMessage(NativeMethods.LVM_GETSUBITEMRECT, itemIndex, ref itemrect)) == 0)
                throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                          "itemIndex",
                                                          (itemIndex).ToString(CultureInfo.CurrentCulture)));

            Rectangle result = Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);

            return result;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HitTest"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewHitTestInfo HitTest(Point point) {
            return HitTest(point.X, point.Y);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.HitTest1"]/*' />
        /// <devdoc>
        /// </devdoc>
        public ListViewHitTestInfo HitTest(int x, int y) {
            if (!this.ClientRectangle.Contains(x, y)) {
                return new ListViewHitTestInfo(null /*hitItem*/, null /*hitSubItem*/, ListViewHitTestLocations.None /*hitLocation*/);
            }

            NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();
            lvhi.pt_x = x;
            lvhi.pt_y = y;

            int iItem;

            if (this.View == View.Details) {
                iItem = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SUBITEMHITTEST, 0, lvhi));
            } else {
                iItem = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_HITTEST, 0, lvhi));
            }

            ListViewItem item = (iItem == -1) ? null : Items[iItem];
            ListViewHitTestLocations location = ListViewHitTestLocations.None;

            if (item == null && (NativeMethods.LVHT_ABOVE & lvhi.flags) == NativeMethods.LVHT_ABOVE) {
                location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & lvhi.flags) | (int)ListViewHitTestLocations.AboveClientArea);
            }
            else if (item != null && (NativeMethods.LVHT_ONITEMSTATEICON & lvhi.flags) == NativeMethods.LVHT_ONITEMSTATEICON) {
                location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & lvhi.flags) | (int)ListViewHitTestLocations.StateImage);
            }
            else {
                location = (ListViewHitTestLocations)lvhi.flags;
            }

            if (this.View == View.Details && item != null) {
                if (lvhi.iSubItem < item.SubItems.Count) {
                    return new ListViewHitTestInfo(item, item.SubItems[lvhi.iSubItem], location);
                } else {
                    return new ListViewHitTestInfo(item, null, location);
                }
            } else {
                return (new ListViewHitTestInfo(item, null, location));
            }
        }

        private void InvalidateColumnHeaders() {
            if (viewStyle == View.Details && IsHandleCreated) {
                // 

                IntPtr hwndHdr = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_GETHEADER, 0, 0);
                if (hwndHdr != IntPtr.Zero)
                    SafeNativeMethods.InvalidateRect(new HandleRef(this, hwndHdr), null, true);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.InsertColumn"]/*' />
        /// <devdoc>
        ///     Inserts a new Column into the ListView
        /// </devdoc>
        internal ColumnHeader InsertColumn(int index, ColumnHeader ch) {
            return InsertColumn(index, ch, true);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.InsertColumn1"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal ColumnHeader InsertColumn(int index, ColumnHeader ch, bool refreshSubItems) {
            if (ch == null)
                throw new ArgumentNullException("ch");
            if (ch.OwnerListview != null)
                throw new ArgumentException(string.Format(SR.OnlyOneControl, ch.Text), "ch");


            int idx;
            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (IsHandleCreated && this.View != View.Tile) {
                idx = InsertColumnNative(index, ch);
            }
            else {
                idx = index;
            }

            // First column must be left aligned

            if (-1 == idx)
                throw new InvalidOperationException(SR.ListViewAddColumnFailed);

            // Add the column to our internal array
            int columnCount = (columnHeaders == null ? 0 : columnHeaders.Length);
            if (columnCount > 0) {
                ColumnHeader[] newHeaders = new ColumnHeader[columnCount + 1];
                if (columnCount > 0)
                    System.Array.Copy(columnHeaders, 0, newHeaders, 0, columnCount);
                columnHeaders = newHeaders;
            }
            else
                columnHeaders = new ColumnHeader[1];

            if (idx < columnCount) {
                System.Array.Copy(columnHeaders, idx, columnHeaders, idx + 1, columnCount - idx);
            }
            columnHeaders[idx] = ch;
            ch.OwnerListview = this;

            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (ch.ActualImageIndex_Internal != -1 && this.IsHandleCreated && this.View != View.Tile) {
                SetColumnInfo(NativeMethods.LVCF_IMAGE, ch);
            }


            // update the DisplayIndex for each column
            // we are only setting an integer in the ColumnHeader, this is not expensive
            int[] indices = new int[this.Columns.Count];
            for (int i = 0; i < this.Columns.Count; i ++) {
                ColumnHeader hdr = this.Columns[i];
                if (hdr == ch) {
                    // the newly added column
                    hdr.DisplayIndexInternal = index;
                } else if (hdr.DisplayIndex >= index) {
                    hdr.DisplayIndexInternal ++;
                }
                indices[i] = hdr.DisplayIndexInternal;
            }

            SetDisplayIndices( indices );
             
#if DEBUG
            CheckDisplayIndices();
#endif
            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (this.IsHandleCreated && this.View == View.Tile) {
                this.RecreateHandleInternal();
            } else if (IsHandleCreated && refreshSubItems)
                RealizeAllSubItems();

            return ch;
        }

        private int InsertColumnNative(int index, ColumnHeader ch) {
            NativeMethods.LVCOLUMN_T lvColumn = new NativeMethods.LVCOLUMN_T();
            lvColumn.mask = NativeMethods.LVCF_FMT | NativeMethods.LVCF_TEXT | NativeMethods.LVCF_WIDTH;// | NativeMethods.LVCF_ORDER | NativeMethods.LVCF_IMAGE;

            if (ch.OwnerListview != null && ch.ActualImageIndex_Internal != -1) {
                lvColumn.mask |= NativeMethods.LVCF_IMAGE;
                lvColumn.iImage = ch.ActualImageIndex_Internal;
            }

            lvColumn.fmt        = (int)ch.TextAlign;
            lvColumn.cx         = ch.Width;
            lvColumn.pszText    = ch.Text;

            return (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_INSERTCOLUMN, index, lvColumn);
        }

        // when the user adds a group, this helper method makes sure that all the items
        // in the list view are parented by a group - be it the DefaultGroup or some other group
        internal void InsertGroupInListView(int index, ListViewGroup group) {
            Debug.Assert(this.groups != null && this.groups.Count > 0, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");
            Debug.Assert(group != this.DefaultGroup, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");

            // the first time we add a group we have to group the items in the Default Group
            bool groupItems = (this.groups.Count == 1) && this.GroupsEnabled;

            UpdateGroupView();
            EnsureDefaultGroup();
            InsertGroupNative(index, group);

            // take all the list view items which don't belong to any group and put them in the default group
            //
            if (groupItems) {
                for (int i = 0; i < this.Items.Count; i ++) {
                    ListViewItem item = this.Items[i];
                    if (item.Group == null) {
                        item.UpdateStateToListView(item.Index);
                    }
                }
            }

#if DEBUG
            // sanity check: all the items in the list view should have a group ID
            if (this.GroupsEnabled) {
                for (int i = 0; i < this.Items.Count; i ++) {
                    ListViewItem item = this.Items[i];
                    NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                    lvItem.iItem = item.Index;
                    lvItem.mask = NativeMethods.LVIF_GROUPID;
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_GETITEM, 0, ref lvItem);
                    Debug.Assert(lvItem.iGroupId != -1, "there is a list view item which is not parented");
                }
            }
#endif
        }

        // does the Win32 part of the job of inserting the group
        private void InsertGroupNative(int index, ListViewGroup group) {
            Debug.Assert(IsHandleCreated,"InsertGroupNative precondition: list-view handle must be created");
            Debug.Assert(group == DefaultGroup || this.Groups.Contains(group),"Make sure ListView.Groups contains this group before adding the native LVGROUP. Otherwise, custom-drawing may break.");

            NativeMethods.LVGROUP lvgroup = new NativeMethods.LVGROUP();
            try
            {
                 lvgroup = GetLVGROUP(group);
                 int retval = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this,this.Handle),NativeMethods.LVM_INSERTGROUP,index,lvgroup);
                 Debug.Assert(retval != -1,"Failed to insert group");
            }
            finally
            {
                 DestroyLVGROUP(lvgroup);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.InsertItem"]/*' />
        /// <devdoc>
        ///     Inserts a new ListViewItem into the ListView.  The item will be inserted
        ///     either in the correct sorted position, or, if no sorting is set, at the
        ///     position indicated by the index parameter.
        /// </devdoc>
        private void InsertItems(int displayIndex, ListViewItem[] items, bool checkHosting) {

            if (items == null || items.Length == 0) {
                return;
            }

            if (this.IsHandleCreated && this.Items.Count == 0 && this.View == View.SmallIcon && this.ComctlSupportsVisualStyles)
            {
                this.FlipViewToLargeIconAndSmallIcon = true;
            }

            // if we're in the middle of a Begin/EndUpdate, just push the items into our array list
            // as they'll get processed on EndUpdate.
            //
            if (updateCounter > 0 && Properties.GetObject(PropDelayedUpdateItems) != null) {
                // CheckHosting.
                if (checkHosting) {
                    for (int i = 0; i < items.Length; i ++) {
                        if (items[i].listView != null) {
                            throw new ArgumentException(string.Format(SR.OnlyOneControl, items[i].Text), "item");
                        }
                    }
                }

                ArrayList itemList = (ArrayList)Properties.GetObject(PropDelayedUpdateItems);
                Debug.Assert(itemList != null, "In Begin/EndUpdate with no delayed array!");
                if (itemList != null) {
                    itemList.AddRange(items);
                }

                // add the list view item to the list view
                // this way we can retrieve the item's index inside BeginUpdate/EndUpdate
                for (int i = 0; i < items.Length; i ++) {
                    items[i].Host(this, GenerateUniqueID(), -1);
                }

                this.FlipViewToLargeIconAndSmallIcon = false;

                return;
            }

            // loop through the items and give them id's so we can identify them later.
            //
            for (int i = 0; i < items.Length; i++) {
                ListViewItem item = items[i];

                if (checkHosting && item.listView != null) {
                    throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text), "item");
                }

                // create an ID..
                //
                int itemID = GenerateUniqueID();
                Debug.Assert(!listItemsTable.ContainsKey(itemID), "internal hash table inconsistent -- inserting item, but it's already in the hash table");
                listItemsTable.Add(itemID, item);

                itemCount++;
                item.Host(this,itemID, -1);

                // if there's no handle created, just ad them to our list items array.
                //
                if (!IsHandleCreated) {
                    Debug.Assert(listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    listItemsArray.Insert(displayIndex + i, item);
                }
            }

            // finally if the handle is created, do the actual add into the real list view
            //
            if (IsHandleCreated) {
                Debug.Assert(listItemsArray == null, "listItemsArray not null, even though handle created");
                InsertItemsNative(displayIndex, items);
            }

            Invalidate();
            ArrangeIcons(alignStyle);

            // Any newly added items shoul dhave the correct location.
            // UpdateListViewItemsLocations();


            // Update sorted order
            if (!VirtualMode) {
                Sort();
            }

            Debug.Assert(!this.FlipViewToLargeIconAndSmallIcon, "if we added even 1 item then we should have been done w/ FlipViewToLargeIconAndSmallIcon");
        }


        /// <devdoc>
        ///     Inserts a new ListViewItem into the list view itself.
        ///     This only will be called when the Handle has been created for the list view.
        ///     This method loops through the items, sets up their state then adds them.
        /// </devdoc>
        private int InsertItemsNative(int index, ListViewItem[] items) {
            if (items == null || items.Length == 0) {
                return 0;
            }
            Debug.Assert(IsHandleCreated, "InsertItemsNative precondition: list-view handle must be created");

            // Much more efficient to call the native insert with max + 1, than with max.  The + 1
            // for the display index accounts for itemCount++ above.
            //
            if (index == itemCount - 1) {
                index++;
            }

            // Create and add the LVITEM
            NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
            int actualIndex = -1;
            IntPtr hGlobalColumns = IntPtr.Zero;
            int maxColumns = 0;
            this.listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = true;

            try {
                // Set the count of items first.
                //
                SendMessage(NativeMethods.LVM_SETITEMCOUNT, itemCount, 0);

                // Now add the items.
                //
                for (int i = 0; i < items.Length; i++) {
                    ListViewItem li = items[i];

                    Debug.Assert(this.Items.Contains(li), "Make sure ListView.Items contains this item before adding the native LVITEM. Otherwise, custom-drawing may break.");

                    lvItem.Reset();
                    lvItem.mask = NativeMethods.LVIF_TEXT | NativeMethods.LVIF_IMAGE | NativeMethods.LVIF_PARAM | NativeMethods.LVIF_INDENT;
                    lvItem.iItem    = index + i;
                    lvItem.pszText  = li.Text;
                    lvItem.iImage   = li.ImageIndexer.ActualIndex;
                    lvItem.iIndent  = li.IndentCount;
                    lvItem.lParam   = (IntPtr)li.ID;

                    if (GroupsEnabled) {
                        lvItem.mask |= NativeMethods.LVIF_GROUPID;
                        lvItem.iGroupId = GetNativeGroupId(li);

                        #if DEBUG
                            Debug.Assert(SendMessage(NativeMethods.LVM_ISGROUPVIEWENABLED, 0, 0) != IntPtr.Zero, "Groups not enabled");
                            Debug.Assert(SendMessage(NativeMethods.LVM_HASGROUP, lvItem.iGroupId, 0) != IntPtr.Zero, "Doesn't contain group id: " + lvItem.iGroupId.ToString(CultureInfo.InvariantCulture));
                        #endif
                    }

                    lvItem.mask |= NativeMethods.LVIF_COLUMNS;
                    lvItem.cColumns = this.columnHeaders != null ? Math.Min(MAXTILECOLUMNS, this.columnHeaders.Length) : 0;

                    // make sure that our columns memory is big enough.
                    // if not, then realloc it.
                    //
                    if (lvItem.cColumns > maxColumns || hGlobalColumns == IntPtr.Zero) {
                        if (hGlobalColumns != IntPtr.Zero) {
                            Marshal.FreeHGlobal(hGlobalColumns);
                        }
                        hGlobalColumns = Marshal.AllocHGlobal(lvItem.cColumns * Marshal.SizeOf(typeof(int)));
                        maxColumns = lvItem.cColumns;
                    }

                    // now build and copy in the column indexes.
                    //
                    lvItem.puColumns = hGlobalColumns;
                    int[] columns = new int[lvItem.cColumns];
                    for(int c = 0; c < lvItem.cColumns; c++) {
                        columns[c] = c + 1;
                    }
                    Marshal.Copy(columns, 0, lvItem.puColumns, lvItem.cColumns);

                    // Inserting an item into a ListView with checkboxes causes one or more
                    // item check events to be fired for the newly added item.
                    // Therefore, we disable the item check event handler temporarily.
                    //
                    ItemCheckEventHandler oldOnItemCheck = onItemCheck;
                    onItemCheck = null;

                    int insertIndex;

                    try {

                        li.UpdateStateToListView(lvItem.iItem, ref lvItem, false);

                        insertIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_INSERTITEM, 0, ref lvItem);
                        if (actualIndex == -1) {
                            actualIndex = insertIndex;

                            // and update our starting index. so we're going from the same point.
                            //
                            index = actualIndex;
                        }
                    } finally {

                        // Restore the item check event handler.
                        //
                        onItemCheck = oldOnItemCheck;
                    }

                    if (-1 == insertIndex) {
                        throw new InvalidOperationException(SR.ListViewAddItemFailed);
                    }

                    // add all sub items
                    for (int nItem = 1; nItem < li.SubItems.Count; ++nItem) {
                        SetItemText(insertIndex, nItem, li.SubItems[nItem].Text, ref lvItem);
                    }

                    // PERF.
                    // Use StateSelected in order to avoid a call into the native list view.
                    if (li.StateImageSet || li.StateSelected) {
                        // lvItem.state and lvItem.stateMask are set when the lvItem is updated in UpdateStateToListView call.
                        SetItemState(insertIndex, lvItem.state, lvItem.stateMask);
                    }
                }
            }
            finally {
                if (hGlobalColumns != IntPtr.Zero) {
                    Marshal.FreeHGlobal(hGlobalColumns);
                }
                this.listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = false;
            }

            if (this.listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped])
            {
                // SelectedIndexChanged event was delayed
                this.listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
                OnSelectedIndexChanged(EventArgs.Empty);
            }

            if (this.FlipViewToLargeIconAndSmallIcon) {
                this.FlipViewToLargeIconAndSmallIcon = false;

                this.View = View.LargeIcon;
                this.View = View.SmallIcon;
            }

            return actualIndex;
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.IsInputKey"]/*' />
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

            bool isInputKey = base.IsInputKey(keyData);
            if (isInputKey)
                return true;

            if (listViewState[LISTVIEWSTATE_inLabelEdit]) {
                switch (keyData & Keys.KeyCode) {
                    case Keys.Return:
                    case Keys.Escape:
                        return true;
                }
            }

            return false;
        }

        private void LargeImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated) {
                IntPtr handle = (LargeImageList == null) ? IntPtr.Zero : LargeImageList.Handle;
                SendMessage(NativeMethods.LVM_SETIMAGELIST, (IntPtr) NativeMethods.LVSIL_NORMAL, handle);

                ForceCheckBoxUpdate();
            }
        }

        private void LargeImageListChangedHandle(object sender, EventArgs e) {
            if (!this.VirtualMode && (null != sender) && (sender == imageListLarge) && IsHandleCreated) {
                foreach (ListViewItem item in Items) {
                    if (item.ImageIndexer.ActualIndex != -1 && item.ImageIndexer.ActualIndex >= imageListLarge.Images.Count) {
                        SetItemImage(item.Index,imageListLarge.Images.Count - 1);
                    }
                    else {
                        SetItemImage(item.Index,item.ImageIndexer.ActualIndex);
                    }
                }
            }
        }

        internal void ListViewItemToolTipChanged(ListViewItem item) {
            if (this.IsHandleCreated) {
                // If we reset the item text then we also reset the tool tip text
                //
                SetItemText(item.Index, 0 /*subItemIndex*/, item.Text);
            }
        }

        private void LvnBeginDrag(MouseButtons buttons, NativeMethods.NMLISTVIEW nmlv) {
            ListViewItem item = Items[nmlv.iItem];
            OnItemDrag(new ItemDragEventArgs(buttons, item));
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnAfterLabelEdit"]/*' />
        /// <devdoc>
        ///     Fires the afterLabelEdit event.
        /// </devdoc>
        protected virtual void OnAfterLabelEdit(LabelEditEventArgs e) {
            if (onAfterLabelEdit != null) onAfterLabelEdit(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnBackgroundImageChanged"]/*' />
        protected override void OnBackgroundImageChanged(EventArgs e) {
            if (IsHandleCreated) {
                SetBackgroundImage();
            }
            base.OnBackgroundImageChanged(e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnMouseLeave"]/*' />
        /// <devdoc>
        ///  We keep track of if we've hovered already so we don't fire multiple hover events
        /// </devdoc>
        /// <internalonly>
        protected override void OnMouseLeave(EventArgs e) {
            hoveredAlready = false;
            base.OnMouseLeave(e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnMouseHover"]/*' />
        /// <devdoc>
        ///     In order for the MouseHover event to fire for each item in a ListView,
        ///     the item the mouse is hovering over is found. Each time a new item is hovered
        ///     over a new event is raised.
        /// </devdoc>
        protected override void OnMouseHover(EventArgs e)  {

            /// Hover events need to be caught for each node
            /// within the TreeView so the appropriate
            /// NodeHovered event can be raised.

            ListViewItem item = null;

            if (this.Items.Count > 0) {
                // APPCOMPAT
                // V1.* users implement virtualization by communicating directly to the native ListView and by passing our virtualization implementation.
                // In that case, the native list view may have an item under the mouse even if our wrapper thinks the item count is 0.
                // And that may cause GetItemAt to throw an out of bounds exception.

                Point pos = Cursor.Position;
                pos = PointToClientInternal(pos);
                item = GetItemAt(pos.X, pos.Y);
            }

            if (item != prevHoveredItem && item != null) {
                OnItemMouseHover(new ListViewItemMouseHoverEventArgs(item));
                prevHoveredItem = item;
            }

            if (!hoveredAlready) {
                base.OnMouseHover(e);
                hoveredAlready = true;
            }

            ResetMouseEventArgs();

        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnBeforeLabelEdit"]/*' />
        /// <devdoc>
        ///     Fires the beforeLabelEdit event.
        /// </devdoc>
        protected virtual void OnBeforeLabelEdit(LabelEditEventArgs e) {
            if (onBeforeLabelEdit != null) onBeforeLabelEdit(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListBox.OnCacheVirtualItems"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected virtual void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
        {
            CacheVirtualItemsEventHandler handler = (CacheVirtualItemsEventHandler)Events[EVENT_CACHEVIRTUALITEMS];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnColumnClick"]/*' />
        /// <devdoc>
        ///     Fires the columnClick event.
        /// </devdoc>
        protected virtual void OnColumnClick(ColumnClickEventArgs e) {
            if (onColumnClick != null) onColumnClick(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnColumnReordered"]/*' />
        /// <devdoc>
        ///     Fires the column header rearranged event.
        /// </devdoc>
        protected virtual void OnColumnReordered(ColumnReorderedEventArgs e) {
            ColumnReorderedEventHandler handler = (ColumnReorderedEventHandler) Events[EVENT_COLUMNREORDERED];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnColumnWidthChanged"]/*' />
        /// <devdoc>
        ///     Fires the column width changing event.
        /// </devdoc>
        protected virtual void OnColumnWidthChanged(ColumnWidthChangedEventArgs e) {
            ColumnWidthChangedEventHandler handler = (ColumnWidthChangedEventHandler)Events[EVENT_COLUMNWIDTHCHANGED];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnColumnWidthChanging"]/*' />
        /// <devdoc>
        ///     Fires the column width changing event.
        /// </devdoc>
        protected virtual void OnColumnWidthChanging(ColumnWidthChangingEventArgs e) {
            ColumnWidthChangingEventHandler handler = (ColumnWidthChangingEventHandler)Events[EVENT_COLUMNWIDTHCHANGING];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnDrawColumnHeader"]/*' />
        /// <devdoc>
        ///     Fires the DrawColumnHeader event.
        /// </devdoc>
        protected virtual void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e) {
            DrawListViewColumnHeaderEventHandler handler = (DrawListViewColumnHeaderEventHandler)Events[EVENT_DRAWCOLUMNHEADER];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnDrawItem"]/*' />
        /// <devdoc>
        ///     Fires the DrawItem event.
        /// </devdoc>
        protected virtual void OnDrawItem(DrawListViewItemEventArgs e) {
            DrawListViewItemEventHandler handler = (DrawListViewItemEventHandler)Events[EVENT_DRAWITEM];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListBox.OnDrawSubItem"]/*' />
        /// <devdoc>
        ///     Fires the DrawSubItem event.
        /// </devdoc>
        protected virtual void OnDrawSubItem(DrawListViewSubItemEventArgs e) {
            DrawListViewSubItemEventHandler handler = (DrawListViewSubItemEventHandler)Events[EVENT_DRAWSUBITEM];
            if (handler != null) {
                handler(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnFontChanged"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);

            // When the user sets the font on the Form, the layout engine will compute the bounds for the native list view
            // and AFTER that will set the window font on the native list view.
            // That means that when the list view computed its item's position it did so w/ the previous font.
            // The solution is to send LVM_UPDATE to the native list view EVEN if the list view is not in SmallIcon or LargeIcon.
            if (!VirtualMode && IsHandleCreated && AutoArrange) {
                BeginUpdate();
                try {
                    SendMessage(NativeMethods.LVM_UPDATE, -1, 0);
                } finally {
                    EndUpdate();
                }
            }

            // If font changes and we have headers, they need to be expicitly invalidated
            //
            InvalidateColumnHeaders();
        }


        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnHandleCreated"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {
            // uncache the "ComctlSupportsVisualStyles" property on a handle creation
            listViewState[LISTVIEWSTATE_comctlSupportsVisualStylesTested] = false;

            // don't persist flipViewToLargeIconAndSmallIcon accross handle recreations...
            this.FlipViewToLargeIconAndSmallIcon = false;

            base.OnHandleCreated(e);
            



            int version = unchecked((int)(long)SendMessage(NativeMethods.CCM_GETVERSION, 0, 0));
            if (version < 5) {
                SendMessage(NativeMethods.CCM_SETVERSION, 5, 0);
            }
            UpdateExtendedStyles();
            RealizeProperties();
            int color = ColorTranslator.ToWin32(BackColor);
            SendMessage(NativeMethods.LVM_SETBKCOLOR,0,color);
            SendMessage(NativeMethods.LVM_SETTEXTCOLOR,0,ColorTranslator.ToWin32(base.ForeColor));

            // The native list view will not invalidate the entire list view item area if the BkColor is not CLR_NONE.
            // This not noticeable if the customer paints the items w/ the same background color as the list view itself.
            // However, if the customer paints the items w/ a color different from the list view's back color
            // then when the user changes selection the native list view will not invalidate the entire list view item area.
            SendMessage(NativeMethods.LVM_SETTEXTBKCOLOR, 0,NativeMethods.CLR_NONE);

            // LVS_NOSCROLL does not work well when the list view is in View.Details or in View.List modes.
            // we have to set this style after the list view was created and before we position the native list view items.
            //
            if (!Scrollable) {
                int style = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                style |= NativeMethods.LVS_NOSCROLL;
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)style));
            }

            // in VirtualMode we have to tell the list view to ask for the list view item's state image index
            if (VirtualMode) {
                int callbackMask = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_GETCALLBACKMASK, 0, 0));
                callbackMask |= NativeMethods.LVIS_STATEIMAGEMASK;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETCALLBACKMASK, callbackMask, 0);
            }

            if (ComctlSupportsVisualStyles) {
                SendMessage(NativeMethods.LVM_SETVIEW,(int)viewStyle,0);
                UpdateGroupView();

                // Add groups
                //
                if (groups != null) {
                    for(int index = 0;index < groups.Count;index++) {
                        InsertGroupNative(index,groups[index]);
                    }
                }

                // Set tile view settings
                //
                if (viewStyle == View.Tile) {
                    UpdateTileView();
                }
            }

            ListViewHandleDestroyed = false;

            // Use a copy of the list items array so that we can maintain the (handle created || listItemsArray != null) invariant
            //
            ListViewItem[] listViewItemsToAdd = null;
            if(listItemsArray != null)     {
                listViewItemsToAdd = (ListViewItem[])listItemsArray.ToArray(typeof(ListViewItem));
                listItemsArray = null;
            }

            int columnCount = (columnHeaders == null ? 0 : columnHeaders.Length);
            if(columnCount > 0)     {
                int[] indices = new int [columnHeaders.Length];
                int index = 0;
                foreach(ColumnHeader column in columnHeaders) {
                    indices[index] = column.DisplayIndex;
                    InsertColumnNative(index++,column);
                }
                SetDisplayIndices( indices );
            }

            // make sure that we're not in a begin/end update call.
            //
            if (itemCount > 0 && listViewItemsToAdd != null) {
                InsertItemsNative(0, listViewItemsToAdd);
            }

            if (VirtualMode && VirtualListSize > -1 && !DesignMode) {
                SendMessage(NativeMethods.LVM_SETITEMCOUNT, VirtualListSize, 0);
            }

            if(columnCount > 0) {
               UpdateColumnWidths(ColumnHeaderAutoResizeStyle.None);
            }

            ArrangeIcons(alignStyle);
            UpdateListViewItemsLocations();

            if (!VirtualMode) {
                Sort();
            }
            if (ComctlSupportsVisualStyles && (InsertionMark.Index > 0)) {
                InsertionMark.UpdateListView();
            }

            //
            // When the handle is recreated, update the SavedCheckedItems.
            // It is possible some checked items were added to the list view while its handle was null.
            //
            this.savedCheckedItems = null;
            if (!this.CheckBoxes && !this.VirtualMode) {
                for (int i = 0; i < this.Items.Count; i ++) {
                    if (this.Items[i].Checked) {
                        this.UpdateSavedCheckedItems(this.Items[i], true /*addItem*/);
                    }
                }
            }

        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnHandleDestroyed"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected override void OnHandleDestroyed(EventArgs e) {
            // don't save the list view items state when in virtual mode : it is the responsability of the
            // user to cache the list view items in virtual mode
            if (!Disposing && !VirtualMode) {

                int count = Items.Count;
                for (int i = 0; i < count; i++) {
                    Items[i].UpdateStateFromListView(i, true);
                }

                // Save away the selected and checked items
                //
                if (SelectedItems != null && !VirtualMode) {
                    // Create an array because the SelectedItems collection is tuned for CopyTo()
                    ListViewItem[] lviArr = new ListViewItem[SelectedItems.Count];
                    SelectedItems.CopyTo(lviArr, 0);
                    savedSelectedItems = new List<ListViewItem>(lviArr.Length);
                    for (int i = 0; i < lviArr.Length; i ++) {
                        savedSelectedItems.Add(lviArr[i]);
                    }
                }
                Debug.Assert(listItemsArray == null, "listItemsArray not null, even though handle created");
                ListViewItem[] items = null;
                ListViewItemCollection tempItems = Items;

                if (tempItems != null) {
                    items = new ListViewItem[tempItems.Count];
                    tempItems.CopyTo(items, 0);
                }

                if (items != null) {
                    listItemsArray = new ArrayList(items.Length);
                    listItemsArray.AddRange(items);
                }

                ListViewHandleDestroyed = true;
            }

            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemActivate"]/*' />
        /// <devdoc>
        ///     Fires the itemActivate event.
        /// </devdoc>
        protected virtual void OnItemActivate(EventArgs e) {
            if (onItemActivate != null) onItemActivate(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemCheck"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the KeyEventArgs.  Don't
        ///     forget to call base.onItemCheck() to ensure that itemCheck vents
        ///     are correctly fired for all other keys.
        /// </devdoc>
        protected virtual void OnItemCheck(ItemCheckEventArgs ice) {
            if (onItemCheck != null) {
                onItemCheck(this, ice);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemChecked"]/*' />
        protected virtual void OnItemChecked(ItemCheckedEventArgs e) {
            if (onItemChecked != null) {
                onItemChecked(this, e);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemDrag"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected virtual void OnItemDrag(ItemDragEventArgs e) {
            if (onItemDrag != null) onItemDrag(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemMouseHover"]/*' />
        /// <devdoc>
        ///     Fires the ItemMouseHover event.
        /// </devdoc>
        protected virtual void OnItemMouseHover(ListViewItemMouseHoverEventArgs e) {
            if (onItemMouseHover != null) onItemMouseHover(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnItemSelectionChanged"]/*' />
        /// <devdoc>
        ///     Fires the ItemSelectionChanged event.
        /// </devdoc>
        protected virtual void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e) {
            ListViewItemSelectionChangedEventHandler eh = (ListViewItemSelectionChangedEventHandler)Events[EVENT_ITEMSELECTIONCHANGED];
            if (eh != null)
                eh(this, e);
        }

        protected override void OnParentChanged(EventArgs e) {
            base.OnParentChanged(e);

            // We must do this because the list view control caches the parent
            // handle and always sends notifications to the same handle.
            if (IsHandleCreated) {
                RecreateHandleInternal();
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListBox.OnResize"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected override void OnResize(EventArgs e) {
            // If the list view is in Details mode and it is not Scrollable, we need to reposition the column header control.
            if (this.View == View.Details && !this.Scrollable && this.IsHandleCreated) {
                PositionHeader();
            }

            base.OnResize(e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListBox.OnRetrieveVirtualItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected virtual void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e) {
            RetrieveVirtualItemEventHandler handler = (RetrieveVirtualItemEventHandler) Events[EVENT_RETRIEVEVIRTUALITEM];
            if (handler != null)
                handler(this, e);
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftLayoutChanged"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandleInternal();
            }

            EventHandler eh = Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }


        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnSearchForVirtualItem"]/*' />
        /// <devdoc>
        ///     Fires the search for virtual item event.
        /// </devdoc>
        protected virtual void OnSearchForVirtualItem(SearchForVirtualItemEventArgs e) {
            SearchForVirtualItemEventHandler handler = (SearchForVirtualItemEventHandler) Events[EVENT_SEARCHFORVIRTUALITEM];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnSelectedIndexChanged"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the selectedIndexChanged event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.onSelectedIndexChanged(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected virtual void OnSelectedIndexChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_SELECTEDINDEXCHANGED];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnSystemColorsChanged"]/*' />
        protected override void OnSystemColorsChanged(EventArgs e) {
            base.OnSystemColorsChanged(e);

            if (IsHandleCreated) {
                int color = ColorTranslator.ToWin32(BackColor);
                SendMessage(NativeMethods.LVM_SETBKCOLOR, 0,color);
                // We should probably be OK if we don't set the TEXTBKCOLOR to CLR_NONE.
                // However, for the sake of being more robust, reset the TECTBKCOLOR to CLR_NONE when the system palette changes.
                SendMessage(NativeMethods.LVM_SETTEXTBKCOLOR, 0,NativeMethods.CLR_NONE);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.OnVirtualItemSelectionRangeChanged"]/*' />
        protected virtual void OnVirtualItemsSelectionRangeChanged(ListViewVirtualItemsSelectionRangeChangedEventArgs e) {
            ListViewVirtualItemsSelectionRangeChangedEventHandler eh = (ListViewVirtualItemsSelectionRangeChangedEventHandler) Events[EVENT_VIRTUALITEMSSELECTIONRANGECHANGED];
            if (eh != null)
                eh(this, e);
        }

        private unsafe void PositionHeader() {
            IntPtr hdrHWND = UnsafeNativeMethods.GetWindow(new HandleRef(this, this.Handle), NativeMethods.GW_CHILD);
            if (hdrHWND != IntPtr.Zero) {

                IntPtr prc   = IntPtr.Zero;
                IntPtr pwpos = IntPtr.Zero;

                prc = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.RECT)));
                if (prc == IntPtr.Zero) {
                    return;
                }

                try {
                    pwpos = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.WINDOWPOS)));

                    if (prc == IntPtr.Zero) {
                        // we could not allocate memory.
                        // return
                        return;
                    }

                    UnsafeNativeMethods.GetClientRect(new HandleRef(this, this.Handle), prc);

                    NativeMethods.HDLAYOUT hd = new NativeMethods.HDLAYOUT();
                    hd.prc = prc;
                    hd.pwpos = pwpos;

                    // get the layout information
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, hdrHWND), NativeMethods.HDM_LAYOUT, 0, ref hd);

                    // now take the information from the native wpos struct and put it into a managed WINDOWPOS
                    NativeMethods.WINDOWPOS wpos = (NativeMethods.WINDOWPOS) Marshal.PtrToStructure(pwpos, typeof(NativeMethods.WINDOWPOS));

                    // position the header control
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, hdrHWND),
                                                   new HandleRef(this, wpos.hwndInsertAfter),
                                                   wpos.x,
                                                   wpos.y,
                                                   wpos.cx,
                                                   wpos.cy,
                                                   wpos.flags | NativeMethods.SWP_SHOWWINDOW);
                } finally {

                    // clean up our memory
                    if (prc != IntPtr.Zero) {
                        Marshal.FreeHGlobal(prc);
                    }
                    if (pwpos != IntPtr.Zero) {
                        Marshal.FreeHGlobal(pwpos);
                    }
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RealizeAllSubItems"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void RealizeAllSubItems() {
            NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
            for (int i = 0; i < itemCount; i++) {
                int subItemCount = Items[i].SubItems.Count;
                for (int j = 0; j < subItemCount; j++) {
                    SetItemText(i, j, Items[i].SubItems[j].Text, ref lvItem);
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RealizeProperties"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected void RealizeProperties() {
            //Realize state information
            Color c;

            c = BackColor;
            if (c != SystemColors.Window) {
                SendMessage(NativeMethods.LVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(c));
            }
            c = ForeColor;
            if (c != SystemColors.WindowText)
                SendMessage(NativeMethods.LVM_SETTEXTCOLOR, 0, ColorTranslator.ToWin32(c));


            if (null != imageListLarge)
                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_NORMAL, imageListLarge.Handle);

            if (null != imageListSmall)
                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_SMALL, imageListSmall.Handle);

            if (null != imageListState) {
                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, imageListState.Handle);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RedrawItems"]/*' />
        /// <devdoc>
        ///     Forces the redraw of a range of listview items.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void RedrawItems(int startIndex, int endIndex, bool invalidateOnly)
        {
            if (this.VirtualMode)
            {
                if (startIndex < 0 || startIndex >= this.VirtualListSize)
                {
                    throw new ArgumentOutOfRangeException("startIndex", string.Format(SR.InvalidArgument, "startIndex", (startIndex).ToString(CultureInfo.CurrentCulture)));
                }
                if (endIndex < 0 || endIndex >= this.VirtualListSize)
                {
                    throw new ArgumentOutOfRangeException("endIndex", string.Format(SR.InvalidArgument, "endIndex", (endIndex).ToString(CultureInfo.CurrentCulture)));
                }
            }
            else
            {
                if (startIndex < 0 || startIndex >= this.Items.Count)
                {
                    throw new ArgumentOutOfRangeException("startIndex", string.Format(SR.InvalidArgument, "startIndex", (startIndex).ToString(CultureInfo.CurrentCulture)));
                }
                if (endIndex < 0 || endIndex >= this.Items.Count)
                {
                    throw new ArgumentOutOfRangeException("endIndex", string.Format(SR.InvalidArgument, "endIndex", (endIndex).ToString(CultureInfo.CurrentCulture)));
                }
            }
            if (startIndex > endIndex)
            {
                throw new ArgumentException(SR.ListViewStartIndexCannotBeLargerThanEndIndex);
            }
            if (this.IsHandleCreated)
            {
                int retval = (int) UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle),
                                                                   NativeMethods.LVM_REDRAWITEMS,
                                                                   startIndex, endIndex);
                Debug.Assert(retval != 0);
                // ListView control seems to be bogus. Items affected need to be invalidated in LargeIcon and SmallIcons views.
                if (this.View == View.LargeIcon || this.View == View.SmallIcon)
                {
                    Rectangle rectInvalid = this.Items[startIndex].Bounds;
                    for (int index = startIndex+1; index <= endIndex; index++)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, this.Items[index].Bounds);
                    }
                    if (startIndex > 0)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, this.Items[startIndex - 1].Bounds);
                    }
                    else
                    {
                        rectInvalid.Width += rectInvalid.X;
                        rectInvalid.Height += rectInvalid.Y;
                        rectInvalid.X = rectInvalid.Y = 0;
                    }
                    if (endIndex < this.Items.Count - 1)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, this.Items[endIndex + 1].Bounds);
                    }
                    else
                    {
                        rectInvalid.Height += this.ClientRectangle.Bottom - rectInvalid.Bottom;
                        rectInvalid.Width += this.ClientRectangle.Right - rectInvalid.Right;
                    }
                    if (this.View == View.LargeIcon)
                    {
                        rectInvalid.Inflate(1, this.Font.Height + 1);
                    }
                    Invalidate(rectInvalid);
                }
                if (!invalidateOnly)
                {
                    Update();
                }
            }
        }

        // makes sure that the list view items which are w/o a listView group are parented to the DefaultGroup - if necessary
        // and then tell win32 to remove this group
        internal void RemoveGroupFromListView(ListViewGroup group) {
               EnsureDefaultGroup();

               foreach(ListViewItem item in group.Items) {
                    if(item.ListView == this) {
                         item.UpdateStateToListView(item.Index);
                    }
               }

               RemoveGroupNative(group);

               UpdateGroupView();
        }

        // does the job of telling win32 listView to remove this group
        private void RemoveGroupNative(ListViewGroup group) {
               Debug.Assert(IsHandleCreated,"RemoveGroupNative precondition: list-view handle must be created");
               Debug.Assert(ComctlSupportsVisualStyles, "we should have checked this already");
               int retval = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_REMOVEGROUP, group.ID, IntPtr.Zero);
               Debug.Assert(retval != -1,"Failed to remove group");

               // it is the job of whoever deletes this group to also turn off grouping if this was the last
               // group deleted
               return;
          }

        private void Scroll(int fromLVItem, int toLVItem) {
            int scrollY = GetItemPosition(toLVItem).Y - GetItemPosition(fromLVItem).Y;
            UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SCROLL, 0, scrollY);
        }

        private void SetBackgroundImage() {
            // needed for OleInitialize
            Application.OleRequired();

            NativeMethods.LVBKIMAGE lvbkImage = new NativeMethods.LVBKIMAGE();
            lvbkImage.xOffset = 0;
            lvbkImage.yOffset = 0;

            // first, is there an existing temporary file to delete, remember its name
            // so that we can delete it if the list control doesn't...
            string fileNameToDelete = this.backgroundImageFileName;

            if (this.BackgroundImage != null) {

                // the list view needs these permissions when the app runs on an UNC share
                // and the list view creates / destroys temporary files for its background image

                // 

                EnvironmentPermission envPermission = new EnvironmentPermission(EnvironmentPermissionAccess.Read, "TEMP");
                FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
                System.Security.PermissionSet permSet = new System.Security.PermissionSet(PermissionState.Unrestricted);
                permSet.AddPermission(envPermission);
                permSet.AddPermission(fiop);
                permSet.Assert();

                // save the image to a temporary file name
                try {
                    string tempDirName = System.IO.Path.GetTempPath();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
                    UnsafeNativeMethods.GetTempFileName(tempDirName, this.GenerateRandomName(), 0, sb);

                    this.backgroundImageFileName = sb.ToString();

                    this.BackgroundImage.Save(this.backgroundImageFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                } finally {
                    System.Security.PermissionSet.RevertAssert();
                }

                lvbkImage.pszImage = this.backgroundImageFileName;
                lvbkImage.cchImageMax = this.backgroundImageFileName.Length + 1;
                lvbkImage.ulFlags = NativeMethods.LVBKIF_SOURCE_URL;
                if (BackgroundImageTiled)
                    lvbkImage.ulFlags |= NativeMethods.LVBKIF_STYLE_TILE;
                else
                    lvbkImage.ulFlags |= NativeMethods.LVBKIF_STYLE_NORMAL;

            } else {
                lvbkImage.ulFlags = NativeMethods.LVBKIF_SOURCE_NONE;
                this.backgroundImageFileName = String.Empty;
            }

            UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETBKIMAGE, 0, lvbkImage);

            if (String.IsNullOrEmpty(fileNameToDelete)) {
                return;
            }

            // we need to cause a paint message on the win32 list view. This way the win 32 list view gives up
            // its reference to the previous image file it was holding on to.
            // 

            // 8 strings should be good enough for us
            if (this.bkImgFileNames == null) {
                this.bkImgFileNames = new string[BKIMGARRAYSIZE];
                this.bkImgFileNamesCount = -1;
            }

            if (this.bkImgFileNamesCount == BKIMGARRAYSIZE - 1) {
                // it should be fine to delete the file name that was added first.
                // if it's not fine, then increase BKIMGARRAYSIZE
                this.DeleteFileName(this.bkImgFileNames[0]);
                this.bkImgFileNames[0] = this.bkImgFileNames[1];
                this.bkImgFileNames[1] = this.bkImgFileNames[2];
                this.bkImgFileNames[2] = this.bkImgFileNames[3];
                this.bkImgFileNames[3] = this.bkImgFileNames[4];
                this.bkImgFileNames[4] = this.bkImgFileNames[5];
                this.bkImgFileNames[5] = this.bkImgFileNames[6];
                this.bkImgFileNames[6] = this.bkImgFileNames[7];
                this.bkImgFileNames[7] = null;

                this.bkImgFileNamesCount --;
            }

            this.bkImgFileNamesCount ++;
            this.bkImgFileNames[this.bkImgFileNamesCount] = fileNameToDelete;

            // now force the paint
            this.Refresh();
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SetColumnInfo"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void SetColumnInfo(int mask, ColumnHeader ch) {
            if (IsHandleCreated) {
                Debug.Assert((mask & ~(NativeMethods.LVCF_FMT | NativeMethods.LVCF_TEXT | NativeMethods.LVCF_IMAGE)) == 0, "Unsupported mask in setColumnInfo");
                NativeMethods.LVCOLUMN lvColumn = new NativeMethods.LVCOLUMN();
                lvColumn.mask  = mask;

                if ((mask & NativeMethods.LVCF_IMAGE) != 0 || (mask & NativeMethods.LVCF_FMT) != 0) {
                    // When we set the ImageIndex we also have to alter the column format.
                    // This means that we have to include the TextAlign into the column format.

                    lvColumn.mask |= NativeMethods.LVCF_FMT;

                    if (ch.ActualImageIndex_Internal > -1) {
                        // you would think that setting iImage would be enough.
                        // actually we also have to set the format to include LVCFMT_IMAGE
                        lvColumn.iImage = ch.ActualImageIndex_Internal;
                        lvColumn.fmt |= NativeMethods.LVCFMT_IMAGE;
                    }

                    lvColumn.fmt |= (int) ch.TextAlign;
                }

                if ((mask & NativeMethods.LVCF_TEXT) != 0) {
                    lvColumn.pszText    = Marshal.StringToHGlobalAuto(ch.Text);
                }

                int retval = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETCOLUMN, ch.Index, lvColumn);
                if ((mask & NativeMethods.LVCF_TEXT) != 0) {
                    Marshal.FreeHGlobal(lvColumn.pszText);
                }

                if (0 == retval)
                    throw new InvalidOperationException(SR.ListViewColumnInfoSet);
                // When running on AMD64 the list view does not invalidate the column header.
                // So we do it ourselves.
                InvalidateColumnHeaders();
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SetColumnWidth"]/*' />
        /// <devdoc>
        ///     Setting width is a special case 'cuz LVM_SETCOLUMNWIDTH accepts more values
        ///     for width than LVM_SETCOLUMN does.
        /// </devdoc>
        /// <internalonly/>
        internal void SetColumnWidth(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize) {
            if ((columnIndex < 0) ||
                (columnIndex >= 0 && this.columnHeaders == null) ||
                (columnIndex >= this.columnHeaders.Length)) {
                throw new ArgumentOutOfRangeException("columnIndex", string.Format(SR.InvalidArgument, "columnIndex", (columnIndex).ToString(CultureInfo.CurrentCulture)));
            }

            //valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(headerAutoResize, (int)headerAutoResize, (int)ColumnHeaderAutoResizeStyle.None, (int)ColumnHeaderAutoResizeStyle.ColumnContent))
            {
                throw new InvalidEnumArgumentException("headerAutoResize", (int)headerAutoResize, typeof(ColumnHeaderAutoResizeStyle));
            }


            int width = 0;
            int compensate = 0;

            if (headerAutoResize == ColumnHeaderAutoResizeStyle.None) {
                width = this.columnHeaders[columnIndex].WidthInternal;

                // If the width maps to a LVCSW_ const, then native control will autoresize.
                // We may need to compensate for that.
                if (width == NativeMethods.LVSCW_AUTOSIZE_USEHEADER) {
                    headerAutoResize = ColumnHeaderAutoResizeStyle.HeaderSize;
                } else if (width == NativeMethods.LVSCW_AUTOSIZE) {
                    headerAutoResize = ColumnHeaderAutoResizeStyle.ColumnContent;
                }
            }

            if (headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize) {
                compensate = this.CompensateColumnHeaderResize(columnIndex, false /*columnHeaderResizeCancelled*/);
                width = NativeMethods.LVSCW_AUTOSIZE_USEHEADER;
            } else if (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent) {
                compensate = this.CompensateColumnHeaderResize(columnIndex, false /*columnHeaderResizeCancelled*/);
                width = NativeMethods.LVSCW_AUTOSIZE;
            }

            if (IsHandleCreated) {
                SendMessage(NativeMethods.LVM_SETCOLUMNWIDTH, columnIndex, NativeMethods.Util.MAKELPARAM(width, 0));
            }

            if (this.IsHandleCreated &&
               (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent ||
                headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize)) {
                if (compensate != 0) {
                    int newWidth = this.columnHeaders[columnIndex].Width + compensate;
                    SendMessage(NativeMethods.LVM_SETCOLUMNWIDTH, columnIndex, NativeMethods.Util.MAKELPARAM(newWidth, 0));
                }
            }
        }

        private void SetColumnWidth(int index, int width) {
            if (IsHandleCreated) {
                SendMessage(NativeMethods.LVM_SETCOLUMNWIDTH, index, NativeMethods.Util.MAKELPARAM(width,0));
            }
        }

        // set the display indices of the listview columns
        private void SetDisplayIndices(int[] indices) {

            int[] orderedColumns = new int[ indices.Length ];
            for (int i=0; i<indices.Length; i++) {
                this.Columns[i].DisplayIndexInternal = indices[i];
                orderedColumns[indices[i]] = i;
            }

            if (this.IsHandleCreated && !this.Disposing) {
               UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETCOLUMNORDERARRAY, orderedColumns.Length, orderedColumns);
        }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.RemovefromSavedCheckedItems"]/*' />
        /// <devdoc>
        ///     This is a new internal method added which is used by ListView Item to set
        ///     the check state of the item in the savedCheckedItems collection
        ///     if the ListView Checkboxes is OFF.
        /// </devdoc>
        /// <internalonly/>
        internal void UpdateSavedCheckedItems(ListViewItem item, bool addItem)
        {
            if (addItem && this.savedCheckedItems == null) {
                this.savedCheckedItems = new List<ListViewItem>();
            }

            if (addItem) {
                this.savedCheckedItems.Add(item);
            } else if (this.savedCheckedItems != null) {
                Debug.Assert(this.savedCheckedItems.Contains(item), "somehow we lost track of one item");
                this.savedCheckedItems.Remove(item);
            }
            #if FALSE
            if (savedCheckedItems != null && savedCheckedItems.Length > 0) {
                ListViewItem[] newSavedCheckedItems;
                int index = 0;
                if (!addItem) {
                    int current = 0;
                    newSavedCheckedItems = new ListViewItem[savedCheckedItems.Length -1];
                    for(index = 0 ; index < savedCheckedItems.Length ; index++)
                    {
                        if (savedCheckedItems[index] == item) {
                            current = 1;
                            continue;
                        }
                        newSavedCheckedItems[index - current] = savedCheckedItems[index];
                    }
                }
                else {
                    newSavedCheckedItems = new ListViewItem[savedCheckedItems.Length +1];
                    for(index = 0 ; index < savedCheckedItems.Length ; index++)
                    {
                        newSavedCheckedItems[index] = savedCheckedItems[index];
                    }
                    newSavedCheckedItems[index] = item;
                }
                savedCheckedItems = newSavedCheckedItems;
            }
            else if (addItem) {
                savedCheckedItems = new ListViewItem[1];
                savedCheckedItems[0] = item;
            }
            #endif // FALSE
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SetToolTip"]/*' />
        /// <devdoc>
        ///     Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </devdoc>
        /// <internalonly/>
        internal void SetToolTip(ToolTip toolTip, string toolTipCaption) {
            this.toolTipCaption = toolTipCaption;
            //native ListView expects tooltip HWND as a wParam and ignores lParam
            IntPtr oldHandle = UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_SETTOOLTIPS, new HandleRef(toolTip, toolTip.Handle), 0);
            UnsafeNativeMethods.DestroyWindow(new HandleRef(null, oldHandle));
        }

        internal void SetItemImage(int index, int image) {
            if (index < 0 || ((this.VirtualMode && index >= this.VirtualListSize) || (!this.VirtualMode && index >= itemCount))) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            if (this.IsHandleCreated) {
                NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                lvItem.mask = NativeMethods.LVIF_IMAGE;
                lvItem.iItem = index;
                lvItem.iImage = image;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETITEM, 0, ref lvItem);
            }
        }

        internal void SetItemIndentCount(int index, int indentCount) {
            if (index < 0 || ((this.VirtualMode && index >= this.VirtualListSize) || (!this.VirtualMode && index >= itemCount))) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            if (this.IsHandleCreated) {
                NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                lvItem.mask = NativeMethods.LVIF_INDENT;
                lvItem.iItem = index;
                lvItem.iIndent = indentCount;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETITEM, 0, ref lvItem);
            }
        }

        internal void SetItemPosition(int index, int x, int y) {
            if (VirtualMode)
                return;
            if (index < 0 || index >= itemCount)
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

            Debug.Assert(IsHandleCreated, "How did we add items without a handle?");

            NativeMethods.POINT pt = new NativeMethods.POINT();
            pt.x = x;
            pt.y = y;
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETITEMPOSITION32, index, pt);
        }

        internal void SetItemState(int index, int state, int mask) {
            if (index < -1 || ((this.VirtualMode && index >= this.VirtualListSize) || (!this.VirtualMode && index >= itemCount))) {
                throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }
            Debug.Assert(index == -1 || this.IsHandleCreated, "How did we add items without a handle?");

            if (this.IsHandleCreated) {
                NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                lvItem.mask = NativeMethods.LVIF_STATE;
                lvItem.state = state;
                lvItem.stateMask = mask;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETITEMSTATE, index, ref lvItem);
            }
        }

        internal void SetItemText(int itemIndex, int subItemIndex, string text) {

            NativeMethods.LVITEM lvItem =  new NativeMethods.LVITEM();
            SetItemText(itemIndex, subItemIndex, text, ref lvItem);
        }

        ///<devdoc>
        /// For perf, allow a LVITEM to be passed in so we can reuse in tight loops.
        ///</devdoc>
        private void SetItemText(int itemIndex, int subItemIndex, string text, ref NativeMethods.LVITEM lvItem) {

            Debug.Assert(IsHandleCreated, "SetItemText with no handle");

            // 

            if (this.View == View.List && subItemIndex == 0) {
                int colWidth = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_GETCOLUMNWIDTH, 0, 0));

                Graphics g = this.CreateGraphicsInternal();
                int textWidth = 0;

                try {
                    textWidth = Size.Ceiling(g.MeasureString(text, this.Font)).Width;
                }
                finally {
                    g.Dispose();
                }

                if (textWidth > colWidth) {
                    SetColumnWidth(0, textWidth);
                }
            }

            lvItem.mask = NativeMethods.LVIF_TEXT;
            lvItem.iItem    = itemIndex;
            lvItem.iSubItem = subItemIndex;
            lvItem.pszText  = text;

            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SETITEMTEXT, itemIndex, ref lvItem);
        }

        //
        // ComCtl32 list view uses a selection mark to keep track of selection state - iMark.
        // ComCtl32 list view updates iMark only when the user hovers over the item.
        // This means that if we programatically set the selection item, then the list view will not update
        // its selection mark.
        // So we explicitly set the selection mark.
        //
        internal void SetSelectionMark(int itemIndex) {
            if (itemIndex < 0 || itemIndex >= this.Items.Count) {
                return;
            }
            SendMessage(NativeMethods.LVM_SETSELECTIONMARK, 0, itemIndex);
        }

        private void SmallImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated) {
                IntPtr handle = (SmallImageList == null) ? IntPtr.Zero : SmallImageList.Handle;
                SendMessage(NativeMethods.LVM_SETIMAGELIST, (IntPtr) NativeMethods.LVSIL_SMALL, handle);

                ForceCheckBoxUpdate();
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.Sort"]/*' />
        /// <devdoc>
        ///      Updated the sorted order
        /// </devdoc>
        public void Sort() {
            if (VirtualMode) {
                throw new InvalidOperationException(SR.ListViewSortNotAllowedInVirtualListView);
            }

            ApplyUpdateCachedItems();
            if (IsHandleCreated && listItemSorter != null) {
                NativeMethods.ListViewCompareCallback callback = new NativeMethods.ListViewCompareCallback(this.CompareFunc);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_SORTITEMS, IntPtr.Zero, callback);
            }
        }

        private void StateImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated) {
                IntPtr handle = IntPtr.Zero;
                if (StateImageList != null) {
                    handle = imageListState.Handle;
                }
                SendMessage(NativeMethods.LVM_SETIMAGELIST, (IntPtr) NativeMethods.LVSIL_STATE, handle);
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();

            if (listItemsArray != null) {
                s += ", Items.Count: " + listItemsArray.Count.ToString(CultureInfo.CurrentCulture);
                if (listItemsArray.Count > 0) {
                    string z = listItemsArray[0].ToString();
                    string txt = (z.Length > 40) ? z.Substring(0, 40) : z;
                    s += ", Items[0]: " + txt;
                }
            }
            else if (Items != null) {
                s += ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
                if (Items.Count > 0 && !VirtualMode) {
                    //Debug.Assert(Items[0] != null, "Why is there a null item in the list view item collection?");
                    string z = (Items[0] == null) ?  "null" : Items[0].ToString();
                    string txt = (z.Length > 40) ? z.Substring(0, 40) : z;
                    s += ", Items[0]: " + txt;
                }

            }
            return s;
        }

        internal void UpdateListViewItemsLocations() {
            if (!VirtualMode && IsHandleCreated && AutoArrange && (View == View.LargeIcon || View == View.SmallIcon)) {

                // this only has an affect for large icon and small icon views.
                //
                try {
                    BeginUpdate();
                    /*
                       Not sure this does anything that just calling the function doesn't...

                      for (int i = 0; i < this.itemCount; i ++) {
                        UnsafeNativeMethods.PostMessage(new HandleRef(this, this.Handle), NativeMethods.LVM_UPDATE, this.Items[i].Index, 0);
                    } */
                    SendMessage(NativeMethods.LVM_UPDATE, -1, 0);
                }
                finally {
                    EndUpdate();
                }
            }

        }

        private void UpdateColumnWidths(ColumnHeaderAutoResizeStyle headerAutoResize) {
            if (columnHeaders != null) {
                for (int i = 0; i < this.columnHeaders.Length; i ++) {
                    SetColumnWidth(i, headerAutoResize);
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.UpdateExtendedStyles"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected void UpdateExtendedStyles() {

            if (IsHandleCreated) {
                int exStyle = 0;
                int exMask = NativeMethods.LVS_EX_ONECLICKACTIVATE | NativeMethods.LVS_EX_TWOCLICKACTIVATE |
                             NativeMethods.LVS_EX_TRACKSELECT | NativeMethods.LVS_EX_UNDERLINEHOT | NativeMethods.LVS_EX_ONECLICKACTIVATE |
                             NativeMethods.LVS_EX_HEADERDRAGDROP | NativeMethods.LVS_EX_CHECKBOXES |
                             NativeMethods.LVS_EX_FULLROWSELECT | NativeMethods.LVS_EX_GRIDLINES |
                             NativeMethods.LVS_EX_INFOTIP | NativeMethods.LVS_EX_DOUBLEBUFFER;

                switch (activation) {
                    case ItemActivation.OneClick:
                        exStyle |= NativeMethods.LVS_EX_ONECLICKACTIVATE;
                        break;
                    case ItemActivation.TwoClick:
                        exStyle |= NativeMethods.LVS_EX_TWOCLICKACTIVATE;
                        break;
                }

                if (AllowColumnReorder)
                    exStyle |= NativeMethods.LVS_EX_HEADERDRAGDROP;

                if (CheckBoxes)
                    exStyle |= NativeMethods.LVS_EX_CHECKBOXES;

                if (DoubleBuffered)
                    exStyle |= NativeMethods.LVS_EX_DOUBLEBUFFER;

                if (FullRowSelect)
                    exStyle |= NativeMethods.LVS_EX_FULLROWSELECT;

                if (GridLines)
                    exStyle |= NativeMethods.LVS_EX_GRIDLINES;

                if (HoverSelection)
                    exStyle |= NativeMethods.LVS_EX_TRACKSELECT;

                if (HotTracking)
                    exStyle |= NativeMethods.LVS_EX_UNDERLINEHOT;

                if (ShowItemToolTips)
                    exStyle |= NativeMethods.LVS_EX_INFOTIP;

                SendMessage(NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, exMask, exStyle);
                Invalidate();
            }
        }

        internal void UpdateGroupNative(ListViewGroup group) {
            Debug.Assert(IsHandleCreated,"UpdateGroupNative precondition: list-view handle must be created");
            Debug.Assert(ComctlSupportsVisualStyles, "we should have checked this already");

            NativeMethods.LVGROUP lvgroup = new NativeMethods.LVGROUP();
            try {
                lvgroup = GetLVGROUP(group);
                int retval = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle),
                                                                  NativeMethods.LVM_SETGROUPINFO,
                                                                  group.ID,
                                                                  lvgroup);
                Debug.Assert(retval != -1, "Failed to set group info");
            }
            finally
            {
                DestroyLVGROUP(lvgroup);
            }

            // The comctl32 ListView does not correctly invalidate itself, so we need to invalidate the entire ListView
            //
            Invalidate();
        }

        // ListViewGroupCollection::Clear needs to remove the items from the Default group
        //
        internal void UpdateGroupView() {
            if (IsHandleCreated && ComctlSupportsVisualStyles && !VirtualMode) {
                int retval = unchecked( (int) (long)SendMessage(NativeMethods.LVM_ENABLEGROUPVIEW, GroupsEnabled ? 1 : 0, 0));
                Debug.Assert(retval != -1, "Error enabling group view");
            }
        }

        // updates the win32 list view w/ our tile info - columns + tile size
        private void UpdateTileView() {
            Debug.Assert(ComctlSupportsVisualStyles, "this function works only when ComCtl 6.0 and higher is loaded");
            Debug.Assert(this.viewStyle == View.Tile, "this function should be called only in Tile view");
            NativeMethods.LVTILEVIEWINFO tileViewInfo = new NativeMethods.LVTILEVIEWINFO();
            // the tile view info line count
            tileViewInfo.dwMask = NativeMethods.LVTVIM_COLUMNS;
            tileViewInfo.cLines = this.columnHeaders != null ? this.columnHeaders.Length : 0;

            // the tile view info size
            tileViewInfo.dwMask |= NativeMethods.LVTVIM_TILESIZE;
            tileViewInfo.dwFlags = NativeMethods.LVTVIF_FIXEDSIZE;
            tileViewInfo.sizeTile = new NativeMethods.SIZE(this.TileSize.Width, this.TileSize.Height);

            bool retval = UnsafeNativeMethods.SendMessage(new HandleRef(this,Handle),NativeMethods.LVM_SETTILEVIEWINFO,0,tileViewInfo);
            Debug.Assert(retval,"LVM_SETTILEVIEWINFO failed");
        }

        private void WmNmClick(ref Message m) {

            // If we're checked, hittest to see if we're
            // on the check mark

            if (CheckBoxes) {
                Point pos = Cursor.Position;
                pos = PointToClientInternal(pos);
                NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();

                lvhi.pt_x = pos.X;
                lvhi.pt_y = pos.Y;

                int displayIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_HITTEST, 0, lvhi);
                if (displayIndex != -1 && (lvhi.flags & NativeMethods.LVHT_ONITEMSTATEICON) != 0) {
                    ListViewItem clickedItem = Items[displayIndex];
                    if (clickedItem.Selected) {
                        bool check = !clickedItem.Checked;
                        if (!VirtualMode) {
                            foreach(ListViewItem item in SelectedItems) {
                                if (item != clickedItem) {
                                    item.Checked = check;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void WmNmDblClick(ref Message m) {

            // If we're checked, hittest to see if we're
            // on the item

            if (CheckBoxes) {
                Point pos = Cursor.Position;
                pos = PointToClientInternal(pos);
                NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();

                lvhi.pt_x = pos.X;
                lvhi.pt_y = pos.Y;

                int displayIndex = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_HITTEST, 0, lvhi);
                if (displayIndex != -1 && (lvhi.flags & NativeMethods.LVHT_ONITEM) != 0) {
                    ListViewItem clickedItem = Items[displayIndex];
                    clickedItem.Checked = !clickedItem.Checked;
                }
            }
        }

        private void WmMouseDown(ref Message m, MouseButtons button, int clicks) {

            //Always Reset the MouseupFired....
            listViewState[LISTVIEWSTATE_mouseUpFired] = false;
            listViewState[LISTVIEWSTATE_expectingMouseUp] = true;

            //This is required to FORCE Validation before Windows ListView pushes its own message loop...
            FocusInternal();

            // Windows ListView pushes its own Windows ListView in WM_xBUTTONDOWN, so fire the
            // event before calling defWndProc or else it won't get fired until the button
            // comes back up.
            int x = NativeMethods.Util.SignedLOWORD(m.LParam);
            int y = NativeMethods.Util.SignedHIWORD(m.LParam);
            OnMouseDown(new MouseEventArgs(button, clicks, x, y, 0));

            //If Validation is cancelled dont fire any events through the Windows ListView's message loop...
            if (!ValidationCancelled) {

                if (CheckBoxes) {
                    ListViewHitTestInfo lvhti = this.HitTest(x, y);
                    if (imageListState != null && imageListState.Images.Count < 2) {
                        // When the user clicks on the check box and the listView's state image list
                        // does not have 2 images, comctl will give us an AttemptToDivideByZero exception.
                        // So don't send the message to DefWndProc in this situation.
                        if (lvhti.Location != ListViewHitTestLocations.StateImage) {
                            DefWndProc(ref m);
                        }
                    }
                    else {
                        // When a user clicks on the state image, focus the item.
                        if (AccessibilityImprovements.Level2 && lvhti.Item != null && lvhti.Location == ListViewHitTestLocations.StateImage) {
                            lvhti.Item.Focused = true;
                        }
                        DefWndProc(ref m);
                    }
                }
                else
                {
                    DefWndProc(ref m);
                }
            }

        }

        private unsafe bool WmNotify(ref Message m) {
            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;

            // column header custom draw message handling
            if (nmhdr->code == NativeMethods.NM_CUSTOMDRAW && OwnerDraw)
            {
                try
                {
                    NativeMethods.NMCUSTOMDRAW* nmcd = (NativeMethods.NMCUSTOMDRAW*)m.LParam;
                    // Find out which stage we're drawing
                    switch (nmcd->dwDrawStage)
                    {
                        case NativeMethods.CDDS_PREPAINT:
                            {
                                m.Result = (IntPtr)(NativeMethods.CDRF_NOTIFYITEMDRAW);
                                return true; // we are done - don't do default handling

                            }
                        case NativeMethods.CDDS_ITEMPREPAINT:
                            {
                                Graphics g = Graphics.FromHdcInternal(nmcd->hdc);
                                Rectangle r = Rectangle.FromLTRB(nmcd->rc.left, nmcd->rc.top, nmcd->rc.right, nmcd->rc.bottom);
                                DrawListViewColumnHeaderEventArgs e = null;

                                try
                                {
                                    Color foreColor = ColorTranslator.FromWin32(SafeNativeMethods.GetTextColor(new HandleRef(this, nmcd->hdc)));
                                    Color backColor = ColorTranslator.FromWin32(SafeNativeMethods.GetBkColor(new HandleRef(this, nmcd->hdc)));
                                    Font font = GetListHeaderFont();
                                    e = new DrawListViewColumnHeaderEventArgs(g, r, (int)(nmcd->dwItemSpec),
                                                                        columnHeaders[(int)nmcd->dwItemSpec],
                                                                        (ListViewItemStates)(nmcd->uItemState),
                                                                        foreColor, backColor, font);

                                    OnDrawColumnHeader(e);
                                }
                                finally
                                {
                                    g.Dispose();
                                }

                                if (e.DrawDefault)
                                {
                                    m.Result = (IntPtr)(NativeMethods.CDRF_DODEFAULT);
                                    return false;
                                }
                                else
                                {
                                
                                    m.Result = (IntPtr)(NativeMethods.CDRF_SKIPDEFAULT);
                                    return true; // we are done - don't do default handling
                                }
                            }

                        default:
                            return false; //default handling
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail("Exception occurred attempting to setup header custom draw. Disabling custom draw for the column header", e.ToString());
                    m.Result = (IntPtr)NativeMethods.CDRF_DODEFAULT;
                }
            }


            if (nmhdr->code == NativeMethods.NM_RELEASEDCAPTURE && listViewState[LISTVIEWSTATE_columnClicked]) {
                listViewState[LISTVIEWSTATE_columnClicked] = false;
                OnColumnClick(new ColumnClickEventArgs(columnIndex));
            }

            if (nmhdr->code == NativeMethods.HDN_BEGINTRACKA ||
                nmhdr->code == NativeMethods.HDN_BEGINTRACKW) {
               this.listViewState[LISTVIEWSTATE_headerControlTracking] = true;

               // Reset our tracking information for the new BEGINTRACK cycle.
               this.listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
               this.newWidthForColumnWidthChangingCancelled = -1;
               this.listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;

               NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));
               if (this.columnHeaders != null && this.columnHeaders.Length > nmheader.iItem) {
                   this.columnHeaderClicked = this.columnHeaders[nmheader.iItem];
                   this.columnHeaderClickedWidth = this.columnHeaderClicked.Width;
               } else {
                   this.columnHeaderClickedWidth = -1;
                   this.columnHeaderClicked = null;
               }
            }

            if (nmhdr->code == NativeMethods.HDN_ITEMCHANGINGA || 
                nmhdr->code == NativeMethods.HDN_ITEMCHANGINGW) {
                NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));

                if (columnHeaders != null && nmheader.iItem < columnHeaders.Length &&
                    (this.listViewState[LISTVIEWSTATE_headerControlTracking] || this.listViewState[LISTVIEWSTATE_headerDividerDblClick])) {
                    // 



                    NativeMethods.HDITEM2 hdItem = (NativeMethods.HDITEM2) UnsafeNativeMethods.PtrToStructure((IntPtr) nmheader.pItem, typeof(NativeMethods.HDITEM2));
                    int newColumnWidth = ((hdItem.mask & NativeMethods.HDI_WIDTH) != 0) ? hdItem.cxy : -1;
                    ColumnWidthChangingEventArgs colWidthChanging = new ColumnWidthChangingEventArgs(nmheader.iItem, newColumnWidth);
                    OnColumnWidthChanging(colWidthChanging);
                    m.Result = (IntPtr) (colWidthChanging.Cancel ? 1 : 0);
                    if (colWidthChanging.Cancel) {
                        hdItem.cxy = colWidthChanging.NewWidth;

                        // We are called inside HDN_DIVIDERDBLCLICK.
                        // Turn off the compensation that our processing of HDN_DIVIDERDBLCLICK would otherwise add.
                        if (this.listViewState[LISTVIEWSTATE_headerDividerDblClick]) {
                            this.listViewState[LISTVIEWSTATE_columnResizeCancelled] = true;
                        }

                        this.listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = true;
                        this.newWidthForColumnWidthChangingCancelled = colWidthChanging.NewWidth;

                        // skip default processing
                        return true;
                    } else {
                        return false;
                    }
                }
            }

            if ((nmhdr->code == NativeMethods.HDN_ITEMCHANGEDA ||
                nmhdr->code == NativeMethods.HDN_ITEMCHANGEDW) &&
                !this.listViewState[LISTVIEWSTATE_headerControlTracking]) {
                NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER)m.GetLParam(typeof(NativeMethods.NMHEADER));
                if (columnHeaders != null && nmheader.iItem < columnHeaders.Length) {
                    int w = columnHeaders[nmheader.iItem].Width;

                    if (this.columnHeaderClicked == null ||
                        (this.columnHeaderClicked == this.columnHeaders[nmheader.iItem] &&
                         this.columnHeaderClickedWidth != -1 &&
                         this.columnHeaderClickedWidth != w)) {

                        //
                        // If the user double clicked on the column header and we still need to compensate for the column resize
                        // then don't fire ColumnWidthChanged because at this point the column header does not have the final width.
                        //
                        if (this.listViewState[LISTVIEWSTATE_headerDividerDblClick]) {
                            if (this.CompensateColumnHeaderResize(m, this.listViewState[LISTVIEWSTATE_columnResizeCancelled]) == 0) {
                                OnColumnWidthChanged(new ColumnWidthChangedEventArgs(nmheader.iItem));
                            }
                        } else {
                            OnColumnWidthChanged(new ColumnWidthChangedEventArgs(nmheader.iItem));
                        }

                    }
                }

                this.columnHeaderClicked = null;
                this.columnHeaderClickedWidth = -1;

                ISite site = Site;

                if (site != null) {
                    IComponentChangeService cs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                    if (cs != null) {
                        try {
                            cs.OnComponentChanging(this, null);
                        }
                        catch (CheckoutException coEx) {
                            if (coEx == CheckoutException.Canceled) {
                                return false;
                            }
                            throw coEx;
                        }
                    }
                }
            }

            if (nmhdr->code == NativeMethods.HDN_ENDTRACKA || 
                nmhdr->code == NativeMethods.HDN_ENDTRACKW) {
                Debug.Assert(this.listViewState[LISTVIEWSTATE_headerControlTracking], "HDN_ENDTRACK and HDN_BEGINTRACK are out of sync...");
                this.listViewState[LISTVIEWSTATE_headerControlTracking] = false;
                if (this.listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging]) {
                    m.Result = (IntPtr) 1;
                    if (this.newWidthForColumnWidthChangingCancelled != -1) {
                        NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER)m.GetLParam(typeof(NativeMethods.NMHEADER));
                        if (this.columnHeaders != null && this.columnHeaders.Length > nmheader.iItem) {
                            this.columnHeaders[nmheader.iItem].Width = this.newWidthForColumnWidthChangingCancelled;
                        }
                    }

                    this.listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
                    this.newWidthForColumnWidthChangingCancelled = -1;

                    // skip default processing
                    return true;
                } else {
                    return false;
                }
            }

            if (nmhdr->code == NativeMethods.HDN_ENDDRAG) {
                NativeMethods.NMHEADER header = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));
                if (header.pItem != IntPtr.Zero) {
                    // 



                    NativeMethods.HDITEM2 hdItem = (NativeMethods.HDITEM2) UnsafeNativeMethods.PtrToStructure((IntPtr) header.pItem, typeof(NativeMethods.HDITEM2));
                    if ((hdItem.mask & NativeMethods.HDI_ORDER) == NativeMethods.HDI_ORDER) {

                        int from = this.Columns[header.iItem].DisplayIndex;
                        int to = hdItem.iOrder;
                        // check this
                        if (from == to) {
                            return false;
                        }

                        // sometimes ComCtl gives us bogus values for HDIItem.iOrder.
                        if (to < 0) {
                            return false;
                        }
                        ColumnReorderedEventArgs chrevent = new ColumnReorderedEventArgs(from,
                                                                                         to,
                                                                                         this.Columns[header.iItem]);
                        OnColumnReordered(chrevent);
                        if (chrevent.Cancel) {
                            m.Result = new IntPtr(1);
                            return true;
                        } else {
                            // set the display indices. This is not an expensive operation because
                            // we only set an integer in the column header class
                            int lowDI = Math.Min(from, to);
                            int hiDI = Math.Max(from, to);
                            bool hdrMovedForward = to > from;
                            ColumnHeader movedHdr = null;
                            int[] indices = new int[this.Columns.Count];
                            for (int i = 0; i < this.Columns.Count; i ++) {

                                ColumnHeader hdr = this.Columns[i];
                                if (hdr.DisplayIndex == from) {
                                    movedHdr = hdr;
                                } else if (hdr.DisplayIndex >= lowDI && hdr.DisplayIndex <= hiDI) {
                                    hdr.DisplayIndexInternal -= hdrMovedForward ? 1 : -1;
                                }
                                indices[i] = hdr.DisplayIndexInternal;
                            }

                            movedHdr.DisplayIndexInternal = to;
                            indices[movedHdr.Index] = movedHdr.DisplayIndexInternal;
                            SetDisplayIndices( indices );
#if DEBUG
                            CheckDisplayIndices();
#endif
                        }
                    }
                }
            }

            if (nmhdr->code == NativeMethods.HDN_DIVIDERDBLCLICKA ||
                nmhdr->code == NativeMethods.HDN_DIVIDERDBLCLICKW) {
                // We need to keep track that the user double clicked the column header divider
                // so we know that the column header width is changing.
                this.listViewState[LISTVIEWSTATE_headerDividerDblClick] = true;

                // Reset ColumnResizeCancelled.
                // It will be set if the user cancels the ColumnWidthChanging event.
                this.listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;

                bool columnResizeCancelled = false;

                // ComCtl32 does not add enough padding when resizing the first column via mouse double click.
                // See for a complete explanation including listing of the comctl32 code.
                // Our wrapper will add 2 pixels. (1 pixel is not enough, 3 pixels is too much)

                // Send the message to ComCtl32 so that it resizes the column.
                try
                {
                    DefWndProc(ref m);
                }
                finally
                {
                    this.listViewState[LISTVIEWSTATE_headerDividerDblClick] = false;
                    columnResizeCancelled = this.listViewState[LISTVIEWSTATE_columnResizeCancelled];
                    this.listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;
                }


                this.columnHeaderClicked = null;
                this.columnHeaderClickedWidth = -1;

                if (columnResizeCancelled) {
                    // If the column resize was cancelled then apply the NewWidth supplied by the user.
                    if (this.newWidthForColumnWidthChangingCancelled != -1) {
                        NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));
                        if (this.columnHeaders != null && this.columnHeaders.Length > nmheader.iItem) {
                            this.columnHeaders[nmheader.iItem].Width = this.newWidthForColumnWidthChangingCancelled;
                        }
                    }

                    // Tell ComCtl that the HDN_DIVIDERDBLCLICK was cancelled.
                    m.Result = (IntPtr) 1;
                } else {
                    // Compensate for the column resize.
                    int compensateForColumnResize = this.CompensateColumnHeaderResize(m, columnResizeCancelled);
                    if (compensateForColumnResize != 0) {
                        #if DEBUG
                        NativeMethods.NMHEADER header = (NativeMethods.NMHEADER) m.GetLParam(typeof(NativeMethods.NMHEADER));
                        Debug.Assert(header.iItem == 0, "we only need to compensate for the first column resize");
                        Debug.Assert(this.columnHeaders.Length > 0, "there should be a column that we need to compensate for");
                        #endif

                        ColumnHeader col = this.columnHeaders[0];
                        col.Width += compensateForColumnResize;
                    }
                }

                // We called DefWndProc so we don't need default handling.
                return true;
            }

            return false; // still need default handling
        }

        private Font GetListHeaderFont(){
            IntPtr hwndHdr = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_GETHEADER, 0, 0);
            IntPtr hFont = UnsafeNativeMethods.SendMessage(new HandleRef(this, hwndHdr), NativeMethods.WM_GETFONT, 0, 0);
            IntSecurity.ObjectFromWin32Handle.Assert();
            return Font.FromHfont(hFont);
        }



        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.GetIndexOfClickedItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private int GetIndexOfClickedItem(NativeMethods.LVHITTESTINFO lvhi) {
            Point pos = Cursor.Position;
            pos = PointToClientInternal(pos);
            lvhi.pt_x = pos.X;
            lvhi.pt_y = pos.Y;
            return (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.LVM_HITTEST, 0, lvhi);

        }

        internal void RecreateHandleInternal() {
            // 
            // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native listView destroys
            // the state imageList.
            // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
            if (this.IsHandleCreated && this.StateImageList != null) {
                SendMessage(NativeMethods.LVM_SETIMAGELIST, NativeMethods.LVSIL_STATE, IntPtr.Zero);
            }

            RecreateHandle();
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.WmNotify"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        private unsafe void WmReflectNotify(ref Message m) {

            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;

            switch (nmhdr->code) {
                case NativeMethods.NM_CUSTOMDRAW:
                    CustomDraw(ref m);
                    break;

                case NativeMethods.LVN_BEGINLABELEDITA:
                case NativeMethods.LVN_BEGINLABELEDITW: {
                        NativeMethods.NMLVDISPINFO_NOTEXT nmlvdp = (NativeMethods.NMLVDISPINFO_NOTEXT)m.GetLParam(typeof(NativeMethods.NMLVDISPINFO_NOTEXT));
                        LabelEditEventArgs e = new LabelEditEventArgs(nmlvdp.item.iItem);
                        OnBeforeLabelEdit(e);
                        m.Result = (IntPtr)(e.CancelEdit ? 1 : 0);
                        listViewState[LISTVIEWSTATE_inLabelEdit] = !e.CancelEdit;
                        break;
                    }

                case NativeMethods.LVN_COLUMNCLICK: {
                        NativeMethods.NMLISTVIEW nmlv = (NativeMethods.NMLISTVIEW)m.GetLParam(typeof(NativeMethods.NMLISTVIEW));
                        listViewState[LISTVIEWSTATE_columnClicked] = true;
                        columnIndex = nmlv.iSubItem;
                        break;
                    }

                case NativeMethods.LVN_ENDLABELEDITA:
                case NativeMethods.LVN_ENDLABELEDITW: {
                        listViewState[LISTVIEWSTATE_inLabelEdit] = false;
                        NativeMethods.NMLVDISPINFO nmlvdp = (NativeMethods.NMLVDISPINFO)m.GetLParam(typeof(NativeMethods.NMLVDISPINFO));
                        LabelEditEventArgs e = new LabelEditEventArgs(nmlvdp.item.iItem, nmlvdp.item.pszText);
                        OnAfterLabelEdit(e);
                        m.Result = (IntPtr)(e.CancelEdit ? 0 : 1);
                        // from msdn:
                        //   "If the user cancels editing, the pszText member of the LVITEM structure is NULL"
                        if (!e.CancelEdit && nmlvdp.item.pszText != null)
                            Items[nmlvdp.item.iItem].Text = nmlvdp.item.pszText;
                        break;
                    }

                case NativeMethods.LVN_ITEMACTIVATE:
                    OnItemActivate(EventArgs.Empty);
                    break;

                case NativeMethods.LVN_BEGINDRAG: {
                    // the items collection was modified while dragging
                    // that means that we can't reliably give the user the item on which the dragging started
                    // so don't tell the user about this operation...
                    // 
                    if (!this.ItemCollectionChangedInMouseDown) {
                        NativeMethods.NMLISTVIEW nmlv = (NativeMethods.NMLISTVIEW)m.GetLParam(typeof(NativeMethods.NMLISTVIEW));
                        LvnBeginDrag(MouseButtons.Left, nmlv);
                    }

                    break;
                }

                case NativeMethods.LVN_BEGINRDRAG: {
                    // the items collection was modified while dragging
                    // that means that we can't reliably give the user the item on which the dragging started
                    // so don't tell the user about this operation...
                    // 
                    if (!this.ItemCollectionChangedInMouseDown) {
                        NativeMethods.NMLISTVIEW nmlv = (NativeMethods.NMLISTVIEW)m.GetLParam(typeof(NativeMethods.NMLISTVIEW));
                        LvnBeginDrag(MouseButtons.Right, nmlv);
                    }
                    break;
                }


                case NativeMethods.LVN_ITEMCHANGING: {
                        NativeMethods.NMLISTVIEW* nmlv = (NativeMethods.NMLISTVIEW*)m.LParam;
                        if ((nmlv->uChanged & NativeMethods.LVIF_STATE) != 0) {
                            // Because the state image mask is 1-based, a value of 1 means unchecked,
                            // anything else means checked.  We convert this to the more standard 0 or 1
                            CheckState oldState = (CheckState)(((nmlv->uOldState & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                            CheckState newState = (CheckState)(((nmlv->uNewState & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                            if (oldState != newState) {
                                ItemCheckEventArgs e = new ItemCheckEventArgs(nmlv->iItem, newState, oldState);
                                OnItemCheck(e);
                                m.Result = (IntPtr)(((int)e.NewValue == 0 ? 0 : 1) == (int)oldState ? 1 : 0);
                            }
                        }
                        break;
                    }

                case NativeMethods.LVN_ITEMCHANGED: {
                        NativeMethods.NMLISTVIEW* nmlv = (NativeMethods.NMLISTVIEW*)m.LParam;
                        // Check for state changes to the selected state...
                        if ((nmlv->uChanged & NativeMethods.LVIF_STATE) != 0) {
                            // Because the state image mask is 1-based, a value of 1 means unchecked,
                            // anything else means checked.  We convert this to the more standard 0 or 1
                            CheckState oldValue = (CheckState)(((nmlv->uOldState & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                            CheckState newValue = (CheckState)(((nmlv->uNewState & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                            if (newValue != oldValue) {
                                ItemCheckedEventArgs e = new ItemCheckedEventArgs(Items[nmlv->iItem]);
                                OnItemChecked(e);
                                if (AccessibilityImprovements.Level1) {
                                    AccessibilityNotifyClients(AccessibleEvents.StateChange, nmlv->iItem);
                                    AccessibilityNotifyClients(AccessibleEvents.NameChange, nmlv->iItem);
                                }
                            }

                            int oldState = nmlv->uOldState & NativeMethods.LVIS_SELECTED;
                            int newState = nmlv->uNewState & NativeMethods.LVIS_SELECTED;
                            // Windows common control always fires
                            // this event twice, once with newState, oldState, and again with
                            // oldState, newState.
                            // Changing this affects the behaviour as the control never
                            // fires the event on a Deselct of an Items from multiple selections.
                            // So leave it as it is...
                            if (newState != oldState) {
                                if (this.VirtualMode && nmlv->iItem == -1)
                                {
                                    if (this.VirtualListSize > 0) {
                                        ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new ListViewVirtualItemsSelectionRangeChangedEventArgs(0, this.VirtualListSize-1, newState != 0);
                                        OnVirtualItemsSelectionRangeChanged(lvvisrce);
                                    }
                                }
                                else
                                {
                                    // APPCOMPAT
                                    // V1.* users implement virtualization by communicating directly to the native ListView and
                                    // by passing our virtualization implementation.
                                    // In that case, the native list view may have an item under the mouse even if our wrapper thinks the item count is 0.
                                    // And that may cause GetItemAt to throw an out of bounds exception.

                                    if (this.Items.Count > 0) {
                                        ListViewItemSelectionChangedEventArgs lvisce = new ListViewItemSelectionChangedEventArgs(this.Items[nmlv->iItem],
                                                                                                                                 nmlv->iItem,
                                                                                                                                 newState != 0);
                                        OnItemSelectionChanged(lvisce);
                                    }
                                }
                                // Delay SelectedIndexChanged event because the last item isn't present yet.
                                if (this.Items.Count == 0 || this.Items[this.Items.Count - 1] != null)
                                {
                                    this.listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
                                    OnSelectedIndexChanged(EventArgs.Empty);
                                }
                                else
                                {
                                    this.listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = true;
                                }
                            }
                        }
                        break;
                    }

                case NativeMethods.NM_CLICK:
                    WmNmClick(ref m);
                    // FALL THROUGH //
                    goto case NativeMethods.NM_RCLICK;

                case NativeMethods.NM_RCLICK:
                    NativeMethods.LVHITTESTINFO lvhi = new NativeMethods.LVHITTESTINFO();
                    int displayIndex = GetIndexOfClickedItem(lvhi);

                    MouseButtons button = nmhdr->code == NativeMethods.NM_CLICK ? MouseButtons.Left : MouseButtons.Right;
                    Point pos = Cursor.Position;
                    pos = PointToClientInternal(pos);

                    if (!ValidationCancelled && displayIndex != -1) {
                        OnClick(EventArgs.Empty);
                        OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                    }
                    if (!listViewState[LISTVIEWSTATE_mouseUpFired])
                    {
                        OnMouseUp(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                        listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    }
                    break;

                case NativeMethods.NM_DBLCLK:
                    WmNmDblClick(ref m);
                    // FALL THROUGH //
                    goto case NativeMethods.NM_RDBLCLK;

                case NativeMethods.NM_RDBLCLK:
                    NativeMethods.LVHITTESTINFO lvhip = new NativeMethods.LVHITTESTINFO();
                    int index = GetIndexOfClickedItem(lvhip);

                    if (index != -1) {
                        //just maintain state and fire double click.. in final mouseUp...
                        listViewState[LISTVIEWSTATE_doubleclickFired] = true;
                    }
                    //fire Up in the Wndproc !!
                    listViewState[LISTVIEWSTATE_mouseUpFired] = false;
                    //problem getting the UP... outside the control...
                    //
                    CaptureInternal = true;
                    break;

                case NativeMethods.LVN_KEYDOWN:
                    if (CheckBoxes) {
                        NativeMethods.NMLVKEYDOWN lvkd = (NativeMethods.NMLVKEYDOWN)m.GetLParam(typeof(NativeMethods.NMLVKEYDOWN));
                        if (lvkd.wVKey == (short)Keys.Space) {
                            ListViewItem focusedItem = FocusedItem;
                            if (focusedItem != null) {
                                bool check = !focusedItem.Checked;
                                if (!VirtualMode) {
                                    foreach(ListViewItem item in SelectedItems) {
                                        if (item != focusedItem) {
                                            item.Checked = check;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case NativeMethods.LVN_ODCACHEHINT:
                    // tell the user to prepare the cache:
                    NativeMethods.NMLVCACHEHINT cacheHint = (NativeMethods.NMLVCACHEHINT) m.GetLParam(typeof(NativeMethods.NMLVCACHEHINT));
                    OnCacheVirtualItems(new CacheVirtualItemsEventArgs(cacheHint.iFrom, cacheHint.iTo));
                    break;

                default:
                    if (nmhdr->code == NativeMethods.LVN_GETDISPINFO) {
                        // we use the LVN_GETDISPINFO message only in virtual mode
                        if (this.VirtualMode && m.LParam != IntPtr.Zero) {
                            NativeMethods.NMLVDISPINFO_NOTEXT dispInfo= (NativeMethods.NMLVDISPINFO_NOTEXT) m.GetLParam(typeof(NativeMethods.NMLVDISPINFO_NOTEXT));

                            RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(dispInfo.item.iItem);
                            OnRetrieveVirtualItem(rVI);
                            ListViewItem lvItem = rVI.Item;
                            if (lvItem == null)
                            {
                                throw new InvalidOperationException(SR.ListViewVirtualItemRequired);
                            }

                            lvItem.SetItemIndex(this, dispInfo.item.iItem);
                            if ((dispInfo.item.mask & NativeMethods.LVIF_TEXT) != 0) {
                                string text;
                                if (dispInfo.item.iSubItem == 0) {
                                    text = lvItem.Text;                                         // we want the item
                                }
                                else {
                                    if (lvItem.SubItems.Count <= dispInfo.item.iSubItem) {
                                        throw new InvalidOperationException(SR.ListViewVirtualModeCantAccessSubItem);
                                    }
                                    else {
                                        text = lvItem.SubItems[dispInfo.item.iSubItem].Text;            // we want the sub item
                                    }
                                }

                                // use the buffer provided by the ComCtrl list view.
                                if (dispInfo.item.cchTextMax <= text.Length) {
                                    text = text.Substring(0, dispInfo.item.cchTextMax - 1);
                                }

                                if (Marshal.SystemDefaultCharSize == 1) {
                                    // ANSI. Use byte
                                    byte[] buff = System.Text.Encoding.Default.GetBytes(text + "\0");
                                    Marshal.Copy(buff, 0, dispInfo.item.pszText, text.Length + 1);
                                } else {
                                    char[] buff = (text + "\0").ToCharArray();
                                    Marshal.Copy(buff, 0, dispInfo.item.pszText, text.Length + 1);
                                }
                            }

                            if ((dispInfo.item.mask & NativeMethods.LVIF_IMAGE) != 0 && lvItem.ImageIndex != -1) {
                                dispInfo.item.iImage = lvItem.ImageIndex;
                            }

                            if ((dispInfo.item.mask & NativeMethods.LVIF_INDENT) != 0) {
                                dispInfo.item.iIndent = lvItem.IndentCount;
                            }

                            /* Microsoft: Couldn't make this work. The dispInfo.item.iSubItem received for the subitems' text
                                       are invalid
                            if ((dispInfo.item.mask & NativeMethods.LVIF_COLUMNS) != 0) {
                                int cColumns = this.columnHeaders != null ? this.columnHeaders.Length : 0;
                                dispInfo.item.cColumns = cColumns;
                                if (cColumns > 0) {
                                    dispInfo.item.puColumns = Marshal.AllocHGlobal(cColumns * Marshal.SizeOf(typeof(int)));
                                    int[] columns = new int[cColumns];
                                    for (int c = 0; c < cColumns; c++) {
                                        columns[c] = c + 1;
                                    }
                                    Marshal.Copy(columns, 0, dispInfo.item.puColumns, cColumns);
                                }
                            }
                            */

                            /* Microsoft: VirtualMode and grouping seem to be incompatible.
                                       dispInfo.item.mask never includes NativeMethods.LVIF_GROUPID.
                                       Besides, trying to send LVM_ENABLEGROUPVIEW to the listview fails in virtual mode.
                            if (this.GroupsEnabled && (dispInfo.item.mask & NativeMethods.LVIF_GROUPID) != 0)
                            {
                                dispInfo.item.iGroupId = GetNativeGroupId(lvItem);
                                #if DEBUG
                                    Debug.Assert(SendMessage(NativeMethods.LVM_ISGROUPVIEWENABLED, 0, 0) != IntPtr.Zero, "Groups not enabled");
                                    Debug.Assert(SendMessage(NativeMethods.LVM_HASGROUP, dispInfo.item.iGroupId, 0) != IntPtr.Zero, "Doesn't contain group id: " + dispInfo.item.iGroupId.ToString(CultureInfo.InvariantCulture));
                                #endif
                            }
                            */

                            if ((dispInfo.item.stateMask & NativeMethods.LVIS_STATEIMAGEMASK) != 0) {
                                dispInfo.item.state |= lvItem.RawStateImageIndex;
                            }
                            Marshal.StructureToPtr(dispInfo, (IntPtr) m.LParam, false);

                        }
                    }
                    else if (nmhdr->code == NativeMethods.LVN_ODSTATECHANGED) {
                        if (VirtualMode && m.LParam != IntPtr.Zero) {
                            NativeMethods.NMLVODSTATECHANGE odStateChange = (NativeMethods.NMLVODSTATECHANGE) m.GetLParam(typeof(NativeMethods.NMLVODSTATECHANGE));
                            bool selectedChanged = (odStateChange.uNewState & NativeMethods.LVIS_SELECTED) != (odStateChange.uOldState & NativeMethods.LVIS_SELECTED);
                            if (selectedChanged) {
                                // we have to substract 1 from iTo
                                // 
                                // 
                                int iTo = odStateChange.iTo;
                                if (!UnsafeNativeMethods.IsVista) {
                                    iTo--;
                                }
                                ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new ListViewVirtualItemsSelectionRangeChangedEventArgs(odStateChange.iFrom, iTo, (odStateChange.uNewState & NativeMethods.LVIS_SELECTED) != 0);
                                OnVirtualItemsSelectionRangeChanged(lvvisrce);
                            }
                        }
                    }
                    else if (nmhdr->code == NativeMethods.LVN_GETINFOTIP) {
                        if (ShowItemToolTips && m.LParam != IntPtr.Zero) {
                            NativeMethods.NMLVGETINFOTIP infoTip = (NativeMethods.NMLVGETINFOTIP) m.GetLParam(typeof(NativeMethods.NMLVGETINFOTIP));
                            ListViewItem lvi = Items[infoTip.item];
                            if (lvi != null && !string.IsNullOrEmpty(lvi.ToolTipText)) {

                                // MSDN:
                                // Setting the max width has the added benefit of enabling multiline
                                // tool tips!
                                //
                                
                                UnsafeNativeMethods.SendMessage(new HandleRef(this, nmhdr->hwndFrom), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);

                                if (Marshal.SystemDefaultCharSize == 1) {
                                    // ANSI. Use byte.
                                    // we need to copy the null terminator character ourselves
                                    byte[] byteBuf = System.Text.Encoding.Default.GetBytes(lvi.ToolTipText + "\0");
                                    Marshal.Copy(byteBuf, 0, infoTip.lpszText, Math.Min(byteBuf.Length, infoTip.cchTextMax));
                                } else {
                                    // UNICODE. Use char.
                                    // we need to copy the null terminator character ourselves
                                    char[] charBuf = (lvi.ToolTipText + "\0").ToCharArray();
                                    Marshal.Copy(charBuf, 0, infoTip.lpszText, Math.Min(charBuf.Length, infoTip.cchTextMax));
                                }
                                Marshal.StructureToPtr(infoTip, (IntPtr) m.LParam, false);
                            }
                        }
                    }
                    else if (nmhdr->code == NativeMethods.LVN_ODFINDITEM) {
                        if (VirtualMode) {
                            NativeMethods.NMLVFINDITEM nmlvif = (NativeMethods.NMLVFINDITEM) m.GetLParam(typeof(NativeMethods.NMLVFINDITEM));

                            if ((nmlvif.lvfi.flags & NativeMethods.LVFI_PARAM) != 0) {
                                m.Result = (IntPtr) (-1);
                                return;
                            }

                            bool isTextSearch = ((nmlvif.lvfi.flags & NativeMethods.LVFI_STRING) != 0) ||
                                                ((nmlvif.lvfi.flags & NativeMethods.LVFI_PARTIAL) != 0);

                            bool isPrefixSearch = (nmlvif.lvfi.flags & NativeMethods.LVFI_PARTIAL) != 0;

                            string text = String.Empty;
                            if (isTextSearch) {
                                text = nmlvif.lvfi.psz;
                            }

                            Point startingPoint = Point.Empty;
                            if ((nmlvif.lvfi.flags & NativeMethods.LVFI_NEARESTXY) != 0) {
                                startingPoint = new Point(nmlvif.lvfi.ptX, nmlvif.lvfi.ptY);
                            }

                            SearchDirectionHint dir = SearchDirectionHint.Down;
                            if ((nmlvif.lvfi.flags & NativeMethods.LVFI_NEARESTXY) != 0) {
                                // We can do this because SearchDirectionHint is set to the VK_*
                                dir = (SearchDirectionHint) nmlvif.lvfi.vkDirection;
                            }

                            int startIndex = nmlvif.iStart;
                            if (startIndex >= this.VirtualListSize) {
                                // we want to search starting from the last item. Wrap around the first item.
                                startIndex = 0;
                            }


                            SearchForVirtualItemEventArgs sviEvent = new SearchForVirtualItemEventArgs(
                                                                         isTextSearch,
                                                                         isPrefixSearch,
                                                                         false, /* includeSubItemsInSearch */
                                                                         text,
                                                                         startingPoint,
                                                                         dir,
                                                                         nmlvif.iStart);

                            OnSearchForVirtualItem(sviEvent);
                            if (sviEvent.Index != -1) {
                                m.Result = (IntPtr) sviEvent.Index;
                            } else {
                                m.Result = (IntPtr) (-1);
                            }
                        }
                    }
                break;
            }
        }

        private void WmPrint(ref Message m) {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & (int)m.LParam) != 0 && Application.RenderWithVisualStyles && this.BorderStyle == BorderStyle.Fixed3D) {
                IntSecurity.UnmanagedCode.Assert();
                try {
                    using (Graphics g = Graphics.FromHdc(m.WParam)) {
                        Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
                        g.DrawRectangle(new Pen(VisualStyleInformation.TextControlBorder), rect);
                        rect.Inflate(-1, -1);
                        g.DrawRectangle(SystemPens.Window, rect);
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.WndProc"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {

            switch (m.Msg) {
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                    WmReflectNotify(ref m);
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:

                    // Ensure that the itemCollectionChangedInMouseDown is not set
                    // before processing the mousedown event.  
                    this.ItemCollectionChangedInMouseDown = false;
                    CaptureInternal = true;
                    WmMouseDown(ref m, MouseButtons.Left, 2);
                    break;

                case NativeMethods.WM_LBUTTONDOWN:

                    // Ensure that the itemCollectionChangedInMouseDown is not set
                    // before processing the mousedown event.  
                    this.ItemCollectionChangedInMouseDown = false;
                    WmMouseDown(ref m, MouseButtons.Left, 1);
                    downButton = MouseButtons.Left;
                    break;

                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_MBUTTONUP:

                    // see the mouse is on item
                    //
                    NativeMethods.LVHITTESTINFO lvhip = new NativeMethods.LVHITTESTINFO();
                    int index = GetIndexOfClickedItem(lvhip);

                    if (!ValidationCancelled && listViewState[LISTVIEWSTATE_doubleclickFired] && index != -1) {
                        listViewState[LISTVIEWSTATE_doubleclickFired] = false;
                        OnDoubleClick(EventArgs.Empty);
                        OnMouseDoubleClick(new MouseEventArgs(downButton, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                    }
                    if (!listViewState[LISTVIEWSTATE_mouseUpFired])
                    {
                        OnMouseUp(new MouseEventArgs(downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        listViewState[LISTVIEWSTATE_expectingMouseUp] = false;
                    }

                    this.ItemCollectionChangedInMouseDown = false;

                    listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    CaptureInternal = false;
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    downButton = MouseButtons.Middle;
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    downButton = MouseButtons.Right;
                    break;
                case NativeMethods.WM_MOUSEMOVE:
                    if (listViewState[LISTVIEWSTATE_expectingMouseUp] && !listViewState[LISTVIEWSTATE_mouseUpFired] && MouseButtons == MouseButtons.None)
                    {
                        OnMouseUp(new MouseEventArgs(downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    }
                    CaptureInternal = false;
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_MOUSEHOVER:
                    if (HoverSelection) {
                        base.WndProc(ref m);
                    }
                    else
                        OnMouseHover(EventArgs.Empty);
                    break;
                case NativeMethods.WM_NOTIFY:
                    if(WmNotify(ref m))
                    {
                        break; // we are done - skip default handling
                    }
                    else
                    {
                        goto default;  //default handling needed
                    }
                case NativeMethods.WM_SETFOCUS:
                    base.WndProc(ref m);

                    if (!this.RecreatingHandle && !this.ListViewHandleDestroyed) {
                        // This means that we get a WM_SETFOCUS on the hWnd that was destroyed.
                        // Don't do anything because the information on the previous hWnd is most likely
                        // out of sync w/ the information in our ListView wrapper.

                        // We should set focus to the first item,
                        // if none of the items are focused already.
                        if (FocusedItem == null && Items.Count > 0)
                        {
                            Items[0].Focused = true;
                        }
                    }
                    break;
                case NativeMethods.WM_MOUSELEAVE:
                    // if the mouse leaves and then re-enters the ListView
                    // ItemHovered events should be raised.
                    prevHoveredItem = null;
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_PAINT:
                    base.WndProc(ref m);

                    // win32 ListView 
                    BeginInvoke(new MethodInvoker(this.CleanPreviousBackgroundImageFiles));
                    break;
                case NativeMethods.WM_PRINT:
                    WmPrint(ref m);
                    break;
                case NativeMethods.WM_TIMER:
                    if (unchecked( (int) (long)m.WParam) != LVTOOLTIPTRACKING || !ComctlSupportsVisualStyles) {
                        base.WndProc(ref m);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            };
        }

        ///new class for comparing and sorting Icons ....
        //subhag
        internal class IconComparer: IComparer {
            private SortOrder sortOrder;

            public IconComparer(SortOrder currentSortOrder) {
                   this.sortOrder = currentSortOrder;
            }

            public SortOrder SortOrder {
                set {
                    sortOrder = value;
                }
            }

            public int Compare(object obj1, object obj2) {
                //subhag
                ListViewItem currentItem =  (ListViewItem)obj1;
                ListViewItem nextItem =  (ListViewItem)obj2;
                if (sortOrder == SortOrder.Ascending) {
                    return (String.Compare(currentItem.Text,nextItem.Text, false, CultureInfo.CurrentCulture));
                }
                else {
                    return (String.Compare(nextItem.Text,currentItem.Text, false, CultureInfo.CurrentCulture));
                }
            }
        }
        //end subhag

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [ListBindable(false)]
        public class CheckedIndexCollection : IList {
            private ListView owner;

            /* C#r: protected */
            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.CheckedIndexCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public CheckedIndexCollection(ListView owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.Count"]/*' />
            /// <devdoc>
            ///     Number of currently selected items.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    if (!owner.CheckBoxes) {
                        return 0;
                    }

                    // Count the number of checked items
                    //
                    int count = 0;
                    foreach(ListViewItem item in owner.Items) {
                        if (item != null && item.Checked) {
                            count++;
                        }
                    }
                    return count;
                }
            }

            private int[] IndicesArray {
                get {
                    int[] indices = new int[Count];
                    int index = 0;
                    for(int i=0; i < owner.Items.Count && index < indices.Length; ++i) {
                        if (owner.Items[i].Checked) {
                            indices[index++] = i;
                        }
                    }
                    return indices;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.this"]/*' />
            /// <devdoc>
            ///     Selected item in the list.
            /// </devdoc>
            public int this[int index] {
                get {

                    if (index < 0) {
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    // Loop through the main collection until we find the right index.
                    //
                    int cnt = owner.Items.Count;
                    int nChecked = 0;
                    for(int i = 0; i < cnt; i++) {
                        ListViewItem item = owner.Items[i];

                        if (item.Checked) {
                            if (nChecked == index) {
                                return i;
                            }
                            nChecked++;
                        }
                    }

                    // Should never get to this point.
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    throw new NotSupportedException();
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(int checkedIndex) {
                if (owner.Items[checkedIndex].Checked) {
                    return true;
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object checkedIndex) {
                if (checkedIndex is Int32) {
                    return Contains((int)checkedIndex);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(int checkedIndex) {
                int[] indices = IndicesArray;
                for(int index=0; index < indices.Length; ++index) {
                    if (indices[index] == checkedIndex) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object checkedIndex) {
                if (checkedIndex is Int32) {
                    return IndexOf((int)checkedIndex);
                }
                else {
                    return -1;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedIndexCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                if (Count > 0) {
                    System.Array.Copy(IndicesArray, 0, dest, index, Count);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedIndexCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                int[] indices = IndicesArray;
                if (indices != null) {
                    return indices.GetEnumerator();
                }
                else {
                    return new int[0].GetEnumerator();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [ListBindable(false)]
        public class CheckedListViewItemCollection : IList {
            private ListView owner;

               /// A caching mechanism for key accessor
               /// We use an index here rather than control so that we don't have lifetime
               /// issues by holding on to extra references.
               private int lastAccessedIndex = -1;

            /* C#r: protected */
            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.CheckedListViewItemCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public CheckedListViewItemCollection(ListView owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.Count"]/*' />
            /// <devdoc>
            ///     Number of currently selected items.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return owner.CheckedIndices.Count;
                }
            }

            private ListViewItem[] ItemArray {
                get {
                    ListViewItem[] items = new ListViewItem[Count];
                    int index = 0;
                    for(int i=0; i < owner.Items.Count && index < items.Length; ++i) {
                        if (owner.Items[i].Checked) {
                            items[index++] = owner.Items[i];
                        }
                    }
                    return items;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.this"]/*' />
            /// <devdoc>
            ///     Selected item in the list.
            /// </devdoc>
            public ListViewItem this[int index] {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    int itemIndex = owner.CheckedIndices[index];
                    return owner.Items[itemIndex];
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return this[index];
                }
                set {
                    throw new NotSupportedException();
                }
            }

               /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedItemCollection.this"]/*' />
               /// <devdoc>
               ///     <para>Retrieves the child control with the specified key.</para>
               /// </devdoc>
               public virtual ListViewItem this[string key] {
                    get {
                        if (owner.VirtualMode) {
                            throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                        }

                         // We do not support null and empty string as valid keys.
                         if(string.IsNullOrEmpty(key)) {
                              return null;
                         }

                         // Search for the key in our collection
                         int index = IndexOfKey(key);
                         if(IsValidIndex(index)) {
                              return this[index];
                         }
                         else {
                              return null;
                         }

                    }
               }

               /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return true;
                }
            }


            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(ListViewItem item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item != null && item.ListView == owner && item.Checked) {
                    return true;
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem) {
                    return Contains((ListViewItem)item);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedItemCollection.ContainsKey"]/* />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                 return IsValidIndex(IndexOfKey(key));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(ListViewItem item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                for(int index=0; index < items.Length; ++index) {
                    if (items[index] == item) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
            /// </devdoc>
            public virtual int IndexOfKey(String key) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key)) {
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


            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedItemCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/>
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem) {
                    return IndexOf((ListViewItem)item);
                }
                else {
                    return -1;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="CheckedListViewItemCollection.CopyTo"]/*' />
            public void CopyTo(Array dest, int index) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0) {
                    System.Array.Copy(ItemArray, 0, dest, index, Count);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.CheckedListViewItemCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                if (items != null) {
                    return items.GetEnumerator();
                }
                else {
                    return new ListViewItem[0].GetEnumerator();
                }
            }

        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [ListBindable(false)]
        public class SelectedIndexCollection : IList {
            private ListView owner;

            /* C#r: protected */
            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.SelectedIndexCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public SelectedIndexCollection(ListView owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.Count"]/*' />
            /// <devdoc>
            ///     Number of currently selected items.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    if (owner.IsHandleCreated) {
                        return unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETSELECTEDCOUNT, 0, 0));
                    }
                    else {
                        if (owner.savedSelectedItems != null) {
                            return owner.savedSelectedItems.Count;
                        }
                        return 0;
                    }
                }
            }

            private int[] IndicesArray {
                get {
                    int count = Count;
                    int[] indices = new int[count];

                    if (owner.IsHandleCreated) {
                        int displayIndex = -1;
                        for (int i = 0; i < count; i++) {
                            int fidx = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETNEXTITEM, displayIndex, NativeMethods.LVNI_SELECTED));

                            if (fidx > -1) {
                                indices[i] = fidx;
                                displayIndex = fidx;
                            }
                            else
                                throw new InvalidOperationException(SR.SelectedNotEqualActual);
                        }
                    }
                    else {
                        Debug.Assert(owner.savedSelectedItems != null || count == 0, "if the count of selectedItems is greater than 0 then the selectedItems should have been saved by now");
                        for (int i = 0; i < count; i++) {
                            indices[i] = owner.savedSelectedItems[i].Index;
                        }
                    }

                    return indices;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.this"]/*' />
            /// <devdoc>
            ///     Selected item in the list.
            /// </devdoc>
            public int this[int index] {
                get {

                    if (index < 0 || index >= Count) {
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    if (owner.IsHandleCreated) {

                        // Count through the selected items in the ListView, until
                        // we reach the 'index'th selected item.
                        //
                        int fidx = -1;
                        for(int count = 0; count <= index; count++) {
                            fidx = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETNEXTITEM, fidx, NativeMethods.LVNI_SELECTED));
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return fidx;
                    }
                    else {
                        Debug.Assert(owner.savedSelectedItems != null, "Null selected items collection");
                        return owner.savedSelectedItems[index].Index;
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    throw new NotSupportedException();
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(int selectedIndex) {
                return owner.Items[selectedIndex].Selected;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object selectedIndex) {
                if (selectedIndex is Int32) {
                    return Contains((int)selectedIndex);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(int selectedIndex) {
                int[] indices = IndicesArray;
                for(int index=0; index < indices.Length; ++index) {
                    if (indices[index] == selectedIndex) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object selectedIndex) {
                if (selectedIndex is Int32) {
                    return IndexOf((int)selectedIndex);
                }
                else {
                    return -1;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value)
            {
                if (value is int)
                {
                    return Add((int) value);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "value", ((value).ToString())));
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Clear"]/*' />
            /// <internalonly/>
            void IList.Clear() {
                Clear();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value)
            {
                if (value is int)
                {
                    Remove((int) value);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "value", ((value).ToString())));
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.Add"]/*' />
            public int Add(int itemIndex)
            {
                if (this.owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= this.owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException("itemIndex", string.Format(SR.InvalidArgument, "itemIndex", (itemIndex).ToString(CultureInfo.CurrentCulture)));
                    }
                    if (this.owner.IsHandleCreated)
                    {
                        this.owner.SetItemState(itemIndex, NativeMethods.LVIS_SELECTED, NativeMethods.LVIS_SELECTED);
                        return this.Count;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= this.owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException("itemIndex", string.Format(SR.InvalidArgument, "itemIndex", (itemIndex).ToString(CultureInfo.CurrentCulture)));
                    }
                    this.owner.Items[itemIndex].Selected = true;
                    return this.Count;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.Clear"]/*' />
            public void Clear()
            {
                if (!this.owner.VirtualMode)
                {
                    this.owner.savedSelectedItems = null;
                }
                if (this.owner.IsHandleCreated)
                {
                    this.owner.SetItemState(-1, 0, NativeMethods.LVIS_SELECTED);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedIndexCollection.CopyTo"]/*' />
            public void CopyTo(Array dest, int index) {
                if (Count > 0) {
                    System.Array.Copy(IndicesArray, 0, dest, index, Count);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                int[] indices = IndicesArray;
                if (indices != null) {
                    return indices.GetEnumerator();
                }
                else {
                    return new int[0].GetEnumerator();
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedIndexCollection.Remove"]/*' />
            public void Remove(int itemIndex)
            {
                if (this.owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= this.owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException("itemIndex", string.Format(SR.InvalidArgument, "itemIndex", (itemIndex).ToString(CultureInfo.CurrentCulture)));
                    }
                    if (this.owner.IsHandleCreated)
                    {
                        this.owner.SetItemState(itemIndex, 0, NativeMethods.LVIS_SELECTED);
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= this.owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException("itemIndex", string.Format(SR.InvalidArgument, "itemIndex", (itemIndex).ToString(CultureInfo.CurrentCulture)));
                    }
                    this.owner.Items[itemIndex].Selected = false;
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [ListBindable(false)]
        public class SelectedListViewItemCollection : IList {
            private ListView owner;

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /* C#r: protected */
            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.SelectedListViewItemCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public SelectedListViewItemCollection(ListView owner) {
                this.owner = owner;
            }

            private ListViewItem[] SelectedItemArray {
                get {
                    if (owner.IsHandleCreated) {
                        int cnt = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETSELECTEDCOUNT, 0, 0));

                        ListViewItem[] lvitems = new ListViewItem[cnt];

                        int displayIndex = -1;

                        for (int i = 0; i < cnt; i++) {
                            int fidx = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETNEXTITEM, displayIndex, NativeMethods.LVNI_SELECTED));

                            if (fidx > -1) {
                                lvitems[i] = owner.Items[fidx];
                                displayIndex = fidx;
                            }
                            else
                                throw new InvalidOperationException(SR.SelectedNotEqualActual);
                        }

                        return lvitems;

                    }
                    else {
                        if (owner.savedSelectedItems != null) {
                            ListViewItem[] cloned = new ListViewItem[owner.savedSelectedItems.Count];
                            for (int i = 0; i < owner.savedSelectedItems.Count; i ++) {
                                cloned[i] = owner.savedSelectedItems[i];
                            }
                            return cloned;
                        }
                        else {
                            return new ListViewItem[0];
                        }
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.Count"]/*' />
            /// <devdoc>
            ///     Number of currently selected items.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (owner.IsHandleCreated) {
                        return unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETSELECTEDCOUNT, 0, 0));
                    }
                    else {
                        if (owner.savedSelectedItems != null) {
                            return owner.savedSelectedItems.Count;
                        }
                        return 0;
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.this"]/*' />
            /// <devdoc>
            ///     Selected item in the list.
            /// </devdoc>
            public ListViewItem this[int index] {
                get {

                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (index < 0 || index >= Count) {
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    if (owner.IsHandleCreated) {

                        // Count through the selected items in the ListView, until
                        // we reach the 'index'th selected item.
                        //
                        int fidx = -1;
                        for(int count = 0; count <= index; count++) {
                            fidx = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_GETNEXTITEM, fidx, NativeMethods.LVNI_SELECTED));
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return owner.Items[fidx];
                    }
                    else {
                        Debug.Assert(owner.savedSelectedItems != null, "Null selected items collection");
                        return owner.savedSelectedItems[index];
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual ListViewItem this[string key] {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

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

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    return this[index];
                }
                set {
                    // SelectedListViewItemCollection is read-only
                    throw new NotSupportedException();
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedItemCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/>
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.RemoveAt"]/*' />
            /// <internalonly/>
            void IList.RemoveAt(int index) {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.Clear"]/*' />
            /// <devdoc>
            ///     Unselects all items.
            /// </devdoc>
            public void Clear() {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                for (int i=0; i < items.Length; i++) {
                    items[i].Selected = false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedItemCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return IsValidIndex(IndexOfKey(key));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(ListViewItem item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return (IndexOf(item) != -1);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem) {
                    return Contains((ListViewItem)item);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.CopyTo"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void CopyTo(Array dest, int index) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0) {
                    System.Array.Copy(SelectedItemArray, 0, dest, index, Count);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                if (items != null) {
                    return items.GetEnumerator();
                }
                else {
                    return new ListViewItem[0].GetEnumerator();
                }
           }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedListViewItemCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(ListViewItem item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                for(int index=0; index < items.Length; ++index) {
                    if (items[index] == item) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="SelectedListViewItemCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object item) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem) {
                    return IndexOf((ListViewItem)item);
                }
                else {
                    return -1;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.SelectedItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
            /// </devdoc>
            public virtual int  IndexOfKey(String key) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

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

        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [ListBindable(false)]
        public class ColumnHeaderCollection : IList {
            private ListView owner;

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.ColumnHeaderCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ColumnHeaderCollection(ListView owner) {
                this.owner = owner;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.this"]/*' />
            /// <devdoc>
            ///     Given a Zero based index, returns the ColumnHeader object
            ///     for the column at that index
            /// </devdoc>
            public virtual ColumnHeader this[int index] {
                get {
                    if (owner.columnHeaders == null || index < 0 || index >= owner.columnHeaders.Length)
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    return owner.columnHeaders[index];
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    throw new NotSupportedException();
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual ColumnHeader this[string key] {
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

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Count"]/*' />
            /// <devdoc>
            ///     The number of columns the ListView currently has in Details view.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    return owner.columnHeaders == null ? 0 : owner.columnHeaders.Length;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return false;
                }
            }





        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.RemoveByKey"]/*' />
        /// <devdoc>
        ///     <para>Removes the child control with the specified key.</para>
        /// </devdoc>
        public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index);
                 }
           }

        /// A caching mechanism for key accessor
        /// We use an index here rather than control so that we don't have lifetime
        /// issues by holding on to extra references.
        /// Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
        private int lastAccessedIndex = -1;


        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.this"]/*' />
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


           /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.IsValidIndex"]/*' />
           /// <devdoc>
           ///     <para>Determines if the index is valid for the collection.</para>
           /// </devdoc>
           /// <internalonly/>
          private bool IsValidIndex(int index) {
            return ((index >= 0) && (index < this.Count));
          }


            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds a column to the end of the Column list
            /// </devdoc>
            public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                return owner.InsertColumn(Count, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add1"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual int Add(ColumnHeader value) {
                int index = Count;
                owner.InsertColumn(index, value);
                return index;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add2"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string text) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                return owner.InsertColumn(Count, columnHeader);
            }

            // <-- NEW ADD OVERLOADS IN WHIDBEY

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add3"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string text, int width) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                columnHeader.Width = width;
                return owner.InsertColumn(Count, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add4"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string key, string text) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = key;
                columnHeader.Text = text;
                return owner.InsertColumn(Count, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add5"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string key, string text, int width) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                return owner.InsertColumn(Count, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add6"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey) {
                ColumnHeader columnHeader = new ColumnHeader(imageKey);
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                return owner.InsertColumn(Count, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Add7"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex) {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex);
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                return owner.InsertColumn(Count, columnHeader);
            }

            // END - NEW ADD OVERLOADS IN WHIDBEY  -->

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.AddRange"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual void AddRange(ColumnHeader[] values) {
                if (values == null) {
                    throw new ArgumentNullException("values");
                }

                Hashtable usedIndices = new Hashtable();
                int [] indices = new int[values.Length];

                for (int i=0; i<values.Length; i++) {
                    
                    if (values[i].DisplayIndex == -1) {
                        values[i].DisplayIndexInternal = i;
                    }

                    if (!usedIndices.ContainsKey(values[i].DisplayIndex) && values[i].DisplayIndex >= 0 && values[i].DisplayIndex < values.Length) {
                       usedIndices.Add(values[i].DisplayIndex, i);
                    }

                    indices[i] = values[i].DisplayIndex;
                    Add( values[i] );
                }

                if (usedIndices.Count == values.Length) {
                   owner.SetDisplayIndices( indices );
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                if (value is ColumnHeader) {
                    return Add((ColumnHeader)value);
                }
                else {
                    throw new ArgumentException(SR.ColumnHeaderCollectionInvalidArgument);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all columns from the list view.
            /// </devdoc>
            public virtual void Clear() {
                // Delete the columns
                if (owner.columnHeaders != null) {
                    if (owner.View == View.Tile) {
                        // in Tile view our ListView uses the column header collection to update the Tile Information
                        for (int colIdx = owner.columnHeaders.Length-1; colIdx >= 0; colIdx--) {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }
                        owner.columnHeaders = null;
                        if (owner.IsHandleCreated) {
                            owner.RecreateHandleInternal();
                        }
                    } else {
                        for (int colIdx = owner.columnHeaders.Length-1; colIdx >= 0; colIdx--) {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            if (owner.IsHandleCreated) {
                                owner.SendMessage(NativeMethods.LVM_DELETECOLUMN, colIdx, 0);
                            }
                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }
                        owner.columnHeaders = null;
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(ColumnHeader value) {
                return IndexOf(value) != -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object value) {
                if (value is ColumnHeader) {
                    return Contains((ColumnHeader)value);
                }
                return false;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
                 return IsValidIndex(IndexOfKey(key));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                if (Count > 0) {
                    System.Array.Copy(owner.columnHeaders, 0, dest, index, Count);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(ColumnHeader value) {
                for(int index=0; index < Count; ++index) {
                    if (this[index] == value) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object value) {
                if (value is ColumnHeader) {
                    return IndexOf((ColumnHeader)value);
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, ColumnHeader value) {
                if (index < 0 || index > Count) {
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }
                owner.InsertColumn(index, value);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                if (value is ColumnHeader) {
                    Insert(index, (ColumnHeader)value);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert1"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string text, int width, HorizontalAlignment textAlign) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                Insert(index, columnHeader);
            }

            // <-- NEW INSERT OVERLOADS IN WHIDBEY

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert2"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string text) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                Insert(index, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert3"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string text, int width) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Text = text;
                columnHeader.Width = width;
                Insert(index, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert4"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string key, string text) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = key;
                columnHeader.Text = text;
                Insert(index, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert5"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string key, string text, int width) {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                Insert(index, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert6"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey) {
                ColumnHeader columnHeader = new ColumnHeader(imageKey);
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                Insert(index, columnHeader);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Insert7"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex) {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex);
                columnHeader.Name = key;
                columnHeader.Text = text;
                columnHeader.Width = width;
                columnHeader.TextAlign = textAlign;
                Insert(index, columnHeader);
            }


            // END - NEW INSERT OVERLOADS IN WHIDBEY -->

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     removes a column from the ListView
            /// </devdoc>
            public virtual void RemoveAt(int index) {
                if (index < 0 || index >= owner.columnHeaders.Length)
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

                int w = owner.columnHeaders[index].Width; // Update width before detaching from ListView

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && this.owner.View != View.Tile) {
                    int retval = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_DELETECOLUMN, index, 0));

                    if (0 == retval)
                        throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                                  "index",
                                                                  (index).ToString(CultureInfo.CurrentCulture)));
                }

                // we need to update the display indices
                int[] indices = new int[this.Count-1];

                ColumnHeader removeHdr = this[index];
                for (int i = 0; i < this.Count; i ++) {
                    ColumnHeader hdr = this[i];
                    if (i != index) {
                        if (hdr.DisplayIndex >= removeHdr.DisplayIndex) {
                           hdr.DisplayIndexInternal --;
                        }
                        indices[ i>index ? i-1:i ] = hdr.DisplayIndexInternal;
                    }
                }

                removeHdr.DisplayIndexInternal = -1;

                owner.columnHeaders[index].OwnerListview = null;
                int columnCount = owner.columnHeaders.Length;
                Debug.Assert(columnCount >= 1, "Column mismatch");
                if (columnCount == 1)
                    owner.columnHeaders = null;
                else {
                    ColumnHeader[] newHeaders = new ColumnHeader[--columnCount];
                    if (index > 0)
                        System.Array.Copy(owner.columnHeaders, 0, newHeaders, 0, index);
                    if (index < columnCount)
                        System.Array.Copy(owner.columnHeaders, index+1, newHeaders, index, columnCount - index);
                    owner.columnHeaders = newHeaders;
                }

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && this.owner.View == View.Tile) {
                    this.owner.RecreateHandleInternal();
                }

                owner.SetDisplayIndices( indices );
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.Remove"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual void Remove(ColumnHeader column) {
                int index = IndexOf(column);
                if (index != -1) {
                    RemoveAt(index);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ColumnHeaderCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                if (value is ColumnHeader) {
                    Remove((ColumnHeader)value);
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                if (owner.columnHeaders != null) {
                    return owner.columnHeaders.GetEnumerator();
                }
                else
                {
                    return new ColumnHeader[0].GetEnumerator();
                }
            }
        }

        /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection"]/*' />
        /// <devdoc>
        ///    <para>Represents the collection of items in a ListView or ListViewGroup</para>
        /// </devdoc>
        [ListBindable(false)]
        public class ListViewItemCollection : IList {

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            internal interface IInnerList
            {
                int Count { get; }
                bool OwnerIsVirtualListView { get; }
                bool OwnerIsDesignMode{ get; }
                ListViewItem this[int index] { get; set; }
                ListViewItem Add(ListViewItem item);
                void AddRange(ListViewItem[] items);
                void Clear();
                bool Contains(ListViewItem item);
                void CopyTo(Array dest, int index);
                IEnumerator GetEnumerator();
                int IndexOf(ListViewItem item);
                ListViewItem Insert(int index, ListViewItem item);
                void Remove(ListViewItem item);
                void RemoveAt(int index);
            }

            private IInnerList innerList;

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.ListViewItemCollection"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ListViewItemCollection(ListView owner) {
                // Kept for APPCOMPAT reasons.
                // In Whidbey this constructor is a no-op.

                // initialize the inner list w/ a dummy list.
                this.innerList = new ListViewNativeItemCollection(owner);
            }

            internal ListViewItemCollection(IInnerList innerList) {
                Debug.Assert(innerList != null, "Can't pass in null innerList");
                this.innerList = innerList;
            }

            private IInnerList InnerList {
                get
                {
                    return innerList;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Count"]/*' />
            /// <devdoc>
            ///     Returns the total number of items within the list view.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    return InnerList.Count;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return true;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.IsReadOnly"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.this"]/*' />
            /// <devdoc>
            ///     Returns the ListViewItem at the given index.
            /// </devdoc>
            public virtual ListViewItem this[int index] {
                get {
                    if (index < 0 || index >= InnerList.Count) {
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    return InnerList[index];
                }
                set {
                    if (index < 0 || index >= InnerList.Count) {
                        throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    InnerList[index] = value;
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    if (value is ListViewItem) {
                        this[index] = (ListViewItem)value;
                    }
                    else if (value != null) {
                        this[index] = new ListViewItem(value.ToString(), -1);
                    }
                }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual ListViewItem this[string key] {
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

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(string text) {
                return Add(text, -1);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object item) {
                if (item is ListViewItem) {
                    return IndexOf(Add((ListViewItem)item));
                }
                else if (item != null) {
                    return IndexOf(Add(item.ToString()));
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add1"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(string text, int imageIndex) {
                ListViewItem li = new ListViewItem(text, imageIndex);
                Add(li);
                return li;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add2"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(ListViewItem value) {
                InnerList.Add(value);
                return value;
            }

            // <-- NEW ADD OVERLOADS IN WHIDBEY

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add3"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(string text, string imageKey) {
                ListViewItem li = new ListViewItem(text, imageKey);
                Add(li);
                return li;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add4"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(string key, string text, string imageKey) {
                ListViewItem li = new ListViewItem(text, imageKey);
                li.Name = key;
                Add(li);
                return li;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Add5"]/*' />
            /// <devdoc>
            ///     Add an item to the ListView.  The item will be inserted either in
            ///     the correct sorted position, or, if no sorting is set, at the end
            ///     of the list.
            /// </devdoc>
            public virtual ListViewItem Add(string key, string text, int imageIndex) {
                ListViewItem li = new ListViewItem(text, imageIndex);
                li.Name = key;
                Add(li);
                return li;
            }


            // END - NEW ADD OVERLOADS IN WHIDBEY  -->

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.AddRange"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void AddRange(ListViewItem[] items) {
                if (items == null) {
                    throw new ArgumentNullException("items");
                }

                InnerList.AddRange(items);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.AddRange"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void AddRange(ListViewItemCollection items) {
                if (items == null) {
                    throw new ArgumentNullException("items");
                }

                ListViewItem[] itemArray = new ListViewItem[items.Count];
                items.CopyTo(itemArray, 0);
                InnerList.AddRange(itemArray);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all items from the list view.
            /// </devdoc>
            public virtual void Clear() {
                InnerList.Clear();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Contains"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public bool Contains(ListViewItem item) {
                return InnerList.Contains(item);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object item) {
                if (item is ListViewItem) {
                    return Contains((ListViewItem)item);
                }
                else {
                    return false;
                }
            }

           /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.ContainsKey"]/*' />
           /// <devdoc>
           ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
           /// </devdoc>
           public virtual bool ContainsKey(string key) {
               return IsValidIndex(IndexOfKey(key));
           }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.CopyTo"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void CopyTo(Array dest, int index) {
                InnerList.CopyTo(dest, index);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.Find"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an array
            ///           of all the controls that match.
            ///     </para>
            /// </devdoc>
            public ListViewItem[] Find (string key, bool searchAllSubItems) {
                ArrayList foundItems =  FindInternal(key, searchAllSubItems, this, new ArrayList());

                ListViewItem[] stronglyTypedFoundItems= new ListViewItem[foundItems.Count];
                foundItems.CopyTo(stronglyTypedFoundItems, 0);

                return stronglyTypedFoundItems;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.FindInternal"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an arraylist
            ///           of all the controls that match.
            ///     </para>
            /// </devdoc>
            /// <internalonly/>
            private ArrayList FindInternal(string key, bool searchAllSubItems, ListViewItemCollection listViewItems, ArrayList foundItems) {
               if ((listViewItems == null) || (foundItems == null)) {
                   return null;  // 
               }

                for (int i = 0; i < listViewItems.Count; i++) {

                   if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].Name, key, /* ignoreCase = */ true)) {
                       foundItems.Add(listViewItems[i]);
                   }
                   else {
                      if (searchAllSubItems) {
                          // start from 1, as we've already compared subitems[0]
                          for(int j = 1; j < listViewItems[i].SubItems.Count; j++) {
                               if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].SubItems[j].Name, key, /* ignoreCase = */ true)) {
                                  foundItems.Add(listViewItems[i]);
                                  break;
                               }
                          }
                       }
                    }
                  }

                  return foundItems;
             }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                if (this.InnerList.OwnerIsVirtualListView && !this.InnerList.OwnerIsDesignMode) {
                    // Throw the exception only at runtime.
                    throw new InvalidOperationException(SR.ListViewCantGetEnumeratorInVirtualMode);
                }
                return InnerList.GetEnumerator();
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.IndexOf"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public int IndexOf(ListViewItem item) {
                for(int index=0; index < Count; ++index) {
                    if (this[index] == item) {
                        return index;
                    }
                }
                return -1;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object item) {
                if (item is ListViewItem) {
                    return IndexOf((ListViewItem)item);
                }
                else {
                    return -1;
                }
            }
            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.this"]/*' />
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

           /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.IsValidIndex"]/*' />
           /// <devdoc>
           ///     <para>Determines if the index is valid for the collection.</para>
           /// </devdoc>
           /// <internalonly/>
           private bool IsValidIndex(int index) {
              return ((index >= 0) && (index < this.Count));
           }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ListViewItem Insert(int index, ListViewItem item) {
                if (index < 0 || index > Count) {
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }
                InnerList.Insert(index, item);
                return item;
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert1"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ListViewItem Insert(int index, string text) {
                return Insert(index, new ListViewItem(text));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert2"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ListViewItem Insert(int index, string text, int imageIndex) {
                return Insert(index, new ListViewItem(text, imageIndex));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object item) {
                if (item is ListViewItem) {
                    Insert(index, (ListViewItem)item);
                }
                else if (item != null) {
                    Insert(index, item.ToString());
                }
            }

            // <-- NEW INSERT OVERLOADS IN WHIDBEY

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert3"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public ListViewItem Insert(int index, string text, string imageKey) {
                return Insert(index, new ListViewItem(text, imageKey));
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert4"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ListViewItem Insert(int index, string key, string text, string imageKey) {
                ListViewItem li = new ListViewItem(text, imageKey);
                li.Name = key;
                return Insert(index, li);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Insert5"]/*' />
            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public virtual ListViewItem Insert(int index, string key, string text, int imageIndex) {
                ListViewItem li = new ListViewItem(text, imageIndex);
                li.Name = key;
                return Insert(index, li);
            }

            // END - NEW INSERT OVERLOADS IN WHIDBEY -->

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes an item from the ListView
            /// </devdoc>
            public virtual void Remove(ListViewItem item) {
                InnerList.Remove(item);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ListViewItemCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     Removes an item from the ListView
            /// </devdoc>
            public virtual void RemoveAt(int index) {
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                InnerList.RemoveAt(index);
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListView.ColumnHeaderCollection.RemoveByKey"]/*' />
            /// <devdoc>
            ///     <para>Removes the child control with the specified key.</para>
            /// </devdoc>
            public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index);
                 }
            }

            /// <include file='doc\ListView.uex' path='docs/doc[@for="ListViewItemCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object item) {
                if (item == null || !(item is ListViewItem)) {
                    return;
                }

                Remove((ListViewItem)item);
            }
        }

        // Overrides ListViewItemCollection methods and properties to automatically update
        // the native listview.
        //
        internal class ListViewNativeItemCollection : ListViewItemCollection.IInnerList {
            private ListView owner;

            public ListViewNativeItemCollection(ListView owner) {
                this.owner = owner;
            }

            public int Count {
                get {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode) {
                        return owner.VirtualListSize;
                    }
                    else  {
                        return owner.itemCount;
                    }
                }
            }

            public bool OwnerIsVirtualListView {
                get {
                    return owner.VirtualMode;
                }
            }

            public bool OwnerIsDesignMode {
                get {
                    return owner.DesignMode;
                }
            }

            public ListViewItem this[int displayIndex] {
                get {
                    owner.ApplyUpdateCachedItems();

                    if (owner.VirtualMode) {

                        // if we are showing virtual items, we need to get the item from the user
                        RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(displayIndex);
                        owner.OnRetrieveVirtualItem(rVI);
                        rVI.Item.SetItemIndex(this.owner, displayIndex);
                        return rVI.Item;
                    }
                    else {
                        if (displayIndex < 0 || displayIndex >= owner.itemCount)
                            throw new ArgumentOutOfRangeException("displayIndex", string.Format(SR.InvalidArgument, "displayIndex", (displayIndex).ToString(CultureInfo.CurrentCulture)));

                        if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed) {
                            Debug.Assert(owner.listItemsArray == null, "listItemsArray not null, even though handle created");
                            return (ListViewItem)owner.listItemsTable[DisplayIndexToID(displayIndex)];
                        }
                        else {
                            Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                            return (ListViewItem)owner.listItemsArray[displayIndex];
                        }
                    }
                }
                set {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode) {
                        throw new InvalidOperationException(SR.ListViewCantModifyTheItemCollInAVirtualListView);
                    }

                    if (displayIndex < 0 || displayIndex >= owner.itemCount)
                        throw new ArgumentOutOfRangeException("displayIndex", string.Format(SR.InvalidArgument, "displayIndex", (displayIndex).ToString(CultureInfo.CurrentCulture)));

                    if (this.owner.ExpectingMouseUp) {
                        this.owner.ItemCollectionChangedInMouseDown = true;
                    }

                    RemoveAt(displayIndex);
                    Insert(displayIndex, value);
                }
            }

            public ListViewItem Add(ListViewItem value) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                } else {
                    Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon || this.Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                    // PERF.
                    // Get the Checked bit before adding it to the back end.
                    // This saves a call into NativeListView to retrieve the real index.
                    bool valueChecked = value.Checked;

                    owner.InsertItems(owner.itemCount, new ListViewItem[]{value}, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes && valueChecked) {
                        owner.UpdateSavedCheckedItems(value, true /*addItem*/);
                    }

                    if (this.owner.ExpectingMouseUp) {
                        this.owner.ItemCollectionChangedInMouseDown = true;
                    }

                    return value;
                }
            }

            public void AddRange(ListViewItem[] values) {
                if (values == null) {
                    throw new ArgumentNullException("values");
                }

                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                IComparer comparer = owner.listItemSorter;
                owner.listItemSorter = null;

                Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon || this.Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                bool[] checkedValues = null;
                
                if (owner.IsHandleCreated && !owner.CheckBoxes) {
                    // PERF.
                    // Cache the Checked bit before adding the item to the list view.
                    // This saves a bunch of calls to native list view when we want to UpdateSavedCheckedItems.
                    //
                    checkedValues = new bool[values.Length];
                    for (int i = 0; i < values.Length; i ++) {
                        checkedValues[i] = values[i].Checked;
                    }
                }

                try {
                    owner.BeginUpdate();
                    owner.InsertItems(owner.itemCount, values, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes) {
                        for (int i = 0; i < values.Length; i ++) {
                            if (checkedValues[i]) {
                                owner.UpdateSavedCheckedItems(values[i], true /*addItem*/);
                            }
                        }
                    }
                    
                }
                finally {
                    owner.listItemSorter = comparer;
                    owner.EndUpdate();
                }

                if (this.owner.ExpectingMouseUp) {
                    this.owner.ItemCollectionChangedInMouseDown = true;
                }

                if (comparer != null || (owner.Sorting != SortOrder.None) && !owner.VirtualMode) {
                    owner.Sort();
                }
            }

            private int DisplayIndexToID(int displayIndex) {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this method does not make any sense");
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed) {
                    // Obtain internal index of the item
                    NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                    lvItem.mask = NativeMethods.LVIF_PARAM;
                    lvItem.iItem = displayIndex;
                    UnsafeNativeMethods.SendMessage(new HandleRef(owner, owner.Handle), NativeMethods.LVM_GETITEM, 0, ref lvItem);
                    return (int) lvItem.lParam;
                }
                else
                {
                    return this[displayIndex].ID;
                }
            }

            public void Clear() {
                if (owner.itemCount > 0) {
                    owner.ApplyUpdateCachedItems();

                    if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed) {
                        // walk the items to see which ones are selected.
                        // we use the LVM_GETNEXTITEM message to see what the next selected item is
                        // so we can avoid checking selection for each one.
                        //
                        int count = owner.Items.Count;
                        int nextSelected = (int)UnsafeNativeMethods.SendMessage(new HandleRef(owner, owner.Handle), NativeMethods.LVM_GETNEXTITEM, -1, NativeMethods.LVNI_SELECTED);
                        for (int i = 0; i < count; i++) {
                            ListViewItem item = owner.Items[i];
                            Debug.Assert(item != null, "Failed to get item at index " + i.ToString(CultureInfo.InvariantCulture));
                            if (item != null) {

                                // if it's the one we're looking for, ask for the next one
                                //
                                if (i == nextSelected) {
                                    item.StateSelected = true;
                                    nextSelected = (int)UnsafeNativeMethods.SendMessage(new HandleRef(owner, owner.Handle), NativeMethods.LVM_GETNEXTITEM, nextSelected, NativeMethods.LVNI_SELECTED);
                                }
                                else {
                                    // otherwise it's false
                                    //
                                    item.StateSelected = false;
                                }
                                item.UnHost(i, false);
                            }
                        }
                        Debug.Assert(owner.listItemsArray == null, "listItemsArray not null, even though handle created");

                        UnsafeNativeMethods.SendMessage(new HandleRef(owner, owner.Handle), NativeMethods.LVM_DELETEALLITEMS, 0, 0);

                        // There's a problem in the list view that if it's in small icon, it won't pick upo the small icon
                        // sizes until it changes from large icon, so we flip it twice here...
                        //
                        if (owner.View == View.SmallIcon) {
                            if (this.owner.ComctlSupportsVisualStyles) {
                                this.owner.FlipViewToLargeIconAndSmallIcon = true;
                            } else {
                                Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon, "we only set this when comctl 6.0 is loaded");
                                owner.View = View.LargeIcon;
                                owner.View = View.SmallIcon;
                            }
                        }
                    }
                    else {

                       int count = owner.Items.Count;

                       for (int i = 0; i < count; i++) {
                           ListViewItem item = owner.Items[i];
                           if (item != null) {
                               item.UnHost(i, true);
                           }
                       }

                       Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                       owner.listItemsArray.Clear();
                    }

                    owner.listItemsTable.Clear();
                    if (owner.IsHandleCreated && !owner.CheckBoxes) {
                        owner.savedCheckedItems = null;
                    }
                    owner.itemCount = 0;

                    if (this.owner.ExpectingMouseUp) {
                        this.owner.ItemCollectionChangedInMouseDown = true;
                    }

                }
            }

            public bool Contains(ListViewItem item) {

                owner.ApplyUpdateCachedItems();
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed) {
                    Debug.Assert(owner.listItemsArray == null, "listItemsArray not null, even though handle created");
                    return owner.listItemsTable[item.ID] == item;
                }
                else {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    return owner.listItemsArray.Contains(item);
                }
            }

            public ListViewItem Insert(int index, ListViewItem item) {
                int count = 0;
                if (owner.VirtualMode) {
                    count = Count;
                }
                else {
                    count = owner.itemCount;
                }

                if (index < 0 || index > count) {
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon || this.Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (index < count) {
                    // if we're not inserting at the end, force the add.
                    //
                    owner.ApplyUpdateCachedItems();
                }

                owner.InsertItems(index, new ListViewItem[]{item}, true);
                if (owner.IsHandleCreated && !owner.CheckBoxes && item.Checked) {
                    owner.UpdateSavedCheckedItems(item, true /*addItem*/);
                }

                if (this.owner.ExpectingMouseUp) {
                    this.owner.ItemCollectionChangedInMouseDown = true;
                }

                return item;
            }

            public int IndexOf(ListViewItem item) {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this function does not make any sense");
                for(int i=0; i < Count; i++) {
                    if (item == this[i]) {
                        return i;
                    }
                }
                return -1;
            }

            public void Remove(ListViewItem item) {
                int index = owner.VirtualMode ? Count - 1 : IndexOf(item);

                Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon || this.Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }
            
                if (index != -1) {
                    RemoveAt(index);
                }
            }

            public void RemoveAt(int index) {
                if (owner.VirtualMode) {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index < 0 || index >= owner.itemCount)
                    throw new ArgumentOutOfRangeException("index", string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

                Debug.Assert(!this.owner.FlipViewToLargeIconAndSmallIcon || this.Count == 0, "the FlipView... bit is turned off after adding 1 item.");
                
                if (owner.IsHandleCreated && !owner.CheckBoxes && this[index].Checked) {
                    owner.UpdateSavedCheckedItems(this[index], false /*addItem*/);
                }

                owner.ApplyUpdateCachedItems();
                int itemID = DisplayIndexToID(index);
                this[index].UnHost(true);

                if (owner.IsHandleCreated) {

                    Debug.Assert(owner.listItemsArray == null, "listItemsArray not null, even though handle created");
                    int retval = unchecked( (int) (long)owner.SendMessage(NativeMethods.LVM_DELETEITEM, index, 0));

                    if (0 == retval)
                        throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                                  "index",
                                                                  (index).ToString(CultureInfo.CurrentCulture)));
                }
                else {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    owner.listItemsArray.RemoveAt(index);
                }

                owner.itemCount--;
                owner.listItemsTable.Remove(itemID);

                if (this.owner.ExpectingMouseUp) {
                    this.owner.ItemCollectionChangedInMouseDown = true;
                }

            }

            public void CopyTo(Array dest, int index) {
                if (owner.itemCount > 0) {
                    for(int displayIndex=0; displayIndex < Count; ++displayIndex) {
                        dest.SetValue(this[displayIndex], index++);
                    }
                }
            }

            public IEnumerator GetEnumerator() {
                ListViewItem[] items = new ListViewItem[this.owner.itemCount];
                this.CopyTo(items, 0);

                return items.GetEnumerator();
            }
        }

        /// <summary>
        /// Creates the new instance of AccessibleObject for this ListView control.
        /// Returning ListViewAccessibleObject is only available in applications that
        /// are recompiled to target .NET Framework 4.7.3 or opt-in into this feature
        /// using a compatibility switch.
        /// </summary>
        /// <returns>
        /// The AccessibleObject for this ListView instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level3) {
                return new ListViewAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }

        internal class ListViewAccessibleObject : ControlAccessibleObject {

            private ListView owner;

            internal ListViewAccessibleObject(ListView owner) : base(owner) {
                this.owner = owner;
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (owner != null) {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_ItemStatusPropertyId) {
                    switch (owner.Sorting) {
                        case SortOrder.None:
                            return SR.NotSortedAccessibleStatus;
                        case SortOrder.Ascending:
                            return SR.SortedAscendingAccessibleStatus;
                        case SortOrder.Descending:
                            return SR.SortedDescendingAccessibleStatus;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
