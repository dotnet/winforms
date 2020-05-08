// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Standard cursors
    /// </summary>
    public static class Cursors
    {
        private static Cursor? s_appStarting;
        private static Cursor? s_arrow;
        private static Cursor? s_cross;
        private static Cursor? s_defaultCursor;
        private static Cursor? s_iBeam;
        private static Cursor? s_no;
        private static Cursor? s_sizeAll;
        private static Cursor? s_sizeNESW;
        private static Cursor? s_sizeNS;
        private static Cursor? s_sizeNWSE;
        private static Cursor? s_sizeWE;
        private static Cursor? s_upArrow;
        private static Cursor? s_wait;
        private static Cursor? s_help;
        private static Cursor? s_hSplit;
        private static Cursor? s_vSplit;
        private static Cursor? s_noMove2D;
        private static Cursor? s_noMoveHoriz;
        private static Cursor? s_noMoveVert;
        private static Cursor? s_panEast;
        private static Cursor? s_panNE;
        private static Cursor? s_panNorth;
        private static Cursor? s_panNW;
        private static Cursor? s_panSE;
        private static Cursor? s_panSouth;
        private static Cursor? s_panSW;
        private static Cursor? s_panWest;
        private static Cursor? s_hand;

        public static Cursor AppStarting => s_appStarting ??= new Cursor(User32.CursorResourceId.IDC_APPSTARTING);

        public static Cursor Arrow => s_arrow ??= new Cursor(User32.CursorResourceId.IDC_ARROW);

        public static Cursor Cross => s_cross ??= new Cursor(User32.CursorResourceId.IDC_CROSS);

        public static Cursor Default => s_defaultCursor ??= new Cursor(User32.CursorResourceId.IDC_ARROW);

        public static Cursor IBeam => s_iBeam ??= new Cursor(User32.CursorResourceId.IDC_IBEAM);

        public static Cursor No => s_no ??= new Cursor(User32.CursorResourceId.IDC_NO);

        public static Cursor SizeAll => s_sizeAll ??= new Cursor(User32.CursorResourceId.IDC_SIZEALL);

        public static Cursor SizeNESW => s_sizeNESW ??= new Cursor(User32.CursorResourceId.IDC_SIZENESW);

        public static Cursor SizeNS => s_sizeNS ??= new Cursor(User32.CursorResourceId.IDC_SIZENS);

        public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor(User32.CursorResourceId.IDC_SIZENWSE);

        public static Cursor SizeWE => s_sizeWE ??= new Cursor(User32.CursorResourceId.IDC_SIZEWE);

        public static Cursor UpArrow => s_upArrow ??= new Cursor(User32.CursorResourceId.IDC_UPARROW);

        public static Cursor WaitCursor => s_wait ??= new Cursor(User32.CursorResourceId.IDC_WAIT);

        public static Cursor Help => s_help ??= new Cursor(User32.CursorResourceId.IDC_HELP);

        public static Cursor HSplit => s_hSplit ??= new Cursor(typeof(Cursor), "hsplit.cur");

        public static Cursor VSplit => s_vSplit ??= new Cursor(typeof(Cursor), "vsplit.cur");

        public static Cursor NoMove2D => s_noMove2D ??= new Cursor(typeof(Cursor), "nomove2d.cur");

        public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new Cursor(typeof(Cursor), "nomoveh.cur");

        public static Cursor NoMoveVert => s_noMoveVert ??= new Cursor(typeof(Cursor), "nomovev.cur");

        public static Cursor PanEast => s_panEast ??= new Cursor(typeof(Cursor), "east.cur");

        public static Cursor PanNE => s_panNE ??= new Cursor(typeof(Cursor), "ne.cur");

        public static Cursor PanNorth => s_panNorth ??= new Cursor(typeof(Cursor), "north.cur");

        public static Cursor PanNW => s_panNW ??= new Cursor(typeof(Cursor), "nw.cur");

        public static Cursor PanSE => s_panSE ??= new Cursor(typeof(Cursor), "se.cur");

        public static Cursor PanSouth => s_panSouth ??= new Cursor(typeof(Cursor), "south.cur");

        public static Cursor PanSW => s_panSW ??= new Cursor(typeof(Cursor), "sw.cur");

        public static Cursor PanWest => s_panWest ??= new Cursor(typeof(Cursor), "west.cur");

        public static Cursor Hand => s_hand ??= new Cursor(User32.CursorResourceId.IDC_HAND);
    }
}
