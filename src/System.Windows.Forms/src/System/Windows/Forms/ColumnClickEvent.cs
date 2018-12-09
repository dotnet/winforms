// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ColumnClickEvent.uex' path='docs/doc[@for="ColumnClickEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ListView.OnColumnClick'/>
    ///       event.
    ///
    ///    </para>
    /// </devdoc>
    public class ColumnClickEventArgs : EventArgs {
        readonly int column;

        /// <include file='doc\ColumnClickEvent.uex' path='docs/doc[@for="ColumnClickEventArgs.ColumnClickEventArgs"]/*' />
        public ColumnClickEventArgs(int column) {
            this.column = column;
        }

        /// <include file='doc\ColumnClickEvent.uex' path='docs/doc[@for="ColumnClickEventArgs.Column"]/*' />
        public int Column {
            get {
                return column;
            }
        }
    }
}
