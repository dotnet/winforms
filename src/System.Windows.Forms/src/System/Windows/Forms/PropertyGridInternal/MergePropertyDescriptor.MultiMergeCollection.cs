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
            private object[] items;
            private bool locked;

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
                    if (items is not null)
                    {
                        return items.Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            /// <summary>
            ///  Prevents the contents of the collection from being re-initialized;
            /// </summary>
            public bool Locked
            {
                get
                {
                    return locked;
                }
                set
                {
                    locked = value;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            public void CopyTo(Array array, int index)
            {
                if (items is null)
                {
                    return;
                }

                Array.Copy(items, 0, array, index, items.Length);
            }

            public IEnumerator GetEnumerator()
            {
                if (items is not null)
                {
                    return items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<object>().GetEnumerator();
                }
            }

            /// <summary>
            ///  Ensures that the new collection equals the exisitng one.
            ///  Otherwise, it wipes out the contents of the new collection.
            /// </summary>
            public bool MergeCollection(ICollection newCollection)
            {
                if (locked)
                {
                    return true;
                }

                if (items.Length != newCollection.Count)
                {
                    items = Array.Empty<object>();
                    return false;
                }

                object[] newItems = new object[newCollection.Count];
                newCollection.CopyTo(newItems, 0);
                for (int i = 0; i < newItems.Length; i++)
                {
                    if (((newItems[i] is null) != (items[i] is null)) ||
                        (items[i] is not null && !items[i].Equals(newItems[i])))
                    {
                        items = Array.Empty<object>();
                        return false;
                    }
                }

                return true;
            }

            public void SetItems(ICollection collection)
            {
                if (locked)
                {
                    return;
                }

                items = new object[collection.Count];
                collection.CopyTo(items, 0);
            }
        }
    }
}
