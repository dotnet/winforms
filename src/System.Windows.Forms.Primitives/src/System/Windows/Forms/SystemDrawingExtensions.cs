// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    internal static class SystemDrawingExtensions
    {
        internal static Gdi32.HBITMAP GetHBITMAP(this Bitmap bitmap) => (Gdi32.HBITMAP)bitmap.GetHbitmap();
        internal static Gdi32.HFONT ToHFONT(this Font font) => (Gdi32.HFONT)font.ToHfont();
        internal static bool HasTransparency(this Color color) => color.A != byte.MaxValue;
    }
}
