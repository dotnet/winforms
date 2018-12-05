// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    public class  ToolStripItemEventArgs : EventArgs {

        private ToolStripItem item;

        public ToolStripItemEventArgs(ToolStripItem item) {
           this.item = item;
        }

        public ToolStripItem Item {
            get {
                return item;
            }
        }
    }
}
