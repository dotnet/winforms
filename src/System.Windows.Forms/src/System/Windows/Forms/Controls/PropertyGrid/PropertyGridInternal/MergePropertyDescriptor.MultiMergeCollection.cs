// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class MergePropertyDescriptor
{
    private class MultiMergeCollection : ICollection
    {
        private object?[]? _items;

        public MultiMergeCollection(ICollection original)
        {
            SetItems(original);
        }

        /// <summary>
        ///  Retrieves the number of items.
        /// </summary>
        public int Count => _items?.Length ?? 0;

        bool ICollection.IsSynchronized => false;

        /// <summary>
        ///  Prevents the contents of the collection from being re-initialized;
        /// </summary>
        public bool Locked { get; set; }

        object ICollection.SyncRoot => this;

        public void CopyTo(Array array, int index)
        {
            if (_items is null)
            {
                return;
            }

            Array.Copy(_items, 0, array, index, _items.Length);
        }

        public IEnumerator GetEnumerator() => _items?.GetEnumerator() ?? Array.Empty<object>().GetEnumerator();

        /// <summary>
        ///  Compare the contents of this <see cref="MultiMergeCollection"/> collection against
        ///  <paramref name="collection"/>. Reinitializes the contents of this <see cref="MultiMergeCollection"/>
        ///  if not <see cref="Locked"/> and the <paramref name="collection"/> does not match.
        /// </summary>
        /// <returns>
        ///  'true' if <see cref="Locked"/> or <paramref name="collection"/> matches the contents of this
        ///  <see cref="MultiMergeCollection"/>.
        /// </returns>
        public bool ReinitializeIfNotEqual(ICollection collection)
        {
            if (Locked)
            {
                return true;
            }

            if (_items is null || _items.Length != collection.Count)
            {
                _items = [];
                return false;
            }

            object?[] newItems = new object?[collection.Count];
            collection.CopyTo(newItems, 0);
            for (int i = 0; i < newItems.Length; i++)
            {
                if (((newItems[i] is null) != (_items[i] is null)) ||
                    (_items[i] is object item && !item.Equals(newItems[i])))
                {
                    _items = [];
                    return false;
                }
            }

            return true;
        }

        public void SetItems(ICollection collection)
        {
            if (Locked)
            {
                return;
            }

            _items = new object[collection.Count];
            collection.CopyTo(_items, 0);
        }
    }
}
