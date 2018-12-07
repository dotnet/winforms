// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    
    using System;

    /// <include file='doc\ItemChangedEventArgs.uex' path='docs/doc[@for="ItemChangedEventArgs"]/*' />
    public class ItemChangedEventArgs : EventArgs {

        private int index;    
    
        internal ItemChangedEventArgs(int index) {
            this.index = index;
        }

        /// <include file='doc\ItemChangedEventArgs.uex' path='docs/doc[@for="ItemChangedEventArgs.Index"]/*' />
        public int Index {
            get {
                return index;
            }
        }
    }
}
