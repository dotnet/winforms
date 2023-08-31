// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewButtonCell
{
    private static class DataGridViewButtonCellRenderer
    {
        private static VisualStyleRenderer? s_visualStyleRenderer;

        public static VisualStyleRenderer DataGridViewButtonRenderer
        {
            get
            {
                s_visualStyleRenderer ??= new VisualStyleRenderer(s_buttonElement);

                return s_visualStyleRenderer;
            }
        }

        public static void DrawButton(Graphics g, Rectangle bounds, int buttonState)
        {
            DataGridViewButtonRenderer.SetParameters(s_buttonElement.ClassName, s_buttonElement.Part, buttonState);
            DataGridViewButtonRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
        }
    }
}
