// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;
using NMHEADERW = Windows.Win32.UI.Controls.NMHEADERW;
using NMLVLINK = Windows.Win32.UI.Controls.NMLVLINK;

namespace System.Windows.Forms;

/// <summary>
///  Displays a list of items in one of four views. Each item displays a caption and optionally an image.
/// </summary>
[Docking(DockingBehavior.Ask)]
[Designer($"System.Windows.Forms.Design.ListViewDesigner, {AssemblyRef.SystemDesign}")]
[DefaultProperty(nameof(Items))]
[DefaultEvent(nameof(SelectedIndexChanged))]
[SRDescription(nameof(SR.DescriptionListView))]
public partial class ListView : Control
{
    private const int MASK_HITTESTFLAG = 0x00F7;

    private static readonly object s_cacheVirtualItemsEvent = new();
    private static readonly object s_columnReorderedEvent = new();
    private static readonly object s_columnWidthChangedEvent = new();
    private static readonly object s_columnWidthChangingEvent = new();
    private static readonly object s_drawColumnHeaderEvent = new();
    private static readonly object s_drawItemEvent = new();
    private static readonly object s_drawSubItemEvent = new();
    private static readonly object s_itemSelectionChangedEvent = new();
    private static readonly object s_retrieveVirtualItemEvent = new();
    private static readonly object s_searchForVirtualItemEvent = new();
    private static readonly object s_selectedIndexChangedEvent = new();
    private static readonly object s_virtualItemSelectionRangeChangedEvent = new();
    private static readonly object s_rightToLeftLayoutChangedEvent = new();
    private static readonly object s_groupCollapsedStateChangedEvent = new();
    private static readonly object s_groupTaskLinkClickEvent = new();

    private ItemActivation _activation = ItemActivation.Standard;
    private ListViewAlignment _alignStyle = ListViewAlignment.Top;
    private BorderStyle _borderStyle = BorderStyle.Fixed3D;
    private ColumnHeaderStyle _headerStyle = ColumnHeaderStyle.Clickable;
    private SortOrder _sorting = SortOrder.None;
    private View _viewStyle = View.LargeIcon;
    private string? _toolTipCaption = string.Empty;

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
    private const int LISTVIEWSTATE1_clearingInnerListOnDispose = 0x00000020;

    private const int LVLABELEDITTIMER = 0x2A;
    private const int LVTOOLTIPTRACKING = 0x30;
    private const int MAXTILECOLUMNS = 20;

    // PERF: take all the bools and put them into a state variable
    private Collections.Specialized.BitVector32 _listViewState; // see LISTVIEWSTATE_ constants above
    private Collections.Specialized.BitVector32 _listViewState1; // see LISTVIEWSTATE1_ constants above

    // Ownerdraw data caches... Only valid inside WM_PAINT.

    private Color _odCacheForeColor = SystemColors.WindowText;
    private Color _odCacheBackColor = SystemColors.Window;
    private Font _odCacheFont;
    private HFONT _odCacheFontHandle;
    private FontHandleWrapper? _odCacheFontHandleWrapper;

    private ImageList? _imageListLarge;
    private ImageList? _imageListSmall;
    private ImageList? _imageListState;
    private ImageList? _imageListGroup;

    private MouseButtons _downButton;
    private int _itemCount;
    private int _columnIndex;
    internal ListViewItem? _selectedItem;
    private int _topIndex;
    private bool _hoveredAlready;

    private bool _rightToLeftLayout;

    // member variables which are used for VirtualMode
    private int _virtualListSize;

    private ListViewGroup? _defaultGroup;
    private ListViewGroup? _focusedGroup;

    // Invariant: the table always contains all Items in the ListView, and maps IDs -> Items.
    // listItemsArray is null if the handle is created; otherwise, it contains all Items.
    // We do not try to sort listItemsArray as items are added, but during a handle recreate
    // we will make sure we get the items in the same order the ListView displays them.
    private readonly Dictionary<int, ListViewItem> _listItemsTable = []; // elements are ListViewItem's
    private List<ListViewItem>? _listViewItems = [];

    private Size _tileSize = Size.Empty;

    // when we are in delayed update mode (that is when BeginUpdate has been called, we want to cache the items to
    // add until EndUpdate is called. To do that, we push in an array list into our PropertyStore
    // under this key. When Endupdate is fired, we process the items all at once.
    private static readonly int s_propDelayedUpdateItems = PropertyStore.CreateKey();

    private int _updateCounter; // the counter we use to track how many BeginUpdate/EndUpdate calls there have been.

    private ColumnHeader[]? _columnHeaders;
    private readonly ListViewItemCollection _listItemCollection;
    private readonly ColumnHeaderCollection _columnHeaderCollection;
    private CheckedIndexCollection? _checkedIndexCollection;
    private CheckedListViewItemCollection? _checkedListViewItemCollection;
    private SelectedListViewItemCollection? _selectedListViewItemCollection;
    private SelectedIndexCollection? _selectedIndexCollection;
    private ListViewGroupCollection? _groups;
    private ListViewInsertionMark? _insertionMark;
    private LabelEditEventHandler? _onAfterLabelEdit;
    private LabelEditEventHandler? _onBeforeLabelEdit;
    private ColumnClickEventHandler? _onColumnClick;
    private EventHandler? _onItemActivate;
    private ItemCheckedEventHandler? _onItemChecked;
    private ItemDragEventHandler? _onItemDrag;
    private ItemCheckEventHandler? _onItemCheck;
    private ListViewItemMouseHoverEventHandler? _onItemMouseHover;

    // IDs for identifying ListViewItem's
    private int _nextID;

    // We save selected and checked items between handle creates.
    private List<ListViewItem>? _savedSelectedItems;
    private List<ListViewItem>? _savedCheckedItems;

    // Sorting
    private IComparer? _listItemSorter;

    private ListViewItem? _prevHoveredItem;

    private bool _blockLabelEdit;

    internal ListViewLabelEditNativeWindow? _labelEdit;

    // Used to record the SubItem to which the Label Edit belongs.
    internal ListViewSubItem? _listViewSubItem;

    // Background image stuff
    // Because we have to create a temporary file and the OS does not clean up the temporary files from the machine
    // we have to do that ourselves
    private string _backgroundImageFileName = string.Empty;

    // it *seems* that if the user changes the background image then the win32 listView will hang on to the previous
    // background image until it gets the first WM_PAINT message -  I use words like *seems* because nothing is guaranteed
    // when it comes to win32 listView.
    // so our wrapper has to hang on to the previousBackgroundImageFileNames and destroy them after it gets the first WM_PAINT message

    private int _bkImgFileNamesCount = -1;
    private string?[]? _bkImgFileNames;
    private const int BKIMGARRAYSIZE = 8;

    // If the user clicked on the column divider, the native ListView fires HDN_ITEMCHANGED on each mouse up event.
    // This means that even if the user did not change the column width our wrapper will still think
    // that the column header width changed.
    // We need to make our ListView wrapper more robust in face of this limitation inside ComCtl ListView.
    // columnHeaderClicked will be set in HDN_BEGINTRACK and reset in HDN_ITEMCHANGED.
    private ColumnHeader? _columnHeaderClicked;
    private int _columnHeaderClickedWidth;

    // The user cancelled the column width changing event.
    // We cache the NewWidth supplied by the user and use it on HDN_ENDTRACK to set the final column width.
    private int _newWidthForColumnWidthChangingCancelled = -1;

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

        _listViewState = new Collections.Specialized.BitVector32(listViewStateFlags);

        _listViewState1 = new Collections.Specialized.BitVector32(LISTVIEWSTATE1_useCompatibleStateImageBehavior);
        SetStyle(ControlStyles.UserPaint, false);
        SetStyle(ControlStyles.StandardClick, false);
        SetStyle(ControlStyles.UseTextForAccessibility, false);

        _odCacheFont = Font;
        _odCacheFontHandle = FontHandle;
        SetBounds(0, 0, 121, 97);

        _listItemCollection = new ListViewItemCollection(new ListViewNativeItemCollection(this));
        _columnHeaderCollection = new ColumnHeaderCollection(this);
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
            return _activation;
        }

        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            if (HotTracking && value != ItemActivation.OneClick)
            {
                throw new ArgumentException(SR.ListViewActivationMustBeOnWhenHotTrackingIsOn, nameof(value));
            }

            if (_activation != value)
            {
                _activation = value;
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
            return _alignStyle;
        }

        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_alignStyle != value)
            {
                _alignStyle = value;
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
            return _listViewState[LISTVIEWSTATE_allowColumnReorder];
        }

        set
        {
            if (AllowColumnReorder != value)
            {
                _listViewState[LISTVIEWSTATE_allowColumnReorder] = value;
                UpdateExtendedStyles();
            }
        }
    }

    /// <summary>
    ///  If AutoArrange is true items are automatically arranged according to
    ///  the alignment property. Items are also kept snapped to grid.
    ///  This property is only meaningful in Large Icon or Small Icon views.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.ListViewAutoArrangeDescr))]
    public bool AutoArrange
    {
        get
        {
            return _listViewState[LISTVIEWSTATE_autoArrange];
        }

        set
        {
            if (AutoArrange != value)
            {
                _listViewState[LISTVIEWSTATE_autoArrange] = value;
                UpdateStyles();
            }
        }
    }

    public override Color BackColor
    {
        get => ShouldSerializeBackColor() ? base.BackColor : SystemColors.Window;
        set
        {
            base.BackColor = value;
            if (IsHandleCreated)
            {
                PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKCOLOR, (WPARAM)0, (LPARAM)BackColor);
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
    public new event EventHandler? BackgroundImageLayoutChanged
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
            return _listViewState[LISTVIEWSTATE_backgroundImageTiled];
        }
        set
        {
            if (BackgroundImageTiled != value)
            {
                _listViewState[LISTVIEWSTATE_backgroundImageTiled] = value;
                if (IsHandleCreated && BackgroundImage is not null)
                {
                    // Don't call SetBackgroundImage because SetBackgroundImage deletes the existing image
                    // We don't need to delete it and this causes BAD problems w/ the Win32 list view control.
                    fixed (char* pBackgroundImageFileName = _backgroundImageFileName)
                    {
                        LVBKIMAGEW lvbkImage = default;
                        lvbkImage.ulFlags = BackgroundImageTiled
                            ? LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_STYLE_TILE
                            : LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_STYLE_NORMAL;

                        lvbkImage.ulFlags |= LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_SOURCE_URL;
                        lvbkImage.pszImage = pBackgroundImageFileName;
                        lvbkImage.cchImageMax = (uint)(_backgroundImageFileName.Length + 1);

                        PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKIMAGEW, (WPARAM)0, ref lvbkImage);
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
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.borderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_borderStyle != value)
            {
                _borderStyle = value;
                UpdateStyles();
            }
        }
    }

    /// <summary>
    ///  If CheckBoxes is true, every item will display a checkbox next
    ///  to it. The user can change the state of the item by clicking the checkbox.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ListViewCheckBoxesDescr))]
    public bool CheckBoxes
    {
        get
        {
            return _listViewState[LISTVIEWSTATE_checkBoxes];
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

                    if (CheckBoxes && !VirtualMode)
                    {
                        // Save away the checked items just in case we re-activate checkboxes
                        _savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                        ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                        CheckedItems.CopyTo(items, 0);
                        for (int i = 0; i < items.Length; i++)
                        {
                            _savedCheckedItems.Add(items[i]);
                        }
                    }

                    _listViewState[LISTVIEWSTATE_checkBoxes] = value;
                    UpdateExtendedStyles();

                    if (CheckBoxes && _savedCheckedItems is not null)
                    {
                        // Check the saved checked items.
                        if (_savedCheckedItems.Count > 0)
                        {
                            foreach (ListViewItem item in _savedCheckedItems)
                            {
                                item.Checked = true;
                            }
                        }

                        _savedCheckedItems = null;
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

                    if (CheckBoxes && !VirtualMode)
                    {
                        // Save away the checked items just in case we re-activate checkboxes
                        _savedCheckedItems = new List<ListViewItem>(CheckedItems.Count);
                        ListViewItem[] items = new ListViewItem[CheckedItems.Count];
                        CheckedItems.CopyTo(items, 0);
                        for (int i = 0; i < items.Length; i++)
                        {
                            _savedCheckedItems.Add(items[i]);
                        }
                    }

                    _listViewState[LISTVIEWSTATE_checkBoxes] = value;

                    if ((!value && StateImageList is not null && IsHandleCreated) ||
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

                    if (CheckBoxes && _savedCheckedItems is not null)
                    {
                        // Check the saved checked items.
                        if (_savedCheckedItems.Count > 0)
                        {
                            foreach (ListViewItem item in _savedCheckedItems)
                            {
                                item.Checked = true;
                            }
                        }

                        _savedCheckedItems = null;
                    }

                    // Setting the LVS_CHECKBOXES window style also causes the ListView to display the default checkbox
                    // images rather than the user specified StateImageList. We send a LVM_SETIMAGELIST to restore the
                    // user's images.
                    if (IsHandleCreated && _imageListState is not null)
                    {
                        if (CheckBoxes)
                        {
                            // We want custom checkboxes.
                            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_STATE, (LPARAM)_imageListState.Handle);
                        }
                        else
                        {
                            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, PInvoke.LVSIL_STATE);
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
            _checkedIndexCollection ??= new CheckedIndexCollection(this);

            return _checkedIndexCollection;
        }
    }

    internal ToolTip KeyboardToolTip { get; } = new();

    /// <summary>
    ///  The currently checked list items.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CheckedListViewItemCollection CheckedItems
    {
        get
        {
            _checkedListViewItemCollection ??= new CheckedListViewItemCollection(this);

            return _checkedListViewItemCollection;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor($"System.Windows.Forms.Design.ColumnHeaderCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.ListViewColumnsDescr))]
    [Localizable(true)]
    [MergableProperty(false)]
    public ColumnHeaderCollection Columns
    {
        get
        {
            return _columnHeaderCollection;
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

            cp.ClassName = PInvoke.WC_LISTVIEW;

            // Keep the scrollbar if we are just updating styles.
            if (IsHandleCreated)
            {
                int currentStyle = (int)PInvokeCore.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
                cp.Style |= currentStyle & (int)(WINDOW_STYLE.WS_HSCROLL | WINDOW_STYLE.WS_VSCROLL);
            }

            cp.Style |= (int)PInvoke.LVS_SHAREIMAGELISTS;

            switch (_alignStyle)
            {
                case ListViewAlignment.Top:
                    cp.Style |= (int)PInvoke.LVS_ALIGNTOP;
                    break;
                case ListViewAlignment.Left:
                    cp.Style |= (int)PInvoke.LVS_ALIGNLEFT;
                    break;
            }

            if (AutoArrange)
            {
                cp.Style |= (int)PInvoke.LVS_AUTOARRANGE;
            }

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                    break;
            }

            switch (_headerStyle)
            {
                case ColumnHeaderStyle.None:
                    cp.Style |= (int)PInvoke.LVS_NOCOLUMNHEADER;
                    break;
                case ColumnHeaderStyle.Nonclickable:
                    cp.Style |= (int)PInvoke.LVS_NOSORTHEADER;
                    break;
            }

            if (LabelEdit)
            {
                cp.Style |= (int)PInvoke.LVS_EDITLABELS;
            }

            if (!LabelWrap)
            {
                cp.Style |= (int)PInvoke.LVS_NOLABELWRAP;
            }

            if (!HideSelection)
            {
                cp.Style |= (int)PInvoke.LVS_SHOWSELALWAYS;
            }

            if (!MultiSelect)
            {
                cp.Style |= (int)PInvoke.LVS_SINGLESEL;
            }

            if (_listItemSorter is null)
            {
                switch (_sorting)
                {
                    case SortOrder.Ascending:
                        cp.Style |= (int)PInvoke.LVS_SORTASCENDING;
                        break;
                    case SortOrder.Descending:
                        cp.Style |= (int)PInvoke.LVS_SORTDESCENDING;
                        break;
                }
            }

            if (VirtualMode)
            {
                cp.Style |= (int)PInvoke.LVS_OWNERDATA;
            }

            // We can do this 'cuz the viewStyle enums are the same values as the actual LVS styles
            // this new check since the value for LV_VIEW_TILE == LVS_SINGLESEL; so don't OR that value since
            // LV_VIEW_TILE is not a STYLE but should be Send via a SENDMESSAGE.
            if (_viewStyle != View.Tile)
            {
                cp.Style |= (int)_viewStyle;
            }

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for Form explicitly.
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    internal ListViewGroup DefaultGroup =>
        _defaultGroup ??= new ListViewGroup(string.Format(SR.ListViewGroupDefaultGroup, "1"))
        {
            ListView = this
        };

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
            return _listViewState[LISTVIEWSTATE_expectingMouseUp];
        }
    }

    /// <summary>
    ///  Retrieves the group which currently has the user focus. This is the
    ///  group that's drawn with the dotted focus rectangle around it.
    ///  Returns null if no group is currently focused.
    /// </summary>
    internal ListViewGroup? FocusedGroup
    {
        get => IsHandleCreated ? _focusedGroup : null;
        set
        {
            if (IsHandleCreated && value is not null)
            {
                value.Focused = true;
                _focusedGroup = value;
            }
        }
    }

    /// <summary>
    ///  Retrieves the item which currently has the user focus. This is the
    ///  item that's drawn with the dotted focus rectangle around it.
    ///  Returns null if no item is currently focused.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListViewFocusedItemDescr))]
    public ListViewItem? FocusedItem
    {
        get
        {
            if (IsHandleCreated)
            {
                int displayIndex = (int)PInvokeCore.SendMessage(
                    this,
                    PInvoke.LVM_GETNEXTITEM,
                    (WPARAM)(-1),
                    (LPARAM)PInvoke.LVNI_FOCUSED);

                if (displayIndex > -1)
                {
                    return Items[displayIndex];
                }
            }

            return null;
        }
        set
        {
            if (IsHandleCreated && value is not null)
            {
                value.Focused = true;
            }
        }
    }

    public override Color ForeColor
    {
        get => ShouldSerializeForeColor() ? base.ForeColor : SystemColors.WindowText;
        set
        {
            base.ForeColor = value;
            if (IsHandleCreated)
            {
                PInvokeCore.SendMessage(this, PInvoke.LVM_SETTEXTCOLOR, (WPARAM)0, (LPARAM)ForeColor);
            }
        }
    }

    private bool FlipViewToLargeIconAndSmallIcon
    {
        get
        {
            // it never hurts to check that our house is in order
            Debug.Assert(!_listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || View == View.SmallIcon, "we need this bit only in SmallIcon view");
            Debug.Assert(!_listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] || Application.ComCtlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

            return _listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon];
        }
        set
        {
            // it never hurts to check that our house is in order
            Debug.Assert(!value || View == View.SmallIcon, "we need this bit only in SmallIcon view");
            Debug.Assert(!value || Application.ComCtlSupportsVisualStyles, "we need this bit only when loading ComCtl 6.0");

            _listViewState[LISTVIEWSTATE_flipViewToLargeIconAndSmallIcon] = value;
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
            return _listViewState[LISTVIEWSTATE_fullRowSelect];
        }
        set
        {
            if (FullRowSelect != value)
            {
                _listViewState[LISTVIEWSTATE_fullRowSelect] = value;
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
            return _listViewState[LISTVIEWSTATE_gridLines];
        }

        set
        {
            if (GridLines != value)
            {
                _listViewState[LISTVIEWSTATE_gridLines] = value;
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
    public ImageList? GroupImageList
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

            PInvokeCore.SendMessage(
                this,
                PInvoke.LVM_SETIMAGELIST,
                (WPARAM)PInvoke.LVSIL_GROUPHEADER,
                (LPARAM)(value is null ? 0 : value.Handle));
        }
    }

    /// <summary>
    ///  The collection of groups belonging to this ListView
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [Editor($"System.Windows.Forms.Design.ListViewGroupCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.ListViewGroupsDescr))]
    [MergableProperty(false)]
    public ListViewGroupCollection Groups
    {
        get
        {
            _groups ??= new ListViewGroupCollection(this);

            return _groups;
        }
    }

    // ListViewGroup are not displayed when the ListView is in "List" view
    internal bool GroupsDisplayed => View != View.List && GroupsEnabled;

    // this essentially means that the version of CommCtl supports list view grouping
    // and that the user wants to make use of list view groups
    internal bool GroupsEnabled
        => ShowGroups && _groups is not null && _groups.Count > 0 && Application.ComCtlSupportsVisualStyles && !VirtualMode;

    /// <summary>
    ///  Column headers can either be invisible, clickable, or non-clickable.
    ///  This property is only meaningful in Details view
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(ColumnHeaderStyle.Clickable)]
    [SRDescription(nameof(SR.ListViewHeaderStyleDescr))]
    public ColumnHeaderStyle HeaderStyle
    {
        get { return _headerStyle; }
        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            if (_headerStyle != value)
            {
                // We can switch between NONE and either *one* of the other styles without
                // recreating the handle, but if we change from CLICKABLE to NONCLICKABLE
                // or vice versa, with or without an intervening setting of NONE, then
                // the handle needs to be recreated.
                _headerStyle = value;
                if ((_listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Clickable) ||
                    (!_listViewState[LISTVIEWSTATE_nonclickHdr] && value == ColumnHeaderStyle.Nonclickable))
                {
                    _listViewState[LISTVIEWSTATE_nonclickHdr] = !_listViewState[LISTVIEWSTATE_nonclickHdr];
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
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ListViewHideSelectionDescr))]
    public bool HideSelection
    {
        get
        {
            return _listViewState[LISTVIEWSTATE_hideSelection];
        }

        set
        {
            if (HideSelection != value)
            {
                _listViewState[LISTVIEWSTATE_hideSelection] = value;
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
            return _listViewState[LISTVIEWSTATE_hotTracking];
        }
        set
        {
            if (HotTracking != value)
            {
                _listViewState[LISTVIEWSTATE_hotTracking] = value;
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
            return _listViewState[LISTVIEWSTATE_hoverSelection];
        }

        set
        {
            if (HoverSelection != value)
            {
                if (HotTracking && !value)
                {
                    throw new ArgumentException(SR.ListViewHoverMustBeOnWhenHotTrackingIsOn, nameof(value));
                }

                _listViewState[LISTVIEWSTATE_hoverSelection] = value;
                UpdateExtendedStyles();
            }
        }
    }

    internal bool InsertingItemsNatively
    {
        get
        {
            return _listViewState1[LISTVIEWSTATE1_insertingItemsNatively];
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListViewInsertionMarkDescr))]
    public ListViewInsertionMark InsertionMark
    {
        get
        {
            _insertionMark ??= new ListViewInsertionMark(this);

            return _insertionMark;
        }
    }

    private bool ItemCollectionChangedInMouseDown
    {
        get
        {
            return _listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown];
        }
        set
        {
            _listViewState[LISTVIEWSTATE_itemCollectionChangedInMouseDown] = value;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [Editor($"System.Windows.Forms.Design.ListViewItemCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.ListViewItemsDescr))]
    [MergableProperty(false)]
    public ListViewItemCollection Items
    {
        get
        {
            return _listItemCollection;
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
            return _listViewState[LISTVIEWSTATE_labelEdit];
        }
        set
        {
            if (LabelEdit != value)
            {
                _listViewState[LISTVIEWSTATE_labelEdit] = value;
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
            return _listViewState[LISTVIEWSTATE_labelWrap];
        }
        set
        {
            if (LabelWrap != value)
            {
                _listViewState[LISTVIEWSTATE_labelWrap] = value;
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
    public ImageList? LargeImageList
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

            PInvokeCore.SendMessage(
                this,
                PInvoke.LVM_SETIMAGELIST,
                (WPARAM)PInvoke.LVSIL_NORMAL,
                (LPARAM)(value?.Handle ?? 0));
            if (AutoArrange && !_listViewState1[LISTVIEWSTATE1_disposingImageLists])
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
            return _listViewState[LISTVIEWSTATE_handleDestroyed];
        }
        set
        {
            _listViewState[LISTVIEWSTATE_handleDestroyed] = value;
        }
    }

    /// <summary>
    ///  The sorting comparer for this ListView.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListViewItemSorterDescr))]
    public IComparer? ListViewItemSorter
    {
        get
        {
            return _listItemSorter;
        }
        set
        {
            if (_listItemSorter != value)
            {
                _listItemSorter = value;

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
            return _listViewState[LISTVIEWSTATE_multiSelect];
        }
        set
        {
            if (MultiSelect != value)
            {
                _listViewState[LISTVIEWSTATE_multiSelect] = value;
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
            return _listViewState[LISTVIEWSTATE_ownerDraw];
        }

        set
        {
            if (OwnerDraw != value)
            {
                _listViewState[LISTVIEWSTATE_ownerDraw] = value;
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
            return _rightToLeftLayout;
        }

        set
        {
            if (value != _rightToLeftLayout)
            {
                _rightToLeftLayout = value;
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                {
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => Events.AddHandler(s_rightToLeftLayoutChangedEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftLayoutChangedEvent, value);
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
            return _listViewState[LISTVIEWSTATE_scrollable];
        }
        set
        {
            if (Scrollable != value)
            {
                _listViewState[LISTVIEWSTATE_scrollable] = value;
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
            _selectedIndexCollection ??= new SelectedIndexCollection(this);

            return _selectedIndexCollection;
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
            _selectedListViewItemCollection ??= new SelectedListViewItemCollection(this);

            return _selectedListViewItemCollection;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.ListViewShowGroupsDescr))]
    public bool ShowGroups
    {
        get
        {
            return _listViewState[LISTVIEWSTATE_showGroups];
        }
        set
        {
            if (value != ShowGroups)
            {
                _listViewState[LISTVIEWSTATE_showGroups] = value;
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
    public ImageList? SmallImageList
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

            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_SMALL, (LPARAM)(value?.Handle ?? 0));

            if (View == View.SmallIcon)
            {
                View = View.LargeIcon;
                View = View.SmallIcon;
            }
            else if (!_listViewState1[LISTVIEWSTATE1_disposingImageLists])
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
            return _listViewState[LISTVIEWSTATE_showItemToolTips];
        }
        set
        {
            if (ShowItemToolTips != value)
            {
                _listViewState[LISTVIEWSTATE_showItemToolTips] = value;
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
            return _sorting;
        }
        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            if (_sorting != value)
            {
                _sorting = value;
                if (View is View.LargeIcon or View.SmallIcon)
                {
                    if (_listItemSorter is null)
                    {
                        _listItemSorter = new IconComparer(_sorting);
                    }
                    else if (_listItemSorter is IconComparer iconComparer)
                    {
                        iconComparer.SortOrder = _sorting;
                    }
                }
                else if (value == SortOrder.None)
                {
                    _listItemSorter = null;
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
    public ImageList? StateImageList
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
                    PInvokeCore.SendMessage(this,
                        PInvoke.LVM_SETIMAGELIST,
                        (WPARAM)PInvoke.LVSIL_STATE,
                        (LPARAM)(value?.Handle ?? 0));
                }
            }
            else
            {
                DetachStateImageListHandlers();

                if (IsHandleCreated && _imageListState is not null && CheckBoxes)
                {
                    // If CheckBoxes are set to true, then we will have to recreate the handle.
                    // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native listView destroys
                    // the state imageList.
                    // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
                    // So we make the native listView forget about its StateImageList just before we recreate the handle.
                    // Likely related to https://devblogs.microsoft.com/oldnewthing/20171128-00/?p=97475
                    PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_STATE);
                }

                _imageListState = value;
                AttachStateImageListHandlers();

                if (!IsHandleCreated)
                {
                    return;
                }

                if (CheckBoxes)
                {
                    // Need to recreate to get the new images pushed in.
                    RecreateHandleInternal();
                }
                else
                {
                    PInvokeCore.SendMessage(
                        this,
                        PInvoke.LVM_SETIMAGELIST,
                        (WPARAM)PInvoke.LVSIL_STATE,
                        (_imageListState is null || _imageListState.Images.Count == 0) ? 0 : _imageListState.Handle);
                }

                // Comctl should handle auto-arrange for us, but doesn't
                if (!_listViewState1[LISTVIEWSTATE1_disposingImageLists])
                {
                    UpdateListViewItemsLocations();
                }
            }
        }
    }

    // Getting a rectangle for a sub item only works for a ListView in "Details" and "Tile" views.
    // Additionally, a ListView in the "Tile" view does not show ListViewSubItems when visual styles are disabled.
    internal bool SupportsListViewSubItems => View == View.Details || (View == View.Tile && Application.ComCtlSupportsVisualStyles);

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
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
            if (!_tileSize.IsEmpty)
            {
                return _tileSize;
            }

            if (!IsHandleCreated)
            {
                return Size.Empty;
            }

            LVTILEVIEWINFO tileViewInfo = new()
            {
                cbSize = (uint)sizeof(LVTILEVIEWINFO),
                dwMask = LVTILEVIEWINFO_MASK.LVTVIM_TILESIZE
            };

            PInvokeCore.SendMessage(this, PInvoke.LVM_GETTILEVIEWINFO, (WPARAM)0, ref tileViewInfo);

            return tileViewInfo.sizeTile;
        }
        set
        {
            if (_tileSize == value)
            {
                return;
            }

            if (value.Height <= 0 || value.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(TileSize), SR.ListViewTileSizeMustBePositive);
            }

            _tileSize = value;
            if (!IsHandleCreated)
            {
                return;
            }

            LVTILEVIEWINFO tileViewInfo = new()
            {
                cbSize = (uint)sizeof(LVTILEVIEWINFO),
                dwMask = LVTILEVIEWINFO_MASK.LVTVIM_TILESIZE,
                dwFlags = LVTILEVIEWINFO_FLAGS.LVTVIF_FIXEDSIZE,
                sizeTile = _tileSize
            };

            nint result = PInvokeCore.SendMessage(this, PInvoke.LVM_SETTILEVIEWINFO, (WPARAM)0, ref tileViewInfo);
            Debug.Assert(result != 0, "LVM_SETTILEVIEWINFO failed");

            if (AutoArrange)
            {
                UpdateListViewItemsLocations();
            }
        }
    }

    private bool ShouldSerializeTileSize()
    {
        return !_tileSize.Equals(Size.Empty);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListViewTopItemDescr))]
    public ListViewItem? TopItem
    {
        get
        {
            if (_viewStyle is View.LargeIcon or View.SmallIcon or View.Tile)
            {
                throw new InvalidOperationException(SR.ListViewGetTopItem);
            }

            if (!IsHandleCreated)
            {
                return Items.Count > 0 ? Items[0] : null;
            }

            _topIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_GETTOPINDEX);
            return _topIndex >= 0 && _topIndex < Items.Count ? Items[_topIndex] : null;
        }
        set
        {
            if (_viewStyle is View.LargeIcon or View.SmallIcon or View.Tile)
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
            ListViewItem? topItem = TopItem;

            if ((topItem is null) && (_topIndex == Items.Count))
            {
                // Result of the message is the number of items in the list rather than an index of an item in the list.
                // This causes TopItem to return null. A side issue is that EnsureVisible doesn't do too well
                // here either, because it causes the listview to go blank rather than displaying anything useful.
                // To work around this, we force the listbox to display the first item, then scroll down to the item
                // user is setting as the top item.
                if (Scrollable)
                {
                    EnsureVisible(0);
                    Scroll(0, value.Index);
                }

                return;
            }

            if (topItem is null || value.Index == topItem.Index)
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
            return _listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior];
        }
        set
        {
            _listViewState1[LISTVIEWSTATE1_useCompatibleStateImageBehavior] = value;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(View.LargeIcon)]
    [SRDescription(nameof(SR.ListViewViewDescr))]
    public View View
    {
        get
        {
            return _viewStyle;
        }
        set
        {
            if (value == View.Tile && CheckBoxes)
            {
                throw new NotSupportedException(SR.ListViewTileViewDoesNotSupportCheckBoxes);
            }

            FlipViewToLargeIconAndSmallIcon = false;

            // valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);

            if (value == View.Tile && VirtualMode)
            {
                throw new NotSupportedException(SR.ListViewCantSetViewToTileViewInVirtualMode);
            }

            if (_viewStyle != value)
            {
                _viewStyle = value;
                if (IsHandleCreated && Application.ComCtlSupportsVisualStyles)
                {
                    PInvokeCore.SendMessage(this, PInvoke.LVM_SETVIEW, (WPARAM)(int)_viewStyle);
                    UpdateGroupView();

                    // if we switched to Tile view we should update the win32 list view tile view info
                    if (_viewStyle == View.Tile)
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
            return _virtualListSize;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(string.Format(SR.ListViewVirtualListSizeInvalidArgument, "value", value));
            }

            if (value == _virtualListSize)
            {
                return;
            }

            bool keepTopItem = IsHandleCreated && VirtualMode && View == View.Details && !DesignMode;
            int topIndex = -1;
            if (keepTopItem)
            {
                topIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_GETTOPINDEX);
            }

            _virtualListSize = value;

            if (IsHandleCreated && VirtualMode && !DesignMode)
            {
                PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMCOUNT, (WPARAM)_virtualListSize);
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
            return _listViewState[LISTVIEWSTATE_virtualMode];
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

            _listViewState[LISTVIEWSTATE_virtualMode] = value;

            RecreateHandleInternal();
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewAfterLabelEditDescr))]
    public event LabelEditEventHandler? AfterLabelEdit
    {
        add => _onAfterLabelEdit += value;
        remove => _onAfterLabelEdit -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewBeforeLabelEditDescr))]
    public event LabelEditEventHandler? BeforeLabelEdit
    {
        add => _onBeforeLabelEdit += value;
        remove => _onBeforeLabelEdit -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewCacheVirtualItemsEventDescr))]
    public event CacheVirtualItemsEventHandler? CacheVirtualItems
    {
        add => Events.AddHandler(s_cacheVirtualItemsEvent, value);
        remove => Events.RemoveHandler(s_cacheVirtualItemsEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewColumnClickDescr))]
    public event ColumnClickEventHandler? ColumnClick
    {
        add => _onColumnClick += value;
        remove => _onColumnClick -= value;
    }

    /// <summary>
    ///  Occurs when the user clicks a <see cref="ListViewGroup.TaskLink"/> on a <see cref="ListViewGroup"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewGroupTaskLinkClickDescr))]
    public event EventHandler<ListViewGroupEventArgs>? GroupTaskLinkClick
    {
        add => Events.AddHandler(s_groupTaskLinkClickEvent, value);
        remove => Events.RemoveHandler(s_groupTaskLinkClickEvent, value);
    }

    /// <summary>
    ///  Tell the user that the column headers are being rearranged
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListViewColumnReorderedDscr))]
    public event ColumnReorderedEventHandler? ColumnReordered
    {
        add => Events.AddHandler(s_columnReorderedEvent, value);
        remove => Events.RemoveHandler(s_columnReorderedEvent, value);
    }

    /// <summary>
    ///  Tell the user that the column width changed
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListViewColumnWidthChangedDscr))]
    public event ColumnWidthChangedEventHandler? ColumnWidthChanged
    {
        add => Events.AddHandler(s_columnWidthChangedEvent, value);
        remove => Events.RemoveHandler(s_columnWidthChangedEvent, value);
    }

    /// <summary>
    ///  Tell the user that the column width is being changed
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListViewColumnWidthChangingDscr))]
    public event ColumnWidthChangingEventHandler? ColumnWidthChanging
    {
        add => Events.AddHandler(s_columnWidthChangingEvent, value);
        remove => Events.RemoveHandler(s_columnWidthChangingEvent, value);
    }

    /// <summary>
    ///  Fires in owner draw + Details mode when a column header needs to be drawn.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewDrawColumnHeaderEventDescr))]
    public event DrawListViewColumnHeaderEventHandler? DrawColumnHeader
    {
        add => Events.AddHandler(s_drawColumnHeaderEvent, value);
        remove => Events.RemoveHandler(s_drawColumnHeaderEvent, value);
    }

    /// <summary>
    ///  Fires in owner draw mode when a ListView item needs to be drawn.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewDrawItemEventDescr))]
    public event DrawListViewItemEventHandler? DrawItem
    {
        add => Events.AddHandler(s_drawItemEvent, value);
        remove => Events.RemoveHandler(s_drawItemEvent, value);
    }

    /// <summary>
    ///  Fires in owner draw mode and Details view when a ListView sub-item needs to be drawn.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewDrawSubItemEventDescr))]
    public event DrawListViewSubItemEventHandler? DrawSubItem
    {
        add => Events.AddHandler(s_drawSubItemEvent, value);
        remove => Events.RemoveHandler(s_drawSubItemEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewItemClickDescr))]
    public event EventHandler? ItemActivate
    {
        add => _onItemActivate += value;
        remove => _onItemActivate -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.CheckedListBoxItemCheckDescr))]
    public event ItemCheckEventHandler? ItemCheck
    {
        add => _onItemCheck += value;
        remove => _onItemCheck -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewItemCheckedDescr))]
    public event ItemCheckedEventHandler? ItemChecked
    {
        add => _onItemChecked += value;
        remove => _onItemChecked -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewItemDragDescr))]
    public event ItemDragEventHandler? ItemDrag
    {
        add => _onItemDrag += value;
        remove => _onItemDrag -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewItemMouseHoverDescr))]
    public event ListViewItemMouseHoverEventHandler? ItemMouseHover
    {
        add => _onItemMouseHover += value;
        remove => _onItemMouseHover -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewItemSelectionChangedDescr))]
    public event ListViewItemSelectionChangedEventHandler? ItemSelectionChanged
    {
        add => Events.AddHandler(s_itemSelectionChangedEvent, value);
        remove => Events.RemoveHandler(s_itemSelectionChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the <see cref="ListViewGroup.CollapsedState"/> changes on a <see cref="ListViewGroup"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewGroupCollapsedStateChangedDescr))]
    public event EventHandler<ListViewGroupEventArgs>? GroupCollapsedStateChanged
    {
        add => Events.AddHandler(s_groupCollapsedStateChangedEvent, value);
        remove => Events.RemoveHandler(s_groupCollapsedStateChangedEvent, value);
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
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  ListView Onpaint.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewRetrieveVirtualItemEventDescr))]
    public event RetrieveVirtualItemEventHandler? RetrieveVirtualItem
    {
        add => Events.AddHandler(s_retrieveVirtualItemEvent, value);
        remove => Events.RemoveHandler(s_retrieveVirtualItemEvent, value);
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewSearchForVirtualItemDescr))]
    public event SearchForVirtualItemEventHandler? SearchForVirtualItem
    {
        add => Events.AddHandler(s_searchForVirtualItemEvent, value);
        remove => Events.RemoveHandler(s_searchForVirtualItemEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewSelectedIndexChangedDescr))]
    public event EventHandler? SelectedIndexChanged
    {
        add => Events.AddHandler(s_selectedIndexChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedIndexChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewVirtualItemsSelectionRangeChangedDescr))]
    public event ListViewVirtualItemsSelectionRangeChangedEventHandler? VirtualItemsSelectionRangeChanged
    {
        add => Events.AddHandler(s_virtualItemSelectionRangeChangedEvent, value);
        remove => Events.RemoveHandler(s_virtualItemSelectionRangeChangedEvent, value);
    }

    internal unsafe void AnnounceColumnHeader(Point point)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        HWND hwnd = (HWND)PInvokeCore.SendMessage(this, PInvoke.LVM_GETHEADER);
        if (hwnd.IsNull)
        {
            return;
        }

        LVHITTESTINFO lvhi = new()
        {
            pt = PointToClient(point)
        };

        SCROLLINFO si = new()
        {
            cbSize = (uint)sizeof(SCROLLINFO),
            fMask = SCROLLINFO_MASK.SIF_POS
        };

        if (PInvoke.GetScrollInfo(this, SCROLLBAR_CONSTANTS.SB_HORZ, ref si))
        {
            lvhi.pt.X += si.nPos;
        }

        if (IsAccessibilityObjectCreated
            && PInvokeCore.SendMessage(hwnd, PInvoke.HDM_HITTEST, (WPARAM)0, ref lvhi) != -1 && lvhi.iItem > -1)
        {
            AccessibilityObject.InternalRaiseAutomationNotification(
                Automation.AutomationNotificationKind.Other,
                Automation.AutomationNotificationProcessing.MostRecent,
                Columns[lvhi.iItem].Text);
        }
    }

    /// <summary>
    ///  Called to add any delayed update items we have to the list view. We do this because
    ///  we have optimized the case where a user is only adding items within a beginupdate/endupdate
    ///  block. If they do any other operations (get the count, remove, insert, etc.), we push in the
    ///  cached up items first, then do the requested operation. This keeps it simple so we don't have to
    ///  try to maintain parallel state of the cache during a begin update end update.
    /// </summary>
    private void ApplyUpdateCachedItems()
    {
        // First check if there is a delayed update array.
        if (Properties.TryGetValue(s_propDelayedUpdateItems, out List<ListViewItem>? newItems))
        {
            // If there is, clear it and push the items in.
            Properties.RemoveValue(s_propDelayedUpdateItems);
            if (newItems.Count > 0)
            {
                InsertItems(_itemCount, [.. newItems], checkHosting: false);
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
        if (_viewStyle != View.SmallIcon)
        {
            return;
        }

        switch (value)
        {
            case PInvoke.LVA_DEFAULT:
            case (ListViewAlignment)PInvoke.LVA_ALIGNLEFT:
            case (ListViewAlignment)PInvoke.LVA_ALIGNTOP:
            case (ListViewAlignment)PInvoke.LVA_SNAPTOGRID:
                if (IsHandleCreated)
                {
                    PInvokeCore.PostMessage(this, PInvoke.LVM_ARRANGE, (WPARAM)(int)value);
                }

                break;

            default:
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
        }

        if (!VirtualMode && _sorting != SortOrder.None)
        {
            Sort();
        }
    }

    /// <summary>
    ///  In Large Icon or Small Icon view, arranges items according to the ListView's
    ///  current alignment style.
    /// </summary>
    public void ArrangeIcons() => ArrangeIcons(PInvoke.LVA_DEFAULT);

    private void AttachGroupImageListHandlers()
    {
        if (_imageListGroup is null)
        {
            return;
        }

        // NOTE: any handlers added here should be removed in DetachGroupImageListHandlers
        _imageListGroup.RecreateHandle += GroupImageListRecreateHandle;
        _imageListGroup.Disposed += DetachImageList;
        _imageListGroup.ChangeHandle += GroupImageListChangedHandle;
    }

    private void AttachLargeImageListHandlers()
    {
        if (_imageListLarge is null)
        {
            return;
        }

        // NOTE: any handlers added here should be removed in DetachLargeImageListHandlers
        _imageListLarge.RecreateHandle += LargeImageListRecreateHandle;
        _imageListLarge.Disposed += DetachImageList;
        _imageListLarge.ChangeHandle += LargeImageListChangedHandle;
    }

    private void AttachSmallImageListListHandlers()
    {
        if (_imageListSmall is null)
        {
            return;
        }

        // NOTE: any handlers added here should be removed in DetachSmallImageListListHandlers
        _imageListSmall.RecreateHandle += SmallImageListRecreateHandle;
        _imageListSmall.Disposed += DetachImageList;
    }

    private void AttachStateImageListHandlers()
    {
        if (_imageListState is null)
        {
            return;
        }

        // NOTE: any handlers added here should be removed in DetachStateImageListHandlers
        _imageListState.RecreateHandle += StateImageListRecreateHandle;
        _imageListState.Disposed += DetachImageList;
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
    ///  being updated. Always call EndUpdate immediately after the last item is updated.
    /// </summary>
    public void BeginUpdate()
    {
        BeginUpdateInternal();

        // If this is the first BeginUpdate call, push an ArrayList into the PropertyStore so
        // we can cache up any items that have been added while this is active.
        if (_updateCounter++ == 0 && !Properties.ContainsKey(s_propDelayedUpdateItems))
        {
            Properties.AddValue(s_propDelayedUpdateItems, new List<ListViewItem>());
        }
    }

    internal void CacheSelectedStateForItem(ListViewItem lvi, bool selected)
    {
        if (selected)
        {
            _savedSelectedItems ??= [];

            if (!_savedSelectedItems.Contains(lvi))
            {
                _savedSelectedItems.Add(lvi);
            }
        }
        else
        {
            if (_savedSelectedItems is not null && _savedSelectedItems.Contains(lvi))
            {
                _savedSelectedItems.Remove(lvi);
            }
        }
    }

    private void CancelPendingLabelEdit()
    {
        // Invoke the timer that was already set, this will cause label editing to start (LVN.BEGINLABELEDITW will be sent).
        // Using _blockLabelEdit will cancel label editing in LVN.BEGINLABELEDITW handler.
        _blockLabelEdit = true;
        try
        {
            PInvokeCore.SendMessage(this, PInvokeCore.WM_TIMER, (WPARAM)LVLABELEDITTIMER);
        }
        finally
        {
            _blockLabelEdit = false;
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
        if (_bkImgFileNames is null)
        {
            return;
        }

        FileInfo fi;
        for (int i = 0; i <= _bkImgFileNamesCount; i++)
        {
            string? bkImgFileName = _bkImgFileNames[i];
            if (bkImgFileName is null)
            {
                continue;
            }

            fi = new FileInfo(bkImgFileName);
            if (fi.Exists)
            {
                // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                // I could not find any resources which explain in detail when the IImgCtx objects
                // release the temporary file. So if we get a FileIO when we delete the temporary file
                // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                try
                {
                    fi.Delete();
                }
                catch (IOException) { }
            }
        }

        _bkImgFileNames = null;
        _bkImgFileNamesCount = -1;
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
    private int CompareFunc(nint lparam1, nint lparam2, nint lparamSort)
    {
        Debug.Assert(_listItemSorter is not null, "null sorter!");
        if (_listItemSorter is not null)
        {
            _listItemsTable.TryGetValue((int)lparam1, out ListViewItem? x);
            _listItemsTable.TryGetValue((int)lparam2, out ListViewItem? y);
            return _listItemSorter.Compare(x, y);
        }
        else
        {
            return 0;
        }
    }

    private unsafe int CompensateColumnHeaderResize(Message m, bool columnResizeCancelled)
    {
        if (Application.ComCtlSupportsVisualStyles
            && View == View.Details
            && !columnResizeCancelled
            && Items.Count > 0)
        {
            NMHEADERW* header = (NMHEADERW*)(nint)m.LParamInternal;
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
                ColumnHeader? col = (_columnHeaders is not null && _columnHeaders.Length > 0) ? _columnHeaders[0] : null;
                if (col is not null)
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

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_LISTVIEW_CLASSES
            });
        }

        base.CreateHandle();

        if (BackgroundImage is not null)
        {
            SetBackgroundImage();
        }
    }

    /// <summary>
    ///  Handles custom drawing of list items - for individual item font/color changes.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///    If OwnerDraw is true, we fire the OnDrawItem and OnDrawSubItem (in Details view)
    ///    events and let the user do the drawing.
    ///  </para>
    /// </remarks>
    private unsafe void CustomDraw(ref Message m)
    {
        bool dontmess = false;
        bool itemDrawDefault = false;

        try
        {
            NMLVCUSTOMDRAW* nmcd = (NMLVCUSTOMDRAW*)(nint)m.LParamInternal;
            // Find out which stage we're drawing
            switch (nmcd->nmcd.dwDrawStage)
            {
                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_PREPAINT:
                    if (OwnerDraw)
                    {
                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYITEMDRAW;
                        return;
                    }

                    // We want custom draw for this paint cycle
                    m.ResultInternal = (LRESULT)(nint)(PInvoke.CDRF_NOTIFYSUBITEMDRAW | PInvoke.CDRF_NEWFONT);

                    // refresh the cache of the current color & font settings for this paint cycle
                    _odCacheBackColor = BackColor;
                    _odCacheForeColor = ForeColor;
                    _odCacheFont = Font;
                    _odCacheFontHandle = FontHandle;

                    // If preparing to paint a group item, make sure its bolded.
                    if (nmcd->dwItemType == NMLVCUSTOMDRAW_ITEM_TYPE.LVCDI_GROUP)
                    {
                        _odCacheFontHandleWrapper?.Dispose();

                        _odCacheFont = new Font(_odCacheFont, FontStyle.Bold);
                        _odCacheFontHandleWrapper = new FontHandleWrapper(_odCacheFont);
                        _odCacheFontHandle = _odCacheFontHandleWrapper.Handle;
                        PInvokeCore.SelectObject(nmcd->nmcd.hdc, _odCacheFontHandleWrapper.Handle);
                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NEWFONT;
                    }

                    return;

                // We have to return a NOTIFYSUBITEMDRAW (called NOTIFYSUBITEMREDRAW in the docs) here to
                // get it to enter "change all subitems instead of whole rows" mode.

                // HOWEVER... we only want to do this for report styles...

                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT:

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
                        DrawListViewItemEventArgs e = new(
                            g,
                            Items[(int)nmcd->nmcd.dwItemSpec],
                            itemBounds,
                            (int)nmcd->nmcd.dwItemSpec,
                            (ListViewItemStates)nmcd->nmcd.uItemState);

                        OnDrawItem(e);

                        itemDrawDefault = e.DrawDefault;

                        // For the Details view, we send a SKIPDEFAULT when we get a sub-item drawing notification.
                        // For other view styles, we do it here.
                        if (_viewStyle == View.Details)
                        {
                            m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYSUBITEMDRAW;
                        }
                        else
                        {
                            if (!e.DrawDefault)
                            {
                                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                            }
                        }

                        if (!e.DrawDefault)
                        {
                            return;   // skip our regular drawing code
                        }
                    }

                    if (_viewStyle is View.Details or View.Tile)
                    {
                        m.ResultInternal = (LRESULT)(nint)(PInvoke.CDRF_NOTIFYSUBITEMDRAW | PInvoke.CDRF_NEWFONT);
                        dontmess = true; // don't mess with our return value!

                        // ITEMPREPAINT is used to work out the rect for the first column!!! GAH!!!
                        // (which means we can't just do our color/font work on SUBITEM|ITEM_PREPAINT)
                        // so fall through... and tell the end of SUBITEM|ITEM_PREPAINT not to mess
                        // with our return value...
                    }

                    // If it's not a report, we fall through and change the main item's styles

                    goto case (NMCUSTOMDRAW_DRAW_STAGE.CDDS_SUBITEM | NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT);

                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_SUBITEM | NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT:

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
                        DrawListViewSubItemEventArgs? e = null;

                        // by default, we want to skip the customDrawCode
                        bool skipCustomDrawCode = true;

                        // The ListView will send notifications for every column, even if no
                        // corresponding subitem exists for a particular item. We shouldn't
                        // fire events in such cases.
                        if (nmcd->iSubItem < Items[itemIndex].SubItems.Count)
                        {
                            Rectangle subItemBounds = GetSubItemRect(itemIndex, nmcd->iSubItem);

                            // For the first sub-item, the rectangle corresponds to the whole item.
                            // We need to handle this case separately.
                            if (nmcd->iSubItem == 0 && Items[itemIndex].SubItems.Count > 1)
                            {
                                // Use the width for the first column header.
                                if (_columnHeaders is not null)
                                {
                                    subItemBounds.Width = _columnHeaders[0].Width;
                                }
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
                                    _columnHeaders![nmcd->iSubItem],
                                    (ListViewItemStates)nmcd->nmcd.uItemState);
                                OnDrawSubItem(e);

                                // the customer still wants to draw the default.
                                // Don't skip the custom draw code then
                                skipCustomDrawCode = !e.DrawDefault;
                            }
                        }

                        if (skipCustomDrawCode)
                        {
                            m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                            return; // skip our custom draw code
                        }
                    }

                    // get the node
                    ListViewItem item = Items[(int)nmcd->nmcd.dwItemSpec];
                    // if we're doing the whole row in one style, change our result!
                    if (dontmess && item.UseItemStyleForSubItems)
                    {
                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NEWFONT;
                    }

                    Debug.Assert(item is not null, "Item was null in ITEMPREPAINT");

                    NMCUSTOMDRAW_DRAW_STATE_FLAGS state = nmcd->nmcd.uItemState;
                    // There is a known and documented problem in the ListView winctl control -
                    // if the LVS_SHOWSELALWAYS style is set, then the item state will have
                    // the CDIS_SELECTED bit set for all items. So we need to verify with the
                    // real item state to be sure.
                    if (!HideSelection)
                    {
                        LIST_VIEW_ITEM_STATE_FLAGS realState = GetItemState((int)nmcd->nmcd.dwItemSpec);
                        if ((realState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED) == 0)
                        {
                            state &= ~NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED;
                        }
                    }

                    // subitem is invalid if the flag isn't set -- and we also use this code in
                    // cases where subitems aren't visible (ie. non-Details modes), so if subitem
                    // is invalid, point it at the main item's render info

                    int subitem = ((nmcd->nmcd.dwDrawStage & NMCUSTOMDRAW_DRAW_STAGE.CDDS_SUBITEM) != 0) ? nmcd->iSubItem : 0;

                    // Work out the style in which to render this item
                    Font? subItemFont = null;
                    Color subItemForeColor = Color.Empty;
                    Color subItemBackColor = Color.Empty;
                    bool haveRenderInfo = false;
                    bool disposeSubItemFont = false;
                    if (item is not null && subitem < item.SubItems.Count)
                    {
                        haveRenderInfo = true;
                        if (subitem == 0 && (state & NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT) != 0 && HotTracking)
                        {
                            disposeSubItemFont = true;
                            subItemFont = new Font(item.SubItems[0].Font, FontStyle.Underline);
                        }
                        else
                        {
                            subItemFont = item.SubItems[subitem].Font;
                        }

                        if (subitem > 0 ||
                            (state & (NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED | NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_GRAYED |
                            NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT | NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_DISABLED)) == 0)
                        {
                            // we only propagate colors if we're displaying things normally
                            // the user can override this method to do all kinds of other bad things if they
                            // want to though - but we don't support that.
                            subItemForeColor = item.SubItems[subitem].ForeColor;
                            subItemBackColor = item.SubItems[subitem].BackColor;
                        }
                    }

                    // We always have to set font and color data, because of comctl design

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
                    else if (_activation is ItemActivation.OneClick or ItemActivation.TwoClick)
                    {
                        if ((state & (NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED
                                    | NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_GRAYED
                                    | NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT
                                    | NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_DISABLED)) != 0)
                        {
                            changeColor = false;
                        }
                    }

                    if (changeColor)
                    {
                        nmcd->clrText = !haveRenderInfo || riFore.IsEmpty
                            ? (COLORREF)ColorTranslator.ToWin32(_odCacheForeColor)
                            : (COLORREF)ColorTranslator.ToWin32(riFore);

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
                                int color = (int)(nmcd->clrText & mask);
                                if (color != 0 || (mask == 0x0000FF)) // The color is not 0
                                // or this is the last option
                                {
                                    int n = 16 - totalshift;
                                    // Make sure the value doesn't overflow
                                    color = color == mask ? ((color >> n) - 1) << n : ((color >> n) + 1) << n;

                                    // Copy the adjustment into nmcd->clrText
                                    nmcd->clrText = (COLORREF)((int)(nmcd->clrText & (~mask)) | color);
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
                            }
                            while (!clrAdjusted);
                        }

                        nmcd->clrTextBk = !haveRenderInfo || riBack.IsEmpty
                            ? (COLORREF)ColorTranslator.ToWin32(_odCacheBackColor)
                            : (COLORREF)ColorTranslator.ToWin32(riBack);
                    }

                    if (!haveRenderInfo || subItemFont is null)
                    {
                        // safety net code just in case
                        if (_odCacheFont is not null)
                        {
                            PInvokeCore.SelectObject(nmcd->nmcd.hdc, _odCacheFontHandle);
                        }
                    }
                    else
                    {
                        _odCacheFontHandleWrapper?.Dispose();

                        _odCacheFontHandleWrapper = new FontHandleWrapper(subItemFont);
                        PInvokeCore.SelectObject(nmcd->nmcd.hdc, _odCacheFontHandleWrapper.Handle);
                    }

                    if (!dontmess)
                    {
                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NEWFONT;
                    }

                    if (disposeSubItemFont)
                    {
                        subItemFont?.Dispose();
                    }

                    return;

                default:
                    m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
                    return;
            }
        }
        catch (Exception e)
        {
            Debug.Fail("Exception occurred attempting to setup custom draw. Disabling custom draw for this control", e.ToString());
            m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
        }
    }

    private static void DeleteFileName(string? fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            FileInfo fi = new(fileName);
            if (fi.Exists)
            {
                // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                // I could not find any resources which explain in detail when the IImgCtx objects
                // release the temporary file. So if we get a FileIO when we delete the temporary file
                // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                try
                {
                    fi.Delete();
                }
                catch (IOException) { }
            }
        }
    }

    /// <summary>
    ///  Resets the imageList to null. We wire this method up to the imageList's
    ///  Dispose event, so that we don't hang onto an imageList that's gone away.
    /// </summary>
    private void DetachImageList(object? sender, EventArgs e)
    {
        _listViewState1[LISTVIEWSTATE1_disposingImageLists] = true;
        try
        {
#if DEBUG
            if (sender is ImageList imageList && !imageList.IsDisposed &&
                sender != _imageListSmall && sender != _imageListState && sender != _imageListLarge && sender != _imageListGroup)
            {
                Debug.Fail("ListView sunk dispose event from unknown component");
            }
#endif
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
            _listViewState1[LISTVIEWSTATE1_disposingImageLists] = false;
        }

        UpdateListViewItemsLocations();
    }

    private void DetachGroupImageListHandlers()
    {
        if (_imageListGroup is null)
        {
            return;
        }

        _imageListGroup.RecreateHandle -= GroupImageListRecreateHandle;
        _imageListGroup.Disposed -= DetachImageList;
        _imageListGroup.ChangeHandle -= GroupImageListChangedHandle;
    }

    private void DetachLargeImageListHandlers()
    {
        if (_imageListLarge is null)
        {
            return;
        }

        _imageListLarge.RecreateHandle -= LargeImageListRecreateHandle;
        _imageListLarge.Disposed -= DetachImageList;
        _imageListLarge.ChangeHandle -= LargeImageListChangedHandle;
    }

    private void DetachSmallImageListListHandlers()
    {
        if (_imageListSmall is null)
        {
            return;
        }

        _imageListSmall.RecreateHandle -= SmallImageListRecreateHandle;
        _imageListSmall.Disposed -= DetachImageList;
    }

    private void DetachStateImageListHandlers()
    {
        if (_imageListState is null)
        {
            return;
        }

        _imageListState.RecreateHandle -= StateImageListRecreateHandle;
        _imageListState.Disposed -= DetachImageList;
    }

    /// <summary>
    ///  Disposes of the component. Call dispose when the component is no longer needed.
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
            if (_columnHeaders is not null)
            {
                for (int colIdx = _columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                {
                    _columnHeaders[colIdx].OwnerListview = null;
                    _columnHeaders[colIdx].Dispose();
                }

                _columnHeaders = null;
            }

            // We do not store data about items when the ListView is in virtual mode,
            // so "Unhook" method is only called for normal mode
            if (!VirtualMode)
            {
                Unhook();
            }

            using (DisposingContext context = new(this))
            {
                // Remove any items we have
                Items.Clear();
            }

            if (_odCacheFontHandleWrapper is not null)
            {
                _odCacheFontHandleWrapper.Dispose();
                _odCacheFontHandleWrapper = null;
            }

            if (!string.IsNullOrEmpty(_backgroundImageFileName) || _bkImgFileNames is not null)
            {
                FileInfo fi;
                if (!string.IsNullOrEmpty(_backgroundImageFileName))
                {
                    fi = new FileInfo(_backgroundImageFileName);
                    Debug.Assert(fi.Exists, "who deleted our temp file?");

                    // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                    // I could not find any resources which explain in detail when the IImgCtx objects
                    // release the temporary file. So if we get a FileIO when we delete the temporary file
                    // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                    try
                    {
                        fi.Delete();
                    }
                    catch (IOException) { }
                    _backgroundImageFileName = string.Empty;
                }

                for (int i = 0; i <= _bkImgFileNamesCount; i++)
                {
                    if (_bkImgFileNames is not null)
                    {
                        string? bkImgFileName = _bkImgFileNames[i];
                        if (bkImgFileName is null)
                        {
                            continue;
                        }

                        fi = new FileInfo(bkImgFileName);
                        Debug.Assert(fi.Exists, "who deleted our temp file?");

                        // ComCtl ListView uses COM objects to manipulate the bitmap we send it to them.
                        // I could not find any resources which explain in detail when the IImgCtx objects
                        // release the temporary file. So if we get a FileIO when we delete the temporary file
                        // we don't do anything about it ( because we don't know what is a good time to try to delete the file again ).
                        try
                        {
                            fi.Delete();
                        }
                        catch (IOException) { }
                    }
                }

                _bkImgFileNames = null;
                _bkImgFileNamesCount = -1;
            }

            KeyboardToolTip.Dispose();
        }

        base.Dispose(disposing);
    }

    private bool ClearingInnerListOnDispose
    {
        get => _listViewState1[LISTVIEWSTATE1_clearingInnerListOnDispose];
        set => _listViewState1[LISTVIEWSTATE1_clearingInnerListOnDispose] = value;
    }

    /// <summary>
    ///  Cancels the effect of BeginUpdate.
    /// </summary>
    public void EndUpdate()
    {
        // On the final EndUpdate, check to see if we've got any cached items.
        // If we do, insert them as normal, then turn off the painting freeze.
        if (--_updateCounter == 0 && Properties.ContainsKey(s_propDelayedUpdateItems))
        {
            ApplyUpdateCachedItems();
        }

        EndUpdateInternal();
    }

    private void EnsureDefaultGroup()
    {
        if (IsHandleCreated && GroupsEnabled)
        {
            if (PInvokeCore.SendMessage(this, PInvoke.LVM_HASGROUP, (WPARAM)DefaultGroup.ID) == 0)
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
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Items.Count);

        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_ENSUREVISIBLE, (WPARAM)index);
        }
    }

    public ListViewItem? FindItemWithText(string text) => Items.Count == 0
        ? null
        : FindItemWithText(text, includeSubItemsInSearch: true, startIndex: 0, isPrefixSearch: true);

    public ListViewItem? FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex) =>
        FindItemWithText(text, includeSubItemsInSearch, startIndex, isPrefixSearch: true);

    public ListViewItem? FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, Items.Count);

        return FindItem(true, text, isPrefixSearch, new Point(0, 0), SearchDirectionHint.Down, startIndex, includeSubItemsInSearch);
    }

    public ListViewItem? FindNearestItem(SearchDirectionHint dir, Point point)
    {
        return FindNearestItem(dir, point.X, point.Y);
    }

    public ListViewItem? FindNearestItem(SearchDirectionHint searchDirection, int x, int y)
    {
        if (View is not View.SmallIcon and not View.LargeIcon)
        {
            throw new InvalidOperationException(SR.ListViewFindNearestItemWorksOnlyInIconView);
        }

        if (searchDirection is < SearchDirectionHint.Left or > SearchDirectionHint.Down)
        {
            throw new ArgumentOutOfRangeException(nameof(searchDirection), searchDirection, string.Format(SR.InvalidArgument, nameof(searchDirection), searchDirection));
        }

        // the win32 ListView::FindNearestItem does some pretty weird things to determine the nearest item.
        // simply passing the (x,y) coordinates will cause problems when we call FindNearestItem for a point inside an item.
        // so we have to do some special processing when (x,y) falls inside an item;
        ListViewItem? lvi = GetItemAt(x, y);

        if (lvi is not null)
        {
            Rectangle itemBounds = lvi.Bounds;
            // LVM_FINDITEM is a nightmare
            // LVM_FINDITEM will use the top left corner of icon rectangle to determine the closest item
            // What happens if there is no icon for this item? then the top left corner of the icon rectangle
            // falls INSIDE the item label (???)

            Rectangle iconBounds = GetItemRect(lvi.Index, ItemBoundsPortion.Icon);

            switch (searchDirection)
            {
                case SearchDirectionHint.Up:
                    x = Math.Max(itemBounds.Left, iconBounds.Left);
                    y = Math.Max(itemBounds.Top, iconBounds.Top) - 1;
                    break;
                case SearchDirectionHint.Down:
                    x = Math.Max(itemBounds.Left, iconBounds.Left);
                    y = Math.Max(itemBounds.Top, iconBounds.Top) + 1;
                    break;
                case SearchDirectionHint.Left:
                    x = Math.Max(itemBounds.Left, iconBounds.Left) - 1;
                    y = Math.Max(itemBounds.Top, iconBounds.Top);
                    break;
                case SearchDirectionHint.Right:
                    x = Math.Max(itemBounds.Left, iconBounds.Left) + 1;
                    y = Math.Max(itemBounds.Top, iconBounds.Top);
                    break;
                default:
                    Debug.Assert(false, "these are all the search directions");
                    break;
            }
        }

        return FindItem(false, string.Empty, false, new Point(x, y), searchDirection, 0, false);
    }

    private unsafe ListViewItem? FindItem(bool isTextSearch, string text, bool isPrefixSearch, Point pt, SearchDirectionHint dir, int startIndex, bool includeSubItemsInSearch)
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
            SearchForVirtualItemEventArgs sviEvent = new(isTextSearch, isPrefixSearch, includeSubItemsInSearch, text, pt, dir, startIndex);

            OnSearchForVirtualItem(sviEvent);
            // NOTE: this will cause a RetrieveVirtualItem event w/o a corresponding cache hint event.
            return sviEvent.Index != -1 ? Items[sviEvent.Index] : null;
        }
        else
        {
            fixed (char* pText = text)
            {
                LVFINDINFOW lvFindInfo = default;
                if (isTextSearch)
                {
                    lvFindInfo.flags = LVFINDINFOW_FLAGS.LVFI_STRING;
                    lvFindInfo.flags |= isPrefixSearch ? LVFINDINFOW_FLAGS.LVFI_PARTIAL : 0;
                    lvFindInfo.psz = pText;
                }
                else
                {
                    lvFindInfo.flags = LVFINDINFOW_FLAGS.LVFI_NEARESTXY;
                    lvFindInfo.pt = pt;
                    // we can do this because SearchDirectionHint is set to the VK_*
                    lvFindInfo.vkDirection = (uint)dir;
                }

                lvFindInfo.lParam = 0;
                int index = (int)PInvokeCore.SendMessage(
                    this,
                    PInvoke.LVM_FINDITEMW,
                    // decrement startIndex so that the search is 0-based
                    (WPARAM)(startIndex - 1),
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
                        ListViewItem listViewItem = Items[i];
                        for (int j = 0; j < listViewItem.SubItems.Count; j++)
                        {
                            ListViewSubItem listViewSubItem = listViewItem.SubItems[j];
                            // the win32 list view search for items w/ text is case insensitive
                            // do the same for sub items
                            // because we are comparing user defined strings we have to do the slower String search
                            // ie, use String.Compare(string, string, case sensitive, CultureInfo)
                            // instead of new Whidbey String.Equals overload
                            // String.Equals(string, string, StringComparison.OrdinalIgnoreCase
                            if (string.Equals(text, listViewSubItem.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                return listViewItem;
                            }

                            if (isPrefixSearch && CultureInfo.CurrentCulture.CompareInfo.IsPrefix(listViewSubItem.Text, text, CompareOptions.IgnoreCase))
                            {
                                return listViewItem;
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
        if (CheckBoxes && IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETEXTENDEDLISTVIEWSTYLE, (WPARAM)PInvoke.LVS_EX_CHECKBOXES);
            PInvokeCore.SendMessage(
                this,
                PInvoke.LVM_SETEXTENDEDLISTVIEWSTYLE,
                (WPARAM)PInvoke.LVS_EX_CHECKBOXES,
                (LPARAM)PInvoke.LVS_EX_CHECKBOXES);

            // Comctl should handle auto-arrange for us, but doesn't.
            if (AutoArrange)
            {
                ArrangeIcons(Alignment);
            }
        }
    }

    // IDs for identifying ListViewItem's
    private int GenerateUniqueID()
    {
        // Okay, if someone adds several billion items to the list and doesn't remove all of them,
        // we can reuse the same ID, but I'm willing to take that risk. We are even tracking IDs
        // on a per-list view basis to reduce the problem.
        int result = _nextID++;
        if (result == -1)
        {
            // leave -1 as a "no such value" ID
            result = 0;
            _nextID = 1;
        }

        return result;
    }

    /// <summary>
    ///  Gets the real index for the given item. lastIndex is the last return
    ///  value from GetDisplayIndex, or -1 if you don't know. If provided,
    ///  the search for the index can be greatly improved.
    /// </summary>
    internal int GetDisplayIndex(ListViewItem item, int lastIndex)
    {
        Debug.Assert(item._listView == this, "Can't GetDisplayIndex if the list item doesn't belong to us");
        Debug.Assert(item._id != -1, "ListViewItem has no ID yet");

        ApplyUpdateCachedItems();
        if (IsHandleCreated && !ListViewHandleDestroyed)
        {
            LVFINDINFOW info = new()
            {
                lParam = item._id,
                flags = LVFINDINFOW_FLAGS.LVFI_PARAM
            };

            int displayIndex = -1;

            if (lastIndex != -1)
            {
                displayIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_FINDITEMW, (WPARAM)(lastIndex - 1), ref info);
            }

            if (displayIndex == -1)
            {
                displayIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_FINDITEMW, (WPARAM)(-1) /* beginning */, ref info);
            }

            Debug.Assert(displayIndex != -1, "This item is in the list view -- why can't we find a display index for it?");
            return displayIndex;
        }
        else
        {
            // PERF: The only reason we should ever call this before the handle is created
            // is if the user calls ListViewItem.Index.
            Debug.Assert(_listViewItems is not null, "listItemsArray is null, but the handle isn't created");

            int index = 0;
            foreach (ListViewItem listViewItem in _listViewItems)
            {
                if (listViewItem == item)
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
        if (_columnHeaders is null)
        {
            return -1;
        }

        for (int i = 0; i < _columnHeaders.Length; i++)
        {
            if (_columnHeaders[i] == ch)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///  Returns the current ListViewItem corresponding to the specific
    ///  x,y co-ordinate.
    /// </summary>
    public ListViewItem? GetItemAt(int x, int y)
    {
        LVHITTESTINFO lvhi = new()
        {
            pt = new Point(x, y)
        };

        int displayIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_HITTEST, (WPARAM)0, ref lvhi);

        ListViewItem? li = null;
        if (displayIndex >= 0 &&
            ((lvhi.flags &
            (LVHITTESTINFO_FLAGS.LVHT_ONITEMICON | LVHITTESTINFO_FLAGS.LVHT_ONITEMLABEL | LVHITTESTINFO_FLAGS.LVHT_ABOVE)) != 0))
        {
            li = Items[displayIndex];
        }

        return li;
    }

    internal int GetNativeGroupId(ListViewItem item)
    {
        item.UpdateGroupFromName();

        if (item.Group is not null && Groups.Contains(item.Group))
        {
            return item.Group.ID;
        }
        else
        {
            EnsureDefaultGroup();
            return DefaultGroup.ID;
        }
    }

    internal override unsafe ToolInfoWrapper<Control> GetToolInfoWrapper(TOOLTIP_FLAGS flags, string? caption, ToolTip tooltip)
    {
        // The "ShowItemToolTips" flag is required so that when the user hovers over the ListViewItem,
        // their own tooltip is displayed, not the ListViewItem tooltip.
        // The second condition is necessary for the correct display of the keyboard tooltip,
        // since the logic of the external tooltip blocks its display
        bool isExternalTooltip = ShowItemToolTips && tooltip != KeyboardToolTip;
        ToolInfoWrapper<Control> wrapper = new(this, flags, isExternalTooltip ? null : caption);
        if (isExternalTooltip)
            wrapper.Info.lpszText = (char*)(-1);

        return wrapper;
    }

    internal void GetSubItemAt(int x, int y, out int iItem, out int iSubItem)
    {
        LVHITTESTINFO lvhi = new()
        {
            pt = new Point(x, y)
        };

        int index = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_SUBITEMHITTEST, (WPARAM)0, ref lvhi);
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
        Point position = default;
        PInvokeCore.SendMessage(this, PInvoke.LVM_GETITEMPOSITION, (WPARAM)index, ref position);
        return position;
    }

    internal LIST_VIEW_ITEM_STATE_FLAGS GetItemState(int index)
        => GetItemState(
            index,
            LIST_VIEW_ITEM_STATE_FLAGS.LVIS_FOCUSED | LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED |
            LIST_VIEW_ITEM_STATE_FLAGS.LVIS_CUT | LIST_VIEW_ITEM_STATE_FLAGS.LVIS_DROPHILITED |
            LIST_VIEW_ITEM_STATE_FLAGS.LVIS_OVERLAYMASK | LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK);

    internal LIST_VIEW_ITEM_STATE_FLAGS GetItemState(int index, LIST_VIEW_ITEM_STATE_FLAGS mask)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, VirtualMode ? VirtualListSize : _itemCount);

        Debug.Assert(IsHandleCreated, "How did we add items without a handle?");
        return (LIST_VIEW_ITEM_STATE_FLAGS)(uint)PInvokeCore.SendMessage(this, PInvoke.LVM_GETITEMSTATE, (WPARAM)index, (LPARAM)(uint)mask);
    }

    /// <summary>
    ///  Returns a list item's bounding rectangle, including subitems.
    /// </summary>
    public Rectangle GetItemRect(int index) => GetItemRect(index, 0);

    /// <summary>
    ///  Returns a specific portion of a list item's bounding rectangle.
    /// </summary>
    public Rectangle GetItemRect(int index, ItemBoundsPortion portion)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Items.Count);

        // Valid values are 0x0 to 0x3
        SourceGenerated.EnumValidator.Validate(portion, nameof(portion));

        if (View == View.Details && Columns.Count == 0)
        {
            return Rectangle.Empty;
        }

        RECT itemrect = new()
        {
            left = (int)portion
        };

        return PInvokeCore.SendMessage(this, PInvoke.LVM_GETITEMRECT, (WPARAM)index, ref itemrect) == 0
            ? throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                string.Format(SR.InvalidArgument, nameof(index), index))
            : (Rectangle)itemrect;
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

        RECT itemrect = new()
        {
            left = 0
        };

        return PInvokeCore.SendMessage(this, PInvoke.LVM_GETITEMRECT, (WPARAM)index, ref itemrect) == 0
            ? Rectangle.Empty
            : (Rectangle)itemrect;
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
        if (!SupportsListViewSubItems)
        {
            return Rectangle.Empty;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, Items.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(subItemIndex);

        if (View == View.Tile)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(subItemIndex, Items[itemIndex].SubItems.Count);
        }

        if (View == View.Details)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(subItemIndex, Columns.Count);
        }

        // valid values are 0x0 to 0x3
        SourceGenerated.EnumValidator.Validate(portion, nameof(portion));

        if (Columns.Count == 0)
        {
            return Rectangle.Empty;
        }

        RECT itemrect = new()
        {
            left = (int)portion,
            top = subItemIndex
        };

        return PInvokeCore.SendMessage(this, PInvoke.LVM_GETSUBITEMRECT, (WPARAM)itemIndex, ref itemrect) == 0
            ? throw new ArgumentOutOfRangeException(
                nameof(itemIndex),
                itemIndex,
                string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex))
            : (Rectangle)itemrect;
    }

    private void GroupImageListChangedHandle(object? sender, EventArgs e)
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

    private void GroupImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        nint handle = (GroupImageList is null) ? 0 : GroupImageList.Handle;
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_GROUPHEADER, handle);
    }

    public ListViewHitTestInfo HitTest(Point point) => HitTest(point.X, point.Y);

    public ListViewHitTestInfo HitTest(int x, int y)
    {
        if (!ClientRectangle.Contains(x, y))
        {
            return new ListViewHitTestInfo(hitItem: null, hitSubItem: null, hitLocation: ListViewHitTestLocations.None);
        }

        LVHITTESTINFO lvhi = new()
        {
            pt = new Point(x, y)
        };

        int iItem = SupportsListViewSubItems
            ? (int)PInvokeCore.SendMessage(this, PInvoke.LVM_SUBITEMHITTEST, (WPARAM)0, ref lvhi)
            : (int)PInvokeCore.SendMessage(this, PInvoke.LVM_HITTEST, (WPARAM)0, ref lvhi);

        ListViewItem? item = (iItem == -1) ? null : Items[iItem];
        ListViewHitTestLocations location;

        if (item is null && (LVHITTESTINFO_FLAGS.LVHT_ABOVE & lvhi.flags) == LVHITTESTINFO_FLAGS.LVHT_ABOVE)
        {
            location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & (int)lvhi.flags) | (int)ListViewHitTestLocations.AboveClientArea);
        }
        else if (item is not null && (LVHITTESTINFO_FLAGS.LVHT_ONITEMSTATEICON & lvhi.flags) == LVHITTESTINFO_FLAGS.LVHT_ONITEMSTATEICON)
        {
            location = (ListViewHitTestLocations)((MASK_HITTESTFLAG & (int)lvhi.flags) | (int)ListViewHitTestLocations.StateImage);
        }
        else
        {
            location = (ListViewHitTestLocations)lvhi.flags;
        }

        if (SupportsListViewSubItems && item is not null)
        {
            if (lvhi.iSubItem < item.SubItems.Count)
            {
                _listViewSubItem = item.SubItems[lvhi.iSubItem];
                return new ListViewHitTestInfo(item, _listViewSubItem, location);
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

    private void NotifyAboutGotFocus(ListViewItem listViewItem)
    {
        if (listViewItem is not null)
        {
            // We do not store data about items when the ListView is in virtual mode, so we need to execute
            // the "Hook" method just before displaying the tooltip.
            if (VirtualMode)
            {
                KeyboardToolTipStateMachine.Instance.Hook(listViewItem, KeyboardToolTip);
            }

            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(listViewItem);
        }
    }

    private void NotifyAboutLostFocus(ListViewItem? listViewItem)
    {
        if (listViewItem is not null)
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(listViewItem);

            // We do not store data about items when the ListView is in virtual mode, so we need to execute
            // the "Unhook" method just after hiding the tooltip.
            if (VirtualMode)
            {
                KeyboardToolTipStateMachine.Instance.Unhook(listViewItem, KeyboardToolTip);
            }
        }
    }

    private unsafe void InvalidateColumnHeaders()
    {
        if (_viewStyle == View.Details && IsHandleCreated)
        {
            HWND header = (HWND)PInvokeCore.SendMessage(this, PInvoke.LVM_GETHEADER);
            if (!header.IsNull)
            {
                PInvoke.InvalidateRect(new HandleRef<HWND>(this, header), lpRect: null, bErase: true);
            }
        }
    }

    internal void UpdateColumnHeaderCorrespondingListViewSubItemIndex()
    {
        if (_columnHeaders is null)
        {
            return;
        }

        for (int i = 0; i < _columnHeaders.Length; i++)
        {
            _columnHeaders[i]._correspondingListViewSubItemIndex = i;
        }
    }

    /// <summary>
    ///  Inserts a new Column into the ListView
    /// </summary>
    internal ColumnHeader InsertColumn(int index, ColumnHeader ch, bool refreshSubItems = true)
    {
        ArgumentNullException.ThrowIfNull(ch);

        if (ch.OwnerListview is not null)
        {
            throw new ArgumentException(string.Format(SR.OnlyOneControl, ch.Text), nameof(ch));
        }

        // In Tile view the ColumnHeaders collection is used for the Tile Information
        // recreate the handle in that case
        int idx = IsHandleCreated && View != View.Tile ? InsertColumnNative(index, ch) : index;

        // First column must be left aligned

        if (idx == -1)
        {
            throw new InvalidOperationException(SR.ListViewAddColumnFailed);
        }

        // Add the column to our internal array
        int columnCount = _columnHeaders is null ? 0 : _columnHeaders.Length;
        if (columnCount > 0)
        {
            ColumnHeader[] newHeaders = new ColumnHeader[columnCount + 1];
            if (columnCount > 0)
            {
                Array.Copy(_columnHeaders!, 0, newHeaders, 0, columnCount);
            }

            _columnHeaders = newHeaders;
        }
        else
        {
            _columnHeaders = new ColumnHeader[1];
        }

        if (idx < columnCount)
        {
            Array.Copy(_columnHeaders, idx, _columnHeaders, idx + 1, columnCount - idx);
        }

        _columnHeaders[idx] = ch;
        ch.OwnerListview = this;

        // in Tile view the ColumnHeaders collection is used for the Tile Information
        // recreate the handle in that case
        if (ch.ActualImageIndex_Internal != -1 && IsHandleCreated && View != View.Tile)
        {
            SetColumnInfo(LVCOLUMNW_MASK.LVCF_IMAGE, ch);
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

        UpdateColumnHeaderCorrespondingListViewSubItemIndex();

        return ch;
    }

    private unsafe int InsertColumnNative(int index, ColumnHeader ch)
    {
        LVCOLUMNW lvColumn = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_FMT | LVCOLUMNW_MASK.LVCF_TEXT | LVCOLUMNW_MASK.LVCF_WIDTH
        };

        if (ch.OwnerListview is not null && ch.ActualImageIndex_Internal != -1)
        {
            lvColumn.mask |= LVCOLUMNW_MASK.LVCF_IMAGE;
            lvColumn.iImage = ch.ActualImageIndex_Internal;
        }

        lvColumn.fmt = (LVCOLUMNW_FORMAT)ch.TextAlign;
        lvColumn.cx = ch.Width;

        fixed (char* columnHeaderText = ch.Text)
        {
            lvColumn.pszText = columnHeaderText;

            return (int)PInvokeCore.SendMessage(this, PInvoke.LVM_INSERTCOLUMNW, (WPARAM)index, ref lvColumn);
        }
    }

    // when the user adds a group, this helper method makes sure that all the items
    // in the list view are parented by a group - be it the DefaultGroup or some other group
    internal void InsertGroupInListView(int index, ListViewGroup group)
    {
        Debug.Assert(_groups is not null && _groups.Count > 0, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");
        Debug.Assert(group != DefaultGroup, "this method should be used only when the user adds a group, not when we add our own DefaultGroup");

        // the first time we add a group we have to group the items in the Default Group
        bool groupItems = (_groups.Count == 1) && GroupsEnabled;

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
                LVITEMW lvItem = new()
                {
                    iItem = item.Index,
                    mask = LIST_VIEW_ITEM_FLAGS.LVIF_GROUPID
                };

                PInvokeCore.SendMessage(this, PInvoke.LVM_GETITEMW, (WPARAM)0, ref lvItem);
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

        nint result = SendGroupMessage(group, PInvoke.LVM_INSERTGROUP, index, LVGROUP_MASK.LVGF_GROUPID);
        Debug.Assert(result != -1, "Failed to insert group");
    }

    /// <summary>
    ///  Inserts a new ListViewItem into the ListView. The item will be inserted
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
        if (_updateCounter > 0 && Properties.TryGetValue(s_propDelayedUpdateItems, out List<ListViewItem>? itemList))
        {
            // CheckHosting.
            if (checkHosting)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i]._listView is not null)
                    {
                        throw new ArgumentException(string.Format(SR.OnlyOneControl, items[i].Text), nameof(items));
                    }
                }
            }

            Debug.Assert(itemList is not null, "In Begin/EndUpdate with no delayed array!");
            itemList?.AddRange(items);

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
        for (int i = 0; i < items.Length; i++)
        {
            ListViewItem item = items[i];

            if (checkHosting && item._listView is not null)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text), nameof(items));
            }

            // create an ID..
            int itemID = GenerateUniqueID();
            Debug.Assert(!_listItemsTable.ContainsKey(itemID), "internal hash table inconsistent -- inserting item, but it's already in the hash table");
            _listItemsTable.Add(itemID, item);

            _itemCount++;
            item.Host(this, itemID, -1);

            // if there's no handle created, just ad them to our list items array.
            if (!IsHandleCreated)
            {
                Debug.Assert(_listViewItems is not null, "listItemsArray is null, but the handle isn't created");
                _listViewItems.Insert(displayIndex + i, item);
            }
        }

        // finally if the handle is created, do the actual add into the real list view
        //
        if (IsHandleCreated)
        {
            InsertItemsNative(displayIndex, items);
        }

        Invalidate();
        ArrangeIcons(_alignStyle);

        // Any newly added items should have the correct location.
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

        // Much more efficient to call the native insert with max + 1, than with max. The + 1
        // for the display index accounts for itemCount++ above.
        if (index == _itemCount - 1)
        {
            index++;
        }

        // Create and add the LVITEM
        int actualIndex = -1;
        IntPtr hGlobalColumns = IntPtr.Zero;
        int maxColumns = 0;
        _listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = true;

        try
        {
            // Set the count of items first.
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMCOUNT, (WPARAM)_itemCount);

            // Now add the items.
            for (int i = 0; i < items.Length; i++)
            {
                ListViewItem li = items[i];

                Debug.Assert(Items.Contains(li), "Make sure ListView.Items contains this item before adding the native LVITEM. Otherwise, custom-drawing may break.");

                LVITEMW lvItem = new()
                {
                    mask = LIST_VIEW_ITEM_FLAGS.LVIF_TEXT | LIST_VIEW_ITEM_FLAGS.LVIF_IMAGE |
                            LIST_VIEW_ITEM_FLAGS.LVIF_PARAM | LIST_VIEW_ITEM_FLAGS.LVIF_INDENT |
                            LIST_VIEW_ITEM_FLAGS.LVIF_COLUMNS,
                    iItem = index + i,
                    iImage = li.ImageIndexer.ActualIndex,
                    iIndent = li.IndentCount,
                    lParam = li._id,
                    cColumns = (uint)(_columnHeaders is not null ? Math.Min(MAXTILECOLUMNS, _columnHeaders.Length) : 0),
                };

                if (GroupsEnabled)
                {
                    lvItem.mask |= LIST_VIEW_ITEM_FLAGS.LVIF_GROUPID;
                    lvItem.iGroupId = GetNativeGroupId(li);

#if DEBUG
                    IntPtr result = PInvokeCore.SendMessage(this, PInvoke.LVM_ISGROUPVIEWENABLED);
                    Debug.Assert(result != IntPtr.Zero, "Groups not enabled");
                    result = PInvokeCore.SendMessage(this, PInvoke.LVM_HASGROUP, (WPARAM)lvItem.iGroupId);
                    Debug.Assert(result != IntPtr.Zero, $"Doesn't contain group id: {lvItem.iGroupId}");
#endif
                }

                // Make sure that our columns memory is big enough. If not, then realloc it.
                if (lvItem.cColumns > maxColumns || hGlobalColumns == IntPtr.Zero)
                {
                    if (hGlobalColumns != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(hGlobalColumns);
                    }

                    hGlobalColumns = Marshal.AllocHGlobal((int)(lvItem.cColumns * sizeof(int)));
                    maxColumns = (int)lvItem.cColumns;
                }

                // now build and copy in the column indexes.
                lvItem.puColumns = (uint*)hGlobalColumns;
                int[] columns = new int[lvItem.cColumns];
                for (int c = 0; c < lvItem.cColumns; c++)
                {
                    columns[c] = c + 1;
                }

                Marshal.Copy(columns, 0, (nint)lvItem.puColumns, (int)lvItem.cColumns);

                // Inserting an item into a ListView with checkboxes causes one or more
                // item check events to be fired for the newly added item.
                // Therefore, we disable the item check event handler temporarily.
                ItemCheckEventHandler? oldOnItemCheck = _onItemCheck;
                _onItemCheck = null;

                int insertIndex;

                try
                {
                    li.UpdateStateToListView(lvItem.iItem, ref lvItem, false);

                    fixed (char* pText = li.Text)
                    {
                        lvItem.pszText = pText;

                        insertIndex = (int)PInvokeCore.SendMessage(
                            this,
                            PInvoke.LVM_INSERTITEMW,
                            (WPARAM)0,
                            ref lvItem);
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
                    _onItemCheck = oldOnItemCheck;
                }

                if (insertIndex == -1)
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

            _listViewState1[LISTVIEWSTATE1_insertingItemsNatively] = false;
        }

        if (_listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped])
        {
            // SelectedIndexChanged event was delayed
            _listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
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

        if (_listViewState[LISTVIEWSTATE_inLabelEdit])
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

    private void LargeImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        nint handle = (LargeImageList is null) ? 0 : LargeImageList.Handle;
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_NORMAL, (LPARAM)handle);

        ForceCheckBoxUpdate();
    }

    private void LargeImageListChangedHandle(object? sender, EventArgs e)
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
            SetItemText(item.Index, 0 /*subItemIndex*/, item.Text);
        }
    }

    /// <summary>
    ///  Fires the afterLabelEdit event.
    /// </summary>
    protected virtual void OnAfterLabelEdit(LabelEditEventArgs e)
    {
        _onAfterLabelEdit?.Invoke(this, e);
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
        _hoveredAlready = false;
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
        ListViewItem? item = null;

        if (Items.Count > 0)
        {
            // APPCOMPAT
            // V1.* users implement virtualization by communicating directly to the native ListView
            // and by passing our virtualization implementation. In that case, the native list view may
            // have an item under the mouse even if our wrapper thinks the item count is 0.
            // And that may cause GetItemAt to throw an out of bounds exception.

            Point pos = Cursor.Position;
            pos = PointToClient(pos);
            item = GetItemAt(pos.X, pos.Y);
        }

        if (item != _prevHoveredItem && item is not null)
        {
            OnItemMouseHover(new ListViewItemMouseHoverEventArgs(item));
            _prevHoveredItem = item;
        }

        if (!_hoveredAlready)
        {
            base.OnMouseHover(e);
            _hoveredAlready = true;
        }

        ResetMouseEventArgs();
    }

    /// <summary>
    ///  Fires the beforeLabelEdit event.
    /// </summary>
    protected virtual void OnBeforeLabelEdit(LabelEditEventArgs e)
    {
        _onBeforeLabelEdit?.Invoke(this, e);
    }

    protected virtual void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
    {
        ((CacheVirtualItemsEventHandler?)Events[s_cacheVirtualItemsEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the <see cref="GroupCollapsedStateChanged"/> event.
    /// </summary>
    protected virtual void OnGroupCollapsedStateChanged(ListViewGroupEventArgs e)
    {
        if (IsAccessibilityObjectCreated && GroupsEnabled && e.GroupIndex >= 0 && e.GroupIndex < Groups.Count)
        {
            ListViewGroup listViewGroup = Groups[e.GroupIndex];
            // A fix for https://github.com/dotnet/winforms/issues/3269.
            // Unfortunately we cannot use RaiseAutomationEvent method here since the control does not respond to
            // CollapseState messages. Use RaiseAutomationNotification instead to announce a custom notification.
            // See https://docs.microsoft.com/dotnet/api/system.windows.forms.accessibleobject.raiseautomationnotification.
            AccessibilityObject.InternalRaiseAutomationNotification(
                Automation.AutomationNotificationKind.ActionCompleted,
                Automation.AutomationNotificationProcessing.CurrentThenMostRecent,
                listViewGroup.CollapsedState == ListViewGroupCollapsedState.Collapsed
                    ? string.Format(SR.ListViewGroupCollapsedStateName, listViewGroup.Header)
                    : string.Format(SR.ListViewGroupExpandedStateName, listViewGroup.Header));
        }

        ((EventHandler<ListViewGroupEventArgs>?)Events[s_groupCollapsedStateChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the columnClick event.
    /// </summary>
    protected virtual void OnColumnClick(ColumnClickEventArgs e)
    {
        _onColumnClick?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the <see cref="GroupTaskLinkClick"/> event.
    /// </summary>
    protected virtual void OnGroupTaskLinkClick(ListViewGroupEventArgs e)
    {
        ((EventHandler<ListViewGroupEventArgs>?)Events[s_groupTaskLinkClickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the column header rearranged event.
    /// </summary>
    protected virtual void OnColumnReordered(ColumnReorderedEventArgs e)
    {
        ((ColumnReorderedEventHandler?)Events[s_columnReorderedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the column width changing event.
    /// </summary>
    protected virtual void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
    {
        ((ColumnWidthChangedEventHandler?)Events[s_columnWidthChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the column width changing event.
    /// </summary>
    protected virtual void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
    {
        ((ColumnWidthChangingEventHandler?)Events[s_columnWidthChangingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the DrawColumnHeader event.
    /// </summary>
    protected virtual void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
    {
        ((DrawListViewColumnHeaderEventHandler?)Events[s_drawColumnHeaderEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the DrawItem event.
    /// </summary>
    protected virtual void OnDrawItem(DrawListViewItemEventArgs e)
    {
        ((DrawListViewItemEventHandler?)Events[s_drawItemEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the DrawSubItem event.
    /// </summary>
    protected virtual void OnDrawSubItem(DrawListViewSubItemEventArgs e)
    {
        ((DrawListViewSubItemEventHandler?)Events[s_drawSubItemEvent])?.Invoke(this, e);
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
                PInvokeCore.SendMessage(this, PInvoke.LVM_UPDATE, (WPARAM)(-1));
            }
            finally
            {
                EndUpdate();
            }
        }

        // If font changes and we have headers, they need to be explicitly invalidated.
        InvalidateColumnHeaders();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        // don't persist flipViewToLargeIconAndSmallIcon across handle recreations...
        FlipViewToLargeIconAndSmallIcon = false;

        base.OnHandleCreated(e);

        int version = (int)PInvokeCore.SendMessage(this, PInvoke.CCM_GETVERSION);
        if (version < 5)
        {
            PInvokeCore.SendMessage(this, PInvoke.CCM_SETVERSION, (WPARAM)5);
        }

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        if (Application.IsDarkModeEnabled)
        {
            _ = PInvoke.SetWindowTheme(HWND, $"{DarkModeIdentifier}_{ExplorerThemeIdentifier}", null);

            // Get the ListView's ColumnHeader handle:
            HWND columnHeaderHandle = (HWND)PInvokeCore.SendMessage(this, PInvoke.LVM_GETHEADER, (WPARAM)0, (LPARAM)0);
            PInvoke.SetWindowTheme(columnHeaderHandle, $"{DarkModeIdentifier}_{ItemsViewThemeIdentifier}", null);
        }
#pragma warning restore WFO5001

        UpdateExtendedStyles();
        RealizeProperties();
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKCOLOR, (WPARAM)0, (LPARAM)BackColor);
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETTEXTCOLOR, (WPARAM)0, (LPARAM)ForeColor);

        // The native list view will not invalidate the entire list view item area if the BkColor is not CLR_NONE.
        // This not noticeable if the customer paints the items w/ the same background color as the list view itself.
        // However, if the customer paints the items w/ a color different from the list view's back color
        // then when the user changes selection the native list view will not invalidate the entire list view item area.
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETTEXTBKCOLOR, (WPARAM)0, (LPARAM)PInvokeCore.CLR_NONE);

        // LVS_NOSCROLL does not work well when the list view is in View.Details or in View.List modes.
        // we have to set this style after the list view was created and before we position the native list view items.
        if (!Scrollable)
        {
            int style = (int)PInvokeCore.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            style |= (int)PInvoke.LVS_NOSCROLL;
            PInvokeCore.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
        }

        // In VirtualMode we have to tell the list view to ask for the list view item's state image index.
        if (VirtualMode)
        {
            LIST_VIEW_ITEM_STATE_FLAGS callbackMask = (LIST_VIEW_ITEM_STATE_FLAGS)(int)PInvokeCore.SendMessage(this, PInvoke.LVM_GETCALLBACKMASK);
            callbackMask |= LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK;
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETCALLBACKMASK, (WPARAM)(uint)callbackMask);
        }

        if (Application.ComCtlSupportsVisualStyles)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETVIEW, (WPARAM)(uint)_viewStyle);
            UpdateGroupView();

            // Add groups.
            if (_groups is not null)
            {
                for (int index = 0; index < _groups.Count; index++)
                {
                    InsertGroupNative(index, _groups[index]);
                }
            }

            // Set tile view settings.
            if (_viewStyle == View.Tile)
            {
                UpdateTileView();
            }
        }

        ListViewHandleDestroyed = false;

        // Use a copy of the list items array so that we can maintain the (handle created || listItemsArray is not null) invariant
        ListViewItem[]? listViewItemsToAdd = null;
        if (_listViewItems is not null)
        {
            listViewItemsToAdd = [.. _listViewItems];
            _listViewItems = null;
        }

        int columnCount = _columnHeaders is null ? 0 : _columnHeaders.Length;
        if (columnCount > 0)
        {
            int[] indices = new int[columnCount];
            int index = 0;
            foreach (ColumnHeader column in _columnHeaders!)
            {
                indices[index] = column.DisplayIndex;
                InsertColumnNative(index++, column);
            }

            SetDisplayIndices(indices);
        }

        // Make sure that we're not in a begin/end update call.
        if (_itemCount > 0 && listViewItemsToAdd is not null)
        {
            InsertItemsNative(0, listViewItemsToAdd);
        }

        if (VirtualMode && VirtualListSize > -1 && !DesignMode)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMCOUNT, (WPARAM)VirtualListSize);
        }

        if (columnCount > 0)
        {
            UpdateColumnWidths(ColumnHeaderAutoResizeStyle.None);
        }

        ArrangeIcons(_alignStyle);
        UpdateListViewItemsLocations();

        if (!VirtualMode)
        {
            Sort();
        }

        if (Application.ComCtlSupportsVisualStyles && (InsertionMark.Index > 0))
        {
            InsertionMark.UpdateListView();
        }

        // When the handle is recreated, update the SavedCheckedItems.
        // It is possible some checked items were added to the list view while its handle was null.
        _savedCheckedItems = null;
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
        // don't save the list view items state when in virtual mode : it is the responsibility of the
        // user to cache the list view items in virtual mode
        if (!Disposing && !VirtualMode)
        {
            int count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                Items[i].UpdateStateFromListView(i, true);
            }

            // Save away the selected and checked items
            if (SelectedItems is not null && !VirtualMode)
            {
                // Create an array because the SelectedItems collection is tuned for CopyTo()
                ListViewItem[] lviArr = new ListViewItem[SelectedItems.Count];
                SelectedItems.CopyTo(lviArr, 0);
                _savedSelectedItems = new List<ListViewItem>(lviArr.Length);
                for (int i = 0; i < lviArr.Length; i++)
                {
                    _savedSelectedItems.Add(lviArr[i]);
                }
            }

            Debug.Assert(_listViewItems is null, "listItemsArray not null, even though handle created");
            ListViewItemCollection tempItems = Items;

            var items = new ListViewItem[tempItems.Count];
            tempItems.CopyTo(items, 0);

            _listViewItems = [.. items];

            ListViewHandleDestroyed = true;
        }

        base.OnHandleDestroyed(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (ClearingInnerListOnDispose)
        {
            return;
        }

        base.OnGotFocus(e);

        if (ShowItemToolTips && Items.Count > 0 && (FocusedItem ?? Items[0]) is ListViewItem focusedItem)
        {
            NotifyAboutGotFocus(focusedItem);
        }

        if (IsHandleCreated &&
            IsAccessibilityObjectCreated &&
            AccessibilityObject.GetFocus() is AccessibleObject focusedAccessibleObject)
        {
            focusedAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        NotifyAboutLostFocus(FocusedItem);
        base.OnLostFocus(e);
    }

    /// <summary>
    ///  Fires the itemActivate event.
    /// </summary>
    protected virtual void OnItemActivate(EventArgs e)
    {
        _onItemActivate?.Invoke(this, e);
    }

    /// <summary>
    ///  This is the code that actually fires the KeyEventArgs. Don't
    ///  forget to call base.onItemCheck() to ensure that itemCheck vents
    ///  are correctly fired for all other keys.
    /// </summary>
    protected virtual void OnItemCheck(ItemCheckEventArgs ice)
    {
        _onItemCheck?.Invoke(this, ice);
    }

    protected virtual void OnItemChecked(ItemCheckedEventArgs e)
    {
        _onItemChecked?.Invoke(this, e);

        if (!CheckBoxes)
        {
            return;
        }

        if (e.Item.ListView == this && IsAccessibilityObjectCreated)
        {
            ListViewItem item = e.Item;
            ToggleState oldValue = item.Checked ? ToggleState.ToggleState_Off : ToggleState.ToggleState_On;
            ToggleState newValue = item.Checked ? ToggleState.ToggleState_On : ToggleState.ToggleState_Off;
            item.AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_ToggleToggleStatePropertyId, (VARIANT)(int)oldValue, (VARIANT)(int)newValue);
        }
    }

    protected virtual void OnItemDrag(ItemDragEventArgs e)
    {
        _onItemDrag?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the ItemMouseHover event.
    /// </summary>
    protected virtual void OnItemMouseHover(ListViewItemMouseHoverEventArgs e)
    {
        _onItemMouseHover?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires the ItemSelectionChanged event.
    /// </summary>
    protected virtual void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
    {
        ((ListViewItemSelectionChangedEventHandler?)Events[s_itemSelectionChangedEvent])?.Invoke(this, e);
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
        ((RetrieveVirtualItemEventHandler?)Events[s_retrieveVirtualItemEvent])?.Invoke(this, e);
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

        if (Events[s_rightToLeftLayoutChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Fires the search for virtual item event.
    /// </summary>
    protected virtual void OnSearchForVirtualItem(SearchForVirtualItemEventArgs e)
    {
        ((SearchForVirtualItemEventHandler?)Events[s_searchForVirtualItemEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Actually goes and fires the selectedIndexChanged event. Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event]. They should,
    ///  however, remember to call base.onSelectedIndexChanged(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_selectedIndexChangedEvent])?.Invoke(this, e);

        if (SelectedIndices.Count == 0)
        {
            return;
        }

        ListViewItem firstSelectedItem = Items[SelectedIndices[0]];

        // The second condition is necessary to avoid unexpected switch of the Inspect's focus
        // when the user clicks on the ListViewSubItem. This is due to the fact that the "OnSelectedIndexChanged"
        // and "WmMouseDown" methods simultaneously send message about the selected item.
        if (firstSelectedItem.Focused && _selectedItem != firstSelectedItem)
        {
            _selectedItem = firstSelectedItem;
            if (IsAccessibilityObjectCreated)
            {
                firstSelectedItem.AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKCOLOR, (WPARAM)0, (LPARAM)BackColor);

            // We should probably be OK if we don't set the TEXTBKCOLOR to CLR_NONE.
            // However, for the sake of being more robust, reset the TECTBKCOLOR to CLR_NONE when the system palette changes.
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETTEXTBKCOLOR, (WPARAM)0, (LPARAM)PInvokeCore.CLR_NONE);
        }
    }

    protected virtual void OnVirtualItemsSelectionRangeChanged(ListViewVirtualItemsSelectionRangeChangedEventArgs e)
    {
        ((ListViewVirtualItemsSelectionRangeChangedEventHandler?)Events[s_virtualItemSelectionRangeChangedEvent])?.Invoke(this, e);
    }

    private unsafe void PositionHeader()
    {
        HWND headerWindow = PInvoke.GetWindow(this, GET_WINDOW_CMD.GW_CHILD);
        if (!headerWindow.IsNull)
        {
            WINDOWPOS position = default;
            PInvokeCore.GetClientRect(this, out RECT clientRect);
            HDLAYOUT hd = new()
            {
                prc = &clientRect,
                pwpos = &position
            };

            // Get the layout information.
            PInvokeCore.SendMessage(headerWindow, PInvoke.HDM_LAYOUT, (WPARAM)0, ref hd);

            // Position the header control.
            PInvoke.SetWindowPos(
                headerWindow,
                position.hwndInsertAfter,
                position.x,
                position.y,
                position.cx,
                position.cy,
                position.flags | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);

            GC.KeepAlive(this);
        }
    }

    private void RealizeAllSubItems()
    {
        LVITEMW item = default;
        for (int i = 0; i < _itemCount; i++)
        {
            int subItemCount = Items[i].SubItems.Count;
            for (int j = 0; j < subItemCount; j++)
            {
                SetItemText(i, j, Items[i].SubItems[j].Text, ref item);
            }
        }
    }

    protected void RealizeProperties()
    {
        // Realize state information
        Color c;

        c = BackColor;
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        if (c != SystemColors.Window || Application.IsDarkModeEnabled)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKCOLOR, (WPARAM)0, (LPARAM)c);
        }

        c = ForeColor;

        if (c != SystemColors.WindowText || Application.IsDarkModeEnabled)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETTEXTCOLOR, (WPARAM)0, (LPARAM)c);
        }
#pragma warning restore WFO5001

        if (_imageListLarge is not null)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_NORMAL, (LPARAM)_imageListLarge.Handle);
        }

        if (_imageListSmall is not null)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_SMALL, (LPARAM)_imageListSmall.Handle);
        }

        if (_imageListState is not null)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_STATE, (LPARAM)_imageListState.Handle);
        }

        if (_imageListGroup is not null)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_GROUPHEADER, (LPARAM)_imageListGroup.Handle);
        }
    }

    /// <summary>
    ///  Forces the redraw of a range of listview items.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void RedrawItems(int startIndex, int endIndex, bool invalidateOnly)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(endIndex);

        int maxSize = VirtualMode ? VirtualListSize : Items.Count;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, maxSize);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(endIndex, maxSize);

        if (startIndex > endIndex)
        {
            throw new ArgumentException(SR.ListViewStartIndexCannotBeLargerThanEndIndex);
        }

        if (IsHandleCreated)
        {
            int retval = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_REDRAWITEMS, (WPARAM)startIndex, (LPARAM)endIndex);
            Debug.Assert(retval != 0);

            // ListView control seems to be bogus. Items affected need to be invalidated in LargeIcon and SmallIcons views.
            if (View is View.LargeIcon or View.SmallIcon)
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

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (!OsVersion.IsWindows8OrGreater())
        {
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            Items.GetItemByIndex(i)?.ReleaseUiaProvider();
        }

        if (_defaultGroup is not null)
        {
            DefaultGroup.ReleaseUiaProvider();
        }

        foreach (ListViewGroup group in Groups)
        {
            group.ReleaseUiaProvider();
        }

        foreach (ColumnHeader columnHeader in Columns)
        {
            columnHeader.ReleaseUiaProvider();
        }

        base.ReleaseUiaProvider(handle);
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
    ///  Does the job of telling win32 listView to remove this group
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   It is the job of whoever deletes this group to also turn off grouping if this was the last group deleted
    ///  </para>
    /// </remarks>
    private void RemoveGroupNative(ListViewGroup group)
    {
        Debug.Assert(IsHandleCreated, "RemoveGroupNative precondition: list-view handle must be created");
        PInvokeCore.SendMessage(this, PInvoke.LVM_REMOVEGROUP, (WPARAM)group.ID);
    }

    private void Scroll(int fromLVItem, int toLVItem)
    {
        int scrollY = GetItemPosition(toLVItem).Y - GetItemPosition(fromLVItem).Y;
        PInvokeCore.SendMessage(this, PInvoke.LVM_SCROLL, (WPARAM)0, (LPARAM)scrollY);
    }

    private unsafe void SetBackgroundImage()
    {
        // needed for OleInitialize
        Application.OleRequired();

        LVBKIMAGEW backgroundImage = default;

        // first, is there an existing temporary file to delete, remember its name
        // so that we can delete it if the list control doesn't...
        string fileNameToDelete = _backgroundImageFileName;

        if (BackgroundImage is not null)
        {
            // save the image to a temporary file name
            _backgroundImageFileName = Path.GetTempFileName();

            BackgroundImage.Save(_backgroundImageFileName, Drawing.Imaging.ImageFormat.Bmp);

            backgroundImage.cchImageMax = (uint)(_backgroundImageFileName.Length + 1);
            backgroundImage.ulFlags = LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_SOURCE_URL;
            if (BackgroundImageTiled)
            {
                backgroundImage.ulFlags |= LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_STYLE_TILE;
            }
            else
            {
                backgroundImage.ulFlags |= LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_STYLE_NORMAL;
            }
        }
        else
        {
            backgroundImage.ulFlags = LIST_VIEW_BACKGROUND_IMAGE_FLAGS.LVBKIF_SOURCE_NONE;
            _backgroundImageFileName = string.Empty;
        }

        fixed (char* pBackgroundImageFileName = _backgroundImageFileName)
        {
            backgroundImage.pszImage = pBackgroundImageFileName;
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETBKIMAGEW, (WPARAM)0, ref backgroundImage);
        }

        if (string.IsNullOrEmpty(fileNameToDelete))
        {
            return;
        }

        // we need to cause a paint message on the win32 list view. This way the win 32 list view gives up
        // its reference to the previous image file it was holding on to.

        // 8 strings should be good enough for us
        if (_bkImgFileNames is null)
        {
            _bkImgFileNames = new string[BKIMGARRAYSIZE];
            _bkImgFileNamesCount = -1;
        }

        if (_bkImgFileNamesCount == BKIMGARRAYSIZE - 1)
        {
            // it should be fine to delete the file name that was added first.
            // if it's not fine, then increase BKIMGARRAYSIZE
            DeleteFileName(_bkImgFileNames[0]);
            _bkImgFileNames[0] = _bkImgFileNames[1];
            _bkImgFileNames[1] = _bkImgFileNames[2];
            _bkImgFileNames[2] = _bkImgFileNames[3];
            _bkImgFileNames[3] = _bkImgFileNames[4];
            _bkImgFileNames[4] = _bkImgFileNames[5];
            _bkImgFileNames[5] = _bkImgFileNames[6];
            _bkImgFileNames[6] = _bkImgFileNames[7];
            _bkImgFileNames[7] = null;

            _bkImgFileNamesCount--;
        }

        _bkImgFileNamesCount++;
        _bkImgFileNames[_bkImgFileNamesCount] = fileNameToDelete;

        // now force the paint
        Refresh();
    }

    internal unsafe void SetColumnInfo(LVCOLUMNW_MASK mask, ColumnHeader ch)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        Debug.Assert((mask & ~(LVCOLUMNW_MASK.LVCF_FMT | LVCOLUMNW_MASK.LVCF_TEXT | LVCOLUMNW_MASK.LVCF_IMAGE)) == 0, "Unsupported mask in setColumnInfo");
        LVCOLUMNW lvColumn = new LVCOLUMNW
        {
            mask = mask
        };

        if ((mask & LVCOLUMNW_MASK.LVCF_IMAGE) != 0 || (mask & LVCOLUMNW_MASK.LVCF_FMT) != 0)
        {
            // When we set the ImageIndex we also have to alter the column format.
            // This means that we have to include the TextAlign into the column format.

            lvColumn.mask |= LVCOLUMNW_MASK.LVCF_FMT;

            if (ch.ActualImageIndex_Internal > -1)
            {
                // you would think that setting iImage would be enough.
                // actually we also have to set the format to include LVCOLUMNW_FORMAT.LVCFMT_IMAGE
                lvColumn.iImage = ch.ActualImageIndex_Internal;
                lvColumn.fmt |= LVCOLUMNW_FORMAT.LVCFMT_IMAGE;
            }

            lvColumn.fmt |= (LVCOLUMNW_FORMAT)ch.TextAlign;
        }

        IntPtr result;
        fixed (char* columnHeaderText = ch.Text)
        {
            if ((mask & LVCOLUMNW_MASK.LVCF_TEXT) != 0)
            {
                lvColumn.pszText = columnHeaderText;
            }

            result = PInvokeCore.SendMessage(this, PInvoke.LVM_SETCOLUMNW, (WPARAM)ch.Index, ref lvColumn);
        }

        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException(SR.ListViewColumnInfoSet);
        }

        // When running on AMD64 the list view does not invalidate the column header.
        // So we do it ourselves.
        InvalidateColumnHeaders();
    }

    [MemberNotNull(nameof(_columnHeaders))]
    private ColumnHeader GetColumnHeader(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _columnHeaders?.Length ?? 0);

        return _columnHeaders![index];
    }

    /// <summary>
    ///  Setting width is a special case 'cuz LVM_SETCOLUMNWIDTH accepts more values
    ///  for width than LVM_SETCOLUMN does.
    /// </summary>
    internal void SetColumnWidth(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        ColumnHeader columnHeader = GetColumnHeader(columnIndex);

        // valid values are 0x0 to 0x2
        SourceGenerated.EnumValidator.Validate(headerAutoResize, nameof(headerAutoResize));

        int width = 0;
        int compensate = 0;

        if (headerAutoResize == ColumnHeaderAutoResizeStyle.None)
        {
            width = columnHeader.WidthInternal;

            // If the width maps to a LVCSW_ const, then native control will autoresize.
            // We may need to compensate for that.
            if (width == PInvoke.LVSCW_AUTOSIZE_USEHEADER)
            {
                headerAutoResize = ColumnHeaderAutoResizeStyle.HeaderSize;
            }
            else if (width == PInvoke.LVSCW_AUTOSIZE)
            {
                headerAutoResize = ColumnHeaderAutoResizeStyle.ColumnContent;
            }
        }

        if (headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize)
        {
            compensate = CompensateColumnHeaderResize(columnIndex, columnResizeCancelled: false);
            width = PInvoke.LVSCW_AUTOSIZE_USEHEADER;
        }
        else if (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent)
        {
            compensate = CompensateColumnHeaderResize(columnIndex, columnResizeCancelled: false);
            width = PInvoke.LVSCW_AUTOSIZE;
        }

        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETCOLUMNWIDTH, (WPARAM)columnIndex, LPARAM.MAKELPARAM(width, 0));
        }

        if (IsHandleCreated &&
           (headerAutoResize == ColumnHeaderAutoResizeStyle.ColumnContent ||
            headerAutoResize == ColumnHeaderAutoResizeStyle.HeaderSize))
        {
            if (compensate != 0)
            {
                int newWidth = columnHeader.Width + compensate;
                PInvokeCore.SendMessage(
                    this,
                    PInvoke.LVM_SETCOLUMNWIDTH,
                    (WPARAM)columnIndex,
                    (LPARAM)newWidth);
            }
        }
    }

    private void SetColumnWidth(int index, int width)
    {
        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETCOLUMNWIDTH, (WPARAM)index, LPARAM.MAKELPARAM(width, 0));
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
                PInvokeCore.SendMessage(
                    this,
                    PInvoke.LVM_SETCOLUMNORDERARRAY,
                    (WPARAM)orderedColumns.Length,
                    (LPARAM)pOrderedColumns);
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
        if (addItem)
        {
            _savedCheckedItems ??= [];
            _savedCheckedItems.Add(item);
        }
        else if (_savedCheckedItems is not null)
        {
            Debug.Assert(_savedCheckedItems.Contains(item), "somehow we lost track of one item");
            _savedCheckedItems.Remove(item);
        }
    }

    /// <summary>
    ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
    /// </summary>
    internal override void SetToolTip(ToolTip toolTip)
    {
        if (toolTip is null)
        {
            return;
        }

        _toolTipCaption = toolTip.GetToolTip(this);

        // Native ListView expects tooltip HWND as a wParam and ignores lParam
        HWND oldHandle = (HWND)PInvokeCore.SendMessage(this, PInvoke.LVM_SETTOOLTIPS, toolTip);
        PInvoke.DestroyWindow(oldHandle);
    }

    internal void SetItemImage(int itemIndex, int imageIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, VirtualMode ? VirtualListSize : _itemCount);

        if (!IsHandleCreated)
        {
            return;
        }

        LVITEMW lvItem = new()
        {
            mask = LIST_VIEW_ITEM_FLAGS.LVIF_IMAGE,
            iItem = itemIndex,
            iImage = imageIndex
        };

        PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMW, (WPARAM)0, ref lvItem);
    }

    internal void SetItemIndentCount(int index, int indentCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, VirtualMode ? VirtualListSize : _itemCount);

        if (!IsHandleCreated)
        {
            return;
        }

        LVITEMW lvItem = new()
        {
            mask = LIST_VIEW_ITEM_FLAGS.LVIF_INDENT,
            iItem = index,
            iIndent = indentCount
        };

        PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMW, (WPARAM)0, ref lvItem);
    }

    internal void SetItemPosition(int index, int x, int y)
    {
        if (VirtualMode)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _itemCount);

        Debug.Assert(IsHandleCreated, "How did we add items without a handle?");

        Point pt = new(x, y);
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMPOSITION32, (WPARAM)index, ref pt);
    }

    internal void SetItemState(int index, LIST_VIEW_ITEM_STATE_FLAGS state, LIST_VIEW_ITEM_STATE_FLAGS mask)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, -1);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, VirtualMode ? VirtualListSize : _itemCount);

        if (!IsHandleCreated)
        {
            return;
        }

        LVITEMW lvItem = new()
        {
            mask = LIST_VIEW_ITEM_FLAGS.LVIF_STATE,
            state = state,
            stateMask = mask
        };

        PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMSTATE, (WPARAM)index, ref lvItem);
    }

    internal void SetItemText(int itemIndex, int subItemIndex, string text)
    {
        LVITEMW lvItem = default;
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
            int colWidth = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_GETCOLUMNWIDTH);

            using Graphics g = CreateGraphicsInternal();

            int textWidth = Size.Ceiling(g.MeasureString(text, Font)).Width;
            if (textWidth > colWidth)
            {
                SetColumnWidth(0, textWidth);
            }
        }

        lvItem.mask = LIST_VIEW_ITEM_FLAGS.LVIF_TEXT;
        lvItem.iItem = itemIndex;
        lvItem.iSubItem = subItemIndex;

        fixed (char* pText = text)
        {
            lvItem.pszText = pText;

            PInvokeCore.SendMessage(this, PInvoke.LVM_SETITEMTEXTW, (WPARAM)itemIndex, ref lvItem);
        }
    }

    // ComCtl32 list view uses a selection mark to keep track of selection state - iMark.
    // ComCtl32 list view updates iMark only when the user hovers over the item.
    // This means that if we programmatically set the selection item, then the list view will not update
    // its selection mark.
    // So we explicitly set the selection mark.
    internal void SetSelectionMark(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Items.Count)
        {
            return;
        }

        PInvokeCore.SendMessage(this, PInvoke.LVM_SETSELECTIONMARK, (WPARAM)0, itemIndex);
    }

    private void SmallImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        nint handle = (SmallImageList is null) ? 0 : SmallImageList.Handle;
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_SMALL, (LPARAM)handle);

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
        if (IsHandleCreated && _listItemSorter is not null)
        {
            NativeMethods.ListViewCompareCallback callback = new(CompareFunc);
            IntPtr callbackPointer = Marshal.GetFunctionPointerForDelegate(callback);
            PInvokeCore.SendMessage(this, PInvoke.LVM_SORTITEMS, (WPARAM)0, (LPARAM)callbackPointer);
            GC.KeepAlive(callback);
        }
    }

    private void StateImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        nint handle = (StateImageList is null) ? 0 : StateImageList.Handle;
        PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_STATE, (LPARAM)handle);
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();

        if (_listViewItems is not null)
        {
            s += $", Items.Count: {_listViewItems.Count}";
            if (_listViewItems.Count > 0)
            {
                string z = _listViewItems[0].ToString();
                ReadOnlySpan<char> txt = (z.Length > 40) ? z.AsSpan(0, 40) : z;
                s += $", Items[0]: {txt}";
            }
        }
        else if (Items is not null)
        {
            s += $", Items.Count: {Items.Count}";
            if (Items.Count > 0 && !VirtualMode)
            {
                string z = (Items[0] is null) ? "null" : Items[0].ToString();
                ReadOnlySpan<char> txt = (z.Length > 40) ? z.AsSpan(0, 40) : z;
                s += $", Items[0]: {txt}";
            }
        }

        return s;
    }

    internal void UpdateListViewItemsLocations()
    {
        if (!VirtualMode && IsHandleCreated && AutoArrange && (View == View.LargeIcon || View == View.SmallIcon))
        {
            // This only has an affect for large icon and small icon views.
            try
            {
                BeginUpdate();
                PInvokeCore.SendMessage(this, PInvoke.LVM_UPDATE, (WPARAM)(-1));
            }
            finally
            {
                EndUpdate();
            }
        }
    }

    private void UpdateColumnWidths(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        if (_columnHeaders is not null)
        {
            for (int i = 0; i < _columnHeaders.Length; i++)
            {
                SetColumnWidth(i, headerAutoResize);
            }
        }
    }

    protected void UpdateExtendedStyles()
    {
        if (IsHandleCreated)
        {
            uint exStyle = 0;
            uint exMask = PInvoke.LVS_EX_ONECLICKACTIVATE | PInvoke.LVS_EX_TWOCLICKACTIVATE |
                            PInvoke.LVS_EX_TRACKSELECT | PInvoke.LVS_EX_UNDERLINEHOT |
                            PInvoke.LVS_EX_ONECLICKACTIVATE | PInvoke.LVS_EX_HEADERDRAGDROP |
                            PInvoke.LVS_EX_CHECKBOXES | PInvoke.LVS_EX_FULLROWSELECT |
                            PInvoke.LVS_EX_GRIDLINES | PInvoke.LVS_EX_INFOTIP | PInvoke.LVS_EX_DOUBLEBUFFER;

            switch (_activation)
            {
                case ItemActivation.OneClick:
                    exStyle |= PInvoke.LVS_EX_ONECLICKACTIVATE;
                    break;
                case ItemActivation.TwoClick:
                    exStyle |= PInvoke.LVS_EX_TWOCLICKACTIVATE;
                    break;
            }

            if (AllowColumnReorder)
            {
                exStyle |= PInvoke.LVS_EX_HEADERDRAGDROP;
            }

            if (CheckBoxes)
            {
                exStyle |= PInvoke.LVS_EX_CHECKBOXES;
            }

            if (DoubleBuffered)
            {
                exStyle |= PInvoke.LVS_EX_DOUBLEBUFFER;
            }

            if (FullRowSelect)
            {
                exStyle |= PInvoke.LVS_EX_FULLROWSELECT;
            }

            if (GridLines)
            {
                exStyle |= PInvoke.LVS_EX_GRIDLINES;
            }

            if (HoverSelection)
            {
                exStyle |= PInvoke.LVS_EX_TRACKSELECT;
            }

            if (HotTracking)
            {
                exStyle |= PInvoke.LVS_EX_UNDERLINEHOT;
            }

            if (ShowItemToolTips)
            {
                exStyle |= PInvoke.LVS_EX_INFOTIP;
            }

            PInvokeCore.SendMessage(this, PInvoke.LVM_SETEXTENDEDLISTVIEWSTYLE, (WPARAM)exMask, (LPARAM)exStyle);
            Invalidate();
        }
    }

    internal void UpdateGroupNative(ListViewGroup group)
    {
        Debug.Assert(IsHandleCreated, "UpdateGroupNative precondition: list-view handle must be created");

        nint result = SendGroupMessage(group, PInvoke.LVM_SETGROUPINFO, group.ID, 0);
        Debug.Assert(result != -1);
    }

    private unsafe nint SendGroupMessage(ListViewGroup group, uint msg, nint lParam, LVGROUP_MASK additionalMask)
    {
        string header = group.Header;
        string footer = group.Footer;
        string subtitle = group.Subtitle;
        string task = group.TaskLink;
        LVGROUP lvgroup = new()
        {
            cbSize = (uint)sizeof(LVGROUP),
            mask = LVGROUP_MASK.LVGF_HEADER | LVGROUP_MASK.LVGF_ALIGN | LVGROUP_MASK.LVGF_STATE | LVGROUP_MASK.LVGF_TITLEIMAGE | additionalMask,
            cchHeader = header.Length,
            iTitleImage = -1,
            iGroupId = group.ID
        };

        if (subtitle.Length != 0)
        {
            lvgroup.mask |= LVGROUP_MASK.LVGF_SUBTITLE;
        }

        if (task.Length != 0)
        {
            lvgroup.mask |= LVGROUP_MASK.LVGF_TASK;
        }

        if (footer.Length != 0)
        {
            lvgroup.mask |= LVGROUP_MASK.LVGF_FOOTER;
        }

        if (group.CollapsedState != ListViewGroupCollapsedState.Default)
        {
            lvgroup.state |= LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSIBLE;
            if (group.CollapsedState == ListViewGroupCollapsedState.Collapsed)
            {
                lvgroup.state |= LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSED;
            }
        }

        switch (group.HeaderAlignment)
        {
            case HorizontalAlignment.Left:
                lvgroup.uAlign = LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_HEADER_LEFT;
                break;
            case HorizontalAlignment.Right:
                lvgroup.uAlign = LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_HEADER_RIGHT;
                break;
            case HorizontalAlignment.Center:
                lvgroup.uAlign = LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_HEADER_CENTER;
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
            if (footer.Length != 0)
            {
                lvgroup.cchFooter = footer.Length;
                lvgroup.pszFooter = pFooter;
                switch (group.FooterAlignment)
                {
                    case HorizontalAlignment.Left:
                        lvgroup.uAlign |= LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_FOOTER_LEFT;
                        break;
                    case HorizontalAlignment.Right:
                        lvgroup.uAlign |= LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_FOOTER_RIGHT;
                        break;
                    case HorizontalAlignment.Center:
                        lvgroup.uAlign |= LIST_VIEW_GROUP_ALIGN_FLAGS.LVGA_FOOTER_CENTER;
                        break;
                }
            }

            if (subtitle.Length != 0)
            {
                lvgroup.cchSubtitle = (uint)subtitle.Length;
                lvgroup.pszSubtitle = pSubtitle;
            }

            if (task.Length != 0)
            {
                lvgroup.cchTask = (uint)task.Length;
                lvgroup.pszTask = pTask;
            }

            lvgroup.pszHeader = pHeader;
            return PInvokeCore.SendMessage(this, msg, (WPARAM)lParam, ref lvgroup);
        }
    }

    // ListViewGroupCollection::Clear needs to remove the items from the Default group
    internal void UpdateGroupView()
    {
        if (IsHandleCreated && Application.ComCtlSupportsVisualStyles && !VirtualMode)
        {
            int retval = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_ENABLEGROUPVIEW, (WPARAM)(BOOL)GroupsEnabled);
            Debug.Assert(retval != -1, "Error enabling group view");
        }
    }

    // updates the win32 list view w/ our tile info - columns + tile size
    private unsafe void UpdateTileView()
    {
        Debug.Assert(Application.ComCtlSupportsVisualStyles, "this function works only when ComCtl 6.0 and higher is loaded");
        Debug.Assert(_viewStyle == View.Tile, "this function should be called only in Tile view");

        LVTILEVIEWINFO tileViewInfo = new()
        {
            cbSize = (uint)sizeof(LVTILEVIEWINFO),

            dwMask = LVTILEVIEWINFO_MASK.LVTVIM_COLUMNS | LVTILEVIEWINFO_MASK.LVTVIM_TILESIZE,
            dwFlags = LVTILEVIEWINFO_FLAGS.LVTVIF_FIXEDSIZE,
            cLines = _columnHeaders is not null ? _columnHeaders.Length : 0,
            sizeTile = TileSize,
        };

        nint retval = PInvokeCore.SendMessage(this, PInvoke.LVM_SETTILEVIEWINFO, (WPARAM)0, ref tileViewInfo);
        Debug.Assert(retval != 0, "LVM_SETTILEVIEWINFO failed");
    }

    private void WmNmClick()
    {
        // If we're checked, hittest to see if we're
        // on the check mark

        if (!CheckBoxes)
        {
            return;
        }

        LVHITTESTINFO lvhi = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        int displayIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_SUBITEMHITTEST, (WPARAM)0, ref lvhi);
        if (displayIndex == -1 || lvhi.iSubItem != 0 || (lvhi.flags & LVHITTESTINFO_FLAGS.LVHT_ONITEMSTATEICON) == 0)
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

    private void WmNmDblClick()
    {
        // If we're checked, hittest to see if we're
        // on the item

        if (!CheckBoxes || VirtualMode)
        {
            return;
        }

        LVHITTESTINFO lvhi = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        int displayIndex = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_HITTEST, (WPARAM)0, ref lvhi);
        if (displayIndex != -1 &&
            (lvhi.flags &
            (LVHITTESTINFO_FLAGS.LVHT_ONITEMICON | LVHITTESTINFO_FLAGS.LVHT_ONITEMLABEL | LVHITTESTINFO_FLAGS.LVHT_ABOVE)) != 0)
        {
            ListViewItem clickedItem = Items[displayIndex];
            clickedItem.Checked = !clickedItem.Checked;
        }
    }

    private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
    {
        // Always Reset the MouseUpFired....
        _listViewState[LISTVIEWSTATE_mouseUpFired] = false;
        _listViewState[LISTVIEWSTATE_expectingMouseUp] = true;

        // This is required to FORCE Validation before Windows ListView pushes its own message loop...
        Focus();

        // Windows ListView pushes its own Windows ListView in WM_xBUTTONDOWN, so fire the
        // event before calling defWndProc or else it won't get fired until the button
        // comes back up.
        Point point = PARAM.ToPoint(m.LParamInternal);
        OnMouseDown(new MouseEventArgs(button, clicks, point));

        // If Validation is cancelled don't fire any events through the Windows ListView's message loop.
        if (!ValidationCancelled)
        {
            if (CheckBoxes)
            {
                ListViewHitTestInfo lvhti = HitTest(point);
                if (_imageListState is not null && _imageListState.Images.Count < 2)
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
                    if (lvhti.Item is not null && lvhti.Location == ListViewHitTestLocations.StateImage)
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

        if (IsAccessibilityObjectCreated)
        {
            Point screenPoint = PointToScreen(point);
            AccessibleObject? accessibilityObject = AccessibilityObject.HitTest(screenPoint.X, screenPoint.Y);
            accessibilityObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    private unsafe bool WmNotify(ref Message m)
    {
        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;

        // We need to set the text color when we are in dark mode,
        // so that the themed headers are actually readable.
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        if (Application.IsDarkModeEnabled
            && !OwnerDraw
            && nmhdr->code == PInvoke.NM_CUSTOMDRAW)
        {
            NMLVCUSTOMDRAW* nmlvcd = (NMLVCUSTOMDRAW*)(nint)m.LParamInternal;

            if (nmlvcd->nmcd.dwDrawStage == NMCUSTOMDRAW_DRAW_STAGE.CDDS_PREPAINT)
            {
                // We request the notification for the items to be drawn.
                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYITEMDRAW;
                return true;
            }
            else if (nmlvcd->nmcd.dwDrawStage == NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT)
            {
                // Setting the current ForeColor to the text color.
                PInvokeCore.SetTextColor(nmlvcd->nmcd.hdc, ForeColor);

                // and the rest remains the same.
                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
                return false;
            }
        }
#pragma warning restore WFO5001

        if (nmhdr->code == PInvoke.NM_CUSTOMDRAW && PInvoke.UiaClientsAreListening())
        {
            // Checking that mouse buttons are not pressed is necessary to avoid
            // multiple annotation of the column header when resizing the column with the mouse
            if (m.LParamInternal != 0 && MouseButtons == MouseButtons.None)
            {
                AnnounceColumnHeader(Cursor.Position);
            }
        }

        // Column header custom draw message handling.
        if (nmhdr->code == PInvoke.NM_CUSTOMDRAW && OwnerDraw)
        {
            try
            {
                NMCUSTOMDRAW* nmcd = (NMCUSTOMDRAW*)(nint)m.LParamInternal;
                // Find out which stage we're drawing
                switch (nmcd->dwDrawStage)
                {
                    case NMCUSTOMDRAW_DRAW_STAGE.CDDS_PREPAINT:
                        {
                            m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYITEMDRAW;
                            return true; // we are done - don't do default handling
                        }

                    case NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT:
                        {
                            using Graphics g = nmcd->hdc.CreateGraphics();
                            Color foreColor = Color.FromArgb((int)PInvoke.GetTextColor(nmcd->hdc).Value);
                            Color backColor = Color.FromArgb((int)PInvoke.GetBkColor(nmcd->hdc).Value);
                            Font font = GetListHeaderFont();
                            DrawListViewColumnHeaderEventArgs e = new(
                                g,
                                nmcd->rc,
                                (int)nmcd->dwItemSpec,
                                _columnHeaders![(int)nmcd->dwItemSpec],
                                (ListViewItemStates)nmcd->uItemState,
                                foreColor,
                                backColor,
                                font);
                            OnDrawColumnHeader(e);
                            if (e.DrawDefault)
                            {
                                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
                                return false;
                            }
                            else
                            {
                                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                                return true; // we are done - don't do default handling
                            }
                        }

                    default:
                        return false; // default handling
                }
            }
            catch (Exception e)
            {
                Debug.Fail("Exception occurred attempting to setup header custom draw. Disabling custom draw for the column header", e.ToString());
                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
            }
        }

        if (nmhdr->code == PInvoke.NM_RELEASEDCAPTURE && _listViewState[LISTVIEWSTATE_columnClicked])
        {
            _listViewState[LISTVIEWSTATE_columnClicked] = false;
            OnColumnClick(new ColumnClickEventArgs(_columnIndex));
        }

        if (nmhdr->code == PInvoke.HDN_BEGINTRACKW)
        {
            _listViewState[LISTVIEWSTATE_headerControlTracking] = true;

            // Reset our tracking information for the new BEGINTRACK cycle.
            _listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
            _newWidthForColumnWidthChangingCancelled = -1;
            _listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;

            NMHEADERW* nmheader = (NMHEADERW*)(nint)m.LParamInternal;
            if (_columnHeaders is not null && _columnHeaders.Length > nmheader->iItem)
            {
                _columnHeaderClicked = _columnHeaders[nmheader->iItem];
                _columnHeaderClickedWidth = _columnHeaderClicked.Width;
            }
            else
            {
                _columnHeaderClickedWidth = -1;
                _columnHeaderClicked = null;
            }
        }

        if (nmhdr->code == PInvoke.HDN_ITEMCHANGINGW)
        {
            NMHEADERW* nmheader = (NMHEADERW*)(nint)m.LParamInternal;

            if (_columnHeaders is not null && nmheader->iItem < _columnHeaders.Length &&
                (_listViewState[LISTVIEWSTATE_headerControlTracking] || _listViewState[LISTVIEWSTATE_headerDividerDblClick]))
            {
                int newColumnWidth = ((nmheader->pitem->mask & HDI_MASK.HDI_WIDTH) != 0) ? nmheader->pitem->cxy : -1;
                ColumnWidthChangingEventArgs colWidthChanging = new(nmheader->iItem, newColumnWidth);
                OnColumnWidthChanging(colWidthChanging);
                m.ResultInternal = (LRESULT)(colWidthChanging.Cancel ? 1 : 0);
                if (colWidthChanging.Cancel)
                {
                    nmheader->pitem->cxy = colWidthChanging.NewWidth;

                    // We are called inside HDN_DIVIDERDBLCLICK.
                    // Turn off the compensation that our processing of HDN_DIVIDERDBLCLICK would otherwise add.
                    if (_listViewState[LISTVIEWSTATE_headerDividerDblClick])
                    {
                        _listViewState[LISTVIEWSTATE_columnResizeCancelled] = true;
                    }

                    _listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = true;
                    _newWidthForColumnWidthChangingCancelled = colWidthChanging.NewWidth;

                    // skip default processing
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        if ((nmhdr->code == PInvoke.HDN_ITEMCHANGEDW) &&
            !_listViewState[LISTVIEWSTATE_headerControlTracking])
        {
            NMHEADERW* nmheader = (NMHEADERW*)(nint)m.LParamInternal;
            if (_columnHeaders is not null && nmheader->iItem < _columnHeaders.Length)
            {
                int w = _columnHeaders[nmheader->iItem].Width;

                if (_columnHeaderClicked is null ||
                    (_columnHeaderClicked == _columnHeaders[nmheader->iItem] &&
                     _columnHeaderClickedWidth != -1 &&
                     _columnHeaderClickedWidth != w))
                {
                    // If the user double clicked on the column header and we still need to compensate for the column resize
                    // then don't fire ColumnWidthChanged because at this point the column header does not have the final width.
                    if (_listViewState[LISTVIEWSTATE_headerDividerDblClick])
                    {
                        if (CompensateColumnHeaderResize(m, _listViewState[LISTVIEWSTATE_columnResizeCancelled]) == 0)
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

            _columnHeaderClicked = null;
            _columnHeaderClickedWidth = -1;

            ISite? site = Site;

            if (site?.TryGetService(out IComponentChangeService? service) == true)
            {
                try
                {
                    service.OnComponentChanging(this, null);
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

        if (nmhdr->code == PInvoke.HDN_ENDTRACKW)
        {
            Debug.Assert(_listViewState[LISTVIEWSTATE_headerControlTracking], "HDN_ENDTRACK and HDN_BEGINTRACK are out of sync.");
            _listViewState[LISTVIEWSTATE_headerControlTracking] = false;
            if (_listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging])
            {
                m.ResultInternal = (LRESULT)1;
                if (_newWidthForColumnWidthChangingCancelled != -1)
                {
                    NMHEADERW* nmheader = (NMHEADERW*)(nint)m.LParamInternal;
                    if (_columnHeaders is not null && _columnHeaders.Length > nmheader->iItem)
                    {
                        _columnHeaders[nmheader->iItem].Width = _newWidthForColumnWidthChangingCancelled;
                    }
                }

                _listViewState1[LISTVIEWSTATE1_cancelledColumnWidthChanging] = false;
                _newWidthForColumnWidthChangingCancelled = -1;

                // skip default processing
                return true;
            }
            else
            {
                return false;
            }
        }

        if (nmhdr->code == PInvoke.HDN_ENDDRAG)
        {
            NMHEADERW* header = (NMHEADERW*)(nint)m.LParamInternal;
            if (header->pitem is not null)
            {
                if (header->pitem->mask.HasFlag(HDI_MASK.HDI_ORDER))
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

                    ColumnReorderedEventArgs chrevent = new(
                        from,
                        to,
                        Columns[header->iItem]);
                    OnColumnReordered(chrevent);
                    if (chrevent.Cancel)
                    {
                        m.ResultInternal = (LRESULT)1;
                        return true;
                    }
                    else
                    {
                        // set the display indices. This is not an expensive operation because
                        // we only set an integer in the column header class
                        int lowDI = Math.Min(from, to);
                        int hiDI = Math.Max(from, to);
                        bool hdrMovedForward = to > from;
                        ColumnHeader? movedHdr = null;
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

                        movedHdr!.DisplayIndexInternal = to;
                        indices[movedHdr.Index] = movedHdr.DisplayIndexInternal;
                        SetDisplayIndices(indices);
#if DEBUG
                        CheckDisplayIndices();
#endif
                    }
                }
            }
        }

        if (nmhdr->code == PInvoke.HDN_DIVIDERDBLCLICKW)
        {
            // We need to keep track that the user double clicked the column header divider
            // so we know that the column header width is changing.
            _listViewState[LISTVIEWSTATE_headerDividerDblClick] = true;

            // Reset ColumnResizeCancelled.
            // It will be set if the user cancels the ColumnWidthChanging event.
            _listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;

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
                _listViewState[LISTVIEWSTATE_headerDividerDblClick] = false;
                columnResizeCancelled = _listViewState[LISTVIEWSTATE_columnResizeCancelled];
                _listViewState[LISTVIEWSTATE_columnResizeCancelled] = false;
            }

            _columnHeaderClicked = null;
            _columnHeaderClickedWidth = -1;

            if (columnResizeCancelled)
            {
                // If the column resize was cancelled then apply the NewWidth supplied by the user.
                if (_newWidthForColumnWidthChangingCancelled != -1)
                {
                    NMHEADERW* nmheader = (NMHEADERW*)(nint)m.LParamInternal;
                    if (_columnHeaders is not null && _columnHeaders.Length > nmheader->iItem)
                    {
                        _columnHeaders[nmheader->iItem].Width = _newWidthForColumnWidthChangingCancelled;
                    }
                }

                // Tell ComCtl that the HDN_DIVIDERDBLCLICK was cancelled.
                m.ResultInternal = (LRESULT)1;
            }
            else
            {
                // Compensate for the column resize.
                int compensateForColumnResize = CompensateColumnHeaderResize(m, columnResizeCancelled);
                if (compensateForColumnResize != 0)
                {
#if DEBUG
                    NMHEADERW* header = (NMHEADERW*)(nint)m.LParamInternal;
                    Debug.Assert(header->iItem == 0, "we only need to compensate for the first column resize");
                    Debug.Assert(_columnHeaders!.Length > 0, "there should be a column that we need to compensate for");
#endif

                    ColumnHeader col = _columnHeaders![0];
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
        HWND hwndHdr = (HWND)PInvokeCore.SendMessage(this, PInvoke.LVM_GETHEADER);
        HFONT hFont = (HFONT)PInvokeCore.SendMessage(hwndHdr, PInvokeCore.WM_GETFONT);
        return Font.FromHfont(hFont);
    }

    private int GetIndexOfClickedItem()
    {
        var lvhi = SetupHitTestInfo();
        return (int)PInvokeCore.SendMessage(this, PInvoke.LVM_HITTEST, (WPARAM)0, ref lvhi);
    }

    private LVHITTESTINFO SetupHitTestInfo()
    {
        LVHITTESTINFO lvhi = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        return lvhi;
    }

    private void Unhook()
    {
        foreach (ListViewItem listViewItem in Items)
        {
            KeyboardToolTipStateMachine.Instance.Unhook(listViewItem, KeyboardToolTip);
        }
    }

    private int UpdateGroupCollapse(MessageId clickType)
    {
        // See if the mouse event occurred on a group.
        var lvhi = SetupHitTestInfo();
        int groupID = (int)PInvokeCore.SendMessage(this, PInvoke.LVM_HITTEST, (WPARAM)(-1), ref lvhi);
        if (groupID == -1)
        {
            return groupID;
        }

        // check if group header was double clicked
        bool groupHeaderDblClicked = lvhi.flags == LVHITTESTINFO_FLAGS.LVHT_EX_GROUP_HEADER && clickType == PInvokeCore.WM_LBUTTONDBLCLK;
        // check if chevron was clicked
        bool chevronClicked = (lvhi.flags & LVHITTESTINFO_FLAGS.LVHT_EX_GROUP_COLLAPSE) == LVHITTESTINFO_FLAGS.LVHT_EX_GROUP_COLLAPSE && clickType == PInvokeCore.WM_LBUTTONUP;
        if (!groupHeaderDblClicked && !chevronClicked)
        {
            return groupID;
        }

        for (int i = 0; i < _groups!.Count; i++)
        {
            ListViewGroup targetGroup = _groups[i];
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
        // For some reason, if CheckBoxes are set to true and the list view has a state imageList, then the native
        // listView destroys the state imageList.
        // (Yes, it does exactly that even though our wrapper sets LVS_SHAREIMAGELISTS on the native listView.)
        if (IsHandleCreated && StateImageList is not null)
        {
            PInvokeCore.SendMessage(this, PInvoke.LVM_SETIMAGELIST, (WPARAM)PInvoke.LVSIL_STATE);
        }

        RecreateHandle();
    }

    private unsafe void WmReflectNotify(ref Message m)
    {
        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;

        switch (nmhdr->code)
        {
            case PInvoke.NM_CUSTOMDRAW:
                CustomDraw(ref m);
                break;

            case PInvoke.LVN_BEGINLABELEDITW:
                {
                    Debug.Assert(_labelEdit is null,
                        "A new label editing shouldn't start before the previous one ended");
                    if (_labelEdit is not null)
                    {
                        _labelEdit.ReleaseHandle();
                        _labelEdit = null;
                    }

                    bool cancelEdit;
                    if (_blockLabelEdit)
                    {
                        cancelEdit = true;
                    }
                    else
                    {
                        NMLVDISPINFOW* dispInfo = (NMLVDISPINFOW*)(nint)m.LParamInternal;
                        LabelEditEventArgs e = new(dispInfo->item.iItem);
                        OnBeforeLabelEdit(e);
                        cancelEdit = e.CancelEdit;
                    }

                    m.ResultInternal = (LRESULT)(nint)(BOOL)cancelEdit;
                    _listViewState[LISTVIEWSTATE_inLabelEdit] = !cancelEdit;

                    if (!cancelEdit)
                    {
                        _labelEdit = new ListViewLabelEditNativeWindow(this);
                        _labelEdit.AssignHandle(PInvokeCore.SendMessage(this, PInvoke.LVM_GETEDITCONTROL));
                    }

                    break;
                }

            case PInvoke.LVN_COLUMNCLICK:
                {
                    NMLISTVIEW* nmlv = (NMLISTVIEW*)(nint)m.LParamInternal;
                    _listViewState[LISTVIEWSTATE_columnClicked] = true;
                    _columnIndex = nmlv->iSubItem;
                    break;
                }

            case PInvoke.LVN_LINKCLICK:
                {
                    NMLVLINK* pLink = (NMLVLINK*)(nint)m.LParamInternal;
                    int groupID = pLink->iSubItem;
                    for (int i = 0; i < _groups!.Count; i++)
                    {
                        if (_groups[i].ID == groupID)
                        {
                            OnGroupTaskLinkClick(new ListViewGroupEventArgs(i));
                            break;
                        }
                    }

                    break;
                }

            case PInvoke.LVN_ENDLABELEDITW:
                {
                    Debug.Assert(_labelEdit is not null, "There is no active label edit to end");
                    if (_labelEdit is null)
                    {
                        break;
                    }

                    _labelEdit.ReleaseHandle();
                    _labelEdit = null;

                    _listViewState[LISTVIEWSTATE_inLabelEdit] = false;
                    NMLVDISPINFOW* dispInfo = (NMLVDISPINFOW*)(nint)m.LParamInternal;
                    string? text = dispInfo->item.pszText.ToString();
                    LabelEditEventArgs e = new(dispInfo->item.iItem, text);
                    OnAfterLabelEdit(e);
                    m.ResultInternal = (LRESULT)(nint)(BOOL)!e.CancelEdit;

                    // from msdn:
                    //   "If the user cancels editing, the pszText member of the LVITEM structure is NULL"
                    if (!e.CancelEdit && !dispInfo->item.pszText.IsNull)
                    {
                        Items[dispInfo->item.iItem].Text = text;
                    }

                    break;
                }

            case PInvoke.LVN_ITEMACTIVATE:
                OnItemActivate(EventArgs.Empty);
                break;

            case PInvoke.LVN_BEGINDRAG:
                {
                    // The items collection was modified while dragging that means that
                    // we can't reliably give the user the item on which the dragging
                    // started so don't tell the user about this operation.
                    if (!ItemCollectionChangedInMouseDown)
                    {
                        NMLISTVIEW* nmlv = (NMLISTVIEW*)(nint)m.LParamInternal;
                        ListViewItem item = Items[nmlv->iItem];
                        OnItemDrag(new ItemDragEventArgs(MouseButtons.Left, item));
                    }

                    break;
                }

            case PInvoke.LVN_BEGINRDRAG:
                {
                    // The items collection was modified while dragging. That means that
                    // we can't reliably give the user the item on which the dragging
                    // started so don't tell the user about this operation.
                    if (!ItemCollectionChangedInMouseDown)
                    {
                        NMLISTVIEW* nmlv = (NMLISTVIEW*)(nint)m.LParamInternal;
                        ListViewItem item = Items[nmlv->iItem];
                        OnItemDrag(new ItemDragEventArgs(MouseButtons.Right, item));
                    }

                    break;
                }

            case PInvoke.LVN_ITEMCHANGING:
                {
                    NMLISTVIEW* nmlv = (NMLISTVIEW*)(nint)m.LParamInternal;
                    if ((nmlv->uChanged & LIST_VIEW_ITEM_FLAGS.LVIF_STATE) != 0)
                    {
                        // Because the state image mask is 1-based, a value of 1 means unchecked,
                        // anything else means checked. We convert this to the more standard 0 or 1
                        CheckState oldState = (CheckState)(((int)((LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uOldState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                        CheckState newState = (CheckState)(((int)((LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uNewState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                        if (oldState != newState)
                        {
                            ItemCheckEventArgs e = new(nmlv->iItem, newState, oldState);
                            OnItemCheck(e);
                            m.ResultInternal = (LRESULT)(nint)(BOOL)(e.NewValue == oldState);
                        }
                    }

                    break;
                }

            case PInvoke.LVN_ITEMCHANGED:
                {
                    NMLISTVIEW* nmlv = (NMLISTVIEW*)(nint)m.LParamInternal;
                    // Check for state changes to the selected state...
                    if ((nmlv->uChanged & LIST_VIEW_ITEM_FLAGS.LVIF_STATE) != 0)
                    {
                        // Because the state image mask is 1-based, a value of 1 means unchecked,
                        // anything else means checked. We convert this to the more standard 0 or 1
                        CheckState oldValue = (CheckState)(((int)((LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uOldState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);
                        CheckState newValue = (CheckState)(((int)((LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uNewState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) >> 12) == 1 ? 0 : 1);

                        if (newValue != oldValue)
                        {
                            ItemCheckedEventArgs e = new(Items[nmlv->iItem]);
                            OnItemChecked(e);

                            AccessibilityNotifyClients(AccessibleEvents.StateChange, nmlv->iItem);
                            AccessibilityNotifyClients(AccessibleEvents.NameChange, nmlv->iItem);
                        }

                        int indexItem = nmlv->iItem;

                        // This code handles a change in the state of an item. We get here twice.
                        // The first time the focus goes off the old item, then we hide the tooltip.
                        // The second time the next item receives focus and we show a tooltip for it.
                        if (indexItem >= 0 && indexItem < Items.Count)
                        {
                            if (ShowItemToolTips)
                            {
                                ListViewItem item = Items[indexItem];
                                if (item.Focused)
                                {
                                    NotifyAboutGotFocus(item);
                                }
                                else
                                {
                                    NotifyAboutLostFocus(item);
                                }
                            }
                        }

                        LIST_VIEW_ITEM_STATE_FLAGS oldState = (LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uOldState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED;
                        LIST_VIEW_ITEM_STATE_FLAGS newState = (LIST_VIEW_ITEM_STATE_FLAGS)nmlv->uNewState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED;
                        // Windows common control always fires
                        // this event twice, once with newState, oldState, and again with
                        // oldState, newState.
                        // Changing this affects the behavior as the control never
                        // fires the event on a Deselect of an Items from multiple selections.
                        // So leave it as it is...
                        if (newState != oldState)
                        {
                            if (VirtualMode && nmlv->iItem == -1)
                            {
                                if (VirtualListSize > 0)
                                {
                                    ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new(0, VirtualListSize - 1, newState != 0);
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
                                    ListViewItemSelectionChangedEventArgs lvisce = new(Items[nmlv->iItem],
                                                                                                                             nmlv->iItem,
                                                                                                                             newState != 0);
                                    OnItemSelectionChanged(lvisce);
                                }
                            }

                            // Delay SelectedIndexChanged event because the last item isn't present yet.
                            if (Items.Count == 0 || Items[^1] is not null)
                            {
                                _listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = false;
                                OnSelectedIndexChanged(EventArgs.Empty);
                            }
                            else
                            {
                                _listViewState1[LISTVIEWSTATE1_selectedIndexChangedSkipped] = true;
                            }
                        }
                    }

                    break;
                }

            case PInvoke.NM_CLICK:
                WmNmClick();
                // FALL THROUGH //
                goto case PInvoke.NM_RCLICK;

            case PInvoke.NM_RCLICK:
                int displayIndex = GetIndexOfClickedItem();

                MouseButtons button = nmhdr->code == PInvoke.NM_CLICK ? MouseButtons.Left : MouseButtons.Right;
                Point pos = Cursor.Position;
                pos = PointToClient(pos);

                if (!ValidationCancelled && displayIndex != -1)
                {
                    OnClick(EventArgs.Empty);
                    OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                }

                if (!_listViewState[LISTVIEWSTATE_mouseUpFired])
                {
                    OnMouseUp(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                    _listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                }

                break;

            case PInvoke.NM_DBLCLK:
                WmNmDblClick();
                // FALL THROUGH //
                goto case PInvoke.NM_RDBLCLK;

            case PInvoke.NM_RDBLCLK:
                int index = GetIndexOfClickedItem();
                if (index != -1)
                {
                    // just maintain state and fire double click.. in final mouseUp...
                    _listViewState[LISTVIEWSTATE_doubleclickFired] = true;
                }

                // Fire mouse up in the Wndproc
                _listViewState[LISTVIEWSTATE_mouseUpFired] = false;

                // Make sure we get the mouse up if it happens outside the control.
                Capture = true;
                break;

            case PInvoke.LVN_KEYDOWN:
                if (GroupsEnabled)
                {
                    NMLVKEYDOWN* lvkd = (NMLVKEYDOWN*)(nint)m.LParamInternal;
                    if ((lvkd->wVKey == (short)Keys.Down || lvkd->wVKey == (short)Keys.Up) && SelectedItems.Count > 0)
                    {
                        AccessibleObject accessibleObject = SelectedItems[0].AccessibilityObject;
                        if (lvkd->wVKey == (short)Keys.Down
                            && accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling) is null)
                        {
                            ListViewGroupAccessibleObject? groupAccObj = (ListViewGroupAccessibleObject?)accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);
                            if (groupAccObj is not null)
                            {
                                ListViewGroupAccessibleObject? nextGroupAccObj = (ListViewGroupAccessibleObject?)groupAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
                                nextGroupAccObj?.SetFocus();
                            }
                        }

                        if (lvkd->wVKey == (short)Keys.Up
                        && accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling) is null)
                        {
                            ListViewGroupAccessibleObject? groupAccObj = (ListViewGroupAccessibleObject?)accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);
                            groupAccObj?.SetFocus();
                        }
                    }
                }

                if (CheckBoxes && !VirtualMode)
                {
                    NMLVKEYDOWN* lvkd = (NMLVKEYDOWN*)(nint)m.LParamInternal;
                    if (lvkd->wVKey == (short)Keys.Space)
                    {
                        ListViewItem? focusedItem = FocusedItem;
                        if (focusedItem is not null)
                        {
                            bool check = !focusedItem.Checked;
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

                break;

            case PInvoke.LVN_ODCACHEHINT:
                // tell the user to prepare the cache:
                NMLVCACHEHINT* cacheHint = (NMLVCACHEHINT*)(nint)m.LParamInternal;
                OnCacheVirtualItems(new CacheVirtualItemsEventArgs(cacheHint->iFrom, cacheHint->iTo));
                break;

            default:
                if (nmhdr->code == PInvoke.LVN_GETDISPINFOW)
                {
                    // we use the LVN_GETDISPINFO message only in virtual mode
                    if (VirtualMode && m.LParamInternal != 0)
                    {
                        NMLVDISPINFOW* dispInfo = (NMLVDISPINFOW*)(nint)m.LParamInternal;

                        RetrieveVirtualItemEventArgs rVI = new(dispInfo->item.iItem);
                        OnRetrieveVirtualItem(rVI);
                        ListViewItem lvItem = rVI.Item ?? throw new InvalidOperationException(SR.ListViewVirtualItemRequired);

                        lvItem.SetItemIndex(this, dispInfo->item.iItem);
                        if ((dispInfo->item.mask & LIST_VIEW_ITEM_FLAGS.LVIF_TEXT) != 0)
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

                        if ((dispInfo->item.mask & LIST_VIEW_ITEM_FLAGS.LVIF_IMAGE) != 0 && lvItem.ImageIndex != -1)
                        {
                            dispInfo->item.iImage = lvItem.ImageIndex;
                        }

                        if ((dispInfo->item.mask & LIST_VIEW_ITEM_FLAGS.LVIF_INDENT) != 0)
                        {
                            dispInfo->item.iIndent = lvItem.IndentCount;
                        }

                        if ((dispInfo->item.stateMask & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) != 0)
                        {
                            dispInfo->item.state |= lvItem.RawStateImageIndex;
                        }
                    }
                }
                else if (nmhdr->code == PInvoke.LVN_ODSTATECHANGED)
                {
                    if (VirtualMode && m.LParamInternal != 0)
                    {
                        NMLVODSTATECHANGE* odStateChange = (NMLVODSTATECHANGE*)(nint)m.LParamInternal;
                        bool selectedChanged = (odStateChange->uNewState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED) !=
                            (odStateChange->uOldState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED);
                        if (selectedChanged)
                        {
                            // we have to substract 1 from iTo
                            int iTo = odStateChange->iTo;
                            ListViewVirtualItemsSelectionRangeChangedEventArgs lvvisrce = new(odStateChange->iFrom, iTo, (odStateChange->uNewState & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED) != 0);
                            OnVirtualItemsSelectionRangeChanged(lvvisrce);
                        }
                    }
                }
                else if (nmhdr->code == PInvoke.LVN_GETINFOTIPW)
                {
                    if (ShowItemToolTips && m.LParamInternal != 0)
                    {
                        NMLVGETINFOTIPW* infoTip = (NMLVGETINFOTIPW*)(nint)m.LParamInternal;
                        ListViewItem lvi = Items[infoTip->iItem];

                        // This code is needed to hide the keyboard tooltip before showing the mouse tooltip
                        NotifyAboutLostFocus(FocusedItem);

                        if (lvi is not null && !string.IsNullOrEmpty(lvi.ToolTipText))
                        {
                            // Setting the max width has the added benefit of enabling multiline tool tips
                            PInvokeCore.SendMessage(nmhdr->hwndFrom, PInvoke.TTM_SETMAXTIPWIDTH, (WPARAM)0, (LPARAM)SystemInformation.MaxWindowTrackSize.Width);

                            // UNICODE. Use char.
                            // we need to copy the null terminator character ourselves
                            char[] charBuf = (lvi.ToolTipText + "\0").ToCharArray();
                            nint destPtr = new(infoTip->pszText);
                            Marshal.Copy(charBuf, 0, destPtr, Math.Min(charBuf.Length, infoTip->cchTextMax));
                        }
                    }
                }
                else if (nmhdr->code == PInvoke.LVN_ODFINDITEMW)
                {
                    if (VirtualMode)
                    {
                        NMLVFINDITEMW* nmlvif = (NMLVFINDITEMW*)(nint)m.LParamInternal;

                        if ((nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_PARAM) != 0)
                        {
                            m.ResultInternal = (LRESULT)(-1);
                            return;
                        }

                        bool isTextSearch = ((nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_STRING) != 0) ||
                                            ((nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_PARTIAL) != 0);

                        bool isPrefixSearch = (nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_PARTIAL) != 0;

                        string text = string.Empty;
                        if (isTextSearch && !nmlvif->lvfi.psz.IsNull)
                        {
                            text = nmlvif->lvfi.psz.ToString();
                        }

                        Point startingPoint = Point.Empty;
                        if ((nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_NEARESTXY) != 0)
                        {
                            startingPoint = nmlvif->lvfi.pt;
                        }

                        SearchDirectionHint dir = SearchDirectionHint.Down;
                        if ((nmlvif->lvfi.flags & LVFINDINFOW_FLAGS.LVFI_NEARESTXY) != 0)
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

                        SearchForVirtualItemEventArgs sviEvent = new(
                            isTextSearch,
                            isPrefixSearch,
                            includeSubItemsInSearch: false,
                            text,
                            startingPoint,
                            dir,
                            nmlvif->iStart);

                        OnSearchForVirtualItem(sviEvent);
                        m.ResultInternal = sviEvent.Index != -1 ? (LRESULT)sviEvent.Index : (LRESULT)(-1);
                    }
                }

                break;
        }
    }

    private void WmPrint(ref Message m)
    {
        base.WndProc(ref m);
        if (((nint)m.LParamInternal & PInvoke.PRF_NONCLIENT) != 0 && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
        {
            using Graphics g = Graphics.FromHdc((HDC)m.WParamInternal);
            Rectangle rect = new(0, 0, Size.Width - 1, Size.Height - 1);
            using var pen = VisualStyleInformation.TextControlBorder.GetCachedPenScope();
            g.DrawRectangle(pen, rect);
            rect.Inflate(-1, -1);
            g.DrawRectangle(SystemPens.Window, rect);
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_NOTIFY:
                WmReflectNotify(ref m);
                break;

            case PInvokeCore.WM_KEYUP:
                var key = (VIRTUAL_KEY)(uint)m.WParamInternal;

                // User can collapse/expand a group using the keyboard by focusing the group header and using left/right.
                if (GroupsDisplayed && (key is VIRTUAL_KEY.VK_LEFT or VIRTUAL_KEY.VK_RIGHT) && SelectedItems.Count > 0)
                {
                    // User can select more than one group.
                    HashSet<int> groups = [];
                    foreach (ListViewItem selectedItem in SelectedItems)
                    {
                        ListViewGroup? group = selectedItem.Group;
                        if (group is null || group.CollapsedState is ListViewGroupCollapsedState.Default || !groups.Add(group.ID))
                        {
                            continue;
                        }

                        ListViewGroupCollapsedState nativeState = group.GetNativeCollapsedState();
                        if (nativeState != group.CollapsedState)
                        {
                            group.SetCollapsedStateInternal(nativeState);
                            OnGroupCollapsedStateChanged(new ListViewGroupEventArgs(Groups.IndexOf(group)));
                        }
                    }
                }

                base.WndProc(ref m);
                break;

            case PInvokeCore.WM_LBUTTONDBLCLK:

                // Ensure that the itemCollectionChangedInMouseDown is not set
                // before processing the mousedown event.
                ItemCollectionChangedInMouseDown = false;
                Capture = true;
                WmMouseDown(ref m, MouseButtons.Left, 2);
                UpdateGroupCollapse(PInvokeCore.WM_LBUTTONDBLCLK);
                break;

            case PInvokeCore.WM_LBUTTONDOWN:

                // Check that before click was handled by the ListView code
                // because otherwise item will always be selected.
                bool cancelLabelEdit =
                    LabelEdit &&
                    View == View.Details &&
                    HitTest(PARAM.ToPoint(m.LParamInternal)) is { SubItem.Index: > 0, Item.Selected: true };

                // Ensure that the itemCollectionChangedInMouseDown is not set
                // before processing the mousedown event.
                ItemCollectionChangedInMouseDown = false;
                WmMouseDown(ref m, MouseButtons.Left, 1);

                if (cancelLabelEdit)
                {
                    CancelPendingLabelEdit();
                }

                _downButton = MouseButtons.Left;
                break;

            case PInvokeCore.WM_LBUTTONUP:
            case PInvokeCore.WM_RBUTTONUP:
            case PInvokeCore.WM_MBUTTONUP:

                // See if the mouse is on the item.
                int index = UpdateGroupCollapse(PInvokeCore.WM_LBUTTONUP);

                if (!ValidationCancelled && _listViewState[LISTVIEWSTATE_doubleclickFired] && index != -1)
                {
                    _listViewState[LISTVIEWSTATE_doubleclickFired] = false;
                    OnDoubleClick(EventArgs.Empty);
                    OnMouseDoubleClick(new MouseEventArgs(_downButton, 2, PARAM.ToPoint(m.LParamInternal)));
                }

                if (!_listViewState[LISTVIEWSTATE_mouseUpFired])
                {
                    OnMouseUp(new MouseEventArgs(_downButton, 1, PARAM.ToPoint(m.LParamInternal)));
                    _listViewState[LISTVIEWSTATE_expectingMouseUp] = false;
                }

                ItemCollectionChangedInMouseDown = false;

                _listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                Capture = false;
                break;
            case PInvokeCore.WM_MBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Middle, 2);
                break;
            case PInvokeCore.WM_MBUTTONDOWN:
                WmMouseDown(ref m, MouseButtons.Middle, 1);
                _downButton = MouseButtons.Middle;
                break;
            case PInvokeCore.WM_RBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Right, 2);
                break;
            case PInvokeCore.WM_RBUTTONDOWN:
                WmMouseDown(ref m, MouseButtons.Right, 1);
                _downButton = MouseButtons.Right;
                break;
            case PInvokeCore.WM_MOUSEMOVE:
                if (_listViewState[LISTVIEWSTATE_expectingMouseUp] && !_listViewState[LISTVIEWSTATE_mouseUpFired] && MouseButtons == MouseButtons.None)
                {
                    OnMouseUp(new MouseEventArgs(_downButton, 1, PARAM.ToPoint(m.LParamInternal)));
                    _listViewState[LISTVIEWSTATE_mouseUpFired] = true;
                }

                Capture = false;
                base.WndProc(ref m);
                break;
            case PInvokeCore.WM_MOUSEHOVER:
                if (HoverSelection)
                {
                    base.WndProc(ref m);
                }
                else
                {
                    OnMouseHover(EventArgs.Empty);
                }

                break;
            case PInvokeCore.WM_NOTIFY:
                if (WmNotify(ref m))
                {
                    break; // we are done - skip default handling
                }
                else
                {
                    goto default;  // default handling needed
                }

            case PInvokeCore.WM_SETFOCUS:
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
            case PInvokeCore.WM_MOUSELEAVE:
                // if the mouse leaves and then re-enters the ListView
                // ItemHovered events should be raised.
                _prevHoveredItem = null;
                base.WndProc(ref m);
                break;

            case PInvokeCore.WM_PAINT:
                base.WndProc(ref m);

                // win32 ListView
                BeginInvoke(new MethodInvoker(CleanPreviousBackgroundImageFiles));
                break;
            case PInvokeCore.WM_PRINT:
                WmPrint(ref m);
                break;
            case PInvokeCore.WM_TIMER:
                if (m.WParamInternal != LVTOOLTIPTRACKING || !Application.ComCtlSupportsVisualStyles)
                {
                    base.WndProc(ref m);
                }

                break;
            default:
                base.WndProc(ref m);
                break;
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
