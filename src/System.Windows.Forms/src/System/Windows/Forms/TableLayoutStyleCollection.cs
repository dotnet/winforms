// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    [Editor("System.Windows.Forms.Design.StyleCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    public abstract class TableLayoutStyleCollection : IList
    {
        private IArrangedElement _owner;
        private readonly ArrayList _innerList = new ArrayList();

        internal TableLayoutStyleCollection(IArrangedElement owner)
        {
            _owner = owner;
        }

        internal IArrangedElement Owner => _owner;

        internal virtual string PropertyName => null;

        int IList.Add(object style)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            EnsureNotOwned((TableLayoutStyle)style);
            ((TableLayoutStyle)style).Owner = Owner;
            int index = _innerList.Add(style);
            PerformLayoutIfOwned();
            return index;
        }

        public int Add(TableLayoutStyle style)
        {
            return ((IList)this).Add(style);
        }

        void IList.Insert(int index, object style)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            EnsureNotOwned((TableLayoutStyle)style);
            ((TableLayoutStyle)style).Owner = Owner;
            _innerList.Insert(index, style);
            PerformLayoutIfOwned();
        }

        object IList.this[int index]
        {
            get => _innerList[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                TableLayoutStyle style = (TableLayoutStyle)value;
                EnsureNotOwned(style);
                style.Owner = Owner;
                _innerList[index] = style;
                PerformLayoutIfOwned();
            }
        }

        public TableLayoutStyle this[int index]
        {
            get => (TableLayoutStyle)((IList)this)[index];
            set => ((IList)this)[index] = value;
        }

        void IList.Remove(object style)
        {
            if (style == null)
            {
                return;
            }

            ((TableLayoutStyle)style).Owner = null;
            _innerList.Remove(style);
            PerformLayoutIfOwned();
        }

        public void Clear()
        {
            foreach (TableLayoutStyle style in _innerList)
            {
                style.Owner = null;
            }

            _innerList.Clear();
            PerformLayoutIfOwned();
        }

        public void RemoveAt(int index)
        {
            TableLayoutStyle style = (TableLayoutStyle)_innerList[index];
            style.Owner = null;
            _innerList.RemoveAt(index);
            PerformLayoutIfOwned();
        }

        bool IList.Contains(object style) => _innerList.Contains(style);

        int IList.IndexOf(object style) => _innerList.IndexOf(style);

        bool IList.IsFixedSize => _innerList.IsFixedSize;

        bool IList.IsReadOnly => _innerList.IsReadOnly;

        void ICollection.CopyTo(Array array, int startIndex) => _innerList.CopyTo(array, startIndex);

        public int Count => _innerList.Count;

        bool ICollection.IsSynchronized => _innerList.IsSynchronized;

        object ICollection.SyncRoot => _innerList.SyncRoot;

        IEnumerator IEnumerable.GetEnumerator() => _innerList.GetEnumerator();

        private void EnsureNotOwned(TableLayoutStyle style)
        {
            if (style.Owner != null)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, style.GetType().Name), nameof(style));
            }
        }
        internal void EnsureOwnership(IArrangedElement owner)
        {
            _owner = owner;
            for (int i = 0; i < Count; i++)
            {
                this[i].Owner = owner;
            }
        }
        private void PerformLayoutIfOwned()
        {
            if (Owner != null)
            {
                LayoutTransaction.DoLayout(Owner, Owner, PropertyName);
            }
        }
    }
}
