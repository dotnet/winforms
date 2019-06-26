// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    // [Serializable] necessary why???  (@Devendar)

    /// <summary>
    /// Implements an item of a <see cref='System.Windows.Forms.ListView'/>.
    /// </summary>
    [TypeConverterAttribute(typeof(ListViewItemConverter))]
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultProperty(nameof(Text))]
    [Serializable]
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    public partial class ListViewItem : ICloneable, ISerializable
    {
        internal ListView listView;

        private const int MaxSubItems = 4096;

        private static readonly BitVector32.Section s_stateSelectedSection = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section s_stateImageMaskSet = BitVector32.CreateSection(1, s_stateSelectedSection);
        private static readonly BitVector32.Section s_stateWholeRowOneStyleSection = BitVector32.CreateSection(1, s_stateImageMaskSet);
        private static readonly BitVector32.Section s_avedStateImageIndexSection = BitVector32.CreateSection(15, s_stateWholeRowOneStyleSection);
        private static readonly BitVector32.Section s_subItemCountSection = BitVector32.CreateSection(MaxSubItems, s_avedStateImageIndexSection);

        private int _indentCount = 0;
        private Point _position;
        private static readonly Point DefaultPosition = new Point(-1, -1);

        internal ListViewGroup group;
        private string _groupName;

        private ListViewSubItemCollection _listViewSubItemCollection;
        private ListViewSubItem[] _listViewSubItems;

        // we stash the last index we got as a seed to GetDisplayIndex.
        private int _lastIndex = DefaultLastIndex;
        private const int DefaultLastIndex = -1;
        private const int MaxStateImageIndex = 14;

        // An ID unique relative to a given list view that comctl uses to identify items.
        internal int ID = -1;

        private BitVector32 _state = new BitVector32();
        private ListViewItemImageIndexer _imageIndexer;
        private string _toolTipText;
        private object _userData;

        #region Constructors

        public ListViewItem()
        {
            StateSelected = false;
            UseItemStyleForSubItems = true;
            SavedStateImageIndex = -1;
        }

        /// <summary>
        /// Creates a ListViewItem object from an Stream.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Changing this would be a breaking change")]
        protected ListViewItem(SerializationInfo info, StreamingContext context) : this()
        {
            Deserialize(info, context);
        }

        public ListViewItem(string text) : this(text, -1)
        {
        }

        public ListViewItem(string text, int imageIndex) : this()
        {
            ImageIndexer.Index = imageIndex;
            Text = text;
        }

        public ListViewItem(string[] items) : this(items, -1)
        {
        }

        public ListViewItem(string[] items, int imageIndex) : this()
        {
            ImageIndexer.Index = imageIndex;
            if (items?.Length > 0)
            {
                _listViewSubItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    _listViewSubItems[i] = new ListViewSubItem(this, items[i]);
                }
                SubItemCount = items.Length;
            }
        }

        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font) : this(items, imageIndex)
        {
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }

        public ListViewItem(ListViewSubItem[] subItems, int imageIndex) : this()
        {
            _listViewSubItems = subItems ?? throw new ArgumentNullException(nameof(subItems));
            ImageIndexer.Index = imageIndex;
            SubItemCount = subItems.Length;

            // Update the owner of these subitems
            for (int i = 0; i < subItems.Length; i++)
            {
                if (subItems[i] == null)
                {
                    throw new ArgumentNullException(nameof(subItems));
                }

                subItems[i].owner = this;
            }
        }

        public ListViewItem(ListViewGroup group) : this()
        {
            Group = group;
        }

        public ListViewItem(string text, ListViewGroup group) : this(text)
        {
            Group = group;
        }

        public ListViewItem(string text, int imageIndex, ListViewGroup group) : this(text, imageIndex)
        {
            Group = group;
        }

        public ListViewItem(string[] items, ListViewGroup group) : this(items)
        {
            Group = group;
        }

        public ListViewItem(string[] items, int imageIndex, ListViewGroup group) : this(items, imageIndex)
        {
            Group = group;
        }

        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group) :
            this(items, imageIndex, foreColor, backColor, font)
        {
            Group = group;
        }

        public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group) : this(subItems, imageIndex)
        {
            Group = group;
        }

        public ListViewItem(string text, string imageKey) : this()
        {
            ImageIndexer.Key = imageKey;
            Text = text;
        }

        public ListViewItem(string[] items, string imageKey) : this()
        {
            ImageIndexer.Key = imageKey;
            if (items != null && items.Length > 0)
            {
                _listViewSubItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    _listViewSubItems[i] = new ListViewSubItem(this, items[i]);
                }
                SubItemCount = items.Length;
            }
        }

        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font) : this(items, imageKey)
        {
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }

        public ListViewItem(ListViewSubItem[] subItems, string imageKey) : this()
        {
            ImageIndexer.Key = imageKey;
            this._listViewSubItems = subItems ?? throw new ArgumentNullException(nameof(subItems));
            SubItemCount = subItems.Length;

            // Update the owner of these subitems
            for (int i = 0; i < subItems.Length; i++)
            {
                if (subItems[i] == null)
                {
                    throw new ArgumentNullException(nameof(subItems));
                }

                subItems[i].owner = this;
            }
        }

        public ListViewItem(string text, string imageKey, ListViewGroup group) : this(text, imageKey)
        {
            Group = group;
        }

        public ListViewItem(string[] items, string imageKey, ListViewGroup group) : this(items, imageKey)
        {
            Group = group;
        }

        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group) :
            this(items, imageKey, foreColor, backColor, font)
        {
            Group = group;
        }

        public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group) : this(subItems, imageKey)
        {
            Group = group;
        }

        #endregion

        /// <summary>
        /// The font that this item will be displayed in. If its value is null, it will be displayed
        /// using the global font for the ListView control that hosts it.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
        public Color BackColor
        { 
            get => SubItemCount == 0 ?
                        (listView == null ?
                            SystemColors.Window :
                            listView.BackColor) :
                        _listViewSubItems[0].BackColor;
            
            set => SubItems[0].BackColor = value;
        }

        /// <summary>
        /// Returns the ListViewItem's bounding rectangle, including subitems. The bounding rectangle is empty if
        /// the ListViewItem has not been added to a ListView control.
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get => listView == null ?
                new Rectangle() :
                listView.GetItemRect(Index);        
        }

        [DefaultValue(false)]
        [RefreshPropertiesAttribute(RefreshProperties.Repaint)]
        [SRCategory(nameof(SR.CatAppearance))]
        public bool Checked
        {
            get => StateImageIndex > 0;

            set
            {
                if (Checked != value)
                {
                    if (listView?.IsHandleCreated == true)
                    {
                        StateImageIndex = value ? 
                            1 : 
                            0;

                        // the setter for StateImageIndex calls ItemChecked handler
                        // thus need to verify validity of the listView again
                        if (listView?.UseCompatibleStateImageBehavior == false)
                        {
                            if (!listView.CheckBoxes)
                            {
                                listView.UpdateSavedCheckedItems(this, value);
                            }
                        }
                    }
                    else
                    {
                        SavedStateImageIndex = value ? 
                            1 : 
                            0;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the focus state of the ListViewItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Focused
        {
            get => listView?.IsHandleCreated == true ?
                listView.GetItemState(Index, NativeMethods.LVIS_FOCUSED) != 0 :
                false;

            set
            {
                if (listView?.IsHandleCreated == true)
                {
                    listView.SetItemState(Index, 
                        value ? 
                            NativeMethods.LVIS_FOCUSED : 
                            0,
                        NativeMethods.LVIS_FOCUSED);
                }
            }
        }

        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
        public Font Font
        {
            get => SubItemCount == 0 ?
                (listView != null ?
                    listView.Font :
                    Control.DefaultFont) :
                _listViewSubItems[0].Font;

            set => SubItems[0].Font = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
        public Color ForeColor
        {
            get => SubItemCount == 0 ?
                        (listView != null ?
                            listView.ForeColor :
                            SystemColors.WindowText) :
                        _listViewSubItems[0].ForeColor;

            set => SubItems[0].ForeColor = value;          
        }

        [DefaultValue(null)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatBehavior))]
        public ListViewGroup Group
        {
            get => group;
            set
            {
                if (group != value)
                {
                    if (value != null)
                    {
                        value.Items.Add(this);
                    }
                    else
                    {
                        group.Items.Remove(this);
                    }
                }

                Debug.Assert(group == value, "BUG: group member variable wasn't updated!");

                // If the user specifically sets the group then don't use the groupName again.
                _groupName = null;
            }
        }

        /// <summary>
        /// Returns the ListViewItem's currently set image index
        /// </summary>
        [DefaultValue(-1)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ListViewItemImageIndexDescr))]
        [TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter))]
        public int ImageIndex
        {
            get => (ImageIndexer.Index >= ImageList?.Images.Count) ?
                        ImageList.Images.Count - 1 :
                        ImageIndexer.Index;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }

                ImageIndexer.Index = value;

                if (listView?.IsHandleCreated == true)
                {
                    listView.SetItemImage(Index, ImageIndexer.ActualIndex);
                }
            }
        }

        internal ListViewItemImageIndexer ImageIndexer 
            => _imageIndexer ?? (_imageIndexer = new ListViewItemImageIndexer(this));

        /// <summary>
        /// Returns the ListViewItem's currently set image index
        /// </summary>
        [DefaultValue("")]
        [TypeConverterAttribute(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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

                if (listView?.IsHandleCreated == true)
                {
                    listView.SetItemImage(Index, ImageIndexer.ActualIndex);
                }
            }
        }

        [Browsable(false)]
        public ImageList ImageList
        {
            get
            {
                switch (listView?.View)
                {
                    case View.LargeIcon:
                    case View.Tile:
                        return listView.LargeImageList;
                    case View.SmallIcon:
                    case View.Details:
                    case View.List:
                        return listView.SmallImageList;
                    default:
                        return null;
                }
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
                    if (listView?.IsHandleCreated == true)
                    {
                        listView.SetItemIndentCount(Index, _indentCount);
                    }
                }
            }
        }

        /// <summary>
        /// Returns ListViewItem's current index in the listview, or -1 if it has not been added to a ListView control.
        /// </summary>
        [Browsable(false)]
        public int Index
        {
            get
            {
                if (listView == null)
                {
                    return DefaultLastIndex;
                }

                // if the list is virtual, the ComCtrl control does not keep any information
                // about any list view items, so we use our cache instead.
                if (!listView.VirtualMode)
                {
                    _lastIndex = listView.GetDisplayIndex(this, _lastIndex);
                }
                return _lastIndex;           
            }
        }

        /// <summary>
        /// Returns the ListView control that holds this ListViewItem. May be null if no
        /// control has been assigned yet.
        /// </summary>
        [Browsable(false)]
        public ListView ListView => listView;

        /// <summary>
        /// Name associated with this ListViewItem
        /// </summary>
        [Localizable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name
        {
            get => 
                SubItemCount == 0 ?
                    string.Empty :
                    _listViewSubItems[0].Name;
                            
            set => SubItems[0].Name = value;
        }

        [SRCategory(nameof(SR.CatDisplay))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Point Position
        {
            get
            {
                _position = listView?.IsHandleCreated == true ? 
                    listView.GetItemPosition(Index) :
                    DefaultPosition;

                return _position;
            }
            set
            {
                if (!value.Equals(_position))
                {
                    _position = value;
                    if (listView?.IsHandleCreated == true)
                    {
                        if (!listView.VirtualMode)
                        {
                            listView.SetItemPosition(Index, _position.X, _position.Y);
                        }
                    }
                }
            }
        }

        internal int RawStateImageIndex => (SavedStateImageIndex + 1) << 12;

        /// <summary>
        /// Accessor for our state bit vector.
        /// </summary>
        private int SavedStateImageIndex
        {
            get => _state[s_avedStateImageIndexSection] - 1;
            
            set
            {
                // flag whether we've set a value.
                _state[s_stateImageMaskSet] = (value == -1 ? 0 : 1);

                // push in the actual value
                _state[s_avedStateImageIndexSection] = value + 1;
            }
        }

        /// <summary>
        /// Treats the ListViewItem as a row of strings, and returns an array of those strings
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Selected
        {
            get => listView?.IsHandleCreated == true ?
                    listView.GetItemState(Index, NativeMethods.LVIS_SELECTED) != 0 :
                    StateSelected;
            
            set
            {
                if (listView?.IsHandleCreated == true)
                {
                    listView.SetItemState(Index, value ? NativeMethods.LVIS_SELECTED : 0, NativeMethods.LVIS_SELECTED);

                    // update comctl32's selection information.
                    listView.SetSelectionMark(Index);
                }
                else
                {
                    StateSelected = value;
                    if (listView?.IsHandleCreated == true)
                    {
                        // Set the selected state on the list view item only if the list view's Handle is already created.
                        listView.CacheSelectedStateForItem(this, value);
                    }
                }
            }
        }

        [Localizable(true)]
        [TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter))]
        [DefaultValue(-1)]
        [SRDescription(nameof(SR.ListViewItemStateImageIndexDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RelatedImageList("ListView.StateImageList")]
        public int StateImageIndex
        {
            get => listView?.IsHandleCreated == true ?
                    ((listView.GetItemState(Index, NativeMethods.LVIS_STATEIMAGEMASK) >> 12) - 1) :  // index is 1-based
                    SavedStateImageIndex;
            
            set
            {
                if (value < DefaultLastIndex || value > MaxStateImageIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(StateImageIndex), value));
                }

                if (listView?.IsHandleCreated == true)
                {
                    _state[s_stateImageMaskSet] = (value == -1 ? 
                        0 : 
                        1);

                    var state = ((value + 1) << 12);  // index is 1-based
                    listView.SetItemState(Index, state, NativeMethods.LVIS_STATEIMAGEMASK);
                }

                SavedStateImageIndex = value;
            }
        }

        internal bool StateImageSet => (_state[s_stateImageMaskSet] != 0);

        /// <summary>
        /// Accessor for our state bit vector.
        /// </summary>
        internal bool StateSelected
        {
            get => _state[s_stateSelectedSection] == 1;
            set => _state[s_stateSelectedSection] = value ? 
                1 : 
                0;
        }

        /// <summary>
        /// Accessor for our state bit vector.
        /// </summary>
        private int SubItemCount
        {
            get => _state[s_subItemCountSection];
            set => _state[s_subItemCountSection] = value;
        }

        [SRCategory(nameof(SR.CatData))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListViewItemSubItemsDescr))]
        [Editor("System.Windows.Forms.Design.ListViewSubItemCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public ListViewSubItemCollection SubItems
        {
            get
            {
                if (SubItemCount == 0)
                {
                    _listViewSubItems = new ListViewSubItem[1];
                    _listViewSubItems[0] = new ListViewSubItem(this, string.Empty);
                    SubItemCount = 1;
                }

                return _listViewSubItemCollection ?? (_listViewSubItemCollection = new ListViewSubItemCollection(this));
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get => _userData;
            set => _userData = value;
        }

        /// <summary>
        /// Text associated with this ListViewItem
        /// </summary>
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
        public string Text
        {
            get => SubItemCount == 0 ?
                    string.Empty :
                    _listViewSubItems[0].Text;

            set => SubItems[0].Text = value;
        }

        /// <summary>
        /// Tool tip text associated with this ListViewItem
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue("")]
        public string ToolTipText
        {
            get => _toolTipText ?? string.Empty;

            set
            {
                value = value ?? string.Empty;

                if (!WindowsFormsUtils.SafeCompareStrings(_toolTipText, value, ignoreCase: false))
                {
                    _toolTipText = value;

                    // tell the list view about this change
                    if (listView?.IsHandleCreated == true)
                    {
                        listView.ListViewItemToolTipChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// Whether or not the font and coloring for the ListViewItem will be used for all of its subitems.
        /// If true, the ListViewItem style will be used when drawing the subitems.
        /// If false, the ListViewItem and its subitems will be drawn in their own individual styles
        /// if any have been set.
        /// </summary>
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        public bool UseItemStyleForSubItems
        {
            get => _state[s_stateWholeRowOneStyleSection] == 1;
            set => _state[s_stateWholeRowOneStyleSection] = value ? 1 : 0;
        }

        /// <summary>
        /// Initiate editing of the item's label. Only effective if LabelEdit property is true.
        /// </summary>
        public void BeginEdit()
        {
            if (Index >= 0)
            {
                ListView lv = ListView;
                if (lv.LabelEdit == false)
                {
                    throw new InvalidOperationException(SR.ListViewBeginEditFailed);
                }
                if (!lv.Focused)
                {
                    lv.Focus();
                }

                UnsafeNativeMethods.SendMessage(new HandleRef(lv, lv.Handle), NativeMethods.LVM_EDITLABEL, Index, 0);
            }
        }

        public virtual object Clone()
        {
            var clonedSubItems = new ListViewSubItem[SubItems.Count];

            for (int index = 0; index < SubItems.Count; ++index)
            {
                var subItem = SubItems[index];
                clonedSubItems[index] = new ListViewSubItem(null,
                    subItem.Text,
                    subItem.ForeColor,
                    subItem.BackColor,
                    subItem.Font)
                    {
                        Tag = subItem.Tag
                    };
            }

            Type clonedType = GetType();
            ListViewItem newItem = clonedType == typeof(ListViewItem) ?
                new ListViewItem(clonedSubItems, ImageIndexer.Index) :
                Activator.CreateInstance(clonedType) as ListViewItem;
            
            newItem._listViewSubItems = clonedSubItems;
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
        /// Ensure that the item is visible, scrolling the view as necessary.
        /// </summary>
        public virtual void EnsureVisible()
        {
            if (listView != null && listView.IsHandleCreated)
            {
                listView.EnsureVisible(Index);
            }
        }

        public ListViewItem FindNearestItem(SearchDirectionHint searchDirection)
        {
            Rectangle rect = Bounds;
            switch (searchDirection)
            {
                case SearchDirectionHint.Up:
                    return ListView.FindNearestItem(searchDirection, rect.Left, rect.Top);
                case SearchDirectionHint.Down:
                    return ListView.FindNearestItem(searchDirection, rect.Left, rect.Bottom);
                case SearchDirectionHint.Left:
                    return ListView.FindNearestItem(searchDirection, rect.Left, rect.Top);
                case SearchDirectionHint.Right:
                    return ListView.FindNearestItem(searchDirection, rect.Right, rect.Top);
                default:
                    Debug.Fail("we handled all the 4 directions");
                    return null;
            }
        }

        /// <summary>
        /// Returns a specific portion of the ListViewItem's bounding rectangle.
        /// The rectangle returned is empty if the ListViewItem has not been added to a ListView control.
        /// </summary>
        public Rectangle GetBounds(ItemBoundsPortion portion)
            => listView?.IsHandleCreated == true ?
                    listView.GetItemRect(Index, portion) :
                    new Rectangle();
        

        public ListViewSubItem GetSubItemAt(int x, int y)
        {
            if (listView?.IsHandleCreated != true || listView.View != View.Details)
            {
                return null;
            }

            listView.GetSubItemAt(x, y, out int iItem, out int iSubItem);
            if (iItem == Index && iSubItem != -1 && iSubItem < SubItems.Count)
            {
                return SubItems[iSubItem];
            }

            return null;                     
        }

        internal void Host(ListView parent, int id, int index)
        {
            // Don't let the name "host" fool you -- Handle is not necessarily created
            Debug.Assert(listView == null || !listView.VirtualMode, "ListViewItem::Host can't be used w/ a virtual item");
            Debug.Assert(parent == null || !parent.VirtualMode, "ListViewItem::Host can't be used w/ a virtual list");

            ID = id;
            listView = parent;

            // If the index is valid, then the handle has been created.
            if (index != -1)
            {
                UpdateStateToListView(index);
            }
        }

        /// <summary>
        /// This is used to map list view items w/ their respective groups in localized forms.
        /// </summary>
        internal void UpdateGroupFromName()
        {
            Debug.Assert(listView != null, "This method is used only when items are parented in a list view");
            Debug.Assert(!listView.VirtualMode, "we need to update the group only when the user specifies the list view items in localizable forms");

            if (string.IsNullOrEmpty(_groupName))
            {
                return;
            }

            ListViewGroup group = listView.Groups[_groupName];
            Group = group;

            // Use the group name only once.
            _groupName = null;
        }

        internal void UpdateStateToListView(int index)
        {
            var lvItem = new NativeMethods.LVITEM();
            UpdateStateToListView(index, ref lvItem, true);
        }

        /// <summary>
        /// Called when we have just pushed this item into a list view and we need
        /// to configure the list view's state for the item. Use a valid index
        /// if you can, or use -1 if you can't.
        /// </summary>
        internal void UpdateStateToListView(int index, ref NativeMethods.LVITEM lvItem, bool updateOwner)
        {
            Debug.Assert(listView.IsHandleCreated, "Should only invoke UpdateStateToListView when handle is created.");

            if (index == -1)
            {
                index = Index;
            }
            else
            {
                _lastIndex = index;
            }

            // Update Item state in one shot
            int itemState = 0;
            int stateMask = 0;

            if (StateSelected)
            {
                itemState |= NativeMethods.LVIS_SELECTED;
                stateMask |= NativeMethods.LVIS_SELECTED;
            }

            if (SavedStateImageIndex > -1)
            {
                itemState |= ((SavedStateImageIndex + 1) << 12);
                stateMask |= NativeMethods.LVIS_STATEIMAGEMASK;
            }

            lvItem.mask |= NativeMethods.LVIF_STATE;
            lvItem.iItem = index;
            lvItem.stateMask |= stateMask;
            lvItem.state |= itemState;

            if (listView.GroupsEnabled)
            {
                lvItem.mask |= NativeMethods.LVIF_GROUPID;
                lvItem.iGroupId = listView.GetNativeGroupId(this);

                Debug.Assert(!updateOwner || listView.SendMessage(NativeMethods.LVM_ISGROUPVIEWENABLED, 0, 0) != IntPtr.Zero, "Groups not enabled");
                Debug.Assert(!updateOwner || listView.SendMessage(NativeMethods.LVM_HASGROUP, lvItem.iGroupId, 0) != IntPtr.Zero, "Doesn't contain group id: " + lvItem.iGroupId.ToString(CultureInfo.InvariantCulture));
            }

            if (updateOwner)
            {
                UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), NativeMethods.LVM_SETITEM, 0, ref lvItem);
            }
        }

        internal void UpdateStateFromListView(int displayIndex, bool checkSelection)
        {
            if (listView?.IsHandleCreated != true || displayIndex == -1)
            {
                return;
            }

            // Get information from comctl control
            var lvItem = new NativeMethods.LVITEM
            {
                mask = NativeMethods.LVIF_PARAM | NativeMethods.LVIF_STATE | NativeMethods.LVIF_GROUPID
            };

            if (checkSelection)
            {
                lvItem.stateMask = NativeMethods.LVIS_SELECTED;
            }

            // we want to get all the information, including the state image mask
            lvItem.stateMask |= NativeMethods.LVIS_STATEIMAGEMASK;

            if (lvItem.stateMask == 0)
            {
                // perf optimization: no work to do.
                return;
            }

            lvItem.iItem = displayIndex;
            UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), NativeMethods.LVM_GETITEM, 0, ref lvItem);

            // Update this class' information
            if (checkSelection)
            {
                StateSelected = (lvItem.state & NativeMethods.LVIS_SELECTED) != 0;
            }
            SavedStateImageIndex = ((lvItem.state & NativeMethods.LVIS_STATEIMAGEMASK) >> 12) - 1;

            group = null;
            foreach (ListViewGroup lvg in ListView.Groups)
            {
                if (lvg.ID == lvItem.iGroupId)
                {
                    group = lvg;
                    break;
                }
            }
            
        }

        internal void UnHost(bool checkSelection) => UnHost(Index, checkSelection);

        internal void UnHost(int displayIndex, bool checkSelection)
        {
            UpdateStateFromListView(displayIndex, checkSelection);

            if (listView != null && (listView.Site == null || !listView.Site.DesignMode) && group != null)
            {
                group.Items.Remove(this);
            }

            // Make sure you do these last, as the first several lines depends on this information
            ID = -1;
            listView = null;
        }

        public virtual void Remove() => listView?.Items.Remove(this);

        protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
        {
            bool foundSubItems = false;

            string imageKey = null;
            int imageIndex = -1;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case nameof(Text):
                        Text = info.GetString(entry.Name);
                        break;

                    case nameof(ImageIndex):
                        imageIndex = info.GetInt32(entry.Name);
                        break;

                    case nameof(ImageKey):
                        imageKey = info.GetString(entry.Name);
                        break;

                    case nameof(BackColor):
                        BackColor = (Color)info.GetValue(entry.Name, typeof(Color));
                        break;

                    case nameof(Checked):
                        Checked = info.GetBoolean(entry.Name);
                        break;

                    case nameof(Font):
                        Font = (Font)info.GetValue(entry.Name, typeof(Font));
                        break;

                    case nameof(ForeColor):
                        ForeColor = (Color)info.GetValue(entry.Name, typeof(Color));
                        break;

                    case nameof(UseItemStyleForSubItems):
                        UseItemStyleForSubItems = info.GetBoolean(entry.Name);
                        break;

                    case nameof(Group):
                        ListViewGroup group = (ListViewGroup)info.GetValue(entry.Name, typeof(ListViewGroup));
                        _groupName = group.Name;
                        break;

                    case nameof(SubItemCount):
                        SubItemCount = info.GetInt32(entry.Name);
                        if (SubItemCount > 0)
                        {
                            foundSubItems = true;
                        }
                        break;
                }
            }

            // let image key take precidence
            if (imageKey != null)
            {
                ImageKey = imageKey;
            }
            else if (imageIndex != -1)
            {
                ImageIndex = imageIndex;
            }

            if (foundSubItems)
            {
                ListViewSubItem[] newItems = new ListViewSubItem[SubItemCount];
                for (int i = 1; i < SubItemCount; i++)
                {
                    var newItem = info.GetValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), typeof(ListViewSubItem)) as ListViewSubItem;
                    newItem.owner = this;
                    newItems[i] = newItem;
                }
                newItems[0] = _listViewSubItems[0];
                _listViewSubItems = newItems;
            }
        }

        /// <summary>
        /// Saves this ListViewItem object to the given data stream.
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
                    info.AddValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), _listViewSubItems[i], typeof(ListViewSubItem));
                }
            }
            info.AddValue(nameof(BackColor), BackColor);
            info.AddValue(nameof(Checked), Checked);
            info.AddValue(nameof(Font), Font);
            info.AddValue(nameof(ForeColor), ForeColor);
            info.AddValue(nameof(UseItemStyleForSubItems), UseItemStyleForSubItems);
            if (Group != null)
            {
                info.AddValue(nameof(Group), Group);
            }
        }

        // we need this function to set the index when the list view is in virtual mode.
        // the index of the list view item is used in ListView::set_TopItem property
        internal void SetItemIndex(ListView listView, int index)
        {
            Debug.Assert(listView != null && listView.VirtualMode, "ListViewItem::SetItemIndex should be used only when the list is virtual");
            Debug.Assert(index > -1, "can't set the index on a virtual list view item to -1");
            this.listView = listView;
            _lastIndex = index;
        }

        internal bool ShouldSerializeText() => false;

        private bool ShouldSerializePosition() => !_position.Equals(DefaultPosition);

        public override string ToString() => "ListViewItem: {" + Text + "}";

        internal void InvalidateListView()
        {
            // The ListItem's state (or a SubItem's state) has changed, so invalidate the ListView control
            if (listView?.IsHandleCreated == true)
            {
                listView.Invalidate();
            }
        }

        internal void UpdateSubItems(int index) => UpdateSubItems(index, SubItemCount);

        internal void UpdateSubItems(int index, int oldCount)
        {
            if (listView?.IsHandleCreated == true)
            {
                int subItemCount = SubItemCount;
                int itemIndex = Index;
                if (index != -1)
                {
                    listView.SetItemText(itemIndex, index, _listViewSubItems[index].Text);
                }
                else
                {
                    for (int i = 0; i < subItemCount; i++)
                    {
                        listView.SetItemText(itemIndex, i, _listViewSubItems[i].Text);
                    }
                }

                for (int i = subItemCount; i < oldCount; i++)
                {
                    listView.SetItemText(itemIndex, i, string.Empty);
                }
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serialize(info, context);
        }
    }
}
