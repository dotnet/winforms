// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms 
{

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs"]/*' />
    /// <devdoc>
    ///     This class contains the information a user needs to paint TreeView nodes.
    /// </devdoc>
    public class DrawTreeNodeEventArgs : EventArgs 
    {

        private readonly Graphics graphics;
        private readonly TreeNode node;
        private readonly Rectangle bounds;
        private readonly TreeNodeStates state;
        private bool drawDefault;

        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.DrawTreeNodeEventArgs"]/*' />
        /// <devdoc>
        ///     Creates a new DrawTreeNodeEventArgs with the given parameters.
        /// </devdoc>
        public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds,
	                             TreeNodeStates state) 
        {
            this.graphics = graphics;
            this.node = node;
            this.bounds = bounds;
            this.state = state;
            this.drawDefault = false;
        }

        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.DrawDefault"]/*' />
        /// <devdoc>
        ///     Causes the item do be drawn by the system instead of owner drawn.
        ///     NOTE: In OwnerDrawText mode, setting this to true is same as calling DrawText.
        /// </devdoc>        
        public bool DrawDefault {
            get  {
                return drawDefault;
            }
            set {
                drawDefault = value;
            }
        }
        
        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics 
        {
            get 
            {
                return graphics;
            }
        }
	
        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.Node"]/*' />
        /// <devdoc>
        ///     The node to be painted. 
        /// </devdoc>
        public TreeNode Node 
        {
            get 
            {
                return node;
            }
        }
	
        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.Bounds"]/*' />
        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds 
        {
            get 
            {
                return bounds;
            }
        }


        /// <include file='doc\DrawTreeNodeEventArgs.uex' path='docs/doc[@for="DrawTreeNodeEventArgs.State"]/*' />
        /// <devdoc>
        ///     Miscellaneous state information.
        /// </devdoc>
        public TreeNodeStates State 
        {
            get 
            {
                return state;
            }
        }
    }
}
