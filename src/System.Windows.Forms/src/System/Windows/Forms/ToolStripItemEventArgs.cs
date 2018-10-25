// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\ToolStripItemEventArgs.uex' path='docs/doc[@for="ToolStripItemEventArgs"]/*' />
    public class  ToolStripItemEventArgs : EventArgs {

        private ToolStripItem item;

        /// <include file='doc\ToolStripItemEventArgs.uex' path='docs/doc[@for="ToolStripItemEventArgs.ToolStripItemEventArgs"]/*' />
        public ToolStripItemEventArgs(ToolStripItem item) {
           this.item = item;
        }

        /// <include file='doc\ToolStripItemEventArgs.uex' path='docs/doc[@for="ToolStripItemEventArgs.Item"]/*' />
        public ToolStripItem Item {
            get {
                return item;
            }
        }
    }
}
