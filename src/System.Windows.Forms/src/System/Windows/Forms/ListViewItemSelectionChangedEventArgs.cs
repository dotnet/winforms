// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  The event class that is created when the selection state of a ListViewItem is changed.
    /// </summary>
    public class ListViewItemSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Constructs a ListViewItemSelectionChangedEventArgs object.
        /// </summary>
        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
        {
            Item = item;
            ItemIndex = itemIndex;
            IsSelected = isSelected;
        }

        /// <summary>
        ///  The list view item whose selection changed
        /// </summary>
        public ListViewItem Item { get; }

        /// <summary>
        ///  The list view item's index
        /// </summary>
        public int ItemIndex { get; }

        /// <summary>
        ///  Return true if the item is selected
        /// </summary>
        public bool IsSelected { get; }
    }
}
