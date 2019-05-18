// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.ListView.LabelEdit'/> event.
    /// </devdoc>
    public class LabelEditEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.LabelEditEventArgs'/>
        /// class with the specified index to the <see cref='System.Windows.Forms.ListViewItem'/>
        /// to edit.
        /// </devdoc>
        public LabelEditEventArgs(int item) : this(item, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.LabelEditEventArgs'/>
        /// class with the specified index to the <see cref='System.Windows.Forms.ListViewItem'/>
        /// being edited and the new text for the label of the <see cref='System.Windows.Forms.ListViewItem'/>.
        /// </devdoc>
        public LabelEditEventArgs(int item, string label)
        {
            Item = item;
            Label = label;
        }

        /// <summary>
        /// Gets the zero-based index of the <see cref='System.Windows.Forms.ListViewItem'/> containing
        /// the label to edit.
        /// </devdoc>
        public int Item { get; }

        /// <summary>
        /// Gets the new text assigned to the label of the <see cref='System.Windows.Forms.ListViewItem'/>.
        /// </devdoc>
        public string Label { get; }

        /// <summary>
        /// Gets or sets a value indicating whether changes made to the label of the
        /// <see cref='System.Windows.Forms.ListViewItem'/> should be canceled.
        /// </devdoc>
        public bool CancelEdit { get; set; }
    }
}
