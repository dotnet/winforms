// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class contains the information a user needs to paint TreeView nodes.
    /// </devdoc>
    public class DrawTreeNodeEventArgs : EventArgs
    {
        /// <devdoc>
        /// Creates a new DrawTreeNodeEventArgs with the given parameters.
        /// </devdoc>
        public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
        {
            Graphics = graphics;
            Node = node;
            Bounds = bounds;
            State = state;
        }

        /// <devdoc>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// The node to be painted.
        /// </devdoc>
        public TreeNode Node { get; }

        /// <devdoc>
        /// The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds { get; }

        /// <devdoc>
        /// Miscellaneous state information.
        /// </devdoc>
        public TreeNodeStates State { get; }
        
        /// <devdoc>
        /// Causes the item do be drawn by the system instead of owner drawn.
        /// NOTE: In OwnerDrawText mode, setting this to true is same as calling DrawText.
        /// </devdoc>
        public bool DrawDefault { get; set; }
    }
}
