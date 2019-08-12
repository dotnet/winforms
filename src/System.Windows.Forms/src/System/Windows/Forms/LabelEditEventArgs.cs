// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='ListView.LabelEdit'/> event.
    /// </summary>
    public class LabelEditEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='LabelEditEventArgs'/>
        ///  class with the specified index to the <see cref='ListViewItem'/>
        ///  to edit.
        /// </summary>
        public LabelEditEventArgs(int item) : this(item, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='LabelEditEventArgs'/>
        ///  class with the specified index to the <see cref='ListViewItem'/>
        ///  being edited and the new text for the label of the <see cref='ListViewItem'/>.
        /// </summary>
        public LabelEditEventArgs(int item, string label)
        {
            Item = item;
            Label = label;
        }

        /// <summary>
        ///  Gets the zero-based index of the <see cref='ListViewItem'/> containing
        ///  the label to edit.
        /// </summary>
        public int Item { get; }

        /// <summary>
        ///  Gets the new text assigned to the label of the <see cref='ListViewItem'/>.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///  Gets or sets a value indicating whether changes made to the label of the
        /// <see cref='ListViewItem'/> should be canceled.
        /// </summary>
        public bool CancelEdit { get; set; }
    }
}
