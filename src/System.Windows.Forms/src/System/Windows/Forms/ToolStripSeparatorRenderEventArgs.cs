// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class represents all the information to render the ToolStrip
    /// </summary>
    public class ToolStripSeparatorRenderEventArgs : ToolStripItemRenderEventArgs
    {
        /// <summary>
        ///  This class represents all the information to render the ToolStrip
        /// </summary>
        public ToolStripSeparatorRenderEventArgs(Graphics g, ToolStripSeparator separator, bool vertical) : base(g, separator)
        {
            Vertical = vertical;
        }

        public bool Vertical { get; }
    }
}
