// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.CheckedListBox.ItemCheck'/> event.
    /// </devdoc>
    public class ItemCheckedEventArgs : EventArgs
    {
        public ItemCheckedEventArgs(ListViewItem item)
        {
            Item = item;
        }
        
        /// <devdoc>
        /// The index of the item that is about to change.
        /// </devdoc>
        public ListViewItem Item { get; }
    }
}
