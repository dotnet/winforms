// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

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

    public static Cursor AppStarting => s_appStarting ??= new Cursor(PInvoke.IDC_APPSTARTING);

    public static Cursor Arrow => s_arrow ??= new Cursor(PInvoke.IDC_ARROW);

    public static Cursor Cross => s_cross ??= new Cursor(PInvoke.IDC_CROSS);

    public static Cursor Default => s_defaultCursor ??= new Cursor(PInvoke.IDC_ARROW);

    public static Cursor IBeam => s_iBeam ??= new Cursor(PInvoke.IDC_IBEAM);

    public static Cursor No => s_no ??= new Cursor(PInvoke.IDC_NO);

    public static Cursor SizeAll => s_sizeAll ??= new Cursor(PInvoke.IDC_SIZEALL);

    public static Cursor SizeNESW => s_sizeNESW ??= new Cursor(PInvoke.IDC_SIZENESW);

    public static Cursor SizeNS => s_sizeNS ??= new Cursor(PInvoke.IDC_SIZENS);

    public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor(PInvoke.IDC_SIZENWSE);

    public static Cursor SizeWE => s_sizeWE ??= new Cursor(PInvoke.IDC_SIZEWE);

    public static Cursor UpArrow => s_upArrow ??= new Cursor(PInvoke.IDC_UPARROW);

    public static Cursor WaitCursor => s_wait ??= new Cursor(PInvoke.IDC_WAIT);

    public static Cursor Help => s_help ??= new Cursor(PInvoke.IDC_HELP);
    public static Cursor Hand => s_hand ??= new Cursor(PInvoke.IDC_HAND);

    public static Cursor HSplit => GetCursor(ref s_hSplit, "hsplit.cur");

    public static Cursor VSplit => GetCursor(ref s_vSplit, "vsplit.cur");

    public static Cursor NoMove2D => GetCursor(ref s_noMove2D, "nomove2d.cur");

    public static Cursor NoMoveHoriz => GetCursor(ref s_noMoveHoriz, "nomoveh.cur");

    public static Cursor NoMoveVert => GetCursor(ref s_noMoveVert, "nomovev.cur");

    public static Cursor PanEast => GetCursor(ref s_panEast, "east.cur");

    public static Cursor PanNE => GetCursor(ref s_panNE, "ne.cur");

    public static Cursor PanNorth => GetCursor(ref s_panNorth, "north.cur");

    public static Cursor PanNW => GetCursor(ref s_panNW, "nw.cur");

    public static Cursor PanSE => GetCursor(ref s_panSE, "se.cur");

    public static Cursor PanSouth => GetCursor(ref s_panSouth, "south.cur");

    public static Cursor PanSW => GetCursor(ref s_panSW, "sw.cur");

    public static Cursor PanWest => GetCursor(ref s_panWest, "west.cur");

    private static Cursor GetCursor(ref Cursor? cursor, string resource)
        => cursor is not null && cursor.IsValid()
            ? cursor
            : cursor = new Cursor(typeof(Cursor), resource);
}
