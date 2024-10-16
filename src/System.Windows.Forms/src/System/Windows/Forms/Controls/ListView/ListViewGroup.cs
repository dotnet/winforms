// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

/// <summary>
///  Represents a group within a ListView.
/// </summary>
[TypeConverter(typeof(ListViewGroupConverter))]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultProperty(nameof(Header))]
[Serializable] // This type is participating in resx serialization scenarios.
public sealed partial class ListViewGroup : ISerializable
{
    private string? _header;
    private HorizontalAlignment _headerAlignment = HorizontalAlignment.Left;
    private string? _footer;
    private HorizontalAlignment _footerAlignment = HorizontalAlignment.Left;
    private ListViewGroupCollapsedState _collapsedState = ListViewGroupCollapsedState.Default;
    private string? _subtitle;
    private string? _taskLink;

    private ListView.ListViewItemCollection? _items;

    private static int s_nextID;

    private static int s_nextHeader = 1;

    private ListViewGroupImageIndexer? _imageIndexer;
    private AccessibleObject? _accessibilityObject;

    /// <summary>
    ///  Creates a ListViewGroup.
    /// </summary>
    public ListViewGroup() : this(string.Format(SR.ListViewGroupDefaultHeader, s_nextHeader++))
    {
    }

    /// <summary>
    ///  Creates a ListViewItem object from an Stream.
    /// </summary>
    private ListViewGroup(SerializationInfo info, StreamingContext context) : this()
    {
        Deserialize(info);
    }

    /// <summary>
    ///  Creates a ListViewItem object from a Key and a Name
    /// </summary>
    public ListViewGroup(string? key, string? headerText) : this()
    {
        Name = key;
        _header = headerText;
    }

    /// <summary>
    ///  Creates a ListViewGroup.
    /// </summary>
    public ListViewGroup(string? header)
    {
        _header = header;
        ID = s_nextID++;
    }

    /// <summary>
    ///  Creates a ListViewGroup.
    /// </summary>
    public ListViewGroup(string? header, HorizontalAlignment headerAlignment) : this(header)
    {
        _headerAlignment = headerAlignment;
    }

    internal AccessibleObject AccessibilityObject
        => _accessibilityObject ??= new ListViewGroupAccessibleObject(this, ListView?.Groups.Contains(this) == false);

    /// <summary>
    ///  The text displayed in the group header.
    /// </summary>
    [AllowNull]
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    public string Header
    {
        get => _header ?? string.Empty;
        set
        {
            if (_header == value)
            {
                return;
            }

            _header = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  The alignment of the group header.
    /// </summary>
    [DefaultValue(HorizontalAlignment.Left)]
    [SRCategory(nameof(SR.CatAppearance))]
    public HorizontalAlignment HeaderAlignment
    {
        get => _headerAlignment;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_headerAlignment == value)
            {
                return;
            }

            _headerAlignment = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  The text displayed in the group footer.
    /// </summary>
    [AllowNull]
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    public string Footer
    {
        get => _footer ?? string.Empty;
        set
        {
            if (_footer == value)
            {
                return;
            }

            _footer = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  The alignment of the group footer.
    /// </summary>
    /// <value>
    ///  One of the <see cref="HorizontalAlignment"/> values that specifies the alignment of the footer text.
    ///  The default is <see cref="HorizontalAlignment.Left"/>.
    /// </value>
    /// <exception cref="InvalidEnumArgumentException">
    ///  The specified value when setting this property is not a valid <see cref="HorizontalAlignment"/> value.
    /// </exception>
    [DefaultValue(HorizontalAlignment.Left)]
    [SRCategory(nameof(SR.CatAppearance))]
    public HorizontalAlignment FooterAlignment
    {
        get => _footerAlignment;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_footerAlignment == value)
            {
                return;
            }

            _footerAlignment = value;
            UpdateListView();
        }
    }

    internal bool Focused { get; set; }

    /// <summary>
    ///  Controls which <see cref="ListViewGroupCollapsedState"/> the group will appear as.
    /// </summary>
    /// <value>
    ///  One of the <see cref="ListViewGroupCollapsedState"/> values that specifies how the group is displayed.
    ///  The default is <see cref="ListViewGroupCollapsedState.Default"/>.
    /// </value>
    /// <exception cref="InvalidEnumArgumentException">
    ///  The specified value when setting this property is not a valid <see cref="ListViewGroupCollapsedState"/> value.
    /// </exception>
    [DefaultValue(ListViewGroupCollapsedState.Default)]
    [SRCategory(nameof(SR.CatAppearance))]
    public ListViewGroupCollapsedState CollapsedState
    {
        get => _collapsedState;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_collapsedState == value)
            {
                return;
            }

            _collapsedState = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  The text displayed in the group subtitle.
    /// </summary>
    [AllowNull]
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    public string Subtitle
    {
        get => _subtitle ?? string.Empty;
        set
        {
            if (_subtitle == value)
            {
                return;
            }

            _subtitle = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  The name of the task link displayed in the group header.
    /// </summary>
    [AllowNull]
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    public string TaskLink
    {
        get => _taskLink ?? string.Empty;
        set
        {
            if (value == _taskLink)
            {
                return;
            }

            _taskLink = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  Gets or sets the index of the image that is displayed for the group.
    /// </summary>
    /// <value>
    ///  The zero-based index of the image in the ImageList that is displayed for the group. The default is -1.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  The value specified is less than -1.
    /// </exception>
    [DefaultValue(-1)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [RelatedImageList("ListView.GroupImageList")]
    [SRCategory(nameof(SR.CatBehavior))]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    public int TitleImageIndex
    {
        get
        {
            ImageList? imageList = ImageIndexer.ImageList;
            return imageList is null || ImageIndexer.Index < imageList.Images.Count
                ? ImageIndexer.Index
                : imageList.Images.Count - 1;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            if (ImageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            ImageIndexer.Index = value;
            UpdateListView();
        }
    }

    /// <summary>
    ///  Gets or sets the key of the image that is displayed for the group.
    /// </summary>
    /// <value>
    ///  The key for the image that is displayed for the group.
    /// </value>
    [DefaultValue("")]
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [RelatedImageList("ListView.GroupImageList")]
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    public string TitleImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            if (ImageIndexer.Key == value && value != ImageList.Indexer.DefaultKey)
            {
                return;
            }

            ImageIndexer.Key = value;
            UpdateListView();
        }
    }

    internal ListViewGroupImageIndexer ImageIndexer => _imageIndexer ??= new ListViewGroupImageIndexer(this);

    internal int ID { get; }

    /// <summary>
    ///  The items that belong to this group.
    /// </summary>
    [Browsable(false)]
    public ListView.ListViewItemCollection Items => _items ??= new ListView.ListViewItemCollection(new ListViewGroupItemCollection(this));

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ListView? ListView { get; internal set; }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ListViewGroupNameDescr))]
    [Browsable(true)]
    [DefaultValue("")]
    public string? Name { get; set; }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    private void Deserialize(SerializationInfo info)
    {
        int count = 0;

        foreach (SerializationEntry entry in info)
        {
            if (entry.Name == "Header")
            {
                Header = (string)entry.Value!;
            }
            else if (entry.Name == "HeaderAlignment")
            {
                HeaderAlignment = (HorizontalAlignment)entry.Value!;
            }
            else if (entry.Name == "Footer")
            {
                Footer = (string)entry.Value!;
            }
            else if (entry.Name == "FooterAlignment")
            {
                FooterAlignment = (HorizontalAlignment)entry.Value!;
            }
            else if (entry.Name == "Tag")
            {
                Tag = entry.Value;
            }
            else if (entry.Name == "ItemsCount")
            {
                count = (int)entry.Value!;
            }
            else if (entry.Name == "Name")
            {
                Name = (string)entry.Value!;
            }
        }

        if (count > 0)
        {
            ListViewItem[] items = new ListViewItem[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = (ListViewItem)info.GetValue($"Item{i}", typeof(ListViewItem))!;
            }

            Items.AddRange(items);
        }
    }

    internal ListViewGroupCollapsedState GetNativeCollapsedState()
    {
        if (ListView is null)
        {
            throw new InvalidOperationException(nameof(ListView));
        }

        if (!ListView.GroupsEnabled)
        {
            return ListViewGroupCollapsedState.Default;
        }

        LIST_VIEW_GROUP_STATE_FLAGS state = (LIST_VIEW_GROUP_STATE_FLAGS)(uint)PInvokeCore.SendMessage(
            ListView,
            PInvoke.LVM_GETGROUPSTATE,
            (WPARAM)ID,
            (LPARAM)(uint)(LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSIBLE | LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSED));

        if (!state.HasFlag(LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSIBLE))
        {
            return ListViewGroupCollapsedState.Default;
        }

        return state.HasFlag(LIST_VIEW_GROUP_STATE_FLAGS.LVGS_COLLAPSED) ? ListViewGroupCollapsedState.Collapsed : ListViewGroupCollapsedState.Expanded;
    }

    internal void ReleaseUiaProvider()
    {
        if (OsVersion.IsWindows8OrGreater() && _accessibilityObject is ListViewGroupAccessibleObject accessibleObject)
        {
            PInvoke.UiaDisconnectProvider(accessibleObject, skipOSCheck: true);
        }

        _accessibilityObject = null;
    }

    // Should be used for the cases when sending the message `PInvoke.LVM_SETGROUPINFO` isn't required
    // (for example, collapsing/expanding groups with keyboard is performed inside the native control already,
    // so this message isn't needed)
    internal void SetCollapsedStateInternal(ListViewGroupCollapsedState state)
    {
        SourceGenerated.EnumValidator.Validate(state);

        if (_collapsedState == state)
        {
            return;
        }

        _collapsedState = state;
    }

    public override string ToString() => Header;

    private void UpdateListView()
    {
        if (ListView is not null && ListView.IsHandleCreated)
        {
            ListView.UpdateGroupNative(this);
        }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue(nameof(Header), Header);
        info.AddValue(nameof(HeaderAlignment), HeaderAlignment);
        info.AddValue(nameof(Footer), Footer);
        info.AddValue(nameof(FooterAlignment), FooterAlignment);
        info.AddValue(nameof(Tag), Tag);
        if (!string.IsNullOrEmpty(Name))
        {
            info.AddValue(nameof(Name), Name);
        }

        if (_items is not null && _items.Count > 0)
        {
            info.AddValue("ItemsCount", Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                info.AddValue($"Item{i}", Items[i], typeof(ListViewItem));
            }
        }
    }
}
