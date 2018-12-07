// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Security.Permissions;    
    using System.Collections;
    using System.Windows.Forms.Design;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Security;
    using System.Globalization;
    using System.Runtime.Versioning;
    
    /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu"]/*' />
    /// <devdoc>
    ///     This is the base class for all menu components (MainMenu, MenuItem, and ContextMenu).
    /// </devdoc>
    [
    ToolboxItemFilter("System.Windows.Forms"),
    ListBindable(false)
    ]
    public abstract class Menu : Component {
        internal const int CHANGE_ITEMS     = 0; // item(s) added or removed
        internal const int CHANGE_VISIBLE   = 1; // item(s) hidden or shown
        internal const int CHANGE_MDI       = 2; // mdi item changed
        internal const int CHANGE_MERGE     = 3; // mergeType or mergeOrder changed
        internal const int CHANGE_ITEMADDED = 4; // mergeType or mergeOrder changed

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.FindHandle"]/*' />
        /// <devdoc>
        ///     Used by findMenuItem
        /// </devdoc>
        /// <internalonly/>
        public const int FindHandle = 0;
        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.FindShortcut"]/*' />
        /// <devdoc>
        ///     Used by findMenuItem
        /// </devdoc>
        /// <internalonly/>
        public const int FindShortcut = 1;

        private MenuItemCollection itemsCollection;
        internal MenuItem[] items;
        private int _itemCount;
        internal IntPtr handle;
        internal bool created;
        private object userData;
        private string name;
        
        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.Menu"]/*' />
        /// <devdoc>
        ///     This is an abstract class.  Instances cannot be created, so the constructor
        ///     is only called from derived classes.
        /// </devdoc>
        /// <internalonly/>
        protected Menu(MenuItem[] items) {
            if (items != null) {
                MenuItems.AddRange(items);
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.Handle"]/*' />
        /// <devdoc>
        ///     The HMENU handle corresponding to this menu.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHandleDescr))
        ]
        public IntPtr Handle {
            get {
                if (handle == IntPtr.Zero) handle = CreateMenuHandle();
                CreateMenuItems();
                return handle;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.IsParent"]/*' />
        /// <devdoc>
        ///     Specifies whether this menu contains any items.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MenuIsParentDescr))
        ]
        public virtual bool IsParent {
            [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
            get {
                return null != items && ItemCount > 0;
            }
        }

        internal int ItemCount {
            get {
                return _itemCount;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MdiListItem"]/*' />
        /// <devdoc>
        ///     The MenuItem that contains the list of MDI child windows.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MenuMDIListItemDescr))
        ]
        public MenuItem MdiListItem {
            get {
                for (int i = 0; i < ItemCount; i++) {
                    MenuItem item = items[i];
                    if (item.MdiList)
                        return item;
                    if (item.IsParent) {
                        item = item.MdiListItem;
                        if (item != null) return item;
                    }
                }
                return null;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.Name"]/*' />
        /// <devdoc>
        ///     Name of this control. The designer will set this to the same
        ///     as the programatic Id "(name)" of the control - however this
        ///     property has no bearing on the runtime aspects of this control.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public string Name {
            get {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set {
                if (value == null || value.Length == 0) {
                    name = null;
                }
                else {
                   name = value;
                }
                if(Site!= null) {
                    Site.Name = name;
                }
            }
        }
        

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItems"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRDescription(nameof(SR.MenuMenuItemsDescr)),
        MergableProperty(false)
        ]
        public MenuItemCollection MenuItems {
            get {
                if (itemsCollection == null) {
                    itemsCollection = new MenuItemCollection(this);
                }
                return itemsCollection;
            }
        }

        internal virtual bool RenderIsRightToLeft {
            get {
                Debug.Assert(true, "Should never get called");
                return false;
                
            }
        }


        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.Tag"]/*' />
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
        
        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.ClearHandles"]/*' />
        /// <devdoc>
        ///     Notifies Menu that someone called Windows.DeleteMenu on its handle.
        /// </devdoc>
        /// <internalonly/>
        internal void ClearHandles() {
            if (handle != IntPtr.Zero) {
                UnsafeNativeMethods.DestroyMenu(new HandleRef(this, handle));
            }
            handle = IntPtr.Zero;
            if (created) {
                for (int i = 0; i < ItemCount; i++) {
                    items[i].ClearHandles();
                }
                created = false;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.CloneMenu"]/*' />
        /// <devdoc>
        ///     Sets this menu to be an identical copy of another menu.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Shipped as is in Everett
        ]
        protected internal void CloneMenu(Menu menuSrc) {
            MenuItem[] newItems = null;
            if (menuSrc.items != null) {
                int count = menuSrc.MenuItems.Count;
                newItems = new MenuItem[count];
                for (int i = 0; i < count; i++)
                    newItems[i] = menuSrc.MenuItems[i].CloneMenu();
            }
            MenuItems.Clear();
            if (newItems != null) {
                MenuItems.AddRange(newItems);
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.CreateMenuHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        protected virtual IntPtr CreateMenuHandle() {
            return UnsafeNativeMethods.CreatePopupMenu();
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.CreateMenuItems"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void CreateMenuItems() {
            if (!created) {
                for (int i = 0; i < ItemCount; i++) {
                    items[i].CreateMenuItem();
                }
                created = true;
            }
        }
   
        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.DestroyMenuItems"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void DestroyMenuItems() {
            if (created) {
                for (int i = 0; i < ItemCount; i++) {
                    items[i].ClearHandles();
                }
                while (UnsafeNativeMethods.GetMenuItemCount(new HandleRef(this, handle)) > 0) {
                    UnsafeNativeMethods.RemoveMenu(new HandleRef(this, handle), 0, NativeMethods.MF_BYPOSITION);
                }
                created = false;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.Dispose"]/*' />
        /// <devdoc>
        ///     Disposes of the component.  Call dispose when the component is no longer needed.
        ///     This method removes the component from its container (if the component has a site)
        ///     and triggers the dispose event.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                while (ItemCount > 0) {
                    MenuItem item = items[--_itemCount];

                    // remove the item before we dispose it so it still has valid state
                    // for undo/redo
                    //
                    if (item.Site != null && item.Site.Container != null) {
                        item.Site.Container.Remove(item);
                    }

                    item.Menu = null;
                    item.Dispose();
                }
                items = null;
            }
            if (handle != IntPtr.Zero) {
                UnsafeNativeMethods.DestroyMenu(new HandleRef(this, handle));
                this.handle = IntPtr.Zero;
                if (disposing) {
                    ClearHandles();
                }
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.FindMenuItem"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        public MenuItem FindMenuItem(int type, IntPtr value) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ControlFromHandleOrLocation Demanded");
            IntSecurity.ControlFromHandleOrLocation.Demand();
            return FindMenuItemInternal(type, value);
        }

        private MenuItem FindMenuItemInternal(int type, IntPtr value) {
            for (int i = 0; i < ItemCount; i++) {
                MenuItem item = items[i];
                switch (type) {
                    case FindHandle:
                        if (item.handle == value) return item;
                        break;
                    case FindShortcut:
                        if (item.Shortcut == (Shortcut)(int)value) return item;
                        break;
                }
                item = item.FindMenuItemInternal(type, value);
                if (item != null) return item;
            }
            return null;
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.FindMergePosition"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected int FindMergePosition(int mergeOrder) {
            int iMin, iLim, iT;

            for (iMin = 0, iLim = ItemCount; iMin < iLim;) {
                iT = (iMin + iLim) / 2;
                if (items[iT].MergeOrder <= mergeOrder)
                    iMin = iT + 1;
                else
                    iLim = iT;
            }
            return iMin;
        }
        
        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.xFindMergePosition"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        // A new method for finding the approximate merge position. The original
        // method assumed (incorrectly) that the MergeOrder of the target menu would be sequential
        // as it's guaranteed to be in the MDI imlementation of merging container and child
        // menus. However, user code can call MergeMenu independently on a source and target
        // menu whose MergeOrder values are not necessarily pre-sorted.
        internal int xFindMergePosition(int mergeOrder) {
            int nPosition = 0;

            // Iterate from beginning to end since we can't assume any sequential ordering to MergeOrder
            for (int nLoop = 0; nLoop < ItemCount; nLoop++) {

                if (items[nLoop].MergeOrder > mergeOrder) {
                    // We didn't find what we're looking for, but we've found a stopping point.
                    break;
                }
                else if (items[nLoop].MergeOrder < mergeOrder) {
                    // We might have found what we're looking for, but we'll have to come around again
                    // to know.
                    nPosition = nLoop + 1;
                }
                else if (mergeOrder == items[nLoop].MergeOrder) {
                    // We've found what we're looking for, so use this value for the merge order
                    nPosition = nLoop;
                    break;
                }
            }

            return nPosition;
        }

        //There's a win32 problem that doesn't allow menus to cascade right to left
        //unless we explicitely set the bit on the menu the first time it pops up
        internal void UpdateRtl(bool setRightToLeftBit) {
            foreach (MenuItem item in MenuItems) {
                item.UpdateItemRtl(setRightToLeftBit);
                item.UpdateRtl(setRightToLeftBit);
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.GetContextMenu"]/*' />
        /// <devdoc>
        ///     Returns the ContextMenu that contains this menu.  The ContextMenu
        ///     is at the top of this menu's parent chain.
        ///     Returns null if this menu is not contained in a ContextMenu.
        ///     This can occur if it's contained in a MainMenu or if it isn't
        ///     currently contained in any menu at all.
        /// </devdoc>
        public ContextMenu GetContextMenu() {
            Menu menuT;
            for (menuT = this; !(menuT is ContextMenu);) {
                if (!(menuT is MenuItem)) return null;
                menuT = ((MenuItem)menuT).Menu;
            }
            return(ContextMenu)menuT;

        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.GetMainMenu"]/*' />
        /// <devdoc>
        ///     Returns the MainMenu item that contains this menu.  The MainMenu
        ///     is at the top of this menu's parent chain.
        ///     Returns null if this menu is not contained in a MainMenu.
        ///     This can occur if it's contained in a ContextMenu or if it isn't
        ///     currently contained in any menu at all.
        /// </devdoc>
        public MainMenu GetMainMenu() {
            Menu menuT;
            for (menuT = this; !(menuT is MainMenu);) {
                if (!(menuT is MenuItem)) return null;
                menuT = ((MenuItem)menuT).Menu;
            }
            return(MainMenu)menuT;
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.ItemsChanged"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal virtual void ItemsChanged(int change) {
            switch (change) {
                case CHANGE_ITEMS:
                case CHANGE_VISIBLE:
                    DestroyMenuItems();
                    break;
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MatchKeyToMenuItem"]/*' />
        /// <devdoc>
        ///     Walks the menu item collection, using a caller-supplied delegate to find one
        ///     with a matching access key. Walk starts at specified item index and performs one
        ///     full pass of the entire collection, looping back to the top if necessary.
        ///
        ///     Return value is intended for return from WM_MENUCHAR message. It includes both
        ///     index of matching item, and action for OS to take (execute or select). Zero is
        ///     used to indicate that no match was found (OS should ignore key and beep).
        /// </devdoc>
        /// <internalonly/>
        private IntPtr MatchKeyToMenuItem(int startItem, char key, MenuItemKeyComparer comparer) {
            int firstMatch = -1;
            bool multipleMatches = false;

            for (int i = 0; i < items.Length && !multipleMatches; ++i) {
                int itemIndex = (startItem + i) % items.Length;
                MenuItem mi = items[itemIndex];
                if (mi != null && comparer(mi, key)) {
                    if (firstMatch < 0){
                        // Using Index doesnt respect hidden items.
                        firstMatch = mi.MenuIndex;
                    }
                    else {
                        multipleMatches = true;
                    }
                }
            }

            if (firstMatch < 0)
                return IntPtr.Zero;

            int action = multipleMatches ? NativeMethods.MNC_SELECT : NativeMethods.MNC_EXECUTE;
            return (IntPtr) NativeMethods.Util.MAKELONG(firstMatch, action);
        }

        /// Delegate type used by MatchKeyToMenuItem
        private delegate bool MenuItemKeyComparer(MenuItem mi, char key);

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MergeMenu"]/*' />
        /// <devdoc>
        ///     Merges another menu's items with this one's.  Menu items are merged according to their
        ///     mergeType and mergeOrder properties.  This function is typically used to
        ///     merge an MDI container's menu with that of its active MDI child.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly") // Shipped as is in Everett
        ]
        public virtual void MergeMenu(Menu menuSrc) {
            int i, j;
            MenuItem item;
            MenuItem itemDst;

            if (menuSrc == this)
                throw new ArgumentException(SR.MenuMergeWithSelf, "menuSrc");

            if (menuSrc.items != null && items == null) {
                MenuItems.Clear();                
            }

            for (i = 0; i < menuSrc.ItemCount; i++) {
                item = menuSrc.items[i];

                switch (item.MergeType) {
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
                for (j = xFindMergePosition(mergeOrder); ; j++) {

                    if (j >= ItemCount) {
                        // A matching merge position could not be found,
                        // so simply append this menu item to the end.
                        MenuItems.Add(j, item.MergeMenu());
                        break;
                    }
                    itemDst = items[j];
                    if (itemDst.MergeOrder != mergeOrder) {
                        MenuItems.Add(j, item.MergeMenu());
                        break;
                    }
                    if (itemDst.MergeType != MenuMerge.Add) {
                        if (item.MergeType != MenuMerge.MergeItems
                            || itemDst.MergeType != MenuMerge.MergeItems) {
                            itemDst.Dispose();
                            MenuItems.Add(j, item.MergeMenu());
                        }
                        else {
                            itemDst.MergeMenu(item);
                        }
                        break;
                    }
                }
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.ProcessInitMenuPopup"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal virtual bool ProcessInitMenuPopup(IntPtr handle) {
            MenuItem item = FindMenuItemInternal(FindHandle, handle);
            if (item != null) {
                item._OnInitMenuPopup(EventArgs.Empty);
                item.CreateMenuItems();
                return true;
            }
            return false;
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.ProcessCmdKey"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode),
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
        ]
        protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData) {
            MenuItem item = FindMenuItemInternal(FindShortcut, (IntPtr)(int)keyData);
            return item != null? item.ShortcutClick(): false;
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.SelectedMenuItemIndex"]/*' />
        /// <devdoc>
        ///     Returns index of currently selected menu item in
        ///     this menu, or -1 if no item is currently selected.
        /// </devdoc>
        /// <internalonly/>
        internal int SelectedMenuItemIndex {
            get {
                for (int i = 0; i < items.Length; ++i) {
                    MenuItem mi = items[i];
                    if (mi != null && mi.Selected)
                        return i;
                }
                return -1;
            }
        }


        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Items.Count: " + ItemCount.ToString(CultureInfo.CurrentCulture);
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.WmMenuChar"]/*' />
        /// <devdoc>
        ///     Handles the WM_MENUCHAR message, forwarding it to the intended Menu
        ///     object. All the real work is done inside WmMenuCharInternal().
        /// </devdoc>
        /// <internalonly/>
        internal void WmMenuChar(ref Message m) {
            Menu menu = (m.LParam == handle) ? this : FindMenuItemInternal(FindHandle, m.LParam);

            if (menu == null)
                return;

            char menuKey = Char.ToUpper((char) NativeMethods.Util.LOWORD(m.WParam), CultureInfo.CurrentCulture);

            m.Result = menu.WmMenuCharInternal(menuKey);
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.WmMenuCharInternal"]/*' />
        /// <devdoc>
        ///     Handles WM_MENUCHAR to provide access key support for owner-draw menu items (which
        ///     means *all* menu items on a menu when IsImageMarginPresent == true). Attempts to
        ///     simulate the exact behavior that the OS provides for non owner-draw menu items.
        /// </devdoc>
        /// <internalonly/>
        internal IntPtr WmMenuCharInternal(char key) {
            // Start looking just beyond the current selected item (otherwise just start at the top)
            int startItem = (SelectedMenuItemIndex + 1) % items.Length;

            // First, search for match among owner-draw items with explicitly defined access keys (eg. "S&ave")
            IntPtr result = MatchKeyToMenuItem(startItem, key, new MenuItemKeyComparer(CheckOwnerDrawItemWithMnemonic));

            // Next, search for match among owner-draw items with no access keys (looking at first char of item text)
            if (result == IntPtr.Zero)
                result = MatchKeyToMenuItem(startItem, key, new MenuItemKeyComparer(CheckOwnerDrawItemNoMnemonic));

            return result;
        }

        /// MenuItemKeyComparer delegate used by WmMenuCharInternal
        private bool CheckOwnerDrawItemWithMnemonic(MenuItem mi, char key) {
            return mi.OwnerDraw &&
                   mi.Mnemonic == key;
        }

        /// MenuItemKeyComparer delegate used by WmMenuCharInternal
        private bool CheckOwnerDrawItemNoMnemonic(MenuItem mi, char key) {
            return mi.OwnerDraw &&
                   mi.Mnemonic == 0 &&
                   mi.Text.Length > 0 &&
                   Char.ToUpper(mi.Text[0], CultureInfo.CurrentCulture) == key;
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection"]/*' />
        [ListBindable(false)]
        public class MenuItemCollection : IList {
            private Menu owner;

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.MenuItemCollection"]/*' />
            public MenuItemCollection(Menu owner) {
                this.owner = owner;
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.this"]/*' />
            public virtual MenuItem this[int index] {
                get {
                    if (index < 0 || index >= owner.ItemCount)
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    return owner.items[index];
                }
                // set not supported
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    throw new NotSupportedException();
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual MenuItem this[string key] {
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
  
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Count"]/*' />
            public int Count {
                get {
                    return owner.ItemCount;
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }
           
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }


            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds a new MenuItem to the end of this menu with the specified caption.
            /// </devdoc>
            public virtual MenuItem Add(string caption) {
                MenuItem item = new MenuItem(caption);
                Add(item);
                return item;
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Add1"]/*' />
            /// <devdoc>
            ///     Adds a new MenuItem to the end of this menu with the specified caption,
            ///     and click handler.
            /// </devdoc>
            public virtual MenuItem Add(string caption, EventHandler onClick) {
                MenuItem item = new MenuItem(caption, onClick);
                Add(item);
                return item;
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Add2"]/*' />
            /// <devdoc>
            ///     Adds a new MenuItem to the end of this menu with the specified caption,
            ///     click handler, and items.
            /// </devdoc>
            public virtual MenuItem Add(string caption, MenuItem[] items) {
                MenuItem item = new MenuItem(caption, items);
                Add(item);
                return item;
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Add3"]/*' />
            /// <devdoc>
            ///     Adds a MenuItem to the end of this menu
            ///     MenuItems can only be contained in one menu at a time, and may not be added
            ///     more than once to the same menu.
            /// </devdoc>
            public virtual int Add(MenuItem item) {                
                return Add(owner.ItemCount, item);
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Add4"]/*' />
            /// <devdoc>
            ///     Adds a MenuItem to this menu at the specified index.  The item currently at
            ///     that index, and all items after it, will be moved up one slot.
            ///     MenuItems can only be contained in one menu at a time, and may not be added
            ///     more than once to the same menu.
            /// </devdoc>
            public virtual int Add(int index, MenuItem item) {
                
                // MenuItems can only belong to one menu at a time
                if (item.Menu != null) {

                    // First check that we're not adding ourself, i.e. walk
                    // the parent chain for equality
                    if (owner is MenuItem) {
                        MenuItem parent = (MenuItem)owner;
                        while (parent != null) {
                            if (parent.Equals(item)) {
                                throw new ArgumentException(string.Format(SR.MenuItemAlreadyExists, item.Text), "item");
                            }
                            if (parent.Parent is MenuItem)
                                parent = (MenuItem)parent.Parent;
                            else 
                                break;
                        }
                    }

                    //if we're re-adding an item back to the same collection
                    //the target index needs to be decremented since we're
                    //removing an item from the collection
                    if (item.Menu.Equals(owner) && index > 0) {
                        index--;
                    }

                    item.Menu.MenuItems.Remove(item);
                }

                // Validate our index
                if (index < 0 || index > owner.ItemCount) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument,"index",(index).ToString(CultureInfo.CurrentCulture)));
                }
                                
                if (owner.items == null || owner.items.Length == owner.ItemCount) {
                    MenuItem[] newItems = new MenuItem[owner.ItemCount < 2? 4: owner.ItemCount * 2];
                    if (owner.ItemCount > 0) System.Array.Copy(owner.items, 0, newItems, 0, owner.ItemCount);
                    owner.items = newItems;
                }
                System.Array.Copy(owner.items, index, owner.items, index + 1, owner.ItemCount - index);
                owner.items[index] = item;
                owner._itemCount++;
                item.Menu = owner;
                owner.ItemsChanged(CHANGE_ITEMS);
                if (owner is MenuItem) {
                   ((MenuItem) owner).ItemsChanged(CHANGE_ITEMADDED, item);
                }

                return index;
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.AddRange"]/*' />
            public virtual void AddRange(MenuItem[] items) {
                if (items == null) {
                    throw new ArgumentNullException(nameof(items));
                }
                foreach(MenuItem item in items) {
                    Add(item);
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                if (value is MenuItem) {
                    return Add((MenuItem)value);
                }
                else {  
                    throw new ArgumentException(SR.MenuBadMenuItem, "value");
                }
            }
           
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Contains"]/*' />
            public bool Contains(MenuItem value) {
                return IndexOf(value) != -1;
            }
        
            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object value) {
                if (value is MenuItem) {
                    return Contains((MenuItem)value);
                }
                else {  
                    return false;
                }
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
               return IsValidIndex(IndexOfKey(key)); 
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Find"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an array 
            ///           of all the controls that match. 
            ///     </para>
            /// </devdoc>
            public MenuItem [] Find(string key, bool searchAllChildren) {

                if ((key == null) || (key.Length == 0)) {
                    throw new System.ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
                }

                
                ArrayList foundMenuItems =  FindInternal(key, searchAllChildren, this, new ArrayList());

                // Make this a stongly typed collection.
                MenuItem[] stronglyTypedfoundMenuItems = new MenuItem[foundMenuItems.Count]; 
                foundMenuItems.CopyTo(stronglyTypedfoundMenuItems, 0);

                return stronglyTypedfoundMenuItems;
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.FindInternal"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an array list
            ///           of all the controls that match. 
            ///     </para>
            /// </devdoc>
            /// <internalonly/>
            private ArrayList FindInternal(string key, bool searchAllChildren, MenuItemCollection menuItemsToLookIn, ArrayList foundMenuItems) {
                if ((menuItemsToLookIn == null) || (foundMenuItems == null)) {
                    return null;  // 
                }

                // Perform breadth first search - as it's likely people will want controls belonging
                // to the same parent close to each other.
                
                for (int i = 0; i < menuItemsToLookIn.Count; i++) {
                      if (menuItemsToLookIn[i] == null){
                          continue;
                      }
                      
                      if (WindowsFormsUtils.SafeCompareStrings(menuItemsToLookIn[i].Name, key, /* ignoreCase = */ true)) {
                           foundMenuItems.Add(menuItemsToLookIn[i]);
                      }
                }

                // Optional recurive search for controls in child collections.
                
                if (searchAllChildren){
                    for (int i = 0; i < menuItemsToLookIn.Count; i++) {    
                      if (menuItemsToLookIn[i] == null){
                          continue;
                      }
                        if ((menuItemsToLookIn[i].MenuItems != null) && menuItemsToLookIn[i].MenuItems.Count > 0){
                            // if it has a valid child collecion, append those results to our collection
                            foundMenuItems = FindInternal(key, searchAllChildren, menuItemsToLookIn[i].MenuItems, foundMenuItems);
                        }
                     }
                }
                return foundMenuItems;
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.IndexOf"]/*' />
            public int IndexOf(MenuItem value) {
                for(int index=0; index < Count; ++index) {
                    if (this[index] == value) {
                        return index;
                    } 
                }
                return -1;
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object value) {
                if (value is MenuItem) {
                    return IndexOf((MenuItem)value);
                }
                else {  
                    return -1;
                }
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.this"]/*' />
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

            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                if (value is MenuItem) {
                    Add(index, (MenuItem)value);                    
                }
                else {  
                    throw new ArgumentException(SR.MenuBadMenuItem,"value");
                }
            }
            

           /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.IsValidIndex"]/*' />
           /// <devdoc>
           ///     <para>Determines if the index is valid for the collection.</para>
           /// </devdoc>
           /// <internalonly/> 
           private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
           }


            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all existing MenuItems from this menu
            /// </devdoc>
            public virtual void Clear() {
                if (owner.ItemCount > 0) {
                    
                    for (int i = 0; i < owner.ItemCount; i++) {
                        owner.items[i].Menu = null;
                    }

                    owner._itemCount = 0;
                    owner.items = null;

                    owner.ItemsChanged(CHANGE_ITEMS);

                    if (owner is MenuItem) {
                        ((MenuItem)(owner)).UpdateMenuItem(true);
                    }
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.CopyTo"]/*' />
            public void CopyTo(Array dest, int index) {
                if (owner.ItemCount > 0) {
                    System.Array.Copy(owner.items, 0, dest, index, owner.ItemCount);
                }
            }
            
            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                return new WindowsFormsUtils.ArraySubsetEnumerator(owner.items, owner.ItemCount);
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     Removes the item at the specified index in this menu.  All subsequent
            ///     items are moved up one slot.
            /// </devdoc>
            public virtual void RemoveAt(int index) {
                if (index < 0 || index >= owner.ItemCount) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument,"index",(index).ToString(CultureInfo.CurrentCulture)));
                }

                MenuItem item = owner.items[index];
                item.Menu = null;
                owner._itemCount--;
                System.Array.Copy(owner.items, index + 1, owner.items, index, owner.ItemCount - index);
                owner.items[owner.ItemCount] = null;
                owner.ItemsChanged(CHANGE_ITEMS);

                //if the last item was removed, clear the collection
                //
                if (owner.ItemCount == 0) {
                    Clear();
                }
            
            }

           /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.RemoveByKey"]/*' />
           /// <devdoc>
           ///     <para>Removes the menu iteml with the specified key.</para>
           /// </devdoc>
           public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index); 
                 }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="Menu.MenuItemCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes the specified item from this menu.  All subsequent
            ///     items are moved down one slot.
            /// </devdoc>
            public virtual void Remove(MenuItem item) {
                if (item.Menu == owner) {
                    RemoveAt(item.Index);
                }
            }

            /// <include file='doc\Menu.uex' path='docs/doc[@for="MenuItemCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                if (value is MenuItem) {
                    Remove((MenuItem)value);
                }                
            }
        }
    }
}
