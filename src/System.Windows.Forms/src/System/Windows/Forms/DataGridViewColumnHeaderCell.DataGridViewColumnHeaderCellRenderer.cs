// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewColumnHeaderCell
    {
        private class DataGridViewColumnHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewColumnHeaderCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(s_headerElement);
                    }
                    return visualStyleRenderer;
                }
            }

            public static void DrawHeader(Graphics g, Rectangle bounds, int headerState)
            {
                Rectangle rectClip = Rectangle.Truncate(g.ClipBounds);
                if ((int)HeaderItemState.Hot == headerState)
                {
                    // Workaround for a
                    VisualStyleRenderer.SetParameters(s_headerElement);
                    Rectangle cornerClip = new Rectangle(bounds.Left, bounds.Bottom - 2, 2, 2);
                    cornerClip.Intersect(rectClip);
                    VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
                    cornerClip = new Rectangle(bounds.Right - 2, bounds.Bottom - 2, 2, 2);
                    cornerClip.Intersect(rectClip);
                    VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
                }
                VisualStyleRenderer.SetParameters(s_headerElement.ClassName, s_headerElement.Part, headerState);
                VisualStyleRenderer.DrawBackground(g, bounds, rectClip);
            }
        }
    }
}
