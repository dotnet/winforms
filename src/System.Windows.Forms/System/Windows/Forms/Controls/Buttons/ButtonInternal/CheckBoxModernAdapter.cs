// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

/// <summary>
///  Paints a normal-appearance <see cref="CheckBox"/> with the modern animated glyph renderer.
/// </summary>
internal sealed class CheckBoxModernAdapter : CheckBoxBaseAdapter
{
    private readonly FlatStyle _flatStyle;

    internal CheckBoxModernAdapter(CheckBox control, FlatStyle flatStyle) : base(control)
    {
        _flatStyle = flatStyle;
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintUp(e, Control.CheckState);
            return;
        }

        PaintCore(e);
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintDown(e, Control.CheckState);
            return;
        }

        PaintCore(e);
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintOver(e, Control.CheckState);
            return;
        }

        PaintCore(e);
    }

    protected override ButtonBaseAdapter CreateButtonAdapter()
        => _flatStyle switch
        {
            FlatStyle.Flat => DarkModeAdapterFactory.CreateFlatAdapter(Control),
            FlatStyle.Popup => DarkModeAdapterFactory.CreatePopupAdapter(Control),
            _ => DarkModeAdapterFactory.CreateStandardAdapter(Control)
        };

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = CommonLayout();
        layout.CheckPaddingSize = Control.LogicalToDeviceUnits(2);
        layout.CheckSize = Math.Max(
            Control.LogicalToDeviceUnits(13),
            (int)(Control.Font.Height * 0.9f));

        return layout;
    }

    internal override LayoutOptions CommonLayout()
    {
        LayoutOptions layout = base.CommonLayout();
        layout.ShadowedText = false;

        return layout;
    }

    private void PaintCore(PaintEventArgs e)
    {
        Graphics graphics = e.GraphicsInternal;
        ParentBackgroundRenderer.Paint(
            Control,
            graphics,
            Control.ClientRectangle,
            Control.BackColor);

        LayoutData layout = Layout(e).Layout();
        AdjustFocusRectangle(layout);
        PaintBackgroundImage(e);

        Color? customOnColor = Control.ShouldSerializeBackColor()
            ? Control.BackColor
            : null;

        Color? customBorderColor = Control.FlatAppearance.BorderColor.IsEmpty
            ? null
            : Control.FlatAppearance.BorderColor;

        Control.CheckGlyphRenderer.NotifyCheckStateChanged(Control.CheckState);
        Control.CheckGlyphRenderer.DrawGlyph(
            graphics,
            layout.CheckBounds,
            _flatStyle,
            Control.Enabled,
            Control.MouseIsOver,
            Control.Focused && Control.ShowFocusCues,
            customOnColor,
            customBorderColor);

        PaintImage(e, layout);

        Color preferredTextColor = Control.ShouldSerializeForeColor()
            ? Control.ForeColor
            : Application.IsDarkModeEnabled
                ? Color.FromArgb(0xF0, 0xF0, 0xF0)
                : SystemColors.WindowText;
        Color textColor = Control.Enabled
            ? preferredTextColor
            : ModernControlColorMath.GetDisabledTextColor(
                preferredTextColor,
                Control.Parent?.BackColor ?? Control.BackColor);

        PaintField(e, layout, PaintRender(e).Calculate(), textColor, drawFocus: true);
    }
}
