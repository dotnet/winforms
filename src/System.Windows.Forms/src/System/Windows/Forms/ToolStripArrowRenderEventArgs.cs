// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    public class ToolStripArrowRenderEventArgs : EventArgs {

        private Graphics graphics = null;
        private Rectangle arrowRect = Rectangle.Empty;
        private Color arrowColor = Color.Empty;
        private Color defaultArrowColor = Color.Empty;
        private ArrowDirection arrowDirection = ArrowDirection.Down;
        private ToolStripItem item = null;
        private bool arrowColorChanged = false;

        public ToolStripArrowRenderEventArgs(Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection) {
            this.item = toolStripItem;
            this.graphics = g;
            this.arrowRect = arrowRectangle;
            this.defaultArrowColor = arrowColor;
            this.arrowDirection = arrowDirection;
        }


        public Rectangle ArrowRectangle {
            get {
                return arrowRect;
            }
            set {
                arrowRect = value;
            }
        }

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

        public ArrowDirection Direction {
            get {
                return arrowDirection;
            }
            set {
                arrowDirection = value;
            }
        }
        public Graphics Graphics {
            get {
                return graphics;
            }
        }


        public ToolStripItem Item {
            get {
                return item;
            }
        }

    }
}
