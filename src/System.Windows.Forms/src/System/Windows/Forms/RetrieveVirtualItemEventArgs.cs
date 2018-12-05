// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace System.Windows.Forms {
    public class RetrieveVirtualItemEventArgs : EventArgs {
        private int itemIndex;
        private ListViewItem item;

        public RetrieveVirtualItemEventArgs(int itemIndex) {
            this.itemIndex = itemIndex;
        }

        public int ItemIndex {
            get {
                return itemIndex;
            }
        }

        public ListViewItem Item {
            get {
                return item;
            }
            set {
                this.item = value;
            }
        }
    }
}
