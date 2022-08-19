// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewCheckBoxCell
    {
        private static class DataGridViewCheckBoxCellRenderer
        {
            private static VisualStyleRenderer? s_visualStyleRenderer;

            public static VisualStyleRenderer CheckBoxRenderer => s_visualStyleRenderer ??= new VisualStyleRenderer(CheckBoxElement);

            public static void DrawCheckBox(Graphics g, Rectangle bounds, int state)
            {
                CheckBoxRenderer.SetParameters(CheckBoxElement.ClassName, CheckBoxElement.Part, state);
                CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));

                // It uses because the checked style of the checkbox in a selected cell has a low contrast ratio with the background. Only for Windows 11.
                const int StateSelectedUnfocused = 5;
                const int StateSelectedFocused = 6;
                if (OsVersion.IsWindows11_OrGreater && (state == StateSelectedUnfocused  || state ==StateSelectedFocused))
                {
                    DrawContrastBorder(g, bounds);
                }
            }

            private static void DrawContrastBorder(Graphics g, Rectangle bounds)
            {
                const int penWidth = 1;
                const int cornerRadius = 3;

                var focusPen = new Pen(SystemColors.ControlText, penWidth);
                var rectangleBounds = bounds with
                {
                    Width = bounds.Width - penWidth,
                    Height = bounds.Height - penWidth
                };

                RoundedRectangle.Paint(g, focusPen, rectangleBounds, cornerRadius);
            }
        }
    }
}
