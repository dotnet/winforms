// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class represents all the information to render the winbar
    /// </devdoc>
    public class ToolStripSeparatorRenderEventArgs : ToolStripItemRenderEventArgs
    {
        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripSeparatorRenderEventArgs(Graphics g, ToolStripSeparator separator, bool vertical) : base(g, separator)
        {
            Vertical = vertical;
        }

        public bool Vertical { get; }
    }
}
