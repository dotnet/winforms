// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.Serialization;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements an item of a <see cref='Forms.ListView'/>.
    /// </summary>
    [TypeConverter(typeof(ListViewItemConverter))]
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultProperty(nameof(Text))]
    [Serializable] // This type is participating in resx serialization scenarios.
    public partial class ListViewItem : ICloneable, ISerializable
    {
        private const int MaxSubItems = 4096;

        private static readonly BitVector32.Section s_stateSelectedSection = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section s_stateImageMaskSet = BitVector32.CreateSection(1, s_stateSelectedSection);
        private static readonly BitVector32.Section s_stateWholeRowOneStyleSection = BitVector32.CreateSection(1, s_stateImageMaskSet);
        private static readonly BitVector32.Section s_avedStateImageIndexSection = BitVector32.CreateSection(15, s_stateWholeRowOneStyleSection);
        private static readonly BitVector32.Section s_subItemCountSection = BitVector32.CreateSection(MaxSubItems, s_avedStateImageIndexSection);

        private int indentCount;
        private Point position = new Point(-1, -1);

        internal ListView listView;

        internal ListViewGroup group;
        private string groupName;

        private ListViewSubItemCollection listViewSubItemCollection;
        private ListViewSubItem[] subItems;

        // we stash the last index we got as a seed to GetDisplayIndex.
        private int lastIndex = -1;

        // An ID unique relative to a given list view that comctl uses to identify items.
        internal int ID = -1;

        private BitVector32 state;
        private ListViewItemImageIndexer imageIndexer;
        private string toolTipText = string.Empty;
        private object userData;

        private AccessibleObject _accessibilityObject;

        public ListViewItem()
        {
            StateSelected = false;
            UseItemStyleForSubItems = true;
            SavedStateImageIndex = -1;
        }

        /// <summary>
        ///  Creates a ListViewItem object from an Stream.
        /// </summary>
        protected ListViewItem(SerializationInfo info, StreamingContext context) : this()
        {
            Deserialize(info, context);
        }

        public ListViewItem(string text) : this(text, ImageList.Indexer.DefaultIndex)
        {
        }

        public ListViewItem(string text, int imageIndex) : this()
        {
            ImageIndexer.Index = imageIndex;
            Text = text;
        }

        public ListViewItem(string[] items) : this(items, ImageList.Indexer.DefaultIndex)
        {
        }

        public ListViewItem(string[] items, int imageIndex) : this()
        {
            ImageIndexer.Index = imageIndex;
            if (items != null && items.Length > 0)
            {
                subItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    subItems[i] = new ListViewSubItem(this, items[i]);
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
            ImageIndexer.Index = imageIndex;
            this.subItems = subItems ?? throw new ArgumentNullException(nameof(subItems));
            SubItemCount = subItems.Length;

            // Update the owner of these subitems
            for (int i = 0; i < subItems.Length; i++)
            {
                if (subItems[i] is null)
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
                subItems = new ListViewSubItem[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    subItems[i] = new ListViewSubItem(this, items[i]);
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
            this.subItems = subItems ?? throw new ArgumentNullException(nameof(subItems));
            SubItemCount = subItems.Length;

            // Update the owner of these subitems
            for (int i = 0; i < subItems.Length; i++)
            {
                if (subItems[i] is null)
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

        internal virtual AccessibleObject AccessibilityObject
        {
            get
            {
                if (_accessibilityObject is null)
                {
                    bool inDefaultGroup = listView?.GroupsEnabled == true && Group is null;

                    _accessibilityObject = new ListViewItemAccessibleObject(
                        this, inDefaultGroup ? listView.DefaultGroup : Group);
                }

                return _accessibilityObject;
            }
        }

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
                    if (listView != null)
                    {
                        return listView.BackColor;
                    }

                    return SystemColors.Window;
                }
                else
                {
                    return subItems[0].BackColor;
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
                if (listView != null)
                {
                    return listView.GetItemRect(Index);
                }
                else
                {
                    return new Rectangle();
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
                    if (listView != null && listView.IsHandleCreated)
                    {
                        StateImageIndex = value ? 1 : 0;

                        // the setter for StateImageIndex calls ItemChecked handler
                        // thus need to verify validity of the listView again
                        if (listView != null && !listView.UseCompatibleStateImageBehavior)
                        {
                            if (!listView.CheckBoxes)
                            {
                                listView.UpdateSavedCheckedItems(this, value);
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
                if (listView != null && listView.IsHandleCreated)
                {
                    return listView.GetItemState(Index, LVIS.FOCUSED) != 0;
                }

                return false;
            }

            set
            {
                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemState(Index, value ? LVIS.FOCUSED : 0, LVIS.FOCUSED);

                    AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }
        }

        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
        public Font Font
        {
            get
            {
                if (SubItemCount == 0)
                {
                    if (listView != null)
                    {
                        return listView.Font;
                    }

                    return Control.DefaultFont;
                }
                else
                {
                    return subItems[0].Font;
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
                    if (listView != null)
                    {
                        return listView.ForeColor;
                    }

                    return SystemColors.WindowText;
                }
                else
                {
                    return subItems[0].ForeColor;
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
                groupName = null;
            }
        }

        /// <summary>
        ///  Returns the ListViewItem's currently set image index
        /// </summary>
        [DefaultValue(ImageList.Indexer.DefaultIndex)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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
                if (value < ImageList.Indexer.DefaultIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, ImageList.Indexer.DefaultIndex));
                }

                ImageIndexer.Index = value;

                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemImage(itemIndex: Index, imageIndex: ImageIndexer.ActualIndex);
                }
            }
        }

        internal ListViewItemImageIndexer ImageIndexer => imageIndexer ??= new ListViewItemImageIndexer(this);

        /// <summary>
        ///  Returns the ListViewItem's currently set image index
        /// </summary>
        [DefaultValue(ImageList.Indexer.DefaultKey)]
        [TypeConverter(typeof(ImageKeyConverter))]
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

                if (listView != null && listView.IsHandleCreated)
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
                if (listView != null)
                {
                    switch (listView.View)
                    {
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

        [DefaultValue(0)]
        [SRDescription(nameof(SR.ListViewItemIndentCountDescr))]
        [SRCategory(nameof(SR.CatDisplay))]
        public int IndentCount
        {
            get => indentCount;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(IndentCount), SR.ListViewIndentCountCantBeNegative);
                }

                if (value != indentCount)
                {
                    indentCount = value;
                    if (listView != null && listView.IsHandleCreated)
                    {
                        listView.SetItemIndentCount(Index, indentCount);
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
                if (listView != null)
                {
                    // if the list is virtual, the ComCtrl control does not keep any information
                    // about any list view items, so we use our cache instead.
                    if (!listView.VirtualMode)
                    {
                        lastIndex = listView.GetDisplayIndex(this, lastIndex);
                    }
                    return lastIndex;
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
        public ListView ListView => listView;

        /// <summary>
        ///  Name associated with this ListViewItem
        /// </summary>
        [Localizable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                    return subItems[0].Name;
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
                if (listView != null && listView.IsHandleCreated)
                {
                    position = listView.GetItemPosition(Index);
                }

                return position;
            }
            set
            {
                if (!value.Equals(position))
                {
                    position = value;
                    if (listView != null && listView.IsHandleCreated)
                    {
                        if (!listView.VirtualMode)
                        {
                            listView.SetItemPosition(Index, position.X, position.Y);
                        }
                    }
                }
            }
        }

        internal LVIS RawStateImageIndex => (LVIS)((SavedStateImageIndex + 1) << 12);

        /// <summary>
        ///  Accessor for our state bit vector.
        /// </summary>
        private int SavedStateImageIndex
        {
            get
            {
                // State goes from zero to 15, but we need a negative
                // number, so we store + 1.
                return state[s_avedStateImageIndexSection] - 1;
            }
            set
            {
                // flag whether we've set a value.
                state[s_stateImageMaskSet] = (value == ImageList.Indexer.DefaultIndex ? 0 : 1);

                // push in the actual value
                state[s_avedStateImageIndexSection] = value + 1;
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
                if (listView != null && listView.IsHandleCreated)
                {
                    return listView.GetItemState(Index, LVIS.SELECTED) != 0;
                }

                return StateSelected;
            }
            set
            {
                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemState(Index, value ? LVIS.SELECTED : 0, LVIS.SELECTED);

                    // update comctl32's selection information.
                    listView.SetSelectionMark(Index);
                }
                else
                {
                    StateSelected = value;
                    if (listView != null && listView.IsHandleCreated)
                    {
                        // Set the selected state on the list view item only if the list view's Handle is already created.
                        listView.CacheSelectedStateForItem(this, value);
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RelatedImageList("ListView.StateImageList")]
        public int StateImageIndex
        {
            get
            {
                if (listView != null && listView.IsHandleCreated)
                {
                    LVIS state = listView.GetItemState(Index, LVIS.STATEIMAGEMASK);
                    return (((int)state >> 12) - 1);   // index is 1-based
                }

                return SavedStateImageIndex;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex || value > 14)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(StateImageIndex), value));
                }

                if (listView != null && listView.IsHandleCreated)
                {
                    this.state[s_stateImageMaskSet] = (value == ImageList.Indexer.DefaultIndex ? 0 : 1);
                    LVIS state = (LVIS)((value + 1) << 12);  // index is 1-based
                    listView.SetItemState(Index, state, LVIS.STATEIMAGEMASK);
                }
                SavedStateImageIndex = value;
            }
        }

        internal bool StateImageSet => (state[s_stateImageMaskSet] != 0);

        /// <summary>
        ///  Accessor for our state bit vector.
        /// </summary>
        internal bool StateSelected
        {
            get => state[s_stateSelectedSection] == 1;
            set => state[s_stateSelectedSection] = value ? 1 : 0;
        }

        /// <summary>
        ///  Accessor for our state bit vector.
        /// </summary>
        private int SubItemCount // Do NOT rename (binary serialization).
        {
            get => state[s_subItemCountSection];
            set => state[s_subItemCountSection] = value;
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
                    subItems = new ListViewSubItem[1];
                    subItems[0] = new ListViewSubItem(this, string.Empty);
                    SubItemCount = 1;
                }

                return listViewSubItemCollection ?? (listViewSubItemCollection = new ListViewSubItemCollection(this));
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
            get => userData;
            set => userData = value;
        }

        /// <summary>
        ///  Text associated with this ListViewItem
        /// </summary>
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatAppearance))]
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
                    return subItems[0].Text;
                }
            }
            set => SubItems[0].Text = value;
        }

        /// <summary>
        ///  Tool tip text associated with this ListViewItem
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue("")]
        public string ToolTipText
        {
            get => toolTipText;
            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                if (!WindowsFormsUtils.SafeCompareStrings(toolTipText, value, ignoreCase: false))
                {
                    toolTipText = value;

                    // tell the list view about this change
                    if (listView != null && listView.IsHandleCreated)
                    {
                        listView.ListViewItemToolTipChanged(this);
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
            get => state[s_stateWholeRowOneStyleSection] == 1;
            set => state[s_stateWholeRowOneStyleSection] = value ? 1 : 0;
        }

        /// <summary>
        ///  Initiate editing of the item's label. Only effective if LabelEdit property is true.
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

                User32.SendMessageW(lv, (User32.WM)LVM.EDITLABELW, (IntPtr)Index);
            }
        }

        public virtual object Clone()
        {
            ListViewSubItem[] clonedSubItems = new ListViewSubItem[SubItems.Count];
            for (int index = 0; index < SubItems.Count; ++index)
            {
                ListViewSubItem subItem = SubItems[index];
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
            ListViewItem newItem = null;

            if (clonedType == typeof(ListViewItem))
            {
                newItem = new ListViewItem(clonedSubItems, ImageIndexer.Index);
            }
            else
            {
                newItem = (ListViewItem)Activator.CreateInstance(clonedType);
            }
            newItem.subItems = clonedSubItems;
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

            newItem.indentCount = indentCount;
            newItem.StateImageIndex = StateImageIndex;
            newItem.toolTipText = toolTipText;
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
            if (listView != null && listView.IsHandleCreated)
            {
                listView.EnsureVisible(Index);
            }
        }

        public ListViewItem FindNearestItem(SearchDirectionHint searchDirection)
        {
            Rectangle r = Bounds;
            switch (searchDirection)
            {
                case SearchDirectionHint.Up:
                    return ListView.FindNearestItem(searchDirection, r.Left, r.Top);
                case SearchDirectionHint.Down:
                    return ListView.FindNearestItem(searchDirection, r.Left, r.Bottom);
                case SearchDirectionHint.Left:
                    return ListView.FindNearestItem(searchDirection, r.Left, r.Top);
                case SearchDirectionHint.Right:
                    return ListView.FindNearestItem(searchDirection, r.Right, r.Top);
                default:
                    Debug.Fail("we handled all the 4 directions");
                    return null;
            }
        }

        /// <summary>
        ///  Returns a specific portion of the ListViewItem's bounding rectangle.
        ///  The rectangle returned is empty if the ListViewItem has not been added to a ListView control.
        /// </summary>
        public Rectangle GetBounds(ItemBoundsPortion portion)
        {
            if (listView != null && listView.IsHandleCreated)
            {
                return listView.GetItemRect(Index, portion);
            }

            return new Rectangle();
        }

        public ListViewSubItem GetSubItemAt(int x, int y)
        {
            if (listView != null && listView.IsHandleCreated && listView.View == View.Details)
            {
                listView.GetSubItemAt(x, y, out int iItem, out int iSubItem);
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
            Debug.Assert(listView is null || !listView.VirtualMode, "ListViewItem::Host can't be used w/ a virtual item");
            Debug.Assert(parent is null || !parent.VirtualMode, "ListViewItem::Host can't be used w/ a virtual list");

            ID = id;
            listView = parent;

            // If the index is valid, then the handle has been created.
            if (index != -1)
            {
                UpdateStateToListView(index);
            }
        }

        /// <summary>
        ///  This is used to map list view items w/ their respective groups in localized forms.
        /// </summary>
        internal void UpdateGroupFromName()
        {
            Debug.Assert(listView != null, "This method is used only when items are parented in a list view");
            Debug.Assert(!listView.VirtualMode, "we need to update the group only when the user specifies the list view items in localizable forms");
            if (string.IsNullOrEmpty(groupName))
            {
                return;
            }

            ListViewGroup group = listView.Groups[groupName];
            Group = group;

            // Use the group name only once.
            groupName = null;
        }

        internal void UpdateStateToListView(int index)
        {
            var lvItem = new LVITEMW();
            UpdateStateToListView(index, ref lvItem, true);
        }

        /// <summary>
        ///  Called when we have just pushed this item into a list view and we need
        ///  to configure the list view's state for the item. Use a valid index
        ///  if you can, or use -1 if you can't.
        /// </summary>
        internal void UpdateStateToListView(int index, ref LVITEMW lvItem, bool updateOwner)
        {
            Debug.Assert(listView.IsHandleCreated, "Should only invoke UpdateStateToListView when handle is created.");

            if (index == -1)
            {
                index = Index;
            }
            else
            {
                lastIndex = index;
            }

            // Update Item state in one shot
            LVIS itemState = 0;
            LVIS stateMask = 0;
            if (StateSelected)
            {
                itemState |= LVIS.SELECTED;
                stateMask |= LVIS.SELECTED;
            }

            if (SavedStateImageIndex > ImageList.Indexer.DefaultIndex)
            {
                itemState |= (LVIS)((SavedStateImageIndex + 1) << 12);
                stateMask |= LVIS.STATEIMAGEMASK;
            }

            lvItem.mask |= LVIF.STATE;
            lvItem.iItem = index;
            lvItem.stateMask |= stateMask;
            lvItem.state |= itemState;

            if (listView.GroupsEnabled)
            {
                lvItem.mask |= LVIF.GROUPID;
                lvItem.iGroupId = listView.GetNativeGroupId(this);

                IntPtr result = User32.SendMessageW(listView, (User32.WM)LVM.ISGROUPVIEWENABLED);
                Debug.Assert(!updateOwner || result != IntPtr.Zero, "Groups not enabled");
                result = User32.SendMessageW(listView, (User32.WM)LVM.HASGROUP, (IntPtr)lvItem.iGroupId);
                Debug.Assert(!updateOwner || result != IntPtr.Zero, "Doesn't contain group id: " + lvItem.iGroupId.ToString(CultureInfo.InvariantCulture));
            }

            if (updateOwner)
            {
                User32.SendMessageW(listView, (User32.WM)LVM.SETITEMW, IntPtr.Zero, ref lvItem);
            }
        }

        internal void UpdateStateFromListView(int displayIndex, bool checkSelection)
        {
            if (listView != null && listView.IsHandleCreated && displayIndex != -1)
            {
                // Get information from comctl control
                var lvItem = new LVITEMW
                {
                    mask = LVIF.PARAM | LVIF.STATE | LVIF.GROUPID
                };

                if (checkSelection)
                {
                    lvItem.stateMask = LVIS.SELECTED;
                }

                // we want to get all the information, including the state image mask
                lvItem.stateMask |= LVIS.STATEIMAGEMASK;

                if (lvItem.stateMask == 0)
                {
                    // perf optimization: no work to do.
                    return;
                }

                lvItem.iItem = displayIndex;
                User32.SendMessageW(listView, (User32.WM)LVM.GETITEMW, IntPtr.Zero, ref lvItem);

                // Update this class' information
                if (checkSelection)
                {
                    StateSelected = (lvItem.state & LVIS.SELECTED) != 0;
                }
                SavedStateImageIndex = ((int)(lvItem.state & LVIS.STATEIMAGEMASK) >> 12) - 1;

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
        }

        internal void UnHost(bool checkSelection) => UnHost(Index, checkSelection);

        internal void UnHost(int displayIndex, bool checkSelection)
        {
            UpdateStateFromListView(displayIndex, checkSelection);

            if (listView != null && (listView.Site is null || !listView.Site.DesignMode) && group != null)
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
                    BackColor = (Color)info.GetValue(nameof(BackColor), typeof(Color));
                }
                else if (entry.Name == nameof(Checked))
                {
                    Checked = info.GetBoolean(nameof(Checked));
                }
                else if (entry.Name == nameof(Font))
                {
                    Font = (Font)info.GetValue(nameof(Font), typeof(Font));
                }
                else if (entry.Name == nameof(ForeColor))
                {
                    ForeColor = (Color)info.GetValue(nameof(ForeColor), typeof(Color));
                }
                else if (entry.Name == nameof(UseItemStyleForSubItems))
                {
                    UseItemStyleForSubItems = info.GetBoolean(nameof(UseItemStyleForSubItems));
                }
                else if (entry.Name == nameof(Group))
                {
                    ListViewGroup group = (ListViewGroup)info.GetValue(nameof(Group), typeof(ListViewGroup));
                    groupName = group.Name;
                }
            }

            // let image key take precidence
            if (imageKey != null)
            {
                ImageKey = imageKey;
            }
            else if (imageIndex != ImageList.Indexer.DefaultIndex)
            {
                ImageIndex = imageIndex;
            }

            if (foundSubItems)
            {
                ListViewSubItem[] newItems = new ListViewSubItem[SubItemCount];
                for (int i = 1; i < SubItemCount; i++)
                {
                    ListViewSubItem newItem = (ListViewSubItem)info.GetValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), typeof(ListViewSubItem));
                    newItem.owner = this;
                    newItems[i] = newItem;
                }
                newItems[0] = subItems[0];
                subItems = newItems;
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
                    info.AddValue("SubItem" + i.ToString(CultureInfo.InvariantCulture), subItems[i], typeof(ListViewSubItem));
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
            lastIndex = index;
        }

        internal bool ShouldSerializeText() => false;

        private bool ShouldSerializePosition() => !position.Equals(new Point(-1, -1));

        public override string ToString() => "ListViewItem: {" + Text + "}";

        internal void InvalidateListView()
        {
            // The ListItem's state (or a SubItem's state) has changed, so invalidate the ListView control
            if (listView != null && listView.IsHandleCreated)
            {
                listView.Invalidate();
            }
        }

        internal void UpdateSubItems(int index) => UpdateSubItems(index, SubItemCount);

        internal void UpdateSubItems(int index, int oldCount)
        {
            if (listView != null && listView.IsHandleCreated)
            {
                int subItemCount = SubItemCount;
                int itemIndex = Index;
                if (index != -1)
                {
                    listView.SetItemText(itemIndex, index, subItems[index].Text);
                }
                else
                {
                    for (int i = 0; i < subItemCount; i++)
                    {
                        listView.SetItemText(itemIndex, i, subItems[i].Text);
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
