// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal class ToolStripProfessionalLowResolutionRenderer : ToolStripProfessionalRenderer
    {
        public ToolStripProfessionalLowResolutionRenderer()
        {
        }

        internal override ToolStripRenderer RendererOverride
        {
            get
            {
                return null;
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is MenuStrip)
            {
                return;
            }
            else if (e.ToolStrip is StatusStrip)
            {
                return;
            }
            else if (e.ToolStrip is ToolStripDropDown)
            {
                base.OnRenderToolStripBorder(e);
            }
            else
            {
                RenderToolStripBorderInternal(e);
            }
        }

        private void RenderToolStripBorderInternal(ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);
            Graphics g = e.Graphics;

            // have to create a pen here because we're not allowed to modify the SystemPens.
            using (Pen p = new Pen(SystemColors.ButtonShadow))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                bool oddWidth = ((bounds.Width & 0x1) == 0x1);
                bool oddHeight = ((bounds.Height & 0x1) == 0x1);
                int indent = 2;

                // top
                g.DrawLine(p, bounds.X + indent, bounds.Y, bounds.Width - 1, bounds.Y);
                // bottom
                g.DrawLine(p, bounds.X + indent, bounds.Height - 1, bounds.Width - 1, bounds.Height - 1);

                // left
                g.DrawLine(p, bounds.X, bounds.Y + indent, bounds.X, bounds.Height - 1);
                // right
                g.DrawLine(p, bounds.Width - 1, bounds.Y + indent, bounds.Width - 1, bounds.Height - 1);

                // connecting pixels

                // top left conntecting pixel - always drawn
                g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(1, 1, 1, 1));

                if (oddWidth)
                {
                    // top right pixel
                    g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(bounds.Width - 2, 1, 1, 1));
                }
                // bottom conntecting pixels - drawn only if height is odd
                if (oddHeight)
                {
                    // bottom left
                    g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(1, bounds.Height - 2, 1, 1));

                }

                // top and bottom right conntecting pixel - drawn only if height and width are odd
                if (oddHeight && oddWidth)
                {
                    // bottom right
                    g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(bounds.Width - 2, bounds.Height - 2, 1, 1));
                }

            }
        }
    }
}
