// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;
#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.MenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public abstract class Menu : Component
{
    public const int FindHandle = 0;
    public const int FindShortcut = 1;

    protected Menu(MenuItem[] items)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IntPtr Handle
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool IsParent
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MenuItem MdiListItem
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Name
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MenuItemCollection MenuItems
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Tag
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public MenuItem FindMenuItem(int type, IntPtr value)
        => throw new PlatformNotSupportedException();

    public ContextMenu GetContextMenu()
        => throw new PlatformNotSupportedException();

    public MainMenu GetMainMenu()
        => throw new PlatformNotSupportedException();

    public virtual void MergeMenu(Menu menuSrc)
        => throw new PlatformNotSupportedException();

    public override string ToString()
        => throw new PlatformNotSupportedException();

    [ListBindable(false)]
    [Obsolete(
        Obsoletions.MenuItemCollectionMessage,
        error: false,
        DiagnosticId = Obsoletions.MenuItemCollectionDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public class MenuItemCollection : IList
    {
        public MenuItemCollection(Menu owner)
            => throw new PlatformNotSupportedException();

        public virtual MenuItem this[int index]
            => throw new PlatformNotSupportedException();

        object IList.this[int index]
        {
            get => throw new PlatformNotSupportedException();
            set => throw new PlatformNotSupportedException();
        }

        public virtual MenuItem this[string key]
            => throw new PlatformNotSupportedException();

        public int Count
            => throw new PlatformNotSupportedException();

        object ICollection.SyncRoot
            => throw new PlatformNotSupportedException();

        bool ICollection.IsSynchronized
            => throw new PlatformNotSupportedException();

        bool IList.IsFixedSize
            => throw new PlatformNotSupportedException();

        public bool IsReadOnly
            => throw new PlatformNotSupportedException();

        public virtual MenuItem Add(string caption)
            => throw new PlatformNotSupportedException();

        public virtual MenuItem Add(string caption, EventHandler onClick)
            => throw new PlatformNotSupportedException();

        public virtual MenuItem Add(string caption, MenuItem[] items)
            => throw new PlatformNotSupportedException();

        public virtual int Add(object value)
            => throw new PlatformNotSupportedException();

        public virtual int Add(int index, MenuItem item)
            => throw new PlatformNotSupportedException();

        public virtual void AddRange(MenuItem[] items)
            => throw new PlatformNotSupportedException();

        public bool Contains(object value)
            => throw new PlatformNotSupportedException();

        public virtual bool ContainsKey(string key)
            => throw new PlatformNotSupportedException();

        public MenuItem[] Find(string key, bool searchAllChildren)
            => throw new PlatformNotSupportedException();

        public int IndexOf(MenuItem value)
            => throw new PlatformNotSupportedException();

        int IList.IndexOf(object value)
            => throw new PlatformNotSupportedException();

        public virtual int IndexOfKey(string key)
            => throw new PlatformNotSupportedException();

        void IList.Insert(int index, object value)
            => throw new PlatformNotSupportedException();

        public virtual void Clear()
            => throw new PlatformNotSupportedException();

        public void CopyTo(Array array, int index)
            => throw new PlatformNotSupportedException();

        public IEnumerator GetEnumerator()
            => throw new PlatformNotSupportedException();

        public virtual void RemoveAt(int index)
            => throw new PlatformNotSupportedException();

        public virtual void RemoveByKey(string key)
            => throw new PlatformNotSupportedException();

        public virtual void Remove(MenuItem item)
            => throw new PlatformNotSupportedException();

        void IList.Remove(object value)
            => throw new PlatformNotSupportedException();
    }
}
