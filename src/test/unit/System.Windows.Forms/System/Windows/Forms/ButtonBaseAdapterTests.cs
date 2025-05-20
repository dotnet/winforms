// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class ButtonBaseAdapterTests
{
    [WinFormsFact]
    public void Button_Calculate_BackColorIsSystemControl()
    {
        using Bitmap bmp = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bmp);
        ColorOptions options = new ColorOptions(graphics, Color.Black, SystemColors.Control)
        {
            Enabled = true
        };
        ColorData data = options.Calculate();

        data.ButtonFace.Should().Be(SystemColors.Control);
        data.ButtonShadow.Should().Be(SystemColors.ControlDark);
        data.ButtonShadowDark.Should().Be(SystemColors.ControlDarkDark);
        data.Highlight.Should().Be(SystemColors.ControlLightLight);
        data.WindowText.Should().Be(data.WindowFrame);
        if (SystemInformation.HighContrast)
        {
            data.WindowDisabled.Should().Be(SystemColors.GrayText);
        }
        else
        {
            data.WindowDisabled.Should().Be(data.ButtonShadow);
        }

        if (data.ButtonFace.GetBrightness() < 0.5f)
        {
            data.ContrastButtonShadow.Should().Be(data.LowHighlight);
        }
        else
        {
            data.ContrastButtonShadow.Should().Be(data.ButtonShadow);
        }
    }

    [WinFormsFact]
    public void Button_Calculate_BackColorNotSystemControl_HighContrastFalse()
    {
        using Bitmap bmp = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bmp);
        Color backColor = Color.FromArgb(100, 120, 140);
        ColorOptions options = new ColorOptions(graphics, Color.Black, backColor)
        {
            Enabled = true
        };
        ColorData data = options.Calculate();

        data.ButtonFace.Should().Be(backColor);
        data.ButtonShadow.Should().Be(ControlPaint.Dark(backColor));
        data.ButtonShadowDark.Should().Be(ControlPaint.DarkDark(backColor));
        data.Highlight.Should().Be(ControlPaint.LightLight(backColor));
        data.WindowText.Should().Be(data.WindowFrame);
        if (SystemInformation.HighContrast)
        {
            data.WindowDisabled.Should().Be(SystemColors.GrayText);
        }
        else
        {
            data.WindowDisabled.Should().Be(data.ButtonShadow);
        }

        if (data.ButtonFace.GetBrightness() < 0.5f)
        {
            data.ContrastButtonShadow.Should().Be(data.LowHighlight);
        }
        else
        {
            data.ContrastButtonShadow.Should().Be(data.ButtonShadow);
        }
    }

    [WinFormsFact]
    public void Button_Calculate_BackColorNotSystemControl_HighContrastTrue()
    {
        if (!SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap bmp = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bmp);
        Color backColor = Color.FromArgb(100, 120, 140);
        ColorOptions options = new ColorOptions(graphics, Color.Black, backColor)
        {
            Enabled = true
        };
        ColorData data = options.Calculate();

        data.ButtonFace.Should().Be(backColor);
        data.ButtonShadow.Should().Be(ControlPaint.Dark(backColor));
        data.ButtonShadowDark.Should().Be(ControlPaint.LightLight(backColor));
        data.Highlight.Should().Be(ControlPaint.LightLight(backColor));
        data.WindowText.Should().Be(data.WindowFrame);
        if (SystemInformation.HighContrast)
        {
            data.WindowDisabled.Should().Be(SystemColors.GrayText);
        }
        else
        {
            data.WindowDisabled.Should().Be(data.ButtonShadow);
        }
    }

    [WinFormsFact]
    public void Button_Calculate_DisabledColors()
    {
        if (!SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap bmp = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bmp);
        Color backColor = Color.FromArgb(100, 120, 140);
        ColorOptions options = new ColorOptions(graphics, Color.Black, backColor)
        {
            Enabled = false
        };
        ColorData data = options.Calculate();

        data.ButtonFace.Should().Be(data.WindowDisabled);
        data.ButtonShadow.Should().Be(data.WindowDisabled);
        data.ButtonShadowDark.Should().Be(data.WindowDisabled);
        data.Highlight.Should().Be(data.WindowDisabled);
        data.WindowFrame.Should().Be(data.WindowDisabled);
        data.WindowText.Should().Be(data.WindowDisabled);
        if (SystemInformation.HighContrast)
        {
            data.WindowDisabled.Should().Be(SystemColors.GrayText);
        }
        else
        {
            data.WindowDisabled.Should().Be(data.ButtonShadow);
        }
    }

    [WinFormsFact]
    public void Button_Calculate_LowButtonFace_And_LowHighlight()
    {
        using Bitmap bmp = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bmp);
        Color backColor = Color.FromArgb(10, 20, 30);
        ColorOptions options = new ColorOptions(graphics, Color.Black, backColor)
        {
            Enabled = true
        };
        ColorData data = options.Calculate();

        float lowlight = 0.1f;
        float adjustDark = 1 + lowlight * 2;
        int expectedR = ColorOptions.Adjust255(adjustDark, data.ButtonFace.R);
        int expectedG = ColorOptions.Adjust255(adjustDark, data.ButtonFace.G);
        int expectedB = ColorOptions.Adjust255(adjustDark, data.ButtonFace.B);
        data.LowButtonFace.Should().Be(Color.FromArgb(expectedR, expectedG, expectedB));

        float adjustHighlight = data.Highlight.GetBrightness() < 0.5f ? 1 + lowlight * 2 : 1 - lowlight;
        int expectedHR = ColorOptions.Adjust255(adjustHighlight, data.Highlight.R);
        int expectedHG = ColorOptions.Adjust255(adjustHighlight, data.Highlight.G);
        int expectedHB = ColorOptions.Adjust255(adjustHighlight, data.Highlight.B);
        data.LowHighlight.Should().Be(Color.FromArgb(expectedHR, expectedHG, expectedHB));
    }
}
