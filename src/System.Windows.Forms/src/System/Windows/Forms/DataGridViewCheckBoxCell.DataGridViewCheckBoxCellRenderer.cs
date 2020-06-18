// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewCheckBoxCell
    {
        private class DataGridViewCheckBoxCellRenderer
        {
            static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewCheckBoxCellRenderer()
            {
            }

            public static VisualStyleRenderer CheckBoxRenderer
            {
                get
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(CheckBoxElement);
                    }
                    return visualStyleRenderer;
                }
            }

            public static void DrawCheckBox(Graphics g, Rectangle bounds, int state)
            {
                CheckBoxRenderer.SetParameters(CheckBoxElement.ClassName, CheckBoxElement.Part, (int)state);
                CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }
    }
}
