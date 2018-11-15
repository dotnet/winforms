// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ListView.LabelEdit'/> event.
    ///    </para>
    /// </devdoc>
    public class LabelEditEventArgs : EventArgs {
        private readonly string label;
        private readonly int item;
        private bool cancelEdit = false;

        /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs.LabelEditEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance
        ///       of the <see cref='System.Windows.Forms.LabelEditEventArgs'/> class with the specified
        ///       index to the <see cref='System.Windows.Forms.ListViewItem'/> to edit.
        ///    </para>
        /// </devdoc>
        public LabelEditEventArgs(int item) {
            this.item = item;
            this.label = null;
        }

        /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs.LabelEditEventArgs1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance
        ///       of the <see cref='System.Windows.Forms.LabelEditEventArgs'/> class with the specified index to the <see cref='System.Windows.Forms.ListViewItem'/> being
        ///       edited and the new text for the label of the <see cref='System.Windows.Forms.ListViewItem'/>.
        ///    </para>
        /// </devdoc>
        public LabelEditEventArgs(int item, string label) {
            this.item = item;
            this.label = label;
        }

        /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs.Label"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the new text assigned to the label of the <see cref='System.Windows.Forms.ListViewItem'/>.
        ///    </para>
        /// </devdoc>
        public string Label {
            get {
                return label;
            }
        }

        /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs.Item"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the zero-based index of the <see cref='System.Windows.Forms.ListViewItem'/> containing the label to
        ///       edit.
        ///    </para>
        /// </devdoc>
        public int Item {
            get {
                return item;
            }
        }

        /// <include file='doc\LabelEditEvent.uex' path='docs/doc[@for="LabelEditEventArgs.CancelEdit"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether changes made to the label of
        ///       the <see cref='System.Windows.Forms.ListViewItem'/> should be canceled.
        ///    </para>
        /// </devdoc>
        public bool CancelEdit {
            get {
                return cancelEdit;
            }
            set {
                cancelEdit = value;
            }
        }
    }
}
