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
            private static VisualStyleRenderer visualStyleRenderer;
            private static readonly VisualStyleElement ComboBoxBorder = VisualStyleElement.ComboBox.Border.Normal;
            private static readonly VisualStyleElement ComboBoxDropDownButtonRight = VisualStyleElement.ComboBox.DropDownButtonRight.Normal;
            private static readonly VisualStyleElement ComboBoxDropDownButtonLeft = VisualStyleElement.ComboBox.DropDownButtonLeft.Normal;
            private static readonly VisualStyleElement ComboBoxReadOnlyButton = VisualStyleElement.ComboBox.ReadOnlyButton.Normal;

            private DataGridViewComboBoxCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxReadOnlyButton);
                    }
                    return visualStyleRenderer;
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
                if (visualStyleRenderer is null)
                {
                    visualStyleRenderer = new VisualStyleRenderer(ComboBoxBorder);
                }
                else
                {
                    visualStyleRenderer.SetParameters(ComboBoxBorder.ClassName, ComboBoxBorder.Part, ComboBoxBorder.State);
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state, bool rightToLeft)
            {
                if (rightToLeft)
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxDropDownButtonLeft.ClassName, ComboBoxDropDownButtonLeft.Part, (int)state);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(ComboBoxDropDownButtonLeft.ClassName, ComboBoxDropDownButtonLeft.Part, (int)state);
                    }
                }
                else
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(ComboBoxDropDownButtonRight.ClassName, ComboBoxDropDownButtonRight.Part, (int)state);
                    }
                    else
                    {
                        visualStyleRenderer.SetParameters(ComboBoxDropDownButtonRight.ClassName, ComboBoxDropDownButtonRight.Part, (int)state);
                    }
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }

            public static void DrawReadOnlyButton(Graphics g, Rectangle bounds, ComboBoxState state)
            {
                if (visualStyleRenderer is null)
                {
                    visualStyleRenderer = new VisualStyleRenderer(ComboBoxReadOnlyButton.ClassName, ComboBoxReadOnlyButton.Part, (int)state);
                }
                else
                {
                    visualStyleRenderer.SetParameters(ComboBoxReadOnlyButton.ClassName, ComboBoxReadOnlyButton.Part, (int)state);
                }
                visualStyleRenderer.DrawBackground(g, bounds);
            }
        }
    }
}
