// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

public abstract partial class Menu
{
    /// <summary>
    ///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
    /// </summary>
    [Obsolete(
        Obsoletions.MenuMessage,
        error: false,
        DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [ListBindable(false)]
    public class MenuItemCollection : IList
    {
        public MenuItemCollection(Menu owner) => throw new PlatformNotSupportedException();

        public virtual MenuItem this[int index] => throw null;

        object IList.this[int index]
        {
            get => throw null;
            set { }
        }

        public virtual MenuItem this[string key] => throw null;

        public int Count => throw null;

        object ICollection.SyncRoot => throw null;

        bool ICollection.IsSynchronized => throw null;

        bool IList.IsFixedSize => throw null;

        public bool IsReadOnly => throw null;

        public virtual MenuItem Add(string caption) => throw null;

        public virtual MenuItem Add(string caption, EventHandler onClick) => throw null;

        public virtual MenuItem Add(string caption, MenuItem[] items) => throw null;

        public virtual int Add(MenuItem item) => throw null;

        public virtual int Add(int index, MenuItem item) => throw null;

        public virtual void AddRange(MenuItem[] items) { }

        int IList.Add(object value) => throw null;

        public bool Contains(MenuItem value) => throw null;

        bool IList.Contains(object value) => throw null;

        public virtual bool ContainsKey(string key) => throw null;

        public virtual void Clear() { }

        public void CopyTo(Array dest, int index) { }

        public MenuItem[] Find(string key, bool searchAllChildren) => throw null;

        public int IndexOf(MenuItem value) => throw null;

        public virtual int IndexOfKey(string key) => throw null;

        int IList.IndexOf(object value) => throw null;

        void IList.Insert(int index, object value) { }

        public IEnumerator GetEnumerator() => throw null;

        public virtual void RemoveAt(int index) { }

        public virtual void RemoveByKey(string key) { }

        public virtual void Remove(MenuItem item) { }

        void IList.Remove(object value) { }
    }
}
