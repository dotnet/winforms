// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal;

internal sealed class CheckBoxStandardAdapter : CheckBoxBaseAdapter
{
    internal CheckBoxStandardAdapter(ButtonBase control) : base(control)
    {
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintUp(e, Control.CheckState);
        }
        else
        {
            ColorData colors = PaintRender(e).Calculate();
            LayoutData layout = Layout(e).Layout();
            PaintButtonBackground(e, Control.ClientRectangle, null);

            if (!layout.Options.DotNetOneButtonCompat)
            {
                layout.TextBounds.Offset(-1, -1);
            }

            layout.ImageBounds.Offset(-1, -1);

            AdjustFocusRectangle(layout);

            if (!string.IsNullOrEmpty(Control.Text))
            {
                // Minor adjustment to make sure the appearance is exactly the same as Win32 app.
                int focusRectFixup = layout.Focus.X & 0x1; // if it's odd, subtract one pixel for fixup.
                if (!Application.RenderWithVisualStyles)
                {
                    focusRectFixup = 1 - focusRectFixup;
                }

                layout.Focus.Offset(-(focusRectFixup + 1), -2);
                layout.Focus.Width = layout.TextBounds.Width + layout.ImageBounds.Width - 1;
                layout.Focus.Intersect(layout.TextBounds);

                if (layout.Options.TextAlign != LayoutUtils.AnyLeft
                    && layout.Options.UseCompatibleTextRendering
                    && layout.Options.Font.Italic)
                {
                    // Fixup for GDI+ text rendering.
                    layout.Focus.Width += 2;
                }
            }

            PaintImage(e, layout);
            DrawCheckBox(e, layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintDown(e, Control.CheckState);
        }
        else
        {
            PaintUp(e, state);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintOver(e, Control.CheckState);
        }
        else
        {
            PaintUp(e, state);
        }
    }

    internal override Size GetPreferredSizeCore(Size proposedSize)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonStandardAdapter adapter = new(Control);
            return adapter.GetPreferredSizeCore(proposedSize);
        }
        else
        {
            LayoutOptions? options = default;
            using (var screen = GdiCache.GetScreenHdc())
            using (PaintEventArgs pe = new(screen, clipRect: default))
            {
                options = Layout(pe);
            }

            return options.GetPreferredSizeCore(proposedSize);
        }
    }

    private new ButtonStandardAdapter ButtonAdapter => (ButtonStandardAdapter)base.ButtonAdapter;

    protected override ButtonBaseAdapter CreateButtonAdapter() => new ButtonStandardAdapter(Control);

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = CommonLayout();
        layout.CheckPaddingSize = 1;
        layout.DotNetOneButtonCompat = !Application.RenderWithVisualStyles;

        if (Application.RenderWithVisualStyles)
        {
            using var screen = GdiCache.GetScreenHdc();
            layout.CheckSize = CheckBoxRenderer.GetGlyphSize(
                screen,
                CheckBoxRenderer.ConvertFromButtonState(
                    GetState(),
                    isMixed: true,
                    Control.MouseIsOver),
                Control.HWNDInternal).Width;
        }
        else
        {
            layout.CheckSize = ScaleHelper.IsThreadPerMonitorV2Aware
                ? Control.LogicalToDeviceUnits(layout.CheckSize)
                : (int)(layout.CheckSize * GetDpiScaleRatio());
        }

        return layout;
    }
}
