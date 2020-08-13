// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewRowHeaderCell
    {
        private class DataGridViewRowHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewRowHeaderCellRenderer()
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
                VisualStyleRenderer.SetParameters(s_headerElement.ClassName, s_headerElement.Part, headerState);
                VisualStyleRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }
    }
}
