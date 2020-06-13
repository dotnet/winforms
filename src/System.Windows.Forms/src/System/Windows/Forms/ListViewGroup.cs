// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a group within a ListView.
    /// </summary>
    [TypeConverter(typeof(ListViewGroupConverter))]
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultProperty(nameof(Header))]
    [Serializable] // This type is participating in resx serialization scenarios.
    public sealed class ListViewGroup : ISerializable
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
            Deserialize(info, context);
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

        /// <summary>
        ///  The text displayed in the group header.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [AllowNull]
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }

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
        [SRCategory(nameof(SR.CatAppearance))]
        [AllowNull]
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
        ///  One of the <see cref="HorizontalAlignment"/> values that specifies the alignment of the footer text. The default is <see cref="HorizontalAlignment.Left"/>.
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }

                if (_footerAlignment == value)
                {
                    return;
                }

                _footerAlignment = value;
                UpdateListView();
            }
        }

        /// <summary>
        ///  Controls which <see cref="ListViewGroupCollapsedState"/> the group will appear as.
        /// </summary>
        /// <value>
        ///  One of the <see cref="ListViewGroupCollapsedState"/> values that specifies how the group is displayed. The default is <see cref="ListViewGroupCollapsedState.Default"/>.
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ListViewGroupCollapsedState.Default, (int)ListViewGroupCollapsedState.Collapsed))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ListViewGroupCollapsedState));
                }

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
        [SRCategory(nameof(SR.CatAppearance))]
        [AllowNull]
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
        [SRCategory(nameof(SR.CatAppearance))]
        [AllowNull]
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

        private void Deserialize(SerializationInfo info, StreamingContext context)
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
                    items[i] = (ListViewItem)info.GetValue("Item" + i, typeof(ListViewItem))!;
                }
                Items.AddRange(items);
            }
        }

        public override string ToString() => Header;

        private void UpdateListView()
        {
            if (ListView != null && ListView.IsHandleCreated)
            {
                ListView.UpdateGroupNative(this);
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Header), Header);
            info.AddValue(nameof(HeaderAlignment), HeaderAlignment);
            info.AddValue(nameof(Footer), Footer);
            info.AddValue(nameof(FooterAlignment), FooterAlignment);
            info.AddValue(nameof(Tag), Tag);
            if (!string.IsNullOrEmpty(Name))
            {
                info.AddValue(nameof(Name), Name);
            }
            if (_items != null && _items.Count > 0)
            {
                info.AddValue("ItemsCount", Items.Count);
                for (int i = 0; i < Items.Count; i++)
                {
                    info.AddValue("Item" + i.ToString(CultureInfo.InvariantCulture), Items[i], typeof(ListViewItem));
                }
            }
        }
    }
}
