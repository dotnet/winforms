// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

public partial class ListView
{
    public partial class ListViewItemCollection
    {
        internal interface IInnerList
        {
            int Count { get; }
            bool OwnerIsVirtualListView { get; }
            bool OwnerIsDesignMode { get; }
            ListViewItem this[int index] { get; set; }

            ListViewItem Add(ListViewItem item);
            void AddRange(params ListViewItem[] items);
            void Clear();
            bool Contains(ListViewItem item);
            void CopyTo(Array dest, int index);
            IEnumerator GetEnumerator();
            int IndexOf(ListViewItem item);
            ListViewItem Insert(int index, ListViewItem item);
            void Remove(ListViewItem item);
            void RemoveAt(int index);
            ListViewItem? GetItemByIndex(int index)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

                try
                {
                    return this[index];
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }
    }
}
