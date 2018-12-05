// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;

    /// <devdoc>
    /// The event class that is created when a property
    /// in the grid is modified by the user.
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class PropertyValueChangedEventArgs : EventArgs {
        private readonly GridItem changedItem;
        private object oldValue;
                /// <devdoc>
        /// Constructor
        /// </devdoc>
        public PropertyValueChangedEventArgs(GridItem changedItem, object oldValue) {
            this.changedItem = changedItem;
            this.oldValue = oldValue;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public GridItem ChangedItem {
            get {
                return changedItem;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object OldValue {
            get {
                return oldValue;
            }
        }
    }
}
