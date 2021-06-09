// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class MergePropertyDescriptor
    {
        private class MultiMergeCollection : ICollection
        {
            private object[] _items;
            private bool _locked;

            public MultiMergeCollection(ICollection original)
            {
                SetItems(original);
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    if (_items is not null)
                    {
                        return _items.Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Prevents the contents of the collection from being re-initialized;
            /// </summary>
            public bool Locked
            {
                get
                {
                    return _locked;
                }
                set
                {
                    _locked = value;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public void CopyTo(Array array, int index)
            {
                if (_items is null)
                {
                    return;
                }

                Array.Copy(_items, 0, array, index, _items.Length);
            }

            public IEnumerator GetEnumerator()
            {
                if (_items is not null)
                {
                    return _items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<object>().GetEnumerator();
                }
            }

            /// <summary>
            ///  Ensures that the new collection equals the existing one.
            ///  Otherwise, it wipes out the contents of the new collection.
            /// </summary>
            public bool MergeCollection(ICollection newCollection)
            {
                if (_locked)
                {
                    return true;
                }

                if (_items.Length != newCollection.Count)
                {
                    _items = Array.Empty<object>();
                    return false;
                }

                object[] newItems = new object[newCollection.Count];
                newCollection.CopyTo(newItems, 0);
                for (int i = 0; i < newItems.Length; i++)
                {
                    if (((newItems[i] is null) != (_items[i] is null)) ||
                        (_items[i] is not null && !_items[i].Equals(newItems[i])))
                    {
                        _items = Array.Empty<object>();
                        return false;
                    }
                }

                return true;
            }

            public void SetItems(ICollection collection)
            {
                if (_locked)
                {
                    return;
                }

                _items = new object[collection.Count];
                collection.CopyTo(_items, 0);
            }
        }
    }
}
