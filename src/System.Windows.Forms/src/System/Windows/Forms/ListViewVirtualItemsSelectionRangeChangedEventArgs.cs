// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// The event class that is created when the selection state of a ListViewItem is changed.
    /// </devdoc>
    public class ListViewVirtualItemsSelectionRangeChangedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Constructs a ListViewVirtualItemsSelectionRangeChangedEventArgs object.
        /// </devdoc>
        public ListViewVirtualItemsSelectionRangeChangedEventArgs(int startIndex, int endIndex, bool isSelected)
        {
            if (startIndex > endIndex)
            {
                throw new ArgumentException(SR.ListViewStartIndexCannotBeLargerThanEndIndex);
            }

            StartIndex = startIndex;
            EndIndex = endIndex;
            IsSelected = isSelected;
        }

        /// <devdoc>
        /// Returns the begining of the range where the selection changed
        /// </devdoc>
        public int StartIndex { get; }

        /// <devdoc>
        /// Returns the end of the range where the selection changed
        /// </devdoc>
        public int EndIndex { get; }

        /// <devdoc>
        /// Return true if the items are selected
        /// </devdoc>
        public bool IsSelected { get; }
    }
}
