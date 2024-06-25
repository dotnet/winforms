// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows control to display a list of items.
/// </summary>
[Designer($"System.Windows.Forms.Design.ListBoxDesigner, {AssemblyRef.SystemDesign}")]
[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(Items))]
[DefaultBindingProperty(nameof(SelectedValue))]
[SRDescription(nameof(SR.DescriptionListBox))]
public partial class ListBox : ListControl
{
    /// <summary>
    ///  while doing a search, if no matches are found, this is returned
    /// </summary>
    public const int NoMatches = PInvoke.LB_ERR;

    /// <summary>
    ///  The default item height for an owner-draw ListBox. The ListBox's non-ownerdraw
    ///  item height is 13 for the default font on Windows.
    /// </summary>
    public const int DefaultItemHeight = 13;

    private static readonly object s_selectedIndexChangedEvent = new();
    private static readonly object s_drawItemEvent = new();
    private static readonly object s_measureItemEvent = new();

    private SelectedObjectCollection? _selectedItems;
    private SelectedIndexCollection? _selectedIndices;
    private ObjectCollection? _itemsCollection;

    private int _itemHeight = DefaultListBoxItemHeight;
    private int _columnWidth;
    private static int s_defaultListBoxItemHeight = -1;
    private int _requestedHeight;
    private int _topIndex;
    private int _horizontalExtent;
    private int _maxWidth = -1;
    private int _updateCount;

    private bool _sorted;
    private bool _scrollAlwaysVisible;
    private bool _integralHeight = true;
    private bool _integralHeightAdjust;
    private bool _multiColumn;
    private bool _horizontalScrollbar;
    private bool _useTabStops = true;
    private bool _useCustomTabOffsets;
    private bool _fontIsChanged;
    private bool _doubleClickFired;
    private bool _selectedValueChangedFired;

    private DrawMode _drawMode = DrawMode.Normal;
    private BorderStyle _borderStyle = BorderStyle.Fixed3D;
    private SelectionMode _selectionMode = SelectionMode.One;

    private SelectionMode _cachedSelectionMode = SelectionMode.One;

    // We need to know that we are in middle of handleRecreate through Setter of SelectionMode.
    // In this case we set a bool denoting that we are changing SelectionMode and
    // in this case we should always use the cachedValue instead of the currently set value.
    // We need to change this in the count as well as SelectedIndex code where we access the SelectionMode.
    private bool _selectionModeChanging;

    /// <summary>
    ///  This field stores focused ListBox item Accessible object before focus changing.
    ///  Used in FocusedItemIsChanged method.
    /// </summary>
    private AccessibleObject? _focusedItem;

    /// <summary>
    ///  This field stores current items count.
    ///  Used in ItemsCountIsChanged method.
    /// </summary>
    private int _itemsCount;

    /// <summary>
    ///  This value stores the array of custom tabstops in the listBox. the array should be populated by
    ///  integers in a ascending order.
    /// </summary>
    private IntegerCollection? _customTabOffsets;

    /// <summary>
    ///  Default start position of items in the checked list box
    /// </summary>
    private const int DefaultListItemStartPos = 1;

    /// <summary>
    ///  Borders are 1 pixel height.
    /// </summary>
    private const int DefaultListItemBorderHeight = 1;

    /// <summary>
    ///  Borders are 1 pixel width and a pixel buffer
    /// </summary>
    private const int DefaultListItemPaddingBuffer = 3;

    private protected int _listItemStartPosition = DefaultListItemStartPos;
    private protected int _listItemBordersHeight = 2 * DefaultListItemBorderHeight;
    private protected int _listItemPaddingBuffer = DefaultListItemPaddingBuffer;

    /// <summary>
    ///  Creates a basic win32 list box with default values for everything.
    /// </summary>
    public ListBox() : base()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, false);

        // This class overrides GetPreferredSizeCore, let Control automatically cache the result.
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        SetBounds(0, 0, 120, 96);
        _requestedHeight = Height;
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ScaleConstants();
    }

    private protected override void InitializeConstantsForInitialDpi(int initialDpi) => ScaleConstants();

    private void ScaleConstants()
    {
        // Scale paddings
        _listItemStartPosition = LogicalToDeviceUnits(DefaultListItemStartPos);

        // Height includes 2 borders (top and bottom). Multiplying by 2 instead of scaling twice guarantees an even
        // number helps in positioning the control in the center for list items.
        _listItemBordersHeight = 2 * LogicalToDeviceUnits(DefaultListItemBorderHeight);
        _listItemPaddingBuffer = LogicalToDeviceUnits(DefaultListItemPaddingBuffer);
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
                return Application.ApplicationColors.Window;
            }
        }
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
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

    /// <summary>
    ///  Retrieves the current border style.  Values for this are taken from
    ///  The System.Windows.Forms.BorderStyle enumeration.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(BorderStyle.Fixed3D)]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.ListBoxBorderDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != _borderStyle)
            {
                _borderStyle = value;
                RecreateHandle();
                // Avoid the listBox and textbox behavior in Collection editors
                //
                _integralHeightAdjust = true;
                try
                {
                    Height = _requestedHeight;
                }
                finally
                {
                    _integralHeightAdjust = false;
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(0)]
    [SRDescription(nameof(SR.ListBoxColumnWidthDescr))]
    public int ColumnWidth
    {
        get
        {
            return _columnWidth;
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_columnWidth != value)
            {
                _columnWidth = value;
                // if it's zero, we need to reset, and only way to do
                // that is to recreate the handle.
                if (_columnWidth == 0)
                {
                    RecreateHandle();
                }
                else if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.LB_SETCOLUMNWIDTH, (WPARAM)_columnWidth);
                }
            }
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_LISTBOX;

            cp.Style |= (int)WINDOW_STYLE.WS_VSCROLL | PInvoke.LBS_NOTIFY | PInvoke.LBS_HASSTRINGS;
            if (_scrollAlwaysVisible)
            {
                cp.Style |= PInvoke.LBS_DISABLENOSCROLL;
            }

            if (!_integralHeight)
            {
                cp.Style |= PInvoke.LBS_NOINTEGRALHEIGHT;
            }

            if (_useTabStops)
            {
                cp.Style |= PInvoke.LBS_USETABSTOPS;
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

            if (_multiColumn)
            {
                cp.Style |= PInvoke.LBS_MULTICOLUMN | (int)WINDOW_STYLE.WS_HSCROLL;
            }
            else if (_horizontalScrollbar)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_HSCROLL;
            }

            switch (_selectionMode)
            {
                case SelectionMode.None:
                    cp.Style |= PInvoke.LBS_NOSEL;
                    break;
                case SelectionMode.MultiSimple:
                    cp.Style |= PInvoke.LBS_MULTIPLESEL;
                    break;
                case SelectionMode.MultiExtended:
                    cp.Style |= PInvoke.LBS_EXTENDEDSEL;
                    break;
                case SelectionMode.One:
                    break;
            }

            switch (_drawMode)
            {
                case DrawMode.Normal:
                    break;
                case DrawMode.OwnerDrawFixed:
                    cp.Style |= PInvoke.LBS_OWNERDRAWFIXED;
                    break;
                case DrawMode.OwnerDrawVariable:
                    cp.Style |= PInvoke.LBS_OWNERDRAWVARIABLE;
                    break;
            }

            return cp;
        }
    }

    /// <summary>
    ///  Enables a list box to recognize and expand tab characters when drawing
    ///  its strings using the CustomTabOffsets integer array.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Browsable(false)]
    public bool UseCustomTabOffsets
    {
        get
        {
            return _useCustomTabOffsets;
        }
        set
        {
            if (_useCustomTabOffsets != value)
            {
                _useCustomTabOffsets = value;
                RecreateHandle();
            }
        }
    }

    private static int DefaultListBoxItemHeight
    {
        get
        {
            if (s_defaultListBoxItemHeight == -1)
            {
                s_defaultListBoxItemHeight = DefaultFont.Height;
            }

            return s_defaultListBoxItemHeight;
        }
    }

    protected override Size DefaultSize
    {
        get
        {
            return new Size(120, 96);
        }
    }

    /// <summary>
    ///  Retrieves the style of the listBox.  This will indicate if the system
    ///  draws it, or if the user paints each item manually.  It also indicates
    ///  whether or not items have to be of the same height.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DrawMode.Normal)]
    [SRDescription(nameof(SR.ListBoxDrawModeDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public virtual DrawMode DrawMode
    {
        get
        {
            return _drawMode;
        }

        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);
            if (_drawMode != value)
            {
                if (MultiColumn && value == DrawMode.OwnerDrawVariable)
                {
                    throw new ArgumentException(SR.ListBoxVarHeightMultiCol, nameof(value));
                }

                _drawMode = value;
                RecreateHandle();
                if (_drawMode == DrawMode.OwnerDrawVariable)
                {
                    // Force a layout after RecreateHandle() completes because now
                    // the LB is definitely fully populated and can report a preferred size accurately.
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.DrawMode);
                }
            }
        }
    }

    internal int FocusedIndex => IsHandleCreated ? (int)PInvoke.SendMessage(this, PInvoke.LB_GETCARETINDEX) : -1;

    // The scroll bars don't display properly when the IntegralHeight == false
    // and the control is resized before the font size is change and the new font size causes
    // the height of all the items to exceed the new height of the control. This is a bug in
    // the control, but can be easily worked around by removing and re-adding all the items.
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set
        {
            base.Font = value;

            if (!_integralHeight)
            {
                // Refresh the list to force the scroll bars to display
                // when the integral height is false.
                RefreshItems();
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
                return Application.ApplicationColors.WindowText;
            }
        }
        set => base.ForeColor = value;
    }

    /// <summary>
    ///  Indicates the width, in pixels, by which a list box can be scrolled horizontally (the scrollable width).
    ///  This property will only have an effect if HorizontalScrollbars is true.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxHorizontalExtentDescr))]
    public int HorizontalExtent
    {
        get
        {
            return _horizontalExtent;
        }

        set
        {
            if (value != _horizontalExtent)
            {
                _horizontalExtent = value;
                UpdateHorizontalExtent();
            }
        }
    }

    /// <summary>
    ///  Indicates whether or not the ListBox should display a horizontal scrollbar
    ///  when the items extend beyond the right edge of the ListBox.
    ///  If true, the scrollbar will automatically set its extent depending on the length
    ///  of items in the ListBox. The exception is if the ListBox is owner-draw, in
    ///  which case HorizontalExtent will need to be explicitly set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxHorizontalScrollbarDescr))]
    public bool HorizontalScrollbar
    {
        get
        {
            return _horizontalScrollbar;
        }

        set
        {
            if (value != _horizontalScrollbar)
            {
                _horizontalScrollbar = value;

                // There seems to be a bug in the native ListBox in that the addition
                // of the horizontal scroll bar does not get reflected in the control
                // right away. So, we refresh the items here.

                RefreshItems();

                // Only need to recreate the handle if not MultiColumn
                // (HorizontalScrollbar has no effect on a MultiColumn listBox)
                //
                if (!MultiColumn)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Indicates if the listBox should avoid showing partial Items.  If so,
    ///  then only full items will be displayed, and the listBox will be resized
    ///  to prevent partial items from being shown.  Otherwise, they will be
    ///  shown
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxIntegralHeightDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public bool IntegralHeight
    {
        get
        {
            return _integralHeight;
        }

        set
        {
            if (_integralHeight != value)
            {
                _integralHeight = value;
                RecreateHandle();
                // Avoid the listBox and textbox behaviour in Collection editors

                _integralHeightAdjust = true;
                try
                {
                    Height = _requestedHeight;
                }
                finally
                {
                    _integralHeightAdjust = false;
                }
            }
        }
    }

    /// <summary>
    ///  Returns
    ///  the height of an item in an owner-draw list box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxItemHeightDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public virtual int ItemHeight
    {
        get
        {
            if (_drawMode is DrawMode.OwnerDrawFixed or DrawMode.OwnerDrawVariable)
            {
                return _itemHeight;
            }

            return GetItemHeight(0);
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 255);

            if (_itemHeight != value)
            {
                _itemHeight = value;
                if (_drawMode == DrawMode.OwnerDrawFixed && IsHandleCreated)
                {
                    BeginUpdate();
                    PInvoke.SendMessage(this, PInvoke.LB_SETITEMHEIGHT, 0, value);

                    // Changing the item height might require a resize for IntegralHeight list boxes
                    if (IntegralHeight)
                    {
                        Size oldSize = Size;
                        Size = new Size(oldSize.Width + 1, oldSize.Height);
                        Size = oldSize;
                    }

                    EndUpdate();
                }
            }
        }
    }

    /// <summary>
    ///  Collection of items in this listBox.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxItemsDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MergableProperty(false)]
    public ObjectCollection Items
    {
        get
        {
            _itemsCollection ??= CreateItemCollection();

            return _itemsCollection;
        }
    }

    private bool ItemsCountIsChanged()
    {
        if (Items.Count == _itemsCount)
        {
            return false;
        }

        _itemsCount = Items.Count;
        return true;
    }

    // Computes the maximum width of all items in the ListBox
    //
    internal virtual int MaxItemWidth
    {
        get
        {
            if (_horizontalExtent > 0)
            {
                return _horizontalExtent;
            }

            if (DrawMode != DrawMode.Normal)
            {
                return -1;
            }

            // Return cached maxWidth if available
            if (_maxWidth > -1)
            {
                return _maxWidth;
            }

            // Compute maximum width
            _maxWidth = ComputeMaxItemWidth(_maxWidth);

            return _maxWidth;
        }
    }

    /// <summary>
    ///  Indicates if the listBox is multi-column
    ///  or not.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ListBoxMultiColumnDescr))]
    public bool MultiColumn
    {
        get
        {
            return _multiColumn;
        }
        set
        {
            if (_multiColumn != value)
            {
                if (value && _drawMode == DrawMode.OwnerDrawVariable)
                {
                    throw new ArgumentException(SR.ListBoxVarHeightMultiCol, nameof(value));
                }

                _multiColumn = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  The total height of the items in the list box.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxPreferredHeightDescr))]
    public int PreferredHeight
    {
        get
        {
            int height = 0;

            if (_drawMode == DrawMode.OwnerDrawVariable)
            {
                // don't try to get item heights from the LB when items haven't been
                // added to the LB yet. Just return current height.
                if (RecreatingHandle || GetState(States.CreatingHandle))
                {
                    height = Height;
                }
                else
                {
                    if (_itemsCollection is not null)
                    {
                        int count = _itemsCollection.Count;
                        for (int i = 0; i < count; i++)
                        {
                            height += GetItemHeight(i);
                        }
                    }
                }
            }
            else
            {
                height = GetItemHeight(0);

                if (_itemsCollection is not null)
                {
                    int count = _itemsCollection.Count;
                    if (count != 0)
                    {
                        height *= count;
                    }
                }
            }

            if (_borderStyle != BorderStyle.None)
            {
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

    /// <summary>
    ///  Gets or sets whether the scrollbar is shown at all times.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxScrollIsVisibleDescr))]
    public bool ScrollAlwaysVisible
    {
        get
        {
            return _scrollAlwaysVisible;
        }
        set
        {
            if (_scrollAlwaysVisible != value)
            {
                _scrollAlwaysVisible = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Indicates whether list currently allows selection of list items.
    ///  For ListBox, this returns true unless SelectionMode is SelectionMode.None.
    /// </summary>
    protected override bool AllowSelection
    {
        get
        {
            return _selectionMode != SelectionMode.None;
        }
    }

    /// <summary>
    ///  The index of the currently selected item in the list, if there
    ///  is one.  If the value is -1, there is currently no selection.  If the
    ///  value is 0 or greater, than the value is the index of the currently
    ///  selected item.  If the MultiSelect property on the ListBox is true,
    ///  then a non-zero value for this property is the index of the first
    ///  selection
    /// </summary>
    [Browsable(false)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxSelectedIndexDescr))]
    public override int SelectedIndex
    {
        get
        {
            SelectionMode current = (_selectionModeChanging) ? _cachedSelectionMode : _selectionMode;

            if (current == SelectionMode.None)
            {
                return -1;
            }

            if (current == SelectionMode.One && IsHandleCreated)
            {
                return (int)PInvoke.SendMessage(this, PInvoke.LB_GETCURSEL);
            }

            if (_itemsCollection is not null && SelectedItems.Count > 0)
            {
                return Items.IndexOfIdentifier(SelectedItems.GetObjectAt(0));
            }

            return -1;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, _itemsCollection?.Count ?? 0);

            if (_selectionMode == SelectionMode.None)
            {
                throw new ArgumentException(SR.ListBoxInvalidSelectionMode, nameof(value));
            }

            if (_selectionMode == SelectionMode.One && value != -1)
            {
                // Single select an individual value.
                int currentIndex = SelectedIndex;

                if (currentIndex != value)
                {
                    if (currentIndex != -1)
                    {
                        SelectedItems.SetSelected(currentIndex, false);
                    }

                    SelectedItems.SetSelected(value, true);

                    if (IsHandleCreated)
                    {
                        NativeSetSelected(value, true);
                    }

                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
            else if (value == -1)
            {
                if (SelectedIndex != -1)
                {
                    ClearSelected();
                    // ClearSelected raises OnSelectedIndexChanged for us
                }
            }
            else
            {
                if (!SelectedItems.GetSelected(value))
                {
                    // Select this item while keeping any previously selected items selected.
                    //
                    SelectedItems.SetSelected(value, true);
                    if (IsHandleCreated)
                    {
                        NativeSetSelected(value, true);
                    }

                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  A collection of the indices of the selected items in the
    ///  list box. If there are no selected items in the list box, the result is
    ///  an empty collection.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxSelectedIndicesDescr))]
    public SelectedIndexCollection SelectedIndices
    {
        get
        {
            _selectedIndices ??= new SelectedIndexCollection(this);

            return _selectedIndices;
        }
    }

    /// <summary>
    ///  The value of the currently selected item in the list, if there
    ///  is one.  If the value is null, there is currently no selection.  If the
    ///  value is non-null, then the value is that of the currently selected
    ///  item. If the MultiSelect property on the ListBox is true, then a
    ///  non-null return value for this method is the value of the first item
    ///  selected
    /// </summary>
    [Browsable(false)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxSelectedItemDescr))]
    public object? SelectedItem
    {
        get
        {
            if (SelectedItems.Count > 0)
            {
                return SelectedItems[0];
            }

            return null;
        }
        set
        {
            if (_itemsCollection is not null)
            {
                if (value is not null)
                {
                    int index = _itemsCollection.IndexOf(value);
                    if (index != -1)
                    {
                        SelectedIndex = index;
                    }
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
        }
    }

    /// <summary>
    ///  The collection of selected items.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxSelectedItemsDescr))]
    public SelectedObjectCollection SelectedItems
    {
        get
        {
            _selectedItems ??= new SelectedObjectCollection(this);

            return _selectedItems;
        }
    }

    /// <summary>
    ///  Controls how many items at a time can be selected in the listBox. Valid
    ///  values are from the System.Windows.Forms.SelectionMode enumeration.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(SelectionMode.One)]
    [SRDescription(nameof(SR.ListBoxSelectionModeDescr))]
    public virtual SelectionMode SelectionMode
    {
        get
        {
            return _selectionMode;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_selectionMode != value)
            {
                SelectedItems.EnsureUpToDate();
                _selectionMode = value;
                try
                {
                    _selectionModeChanging = true;
                    RecreateHandle();
                }
                finally
                {
                    _selectionModeChanging = false;
                    _cachedSelectionMode = _selectionMode;
                    // update the selectedItems list and SelectedItems index collection
                    if (IsHandleCreated)
                    {
                        NativeUpdateSelection();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Indicates if the ListBox is sorted or not.  'true' means that strings in
    ///  the list will be sorted alphabetically
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ListBoxSortedDescr))]
    public bool Sorted
    {
        get
        {
            return _sorted;
        }
        set
        {
            if (_sorted != value)
            {
                _sorted = value;

                if (_sorted && _itemsCollection is not null && _itemsCollection.Count >= 1)
                {
                    Sort();
                }
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get
        {
            if (SelectionMode != SelectionMode.None && SelectedItem is not null)
            {
                if (FormattingEnabled)
                {
                    return GetItemText(SelectedItem) ?? string.Empty;
                }
                else
                {
                    return FilterItemOnProperty(SelectedItem)?.ToString() ?? string.Empty;
                }
            }
            else
            {
                return base.Text;
            }
        }
        set
        {
            base.Text = value;

            // Scan through the list items looking for the supplied text string.  If we find it,
            // select it.
            if (SelectionMode != SelectionMode.None && value is not null && (SelectedItem is null || !value.Equals(GetItemText(SelectedItem))))
            {
                int cnt = Items.Count;
                for (int index = 0; index < cnt; ++index)
                {
                    if (string.Compare(value, GetItemText(Items[index]), true, CultureInfo.CurrentCulture) == 0)
                    {
                        SelectedIndex = index;
                        return;
                    }
                }
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  The index of the first visible item in a list box. Initially
    ///  the item with index 0 is at the top of the list box, but if the list
    ///  box contents have been scrolled another item may be at the top.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListBoxTopIndexDescr))]
    public int TopIndex
    {
        get => IsHandleCreated ? (int)PInvoke.SendMessage(this, PInvoke.LB_GETTOPINDEX) : _topIndex;
        set
        {
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.LB_SETTOPINDEX, (WPARAM)value);
            }
            else
            {
                _topIndex = value;
            }
        }
    }

    /// <summary>
    ///  Enables a list box to recognize and expand tab characters when drawing
    ///  its strings.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.ListBoxUseTabStopsDescr))]
    public bool UseTabStops
    {
        get
        {
            return _useTabStops;
        }
        set
        {
            if (_useTabStops != value)
            {
                _useTabStops = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Allows to set the width of the tabs between the items in the list box.
    ///  The integer array should have the tab spaces in the ascending order.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListBoxCustomTabOffsetsDescr))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Browsable(false)]
    public IntegerCollection CustomTabOffsets
    {
        get
        {
            _customTabOffsets ??= new IntegerCollection(this);

            return _customTabOffsets;
        }
    }

    /// <summary>
    ///  Performs the work of adding the specified items to the ListBox
    /// </summary>
    [Obsolete("This method has been deprecated.  There is no replacement.  https://go.microsoft.com/fwlink/?linkid=14202")]
    protected virtual void AddItemsCore(object[] value)
    {
        if (value is null || value.Length == 0)
        {
            return;
        }

        Items.AddRangeInternal(value);
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event MouseEventHandler? MouseClick
    {
        add => base.MouseClick += value;
        remove => base.MouseClick -= value;
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
    ///  ListBox / CheckedListBox OnPaint.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.drawItemEventDescr))]
    public event DrawItemEventHandler? DrawItem
    {
        add => Events.AddHandler(s_drawItemEvent, value);
        remove => Events.RemoveHandler(s_drawItemEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.measureItemEventDescr))]
    public event MeasureItemEventHandler? MeasureItem
    {
        add => Events.AddHandler(s_measureItemEvent, value);
        remove => Events.RemoveHandler(s_measureItemEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
    public event EventHandler? SelectedIndexChanged
    {
        add => Events.AddHandler(s_selectedIndexChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedIndexChangedEvent, value);
    }

    /// <summary>
    ///  While the preferred way to insert items is to set Items.All,
    ///  and set all the items at once, there are times when you may wish to
    ///  insert each item one at a time.  To help with the performance of this,
    ///  it is desirable to prevent the ListBox from painting during these
    ///  operations.  This method, along with EndUpdate, is the preferred
    ///  way of doing this.  Don't forget to call EndUpdate when you're done,
    ///  or else the ListBox won't paint properly afterwards.
    /// </summary>
    public void BeginUpdate()
    {
        BeginUpdateInternal();
        _updateCount++;
    }

    private void CheckIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Items.Count);
    }

    private void CheckNoDataSource()
    {
        if (DataSource is not null)
        {
            throw new ArgumentException(SR.DataSourceLocksItems);
        }
    }

    /// <summary>
    ///  constructs the new instance of the accessibility object for this control. Subclasses
    ///  should not call base.CreateAccessibilityObject.
    /// </summary>
    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ListBoxAccessibleObject(this);
    }

    protected virtual ObjectCollection CreateItemCollection()
    {
        return new ObjectCollection(this);
    }

    internal virtual int ComputeMaxItemWidth(int oldMax)
    {
        // pass LayoutUtils the collection of strings
        string?[] strings = new string[Items.Count];

        for (int i = 0; i < Items.Count; i++)
        {
            strings[i] = GetItemText(Items[i]);
        }

        Size textSize = LayoutUtils.OldGetLargestStringSizeInCollection(Font, strings);
        return Math.Max(oldMax, textSize.Width);
    }

    private void ClearListItemAccessibleObjects()
    {
        if (IsAccessibilityObjectCreated && AccessibilityObject is ListBoxAccessibleObject accessibilityObject)
        {
            accessibilityObject.ResetListItemAccessibleObjects();
        }
    }

    /// <summary>
    ///  Deselects all currently selected items.
    /// </summary>
    public void ClearSelected()
    {
        bool hadSelection = false;

        int itemCount = (_itemsCollection is null) ? 0 : _itemsCollection.Count;
        for (int x = 0; x < itemCount; x++)
        {
            if (SelectedItems.GetSelected(x))
            {
                hadSelection = true;
                SelectedItems.SetSelected(x, false);
                if (IsHandleCreated)
                {
                    NativeSetSelected(x, false);
                }
            }
        }

        if (hadSelection)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  While the preferred way to insert items is to set Items.All,
    ///  and set all the items at once, there are times when you may wish to
    ///  insert each item one at a time.  To help with the performance of this,
    ///  it is desirable to prevent the ListBox from painting during these
    ///  operations.  This method, along with BeginUpdate, is the preferred
    ///  way of doing this.  BeginUpdate should be called first, and this method
    ///  should be called when you want the control to start painting again.
    /// </summary>
    public void EndUpdate()
    {
        EndUpdateInternal();
        --_updateCount;
    }

    /// <summary>
    ///  Finds the first item in the list box that starts with the given string.
    ///  The search is not case sensitive.
    /// </summary>
    public int FindString(string s) => FindString(s, startIndex: -1);

    /// <summary>
    ///  Finds the first item after the given index which starts with the given string.
    ///  The search is not case sensitive.
    /// </summary>
    public int FindString(string s, int startIndex)
    {
        return FindStringInternal(s, _itemsCollection, startIndex, exact: false, ignoreCase: true);
    }

    /// <summary>
    ///  Finds the first item in the list box that matches the given string.
    ///  The strings must match exactly, except for differences in casing.
    /// </summary>
    public int FindStringExact(string s) => FindStringExact(s, startIndex: -1);

    /// <summary>
    ///  Finds the first item after the given index that matches the given string.
    ///  The strings must match exactly, except for differences in casing.
    /// </summary>
    public int FindStringExact(string s, int startIndex)
    {
        return FindStringInternal(s, _itemsCollection, startIndex, exact: true, ignoreCase: true);
    }

    /// <summary>
    ///  Shows whether the focused item has changed when calling OnSelectedIndexChanged event.
    /// </summary>
    private bool FocusedItemIsChanged()
    {
        if (_focusedItem == AccessibilityObject.GetFocused())
        {
            return false;
        }

        _focusedItem = AccessibilityObject.GetFocused();
        return true;
    }

    /// <summary>
    ///  Returns the height of the given item in a list box. The index parameter
    ///  is ignored if drawMode is not OwnerDrawVariable.
    /// </summary>
    public int GetItemHeight(int index)
    {
        int itemCount = (_itemsCollection is null) ? 0 : _itemsCollection.Count;

        ArgumentOutOfRangeException.ThrowIfNegative(index);

        // Note: index == 0 is OK even if the ListBox currently has no items.
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Math.Max(1, itemCount));

        if (_drawMode != DrawMode.OwnerDrawVariable)
        {
            index = 0;
        }

        if (IsHandleCreated)
        {
            int height = (int)PInvoke.SendMessage(this, PInvoke.LB_GETITEMHEIGHT, (WPARAM)index);
            if (height == -1)
            {
                throw new Win32Exception();
            }

            return height;
        }

        return _itemHeight;
    }

    /// <summary>
    ///  Retrieves a Rectangle object which describes the bounding rectangle
    ///  around an item in the list.  If the item in question is not visible,
    ///  the rectangle will be empty.
    /// </summary>
    public Rectangle GetItemRectangle(int index)
    {
        CheckIndex(index);
        RECT rect = default;
        if (PInvoke.SendMessage(this, PInvoke.LB_GETITEMRECT, (uint)index, ref rect) == 0)
        {
            return Rectangle.Empty;
        }

        return rect;
    }

    /// <summary>
    ///  List box overrides GetScaledBounds to ensure we always scale the requested
    ///  height, not the current height.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
    {
        // update bounds' height to use the requested height, not the current height.  These
        // can be different if integral height is turned on.
        bounds.Height = _requestedHeight;
        return base.GetScaledBounds(bounds, factor, specified);
    }

    /// <summary>
    ///  Tells you whether or not the item at the supplied index is selected or not.
    /// </summary>
    public bool GetSelected(int index)
    {
        CheckIndex(index);
        return GetSelectedInternal(index);
    }

    private bool GetSelectedInternal(int index)
    {
        if (IsHandleCreated)
        {
            int selection = (int)PInvoke.SendMessage(this, PInvoke.LB_GETSEL, (WPARAM)index);
            if (selection == -1)
            {
                throw new Win32Exception();
            }

            return selection > 0;
        }
        else
        {
            if (_itemsCollection is not null && SelectedItems.GetSelected(index))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    ///  Retrieves the index of the item at the given coordinates.
    /// </summary>
    public int IndexFromPoint(Point p)
    {
        return IndexFromPoint(p.X, p.Y);
    }

    /// <summary>
    ///  Retrieves the index of the item at the given coordinates.
    /// </summary>
    public int IndexFromPoint(int x, int y)
    {
        // NT4 SP6A : SendMessage Fails. So First check whether the point is in Client Co-ordinates and then
        // call SendMessage.
        PInvokeCore.GetClientRect(this, out RECT r);
        if (r.left <= x && x < r.right && r.top <= y && y < r.bottom)
        {
            int index = (int)PInvoke.SendMessage(this, PInvoke.LB_ITEMFROMPOINT, 0, PARAM.FromLowHigh(x, y));
            if (PARAM.HIWORD(index) == 0)
            {
                // Inside ListBox client area
                return PARAM.LOWORD(index);
            }
        }

        return NoMatches;
    }

    /// <summary>
    ///  Adds the given item to the native List box.
    /// </summary>
    private int NativeAdd(object item)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        int insertIndex = (int)PInvoke.SendMessage(this, PInvoke.LB_ADDSTRING, 0, GetItemText(item));
        if (insertIndex == PInvoke.LB_ERRSPACE)
        {
            throw new OutOfMemoryException();
        }

        if (insertIndex == PInvoke.LB_ERR)
        {
            // On older platforms the ListBox control returns LB_ERR if there are a
            // large number (>32000) of items. It doesn't appear to set error codes
            // appropriately, so we'll have to assume that LB_ERR corresponds to item
            // overflow.
            throw new OutOfMemoryException(SR.ListBoxItemOverflow);
        }

        return insertIndex;
    }

    /// <summary>
    ///  Clears the contents of the List box.
    /// </summary>
    private void NativeClear()
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        PInvoke.SendMessage(this, PInvoke.LB_RESETCONTENT);
    }

    /// <summary>
    ///  Get the text stored by the native control for the specified list item.
    /// </summary>
    [SkipLocalsInit]
    internal unsafe string NativeGetItemText(int index)
    {
        int maxLength = (int)PInvoke.SendMessage(this, PInvoke.LB_GETTEXTLEN, (WPARAM)index);
        if (maxLength == PInvoke.LB_ERR)
        {
            return string.Empty;
        }

        using BufferScope<char> buffer = new(stackalloc char[128], minimumLength: maxLength + 1);
        fixed (char* b = buffer)
        {
            int actualLength = (int)PInvoke.SendMessage(this, PInvoke.LB_GETTEXT, (WPARAM)index, (LPARAM)b);
            Debug.Assert(actualLength != PInvoke.LB_ERR, "Should have validated the index above");
            return actualLength == PInvoke.LB_ERR ? string.Empty : buffer[..Math.Min(maxLength, actualLength)].ToString();
        }
    }

    /// <summary>
    ///  Inserts the given item to the native List box at the index.  This asserts if the handle hasn't been
    ///  created or if the resulting insert index doesn't match the passed in index.
    /// </summary>
    private int NativeInsert(int index, object item)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        int insertIndex = (int)PInvoke.SendMessage(this, PInvoke.LB_INSERTSTRING, (uint)index, GetItemText(item));

        if (insertIndex == PInvoke.LB_ERRSPACE)
        {
            throw new OutOfMemoryException();
        }

        if (insertIndex == PInvoke.LB_ERR)
        {
            // On older platforms the ListBox control returns LB_ERR if there are a
            // large number (>32000) of items. It doesn't appear to set error codes
            // appropriately, so we'll have to assume that LB_ERR corresponds to item
            // overflow.
            throw new OutOfMemoryException(SR.ListBoxItemOverflow);
        }

        Debug.Assert(insertIndex == index, $"NativeListBox inserted at {insertIndex} not the requested index of {index}");
        return insertIndex;
    }

    /// <summary>
    ///  Removes the native item from the given index.
    /// </summary>
    private void NativeRemoveAt(int index)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");

        bool selected = (int)PInvoke.SendMessage(this, PInvoke.LB_GETSEL, (WPARAM)index) > 0;
        PInvoke.SendMessage(this, PInvoke.LB_DELETESTRING, (WPARAM)index);

        // If the item currently selected is removed then we should fire a SelectionChanged event
        // as the next time selected index returns -1.

        if (selected)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Sets the selection of the given index to the native window.  This does not change
    ///  the collection; you must update the collection yourself.
    /// </summary>
    private void NativeSetSelected(int index, bool value)
    {
        Debug.Assert(IsHandleCreated, "Should only call Native methods after the handle has been created");
        Debug.Assert(_selectionMode != SelectionMode.None, "Guard against setting selection for None selection mode outside this code.");

        if (_selectionMode == SelectionMode.One)
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETCURSEL, (WPARAM)(value ? index : -1));
        }
        else
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETSEL, (WPARAM)(BOOL)value, (LPARAM)index);
        }
    }

    /// <summary>
    ///  This is called by the SelectedObjectCollection in response to the first
    ///  query on that collection after we have called Dirty().  Dirty() is called
    ///  when we receive a LBN_SELCHANGE message.
    /// </summary>
    private unsafe void NativeUpdateSelection()
    {
        Debug.Assert(IsHandleCreated, "Should only call native methods if handle is created");

        // Clear the selection state.
        int cnt = Items.Count;
        for (int i = 0; i < cnt; i++)
        {
            SelectedItems.SetSelected(i, false);
        }

        switch (_selectionMode)
        {
            case SelectionMode.One:
                int index = (int)PInvoke.SendMessage(this, PInvoke.LB_GETCURSEL);
                if (index >= 0)
                {
                    SelectedItems.SetSelected(index, true);
                }

                break;

            case SelectionMode.MultiSimple:
            case SelectionMode.MultiExtended:
                int count = (int)PInvoke.SendMessage(this, PInvoke.LB_GETSELCOUNT);
                if (count > 0)
                {
                    int[] result = new int[count];
                    fixed (int* pResult = result)
                    {
                        PInvoke.SendMessage(this, PInvoke.LB_GETSELITEMS, (WPARAM)count, (LPARAM)pResult);
                    }

                    foreach (int i in result)
                    {
                        SelectedItems.SetSelected(i, true);
                    }
                }

                break;
        }
    }

    protected override void OnChangeUICues(UICuesEventArgs e)
    {
        // ListBox seems to get a bit confused when the UI cues change for the first
        // time - it draws the focus rect when it shouldn't and vice-versa. So when
        // the UI cues change, we just do an extra invalidate to get it into the
        // right state.
        Invalidate();

        base.OnChangeUICues(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (IsHandleCreated && IsAccessibilityObjectCreated)
        {
            AccessibleObject? item = AccessibilityObject.GetFocused();

            if (item is not null)
            {
                item.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
            else
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }

        base.OnGotFocus(e);
    }

    /// <summary>
    ///  Actually goes and fires the drawItem event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler yourself for this event].  They should,
    ///  however, remember to call base.onDrawItem(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected virtual void OnDrawItem(DrawItemEventArgs e)
    {
        ((DrawItemEventHandler?)Events[s_drawItemEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  We need to know when the window handle has been created so we can
    ///  set up a few things, like column width, etc!  Inheriting classes should
    ///  not forget to call base.OnHandleCreated().
    /// </summary>
    protected override unsafe void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // Get the current locale to set the Scrollbars
        PInvoke.SendMessage(this, PInvoke.LB_SETLOCALE, (WPARAM)PInvokeCore.GetThreadLocale());

        if (_columnWidth != 0)
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETCOLUMNWIDTH, (WPARAM)_columnWidth);
        }

        if (_drawMode == DrawMode.OwnerDrawFixed)
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETITEMHEIGHT, (WPARAM)0, (LPARAM)ItemHeight);
        }

        if (_topIndex != 0)
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETTOPINDEX, (WPARAM)_topIndex);
        }

        if (UseCustomTabOffsets && CustomTabOffsets is not null)
        {
            int wpar = CustomTabOffsets.Count;
            int[] offsets = new int[wpar];
            CustomTabOffsets.CopyTo(offsets, 0);

            fixed (int* pOffsets = offsets)
            {
                PInvoke.SendMessage(this, PInvoke.LB_SETTABSTOPS, (WPARAM)wpar, (LPARAM)pOffsets);
            }
        }

        if (_itemsCollection is not null)
        {
            int count = _itemsCollection.Count;

            for (int i = 0; i < count; i++)
            {
                NativeAdd(_itemsCollection[i]);

                if (_selectionMode != SelectionMode.None)
                {
                    _selectedItems?.PushSelectionIntoNativeListBox(i);
                }
            }
        }

        if (_selectedItems is not null)
        {
            if (_selectedItems.Count > 0 && _selectionMode == SelectionMode.One)
            {
                SelectedItems.Dirty();
                SelectedItems.EnsureUpToDate();
            }
        }

        UpdateHorizontalExtent();
    }

    /// <summary>
    ///  Overridden to make sure that we set up and clear out items
    ///  correctly.  Inheriting controls should not forget to call
    ///  base.OnHandleDestroyed()
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        SelectedItems.EnsureUpToDate();
        if (Disposing)
        {
            _itemsCollection = null;
        }

        base.OnHandleDestroyed(e);
    }

    protected virtual void OnMeasureItem(MeasureItemEventArgs e)
    {
        ((MeasureItemEventHandler?)Events[s_measureItemEvent])?.Invoke(this, e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);

        // Changing the font causes us to resize, always rounding down.
        // Make sure we do this after base.OnPropertyChanged, which sends the WM_SETFONT message

        // Avoid the listBox and textbox behaviour in Collection editors
        UpdateFontCache();
    }

    /// <summary>
    ///  We override this so we can re-create the handle if the parent has changed.
    /// </summary>
    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        // No need to RecreateHandle if we are removing the ListBox from controls collection...
        // so check the parent before recreating the handle...
        if (ParentInternal is not null)
        {
            RecreateHandle();
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        // There are some repainting issues for RightToLeft - so invalidate when we resize.
        if (RightToLeft == RightToLeft.Yes || HorizontalScrollbar)
        {
            Invalidate();
        }
    }

    /// <summary>
    ///  Actually goes and fires the selectedIndexChanged event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        if (IsHandleCreated && IsAccessibilityObjectCreated)
        {
            if (Focused && FocusedItemIsChanged())
            {
                var focused = AccessibilityObject.GetFocused();
                if (focused == AccessibilityObject.GetSelected())
                {
                    focused?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);
                }

                focused?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
            else if (ItemsCountIsChanged())
            {
                AccessibilityObject?.GetChild(Items.Count - 1)?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }

        base.OnSelectedIndexChanged(e);

        // set the position in the dataSource, if there is any
        // we will only set the position in the currencyManager if it is different
        // from the SelectedIndex. Setting CurrencyManager::Position (even w/o changing it)
        // calls CurrencyManager::EndCurrentEdit, and that will pull the dataFrom the controls
        // into the backEnd. We do not need to do that.
        if (DataManager is not null && DataManager.Position != SelectedIndex)
        {
            // read this as "if everett or   (whidbey and selindex is valid)"
            if (!FormattingEnabled || SelectedIndex != -1)
            {
                // Don't change dataManager position if we simply unselected everything.
                // (Doing so would cause the first LB item to be selected...)
                DataManager.Position = SelectedIndex;
            }
        }

        // Call the handler after updating the DataManager's position so that
        // the DataManager's selected index will be correct in an event handler.
        ((EventHandler?)Events[s_selectedIndexChangedEvent])?.Invoke(this, e);
    }

    protected override void OnSelectedValueChanged(EventArgs e)
    {
        base.OnSelectedValueChanged(e);
        _selectedValueChangedFired = true;
    }

    protected override void OnDataSourceChanged(EventArgs e)
    {
        if (DataSource is null)
        {
            BeginUpdate();
            SelectedIndex = -1;
            Items.ClearInternal();
            EndUpdate();
        }

        base.OnDataSourceChanged(e);
        RefreshItems();
    }

    protected override void OnDisplayMemberChanged(EventArgs e)
    {
        base.OnDisplayMemberChanged(e);

        // we want to use the new DisplayMember even if there is no data source
        RefreshItems();

        if (SelectionMode != SelectionMode.None && DataManager is not null)
        {
            SelectedIndex = DataManager.Position;
        }
    }

    /// <summary>
    ///  Forces the ListBox to invalidate and immediately
    ///  repaint itself and any children if OwnerDrawVariable.
    /// </summary>
    public override void Refresh()
    {
        if (_drawMode == DrawMode.OwnerDrawVariable)
        {
            // Fire MeasureItem for Each Item in the ListBox...
            int cnt = Items.Count;
            using Graphics graphics = CreateGraphicsInternal();

            for (int i = 0; i < cnt; i++)
            {
                MeasureItemEventArgs mie = new(graphics, i, ItemHeight);
                OnMeasureItem(mie);
            }
        }

        base.Refresh();
    }

    /// <summary>
    ///  Reparses the objects, getting new text strings for them.
    /// </summary>
    protected override void RefreshItems()
    {
        // Store the currently selected object collection.
        ObjectCollection? savedItems = _itemsCollection;

        // Clear the items.
        _itemsCollection = null;
        _selectedIndices = null;

        if (IsHandleCreated)
        {
            NativeClear();
        }

        object[]? newItems = null;

        // If we have a DataSource and a DisplayMember, then use it
        // to populate the Items collection
        if (DataManager is not null && DataManager.Count != -1)
        {
            newItems = new object[DataManager.Count];
            for (int i = 0; i < newItems.Length; i++)
            {
                newItems[i] = DataManager[i]!;
            }
        }
        else if (savedItems is not null)
        {
            newItems = new object[savedItems.Count];
            savedItems.CopyTo(newItems, 0);
        }

        // Store the current list of items
        if (newItems is not null)
        {
            Items.AddRangeInternal(newItems);
        }

        // Restore the selected indices if SelectionMode allows it.
        if (SelectionMode != SelectionMode.None)
        {
            if (DataManager is not null)
            {
                // Put the selectedIndex in sync with the position in the dataManager
                SelectedIndex = DataManager.Position;
            }
            else
            {
                if (savedItems is not null)
                {
                    int cnt = savedItems.Count;
                    for (int index = 0; index < cnt; index++)
                    {
                        if (savedItems.InnerArray.GetState(index, SelectedObjectCollection.SelectedObjectMask))
                        {
                            SelectedItem = savedItems[index];
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Reparses the object at the given index, getting new text string for it.
    /// </summary>
    protected override void RefreshItem(int index)
    {
        Items.SetItemInternal(index, Items[index]);
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        ClearListItemAccessibleObjects();
        base.ReleaseUiaProvider(handle);
    }

    private void RemoveListItemAccessibleObjectAt(int index)
    {
        if (IsAccessibilityObjectCreated && AccessibilityObject is ListBoxAccessibleObject accessibilityObject)
        {
            accessibilityObject.RemoveListItemAccessibleObjectAt(index);
        }
    }

    public override void ResetBackColor()
    {
        base.ResetBackColor();
    }

    public override void ResetForeColor()
    {
        base.ResetForeColor();
    }

    // ShouldSerialize and Reset Methods are being used by Designer via reflection.
    private void ResetItemHeight()
    {
        _itemHeight = DefaultListBoxItemHeight;
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        if (factor.Width != 1F && factor.Height != 1F)
        {
            UpdateFontCache();
        }

        base.ScaleControl(factor, specified);
    }

    /// <summary>
    ///  Overrides Control.SetBoundsCore to remember the requestedHeight.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        // Avoid the listBox and textbox behaviour in Collection editors
        if (!_integralHeightAdjust && height != Height)
        {
            _requestedHeight = height;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    ///  Performs the work of setting the specified items into the ListBox.
    /// </summary>
    protected override void SetItemsCore(IList value)
    {
        ArgumentNullException.ThrowIfNull(value);

        BeginUpdate();
        Items.ClearInternal();
        Items.AddRangeInternal(value);

        SelectedItems.Dirty();

        // if the list changed, we want to keep the same selected index
        // CurrencyManager will provide the PositionChanged event
        // it will be provided before changing the list though...
        if (DataManager is not null)
        {
            if (DataSource is ICurrencyManagerProvider)
            {
                _selectedValueChangedFired = false;
            }

            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.LB_SETCURSEL, (WPARAM)DataManager.Position);
            }

            // if the list changed and we still did not fire the
            // onSelectedChanged event, then fire it now;
            if (!_selectedValueChangedFired)
            {
                OnSelectedValueChanged(EventArgs.Empty);
                _selectedValueChangedFired = false;
            }
        }

        EndUpdate();
    }

    protected override void SetItemCore(int index, object value)
    {
        Items.SetItemInternal(index, value);
    }

    /// <summary>
    ///  Allows the user to set an item as being selected or not.  This should
    ///  only be used with ListBoxes that allow some sort of multi-selection.
    /// </summary>
    public void SetSelected(int index, bool value)
    {
        int itemCount = (_itemsCollection is null) ? 0 : _itemsCollection.Count;

        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, itemCount);

        if (_selectionMode == SelectionMode.None)
        {
            throw new InvalidOperationException(SR.ListBoxInvalidSelectionMode);
        }

        SelectedItems.SetSelected(index, value);
        if (IsHandleCreated)
        {
            NativeSetSelected(index, value);
        }

        SelectedItems.Dirty();
        OnSelectedIndexChanged(EventArgs.Empty);
    }

    // ShouldSerialize and Reset Methods are being used by Designer via reflection.
    private bool ShouldSerializeItemHeight()
    {
        return ItemHeight != DefaultListBoxItemHeight && _drawMode != DrawMode.Normal;
    }

    /// <summary>
    ///  Sorts the items in the listBox.
    /// </summary>
    protected virtual void Sort()
    {
        // This will force the collection to add each item back to itself
        // if sorted is now true, then the add method will insert the item
        // into the correct position
        CheckNoDataSource();

        SelectedObjectCollection currentSelections = SelectedItems;
        currentSelections.EnsureUpToDate();

        if (_sorted && _itemsCollection is not null)
        {
            _itemsCollection.InnerArray.Sort();

            // Now that we've sorted, update our handle
            // if it has been created.
            if (IsHandleCreated)
            {
                NativeClear();
                int count = _itemsCollection.Count;
                for (int i = 0; i < count; i++)
                {
                    NativeAdd(_itemsCollection[i]);
                    if (currentSelections.GetSelected(i))
                    {
                        NativeSetSelected(i, true);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        if (_itemsCollection is not null)
        {
            s += $", Items.Count: {Items.Count}";
            if (Items.Count > 0)
            {
                string? z = GetItemText(Items[0]);
                if (z is not null)
                {
                    ReadOnlySpan<char> txt = (z.Length > 40) ? z.AsSpan(0, 40) : z;
                    s += $", Items[0]: {txt}";
                }
            }
        }

        return s;
    }

    private void UpdateFontCache()
    {
        _fontIsChanged = true;
        _integralHeightAdjust = true;
        try
        {
            Height = _requestedHeight;
        }
        finally
        {
            _integralHeightAdjust = false;
        }

        _maxWidth = -1;
        UpdateHorizontalExtent();
        // clear the preferred size cache.
        CommonProperties.xClearPreferredSizeCache(this);
    }

    private void UpdateHorizontalExtent()
    {
        if (!_multiColumn && _horizontalScrollbar && IsHandleCreated)
        {
            int width = _horizontalExtent;
            if (width == 0)
            {
                width = MaxItemWidth;
            }

            PInvoke.SendMessage(this, PInvoke.LB_SETHORIZONTALEXTENT, (WPARAM)width);
        }
    }

    // Updates the cached max item width
    //
    private void UpdateMaxItemWidth(object item, bool removing)
    {
        // We shouldn't be caching maxWidth if we don't have horizontal scrollbars,
        // or horizontal extent has been set
        if (!_horizontalScrollbar || _horizontalExtent > 0)
        {
            _maxWidth = -1;
            return;
        }

        // Only update if we are currently caching maxWidth
        //
        if (_maxWidth > -1)
        {
            // Compute item width
            //
            int width;
            using (Graphics graphics = CreateGraphicsInternal())
            {
                width = (int)(Math.Ceiling(graphics.MeasureString(GetItemText(item), Font).Width));
            }

            if (removing)
            {
                // We're removing this item, so if it's the longest
                // in the list, reset the cache
                if (width >= _maxWidth)
                {
                    _maxWidth = -1;
                }
            }
            else
            {
                // We're adding or inserting this item - update the cache
                if (width > _maxWidth)
                {
                    _maxWidth = width;
                }
            }
        }
    }

    private unsafe void UpdateCustomTabOffsets()
    {
        if (IsHandleCreated && UseCustomTabOffsets && CustomTabOffsets is not null)
        {
            int wpar = CustomTabOffsets.Count;
            int[] offsets = new int[wpar];
            CustomTabOffsets.CopyTo(offsets, 0);
            fixed (int* pOffsets = offsets)
            {
                PInvoke.SendMessage(this, PInvoke.LB_SETTABSTOPS, (WPARAM)wpar, (nint)pOffsets);
            }

            Invalidate();
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

    protected virtual void WmReflectCommand(ref Message m)
    {
        switch ((uint)m.WParamInternal.HIWORD)
        {
            case PInvoke.LBN_SELCHANGE:
                _selectedItems?.Dirty();
                OnSelectedIndexChanged(EventArgs.Empty);
                break;
            case PInvoke.LBN_DBLCLK:
                // Handle this inside WM_LBUTTONDBLCLK
                // OnDoubleClick(EventArgs.Empty);
                break;
        }
    }

    private unsafe void WmReflectDrawItem(ref Message m)
    {
        DRAWITEMSTRUCT* dis = (DRAWITEMSTRUCT*)(nint)m.LParamInternal;

        Rectangle bounds = dis->rcItem;
        if (HorizontalScrollbar)
        {
            bounds.Width = MultiColumn ? Math.Max(ColumnWidth, bounds.Width) : Math.Max(MaxItemWidth, bounds.Width);
        }

        using DrawItemEventArgs e = new(
            dis->hDC,
            Font,
            bounds,
            dis->itemID,
            dis->itemState,
            ForeColor,
            BackColor);

        OnDrawItem(e);

        m.ResultInternal = (LRESULT)1;
    }

    // This method is only called if in owner draw mode
    private unsafe void WmReflectMeasureItem(ref Message m)
    {
        MEASUREITEMSTRUCT* mis = (MEASUREITEMSTRUCT*)(nint)m.LParamInternal;

        if (_drawMode == DrawMode.OwnerDrawVariable && mis->itemID >= 0)
        {
            using Graphics graphics = CreateGraphicsInternal();
            MeasureItemEventArgs mie = new(graphics, (int)mis->itemID, ItemHeight);
            OnMeasureItem(mie);
            mis->itemHeight = (uint)mie.ItemHeight;
        }
        else
        {
            mis->itemHeight = (uint)ItemHeight;
        }

        m.ResultInternal = (LRESULT)1;
    }

    /// <summary>
    ///  The list's window procedure.  Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the list continues to function properly.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_COMMAND:
                WmReflectCommand(ref m);
                break;
            case MessageId.WM_REFLECT_DRAWITEM:
                WmReflectDrawItem(ref m);
                break;
            case MessageId.WM_REFLECT_MEASUREITEM:
                WmReflectMeasureItem(ref m);
                break;
            case PInvoke.WM_PRINT:
                WmPrint(ref m);
                break;
            case PInvoke.WM_LBUTTONDOWN:
                _selectedItems?.Dirty();
                base.WndProc(ref m);
                break;
            case PInvoke.WM_LBUTTONUP:
                Point point = PARAM.ToPoint(m.LParamInternal);
                bool captured = Capture;
                if (captured && PInvoke.WindowFromPoint(PointToScreen(point)) == HWND)
                {
                    if (!_doubleClickFired && !ValidationCancelled)
                    {
                        OnClick(new MouseEventArgs(MouseButtons.Left, 1, point));
                        OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, point));
                    }
                    else
                    {
                        _doubleClickFired = false;

                        // WM_COMMAND is only fired if the user double clicks an item,
                        // so we can't use that as a double-click substitute.
                        if (!ValidationCancelled)
                        {
                            OnDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, point));
                            OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, point));
                        }
                    }
                }

                // If this control has been disposed in the user's event handler, then we need to ignore the WM_LBUTTONUP
                // message to avoid exceptions thrown as a result of handle re-creation.
                // We handle this situation here and not at the top of the window procedure since this is the only place
                // where we can get disposed as an effect of external code (form.Close() for instance) and then pass the
                // message to the base class.

                if (GetState(States.Disposed))
                {
                    base.DefWndProc(ref m);
                }
                else
                {
                    base.WndProc(ref m);
                }

                _doubleClickFired = false;
                break;

            case PInvoke.WM_RBUTTONUP:
                if (Capture && PInvoke.WindowFromPoint(PointToScreen((Point)m.LParamInternal)) == HWND)
                {
                    _selectedItems?.Dirty();
                }

                base.WndProc(ref m);
                break;

            case PInvoke.WM_LBUTTONDBLCLK:
                // The ListBox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP sequence for
                // doubleClick. The first WM_LBUTTONUP, resets the flag for double click so its necessary for us
                // to set it again.
                _doubleClickFired = true;
                base.WndProc(ref m);
                break;

            case PInvoke.WM_WINDOWPOSCHANGED:
                base.WndProc(ref m);
                if (_integralHeight && _fontIsChanged)
                {
                    Height = Math.Max(Height, ItemHeight);
                    _fontIsChanged = false;
                }

                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }
}
