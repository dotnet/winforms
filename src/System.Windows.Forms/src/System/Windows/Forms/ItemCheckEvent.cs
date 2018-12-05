// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.CheckedListBox.ItemCheck'/>
    ///       event.
    ///
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ItemCheckEventArgs : EventArgs {

        readonly int index;
        CheckState newValue;
        readonly CheckState currentValue;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue) {
            this.index = index;
            this.newValue = newCheckValue;
            this.currentValue = currentValue;
        }
        /// <devdoc>
        ///     The index of the item that is about to change.
        /// </devdoc>
        public int Index {
            get { return index; }
        }

        /// <devdoc>
        ///     The proposed new value of the CheckBox.
        /// </devdoc>
        public CheckState NewValue {
            get { return newValue; }
            set { newValue = value; }
        }

        /// <devdoc>
        ///     The current state of the CheckBox.
        /// </devdoc>
        public CheckState CurrentValue {
            get { return currentValue; }
        }

    }
}
