// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;
#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("Menu has been deprecated.")]
public abstract class Menu : Component
{
    #nullable disable
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
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  The HMENU handle corresponding to this menu.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    SRDescription(nameof(SR.ControlHandleDescr))]
    public IntPtr Handle
    {
        get
        {
            return handle;
        }
    }

    /// <summary>
    ///  Specifies whether this menu contains any items.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool IsParent
    {
        get
        {
            return items is not null;
        }
    }

    /// <summary>
    ///  The MenuItem that contains the list of MDI child windows.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MenuItem MdiListItem
    {
        get
        {
            return null;
        }
    }

    /// <summary>
    ///  Name of this control. The designer will set this to the same
    ///  as the programatic Id "(name)" of the control - however this
    ///  property has no bearing on the runtime aspects of this control.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Name
    {
        get => WindowsFormsUtils.GetComponentName(this, name);
        set
        {
            if (value is null || value.Length == 0)
            {
                name = null;
            }
            else
            {
                name = value;
            }

            if (Site is not null)
            {
                Site.Name = name;
            }
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
    MergableProperty(false)]
    public MenuItemCollection MenuItems
    {
        get
        {
            if (itemsCollection is null)
            {
                itemsCollection = new MenuItemCollection(this);
            }

            return itemsCollection;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    SRCategory(nameof(SR.CatData)),
    Localizable(false),
    Bindable(true),
    SRDescription(nameof(SR.ControlTagDescr)),
    DefaultValue(null),
    TypeConverter(typeof(StringConverter))]
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

    public MenuItem FindMenuItem(int type, IntPtr value)
    {
        throw new PlatformNotSupportedException();
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
        throw new PlatformNotSupportedException();
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
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Merges another menu's items with this one's.  Menu items are merged according to their
    ///  mergeType and mergeOrder properties.  This function is typically used to
    ///  merge an MDI container's menu with that of its active MDI child.
    /// </summary>
    public virtual void MergeMenu(Menu menuSrc)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        throw new PlatformNotSupportedException();
    }

    [ListBindable(false)]
    [Obsolete("MenuItemCollection has been deprecated.")]
    public class MenuItemCollection : IList
    {
        private readonly Menu owner;

        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        // private int lastAccessedIndex = -1;

        public MenuItemCollection(Menu owner)
        {
            this.owner = owner;
            throw new PlatformNotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual MenuItem this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                return owner.items[index];
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Count
        {
            get
            {
                return 0;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Adds a new MenuItem to the end of this menu with the specified caption,
        ///  and click handler.
        /// </summary>
        public virtual MenuItem Add(string caption, EventHandler onClick)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Adds a new MenuItem to the end of this menu with the specified caption,
        ///  click handler, and items.
        /// </summary>
        public virtual MenuItem Add(string caption, MenuItem[] items)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Adds a MenuItem to the end of this menu
        ///  MenuItems can only be contained in one menu at a time, and may not be added
        ///  more than once to the same menu.
        /// </summary>
        public virtual int Add(object item)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Adds a MenuItem to this menu at the specified index.  The item currently at
        ///  that index, and all items after it, will be moved up one slot.
        ///  MenuItems can only be contained in one menu at a time, and may not be added
        ///  more than once to the same menu.
        /// </summary>
        public virtual int Add(int index, MenuItem item)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual void AddRange(MenuItem[] items)
        {
            throw new PlatformNotSupportedException();
        }

        public bool Contains(object value)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Searches for Controls by their Name property, builds up an array
        ///  of all the controls that match.
        /// </summary>
        public MenuItem[] Find(string key, bool searchAllChildren)
        {
            throw new PlatformNotSupportedException();
        }

        public int IndexOf(MenuItem value)
        {
            throw new PlatformNotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes all existing MenuItems from this menu
        /// </summary>
        public virtual void Clear()
        {
            throw new PlatformNotSupportedException();
        }

        public void CopyTo(Array dest, int index)
        {
            throw new PlatformNotSupportedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes the item at the specified index in this menu.  All subsequent
        ///  items are moved up one slot.
        /// </summary>
        public virtual void RemoveAt(int index)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes the menu iteml with the specified key.
        /// </summary>
        public virtual void RemoveByKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes the specified item from this menu.  All subsequent
        ///  items are moved down one slot.
        /// </summary>
        public virtual void Remove(MenuItem item)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
