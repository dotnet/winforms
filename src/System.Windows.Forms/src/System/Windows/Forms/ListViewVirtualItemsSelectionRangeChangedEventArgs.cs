// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  The event class that is created when the selection state of a ListViewItem is changed.
    /// </summary>
    public class ListViewVirtualItemsSelectionRangeChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Constructs a ListViewVirtualItemsSelectionRangeChangedEventArgs object.
        /// </summary>
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

        /// <summary>
        ///  Returns the beginning of the range where the selection changed
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        ///  Returns the end of the range where the selection changed
        /// </summary>
        public int EndIndex { get; }

        /// <summary>
        ///  Return true if the items are selected
        /// </summary>
        public bool IsSelected { get; }
    }
}
