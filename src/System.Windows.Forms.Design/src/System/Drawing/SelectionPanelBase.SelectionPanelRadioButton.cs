// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.Drawing.Design;

internal abstract partial class SelectionPanelBase
{
    protected class SelectionPanelRadioButton : RadioButton
    {
        public SelectionPanelRadioButton()
        {
            AutoCheck = false;
        }

        protected override bool ShowFocusCues => true;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.MsgInternal != PInvokeCore.WM_PAINT || !Focused || !Application.IsDarkModeEnabled)
            {
                return;
            }

            using Graphics g = CreateGraphics();
            Rectangle focusBounds = GetFocusBounds(g);
            if (focusBounds.Width <= 0 || focusBounds.Height <= 0)
            {
                return;
            }

            ControlPaint.DrawFocusRectangle(g, focusBounds, ForeColor, BackColor);
        }

        private Rectangle GetFocusBounds(Graphics graphics)
        {
            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsSupported)
            {
                VisualStyleElement element = Checked
                    ? VisualStyleElement.Button.PushButton.Pressed
                    : VisualStyleElement.Button.PushButton.Normal;

                if (VisualStyleRenderer.IsElementDefined(element))
                {
                    VisualStyleRenderer renderer = new(element);
                    return renderer.GetBackgroundContentRectangle(graphics, ClientRectangle);
                }
            }

            Rectangle focusBounds = ClientRectangle;
            focusBounds.Inflate(-SystemInformation.Border3DSize.Width, -SystemInformation.Border3DSize.Height);
            return focusBounds;
        }

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Return => true,
            _ => base.IsInputKey(keyData),
        };
    }
}
