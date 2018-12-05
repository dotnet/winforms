// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace System.Windows.Forms {
    public class ColumnReorderedEventArgs : CancelEventArgs {
        private int oldDisplayIndex;
        private int newDisplayIndex;
        private ColumnHeader header;

        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header) : base() {
            this.oldDisplayIndex = oldDisplayIndex;
            this.newDisplayIndex = newDisplayIndex;
            this.header = header;
        }

        public int OldDisplayIndex {
            get {
                return oldDisplayIndex;
            }
        }

        public int NewDisplayIndex {
            get {
                return newDisplayIndex;
            }
        }

        public ColumnHeader Header {
            get {
                return header;
            }
        }
    }
}
