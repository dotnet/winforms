// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an individual item that is displayed within a <see cref='Menu'/>
    ///  or <see cref='Menu'/>.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultEvent(nameof(Click))]
    [DefaultProperty(nameof(Text))]
    public class MenuItem : Menu
    {
        private const int StateBarBreak = 0x00000020;
        private const int StateBreak = 0x00000040;
        private const int StateChecked = 0x00000008;
        private const int StateDefault = 0x00001000;
        private const int StateDisabled = 0x00000003;
        private const int StateRadioCheck = 0x00000200;
        private const int StateHidden = 0x00010000;
        private const int StateMdiList = 0x00020000;
        private const int StateCloneMask = 0x0003136B;
        private const int StateOwnerDraw = 0x00000100;
        private const int StateInMdiPopup = 0x00000200;
        private const int StateHiLite = 0x00000080;

        private bool _hasHandle;
        private MenuItemData _data;
        private int _dataVersion;
        private MenuItem _nextLinkedItem; // Next item linked to the same MenuItemData.

        // We need to store a table of all created menuitems, so that other objects
        // such as ContainerControl can get a reference to a particular menuitem,
        // given a unique ID.
        private static readonly Hashtable s_allCreatedMenuItems = new Hashtable();
        private const uint FirstUniqueID = 0xC0000000;
        private static long s_nextUniqueID = FirstUniqueID;
        private uint _uniqueID = 0;
        private IntPtr _msaaMenuInfoPtr = IntPtr.Zero;
        private bool _menuItemIsCreated = false;

#if DEBUG
        private string _debugText;
        private readonly int _creationNumber;
        private static int CreateCount;
#endif

        /// <summary>
        ///  Initializes a <see cref='MenuItem'/> with a blank caption.
        /// </summary>
        public MenuItem() : this(MenuMerge.Add, 0, 0, null, null, null, null, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='MenuItem'/> class
        ///  with a specified caption for the menu item.
        /// </summary>
        public MenuItem(string text) : this(MenuMerge.Add, 0, 0, text, null, null, null, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the class with a specified caption and event handler
        ///  for the menu item.
        /// </summary>
        public MenuItem(string text, EventHandler onClick) : this(MenuMerge.Add, 0, 0, text, onClick, null, null, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the class with a specified caption, event handler,
        ///  and associated shorcut key for the menu item.
        /// </summary>
        public MenuItem(string text, EventHandler onClick, Shortcut shortcut) : this(MenuMerge.Add, 0, shortcut, text, onClick, null, null, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the class with a specified caption and an array of
        ///  submenu items defined for the menu item.
        /// </summary>
        public MenuItem(string text, MenuItem[] items) : this(MenuMerge.Add, 0, 0, text, null, null, null, items)
        {
        }

        internal MenuItem(MenuItemData data) : base(null)
        {
            data.AddItem(this);

#if DEBUG
            _debugText = data._caption;
#endif
        }

        /// <summary>
        ///  Initializes a new instance of the class with a specified caption, defined
        ///  event-handlers for the Click, Select and Popup events, a shortcut key,
        ///  a merge type, and order specified for the menu item.
        /// </summary>
        public MenuItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut,
                        string text, EventHandler onClick, EventHandler onPopup,
                        EventHandler onSelect, MenuItem[] items) : base(items)
        {
            new MenuItemData(this, mergeType, mergeOrder, shortcut, true,
                             text, onClick, onPopup, onSelect, null, null);

#if DEBUG
            _debugText = text;
            _creationNumber = CreateCount++;
#endif
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the item is placed on a new line (for a
        ///  menu item added to a <see cref='MainMenu'/> object) or in a
        ///  new column (for a submenu or menu displayed in a <see cref='ContextMenu'/>).
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool BarBreak
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateBarBreak) != 0;
            }
            set
            {
                CheckIfDisposed();
                _data.SetState(StateBarBreak, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the item is placed on a new line (for a
        ///  menu item added to a <see cref='MainMenu'/> object) or in a
        ///  new column (for a submenu or menu displayed in a <see cref='ContextMenu'/>).
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Break
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateBreak) != 0;
            }
            set
            {
                CheckIfDisposed();
                _data.SetState(StateBreak, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether a checkmark appears beside the text of
        ///  the menu item.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.MenuItemCheckedDescr))]
        public bool Checked
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateChecked) != 0;
            }
            set
            {
                CheckIfDisposed();

                if (value && (ItemCount != 0 || Parent is MainMenu))
                {
                    throw new ArgumentException(SR.MenuItemInvalidCheckProperty, nameof(value));
                }

                _data.SetState(StateChecked, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the menu item is the default.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.MenuItemDefaultDescr))]
        public bool DefaultItem
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateDefault) != 0;
            }
            set
            {
                CheckIfDisposed();
                if (Parent != null)
                {
                    if (value)
                    {
                        UnsafeNativeMethods.SetMenuDefaultItem(new HandleRef(Parent, Parent.handle), MenuID, false);
                    }
                    else if (DefaultItem)
                    {
                        UnsafeNativeMethods.SetMenuDefaultItem(new HandleRef(Parent, Parent.handle), -1, false);
                    }
                }

                _data.SetState(StateDefault, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether code that you provide draws the menu
        ///  item or Windows draws the menu item.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.MenuItemOwnerDrawDescr))]
        public bool OwnerDraw
        {
            get
            {
                CheckIfDisposed();
                return ((_data.State & StateOwnerDraw) != 0);
            }
            set
            {
                CheckIfDisposed();
                _data.SetState(StateOwnerDraw, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the menu item is enabled.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.MenuItemEnabledDescr))]
        public bool Enabled
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateDisabled) == 0;
            }
            set
            {
                CheckIfDisposed();
                _data.SetState(StateDisabled, !value);
            }
        }

        /// <summary>
        ///  Gets or sets the menu item's position in its parent menu.
        /// </summary>
        [Browsable(false)]
        public int Index
        {
            get
            {
                if (Parent != null)
                {
                    for (int i = 0; i < Parent.ItemCount; i++)
                    {
                        if (Parent.items[i] == this)
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }
            set
            {
                int oldIndex = Index;
                if (oldIndex >= 0)
                {
                    if (value < 0 || value >= Parent.ItemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidArgument, nameof(Index), value));
                    }

                    if (value != oldIndex)
                    {
                        // The menu reverts to null when we're removed, so hold onto it in a
                        // local variable
                        Menu parent = Parent;
                        parent.MenuItems.RemoveAt(oldIndex);
                        parent.MenuItems.Add(value, this);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the menu item contains child menu items.
        /// </summary>
        [Browsable(false)]
        public override bool IsParent
        {
            get
            {
                if (_data != null && MdiList)
                {
                    for (int i = 0; i < ItemCount; i++)
                    {
                        if (!(items[i]._data.UserData is MdiListUserData))
                        {
                            return true;
                        }
                    }

                    if (FindMdiForms().Length > 0)
                    {
                        return true;
                    }

                    if (Parent != null && !(Parent is MenuItem))
                    {
                        return true;
                    }

                    return false;
                }

                return base.IsParent;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the menu item will be populated with a
        ///  list of the MDI child windows that are displayed within the associated form.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.MenuItemMDIListDescr))]
        public bool MdiList
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateMdiList) != 0;
            }
            set
            {
                CheckIfDisposed();
                _data.MdiList = value;
                CleanListItems(this);
            }
        }

        /// <summary>
        ///  Gets the Windows identifier for this menu item.
        /// </summary>
        protected int MenuID
        {
            get
            {
                CheckIfDisposed();
                return _data.GetMenuID();
            }
        }

        /// <summary>
        ///  Is this menu item currently selected (highlighted) by the user?
        /// </summary>
        internal bool Selected
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }

                var info = new NativeMethods.MENUITEMINFO_T
                {
                    cbSize = Marshal.SizeOf<NativeMethods.MENUITEMINFO_T>(),
                    fMask = NativeMethods.MIIM_STATE
                };
                UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, info);

                return (info.fState & StateHiLite) != 0;
            }
        }

        /// <summary>
        ///  Gets the zero-based index of this menu item in the parent menu, or -1 if this
        ///  menu item is not associated with a parent menu.
        /// </summary>
        internal int MenuIndex
        {
            get
            {
                if (Parent == null)
                {
                    return -1;
                }

                int count = UnsafeNativeMethods.GetMenuItemCount(new HandleRef(Parent, Parent.Handle));
                int id = MenuID;
                NativeMethods.MENUITEMINFO_T info = new NativeMethods.MENUITEMINFO_T
                {
                    cbSize = Marshal.SizeOf<NativeMethods.MENUITEMINFO_T>(),
                    fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_SUBMENU
                };

                for (int i = 0; i < count; i++)
                {
                    UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(Parent, Parent.handle), i, true, info);

                    // For sub menus, the handle is always valid.
                    // For items, however, it is always zero.
                    if ((info.hSubMenu == IntPtr.Zero || info.hSubMenu == Handle) && info.wID == id)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        ///  Gets or sets a value that indicates the behavior of this
        ///  menu item when its menu is merged with another.
        /// </summary>
        [DefaultValue(MenuMerge.Add)]
        [SRDescription(nameof(SR.MenuItemMergeTypeDescr))]
        public MenuMerge MergeType
        {
            get
            {
                CheckIfDisposed();
                return _data._mergeType;
            }
            set
            {
                CheckIfDisposed();
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)MenuMerge.Add, (int)MenuMerge.Remove))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MenuMerge));
                }

                _data.MergeType = value;
            }
        }

        /// <summary>
        ///  Gets or sets the relative position the menu item when its
        ///  menu is merged with another.
        /// </summary>
        [DefaultValue(0)]
        [SRDescription(nameof(SR.MenuItemMergeOrderDescr))]
        public int MergeOrder
        {
            get
            {
                CheckIfDisposed();
                return _data._mergeOrder;
            }
            set
            {
                CheckIfDisposed();
                _data.MergeOrder = value;
            }
        }

        /// <summary>
        ///  Retrieves the hotkey mnemonic that is associated with this menu item.
        ///  The mnemonic is the first character after an ampersand symbol in the menu's text
        ///  that is not itself an ampersand symbol. If no such mnemonic is defined this
        ///  will return zero.
        /// </summary>
        [Browsable(false)]
        public char Mnemonic
        {
            get
            {
                CheckIfDisposed();
                return _data.Mnemonic;
            }
        }

        /// <summary>
        ///  Gets the menu in which this menu item appears.
        /// </summary>
        [Browsable(false)]
        public Menu Parent { get; internal set; }

        /// <summary>
        ///  Gets or sets a value that indicates whether the menu item, if checked,
        ///  displays a radio-button mark instead of a check mark.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.MenuItemRadioCheckDescr))]
        public bool RadioCheck
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateRadioCheck) != 0;
            }
            set
            {
                CheckIfDisposed();
                _data.SetState(StateRadioCheck, value);
            }
        }

        internal override bool RenderIsRightToLeft => Parent != null && Parent.RenderIsRightToLeft;

        /// <summary>
        ///  Gets or sets the text of the menu item.
        /// </summary>
        [Localizable(true)]
        [SRDescription(nameof(SR.MenuItemTextDescr))]
        public string Text
        {
            get
            {
                CheckIfDisposed();
                return _data._caption;
            }
            set
            {
                CheckIfDisposed();
                _data.SetCaption(value);
            }
        }

        /// <summary>
        ///  Gets or sets the shortcut key associated with the menu item.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(Shortcut.None)]
        [SRDescription(nameof(SR.MenuItemShortCutDescr))]
        public Shortcut Shortcut
        {
            get
            {
                CheckIfDisposed();
                return _data._shortcut;
            }
            set
            {
                CheckIfDisposed();
                if (!Enum.IsDefined(typeof(Shortcut), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Shortcut));
                }

                _data._shortcut = value;
                UpdateMenuItem(force: true);
            }
        }

        /// <summary>
        ///  Gets or sets a value that indicates whether the shortcut key that is associated
        ///  with the menu item is displayed next to the menu item caption.
        /// </summary>
        [DefaultValue(true),
        Localizable(true)]
        [SRDescription(nameof(SR.MenuItemShowShortCutDescr))]
        public bool ShowShortcut
        {
            get
            {
                CheckIfDisposed();
                return _data._showShortcut;
            }
            set
            {
                CheckIfDisposed();
                if (value != _data._showShortcut)
                {
                    _data._showShortcut = value;
                    UpdateMenuItem(force: true);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value that indicates whether the menu item is visible on its
        ///  parent menu.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.MenuItemVisibleDescr))]
        public bool Visible
        {
            get
            {
                CheckIfDisposed();
                return (_data.State & StateHidden) == 0;
            }
            set
            {
                CheckIfDisposed();
                _data.Visible = value;
            }
        }

        /// <summary>
        ///  Occurs when the menu item is clicked or selected using a shortcut key defined
        ///  for the menu item.
        /// </summary>
        [SRDescription(nameof(SR.MenuItemOnClickDescr))]
        public event EventHandler Click
        {
            add
            {
                CheckIfDisposed();
                _data._onClick += value;
            }
            remove
            {
                CheckIfDisposed();
                _data._onClick -= value;
            }
        }

        /// <summary>
        ///  Occurs when when the property of a menu item is set to <see langword='true'/> and
        ///  a request is made to draw the menu item.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem
        {
            add
            {
                CheckIfDisposed();
                _data._onDrawItem += value;
            }
            remove
            {
                CheckIfDisposed();
                _data._onDrawItem -= value;
            }
        }

        /// <summary>
        ///  Occurs when when the menu needs to know the size of a menu item before drawing it.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.measureItemEventDescr))]
        public event MeasureItemEventHandler MeasureItem
        {
            add
            {
                CheckIfDisposed();
                _data._onMeasureItem += value;
            }
            remove
            {
                CheckIfDisposed();
                _data._onMeasureItem -= value;
            }
        }

        /// <summary>
        ///  Occurs before a menu item's list of menu items is displayed.
        /// </summary>
        [SRDescription(nameof(SR.MenuItemOnInitDescr))]
        public event EventHandler Popup
        {
            add
            {
                CheckIfDisposed();
                _data._onPopup += value;
            }
            remove
            {
                CheckIfDisposed();
                _data._onPopup -= value;
            }
        }

        /// <summary>
        ///  Occurs when the user hovers their mouse over a menu item or selects it with the
        ///  keyboard but has not activated it.
        /// </summary>
        [SRDescription(nameof(SR.MenuItemOnSelectDescr))]
        public event EventHandler Select
        {
            add
            {
                CheckIfDisposed();
                _data._onSelect += value;
            }
            remove
            {
                CheckIfDisposed();
                _data._onSelect -= value;
            }
        }

        private static void CleanListItems(MenuItem senderMenu)
        {
            // Remove dynamic items.
            for (int i = senderMenu.MenuItems.Count - 1; i >= 0; i--)
            {
                MenuItem item = senderMenu.MenuItems[i];
                if (item._data.UserData is MdiListUserData)
                {
                    item.Dispose();
                    continue;
                }
            }
        }

        /// <summary>
        ///  Creates and returns an identical copy of this menu item.
        /// </summary>
        public virtual MenuItem CloneMenu()
        {
            var newItem = new MenuItem();
            newItem.CloneMenu(this);
            return newItem;
        }

        /// <summary>
        ///  Creates a copy of the specified menu item.
        /// </summary>
        protected void CloneMenu(MenuItem itemSrc)
        {
            base.CloneMenu(itemSrc);
            int state = itemSrc._data.State;
            new MenuItemData(this,
                             itemSrc.MergeType, itemSrc.MergeOrder, itemSrc.Shortcut, itemSrc.ShowShortcut,
                             itemSrc.Text, itemSrc._data._onClick, itemSrc._data._onPopup, itemSrc._data._onSelect,
                             itemSrc._data._onDrawItem, itemSrc._data._onMeasureItem);
            _data.SetState(state & StateCloneMask, true);
        }

        internal virtual void CreateMenuItem()
        {
            if ((_data.State & StateHidden) == 0)
            {
                NativeMethods.MENUITEMINFO_T info = CreateMenuItemInfo();
                UnsafeNativeMethods.InsertMenuItem(new HandleRef(Parent, Parent.handle), -1, true, info);

                _hasHandle = info.hSubMenu != IntPtr.Zero;
                _dataVersion = _data._version;

                _menuItemIsCreated = true;
                if (RenderIsRightToLeft)
                {
                    Parent.UpdateRtl(true);
                }

#if DEBUG
                NativeMethods.MENUITEMINFO_T infoVerify = new NativeMethods.MENUITEMINFO_T
                {
                    cbSize = Marshal.SizeOf<NativeMethods.MENUITEMINFO_T>(),
                    fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE |
                                   NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE
                };
                UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, infoVerify);
#endif
            }
        }

        private NativeMethods.MENUITEMINFO_T CreateMenuItemInfo()
        {
            var info = new NativeMethods.MENUITEMINFO_T
            {
                fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE |
                         NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE | NativeMethods.MIIM_DATA,
                fType = _data.State & (StateBarBreak | StateBreak | StateRadioCheck | StateOwnerDraw)
            };

            // Top level menu items shouldn't have barbreak or break bits set on them.
            bool isTopLevel = Parent == GetMainMenu();

            if (_data._caption.Equals("-"))
            {
                if (isTopLevel)
                {
                    _data._caption = " ";
                    info.fType |= NativeMethods.MFT_MENUBREAK;
                }
                else
                {
                    info.fType |= NativeMethods.MFT_SEPARATOR;
                }
            }

            info.fState = _data.State & (StateChecked | StateDefault | StateDisabled);

            info.wID = MenuID;
            if (IsParent)
            {
                info.hSubMenu = Handle;
            }

            info.hbmpChecked = IntPtr.Zero;
            info.hbmpUnchecked = IntPtr.Zero;

            // Assign a unique ID to this menu item object.
            // The ID is stored in the dwItemData of the corresponding Win32 menu item, so
            // that when we get Win32 messages about the item later, we can delegate to the
            // original object menu item object. A static hash table is used to map IDs to
            // menu item objects.
            if (_uniqueID == 0)
            {
                lock (s_allCreatedMenuItems)
                {
                    _uniqueID = (uint)Interlocked.Increment(ref s_nextUniqueID);
                    Debug.Assert(_uniqueID >= FirstUniqueID); // ...check for ID range exhaustion (unlikely!)
                    // We add a weak ref wrapping a MenuItem to the static hash table, as
                    // supposed to adding the item ref itself, to allow the item to be finalized
                    // in case it is not disposed and no longer referenced anywhere else, hence
                    // preventing leaks.
                    s_allCreatedMenuItems.Add(_uniqueID, new WeakReference(this));
                }
            }

            if (IntPtr.Size == 4)
            {
                // Store the unique ID in the dwItemData..
                // For simple menu items, we can just put the unique ID in the dwItemData.
                // But for owner-draw items, we need to point the dwItemData at an MSAAMENUINFO
                // structure so that MSAA can get the item text.
                // To allow us to reliably distinguish between IDs and structure pointers later
                // on, we keep IDs in the 0xC0000000-0xFFFFFFFF range. This is the top 1Gb of
                // unmananged process memory, where an app's heap allocations should never come
                // from. So that we can still get the ID from the dwItemData for an owner-draw
                // item later on, a copy of the ID is tacked onto the end of the MSAAMENUINFO
                // structure.
                if (_data.OwnerDraw)
                {
                    info.dwItemData = AllocMsaaMenuInfo();
                }
                else
                {
                    info.dwItemData = (IntPtr)unchecked((int)_uniqueID);
                }
            }
            else
            {
                // On Win64, there are no reserved address ranges we can use for menu item IDs. So instead we will
                // have to allocate an MSAMENUINFO heap structure for all menu items, not just owner-drawn ones.
                info.dwItemData = AllocMsaaMenuInfo();
            }

            // We won't render the shortcut if: 1) it's not set, 2) we're a parent, 3) we're toplevel
            if (_data._showShortcut && _data._shortcut != 0 && !IsParent && !isTopLevel)
            {
                info.dwTypeData = _data._caption + "\t" + TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString((Keys)(int)_data._shortcut);
            }
            else
            {
                // Windows issue: Items with empty captions sometimes block keyboard
                // access to other items in same menu.
                info.dwTypeData = (_data._caption.Length == 0 ? " " : _data._caption);
            }
            info.cch = 0;

            return info;
        }

        /// <summary>
        ///  Disposes the <see cref='MenuItem'/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.MenuItems.Remove(this);
                _data?.RemoveItem(this);
                lock (s_allCreatedMenuItems)
                {
                    s_allCreatedMenuItems.Remove(_uniqueID);
                }

                _uniqueID = 0;

            }

            FreeMsaaMenuInfo();
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Given a unique menu item ID, find the corresponding MenuItem
        ///  object, using the master lookup table of all created MenuItems.
        /// </summary>
        internal static MenuItem GetMenuItemFromUniqueID(uint uniqueID)
        {
            WeakReference weakRef = (WeakReference)s_allCreatedMenuItems[uniqueID];
            if (weakRef != null && weakRef.IsAlive)
            {
                return (MenuItem)weakRef.Target;
            }
            Debug.Fail("Weakref for menu item has expired or has been removed!  Who is trying to access this ID?");
            return null;
        }

        /// <summary>
        ///  Given the "item data" value of a Win32 menu item, find the corresponding MenuItem object (using
        ///  the master lookup table of all created MenuItems). The item data may be either the unique menu
        ///  item ID, or a pointer to an MSAAMENUINFO structure with a copy of the unique ID tacked to the end.
        ///  To reliably tell IDs and structure addresses apart, IDs live in the 0xC0000000-0xFFFFFFFF range.
        ///  This is the top 1Gb of unmananged process memory, where an app's heap allocations should never be.
        /// </summary>
        internal static MenuItem GetMenuItemFromItemData(IntPtr itemData)
        {
            uint uniqueID = (uint)(ulong)itemData;
            if (uniqueID == 0)
            {
                return null;
            }

            if (IntPtr.Size == 4)
            {
                if (uniqueID < FirstUniqueID)
                {
                    MsaaMenuInfoWithId msaaMenuInfo = Marshal.PtrToStructure<MsaaMenuInfoWithId>(itemData);
                    uniqueID = msaaMenuInfo._uniqueID;
                }
            }
            else
            {
                // Its always a pointer on Win64 (see CreateMenuItemInfo)
                MsaaMenuInfoWithId msaaMenuInfo = Marshal.PtrToStructure<MsaaMenuInfoWithId>(itemData);
                uniqueID = msaaMenuInfo._uniqueID;
            }

            return GetMenuItemFromUniqueID(uniqueID);
        }

        /// <summary>
        ///  MsaaMenuInfoWithId is an MSAAMENUINFO structure with a menu item ID field tacked onto the
        ///  end. This allows us to pass the data we need to Win32 / MSAA, and still be able to get the ID
        ///  out again later on, so we can delegate Win32 menu messages back to the correct MenuItem object.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MsaaMenuInfoWithId
        {
            public readonly NativeMethods.MSAAMENUINFO _msaaMenuInfo;
            public readonly uint _uniqueID;

            public MsaaMenuInfoWithId(string text, uint uniqueID)
            {
                _msaaMenuInfo = new NativeMethods.MSAAMENUINFO(text);
                _uniqueID = uniqueID;
            }
        }

        /// <summary>
        ///  Creates an MSAAMENUINFO structure (in the unmanaged heap) based on the current state
        ///  of this MenuItem object. Address of this structure is cached in the object so we can
        ///  free it later on using FreeMsaaMenuInfo(). If structure has already been allocated,
        ///  it is destroyed and a new one created.
        /// </summary>
        private IntPtr AllocMsaaMenuInfo()
        {
            FreeMsaaMenuInfo();
            _msaaMenuInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MsaaMenuInfoWithId>());

            if (IntPtr.Size == 4)
            {
                // We only check this on Win32, irrelevant on Win64 (see CreateMenuItemInfo)
                // Check for incursion into menu item ID range (unlikely!)
                Debug.Assert(((uint)(ulong)_msaaMenuInfoPtr) < FirstUniqueID);
            }

            MsaaMenuInfoWithId msaaMenuInfoStruct = new MsaaMenuInfoWithId(_data._caption, _uniqueID);
            Marshal.StructureToPtr(msaaMenuInfoStruct, _msaaMenuInfoPtr, false);
            Debug.Assert(_msaaMenuInfoPtr != IntPtr.Zero);
            return _msaaMenuInfoPtr;
        }

        /// <summary>
        ///  Frees the MSAAMENUINFO structure (in the unmanaged heap) for the current MenuObject
        ///  object, if one has previously been allocated. Takes care to free sub-structures too,
        ///  to avoid leaks!
        /// <summary>
        private void FreeMsaaMenuInfo()
        {
            if (_msaaMenuInfoPtr != IntPtr.Zero)
            {
                Marshal.DestroyStructure(_msaaMenuInfoPtr, typeof(MsaaMenuInfoWithId));
                Marshal.FreeHGlobal(_msaaMenuInfoPtr);
                _msaaMenuInfoPtr = IntPtr.Zero;
            }
        }

        internal override void ItemsChanged(int change)
        {
            base.ItemsChanged(change);

            if (change == CHANGE_ITEMS)
            {
                // when the menu collection changes deal with it locally
                Debug.Assert(!created, "base.ItemsChanged should have wiped out our handles");
                if (Parent != null && Parent.created)
                {
                    UpdateMenuItem(force: true);
                    CreateMenuItems();
                }
            }
            else
            {
                if (!_hasHandle && IsParent)
                {
                    UpdateMenuItem(force: true);
                }

                MainMenu main = GetMainMenu();
                if (main != null && ((_data.State & StateInMdiPopup) == 0))
                {
                    main.ItemsChanged(change, this);
                }
            }
        }

        internal void ItemsChanged(int change, MenuItem item)
        {
            if (change == CHANGE_ITEMADDED &&
                _data != null &&
                _data.baseItem != null &&
                _data.baseItem.MenuItems.Contains(item))
            {
                if (Parent != null && Parent.created)
                {
                    UpdateMenuItem(force: true);
                    CreateMenuItems();
                }
                else if (_data != null)
                {
                    MenuItem currentMenuItem = _data.firstItem;
                    while (currentMenuItem != null)
                    {
                        if (currentMenuItem.created)
                        {
                            MenuItem newItem = item.CloneMenu();
                            item._data.AddItem(newItem);
                            currentMenuItem.MenuItems.Add(newItem);
                            break;
                        }
                        currentMenuItem = currentMenuItem._nextLinkedItem;
                    }
                }
            }
        }

        internal Form[] FindMdiForms()
        {
            Form[] forms = null;
            MainMenu main = GetMainMenu();
            Form menuForm = null;
            if (main != null)
            {
                menuForm = main.GetFormUnsafe();
            }
            if (menuForm != null)
            {
                forms = menuForm.MdiChildren;
            }
            if (forms == null)
            {
                forms = Array.Empty<Form>();
            }

            return forms;
        }

        /// <summary>
        ///  See the similar code in MdiWindowListStrip.PopulateItems, which is
        ///  unfortunately just different enough in its working environment that we
        ///  can't readily combine the two. But if you're fixing something here, chances
        ///  are that the same issue will need scrutiny over there.
        ///</summary>
// "-" is OK
        private void PopulateMdiList()
        {
            MenuItem senderMenu = this;
            _data.SetState(StateInMdiPopup, true);
            try
            {
                CleanListItems(this);

                // Add new items
                Form[] forms = FindMdiForms();
                if (forms != null && forms.Length > 0)
                {

                    Form activeMdiChild = GetMainMenu().GetFormUnsafe().ActiveMdiChild;

                    if (senderMenu.MenuItems.Count > 0)
                    {
                        MenuItem sep = (MenuItem)Activator.CreateInstance(GetType());
                        sep._data.UserData = new MdiListUserData();
                        sep.Text = "-";
                        senderMenu.MenuItems.Add(sep);
                    }

                    // Build a list of child windows to be displayed in
                    // the MDIList menu item...
                    // Show the first maxMenuForms visible elements of forms[] as Window menu items, except:
                    // Always show the active form, even if it's not in the first maxMenuForms visible elements of forms[].
                    // If the active form isn't in the first maxMenuForms forms, then show the first maxMenuForms-1 elements
                    // in forms[], and make the active form the last one on the menu.
                    // Don't count nonvisible forms against the limit on Window menu items.

                    const int MaxMenuForms = 9; // Max number of Window menu items for forms
                    int visibleChildren = 0;    // the number of visible child forms (so we know to show More Windows...)
                    int accel = 1;              // prefix the form name with this digit, underlined, as an accelerator
                    int formsAddedToMenu = 0;
                    bool activeFormAdded = false;
                    for (int i = 0; i < forms.Length; i++)
                    {
                        if (forms[i].Visible)
                        {
                            visibleChildren++;
                            if ((activeFormAdded && (formsAddedToMenu < MaxMenuForms)) ||  // don't exceed max
                                (!activeFormAdded && (formsAddedToMenu < (MaxMenuForms - 1)) ||  // save room for active if it's not in yet
                                (forms[i].Equals(activeMdiChild))))
                            {
                                // there's always room for activeMdiChild
                                MenuItem windowItem = (MenuItem)Activator.CreateInstance(GetType());
                                windowItem._data.UserData = new MdiListFormData(this, i);

                                if (forms[i].Equals(activeMdiChild))
                                {
                                    windowItem.Checked = true;
                                    activeFormAdded = true;
                                }

                                windowItem.Text = string.Format(CultureInfo.CurrentUICulture, "&{0} {1}", accel, forms[i].Text);
                                accel++;
                                formsAddedToMenu++;
                                senderMenu.MenuItems.Add(windowItem);
                            }
                        }
                    }

                    // Display the More Windows menu option when there are more than 9 MDI
                    // Child menu items to be displayed. This is necessary because we're managing our own
                    // MDI lists, rather than letting Windows do this for us.
                    if (visibleChildren > MaxMenuForms)
                    {
                        MenuItem moreWindows = (MenuItem)Activator.CreateInstance(GetType());
                        moreWindows._data.UserData = new MdiListMoreWindowsData(this);
                        moreWindows.Text = SR.MDIMenuMoreWindows;
                        senderMenu.MenuItems.Add(moreWindows);
                    }
                }
            }
            finally
            {
                _data.SetState(StateInMdiPopup, false);
            }
        }

        /// <summary>
        ///  Merges this menu item with another menu item and returns the resulting merged
        /// <see cref='MenuItem'/>.
        /// </summary>
        public virtual MenuItem MergeMenu()
        {
            CheckIfDisposed();

            MenuItem newItem = (MenuItem)Activator.CreateInstance(GetType());
            _data.AddItem(newItem);
            newItem.MergeMenu(this);
            return newItem;
        }

        /// <summary>
        ///  Merges another menu item with this menu item.
        /// </summary>
        public void MergeMenu(MenuItem itemSrc)
        {
            base.MergeMenu(itemSrc);
            itemSrc._data.AddItem(this);
        }

        /// <summary>
        ///  Raises the <see cref='Click'/> event.
        /// </summary>
        protected virtual void OnClick(EventArgs e)
        {
            CheckIfDisposed();

            if (_data.UserData is MdiListUserData)
            {
                ((MdiListUserData)_data.UserData).OnClick(e);
            }
            else if (_data.baseItem != this)
            {
                _data.baseItem.OnClick(e);
            }
            else
            {
                _data._onClick?.Invoke(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='DrawItem'/> event.
        /// </summary>
        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            CheckIfDisposed();

            if (_data.baseItem != this)
            {
                _data.baseItem.OnDrawItem(e);
            }
            else
            {
                _data._onDrawItem?.Invoke(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='MeasureItem'/> event.
        /// </summary>
        protected virtual void OnMeasureItem(MeasureItemEventArgs e)
        {
            CheckIfDisposed();

            if (_data.baseItem != this)
            {
                _data.baseItem.OnMeasureItem(e);
            }
            else
            {
                _data._onMeasureItem?.Invoke(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Popup'/> event.
        /// </summary>
        protected virtual void OnPopup(EventArgs e)
        {
            CheckIfDisposed();

            bool recreate = false;
            for (int i = 0; i < ItemCount; i++)
            {
                if (items[i].MdiList)
                {
                    recreate = true;
                    items[i].UpdateMenuItem(force: true);
                }
            }
            if (recreate || (_hasHandle && !IsParent))
            {
                UpdateMenuItem(force: true);
            }

            if (_data.baseItem != this)
            {
                _data.baseItem.OnPopup(e);
            }
            else
            {
                _data._onPopup?.Invoke(this, e);
            }

            // Update any subitem states that got changed in the event
            for (int i = 0; i < ItemCount; i++)
            {
                MenuItem item = items[i];
                if (item._dataVersion != item._data._version)
                {
                    item.UpdateMenuItem(force: true);
                }
            }

            if (MdiList)
            {
                PopulateMdiList();
            }
        }

        /// <summary>
        ///  Raises the <see cref='Select'/> event.
        /// </summary>
        protected virtual void OnSelect(EventArgs e)
        {
            CheckIfDisposed();

            if (_data.baseItem != this)
            {
                _data.baseItem.OnSelect(e);
            }
            else
            {
                _data._onSelect?.Invoke(this, e);
            }
        }

        protected internal virtual void OnInitMenuPopup(EventArgs e) => OnPopup(e);

        /// <summary>
        ///  Generates a <see cref='Control.Click'/> event for the MenuItem,
        ///  simulating a click by a user.
        /// </summary>
        public void PerformClick() => OnClick(EventArgs.Empty);

        /// <summary>
        ///  Raises the <see cref='Select'/> event for this menu item.
        /// </summary>
        public virtual void PerformSelect() => OnSelect(EventArgs.Empty);

        internal virtual bool ShortcutClick()
        {
            if (Parent is MenuItem parent)
            {
                if (!parent.ShortcutClick() || Parent != parent)
                {
                    return false;
                }
            }
            if ((_data.State & StateDisabled) != 0)
            {
                return false;
            }
            if (ItemCount > 0)
            {
                OnPopup(EventArgs.Empty);
            }
            else
            {
                OnClick(EventArgs.Empty);
            }

            return true;
        }

        public override string ToString()
        {
            string s = base.ToString();
            string menuItemText = _data?._caption ?? string.Empty;
            ;
            return s + ", Text: " + menuItemText;
        }

        internal void UpdateItemRtl(bool setRightToLeftBit)
        {
            if (!_menuItemIsCreated)
            {
                return;
            }

            var info = new NativeMethods.MENUITEMINFO_T
            {
                fMask = NativeMethods.MIIM_TYPE | NativeMethods.MIIM_STATE | NativeMethods.MIIM_SUBMENU,
                dwTypeData = new string('\0', Text.Length + 2),
                cbSize = Marshal.SizeOf<NativeMethods.MENUITEMINFO_T>()
            };
            info.cch = info.dwTypeData.Length - 1;
            UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, info);
            if (setRightToLeftBit)
            {
                info.fType |= NativeMethods.MFT_RIGHTJUSTIFY | NativeMethods.MFT_RIGHTORDER;
            }
            else
            {
                info.fType &= ~(NativeMethods.MFT_RIGHTJUSTIFY | NativeMethods.MFT_RIGHTORDER);
            }

            UnsafeNativeMethods.SetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, info);
        }

        internal void UpdateMenuItem(bool force)
        {
            if (Parent == null || !Parent.created)
            {
                return;
            }

            if (force || Parent is MainMenu || Parent is ContextMenu)
            {
                NativeMethods.MENUITEMINFO_T info = CreateMenuItemInfo();
                UnsafeNativeMethods.SetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, info);
#if DEBUG
                var infoVerify = new NativeMethods.MENUITEMINFO_T
                {
                    cbSize = Marshal.SizeOf<NativeMethods.MENUITEMINFO_T>(),
                    fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE |
                                   NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE
                };
                UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, infoVerify);
#endif

                if (_hasHandle && info.hSubMenu == IntPtr.Zero)
                {
                    ClearHandles();
                }

                _hasHandle = info.hSubMenu != IntPtr.Zero;
                _dataVersion = _data._version;
                if (Parent is MainMenu mainMenu)
                {
                    Form f = mainMenu.GetFormUnsafe();
                    if (f != null)
                    {
                        SafeNativeMethods.DrawMenuBar(new HandleRef(f, f.Handle));
                    }
                }
            }
        }

        internal void WmDrawItem(ref Message m)
        {
            // Handles the OnDrawItem message sent from ContainerControl
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));
            Debug.WriteLineIf(Control.s_paletteTracing.TraceVerbose, Handle + ": Force set palette in MenuItem drawitem");
            IntPtr oldPal = Control.SetUpPalette(dis.hDC, false /*force*/, false);
            try
            {
                Graphics g = Graphics.FromHdcInternal(dis.hDC);
                try
                {
                    OnDrawItem(new DrawItemEventArgs(g, SystemInformation.MenuFont, Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom), Index, (DrawItemState)dis.itemState));
                }
                finally
                {
                    g.Dispose();
                }
            }
            finally
            {
                if (oldPal != IntPtr.Zero)
                {
                    SafeNativeMethods.SelectPalette(new HandleRef(null, dis.hDC), new HandleRef(null, oldPal), 0);
                }
            }

            m.Result = (IntPtr)1;
        }

        internal void WmMeasureItem(ref Message m)
        {
            // Handles the OnMeasureItem message sent from ContainerControl

            // Obtain the measure item struct
            NativeMethods.MEASUREITEMSTRUCT mis = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));

            // The OnMeasureItem handler now determines the height and width of the item
            using ScreenDC screendc = ScreenDC.Create();
            Graphics graphics = Graphics.FromHdcInternal(screendc);
            MeasureItemEventArgs mie = new MeasureItemEventArgs(graphics, Index);
            using (graphics)
            {
                OnMeasureItem(mie);
            }

            // Update the measure item struct with the new width and height
            mis.itemHeight = mie.ItemHeight;
            mis.itemWidth = mie.ItemWidth;
            Marshal.StructureToPtr(mis, m.LParam, false);

            m.Result = (IntPtr)1;
        }

        private void CheckIfDisposed()
        {
            if (_data == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        internal class MenuItemData : ICommandExecutor
        {
            internal MenuItem baseItem;
            internal MenuItem firstItem;

            private int _state;
            internal int _version;
            internal MenuMerge _mergeType;
            internal int _mergeOrder;
            internal string _caption;
            internal short _mnemonic;
            internal Shortcut _shortcut;
            internal bool _showShortcut;
            internal EventHandler _onClick;
            internal EventHandler _onPopup;
            internal EventHandler _onSelect;
            internal DrawItemEventHandler _onDrawItem;
            internal MeasureItemEventHandler _onMeasureItem;

            private Command _cmd;

            internal MenuItemData(MenuItem baseItem, MenuMerge mergeType, int mergeOrder, Shortcut shortcut, bool showShortcut,
                                  string caption, EventHandler onClick, EventHandler onPopup, EventHandler onSelect,
                                  DrawItemEventHandler onDrawItem, MeasureItemEventHandler onMeasureItem)
            {
                AddItem(baseItem);
                _mergeType = mergeType;
                _mergeOrder = mergeOrder;
                _shortcut = shortcut;
                _showShortcut = showShortcut;
                _caption = caption ?? string.Empty;
                _onClick = onClick;
                _onPopup = onPopup;
                _onSelect = onSelect;
                _onDrawItem = onDrawItem;
                _onMeasureItem = onMeasureItem;
                _version = 1;
                _mnemonic = -1;
            }

            internal bool OwnerDraw
            {
                get => ((State & StateOwnerDraw) != 0);
                set => SetState(StateOwnerDraw, value);
            }

            internal bool MdiList
            {
                get => ((State & StateMdiList) == StateMdiList);
                set
                {
                    if (((_state & StateMdiList) != 0) != value)
                    {
                        SetState(StateMdiList, value);
                        for (MenuItem item = firstItem; item != null; item = item._nextLinkedItem)
                        {
                            item.ItemsChanged(Menu.CHANGE_MDI);
                        }
                    }
                }
            }

            internal MenuMerge MergeType
            {
                get => _mergeType;
                set
                {
                    if (_mergeType != value)
                    {
                        _mergeType = value;
                        ItemsChanged(Menu.CHANGE_MERGE);
                    }
                }
            }

            internal int MergeOrder
            {
                get => _mergeOrder;
                set
                {
                    if (_mergeOrder != value)
                    {
                        _mergeOrder = value;
                        ItemsChanged(Menu.CHANGE_MERGE);
                    }
                }
            }

            internal char Mnemonic
            {
                get
                {
                    if (_mnemonic == -1)
                    {
                        _mnemonic = (short)WindowsFormsUtils.GetMnemonic(_caption, true);
                    }

                    return (char)_mnemonic;
                }
            }

            internal int State => _state;

            internal bool Visible
            {
                get => (_state & MenuItem.StateHidden) == 0;
                set
                {
                    if (((_state & MenuItem.StateHidden) == 0) != value)
                    {
                        _state = value ? _state & ~MenuItem.StateHidden : _state | MenuItem.StateHidden;
                        ItemsChanged(Menu.CHANGE_VISIBLE);
                    }
                }
            }

            internal object UserData { get; set; }

            internal void AddItem(MenuItem item)
            {
                if (item._data != this)
                {
                    item._data?.RemoveItem(item);

                    item._nextLinkedItem = firstItem;
                    firstItem = item;
                    if (baseItem == null)
                    {
                        baseItem = item;
                    }

                    item._data = this;
                    item._dataVersion = 0;
                    item.UpdateMenuItem(false);
                }
            }

            public void Execute()
            {
                baseItem?.OnClick(EventArgs.Empty);
            }

            internal int GetMenuID()
            {
                if (_cmd == null)
                {
                    _cmd = new Command(this);
                }

                return _cmd.ID;
            }

            internal void ItemsChanged(int change)
            {
                for (MenuItem item = firstItem; item != null; item = item._nextLinkedItem)
                {
                    item.Parent?.ItemsChanged(change);
                }
            }

            internal void RemoveItem(MenuItem item)
            {
                Debug.Assert(item._data == this, "bad item passed to MenuItemData.removeItem");

                if (item == firstItem)
                {
                    firstItem = item._nextLinkedItem;
                }
                else
                {
                    MenuItem itemT;
                    for (itemT = firstItem; item != itemT._nextLinkedItem;)
                    {
                        itemT = itemT._nextLinkedItem;
                    }

                    itemT._nextLinkedItem = item._nextLinkedItem;
                }
                item._nextLinkedItem = null;
                item._data = null;
                item._dataVersion = 0;

                if (item == baseItem)
                {
                    baseItem = firstItem;
                }

                if (firstItem == null)
                {
                    // No longer needed. Toss all references and the Command object.
                    Debug.Assert(baseItem == null, "why isn't baseItem null?");
                    _onClick = null;
                    _onPopup = null;
                    _onSelect = null;
                    _onDrawItem = null;
                    _onMeasureItem = null;
                    if (_cmd != null)
                    {
                        _cmd.Dispose();
                        _cmd = null;
                    }
                }
            }

            internal void SetCaption(string value)
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!_caption.Equals(value))
                {
                    _caption = value;
                    UpdateMenuItems();
                }

#if DEBUG
                if (value.Length > 0)
                {
                    baseItem._debugText = value;
                }
#endif
            }

            internal void SetState(int flag, bool value)
            {
                if (((_state & flag) != 0) != value)
                {
                    _state = value ? _state | flag : _state & ~flag;
                    UpdateMenuItems();
                }
            }

            internal void UpdateMenuItems()
            {
                _version++;
                for (MenuItem item = firstItem; item != null; item = item._nextLinkedItem)
                {
                    item.UpdateMenuItem(force: true);
                }
            }

        }

        private class MdiListUserData
        {
            public virtual void OnClick(EventArgs e)
            {
            }
        }

        private class MdiListFormData : MdiListUserData
        {
            private readonly MenuItem _parent;
            private readonly int _boundIndex;

            public MdiListFormData(MenuItem parentItem, int boundFormIndex)
            {
                _boundIndex = boundFormIndex;
                _parent = parentItem;
            }

            public override void OnClick(EventArgs e)
            {
                if (_boundIndex != -1)
                {
                    Form[] forms = _parent.FindMdiForms();
                    Debug.Assert(forms != null, "Didn't get a list of the MDI Forms.");

                    if (forms != null && forms.Length > _boundIndex)
                    {
                        Form boundForm = forms[_boundIndex];
                        boundForm.Activate();
                        if (boundForm.ActiveControl != null && !boundForm.ActiveControl.Focused)
                        {
                            boundForm.ActiveControl.Focus();
                        }
                    }
                }
            }
        }

        private class MdiListMoreWindowsData : MdiListUserData
        {
            private readonly MenuItem _parent;

            public MdiListMoreWindowsData(MenuItem parent)
            {
                _parent = parent;
            }

            public override void OnClick(EventArgs e)
            {
                Form[] forms = _parent.FindMdiForms();
                Debug.Assert(forms != null, "Didn't get a list of the MDI Forms.");
                Form active = _parent.GetMainMenu().GetFormUnsafe().ActiveMdiChild;
                Debug.Assert(active != null, "Didn't get the active MDI child");
                if (forms != null && forms.Length > 0 && active != null)
                {
                    using (var dialog = new MdiWindowDialog())
                    {
                        dialog.SetItems(active, forms);
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            dialog.ActiveChildForm.Activate();
                            if (dialog.ActiveChildForm.ActiveControl != null && !dialog.ActiveChildForm.ActiveControl.Focused)
                            {
                                dialog.ActiveChildForm.ActiveControl.Focus();
                            }
                        }
                    }
                }
            }
        }
    }
}

