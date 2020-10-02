// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.ListViewGroup;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays a list of items in one of four
    ///  views. Each item displays a caption and optionally an image.
    /// </summary>
    [Docking(DockingBehavior.Ask)]
    [Designer("System.Windows.Forms.Design.ListViewDesigner, " + AssemblyRef.SystemDesign)]
    [DefaultProperty(nameof(Items))]
    [DefaultEvent(nameof(SelectedIndexChanged))]
    [SRDescription(nameof(SR.DescriptionListView))]
    public partial class ListView : Control
    {
        //members
        private const int MASK_HITTESTFLAG = 0x00F7;

        private static readonly object EVENT_CACHEVIRTUALITEMS = new object();
        private static readonly object EVENT_COLUMNREORDERED = new object();
        private static readonly object EVENT_COLUMNWIDTHCHANGED = new object();
        private static readonly object EVENT_COLUMNWIDTHCHANGING = new object();
        private static readonly object EVENT_DRAWCOLUMNHEADER = new object();
        private static readonly object EVENT_DRAWITEM = new object();
        private static readonly object EVENT_DRAWSUBITEM = new object();
        private static readonly object EVENT_ITEMSELECTIONCHANGED = new object();
        private static readonly object EVENT_RETRIEVEVIRTUALITEM = new object();
        private static readonly object EVENT_SEARCHFORVIRTUALITEM = new object();
        private static readonly object EVENT_SELECTEDINDEXCHANGED = new object();
        private static readonly object EVENT_VIRTUALITEMSSELECTIONRANGECHANGED = new object();
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED = new object();
        private static readonly object EVENT_GROUPCOLLAPSEDSTATECHANGED = new object();
        private static readonly object EVENT_GROUPTASKLINKCLICK = new object();

        private ItemActivation activation = ItemActivation.Standard;
        private ListViewAlignment alignStyle = ListViewAlignment.Top;
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        private ColumnHeaderStyle headerStyle = ColumnHeaderStyle.Clickable;
        private SortOrder sorting = SortOrder.None;
        private View viewStyle = System.Windows.Forms.View.LargeIcon;
        private string toolTipCaption = string.Empty;

        private const int LISTVIEWSTATE_ownerDraw = 0x00000001;
        private const int LISTVIEWSTATE_allowColumnReorder = 0x00000002;
        private const int LISTVIEWSTATE_autoArrange = 0x00000004;
        private const int LISTVIEWSTATE_checkBoxes = 0x00000008;
        private const int LISTVIEWSTATE_fullRowSelect = 0x00000010;
        private const int LISTVIEWSTATE_gridLines = 0x00000020;
        private const int LISTVIEWSTATE_hideSelection = 0x00000040;
        private const int LISTVIEWSTATE_hotTracking = 0x00000080;
        private const int LISTVIEWSTATE_labelEdit = 0x00000100;
        private const int LISTVIEWSTATE_labelWrap = 0x00000200;
        private const int LISTVIEWSTATE_multiSelect = 0x00000400;
        private const int LISTVIEWSTATE_scrollable = 0x00000800;
        private const int LISTVIEWSTATE_hoverSelection = 0x00001000;
        private const int LISTVIEWSTATE_nonclickHdr = 0x00002000;
        private const int LISTVIEWSTATE_inLabelEdit = 0x00004000;
        private const int LISTVIEWSTATE_showItemToolTips = 0x00008000;
        private const int LISTVIEWSTATE_backgroundImageTiled = 0x00010000;
        private const int LISTVIEWSTATE_columnClicked = 0x00020000;
        private const int LISTVIEWSTATE_doubleclickFired = 0x00040000;
        private const int LISTVIEWSTATE_mouseUpFired = 0x00080000;
        private const int LISTVIEWSTATE_expectingMouseUp = 0x00100000;
        private const int LISTVIEWSTATE_showGroups = 0x00800000;
        private const int LISTVIEWSTATE_handleDestroyed = 0x01000000; // while we are recreating the handle we want to know if we can still get data from the handle
        private const int LISTVIEWSTATE_virtualMode = 0x02000000;
        private const int LISTVIEWSTATE_headerControlTracking = 0x04000000;
        private const int LISTVIEWSTATE_itemCollectionChangedInMouseDown = 0x08000000;
        private const int LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon = 0x10000000;
        private const int LISTVIEWSTATE_headerDividerDblClick = 0x20000000;
        private const int LISTVIEWSTATE_columnResizeCancelled = 0x40000000;

        private const int LISTVIEWSTATE1_insertingItemsNatively = 0x00000001;
        private const int LISTVIEWSTATE1_cancelledColumnWidthChanging = 0x00000002;
        private const int LISTVIEWSTATE1_disposingImageLists = 0x00000004;
        private const int LISTVIEWSTATE1_useCompatibleStateImageBehavior = 0x00000008;
        private const int LISTVIEWSTATE1_selectedIndexChangedSkipped = 0x00000010;

        private const int LVTOOLTIPTRACKING = 0x30;
        private const int MAXTILECOLUMNS = 20;

        // PERF: take all the bools and put them into a state variable
        private Collections.Specialized.BitVector32 listViewState; // see LISTVIEWSTATE_ consts above
        private Collections.Specialized.BitVector32 listViewState1;// see LISTVIEWSTATE1_ consts above

        // Ownerdraw data caches...  Only valid inside WM_PAINT.
        //

        private Color odCacheForeColor = SystemColors.WindowText;
        private Color odCacheBackColor = SystemColors.Window;
        private Font odCacheFont;
        private Gdi32.HFONT odCacheFontHandle;
        private FontHandleWrapper odCacheFontHandleWrapper;

        private ImageList _imageListLarge;
        private ImageList _imageListSmall;
        private ImageList _imageListState;
        private ImageList _imageListGroup;

        private MouseButtons downButton;
        private int itemCount;
        private int columnIndex;
        private int topIndex;
        private bool hoveredAlready;

        private bool rightToLeftLayout;

        // member variables which are used for VirtualMode
        private int virtualListSize;

        private ListViewGroup defaultGroup;
        private ListViewGroup focusedGroup;

        // Invariant: the table always contains all Items in the ListView, and maps IDs -> Items.
        // listItemsArray is null if the handle is created; otherwise, it contains all Items.
        // We do not try to sort listItemsArray as items are added, but during a handle recreate
        // we will make sure we get the items in the same order the ListView displays them.
        private readonly Hashtable listItemsTable = new Hashtable(); // elements are ListViewItem's
        private ArrayList listItemsArray = new ArrayList(); // elements are ListViewItem's

        private Size tileSize = Size.Empty;

        // when we are in delayed update mode (that is when BeginUpdate has been called, we want to cache the items to
        // add until EndUpdate is called. To do that, we push in an array list into our PropertyStore
        // under this key.  When Endupdate is fired, we process the items all at once.
        //
        private static readonly int PropDelayedUpdateItems = PropertyStore.CreateKey();

        private int updateCounter; // the counter we use to track how many BeginUpdate/EndUpdate calls there have been.

        private ColumnHeader[] columnHeaders;
        private readonly ListViewItemCollection listItemCollection;
        private readonly ColumnHeaderCollection columnHeaderCollection;
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
        private int nextID;

        // We save selected and checked items between handle creates.
        private List<ListViewItem> savedSelectedItems;
        private List<ListViewItem> savedCheckedItems;

        // Sorting
        private IComparer listItemSorter;

        private ListViewItem prevHoveredItem;

        // Background image stuff
        // Because we have to create a temporary file and the OS does not clean up the temporary files from the machine
        // we have to do that ourselves
        string backgroundImageFileName = string.Empty;

        // it *seems* that if the user changes the background image then the win32 listView will hang on to the previous
        // background image until it gets the first WM_PAINT message -  I use words like *seems* because nothing is guaranteed
        // when it comes to win32 listView.
        // so our wrapper has to hang on to the previousBackgroundImageFileNames and destroy them after it gets the first WM_PAINT message

        int bkImgFileNamesCount = -1;
        string[] bkImgFileNames;
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

        /// <summary>
        ///  Creates an empty ListView with default styles.
        /// </summary>
        public ListView() : base()
        {
            int listViewStateFlags = LISTVIEWSTATE_scrollable |
                                     LISTVIEWSTATE_multiSelect |
                                     LISTVIEWSTATE_labelWrap |
                                     LISTVIEWSTATE_autoArrange |
                                     LISTVIEWSTATE_showGroups;

            listViewState = new Collections.Specialized.BitVector32(listViewStateFlags);

            listViewState1 = new Collections.Specialized.BitVector32(LISTVIEWSTATE1_useCompatibleStateImageBehavior);
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);

            odCacheFont = Font;
            odCacheFontHandle = FontHandle;
            SetBounds(0, 0, 121, 97);

            listItemCollection = new ListViewItemCollection(new ListViewNativeItemCollection(this));
            columnHeaderCollection = new ColumnHeaderCollection(this);
        }

        /// <summary>
        ///  The activation style specifies what kind of user action is required to
        ///  activate an item.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(ItemActivation.Standard)]
        [SRDescription(nameof(SR.ListViewActivationDescr))]
        public ItemActivation Activation
        {
            get
            {
                return activation;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ItemActivation.Standard, (int)ItemActivation.TwoClick))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ItemActivation));
                }

                if (HotTracking && value != ItemActivation.OneClick)
                {
                    throw new ArgumentException(SR.ListViewActivationMustBeOnWhenHotTrackingIsOn, nameof(value));
                }

                if (activation != value)
                {
                    activation = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        ///  The alignment style specifies which side of the window items are aligned
        ///  to by default
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(ListViewAlignment.Top)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ListViewAlignmentDescr))]
        public ListViewAlignment Alignment
        {
            get
            {
                return alignStyle;
            }

            set
            {
                // using this as ListViewAlignment has discontiguous values.
                if (!ClientUtils.IsEnumValid_NotSequential(value,
                                                (int)value,
                                                (int)ListViewAlignment.Default,
                                                (int)ListViewAlignment.Top,
                                                (int)ListViewAlignment.Left,
                                                (int)ListViewAlignment.SnapToGrid))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ListViewAlignment));
                }
                if (alignStyle != value)
                {
                    alignStyle = value;
                    RecreateHandleInternal();
                }
            }
        }

        /// <summary>
        ///  Specifies whether the user can drag column headers to
        ///  other column positions, thus changing the order of displayed columns.
        ///  This property is only meaningful in Details view.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewAllowColumnReorderDescr))]
        public bool AllowColumnReorder
        {
            get
            {
                return listViewState[LISTVIEWSTATE_allowColumnReorder];
            }

            set
            {
                if (AllowColumnReorder != value)
                {
                    listViewState[LISTVIEWSTATE_allowColumnReorder] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        ///  If AutoArrange is true items are automatically arranged according to
        ///  the alignment property.  Items are also kept snapped to grid.
        ///  This property is only meaningful in Large Icon or Small Icon views.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ListViewAutoArrangeDescr))]
        public bool AutoArrange
        {
            get
            {
                return listViewState[LISTVIEWSTATE_autoArrange];
            }

            set
            {
                if (AutoArrange != value)
                {
                    listViewState[LISTVIEWSTATE_autoArrange] = value;
                    UpdateStyles();
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                if (ShouldSerializeBackColor())
                {
                    return base.BackColor;
                }
                else
                {
                    return SystemColors.Window;
                }
            }
            set
            {
                base.BackColor = value;
                if (IsHandleCreated)
                {
                    User32.SendMessageW(this, (User32.WM)LVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(BackColor));
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewBackgroundImageTiledDescr))]
        public unsafe bool BackgroundImageTiled
        {
            get
            {
                return listViewState[LISTVIEWSTATE_backgroundImageTiled];
            }
            set
            {
                if (BackgroundImageTiled != value)
                {
                    listViewState[LISTVIEWSTATE_backgroundImageTiled] = value;
                    if (IsHandleCreated && BackgroundImage != null)
                    {
                        // Don't call SetBackgroundImage because SetBackgroundImage deletes the existing image
                        // We don't need to delete it and this causes BAD problems w/ the Win32 list view control.
                        fixed (char* pBackgroundImageFileName = backgroundImageFileName)
                        {
                            var lvbkImage = new LVBKIMAGEW();
                            if (BackgroundImageTiled)
                            {
                                lvbkImage.ulFlags = LVBKIF.STYLE_TILE;
                            }
                            else
                            {
                                lvbkImage.ulFlags = LVBKIF.STYLE_NORMAL;
                            }

                            lvbkImage.ulFlags |= LVBKIF.SOURCE_URL;
                            lvbkImage.pszImage = pBackgroundImageFileName;
                            lvbkImage.cchImageMax = (uint)(backgroundImageFileName.Length + 1);

                            User32.SendMessageW(this, (User32.WM)LVM.SETBKIMAGEW, IntPtr.Zero, ref lvbkImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Describes the border style of the window.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.Fixed3D)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.borderStyleDescr))]
        public BorderStyle BorderStyle
        {
            get => borderStyle;
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value)
                {
                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  If CheckBoxes is true, every item will display a checkbox next
        ///  to it.  The user can change the state of the item by clicking the checkbox.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewCheckBoxesDescr))]
        public bool CheckBoxes
        {
            get
            {
                return listViewState[LISTVIEWSTATE_checkBoxes];
            }

            set
            {
                if (UseCompatibleStateImageBehavior)
                {
                    if (CheckBoxes != value)
                    {
                        if (value && View == View.Tile)
                        {
                            throw new NotSupportedException(SR.ListViewCheckBoxesNotSupportedInTileView);
                        }

                        if (CheckBoxes)
                        {
                            // Save away the checked items just in case we re-activate checkboxes
                            //
                            savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                            ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                            CheckedItems.CopyTo(items, 0);
                            for (int i = 0; i < items.Length; i++)
                            {
                                savedCheckedItems.Add(items[i]);
                            }
                        }

                        listViewState[LISTVIEWSTATE_checkBoxes] = value;
                        UpdateExtendedStyles();

                        if (CheckBoxes && savedCheckedItems != null)
                        {
                            // Check the saved checked items.
                            //
                            if (savedCheckedItems.Count > 0)
                            {
                                foreach (ListViewItem item in savedCheckedItems)
                                {
                                    item.Checked = true;
                                }
                            }
                            savedCheckedItems = null;
                        }

                        // Comctl should handle auto-arrange for us, but doesn't
                        if (AutoArrange)
                        {
                            ArrangeIcons(Alignment);
                        }
                    }
                }
                else
                {
                    if (CheckBoxes != value)
                    {
                        if (value && View == View.Tile)
                        {
                            throw new NotSupportedException(SR.ListViewCheckBoxesNotSupportedInTileView);
                        }

                        if (CheckBoxes)
                        {
                            // Save away the checked items just in case we re-activate checkboxes
                            //
                            savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                            ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                            CheckedItems.CopyTo(items, 0);
                            for (int i = 0; i < items.Length; i++)
                            {
                                savedCheckedItems.Add(items[i]);
                            }
                        }

                        listViewState[LISTVIEWSTATE_checkBoxes] = value;

                        if ((!value && StateImageList != null && IsHandleCreated) ||
                            (!value && Alignment == ListViewAlignment.Left && IsHandleCreated) ||
                            (value && View == View.List && IsHandleCreated) ||
                            (value && (View == View.SmallIcon || View == View.LargeIcon) && IsHandleCreated))
                        {
                            // we have to recreate the handle when we are going from CheckBoxes == true to CheckBoxes == false
                            // if we want to have the bitmaps from the StateImageList on the items.

                            // There are a LOT of issues with setting CheckBoxes to true when in View.List,
                            // View.SmallIcon or View.LargeIcon:
                            // these are caused by the fact that the win32 ListView control does not
                            // resize its column width when CheckBoxes changes from false to true.
                            // we need to recreate the handle when we set CheckBoxes to TRUE
                            RecreateHandleInternal();
                        }
                        else
                        {
                            UpdateExtendedStyles();
                        }

                        if (CheckBoxes && savedCheckedItems != null)
                        {
                            // Check the saved checked items.
                            //
                            if (savedCheckedItems.Count > 0)
                            {
                                foreach (ListViewItem item in savedCheckedItems)
                                {
                                    item.Checked = true;
                                }
                            }
                            savedCheckedItems = null;
                        }

                        // Setting the LVS_CHECKBOXES window style also causes the ListView to display the default checkbox
                        // images rather than the user specified StateImageList.  We send a LVM_SETIMAGELIST to restore the
                        // user's images.
                        if (IsHandleCreated && _imageListState != null)
                        {
                            if (CheckBoxes)
                            { // we want custom checkboxes
                                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, _imageListState.CreateUniqueHandle());
                                if (previousHandle != IntPtr.Zero)
                                {
                                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                                    Debug.Assert(result.IsTrue());
                                }
                            }
                            else
                            {
                                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, IntPtr.Zero);
                                if (previousHandle != IntPtr.Zero)
                                {
                                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                                    Debug.Assert(result.IsTrue());
                                }
                            }
                        }

                        // Comctl should handle auto-arrange for us, but doesn't
                        if (AutoArrange)
                        {
                            ArrangeIcons(Alignment);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  The indices of the currently checked list items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CheckedIndexCollection CheckedIndices
        {
            get
            {
                if (checkedIndexCollection is null)
                {
                    checkedIndexCollection = new CheckedIndexCollection(this);
                }
                return checkedIndexCollection;
            }
        }

        /// <summary>
        ///  The currently checked list items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CheckedListViewItemCollection CheckedItems
        {
            get
            {
                if (checkedListViewItemCollection is null)
                {
                    checkedListViewItemCollection = new CheckedListViewItemCollection(this);
                }
                return checkedListViewItemCollection;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.ListViewColumnsDescr))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ColumnHeaderCollection Columns
        {
            get
            {
                return columnHeaderCollection;
            }
        }

        /// <summary>
        ///  Computes the handle creation parameters for the ListView control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ClassName = WindowClasses.WC_LISTVIEW;

                // Keep the scrollbar if we are just updating styles...
                //
                if (IsHandleCreated)
                {
                    int currentStyle = unchecked((int)((long)User32.GetWindowLong(this, User32.GWL.STYLE)));
                    cp.Style |= (currentStyle & (int)(User32.WS.HSCROLL | User32.WS.VSCROLL));
                }

                // disabled until ownership management of list handles is fixed
                // https://github.com/dotnet/winforms/issues/3531
                //cp.Style |= (int)LVS.SHAREIMAGELISTS;

                switch (alignStyle)
                {
                    case ListViewAlignment.Top:
                        cp.Style |= (int)LVS.ALIGNTOP;
                        break;
                    case ListViewAlignment.Left:
                        cp.Style |= (int)LVS.ALIGNLEFT;
                        break;
                }

                if (AutoArrange)
                {
                    cp.Style |= (int)LVS.AUTOARRANGE;
                }

                switch (borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)User32.WS.BORDER;
                        break;
                }

                switch (headerStyle)
                {
                    case ColumnHeaderStyle.None:
                        cp.Style |= (int)LVS.NOCOLUMNHEADER;
                        break;
                    case ColumnHeaderStyle.Nonclickable:
                        cp.Style |= (int)LVS.NOSORTHEADER;
                        break;
                }

                if (LabelEdit)
                {
                    cp.Style |= (int)LVS.EDITLABELS;
                }

                if (!LabelWrap)
                {
                    cp.Style |= (int)LVS.NOLABELWRAP;
                }

                if (!HideSelection)
                {
                    cp.Style |= (int)LVS.SHOWSELALWAYS;
                }

                if (!MultiSelect)
                {
                    cp.Style |= (int)LVS.SINGLESEL;
                }

                if (listItemSorter is null)
                {
                    switch (sorting)
                    {
                        case SortOrder.Ascending:
                            cp.Style |= (int)LVS.SORTASCENDING;
                            break;
                        case SortOrder.Descending:
                            cp.Style |= (int)LVS.SORTDESCENDING;
                            break;
                    }
                }

                if (VirtualMode)
                {
                    cp.Style |= (int)LVS.OWNERDATA;
                }

                // We can do this 'cuz the viewStyle enums are the same values as the actual LVS styles
                // this new check since the value for LV_VIEW_TILE == LVS_SINGLESEL; so dont OR that value since
                // LV_VIEW_TILE is not a STYLE but should be Send via a SENDMESSAGE.
                if (viewStyle != View.Tile)
                {
                    cp.Style |= (int)viewStyle;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
                {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= (int)User32.WS_EX.LAYOUTRTL;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
                }
                return cp;
            }
        }

        internal ListViewGroup DefaultGroup
        {
            get
            {
                if (defaultGroup is null)
                {
                    defaultGroup = new ListViewGroup(string.Format(SR.ListViewGroupDefaultGroup, "1"));
                    defaultGroup.ListView = this;
                }

                return defaultGroup;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(121, 97);
            }
        }

        protected override bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set
            {
                if (DoubleBuffered != value)
                {
                    base.DoubleBuffered = value;
                    UpdateExtendedStyles();
                }
            }
        }

        internal bool ExpectingMouseUp
        {
            get
            {
                return listViewState[LISTVIEWSTATE_expectingMouseUp];
            }
        }

        /// <summary>
        ///  Retreives the group which currently has the user focus.  This is the
        ///  group that's drawn with the dotted focus rectangle around it.
        ///  Returns null if no group is currently focused.
        /// </summary>
        internal ListViewGroup FocusedGroup
        {
            get => IsHandleCreated ? focusedGroup : null;
            set
            {
                if (IsHandleCreated && value != null)
                {
                    value.Focused = true;
                    focusedGroup = value;
                }
            }
        }

        /// <summary>
        ///  Retreives the item which currently has the user focus.  This is the
        ///  item that's drawn with the dotted focus rectangle around it.
        ///  Returns null if no item is currently focused.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewFocusedItemDescr))]
        public ListViewItem FocusedItem
        {
            get
            {
                if (IsHandleCreated)
                {
                    int displayIndex = PARAM.ToInt(User32.SendMessageW(this, (User32.WM)LVM.GETNEXTITEM, (IntPtr)(-1), (IntPtr)LVNI.FOCUSED));
                    if (displayIndex > -1)
                    {
                        return Items[displayIndex];
                    }
                }
                return null;
            }
            set
            {
                if (IsHandleCreated && value != null)
                {
                    value.Focused = true;
                }
            }
        }

        public override Color ForeColor
        {
            get
            {
                if (ShouldSerializeForeColor())
                {
                    return base.ForeColor;
                }
                else
                {
                    return SystemColors.WindowText;
                }
            }
            set
            {
                base.ForeColor = value;
                if (IsHandleCreated)
                {
                    User32.SendMessageW(this, (User32.WM)LVM.SETTEXTCOLOR, IntPtr.Zero, PARAM.FromColor(ForeColor));
                }
            }
        }

        private bool FlipViewToLargeIconAndSmallIcon
        {
            get
            {
                // it never hurts to check that our house is in order
                Debug.Assert(!listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || View == View.SmallIcon, "we need this bit only in SmallIcon view");
                Debug.Assert(!listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || Application.ComCtlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

                return listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon];
            }
            set
            {
                // it never hurts to check that our house is in order
                Debug.Assert(!value || View == View.SmallIcon, "we need this bit only in SmallIcon view");
                Debug.Assert(!value || Application.ComCtlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

                listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] = value;
            }
        }

        /// <summary>
        ///  Specifies whether a click on an item will select the entire row instead
        ///  of just the item itself.
        ///  This property is only meaningful in Details view
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewFullRowSelectDescr))]
        public bool FullRowSelect
        {
            get
            {
                return listViewState[LISTVIEWSTATE_fullRowSelect];
            }
            set
            {
                if (FullRowSelect != value)
                {
                    listViewState[LISTVIEWSTATE_fullRowSelect] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        ///  If true, draws grid lines between items and subItems.
        ///  This property is only meaningful in Details view
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewGridLinesDescr))]
        public bool GridLines
        {
            get
            {
                return listViewState[LISTVIEWSTATE_gridLines];
            }

            set
            {
                if (GridLines != value)
                {
                    listViewState[LISTVIEWSTATE_gridLines] = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        ///  The currently set GroupIcon image list.
        /// </summary>
        /// <value>
        ///  An <see cref="ImageList"/> that contains the icons to use for <see cref="ListViewGroup"/>.
        ///  The default is <see langword="null"/>.
        /// </value>
        /// <remarks>
        ///  <para>
        ///   The <see cref="GroupImageList"/> property allows you to specify an <see cref="ImageList"/> object that
        ///   contains icons to use when displaying groups. The <see cref="ListView"/> control can accept any graphics
        ///   format that the <see cref="ImageList"/> control supports when displaying icons. The <see cref="ListView"/>
        ///   control is not limited to .ico files. Once an <see cref="ImageList"/> is assigned to the <see cref="GroupImageList"/>
        ///   property, you can set the <see cref="ListViewGroup.TitleImageIndex"/> property of each <see cref="ListViewGroup"/>
        ///   in the <see cref="ListView"/> control to the index position of the appropriate image in the <see cref="ImageList"/>.
        ///   The size of the icons for the <see cref="GroupImageList"/> is specified by the <see cref="ImageList.ImageSize"/> property.
        ///  </para>
        /// </remarks>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ListViewGroupImageListDescr))]
        public ImageList GroupImageList
        {
            get => _imageListGroup;
            set
            {
                if (_imageListGroup == value)
                {
                    return;
                }

                DetachGroupImageListHandlers();
                _imageListGroup = value;
                AttachGroupImageListHandlers();

                if (!IsHandleCreated)
                {
                    return;
                }

                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.GROUPHEADER,
                        value is null ? IntPtr.Zero : value.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }
        }

        /// <summary>
        ///  The collection of groups belonging to this ListView
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListViewGroupCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.ListViewGroupsDescr))]
        [MergableProperty(false)]
        public ListViewGroupCollection Groups
        {
            get
            {
                if (groups is null)
                {
                    groups = new ListViewGroupCollection(this);
                }
                return groups;
            }
        }

        // this essentially means that the version of CommCtl supports list view grouping
        // and that the user wants to make use of list view groups
        internal bool GroupsEnabled
        {
            get
            {
                return ShowGroups && groups != null && groups.Count > 0 && Application.ComCtlSupportsVisualStyles && !VirtualMode;
            }
        }

        /// <summary>
        ///  Column headers can either be invisible, clickable, or non-clickable.
        ///  This property is only meaningful in Details view
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(ColumnHeaderStyle.Clickable)]
        [SRDescription(nameof(SR.ListViewHeaderStyleDescr))]
        public ColumnHeaderStyle HeaderStyle
        {
            get { return headerStyle; }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ColumnHeaderStyle.None, (int)ColumnHeaderStyle.Clickable))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ColumnHeaderStyle));
                }
                if (headerStyle != value)
                {
                    // We can switch between NONE and either *one* of the other styles without
                    // recreating the handle, but if we change from CLICKABLE to NONCLICKABLE
                    // or vice versa, with or without an intervening setting of NONE, then
                    // the handle needs to be recreated.
                    headerStyle = value;
                    if ((listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Clickable) ||
                        (!listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Nonclickable))
                    {
                        listViewState[LISTVIEWSTATE_nonclickHdr] = !listViewState[LISTVIEWSTATE_nonclickHdr];
                        RecreateHandleInternal();
                    }
                    else
                    {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <summary>
        ///  If false, selected items will still be highlighted (in a
        ///  different color) when focus is moved away from the ListView.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ListViewHideSelectionDescr))]
        public bool HideSelection
        {
            get
            {
                return listViewState[LISTVIEWSTATE_hideSelection];
            }

            set
            {
                if (HideSelection != value)
                {
                    listViewState[LISTVIEWSTATE_hideSelection] = value;
                    UpdateStyles();
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewHotTrackingDescr))]
        public bool HotTracking
        {
            get
            {
                return listViewState[LISTVIEWSTATE_hotTracking];
            }
            set
            {
                if (HotTracking != value)
                {
                    listViewState[LISTVIEWSTATE_hotTracking] = value;
                    if (value)
                    {
                        HoverSelection = true;
                        Activation = ItemActivation.OneClick;
                    }
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        ///  Determines whether items can be selected by hovering over them with the mouse.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewHoverSelectDescr))]
        public bool HoverSelection
        {
            get
            {
                return listViewState[LISTVIEWSTATE_hoverSelection];
            }

            set
            {
                if (HoverSelection != value)
                {
                    if (HotTracking && !value)
                    {
                        throw new ArgumentException(SR.ListViewHoverMustBeOnWhenHotTrackingIsOn, nameof(value));
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
                return listViewState1[LISTVIEWSTATE1_insertingItemsNatively];
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewInsertionMarkDescr))]
        public ListViewInsertionMark InsertionMark
        {
            get
            {
                if (insertionMark is null)
                {
                    insertionMark = new ListViewInsertionMark(this);
                }
                return insertionMark;
            }
        }

        private bool ItemCollectionChangedInMouseDown
        {
            get
            {
                return listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown];
            }
            set
            {
                listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown] = value;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.ListViewItemsDescr))]
        [MergableProperty(false)]
        public ListViewItemCollection Items
        {
            get
            {
                return listItemCollection;
            }
        }

        /// <summary>
        ///  Tells whether the EditLabels style is currently set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewLabelEditDescr))]
        public bool LabelEdit
        {
            get
            {
                return listViewState[LISTVIEWSTATE_labelEdit];
            }
            set
            {
                if (LabelEdit != value)
                {
                    listViewState[LISTVIEWSTATE_labelEdit] = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  Tells whether the LabelWrap style is currently set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ListViewLabelWrapDescr))]
        public bool LabelWrap
        {
            get
            {
                return listViewState[LISTVIEWSTATE_labelWrap];
            }
            set
            {
                if (LabelWrap != value)
                {
                    listViewState[LISTVIEWSTATE_labelWrap] = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  The Currently set ImageList for Large Icon mode.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ListViewLargeImageListDescr))]
        public ImageList LargeImageList
        {
            get => _imageListLarge;
            set
            {
                if (value == _imageListLarge)
                {
                    return;
                }

                DetachLargeImageListHandlers();
                _imageListLarge = value;
                AttachLargeImageListHandlers();

                if (!IsHandleCreated)
                {
                    return;
                }

                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.NORMAL, value is null ? IntPtr.Zero : value.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }

                if (AutoArrange && !listViewState1[LISTVIEWSTATE1_disposingImageLists])
                {
                    UpdateListViewItemsLocations();
                }
            }
        }

        /// <summary>
        ///  Returns the current LISTVIEWSTATE_handleDestroyed value so that this
        ///  value can be accessed from child classes.
        /// </summary>
        internal bool ListViewHandleDestroyed
        {
            get
            {
                return listViewState[LISTVIEWSTATE_handleDestroyed];
            }
            set
            {
                listViewState[LISTVIEWSTATE_handleDestroyed] = value;
            }
        }

        /// <summary>
        ///  The sorting comparer for this ListView.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewItemSorterDescr))]
        public IComparer ListViewItemSorter
        {
            get
            {
                return listItemSorter;
            }
            set
            {
                if (listItemSorter != value)
                {
                    listItemSorter = value;

                    if (!VirtualMode)
                    {
                        Sort();
                    }
                }
            }
        }

        /// <summary>
        ///  Tells whether the MultiSelect style is currently set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ListViewMultiSelectDescr))]
        public bool MultiSelect
        {
            get
            {
                return listViewState[LISTVIEWSTATE_multiSelect];
            }
            set
            {
                if (MultiSelect != value)
                {
                    listViewState[LISTVIEWSTATE_multiSelect] = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  Indicates whether the list view items (and sub-items in the Details view) will be
        ///  drawn by the system or the user. This includes the column header when item index = -1.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewOwnerDrawDescr))]
        public bool OwnerDraw
        {
            get
            {
                return listViewState[LISTVIEWSTATE_ownerDraw];
            }

            set
            {
                if (OwnerDraw != value)
                {
                    listViewState[LISTVIEWSTATE_ownerDraw] = value;
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        ///  This is used for international applications where the language is written from RightToLeft.
        ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
        ///  the form, and control placement and text will be from right to left.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
        public virtual bool RightToLeftLayout
        {
            get
            {
                return rightToLeftLayout;
            }

            set
            {
                if (value != rightToLeftLayout)
                {
                    rightToLeftLayout = value;
                    using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                    {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
        }

        /// <summary>
        ///  Tells whether the ScrollBars are visible or not.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ListViewScrollableDescr))]
        public bool Scrollable
        {
            get
            {
                return listViewState[LISTVIEWSTATE_scrollable];
            }
            set
            {
                if (Scrollable != value)
                {
                    listViewState[LISTVIEWSTATE_scrollable] = value;
                    RecreateHandleInternal();
                }
            }
        }

        /// <summary>
        ///  The indices of the currently selected list items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectedIndexCollection SelectedIndices
        {
            get
            {
                if (selectedIndexCollection is null)
                {
                    selectedIndexCollection = new SelectedIndexCollection(this);
                }
                return selectedIndexCollection;
            }
        }

        /// <summary>
        ///  The currently selected list items.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewSelectedItemsDescr))]
        public SelectedListViewItemCollection SelectedItems
        {
            get
            {
                if (selectedListViewItemCollection is null)
                {
                    selectedListViewItemCollection = new SelectedListViewItemCollection(this);
                }
                return selectedListViewItemCollection;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ListViewShowGroupsDescr))]
        public bool ShowGroups
        {
            get
            {
                return listViewState[LISTVIEWSTATE_showGroups];
            }
            set
            {
                if (value != ShowGroups)
                {
                    listViewState[LISTVIEWSTATE_showGroups] = value;
                    if (IsHandleCreated)
                    {
                        UpdateGroupView();
                    }
                }
            }
        }

        /// <summary>
        ///  The currently set SmallIcon image list.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ListViewSmallImageListDescr))]
        public ImageList SmallImageList
        {
            get => _imageListSmall;
            set
            {
                if (_imageListSmall == value)
                {
                    return;
                }

                DetachSmallImageListListHandlers();
                _imageListSmall = value;
                AttachSmallImageListListHandlers();

                if (!IsHandleCreated)
                {
                    return;
                }

                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.SMALL, value is null ? IntPtr.Zero : value.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }

                if (View == View.SmallIcon)
                {
                    View = View.LargeIcon;
                    View = View.SmallIcon;
                }
                else if (!listViewState1[LISTVIEWSTATE1_disposingImageLists])
                {
                    UpdateListViewItemsLocations();
                }

                if (View == View.Details)
                {
                    Invalidate(invalidateChildren: true);
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListViewShowItemToolTipsDescr))]
        public bool ShowItemToolTips
        {
            get
            {
                return listViewState[LISTVIEWSTATE_showItemToolTips];
            }
            set
            {
                if (ShowItemToolTips != value)
                {
                    listViewState[LISTVIEWSTATE_showItemToolTips] = value;
                    RecreateHandleInternal();
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(SortOrder.None)]
        [SRDescription(nameof(SR.ListViewSortingDescr))]
        public SortOrder Sorting
        {
            get
            {
                return sorting;
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SortOrder.None, (int)SortOrder.Descending))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SortOrder));
                }
                if (sorting != value)
                {
                    sorting = value;
                    if (View == View.LargeIcon || View == View.SmallIcon)
                    {
                        if (listItemSorter is null)
                        {
                            listItemSorter = new IconComparer(sorting);
                        }
                        else if (listItemSorter is IconComparer)
                        {
                            ((IconComparer)listItemSorter).SortOrder = sorting;
                        }
                    }
                    else if (value == SortOrder.None)
                    {
                        listItemSorter = null;
                    }

                    // If we're changing to No Sorting, no need to recreate the handle
                    // because none of the existing items need to be rearranged.
                    if (value == SortOrder.None)
                    {
                        UpdateStyles();
                    }
                    else
                    {
                        RecreateHandleInternal();
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ListViewStateImageListDescr))]
        public ImageList StateImageList
        {
            get => _imageListState;
            set
            {
                if (_imageListState == value)
                {
                    return;
                }

                if (UseCompatibleStateImageBehavior)
                {
                    DetachStateImageListHandlers();
                    _imageListState = value;
                    AttachStateImageListHandlers();

                    if (IsHandleCreated)
                    {
                        var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, value is null ? IntPtr.Zero : value.CreateUniqueHandle());
                        if (previousHandle != IntPtr.Zero)
                        {
                            var result = ComCtl32.ImageList.Destroy(previousHandle);
                            Debug.Assert(result.IsTrue());
                        }
                    }
                }
                else
                {
                    DetachStateImageListHandlers();

                    if (IsHandleCreated && _imageListState != null && CheckBoxes)
                    {
                        // If CheckBoxes are set to true, then we will have to recreate the handle.
                        // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native listView destroys
                        // the state imageList.
                        // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
                        // So we make the native listView forget about its StateImageList just before we recreate the handle.
                        // Likely related to https://devblogs.microsoft.com/oldnewthing/20171128-00/?p=97475
                        var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, IntPtr.Zero);
                        if (previousHandle != IntPtr.Zero)
                        {
                            var result = ComCtl32.ImageList.Destroy(previousHandle);
                            Debug.Assert(result.IsTrue());
                        }
                    }

                    _imageListState = value;
                    AttachStateImageListHandlers();

                    if (!IsHandleCreated)
                    {
                        return;
                    }

                    if (CheckBoxes)
                    {
                        // need to recreate to get the new images pushed in.
                        RecreateHandleInternal();
                    }
                    else
                    {
                        var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE,
                            (_imageListState is null || _imageListState.Images.Count == 0) ? IntPtr.Zero : _imageListState.CreateUniqueHandle());
                        if (previousHandle != IntPtr.Zero)
                        {
                            var result = ComCtl32.ImageList.Destroy(previousHandle);
                            Debug.Assert(result.IsTrue());
                        }
                    }

                    // Comctl should handle auto-arrange for us, but doesn't
                    if (!listViewState1[LISTVIEWSTATE1_disposingImageLists])
                    {
                        UpdateListViewItemsLocations();
                    }
                }
            }
        }

        internal override bool SupportsUiaProviders => true;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(true)]
        [SRDescription(nameof(SR.ListViewTileSizeDescr))]
        public unsafe Size TileSize
        {
            get
            {
                if (!tileSize.IsEmpty)
                {
                    return tileSize;
                }

                if (!IsHandleCreated)
                {
                    return Size.Empty;
                }

                var tileViewInfo = new LVTILEVIEWINFO
                {
                    cbSize = (uint)sizeof(LVTILEVIEWINFO),
                    dwMask = LVTVIM.TILESIZE
                };
                User32.SendMessageW(this, (User32.WM)LVM.GETTILEVIEWINFO, IntPtr.Zero, ref tileViewInfo);

                return tileViewInfo.sizeTile;
            }
            set
            {
                if (tileSize == value)
                {
                    return;
                }

                if (value.Height <= 0 || value.Width <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(TileSize), SR.ListViewTileSizeMustBePositive);
                }

                tileSize = value;
                if (!IsHandleCreated)
                {
                    return;
                }

                var tileViewInfo = new LVTILEVIEWINFO
                {
                    cbSize = (uint)sizeof(LVTILEVIEWINFO),
                    dwMask = LVTVIM.TILESIZE,
                    dwFlags = LVTVIF.FIXEDSIZE,
                    sizeTile = tileSize
                };
                IntPtr retval = User32.SendMessageW(this, (User32.WM)LVM.SETTILEVIEWINFO, IntPtr.Zero, ref tileViewInfo);
                Debug.Assert(retval != IntPtr.Zero, "LVM_SETTILEVIEWINFO failed");

                if (AutoArrange)
                {
                    UpdateListViewItemsLocations();
                }
            }
        }

        private bool ShouldSerializeTileSize()
        {
            return !tileSize.Equals(Size.Empty);
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewTopItemDescr))]
        public ListViewItem TopItem
        {
            get
            {
                if (viewStyle == View.LargeIcon || viewStyle == View.SmallIcon || viewStyle == View.Tile)
                {
                    throw new InvalidOperationException(SR.ListViewGetTopItem);
                }

                if (!IsHandleCreated)
                {
                    if (Items.Count > 0)
                    {
                        return Items[0];
                    }
                    else
                    {
                        return null;
                    }
                }

                topIndex = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.GETTOPINDEX));
                if (topIndex >= 0 && topIndex < Items.Count)
                {
                    return Items[topIndex];
                }

                return null;
            }
            set
            {
                if (viewStyle == View.LargeIcon || viewStyle == View.SmallIcon || viewStyle == View.Tile)
                {
                    throw new InvalidOperationException(SR.ListViewSetTopItem);
                }

                if (value is null)
                {
                    return;
                }

                if (value.ListView != this)
                {
                    return;
                }

                if (!IsHandleCreated)
                {
                    CreateHandle();
                }
                if (value == TopItem)
                {
                    return;
                }

                EnsureVisible(value.Index);
                ListViewItem topItem = TopItem;

                if ((topItem is null) && (topIndex == Items.Count)) //
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
                {
                    return;
                }

                if (Scrollable)
                {
                    Scroll(topItem.Index, value.Index);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DefaultValue(true)]
        public bool UseCompatibleStateImageBehavior
        {
            get
            {
                return listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior];
            }
            set
            {
                listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior] = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(View.LargeIcon)]
        [SRDescription(nameof(SR.ListViewViewDescr))]
        public View View
        {
            get
            {
                return viewStyle;
            }
            set
            {
                if (value == View.Tile && CheckBoxes)
                {
                    throw new NotSupportedException(SR.ListViewTileViewDoesNotSupportCheckBoxes);
                }

                FlipViewToLargeIconAndSmallIcon = false;

                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)View.LargeIcon, (int)View.Tile))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(View));
                }

                if (value == View.Tile && VirtualMode)
                {
                    throw new NotSupportedException(SR.ListViewCantSetViewToTileViewInVirtualMode);
                }

                if (viewStyle != value)
                {
                    viewStyle = value;
                    if (IsHandleCreated && Application.ComCtlSupportsVisualStyles)
                    {
                        User32.SendMessageW(this, (User32.WM)LVM.SETVIEW, (IntPtr)viewStyle);
                        UpdateGroupView();

                        // if we switched to Tile view we should update the win32 list view tile view info
                        if (viewStyle == View.Tile)
                        {
                            UpdateTileView();
                        }
                    }
                    else
                    {
                        UpdateStyles();
                    }

                    UpdateListViewItemsLocations();
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ListViewVirtualListSizeDescr))]
        public int VirtualListSize
        {
            get
            {
                return virtualListSize;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(string.Format(SR.ListViewVirtualListSizeInvalidArgument, "value", value));
                }

                if (value == virtualListSize)
                {
                    return;
                }

                bool keepTopItem = IsHandleCreated && VirtualMode && View == View.Details && !DesignMode;
                int topIndex = -1;
                if (keepTopItem)
                {
                    topIndex = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.GETTOPINDEX));
                }

                virtualListSize = value;

                if (IsHandleCreated && VirtualMode && !DesignMode)
                {
                    User32.SendMessageW(this, (User32.WM)LVM.SETITEMCOUNT, (IntPtr)virtualListSize);
                }

                if (keepTopItem)
                {
                    topIndex = Math.Min(topIndex, VirtualListSize - 1);
                    // After setting the virtual list size ComCtl makes the first item the top item.
                    // So we set the top item only if it wasn't the first item to begin with.
                    if (topIndex > 0)
                    {
                        ListViewItem lvItem = Items[topIndex];
                        TopItem = lvItem;
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ListViewVirtualModeDescr))]
        public bool VirtualMode
        {
            get
            {
                return listViewState[LISTVIEWSTATE_virtualMode];
            }
            set
            {
                if (value == VirtualMode)
                {
                    return;
                }

                if (value && Items.Count > 0)
                {
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoItems);
                }

                if (value && CheckedItems.Count > 0)
                {
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoCheckedItems);
                }

                if (value && SelectedItems.Count > 0)
                {
                    throw new InvalidOperationException(SR.ListViewVirtualListViewRequiresNoSelectedItems);
                }

                // Tile view does not work w/ VirtualMode.
                if (value && View == View.Tile)
                {
                    throw new NotSupportedException(SR.ListViewCantSetVirtualModeWhenInTileView);
                }

                listViewState[LISTVIEWSTATE_virtualMode] = value;

                RecreateHandleInternal();
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewAfterLabelEditDescr))]
        public event LabelEditEventHandler AfterLabelEdit
        {
            add => onAfterLabelEdit += value;
            remove => onAfterLabelEdit -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewBeforeLabelEditDescr))]
        public event LabelEditEventHandler BeforeLabelEdit
        {
            add => onBeforeLabelEdit += value;
            remove => onBeforeLabelEdit -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewCacheVirtualItemsEventDescr))]
        public event CacheVirtualItemsEventHandler CacheVirtualItems
        {
            add => Events.AddHandler(EVENT_CACHEVIRTUALITEMS, value);
            remove => Events.RemoveHandler(EVENT_CACHEVIRTUALITEMS, value);
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewColumnClickDescr))]
        public event ColumnClickEventHandler ColumnClick
        {
            add => onColumnClick += value;
            remove => onColumnClick -= value;
        }

        /// <summary>
        ///  Occurs when the user clicks a <see cref="ListViewGroup.TaskLink"/> on a <see cref="ListViewGroup"/>.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewGroupTaskLinkClickDescr))]
        public event EventHandler<ListViewGroupEventArgs> GroupTaskLinkClick
        {
            add => Events.AddHandler(EVENT_GROUPTASKLINKCLICK, value);
            remove => Events.RemoveHandler(EVENT_GROUPTASKLINKCLICK, value);
        }

        /// <summary>
        ///  Tell the user that the column headers are being rearranged
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListViewColumnReorderedDscr))]
        public event ColumnReorderedEventHandler ColumnReordered
        {
            add => Events.AddHandler(EVENT_COLUMNREORDERED, value);
            remove => Events.RemoveHandler(EVENT_COLUMNREORDERED, value);
        }

        /// <summary>
        ///  Tell the user that the column width changed
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListViewColumnWidthChangedDscr))]
        public event ColumnWidthChangedEventHandler ColumnWidthChanged
        {
            add => Events.AddHandler(EVENT_COLUMNWIDTHCHANGED, value);
            remove => Events.RemoveHandler(EVENT_COLUMNWIDTHCHANGED, value);
        }

        /// <summary>
        ///  Tell the user that the column width is being changed
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListViewColumnWidthChangingDscr))]
        public event ColumnWidthChangingEventHandler ColumnWidthChanging
        {
            add => Events.AddHandler(EVENT_COLUMNWIDTHCHANGING, value);
            remove => Events.RemoveHandler(EVENT_COLUMNWIDTHCHANGING, value);
        }

        /// <summary>
        ///  Fires in owner draw + Details mode when a column header needs to be drawn.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewDrawColumnHeaderEventDescr))]
        public event DrawListViewColumnHeaderEventHandler DrawColumnHeader
        {
            add => Events.AddHandler(EVENT_DRAWCOLUMNHEADER, value);
            remove => Events.RemoveHandler(EVENT_DRAWCOLUMNHEADER, value);
        }

        /// <summary>
        ///  Fires in owner draw mode when a ListView item needs to be drawn.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewDrawItemEventDescr))]
        public event DrawListViewItemEventHandler DrawItem
        {
            add => Events.AddHandler(EVENT_DRAWITEM, value);
            remove => Events.RemoveHandler(EVENT_DRAWITEM, value);
        }

        /// <summary>
        ///  Fires in owner draw mode and Details view when a ListView sub-item needs to be drawn.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewDrawSubItemEventDescr))]
        public event DrawListViewSubItemEventHandler DrawSubItem
        {
            add => Events.AddHandler(EVENT_DRAWSUBITEM, value);
            remove => Events.RemoveHandler(EVENT_DRAWSUBITEM, value);
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewItemClickDescr))]
        public event EventHandler ItemActivate
        {
            add => onItemActivate += value;
            remove => onItemActivate -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.CheckedListBoxItemCheckDescr))]
        public event ItemCheckEventHandler ItemCheck
        {
            add => onItemCheck += value;
            remove => onItemCheck -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewItemCheckedDescr))]
        public event ItemCheckedEventHandler ItemChecked
        {
            add => onItemChecked += value;
            remove => onItemChecked -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewItemDragDescr))]
        public event ItemDragEventHandler ItemDrag
        {
            add => onItemDrag += value;
            remove => onItemDrag -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewItemMouseHoverDescr))]
        public event ListViewItemMouseHoverEventHandler ItemMouseHover
        {
            add => onItemMouseHover += value;
            remove => onItemMouseHover -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewItemSelectionChangedDescr))]
        public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged
        {
            add => Events.AddHandler(EVENT_ITEMSELECTIONCHANGED, value);
            remove => Events.RemoveHandler(EVENT_ITEMSELECTIONCHANGED, value);
        }

        /// <summary>
        ///  Occurs when the <see cref="ListViewGroup.CollapsedState"/> changes on a <see cref="ListViewGroup"/>.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewGroupCollapsedStateChangedDescr))]
        public event EventHandler<ListViewGroupEventArgs> GroupCollapsedStateChanged
        {
            add => Events.AddHandler(EVENT_GROUPCOLLAPSEDSTATECHANGED, value);
            remove => Events.RemoveHandler(EVENT_GROUPCOLLAPSEDSTATECHANGED, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  ListView Onpaint.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewRetrieveVirtualItemEventDescr))]
        public event RetrieveVirtualItemEventHandler RetrieveVirtualItem
        {
            add => Events.AddHandler(EVENT_RETRIEVEVIRTUALITEM, value);
            remove => Events.RemoveHandler(EVENT_RETRIEVEVIRTUALITEM, value);
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewSearchForVirtualItemDescr))]
        public event SearchForVirtualItemEventHandler SearchForVirtualItem
        {
            add => Events.AddHandler(EVENT_SEARCHFORVIRTUALITEM, value);
            remove => Events.RemoveHandler(EVENT_SEARCHFORVIRTUALITEM, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewSelectedIndexChangedDescr))]
        public event EventHandler SelectedIndexChanged
        {
            add => Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
            remove => Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewVirtualItemsSelectionRangeChangedDescr))]
        public event ListViewVirtualItemsSelectionRangeChangedEventHandler VirtualItemsSelectionRangeChanged
        {
            add => Events.AddHandler(EVENT_VIRTUALITEMSSELECTIONRANGECHANGED, value);
            remove => Events.RemoveHandler(EVENT_VIRTUALITEMSSELECTIONRANGECHANGED, value);
        }

        /// <summary>
        ///  Called to add any delayed update items we have to the list view.  We do this because
        ///  we have optimnized the case where a user is only adding items within a beginupdate/endupdate
        ///  block.  If they do any other operations (get the count, remove, insert, etc.), we push in the
        ///  cached up items first, then do the requested operation.  This keeps it simple so we don't have to
        ///  try to maintain parellel state of the cache during a begin update end update.
        /// </summary>
        private void ApplyUpdateCachedItems()
        {
            // first check if there is a delayed update array
            //
            ArrayList newItems = (ArrayList)Properties.GetObject(PropDelayedUpdateItems);
            if (newItems != null)
            {
                // if there is, clear it and push the items in.
                //
                Properties.SetObject(PropDelayedUpdateItems, null);
                ListViewItem[] items = (ListViewItem[])newItems.ToArray(typeof(ListViewItem));
                if (items.Length > 0)
                {
                    InsertItems(itemCount, items, false /*checkHosting*/);
                }
            }
        }

        /// <summary>
        ///  In Large Icon or Small Icon view, arranges the items according to one
        ///  of the following behaviors:
        /// </summary>
        public void ArrangeIcons(ListViewAlignment value)
        {
            // LVM_ARRANGE only work in SmallIcon view
            if (viewStyle != View.SmallIcon)
            {
                return;
            }

            switch ((LVA)value)
            {
                case LVA.DEFAULT:
                case LVA.ALIGNLEFT:
                case LVA.ALIGNTOP:
                case LVA.SNAPTOGRID:
                    if (IsHandleCreated)
                    {
                        User32.PostMessageW(this, (User32.WM)LVM.ARRANGE, (IntPtr)value, IntPtr.Zero);
                    }
                    break;

                default:
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
            }

            if (!VirtualMode && sorting != SortOrder.None)
            {
                Sort();
            }
        }

        /// <summary>
        ///  In Large Icon or Small Icon view, arranges items according to the ListView's
        ///  current alignment style.
        /// </summary>
        public void ArrangeIcons() => ArrangeIcons((ListViewAlignment)LVA.DEFAULT);

        private void AttachGroupImageListHandlers()
        {
            if (_imageListGroup is null)
            {
                return;
            }

            // NOTE: any handlers added here should be removed in DetachGroupImageListHandlers
            _imageListGroup.RecreateHandle += new EventHandler(GroupImageListRecreateHandle);
            _imageListGroup.Disposed += new EventHandler(DetachImageList);
            _imageListGroup.ChangeHandle += new EventHandler(GroupImageListChangedHandle);
        }

        private void AttachLargeImageListHandlers()
        {
            if (_imageListLarge is null)
            {
                return;
            }

            // NOTE: any handlers added here should be removed in DetachLargeImageListHandlers
            _imageListLarge.RecreateHandle += new EventHandler(LargeImageListRecreateHandle);
            _imageListLarge.Disposed += new EventHandler(DetachImageList);
            _imageListLarge.ChangeHandle += new EventHandler(LargeImageListChangedHandle);
        }

        private void AttachSmallImageListListHandlers()
        {
            if (_imageListSmall is null)
            {
                return;
            }

            // NOTE: any handlers added here should be removed in DetachSmallImageListListHandlers
            _imageListSmall.RecreateHandle += new EventHandler(SmallImageListRecreateHandle);
            _imageListSmall.Disposed += new EventHandler(DetachImageList);
        }

        private void AttachStateImageListHandlers()
        {
            if (_imageListState is null)
            {
                return;
            }

            // NOTE: any handlers added here should be removed in DetachStateImageListHandlers
            _imageListState.RecreateHandle += new EventHandler(StateImageListRecreateHandle);
            _imageListState.Disposed += new EventHandler(DetachImageList);
        }

        public void AutoResizeColumns(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }
            UpdateColumnWidths(headerAutoResize);
        }

        public void AutoResizeColumn(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }
            SetColumnWidth(columnIndex, headerAutoResize);
        }

        /// <summary>
        ///  Prevents the ListView from redrawing itself until EndUpdate is called.
        ///  Calling this method before individually adding or removing a large number of Items
        ///  will improve performance and reduce flicker on the ListView as items are
        ///  being updated.  Always call EndUpdate immediately after the last item is updated.
        /// </summary>
        public void BeginUpdate()
        {
            BeginUpdateInternal();

            // if this is the first BeginUpdate call, push an ArrayList into the PropertyStore so
            // we can cache up any items that have been added while this is active.
            //
            if (updateCounter++ == 0 && null == Properties.GetObject(PropDelayedUpdateItems))
            {
                Properties.SetObject(PropDelayedUpdateItems, new ArrayList());
            }
        }

        internal void CacheSelectedStateForItem(ListViewItem lvi, bool selected)
        {
            if (selected)
            {
                if (savedSelectedItems is null)
                {
                    savedSelectedItems = new List<ListViewItem>();
                }
                if (!savedSelectedItems.Contains(lvi))
                {
                    savedSelectedItems.Add(lvi);
                }
            }
            else
            {
                if (savedSelectedItems != null && savedSelectedItems.Contains(lvi))
                {
                    savedSelectedItems.Remove(lvi);
                }
            }
        }

#if DEBUG
        private void CheckDisplayIndices()
        {
            // sanity check
            // all the column headers should have a displayIndex between 0 and this.Columns.Count - 1;
            // DisplayIndex should be different for different column headers
            int sumOfDisplayIndices = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                sumOfDisplayIndices += Columns[i].DisplayIndex;
                Debug.Assert(Columns[i].DisplayIndex > -1 && Columns[i].DisplayIndex < Columns.Count, "display indices out of whack");
            }
            int colsCount = Columns.Count;
            Debug.Assert(sumOfDisplayIndices == (colsCount - 1) * colsCount / 2, "display indices out of whack");
        }
#endif

        private void CleanPreviousBackgroundImageFiles()
        {
            if (bkImgFileNames is null)
            {
                return;
            }

            IO.FileInfo fi;
            for (int i = 0; i <= bkImgFileNamesCount; i++)
            {
                fi = new IO.FileInfo(bkImgFileNames[i]);
                if (fi.Exists)
                {
                    //
                    // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                    // I could not find any resources which explain in detail when the IImgCtx objects
                    // release the temporary file. So if we get a FileIO when we delete the temporary file
                    // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                    try
                    {
                        fi.Delete();
                    }
                    catch (IO.IOException) { }
                }
            }

            bkImgFileNames = null;
            bkImgFileNamesCount = -1;
        }

        /// <summary>
        ///  Removes all items and columns from the ListView.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            Columns.Clear();
        }

        /// <summary>
        ///  This is the sorting callback function called by the system ListView control.
        /// </summary>
        private int CompareFunc(IntPtr lparam1, IntPtr lparam2, IntPtr lparamSort)
        {
            Debug.Assert(listItemSorter != null, "null sorter!");
            if (listItemSorter != null)
            {
                return listItemSorter.Compare(listItemsTable[(int)lparam1], listItemsTable[(int)lparam2]);
            }
            else
            {
                return 0;
            }
        }

        private unsafe int CompensateColumnHeaderResize(Message m, bool columnResizeCancelled)
        {
            if (Application.ComCtlSupportsVisualStyles &&
                View == View.Details &&
                !columnResizeCancelled &&
                Items.Count > 0)
            {
                NMHEADERW* header = (NMHEADERW*)m.LParam;
                return CompensateColumnHeaderResize(header->iItem, columnResizeCancelled);
            }
            else
            {
                return 0;
            }
        }

        private int CompensateColumnHeaderResize(int columnIndex, bool columnResizeCancelled)
        {
            // We need to compensate padding only when ComCtl60 is loaded.
            // We need to compensate padding only if the list view is in Details mode.
            // We need to compensate padding only if the user did not cancel ColumnWidthChanging event.
            // We need to compensate padding only if the list view contains items.
            // We need to compensate padding if the user resizes the first column.
            // If the list view has a small image list then:
            //  1. if there is a list view item w/ ImageIndex > -1 then we don't have to resize the first column.
            //  2. Otherwise, we need to add 18 pixels.
            // If the list view does not have a small image list then we need to add 2 pixels.

            if (Application.ComCtlSupportsVisualStyles &&
                View == View.Details &&
                !columnResizeCancelled &&
                Items.Count > 0)
            {
                // The user resized the first column.
                if (columnIndex == 0)
                {
                    ColumnHeader col = (columnHeaders != null && columnHeaders.Length > 0) ? columnHeaders[0] : null;
                    if (col != null)
                    {
                        if (SmallImageList is null)
                        {
                            return 2;
                        }
                        else
                        {
                            // If the list view contains an item w/ a non-negative ImageIndex then we don't need to
                            // add extra padding.
                            bool addPadding = true;
                            for (int i = 0; i < Items.Count; i++)
                            {
                                if (Items[i].ImageIndexer.ActualIndex > -1)
                                {
                                    addPadding = false;
                                    break;
                                }
                            }

                            if (addPadding)
                            {
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

        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);

                try
                {
                    var icc = new INITCOMMONCONTROLSEX
                    {
                        dwICC = ICC.LISTVIEW_CLASSES
                    };
                    InitCommonControlsEx(ref icc);
                }
                finally
                {
                    ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();

            // image location
            if (BackgroundImage != null)
            {
                SetBackgroundImage();
            }
        }

        /// <summary>
        ///  Handles custom drawing of list items - for individual item font/color changes.
        ///
        ///  If OwnerDraw is true, we fire the OnDrawItem and OnDrawSubItem (in Details view)
        ///  events and let the user do the drawing.
        /// </summary>
        unsafe void CustomDraw(ref Message m)
        {
            bool dontmess = false;
            bool itemDrawDefault = false;

            try
            {
                NMLVCUSTOMDRAW* nmcd = (NMLVCUSTOMDRAW*)m.LParam;
                // Find out which stage we're drawing
                switch (nmcd->nmcd.dwDrawStage)
                {
                    case CDDS.PREPAINT:
                        if (OwnerDraw)
                        {
                            m.Result = (IntPtr)CDRF.NOTIFYITEMDRAW;
                            return;
                        }

                        // We want custom draw for this paint cycle
                        m.Result = (IntPtr)(CDRF.NOTIFYSUBITEMDRAW | CDRF.NEWFONT);

                        // refresh the cache of the current color & font settings for this paint cycle
                        odCacheBackColor = BackColor;
                        odCacheForeColor = ForeColor;
                        odCacheFont = Font;
                        odCacheFontHandle = FontHandle;

                        // If preparing to paint a group item, make sure its bolded.
                        if (nmcd->dwItemType == LVCDI.GROUP)
                        {
                            odCacheFontHandleWrapper?.Dispose();

                            odCacheFont = new Font(odCacheFont, FontStyle.Bold);
                            odCacheFontHandleWrapper = new FontHandleWrapper(odCacheFont);
                            odCacheFontHandle = odCacheFontHandleWrapper.Handle;
                            Gdi32.SelectObject(nmcd->nmcd.hdc, odCacheFontHandleWrapper.Handle);
                            m.Result = (IntPtr)CDRF.NEWFONT;
                        }

                        return;

                    //We have to return a NOTIFYSUBITEMDRAW (called NOTIFYSUBITEMREDRAW in the docs) here to
                    //get it to enter "change all subitems instead of whole rows" mode.

                    //HOWEVER... we only want to do this for report styles...

                    case CDDS.ITEMPREPAINT:

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
                            using Graphics g = nmcd->nmcd.hdc.CreateGraphics();
                            DrawListViewItemEventArgs e = new DrawListViewItemEventArgs(
                                g,
                                Items[(int)nmcd->nmcd.dwItemSpec],
                                itemBounds,
                                (int)nmcd->nmcd.dwItemSpec,
                                (ListViewItemStates)nmcd->nmcd.uItemState);

                            OnDrawItem(e);

                            itemDrawDefault = e.DrawDefault;

                            // For the Details view, we send a SKIPDEFAULT when we get a sub-item drawing notification.
                            // For other view styles, we do it here.
                            if (viewStyle == View.Details)
                            {
                                m.Result = (IntPtr)CDRF.NOTIFYSUBITEMDRAW;
                            }
                            else
                            {
                                if (!e.DrawDefault)
                                {
                                    m.Result = (IntPtr)CDRF.SKIPDEFAULT;
                                }
                            }

                            if (!e.DrawDefault)
                            {
                                return;   // skip our regular drawing code
                            }
                        }

                        if (viewStyle == View.Details || viewStyle == View.Tile)
                        {
                            m.Result = (IntPtr)(CDRF.NOTIFYSUBITEMDRAW | CDRF.NEWFONT);
                            dontmess = true; // don't mess with our return value!

                            //ITEMPREPAINT is used to work out the rect for the first column!!! GAH!!!
                            //(which means we can't just do our color/font work on SUBITEM|ITEM_PREPAINT)
                            //so fall through... and tell the end of SUBITEM|ITEM_PREPAINT not to mess
                            //with our return value...
                        }

                        //If it's not a report, we fall through and change the main item's styles

                        goto case (CDDS.SUBITEM | CDDS.ITEMPREPAINT);

                    case CDDS.SUBITEM | CDDS.ITEMPREPAINT:

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
                            using Graphics g = nmcd->nmcd.hdc.CreateGraphics();
                            DrawListViewSubItemEventArgs e = null;

                            // by default, we want to skip the customDrawCode
                            bool skipCustomDrawCode = true;

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
                                    subItemBounds.Width = columnHeaders[0].Width;
                                }

                                if (ClientRectangle.IntersectsWith(subItemBounds))
                                {
                                    e = new DrawListViewSubItemEventArgs(
                                        g,
                                        subItemBounds,
                                        Items[itemIndex],
                                        Items[itemIndex].SubItems[nmcd->iSubItem],
                                        itemIndex,
                                        nmcd->iSubItem,
                                        columnHeaders[nmcd->iSubItem],
                                        (ListViewItemStates)nmcd->nmcd.uItemState);
                                    OnDrawSubItem(e);

                                    // the customer still wants to draw the default.
                                    // Don't skip the custom draw code then
                                    skipCustomDrawCode = !e.DrawDefault;
                                }
                            }

                            if (skipCustomDrawCode)
                            {
                                m.Result = (IntPtr)CDRF.SKIPDEFAULT;
                                return; // skip our custom draw code
                            }
                        }

                        // get the node
                        ListViewItem item = Items[(int)nmcd->nmcd.dwItemSpec];
                        // if we're doing the whole row in one style, change our result!
                        if (dontmess && item.UseItemStyleForSubItems)
                        {
                            m.Result = (IntPtr)CDRF.NEWFONT;
                        }
                        Debug.Assert(item != null, "Item was null in ITEMPREPAINT");

                        CDIS state = nmcd->nmcd.uItemState;
                        // There is a known and documented problem in the ListView winctl control -
                        // if the LVS_SHOWSELALWAYS style is set, then the item state will have
                        // the CDIS_SELECTED bit set for all items. So we need to verify with the
                        // real item state to be sure.
                        if (!HideSelection)
                        {
                            LVIS realState = GetItemState((int)nmcd->nmcd.dwItemSpec);
                            if ((realState & LVIS.SELECTED) == 0)
                            {
                                state &= ~CDIS.SELECTED;
                            }
                        }

                        // subitem is invalid if the flag isn't set -- and we also use this code in
                        // cases where subitems aren't visible (ie. non-Details modes), so if subitem
                        // is invalid, point it at the main item's render info

                        int subitem = ((nmcd->nmcd.dwDrawStage & CDDS.SUBITEM) != 0) ? nmcd->iSubItem : 0;

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
                            if (subitem == 0 && (state & CDIS.HOT) != 0 && HotTracking)
                            {
                                disposeSubItemFont = true;
                                subItemFont = new Font(item.SubItems[0].Font, FontStyle.Underline);
                            }
                            else
                            {
                                subItemFont = item.SubItems[subitem].Font;
                            }

                            if (subitem > 0 || (state & (CDIS.SELECTED | CDIS.GRAYED | CDIS.HOT | CDIS.DISABLED)) == 0)
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
                            if ((state & (CDIS.SELECTED
                                        | CDIS.GRAYED
                                        | CDIS.HOT
                                        | CDIS.DISABLED)) != 0)
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

                        if (!haveRenderInfo || subItemFont is null)
                        {
                            // safety net code just in case
                            if (odCacheFont != null)
                            {
                                Gdi32.SelectObject(nmcd->nmcd.hdc, odCacheFontHandle);
                            }
                        }
                        else
                        {
                            if (odCacheFontHandleWrapper != null)
                            {
                                odCacheFontHandleWrapper.Dispose();
                            }
                            odCacheFontHandleWrapper = new FontHandleWrapper(subItemFont);
                            Gdi32.SelectObject(nmcd->nmcd.hdc, odCacheFontHandleWrapper.Handle);
                        }

                        if (!dontmess)
                        {
                            m.Result = (IntPtr)CDRF.NEWFONT;
                        }
                        if (disposeSubItemFont)
                        {
                            subItemFont.Dispose();
                        }
                        return;

                    default:
                        m.Result = (IntPtr)CDRF.DODEFAULT;
                        return;
                }
            }
            catch (Exception e)
            {
                Debug.Fail("Exception occurred attempting to setup custom draw. Disabling custom draw for this control", e.ToString());
                m.Result = (IntPtr)CDRF.DODEFAULT;
            }
        }

        private void DeleteFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                IO.FileInfo fi = new IO.FileInfo(fileName);
                if (fi.Exists)
                {
                    //
                    // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                    // I could not find any resources which explain in detail when the IImgCtx objects
                    // release the temporary file. So if we get a FileIO when we delete the temporary file
                    // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                    try
                    {
                        fi.Delete();
                    }
                    catch (IO.IOException) { }
                }
            }
        }

        /// <summary>
        ///  Resets the imageList to null.  We wire this method up to the imageList's
        ///  Dispose event, so that we don't hang onto an imageList that's gone away.
        /// </summary>
        private void DetachImageList(object sender, EventArgs e)
        {
            listViewState1[LISTVIEWSTATE1_disposingImageLists] = true;
            try
            {
#if DEBUG
                if (sender != _imageListSmall && sender != _imageListState && sender != _imageListLarge && sender != _imageListGroup)
                {
                    Debug.Fail("ListView sunk dispose event from unknown component");
                }
#endif // DEBUG
                if (sender == _imageListSmall)
                {
                    SmallImageList = null;
                }

                if (sender == _imageListLarge)
                {
                    LargeImageList = null;
                }

                if (sender == _imageListState)
                {
                    StateImageList = null;
                }

                if (sender == _imageListGroup)
                {
                    GroupImageList = null;
                }
            }
            finally
            {
                listViewState1[LISTVIEWSTATE1_disposingImageLists] = false;
            }

            UpdateListViewItemsLocations();
        }

        private void DetachGroupImageListHandlers()
        {
            if (_imageListGroup is null)
            {
                return;
            }

            _imageListGroup.RecreateHandle -= new EventHandler(GroupImageListRecreateHandle);
            _imageListGroup.Disposed -= new EventHandler(DetachImageList);
            _imageListGroup.ChangeHandle -= new EventHandler(GroupImageListChangedHandle);
        }

        private void DetachLargeImageListHandlers()
        {
            if (_imageListLarge is null)
            {
                return;
            }

            _imageListLarge.RecreateHandle -= new EventHandler(LargeImageListRecreateHandle);
            _imageListLarge.Disposed -= new EventHandler(DetachImageList);
            _imageListLarge.ChangeHandle -= new EventHandler(LargeImageListChangedHandle);
        }

        private void DetachSmallImageListListHandlers()
        {
            if (_imageListSmall is null)
            {
                return;
            }

            _imageListSmall.RecreateHandle -= new EventHandler(SmallImageListRecreateHandle);
            _imageListSmall.Disposed -= new EventHandler(DetachImageList);
        }

        private void DetachStateImageListHandlers()
        {
            if (_imageListState is null)
            {
                return;
            }

            _imageListState.RecreateHandle -= new EventHandler(StateImageListRecreateHandle);
            _imageListState.Disposed -= new EventHandler(DetachImageList);
        }

        /// <summary>
        ///  Disposes of the component.  Call dispose when the component is no longer needed.
        ///  This method removes the component from its container (if the component has a site)
        ///  and triggers the dispose event.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove any event sinks we have hooked up to imageLists
                DetachSmallImageListListHandlers();
                _imageListSmall = null;
                DetachLargeImageListHandlers();
                _imageListLarge = null;
                DetachStateImageListHandlers();
                _imageListState = null;
                DetachGroupImageListHandlers();
                _imageListGroup = null;

                // Remove any ColumnHeaders contained in this control
                if (columnHeaders != null)
                {
                    for (int colIdx = columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                    {
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

                if (!string.IsNullOrEmpty(backgroundImageFileName) || bkImgFileNames != null)
                {
                    IO.FileInfo fi;
                    if (!string.IsNullOrEmpty(backgroundImageFileName))
                    {
                        fi = new IO.FileInfo(backgroundImageFileName);
                        Debug.Assert(fi.Exists, "who deleted our temp file?");
                        //
                        // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                        // I could not find any resources which explain in detail when the IImgCtx objects
                        // release the temporary file. So if we get a FileIO when we delete the temporary file
                        // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                        try
                        {
                            fi.Delete();
                        }
                        catch (IO.IOException) { }
                        backgroundImageFileName = string.Empty;
                    }
                    for (int i = 0; i <= bkImgFileNamesCount; i++)
                    {
                        fi = new IO.FileInfo(bkImgFileNames[i]);
                        Debug.Assert(fi.Exists, "who deleted our temp file?");
                        //
                        // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                        // I could not find any resources which explain in detail when the IImgCtx objects
                        // release the temporary file. So if we get a FileIO when we delete the temporary file
                        // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                        try
                        {
                            fi.Delete();
                        }
                        catch (IO.IOException) { }
                    }

                    bkImgFileNames = null;
                    bkImgFileNamesCount = -1;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Cancels the effect of BeginUpdate.
        /// </summary>
        public void EndUpdate()
        {
            // On the final EndUpdate, check to see if we've got any cached items.
            // If we do, insert them as normal, then turn off the painting freeze.
            //
            if (--updateCounter == 0 && null != Properties.GetObject(PropDelayedUpdateItems))
            {
                ApplyUpdateCachedItems();
            }
            EndUpdateInternal();
        }

        private void EnsureDefaultGroup()
        {
            if (IsHandleCreated && Application.ComCtlSupportsVisualStyles && GroupsEnabled)
            {
                if (User32.SendMessageW(this, (User32.WM)LVM.HASGROUP, (IntPtr)DefaultGroup.ID) == IntPtr.Zero)
                {
                    UpdateGroupView();
                    InsertGroupNative(0, DefaultGroup);
                }
            }
        }

        /// <summary>
        ///  Ensure that the item is visible, scrolling the view as necessary.
        ///  @index  Index of item to scroll into view
        /// </summary>
        public void EnsureVisible(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)LVM.ENSUREVISIBLE, (IntPtr)index);
            }
        }

        public ListViewItem FindItemWithText(string text)
        {
            // if the user does not use the FindItemWithText overloads that specify a StartIndex and the listView is empty then return null
            if (Items.Count == 0)
            {
                return null;
            }

            return FindItemWithText(text, true, 0, true);
        }

        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex)
        {
            return FindItemWithText(text, includeSubItemsInSearch, startIndex, true);
        }

        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
        {
            if (startIndex < 0 || startIndex >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, string.Format(SR.InvalidArgument, nameof(startIndex), startIndex));
            }
            return FindItem(true, text, isPrefixSearch, new Point(0, 0), SearchDirectionHint.Down, startIndex, includeSubItemsInSearch);
        }

        public ListViewItem FindNearestItem(SearchDirectionHint dir, Point point)
        {
            return FindNearestItem(dir, point.X, point.Y);
        }

        public ListViewItem FindNearestItem(SearchDirectionHint searchDirection, int x, int y)
        {
            if (View != View.SmallIcon && View != View.LargeIcon)
            {
                throw new InvalidOperationException(SR.ListViewFindNearestItemWorksOnlyInIconView);
            }

            if (searchDirection < SearchDirectionHint.Left || searchDirection > SearchDirectionHint.Down)
            {
                throw new ArgumentOutOfRangeException(nameof(searchDirection), searchDirection, string.Format(SR.InvalidArgument, nameof(searchDirection), searchDirection));
            }

            // the win32 ListView::FindNearestItem does some pretty weird things to determine the nearest item.
            // simply passing the (x,y) coordinates will cause problems when we call FindNearestItem for a point inside an item.
            // so we have to do some special processing when (x,y) falls inside an item;
            //
            ListViewItem lvi = GetItemAt(x, y);

            if (lvi != null)
            {
                Rectangle itemBounds = lvi.Bounds;
                // LVM_FINDITEM is a nightmare
                // LVM_FINDITEM will use the top left corner of icon rectangle to determine the closest item
                // What happens if there is no icon for this item? then the top left corner of the icon rectangle falls INSIDE the item label (???)
                //

                Rectangle iconBounds = GetItemRect(lvi.Index, ItemBoundsPortion.Icon);

                switch (searchDirection)
                {
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

            return FindItem(false, string.Empty, false, new Point(x, y), searchDirection, -1, false);
        }

        private unsafe ListViewItem FindItem(bool isTextSearch, string text, bool isPrefixSearch, Point pt, SearchDirectionHint dir, int startIndex, bool includeSubItemsInSearch)
        {
            if (Items.Count == 0)
            {
                return null;
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            if (VirtualMode)
            {
                SearchForVirtualItemEventArgs sviEvent = new SearchForVirtualItemEventArgs(isTextSearch, isPrefixSearch, includeSubItemsInSearch, text, pt, dir, startIndex);

                OnSearchForVirtualItem(sviEvent);
                // NOTE: this will cause a RetrieveVirtualItem event w/o a corresponding cache hint event.
                if (sviEvent.Index != -1)
                {
                    return Items[sviEvent.Index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                fixed (char* pText = text)
                {
                    var lvFindInfo = new LVFINDINFOW();
                    if (isTextSearch)
                    {
                        lvFindInfo.flags = LVFI.STRING;
                        lvFindInfo.flags |= isPrefixSearch ? LVFI.PARTIAL : 0;
                        lvFindInfo.psz = pText;
                    }
                    else
                    {
                        lvFindInfo.flags = LVFI.NEARESTXY;
                        lvFindInfo.pt = pt;
                        // we can do this because SearchDirectionHint is set to the VK_*
                        lvFindInfo.vkDirection = (uint)dir;
                    }
                    lvFindInfo.lParam = IntPtr.Zero;
                    int index = (int)User32.SendMessageW(
                        this,
                        (User32.WM)LVM.FINDITEMW,
                        (IntPtr)(startIndex - 1), // decrement startIndex so that the search is 0-based
                        ref lvFindInfo);
                    if (index >= 0)
                    {
                        return Items[index];
                    }

                    if (isTextSearch && includeSubItemsInSearch)
                    {
                        // win32 listView control can't search inside sub items
                        for (int i = startIndex; i < Items.Count; i++)
                        {
                            ListViewItem lvi = Items[i];
                            for (int j = 0; j < lvi.SubItems.Count; j++)
                            {
                                ListViewItem.ListViewSubItem lvsi = lvi.SubItems[j];
                                // the win32 list view search for items w/ text is case insensitive
                                // do the same for sub items
                                // because we are comparing user defined strings we have to do the slower String search
                                // ie, use String.Compare(string, string, case sensitive, CultureInfo)
                                // instead of new Whidbey String.Equals overload
                                // String.Equals(string, string, StringComparison.OrdinalIgnoreCase
                                if (string.Equals(text, lvsi.Text, StringComparison.OrdinalIgnoreCase))
                                {
                                    return lvi;
                                }

                                if (isPrefixSearch && CultureInfo.CurrentCulture.CompareInfo.IsPrefix(lvsi.Text, text, CompareOptions.IgnoreCase))
                                {
                                    return lvi;
                                }
                            }
                        }

                        return null;
                    }

                    return null;
                }
            }
        }

        private void ForceCheckBoxUpdate()
        {
            // Force ListView to update its checkbox bitmaps.
            //
            if (CheckBoxes && IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETEXTENDEDLISTVIEWSTYLE, (IntPtr)LVS_EX.CHECKBOXES, IntPtr.Zero);
                User32.SendMessageW(this, (User32.WM)LVM.SETEXTENDEDLISTVIEWSTYLE, (IntPtr)LVS_EX.CHECKBOXES, (IntPtr)LVS_EX.CHECKBOXES);

                // Comctl should handle auto-arrange for us, but doesn't
                if (AutoArrange)
                {
                    ArrangeIcons(Alignment);
                }
            }
        }

        private string GenerateRandomName()
        {
            Debug.Assert(BackgroundImage != null, "we need to generate random numbers only when saving the background image to disk");
            Bitmap bm = new Bitmap(BackgroundImage);
            int handle = 0;

            try
            {
                handle = unchecked((int)(long)bm.GetHicon());
            }
            catch
            {
                bm.Dispose();
            }

            Random rnd;
            if (handle == 0)
            {
                // there was a problem when we got the icon handle
                // use DateTime.Now to seed the randomizer
                rnd = new Random((int)System.DateTime.Now.Ticks);
            }
            else
            {
                rnd = new Random(handle);
            }
            return rnd.Next().ToString(CultureInfo.InvariantCulture);
        }

        // IDs for identifying ListViewItem's
        private int GenerateUniqueID()
        {
            // Okay, if someone adds several billion items to the list and doesn't remove all of them,
            // we can reuse the same ID, but I'm willing to take that risk.  We are even tracking IDs
            // on a per-list view basis to reduce the problem.
            int result = nextID++;
            if (result == -1)
            {// leave -1 as a "no such value" ID
                result = 0;
                nextID = 1;
            }
            return result;
        }

        /// <summary>
        ///  Gets the real index for the given item.  lastIndex is the last return
        ///  value from GetDisplayIndex, or -1 if you don't know.  If provided,
        ///  the search for the index can be greatly improved.
        /// </summary>
        internal int GetDisplayIndex(ListViewItem item, int lastIndex)
        {
            Debug.Assert(item.listView == this, "Can't GetDisplayIndex if the list item doesn't belong to us");
            Debug.Assert(item.ID != -1, "ListViewItem has no ID yet");

            ApplyUpdateCachedItems();
            if (IsHandleCreated && !ListViewHandleDestroyed)
            {
                var info = new LVFINDINFOW
                {
                    lParam = (IntPtr)item.ID,
                    flags = LVFI.PARAM
                };

                int displayIndex = -1;

                if (lastIndex != -1)
                {
                    displayIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.FINDITEMW, (IntPtr)(lastIndex - 1), ref info);
                }

                if (displayIndex == -1)
                {
                    displayIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.FINDITEMW, (IntPtr)(-1) /* beginning */, ref info);
                }
                Debug.Assert(displayIndex != -1, "This item is in the list view -- why can't we find a display index for it?");
                return displayIndex;
            }
            else
            {
                // PERF: The only reason we should ever call this before the handle is created
                // is if the user calls ListViewItem.Index.
                Debug.Assert(listItemsArray != null, "listItemsArray is null, but the handle isn't created");

                int index = 0;
                foreach (object o in listItemsArray)
                {
                    if (o == item)
                    {
                        return index;
                    }

                    index++;
                }
                return -1;
            }
        }

        /// <summary>
        ///  Called by ColumnHeader objects to determine their position
        ///  in the ListView
        /// </summary>
        internal int GetColumnIndex(ColumnHeader ch)
        {
            if (columnHeaders is null)
            {
                return -1;
            }

            for (int i = 0; i < columnHeaders.Length; i++)
            {
                if (columnHeaders[i] == ch)
                {
                    return i;
                }
            }

            return -1;
        }

        internal int GetGroupIndex(ListViewGroup owningGroup)
        {
            if (Groups.Count == 0)
            {
                return -1;
            }
            // Default group it's last group in accessibility and not specified in Groups collection, therefore default group index = Groups.Count
            if (DefaultGroup == owningGroup)
            {
                return Groups.Count;
            }

            return Groups.IndexOf(owningGroup);
        }

        /// <summary>
        ///  Returns the current ListViewItem corresponding to the specific
        ///  x,y co-ordinate.
        /// </summary>
        public ListViewItem GetItemAt(int x, int y)
        {
            var lvhi = new LVHITTESTINFO
            {
                pt = new Point(x, y)
            };

            int displayIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.HITTEST, IntPtr.Zero, ref lvhi);

            ListViewItem li = null;
            if (displayIndex >= 0 && ((lvhi.flags & LVHT.ONITEM) != 0))
            {
                li = Items[displayIndex];
            }

            return li;
        }

        internal int GetNativeGroupId(ListViewItem item)
        {
            item.UpdateGroupFromName();

            if (item.Group != null && Groups.Contains(item.Group))
            {
                return item.Group.ID;
            }
            else
            {
                EnsureDefaultGroup();
                return DefaultGroup.ID;
            }
        }

        internal void GetSubItemAt(int x, int y, out int iItem, out int iSubItem)
        {
            var lvhi = new LVHITTESTINFO
            {
                pt = new Point(x, y)
            };

            int index = (int)User32.SendMessageW(this, (User32.WM)LVM.SUBITEMHITTEST, IntPtr.Zero, ref lvhi);
            if (index > -1)
            {
                iItem = lvhi.iItem;
                iSubItem = lvhi.iSubItem;
            }
            else
            {
                iItem = -1;
                iSubItem = -1;
            }
        }

        internal Point GetItemPosition(int index)
        {
            var pt = new Point();
            User32.SendMessageW(this, (User32.WM)LVM.GETITEMPOSITION, (IntPtr)index, ref pt);
            return pt;
        }

        internal LVIS GetItemState(int index)
        {
            return GetItemState(index, LVIS.FOCUSED | LVIS.SELECTED | LVIS.CUT |
                                LVIS.DROPHILITED | LVIS.OVERLAYMASK |
                                LVIS.STATEIMAGEMASK);
        }

        internal LVIS GetItemState(int index, LVIS mask)
        {
            if (index < 0 || (VirtualMode && index >= VirtualListSize) || (!VirtualMode && index >= itemCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            Debug.Assert(IsHandleCreated, "How did we add items without a handle?");
            return unchecked((LVIS)(long)User32.SendMessageW(this, (User32.WM)LVM.GETITEMSTATE, (IntPtr)index, (IntPtr)mask));
        }

        /// <summary>
        ///  Returns a list item's bounding rectangle, including subitems.
        /// </summary>
        public Rectangle GetItemRect(int index)
        {
            return GetItemRect(index, 0);
        }

        /// <summary>
        ///  Returns a specific portion of a list item's bounding rectangle.
        /// </summary>
        public Rectangle GetItemRect(int index, ItemBoundsPortion portion)
        {
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(portion, (int)portion, (int)ItemBoundsPortion.Entire, (int)ItemBoundsPortion.ItemOnly))
            {
                throw new InvalidEnumArgumentException(nameof(portion), (int)portion, typeof(ItemBoundsPortion));
            }

            if (View == View.Details && Columns.Count == 0)
            {
                return Rectangle.Empty;
            }

            var itemrect = new RECT
            {
                left = (int)portion
            };
            if (User32.SendMessageW(this, (User32.WM)LVM.GETITEMRECT, (IntPtr)index, ref itemrect) == IntPtr.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            return Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);
        }

        /// <summary>
        ///  Private version of GetItemRect that fails silently. We use this instead of catching
        ///  exceptions thrown by GetItemRect, to avoid first chance exceptions confusing the user.
        /// </summary>
        private Rectangle GetItemRectOrEmpty(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return Rectangle.Empty;
            }

            if (View == View.Details && Columns.Count == 0)
            {
                return Rectangle.Empty;
            }

            var itemrect = new RECT
            {
                left = 0
            };
            if (User32.SendMessageW(this, (User32.WM)LVM.GETITEMRECT, (IntPtr)index, ref itemrect) == IntPtr.Zero)
            {
                return Rectangle.Empty;
            }

            return Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);
        }

        /// <summary>
        ///  Returns a listview sub-item's bounding rectangle.
        /// </summary>
        internal Rectangle GetSubItemRect(int itemIndex, int subItemIndex)
        {
            return GetSubItemRect(itemIndex, subItemIndex, 0);
        }

        internal Rectangle GetSubItemRect(int itemIndex, int subItemIndex, ItemBoundsPortion portion)
        {
            // it seems that getting the rectangle for a sub item only works for list view which are in Details view
            if (View != View.Details)
            {
                return Rectangle.Empty;
            }
            if (itemIndex < 0 || itemIndex >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
            }
            int subItemCount = Items[itemIndex].SubItems.Count;

            if (subItemIndex < 0 || subItemIndex >= subItemCount)
            {
                throw new ArgumentOutOfRangeException(nameof(subItemIndex), subItemIndex, string.Format(SR.InvalidArgument, nameof(subItemIndex), subItemIndex));
            }
            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(portion, (int)portion, (int)ItemBoundsPortion.Entire, (int)ItemBoundsPortion.ItemOnly))
            {
                throw new InvalidEnumArgumentException(nameof(portion), (int)portion, typeof(ItemBoundsPortion));
            }

            if (Columns.Count == 0)
            {
                return Rectangle.Empty;
            }

            var itemrect = new RECT
            {
                left = (int)portion,
                top = subItemIndex
            };
            if (User32.SendMessageW(this, (User32.WM)LVM.GETSUBITEMRECT, (IntPtr)itemIndex, ref itemrect) == IntPtr.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
            }

            Rectangle result = Rectangle.FromLTRB(itemrect.left, itemrect.top, itemrect.right, itemrect.bottom);

            return result;
        }

        private void GroupImageListChangedHandle(object sender, EventArgs e)
        {
            if (VirtualMode || sender is null || sender != _imageListGroup || !IsHandleCreated)
            {
                return;
            }

            foreach (ListViewGroup group in Groups)
            {
                group.TitleImageIndex = group.ImageIndexer.ActualIndex < _imageListGroup.Images.Count
                    ? group.ImageIndexer.ActualIndex
                    : _imageListGroup.Images.Count - 1;
            }
        }

        private void GroupImageListRecreateHandle(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            IntPtr handle = (GroupImageList is null) ? IntPtr.Zero : GroupImageList.CreateUniqueHandle();
            var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.GROUPHEADER, handle);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }
        }

        public ListViewHitTestInfo HitTest(Point point)
        {
            return HitTest(point.X, point.Y);
        }

        public ListViewHitTestInfo HitTest(int x, int y)
        {
            if (!ClientRectangle.Contains(x, y))
            {
                return new ListViewHitTestInfo(null /*hitItem*/, null /*hitSubItem*/, ListViewHitTestLocations.None /*hitLocation*/);
            }

            var lvhi = new LVHITTESTINFO
            {
                pt = new Point(x, y)
            };

            int iItem;
            if (View == View.Details)
            {
                iItem = (int)User32.SendMessageW(this, (User32.WM)LVM.SUBITEMHITTEST, IntPtr.Zero, ref lvhi);
            }
            else
            {
                iItem = (int)User32.SendMessageW(this, (User32.WM)LVM.HITTEST, IntPtr.Zero, ref lvhi);
            }

            ListViewItem item = (iItem == -1) ? null : Items[iItem];
            ListViewHitTestLocations location = ListViewHitTestLocations.None;

            if (item is null && (LVHT.ABOVE & lvhi.flags) == LVHT.ABOVE)
            {
                location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & (int)lvhi.flags) | (int)ListViewHitTestLocations.AboveClientArea);
            }
            else if (item != null && (LVHT.ONITEMSTATEICON & lvhi.flags) == LVHT.ONITEMSTATEICON)
            {
                location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & (int)lvhi.flags) | (int)ListViewHitTestLocations.StateImage);
            }
            else
            {
                location = (ListViewHitTestLocations)lvhi.flags;
            }

            if (View == View.Details && item != null)
            {
                if (lvhi.iSubItem < item.SubItems.Count)
                {
                    return new ListViewHitTestInfo(item, item.SubItems[lvhi.iSubItem], location);
                }
                else
                {
                    return new ListViewHitTestInfo(item, null, location);
                }
            }
            else
            {
                return new ListViewHitTestInfo(item, null, location);
            }
        }

        private unsafe void InvalidateColumnHeaders()
        {
            if (viewStyle == View.Details && IsHandleCreated)
            {
                IntPtr hwndHdr = User32.SendMessageW(this, (User32.WM)LVM.GETHEADER);
                if (hwndHdr != IntPtr.Zero)
                {
                    User32.InvalidateRect(new HandleRef(this, hwndHdr), null, BOOL.TRUE);
                }
            }
        }

        /// <summary>
        ///  Inserts a new Column into the ListView
        /// </summary>
        internal ColumnHeader InsertColumn(int index, ColumnHeader ch, bool refreshSubItems = true)
        {
            if (ch is null)
            {
                throw new ArgumentNullException(nameof(ch));
            }

            if (ch.OwnerListview != null)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, ch.Text), nameof(ch));
            }

            int idx;
            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (IsHandleCreated && View != View.Tile)
            {
                idx = InsertColumnNative(index, ch);
            }
            else
            {
                idx = index;
            }

            // First column must be left aligned

            if (-1 == idx)
            {
                throw new InvalidOperationException(SR.ListViewAddColumnFailed);
            }

            // Add the column to our internal array
            int columnCount = columnHeaders is null ? 0 : columnHeaders.Length;
            if (columnCount > 0)
            {
                ColumnHeader[] newHeaders = new ColumnHeader[columnCount + 1];
                if (columnCount > 0)
                {
                    System.Array.Copy(columnHeaders, 0, newHeaders, 0, columnCount);
                }

                columnHeaders = newHeaders;
            }
            else
            {
                columnHeaders = new ColumnHeader[1];
            }

            if (idx < columnCount)
            {
                System.Array.Copy(columnHeaders, idx, columnHeaders, idx + 1, columnCount - idx);
            }
            columnHeaders[idx] = ch;
            ch.OwnerListview = this;

            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (ch.ActualImageIndex_Internal != -1 && IsHandleCreated && View != View.Tile)
            {
                SetColumnInfo(LVCF.IMAGE, ch);
            }

            // update the DisplayIndex for each column
            // we are only setting an integer in the ColumnHeader, this is not expensive
            int[] indices = new int[Columns.Count];
            for (int i = 0; i < Columns.Count; i++)
            {
                ColumnHeader hdr = Columns[i];
                if (hdr == ch)
                {
                    // the newly added column
                    hdr.DisplayIndexInternal = index;
                }
                else if (hdr.DisplayIndex >= index)
                {
                    hdr.DisplayIndexInternal++;
                }
                indices[i] = hdr.DisplayIndexInternal;
            }

            SetDisplayIndices(indices);

#if DEBUG
            CheckDisplayIndices();
#endif
            // in Tile view the ColumnHeaders collection is used for the Tile Information
            // recreate the handle in that case
            if (IsHandleCreated && View == View.Tile)
            {
                RecreateHandleInternal();
            }
            else if (IsHandleCreated && refreshSubItems)
            {
                RealizeAllSubItems();
            }

            return ch;
        }

        private unsafe int InsertColumnNative(int index, ColumnHeader ch)
        {
            var lvColumn = new LVCOLUMNW
            {
                mask = LVCF.FMT | LVCF.TEXT | LVCF.WIDTH
            };

            if (ch.OwnerListview != null && ch.ActualImageIndex_Internal != -1)
            {
                lvColumn.mask |= LVCF.IMAGE;
                lvColumn.iImage = ch.ActualImageIndex_Internal;
            }

            lvColumn.fmt = (LVCFMT)ch.TextAlign;
            lvColumn.cx = ch.Width;

            fixed (char* columnHeaderText = ch.Text)
            {
                lvColumn.pszText = columnHeaderText;

                return (int)User32.SendMessageW(this, (User32.WM)LVM.INSERTCOLUMNW, (IntPtr)index, ref lvColumn);
            }
        }

        // when the user adds a group, this helper method makes sure that all the items
        // in the list view are parented by a group - be it the DefaultGroup or some other group
        internal void InsertGroupInListView(int index, ListViewGroup group)
        {
            Debug.Assert(groups != null && groups.Count > 0, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");
            Debug.Assert(group != DefaultGroup, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");

            // the first time we add a group we have to group the items in the Default Group
            bool groupItems = (groups.Count == 1) && GroupsEnabled;

            UpdateGroupView();
            EnsureDefaultGroup();
            InsertGroupNative(index, group);

            // take all the list view items which don't belong to any group and put them in the default group
            //
            if (groupItems)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    ListViewItem item = Items[i];
                    if (item.Group is null)
                    {
                        item.UpdateStateToListView(item.Index);
                    }
                }
            }

#if DEBUG
            // sanity check: all the items in the list view should have a group ID
            if (GroupsEnabled)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    ListViewItem item = Items[i];
                    var lvItem = new LVITEMW
                    {
                        iItem = item.Index,
                        mask = LVIF.GROUPID
                    };
                    User32.SendMessageW(this, (User32.WM)LVM.GETITEMW, IntPtr.Zero, ref lvItem);
                    Debug.Assert(lvItem.iGroupId != -1, "there is a list view item which is not parented");
                }
            }
#endif
        }

        // does the Win32 part of the job of inserting the group
        private unsafe void InsertGroupNative(int index, ListViewGroup group)
        {
            Debug.Assert(IsHandleCreated, "InsertGroupNative precondition: list-view handle must be created");
            Debug.Assert(group == DefaultGroup || Groups.Contains(group), "Make sure ListView.Groups contains this group before adding the native LVGROUP. Otherwise, custom-drawing may break.");

            IntPtr result = SendGroupMessage(group, LVM.INSERTGROUP, (IntPtr)index, LVGF.GROUPID);
            Debug.Assert(result != (IntPtr)(-1), "Failed to insert group");
        }

        /// <summary>
        ///  Inserts a new ListViewItem into the ListView.  The item will be inserted
        ///  either in the correct sorted position, or, if no sorting is set, at the
        ///  position indicated by the index parameter.
        /// </summary>
        private void InsertItems(int displayIndex, ListViewItem[] items, bool checkHosting)
        {
            if (items is null || items.Length == 0)
            {
                return;
            }

            if (IsHandleCreated && Items.Count == 0 && View == View.SmallIcon && Application.ComCtlSupportsVisualStyles)
            {
                FlipViewToLargeIconAndSmallIcon = true;
            }

            // if we're in the middle of a Begin/EndUpdate, just push the items into our array list
            // as they'll get processed on EndUpdate.
            //
            if (updateCounter > 0 && Properties.GetObject(PropDelayedUpdateItems) != null)
            {
                // CheckHosting.
                if (checkHosting)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].listView != null)
                        {
                            throw new ArgumentException(string.Format(SR.OnlyOneControl, items[i].Text), "item");
                        }
                    }
                }

                ArrayList itemList = (ArrayList)Properties.GetObject(PropDelayedUpdateItems);
                Debug.Assert(itemList != null, "In Begin/EndUpdate with no delayed array!");
                if (itemList != null)
                {
                    itemList.AddRange(items);
                }

                // add the list view item to the list view
                // this way we can retrieve the item's index inside BeginUpdate/EndUpdate
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Host(this, GenerateUniqueID(), -1);
                }

                FlipViewToLargeIconAndSmallIcon = false;

                return;
            }

            // loop through the items and give them id's so we can identify them later.
            //
            for (int i = 0; i < items.Length; i++)
            {
                ListViewItem item = items[i];

                if (checkHosting && item.listView != null)
                {
                    throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text), "item");
                }

                // create an ID..
                //
                int itemID = GenerateUniqueID();
                Debug.Assert(!listItemsTable.ContainsKey(itemID), "internal hash table inconsistent -- inserting item, but it's already in the hash table");
                listItemsTable.Add(itemID, item);

                itemCount++;
                item.Host(this, itemID, -1);

                // if there's no handle created, just ad them to our list items array.
                //
                if (!IsHandleCreated)
                {
                    Debug.Assert(listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    listItemsArray.Insert(displayIndex + i, item);
                }
            }

            // finally if the handle is created, do the actual add into the real list view
            //
            if (IsHandleCreated)
            {
                InsertItemsNative(displayIndex, items);
            }

            Invalidate();
            ArrangeIcons(alignStyle);

            // Any newly added items shoul dhave the correct location.
            // UpdateListViewItemsLocations();

            // Update sorted order
            if (!VirtualMode)
            {
                Sort();
            }

            Debug.Assert(!FlipViewToLargeIconAndSmallIcon, "if we added even 1 item then we should have been done w/ FlipViewToLargeIconAndSmallIcon");
        }

        /// <summary>
        ///  Inserts a new ListViewItem into the list view itself.
        ///  This only will be called when the Handle has been created for the list view.
        ///  This method loops through the items, sets up their state then adds them.
        /// </summary>
        private unsafe int InsertItemsNative(int index, ListViewItem[] items)
        {
            if (items is null || items.Length == 0)
            {
                return 0;
            }
            Debug.Assert(IsHandleCreated, "InsertItemsNative precondition: list-view handle must be created");

            // Much more efficient to call the native insert with max + 1, than with max.  The + 1
            // for the display index accounts for itemCount++ above.
            //
            if (index == itemCount - 1)
            {
                index++;
            }

            // Create and add the LVITEM
            int actualIndex = -1;
            IntPtr hGlobalColumns = IntPtr.Zero;
            int maxColumns = 0;
            listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = true;

            try
            {
                // Set the count of items first.
                User32.SendMessageW(this, (User32.WM)LVM.SETITEMCOUNT, (IntPtr)itemCount);

                // Now add the items.
                for (int i = 0; i < items.Length; i++)
                {
                    ListViewItem li = items[i];

                    Debug.Assert(Items.Contains(li), "Make sure ListView.Items contains this item before adding the native LVITEM. Otherwise, custom-drawing may break.");

                    var lvItem = new LVITEMW
                    {
                        mask = LVIF.TEXT | LVIF.IMAGE | LVIF.PARAM | LVIF.INDENT | LVIF.COLUMNS,
                        iItem = index + i,
                        iImage = li.ImageIndexer.ActualIndex,
                        iIndent = li.IndentCount,
                        lParam = (IntPtr)li.ID,
                        cColumns = columnHeaders != null ? Math.Min(MAXTILECOLUMNS, columnHeaders.Length) : 0,
                    };

                    if (GroupsEnabled)
                    {
                        lvItem.mask |= LVIF.GROUPID;
                        lvItem.iGroupId = GetNativeGroupId(li);

#if DEBUG
                        IntPtr result = User32.SendMessageW(this, (User32.WM)LVM.ISGROUPVIEWENABLED);
                        Debug.Assert(result != IntPtr.Zero, "Groups not enabled");
                        result = User32.SendMessageW(this, (User32.WM)LVM.HASGROUP, (IntPtr)lvItem.iGroupId);
                        Debug.Assert(result != IntPtr.Zero, "Doesn't contain group id: " + lvItem.iGroupId.ToString(CultureInfo.InvariantCulture));
#endif
                    }

                    // make sure that our columns memory is big enough.
                    // if not, then realloc it.
                    if (lvItem.cColumns > maxColumns || hGlobalColumns == IntPtr.Zero)
                    {
                        if (hGlobalColumns != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(hGlobalColumns);
                        }
                        hGlobalColumns = Marshal.AllocHGlobal(lvItem.cColumns * sizeof(int));
                        maxColumns = lvItem.cColumns;
                    }

                    // now build and copy in the column indexes.
                    lvItem.puColumns = hGlobalColumns;
                    int[] columns = new int[lvItem.cColumns];
                    for (int c = 0; c < lvItem.cColumns; c++)
                    {
                        columns[c] = c + 1;
                    }
                    Marshal.Copy(columns, 0, lvItem.puColumns, lvItem.cColumns);

                    // Inserting an item into a ListView with checkboxes causes one or more
                    // item check events to be fired for the newly added item.
                    // Therefore, we disable the item check event handler temporarily.
                    ItemCheckEventHandler oldOnItemCheck = onItemCheck;
                    onItemCheck = null;

                    int insertIndex;

                    try
                    {
                        li.UpdateStateToListView(lvItem.iItem, ref lvItem, false);

                        fixed (char* pText = li.Text)
                        {
                            lvItem.pszText = pText;

                            insertIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.INSERTITEMW, IntPtr.Zero, ref lvItem);
                        }

                        if (actualIndex == -1)
                        {
                            actualIndex = insertIndex;

                            // and update our starting index. so we're going from the same point.
                            index = actualIndex;
                        }
                    }
                    finally
                    {
                        // Restore the item check event handler.
                        onItemCheck = oldOnItemCheck;
                    }

                    if (-1 == insertIndex)
                    {
                        throw new InvalidOperationException(SR.ListViewAddItemFailed);
                    }

                    // add all sub items
                    for (int nItem = 1; nItem < li.SubItems.Count; ++nItem)
                    {
                        SetItemText(insertIndex, nItem, li.SubItems[nItem].Text, ref lvItem);
                    }

                    // PERF.
                    // Use StateSelected in order to avoid a call into the native list view.
                    if (li.StateImageSet || li.StateSelected)
                    {
                        // lvItem.state and lvItem.stateMask are set when the lvItem is updated in UpdateStateToListView call.
                        SetItemState(insertIndex, lvItem.state, lvItem.stateMask);
                    }
                }
            }
            finally
            {
                if (hGlobalColumns != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hGlobalColumns);
                }
                listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = false;
            }

            if (listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped])
            {
                // SelectedIndexChanged event was delayed
                listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
                OnSelectedIndexChanged(EventArgs.Empty);
            }

            if (FlipViewToLargeIconAndSmallIcon)
            {
                FlipViewToLargeIconAndSmallIcon = false;

                View = View.LargeIcon;
                View = View.SmallIcon;
            }

            return actualIndex;
        }

        /// <summary>
        ///  Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }

            bool isInputKey = base.IsInputKey(keyData);
            if (isInputKey)
            {
                return true;
            }

            if (listViewState[LISTVIEWSTATE_inLabelEdit])
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Return:
                    case Keys.Escape:
                        return true;
                }
            }

            return false;
        }

        private void LargeImageListRecreateHandle(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            IntPtr handle = (LargeImageList is null) ? IntPtr.Zero : LargeImageList.CreateUniqueHandle();
            var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.NORMAL, handle);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            ForceCheckBoxUpdate();
        }

        private void LargeImageListChangedHandle(object sender, EventArgs e)
        {
            if (VirtualMode || sender is null || sender != _imageListLarge || !IsHandleCreated)
            {
                return;
            }

            foreach (ListViewItem item in Items)
            {
                int imageIndex = item.ImageIndexer.ActualIndex < _imageListLarge.Images.Count
                    ? item.ImageIndexer.ActualIndex
                    : _imageListLarge.Images.Count - 1;
                SetItemImage(item.Index, imageIndex);
            }
        }

        internal void ListViewItemToolTipChanged(ListViewItem item)
        {
            if (IsHandleCreated)
            {
                // If we reset the item text then we also reset the tool tip text
                //
                SetItemText(item.Index, 0 /*subItemIndex*/, item.Text);
            }
        }

        /// <summary>
        ///  Fires the afterLabelEdit event.
        /// </summary>
        protected virtual void OnAfterLabelEdit(LabelEditEventArgs e)
        {
            onAfterLabelEdit?.Invoke(this, e);
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            if (IsHandleCreated)
            {
                SetBackgroundImage();
            }
            base.OnBackgroundImageChanged(e);
        }

        /// <summary>
        ///  We keep track of if we've hovered already so we don't fire multiple hover events
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            hoveredAlready = false;
            base.OnMouseLeave(e);
        }

        /// <summary>
        ///  In order for the MouseHover event to fire for each item in a ListView,
        ///  the item the mouse is hovering over is found. Each time a new item is hovered
        ///  over a new event is raised.
        /// </summary>
        protected override void OnMouseHover(EventArgs e)
        {
            // Hover events need to be caught for each node within the TreeView so
            // the appropriate NodeHovered event can be raised.
            ListViewItem item = null;

            if (Items.Count > 0)
            {
                // APPCOMPAT
                // V1.* users implement virtualization by communicating directly to the native ListView and by passing our virtualization implementation.
                // In that case, the native list view may have an item under the mouse even if our wrapper thinks the item count is 0.
                // And that may cause GetItemAt to throw an out of bounds exception.

                Point pos = Cursor.Position;
                pos = PointToClient(pos);
                item = GetItemAt(pos.X, pos.Y);
            }

            if (item != prevHoveredItem && item != null)
            {
                OnItemMouseHover(new ListViewItemMouseHoverEventArgs(item));
                prevHoveredItem = item;
            }

            if (!hoveredAlready)
            {
                base.OnMouseHover(e);
                hoveredAlready = true;
            }

            ResetMouseEventArgs();
        }

        /// <summary>
        ///  Fires the beforeLabelEdit event.
        /// </summary>
        protected virtual void OnBeforeLabelEdit(LabelEditEventArgs e)
        {
            onBeforeLabelEdit?.Invoke(this, e);
        }

        protected virtual void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
        {
            ((CacheVirtualItemsEventHandler)Events[EVENT_CACHEVIRTUALITEMS])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the <see cref="GroupCollapsedStateChanged"/> event.
        /// </summary>
        protected virtual void OnGroupCollapsedStateChanged(ListViewGroupEventArgs e)
        {
            ((EventHandler<ListViewGroupEventArgs>)Events[EVENT_GROUPCOLLAPSEDSTATECHANGED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the columnClick event.
        /// </summary>
        protected virtual void OnColumnClick(ColumnClickEventArgs e)
        {
            onColumnClick?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the <see cref="GroupTaskLinkClick"/> event.
        /// </summary>
        protected virtual void OnGroupTaskLinkClick(ListViewGroupEventArgs e)
        {
            ((EventHandler<ListViewGroupEventArgs>)Events[EVENT_GROUPTASKLINKCLICK])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the column header rearranged event.
        /// </summary>
        protected virtual void OnColumnReordered(ColumnReorderedEventArgs e)
        {
            ((ColumnReorderedEventHandler)Events[EVENT_COLUMNREORDERED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the column width changing event.
        /// </summary>
        protected virtual void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
        {
            ((ColumnWidthChangedEventHandler)Events[EVENT_COLUMNWIDTHCHANGED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the column width changing event.
        /// </summary>
        protected virtual void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            ((ColumnWidthChangingEventHandler)Events[EVENT_COLUMNWIDTHCHANGING])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the DrawColumnHeader event.
        /// </summary>
        protected virtual void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            ((DrawListViewColumnHeaderEventHandler)Events[EVENT_DRAWCOLUMNHEADER])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the DrawItem event.
        /// </summary>
        protected virtual void OnDrawItem(DrawListViewItemEventArgs e)
        {
            ((DrawListViewItemEventHandler)Events[EVENT_DRAWITEM])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the DrawSubItem event.
        /// </summary>
        protected virtual void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            ((DrawListViewSubItemEventHandler)Events[EVENT_DRAWSUBITEM])?.Invoke(this, e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            // When the user sets the font on the Form, the layout engine will compute the bounds for the native list view
            // and AFTER that will set the window font on the native list view.
            // That means that when the list view computed its item's position it did so w/ the previous font.
            // The solution is to send LVM_UPDATE to the native list view EVEN if the list view is not in SmallIcon or LargeIcon.
            if (!VirtualMode && IsHandleCreated && AutoArrange)
            {
                BeginUpdate();
                try
                {
                    User32.SendMessageW(this, (User32.WM)LVM.UPDATE, (IntPtr)(-1));
                }
                finally
                {
                    EndUpdate();
                }
            }

            // If font changes and we have headers, they need to be expicitly invalidated
            //
            InvalidateColumnHeaders();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // don't persist flipViewToLargeIconAndSmallIcon accross handle recreations...
            FlipViewToLargeIconAndSmallIcon = false;

            base.OnHandleCreated(e);

            int version = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)CCM.GETVERSION));
            if (version < 5)
            {
                User32.SendMessageW(this, (User32.WM)CCM.SETVERSION, (IntPtr)5);
            }

            UpdateExtendedStyles();
            RealizeProperties();
            User32.SendMessageW(this, (User32.WM)LVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(BackColor));
            User32.SendMessageW(this, (User32.WM)LVM.SETTEXTCOLOR, IntPtr.Zero, PARAM.FromColor(base.ForeColor));

            // The native list view will not invalidate the entire list view item area if the BkColor is not CLR_NONE.
            // This not noticeable if the customer paints the items w/ the same background color as the list view itself.
            // However, if the customer paints the items w/ a color different from the list view's back color
            // then when the user changes selection the native list view will not invalidate the entire list view item area.
            User32.SendMessageW(this, (User32.WM)LVM.SETTEXTBKCOLOR, IntPtr.Zero, (IntPtr)CLR.NONE);

            // LVS_NOSCROLL does not work well when the list view is in View.Details or in View.List modes.
            // we have to set this style after the list view was created and before we position the native list view items.
            //
            if (!Scrollable)
            {
                int style = unchecked((int)((long)User32.GetWindowLong(this, User32.GWL.STYLE)));
                style |= (int)LVS.NOSCROLL;
                User32.SetWindowLong(this, User32.GWL.STYLE, (IntPtr)style);
            }

            // in VirtualMode we have to tell the list view to ask for the list view item's state image index
            if (VirtualMode)
            {
                LVIS callbackMask = unchecked((LVIS)(long)User32.SendMessageW(this, (User32.WM)LVM.GETCALLBACKMASK));
                callbackMask |= LVIS.STATEIMAGEMASK;
                User32.SendMessageW(this, (User32.WM)LVM.SETCALLBACKMASK, (IntPtr)callbackMask);
            }

            if (Application.ComCtlSupportsVisualStyles)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETVIEW, (IntPtr)viewStyle);
                UpdateGroupView();

                // Add groups
                //
                if (groups != null)
                {
                    for (int index = 0; index < groups.Count; index++)
                    {
                        InsertGroupNative(index, groups[index]);
                    }
                }

                // Set tile view settings
                //
                if (viewStyle == View.Tile)
                {
                    UpdateTileView();
                }
            }

            ListViewHandleDestroyed = false;

            // Use a copy of the list items array so that we can maintain the (handle created || listItemsArray != null) invariant
            //
            ListViewItem[] listViewItemsToAdd = null;
            if (listItemsArray != null)
            {
                listViewItemsToAdd = (ListViewItem[])listItemsArray.ToArray(typeof(ListViewItem));
                listItemsArray = null;
            }

            int columnCount = columnHeaders is null ? 0 : columnHeaders.Length;
            if (columnCount > 0)
            {
                int[] indices = new int[columnHeaders.Length];
                int index = 0;
                foreach (ColumnHeader column in columnHeaders)
                {
                    indices[index] = column.DisplayIndex;
                    InsertColumnNative(index++, column);
                }
                SetDisplayIndices(indices);
            }

            // make sure that we're not in a begin/end update call.
            //
            if (itemCount > 0 && listViewItemsToAdd != null)
            {
                InsertItemsNative(0, listViewItemsToAdd);
            }

            if (VirtualMode && VirtualListSize > -1 && !DesignMode)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETITEMCOUNT, (IntPtr)VirtualListSize);
            }

            if (columnCount > 0)
            {
                UpdateColumnWidths(ColumnHeaderAutoResizeStyle.None);
            }

            ArrangeIcons(alignStyle);
            UpdateListViewItemsLocations();

            if (!VirtualMode)
            {
                Sort();
            }
            if (Application.ComCtlSupportsVisualStyles && (InsertionMark.Index > 0))
            {
                InsertionMark.UpdateListView();
            }

            //
            // When the handle is recreated, update the SavedCheckedItems.
            // It is possible some checked items were added to the list view while its handle was null.
            //
            savedCheckedItems = null;
            if (!CheckBoxes && !VirtualMode)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Checked)
                    {
                        UpdateSavedCheckedItems(Items[i], true /*addItem*/);
                    }
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.NORMAL);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.SMALL);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.GROUPHEADER);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            // don't save the list view items state when in virtual mode : it is the responsability of the
            // user to cache the list view items in virtual mode
            if (!Disposing && !VirtualMode)
            {
                int count = Items.Count;
                for (int i = 0; i < count; i++)
                {
                    Items[i].UpdateStateFromListView(i, true);
                }

                // Save away the selected and checked items
                //
                if (SelectedItems != null && !VirtualMode)
                {
                    // Create an array because the SelectedItems collection is tuned for CopyTo()
                    ListViewItem[] lviArr = new ListViewItem[SelectedItems.Count];
                    SelectedItems.CopyTo(lviArr, 0);
                    savedSelectedItems = new List<ListViewItem>(lviArr.Length);
                    for (int i = 0; i < lviArr.Length; i++)
                    {
                        savedSelectedItems.Add(lviArr[i]);
                    }
                }
                Debug.Assert(listItemsArray is null, "listItemsArray not null, even though handle created");
                ListViewItem[] items = null;
                ListViewItemCollection tempItems = Items;

                if (tempItems != null)
                {
                    items = new ListViewItem[tempItems.Count];
                    tempItems.CopyTo(items, 0);
                }

                if (items != null)
                {
                    listItemsArray = new ArrayList(items.Length);
                    listItemsArray.AddRange(items);
                }

                ListViewHandleDestroyed = true;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  Fires the itemActivate event.
        /// </summary>
        protected virtual void OnItemActivate(EventArgs e)
        {
            onItemActivate?.Invoke(this, e);
        }

        /// <summary>
        ///  This is the code that actually fires the KeyEventArgs.  Don't
        ///  forget to call base.onItemCheck() to ensure that itemCheck vents
        ///  are correctly fired for all other keys.
        /// </summary>
        protected virtual void OnItemCheck(ItemCheckEventArgs ice)
        {
            onItemCheck?.Invoke(this, ice);
        }

        protected virtual void OnItemChecked(ItemCheckedEventArgs e)
        {
            onItemChecked?.Invoke(this, e);

            if (!CheckBoxes)
            {
                return;
            }

            ListViewItem item = e.Item;
            UiaCore.ToggleState oldValue = item.Checked ? UiaCore.ToggleState.Off : UiaCore.ToggleState.On;
            UiaCore.ToggleState newValue = item.Checked ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;
            item.AccessibilityObject.RaiseAutomationPropertyChangedEvent(UiaCore.UIA.ToggleToggleStatePropertyId, oldValue, newValue);
        }

        protected virtual void OnItemDrag(ItemDragEventArgs e)
        {
            onItemDrag?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the ItemMouseHover event.
        /// </summary>
        protected virtual void OnItemMouseHover(ListViewItemMouseHoverEventArgs e)
        {
            onItemMouseHover?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the ItemSelectionChanged event.
        /// </summary>
        protected virtual void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            ((ListViewItemSelectionChangedEventHandler)Events[EVENT_ITEMSELECTIONCHANGED])?.Invoke(this, e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            // We must do this because the list view control caches the parent
            // handle and always sends notifications to the same handle.
            if (IsHandleCreated)
            {
                RecreateHandleInternal();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            // If the list view is in Details mode and it is not Scrollable, we need to reposition the column header control.
            if (View == View.Details && !Scrollable && IsHandleCreated)
            {
                PositionHeader();
            }

            base.OnResize(e);
        }

        protected virtual void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
        {
            ((RetrieveVirtualItemEventHandler)Events[EVENT_RETRIEVEVIRTUALITEM])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                RecreateHandleInternal();
            }

            if (Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Fires the search for virtual item event.
        /// </summary>
        protected virtual void OnSearchForVirtualItem(SearchForVirtualItemEventArgs e)
        {
            ((SearchForVirtualItemEventHandler)Events[EVENT_SEARCHFORVIRTUALITEM])?.Invoke(this, e);
        }

        /// <summary>
        ///  Actually goes and fires the selectedIndexChanged event.  Inheriting controls
        ///  should use this to know when the event is fired [this is preferable to
        ///  adding an event handler on yourself for this event].  They should,
        ///  however, remember to call base.onSelectedIndexChanged(e); to ensure the event is
        ///  still fired to external listeners
        /// </summary>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            ((EventHandler)Events[EVENT_SELECTEDINDEXCHANGED])?.Invoke(this, e);

            if (SelectedIndices.Count == 0)
            {
                return;
            }

            ListViewItem firstSelectedItem = Items[SelectedIndices[0]];
            if (firstSelectedItem.Focused)
            {
                firstSelectedItem.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);

            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(BackColor));
                // We should probably be OK if we don't set the TEXTBKCOLOR to CLR_NONE.
                // However, for the sake of being more robust, reset the TECTBKCOLOR to CLR_NONE when the system palette changes.
                User32.SendMessageW(this, (User32.WM)LVM.SETTEXTBKCOLOR, IntPtr.Zero, (IntPtr)CLR.NONE);
            }
        }

        protected virtual void OnVirtualItemsSelectionRangeChanged(ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            ((ListViewVirtualItemsSelectionRangeChangedEventHandler)Events[EVENT_VIRTUALITEMSSELECTIONRANGECHANGED])?.Invoke(this, e);
        }

        private unsafe void PositionHeader()
        {
            IntPtr hdrHWND = User32.GetWindow(new HandleRef(this, Handle), User32.GW.CHILD);
            if (hdrHWND != IntPtr.Zero)
            {
                var rc = new RECT();
                var wpos = new User32.WINDOWPOS();
                User32.GetClientRect(new HandleRef(this, Handle), ref rc);

                var hd = new User32.HDLAYOUT
                {
                    prc = &rc,
                    pwpos = &wpos
                };

                // get the layout information
                User32.SendMessageW(hdrHWND, (User32.WM)HDM.LAYOUT, IntPtr.Zero, ref hd);

                // position the header control
                User32.SetWindowPos(
                    new HandleRef(this, hdrHWND),
                    new HandleRef(this, wpos.hwndInsertAfter),
                    wpos.x,
                    wpos.y,
                    wpos.cx,
                    wpos.cy,
                    wpos.flags | User32.SWP.SHOWWINDOW);
            }
        }

        private void RealizeAllSubItems()
        {
            var lvItem = new LVITEMW();
            for (int i = 0; i < itemCount; i++)
            {
                int subItemCount = Items[i].SubItems.Count;
                for (int j = 0; j < subItemCount; j++)
                {
                    SetItemText(i, j, Items[i].SubItems[j].Text, ref lvItem);
                }
            }
        }

        protected void RealizeProperties()
        {
            //Realize state information
            Color c;

            c = BackColor;
            if (c != SystemColors.Window)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(c));
            }
            c = ForeColor;
            if (c != SystemColors.WindowText)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETTEXTCOLOR, IntPtr.Zero, PARAM.FromColor(c));
            }

            if (_imageListLarge != null)
            {
                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.NORMAL, _imageListLarge.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }

            if (_imageListSmall != null)
            {
                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.SMALL, _imageListSmall.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }

            if (_imageListState != null)
            {
                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, _imageListState.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }

            if (_imageListGroup != null)
            {
                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.GROUPHEADER, _imageListGroup.CreateUniqueHandle());
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }
        }

        /// <summary>
        ///  Forces the redraw of a range of listview items.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void RedrawItems(int startIndex, int endIndex, bool invalidateOnly)
        {
            if (VirtualMode)
            {
                if (startIndex < 0 || startIndex >= VirtualListSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, string.Format(SR.InvalidArgument, nameof(startIndex), startIndex));
                }
                if (endIndex < 0 || endIndex >= VirtualListSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(endIndex), endIndex, string.Format(SR.InvalidArgument, nameof(endIndex), endIndex));
                }
            }
            else
            {
                if (startIndex < 0 || startIndex >= Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, string.Format(SR.InvalidArgument, nameof(startIndex), startIndex));
                }
                if (endIndex < 0 || endIndex >= Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(endIndex), endIndex, string.Format(SR.InvalidArgument, nameof(endIndex), endIndex));
                }
            }
            if (startIndex > endIndex)
            {
                throw new ArgumentException(SR.ListViewStartIndexCannotBeLargerThanEndIndex);
            }
            if (IsHandleCreated)
            {
                int retval = (int)User32.SendMessageW(this, (User32.WM)LVM.REDRAWITEMS, (IntPtr)startIndex, (IntPtr)endIndex);
                Debug.Assert(retval != 0);
                // ListView control seems to be bogus. Items affected need to be invalidated in LargeIcon and SmallIcons views.
                if (View == View.LargeIcon || View == View.SmallIcon)
                {
                    Rectangle rectInvalid = Items[startIndex].Bounds;
                    for (int index = startIndex + 1; index <= endIndex; index++)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, Items[index].Bounds);
                    }
                    if (startIndex > 0)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, Items[startIndex - 1].Bounds);
                    }
                    else
                    {
                        rectInvalid.Width += rectInvalid.X;
                        rectInvalid.Height += rectInvalid.Y;
                        rectInvalid.X = rectInvalid.Y = 0;
                    }
                    if (endIndex < Items.Count - 1)
                    {
                        rectInvalid = Rectangle.Union(rectInvalid, Items[endIndex + 1].Bounds);
                    }
                    else
                    {
                        rectInvalid.Height += ClientRectangle.Bottom - rectInvalid.Bottom;
                        rectInvalid.Width += ClientRectangle.Right - rectInvalid.Right;
                    }
                    if (View == View.LargeIcon)
                    {
                        rectInvalid.Inflate(1, Font.Height + 1);
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
        internal void RemoveGroupFromListView(ListViewGroup group)
        {
            EnsureDefaultGroup();

            foreach (ListViewItem item in group.Items)
            {
                if (item.ListView == this)
                {
                    item.UpdateStateToListView(item.Index);
                }
            }

            RemoveGroupNative(group);

            UpdateGroupView();
        }

        /// <summary>
        /// Does the job of telling win32 listView to remove this group
        /// </summary>
        /// <remarks>
        /// It is the job of whoever deletes this group to also turn off grouping if this was the last
        /// group deleted
        /// </remarks>
        private void RemoveGroupNative(ListViewGroup group)
        {
            Debug.Assert(IsHandleCreated, "RemoveGroupNative precondition: list-view handle must be created");
            User32.SendMessageW(this, (User32.WM)LVM.REMOVEGROUP, (IntPtr)group.ID);
        }

        private void Scroll(int fromLVItem, int toLVItem)
        {
            int scrollY = GetItemPosition(toLVItem).Y - GetItemPosition(fromLVItem).Y;
            User32.SendMessageW(this, (User32.WM)LVM.SCROLL, IntPtr.Zero, (IntPtr)scrollY);
        }

        private unsafe void SetBackgroundImage()
        {
            // needed for OleInitialize
            Application.OleRequired();

            var lvbkImage = new LVBKIMAGEW();

            // first, is there an existing temporary file to delete, remember its name
            // so that we can delete it if the list control doesn't...
            string fileNameToDelete = backgroundImageFileName;

            if (BackgroundImage != null)
            {
                // save the image to a temporary file name
                string tempDirName = System.IO.Path.GetTempPath();
                Text.StringBuilder sb = new Text.StringBuilder(1024);
                UnsafeNativeMethods.GetTempFileName(tempDirName, GenerateRandomName(), 0, sb);

                backgroundImageFileName = sb.ToString();

                BackgroundImage.Save(backgroundImageFileName, System.Drawing.Imaging.ImageFormat.Bmp);

                lvbkImage.cchImageMax = (uint)(backgroundImageFileName.Length + 1);
                lvbkImage.ulFlags = LVBKIF.SOURCE_URL;
                if (BackgroundImageTiled)
                {
                    lvbkImage.ulFlags |= LVBKIF.STYLE_TILE;
                }
                else
                {
                    lvbkImage.ulFlags |= LVBKIF.STYLE_NORMAL;
                }
            }
            else
            {
                lvbkImage.ulFlags = LVBKIF.SOURCE_NONE;
                backgroundImageFileName = string.Empty;
            }

            fixed (char* pBackgroundImageFileName = backgroundImageFileName)
            {
                lvbkImage.pszImage = pBackgroundImageFileName;
                User32.SendMessageW(this, (User32.WM)LVM.SETBKIMAGEW, IntPtr.Zero, ref lvbkImage);
            }

            if (string.IsNullOrEmpty(fileNameToDelete))
            {
                return;
            }

            // we need to cause a paint message on the win32 list view. This way the win 32 list view gives up
            // its reference to the previous image file it was holding on to.
            //

            // 8 strings should be good enough for us
            if (bkImgFileNames is null)
            {
                bkImgFileNames = new string[BKIMGARRAYSIZE];
                bkImgFileNamesCount = -1;
            }

            if (bkImgFileNamesCount == BKIMGARRAYSIZE - 1)
            {
                // it should be fine to delete the file name that was added first.
                // if it's not fine, then increase BKIMGARRAYSIZE
                DeleteFileName(bkImgFileNames[0]);
                bkImgFileNames[0] = bkImgFileNames[1];
                bkImgFileNames[1] = bkImgFileNames[2];
                bkImgFileNames[2] = bkImgFileNames[3];
                bkImgFileNames[3] = bkImgFileNames[4];
                bkImgFileNames[4] = bkImgFileNames[5];
                bkImgFileNames[5] = bkImgFileNames[6];
                bkImgFileNames[6] = bkImgFileNames[7];
                bkImgFileNames[7] = null;

                bkImgFileNamesCount--;
            }

            bkImgFileNamesCount++;
            bkImgFileNames[bkImgFileNamesCount] = fileNameToDelete;

            // now force the paint
            Refresh();
        }

        internal unsafe void SetColumnInfo(LVCF mask, ColumnHeader ch)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            Debug.Assert((mask & ~(LVCF.FMT | LVCF.TEXT | LVCF.IMAGE)) == 0, "Unsupported mask in setColumnInfo");
            LVCOLUMNW lvColumn = new LVCOLUMNW
            {
                mask = mask
            };

            if ((mask & LVCF.IMAGE) != 0 || (mask & LVCF.FMT) != 0)
            {
                // When we set the ImageIndex we also have to alter the column format.
                // This means that we have to include the TextAlign into the column format.

                lvColumn.mask |= LVCF.FMT;

                if (ch.ActualImageIndex_Internal > -1)
                {
                    // you would think that setting iImage would be enough.
                    // actually we also have to set the format to include LVCFMT_IMAGE
                    lvColumn.iImage = ch.ActualImageIndex_Internal;
                    lvColumn.fmt |= LVCFMT.IMAGE;
                }

                lvColumn.fmt |= (LVCFMT)ch.TextAlign;
            }

            IntPtr result;
            fixed (char* columnHeaderText = ch.Text)
            {
                if ((mask & LVCF.TEXT) != 0)
                {
                    lvColumn.pszText = columnHeaderText;
                }

                result = User32.SendMessageW(this, (User32.WM)LVM.SETCOLUMNW, (IntPtr)ch.Index, ref lvColumn);
            }

            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException(SR.ListViewColumnInfoSet);
            }

            // When running on AMD64 the list view does not invalidate the column header.
            // So we do it ourselves.
            InvalidateColumnHeaders();
        }

        /// <summary>
        ///  Setting width is a special case 'cuz LVM_SETCOLUMNWIDTH accepts more values
        ///  for width than LVM_SETCOLUMN does.
        /// </summary>
        internal void SetColumnWidth(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            if ((columnIndex < 0) ||
                (columnIndex >= 0 && columnHeaders is null) ||
                (columnIndex >= columnHeaders.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, string.Format(SR.InvalidArgument, nameof(columnIndex), columnIndex));
            }

            //valid values are 0x0 to 0x2
            if (!ClientUtils.IsEnumValid(headerAutoResize, (int)headerAutoResize, (int)ColumnHeaderAutoResizeStyle.None, (int)ColumnHeaderAutoResizeStyle.ColumnContent))
            {
                throw new InvalidEnumArgumentException(nameof(headerAutoResize), (int)headerAutoResize, typeof(ColumnHeaderAutoResizeStyle));
            }

            int width = 0;
            int compensate = 0;

            if (headerAutoResize == ColumnHeaderAutoResizeStyle.None)
            {
                width = columnHeaders[columnIndex].WidthInternal;

                // If the width maps to a LVCSW_ const, then native control will autoresize.
                // We may need to compensate for that.
                if (width == (int)LVSCW.AUTOSIZE_USEHEADER)
                {
                    headerAutoResize = ColumnHeaderAutoResizeStyle.HeaderSize;
                }
                else if (width == (int)LVSCW.AUTOSIZE)
                {
                    headerAutoResize = ColumnHeaderAutoResizeStyle.ColumnContent;
                }
            }

            if (headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize)
            {
                compensate = CompensateColumnHeaderResize(columnIndex, columnResizeCancelled: false);
                width = (int)LVSCW.AUTOSIZE_USEHEADER;
            }
            else if (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent)
            {
                compensate = CompensateColumnHeaderResize(columnIndex, columnResizeCancelled: false);
                width = (int)LVSCW.AUTOSIZE;
            }

            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETCOLUMNWIDTH, (IntPtr)columnIndex, PARAM.FromLowHigh(width, 0));
            }

            if (IsHandleCreated &&
               (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent ||
                headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize))
            {
                if (compensate != 0)
                {
                    int newWidth = columnHeaders[columnIndex].Width + compensate;
                    User32.SendMessageW(this, (User32.WM)LVM.SETCOLUMNWIDTH, (IntPtr)columnIndex, PARAM.FromLowHigh(newWidth, 0));
                }
            }
        }

        private void SetColumnWidth(int index, int width)
        {
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)LVM.SETCOLUMNWIDTH, (IntPtr)index, PARAM.FromLowHigh(width, 0));
            }
        }

        // set the display indices of the listview columns
        private unsafe void SetDisplayIndices(int[] indices)
        {
            int[] orderedColumns = new int[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                Columns[i].DisplayIndexInternal = indices[i];
                orderedColumns[indices[i]] = i;
            }

            if (IsHandleCreated && !Disposing)
            {
                fixed (int* pOrderedColumns = orderedColumns)
                {
                    User32.SendMessageW(this, (User32.WM)LVM.SETCOLUMNORDERARRAY, (IntPtr)orderedColumns.Length, (IntPtr)pOrderedColumns);
                }
            }
        }

        /// <summary>
        ///  This is a new internal method added which is used by ListView Item to set
        ///  the check state of the item in the savedCheckedItems collection
        ///  if the ListView Checkboxes is OFF.
        /// </summary>
        internal void UpdateSavedCheckedItems(ListViewItem item, bool addItem)
        {
            if (addItem && savedCheckedItems is null)
            {
                savedCheckedItems = new List<ListViewItem>();
            }

            if (addItem)
            {
                savedCheckedItems.Add(item);
            }
            else if (savedCheckedItems != null)
            {
                Debug.Assert(savedCheckedItems.Contains(item), "somehow we lost track of one item");
                savedCheckedItems.Remove(item);
            }
        }

        /// <summary>
        ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </summary>
        internal void SetToolTip(ToolTip toolTip, string toolTipCaption)
        {
            this.toolTipCaption = toolTipCaption;

            // native ListView expects tooltip HWND as a wParam and ignores lParam
            IntPtr oldHandle = User32.SendMessageW(this, (User32.WM)LVM.SETTOOLTIPS, toolTip.Handle, IntPtr.Zero);
            GC.KeepAlive(toolTip);
            User32.DestroyWindow(oldHandle);
        }

        internal void SetItemImage(int itemIndex, int imageIndex)
        {
            if (itemIndex < 0 || (VirtualMode && itemIndex >= VirtualListSize) || (!VirtualMode && itemIndex >= itemCount))
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
            }

            if (!IsHandleCreated)
            {
                return;
            }

            var lvItem = new LVITEMW
            {
                mask = LVIF.IMAGE,
                iItem = itemIndex,
                iImage = imageIndex
            };
            User32.SendMessageW(this, (User32.WM)LVM.SETITEMW, IntPtr.Zero, ref lvItem);
        }

        internal void SetItemIndentCount(int index, int indentCount)
        {
            if (index < 0 || (VirtualMode && index >= VirtualListSize) || (!VirtualMode && index >= itemCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            if (!IsHandleCreated)
            {
                return;
            }

            var lvItem = new LVITEMW
            {
                mask = LVIF.INDENT,
                iItem = index,
                iIndent = indentCount
            };
            User32.SendMessageW(this, (User32.WM)LVM.SETITEMW, IntPtr.Zero, ref lvItem);
        }

        internal void SetItemPosition(int index, int x, int y)
        {
            if (VirtualMode)
            {
                return;
            }

            if (index < 0 || index >= itemCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            Debug.Assert(IsHandleCreated, "How did we add items without a handle?");

            var pt = new Point(x, y);
            User32.SendMessageW(this, (User32.WM)LVM.SETITEMPOSITION32, (IntPtr)index, ref pt);
        }

        internal void SetItemState(int index, LVIS state, LVIS mask)
        {
            if (index < -1 || (VirtualMode && index >= VirtualListSize) || (!VirtualMode && index >= itemCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            if (!IsHandleCreated)
            {
                return;
            }

            var lvItem = new LVITEMW
            {
                mask = LVIF.STATE,
                state = state,
                stateMask = mask
            };
            User32.SendMessageW(this, (User32.WM)LVM.SETITEMSTATE, (IntPtr)index, ref lvItem);
        }

        internal void SetItemText(int itemIndex, int subItemIndex, string text)
        {
            var lvItem = new LVITEMW();
            SetItemText(itemIndex, subItemIndex, text, ref lvItem);
        }

        /// <summary>
        ///  For perf, allow a LVITEM to be passed in so we can reuse in tight loops.
        /// </summary>
        private unsafe void SetItemText(int itemIndex, int subItemIndex, string text, ref LVITEMW lvItem)
        {
            Debug.Assert(IsHandleCreated, "SetItemText with no handle");

            if (View == View.List && subItemIndex == 0)
            {
                int colWidth = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.GETCOLUMNWIDTH));

                using Graphics g = CreateGraphicsInternal();

                int textWidth = Size.Ceiling(g.MeasureString(text, Font)).Width;
                if (textWidth > colWidth)
                {
                    SetColumnWidth(0, textWidth);
                }
            }

            lvItem.mask = LVIF.TEXT;
            lvItem.iItem = itemIndex;
            lvItem.iSubItem = subItemIndex;

            fixed (char* pText = text)
            {
                lvItem.pszText = pText;

                User32.SendMessageW(this, (User32.WM)LVM.SETITEMTEXTW, (IntPtr)itemIndex, ref lvItem);
            }
        }

        //
        // ComCtl32 list view uses a selection mark to keep track of selection state - iMark.
        // ComCtl32 list view updates iMark only when the user hovers over the item.
        // This means that if we programatically set the selection item, then the list view will not update
        // its selection mark.
        // So we explicitly set the selection mark.
        //
        internal void SetSelectionMark(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= Items.Count)
            {
                return;
            }

            User32.SendMessageW(this, (User32.WM)LVM.SETSELECTIONMARK, IntPtr.Zero, (IntPtr)itemIndex);
        }

        private void SmallImageListRecreateHandle(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            IntPtr handle = (SmallImageList is null) ? IntPtr.Zero : SmallImageList.CreateUniqueHandle();
            var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.SMALL, handle);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }

            ForceCheckBoxUpdate();
        }

        /// <summary>
        ///  Updated the sorted order
        /// </summary>
        public void Sort()
        {
            if (VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewSortNotAllowedInVirtualListView);
            }

            ApplyUpdateCachedItems();
            if (IsHandleCreated && listItemSorter != null)
            {
                NativeMethods.ListViewCompareCallback callback = new NativeMethods.ListViewCompareCallback(CompareFunc);
                IntPtr callbackPointer = Marshal.GetFunctionPointerForDelegate(callback);
                User32.SendMessageW(this, (User32.WM)LVM.SORTITEMS, IntPtr.Zero, callbackPointer);
            }
        }

        private void StateImageListRecreateHandle(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            IntPtr handle = (StateImageList is null) ? IntPtr.Zero : StateImageList.CreateUniqueHandle();
            var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, handle);
            if (previousHandle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(previousHandle);
                Debug.Assert(result.IsTrue());
            }
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();

            if (listItemsArray != null)
            {
                s += ", Items.Count: " + listItemsArray.Count.ToString(CultureInfo.CurrentCulture);
                if (listItemsArray.Count > 0)
                {
                    string z = listItemsArray[0].ToString();
                    string txt = (z.Length > 40) ? z.Substring(0, 40) : z;
                    s += ", Items[0]: " + txt;
                }
            }
            else if (Items != null)
            {
                s += ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
                if (Items.Count > 0 && !VirtualMode)
                {
                    string z = (Items[0] is null) ? "null" : Items[0].ToString();
                    string txt = (z.Length > 40) ? z.Substring(0, 40) : z;
                    s += ", Items[0]: " + txt;
                }
            }
            return s;
        }

        internal void UpdateListViewItemsLocations()
        {
            if (!VirtualMode && IsHandleCreated && AutoArrange && (View == View.LargeIcon || View == View.SmallIcon))
            {
                // this only has an affect for large icon and small icon views.
                try
                {
                    BeginUpdate();
                    User32.SendMessageW(this, (User32.WM)LVM.UPDATE, (IntPtr)(-1));
                }
                finally
                {
                    EndUpdate();
                }
            }
        }

        private void UpdateColumnWidths(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            if (columnHeaders != null)
            {
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    SetColumnWidth(i, headerAutoResize);
                }
            }
        }

        protected void UpdateExtendedStyles()
        {
            if (IsHandleCreated)
            {
                LVS_EX exStyle = 0;
                LVS_EX exMask = LVS_EX.ONECLICKACTIVATE | LVS_EX.TWOCLICKACTIVATE |
                                LVS_EX.TRACKSELECT | LVS_EX.UNDERLINEHOT |
                                LVS_EX.ONECLICKACTIVATE | LVS_EX.HEADERDRAGDROP |
                                LVS_EX.CHECKBOXES | LVS_EX.FULLROWSELECT |
                                LVS_EX.GRIDLINES | LVS_EX.INFOTIP | LVS_EX.DOUBLEBUFFER;

                switch (activation)
                {
                    case ItemActivation.OneClick:
                        exStyle |= LVS_EX.ONECLICKACTIVATE;
                        break;
                    case ItemActivation.TwoClick:
                        exStyle |= LVS_EX.TWOCLICKACTIVATE;
                        break;
                }

                if (AllowColumnReorder)
                {
                    exStyle |= LVS_EX.HEADERDRAGDROP;
                }

                if (CheckBoxes)
                {
                    exStyle |= LVS_EX.CHECKBOXES;
                }

                if (DoubleBuffered)
                {
                    exStyle |= LVS_EX.DOUBLEBUFFER;
                }

                if (FullRowSelect)
                {
                    exStyle |= LVS_EX.FULLROWSELECT;
                }

                if (GridLines)
                {
                    exStyle |= LVS_EX.GRIDLINES;
                }

                if (HoverSelection)
                {
                    exStyle |= LVS_EX.TRACKSELECT;
                }

                if (HotTracking)
                {
                    exStyle |= LVS_EX.UNDERLINEHOT;
                }

                if (ShowItemToolTips)
                {
                    exStyle |= LVS_EX.INFOTIP;
                }

                User32.SendMessageW(this, (User32.WM)LVM.SETEXTENDEDLISTVIEWSTYLE, (IntPtr)exMask, (IntPtr)exStyle);
                Invalidate();
            }
        }

        internal void UpdateGroupNative(ListViewGroup group)
        {
            Debug.Assert(IsHandleCreated, "UpdateGroupNative precondition: list-view handle must be created");

            IntPtr result = SendGroupMessage(group, LVM.SETGROUPINFO, (IntPtr)group.ID, (LVGF)0);
            Debug.Assert(result != (IntPtr)(-1));
        }

        private unsafe IntPtr SendGroupMessage(ListViewGroup group, LVM msg, IntPtr lParam, LVGF additionalMask)
        {
            string header = group.Header;
            string footer = group.Footer;
            string subtitle = group.Subtitle;
            string task = group.TaskLink;
            var lvgroup = new LVGROUPW
            {
                cbSize = (uint)sizeof(LVGROUPW),
                mask = LVGF.HEADER | LVGF.FOOTER | LVGF.ALIGN | LVGF.STATE | LVGF.SUBTITLE | LVGF.TASK | LVGF.TITLEIMAGE | additionalMask,
                cchHeader = header.Length,
                iTitleImage = -1,
                iGroupId = group.ID
            };

            if (group.CollapsedState != ListViewGroupCollapsedState.Default)
            {
                lvgroup.state |= LVGS.COLLAPSIBLE;
                if (group.CollapsedState == ListViewGroupCollapsedState.Collapsed)
                {
                    lvgroup.state |= LVGS.COLLAPSED;
                }
            }

            switch (group.HeaderAlignment)
            {
                case HorizontalAlignment.Left:
                    lvgroup.uAlign = LVGA.HEADER_LEFT;
                    break;
                case HorizontalAlignment.Right:
                    lvgroup.uAlign = LVGA.HEADER_RIGHT;
                    break;
                case HorizontalAlignment.Center:
                    lvgroup.uAlign = LVGA.HEADER_CENTER;
                    break;
            }

            if (group.TitleImageIndex != ImageList.Indexer.DefaultIndex || group.TitleImageKey != ImageList.Indexer.DefaultKey)
            {
                lvgroup.iTitleImage = group.ImageIndexer.ActualIndex;
            }

            fixed (char* pSubtitle = subtitle)
            fixed (char* pTask = task)
            fixed (char* pHeader = header)
            fixed (char* pFooter = footer)
            {
                lvgroup.cchFooter = footer.Length;
                lvgroup.pszFooter = pFooter;
                switch (group.FooterAlignment)
                {
                    case HorizontalAlignment.Left:
                        lvgroup.uAlign |= LVGA.FOOTER_LEFT;
                        break;
                    case HorizontalAlignment.Right:
                        lvgroup.uAlign |= LVGA.FOOTER_RIGHT;
                        break;
                    case HorizontalAlignment.Center:
                        lvgroup.uAlign |= LVGA.FOOTER_CENTER;
                        break;
                }

                lvgroup.cchSubtitle = (uint)subtitle.Length;
                lvgroup.pszSubtitle = pSubtitle;
                lvgroup.cchTask = (uint)task.Length;
                lvgroup.pszTask = pTask;
                lvgroup.pszHeader = pHeader;
                return User32.SendMessageW(this, (User32.WM)msg, lParam, ref lvgroup);
            }
        }

        // ListViewGroupCollection::Clear needs to remove the items from the Default group
        //
        internal void UpdateGroupView()
        {
            if (IsHandleCreated && Application.ComCtlSupportsVisualStyles && !VirtualMode)
            {
                int retval = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.ENABLEGROUPVIEW, PARAM.FromBool(GroupsEnabled)));
                Debug.Assert(retval != -1, "Error enabling group view");
            }
        }

        // updates the win32 list view w/ our tile info - columns + tile size
        private unsafe void UpdateTileView()
        {
            Debug.Assert(Application.ComCtlSupportsVisualStyles, "this function works only when ComCtl 6.0 and higher is loaded");
            Debug.Assert(viewStyle == View.Tile, "this function should be called only in Tile view");

            var tileViewInfo = new LVTILEVIEWINFO
            {
                cbSize = (uint)sizeof(LVTILEVIEWINFO),

                dwMask = LVTVIM.COLUMNS | LVTVIM.TILESIZE,
                dwFlags = LVTVIF.FIXEDSIZE,
                cLines = columnHeaders != null ? columnHeaders.Length : 0,
                sizeTile = TileSize,
            };

            IntPtr retval = User32.SendMessageW(this, (User32.WM)LVM.SETTILEVIEWINFO, IntPtr.Zero, ref tileViewInfo);
            Debug.Assert(retval != IntPtr.Zero, "LVM_SETTILEVIEWINFO failed");
        }

        private void WmNmClick(ref Message m)
        {
            // If we're checked, hittest to see if we're
            // on the check mark

            if (!CheckBoxes)
            {
                return;
            }

            var lvhi = new LVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };

            int displayIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.SUBITEMHITTEST, IntPtr.Zero, ref lvhi);
            if (displayIndex == -1 || lvhi.iSubItem != 0 || (lvhi.flags & LVHT.ONITEMSTATEICON) == 0)
            {
                return;
            }

            ListViewItem clickedItem = Items[displayIndex];
            if (!clickedItem.Selected)
            {
                return;
            }

            bool check = !clickedItem.Checked;
            if (VirtualMode)
            {
                return;
            }

            foreach (ListViewItem item in SelectedItems)
            {
                if (item != clickedItem)
                {
                    item.Checked = check;
                }
            }
        }

        private void WmNmDblClick(ref Message m)
        {
            // If we're checked, hittest to see if we're
            // on the item

            if (!CheckBoxes)
            {
                return;
            }

            var lvhi = new LVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };

            int displayIndex = (int)User32.SendMessageW(this, (User32.WM)LVM.HITTEST, IntPtr.Zero, ref lvhi);
            if (displayIndex != -1 && (lvhi.flags & LVHT.ONITEM) != 0)
            {
                ListViewItem clickedItem = Items[displayIndex];
                clickedItem.Checked = !clickedItem.Checked;
            }
        }

        private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
        {
            //Always Reset the MouseupFired....
            listViewState[LISTVIEWSTATE_mouseUpFired] = false;
            listViewState[LISTVIEWSTATE_expectingMouseUp] = true;

            //This is required to FORCE Validation before Windows ListView pushes its own message loop...
            Focus();

            // Windows ListView pushes its own Windows ListView in WM_xBUTTONDOWN, so fire the
            // event before calling defWndProc or else it won't get fired until the button
            // comes back up.
            int x = PARAM.SignedLOWORD(m.LParam);
            int y = PARAM.SignedHIWORD(m.LParam);
            OnMouseDown(new MouseEventArgs(button, clicks, x, y, 0));

            //If Validation is cancelled dont fire any events through the Windows ListView's message loop...
            if (!ValidationCancelled)
            {
                if (CheckBoxes)
                {
                    ListViewHitTestInfo lvhti = HitTest(x, y);
                    if (_imageListState != null && _imageListState.Images.Count < 2)
                    {
                        // When the user clicks on the check box and the listView's state image list
                        // does not have 2 images, comctl will give us an AttemptToDivideByZero exception.
                        // So don't send the message to DefWndProc in this situation.
                        if (lvhti.Location != ListViewHitTestLocations.StateImage)
                        {
                            DefWndProc(ref m);
                        }
                    }
                    else
                    {
                        // When a user clicks on the state image, focus the item.
                        if (lvhti.Item != null && lvhti.Location == ListViewHitTestLocations.StateImage)
                        {
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

            Point screenPoint = PointToScreen(new Point(x, y));
            AccessibleObject accessibilityObject = AccessibilityObject.HitTest(screenPoint.X, screenPoint.Y);
            accessibilityObject?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }

        private unsafe bool WmNotify(ref Message m)
        {
            User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;

            // column header custom draw message handling
            if (nmhdr->code == (int)NM.CUSTOMDRAW && OwnerDraw)
            {
                try
                {
                    NMCUSTOMDRAW* nmcd = (NMCUSTOMDRAW*)m.LParam;
                    // Find out which stage we're drawing
                    switch (nmcd->dwDrawStage)
                    {
                        case CDDS.PREPAINT:
                            {
                                m.Result = (IntPtr)CDRF.NOTIFYITEMDRAW;
                                return true; // we are done - don't do default handling
                            }
                        case CDDS.ITEMPREPAINT:
                            {
                                using Graphics g = nmcd->hdc.CreateGraphics();
                                Color foreColor = Gdi32.GetTextColor(nmcd->hdc);
                                Color backColor = Gdi32.GetBkColor(nmcd->hdc);
                                Font font = GetListHeaderFont();
                                var e = new DrawListViewColumnHeaderEventArgs(
                                    g,
                                    nmcd->rc,
                                    (int)nmcd->dwItemSpec,
                                    columnHeaders[(int)nmcd->dwItemSpec],
                                    (ListViewItemStates)nmcd->uItemState,
                                    foreColor,
                                    backColor,
                                    font);
                                OnDrawColumnHeader(e);
                                if (e.DrawDefault)
                                {
                                    m.Result = (IntPtr)CDRF.DODEFAULT;
                                    return false;
                                }
                                else
                                {
                                    m.Result = (IntPtr)CDRF.SKIPDEFAULT;
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
                    m.Result = (IntPtr)CDRF.DODEFAULT;
                }
            }

            if (nmhdr->code == (int)NM.RELEASEDCAPTURE && listViewState[LISTVIEWSTATE_columnClicked])
            {
                listViewState[LISTVIEWSTATE_columnClicked] = false;
                OnColumnClick(new ColumnClickEventArgs(columnIndex));
            }

            if (nmhdr->code == (int)HDN.BEGINTRACKW)
            {
                listViewState[LISTVIEWSTATE_headerControlTracking] = true;

                // Reset our tracking information for the new BEGINTRACK cycle.
                listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
                newWidthForColumnWidthChangingCancelled = -1;
                listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;

                NMHEADERW* nmheader = (NMHEADERW*)m.LParam;
                if (columnHeaders != null && columnHeaders.Length > nmheader->iItem)
                {
                    columnHeaderClicked = columnHeaders[nmheader->iItem];
                    columnHeaderClickedWidth = columnHeaderClicked.Width;
                }
                else
                {
                    columnHeaderClickedWidth = -1;
                    columnHeaderClicked = null;
                }
            }

            if (nmhdr->code == (int)HDN.ITEMCHANGINGW)
            {
                NMHEADERW* nmheader = (NMHEADERW*)m.LParam;

                if (columnHeaders != null && nmheader->iItem < columnHeaders.Length &&
                    (listViewState[LISTVIEWSTATE_headerControlTracking] || listViewState[LISTVIEWSTATE_headerDividerDblClick]))
                {
                    int newColumnWidth = ((nmheader->pitem->mask & HDI.WIDTH) != 0) ? nmheader->pitem->cxy : -1;
                    ColumnWidthChangingEventArgs colWidthChanging = new ColumnWidthChangingEventArgs(nmheader->iItem, newColumnWidth);
                    OnColumnWidthChanging(colWidthChanging);
                    m.Result = (IntPtr)(colWidthChanging.Cancel ? 1 : 0);
                    if (colWidthChanging.Cancel)
                    {
                        nmheader->pitem->cxy = colWidthChanging.NewWidth;

                        // We are called inside HDN_DIVIDERDBLCLICK.
                        // Turn off the compensation that our processing of HDN_DIVIDERDBLCLICK would otherwise add.
                        if (listViewState[LISTVIEWSTATE_headerDividerDblClick])
                        {
                            listViewState[LISTVIEWSTATE_columnResizeCancelled] = true;
                        }

                        listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = true;
                        newWidthForColumnWidthChangingCancelled = colWidthChanging.NewWidth;

                        // skip default processing
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if ((nmhdr->code == (int)HDN.ITEMCHANGEDW) &&
                !listViewState[LISTVIEWSTATE_headerControlTracking])
            {
                NMHEADERW* nmheader = (NMHEADERW*)m.LParam;
                if (columnHeaders != null && nmheader->iItem < columnHeaders.Length)
                {
                    int w = columnHeaders[nmheader->iItem].Width;

                    if (columnHeaderClicked is null ||
                        (columnHeaderClicked == columnHeaders[nmheader->iItem] &&
                         columnHeaderClickedWidth != -1 &&
                         columnHeaderClickedWidth != w))
                    {
                        //
                        // If the user double clicked on the column header and we still need to compensate for the column resize
                        // then don't fire ColumnWidthChanged because at this point the column header does not have the final width.
                        //
                        if (listViewState[LISTVIEWSTATE_headerDividerDblClick])
                        {
                            if (CompensateColumnHeaderResize(m, listViewState[LISTVIEWSTATE_columnResizeCancelled]) == 0)
                            {
                                OnColumnWidthChanged(new ColumnWidthChangedEventArgs(nmheader->iItem));
                            }
                        }
                        else
                        {
                            OnColumnWidthChanged(new ColumnWidthChangedEventArgs(nmheader->iItem));
                        }
                    }
                }

                columnHeaderClicked = null;
                columnHeaderClickedWidth = -1;

                ISite site = Site;

                if (site != null)
                {
                    IComponentChangeService cs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                    if (cs != null)
                    {
                        try
                        {
                            cs.OnComponentChanging(this, null);
                        }
                        catch (CheckoutException coEx)
                        {
                            if (coEx == CheckoutException.Canceled)
                            {
                                return false;
                            }
                            throw;
                        }
                    }
                }
            }

            if (nmhdr->code == (int)HDN.ENDTRACKW)
            {
                Debug.Assert(listViewState[LISTVIEWSTATE_headerControlTracking], "HDN_ENDTRACK and HDN_BEGINTRACK are out of sync...");
                listViewState[LISTVIEWSTATE_headerControlTracking] = false;
                if (listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging])
                {
                    m.Result = (IntPtr)1;
                    if (newWidthForColumnWidthChangingCancelled != -1)
                    {
                        NMHEADERW* nmheader = (NMHEADERW*)m.LParam;
                        if (columnHeaders != null && columnHeaders.Length > nmheader->iItem)
                        {
                            columnHeaders[nmheader->iItem].Width = newWidthForColumnWidthChangingCancelled;
                        }
                    }

                    listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
                    newWidthForColumnWidthChangingCancelled = -1;

                    // skip default processing
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (nmhdr->code == (int)HDN.ENDDRAG)
            {
                NMHEADERW* header = (NMHEADERW*)m.LParam;
                if (header->pitem != null)
                {
                    if ((header->pitem->mask & HDI.ORDER) == HDI.ORDER)
                    {
                        int from = Columns[header->iItem].DisplayIndex;
                        int to = header->pitem->iOrder;

                        // check this
                        if (from == to)
                        {
                            return false;
                        }

                        // sometimes ComCtl gives us bogus values for HDIItem.iOrder.
                        if (to < 0)
                        {
                            return false;
                        }
                        ColumnReorderedEventArgs chrevent = new ColumnReorderedEventArgs(
                            from,
                            to,
                            Columns[header->iItem]);
                        OnColumnReordered(chrevent);
                        if (chrevent.Cancel)
                        {
                            m.Result = new IntPtr(1);
                            return true;
                        }
                        else
                        {
                            // set the display indices. This is not an expensive operation because
                            // we only set an integer in the column header class
                            int lowDI = Math.Min(from, to);
                            int hiDI = Math.Max(from, to);
                            bool hdrMovedForward = to > from;
                            ColumnHeader movedHdr = null;
                            int[] indices = new int[Columns.Count];
                            for (int i = 0; i < Columns.Count; i++)
                            {
                                ColumnHeader hdr = Columns[i];
                                if (hdr.DisplayIndex == from)
                                {
                                    movedHdr = hdr;
                                }
                                else if (hdr.DisplayIndex >= lowDI && hdr.DisplayIndex <= hiDI)
                                {
                                    hdr.DisplayIndexInternal -= hdrMovedForward ? 1 : -1;
                                }
                                indices[i] = hdr.DisplayIndexInternal;
                            }

                            movedHdr.DisplayIndexInternal = to;
                            indices[movedHdr.Index] = movedHdr.DisplayIndexInternal;
                            SetDisplayIndices(indices);
#if DEBUG
                            CheckDisplayIndices();
#endif
                        }
                    }
                }
            }

            if (nmhdr->code == (int)HDN.DIVIDERDBLCLICKW)
            {
                // We need to keep track that the user double clicked the column header divider
                // so we know that the column header width is changing.
                listViewState[LISTVIEWSTATE_headerDividerDblClick] = true;

                // Reset ColumnResizeCancelled.
                // It will be set if the user cancels the ColumnWidthChanging event.
                listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;

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
                    listViewState[LISTVIEWSTATE_headerDividerDblClick] = false;
                    columnResizeCancelled = listViewState[LISTVIEWSTATE_columnResizeCancelled];
                    listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;
                }

                columnHeaderClicked = null;
                columnHeaderClickedWidth = -1;

                if (columnResizeCancelled)
                {
                    // If the column resize was cancelled then apply the NewWidth supplied by the user.
                    if (newWidthForColumnWidthChangingCancelled != -1)
                    {
                        NMHEADERW* nmheader = (NMHEADERW*)m.LParam;
                        if (columnHeaders != null && columnHeaders.Length > nmheader->iItem)
                        {
                            columnHeaders[nmheader->iItem].Width = newWidthForColumnWidthChangingCancelled;
                        }
                    }

                    // Tell ComCtl that the HDN_DIVIDERDBLCLICK was cancelled.
                    m.Result = (IntPtr)1;
                }
                else
                {
                    // Compensate for the column resize.
                    int compensateForColumnResize = CompensateColumnHeaderResize(m, columnResizeCancelled);
                    if (compensateForColumnResize != 0)
                    {
#if DEBUG
                        NMHEADERW* header = (NMHEADERW*)m.LParam;
                        Debug.Assert(header->iItem == 0, "we only need to compensate for the first column resize");
                        Debug.Assert(columnHeaders.Length > 0, "there should be a column that we need to compensate for");
#endif

                        ColumnHeader col = columnHeaders[0];
                        col.Width += compensateForColumnResize;
                    }
                }

                // We called DefWndProc so we don't need default handling.
                return true;
            }

            return false; // still need default handling
        }

        private Font GetListHeaderFont()
        {
            IntPtr hwndHdr = User32.SendMessageW(this, (User32.WM)LVM.GETHEADER);
            IntPtr hFont = User32.SendMessageW(hwndHdr, User32.WM.GETFONT);
            return Font.FromHfont(hFont);
        }

        private int GetIndexOfClickedItem()
        {
            var lvhi = SetupHitTestInfo();
            return unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.HITTEST, IntPtr.Zero, ref lvhi));
        }

        private LVHITTESTINFO SetupHitTestInfo()
        {
            var lvhi = new LVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };

            return lvhi;
        }

        private int UpdateGroupCollapse(Interop.User32.WM clickType)
        {
            // see if the mouse event occurred on a group
            var lvhi = SetupHitTestInfo();
            int groupID = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)LVM.HITTEST, (IntPtr)(-1), ref lvhi));
            if (groupID == -1)
            {
                return groupID;
            }

            // check if group header was double clicked
            bool groupHeaderDblClked = lvhi.flags == LVHT.EX_GROUP_HEADER && clickType == User32.WM.LBUTTONDBLCLK;
            // check if chevron was clicked
            bool chevronClked = (lvhi.flags & LVHT.EX_GROUP_COLLAPSE) == LVHT.EX_GROUP_COLLAPSE && clickType == User32.WM.LBUTTONUP;
            if (!groupHeaderDblClked && !chevronClked)
            {
                return groupID;
            }
            for (int i = 0; i < groups.Count; i++)
            {
                ListViewGroup targetGroup = groups[i];
                if (targetGroup.ID == groupID)
                {
                    if (targetGroup.CollapsedState == ListViewGroupCollapsedState.Default)
                    {
                        return groupID;
                    }

                    targetGroup.CollapsedState = targetGroup.CollapsedState == ListViewGroupCollapsedState.Expanded
                                                ? ListViewGroupCollapsedState.Collapsed
                                                : ListViewGroupCollapsedState.Expanded;
                    OnGroupCollapsedStateChanged(new ListViewGroupEventArgs(i));
                    break;
                }
            }

            return groupID;
        }

        internal void RecreateHandleInternal()
        {
            //
            // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native listView destroys
            // the state imageList.
            // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
            if (IsHandleCreated && StateImageList != null)
            {
                var previousHandle = User32.SendMessageW(this, (User32.WM)LVM.SETIMAGELIST, (IntPtr)LVSIL.STATE, IntPtr.Zero);
                if (previousHandle != IntPtr.Zero)
                {
                    var result = ComCtl32.ImageList.Destroy(previousHandle);
                    Debug.Assert(result.IsTrue());
                }
            }

            RecreateHandle();
        }

        private unsafe void WmReflectNotify(ref Message m)
        {
            User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;

            switch (nmhdr->code)
            {
                case (int)NM.CUSTOMDRAW:
                    CustomDraw(ref m);
                    break;

                case (int)LVN.BEGINLABELEDITW:
                    {
                        NMLVDISPINFO* dispInfo = (NMLVDISPINFO*)m.LParam;
                        LabelEditEventArgs e = new LabelEditEventArgs(dispInfo->item.iItem);
                        OnBeforeLabelEdit(e);
                        m.Result = (IntPtr)(e.CancelEdit ? 1 : 0);
                        listViewState[LISTVIEWSTATE_inLabelEdit] = !e.CancelEdit;
                        break;
                    }

                case (int)LVN.COLUMNCLICK:
                    {
                        NMLISTVIEW* nmlv = (NMLISTVIEW*)m.LParam;
                        listViewState[LISTVIEWSTATE_columnClicked] = true;
                        columnIndex = nmlv->iSubItem;
                        break;
                    }

                case (int)LVN.LINKCLICK:
                    {
                        NMLVLINK* pLink = (NMLVLINK*)m.LParam;
                        int groupID = pLink->iSubItem;
                        for (int i = 0; i < groups.Count; i++)
                        {
                            if (groups[i].ID == groupID)
                            {
                                OnGroupTaskLinkClick(new ListViewGroupEventArgs(i));
                                break;
                            }
                        }

                        break;
                    }

                case (int)LVN.ENDLABELEDITW:
                    {
                        listViewState[LISTVIEWSTATE_inLabelEdit] = false;
                        NMLVDISPINFO* dispInfo = (NMLVDISPINFO*)m.LParam;
                        var text = new string(dispInfo->item.pszText);
                        LabelEditEventArgs e = new LabelEditEventArgs(dispInfo->item.iItem, text);
                        OnAfterLabelEdit(e);
                        m.Result = (IntPtr)(e.CancelEdit ? 0 : 1);

                        // from msdn:
                        //   "If the user cancels editing, the pszText member of the LVITEM structure is NULL"
                        if (!e.CancelEdit && dispInfo->item.pszText != null)
                        {
                            Items[dispInfo->item.iItem].Text = text;
                        }

                        break;
                    }

                case (int)LVN.ITEMACTIVATE:
                    OnItemActivate(EventArgs.Empty);
                    break;

                case (int)LVN.BEGINDRAG:
                    {
                        // The items collection was modified while dragging that means that
                        // we can't reliably give the user the item on which the dragging
                        // started so don't tell the user about this operation.
                        if (!ItemCollectionChangedInMouseDown)
                        {
                            NMLISTVIEW* nmlv = (NMLISTVIEW*)m.LParam;
                            ListViewItem item = Items[nmlv->iItem];
                            OnItemDrag(new ItemDragEventArgs(MouseButtons.Left, item));
                        }

                        break;
                    }

                case (int)LVN.BEGINRDRAG:
                    {
                        // The items collection was modified while dragging. That means that
                        // we can't reliably give the user the item on which the dragging
                        // started so don't tell the user about this operation.
                        if (!ItemCollectionChangedInMouseDown)
                        {
                            NMLISTVIEW* nmlv = (NMLISTVIEW*)m.LParam;
                            ListViewItem item = Items[nmlv->iItem];
                            OnItemDrag(new ItemDragEventArgs(MouseButtons.Right, item));
                        }

                        break;
                    }

                case (int)LVN.ITEMCHANGING:
                    {
                        NMLISTVIEW* nmlv = (NMLISTVIEW*)m.LParam;
                        if ((nmlv->uChanged & LVIF.STATE) != 0)
                        {
                            // Because the state image mask is 1-based, a value of 1 means unchecked,
                            // anything else means checked.  We convert this to the more standard 0 or 1
                            CheckState oldState = (CheckState)(((int)(nmlv->uOldState & LVIS.STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                            CheckState newState = (CheckState)(((int)(nmlv->uNewState & LVIS.STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                            if (oldState != newState)
                            {
                                ItemCheckEventArgs e = new ItemCheckEventArgs(nmlv->iItem, newState, oldState);
                                OnItemCheck(e);
                                m.Result = (IntPtr)(((int)e.NewValue == 0 ? 0 : 1) == (int)oldState ? 1 : 0);
                            }
                        }
                        break;
                    }

                case (int)LVN.ITEMCHANGED:
                    {
                        NMLISTVIEW* nmlv = (NMLISTVIEW*)m.LParam;
                        // Check for state changes to the selected state...
                        if ((nmlv->uChanged & LVIF.STATE) != 0)
                        {
                            // Because the state image mask is 1-based, a value of 1 means unchecked,
                            // anything else means checked.  We convert this to the more standard 0 or 1
                            CheckState oldValue = (CheckState)(((int)(nmlv->uOldState & LVIS.STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                            CheckState newValue = (CheckState)(((int)(nmlv->uNewState & LVIS.STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                            if (newValue != oldValue)
                            {
                                ItemCheckedEventArgs e = new ItemCheckedEventArgs(Items[nmlv->iItem]);
                                OnItemChecked(e);

                                AccessibilityNotifyClients(AccessibleEvents.StateChange, nmlv->iItem);
                                AccessibilityNotifyClients(AccessibleEvents.NameChange, nmlv->iItem);
                            }

                            LVIS oldState = nmlv->uOldState & LVIS.SELECTED;
                            LVIS newState = nmlv->uNewState & LVIS.SELECTED;
                            // Windows common control always fires
                            // this event twice, once with newState, oldState, and again with
                            // oldState, newState.
                            // Changing this affects the behaviour as the control never
                            // fires the event on a Deselct of an Items from multiple selections.
                            // So leave it as it is...
                            if (newState != oldState)
                            {
                                if (VirtualMode && nmlv->iItem == -1)
                                {
                                    if (VirtualListSize > 0)
                                    {
                                        ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new ListViewVirtualItemsSelectionRangeChangedEventArgs(0, VirtualListSize - 1, newState != 0);
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

                                    if (Items.Count > 0)
                                    {
                                        ListViewItemSelectionChangedEventArgs lvisce = new ListViewItemSelectionChangedEventArgs(Items[nmlv->iItem],
                                                                                                                                 nmlv->iItem,
                                                                                                                                 newState != 0);
                                        OnItemSelectionChanged(lvisce);
                                    }
                                }
                                // Delay SelectedIndexChanged event because the last item isn't present yet.
                                if (Items.Count == 0 || Items[Items.Count - 1] != null)
                                {
                                    listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
                                    OnSelectedIndexChanged(EventArgs.Empty);
                                }
                                else
                                {
                                    listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = true;
                                }
                            }
                        }
                        break;
                    }

                case (int)NM.CLICK:
                    WmNmClick(ref m);
                    // FALL THROUGH //
                    goto case (int)NM.RCLICK;

                case (int)NM.RCLICK:
                    int displayIndex = GetIndexOfClickedItem();

                    MouseButtons button = nmhdr->code == (int)NM.CLICK ? MouseButtons.Left : MouseButtons.Right;
                    Point pos = Cursor.Position;
                    pos = PointToClient(pos);

                    if (!ValidationCancelled && displayIndex != -1)
                    {
                        OnClick(EventArgs.Empty);
                        OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                    }
                    if (!listViewState[LISTVIEWSTATE_mouseUpFired])
                    {
                        OnMouseUp(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                        listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    }
                    break;

                case (int)NM.DBLCLK:
                    WmNmDblClick(ref m);
                    // FALL THROUGH //
                    goto case (int)NM.RDBLCLK;

                case (int)NM.RDBLCLK:
                    int index = GetIndexOfClickedItem();
                    if (index != -1)
                    {
                        //just maintain state and fire double click.. in final mouseUp...
                        listViewState[LISTVIEWSTATE_doubleclickFired] = true;
                    }

                    // Fire mouse up in the Wndproc
                    listViewState[LISTVIEWSTATE_mouseUpFired] = false;

                    // Make sure we get the mouse up if it happens outside the control.
                    Capture = true;
                    break;

                case (int)LVN.KEYDOWN:
                    if (GroupsEnabled)
                    {
                        NMLVKEYDOWN* lvkd = (NMLVKEYDOWN*)m.LParam;
                        if ((lvkd->wVKey == (short)Keys.Down || lvkd->wVKey == (short)Keys.Up) && SelectedItems.Count > 0)
                        {
                            AccessibleObject accessibleObject = SelectedItems[0].AccessibilityObject;
                            if (lvkd->wVKey == (short)Keys.Down
                                && accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling) is null)
                            {
                                ListViewGroupAccessibleObject groupAccObj = (ListViewGroupAccessibleObject)accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);
                                ListViewGroupAccessibleObject nextGroupAccObj = (ListViewGroupAccessibleObject)groupAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
                                nextGroupAccObj?.SetFocus();
                            }

                            if (lvkd->wVKey == (short)Keys.Up
                            && accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) is null)
                            {
                                ListViewGroupAccessibleObject groupAccObj = (ListViewGroupAccessibleObject)accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);
                                groupAccObj?.SetFocus();
                            }
                        }
                    }

                    if (CheckBoxes)
                    {
                        NMLVKEYDOWN* lvkd = (NMLVKEYDOWN*)m.LParam;
                        if (lvkd->wVKey == (short)Keys.Space)
                        {
                            ListViewItem focusedItem = FocusedItem;
                            if (focusedItem != null)
                            {
                                bool check = !focusedItem.Checked;
                                if (!VirtualMode)
                                {
                                    foreach (ListViewItem item in SelectedItems)
                                    {
                                        if (item != focusedItem)
                                        {
                                            item.Checked = check;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case (int)LVN.ODCACHEHINT:
                    // tell the user to prepare the cache:
                    NMLVCACHEHINT* cacheHint = (NMLVCACHEHINT*)m.LParam;
                    OnCacheVirtualItems(new CacheVirtualItemsEventArgs(cacheHint->iFrom, cacheHint->iTo));
                    break;

                default:
                    if (nmhdr->code == (int)LVN.GETDISPINFOW)
                    {
                        // we use the LVN_GETDISPINFO message only in virtual mode
                        if (VirtualMode && m.LParam != IntPtr.Zero)
                        {
                            NMLVDISPINFO* dispInfo = (NMLVDISPINFO*)m.LParam;

                            RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(dispInfo->item.iItem);
                            OnRetrieveVirtualItem(rVI);
                            ListViewItem lvItem = rVI.Item;
                            if (lvItem is null)
                            {
                                throw new InvalidOperationException(SR.ListViewVirtualItemRequired);
                            }

                            lvItem.SetItemIndex(this, dispInfo->item.iItem);
                            if ((dispInfo->item.mask & LVIF.TEXT) != 0)
                            {
                                ReadOnlySpan<char> text = default;
                                if (dispInfo->item.iSubItem == 0)
                                {
                                    text = lvItem.Text;                                         // we want the item
                                }
                                else
                                {
                                    if (lvItem.SubItems.Count <= dispInfo->item.iSubItem)
                                    {
                                        throw new InvalidOperationException(SR.ListViewVirtualModeCantAccessSubItem);
                                    }
                                    else
                                    {
                                        text = lvItem.SubItems[dispInfo->item.iSubItem].Text;   // we want the sub item
                                    }
                                }

                                dispInfo->item.UpdateText(text);
                            }

                            if ((dispInfo->item.mask & LVIF.IMAGE) != 0 && lvItem.ImageIndex != -1)
                            {
                                dispInfo->item.iImage = lvItem.ImageIndex;
                            }

                            if ((dispInfo->item.mask & LVIF.INDENT) != 0)
                            {
                                dispInfo->item.iIndent = lvItem.IndentCount;
                            }

                            if ((dispInfo->item.stateMask & LVIS.STATEIMAGEMASK) != 0)
                            {
                                dispInfo->item.state |= lvItem.RawStateImageIndex;
                            }
                        }
                    }
                    else if (nmhdr->code == (int)LVN.ODSTATECHANGED)
                    {
                        if (VirtualMode && m.LParam != IntPtr.Zero)
                        {
                            NMLVODSTATECHANGE* odStateChange = (NMLVODSTATECHANGE*)m.LParam;
                            bool selectedChanged = (odStateChange->uNewState & LVIS.SELECTED) != (odStateChange->uOldState & LVIS.SELECTED);
                            if (selectedChanged)
                            {
                                // we have to substract 1 from iTo
                                int iTo = odStateChange->iTo;
                                ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new ListViewVirtualItemsSelectionRangeChangedEventArgs(odStateChange->iFrom, iTo, (odStateChange->uNewState & LVIS.SELECTED) != 0);
                                OnVirtualItemsSelectionRangeChanged(lvvisrce);
                            }
                        }
                    }
                    else if (nmhdr->code == (int)LVN.GETINFOTIPW)
                    {
                        if (ShowItemToolTips && m.LParam != IntPtr.Zero)
                        {
                            NMLVGETINFOTIPW* infoTip = (NMLVGETINFOTIPW*)m.LParam;
                            ListViewItem lvi = Items[infoTip->item];
                            if (lvi != null && !string.IsNullOrEmpty(lvi.ToolTipText))
                            {
                                // Setting the max width has the added benefit of enabling multiline tool tips
                                User32.SendMessageW(nmhdr->hwndFrom, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);

                                // UNICODE. Use char.
                                // we need to copy the null terminator character ourselves
                                char[] charBuf = (lvi.ToolTipText + "\0").ToCharArray();
                                Marshal.Copy(charBuf, 0, infoTip->lpszText, Math.Min(charBuf.Length, infoTip->cchTextMax));
                            }
                        }
                    }
                    else if (nmhdr->code == (int)LVN.ODFINDITEMW)
                    {
                        if (VirtualMode)
                        {
                            NMLVFINDITEMW* nmlvif = (NMLVFINDITEMW*)m.LParam;

                            if ((nmlvif->lvfi.flags & LVFI.PARAM) != 0)
                            {
                                m.Result = (IntPtr)(-1);
                                return;
                            }

                            bool isTextSearch = ((nmlvif->lvfi.flags & LVFI.STRING) != 0) ||
                                                ((nmlvif->lvfi.flags & LVFI.PARTIAL) != 0);

                            bool isPrefixSearch = (nmlvif->lvfi.flags & LVFI.PARTIAL) != 0;

                            string text = string.Empty;
                            if (isTextSearch && nmlvif->lvfi.psz != null)
                            {
                                text = new string(nmlvif->lvfi.psz);
                            }

                            Point startingPoint = Point.Empty;
                            if ((nmlvif->lvfi.flags & LVFI.NEARESTXY) != 0)
                            {
                                startingPoint = nmlvif->lvfi.pt;
                            }

                            SearchDirectionHint dir = SearchDirectionHint.Down;
                            if ((nmlvif->lvfi.flags & LVFI.NEARESTXY) != 0)
                            {
                                // We can do this because SearchDirectionHint is set to the VK_*
                                dir = (SearchDirectionHint)nmlvif->lvfi.vkDirection;
                            }

                            int startIndex = nmlvif->iStart;
                            if (startIndex >= VirtualListSize)
                            {
                                // we want to search starting from the last item. Wrap around the first item.
                                startIndex = 0;
                            }

                            var sviEvent = new SearchForVirtualItemEventArgs(
                                isTextSearch,
                                isPrefixSearch,
                                false, /* includeSubItemsInSearch */
                                text,
                                startingPoint,
                                dir,
                                nmlvif->iStart);

                            OnSearchForVirtualItem(sviEvent);
                            if (sviEvent.Index != -1)
                            {
                                m.Result = (IntPtr)sviEvent.Index;
                            }
                            else
                            {
                                m.Result = (IntPtr)(-1);
                            }
                        }
                    }
                    break;
            }
        }

        private void WmPrint(ref Message m)
        {
            base.WndProc(ref m);
            if (((User32.PRF)m.LParam & User32.PRF.NONCLIENT) != 0 && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
            {
                using Graphics g = Graphics.FromHdc(m.WParam);
                Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
                using var pen = VisualStyleInformation.TextControlBorder.GetCachedPenScope();
                g.DrawRectangle(pen, rect);
                rect.Inflate(-1, -1);
                g.DrawRectangle(SystemPens.Window, rect);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.REFLECT_NOTIFY:
                    WmReflectNotify(ref m);
                    break;
                case User32.WM.LBUTTONDBLCLK:

                    // Ensure that the itemCollectionChangedInMouseDown is not set
                    // before processing the mousedown event.
                    ItemCollectionChangedInMouseDown = false;
                    Capture = true;
                    WmMouseDown(ref m, MouseButtons.Left, 2);
                    UpdateGroupCollapse(User32.WM.LBUTTONDBLCLK);
                    break;

                case User32.WM.LBUTTONDOWN:

                    // Ensure that the itemCollectionChangedInMouseDown is not set
                    // before processing the mousedown event.
                    ItemCollectionChangedInMouseDown = false;
                    WmMouseDown(ref m, MouseButtons.Left, 1);
                    downButton = MouseButtons.Left;
                    break;

                case User32.WM.LBUTTONUP:
                case User32.WM.RBUTTONUP:
                case User32.WM.MBUTTONUP:

                    // see the mouse is on item
                    int index = UpdateGroupCollapse(User32.WM.LBUTTONUP);

                    if (!ValidationCancelled && listViewState[LISTVIEWSTATE_doubleclickFired] && index != -1)
                    {
                        listViewState[LISTVIEWSTATE_doubleclickFired] = false;
                        OnDoubleClick(EventArgs.Empty);
                        OnMouseDoubleClick(new MouseEventArgs(downButton, 2, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                    }
                    if (!listViewState[LISTVIEWSTATE_mouseUpFired])
                    {
                        OnMouseUp(new MouseEventArgs(downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                        listViewState[LISTVIEWSTATE_expectingMouseUp] = false;
                    }

                    ItemCollectionChangedInMouseDown = false;

                    listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    Capture = false;
                    break;
                case User32.WM.MBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    break;
                case User32.WM.MBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    downButton = MouseButtons.Middle;
                    break;
                case User32.WM.RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);
                    break;
                case User32.WM.RBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    downButton = MouseButtons.Right;
                    break;
                case User32.WM.MOUSEMOVE:
                    if (listViewState[LISTVIEWSTATE_expectingMouseUp] && !listViewState[LISTVIEWSTATE_mouseUpFired] && MouseButtons == MouseButtons.None)
                    {
                        OnMouseUp(new MouseEventArgs(downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                        listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                    }
                    Capture = false;
                    base.WndProc(ref m);
                    break;
                case User32.WM.MOUSEHOVER:
                    if (HoverSelection)
                    {
                        base.WndProc(ref m);
                    }
                    else
                    {
                        OnMouseHover(EventArgs.Empty);
                    }

                    break;
                case User32.WM.NOTIFY:
                    if (WmNotify(ref m))
                    {
                        break; // we are done - skip default handling
                    }
                    else
                    {
                        goto default;  //default handling needed
                    }
                case User32.WM.SETFOCUS:
                    base.WndProc(ref m);

                    if (!RecreatingHandle && !ListViewHandleDestroyed)
                    {
                        // This means that we get a WM_SETFOCUS on the hWnd that was destroyed.
                        // Don't do anything because the information on the previous hWnd is most likely
                        // out of sync w/ the information in our ListView wrapper.

                        // We should set focus to the first item,
                        // if none of the items are focused already.
                        if (FocusedItem is null && Items.Count > 0)
                        {
                            Items[0].Focused = true;
                        }
                    }
                    break;
                case User32.WM.MOUSELEAVE:
                    // if the mouse leaves and then re-enters the ListView
                    // ItemHovered events should be raised.
                    prevHoveredItem = null;
                    base.WndProc(ref m);
                    break;

                case User32.WM.PAINT:
                    base.WndProc(ref m);

                    // win32 ListView
                    BeginInvoke(new MethodInvoker(CleanPreviousBackgroundImageFiles));
                    break;
                case User32.WM.PRINT:
                    WmPrint(ref m);
                    break;
                case User32.WM.TIMER:
                    if (unchecked((int)(long)m.WParam) != LVTOOLTIPTRACKING || !Application.ComCtlSupportsVisualStyles)
                    {
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
        internal class IconComparer : IComparer
        {
            private SortOrder sortOrder;

            public IconComparer(SortOrder currentSortOrder)
            {
                sortOrder = currentSortOrder;
            }

            public SortOrder SortOrder
            {
                set
                {
                    sortOrder = value;
                }
            }

            public int Compare(object obj1, object obj2)
            {
                //subhag
                ListViewItem currentItem = (ListViewItem)obj1;
                ListViewItem nextItem = (ListViewItem)obj2;
                if (sortOrder == SortOrder.Ascending)
                {
                    return string.Compare(currentItem.Text, nextItem.Text, false, CultureInfo.CurrentCulture);
                }
                else
                {
                    return string.Compare(nextItem.Text, currentItem.Text, false, CultureInfo.CurrentCulture);
                }
            }
        }
        //end subhag

        [ListBindable(false)]
        public class CheckedIndexCollection : IList
        {
            private readonly ListView owner;

            /* C#r: protected */
            public CheckedIndexCollection(ListView owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of currently selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    if (!owner.CheckBoxes)
                    {
                        return 0;
                    }

                    // Count the number of checked items
                    //
                    int count = 0;
                    foreach (ListViewItem item in owner.Items)
                    {
                        if (item != null && item.Checked)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            private int[] IndicesArray
            {
                get
                {
                    int[] indices = new int[Count];
                    int index = 0;
                    for (int i = 0; i < owner.Items.Count && index < indices.Length; ++i)
                    {
                        if (owner.Items[i].Checked)
                        {
                            indices[index++] = i;
                        }
                    }
                    return indices;
                }
            }

            /// <summary>
            ///  Selected item in the list.
            /// </summary>
            public int this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    // Loop through the main collection until we find the right index.
                    //
                    int cnt = owner.Items.Count;
                    int nChecked = 0;
                    for (int i = 0; i < cnt; i++)
                    {
                        ListViewItem item = owner.Items[i];

                        if (item.Checked)
                        {
                            if (nChecked == index)
                            {
                                return i;
                            }
                            nChecked++;
                        }
                    }

                    // Should never get to this point.
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool Contains(int checkedIndex)
            {
                if (owner.Items[checkedIndex].Checked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool IList.Contains(object checkedIndex)
            {
                if (checkedIndex is int)
                {
                    return Contains((int)checkedIndex);
                }
                else
                {
                    return false;
                }
            }

            public int IndexOf(int checkedIndex)
            {
                int[] indices = IndicesArray;
                for (int index = 0; index < indices.Length; ++index)
                {
                    if (indices[index] == checkedIndex)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object checkedIndex)
            {
                if (checkedIndex is int)
                {
                    return IndexOf((int)checkedIndex);
                }
                else
                {
                    return -1;
                }
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException();
            }

            void IList.Clear()
            {
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    System.Array.Copy(IndicesArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                int[] indices = IndicesArray;
                if (indices != null)
                {
                    return indices.GetEnumerator();
                }
                else
                {
                    return Array.Empty<int>().GetEnumerator();
                }
            }
        }

        [ListBindable(false)]
        public class CheckedListViewItemCollection : IList
        {
            private readonly ListView owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /* C#r: protected */
            public CheckedListViewItemCollection(ListView owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of currently selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return owner.CheckedIndices.Count;
                }
            }

            private ListViewItem[] ItemArray
            {
                get
                {
                    ListViewItem[] items = new ListViewItem[Count];
                    int index = 0;
                    for (int i = 0; i < owner.Items.Count && index < items.Length; ++i)
                    {
                        if (owner.Items[i].Checked)
                        {
                            items[index++] = owner.Items[i];
                        }
                    }
                    return items;
                }
            }

            /// <summary>
            ///  Selected item in the list.
            /// </summary>
            public ListViewItem this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    int itemIndex = owner.CheckedIndices[index];
                    return owner.Items[itemIndex];
                }
            }

            object IList.this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewItem this[string key]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool Contains(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item != null && item.ListView == owner && item.Checked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool IList.Contains(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return Contains((ListViewItem)item);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                return IsValidIndex(IndexOfKey(key));
            }

            public int IndexOf(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                for (int index = 0; index < items.Length; ++index)
                {
                    if (items[index] == item)
                    {
                        return index;
                    }
                }
                return -1;
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return (index >= 0) && (index < Count);
            }

            int IList.IndexOf(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return IndexOf((ListViewItem)item);
                }
                else
                {
                    return -1;
                }
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException();
            }

            void IList.Clear()
            {
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0)
                {
                    System.Array.Copy(ItemArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                if (items != null)
                {
                    return items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<ListViewItem>().GetEnumerator();
                }
            }
        }

        [ListBindable(false)]
        public class SelectedIndexCollection : IList
        {
            private readonly ListView owner;

            /* C#r: protected */
            public SelectedIndexCollection(ListView owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of currently selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    if (owner.IsHandleCreated)
                    {
                        return unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT));
                    }
                    else
                    {
                        if (owner.savedSelectedItems != null)
                        {
                            return owner.savedSelectedItems.Count;
                        }
                        return 0;
                    }
                }
            }

            private int[] IndicesArray
            {
                get
                {
                    int count = Count;
                    int[] indices = new int[count];

                    if (owner.IsHandleCreated)
                    {
                        int displayIndex = -1;
                        for (int i = 0; i < count; i++)
                        {
                            int fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)displayIndex, (IntPtr)LVNI.SELECTED));
                            if (fidx > -1)
                            {
                                indices[i] = fidx;
                                displayIndex = fidx;
                            }
                            else
                            {
                                throw new InvalidOperationException(SR.SelectedNotEqualActual);
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(owner.savedSelectedItems != null || count == 0, "if the count of selectedItems is greater than 0 then the selectedItems should have been saved by now");
                        for (int i = 0; i < count; i++)
                        {
                            indices[i] = owner.savedSelectedItems[i].Index;
                        }
                    }

                    return indices;
                }
            }

            /// <summary>
            ///  Selected item in the list.
            /// </summary>
            public int this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (owner.IsHandleCreated)
                    {
                        // Count through the selected items in the ListView, until
                        // we reach the 'index'th selected item.
                        //
                        int fidx = -1;
                        for (int count = 0; count <= index; count++)
                        {
                            fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)fidx, (IntPtr)LVNI.SELECTED));
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return fidx;
                    }
                    else
                    {
                        Debug.Assert(owner.savedSelectedItems != null, "Null selected items collection");
                        return owner.savedSelectedItems[index].Index;
                    }
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool Contains(int selectedIndex)
            {
                if (selectedIndex < 0 || selectedIndex >= owner.Items.Count)
                {
                    return false;
                }

                return owner.Items[selectedIndex].Selected;
            }

            bool IList.Contains(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return Contains((int)selectedIndex);
                }
                else
                {
                    return false;
                }
            }

            public int IndexOf(int selectedIndex)
            {
                int[] indices = IndicesArray;
                for (int index = 0; index < indices.Length; ++index)
                {
                    if (indices[index] == selectedIndex)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return IndexOf((int)selectedIndex);
                }
                else
                {
                    return -1;
                }
            }

            int IList.Add(object value)
            {
                if (value is int)
                {
                    return Add((int)value);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
                }
            }

            void IList.Clear()
            {
                Clear();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                if (value is int)
                {
                    Remove((int)value);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
                }
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public int Add(int itemIndex)
            {
                if (owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }
                    if (owner.IsHandleCreated)
                    {
                        owner.SetItemState(itemIndex, LVIS.SELECTED, LVIS.SELECTED);
                        return Count;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }
                    owner.Items[itemIndex].Selected = true;
                    return Count;
                }
            }

            public void Clear()
            {
                if (!owner.VirtualMode)
                {
                    owner.savedSelectedItems = null;
                }
                if (owner.IsHandleCreated)
                {
                    owner.SetItemState(-1, 0, LVIS.SELECTED);
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    System.Array.Copy(IndicesArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                int[] indices = IndicesArray;
                if (indices != null)
                {
                    return indices.GetEnumerator();
                }
                else
                {
                    return Array.Empty<int>().GetEnumerator();
                }
            }

            public void Remove(int itemIndex)
            {
                if (owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }
                    if (owner.IsHandleCreated)
                    {
                        owner.SetItemState(itemIndex, 0, LVIS.SELECTED);
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }
                    owner.Items[itemIndex].Selected = false;
                }
            }
        }

        [ListBindable(false)]
        public class SelectedListViewItemCollection : IList
        {
            private readonly ListView owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /* C#r: protected */
            public SelectedListViewItemCollection(ListView owner)
            {
                this.owner = owner;
            }

            private ListViewItem[] SelectedItemArray
            {
                get
                {
                    if (owner.IsHandleCreated)
                    {
                        int cnt = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT));

                        ListViewItem[] lvitems = new ListViewItem[cnt];

                        int displayIndex = -1;

                        for (int i = 0; i < cnt; i++)
                        {
                            int fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)displayIndex, (IntPtr)LVNI.SELECTED));
                            if (fidx > -1)
                            {
                                lvitems[i] = owner.Items[fidx];
                                displayIndex = fidx;
                            }
                            else
                            {
                                throw new InvalidOperationException(SR.SelectedNotEqualActual);
                            }
                        }

                        return lvitems;
                    }
                    else
                    {
                        if (owner.savedSelectedItems != null)
                        {
                            ListViewItem[] cloned = new ListViewItem[owner.savedSelectedItems.Count];
                            for (int i = 0; i < owner.savedSelectedItems.Count; i++)
                            {
                                cloned[i] = owner.savedSelectedItems[i];
                            }
                            return cloned;
                        }
                        else
                        {
                            return Array.Empty<ListViewItem>();
                        }
                    }
                }
            }

            /// <summary>
            ///  Number of currently selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (owner.IsHandleCreated)
                    {
                        return unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT));
                    }
                    else
                    {
                        if (owner.savedSelectedItems != null)
                        {
                            return owner.savedSelectedItems.Count;
                        }
                        return 0;
                    }
                }
            }

            /// <summary>
            ///  Selected item in the list.
            /// </summary>
            public ListViewItem this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (owner.IsHandleCreated)
                    {
                        // Count through the selected items in the ListView, until
                        // we reach the 'index'th selected item.
                        //
                        int fidx = -1;
                        for (int count = 0; count <= index; count++)
                        {
                            fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)fidx, (IntPtr)LVNI.SELECTED));
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return owner.Items[fidx];
                    }
                    else
                    {
                        Debug.Assert(owner.savedSelectedItems != null, "Null selected items collection");
                        return owner.savedSelectedItems[index];
                    }
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewItem this[string key]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            object IList.this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    return this[index];
                }
                set
                {
                    // SelectedListViewItemCollection is read-only
                    throw new NotSupportedException();
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            int IList.Add(object value)
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return (index >= 0) && (index < Count);
            }

            void IList.Remove(object value)
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            /// <summary>
            ///  Unselects all items.
            /// </summary>
            public void Clear()
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Selected = false;
                }
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return IsValidIndex(IndexOfKey(key));
            }

            public bool Contains(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return IndexOf(item) != -1;
            }

            bool IList.Contains(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return Contains((ListViewItem)item);
                }
                else
                {
                    return false;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0)
                {
                    System.Array.Copy(SelectedItemArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                if (items != null)
                {
                    return items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<ListViewItem>().GetEnumerator();
                }
            }

            public int IndexOf(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = SelectedItemArray;
                for (int index = 0; index < items.Length; ++index)
                {
                    if (items[index] == item)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return IndexOf((ListViewItem)item);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }
        }

        [ListBindable(false)]
        public class ColumnHeaderCollection : IList
        {
            private readonly ListView owner;

            public ColumnHeaderCollection(ListView owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Given a Zero based index, returns the ColumnHeader object
            ///  for the column at that index
            /// </summary>
            public virtual ColumnHeader this[int index]
            {
                get
                {
                    if (owner.columnHeaders is null || index < 0 || index >= owner.columnHeaders.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return owner.columnHeaders[index];
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ColumnHeader this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            ///  The number of columns the ListView currently has in Details view.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    return owner.columnHeaders is null ? 0 : owner.columnHeaders.Length;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return true;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int lastAccessedIndex = -1;

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return (index >= 0) && (index < Count);
            }

            /// <summary>
            ///  Adds a column to the end of the Column list
            /// </summary>
            public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual int Add(ColumnHeader value)
            {
                int index = Count;
                owner.InsertColumn(index, value);
                return index;
            }

            public virtual ColumnHeader Add(string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            // <-- NEW ADD OVERLOADS IN WHIDBEY

            public virtual ColumnHeader Add(string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text,
                    Width = width
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageKey)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            // END - NEW ADD OVERLOADS IN WHIDBEY  -->

            public virtual void AddRange(ColumnHeader[] values)
            {
                if (values is null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                Hashtable usedIndices = new Hashtable();
                int[] indices = new int[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] is null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }

                    if (values[i].DisplayIndex == -1)
                    {
                        values[i].DisplayIndexInternal = i;
                    }

                    if (!usedIndices.ContainsKey(values[i].DisplayIndex) && values[i].DisplayIndex >= 0 && values[i].DisplayIndex < values.Length)
                    {
                        usedIndices.Add(values[i].DisplayIndex, i);
                    }

                    indices[i] = values[i].DisplayIndex;
                    Add(values[i]);
                }

                if (usedIndices.Count == values.Length)
                {
                    owner.SetDisplayIndices(indices);
                }
            }

            int IList.Add(object value)
            {
                if (value is ColumnHeader)
                {
                    return Add((ColumnHeader)value);
                }
                else
                {
                    throw new ArgumentException(SR.ColumnHeaderCollectionInvalidArgument, nameof(value));
                }
            }

            /// <summary>
            ///  Removes all columns from the list view.
            /// </summary>
            public virtual void Clear()
            {
                // Delete the columns
                if (owner.columnHeaders != null)
                {
                    if (owner.View == View.Tile)
                    {
                        // in Tile view our ListView uses the column header collection to update the Tile Information
                        for (int colIdx = owner.columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                        {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }
                        owner.columnHeaders = null;
                        if (owner.IsHandleCreated)
                        {
                            owner.RecreateHandleInternal();
                        }
                    }
                    else
                    {
                        for (int colIdx = owner.columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                        {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            if (owner.IsHandleCreated)
                            {
                                User32.SendMessageW(owner, (User32.WM)LVM.DELETECOLUMN, (IntPtr)colIdx);
                            }
                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }
                        owner.columnHeaders = null;
                    }
                }
            }

            public bool Contains(ColumnHeader value)
            {
                return IndexOf(value) != -1;
            }

            bool IList.Contains(object value)
            {
                if (value is ColumnHeader)
                {
                    return Contains((ColumnHeader)value);
                }
                return false;
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    System.Array.Copy(owner.columnHeaders, 0, dest, index, Count);
                }
            }

            public int IndexOf(ColumnHeader value)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (this[index] == value)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object value)
            {
                if (value is ColumnHeader)
                {
                    return IndexOf((ColumnHeader)value);
                }
                return -1;
            }

            public void Insert(int index, ColumnHeader value)
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }
                owner.InsertColumn(index, value);
            }

            void IList.Insert(int index, object value)
            {
                if (value is ColumnHeader)
                {
                    Insert(index, (ColumnHeader)value);
                }
            }

            public void Insert(int index, string text, int width, HorizontalAlignment textAlign)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            // <-- NEW INSERT OVERLOADS IN WHIDBEY

            public void Insert(int index, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text,
                    Width = width
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageKey)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            // END - NEW INSERT OVERLOADS IN WHIDBEY -->

            /// <summary>
            ///  removes a column from the ListView
            /// </summary>
            public virtual void RemoveAt(int index)
            {
                if (owner.columnHeaders is null || index < 0 || index >= owner.columnHeaders.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                int w = owner.columnHeaders[index].Width; // Update width before detaching from ListView

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && owner.View != View.Tile)
                {
                    int retval = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.DELETECOLUMN, (IntPtr)index));
                    if (0 == retval)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                }

                // we need to update the display indices
                int[] indices = new int[Count - 1];

                ColumnHeader removeHdr = this[index];
                for (int i = 0; i < Count; i++)
                {
                    ColumnHeader hdr = this[i];
                    if (i != index)
                    {
                        if (hdr.DisplayIndex >= removeHdr.DisplayIndex)
                        {
                            hdr.DisplayIndexInternal--;
                        }
                        indices[i > index ? i - 1 : i] = hdr.DisplayIndexInternal;
                    }
                }

                removeHdr.DisplayIndexInternal = -1;

                owner.columnHeaders[index].OwnerListview = null;
                int columnCount = owner.columnHeaders.Length;
                Debug.Assert(columnCount >= 1, "Column mismatch");
                if (columnCount == 1)
                {
                    owner.columnHeaders = null;
                }
                else
                {
                    ColumnHeader[] newHeaders = new ColumnHeader[--columnCount];
                    if (index > 0)
                    {
                        System.Array.Copy(owner.columnHeaders, 0, newHeaders, 0, index);
                    }

                    if (index < columnCount)
                    {
                        System.Array.Copy(owner.columnHeaders, index + 1, newHeaders, index, columnCount - index);
                    }

                    owner.columnHeaders = newHeaders;
                }

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && owner.View == View.Tile)
                {
                    owner.RecreateHandleInternal();
                }

                owner.SetDisplayIndices(indices);
            }

            public virtual void Remove(ColumnHeader column)
            {
                int index = IndexOf(column);
                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object value)
            {
                if (value is ColumnHeader)
                {
                    Remove((ColumnHeader)value);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.columnHeaders != null)
                {
                    return owner.columnHeaders.GetEnumerator();
                }
                else
                {
                    return Array.Empty<ColumnHeader>().GetEnumerator();
                }
            }
        }

        /// <summary>
        ///  Represents the collection of items in a ListView or ListViewGroup
        /// </summary>
        [ListBindable(false)]
        public class ListViewItemCollection : IList
        {
            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            internal interface IInnerList
            {
                int Count { get; }
                bool OwnerIsVirtualListView { get; }
                bool OwnerIsDesignMode { get; }
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

            private readonly IInnerList innerList;

            public ListViewItemCollection(ListView owner)
            {
                // Kept for APPCOMPAT reasons.
                // In Whidbey this constructor is a no-op.

                // initialize the inner list w/ a dummy list.
                innerList = new ListViewNativeItemCollection(owner);
            }

            internal ListViewItemCollection(IInnerList innerList)
            {
                Debug.Assert(innerList != null, "Can't pass in null innerList");
                this.innerList = innerList;
            }

            private IInnerList InnerList
            {
                get
                {
                    return innerList;
                }
            }

            /// <summary>
            ///  Returns the total number of items within the list view.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    return InnerList.Count;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return true;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Returns the ListViewItem at the given index.
            /// </summary>
            public virtual ListViewItem this[int index]
            {
                get
                {
                    if (index < 0 || index >= InnerList.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return InnerList[index];
                }
                set
                {
                    if (index < 0 || index >= InnerList.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    InnerList[index] = value;
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (value is ListViewItem)
                    {
                        this[index] = (ListViewItem)value;
                    }
                    else if (value != null)
                    {
                        this[index] = new ListViewItem(value.ToString(), -1);
                    }
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewItem this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(string text)
            {
                return Add(text, -1);
            }

            int IList.Add(object item)
            {
                if (item is ListViewItem)
                {
                    return IndexOf(Add((ListViewItem)item));
                }
                else if (item != null)
                {
                    return IndexOf(Add(item.ToString()));
                }
                return -1;
            }

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(string text, int imageIndex)
            {
                ListViewItem li = new ListViewItem(text, imageIndex);
                Add(li);
                return li;
            }

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(ListViewItem value)
            {
                InnerList.Add(value);
                return value;
            }

            // <-- NEW ADD OVERLOADS IN WHIDBEY

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(string text, string imageKey)
            {
                ListViewItem li = new ListViewItem(text, imageKey);
                Add(li);
                return li;
            }

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(string key, string text, string imageKey)
            {
                ListViewItem li = new ListViewItem(text, imageKey)
                {
                    Name = key
                };
                Add(li);
                return li;
            }

            /// <summary>
            ///  Add an item to the ListView.  The item will be inserted either in
            ///  the correct sorted position, or, if no sorting is set, at the end
            ///  of the list.
            /// </summary>
            public virtual ListViewItem Add(string key, string text, int imageIndex)
            {
                ListViewItem li = new ListViewItem(text, imageIndex)
                {
                    Name = key
                };
                Add(li);
                return li;
            }

            // END - NEW ADD OVERLOADS IN WHIDBEY  -->

            public void AddRange(ListViewItem[] items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                InnerList.AddRange(items);
            }

            public void AddRange(ListViewItemCollection items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                ListViewItem[] itemArray = new ListViewItem[items.Count];
                items.CopyTo(itemArray, 0);
                InnerList.AddRange(itemArray);
            }

            /// <summary>
            ///  Removes all items from the list view.
            /// </summary>
            public virtual void Clear()
            {
                InnerList.Clear();
            }

            public bool Contains(ListViewItem item)
            {
                return InnerList.Contains(item);
            }

            bool IList.Contains(object item)
            {
                if (item is ListViewItem)
                {
                    return Contains((ListViewItem)item);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            public void CopyTo(Array dest, int index)
            {
                InnerList.CopyTo(dest, index);
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an array
            ///  of all the controls that match.
            /// </summary>
            public ListViewItem[] Find(string key, bool searchAllSubItems)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
                }

                ArrayList foundItems = FindInternal(key, searchAllSubItems, this, new ArrayList());

                ListViewItem[] stronglyTypedFoundItems = new ListViewItem[foundItems.Count];
                foundItems.CopyTo(stronglyTypedFoundItems, 0);

                return stronglyTypedFoundItems;
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an arraylist
            ///  of all the controls that match.
            /// </summary>
            private ArrayList FindInternal(string key, bool searchAllSubItems, ListViewItemCollection listViewItems, ArrayList foundItems)
            {
                if ((listViewItems is null) || (foundItems is null))
                {
                    return null;  //
                }

                for (int i = 0; i < listViewItems.Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].Name, key, /* ignoreCase = */ true))
                    {
                        foundItems.Add(listViewItems[i]);
                    }
                    else
                    {
                        if (searchAllSubItems)
                        {
                            // start from 1, as we've already compared subitems[0]
                            for (int j = 1; j < listViewItems[i].SubItems.Count; j++)
                            {
                                if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].SubItems[j].Name, key, /* ignoreCase = */ true))
                                {
                                    foundItems.Add(listViewItems[i]);
                                    break;
                                }
                            }
                        }
                    }
                }

                return foundItems;
            }

            public IEnumerator GetEnumerator()
            {
                if (InnerList.OwnerIsVirtualListView && !InnerList.OwnerIsDesignMode)
                {
                    // Throw the exception only at runtime.
                    throw new InvalidOperationException(SR.ListViewCantGetEnumeratorInVirtualMode);
                }
                return InnerList.GetEnumerator();
            }

            public int IndexOf(ListViewItem item)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (this[index] == item)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object item)
            {
                if (item is ListViewItem)
                {
                    return IndexOf((ListViewItem)item);
                }
                else
                {
                    return -1;
                }
            }
            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return (index >= 0) && (index < Count);
            }

            public ListViewItem Insert(int index, ListViewItem item)
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }
                InnerList.Insert(index, item);
                return item;
            }

            public ListViewItem Insert(int index, string text)
            {
                return Insert(index, new ListViewItem(text));
            }

            public ListViewItem Insert(int index, string text, int imageIndex)
            {
                return Insert(index, new ListViewItem(text, imageIndex));
            }

            void IList.Insert(int index, object item)
            {
                if (item is ListViewItem)
                {
                    Insert(index, (ListViewItem)item);
                }
                else if (item != null)
                {
                    Insert(index, item.ToString());
                }
            }

            // <-- NEW INSERT OVERLOADS IN WHIDBEY

            public ListViewItem Insert(int index, string text, string imageKey)
            {
                return Insert(index, new ListViewItem(text, imageKey));
            }

            public virtual ListViewItem Insert(int index, string key, string text, string imageKey)
            {
                ListViewItem li = new ListViewItem(text, imageKey)
                {
                    Name = key
                };
                return Insert(index, li);
            }

            public virtual ListViewItem Insert(int index, string key, string text, int imageIndex)
            {
                ListViewItem li = new ListViewItem(text, imageIndex)
                {
                    Name = key
                };
                return Insert(index, li);
            }

            // END - NEW INSERT OVERLOADS IN WHIDBEY -->

            /// <summary>
            ///  Removes an item from the ListView
            /// </summary>
            public virtual void Remove(ListViewItem item)
            {
                InnerList.Remove(item);
            }

            /// <summary>
            ///  Removes an item from the ListView
            /// </summary>
            public virtual void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                InnerList.RemoveAt(index);
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object item)
            {
                if (item is null || !(item is ListViewItem))
                {
                    return;
                }

                Remove((ListViewItem)item);
            }
        }

        // Overrides ListViewItemCollection methods and properties to automatically update
        // the native listview.
        //
        internal class ListViewNativeItemCollection : ListViewItemCollection.IInnerList
        {
            private readonly ListView owner;

            public ListViewNativeItemCollection(ListView owner)
            {
                this.owner = owner;
            }

            public int Count
            {
                get
                {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode)
                    {
                        return owner.VirtualListSize;
                    }
                    else
                    {
                        return owner.itemCount;
                    }
                }
            }

            public bool OwnerIsVirtualListView
            {
                get
                {
                    return owner.VirtualMode;
                }
            }

            public bool OwnerIsDesignMode
            {
                get
                {
                    return owner.DesignMode;
                }
            }

            public ListViewItem this[int displayIndex]
            {
                get
                {
                    owner.ApplyUpdateCachedItems();

                    if (owner.VirtualMode)
                    {
                        // if we are showing virtual items, we need to get the item from the user
                        RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(displayIndex);
                        owner.OnRetrieveVirtualItem(rVI);
                        rVI.Item.SetItemIndex(owner, displayIndex);
                        return rVI.Item;
                    }
                    else
                    {
                        if (displayIndex < 0 || displayIndex >= owner.itemCount)
                        {
                            throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                        }

                        if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                        {
                            return (ListViewItem)owner.listItemsTable[DisplayIndexToID(displayIndex)];
                        }
                        else
                        {
                            Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                            return (ListViewItem)owner.listItemsArray[displayIndex];
                        }
                    }
                }
                set
                {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantModifyTheItemCollInAVirtualListView);
                    }

                    if (displayIndex < 0 || displayIndex >= owner.itemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                    }

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }

                    RemoveAt(displayIndex);
                    Insert(displayIndex, value);
                }
            }

            public ListViewItem Add(ListViewItem value)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }
                else
                {
                    Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                    // PERF.
                    // Get the Checked bit before adding it to the back end.
                    // This saves a call into NativeListView to retrieve the real index.
                    bool valueChecked = value.Checked;

                    owner.InsertItems(owner.itemCount, new ListViewItem[] { value }, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes && valueChecked)
                    {
                        owner.UpdateSavedCheckedItems(value, true /*addItem*/);
                    }

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }

                    return value;
                }
            }

            public void AddRange(ListViewItem[] values)
            {
                if (values is null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                IComparer comparer = owner.listItemSorter;
                owner.listItemSorter = null;

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                bool[] checkedValues = null;

                if (owner.IsHandleCreated && !owner.CheckBoxes)
                {
                    // PERF.
                    // Cache the Checked bit before adding the item to the list view.
                    // This saves a bunch of calls to native list view when we want to UpdateSavedCheckedItems.
                    //
                    checkedValues = new bool[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        checkedValues[i] = values[i].Checked;
                    }
                }

                try
                {
                    owner.BeginUpdate();
                    owner.InsertItems(owner.itemCount, values, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (checkedValues[i])
                            {
                                owner.UpdateSavedCheckedItems(values[i], true /*addItem*/);
                            }
                        }
                    }
                }
                finally
                {
                    owner.listItemSorter = comparer;
                    owner.EndUpdate();
                }

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }

#pragma warning disable SA1408 // Conditional expressions should declare precedence
                if (comparer != null || (owner.Sorting != SortOrder.None) && !owner.VirtualMode)
#pragma warning restore SA1408 // Conditional expressions should declare precedence
                {
                    owner.Sort();
                }
            }

            private int DisplayIndexToID(int displayIndex)
            {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this method does not make any sense");
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                {
                    // Obtain internal index of the item
                    var lvItem = new LVITEMW
                    {
                        mask = LVIF.PARAM,
                        iItem = displayIndex
                    };
                    User32.SendMessageW(owner, (User32.WM)LVM.GETITEMW, IntPtr.Zero, ref lvItem);
                    return (int)lvItem.lParam;
                }
                else
                {
                    return this[displayIndex].ID;
                }
            }

            public void Clear()
            {
                if (owner.itemCount > 0)
                {
                    owner.ApplyUpdateCachedItems();

                    if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                    {
                        // walk the items to see which ones are selected.
                        // we use the LVM_GETNEXTITEM message to see what the next selected item is
                        // so we can avoid checking selection for each one.
                        //
                        int count = owner.Items.Count;
                        int nextSelected = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)(-1), (IntPtr)LVNI.SELECTED);
                        for (int i = 0; i < count; i++)
                        {
                            ListViewItem item = owner.Items[i];
                            Debug.Assert(item != null, "Failed to get item at index " + i.ToString(CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                // if it's the one we're looking for, ask for the next one
                                //
                                if (i == nextSelected)
                                {
                                    item.StateSelected = true;
                                    nextSelected = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)nextSelected, (IntPtr)LVNI.SELECTED);
                                }
                                else
                                {
                                    // otherwise it's false
                                    //
                                    item.StateSelected = false;
                                }
                                item.UnHost(i, false);
                            }
                        }
                        Debug.Assert(owner.listItemsArray is null, "listItemsArray not null, even though handle created");

                        User32.SendMessageW(owner, (User32.WM)LVM.DELETEALLITEMS);

                        // There's a problem in the list view that if it's in small icon, it won't pick upo the small icon
                        // sizes until it changes from large icon, so we flip it twice here...
                        //
                        if (owner.View == View.SmallIcon)
                        {
                            if (Application.ComCtlSupportsVisualStyles)
                            {
                                owner.FlipViewToLargeIconAndSmallIcon = true;
                            }
                            else
                            {
                                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon, "we only set this when comctl 6.0 is loaded");
                                owner.View = View.LargeIcon;
                                owner.View = View.SmallIcon;
                            }
                        }
                    }
                    else
                    {
                        int count = owner.Items.Count;

                        for (int i = 0; i < count; i++)
                        {
                            ListViewItem item = owner.Items[i];
                            if (item != null)
                            {
                                item.UnHost(i, true);
                            }
                        }

                        Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                        owner.listItemsArray.Clear();
                    }

                    owner.listItemsTable.Clear();
                    if (owner.IsHandleCreated && !owner.CheckBoxes)
                    {
                        owner.savedCheckedItems = null;
                    }
                    owner.itemCount = 0;

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }
                }
            }

            public bool Contains(ListViewItem item)
            {
                owner.ApplyUpdateCachedItems();
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                {
                    return owner.listItemsTable[item.ID] == item;
                }
                else
                {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    return owner.listItemsArray.Contains(item);
                }
            }

            public ListViewItem Insert(int index, ListViewItem item)
            {
                int count = 0;
                if (owner.VirtualMode)
                {
                    count = Count;
                }
                else
                {
                    count = owner.itemCount;
                }

                if (index < 0 || index > count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (index < count)
                {
                    // if we're not inserting at the end, force the add.
                    //
                    owner.ApplyUpdateCachedItems();
                }

                owner.InsertItems(index, new ListViewItem[] { item }, true);
                if (owner.IsHandleCreated && !owner.CheckBoxes && item.Checked)
                {
                    owner.UpdateSavedCheckedItems(item, true /*addItem*/);
                }

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }

                return item;
            }

            public int IndexOf(ListViewItem item)
            {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this function does not make any sense");
                for (int i = 0; i < Count; i++)
                {
                    if (item == this[i])
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void Remove(ListViewItem item)
            {
                int index = owner.VirtualMode ? Count - 1 : IndexOf(item);

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            public void RemoveAt(int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index < 0 || index >= owner.itemCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (owner.IsHandleCreated && !owner.CheckBoxes && this[index].Checked)
                {
                    owner.UpdateSavedCheckedItems(this[index], false /*addItem*/);
                }

                owner.ApplyUpdateCachedItems();
                int itemID = DisplayIndexToID(index);
                this[index].UnHost(true);

                if (owner.IsHandleCreated)
                {
                    Debug.Assert(owner.listItemsArray is null, "listItemsArray not null, even though handle created");
                    int retval = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.DELETEITEM, (IntPtr)index));

                    if (0 == retval)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                }
                else
                {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    owner.listItemsArray.RemoveAt(index);
                }

                owner.itemCount--;
                owner.listItemsTable.Remove(itemID);

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.itemCount > 0)
                {
                    for (int displayIndex = 0; displayIndex < Count; ++displayIndex)
                    {
                        dest.SetValue(this[displayIndex], index++);
                    }
                }
            }

            public IEnumerator GetEnumerator()
            {
                ListViewItem[] items = new ListViewItem[owner.itemCount];
                CopyTo(items, 0);

                return items.GetEnumerator();
            }
        }

        /// <summary>
        ///  Creates the new instance of AccessibleObject for this ListView control.
        ///  Returning ListViewAccessibleObject.
        /// </summary>
        /// <returns>
        ///  The AccessibleObject for this ListView instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ListViewAccessibleObject(this);
        }
    }
}
