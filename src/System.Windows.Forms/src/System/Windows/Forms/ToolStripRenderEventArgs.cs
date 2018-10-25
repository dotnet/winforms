// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    
    /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs"]/*' />
    /// <devdoc>
    ///   ToolStripRenderEventArgs
    /// </devdoc>
    public class ToolStripRenderEventArgs : EventArgs {

        private ToolStrip      toolStrip         = null;
        private Graphics    graphics       = null;
        private Rectangle   affectedBounds = Rectangle.Empty;
        private Color       backColor      = Color.Empty;
        
        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.ToolStripRenderEventArgs"]/*' />
        /// <devdoc>
        ///  This class represents all the information to render the toolStrip
        /// </devdoc>        
        public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip) {
            this.toolStrip = toolStrip;
            this.graphics = g;
            this.affectedBounds = new Rectangle(Point.Empty, toolStrip.Size);
           
           
        }

        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.ToolStripRenderEventArgs"]/*' />
        /// <devdoc>
        ///  This class represents all the information to render the toolStrip
        /// </devdoc>        
        public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip, Rectangle affectedBounds, Color backColor) {
            this.toolStrip = toolStrip;
            this.affectedBounds = affectedBounds;
            this.graphics = g;
            this.backColor = backColor;
        }

        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.AffectedBounds"]/*' />
        /// <devdoc>
        ///  the bounds to draw in
        /// </devdoc>
        public Rectangle AffectedBounds {
            get {
                return affectedBounds;
            }
        }

        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.BackColor"]/*' />
        /// <devdoc>
        ///  the back color to draw with.
        /// </devdoc>
        public Color BackColor {
            get {
                if (backColor == Color.Empty) {
                    // get the user specified color
                    backColor = toolStrip.RawBackColor;
                    if (backColor == Color.Empty) {
                        if (toolStrip is ToolStripDropDown) {
                            backColor = SystemColors.Menu;
                        }
                        else if (toolStrip is MenuStrip) {
                            backColor = SystemColors.MenuBar;
                        }
                        else {
                            backColor = SystemColors.Control;
                        }
                    }
                }
                return backColor;
            }
        }
        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///  the graphics object to draw with
        /// </devdoc>
        public Graphics Graphics {
            get {
                return graphics;
            }
        }

        /// <include file='doc\ToolStripRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.ToolStrip"]/*' />    
        /// <devdoc>
        ///  Represents which toolStrip was affected by the click
        /// </devdoc>
        public ToolStrip ToolStrip {
            get {
                return toolStrip;
            }
        }

        /// <include file='doc\WinBarRenderEventArgs.uex' path='docs/doc[@for="ToolStripRenderEventArgs.ConnectedArea"]/*' />
        public Rectangle ConnectedArea {
            get {
                ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;
                if (dropDown != null) {
                    ToolStripDropDownItem ownerItem = dropDown.OwnerItem as ToolStripDropDownItem;

                    if (ownerItem is MdiControlStrip.SystemMenuItem) {
                        // there's no connected rect between a system menu item and a dropdown.
                        return Rectangle.Empty;
                    }
                    if (ownerItem !=null && ownerItem.ParentInternal != null && !ownerItem.IsOnDropDown) {
                           // translate the item into our coordinate system.
                           Rectangle itemBounds = new Rectangle(toolStrip.PointToClient(ownerItem.TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords)), ownerItem.Size);
                           Rectangle bounds = ToolStrip.Bounds;

                           Rectangle overlap = ToolStrip.ClientRectangle;
                           overlap.Inflate(1,1);
                           if (overlap.IntersectsWith(itemBounds)) {
                               switch (ownerItem.DropDownDirection) {
                                   case ToolStripDropDownDirection.AboveLeft:
                                   case ToolStripDropDownDirection.AboveRight:                        
                                      // Consider... the shadow effect interferes with the connected area.
                                      return Rectangle.Empty;
                                      // return new Rectangle(itemBounds.X+1, bounds.Height -2, itemBounds.Width -2, 2);                                   
                                   case ToolStripDropDownDirection.BelowRight:
                                   case ToolStripDropDownDirection.BelowLeft:
                                       overlap.Intersect(itemBounds);
                                       if (overlap.Height == 2) {
                                            return new Rectangle(itemBounds.X+1, 0, itemBounds.Width -2, 2);                                   
                                       }
                                       // if its overlapping more than one pixel, this means we've pushed it to obscure 
                                       // the menu item.  in this case pretend it's not connected.
                                       return Rectangle.Empty;
                                   case ToolStripDropDownDirection.Right:
                                   case ToolStripDropDownDirection.Left:
                                       return Rectangle.Empty;
                                       // Consider... the shadow effect interferes with the connected area.
                                      // return new Rectangle(bounds.Width-2, 1, 2, itemBounds.Height-2);                                   
                               }
                           }
                     }
                }
                return Rectangle.Empty;
            }
        }


    }
}
