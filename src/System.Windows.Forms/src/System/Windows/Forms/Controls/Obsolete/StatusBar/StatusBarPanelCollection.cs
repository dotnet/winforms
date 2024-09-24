// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class StatusBar
{
    [Obsolete(
        Obsoletions.StatusBarMessage,
        error: false,
        DiagnosticId = Obsoletions.StatusBarDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class StatusBarPanelCollection : IList
    {
        public StatusBarPanelCollection(StatusBar owner) => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[int index]
        {
            get => throw new PlatformNotSupportedException();
            set => throw new PlatformNotSupportedException();
        }

        object IList.this[int index]
        {
            get => throw new PlatformNotSupportedException();
#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
            set => throw new PlatformNotSupportedException();
#pragma warning restore CS8769
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[string key] => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Count => throw new PlatformNotSupportedException();

        object ICollection.SyncRoot => throw new PlatformNotSupportedException();

        bool ICollection.IsSynchronized => throw new PlatformNotSupportedException();

        bool IList.IsFixedSize => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReadOnly => throw new PlatformNotSupportedException();

        public virtual StatusBarPanel Add(string text) => throw new PlatformNotSupportedException();

        public virtual int Add(StatusBarPanel value) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        int IList.Add(object value) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769

        public virtual void AddRange(StatusBarPanel[] panels) => throw new PlatformNotSupportedException();

        public bool Contains(StatusBarPanel panel) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        bool IList.Contains(object panel) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769

        public virtual bool ContainsKey(string key) => throw new PlatformNotSupportedException();

        public int IndexOf(StatusBarPanel panel) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        int IList.IndexOf(object panel) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769

        public virtual int IndexOfKey(string key) => throw new PlatformNotSupportedException();

        public virtual void Insert(int index, StatusBarPanel value) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        void IList.Insert(int index, object value) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769

        public virtual void Clear() => throw new PlatformNotSupportedException();

        public virtual void Remove(StatusBarPanel value) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        void IList.Remove(object value) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769

        public virtual void RemoveAt(int index) => throw new PlatformNotSupportedException();

        public virtual void RemoveByKey(string key) => throw new PlatformNotSupportedException();

        void ICollection.CopyTo(Array dest, int index) => throw new PlatformNotSupportedException();

        public IEnumerator GetEnumerator() => throw new PlatformNotSupportedException();
    }
}
