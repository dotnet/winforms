// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

internal abstract class CheckBoxBaseAdapter : CheckableControlBaseAdapter
{
    protected const int FlatCheckSize = 11;

    [ThreadStatic]
    private static Bitmap? t_checkImageChecked;
    [ThreadStatic]
    private static Color t_checkImageCheckedBackColor;

    [ThreadStatic]
    private static Bitmap? t_checkImageIndeterminate;
    [ThreadStatic]
    private static Color t_checkImageIndeterminateBackColor;

    internal CheckBoxBaseAdapter(ButtonBase control) : base(control)
    {
    }

    protected new CheckBox Control => (CheckBox)base.Control;

    protected void DrawCheckFlat(
        PaintEventArgs e,
        LayoutData layout,
        Color checkColor,
        Color checkBackground,
        Color checkBorder,
        ColorData colors)
    {
        Rectangle bounds = layout.CheckBounds;

        // Removed subtracting one for Width and Height. In VS 2003 (Everett) we needed to do this, as we were using
        // GDI+ to draw the border. Now that we are using GDI, this is unnecessary.

        if (!layout.Options.DotNetOneButtonCompat)
        {
            bounds.Width--;
            bounds.Height--;
        }

        using (DeviceContextHdcScope hdc = new(e))
        {
            using CreatePenScope hpen = new(checkBorder);
            hdc.DrawRectangle(bounds, hpen);

            // Now subtract, since the rest of the code is like Everett.
            if (layout.Options.DotNetOneButtonCompat)
            {
                bounds.Width--;
                bounds.Height--;
            }

            bounds.Inflate(-1, -1);
        }

        if (Control.CheckState == CheckState.Indeterminate)
        {
            bounds.Width++;
            bounds.Height++;
            DrawDitheredFill(e.Graphics, colors.ButtonFace, checkBackground, bounds);
        }
        else
        {
            using DeviceContextHdcScope hdc = new(e);
            using CreateBrushScope hbrush = new(checkBackground);

            // Even though we are using GDI here as opposed to GDI+ in VS 2003 (Everett), we still need to add 1.
            bounds.Width++;
            bounds.Height++;
            hdc.FillRectangle(bounds, hbrush);
        }

        DrawCheckOnly(e, layout, colors, checkColor);
    }

    internal static void DrawCheckBackground(
        bool controlEnabled,
        CheckState controlCheckState,
        IDeviceContext deviceContext,
        Rectangle bounds,
        Color checkBackground,
        bool disabledColors)
    {
        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();

        Color color;

        if (!controlEnabled && disabledColors)
        {
            color = SystemColors.Control;
        }
        else if (controlCheckState == CheckState.Indeterminate && checkBackground == SystemColors.Window && disabledColors)
        {
            Color comboColor = SystemInformation.HighContrast ? SystemColors.ControlDark : SystemColors.Control;
            color = Color.FromArgb(
                (byte)((comboColor.R + SystemColors.Window.R) / 2),
                (byte)((comboColor.G + SystemColors.Window.G) / 2),
                (byte)((comboColor.B + SystemColors.Window.B) / 2));
        }
        else
        {
            color = checkBackground;
        }

        using CreateBrushScope hbrush = new(color);

        RECT rect = bounds;
        PInvoke.FillRect(hdc, rect, hbrush);
    }

    protected void DrawCheckBackground(
        PaintEventArgs e,
        Rectangle bounds,
        Color checkBackground,
        bool disabledColors,
        ColorData colors)
    {
        // Area behind check

        if (Control.CheckState == CheckState.Indeterminate)
        {
            DrawDitheredFill(e.GraphicsInternal, colors.ButtonFace, checkBackground, bounds);
        }
        else
        {
            DrawCheckBackground(Control.Enabled, Control.CheckState, e, bounds, checkBackground, disabledColors);
        }
    }

    protected void DrawCheckOnly(PaintEventArgs e, LayoutData layout, ColorData colors, Color checkColor) =>
        DrawCheckOnly(
            FlatCheckSize,
            Control.Checked,
            Control.Enabled,
            Control.CheckState,
            e.GraphicsInternal,
            layout,
            colors,
            checkColor);

    internal static void DrawCheckOnly(
        int checkSize,
        bool controlChecked,
        bool controlEnabled,
        CheckState controlCheckState,
        Graphics g,
        LayoutData layout,
        ColorData colors,
        Color checkColor)
    {
        if (!controlChecked)
        {
            return;
        }

        if (!controlEnabled)
        {
            checkColor = colors.ButtonShadow;
        }
        else if (controlCheckState == CheckState.Indeterminate)
        {
            checkColor = SystemInformation.HighContrast ? colors.Highlight : colors.ButtonShadow;
        }

        Rectangle fullSize = layout.CheckBounds;

        if (fullSize.Width == checkSize)
        {
            fullSize.Width++;
            fullSize.Height++;
        }

        fullSize.Width++;

        fullSize.Height++;
        Bitmap checkImage;

        if (controlCheckState == CheckState.Checked)
        {
            checkImage = GetCheckBoxImage(checkColor, fullSize, ref t_checkImageCheckedBackColor, ref t_checkImageChecked);
        }
        else
        {
            Debug.Assert(
                controlCheckState == CheckState.Indeterminate,
                "we want to paint the check box only if the item is checked or indeterminate");
            checkImage = GetCheckBoxImage(checkColor, fullSize, ref t_checkImageIndeterminateBackColor, ref t_checkImageIndeterminate);
        }

        fullSize.Y -= layout.Options.DotNetOneButtonCompat ? 1 : 2;

        ControlPaint.DrawImageColorized(g, checkImage, fullSize, checkColor);
    }

    internal static Rectangle DrawPopupBorder(Graphics g, Rectangle r, ColorData colors)
    {
        using DeviceContextHdcScope hdc = new(g);
        return DrawPopupBorder(hdc, r, colors);
    }

    internal static Rectangle DrawPopupBorder(PaintEventArgs e, Rectangle r, ColorData colors)
    {
        using DeviceContextHdcScope hdc = new(e);
        return DrawPopupBorder(hdc, r, colors);
    }

    internal static Rectangle DrawPopupBorder(HDC hdc, Rectangle r, ColorData colors)
    {
        using CreatePenScope high = new(colors.Highlight);
        using CreatePenScope shadow = new(colors.ButtonShadow);
        using CreatePenScope face = new(colors.ButtonFace);

        hdc.DrawLine(high, r.Right - 1, r.Top, r.Right - 1, r.Bottom);
        hdc.DrawLine(high, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);

        hdc.DrawLine(shadow, r.Left, r.Top, r.Left, r.Bottom);
        hdc.DrawLine(shadow, r.Left, r.Top, r.Right - 1, r.Top);

        hdc.DrawLine(face, r.Right - 2, r.Top + 1, r.Right - 2, r.Bottom - 1);
        hdc.DrawLine(face, r.Left + 1, r.Bottom - 2, r.Right - 1, r.Bottom - 2);

        r.Inflate(-1, -1);
        return r;
    }

    protected ButtonState GetState()
    {
        ButtonState style = 0;

        if (Control.CheckState == CheckState.Unchecked)
        {
            style |= ButtonState.Normal;
        }
        else
        {
            style |= ButtonState.Checked;
        }

        if (!Control.Enabled)
        {
            style |= ButtonState.Inactive;
        }

        if (Control.MouseIsDown)
        {
            style |= ButtonState.Pushed;
        }

        return style;
    }

    protected void DrawCheckBox(PaintEventArgs e, LayoutData layout)
    {
        ButtonState style = GetState();

        if (Control.CheckState == CheckState.Indeterminate)
        {
            if (Application.RenderWithVisualStyles)
            {
                CheckBoxRenderer.DrawCheckBoxWithVisualStyles(
                    e,
                    new Point(layout.CheckBounds.Left, layout.CheckBounds.Top),
                    CheckBoxRenderer.ConvertFromButtonState(style, isMixed: true, Control.MouseIsOver),
                    Control.HWNDInternal);
            }
            else
            {
                ControlPaint.DrawMixedCheckBox(e.GraphicsInternal, layout.CheckBounds, style);
            }
        }
        else
        {
            if (Application.RenderWithVisualStyles)
            {
                CheckBoxRenderer.DrawCheckBoxWithVisualStyles(
                    e,
                    new Point(layout.CheckBounds.Left, layout.CheckBounds.Top),
                    CheckBoxRenderer.ConvertFromButtonState(style, isMixed: false, Control.MouseIsOver),
                    Control.HWNDInternal);
            }
            else
            {
                ControlPaint.DrawCheckBox(e.GraphicsInternal, layout.CheckBounds, style);
            }
        }
    }

    private static Bitmap GetCheckBoxImage(Color checkColor, Rectangle fullSize, ref Color cacheCheckColor, ref Bitmap? cacheCheckImage)
    {
        if (cacheCheckImage is not null
            && cacheCheckColor.Equals(checkColor)
            && cacheCheckImage.Width == fullSize.Width
            && cacheCheckImage.Height == fullSize.Height)
        {
            return cacheCheckImage;
        }

        cacheCheckImage?.Dispose();

        // We draw the checkmark slightly off center to eliminate 3-D border artifacts and compensate below
        RECT rcCheck = new Rectangle(0, 0, fullSize.Width, fullSize.Height);
        Bitmap bitmap = new(fullSize.Width, fullSize.Height);

        using (Graphics offscreen = Graphics.FromImage(bitmap))
        {
            offscreen.Clear(Color.Transparent);
            using DeviceContextHdcScope hdc = new(offscreen, applyGraphicsState: false);
            PInvoke.DrawFrameControl(
                hdc,
                ref rcCheck,
                DFC_TYPE.DFC_MENU,
                DFCS_STATE.DFCS_MENUCHECK);
        }

        bitmap.MakeTransparent();
        cacheCheckImage = bitmap;
        cacheCheckColor = checkColor;

        return cacheCheckImage;
    }

    protected void AdjustFocusRectangle(LayoutData layout)
    {
        if (string.IsNullOrEmpty(Control.Text))
        {
            // When a CheckBox has no text, AutoSize sets the size to zero and thus there's no place around which
            // to draw the focus rectangle. So, when AutoSize == true we want the focus rectangle to be rendered
            // inside the box. Otherwise, it should encircle all the available space next to the box (like it's
            // done in WPF and ComCtl32).
            layout.Focus = Control.AutoSize ? Rectangle.Inflate(layout.CheckBounds, -2, -2) : layout.Field;
        }
    }

    internal override LayoutOptions CommonLayout()
    {
        LayoutOptions layout = base.CommonLayout();
        layout.CheckAlign = Control.CheckAlign;
        layout.TextOffset = false;
        layout.ShadowedText = !Control.Enabled;
        layout.LayoutRTL = Control.RightToLeft == RightToLeft.Yes;

        return layout;
    }
}
