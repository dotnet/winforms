// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs"]/*' />
    public class ToolStripArrowRenderEventArgs : EventArgs {

        private Graphics graphics = null;
        private Rectangle arrowRect = Rectangle.Empty;
        private Color arrowColor = Color.Empty;
        private Color defaultArrowColor = Color.Empty;
        private ArrowDirection arrowDirection = ArrowDirection.Down;
        private ToolStripItem item = null;
        private bool arrowColorChanged = false;

        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.ToolStripArrowRenderEventArgs"]/*' />
        public ToolStripArrowRenderEventArgs(Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection) {
            this.item = toolStripItem;
            this.graphics = g;
            this.arrowRect = arrowRectangle;
            this.defaultArrowColor = arrowColor;
            this.arrowDirection = arrowDirection;
        }


        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.ArrowRectangle"]/*' />
        public Rectangle ArrowRectangle {
            get {
                return arrowRect;
            }
            set {
                arrowRect = value;
            }
        }

        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.ArrowColor"]/*' />
        public Color ArrowColor {
            get {
                if (arrowColorChanged) {
                    return arrowColor;
                }
                return DefaultArrowColor;
            }
            set {
                arrowColor = value;
                arrowColorChanged = true;
            }
        }

        internal Color DefaultArrowColor {
            get {
                return defaultArrowColor;
            }
            set {
                defaultArrowColor = value;
            }
        }

        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.Direction"]/*' />
        public ArrowDirection Direction {
            get {
                return arrowDirection;
            }
            set {
                arrowDirection = value;
            }
        }
        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.Graphics"]/*' />
        public Graphics Graphics {
            get {
                return graphics;
            }
        }


        /// <include file='doc\ToolStripArrowRenderEventArgs.uex' path='docs/doc[@for="ToolStripArrowRenderEventArgs.Item"]/*' />
        public ToolStripItem Item {
            get {
                return item;
            }
        }

    }
}
