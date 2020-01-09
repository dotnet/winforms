// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms
{
    public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs
    {
        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle) : base(g, item)
        {
            Image = (item != null && item.RightToLeftAutoMirrorImage && item.RightToLeft == RightToLeft.Yes) ? item.MirroredImage : item?.Image;
            ImageRectangle = imageRectangle;
        }

        /// <summary>
        ///  This class represents all the information to render the ToolStrip
        /// </summary>
        public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle) : base(g, item)
        {
            Image = image;
            ImageRectangle = imageRectangle;
        }

        /// <summary>
        ///  The image to draw
        /// </summary>
        public Image Image { get; }

        /// <summary>
        ///  The rectangle to draw the Image in
        /// </summary>
        public Rectangle ImageRectangle { get; }

        /// <summary>
        ///  Not public as it currently pertains to button &amp; system renderer.
        /// </summary>
        internal bool ShiftOnPress { get; set; }

        /// <summary>
        ///  Not public as it currently pertains to ToolStripRenderer.
        /// </summary>
        internal ImageAttributes ImageAttributes { get; set; }
    }
}
