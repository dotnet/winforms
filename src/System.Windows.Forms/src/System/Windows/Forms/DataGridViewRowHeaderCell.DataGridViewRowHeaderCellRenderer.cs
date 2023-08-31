// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewRowHeaderCell
{
    private static class DataGridViewRowHeaderCellRenderer
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
            VisualStyleRenderer.SetParameters(s_headerElement.ClassName, s_headerElement.Part, headerState);
            VisualStyleRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));

            ControlPaint.EnforceHeaderCellDividerContrast(g, bounds);
        }
    }
}
