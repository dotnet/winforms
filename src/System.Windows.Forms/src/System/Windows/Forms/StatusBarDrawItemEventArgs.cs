// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.StatusBar.OnDrawItem'/>
    /// event.
    /// </devdoc>
    public class StatusBarDrawItemEventArgs : DrawItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarDrawItemEventArgs'/>
        /// class.
        /// </devdoc>
        public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId,
                                          DrawItemState itemState, StatusBarPanel panel)
            : base(g, font, r, itemId, itemState)
        {
            Panel = panel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarDrawItemEventArgs'/>
        /// class using the Forecolor and Backcolor.
        /// </devdoc>
        public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId,
                                          DrawItemState itemState, StatusBarPanel panel,
                                          Color foreColor, Color backColor)
            : base(g, font, r, itemId, itemState, foreColor, backColor)
        {
            Panel = panel;
        }

        /// <summary>
        /// Specifies the <see cref='System.Windows.Forms.StatusBarPanel'/> to draw.
        /// </devdoc>
        public StatusBarPanel Panel { get; }
    }
}