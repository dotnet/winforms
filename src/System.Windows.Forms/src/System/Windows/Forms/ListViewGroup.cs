// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
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
        private string _header;
        private HorizontalAlignment _headerAlignment = HorizontalAlignment.Left;

        private ListView.ListViewItemCollection _items;

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
        public ListViewGroup(string key, string headerText) : this()
        {
            Name = key;
            _header = headerText;
        }

        /// <summary>
        ///  Creates a ListViewGroup.
        /// </summary>
        public ListViewGroup(string header)
        {
            _header = header;
            ID = s_nextID++;
        }

        /// <summary>
        ///  Creates a ListViewGroup.
        /// </summary>
    	public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header)
        {
            _headerAlignment = headerAlignment;
        }

        /// <summary>
        ///  The text displayed in the group header.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        public string Header
        {
            get => _header ?? string.Empty;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    ListView?.RecreateHandleInternal();
                }
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

                if (_headerAlignment != value)
                {
                    _headerAlignment = value;
                    UpdateListView();
                }
            }
        }

        internal int ID { get; }

        /// <summary>
        ///  The items that belong to this group.
        /// </summary>
        [Browsable(false)]
        public ListView.ListViewItemCollection Items
        {
            get => _items ?? (_items = new ListView.ListViewItemCollection(new ListViewGroupItemCollection(this)));
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListView ListView { get; internal set; }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewGroupNameDescr))]
        [Browsable(true)]
        [DefaultValue("")]
        public string Name { get; set; }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        private void Deserialize(SerializationInfo info, StreamingContext context)
        {
            int count = 0;

            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "Header")
                {
                    Header = (string)entry.Value;
                }
                else if (entry.Name == "HeaderAlignment")
                {
                    HeaderAlignment = (HorizontalAlignment)entry.Value;
                }
                else if (entry.Name == "Tag")
                {
                    Tag = entry.Value;
                }
                else if (entry.Name == "ItemsCount")
                {
                    count = (int)entry.Value;
                }
                else if (entry.Name == "Name")
                {
                    Name = (string)entry.Value;
                }
            }
            if (count > 0)
            {
                ListViewItem[] items = new ListViewItem[count];
                for (int i = 0; i < count; i++)
                {
                    items[i] = (ListViewItem)info.GetValue("Item" + i, typeof(ListViewItem));
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
            info.AddValue(nameof(Header), Header);
            info.AddValue(nameof(HeaderAlignment), HeaderAlignment);
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
