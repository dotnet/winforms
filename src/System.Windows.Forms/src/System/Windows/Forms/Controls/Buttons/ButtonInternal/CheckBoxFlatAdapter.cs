// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

internal class CheckBoxFlatAdapter : CheckBoxBaseAdapter
{
    internal CheckBoxFlatAdapter(ButtonBase control)
        : base(control)
    {
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintDown(e, Control.CheckState);
            return;
        }

        ColorData colors = PaintFlatRender(e).Calculate();
        if (Control.Enabled)
        {
            PaintFlatWorker(e, colors.WindowText, colors.Highlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintOver(e, Control.CheckState);
            return;
        }

        ColorData colors = PaintFlatRender(e).Calculate();
        if (Control.Enabled)
        {
            PaintFlatWorker(e, colors.WindowText, colors.LowHighlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
        }
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintUp(e, Control.CheckState);
            return;
        }

        ColorData colors = PaintFlatRender(e).Calculate();
        if (Control.Enabled)
        {
            PaintFlatWorker(e, colors.WindowText, colors.Highlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
        }
    }

    private void PaintFlatWorker(PaintEventArgs e, Color checkColor, Color checkBackground, Color checkBorder, ColorData colors)
    {
        LayoutData layout = Layout(e).Layout();
        PaintButtonBackground(e, Control.ClientRectangle, null);

        PaintImage(e, layout);
        DrawCheckFlat(
            e,
            layout,
            checkColor,
            colors.Options.HighContrast ? colors.ButtonFace : checkBackground,
            checkBorder,
            colors);

        AdjustFocusRectangle(layout);
        PaintField(e, layout, colors, checkColor, true);
    }

    #region Layout

    private new ButtonFlatAdapter ButtonAdapter
    {
        get
        {
            return ((ButtonFlatAdapter)base.ButtonAdapter);
        }
    }

    protected override ButtonBaseAdapter CreateButtonAdapter()
    {
        return new ButtonFlatAdapter(Control);
    }

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = CommonLayout();
        layout.CheckSize = (int)(FlatCheckSize * GetDpiScaleRatio());
        layout.ShadowedText = false;

        return layout;
    }

    #endregion
}
