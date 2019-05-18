// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.StatusBar.OnPanelClick'/>
    /// event.
    /// </devdoc>
    public class StatusBarPanelClickEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarPanelClickEventArgs'/>
        /// class.
        /// </devdoc>
        public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
        {
            StatusBarPanel = statusBarPanel;
        }

        /// <summary>
        /// Specifies the <see cref='System.Windows.Forms.StatusBarPanel'/> that represents the clicked panel.
        /// </devdoc>
        public StatusBarPanel StatusBarPanel { get; }
    }
}
