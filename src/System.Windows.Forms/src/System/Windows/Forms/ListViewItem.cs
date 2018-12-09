// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Security; 
    using System.Security.Permissions;
    using System;
    using System.Drawing;
    using System.Drawing.Design;
    using System.ComponentModel;
    using System.IO;
    using Microsoft.Win32;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel.Design.Serialization;
    using System.Reflection;
    using System.Globalization;
    
    /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Implements an item of a <see cref='System.Windows.Forms.ListView'/>.
    ///    </para>
    /// </devdoc>
    [
    TypeConverterAttribute(typeof(ListViewItemConverter)),
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Text)),
    Serializable,    
    SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")
    ]
    public class ListViewItem : ICloneable, ISerializable {

        private const int MAX_SUBITEMS = 4096;
        
        private static readonly BitVector32.Section StateSelectedSection         = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section StateImageMaskSet            = BitVector32.CreateSection(1, StateSelectedSection);
        private static readonly BitVector32.Section StateWholeRowOneStyleSection = BitVector32.CreateSection(1, StateImageMaskSet);
        private static readonly BitVector32.Section SavedStateImageIndexSection  = BitVector32.CreateSection(15, StateWholeRowOneStyleSection);
        private static readonly BitVector32.Section SubItemCountSection          = BitVector32.CreateSection(MAX_SUBITEMS, SavedStateImageIndexSection);

        private int indentCount = 0;
        private Point position = new Point(-1,-1);
        
        internal ListView listView;

        internal ListViewGroup group;
        private string groupName;
        
        private ListViewSubItemCollection listViewSubItemCollection = null;
        private ListViewSubItem[] subItems;
        
        // we stash the last index we got as a seed to GetDisplayIndex.
        private int lastIndex = -1;

        // An ID unique relative to a given list view that comctl uses to identify items.
        internal int ID = -1;

        private BitVector32 state = new BitVector32();
        private ListViewItemImageIndexer imageIndexer;
        private String toolTipText = String.Empty;
        object userData;

        // We need a special way to defer to the ListView's image
        // list for indexing purposes.
        internal class ListViewItemImageIndexer : ImageList.Indexer {
           private ListViewItem owner;

           
           /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ImageListType.ListViewItemImageIndexer"]/*' />
            public ListViewItemImageIndexer(ListViewItem item) {
              owner = item;
            }

           
           public override ImageList ImageList {
                get { 
                        if (owner != null) {
                            return owner.ImageList;
                        }
                        return null;
                    }
                set { Debug.Assert(false, "We should never set the image list"); }
            }
        }

        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem"]/*' />
        public ListViewItem() {
            StateSelected = false;
            UseItemStyleForSubItems = true;
            SavedStateImageIndex = -1;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem1"]/*' />
        /// <devdoc>
        ///     Creates a ListViewItem object from an Stream.
        ///     The Serialization constructor is protected, as per FxCop Microsoft.Usage, CA2229 Rule.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // Changing Deserialize to be non-virtual
                                                                                                    // would be a breaking change.
        ]
        protected ListViewItem(SerializationInfo info, StreamingContext context) : this() {
            Deserialize(info, context);
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem2"]/*' />
        public ListViewItem(string text) : this(text, -1) {            
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem3"]/*' />
        public ListViewItem(string text, int imageIndex) : this() {
            this.ImageIndexer.Index = imageIndex;
            Text = text;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem4"]/*' />
        public ListViewItem(string[] items) : this(items, -1) {
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem5"]/*' />
        public ListViewItem(string[] items, int imageIndex) : this() {

            this.ImageIndexer.Index = imageIndex;
            if (items != null && items.Length > 0) {
                this.subItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++) {
                    subItems[i] = new ListViewSubItem(this, items[i]);
                }
                this.SubItemCount = items.Length;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem6"]/*' />
        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font) : this(items, imageIndex) {
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem7"]/*' />
        public ListViewItem(ListViewSubItem[] subItems, int imageIndex) : this() {

            this.ImageIndexer.Index = imageIndex;
            this.subItems = subItems;
            this.SubItemCount = this.subItems.Length;
            
            // Update the owner of these subitems
            //
            for(int i=0; i < subItems.Length; ++i) {
                subItems[i].owner = this;
            }
        }
          
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem8"]/*' />
        public ListViewItem(ListViewGroup group) : this() {
            this.Group = group;
        }        

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem9"]/*' />
        public ListViewItem(string text, ListViewGroup group) : this(text) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem10"]/*' />
        public ListViewItem(string text, int imageIndex, ListViewGroup group) : this(text, imageIndex) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem11"]/*' />
        public ListViewItem(string[] items, ListViewGroup group) : this(items) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem12"]/*' />
        public ListViewItem(string[] items, int imageIndex, ListViewGroup group) : this(items, imageIndex) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem13"]/*' />
        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group) :
            this(items, imageIndex, foreColor, backColor, font) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem14"]/*' />
        public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group) : this(subItems, imageIndex) {
            this.Group = group;
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem15"]/*' />
        public ListViewItem(string text, string imageKey) : this() {
            this.ImageIndexer.Key = imageKey;
            Text = text;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem16"]/*' />
        public ListViewItem(string[] items, string imageKey) : this() {

            this.ImageIndexer.Key = imageKey;
            if (items != null && items.Length > 0) {
                this.subItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++) {
                    subItems[i] = new ListViewSubItem(this, items[i]);
                }
                this.SubItemCount = items.Length;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem17"]/*' />
        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font) : this(items, imageKey) {
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem18"]/*' />
        public ListViewItem(ListViewSubItem[] subItems, string imageKey) : this() {

            this.ImageIndexer.Key = imageKey;
            this.subItems = subItems;
            this.SubItemCount = this.subItems.Length;
            
            // Update the owner of these subitems
            //
            for(int i=0; i < subItems.Length; ++i) {
                subItems[i].owner = this;
            }
        }
          
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem19"]/*' />
        public ListViewItem(string text, string imageKey, ListViewGroup group) : this(text, imageKey) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem20"]/*' />
        public ListViewItem(string[] items, string imageKey, ListViewGroup group) : this(items, imageKey) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem21"]/*' />
        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group) :
            this(items, imageKey, foreColor, backColor, font) {
            this.Group = group;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewItem22"]/*' />
        public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group) : this(subItems, imageKey) {
            this.Group = group;
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.BackColor"]/*' />
        /// <devdoc>
        ///     The font that this item will be displayed in. If its value is null, it will be displayed
        ///     using the global font for the ListView control that hosts it.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public Color BackColor {
            get {
                if (SubItemCount == 0) {
                    if (listView != null) {
                        return listView.BackColor;
                    }
                    return SystemColors.Window;
                }
                else {
                    return subItems[0].BackColor;
                }
            }
            set {
                SubItems[0].BackColor = value;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Bounds"]/*' />
        /// <devdoc>
        ///     Returns the ListViewItem's bounding rectangle, including subitems. The bounding rectangle is empty if
        ///     the ListViewItem has not been added to a ListView control.
        /// </devdoc>
        [Browsable(false)]
        public Rectangle Bounds {
            get {
                if (listView != null) {
                    return listView.GetItemRect(Index);
                }
                else
                    return new Rectangle();
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Checked"]/*' />
        [
        DefaultValue(false),
        RefreshPropertiesAttribute(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public bool Checked {
            get {
                return StateImageIndex > 0;
            }

            set {
                if (Checked != value) {
                    if (listView != null && listView.IsHandleCreated) {
                        StateImageIndex = value ? 1 : 0;
             
                        // the setter for StateImageIndex calls ItemChecked handler
                        // thus need to verify validity of the listView again
                        if ((this.listView != null) && !this.listView.UseCompatibleStateImageBehavior) {
                            if (!listView.CheckBoxes) {
                                listView.UpdateSavedCheckedItems(this, value);
                            }
                        }
                        
                    }
                    else {
                        SavedStateImageIndex = value ? 1 : 0;
                    }
                }
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Focused"]/*' />
        /// <devdoc>
        ///     Returns the focus state of the ListViewItem.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public bool Focused {
            get {
                if (listView != null && listView.IsHandleCreated) {
                    return(listView.GetItemState(Index, NativeMethods.LVIS_FOCUSED) != 0);
                }
                else return false;
            }

            set {
                if (listView != null && listView.IsHandleCreated) {
                    listView.SetItemState(Index, value ? NativeMethods.LVIS_FOCUSED : 0, NativeMethods.LVIS_FOCUSED);
                }
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Font"]/*' />
        [
        Localizable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public Font Font {
            get {
                if (SubItemCount == 0) {
                    if (listView != null) {
                        return listView.Font;
                    }
                    return Control.DefaultFont;
                }
                else {
                    return subItems[0].Font;
                }
            }
            set {
                SubItems[0].Font = value;
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ForeColor"]/*' />
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public Color ForeColor {
            get {
                if (SubItemCount == 0) {
                    if (listView != null) {
                        return listView.ForeColor;
                    }
                    return SystemColors.WindowText;
                }
                else {
                    return subItems[0].ForeColor;
                }
            }
            set {
                SubItems[0].ForeColor = value;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Group"]/*' />
        /// <devdoc>
        ///    <para>The group to which this item belongs</para>
        /// </devdoc>
        [
            DefaultValue(null),
            Localizable(true),
            SRCategory(nameof(SR.CatBehavior))
        ]
        public ListViewGroup Group {
            get
            {                
                return group;
            }
            set
            {                
                if (group != value) {
                    if (value != null) {
                        value.Items.Add(this);
                    }
                    else {
                        group.Items.Remove(this);
                    }
                }
                Debug.Assert(this.group == value, "BUG: group member variable wasn't updated!");                

                // If the user specifically sets the group then don't use the groupName again.
                this.groupName = null;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ImageIndex"]/*' />
        /// <devdoc>
        ///     Returns the ListViewItem's currently set image index        
        /// </devdoc>
        [
        DefaultValue(-1), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ListViewItemImageIndexDescr)),
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter))
        ]        
        public int ImageIndex {
            get {
                if (ImageIndexer.Index != -1 && ImageList != null && ImageIndexer.Index >= ImageList.Images.Count) {
                    return ImageList.Images.Count - 1;
                } 
                return this.ImageIndexer.Index;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException(nameof(ImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "ImageIndex", value.ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
                }
            
                ImageIndexer.Index = value;

                if (listView != null && listView.IsHandleCreated) {
                    listView.SetItemImage(Index, ImageIndexer.ActualIndex);
               }
            }
        }

        internal ListViewItemImageIndexer ImageIndexer {
            get { 
                if (imageIndexer == null)  {
                    imageIndexer = new ListViewItemImageIndexer(this);
                }
                return imageIndexer;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ImageIndex"]/*' />
        /// <devdoc>
        ///     Returns the ListViewItem's currently set image index        
        /// </devdoc>
        [
        DefaultValue(""), 
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true)
        ]        
        public string ImageKey {
            get { 
                return this.ImageIndexer.Key;
            }
            set {
                   ImageIndexer.Key = value;

                   if (listView != null && listView.IsHandleCreated) {
                    listView.SetItemImage(Index, ImageIndexer.ActualIndex);
                   }
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ImageList"]/*' />
        [Browsable(false)]
        public ImageList ImageList {
            get {
                if (listView != null) {
                    switch(listView.View) {
                        case View.LargeIcon:
                        case View.Tile:
                            return listView.LargeImageList;
                        case View.SmallIcon:
                        case View.Details:
                        case View.List:
                            return listView.SmallImageList;
                    }
                }
                return null;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.IndentCount"]/*' />
        [
        DefaultValue(0),
        SRDescription(nameof(SR.ListViewItemIndentCountDescr)),
        SRCategory(nameof(SR.CatDisplay))
        ]
        public int IndentCount {
            get {
                return indentCount;
            }
            set {
                if (value == indentCount)
                    return;
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(IndentCount), SR.ListViewIndentCountCantBeNegative);
                }
                indentCount = value;
                if (listView != null && listView.IsHandleCreated) {
                    listView.SetItemIndentCount(Index, indentCount);
                }
            }
        }


        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Index"]/*' />
        /// <devdoc>
        ///     Returns ListViewItem's current index in the listview, or -1 if it has not been added to a ListView control.
        /// </devdoc>
        [Browsable(false)]
        public int Index {
            get {
                if (listView != null) {
                    // if the list is virtual, the ComCtrl control does not keep any information
                    // about any list view items, so we use our cache instead.
                    if (!listView.VirtualMode) {
                        lastIndex = listView.GetDisplayIndex(this, lastIndex);
                    }
                    return lastIndex;
                }   
                else {
                    return -1;
                }
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListView"]/*' />
        /// <devdoc>
        /// Returns the ListView control that holds this ListViewItem. May be null if no
        /// control has been assigned yet.
        /// </devdoc>
        [Browsable(false)]
        public ListView ListView {
            get {
                return listView;
            }
        }


        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Name"]/*' />
        /// <devdoc>
        ///     Name associated with this ListViewItem
        /// </devdoc>
        [
        Localizable(true),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string Name {
            get {
                if (SubItemCount == 0) {
                    return string.Empty;
                }
                else {
                    return subItems[0].Name;
                }
            }
            set {
                SubItems[0].Name = value;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Position"]/*' />
        [
        SRCategory(nameof(SR.CatDisplay)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public Point Position {
            get {
                if (listView != null && listView.IsHandleCreated) {
                    position = listView.GetItemPosition(Index);
                }
                return position;
            }
            set {
                if (value.Equals(position))
                    return;
                position = value;
                if (listView != null && listView.IsHandleCreated) {
                    if (!listView.VirtualMode) {
                        listView.SetItemPosition(Index, position.X, position.Y);
                    }
                }
            }
        }

        // the listView needs the raw encoding for the state image index when in VirtualMode
        internal int RawStateImageIndex {
            get {
                return (this.SavedStateImageIndex + 1) << 12;
            }
        }

        /// <devdoc>
        ///     Accessor for our state bit vector.
        /// </devdoc>
        private int SavedStateImageIndex {
            get {
                // State goes from zero to 15, but we need a negative
                // number, so we store + 1.
                return state[SavedStateImageIndexSection] - 1;
            }
            set {
                // flag whether we've set a value.
                //
                state[StateImageMaskSet] = (value == -1 ? 0 : 1);

                // push in the actual value
                //
                state[SavedStateImageIndexSection] = value + 1;
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Selected"]/*' />
        /// <devdoc>
        ///     Treats the ListViewItem as a row of strings, and returns an array of those strings
        /// </devdoc>
        [
        Browsable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Selected {
            get {
                if (listView != null && listView.IsHandleCreated) {
                    return(listView.GetItemState(Index, NativeMethods.LVIS_SELECTED) != 0);
                }
                else
                    return StateSelected;
            }
            set {
                if (listView != null && listView.IsHandleCreated) {
                    listView.SetItemState(Index, value ? NativeMethods.LVIS_SELECTED: 0, NativeMethods.LVIS_SELECTED);

                    // update comctl32's selection information.
                    listView.SetSelectionMark(Index);
                }
                else {
                    StateSelected = value;
                    if (this.listView != null && this.listView.IsHandleCreated) {
                        // APPCOMPAT: set the selected state on the list view item only if the list view's Handle is already created.
                        listView.CacheSelectedStateForItem(this, value);
                    }
                }
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.StateImageIndex"]/*' />
        [
        Localizable(true),
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter)),
        DefaultValue(-1),
        SRDescription(nameof(SR.ListViewItemStateImageIndexDescr)),
        SRCategory(nameof(SR.CatBehavior)),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RelatedImageList("ListView.StateImageList")
        ]
        public int StateImageIndex {
            get {
                if (listView != null && listView.IsHandleCreated) {
                    int state = listView.GetItemState(Index, NativeMethods.LVIS_STATEIMAGEMASK);
                    return ((state >> 12) - 1);   // index is 1-based
                }
                else return SavedStateImageIndex;
            }
            set {
                if (value < -1 || value > 14)
                    throw new ArgumentOutOfRangeException(nameof(StateImageIndex), string.Format(SR.InvalidArgument, "StateImageIndex", (value).ToString(CultureInfo.CurrentCulture)));

                if (listView != null && listView.IsHandleCreated) {
                    this.state[StateImageMaskSet] = (value == -1 ? 0 : 1);
                    int state = ((value + 1) << 12);  // index is 1-based
                    listView.SetItemState(Index, state, NativeMethods.LVIS_STATEIMAGEMASK);
                }
                SavedStateImageIndex = value;
            }
        }


        internal bool StateImageSet {
            get {
                return (this.state[StateImageMaskSet] != 0);
            }
        }


        /// <devdoc>
        ///     Accessor for our state bit vector.
        /// </devdoc>
        internal bool StateSelected {
            get {
                return state[StateSelectedSection] == 1;
            }
            set {
                state[StateSelectedSection] = value ? 1 : 0;
            }
        }
        
        /// <devdoc>
        ///     Accessor for our state bit vector.
        /// </devdoc>
        private int SubItemCount {
            get {
                return state[SubItemCountSection];
            }
            set {
                state[SubItemCountSection] = value;
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.SubItems"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),        
        SRDescription(nameof(SR.ListViewItemSubItemsDescr)),
        Editor("System.Windows.Forms.Design.ListViewSubItemCollectionEditor, " + AssemblyRef.SystemDesign,typeof(UITypeEditor)),
        ]
        public ListViewSubItemCollection SubItems {
            get {
                if (SubItemCount == 0) {
                    subItems = new ListViewSubItem[1];
                    subItems[0] = new ListViewSubItem(this, string.Empty);                        
                    SubItemCount = 1;
                }
            
                if (listViewSubItemCollection == null) {
                    listViewSubItemCollection = new ListViewSubItemCollection(this);
                }
                return listViewSubItemCollection;
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Tag"]/*' />
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

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Text"]/*' />
        /// <devdoc>
        ///     Text associated with this ListViewItem
        /// </devdoc>
        [
        Localizable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public string Text {
            get {
                if (SubItemCount == 0) {
                    return string.Empty;
                }
                else {
                    return subItems[0].Text;
                }
            }
            set {
                SubItems[0].Text = value;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ToolTipText"]/*' />
        /// <devdoc>
        ///     Tool tip text associated with this ListViewItem
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue("")
        ]
        public string ToolTipText {
            get {
                return toolTipText;
            }
            set {
                if (value == null)
                    value = String.Empty;
                if (WindowsFormsUtils.SafeCompareStrings(toolTipText, value, false /*ignoreCase*/)) {
                    return;
                }

                toolTipText = value;

                // tell the list view about this change
                if (this.listView != null && this.listView.IsHandleCreated) {
                    this.listView.ListViewItemToolTipChanged(this);
                }
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.UseItemStyleForSubItems"]/*' />
        /// <devdoc>
        ///     Whether or not the font and coloring for the ListViewItem will be used for all of its subitems.
        ///     If true, the ListViewItem style will be used when drawing the subitems.
        ///     If false, the ListViewItem and its subitems will be drawn in their own individual styles
        ///     if any have been set.
        /// </devdoc>
        [
        DefaultValue(true),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public bool UseItemStyleForSubItems {
            get {
                return state[StateWholeRowOneStyleSection] == 1;
            }
            set {
                state[StateWholeRowOneStyleSection] = value ? 1 : 0;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.BeginEdit"]/*' />
        /// <devdoc>
        ///     Initiate editing of the item's label.
        ///     Only effective if LabelEdit property is true.
        /// </devdoc>
        public void BeginEdit() {
            if (Index >= 0) {
                ListView lv = ListView;
                if (lv.LabelEdit == false)
                    throw new InvalidOperationException(SR.ListViewBeginEditFailed);
                if (!lv.Focused)
                    lv.FocusInternal();
                UnsafeNativeMethods.SendMessage(new HandleRef(lv, lv.Handle), NativeMethods.LVM_EDITLABEL, Index, 0);
            }
        }                   
                   
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Clone"]/*' />
        public virtual object Clone() {
            ListViewSubItem[] clonedSubItems = new ListViewSubItem[this.SubItems.Count];
            for(int index=0; index < this.SubItems.Count; ++index) {
                ListViewSubItem subItem = this.SubItems[index];
                clonedSubItems[index] = new ListViewSubItem(null, 
                                                            subItem.Text, 
                                                            subItem.ForeColor, 
                                                            subItem.BackColor,
                                                            subItem.Font);
                clonedSubItems[index].Tag = subItem.Tag;
            }
        
            Type clonedType = this.GetType();
            ListViewItem newItem = null;
            
            if (clonedType == typeof(ListViewItem)) {
                newItem = new ListViewItem(clonedSubItems, this.ImageIndexer.Index);
            }
            else { 
                // 

                newItem = (ListViewItem)Activator.CreateInstance(clonedType);
            }
            newItem.subItems = clonedSubItems;
            newItem.ImageIndexer.Index = this.ImageIndexer.Index;
            newItem.SubItemCount = this.SubItemCount;
            newItem.Checked = this.Checked;
            newItem.UseItemStyleForSubItems = this.UseItemStyleForSubItems;
            newItem.Tag = this.Tag;

            // Only copy over the ImageKey if we're using it.
            if (!String.IsNullOrEmpty(this.ImageIndexer.Key)) {
                newItem.ImageIndexer.Key = this.ImageIndexer.Key;
            }

            newItem.indentCount = this.indentCount;
            newItem.StateImageIndex = this.StateImageIndex;
            newItem.toolTipText = this.toolTipText;
            newItem.BackColor = this.BackColor;
            newItem.ForeColor = this.ForeColor;
            newItem.Font = this.Font;
            newItem.Text = this.Text;
            newItem.Group = this.Group;
            
            return newItem;
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.EnsureVisible"]/*' />
        /// <devdoc>
        ///     Ensure that the item is visible, scrolling the view as necessary.
        /// </devdoc>
        public virtual void EnsureVisible() {
            if (listView != null && listView.IsHandleCreated) {
                listView.EnsureVisible(Index);
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.FindNearestItem"]/*' />
        public ListViewItem FindNearestItem(SearchDirectionHint searchDirection) {
            Rectangle r = this.Bounds;

            switch (searchDirection) {
                case SearchDirectionHint.Up: 
                    return this.ListView.FindNearestItem(searchDirection, r.Left, r.Top);
                case SearchDirectionHint.Down:
                    return this.ListView.FindNearestItem(searchDirection, r.Left, r.Bottom);
                case SearchDirectionHint.Left:
                    return this.ListView.FindNearestItem(searchDirection, r.Left, r.Top);
                case SearchDirectionHint.Right:
                    return this.ListView.FindNearestItem(searchDirection, r.Right, r.Top);
                default :
                    Debug.Fail("we handled all the 4 directions");
                    return null;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.GetBounds"]/*' />
        /// <devdoc>
        ///     Returns a specific portion of the ListViewItem's bounding rectangle.
        ///     The rectangle returned is empty if the ListViewItem has not been added to a ListView control.
        /// </devdoc>
        public Rectangle GetBounds(ItemBoundsPortion portion) {
            if (listView != null && listView.IsHandleCreated) {
                return listView.GetItemRect(Index, portion);
            }
            else return new Rectangle();
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.GetSubItemAt"]/*' />
        public ListViewSubItem GetSubItemAt(int x, int y) {
            if (listView != null && listView.IsHandleCreated && listView.View == View.Details) {
                int iItem = -1;
                int iSubItem = -1;

                listView.GetSubItemAt(x,y, out iItem, out iSubItem);

                // 


                if (iItem == this.Index && iSubItem != -1 && iSubItem < SubItems.Count) {
                    return SubItems[iSubItem];
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Host"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void Host(ListView parent, int ID, int index) {
            // Don't let the name "host" fool you -- Handle is not necessarily created
            Debug.Assert(this.listView == null || !this.listView.VirtualMode, "ListViewItem::Host can't be used w/ a virtual item");
            Debug.Assert(parent == null || !parent.VirtualMode, "ListViewItem::Host can't be used w/ a virtual list");
            
            this.ID = ID;
            listView = parent;

            // If the index is valid, then the handle has been created.
            if (index != -1) {
                UpdateStateToListView(index);
            }
        }

        /// <devdoc>
        ///     This is used to map list view items w/ their respective groups
        ///     in localized forms.
        /// </devdoc>
        internal void UpdateGroupFromName() {
            Debug.Assert(this.listView != null, "This method is used only when items are parented in a list view");
            Debug.Assert(!this.listView.VirtualMode, "we need to update the group only when the user specifies the list view items in localizable forms");
            if (String.IsNullOrEmpty(this.groupName)) {
                return;
            }

            ListViewGroup group = this.listView.Groups[this.groupName];
            this.Group = group;

            // Use the group name only once.
            this.groupName = null;
        }

        internal void UpdateStateToListView(int index) {
            NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
            UpdateStateToListView(index, ref lvItem, true);
        }
        
        /// <devdoc>
        ///     Called when we have just pushed this item into a list view and we need
        ///     to configure the list view's state for the item.  Use a valid index
        ///     if you can, or use -1 if you can't.
        /// </devdoc>
        internal void UpdateStateToListView(int index, ref NativeMethods.LVITEM lvItem, bool updateOwner) {

            Debug.Assert(listView.IsHandleCreated, "Should only invoke UpdateStateToListView when handle is created.");
            
            if (index == -1) {
                index = Index;
            }
            else {
                lastIndex = index;
            }
            
            // Update Item state in one shot
            //
            int itemState = 0;
            int stateMask = 0;
            
            if (StateSelected) {
                itemState |= NativeMethods.LVIS_SELECTED;
                stateMask |= NativeMethods.LVIS_SELECTED;
            }
            
            if (SavedStateImageIndex > -1) {
                itemState |= ((SavedStateImageIndex + 1) << 12);
                stateMask |= NativeMethods.LVIS_STATEIMAGEMASK;
            }                        
            
            lvItem.mask |= NativeMethods.LVIF_STATE;
            lvItem.iItem = index;                        
            lvItem.stateMask |= stateMask;
            lvItem.state |= itemState;

            if (listView.GroupsEnabled) {
                lvItem.mask |= NativeMethods.LVIF_GROUPID;                
                lvItem.iGroupId = listView.GetNativeGroupId(this);
                
                Debug.Assert(!updateOwner || listView.SendMessage(NativeMethods.LVM_ISGROUPVIEWENABLED, 0, 0) != IntPtr.Zero, "Groups not enabled");
                Debug.Assert(!updateOwner || listView.SendMessage(NativeMethods.LVM_HASGROUP, lvItem.iGroupId, 0) != IntPtr.Zero, "Doesn't contain group id: " + lvItem.iGroupId.ToString(CultureInfo.InvariantCulture));
            }            

            if (updateOwner) {
                UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), NativeMethods.LVM_SETITEM, 0, ref lvItem);
            }
        }
        
        internal void UpdateStateFromListView(int displayIndex, bool checkSelection) {
            if (listView != null && listView.IsHandleCreated && displayIndex != -1) {

                // Get information from comctl control
                //
                NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
                lvItem.mask = NativeMethods.LVIF_PARAM | NativeMethods.LVIF_STATE | NativeMethods.LVIF_GROUPID;
             
                if (checkSelection) {
                    lvItem.stateMask = NativeMethods.LVIS_SELECTED;
                }
                

                // we want to get all the information, including the state image mask
                lvItem.stateMask |= NativeMethods.LVIS_STATEIMAGEMASK;

                if (lvItem.stateMask == 0) {
                    // perf optimization: no work to do.
                    //
                    return;
                }


                lvItem.iItem = displayIndex;
                UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), NativeMethods.LVM_GETITEM, 0, ref lvItem);                
                
                // Update this class' information
                //
                if (checkSelection) {
                    StateSelected = (lvItem.state & NativeMethods.LVIS_SELECTED) != 0;
                }
                SavedStateImageIndex = ((lvItem.state & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) - 1;

                group = null;
                foreach (ListViewGroup lvg in ListView.Groups) {
                    if (lvg.ID == lvItem.iGroupId) {
                        group = lvg;
                        break;
                    }
                }
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.UnHost"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void UnHost(bool checkSelection) {
            UnHost(Index, checkSelection);
        }

        internal void UnHost(int displayIndex, bool checkSelection) {
            UpdateStateFromListView(displayIndex, checkSelection);
            
            if (this.listView != null && (this.listView.Site == null || !this.listView.Site.DesignMode) && this.group != null) {
                this.group.Items.Remove(this);
            }

            // Make sure you do these last, as the first several lines depends on this information
            ID = -1;
            listView = null;            
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Remove"]/*' />
        public virtual void Remove() {
            if (listView != null) {
                listView.Items.Remove(this);
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Deserialize"]/*' />
        protected virtual void Deserialize(SerializationInfo info, StreamingContext context) {

            bool foundSubItems = false;

            string imageKey = null;
            int imageIndex = -1;
            
            foreach (SerializationEntry entry in info) {
                if (entry.Name == "Text") {
                    Text = info.GetString(entry.Name);
                }
                else if (entry.Name == "ImageIndex") {
                    imageIndex = info.GetInt32(entry.Name);
                }
                else if (entry.Name == "ImageKey") { 
                    imageKey = info.GetString(entry.Name);
                }
                else if (entry.Name == "SubItemCount") {
                    SubItemCount = info.GetInt32(entry.Name);
                    // foundSubItems true only if count > 0
                    if (SubItemCount > 0)
                    {
                        foundSubItems = true;
                    }
                }
                else if (entry.Name == "BackColor") {
                    BackColor = (Color)info.GetValue(entry.Name, typeof(Color));
                }
                else if (entry.Name == "Checked") {
                    Checked = info.GetBoolean(entry.Name);
                }
                else if (entry.Name == "Font") {
                    Font = (Font)info.GetValue(entry.Name, typeof(Font));
                }
                else if (entry.Name == "ForeColor") {
                    ForeColor = (Color)info.GetValue(entry.Name, typeof(Color));
                }
                else if (entry.Name == "UseItemStyleForSubItems") {
                    UseItemStyleForSubItems = info.GetBoolean(entry.Name);
                }
                else if (entry.Name == "Group") {
                    ListViewGroup group = (ListViewGroup) info.GetValue(entry.Name, typeof(ListViewGroup));
                    this.groupName = group.Name;
                }
            }

            // let image key take precidence
            if (imageKey != null) {
                ImageKey = imageKey;    
            } 
            else if (imageIndex != -1) {
                ImageIndex = imageIndex;
            }

            if (foundSubItems) {
                ListViewSubItem[] newItems = new ListViewSubItem[SubItemCount];
                for (int i = 1; i < SubItemCount; i++) {
                    ListViewSubItem newItem = (ListViewSubItem)info.GetValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), typeof(ListViewSubItem));
                    newItem.owner = this;
                    newItems[i] = newItem;
                }
                newItems[0] = subItems[0];
                subItems = newItems;
            }
        }

        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.Serialize"]/*' />
        /// <devdoc>
        ///     Saves this ListViewItem object to the given data stream.
        /// </devdoc>
        /// 
       	[SecurityPermissionAttribute(SecurityAction.Demand, Flags=SecurityPermissionFlag.SerializationFormatter), 		
         SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        protected virtual void Serialize(SerializationInfo info, StreamingContext context) {
            info.AddValue("Text", Text);
            info.AddValue("ImageIndex", ImageIndexer.Index);  
            if (!String.IsNullOrEmpty(ImageIndexer.Key)) {
                info.AddValue("ImageKey", ImageIndexer.Key);
            }
            if (SubItemCount > 1) {
                info.AddValue("SubItemCount", SubItemCount);
                for (int i = 1; i < SubItemCount; i++) {
                    info.AddValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), subItems[i], typeof(ListViewSubItem));
                }
            }
            info.AddValue("BackColor", BackColor);
            info.AddValue("Checked", Checked);
            info.AddValue("Font", Font);
            info.AddValue("ForeColor", ForeColor);
            info.AddValue("UseItemStyleForSubItems", UseItemStyleForSubItems);
            if (this.Group != null) {
                info.AddValue("Group", this.Group);
            }
        }

        // we need this function to set the index when the list view is in virtual mode.
        // the index of the list view item is used in ListView::set_TopItem property
        internal void SetItemIndex(ListView listView, int index) {
            Debug.Assert(listView != null && listView.VirtualMode, "ListViewItem::SetItemIndex should be used only when the list is virtual");
            Debug.Assert(index > -1, "can't set the index on a virtual list view item to -1");
            this.listView = listView;
            this.lastIndex = index;
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ShouldSerializeText"]/*' />
        internal bool ShouldSerializeText() {
            return false;
        }
        
        private bool ShouldSerializePosition() {
            return !this.position.Equals(new Point(-1,-1));
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ToString"]/*' />
        public override string ToString() {
            return "ListViewItem: {" + Text + "}";
        }
        
        // The ListItem's state (or a SubItem's state) has changed, so invalidate the ListView control
        internal void InvalidateListView() {
            if (listView != null && listView.IsHandleCreated) {
                listView.Invalidate();
            }
        }
        
        internal void UpdateSubItems(int index){
            UpdateSubItems(index, SubItemCount);
        }

        internal void UpdateSubItems(int index, int oldCount){
            if (listView != null && listView.IsHandleCreated) {
                int subItemCount = SubItemCount;
                
                int itemIndex = Index;
                    
                if (index != -1) {
                    listView.SetItemText(itemIndex, index, subItems[index].Text);
                }
                else {
                    for(int i=0; i < subItemCount; i++) {
                        listView.SetItemText(itemIndex, i, subItems[i].Text);
                    }
                }

                for (int i = subItemCount; i < oldCount; i++) {
                    listView.SetItemText(itemIndex, i, string.Empty);
                }
            }
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ISerializable.GetObjectData"]/*' />
        /// <internalonly/>        
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]       
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
            Serialize(info, context);
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem"]/*' />
        [
            TypeConverterAttribute(typeof(ListViewSubItemConverter)),
            ToolboxItem(false),
            DesignTimeVisible(false),
            DefaultProperty(nameof(Text)),
            Serializable
        ]
        public class ListViewSubItem {
        
            [NonSerialized]
            internal ListViewItem owner;

            private string text;

            [OptionalField(VersionAdded=2)]
            private string name = null;

            private SubItemStyle style;

            [OptionalField(VersionAdded=2)]
            private object userData;
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ListViewSubItem"]/*' />
            public ListViewSubItem() {
            }
                
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ListViewSubItem1"]/*' />
            public ListViewSubItem(ListViewItem owner, string text) {
                this.owner = owner;
                this.text = text;
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ListViewSubItem2"]/*' />
            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font) {
                this.owner = owner;
                this.text = text;
                this.style = new SubItemStyle();
                this.style.foreColor = foreColor;
                this.style.backColor = backColor;
                this.style.font = font;
            }


            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.BackColor"]/*' />
            public Color BackColor {
                get {
                    if (style != null && style.backColor != Color.Empty) {
                        return style.backColor;
                    }
                    
                    if (owner != null && owner.listView != null) {
                        return owner.listView.BackColor;
                    }
                    
                    return SystemColors.Window;
                }
                set {
                    if (style == null) {
                        style = new SubItemStyle();
                    }
                    
                    if (style.backColor != value) {
                        style.backColor = value;
                        if (owner != null) {
                            owner.InvalidateListView();
                        }
                    }
                }
            }
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.Bounds"]/*' />
            [Browsable(false)]
            public Rectangle Bounds {
                get {
                    if(owner != null && owner.listView != null && owner.listView.IsHandleCreated) {
                        return owner.listView.GetSubItemRect(owner.Index, owner.SubItems.IndexOf(this));
                    } else {
                        return Rectangle.Empty;
                    }
                }
            }

            internal bool CustomBackColor {
                get {
                    return style != null && !style.backColor.IsEmpty;
                }
            }

            internal bool CustomFont {
                get {
                    return style != null && style.font != null;
                }
            }

            internal bool CustomForeColor {
                get {
                    return style != null && !style.foreColor.IsEmpty;
                }
            }

            internal bool CustomStyle {
                get {
                    return style != null;
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.Font"]/*' />
            [
            Localizable(true)
            ]
            public Font Font {
                get {
                    if (style != null && style.font != null) {
                        return style.font;
                    }
                    
                    if (owner != null && owner.listView != null) {
                        return owner.listView.Font;
                    }
                    
                    return Control.DefaultFont;
                }
                set {
                    if (style == null) {
                        style = new SubItemStyle();
                    }
                    
                    if (style.font != value) {
                        style.font = value;
                        if (owner != null) {
                            owner.InvalidateListView();
                        }
                    }
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ForeColor"]/*' />
            public Color ForeColor {
                get {
                    if (style != null && style.foreColor != Color.Empty) {
                        return style.foreColor;
                    }
                    
                    if (owner != null && owner.listView != null) {
                        return owner.listView.ForeColor;
                    }
                    
                    return SystemColors.WindowText;
                }
                set {
                    if (style == null) {
                        style = new SubItemStyle();
                    }
                    
                    if (style.foreColor != value) {
                        style.foreColor = value;
                        if (owner != null) {
                            owner.InvalidateListView();
                        }
                    }
                }
            }

            /// <include file='doc\ListViewSubItem.uex' path='docs/doc[@for="ListViewSubItem.Tag"]/*' />
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
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.Text"]/*' />
            [
            Localizable(true)
            ]
            public string Text {
                get {
                    return text == null ? "" : text;
                }
                set {
                    text = value;
                    if (owner != null) {
                        owner.UpdateSubItems(-1);
                    }
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.Name"]/*' />
            [
            Localizable(true)
            ]
            public string Name {
                get {
                    return (name == null) ? "": name;
                }
                set {
                    name = value;
                    if (owner != null) {
                        owner.UpdateSubItems(-1);
                    }
                }
            }

            //
            // Fix for Serialization Breaking change from v1.* to v2.0
            //
            // see http://devdiv/SpecTool/SHADOW/Documents/Whidbey/Versioning/VersionTolerantSerializationGuidelines(1.1).doc
            //
            [OnDeserializing]
            private void OnDeserializing(StreamingContext ctx) {
            }

            [OnDeserialized]
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
            private void OnDeserialized(StreamingContext ctx) {
                this.name = null;
                this.userData = null;
            }

            [OnSerializing]
            private void OnSerializing(StreamingContext ctx) {
            }

            [OnSerialized]
            private void OnSerialized(StreamingContext ctx) {
            }

            //
            // End fix for Serialization Breaking change from v1.* to v2.0
            //
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ResetStyle"]/*' />
            public void ResetStyle() {
                if (style != null) {
                    style = null;
                    if (owner != null) {
                        owner.InvalidateListView();                                                                    
                    }
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItem.ToString"]/*' />
            public override string ToString() {
                return "ListViewSubItem: {" + Text + "}";
            }
            
            [Serializable]
            private class SubItemStyle {
                public Color backColor = Color.Empty;
                public Color foreColor = Color.Empty;
                public Font font = null;
            }            
        }
        
        /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection"]/*' />
        public class ListViewSubItemCollection : IList {
            private ListViewItem owner;


            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;
        

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.ListViewSubItemCollection"]/*' />
            public ListViewSubItemCollection(ListViewItem owner) {
                this.owner = owner;
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Count"]/*' />
            /// <devdoc>
            ///     Returns the total number of items within the list view.
            /// </devdoc>
            [Browsable(false)]
            public int Count {
                get {
                    return owner.SubItemCount;
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return true;
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.this"]/*' />
            /// <devdoc>
            ///     Returns a ListViewSubItem given it's zero based index into the ListViewSubItemCollection.
            /// </devdoc>
            public ListViewSubItem this[int index] {
                get {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

                    return owner.subItems[index];
                }
                set {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));

                    owner.subItems[index] = value;
                    owner.UpdateSubItems(index);                    
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    if (value is ListViewSubItem) {
                        this[index] = (ListViewSubItem)value;
                    }
                    else {
                        throw new ArgumentException(SR.ListViewBadListViewSubItem,"value");
                    }
                }
            }
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual ListViewSubItem this[string key] {
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

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Add"]/*' />
            public ListViewSubItem Add(ListViewSubItem item) {
                EnsureSubItemSpace(1, -1);
                item.owner = this.owner;
                owner.subItems[owner.SubItemCount] = item;
                owner.UpdateSubItems(owner.SubItemCount++);
                return item;    
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.Add"]/*' />
            public ListViewSubItem Add(string text) {
                ListViewSubItem item = new ListViewSubItem(owner, text);
                Add(item);                
                return item;
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.Add1"]/*' />
            public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font) {
                ListViewSubItem item = new ListViewSubItem(owner, text, foreColor, backColor, font);
                Add(item);                
                return item;
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.AddRange"]/*' />
            public void AddRange(ListViewSubItem[] items) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSubItemSpace(items.Length, -1);
                
                foreach(ListViewSubItem item in items) {
                    if (item != null) {
                        owner.subItems[owner.SubItemCount++] = item;
                    }
                }
                
                owner.UpdateSubItems(-1);
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.AddRange1"]/*' />
            public void AddRange(string[] items) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSubItemSpace(items.Length, -1);
                
                foreach(string item in items) {
                    if (item != null) {
                        owner.subItems[owner.SubItemCount++] = new ListViewSubItem(owner, item);
                    }
                }
                
                owner.UpdateSubItems(-1);
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.AddRange2"]/*' />
            public void AddRange(string[] items, Color foreColor, Color backColor, Font font) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSubItemSpace(items.Length, -1);
                
                foreach(string item in items) {
                    if (item != null) {
                        owner.subItems[owner.SubItemCount++] = new ListViewSubItem(owner, item, foreColor, backColor, font);
                    }
                }
                
                owner.UpdateSubItems(-1);
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object item) {
                if (item is ListViewSubItem) {
                    return IndexOf(Add((ListViewSubItem)item));
                }
                else {
                    throw new ArgumentException(SR.ListViewSubItemCollectionInvalidArgument);
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Clear"]/*' />
            public void Clear() {
                int oldCount = owner.SubItemCount;
                if (oldCount > 0) {
                    owner.SubItemCount = 0;
                    owner.UpdateSubItems(-1, oldCount);
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Contains"]/*' />
            public bool Contains(ListViewSubItem subItem) {
                return IndexOf(subItem) != -1;
            }
            

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object subItem) {
                if (subItem is ListViewSubItem) {
                    return Contains((ListViewSubItem)subItem);
                }
                else {
                    return false;
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
                return IsValidIndex(IndexOfKey(key)); 
            }
            
            /// <devdoc>
            ///     Ensures that the sub item array has the given
            ///     capacity.  If it doesn't, it enlarges the
            ///     array until it does.  If index is -1, additional
            ///     space is tacked onto the end.  If it is a valid
            ///     insertion index into the array, this will move
            ///     the array data to accomodate the space.
            /// </devdoc>
            private void EnsureSubItemSpace(int size, int index) {
            
                // Range check subItems.
                if (owner.SubItemCount == ListViewItem.MAX_SUBITEMS) {
                    throw new InvalidOperationException(SR.ErrorCollectionFull);
                }
                
                if (owner.SubItemCount + size > owner.subItems.Length) {
                
                    // must grow array.  Don't do it just by size, though;
                    // chunk it for efficiency.
                    
                    if (owner.subItems == null) {
                        int newSize = (size > 4) ? size : 4;
                        owner.subItems = new ListViewSubItem[newSize];
                    }
                    else {
                        int newSize = owner.subItems.Length * 2;
                        while(newSize - owner.SubItemCount < size) {
                            newSize *= 2;
                        }
                        
                        ListViewSubItem[] newItems = new ListViewSubItem[newSize];
                        
                        // Now, when copying to the member variable, use index
                        // if it was provided.
                        //
                        if (index != -1) {
                            Array.Copy(owner.subItems, 0, newItems, 0, index);
                            Array.Copy(owner.subItems, index, newItems, index + size, owner.SubItemCount - index);
                        }
                        else {
                            Array.Copy(owner.subItems, newItems, owner.SubItemCount);
                        }
                        owner.subItems = newItems;
                    }
                }
                else {
                
                    // We had plenty of room.  Just move the items if we need to
                    //
                    if (index != -1) {
                        for(int i = owner.SubItemCount - 1; i >= index; i--) {
                            owner.subItems[i + size] = owner.subItems[i];
                        }
                    }
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.IndexOf"]/*' />
            public int IndexOf(ListViewSubItem subItem) {
                for(int index=0; index < Count; ++index) {
                    if (owner.subItems[index] == subItem) {
                        return index;
                    }
                }    
                return -1;
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object subItem) {
                if (subItem is ListViewSubItem) {
                    return IndexOf((ListViewSubItem)subItem);
                }
                else {
                    return -1;
                }
            }
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.this"]/*' />
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

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/> 
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Insert"]/*' />
            public void Insert(int index, ListViewSubItem item) {
            
                if (index < 0 || index > Count) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                
                item.owner = owner;
                
                EnsureSubItemSpace(1, index);
            
                // Insert new item
                //
                owner.subItems[index] = item;
                owner.SubItemCount++;
                owner.UpdateSubItems(-1);
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object item) {
                if (item is ListViewSubItem) {
                    Insert(index, (ListViewSubItem)item);
                }
                else {
                    throw new ArgumentException(SR.ListViewBadListViewSubItem,"item");
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.Remove"]/*' />
            public void Remove(ListViewSubItem item) {
                int index = IndexOf(item);
                if (index != -1) {                    
                    RemoveAt(index);
                }
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object item) {
                if (item is ListViewSubItem) {
                    Remove((ListViewSubItem)item);
                }                
            }
            
            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.RemoveAt"]/*' />
            public void RemoveAt(int index) {
            
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                
                // Collapse the items
                for (int i = index + 1; i < owner.SubItemCount; i++) {
                    owner.subItems[i - 1] = owner.subItems[i];
                }

                int oldCount = owner.SubItemCount;
                owner.SubItemCount--;
                owner.subItems[owner.SubItemCount] = null;
                owner.UpdateSubItems(-1, oldCount);
            }

          /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.RemoveByKey"]/*' />
          /// <devdoc>
          ///     <para>Removes the child control with the specified key.</para>
          /// </devdoc>
          public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index); 
                 }
           }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewSubItemCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                if (Count > 0) {
                    System.Array.Copy(owner.subItems, 0, dest, index, Count);           
                }
            }

            /// <include file='doc\ListViewItem.uex' path='docs/doc[@for="ListViewItem.ListViewSubItemCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                if (owner.subItems != null) {
                    return new WindowsFormsUtils.ArraySubsetEnumerator(owner.subItems, owner.SubItemCount);
                }   
                else 
                {
                    return new ListViewSubItem[0].GetEnumerator();
                }

            }
        }
    }
}
