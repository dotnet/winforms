// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Implements an item of a <see cref="Forms.ListView"/>.
/// </summary>
[TypeConverter(typeof(ListViewItemConverter))]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultProperty(nameof(Text))]
[Serializable] // This type is participating in resx serialization scenarios.
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public partial class ListViewItem : ICloneable, ISerializable
{
    private const int MaxSubItems = 4096;

    private static readonly BitVector32.Section s_stateSelectedSection = BitVector32.CreateSection(1);
    private static readonly BitVector32.Section s_stateImageMaskSet = BitVector32.CreateSection(1, s_stateSelectedSection);
    private static readonly BitVector32.Section s_stateWholeRowOneStyleSection = BitVector32.CreateSection(1, s_stateImageMaskSet);
    private static readonly BitVector32.Section s_savedStateImageIndexSection = BitVector32.CreateSection(15, s_stateWholeRowOneStyleSection);
    private static readonly BitVector32.Section s_subItemCountSection = BitVector32.CreateSection(MaxSubItems, s_savedStateImageIndexSection);

    private int _indentCount;
    private Point _position = new(-1, -1);

    internal ListView? _listView;

    internal ListViewGroup? _group;
    private string? _groupName;

    private ListViewSubItemCollection? _listViewSubItemCollection;
    private List<ListViewSubItem> _subItems = [];

    // we stash the last index we got as a seed to GetDisplayIndex.
    private int _lastIndex = -1;

    // An ID unique relative to a given list view that comctl uses to identify items.
    internal int _id = -1;

    private BitVector32 _state;
    private ListViewItemImageIndexer? _imageIndexer;
    private string _toolTipText = string.Empty;
    private object? _userData;

    private AccessibleObject? _accessibilityObject;
    private View _accessibilityObjectView;

    public ListViewItem()
    {
        StateSelected = false;
        UseItemStyleForSubItems = true;
        SavedStateImageIndex = -1;
    }

    /// <summary>
    ///  Creates a ListViewItem object from an Stream.
    /// </summary>
    protected ListViewItem(SerializationInfo info, StreamingContext context)
        : this()
    {
        Deserialize(info, context);
    }

    public ListViewItem(string? text)
        : this(text, ImageList.Indexer.DefaultIndex)
    {
    }

    public ListViewItem(string? text, int imageIndex)
        : this()
    {
        ImageIndexer.Index = imageIndex;
        Text = text;
    }

    public ListViewItem(string[]? items)
        : this(items, ImageList.Indexer.DefaultIndex)
    {
    }

    public ListViewItem(string[]? items, int imageIndex)
        : this()
    {
        ImageIndexer.Index = imageIndex;
        if (items is not null && items.Length > 0)
        {
            _subItems.EnsureCapacity(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                _subItems.Add(new ListViewSubItem(this, items[i]));
            }

            SubItemCount = items.Length;
        }
    }

    public ListViewItem(string[]? items, int imageIndex, Color foreColor, Color backColor, Font? font)
        : this(items, imageIndex)
    {
        ForeColor = foreColor;
        BackColor = backColor;
        Font = font;
    }

    public ListViewItem(ListViewSubItem[] subItems, int imageIndex)
        : this()
    {
        ArgumentNullException.ThrowIfNull(subItems);

        ImageIndexer.Index = imageIndex;
        SubItemCount = subItems.Length;

        // Update the owner of these subitems
        for (int i = 0; i < subItems.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(subItems[i], nameof(subItems));

            subItems[i]._owner = this;
            _subItems.Add(subItems[i]);
        }
    }

    public ListViewItem(ListViewGroup? group)
        : this()
    {
        Group = group;
    }

    public ListViewItem(string? text, ListViewGroup? group)
        : this(text)
    {
        Group = group;
    }

    public ListViewItem(string? text, int imageIndex, ListViewGroup? group)
        : this(text, imageIndex)
    {
        Group = group;
    }

    public ListViewItem(string[]? items, ListViewGroup? group)
        : this(items)
    {
        Group = group;
    }

    public ListViewItem(string[]? items, int imageIndex, ListViewGroup? group)
        : this(items, imageIndex)
    {
        Group = group;
    }

    public ListViewItem(string[]? items, int imageIndex, Color foreColor, Color backColor, Font? font, ListViewGroup? group)
        : this(items, imageIndex, foreColor, backColor, font)
    {
        Group = group;
    }

    public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup? group)
        : this(subItems, imageIndex)
    {
        Group = group;
    }

    public ListViewItem(string? text, string? imageKey)
        : this()
    {
        ImageIndexer.Key = imageKey;
        Text = text;
    }

    public ListViewItem(string[]? items, string? imageKey)
        : this()
    {
        ImageIndexer.Key = imageKey;
        if (items is not null && items.Length > 0)
        {
            _subItems = new List<ListViewSubItem>(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                _subItems.Add(new ListViewSubItem(this, items[i]));
            }

            SubItemCount = items.Length;
        }
    }

    public ListViewItem(string[]? items, string? imageKey, Color foreColor, Color backColor, Font? font)
        : this(items, imageKey)
    {
        ForeColor = foreColor;
        BackColor = backColor;
        Font = font;
    }

    public ListViewItem(ListViewSubItem[] subItems, string? imageKey)
        : this()
    {
        ArgumentNullException.ThrowIfNull(subItems);

        ImageIndexer.Key = imageKey;
        SubItemCount = subItems.Length;

        // Update the owner of these subitems
        for (int i = 0; i < subItems.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(subItems[i], nameof(subItems));

            subItems[i]._owner = this;
            _subItems.Add(subItems[i]);
        }
    }

    public ListViewItem(string? text, string? imageKey, ListViewGroup? group)
        : this(text, imageKey)
    {
        Group = group;
    }

    public ListViewItem(string[]? items, string? imageKey, ListViewGroup? group)
        : this(items, imageKey)
    {
        Group = group;
    }

    public ListViewItem(string[]? items, string? imageKey, Color foreColor, Color backColor, Font? font, ListViewGroup? group)
        : this(items, imageKey, foreColor, backColor, font)
    {
        Group = group;
    }

    public ListViewItem(ListViewSubItem[] subItems, string? imageKey, ListViewGroup? group)
        : this(subItems, imageKey)
    {
        Group = group;
    }

    internal virtual AccessibleObject AccessibilityObject
    {
        get
        {
            ListView owningListView = _listView ?? Group?.ListView
                ?? throw new InvalidOperationException(SR.ListViewItemAccessibilityObjectRequiresListView);

            if (_accessibilityObject is null || owningListView.View != _accessibilityObjectView)
            {
                _accessibilityObjectView = owningListView.View;
                _accessibilityObject = _accessibilityObjectView switch
                {
                    View.Details => new ListViewItemDetailsAccessibleObject(this),
                    View.LargeIcon => new ListViewItemLargeIconAccessibleObject(this),
                    View.List => new ListViewItemListAccessibleObject(this),
                    View.SmallIcon => new ListViewItemSmallIconAccessibleObject(this),
                    View.Tile => new ListViewItemTileAccessibleObject(this),
                    _ => throw new InvalidOperationException()
                };
            }

            return _accessibilityObject;
        }
    }

    private bool IsAccessibilityObjectCreated => _accessibilityObject is not null;

    /// <summary>
    ///  The font that this item will be displayed in. If its value is null, it will be displayed
    ///  using the global font for the ListView control that hosts it.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatAppearance))]
    public Color BackColor
    {
        get
        {
            if (SubItemCount == 0)
            {
                if (_listView is not null)
                {
                    return _listView.BackColor;
                }

                return Application.ApplicationColors.Window;
            }
            else
            {
                return _subItems[0].BackColor;
            }
        }
        set => SubItems[0].BackColor = value;
    }

    /// <summary>
    ///  Returns the ListViewItem's bounding rectangle, including subitems. The bounding rectangle is empty if
    ///  the ListViewItem has not been added to a ListView control.
    /// </summary>
    [Browsable(false)]
    public Rectangle Bounds
    {
        get
        {
            if (_listView is not null)
            {
                return _listView.GetItemRect(Index);
            }
            else
            {
                return default;
            }
        }
    }

    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool Checked
    {
        get => StateImageIndex > 0;
        set
        {
            if (Checked != value)
            {
                if (_listView is not null && _listView.IsHandleCreated)
                {
                    StateImageIndex = value ? 1 : 0;

                    // the setter for StateImageIndex calls ItemChecked handler
                    // thus need to verify validity of the listView again
                    if (_listView is not null && !_listView.UseCompatibleStateImageBehavior)
                    {
                        if (!_listView.CheckBoxes)
                        {
                            _listView.UpdateSavedCheckedItems(this, value);
                        }
                    }
                }
                else
                {
                    SavedStateImageIndex = value ? 1 : 0;
                }
            }
        }
    }

    /// <summary>
    ///  Returns the focus state of the ListViewItem.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public bool Focused
    {
        get
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                return _listView.GetItemState(Index, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_FOCUSED) != 0;
            }

            return false;
        }

        set
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                _listView.SetItemState(Index, value ? LIST_VIEW_ITEM_STATE_FLAGS.LVIS_FOCUSED : 0, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_FOCUSED);

                if (_listView.IsAccessibilityObjectCreated)
                {
                    AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
                }
            }
        }
    }

    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatAppearance))]
    [AllowNull]
    public Font Font
    {
        get
        {
            if (SubItemCount == 0)
            {
                if (_listView is not null)
                {
                    return _listView.Font;
                }

                return Control.DefaultFont;
            }
            else
            {
                return _subItems[0].Font;
            }
        }
        set => SubItems[0].Font = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatAppearance))]
    public Color ForeColor
    {
        get
        {
            if (SubItemCount == 0)
            {
                if (_listView is not null)
                {
                    return _listView.ForeColor;
                }

                return Application.ApplicationColors.WindowText;
            }
            else
            {
                return _subItems[0].ForeColor;
            }
        }
        set
        {
            SubItems[0].ForeColor = value;
        }
    }

    [DefaultValue(null)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    public ListViewGroup? Group
    {
        get => _group;
        set
        {
            if (_group != value)
            {
                if (value is not null)
                {
                    value.Items.Add(this);
                }
                else
                {
                    _group!.Items.Remove(this);
                }
            }

            Debug.Assert(_group == value, "BUG: group member variable wasn't updated!");

            // If the user specifically sets the group then don't use the groupName again.
            _groupName = null;
        }
    }

    /// <summary>
    ///  Returns the ListViewItem's currently set image index
    /// </summary>
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewItemImageIndexDescr))]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    public int ImageIndex
    {
        get
        {
            return ImageList is null || ImageIndexer.Index < ImageList.Images.Count
                ? ImageIndexer.Index
                : ImageList.Images.Count - 1;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            ImageIndexer.Index = value;

            if (_listView is not null && _listView.IsHandleCreated)
            {
                _listView.SetItemImage(itemIndex: Index, imageIndex: ImageIndexer.ActualIndex);
            }
        }
    }

    internal ListViewItemImageIndexer ImageIndexer => _imageIndexer ??= new ListViewItemImageIndexer(this);

    /// <summary>
    ///  Returns the ListViewItem's currently set image index
    /// </summary>
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    public string ImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            ImageIndexer.Key = value;

            if (_listView is not null && _listView.IsHandleCreated)
            {
                _listView.SetItemImage(Index, ImageIndexer.ActualIndex);
            }
        }
    }

    [Browsable(false)]
    public ImageList? ImageList
    {
        get
        {
            if (_listView is not null)
            {
                switch (_listView.View)
                {
                    case View.LargeIcon:
                    case View.Tile:
                        return _listView.LargeImageList;
                    case View.SmallIcon:
                    case View.Details:
                    case View.List:
                        return _listView.SmallImageList;
                }
            }

            return null;
        }
    }

    [DefaultValue(0)]
    [SRDescription(nameof(SR.ListViewItemIndentCountDescr))]
    [SRCategory(nameof(SR.CatDisplay))]
    public int IndentCount
    {
        get => _indentCount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(IndentCount), SR.ListViewIndentCountCantBeNegative);
            }

            if (value != _indentCount)
            {
                _indentCount = value;
                if (_listView is not null && _listView.IsHandleCreated)
                {
                    _listView.SetItemIndentCount(Index, _indentCount);
                }
            }
        }
    }

    /// <summary>
    ///  Returns ListViewItem's current index in the listview, or -1 if it has not been added to a ListView control.
    /// </summary>
    [Browsable(false)]
    public int Index
    {
        get
        {
            if (_listView is not null)
            {
                // if the list is virtual, the ComCtrl control does not keep any information
                // about any list view items, so we use our cache instead.
                if (!_listView.VirtualMode)
                {
                    _lastIndex = _listView.GetDisplayIndex(this, _lastIndex);
                }

                return _lastIndex;
            }
            else
            {
                return -1;
            }
        }
    }

    /// <summary>
    ///  Returns the ListView control that holds this ListViewItem. May be null if no
    ///  control has been assigned yet.
    /// </summary>
    [Browsable(false)]
    public ListView? ListView => _listView;

    /// <summary>
    ///  Name associated with this ListViewItem
    /// </summary>
    [Localizable(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public string Name
    {
        get
        {
            if (SubItemCount == 0)
            {
                return string.Empty;
            }
            else
            {
                return _subItems[0].Name;
            }
        }
        set => SubItems[0].Name = value;
    }

    [SRCategory(nameof(SR.CatDisplay))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public Point Position
    {
        get
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                _position = _listView.GetItemPosition(Index);
            }

            return _position;
        }
        set
        {
            if (!value.Equals(_position))
            {
                _position = value;
                if (_listView is not null && _listView.IsHandleCreated)
                {
                    if (!_listView.VirtualMode)
                    {
                        _listView.SetItemPosition(Index, _position.X, _position.Y);
                    }
                }
            }
        }
    }

    internal LIST_VIEW_ITEM_STATE_FLAGS RawStateImageIndex => (LIST_VIEW_ITEM_STATE_FLAGS)((SavedStateImageIndex + 1) << 12);

    /// <summary>
    ///  Accessor for our state bit vector.
    /// </summary>
    private int SavedStateImageIndex
    {
        get
        {
            // State goes from zero to 15, but we need a negative
            // number, so we store + 1.
            return _state[s_savedStateImageIndexSection] - 1;
        }
        set
        {
            // flag whether we've set a value.
            _state[s_stateImageMaskSet] = (value == ImageList.Indexer.DefaultIndex ? 0 : 1);

            // push in the actual value
            _state[s_savedStateImageIndexSection] = value + 1;
        }
    }

    /// <summary>
    ///  Treats the ListViewItem as a row of strings, and returns an array of those strings
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Selected
    {
        get
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                return _listView.GetItemState(Index, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED) != 0;
            }

            return StateSelected;
        }
        set
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                _listView.SetItemState(Index, value ? LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED : 0, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED);

                // update comctl32's selection information.
                _listView.SetSelectionMark(Index);
            }
            else
            {
                StateSelected = value;
                if (_listView is not null && _listView.IsHandleCreated)
                {
                    // Set the selected state on the list view item only if the list view's Handle is already created.
                    _listView.CacheSelectedStateForItem(this, value);
                }
            }
        }
    }

    [Localizable(true)]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [SRDescription(nameof(SR.ListViewItemStateImageIndexDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RelatedImageList("ListView.StateImageList")]
    public int StateImageIndex
    {
        get
        {
            if (_listView is not null && _listView.IsHandleCreated)
            {
                LIST_VIEW_ITEM_STATE_FLAGS state = _listView.GetItemState(Index, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK);
                return (((int)state >> 12) - 1);   // index is 1-based
            }

            return SavedStateImageIndex;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 14);

            if (_listView is not null && _listView.IsHandleCreated)
            {
                _state[s_stateImageMaskSet] = (value == ImageList.Indexer.DefaultIndex ? 0 : 1);
                LIST_VIEW_ITEM_STATE_FLAGS state = (LIST_VIEW_ITEM_STATE_FLAGS)((value + 1) << 12);  // index is 1-based
                _listView.SetItemState(Index, state, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK);
            }

            SavedStateImageIndex = value;
        }
    }

    internal bool StateImageSet => (_state[s_stateImageMaskSet] != 0);

    /// <summary>
    ///  Accessor for our state bit vector.
    /// </summary>
    internal bool StateSelected
    {
        get => _state[s_stateSelectedSection] == 1;
        set => _state[s_stateSelectedSection] = value ? 1 : 0;
    }

    /// <summary>
    ///  Accessor for our state bit vector.
    /// </summary>
    private int SubItemCount // Do NOT rename (binary serialization).
    {
        get => _state[s_subItemCountSection];
        set => _state[s_subItemCountSection] = value;
    }

    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListViewItemSubItemsDescr))]
    [Editor($"System.Windows.Forms.Design.ListViewSubItemCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    public ListViewSubItemCollection SubItems
    {
        get
        {
            if (SubItemCount == 0)
            {
                _subItems = [new ListViewSubItem(this, string.Empty)];
                SubItemCount = 1;
            }

            return _listViewSubItemCollection ??= new ListViewSubItemCollection(this);
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get => _userData;
        set => _userData = value;
    }

    /// <summary>
    ///  Text associated with this ListViewItem
    /// </summary>
    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatAppearance))]
    [AllowNull]
    public string Text
    {
        get
        {
            if (SubItemCount == 0)
            {
                return string.Empty;
            }
            else
            {
                return _subItems[0].Text;
            }
        }
        set => SubItems[0].Text = value;
    }

    /// <summary>
    ///  Tool tip text associated with this ListViewItem
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue("")]
    [AllowNull]
    public string ToolTipText
    {
        get => _toolTipText;
        set
        {
            value ??= string.Empty;

            if (!WindowsFormsUtils.SafeCompareStrings(_toolTipText, value, ignoreCase: false))
            {
                _toolTipText = value;

                // tell the list view about this change
                if (_listView is not null && _listView.IsHandleCreated)
                {
                    _listView.ListViewItemToolTipChanged(this);
                }
            }
        }
    }

    /// <summary>
    ///  Whether or not the font and coloring for the ListViewItem will be used for all of its subitems.
    ///  If true, the ListViewItem style will be used when drawing the subitems.
    ///  If false, the ListViewItem and its subitems will be drawn in their own individual styles
    ///  if any have been set.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool UseItemStyleForSubItems
    {
        get => _state[s_stateWholeRowOneStyleSection] == 1;
        set => _state[s_stateWholeRowOneStyleSection] = value ? 1 : 0;
    }

    /// <summary>
    ///  Initiate editing of the item's label. Only effective if LabelEdit property is true.
    /// </summary>
    public void BeginEdit()
    {
        if (Index >= 0)
        {
            ListView lv = ListView!;
            if (!lv.LabelEdit)
            {
                throw new InvalidOperationException(SR.ListViewBeginEditFailed);
            }

            if (!lv.Focused)
            {
                lv.Focus();
            }

            PInvoke.SendMessage(lv, PInvoke.LVM_EDITLABELW, (WPARAM)Index);
        }
    }

    public virtual object Clone()
    {
        ListViewSubItem[] clonedSubItems = new ListViewSubItem[SubItems.Count];
        for (int index = 0; index < SubItems.Count; ++index)
        {
            ListViewSubItem subItem = SubItems[index];
            clonedSubItems[index] = new ListViewSubItem(
                owner: null,
                subItem.Text,
                subItem.ForeColor,
                subItem.BackColor,
                subItem.Font)
            {
                Tag = subItem.Tag
            };
        }

        Type clonedType = GetType();

        ListViewItem newItem;
        if (clonedType == typeof(ListViewItem))
        {
            newItem = new ListViewItem(clonedSubItems, ImageIndexer.Index);
        }
        else
        {
            newItem = (ListViewItem)Activator.CreateInstance(clonedType)!;
        }

        foreach (ListViewSubItem subItem in clonedSubItems)
        {
            newItem._subItems.Add(subItem);
        }

        newItem.ImageIndexer.Index = ImageIndexer.Index;
        newItem.SubItemCount = SubItemCount;
        newItem.Checked = Checked;
        newItem.UseItemStyleForSubItems = UseItemStyleForSubItems;
        newItem.Tag = Tag;

        // Only copy over the ImageKey if we're using it.
        if (!string.IsNullOrEmpty(ImageIndexer.Key))
        {
            newItem.ImageIndexer.Key = ImageIndexer.Key;
        }

        newItem._indentCount = _indentCount;
        newItem.StateImageIndex = StateImageIndex;
        newItem._toolTipText = _toolTipText;
        newItem.BackColor = BackColor;
        newItem.ForeColor = ForeColor;
        newItem.Font = Font;
        newItem.Text = Text;
        newItem.Group = Group;

        return newItem;
    }

    /// <summary>
    ///  Ensure that the item is visible, scrolling the view as necessary.
    /// </summary>
    public virtual void EnsureVisible()
    {
        if (_listView is not null && _listView.IsHandleCreated)
        {
            _listView.EnsureVisible(Index);
        }
    }

    public ListViewItem? FindNearestItem(SearchDirectionHint searchDirection)
    {
        Rectangle r = Bounds;
        int xCenter = r.Left + (r.Right - r.Left) / 2;
        int yCenter = r.Top + (r.Bottom - r.Top) / 2;

        return ListView?.FindNearestItem(searchDirection, xCenter, yCenter);
    }

    /// <summary>
    ///  Returns a specific portion of the ListViewItem's bounding rectangle.
    ///  The rectangle returned is empty if the ListViewItem has not been added to a ListView control.
    /// </summary>
    public Rectangle GetBounds(ItemBoundsPortion portion)
    {
        if (_listView is not null && _listView.IsHandleCreated)
        {
            return _listView.GetItemRect(Index, portion);
        }

        return default;
    }

    public ListViewSubItem? GetSubItemAt(int x, int y)
    {
        if (_listView is not null && _listView.IsHandleCreated && _listView.View == View.Details)
        {
            _listView.GetSubItemAt(x, y, out int iItem, out int iSubItem);
            if (iItem == Index && iSubItem != -1 && iSubItem < SubItems.Count)
            {
                return SubItems[iSubItem];
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    internal void Host(ListView parent, int id, int index)
    {
        // Don't let the name "host" fool you -- Handle is not necessarily created
        Debug.Assert(_listView is null || !_listView.VirtualMode, "ListViewItem::Host can't be used w/ a virtual item");
        Debug.Assert(parent is null || !parent.VirtualMode, "ListViewItem::Host can't be used w/ a virtual list");

        _id = id;
        _listView = parent;

        // If the index is valid, then the handle has been created.
        if (index != -1)
        {
            UpdateStateToListView(index);
        }

        KeyboardToolTipStateMachine.Instance.Hook(this, _listView!.KeyboardToolTip);
    }

    internal void ReleaseUiaProvider()
    {
        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        if (OsVersion.IsWindows8OrGreater())
        {
            if (_accessibilityObject is ListViewItemBaseAccessibleObject itemAccessibleObject)
            {
                itemAccessibleObject.ReleaseChildUiaProviders();
            }

            PInvoke.UiaDisconnectProvider(_accessibilityObject, skipOSCheck: true);
        }

        _accessibilityObject = null;
    }

    /// <summary>
    ///  This is used to map list view items w/ their respective groups in localized forms.
    /// </summary>
    internal void UpdateGroupFromName()
    {
        Debug.Assert(_listView is not null, "This method is used only when items are parented in a list view");
        Debug.Assert(!_listView.VirtualMode, "we need to update the group only when the user specifies the list view items in localizable forms");
        if (string.IsNullOrEmpty(_groupName))
        {
            return;
        }

        Group = _listView.Groups[_groupName];

        // Use the group name only once.
        _groupName = null;
    }

    internal void UpdateStateToListView(int index)
    {
        LVITEMW item = default;
        UpdateStateToListView(index, ref item, updateOwner: true);
    }

    /// <summary>
    ///  Called when we have just pushed this item into a list view and we need
    ///  to configure the list view's state for the item. Use a valid index
    ///  if you can, or use -1 if you can't.
    /// </summary>
    internal void UpdateStateToListView(int index, ref LVITEMW lvItem, bool updateOwner)
    {
        Debug.Assert(_listView!.IsHandleCreated, "Should only invoke UpdateStateToListView when handle is created.");

        if (index == -1)
        {
            index = Index;
        }
        else
        {
            _lastIndex = index;
        }

        // Update Item state in one shot
        LIST_VIEW_ITEM_STATE_FLAGS itemState = 0;
        LIST_VIEW_ITEM_STATE_FLAGS stateMask = 0;
        if (StateSelected)
        {
            itemState |= LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED;
            stateMask |= LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED;
        }

        if (SavedStateImageIndex > ImageList.Indexer.DefaultIndex)
        {
            itemState |= (LIST_VIEW_ITEM_STATE_FLAGS)((SavedStateImageIndex + 1) << 12);
            stateMask |= LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK;
        }

        lvItem.mask |= LIST_VIEW_ITEM_FLAGS.LVIF_STATE;
        lvItem.iItem = index;
        lvItem.stateMask |= stateMask;
        lvItem.state |= itemState;

        if (_listView.GroupsEnabled)
        {
            lvItem.mask |= LIST_VIEW_ITEM_FLAGS.LVIF_GROUPID;
            lvItem.iGroupId = _listView.GetNativeGroupId(this);

            nint result = PInvoke.SendMessage(_listView, PInvoke.LVM_ISGROUPVIEWENABLED);
            Debug.Assert(!updateOwner || result != 0, "Groups not enabled");
            result = PInvoke.SendMessage(_listView, PInvoke.LVM_HASGROUP, (WPARAM)lvItem.iGroupId);
            Debug.Assert(!updateOwner || result != 0, $"Doesn't contain group id: {lvItem.iGroupId}");
        }

        if (updateOwner)
        {
            PInvoke.SendMessage(_listView, PInvoke.LVM_SETITEMW, 0, ref lvItem);
        }
    }

    internal void UpdateStateFromListView(int displayIndex, bool checkSelection)
    {
        if (_listView is not null && _listView.IsHandleCreated && displayIndex != -1)
        {
            // Get information from comctl control
            LVITEMW lvItem = new()
            {
                mask = LIST_VIEW_ITEM_FLAGS.LVIF_PARAM | LIST_VIEW_ITEM_FLAGS.LVIF_STATE | LIST_VIEW_ITEM_FLAGS.LVIF_GROUPID
            };

            if (checkSelection)
            {
                lvItem.stateMask = LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED;
            }

            // we want to get all the information, including the state image mask
            lvItem.stateMask |= LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK;

            if (lvItem.stateMask == 0)
            {
                // perf optimization: no work to do.
                return;
            }

            lvItem.iItem = displayIndex;
            PInvoke.SendMessage(_listView, PInvoke.LVM_GETITEMW, 0, ref lvItem);

            // Update this class' information
            if (checkSelection)
            {
                StateSelected = (lvItem.state & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED) != 0;
            }

            SavedStateImageIndex = ((int)(lvItem.state & LIST_VIEW_ITEM_STATE_FLAGS.LVIS_STATEIMAGEMASK) >> 12) - 1;

            _group = null;
            foreach (ListViewGroup lvg in ListView!.Groups)
            {
                if (lvg.ID == lvItem.iGroupId)
                {
                    _group = lvg;
                    break;
                }
            }
        }
    }

    internal void UnHost(bool checkSelection) => UnHost(Index, checkSelection);

    internal void UnHost(int displayIndex, bool checkSelection)
    {
        UpdateStateFromListView(displayIndex, checkSelection);

        if (_listView is not null)
        {
            if ((_listView.Site is null || !_listView.Site.DesignMode) && _group is not null)
            {
                _group.Items.Remove(this);
            }

            KeyboardToolTipStateMachine.Instance.Unhook(this, _listView.KeyboardToolTip);
        }

        ReleaseUiaProvider();

        // Make sure you do these last, as the first several lines depends on this information
        _id = -1;
        _listView = null;
    }

    public virtual void Remove() => _listView?.Items.Remove(this);

    protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
    {
        bool foundSubItems = false;

        string? imageKey = null;
        int imageIndex = ImageList.Indexer.DefaultIndex;

        foreach (SerializationEntry entry in info)
        {
            if (entry.Name == nameof(Text))
            {
                Text = info.GetString(nameof(Text));
            }
            else if (entry.Name == nameof(ImageIndex))
            {
                imageIndex = info.GetInt32(nameof(ImageIndex));
            }
            else if (entry.Name == nameof(ImageKey))
            {
                imageKey = info.GetString(nameof(ImageKey));
            }
            else if (entry.Name == nameof(SubItemCount))
            {
                SubItemCount = info.GetInt32(nameof(SubItemCount));
                if (SubItemCount > 0)
                {
                    foundSubItems = true;
                }
            }
            else if (entry.Name == nameof(BackColor))
            {
                BackColor = (Color)info.GetValue(nameof(BackColor), typeof(Color))!;
            }
            else if (entry.Name == nameof(Checked))
            {
                Checked = info.GetBoolean(nameof(Checked));
            }
            else if (entry.Name == nameof(Font))
            {
                Font = (Font)info.GetValue(nameof(Font), typeof(Font))!;
            }
            else if (entry.Name == nameof(ForeColor))
            {
                ForeColor = (Color)info.GetValue(nameof(ForeColor), typeof(Color))!;
            }
            else if (entry.Name == nameof(UseItemStyleForSubItems))
            {
                UseItemStyleForSubItems = info.GetBoolean(nameof(UseItemStyleForSubItems));
            }
            else if (entry.Name == nameof(Group))
            {
                ListViewGroup group = (ListViewGroup)info.GetValue(nameof(Group), typeof(ListViewGroup))!;
                _groupName = group.Name;
            }
        }

        // let image key take precedence
        if (imageKey is not null)
        {
            ImageKey = imageKey;
        }
        else if (imageIndex != ImageList.Indexer.DefaultIndex)
        {
            ImageIndex = imageIndex;
        }

        if (foundSubItems)
        {
            _subItems.EnsureCapacity(SubItemCount);
            for (int i = 1; i < SubItemCount; i++)
            {
                ListViewSubItem newItem = (ListViewSubItem)info.GetValue($"SubItem{i}", typeof(ListViewSubItem))!;
                newItem._owner = this;
                _subItems.Add(newItem);
            }
        }
    }

    /// <summary>
    ///  Saves this ListViewItem object to the given data stream.
    /// </summary>
    protected virtual void Serialize(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Text), Text);
        info.AddValue(nameof(ImageIndex), ImageIndexer.Index);
        if (!string.IsNullOrEmpty(ImageIndexer.Key))
        {
            info.AddValue(nameof(ImageKey), ImageIndexer.Key);
        }

        if (SubItemCount > 1)
        {
            info.AddValue(nameof(SubItemCount), SubItemCount);
            for (int i = 1; i < SubItemCount; i++)
            {
                info.AddValue($"SubItem{i}", _subItems[i], typeof(ListViewSubItem));
            }
        }

        info.AddValue(nameof(BackColor), BackColor);
        info.AddValue(nameof(Checked), Checked);
        info.AddValue(nameof(Font), Font);
        info.AddValue(nameof(ForeColor), ForeColor);
        info.AddValue(nameof(UseItemStyleForSubItems), UseItemStyleForSubItems);
        if (Group is not null)
        {
            info.AddValue(nameof(Group), Group);
        }
    }

    // we need this function to set the index when the list view is in virtual mode.
    // the index of the list view item is used in ListView::set_TopItem property
    internal void SetItemIndex(ListView listView, int index)
    {
        Debug.Assert(listView is not null && listView.VirtualMode, "ListViewItem::SetItemIndex should be used only when the list is virtual");
        Debug.Assert(index > -1, "can't set the index on a virtual list view item to -1");
        _listView = listView;
        _lastIndex = index;
    }

    internal static bool ShouldSerializeText() => false;

    private bool ShouldSerializePosition() => !_position.Equals(new Point(-1, -1));

    public override string ToString() => $"ListViewItem: {{{Text}}}";

    internal void InvalidateListView()
    {
        // The ListItem's state (or a SubItem's state) has changed, so invalidate the ListView control
        if (_listView is not null && _listView.IsHandleCreated)
        {
            _listView.Invalidate();
        }
    }

    internal void UpdateSubItems(int index) => UpdateSubItems(index, SubItemCount);

    internal void UpdateSubItems(int index, int oldCount)
    {
        if (_listView is null || !_listView.IsHandleCreated)
        {
            return;
        }

        int subItemCount = SubItemCount;
        int itemIndex = Index;
        if (index != -1)
        {
            // Update the specified subitem text.
            _listView.SetItemText(itemIndex, index, _subItems[index].Text);
        }
        else
        {
            // Update all subitems text.
            for (int i = 0; i < subItemCount; i++)
            {
                _listView.SetItemText(itemIndex, i, _subItems[i].Text);
            }
        }

        // Clear subitems beyond the current count.
        for (int i = subItemCount; i < oldCount; i++)
        {
            _listView.SetItemText(itemIndex, i, string.Empty);
        }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => Serialize(info, context);
}
