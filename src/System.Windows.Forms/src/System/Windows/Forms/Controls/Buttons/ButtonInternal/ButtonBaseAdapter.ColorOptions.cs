// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

internal abstract partial class ButtonBaseAdapter
{
    internal class ColorOptions
    {
        private readonly Color _backColor;
        private readonly Color _foreColor;
        private readonly IDeviceContext _deviceContext;

        public bool Enabled { get; set; }
        public bool HighContrast { get; }

        internal ColorOptions(IDeviceContext deviceContext, Color foreColor, Color backColor)
        {
            _deviceContext = deviceContext;
            _backColor = backColor;
            _foreColor = foreColor;
            HighContrast = SystemInformation.HighContrast;
        }

        internal static int Adjust255(float percentage, int value)
        {
            int result = (int)(percentage * value);
            return result > 255 ? 255 : result;
        }

        internal ColorData Calculate()
        {
            ColorData colors = new(this)
            {
                ButtonFace = _backColor
            };

            if (_backColor == Application.ApplicationColors.Control)
            {
                colors.ButtonShadow = Application.ApplicationColors.ControlDark;
                colors.ButtonShadowDark = Application.ApplicationColors.ControlDarkDark;
                colors.Highlight = Application.ApplicationColors.ControlLightLight;
            }
            else
            {
                if (!HighContrast)
                {
                    colors.ButtonShadow = ControlPaint.Dark(_backColor);
                    colors.ButtonShadowDark = ControlPaint.DarkDark(_backColor);
                    colors.Highlight = ControlPaint.LightLight(_backColor);
                }
                else
                {
                    colors.ButtonShadow = ControlPaint.Dark(_backColor);
                    colors.ButtonShadowDark = ControlPaint.LightLight(_backColor);
                    colors.Highlight = ControlPaint.LightLight(_backColor);
                }
            }

            colors.WindowDisabled = HighContrast ? Application.ApplicationColors.GrayText : colors.ButtonShadow;

            const float lowlight = .1f;
            float adjust = 1 - lowlight;

            if (colors.ButtonFace.GetBrightness() < .5)
            {
                adjust = 1 + lowlight * 2;
            }

            colors.LowButtonFace = Color.FromArgb(
                Adjust255(adjust, colors.ButtonFace.R),
                Adjust255(adjust, colors.ButtonFace.G),
                Adjust255(adjust, colors.ButtonFace.B));

            adjust = 1 - lowlight;
            if (colors.Highlight.GetBrightness() < .5)
            {
                adjust = 1 + lowlight * 2;
            }

            colors.LowHighlight = Color.FromArgb(
                Adjust255(adjust, colors.Highlight.R),
                Adjust255(adjust, colors.Highlight.G),
                Adjust255(adjust, colors.Highlight.B));

            if (HighContrast && _backColor != Application.ApplicationColors.Control)
            {
                colors.Highlight = colors.LowHighlight;
            }

            colors.WindowFrame = _foreColor;

            colors.ContrastButtonShadow = colors.ButtonFace.GetBrightness() < .5 ? colors.LowHighlight : colors.ButtonShadow;

            if (!Enabled)
            {
                colors.WindowText = colors.WindowDisabled;
                if (HighContrast)
                {
                    colors.WindowFrame = colors.WindowDisabled;
                    colors.ButtonShadow = colors.WindowDisabled;
                }
            }
            else
            {
                colors.WindowText = colors.WindowFrame;
            }

            using DeviceContextHdcScope hdc = _deviceContext.ToHdcScope(ApplyGraphicsProperties.None);

            colors.ButtonFace = hdc.FindNearestColor(colors.ButtonFace);
            colors.ButtonShadow = hdc.FindNearestColor(colors.ButtonShadow);
            colors.ButtonShadowDark = hdc.FindNearestColor(colors.ButtonShadowDark);
            colors.ContrastButtonShadow = hdc.FindNearestColor(colors.ContrastButtonShadow);
            colors.WindowText = hdc.FindNearestColor(colors.WindowText);
            colors.Highlight = hdc.FindNearestColor(colors.Highlight);
            colors.LowHighlight = hdc.FindNearestColor(colors.LowHighlight);
            colors.LowButtonFace = hdc.FindNearestColor(colors.LowButtonFace);
            colors.WindowFrame = hdc.FindNearestColor(colors.WindowFrame);
            colors.WindowDisabled = hdc.FindNearestColor(colors.WindowDisabled);

            return colors;
        }
    }
}
