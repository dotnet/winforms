// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

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
    public class ListViewItem : ICloneable, ISerializable
    {
        private const int MaxSubItems = 4096;

        private static readonly BitVector32.Section s_stateSelectedSection = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section s_stateImageMaskSet = BitVector32.CreateSection(1, s_stateSelectedSection);
        private static readonly BitVector32.Section s_stateWholeRowOneStyleSection = BitVector32.CreateSection(1, s_stateImageMaskSet);
        private static readonly BitVector32.Section s_avedStateImageIndexSection = BitVector32.CreateSection(15, s_stateWholeRowOneStyleSection);
        private static readonly BitVector32.Section s_subItemCountSection = BitVector32.CreateSection(MaxSubItems, s_avedStateImageIndexSection);

        private int indentCount = 0;
        private Point position = new Point(-1, -1);

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
        private string toolTipText = string.Empty;
        private object userData;

        // We need a special way to defer to the ListView's image
        // list for indexing purposes.
        internal class ListViewItemImageIndexer : ImageList.Indexer
        {
            private readonly ListViewItem _owner;

            public ListViewItemImageIndexer(ListViewItem item)
            {
                _owner = item;
            }

            public override ImageList ImageList
            {
                get => _owner?.ImageList;
                set => Debug.Fail("We should never set the image list");
            }
        }

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
                    return listView.GetItemState(Index, NativeMethods.LVIS_FOCUSED) != 0;
                }

                return false;
            }

            set
            {
                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemState(Index, value ? NativeMethods.LVIS_FOCUSED : 0, NativeMethods.LVIS_FOCUSED);
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
        [DefaultValue(-1)]
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
                if (ImageIndexer.Index != -1 && ImageList != null && ImageIndexer.Index >= ImageList.Images.Count)
                {
                    return ImageList.Images.Count - 1;
                }

                return ImageIndexer.Index;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }

                ImageIndexer.Index = value;

                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemImage(Index, ImageIndexer.ActualIndex);
                }
            }
        }

        internal ListViewItemImageIndexer ImageIndexer => imageIndexer ?? (imageIndexer = new ListViewItemImageIndexer(this));

        /// <summary>
        ///  Returns the ListViewItem's currently set image index
        /// </summary>
        [DefaultValue("")]
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

        internal int RawStateImageIndex => (SavedStateImageIndex + 1) << 12;

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
                state[s_stateImageMaskSet] = (value == -1 ? 0 : 1);

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
                    return listView.GetItemState(Index, NativeMethods.LVIS_SELECTED) != 0;
                }

                return StateSelected;
            }
            set
            {
                if (listView != null && listView.IsHandleCreated)
                {
                    listView.SetItemState(Index, value ? NativeMethods.LVIS_SELECTED : 0, NativeMethods.LVIS_SELECTED);

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
        [DefaultValue(-1)]
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
                    int state = listView.GetItemState(Index, NativeMethods.LVIS_STATEIMAGEMASK);
                    return ((state >> 12) - 1);   // index is 1-based
                }

                return SavedStateImageIndex;
            }
            set
            {
                if (value < -1 || value > 14)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(StateImageIndex), value));
                }

                if (listView != null && listView.IsHandleCreated)
                {
                    this.state[s_stateImageMaskSet] = (value == -1 ? 0 : 1);
                    int state = ((value + 1) << 12);  // index is 1-based
                    listView.SetItemState(Index, state, NativeMethods.LVIS_STATEIMAGEMASK);
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
                if (value == null)
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

                UnsafeNativeMethods.SendMessage(new HandleRef(lv, lv.Handle), NativeMethods.LVM_EDITLABEL, Index, 0);
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
            var lvItem = new NativeMethods.LVITEM();
            UpdateStateToListView(index, ref lvItem, true);
        }

        /// <summary>
        ///  Called when we have just pushed this item into a list view and we need
        ///  to configure the list view's state for the item. Use a valid index
        ///  if you can, or use -1 if you can't.
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
                lastIndex = index;
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
            if (listView != null && listView.IsHandleCreated && displayIndex != -1)
            {
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
            else if (imageIndex != -1)
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

        [TypeConverter(typeof(ListViewSubItemConverter))]
        [ToolboxItem(false)]
        [DesignTimeVisible(false)]
        [DefaultProperty(nameof(Text))]
        [Serializable] // This type is participating in resx serialization scenarios.
        public class ListViewSubItem
        {
            [NonSerialized]
            internal ListViewItem owner;
#pragma warning disable IDE1006
            private string text;  // Do NOT rename (binary serialization).

            [OptionalField(VersionAdded = 2)]
            private string name = null;  // Do NOT rename (binary serialization).

            private SubItemStyle style;  // Do NOT rename (binary serialization).

            [OptionalField(VersionAdded = 2)]
            private object userData;  // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

            public ListViewSubItem()
            {
            }

            public ListViewSubItem(ListViewItem owner, string text)
            {
                this.owner = owner;
                this.text = text;
            }

            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
            {
                this.owner = owner;
                this.text = text;
                style = new SubItemStyle
                {
                    foreColor = foreColor,
                    backColor = backColor,
                    font = font
                };
            }

            public Color BackColor
            {
                get
                {
                    if (style != null && style.backColor != Color.Empty)
                    {
                        return style.backColor;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.BackColor;
                    }

                    return SystemColors.Window;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.backColor != value)
                    {
                        style.backColor = value;
                        owner?.InvalidateListView();
                    }
                }
            }
            [Browsable(false)]
            public Rectangle Bounds
            {
                get
                {
                    if (owner != null && owner.listView != null && owner.listView.IsHandleCreated)
                    {
                        return owner.listView.GetSubItemRect(owner.Index, owner.SubItems.IndexOf(this));
                    }
                    else
                    {
                        return Rectangle.Empty;
                    }
                }
            }

            internal bool CustomBackColor
            {
                get
                {
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return !style.backColor.IsEmpty;
                }
            }

            internal bool CustomFont
            {
                get
                {
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return style.font != null;
                }
            }

            internal bool CustomForeColor
            {
                get
                {
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return !style.foreColor.IsEmpty;
                }
            }

            internal bool CustomStyle => style != null;

            [Localizable(true)]
            public Font Font
            {
                get
                {
                    if (style != null && style.font != null)
                    {
                        return style.font;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.Font;
                    }

                    return Control.DefaultFont;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.font != value)
                    {
                        style.font = value;
                        owner?.InvalidateListView();
                    }
                }
            }

            public Color ForeColor
            {
                get
                {
                    if (style != null && style.foreColor != Color.Empty)
                    {
                        return style.foreColor;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.ForeColor;
                    }

                    return SystemColors.WindowText;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.foreColor != value)
                    {
                        style.foreColor = value;
                        owner?.InvalidateListView();
                    }
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

            [Localizable(true)]
            public string Text
            {
                get => text ?? string.Empty;
                set
                {
                    text = value;
                    owner?.UpdateSubItems(-1);
                }
            }

            [Localizable(true)]
            public string Name
            {
                get => name ?? string.Empty;
                set
                {
                    name = value;
                    owner?.UpdateSubItems(-1);
                }
            }

            [OnDeserializing]
            private void OnDeserializing(StreamingContext ctx)
            {
            }

            [OnDeserialized]
            private void OnDeserialized(StreamingContext ctx)
            {
                name = null;
                userData = null;
            }

            [OnSerializing]
            private void OnSerializing(StreamingContext ctx)
            {
            }

            [OnSerialized]
            private void OnSerialized(StreamingContext ctx)
            {
            }

            public void ResetStyle()
            {
                if (style != null)
                {
                    style = null;
                    owner?.InvalidateListView();
                }
            }

            public override string ToString() => "ListViewSubItem: {" + Text + "}";

            [Serializable] // This type is participating in resx serialization scenarios.
            private class SubItemStyle
            {
#pragma warning disable IDE1006
                public Color backColor = Color.Empty; // Do NOT rename (binary serialization).
                public Color foreColor = Color.Empty; // Do NOT rename (binary serialization).
                public Font font = null; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006
            }
        }

        public class ListViewSubItemCollection : IList
        {
            private readonly ListViewItem _owner;

            // A caching mechanism for key accessor
            // We use an index here rather than control so that we don't have lifetime
            // issues by holding on to extra references.
            private int _lastAccessedIndex = -1;

            public ListViewSubItemCollection(ListViewItem owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Returns the total number of items within the list view.
            /// </summary>
            [Browsable(false)]
            public int Count => _owner.SubItemCount;

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => true;

            bool IList.IsFixedSize => false;

            public bool IsReadOnly => false;

            /// <summary>
            ///  Returns a ListViewSubItem given it's zero based index into the ListViewSubItemCollection.
            /// </summary>
            public ListViewSubItem this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return _owner.subItems[index];
                }
                set
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    _owner.subItems[index] = value ?? throw new ArgumentNullException(nameof(value));
                    _owner.UpdateSubItems(index);
                }
            }

            object IList.this[int index]
            {
                get => this[index];
                set
                {
                    if (!(value is ListViewSubItem item))
                    {
                        throw new ArgumentException(SR.ListViewBadListViewSubItem, nameof(value));
                    }

                    this[index] = item;
                }
            }
            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewSubItem this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (!IsValidIndex(index))
                    {
                        return null;
                    }

                    return this[index];
                }
            }

            public ListViewSubItem Add(ListViewSubItem item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                EnsureSubItemSpace(1, -1);
                item.owner = _owner;
                _owner.subItems[_owner.SubItemCount] = item;
                _owner.UpdateSubItems(_owner.SubItemCount++);
                return item;
            }

            public ListViewSubItem Add(string text)
            {
                ListViewSubItem item = new ListViewSubItem(_owner, text);
                Add(item);
                return item;
            }

            public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
            {
                ListViewSubItem item = new ListViewSubItem(_owner, text, foreColor, backColor, font);
                Add(item);
                return item;
            }

            public void AddRange(ListViewSubItem[] items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (ListViewSubItem item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = item;
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            public void AddRange(string[] items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (string item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = new ListViewSubItem(_owner, item);
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (string item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = new ListViewSubItem(_owner, item, foreColor, backColor, font);
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            int IList.Add(object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    throw new ArgumentException(SR.ListViewSubItemCollectionInvalidArgument, nameof(item));
                }

                return IndexOf(Add(itemValue));
            }

            public void Clear()
            {
                int oldCount = _owner.SubItemCount;
                if (oldCount > 0)
                {
                    _owner.SubItemCount = 0;
                    _owner.UpdateSubItems(-1, oldCount);
                }
            }

            public bool Contains(ListViewSubItem subItem) => IndexOf(subItem) != -1;

            bool IList.Contains(object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    return false;
                }

                return Contains(itemValue);
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key) => IsValidIndex(IndexOfKey(key));

            /// <summary>
            ///  Ensures that the sub item array has the given
            ///  capacity. If it doesn't, it enlarges the
            ///  array until it does. If index is -1, additional
            ///  space is tacked onto the end. If it is a valid
            ///  insertion index into the array, this will move
            ///  the array data to accomodate the space.
            /// </summary>
            private void EnsureSubItemSpace(int size, int index)
            {
                if (_owner.SubItemCount == ListViewItem.MaxSubItems)
                {
                    throw new InvalidOperationException(SR.ErrorCollectionFull);
                }

                if (_owner.subItems == null || _owner.SubItemCount + size > _owner.subItems.Length)
                {
                    // Must grow array. Don't do it just by size, though;
                    // chunk it for efficiency.
                    if (_owner.subItems == null)
                    {
                        int newSize = (size > 4) ? size : 4;
                        _owner.subItems = new ListViewSubItem[newSize];
                    }
                    else
                    {
                        int newSize = _owner.subItems.Length * 2;
                        while (newSize - _owner.SubItemCount < size)
                        {
                            newSize *= 2;
                        }

                        ListViewSubItem[] newItems = new ListViewSubItem[newSize];

                        // Now, when copying to the member variable, use index
                        // if it was provided.
                        if (index != -1)
                        {
                            Array.Copy(_owner.subItems, 0, newItems, 0, index);
                            Array.Copy(_owner.subItems, index, newItems, index + size, _owner.SubItemCount - index);
                        }
                        else
                        {
                            Array.Copy(_owner.subItems, newItems, _owner.SubItemCount);
                        }
                        _owner.subItems = newItems;
                    }
                }
                else
                {
                    // We had plenty of room. Just move the items if we need to
                    if (index != -1)
                    {
                        for (int i = _owner.SubItemCount - 1; i >= index; i--)
                        {
                            _owner.subItems[i + size] = _owner.subItems[i];
                        }
                    }
                }
            }

            public int IndexOf(ListViewSubItem subItem)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (_owner.subItems[index] == subItem)
                    {
                        return index;
                    }
                }

                return -1;
            }

            int IList.IndexOf(object subItem)
            {
                if (!(subItem is ListViewSubItem subItemValue))
                {
                    return -1;
                }

                return IndexOf(subItemValue);
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return -1;
                }

                if (IsValidIndex(_lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, ignoreCase: true))
                    {
                        return _lastAccessedIndex;
                    }
                }

                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, ignoreCase: true))
                    {
                        _lastAccessedIndex = i;
                        return i;
                    }
                }

                _lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index) => ((index >= 0) && (index < Count));

            public void Insert(int index, ListViewSubItem item)
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                item.owner = _owner;

                EnsureSubItemSpace(1, index);

                // Insert new item
                _owner.subItems[index] = item;
                _owner.SubItemCount++;
                _owner.UpdateSubItems(-1);
            }

            void IList.Insert(int index, object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    throw new ArgumentException(SR.ListViewBadListViewSubItem, nameof(item));
                }

                Insert(index, (ListViewSubItem)item);
            }

            public void Remove(ListViewSubItem item)
            {
                int index = IndexOf(item);
                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object item)
            {
                if (item is ListViewSubItem itemValue)
                {
                    Remove(itemValue);
                }
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                // Remove ourselves as the owner.
                _owner.subItems[index].owner = null;

                // Collapse the items
                for (int i = index + 1; i < _owner.SubItemCount; i++)
                {
                    _owner.subItems[i - 1] = _owner.subItems[i];
                }

                int oldCount = _owner.SubItemCount;
                _owner.SubItemCount--;
                _owner.subItems[_owner.SubItemCount] = null;
                _owner.UpdateSubItems(-1, oldCount);
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    Array.Copy(_owner.subItems, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (_owner.subItems != null)
                {
                    return new WindowsFormsUtils.ArraySubsetEnumerator(_owner.subItems, _owner.SubItemCount);
                }
                else
                {
                    return Array.Empty<ListViewSubItem>().GetEnumerator();
                }

            }
        }
    }
}
