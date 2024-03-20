// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewCheckBoxCell
{
    private static class DataGridViewCheckBoxCellRenderer
    {
        private static VisualStyleRenderer? s_visualStyleRenderer;

        public static VisualStyleRenderer CheckBoxRenderer
        {
            get
            {
                s_visualStyleRenderer ??= new VisualStyleRenderer(s_checkBoxElement);

                return s_visualStyleRenderer;
            }
        }

        public static void DrawCheckBox(Graphics g, Rectangle bounds, int state)
        {
            CheckBoxRenderer.SetParameters(s_checkBoxElement.ClassName, s_checkBoxElement.Part, state);
            CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
        }
    }
}
