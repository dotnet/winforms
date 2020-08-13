// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class contains the information a user needs to paint TreeView nodes.
    /// </summary>
    public class DrawTreeNodeEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new DrawTreeNodeEventArgs with the given parameters.
        /// </summary>
        public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
        {
            Graphics = graphics;
            Node = node;
            Bounds = bounds;
            State = state;
        }

        /// <summary>
        ///  Graphics object with which painting should be done.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        ///  The node to be painted.
        /// </summary>
        public TreeNode Node { get; }

        /// <summary>
        ///  The rectangle outlining the area in which the painting should be done.
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        ///  Miscellaneous state information.
        /// </summary>
        public TreeNodeStates State { get; }

        /// <summary>
        ///  Causes the item do be drawn by the system instead of owner drawn.
        ///  NOTE: In OwnerDrawText mode, setting this to true is same as calling DrawText.
        /// </summary>
        public bool DrawDefault { get; set; }
    }
}
