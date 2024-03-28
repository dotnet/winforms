// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public static partial class ControlPaint
{
    /// <summary>
    ///  Logic copied from Windows sources to copy the lightening and darkening of colors.
    /// </summary>
    private readonly struct HLSColor : IEquatable<HLSColor>
    {
        private const int ShadowAdjustment = -333;
        private const int HighlightAdjustment = 500;

        private const int Range = 240;
        private const int HLSMax = Range;
        private const int RGBMax = 255;
        private const int Undefined = HLSMax * 2 / 3;

        private readonly int _hue;
        private readonly int _saturation;
        private readonly bool _isSystemColors_Control;

        public HLSColor(Color color)
        {
            _isSystemColors_Control = color.ToKnownColor() == SystemColors.Control.ToKnownColor();

            ARGB argb = color;
            int r = argb.R;
            int g = argb.G;
            int b = argb.B;
            int Rdelta, Gdelta, Bdelta;  // intermediate value: % of spread from max

            // calculate lightness
            int max = Math.Max(Math.Max(r, g), b);
            int min = Math.Min(Math.Min(r, g), b);
            int sum = max + min;

            Luminosity = ((sum * HLSMax) + RGBMax) / (2 * RGBMax);

            int dif = max - min;
            if (dif == 0)
            {
                // r=g=b --> achromatic case
                _saturation = 0;
                _hue = Undefined;
            }
            else
            {
                // chromatic case
                _saturation = Luminosity <= (HLSMax / 2)
                    ? ((dif * HLSMax) + (sum / 2)) / sum
                    : ((dif * HLSMax) + (2 * RGBMax - sum) / 2) / (2 * RGBMax - sum);

                Rdelta = (((max - r) * (HLSMax / 6)) + (dif / 2)) / dif;
                Gdelta = (((max - g) * (HLSMax / 6)) + (dif / 2)) / dif;
                Bdelta = (((max - b) * (HLSMax / 6)) + (dif / 2)) / dif;

                if (r == max)
                {
                    _hue = Bdelta - Gdelta;
                }
                else if (g == max)
                {
                    _hue = (HLSMax / 3) + Rdelta - Bdelta;
                }
                else
                {
                    // B == cMax
                    _hue = (2 * HLSMax / 3) + Gdelta - Rdelta;
                }

                if (_hue < 0)
                {
                    _hue += HLSMax;
                }

                if (_hue > HLSMax)
                {
                    _hue -= HLSMax;
                }
            }
        }

        public int Luminosity { get; }

        public Color Darker(float percDarker)
        {
            if (!_isSystemColors_Control)
            {
                int zeroLum = NewLuma(ShadowAdjustment, true);
                return ColorFromHLS(_hue, zeroLum - (int)(zeroLum * percDarker), _saturation);
            }
            else
            {
                // With the usual color scheme, ControlDark/DarkDark is not exactly
                // what we would otherwise calculate
                if (percDarker == 0.0f)
                {
                    return SystemColors.ControlDark;
                }
                else if (percDarker == 1.0f)
                {
                    return SystemColors.ControlDarkDark;
                }
                else
                {
                    ARGB dark = SystemColors.ControlDark;
                    ARGB darkDark = SystemColors.ControlDarkDark;

                    return Color.FromArgb(
                        (byte)(dark.R - (byte)((dark.R - darkDark.R) * percDarker)),
                        (byte)(dark.G - (byte)((dark.G - darkDark.G) * percDarker)),
                        (byte)(dark.B - (byte)((dark.B - darkDark.B) * percDarker)));
                }
            }
        }

        public override bool Equals(object? o)
        {
            if (o is not HLSColor hlsColor)
            {
                return false;
            }

            return Equals(hlsColor);
        }

        public bool Equals(HLSColor other)
        {
            return _hue == other._hue
                && _saturation == other._saturation
                && Luminosity == other.Luminosity
                && _isSystemColors_Control == other._isSystemColors_Control;
        }

        public static bool operator ==(HLSColor a, HLSColor b) => a.Equals(b);

        public static bool operator !=(HLSColor a, HLSColor b) => !a.Equals(b);

        public override int GetHashCode() => HashCode.Combine(_hue, _saturation, Luminosity);

        public Color Lighter(float percentLighter)
        {
            if (_isSystemColors_Control)
            {
                // With the usual color scheme, ControlLight/LightLight is not exactly
                // what we would otherwise calculate
                if (percentLighter == 0.0f)
                {
                    return SystemColors.ControlLight;
                }
                else if (percentLighter == 1.0f)
                {
                    return SystemColors.ControlLightLight;
                }
                else
                {
                    ARGB light = SystemColors.ControlLight;
                    ARGB lightLight = SystemColors.ControlLightLight;

                    return Color.FromArgb(
                        (byte)(light.R - (byte)((light.R - lightLight.R) * percentLighter)),
                        (byte)(light.G - (byte)((light.G - lightLight.G) * percentLighter)),
                        (byte)(light.B - (byte)((light.B - lightLight.B) * percentLighter)));
                }
            }
            else
            {
                int zeroLum = Luminosity;
                int oneLum = NewLuma(HighlightAdjustment, true);
                return ColorFromHLS(_hue, zeroLum + (int)((oneLum - zeroLum) * percentLighter), _saturation);
            }
        }

        private int NewLuma(int n, bool scale)
        {
            return NewLuma(Luminosity, n, scale);
        }

        private static int NewLuma(int luminosity, int n, bool scale)
        {
            if (n == 0)
            {
                return luminosity;
            }

            if (scale)
            {
                return n > 0
                    ? (int)((luminosity * (1000 - n) + (Range + 1L) * n) / 1000)
                    : luminosity * (n + 1000) / 1000;
            }

            luminosity += (int)((long)n * Range / 1000);

            if (luminosity < 0)
            {
                return 0;
            }
            else if (luminosity > HLSMax)
            {
                return HLSMax;
            }

            return luminosity;
        }

        private static Color ColorFromHLS(int hue, int luminosity, int saturation)
        {
            byte r, g, b;
            int magic1, magic2;

            if (saturation == 0)
            {
                // achromatic case
                r = g = b = (byte)(luminosity * RGBMax / HLSMax);
                if (hue != Undefined)
                {
                    /* ERROR */
                }
            }
            else
            {
                // chromatic case
                if (luminosity <= (HLSMax / 2))
                {
                    magic2 = ((luminosity * (HLSMax + saturation)) + (HLSMax / 2)) / HLSMax;
                }
                else
                {
                    magic2 = luminosity + saturation - (((luminosity * saturation) + (HLSMax / 2)) / HLSMax);
                }

                magic1 = 2 * luminosity - magic2;

                // get RGB, change units from HLSMax to RGBMax
                r = (byte)((HueToRGB(magic1, magic2, hue + HLSMax / 3) * RGBMax + (HLSMax / 2)) / HLSMax);
                g = (byte)((HueToRGB(magic1, magic2, hue) * RGBMax + (HLSMax / 2)) / HLSMax);
                b = (byte)((HueToRGB(magic1, magic2, hue - HLSMax / 3) * RGBMax + (HLSMax / 2)) / HLSMax);
            }

            return Color.FromArgb(r, g, b);
        }

        private static int HueToRGB(int n1, int n2, int hue)
        {
            // range check: note values passed add/subtract thirds of range

            /* The following is redundant for WORD (unsigned int) */
            if (hue < 0)
            {
                hue += HLSMax;
            }

            if (hue > HLSMax)
            {
                hue -= HLSMax;
            }

            // return r, g, or b value from this sector
            if (hue < (HLSMax / 6))
            {
                return n1 + (((n2 - n1) * hue + (HLSMax / 12)) / (HLSMax / 6));
            }

            if (hue < (HLSMax / 2))
            {
                return n2;
            }

            if (hue < (HLSMax * 2 / 3))
            {
                return n1 + (((n2 - n1) * ((HLSMax * 2 / 3) - hue) + (HLSMax / 12)) / (HLSMax / 6));
            }
            else
            {
                return n1;
            }
        }
    }
}
