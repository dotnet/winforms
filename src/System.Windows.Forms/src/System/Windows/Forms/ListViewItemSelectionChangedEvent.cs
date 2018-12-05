// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms
{
    using System.ComponentModel;

    /// <devdoc>
    /// The event class that is created when the selection state of a ListViewItem is changed.
    /// </devdoc>
    public class ListViewItemSelectionChangedEventArgs : EventArgs
    {
        private ListViewItem item;
        private int itemIndex;
        private bool isSelected;
        
        /// <devdoc>
        /// Constructs a ListViewItemSelectionChangedEventArgs object.
        /// </devdoc>
        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
        {
            this.item = item;
            this.itemIndex = itemIndex;
            this.isSelected = isSelected;
        }

        /// <devdoc>
        /// Return true if the item is selected
        /// </devdoc>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
        }

        /// <devdoc>
        /// The list view item whose selection changed
        /// </devdoc>
        public ListViewItem Item
        {
            get
            {
                return this.item;
            }
        }

        /// <devdoc>
        /// The list view item's index
        /// </devdoc>
        public int ItemIndex
        {
            get
            {
                return this.itemIndex;
            }
        }
    }
}
