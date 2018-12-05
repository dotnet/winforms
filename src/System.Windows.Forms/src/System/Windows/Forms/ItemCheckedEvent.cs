// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.CheckedListBox.ItemCheck'/>
    ///       event.
    ///
    ///    </para>
    /// </devdoc>
    
    public class ItemCheckedEventArgs : EventArgs {
        private ListViewItem lvi;


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ItemCheckedEventArgs(ListViewItem item) {
            this.lvi = item;
        }
        /// <devdoc>
        ///     The index of the item that is about to change.
        /// </devdoc>
        public ListViewItem Item {
            get { return lvi; }
        }
    }
}
