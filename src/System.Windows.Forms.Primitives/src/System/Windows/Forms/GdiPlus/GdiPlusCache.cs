// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Cache of GDI+ objects to reuse commonly created items.
    /// </summary>
    internal static class GdiPlusCache
    {
        // Keeping these as ThreadStatic for now. With testing, and if we can make them immutable ourselves (needs
        // additional System.Drawing functionality), we can look at making this shared. Adding locking, however, does
        // add cost to retrieving entries so it might not be worth it.
        [ThreadStatic]
        private static PenCache? s_penCache;
        [ThreadStatic]
        private static SolidBrushCache? s_brushCache;

        // Picking soft and hard limits expecting that 30 of each is a reasonable upper limit of permanently reserved
        // space. These items are roughly .5KB each, with _most_ of the memory being native (in the GDI+ object).

        private static PenCache PenCache => s_penCache ??= new PenCache(softLimit: 30, hardLimit: 40);
        private static SolidBrushCache BrushCache => s_brushCache ??= new SolidBrushCache(softLimit: 30, hardLimit: 40);

        private static PenCache.Scope GetPenScope(Color color)
        {
            if (color.IsKnownColor)
            {
                Pen? pen = color.IsSystemColor
                    ? SystemPens.FromSystemColor(color)
                    : PenFromKnownColor(color.ToKnownColor());

                if (pen != null)
                {
                    return new PenCache.Scope(pen);
                }
            }

            return PenCache.GetEntry(color);
        }

        private static SolidBrushCache.Scope GetSolidBrushScope(Color color)
        {
            if (color.IsKnownColor)
            {
                SolidBrush? solidBrush = color.IsSystemColor
                    ? (SolidBrush?)SystemBrushes.FromSystemColor(color)
                    : (SolidBrush?)BrushFromKnownColor(color.ToKnownColor());

                if (solidBrush != null)
                {
                    return new SolidBrushCache.Scope(solidBrush);
                }
            }

            return BrushCache.GetEntry(color);
        }

        /// <summary>
        ///  Returns a cached <see cref="Pen"/>. Use in a using and assign to var.
        /// </summary>
        /// <remarks>
        ///  Correct: using var pen = GdiPlusCache.GetCachedPen(Color.Blue);
        ///  Incorrect (LEAKS): using Pen pen = GdiPlusCache.GetCachedPen(Color.Blue);
        /// </remarks>
        internal static PenCache.Scope GetCachedPenScope(this Color color) => GetPenScope(color);

        /// <summary>
        ///  Returns a cached <see cref="Pen"/>. Use in a using and assign to var.
        /// </summary>
        /// <remarks>
        ///  Correct: using var pen = GdiPlusCache.GetCachedPen(Color.Blue);
        ///  Incorrect (LEAKS): using Pen pen = GdiPlusCache.GetCachedPen(Color.Blue);
        ///
        ///  Debug builds track proper disposal.
        /// </remarks>
        internal static PenCache.Scope GetCachedPenScope(this Color color, int width)
            => width == 1 ? GetPenScope(color) : new PenCache.Scope(new Pen(color, width));

        /// <summary>
        ///  Returns a cached <see cref="SolidBrush"/>. Use in a using and assign to var.
        /// </summary>
        /// <remarks>
        ///  Correct: using var pen = GdiPlusCache.GetCachedSolidBrush(Color.Blue);
        ///  Incorrect (LEAKS): using Pen pen = GdiPlusCache.GetCachedSolidBrush(Color.Blue);
        ///
        ///  Debug builds track proper disposal.
        /// </remarks>
        internal static SolidBrushCache.Scope GetCachedSolidBrushScope(this Color color) => GetSolidBrushScope(color);

        private static Brush? BrushFromKnownColor(KnownColor color) => color switch
        {
            // Starting with the expected most common
            KnownColor.Black => Brushes.Black,
            KnownColor.White => Brushes.White,
            KnownColor.Gray => Brushes.Gray,
            KnownColor.Red => Brushes.Red,
            KnownColor.Green => Brushes.Green,
            KnownColor.Blue => Brushes.Blue,
            KnownColor.Yellow => Brushes.Yellow,
            KnownColor.Brown => Brushes.Brown,
            KnownColor.LightGray => Brushes.LightGray,
            KnownColor.LightGreen => Brushes.LightGreen,
            KnownColor.LightBlue => Brushes.LightBlue,
            KnownColor.LightYellow => Brushes.LightYellow,
            KnownColor.DarkGray => Brushes.DarkGray,
            KnownColor.DarkRed => Brushes.DarkRed,
            KnownColor.DarkGreen => Brushes.DarkGreen,
            KnownColor.DarkBlue => Brushes.DarkBlue,
            KnownColor.Transparent => Brushes.Transparent,
            _ => null
        };

        private static Pen? PenFromKnownColor(KnownColor color) => color switch
        {
            // Starting with the expected most common
            KnownColor.Black => Pens.Black,
            KnownColor.White => Pens.White,
            KnownColor.Gray => Pens.Gray,
            KnownColor.Red => Pens.Red,
            KnownColor.Green => Pens.Green,
            KnownColor.Blue => Pens.Blue,
            KnownColor.Yellow => Pens.Yellow,
            KnownColor.Brown => Pens.Brown,
            KnownColor.LightGray => Pens.LightGray,
            KnownColor.LightGreen => Pens.LightGreen,
            KnownColor.LightBlue => Pens.LightBlue,
            KnownColor.LightYellow => Pens.LightYellow,
            KnownColor.DarkGray => Pens.DarkGray,
            KnownColor.DarkRed => Pens.DarkRed,
            KnownColor.DarkGreen => Pens.DarkGreen,
            KnownColor.DarkBlue => Pens.DarkBlue,
            KnownColor.Transparent => Pens.Transparent,
            _ => null
        };
    }
}
