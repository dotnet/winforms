// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Text;
using static Interop;

namespace System.Windows.Forms
{
    internal static class SystemDrawingExtensions
    {
        internal static Gdi32.HBITMAP GetHBITMAP(this Bitmap bitmap) => (Gdi32.HBITMAP)bitmap.GetHbitmap();
        internal static Gdi32.HFONT ToHFONT(this Font font) => (Gdi32.HFONT)font.ToHfont();

        /// <summary>
        ///  Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
        /// </summary>
        internal static Gdi32.QUALITY FontQualityFromTextRenderingHint(this IDeviceContext? deviceContext)
        {
            if (!(deviceContext is Graphics g))
            {
                return Gdi32.QUALITY.DEFAULT;
            }

            switch (g.TextRenderingHint)
            {
                case TextRenderingHint.ClearTypeGridFit:
                    return Gdi32.QUALITY.CLEARTYPE;
                case TextRenderingHint.AntiAliasGridFit:
                case TextRenderingHint.AntiAlias:
                    return Gdi32.QUALITY.ANTIALIASED;
                case TextRenderingHint.SingleBitPerPixelGridFit:
                    return Gdi32.QUALITY.PROOF;
                case TextRenderingHint.SingleBitPerPixel:
                    return Gdi32.QUALITY.DRAFT;
                default:
                case TextRenderingHint.SystemDefault:
                    return Gdi32.QUALITY.DEFAULT;
            }
        }
    }
}
