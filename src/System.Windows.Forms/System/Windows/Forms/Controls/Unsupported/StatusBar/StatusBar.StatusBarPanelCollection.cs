// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

public partial class StatusBar
{
    /// <summary>
    ///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
    /// </summary>
    [Obsolete(
        Obsoletions.StatusBarMessage,
        error: false,
        DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [ListBindable(false)]
    public class StatusBarPanelCollection : IList
    {
        public StatusBarPanelCollection(StatusBar owner) => throw new PlatformNotSupportedException();

        public virtual StatusBarPanel this[int index]
        {
            get => throw null;
            set { }
        }

        object IList.this[int index]
        {
            get => throw null;
            set { }
        }

        public virtual StatusBarPanel this[string key] => throw null;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Count => throw null;

        object ICollection.SyncRoot => throw null;

        bool ICollection.IsSynchronized => throw null;

        bool IList.IsFixedSize => throw null;

        public bool IsReadOnly => throw null;

        public virtual StatusBarPanel Add(string text) => throw null;

        public virtual int Add(StatusBarPanel value) => throw null;

        int IList.Add(object value) => throw null;

        public virtual void AddRange(StatusBarPanel[] panels) { }

        public bool Contains(StatusBarPanel panel) => throw null;

        bool IList.Contains(object panel) => throw null;

        public virtual bool ContainsKey(string key) => throw null;

        public int IndexOf(StatusBarPanel panel) => throw null;

        int IList.IndexOf(object panel) => throw null;

        public virtual int IndexOfKey(string key) => throw null;

        public virtual void Insert(int index, StatusBarPanel value) { }

        void IList.Insert(int index, object value) { }

        public virtual void Clear() { }

        public virtual void Remove(StatusBarPanel value) { }

        void IList.Remove(object value) { }

        public virtual void RemoveAt(int index) { }

        public virtual void RemoveByKey(string key) { }

        void ICollection.CopyTo(Array dest, int index) { }

        public IEnumerator GetEnumerator() => throw null;
    }
}
