// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms
{
    public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs
    {
        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle) : base(g, item)
        {
            Image = (item.RightToLeftAutoMirrorImage && (item.RightToLeft == RightToLeft.Yes)) ? item.MirroredImage : item.Image;
            ImageRectangle = imageRectangle;
        }

        /// <devdoc>
        /// This class represents all the information to render the ToolStrip
        /// </devdoc>
        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle) : base(g, item)
        {
            Image = image;
            ImageRectangle = imageRectangle;
        }

        /// <devdoc>
        /// The image to draw
        /// </devdoc>
        public Image Image { get; }

	    /// <devdoc>
	    /// The rectangle to draw the Image in
	    /// </devdoc>
	    public Rectangle ImageRectangle { get; }

        /// <devdoc>
        /// Not public as it currently pertains to button &amp; system renderer.
        /// </devdoc>
        internal bool ShiftOnPress { get; set; }

        /// <devdoc>
        /// Not public as it currently pertains to ToolStripRenderer.
        /// </devdoc>
        internal ImageAttributes ImageAttributes { get; set; }
    }
}
