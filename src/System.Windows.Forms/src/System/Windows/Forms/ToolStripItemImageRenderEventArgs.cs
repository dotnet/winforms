// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;
    using System.Drawing.Imaging;

    /// <include file='doc\ToolStripItemImageRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemImageRenderEventArgs"]/*' />
    /// <devdoc/>
    public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs {

        private Image              image             = null;
        private Rectangle          imageRectangle    = Rectangle.Empty;
        private bool               shiftOnPress      = false;
        private ImageAttributes    imageAttr         = null;

        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle) : base(g, item) {
            this.image = (item.RightToLeftAutoMirrorImage && (item.RightToLeft == RightToLeft.Yes)) ? item.MirroredImage : item.Image;
            this.imageRectangle = imageRectangle;

        }
        /// <include file='doc\ToolStripItemImageRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemImageRenderEventArgs.ToolStripItemImageRenderEventArgs"]/*' />
        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle) : base(g, item) {
            this.image = image;
            this.imageRectangle = imageRectangle;
        }


        /// <include file='doc\ToolStripItemImageRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemImageRenderEventArgs.Image"]/*' />
        /// <devdoc>
        /// the string to draw
        /// </devdoc>
        public Image Image  {
            get {
                return image;    
            }
        }

	    /// <include file='doc\ToolStripItemImageRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemImageRenderEventArgs.ImageRectangle"]/*' />
	    /// <devdoc>
	    /// the rectangle to draw the Image in 
	    /// </devdoc>
	    public Rectangle ImageRectangle { 
	        get {
                return imageRectangle;
	        }
	    }

        // not public as it currently pertains to button & system renderer.
        internal bool ShiftOnPress {
            get { return shiftOnPress; }
            set { shiftOnPress = value; }
        }

        // not public as it currently pertains to ToolStripRenderer.
        internal ImageAttributes ImageAttributes {
            get { return imageAttr; }
            set { imageAttr = value; }
        }
    
    }
}
