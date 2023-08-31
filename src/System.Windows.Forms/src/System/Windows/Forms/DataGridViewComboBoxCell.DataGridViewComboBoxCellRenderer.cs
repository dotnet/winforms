// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxCell
{
    private static class DataGridViewComboBoxCellRenderer
    {
        [ThreadStatic]
        private static VisualStyleRenderer? t_visualStyleRenderer;
        private static readonly VisualStyleElement s_comboBoxBorder = VisualStyleElement.ComboBox.Border.Normal;
        private static readonly VisualStyleElement s_comboBoxDropDownButtonRight = VisualStyleElement.ComboBox.DropDownButtonRight.Normal;
        private static readonly VisualStyleElement s_comboBoxDropDownButtonLeft = VisualStyleElement.ComboBox.DropDownButtonLeft.Normal;
        private static readonly VisualStyleElement s_comboBoxReadOnlyButton = VisualStyleElement.ComboBox.ReadOnlyButton.Normal;

        public static VisualStyleRenderer VisualStyleRenderer
        {
            get
            {
                t_visualStyleRenderer ??= new VisualStyleRenderer(s_comboBoxReadOnlyButton);

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
                InitializeRenderer(s_comboBoxDropDownButtonLeft, (int)state);
            }
            else
            {
                InitializeRenderer(s_comboBoxDropDownButtonRight, (int)state);
            }

            t_visualStyleRenderer.DrawBackground(g, bounds);
        }

        public static void DrawReadOnlyButton(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            InitializeRenderer(s_comboBoxReadOnlyButton, (int)state);

            t_visualStyleRenderer.DrawBackground(g, bounds);
        }

        [MemberNotNull(nameof(t_visualStyleRenderer))]
        private static void InitializeRenderer(VisualStyleElement visualStyleElement, int state)
        {
            if (t_visualStyleRenderer is null)
            {
                t_visualStyleRenderer = new VisualStyleRenderer(visualStyleElement.ClassName, visualStyleElement.Part, state);
            }
            else
            {
                t_visualStyleRenderer.SetParameters(visualStyleElement.ClassName, visualStyleElement.Part, state);
            }
        }
    }
}
