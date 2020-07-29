// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System
{
    /// <summary>
    ///  Holder for tracking HDC state.
    /// </summary>
    internal unsafe class DeviceContextState
    {
        // Not all state is handled yet. Backfilling in as we write specific tests. Of special note is that we don't
        // have tracking for Save/RestoreDC yet.

        /// <summary>
        ///  Initialize the current state of <paramref name="hdc"/>.
        /// </summary>
        public DeviceContextState(Gdi32.HDC hdc)
        {
            MapMode = Gdi32.GetMapMode(hdc);
            BackColor = Gdi32.GetBkColor(hdc);
            TextColor = Gdi32.GetTextColor(hdc);
            Point point = default;
            Gdi32.GetBrushOrgEx(hdc, ref point);
            BrushOrigin = point;
            TextAlign = Gdi32.GetTextAlign(hdc);
            BackgroundMode = Gdi32.GetBkMode(hdc);
        }

        public Gdi32.MM MapMode { get; set; }
        public COLORREF BackColor { get; set; }
        public COLORREF TextColor { get; set; }
        public Point BrushOrigin { get; set; }
        public Gdi32.TA TextAlign { get; set; }
        public Gdi32.BKMODE BackgroundMode { get; set; }
        public string SelectedFont { get; set; }
    }
}
