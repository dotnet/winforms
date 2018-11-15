// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;

    /// <include file='doc\SelectedGridItemChangedEvent.uex' path='docs/doc[@for="SelectedGridItemChangedEventArgs"]/*' />
    /// <devdoc>
    /// The event class that is created when the selected GridItem in the PropertyGrid is changed by the user.
    /// </devdoc>
    public class SelectedGridItemChangedEventArgs : EventArgs {
        private GridItem oldSelection;
        private GridItem newSelection;
        
        /// <include file='doc\SelectedGridItemChangedEvent.uex' path='docs/doc[@for="SelectedGridItemChangedEventArgs.SelectedGridItemChangedEventArgs"]/*' />
        /// <devdoc>
        /// Constructs a SelectedGridItemChangedEventArgs object.
        /// </devdoc>
        public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel) {
            this.oldSelection = oldSel;
            this.newSelection = newSel;
        }
        
        
        
        /// <include file='doc\SelectedGridItemChangedEvent.uex' path='docs/doc[@for="SelectedGridItemChangedEventArgs.NewSelection"]/*' />
        /// <devdoc>
        /// The newly selected GridItem object
        /// </devdoc>
        public GridItem NewSelection {
            get {
                return this.newSelection;
            }
        }
        
        /// <include file='doc\SelectedGridItemChangedEvent.uex' path='docs/doc[@for="SelectedGridItemChangedEventArgs.OldSelection"]/*' />
        /// <devdoc>
        /// The previously selected GridItem object.  This can be null.
        /// </devdoc>
        public GridItem OldSelection {
            get {
                return this.oldSelection;
            }
        }    
        
   }
}
