// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
#nullable disable
[Obsolete("MenuItem has been deprecated. Use ToolStripMenuItem instead.")]
public class MenuItem : Menu
{
    /// <summary>
    ///  Initializes a <see cref='MenuItem'/> with a blank caption.
    /// </summary>
    public MenuItem() : this(MenuMerge.Add, 0, 0, null, null, null, null, null)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='MenuItem'/> class
    ///  with a specified caption for the menu item.
    /// </summary>
    public MenuItem(string text) : this(MenuMerge.Add, 0, 0, text, null, null, null, null)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Initializes a new instance of the class with a specified caption and event handler
    ///  for the menu item.
    /// </summary>
    public MenuItem(string text, EventHandler onClick) : this(MenuMerge.Add, 0, 0, text, onClick, null, null, null)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Initializes a new instance of the class with a specified caption, event handler,
    ///  and associated shorcut key for the menu item.
    /// </summary>
    public MenuItem(string text, EventHandler onClick, Shortcut shortcut) : this(MenuMerge.Add, 0, shortcut, text, onClick, null, null, null)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Initializes a new instance of the class with a specified caption and an array of
    ///  submenu items defined for the menu item.
    /// </summary>
    public MenuItem(string text, MenuItem[] items) : this(MenuMerge.Add, 0, 0, text, null, null, null, items)
    {
        throw new PlatformNotSupportedException();
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
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the item is placed on a new line (for a
    ///  menu item added to a <see cref='MainMenu'/> object) or in a
    ///  new column (for a submenu or menu displayed in a <see cref='ContextMenu'/>).
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(false)]
    public bool BarBreak
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the item is placed on a new line (for a
    ///  menu item added to a <see cref='MainMenu'/> object) or in a
    ///  new column (for a submenu or menu displayed in a <see cref='ContextMenu'/>).
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(false)]
    public bool Break
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether a checkmark appears beside the text of
    ///  the menu item.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    [SRDescription("Indicates whether the item is checked.")]
    public bool Checked
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the menu item is the default.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    [SRDescription("Indicates whether the item is the default item.")]
    public bool DefaultItem
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether code that you provide draws the menu
    ///  item or Windows draws the menu item.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription("Indicates if Windows will draw the menu item or if the user will handle the painting.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool OwnerDraw
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the menu item is enabled.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(true)]
    [SRDescription("Indicates whether the item is enabled.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Enabled
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets the menu item's position in its parent menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int Index
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets a value indicating whether the menu item contains child menu items.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override bool IsParent
    {
        get => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the menu item will be populated with a
    ///  list of the MDI child windows that are displayed within the associated form.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription("Determines whether the MDI child window list is appended to this item.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool MdiList
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value that indicates the behavior of this
    ///  menu item when its menu is merged with another.
    /// </summary>
    [DefaultValue(MenuMerge.Add)]
    [SRDescription("Determines how the item is handled when menus are merged.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public MenuMerge MergeType
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets the relative position the menu item when its
    ///  menu is merged with another.
    /// </summary>
    [DefaultValue(0)]
    [SRDescription("Determines the merge order of the item.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int MergeOrder
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Retrieves the hotkey mnemonic that is associated with this menu item.
    ///  The mnemonic is the first character after an ampersand symbol in the menu's text
    ///  that is not itself an ampersand symbol. If no such mnemonic is defined this
    ///  will return zero.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public char Mnemonic => '\0';

    /// <summary>
    ///  Gets the menu in which this menu item appears.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Menu Parent
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value that indicates whether the menu item, if checked,
    ///  displays a radio-button mark instead of a check mark.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription("If the item is checked, this value will determine whether the check style is a radio button.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool RadioCheck
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets the text of the menu item.
    /// </summary>
    [Localizable(true)]
    [SRDescription("The caption displayed by the item.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Text
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets the shortcut key associated with the menu item.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(Shortcut.None)]
    [SRDescription(nameof(SR.MenuItemShortCutDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Shortcut Shortcut
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value that indicates whether the shortcut key that is associated
    ///  with the menu item is displayed next to the menu item caption.
    /// </summary>
    [DefaultValue(true), Localizable(true)]
    [SRDescription(nameof(SR.MenuItemShowShortCutDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowShortcut
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets a value that indicates whether the menu item is visible on its
    ///  parent menu.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(true)]
    [SRDescription("Indicates whether the item is visible.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Visible
    {
        get => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs when the menu item is clicked or selected using a shortcut key defined
    ///  for the menu item.
    /// </summary>
    [SRDescription("Occurs when the menu item is selected.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Click
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs when when the property of a menu item is set to <see langword='true'/> and
    ///  a request is made to draw the menu item.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.drawItemEventDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event DrawItemEventHandler DrawItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs when when the menu needs to know the size of a menu item before drawing it.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.measureItemEventDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event MeasureItemEventHandler MeasureItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs before a menu item's list of menu items is displayed.
    /// </summary>
    [SRDescription("Occurs before the containing menu is displayed.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Popup
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs when the user hovers their mouse over a menu item or selects it with the
    ///  keyboard but has not activated it.
    /// </summary>
    [SRDescription("Occurs when the menu item is selected.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Select
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Creates and returns an identical copy of this menu item.
    /// </summary>
    public virtual MenuItem CloneMenu()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Merges this menu item with another menu item and returns the resulting merged
    /// <see cref='MenuItem'/>.
    /// </summary>
    public virtual MenuItem MergeMenu()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Merges another menu item with this menu item.
    /// </summary>
    public void MergeMenu(MenuItem itemSrc)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Generates a <see cref='Control.Click'/> event for the MenuItem,
    ///  simulating a click by a user.
    /// </summary>
    public void PerformClick()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Raises the <see cref='Select'/> event for this menu item.
    /// </summary>
    public virtual void PerformSelect()
    {
        throw new PlatformNotSupportedException();
    }

    public override string ToString()
    {
        throw new PlatformNotSupportedException();
    }

    [Obsolete("MenuItemData has been deprecated.")]
    internal class MenuItemData : ICommandExecutor
    {
        internal MenuItem baseItem;
        internal MenuItem firstItem;
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

        public void Execute()
        {
            throw new PlatformNotSupportedException();
        }

        internal void SetState(int flag, bool value)
        {
            throw new PlatformNotSupportedException();
        }

        internal void SetCaption(string value)
        {
            throw new PlatformNotSupportedException();
        }
    }

    private class MdiListUserData
    {
        public virtual void OnClick(EventArgs e)
        {
            throw new PlatformNotSupportedException();
        }
    }

    private class MdiListFormData : MdiListUserData
    {
        public MdiListFormData(MenuItem parentItem, int boundFormIndex)
        {
            throw new PlatformNotSupportedException();
        }

        public override void OnClick(EventArgs e)
        {
            throw new PlatformNotSupportedException();
        }
    }

    private class MdiListMoreWindowsData : MdiListUserData
    {
        public MdiListMoreWindowsData(MenuItem parent)
        {
            throw new PlatformNotSupportedException();
        }

        public override void OnClick(EventArgs e)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
