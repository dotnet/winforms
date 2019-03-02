// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <include file='doc\ListViewItemSelectionChangedEvent.uex' path='docs/doc[@for="ListViewItemSelectionChangedEventArgs"]/*' />
    /// <devdoc>
    /// The event class that is created when the selection state of a ListViewItem is changed.
    /// </devdoc>
    public class ListViewItemSelectionChangedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Constructs a ListViewItemSelectionChangedEventArgs object.
        /// </devdoc>
        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
        {
            Item = item;
            ItemIndex = itemIndex;
            IsSelected = isSelected;
        }

        /// <devdoc>
        /// The list view item whose selection changed
        /// </devdoc>
        public ListViewItem Item { get; }

        /// <devdoc>
        /// The list view item's index
        /// </devdoc>
        public int ItemIndex { get; }

        /// <devdoc>
        /// Return true if the item is selected
        /// </devdoc>
        public bool IsSelected { get; }
    }
}
