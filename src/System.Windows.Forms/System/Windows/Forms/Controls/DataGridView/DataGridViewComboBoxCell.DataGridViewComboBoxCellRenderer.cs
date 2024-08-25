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
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private static readonly VisualStyleElement s_comboBoxBorder = Application.IsDarkModeEnabled ?
            VisualStyleElement.CreateElement("DarkMode_CFD::COMBOBOX", 4, 1)
            : VisualStyleElement.ComboBox.Border.Normal;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private static readonly VisualStyleElement s_comboBoxDropDownButtonRight = Application.IsDarkModeEnabled ?
            VisualStyleElement.CreateElement("DarkMode_CFD::COMBOBOX", 6, 1)
            : VisualStyleElement.ComboBox.DropDownButtonRight.Normal;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private static readonly VisualStyleElement s_comboBoxDropDownButtonLeft = Application.IsDarkModeEnabled ?
            VisualStyleElement.CreateElement("DarkMode_CFD::COMBOBOX", 7, 1) :
            VisualStyleElement.ComboBox.DropDownButtonLeft.Normal;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private static readonly VisualStyleElement s_comboBoxReadOnlyButton = Application.IsDarkModeEnabled ?
            VisualStyleElement.CreateElement("DarkMode_CFD::COMBOBOX", 5, 1)
            : VisualStyleElement.ComboBox.ReadOnlyButton.Normal;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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
