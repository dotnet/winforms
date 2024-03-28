// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal;

internal class CheckBoxPopupAdapter : CheckBoxBaseAdapter
{
    internal CheckBoxPopupAdapter(ButtonBase control) : base(control)
    {
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintUp(e, Control.CheckState);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = PaintPopupLayout(show3D: false).Layout();

            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);

            DrawCheckBackground(
                e,
                layout.CheckBounds,
                colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight,
                disabledColors: true,
                colors);

            ControlPaint.DrawBorderSimple(
                e,
                layout.CheckBounds,
                (colors.Options.HighContrast && !Control.Enabled) ? colors.WindowFrame : colors.ButtonShadow);

            DrawCheckOnly(e, layout, colors, colors.WindowText);

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintOver(e, Control.CheckState);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = PaintPopupLayout(show3D: true).Layout();

            Control.PaintBackground(e, Control.ClientRectangle);

            PaintImage(e, layout);

            DrawCheckBackground(
                e,
                layout.CheckBounds,
                colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight,
                disabledColors: true,
                colors);

            DrawPopupBorder(e, layout.CheckBounds, colors);
            DrawCheckOnly(e, layout, colors, colors.WindowText);

            Region? originalClip = null;
            if (!string.IsNullOrEmpty(Control.Text))
            {
                originalClip = e.GraphicsInternal.Clip;
                e.GraphicsInternal.ExcludeClip(layout.CheckArea);
            }

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);

            if (originalClip is not null)
            {
                e.GraphicsInternal.Clip = originalClip;
            }
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintDown(e, Control.CheckState);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = PaintPopupLayout(show3D: true).Layout();

            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);

            DrawCheckBackground(e, layout.CheckBounds, colors.ButtonFace, disabledColors: true, colors);
            DrawPopupBorder(e, layout.CheckBounds, colors);
            DrawCheckOnly(e, layout, colors, colors.WindowText);

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    protected override ButtonBaseAdapter CreateButtonAdapter() => new ButtonPopupAdapter(Control);

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = PaintPopupLayout(show3D: true);
        Debug.Assert(
            layout.GetPreferredSizeCore(LayoutUtils.s_maxSize) == PaintPopupLayout(show3D: false).GetPreferredSizeCore(LayoutUtils.s_maxSize),
            "The state of show3D should not effect PreferredSize");
        return layout;
    }

    internal static LayoutOptions PaintPopupLayout(
        bool show3D,
        int checkSize,
        Rectangle clientRectangle,
        Padding padding,
        bool isDefault,
        Font font,
        string text,
        bool enabled,
        ContentAlignment textAlign,
        RightToLeft rtl,
        Control? control = null)
    {
        LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
        layout.ShadowedText = false;
        checkSize = (int)(checkSize * GetDpiScaleRatio(control));

        if (show3D)
        {
            layout.CheckSize = checkSize + 1;
        }
        else
        {
            layout.CheckSize = checkSize;
            layout.CheckPaddingSize = 1;
        }

        return layout;
    }

    private LayoutOptions PaintPopupLayout(bool show3D)
    {
        LayoutOptions layout = CommonLayout();
        layout.ShadowedText = false;
        int checkSize = (int)(FlatCheckSize * GetDpiScaleRatio());

        if (show3D)
        {
            layout.CheckSize = checkSize + 1;
        }
        else
        {
            layout.CheckSize = checkSize;
            layout.CheckPaddingSize = 1;
        }

        return layout;
    }
}
