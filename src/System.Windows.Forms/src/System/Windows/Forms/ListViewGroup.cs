// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;
using System.Globalization;

namespace System.Windows.Forms {
                               

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


        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
        public ListViewGroup() : this(string.Format(SR.ListViewGroupDefaultHeader, nextHeader++))
        {
        }


        /// <devdoc>
        ///     Creates a ListViewItem object from an Stream.
        /// </devdoc>
        private ListViewGroup(SerializationInfo info, StreamingContext context) : this() {
            Deserialize(info, context);
        }


        /// <devdoc>
        ///     Creates a ListViewItem object from a Key and a Name
        /// </devdoc>
        public ListViewGroup(string key, string headerText) : this() {
            this.name = key;
            this.header = headerText;
        }


        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
        public ListViewGroup(string header) 
        {
            this.header = header;
            this.id = nextID++;
        }


        /// <devdoc>
        ///     Creates a ListViewGroup.
        /// </devdoc>
    	public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header) {        
            this.headerAlignment = headerAlignment;
        }    	                


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
            get
            {
                return listView;
            }
            set
            {
                if (listView != value)
                {
                    listView = value;
                }
            }
        }


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


        public override string ToString() {
            return Header;
        }

        private void UpdateListView() {
            if (listView != null && listView.IsHandleCreated) {
                listView.UpdateGroupNative(this);                
            }
        }                


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
        

    /// <devdoc>
    ///    <para>
    ///         A collection of listview groups.
    ///    </para>
    /// </devdoc>
    [ListBindable(false)]
    public class ListViewGroupCollection : IList {

        private ListView listView;

        private ArrayList list;

        internal ListViewGroupCollection(ListView listView) {
            this.listView = listView;
        }


        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }


        /// <internalonly/>
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }


        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {
                return true;
            }
        }
        

        /// <internalonly/>
        bool IList.IsFixedSize {
            get {
                return false;
            }
        }
        

        /// <internalonly/>
        bool IList.IsReadOnly {
            get {
                return false;
            }
        }

        private ArrayList List
        {
            get
            {
                if (list == null) {
                    list = new ArrayList();
                }
                return list;
            }
        }
                

        public ListViewGroup this[int index] {
            get
            {
                return (ListViewGroup)this.List[index];
            }
            set
            {
                if (this.List.Contains(value)) {
                    return;
                }
                this.List[index] = value;
            }
        }
                

        public ListViewGroup this[string key] {
            get {

                if (list == null) {
                    return null;
                }

                for (int i = 0; i < list.Count; i ++) {
                    if (string.Compare(key, this[i].Name, false /*case insensitive*/, System.Globalization.CultureInfo.CurrentCulture) == 0) {
                        return this[i];
                    }
                }

                return null;
            }
            set {
                int index = -1;

                if (this.list == null) {
                    return; // nothing to do
                }

                for (int i = 0; i < this.list.Count; i ++) {
                    if (string.Compare(key, this[i].Name, false /*case insensitive*/, System.Globalization.CultureInfo.CurrentCulture) ==0) {
                        index = i;
                        break;
                    }
                }

                if (index != -1) {
                    this.list[index] = value;
                }
            }
        }


        /// <internalonly/>
        object IList.this[int index] {
            get
            {
                return this[index];
            }
            set
            {
                if (value is ListViewGroup) {
                    this[index] = (ListViewGroup)value;
                }
            }
        }
        

        public int Add(ListViewGroup group)
        {
            if (this.Contains(group)) {
                return -1;
            }

            // Will throw InvalidOperationException if group contains items which are parented by another listView.
            CheckListViewItems(group);
            group.ListViewInternal = this.listView;
            int index = this.List.Add(group);
            if (listView.IsHandleCreated) {
                listView.InsertGroupInListView(this.List.Count, group);
                MoveGroupItems(group);
            }            
            return index;
        }        
        

        public ListViewGroup Add(string key, string headerText)
        {
            ListViewGroup group = new ListViewGroup(key, headerText);
            this.Add(group);
            return group;
        }


        /// <internalonly/>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "value" is the name of the param passed in.
                                                                                                        // So we don't have to localize it.
        ]
        int IList.Add(object value) {
            if (value is ListViewGroup) {
                return Add((ListViewGroup)value);
            }
            throw new ArgumentException(nameof(value));
        }
                

        public void AddRange(ListViewGroup[] groups)
        {
            for(int i=0; i < groups.Length; i++) {
                Add(groups[i]);
            }
        }


        public void AddRange(ListViewGroupCollection groups)
        {
            for(int i=0; i < groups.Count; i++) {
                Add(groups[i]);
            }
        }

        private void CheckListViewItems(ListViewGroup group) {
            for (int i = 0; i < group.Items.Count; i ++) {
                ListViewItem item = group.Items[i];
                if (item.ListView != null && item.ListView != this.listView) {
                    throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text));
                }
            }
        }


        public void Clear() {
            if (listView.IsHandleCreated) {
                for(int i=0; i < Count; i++) {
                    listView.RemoveGroupFromListView(this[i]);
                }
            }
            // Dissociate groups from the ListView
            //
            for(int i=0; i < Count; i++) {
                this[i].ListViewInternal = null;
            }
            this.List.Clear();

            // we have to tell the listView that there are no more groups
            // so the list view knows to remove items from the default group
            this.listView.UpdateGroupView();
        }
        

        public bool Contains(ListViewGroup value) {
            return this.List.Contains(value);
        }


        /// <internalonly/>
        bool IList.Contains(object value)
        {
            if (value is ListViewGroup) {
                return Contains((ListViewGroup)value);
            }
            return false;
        }


        public void CopyTo(Array array, int index) {
            this.List.CopyTo(array, index);
        }


        public IEnumerator GetEnumerator()
        {
            return this.List.GetEnumerator();
        }


        public int IndexOf(ListViewGroup value) {
            return this.List.IndexOf(value);
        }


        /// <internalonly/>
        int IList.IndexOf(object value) {
            if (value is ListViewGroup) {
                return IndexOf((ListViewGroup)value);
            }
            return -1;
        }


        public void Insert(int index, ListViewGroup group) {
            if (Contains(group)) {
                return;
            }
            group.ListViewInternal = this.listView;
            this.List.Insert(index, group);
            if (listView.IsHandleCreated) {
                listView.InsertGroupInListView(index, group);
                MoveGroupItems(group);
            }            
        }       


        /// <internalonly/>
        void IList.Insert(int index, object value) {
            if (value is ListViewGroup) {
                Insert(index, (ListViewGroup)value);
            }            
        }

        private void MoveGroupItems(ListViewGroup group) {
            Debug.Assert(listView.IsHandleCreated, "MoveGroupItems pre-condition: listView handle must be created");

            foreach(ListViewItem item in group.Items) {
                if (item.ListView == this.listView) {
                    item.UpdateStateToListView(item.Index);                    
                }
            }
        }
       

        public void Remove(ListViewGroup group) {
            group.ListViewInternal = null;            
            this.List.Remove(group);

            if (listView.IsHandleCreated) {
                listView.RemoveGroupFromListView(group);
            }
        }


        /// <internalonly/>
        void IList.Remove(object value)
        {
            if (value is ListViewGroup) {
                Remove((ListViewGroup)value);
            }
        }


        public void RemoveAt(int index) {
            Remove(this[index]);
        }        
    }        
}

