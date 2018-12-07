// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;

    /// <include file='doc\PropertyValueChangedEvent.uex' path='docs/doc[@for="PropertyValueChangedEventArgs"]/*' />
    /// <devdoc>
    /// The event class that is created when a property
    /// in the grid is modified by the user.
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class PropertyValueChangedEventArgs : EventArgs {
        private readonly GridItem changedItem;
        private object oldValue;
                /// <include file='doc\PropertyValueChangedEvent.uex' path='docs/doc[@for="PropertyValueChangedEventArgs.PropertyValueChangedEventArgs"]/*' />
                /// <devdoc>
        /// Constructor
        /// </devdoc>
        public PropertyValueChangedEventArgs(GridItem changedItem, object oldValue) {
            this.changedItem = changedItem;
            this.oldValue = oldValue;
        }

        /// <include file='doc\PropertyValueChangedEvent.uex' path='docs/doc[@for="PropertyValueChangedEventArgs.ChangedItem"]/*' />
        public GridItem ChangedItem {
            get {
                return changedItem;
            }
        }
        
        /// <include file='doc\PropertyValueChangedEvent.uex' path='docs/doc[@for="PropertyValueChangedEventArgs.OldValue"]/*' />
        public object OldValue {
            get {
                return oldValue;
            }
        }
    }
}
