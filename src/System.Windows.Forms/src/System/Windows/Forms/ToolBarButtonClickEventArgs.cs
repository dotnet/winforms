// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='ToolBar.ButtonClick'/>
    ///  event.
    /// </summary>
    public class ToolBarButtonClickEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='ToolBarButtonClickEventArgs'/>
        ///  class.
        /// </summary>
        public ToolBarButtonClickEventArgs(ToolBarButton button)
        {
            Button = button;
        }

        /// <summary>
        ///  Specifies the <see cref='ToolBarButton'/>
        ///  that was clicked.
        /// </summary>
        public ToolBarButton Button { get; set; }
    }
}
