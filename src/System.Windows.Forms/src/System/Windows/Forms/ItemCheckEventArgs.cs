// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.CheckedListBox.ItemCheck'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class ItemCheckEventArgs : EventArgs
    {
        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
        {
            Index = index;
            NewValue = newCheckValue;
            CurrentValue = currentValue;
        }

        /// <devdoc>
        /// The index of the item that is about to change.
        /// </devdoc>
        public int Index { get; }

        /// <devdoc>
        /// The proposed new value of the CheckBox.
        /// </devdoc>
        public CheckState NewValue { get; set; }

        /// <devdoc>
        /// The current state of the CheckBox.
        /// </devdoc>
        public CheckState CurrentValue { get; }
    }
}
