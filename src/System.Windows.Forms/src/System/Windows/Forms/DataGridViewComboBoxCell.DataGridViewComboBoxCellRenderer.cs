// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxCell
    {
        private class DataGridViewComboBoxCellRenderer
        {
            [ThreadStatic]
            private static VisualStyleRenderer t_visualStyleRenderer;
            private static readonly VisualStyleElement s_comboBoxBorder = VisualStyleElement.ComboBox.Border.Normal;
            private static readonly VisualStyleElement s_comboBoxDropDownButtonRight = VisualStyleElement.ComboBox.DropDownButtonRight.Normal;
            private static readonly VisualStyleElement s_comboBoxDropDownButtonLeft = VisualStyleElement.ComboBox.DropDownButtonLeft.Normal;
            private static readonly VisualStyleElement s_comboBoxReadOnlyButton = VisualStyleElement.ComboBox.ReadOnlyButton.Normal;

            private DataGridViewComboBoxCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (t_visualStyleRenderer is null)
                    {
                        t_visualStyleRenderer = new VisualStyleRenderer(s_comboBoxReadOnlyButton);
                    }
                    return t_visualStyleRenderer;
                }
            }

            public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                ComboBoxRenderer.DrawTextBox(g, bounds, state);
            }

            public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                ComboBoxRenderer.DrawDropDownButton(g, bounds, state);
            }

            // Post theming functions
            public static void DrawBorder(Graphics g, Rectangle bounds)
            {
                if (t_visualStyleRenderer is null)
                {
                    t_visualStyleRenderer = new VisualStyleRenderer(s_comboBoxBorder);
                }
                else
                {
                    t_visualStyleRenderer.SetParameters(s_comboBoxBorder.ClassName, s_comboBoxBorder.Part, s_comboBoxBorder.State);
                }
                t_visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state, bool rightToLeft)
            {
                if (rightToLeft)
                {
                    if (t_visualStyleRenderer is null)
                    {
                        t_visualStyleRenderer = new VisualStyleRenderer(s_comboBoxDropDownButtonLeft.ClassName, s_comboBoxDropDownButtonLeft.Part, (int)state);
                    }
                    else
                    {
                        t_visualStyleRenderer.SetParameters(s_comboBoxDropDownButtonLeft.ClassName, s_comboBoxDropDownButtonLeft.Part, (int)state);
                    }
                }
                else
                {
                    if (t_visualStyleRenderer is null)
                    {
                        t_visualStyleRenderer = new VisualStyleRenderer(s_comboBoxDropDownButtonRight.ClassName, s_comboBoxDropDownButtonRight.Part, (int)state);
                    }
                    else
                    {
                        t_visualStyleRenderer.SetParameters(s_comboBoxDropDownButtonRight.ClassName, s_comboBoxDropDownButtonRight.Part, (int)state);
                    }
                }
                t_visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawReadOnlyButton(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                if (t_visualStyleRenderer is null)
                {
                    t_visualStyleRenderer = new VisualStyleRenderer(s_comboBoxReadOnlyButton.ClassName, s_comboBoxReadOnlyButton.Part, (int)state);
                }
                else
                {
                    t_visualStyleRenderer.SetParameters(s_comboBoxReadOnlyButton.ClassName, s_comboBoxReadOnlyButton.Part, (int)state);
                }
                t_visualStyleRenderer.DrawBackground(g, bounds);
            }
        }
    }
}
