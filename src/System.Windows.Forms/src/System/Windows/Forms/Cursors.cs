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
        private static Cursor s_appStarting;
        private static Cursor s_arrow;
        private static Cursor s_cross;
        private static Cursor s_defaultCursor;
        private static Cursor s_iBeam;
        private static Cursor s_no;
        private static Cursor s_sizeAll;
        private static Cursor s_sizeNESW;
        private static Cursor s_sizeNS;
        private static Cursor s_sizeNWSE;
        private static Cursor s_sizeWE;
        private static Cursor s_upArrow;
        private static Cursor s_wait;
        private static Cursor s_help;
        private static Cursor s_hSplit;
        private static Cursor s_vSplit;
        private static Cursor s_noMove2D;
        private static Cursor s_noMoveHoriz;
        private static Cursor s_noMoveVert;
        private static Cursor s_panEast;
        private static Cursor s_panNE;
        private static Cursor s_panNorth;
        private static Cursor s_panNW;
        private static Cursor s_panSE;
        private static Cursor s_panSouth;
        private static Cursor s_panSW;
        private static Cursor s_panWest;
        private static Cursor s_hand;

        public static Cursor AppStarting => s_appStarting ??= new Cursor(User32.CursorResourceId.IDC_APPSTARTING, 0);

        public static Cursor Arrow => s_arrow ??= new Cursor(User32.CursorResourceId.IDC_ARROW, 0);

        public static Cursor Cross => s_cross ??= new Cursor(User32.CursorResourceId.IDC_CROSS, 0);

        public static Cursor Default => s_defaultCursor ??= new Cursor(User32.CursorResourceId.IDC_ARROW, 0);

        public static Cursor IBeam => s_iBeam ??= new Cursor(User32.CursorResourceId.IDC_IBEAM, 0);

        public static Cursor No => s_no ??= new Cursor(User32.CursorResourceId.IDC_NO, 0);

        public static Cursor SizeAll => s_sizeAll ??= new Cursor(User32.CursorResourceId.IDC_SIZEALL, 0);

        public static Cursor SizeNESW => s_sizeNESW ??= new Cursor(User32.CursorResourceId.IDC_SIZENESW, 0);

        public static Cursor SizeNS => s_sizeNS ??= new Cursor(User32.CursorResourceId.IDC_SIZENS, 0);

        public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor(User32.CursorResourceId.IDC_SIZENWSE, 0);

        public static Cursor SizeWE => s_sizeWE ??= new Cursor(User32.CursorResourceId.IDC_SIZEWE, 0);

        public static Cursor UpArrow => s_upArrow ??= new Cursor(User32.CursorResourceId.IDC_UPARROW, 0);

        public static Cursor WaitCursor => s_wait ??= new Cursor(User32.CursorResourceId.IDC_WAIT, 0);

        public static Cursor Help => s_help ??= new Cursor(User32.CursorResourceId.IDC_HELP, 0);

        public static Cursor HSplit => s_hSplit ??= new Cursor("hsplit.cur", 0);

        public static Cursor VSplit => s_vSplit ??= new Cursor("vsplit.cur", 0);

        public static Cursor NoMove2D => s_noMove2D ??= new Cursor("nomove2d.cur", 0);

        public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new Cursor("nomoveh.cur", 0);

        public static Cursor NoMoveVert => s_noMoveVert ??= new Cursor("nomovev.cur", 0);

        public static Cursor PanEast => s_panEast ??= new Cursor("east.cur", 0);

        public static Cursor PanNE => s_panNE ??= new Cursor("ne.cur", 0);

        public static Cursor PanNorth => s_panNorth ??= new Cursor("north.cur", 0);

        public static Cursor PanNW => s_panNW ??= new Cursor("nw.cur", 0);

        public static Cursor PanSE => s_panSE ??= new Cursor("se.cur", 0);

        public static Cursor PanSouth => s_panSouth ??= new Cursor("south.cur", 0);

        public static Cursor PanSW => s_panSW ??= new Cursor("sw.cur", 0);

        public static Cursor PanWest => s_panWest ??= new Cursor("west.cur", 0);

        public static Cursor Hand => s_hand ??= new Cursor("hand.cur", 0);
    }
}
