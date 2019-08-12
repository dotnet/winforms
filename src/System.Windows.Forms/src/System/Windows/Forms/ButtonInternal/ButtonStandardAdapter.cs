// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms.ButtonInternal
{
    internal class ButtonStandardAdapter : ButtonBaseAdapter
    {
        private const int borderWidth = 2;

        internal ButtonStandardAdapter(ButtonBase control) : base(control) { }

        private PushButtonState DetermineState(bool up)
        {
            PushButtonState state = PushButtonState.Normal;

            if (!up)
            {
                state = PushButtonState.Pressed;
            }
            else if (Control.MouseIsOver)
            {
                state = PushButtonState.Hot;
            }
            else if (!Control.Enabled)
            {
                state = PushButtonState.Disabled;
            }
            else if (Control.Focused || Control.IsDefault)
            {
                state = PushButtonState.Default;
            }

            return state;
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
            PaintWorker(e, true, state);
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
            PaintWorker(e, false, state);
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
            PaintUp(e, state);
        }

        private void PaintThemedButtonBackground(PaintEventArgs e, Rectangle bounds, bool up)
        {
            PushButtonState pbState = DetermineState(up);

            // First handle transparent case

            if (ButtonRenderer.IsBackgroundPartiallyTransparent(pbState))
            {
                ButtonRenderer.DrawParentBackground(e.Graphics, bounds, Control);
            }

            // Now draw the actual themed background
            if (!DpiHelper.IsScalingRequirementMet)
            {
                ButtonRenderer.DrawButton(e.Graphics, Control.ClientRectangle, false, pbState);
            }
            else
            {
                ButtonRenderer.DrawButtonForHandle(e.Graphics, Control.ClientRectangle, false, pbState, Control.HandleInternal);
            }

            // Now overlay the background image or backcolor (the former overrides the latter), leaving a
            // margin. We hardcode this margin for now since GetThemeMargins returns 0 all the
            // time.
            // Changing this because GetThemeMargins simply does not
            // work in some cases.
            bounds.Inflate(-buttonBorderSize, -buttonBorderSize);

            //only paint if the user said not to use the themed backcolor.
            if (!Control.UseVisualStyleBackColor)
            {
                bool painted = false;
                bool isHighContrastHighlighted = up && IsHighContrastHighlighted();
                Color color = isHighContrastHighlighted ? SystemColors.Highlight : Control.BackColor;

                // Note: PaintEvent.HDC == 0 if GDI+ has used the HDC -- it wouldn't be safe for us
                // to use it without enough bookkeeping to negate any performance gain of using GDI.
                if (color.A == 255 && e.HDC != IntPtr.Zero)
                {

                    if (DisplayInformation.BitsPerPixel > 8)
                    {
                        RECT r = new RECT(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
                        // SysColorBrush does not have to be deleted.
                        SafeNativeMethods.FillRect(new HandleRef(e, e.HDC), ref r, new HandleRef(this,
                            isHighContrastHighlighted ? SafeNativeMethods.GetSysColorBrush(ColorTranslator.ToOle(color) & 0xFF) : Control.BackColorBrush));
                        painted = true;
                    }
                }

                if (!painted)
                {
                    // don't paint anything from 100% transparent background
                    //
                    if (color.A > 0)
                    {
                        if (color.A == 255)
                        {
                            color = e.Graphics.GetNearestColor(color);
                        }

                        // Color has some transparency or we have no HDC, so we must
                        // fall back to using GDI+.
                        //
                        using (Brush brush = new SolidBrush(color))
                        {
                            e.Graphics.FillRectangle(brush, bounds);
                        }
                    }
                }
            }

            //This code is mostly taken from the non-themed rendering code path.
            if (Control.BackgroundImage != null && !DisplayInformation.HighContrast)
            {
                ControlPaint.DrawBackgroundImage(e.Graphics, Control.BackgroundImage, Color.Transparent, Control.BackgroundImageLayout, Control.ClientRectangle, bounds, Control.DisplayRectangle.Location, Control.RightToLeft);
            }
        }

        void PaintWorker(PaintEventArgs e, bool up, CheckState state)
        {
            up = up && state == CheckState.Unchecked;

            ColorData colors = PaintRender(e.Graphics).Calculate();
            LayoutData layout;
            if (Application.RenderWithVisualStyles)
            {
                //don't have the text-pressed-down effect when we use themed painting
                //this is for consistency with win32 app.
                layout = PaintLayout(e, true).Layout();
            }
            else
            {
                layout = PaintLayout(e, up).Layout();
            }

            Graphics g = e.Graphics;

            Button thisbutton = Control as Button;
            if (Application.RenderWithVisualStyles)
            {
                PaintThemedButtonBackground(e, Control.ClientRectangle, up);
            }
            else
            {
                Brush backbrush = null;
                if (state == CheckState.Indeterminate)
                {
                    backbrush = CreateDitherBrush(colors.highlight, colors.buttonFace);
                }

                try
                {
                    Rectangle bounds = Control.ClientRectangle;
                    if (up)
                    {
                        // We are going to draw a 2 pixel border
                        bounds.Inflate(-borderWidth, -borderWidth);
                    }
                    else
                    {
                        // We are going to draw a 1 pixel border.
                        bounds.Inflate(-1, -1);
                    }

                    PaintButtonBackground(e, bounds, backbrush);
                }
                finally
                {
                    if (backbrush != null)
                    {
                        backbrush.Dispose();
                        backbrush = null;
                    }
                }
            }

            PaintImage(e, layout);
            //inflate the focus rectangle to be consistent with the behavior of Win32 app
            if (Application.RenderWithVisualStyles)
            {
                layout.focus.Inflate(1, 1);
            }

            if (up & IsHighContrastHighlighted())
            {
                Color highlightTextColor = SystemColors.HighlightText;
                PaintField(e, layout, colors, highlightTextColor, false);

                if (Control.Focused && Control.ShowFocusCues)
                {
                    // drawing focus rectangle of HighlightText color
                    ControlPaint.DrawHighContrastFocusRectangle(g, layout.focus, highlightTextColor);
                }
            }
            else if (up & IsHighContrastHighlighted())
            {
                PaintField(e, layout, colors, SystemColors.HighlightText, true);
            }
            else
            {
                PaintField(e, layout, colors, colors.windowText, true);
            }

            if (!Application.RenderWithVisualStyles)
            {
                Rectangle r = Control.ClientRectangle;
                if (Control.IsDefault)
                {
                    r.Inflate(-1, -1);
                }

                DrawDefaultBorder(g, r, colors.windowFrame, Control.IsDefault);

                if (up)
                {
                    Draw3DBorder(g, r, colors, up);
                }
                else
                {
                    // contrary to popular belief, not Draw3DBorder(..., false);
                    //
                    ControlPaint.DrawBorder(g, r, colors.buttonShadow, ButtonBorderStyle.Solid);
                }
            }
        }

        #region Layout

        protected override LayoutOptions Layout(PaintEventArgs e)
        {
            LayoutOptions layout = PaintLayout(e, /* up = */ false);
            Debug.Assert(layout.GetPreferredSizeCore(LayoutUtils.MaxSize) == PaintLayout(e, /* up = */ true).GetPreferredSizeCore(LayoutUtils.MaxSize),
                "The state of up should not effect PreferredSize");
            return layout;
        }

        private LayoutOptions PaintLayout(PaintEventArgs e, bool up)
        {
            LayoutOptions layout = CommonLayout();
            layout.textOffset = !up;
            layout.everettButtonCompat = !Application.RenderWithVisualStyles;

            return layout;
        }

        #endregion
    }
}
