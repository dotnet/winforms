// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

internal partial class Interop
{
    /// <summary>
    ///  Helpers for color conversion.
    /// </summary>
    internal static class COLORREF
    {
        public static int RgbToCOLORREF(int rgbValue)
        {
            // Clear the A value, swap R & B values.
            int bValue = (rgbValue & 0xFF) << 16;

            rgbValue &= 0xFFFF00;
            rgbValue |= (rgbValue >> 16) & 0xFF;
            rgbValue &= 0x00FFFF;
            rgbValue |= bValue;
            return rgbValue;
        }

        public static Color COLORREFToColor(int colorref)
        {
            int r = colorref & 0xFF;
            int g = (colorref >> 8) & 0xFF;
            int b = (colorref >> 16) & 0xFF;
            return Color.FromArgb(r, g, b);
        }

        public static int ColorToCOLORREF(Color color)
            => color.R | (color.G << 8) | (color.B << 16);
    }
}
