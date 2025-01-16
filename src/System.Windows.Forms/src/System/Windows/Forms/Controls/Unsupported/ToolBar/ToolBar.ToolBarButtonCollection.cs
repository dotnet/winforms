// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

public partial class ToolBar
{
    /// <summary>
    ///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
    /// </summary>
    [Obsolete(
        Obsoletions.ToolBarMessage,
        error: false,
        DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public class ToolBarButtonCollection : IList
    {
        public ToolBarButtonCollection(ToolBar owner) => throw new PlatformNotSupportedException();

        public virtual ToolBarButton this[int index]
        {
            get => throw null;
            set { }
        }

        object IList.this[int index]
        {
            get => throw null;
            set { }
        }

        public virtual ToolBarButton this[string key] => throw null;

        [Browsable(false)]
        public int Count => throw null;

        object ICollection.SyncRoot => throw null;

        bool ICollection.IsSynchronized => throw null;

        bool IList.IsFixedSize => throw null;

        public bool IsReadOnly => throw null;

        public int Add(ToolBarButton button) => throw null;

        public int Add(string text) => throw null;

        int IList.Add(object button) => throw null;

        public void AddRange(ToolBarButton[] buttons) { }

        public void Clear() { }

        public bool Contains(ToolBarButton button) => throw null;

        bool IList.Contains(object button) => throw null;

        public virtual bool ContainsKey(string key) => throw null;

        void ICollection.CopyTo(Array dest, int index) { }

        public int IndexOf(ToolBarButton button) => throw null;

        int IList.IndexOf(object button) => throw null;

        public virtual int IndexOfKey(string key) => throw null;

        public void Insert(int index, ToolBarButton button) { }

        void IList.Insert(int index, object button) { }

        public void RemoveAt(int index) { }

        public virtual void RemoveByKey(string key) { }

        public void Remove(ToolBarButton button) { }

        void IList.Remove(object button) { }

        public IEnumerator GetEnumerator() => throw null;
    }
}
