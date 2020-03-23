// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='CheckedListBox.ItemCheck'/> event.
    /// </summary>
    [ComVisible(true)]
    public class ItemCheckEventArgs : EventArgs
    {
        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
        {
            Index = index;
            NewValue = newCheckValue;
            CurrentValue = currentValue;
        }

        /// <summary>
        ///  The index of the item that is about to change.
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///  The proposed new value of the CheckBox.
        /// </summary>
        public CheckState NewValue { get; set; }

        /// <summary>
        ///  The current state of the CheckBox.
        /// </summary>
        public CheckState CurrentValue { get; }
    }
}
