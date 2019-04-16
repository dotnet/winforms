// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Globalization;

namespace System.Windows.Forms
{

    /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup"]/*' />
    /// <devdoc>
    ///    <para>
    ///         Represents a group within a ListView.
    ///
    ///    </para>
    /// </devdoc>
    [
    TypeConverterAttribute(typeof(ListViewGroupConverter)),
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Header)),
    Serializable
    ]
    public sealed class ListViewGroup : ISerializable {

        private ListView listView;              
        private int id;

        private string header;
        private HorizontalAlignment headerAlignment = HorizontalAlignment.Left;

        private ListView.ListViewItemCollection items;        

        private static int nextID;

        private static int nextHeader = 1;

        private object userData;

        private string name;

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroup"]/*' />
        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
        public ListViewGroup() : this(string.Format(SR.ListViewGroupDefaultHeader, nextHeader++))
        {
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroup1"]/*' />
        /// <devdoc>
        ///     Creates a ListViewItem object from an Stream.
        /// </devdoc>
        private ListViewGroup(SerializationInfo info, StreamingContext context) : this() {
            Deserialize(info, context);
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroup2"]/*' />
        /// <devdoc>
        ///     Creates a ListViewItem object from a Key and a Name
        /// </devdoc>
        public ListViewGroup(string key, string headerText) : this() {
            this.name = key;
            this.header = headerText;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroup2"]/*' />
        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
        public ListViewGroup(string header) 
        {
            this.header = header;
            this.id = nextID++;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroup3"]/*' />
        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
    	public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header) {        
            this.headerAlignment = headerAlignment;
        }    	                

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.Header"]/*' />
        /// <devdoc>
        ///     The text displayed in the group header.
        /// </devdoc>
        [SRCategory(nameof(SR.CatAppearance))]
        public string Header {
            get
            {
                return header == null ? "" : header;
            }
            set
            {
                if (header != value) {
                    header = value;

                    if (listView != null) {
                        listView.RecreateHandleInternal();
                    }
                }
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.HeaderAlignment"]/*' />
        /// <devdoc>
        ///     The alignment of the group header.
        /// </devdoc>
        [
            DefaultValue(HorizontalAlignment.Left),
            SRCategory(nameof(SR.CatAppearance))
        ]
        public HorizontalAlignment HeaderAlignment {
            get
            {
                return headerAlignment;
            }
            set
            {
                // Verify that the value is within the enum's range.
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }
                if (headerAlignment != value) {
                    headerAlignment = value;
                    UpdateListView();
                }
            }
        }

        internal int ID {
            get
            {
                return id;
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.Items"]/*' />
        /// <devdoc>
        ///     The items that belong to this group.
        /// </devdoc>
        [Browsable(false)]
        public ListView.ListViewItemCollection Items {
            get
            {
                if (items == null) {
                    items = new ListView.ListViewItemCollection(new ListViewGroupItemCollection(this));
                }
                return items;
            }
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ListView ListView
        {
            get
            {
                return listView;
            }
        }        

        internal ListView ListViewInternal
        {
            set
            {
                if (listView != value)
                {
                    listView = value;
                }
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.Name"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ListViewGroupNameDescr)),
        Browsable(true),
        DefaultValue("")
        ]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }


        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }


        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.Deserialize"]/*' />
        private void Deserialize(SerializationInfo info, StreamingContext context) {

            int count = 0;
            
            foreach (SerializationEntry entry in info) {
                if (entry.Name == "Header") {
                    Header = (string)entry.Value;
                }
                else if (entry.Name == "HeaderAlignment") {
                    HeaderAlignment = (HorizontalAlignment)entry.Value;
                }
                else if (entry.Name == "Tag") { 
                    Tag = entry.Value;
                }
                else if (entry.Name == "ItemsCount") {
                    count = (int)entry.Value;
                }
                else if (entry.Name == "Name") {
                    Name =  (string) entry.Value;
                }
            }
            if (count > 0) {
                ListViewItem[] items = new ListViewItem[count];

                for (int i = 0; i < count; i++) {
                    items[i] = (ListViewItem)info.GetValue("Item" + i, typeof(ListViewItem));
                }
                Items.AddRange(items);
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ToString"]/*' />
        public override string ToString() {
            return Header;
        }

        private void UpdateListView() {
            if (listView != null && listView.IsHandleCreated) {
                listView.UpdateGroupNative(this);                
            }
        }                

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.GetObjectData"]/*' />
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Header", this.Header);
            info.AddValue("HeaderAlignment", this.HeaderAlignment);
            info.AddValue("Tag", this.Tag);
            if (!string.IsNullOrEmpty(this.Name)) {
                info.AddValue("Name", this.Name);
            }
            if (items != null && items.Count > 0) {
                info.AddValue("ItemsCount", this.Items.Count);
                for (int i = 0; i < Items.Count; i ++) {
                    info.AddValue("Item" + i.ToString(CultureInfo.InvariantCulture), Items[i], typeof(ListViewItem));
                }
            }
        }
    }
}

