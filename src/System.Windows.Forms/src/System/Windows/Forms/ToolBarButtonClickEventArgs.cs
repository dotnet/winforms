// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.ToolBar.ButtonClick'/>
    /// event.
    /// </devdoc>
    public class ToolBarButtonClickEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.ToolBarButtonClickEventArgs'/>
        /// class.
        /// </devdoc>
        public ToolBarButtonClickEventArgs(ToolBarButton button)
        {
            Button = button;
        }

        /// <devdoc>
        /// Specifies the <see cref='System.Windows.Forms.ToolBarButton'/>
        /// that was clicked.
        /// </devdoc>
        public ToolBarButton Button { get; set; }
    }
}
