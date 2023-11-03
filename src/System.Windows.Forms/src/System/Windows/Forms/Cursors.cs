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

    public static Cursor AppStarting => s_appStarting ??= new(PInvoke.IDC_APPSTARTING, nameof(AppStarting));
    public static Cursor Arrow => s_arrow ??= new(PInvoke.IDC_ARROW, nameof(Arrow));
    public static Cursor Cross => s_cross ??= new(PInvoke.IDC_CROSS, nameof(Cross));
    public static Cursor Default => s_defaultCursor ??= new(PInvoke.IDC_ARROW, nameof(Default));
    public static Cursor IBeam => s_iBeam ??= new(PInvoke.IDC_IBEAM, nameof(IBeam));
    public static Cursor No => s_no ??= new(PInvoke.IDC_NO, nameof(No));
    public static Cursor SizeAll => s_sizeAll ??= new(PInvoke.IDC_SIZEALL, nameof(SizeAll));
    public static Cursor SizeNESW => s_sizeNESW ??= new(PInvoke.IDC_SIZENESW, nameof(SizeNESW));
    public static Cursor SizeNS => s_sizeNS ??= new(PInvoke.IDC_SIZENS, nameof(SizeNS));
    public static Cursor SizeNWSE => s_sizeNWSE ??= new(PInvoke.IDC_SIZENWSE, nameof(SizeNWSE));
    public static Cursor SizeWE => s_sizeWE ??= new(PInvoke.IDC_SIZEWE, nameof(SizeWE));
    public static Cursor UpArrow => s_upArrow ??= new(PInvoke.IDC_UPARROW, nameof(UpArrow));
    public static Cursor WaitCursor => s_wait ??= new(PInvoke.IDC_WAIT, nameof(WaitCursor));
    public static Cursor Help => s_help ??= new(PInvoke.IDC_HELP, nameof(Help));
    public static Cursor Hand => s_hand ??= new(PInvoke.IDC_HAND, nameof(Hand));
    public static Cursor HSplit => s_hSplit ??= new("hsplit.cur", nameof(HSplit));
    public static Cursor VSplit => s_vSplit ??= new("vsplit.cur", nameof(VSplit));
    public static Cursor NoMove2D => s_noMove2D ??= new("nomove2d.cur", nameof(NoMove2D));
    public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new("nomoveh.cur", nameof(NoMoveHoriz));
    public static Cursor NoMoveVert => s_noMoveVert ??= new("nomovev.cur", nameof(NoMoveVert));
    public static Cursor PanEast => s_panEast ??= new("east.cur", nameof(PanEast));
    public static Cursor PanNE => s_panNE ??= new("ne.cur", nameof(PanNE));
    public static Cursor PanNorth => s_panNorth ??= new("north.cur", nameof(PanNorth));
    public static Cursor PanNW => s_panNW ??= new("nw.cur", nameof(PanNW));
    public static Cursor PanSE => s_panSE ??= new("se.cur", nameof(PanSE));
    public static Cursor PanSouth => s_panSouth ??= new("south.cur", nameof(PanSouth));
    public static Cursor PanSW => s_panSW ??= new("sw.cur", nameof(PanSW));
    public static Cursor PanWest => s_panWest ??= new("west.cur", nameof(PanWest));
}
