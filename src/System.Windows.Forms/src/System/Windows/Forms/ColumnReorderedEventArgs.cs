// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace System.Windows.Forms {
    /// <include file='doc\ColumnReorderedEventArgs.uex' path='docs/doc[@for="ColumnReorderedEventArgs"]/*' />
    public class ColumnReorderedEventArgs : CancelEventArgs {
        private int oldDisplayIndex;
        private int newDisplayIndex;
        private ColumnHeader header;

        /// <include file='doc\ColumnReorderedEventArgs.uex' path='docs/doc[@for="ColumnReorderedEventArgs.ColumnReorderedEventArgs"]/*' />
        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header) : base() {
            this.oldDisplayIndex = oldDisplayIndex;
            this.newDisplayIndex = newDisplayIndex;
            this.header = header;
        }

        /// <include file='doc\ColumnReorderedEventArgs.uex' path='docs/doc[@for="ColumnReorderedEventArgs.OldDisplayIndex"]/*' />
        public int OldDisplayIndex {
            get {
                return oldDisplayIndex;
            }
        }

        /// <include file='doc\ColumnReorderedEventArgs.uex' path='docs/doc[@for="ColumnReorderedEventArgs.NewDisplayIndex"]/*' />
        public int NewDisplayIndex {
            get {
                return newDisplayIndex;
            }
        }

        /// <include file='doc\ColumnReorderedEventArgs.uex' path='docs/doc[@for="ColumnReorderedEventArgs.Header"]/*' />
        public ColumnHeader Header {
            get {
                return header;
            }
        }
    }
}
