// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonStandardAdapter : ButtonBaseAdapter
{
    private const int BorderWidth = 2;

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

    internal override void PaintUp(PaintEventArgs e, CheckState state) => PaintWorker(e, up: true, state);

    internal override void PaintDown(PaintEventArgs e, CheckState state) => PaintWorker(e, up: false, state);

    internal override void PaintOver(PaintEventArgs e, CheckState state) => PaintUp(e, state);

    private void PaintThemedButtonBackground(PaintEventArgs e, Rectangle bounds, bool up)
    {
        PushButtonState pbState = DetermineState(up);

        // First handle transparent case.
        if (ButtonRenderer.IsBackgroundPartiallyTransparent(pbState))
        {
            ButtonRenderer.DrawParentBackground(e, bounds, Control);
        }

        ButtonRenderer.DrawButtonForHandle(
            e,
            Control.ClientRectangle,
            focused: false,
            pbState,
            ScaleHelper.IsScalingRequirementMet ? Control.HWNDInternal : HWND.Null);

        // Now overlay the background image or color (the former overrides the latter), leaving a margin.
        // We hardcode this margin for now since GetThemeMargins returns 0 all the time.
        //
        // Changing this because GetThemeMargins simply does not work in some cases.
        bounds.Inflate(-ButtonBorderSize, -ButtonBorderSize);

        // Only paint if the user said not to use the themed background color.
        if (!Control.UseVisualStyleBackColor)
        {
            bool isHighContrastHighlighted = up && IsHighContrastHighlighted();
            Color color = isHighContrastHighlighted ? SystemColors.Highlight : Control.BackColor;

            if (color.HasTransparency())
            {
                using var brush = color.GetCachedSolidBrushScope();
                e.GraphicsInternal.FillRectangle(brush, bounds);
            }
            else
            {
                using DeviceContextHdcScope hdc = new(e);
                hdc.FillRectangle(
                    bounds,
                    isHighContrastHighlighted
                        ? PInvoke.GetSysColorBrush(SYS_COLOR_INDEX.COLOR_HIGHLIGHT)
                        : Control.BackColorBrush);
            }
        }

        // This code is mostly taken from the non-themed rendering code path.
        if (Control.BackgroundImage is not null && !DisplayInformation.HighContrast)
        {
            ControlPaint.DrawBackgroundImage(
                e.GraphicsInternal,
                Control.BackgroundImage,
                Color.Transparent,
                Control.BackgroundImageLayout,
                Control.ClientRectangle,
                bounds,
                Control.DisplayRectangle.Location,
                Control.RightToLeft);
        }
    }

    private void PaintWorker(PaintEventArgs e, bool up, CheckState state)
    {
        up = up && state == CheckState.Unchecked;

        ColorData colors = PaintRender(e).Calculate();
        LayoutData layout;
        if (Application.RenderWithVisualStyles)
        {
            // Don't have the text-pressed-down effect when we use themed painting to be consistent with Win32.
            layout = PaintLayout(up: true).Layout();
        }
        else
        {
            layout = PaintLayout(up).Layout();
        }

        _ = Control as Button;
        if (Application.RenderWithVisualStyles)
        {
            PaintThemedButtonBackground(e, Control.ClientRectangle, up);
        }
        else
        {
            Brush? backgroundBrush = null;
            if (state == CheckState.Indeterminate)
            {
                backgroundBrush = CreateDitherBrush(colors.Highlight, colors.ButtonFace);
            }

            try
            {
                Rectangle bounds = Control.ClientRectangle;
                if (up)
                {
                    // We are going to draw a 2 pixel border
                    bounds.Inflate(-BorderWidth, -BorderWidth);
                }
                else
                {
                    // We are going to draw a 1 pixel border.
                    bounds.Inflate(-1, -1);
                }

                PaintButtonBackground(e, bounds, backgroundBrush);
            }
            finally
            {
                backgroundBrush?.Dispose();
            }
        }

        PaintImage(e, layout);

        // Inflate the focus rectangle to be consistent with the behavior of Win32 app
        if (Application.RenderWithVisualStyles && Control.FlatStyle != FlatStyle.Standard)
        {
            layout.Focus.Inflate(1, 1);
        }

        if (up & IsHighContrastHighlighted())
        {
            Color highlightTextColor = SystemColors.HighlightText;
            PaintField(e, layout, colors, highlightTextColor, drawFocus: false);

            if (Control.Focused && Control.ShowFocusCues)
            {
                // Drawing focus rectangle of HighlightText color
                ControlPaint.DrawHighContrastFocusRectangle(e.GraphicsInternal, layout.Focus, highlightTextColor);
            }
        }
        else if (up & IsHighContrastHighlighted())
        {
            PaintField(e, layout, colors, SystemColors.HighlightText, drawFocus: true);
        }
        else
        {
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }

        if (!Application.RenderWithVisualStyles)
        {
            Rectangle r = Control.ClientRectangle;
            if (Control.IsDefault)
            {
                r.Inflate(-1, -1);
            }

            DrawDefaultBorder(e, r, colors.WindowFrame, Control.IsDefault);

            if (up)
            {
                Draw3DBorder(e, r, colors, raised: up);
            }
            else
            {
                // Not Draw3DBorder(..., raised: false);
                ControlPaint.DrawBorderSimple(e, r, colors.ButtonShadow);
            }
        }
    }

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = PaintLayout(up: false);
        Debug.Assert(
            layout.GetPreferredSizeCore(LayoutUtils.s_maxSize) == PaintLayout(up: true).GetPreferredSizeCore(LayoutUtils.s_maxSize),
            "The state of up should not effect PreferredSize");
        return layout;
    }

    private LayoutOptions PaintLayout(bool up)
    {
        LayoutOptions layout = CommonLayout();
        layout.TextOffset = !up;
        layout.DotNetOneButtonCompat = !Application.RenderWithVisualStyles;

        return layout;
    }
}
