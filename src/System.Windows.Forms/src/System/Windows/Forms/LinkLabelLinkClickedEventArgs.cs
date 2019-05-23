﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.LinkLabel.OnLinkClicked'/> event.
    /// </summary>
    [ComVisible(true)]
    public class LinkLabelLinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.LinkLabelLinkClickedEventArgs'/> class, given the link.
        /// </summary>
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link) : this(link, MouseButtons.Left)
        {
        }
        
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button)
        {
            Link = link;
            Button = button;
        }
        
        /// <summary>
        /// Gets the <see cref='System.Windows.Forms.LinkLabel.Link'/> that was clicked.
        /// </summary>
        public LinkLabel.Link Link { get; }

        /// <summary>
        /// Gets the mouseButton which causes the link to be clicked
        /// </summary>
        public MouseButtons Button { get; }
    }
}
