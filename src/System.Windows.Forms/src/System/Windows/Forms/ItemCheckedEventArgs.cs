// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='CheckedListBox.ItemCheck'/> event.
    /// </summary>
    public class ItemCheckedEventArgs : EventArgs
    {
        public ItemCheckedEventArgs(ListViewItem item)
        {
            Item = item;
        }

        /// <summary>
        ///  The index of the item that is about to change.
        /// </summary>
        public ListViewItem Item { get; }
    }
}
