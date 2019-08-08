// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is the base class for all menu components (MainMenu, MenuItem, and ContextMenu).
    /// </summary>
    [ToolboxItemFilter("System.Windows.Forms")]
    [ListBindable(false)]
    public abstract class Menu : Component
    {
        internal const int CHANGE_ITEMS = 0; // item(s) added or removed
        internal const int CHANGE_VISIBLE = 1; // item(s) hidden or shown
        internal const int CHANGE_MDI = 2; // mdi item changed
        internal const int CHANGE_MERGE = 3; // mergeType or mergeOrder changed
        internal const int CHANGE_ITEMADDED = 4; // mergeType or mergeOrder changed

        /// <summary>
        ///  Used by findMenuItem
        /// </summary>
        public const int FindHandle = 0;
        /// <summary>
        ///  Used by findMenuItem
        /// </summary>
        public const int FindShortcut = 1;

        private MenuItemCollection itemsCollection;
        internal MenuItem[] items;
        private int _itemCount;
        internal IntPtr handle;
        internal bool created;
        private object userData;
        private string name;

        /// <summary>
        ///  This is an abstract class.  Instances cannot be created, so the constructor
        ///  is only called from derived classes.
        /// </summary>
        protected Menu(MenuItem[] items)
        {
            if (items != null)
            {
                MenuItems.AddRange(items);
            }
        }

        /// <summary>
        ///  The HMENU handle corresponding to this menu.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHandleDescr))
        ]
        public IntPtr Handle
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    handle = CreateMenuHandle();
                }

                CreateMenuItems();
                return handle;
            }
        }

        /// <summary>
        ///  Specifies whether this menu contains any items.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MenuIsParentDescr))
        ]
        public virtual bool IsParent
        {
            get
            {
                return null != items && ItemCount > 0;
            }
        }

        internal int ItemCount
        {
            get
            {
                return _itemCount;
            }
        }

        /// <summary>
        ///  The MenuItem that contains the list of MDI child windows.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MenuMDIListItemDescr))
        ]
        public MenuItem MdiListItem
        {
            get
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    MenuItem item = items[i];
                    if (item.MdiList)
                    {
                        return item;
                    }

                    if (item.IsParent)
                    {
                        item = item.MdiListItem;
                        if (item != null)
                        {
                            return item;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///  Name of this control. The designer will set this to the same
        ///  as the programatic Id "(name)" of the control - however this
        ///  property has no bearing on the runtime aspects of this control.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public string Name
        {
            get
            {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    name = null;
                }
                else
                {
                    name = value;
                }
                if (Site != null)
                {
                    Site.Name = name;
                }
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRDescription(nameof(SR.MenuMenuItemsDescr)),
        MergableProperty(false)
        ]
        public MenuItemCollection MenuItems
        {
            get
            {
                if (itemsCollection == null)
                {
                    itemsCollection = new MenuItemCollection(this);
                }
                return itemsCollection;
            }
        }

        internal virtual bool RenderIsRightToLeft
        {
            get
            {
                Debug.Assert(true, "Should never get called");
                return false;

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
        public object Tag
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        /// <summary>
        ///  Notifies Menu that someone called Windows.DeleteMenu on its handle.
        /// </summary>
        internal void ClearHandles()
        {
            if (handle != IntPtr.Zero)
            {
                UnsafeNativeMethods.DestroyMenu(new HandleRef(this, handle));
            }
            handle = IntPtr.Zero;
            if (created)
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    items[i].ClearHandles();
                }
                created = false;
            }
        }

        /// <summary>
        ///  Sets this menu to be an identical copy of another menu.
        /// </summary>
        protected internal void CloneMenu(Menu menuSrc)
        {
            if (menuSrc == null)
            {
                throw new ArgumentNullException(nameof(menuSrc));
            }

            MenuItem[] newItems = null;
            if (menuSrc.items != null)
            {
                int count = menuSrc.MenuItems.Count;
                newItems = new MenuItem[count];
                for (int i = 0; i < count; i++)
                {
                    newItems[i] = menuSrc.MenuItems[i].CloneMenu();
                }
            }
            MenuItems.Clear();
            if (newItems != null)
            {
                MenuItems.AddRange(newItems);
            }
        }

        protected virtual IntPtr CreateMenuHandle()
        {
            return UnsafeNativeMethods.CreatePopupMenu();
        }

        internal void CreateMenuItems()
        {
            if (!created)
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    items[i].CreateMenuItem();
                }
                created = true;
            }
        }

        internal void DestroyMenuItems()
        {
            if (created)
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    items[i].ClearHandles();
                }
                while (UnsafeNativeMethods.GetMenuItemCount(new HandleRef(this, handle)) > 0)
                {
                    UnsafeNativeMethods.RemoveMenu(new HandleRef(this, handle), 0, NativeMethods.MF_BYPOSITION);
                }
                created = false;
            }
        }

        /// <summary>
        ///  Disposes of the component.  Call dispose when the component is no longer needed.
        ///  This method removes the component from its container (if the component has a site)
        ///  and triggers the dispose event.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                while (ItemCount > 0)
                {
                    MenuItem item = items[--_itemCount];

                    // remove the item before we dispose it so it still has valid state
                    // for undo/redo
                    //
                    if (item.Site != null && item.Site.Container != null)
                    {
                        item.Site.Container.Remove(item);
                    }

                    item.Parent = null;
                    item.Dispose();
                }
                items = null;
            }
            if (handle != IntPtr.Zero)
            {
                UnsafeNativeMethods.DestroyMenu(new HandleRef(this, handle));
                handle = IntPtr.Zero;
                if (disposing)
                {
                    ClearHandles();
                }
            }
            base.Dispose(disposing);
        }

        public MenuItem FindMenuItem(int type, IntPtr value)
        {
            for (int i = 0; i < ItemCount; i++)
            {
                MenuItem item = items[i];
                switch (type)
                {
                    case FindHandle:
                        if (item.handle == value)
                        {
                            return item;
                        }

                        break;
                    case FindShortcut:
                        if (item.Shortcut == (Shortcut)(int)value)
                        {
                            return item;
                        }

                        break;
                }
                item = item.FindMenuItem(type, value);
                if (item != null)
                {
                    return item;
                }
            }
            return null;
        }

        protected int FindMergePosition(int mergeOrder)
        {
            int iMin, iLim, iT;

            for (iMin = 0, iLim = ItemCount; iMin < iLim;)
            {
                iT = (iMin + iLim) / 2;
                if (items[iT].MergeOrder <= mergeOrder)
                {
                    iMin = iT + 1;
                }
                else
                {
                    iLim = iT;
                }
            }
            return iMin;
        }

        // A new method for finding the approximate merge position. The original
        // method assumed (incorrectly) that the MergeOrder of the target menu would be sequential
        // as it's guaranteed to be in the MDI imlementation of merging container and child
        // menus. However, user code can call MergeMenu independently on a source and target
        // menu whose MergeOrder values are not necessarily pre-sorted.
        internal int xFindMergePosition(int mergeOrder)
        {
            int nPosition = 0;

            // Iterate from beginning to end since we can't assume any sequential ordering to MergeOrder
            for (int nLoop = 0; nLoop < ItemCount; nLoop++)
            {

                if (items[nLoop].MergeOrder > mergeOrder)
                {
                    // We didn't find what we're looking for, but we've found a stopping point.
                    break;
                }
                else if (items[nLoop].MergeOrder < mergeOrder)
                {
                    // We might have found what we're looking for, but we'll have to come around again
                    // to know.
                    nPosition = nLoop + 1;
                }
                else if (mergeOrder == items[nLoop].MergeOrder)
                {
                    // We've found what we're looking for, so use this value for the merge order
                    nPosition = nLoop;
                    break;
                }
            }

            return nPosition;
        }

        //There's a win32 problem that doesn't allow menus to cascade right to left
        //unless we explicitely set the bit on the menu the first time it pops up
        internal void UpdateRtl(bool setRightToLeftBit)
        {
            foreach (MenuItem item in MenuItems)
            {
                item.UpdateItemRtl(setRightToLeftBit);
                item.UpdateRtl(setRightToLeftBit);
            }
        }

        /// <summary>
        ///  Returns the ContextMenu that contains this menu.  The ContextMenu
        ///  is at the top of this menu's parent chain.
        ///  Returns null if this menu is not contained in a ContextMenu.
        ///  This can occur if it's contained in a MainMenu or if it isn't
        ///  currently contained in any menu at all.
        /// </summary>
        public ContextMenu GetContextMenu()
        {
            Menu menuT;
            for (menuT = this; !(menuT is ContextMenu);)
            {
                if (!(menuT is MenuItem))
                {
                    return null;
                }

                menuT = ((MenuItem)menuT).Parent;
            }
            return (ContextMenu)menuT;

        }

        /// <summary>
        ///  Returns the MainMenu item that contains this menu.  The MainMenu
        ///  is at the top of this menu's parent chain.
        ///  Returns null if this menu is not contained in a MainMenu.
        ///  This can occur if it's contained in a ContextMenu or if it isn't
        ///  currently contained in any menu at all.
        /// </summary>
        public MainMenu GetMainMenu()
        {
            Menu menuT;
            for (menuT = this; !(menuT is MainMenu);)
            {
                if (!(menuT is MenuItem))
                {
                    return null;
                }

                menuT = ((MenuItem)menuT).Parent;
            }
            return (MainMenu)menuT;
        }

        internal virtual void ItemsChanged(int change)
        {
            switch (change)
            {
                case CHANGE_ITEMS:
                case CHANGE_VISIBLE:
                    DestroyMenuItems();
                    break;
            }
        }

        /// <summary>
        ///  Walks the menu item collection, using a caller-supplied delegate to find one
        ///  with a matching access key. Walk starts at specified item index and performs one
        ///  full pass of the entire collection, looping back to the top if necessary.
        ///
        ///  Return value is intended for return from WM_MENUCHAR message. It includes both
        ///  index of matching item, and action for OS to take (execute or select). Zero is
        ///  used to indicate that no match was found (OS should ignore key and beep).
        /// </summary>
        private IntPtr MatchKeyToMenuItem(int startItem, char key, MenuItemKeyComparer comparer)
        {
            int firstMatch = -1;
            bool multipleMatches = false;

            for (int i = 0; i < items.Length && !multipleMatches; ++i)
            {
                int itemIndex = (startItem + i) % items.Length;
                MenuItem mi = items[itemIndex];
                if (mi != null && comparer(mi, key))
                {
                    if (firstMatch < 0)
                    {
                        // Using Index doesnt respect hidden items.
                        firstMatch = mi.MenuIndex;
                    }
                    else
                    {
                        multipleMatches = true;
                    }
                }
            }

            if (firstMatch < 0)
            {
                return IntPtr.Zero;
            }

            int action = multipleMatches ? NativeMethods.MNC_SELECT : NativeMethods.MNC_EXECUTE;
            return (IntPtr)NativeMethods.Util.MAKELONG(firstMatch, action);
        }

        ///  Delegate type used by MatchKeyToMenuItem
        private delegate bool MenuItemKeyComparer(MenuItem mi, char key);

        /// <summary>
        ///  Merges another menu's items with this one's.  Menu items are merged according to their
        ///  mergeType and mergeOrder properties.  This function is typically used to
        ///  merge an MDI container's menu with that of its active MDI child.
        /// </summary>
        public virtual void MergeMenu(Menu menuSrc)
        {
            if (menuSrc == null)
            {
                throw new ArgumentNullException(nameof(menuSrc));
            }
            if (menuSrc == this)
            {
                throw new ArgumentException(SR.MenuMergeWithSelf, nameof(menuSrc));
            }

            int i, j;
            MenuItem item;
            MenuItem itemDst;

            if (menuSrc.items != null && items == null)
            {
                MenuItems.Clear();
            }

            for (i = 0; i < menuSrc.ItemCount; i++)
            {
                item = menuSrc.items[i];

                switch (item.MergeType)
                {
                    default:
                        continue;
                    case MenuMerge.Add:
                        MenuItems.Add(FindMergePosition(item.MergeOrder), item.MergeMenu());
                        continue;
                    case MenuMerge.Replace:
                    case MenuMerge.MergeItems:
                        break;
                }

                int mergeOrder = item.MergeOrder;
                // Can we find a menu item with a matching merge order?
                // Use new method to find the approximate merge position. The original
                // method assumed (incorrectly) that the MergeOrder of the target menu would be sequential
                // as it's guaranteed to be in the MDI imlementation of merging container and child
                // menus. However, user code can call MergeMenu independently on a source and target
                // menu whose MergeOrder values are not necessarily pre-sorted.
                for (j = xFindMergePosition(mergeOrder); ; j++)
                {

                    if (j >= ItemCount)
                    {
                        // A matching merge position could not be found,
                        // so simply append this menu item to the end.
                        MenuItems.Add(j, item.MergeMenu());
                        break;
                    }
                    itemDst = items[j];
                    if (itemDst.MergeOrder != mergeOrder)
                    {
                        MenuItems.Add(j, item.MergeMenu());
                        break;
                    }
                    if (itemDst.MergeType != MenuMerge.Add)
                    {
                        if (item.MergeType != MenuMerge.MergeItems
                            || itemDst.MergeType != MenuMerge.MergeItems)
                        {
                            itemDst.Dispose();
                            MenuItems.Add(j, item.MergeMenu());
                        }
                        else
                        {
                            itemDst.MergeMenu(item);
                        }
                        break;
                    }
                }
            }
        }

        internal virtual bool ProcessInitMenuPopup(IntPtr handle)
        {
            MenuItem item = FindMenuItem(FindHandle, handle);
            if (item != null)
            {
                item.OnInitMenuPopup(EventArgs.Empty);
                item.CreateMenuItems();
                return true;
            }
            return false;
        }

        protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            MenuItem item = FindMenuItem(FindShortcut, (IntPtr)(int)keyData);
            return item != null ? item.ShortcutClick() : false;
        }

        /// <summary>
        ///  Returns index of currently selected menu item in
        ///  this menu, or -1 if no item is currently selected.
        /// </summary>
        internal int SelectedMenuItemIndex
        {
            get
            {
                for (int i = 0; i < items.Length; ++i)
                {
                    MenuItem mi = items[i];
                    if (mi != null && mi.Selected)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Items.Count: " + ItemCount.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///  Handles the WM_MENUCHAR message, forwarding it to the intended Menu
        ///  object. All the real work is done inside WmMenuCharInternal().
        /// </summary>
        internal void WmMenuChar(ref Message m)
        {
            Menu menu = (m.LParam == handle) ? this : FindMenuItem(FindHandle, m.LParam);

            if (menu == null)
            {
                return;
            }

            char menuKey = char.ToUpper((char)NativeMethods.Util.LOWORD(m.WParam), CultureInfo.CurrentCulture);

            m.Result = menu.WmMenuCharInternal(menuKey);
        }

        /// <summary>
        ///  Handles WM_MENUCHAR to provide access key support for owner-draw menu items (which
        ///  means *all* menu items on a menu when IsImageMarginPresent == true). Attempts to
        ///  simulate the exact behavior that the OS provides for non owner-draw menu items.
        /// </summary>
        internal IntPtr WmMenuCharInternal(char key)
        {
            // Start looking just beyond the current selected item (otherwise just start at the top)
            int startItem = (SelectedMenuItemIndex + 1) % items.Length;

            // First, search for match among owner-draw items with explicitly defined access keys (eg. "S&ave")
            IntPtr result = MatchKeyToMenuItem(startItem, key, new MenuItemKeyComparer(CheckOwnerDrawItemWithMnemonic));

            // Next, search for match among owner-draw items with no access keys (looking at first char of item text)
            if (result == IntPtr.Zero)
            {
                result = MatchKeyToMenuItem(startItem, key, new MenuItemKeyComparer(CheckOwnerDrawItemNoMnemonic));
            }

            return result;
        }

        ///  MenuItemKeyComparer delegate used by WmMenuCharInternal
        private bool CheckOwnerDrawItemWithMnemonic(MenuItem mi, char key)
        {
            return mi.OwnerDraw &&
                   mi.Mnemonic == key;
        }

        ///  MenuItemKeyComparer delegate used by WmMenuCharInternal
        private bool CheckOwnerDrawItemNoMnemonic(MenuItem mi, char key)
        {
            return mi.OwnerDraw &&
                   mi.Mnemonic == 0 &&
                   mi.Text.Length > 0 &&
                   char.ToUpper(mi.Text[0], CultureInfo.CurrentCulture) == key;
        }

        [ListBindable(false)]
        public class MenuItemCollection : IList
        {
            private readonly Menu owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            public MenuItemCollection(Menu owner)
            {
                this.owner = owner;
            }

            public virtual MenuItem this[int index]
            {
                get
                {
                    if (index < 0 || index >= owner.ItemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return owner.items[index];
                }
                // set not supported
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual MenuItem this[string key]
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
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }

                }
            }

            public int Count
            {
                get
                {
                    return owner.ItemCount;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Adds a new MenuItem to the end of this menu with the specified caption.
            /// </summary>
            public virtual MenuItem Add(string caption)
            {
                MenuItem item = new MenuItem(caption);
                Add(item);
                return item;
            }

            /// <summary>
            ///  Adds a new MenuItem to the end of this menu with the specified caption,
            ///  and click handler.
            /// </summary>
            public virtual MenuItem Add(string caption, EventHandler onClick)
            {
                MenuItem item = new MenuItem(caption, onClick);
                Add(item);
                return item;
            }

            /// <summary>
            ///  Adds a new MenuItem to the end of this menu with the specified caption,
            ///  click handler, and items.
            /// </summary>
            public virtual MenuItem Add(string caption, MenuItem[] items)
            {
                MenuItem item = new MenuItem(caption, items);
                Add(item);
                return item;
            }

            /// <summary>
            ///  Adds a MenuItem to the end of this menu
            ///  MenuItems can only be contained in one menu at a time, and may not be added
            ///  more than once to the same menu.
            /// </summary>
            public virtual int Add(MenuItem item)
            {
                return Add(owner.ItemCount, item);
            }

            /// <summary>
            ///  Adds a MenuItem to this menu at the specified index.  The item currently at
            ///  that index, and all items after it, will be moved up one slot.
            ///  MenuItems can only be contained in one menu at a time, and may not be added
            ///  more than once to the same menu.
            /// </summary>
            public virtual int Add(int index, MenuItem item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                // MenuItems can only belong to one menu at a time
                if (item.Parent != null)
                {

                    // First check that we're not adding ourself, i.e. walk
                    // the parent chain for equality
                    if (owner is MenuItem parent)
                    {
                        while (parent != null)
                        {
                            if (parent.Equals(item))
                            {
                                throw new ArgumentException(string.Format(SR.MenuItemAlreadyExists, item.Text), "item");
                            }
                            if (parent.Parent is MenuItem)
                            {
                                parent = (MenuItem)parent.Parent;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //if we're re-adding an item back to the same collection
                    //the target index needs to be decremented since we're
                    //removing an item from the collection
                    if (item.Parent.Equals(owner) && index > 0)
                    {
                        index--;
                    }

                    item.Parent.MenuItems.Remove(item);
                }

                // Validate our index
                if (index < 0 || index > owner.ItemCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (owner.items == null || owner.items.Length == owner.ItemCount)
                {
                    MenuItem[] newItems = new MenuItem[owner.ItemCount < 2 ? 4 : owner.ItemCount * 2];
                    if (owner.ItemCount > 0)
                    {
                        System.Array.Copy(owner.items, 0, newItems, 0, owner.ItemCount);
                    }

                    owner.items = newItems;
                }
                System.Array.Copy(owner.items, index, owner.items, index + 1, owner.ItemCount - index);
                owner.items[index] = item;
                owner._itemCount++;
                item.Parent = owner;
                owner.ItemsChanged(CHANGE_ITEMS);
                if (owner is MenuItem)
                {
                    ((MenuItem)owner).ItemsChanged(CHANGE_ITEMADDED, item);
                }

                return index;
            }

            public virtual void AddRange(MenuItem[] items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                foreach (MenuItem item in items)
                {
                    Add(item);
                }
            }

            int IList.Add(object value)
            {
                if (value is MenuItem)
                {
                    return Add((MenuItem)value);
                }
                else
                {
                    throw new ArgumentException(SR.MenuBadMenuItem, "value");
                }
            }

            public bool Contains(MenuItem value)
            {
                return IndexOf(value) != -1;
            }

            bool IList.Contains(object value)
            {
                if (value is MenuItem)
                {
                    return Contains((MenuItem)value);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an array
            ///  of all the controls that match.
                    /// </summary>
            public MenuItem[] Find(string key, bool searchAllChildren)
            {

                if ((key == null) || (key.Length == 0))
                {
                    throw new ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
                }

                ArrayList foundMenuItems = FindInternal(key, searchAllChildren, this, new ArrayList());

                // Make this a stongly typed collection.
                MenuItem[] stronglyTypedfoundMenuItems = new MenuItem[foundMenuItems.Count];
                foundMenuItems.CopyTo(stronglyTypedfoundMenuItems, 0);

                return stronglyTypedfoundMenuItems;
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an array list
            ///  of all the controls that match.
                    /// </summary>
            private ArrayList FindInternal(string key, bool searchAllChildren, MenuItemCollection menuItemsToLookIn, ArrayList foundMenuItems)
            {
                if ((menuItemsToLookIn == null) || (foundMenuItems == null))
                {
                    return null;  //
                }

                // Perform breadth first search - as it's likely people will want controls belonging
                // to the same parent close to each other.

                for (int i = 0; i < menuItemsToLookIn.Count; i++)
                {
                    if (menuItemsToLookIn[i] == null)
                    {
                        continue;
                    }

                    if (WindowsFormsUtils.SafeCompareStrings(menuItemsToLookIn[i].Name, key, /* ignoreCase = */ true))
                    {
                        foundMenuItems.Add(menuItemsToLookIn[i]);
                    }
                }

                // Optional recurive search for controls in child collections.

                if (searchAllChildren)
                {
                    for (int i = 0; i < menuItemsToLookIn.Count; i++)
                    {
                        if (menuItemsToLookIn[i] == null)
                        {
                            continue;
                        }
                        if ((menuItemsToLookIn[i].MenuItems != null) && menuItemsToLookIn[i].MenuItems.Count > 0)
                        {
                            // if it has a valid child collecion, append those results to our collection
                            foundMenuItems = FindInternal(key, searchAllChildren, menuItemsToLookIn[i].MenuItems, foundMenuItems);
                        }
                    }
                }
                return foundMenuItems;
            }

            public int IndexOf(MenuItem value)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (this[index] == value)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object value)
            {
                if (value is MenuItem)
                {
                    return IndexOf((MenuItem)value);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            void IList.Insert(int index, object value)
            {
                if (value is MenuItem)
                {
                    Add(index, (MenuItem)value);
                }
                else
                {
                    throw new ArgumentException(SR.MenuBadMenuItem, "value");
                }
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            /// <summary>
            ///  Removes all existing MenuItems from this menu
            /// </summary>
            public virtual void Clear()
            {
                if (owner.ItemCount > 0)
                {

                    for (int i = 0; i < owner.ItemCount; i++)
                    {
                        owner.items[i].Parent = null;
                    }

                    owner._itemCount = 0;
                    owner.items = null;

                    owner.ItemsChanged(CHANGE_ITEMS);

                    if (owner is MenuItem)
                    {
                        ((MenuItem)(owner)).UpdateMenuItem(true);
                    }
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.ItemCount > 0)
                {
                    System.Array.Copy(owner.items, 0, dest, index, owner.ItemCount);
                }
            }

            public IEnumerator GetEnumerator()
            {
                return new WindowsFormsUtils.ArraySubsetEnumerator(owner.items, owner.ItemCount);
            }

            /// <summary>
            ///  Removes the item at the specified index in this menu.  All subsequent
            ///  items are moved up one slot.
            /// </summary>
            public virtual void RemoveAt(int index)
            {
                if (index < 0 || index >= owner.ItemCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                MenuItem item = owner.items[index];
                item.Parent = null;
                owner._itemCount--;
                System.Array.Copy(owner.items, index + 1, owner.items, index, owner.ItemCount - index);
                owner.items[owner.ItemCount] = null;
                owner.ItemsChanged(CHANGE_ITEMS);

                //if the last item was removed, clear the collection
                //
                if (owner.ItemCount == 0)
                {
                    Clear();
                }

            }

            /// <summary>
            ///  Removes the menu iteml with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Removes the specified item from this menu.  All subsequent
            ///  items are moved down one slot.
            /// </summary>
            public virtual void Remove(MenuItem item)
            {
                if (item.Parent == owner)
                {
                    RemoveAt(item.Index);
                }
            }

            void IList.Remove(object value)
            {
                if (value is MenuItem)
                {
                    Remove((MenuItem)value);
                }
            }
        }
    }
}
