// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewColumnHeaderCell
{
    private static class DataGridViewColumnHeaderCellRenderer
    {
        private static VisualStyleRenderer? s_visualStyleRenderer;

        public static VisualStyleRenderer VisualStyleRenderer
        {
            get
            {
                s_visualStyleRenderer ??= new VisualStyleRenderer(s_headerElement);

                return s_visualStyleRenderer;
            }
        }

        public static void DrawHeader(Graphics g, Rectangle bounds, int headerState)
        {
            Rectangle rectClip = Rectangle.Truncate(g.ClipBounds);
            if (headerState == (int)HeaderItemState.Hot)
            {
                // Workaround for a
                VisualStyleRenderer.SetParameters(s_headerElement);
                Rectangle cornerClip = new(bounds.Left, bounds.Bottom - 2, 2, 2);
                cornerClip.Intersect(rectClip);
                VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
                cornerClip = new Rectangle(bounds.Right - 2, bounds.Bottom - 2, 2, 2);
                cornerClip.Intersect(rectClip);
                VisualStyleRenderer.DrawBackground(g, bounds, cornerClip);
            }

            VisualStyleRenderer.SetParameters(s_headerElement.ClassName, s_headerElement.Part, headerState);
            VisualStyleRenderer.DrawBackground(g, bounds, rectClip);

            ControlPaint.EnforceHeaderCellDividerContrast(g, bounds);
        }
    }
}
