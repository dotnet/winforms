// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ToolBar
{
    [Obsolete(
        Obsoletions.ToolBarButtonCollectionMessage,
        error: false,
        DiagnosticId = Obsoletions.ToolBarButtonCollectionDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat),
        EditorBrowsable(EditorBrowsableState.Never)]
    public class ToolBarButtonCollection : IList
    {
        public ToolBarButtonCollection(ToolBar owner) => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ToolBarButton this[int index]
        {
            get => throw new PlatformNotSupportedException();
            set => throw new PlatformNotSupportedException();
        }

        object IList.this[int index]
        {
            get => throw new PlatformNotSupportedException();
#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
            set => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ToolBarButton this[string key] => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Count => throw new PlatformNotSupportedException();

        object ICollection.SyncRoot => throw new PlatformNotSupportedException();

        bool ICollection.IsSynchronized => throw new PlatformNotSupportedException();

        bool IList.IsFixedSize => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReadOnly => throw new PlatformNotSupportedException();

        public int Add(ToolBarButton button) => throw new PlatformNotSupportedException();

        public int Add(string text) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        int IList.Add(object button) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).

        public void AddRange(ToolBarButton[] buttons) => throw new PlatformNotSupportedException();

        public void Clear() => throw new PlatformNotSupportedException();

        public bool Contains(ToolBarButton button) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        bool IList.Contains(object button) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).

        public virtual bool ContainsKey(string key) => throw new PlatformNotSupportedException();

        void ICollection.CopyTo(Array dest, int index) => throw new PlatformNotSupportedException();

        public int IndexOf(ToolBarButton button) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        int IList.IndexOf(object button) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).

        public virtual int IndexOfKey(string key) => throw new PlatformNotSupportedException();

        public void Insert(int index, ToolBarButton button) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        void IList.Insert(int index, object button) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).

        public void RemoveAt(int index) => throw new PlatformNotSupportedException();

        public virtual void RemoveByKey(string key) => throw new PlatformNotSupportedException();

        public void Remove(ToolBarButton button) => throw new PlatformNotSupportedException();

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        void IList.Remove(object button) => throw new PlatformNotSupportedException();
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).

        public IEnumerator GetEnumerator() => throw new PlatformNotSupportedException();
    }
}
