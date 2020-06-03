// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies shortcut keys that can be used by menu items.
    /// </summary>
    public enum Shortcut
    {
        /// <summary>
        ///  No shortcut key is associated with the menu item.
        /// </summary>
        None = 0,

        /// <summary>
        ///  The shorcut keys CTRL+A.
        /// </summary>
        CtrlA = Keys.Control + Keys.A,

        /// <summary>
        ///  The shorcut keys CTRL+B.
        /// </summary>
        CtrlB = Keys.Control + Keys.B,

        /// <summary>
        ///  The shorcut keys CTRL+C.
        /// </summary>
        CtrlC = Keys.Control + Keys.C,

        /// <summary>
        ///  The shorcut keys CTRL+D.
        /// </summary>
        CtrlD = Keys.Control + Keys.D,

        /// <summary>
        ///  The shorcut keys CTRL+E.
        /// </summary>
        CtrlE = Keys.Control + Keys.E,

        /// <summary>
        ///  The shorcut keys CTRL+F.
        /// </summary>
        CtrlF = Keys.Control + Keys.F,

        /// <summary>
        ///  The shorcut keys CTRL+G.
        /// </summary>
        CtrlG = Keys.Control + Keys.G,

        /// <summary>
        ///  The shorcut keys CTRL+H.
        /// </summary>
        CtrlH = Keys.Control + Keys.H,

        /// <summary>
        ///  The shorcut keys CTRL+I.
        /// </summary>
        CtrlI = Keys.Control + Keys.I,

        /// <summary>
        ///  The shorcut keys CTRL+J.
        /// </summary>
        CtrlJ = Keys.Control + Keys.J,

        /// <summary>
        ///  The shorcut keys CTRL+K.
        /// </summary>
        CtrlK = Keys.Control + Keys.K,

        /// <summary>
        ///  The shorcut keys CTRL+L.
        /// </summary>
        CtrlL = Keys.Control + Keys.L,

        /// <summary>
        ///  The shorcut keys CTRL+M.
        /// </summary>
        CtrlM = Keys.Control + Keys.M,

        /// <summary>
        ///  The shorcut keys CTRL+N.
        /// </summary>
        CtrlN = Keys.Control + Keys.N,

        /// <summary>
        ///  The shorcut keys CTRL+O.
        /// </summary>
        CtrlO = Keys.Control + Keys.O,

        /// <summary>
        ///  The shorcut keys CTRL+P.
        /// </summary>
        CtrlP = Keys.Control + Keys.P,

        /// <summary>
        ///  The shorcut keys CTRL+Q.
        /// </summary>
        CtrlQ = Keys.Control + Keys.Q,

        /// <summary>
        ///  The shorcut keys CTRL+R.
        /// </summary>
        CtrlR = Keys.Control + Keys.R,

        /// <summary>
        ///  The shorcut keys CTRL+S.
        /// </summary>
        CtrlS = Keys.Control + Keys.S,

        /// <summary>
        ///  The shorcut keys CTRL+T.
        /// </summary>
        CtrlT = Keys.Control + Keys.T,

        /// <summary>
        ///  The shorcut keys CTRL+U
        /// </summary>
        CtrlU = Keys.Control + Keys.U,

        /// <summary>
        ///  The shorcut keys CTRL+V.
        /// </summary>
        CtrlV = Keys.Control + Keys.V,

        /// <summary>
        ///  The shorcut keys CTRL+W.
        /// </summary>
        CtrlW = Keys.Control + Keys.W,

        /// <summary>
        ///  The shorcut keys CTRL+X.
        /// </summary>
        CtrlX = Keys.Control + Keys.X,

        /// <summary>
        ///  The shorcut keys CTRL+Y.
        /// </summary>
        CtrlY = Keys.Control + Keys.Y,

        /// <summary>
        ///  The shorcut keys CTRL+Z.
        /// </summary>
        CtrlZ = Keys.Control + Keys.Z,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+A.
        /// </summary>
        CtrlShiftA = Keys.Control + Keys.Shift + Keys.A,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+B.
        /// </summary>
        CtrlShiftB = Keys.Control + Keys.Shift + Keys.B,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+C.
        /// </summary>
        CtrlShiftC = Keys.Control + Keys.Shift + Keys.C,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+D.
        /// </summary>
        CtrlShiftD = Keys.Control + Keys.Shift + Keys.D,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+E.
        /// </summary>
        CtrlShiftE = Keys.Control + Keys.Shift + Keys.E,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F.
        /// </summary>
        CtrlShiftF = Keys.Control + Keys.Shift + Keys.F,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+G.
        /// </summary>
        CtrlShiftG = Keys.Control + Keys.Shift + Keys.G,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+H.
        /// </summary>
        CtrlShiftH = Keys.Control + Keys.Shift + Keys.H,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+I.
        /// </summary>
        CtrlShiftI = Keys.Control + Keys.Shift + Keys.I,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+J.
        /// </summary>
        CtrlShiftJ = Keys.Control + Keys.Shift + Keys.J,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+K.
        /// </summary>
        CtrlShiftK = Keys.Control + Keys.Shift + Keys.K,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+L.
        /// </summary>
        CtrlShiftL = Keys.Control + Keys.Shift + Keys.L,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+M.
        /// </summary>
        CtrlShiftM = Keys.Control + Keys.Shift + Keys.M,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+N.
        /// </summary>
        CtrlShiftN = Keys.Control + Keys.Shift + Keys.N,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+O.
        /// </summary>
        CtrlShiftO = Keys.Control + Keys.Shift + Keys.O,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+P.
        /// </summary>
        CtrlShiftP = Keys.Control + Keys.Shift + Keys.P,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+Q.
        /// </summary>
        CtrlShiftQ = Keys.Control + Keys.Shift + Keys.Q,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+R.
        /// </summary>
        CtrlShiftR = Keys.Control + Keys.Shift + Keys.R,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+S.
        /// </summary>
        CtrlShiftS = Keys.Control + Keys.Shift + Keys.S,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+T.
        /// </summary>
        CtrlShiftT = Keys.Control + Keys.Shift + Keys.T,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+U.
        /// </summary>
        CtrlShiftU = Keys.Control + Keys.Shift + Keys.U,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+V.
        /// </summary>
        CtrlShiftV = Keys.Control + Keys.Shift + Keys.V,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+W.
        /// </summary>
        CtrlShiftW = Keys.Control + Keys.Shift + Keys.W,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+X.
        /// </summary>
        CtrlShiftX = Keys.Control + Keys.Shift + Keys.X,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+Y.
        /// </summary>
        CtrlShiftY = Keys.Control + Keys.Shift + Keys.Y,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+Z.
        /// </summary>
        CtrlShiftZ = Keys.Control + Keys.Shift + Keys.Z,

        /// <summary>
        ///  The shortcut key F1.
        /// </summary>
        F1 = Keys.F1,

        /// <summary>
        ///  The shortcut key F2.
        /// </summary>
        F2 = Keys.F2,

        /// <summary>
        ///  The shortcut key F3.
        /// </summary>
        F3 = Keys.F3,

        /// <summary>
        ///  The shortcut key F4.
        /// </summary>
        F4 = Keys.F4,

        /// <summary>
        ///  The shortcut key F5.
        /// </summary>
        F5 = Keys.F5,

        /// <summary>
        ///  The shortcut key F6.
        /// </summary>
        F6 = Keys.F6,

        /// <summary>
        ///  The shortcut key F7.
        /// </summary>
        F7 = Keys.F7,
        F8 = Keys.F8,

        /// <summary>
        ///  The shortcut key F9.
        /// </summary>
        F9 = Keys.F9,

        /// <summary>
        ///  The shortcut key F10.
        /// </summary>
        F10 = Keys.F10,

        /// <summary>
        ///  The shortcut key F11.
        /// </summary>
        F11 = Keys.F11,

        /// <summary>
        ///  The shortcut key F12.
        /// </summary>
        F12 = Keys.F12,

        /// <summary>
        ///  The shortcut keys SHIFT+F1.
        /// </summary>
        ShiftF1 = Keys.Shift + Keys.F1,

        /// <summary>
        ///  The shortcut keys SHIFT+F2.
        /// </summary>
        ShiftF2 = Keys.Shift + Keys.F2,

        /// <summary>
        ///  The shortcut keys SHIFT+F3.
        /// </summary>
        ShiftF3 = Keys.Shift + Keys.F3,

        /// <summary>
        ///  The shortcut keys SHIFT+F4.
        /// </summary>
        ShiftF4 = Keys.Shift + Keys.F4,

        /// <summary>
        ///  The shortcut keys SHIFT+F5.
        /// </summary>
        ShiftF5 = Keys.Shift + Keys.F5,

        /// <summary>
        ///  The shortcut keys SHIFT+F6.
        /// </summary>
        ShiftF6 = Keys.Shift + Keys.F6,

        /// <summary>
        ///  The shortcut keys SHIFT+F7.
        /// </summary>
        ShiftF7 = Keys.Shift + Keys.F7,

        /// <summary>
        ///  The shortcut keys SHIFT+F8.
        /// </summary>
        ShiftF8 = Keys.Shift + Keys.F8,

        /// <summary>
        ///  The shortcut keys SHIFT+F9.
        /// </summary>
        ShiftF9 = Keys.Shift + Keys.F9,

        /// <summary>
        ///  The shortcut keys SHIFT+F10.
        /// </summary>
        ShiftF10 = Keys.Shift + Keys.F10,

        /// <summary>
        ///  The shortcut keys SHIFT+F11.
        /// </summary>
        ShiftF11 = Keys.Shift + Keys.F11,

        /// <summary>
        ///  The shortcut keys SHIFT+F12.
        /// </summary>
        ShiftF12 = Keys.Shift + Keys.F12,

        /// <summary>
        ///  The shortcut keys CTRL+F1.
        /// </summary>
        CtrlF1 = Keys.Control + Keys.F1,

        /// <summary>
        ///  The shortcut keys CTRL+F2.
        /// </summary>
        CtrlF2 = Keys.Control + Keys.F2,

        /// <summary>
        ///  The shortcut keys CTRL+F3.
        /// </summary>
        CtrlF3 = Keys.Control + Keys.F3,

        /// <summary>
        ///  The shortcut keys CTRL+F4.
        /// </summary>
        CtrlF4 = Keys.Control + Keys.F4,

        /// <summary>
        ///  The shortcut keys CTRL+F5.
        /// </summary>
        CtrlF5 = Keys.Control + Keys.F5,

        /// <summary>
        ///  The shortcut keys CTRL+F6.
        /// </summary>
        CtrlF6 = Keys.Control + Keys.F6,

        /// <summary>
        ///  The shortcut keys CTRL+F7.
        /// </summary>
        CtrlF7 = Keys.Control + Keys.F7,

        /// <summary>
        ///  The shortcut keys CTRL+F8.
        /// </summary>
        CtrlF8 = Keys.Control + Keys.F8,

        /// <summary>
        ///  The shortcut keys CTRL+F9.
        /// </summary>
        CtrlF9 = Keys.Control + Keys.F9,

        /// <summary>
        ///  The shortcut keys CTRL+F10.
        /// </summary>
        CtrlF10 = Keys.Control + Keys.F10,

        /// <summary>
        ///  The shortcut keys CTRL+F11.
        /// </summary>
        CtrlF11 = Keys.Control + Keys.F11,

        /// <summary>
        ///  The shortcut keys CTRL+F12.
        /// </summary>
        CtrlF12 = Keys.Control + Keys.F12,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F1.
        /// </summary>
        CtrlShiftF1 = Keys.Control + Keys.Shift + Keys.F1,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F2.
        /// </summary>
        CtrlShiftF2 = Keys.Control + Keys.Shift + Keys.F2,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F3.
        /// </summary>
        CtrlShiftF3 = Keys.Control + Keys.Shift + Keys.F3,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F4.
        /// </summary>
        CtrlShiftF4 = Keys.Control + Keys.Shift + Keys.F4,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F5.
        /// </summary>
        CtrlShiftF5 = Keys.Control + Keys.Shift + Keys.F5,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F6.
        /// </summary>
        CtrlShiftF6 = Keys.Control + Keys.Shift + Keys.F6,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F7.
        /// </summary>
        CtrlShiftF7 = Keys.Control + Keys.Shift + Keys.F7,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F8.
        /// </summary>
        CtrlShiftF8 = Keys.Control + Keys.Shift + Keys.F8,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F9.
        /// </summary>
        CtrlShiftF9 = Keys.Control + Keys.Shift + Keys.F9,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F10.
        /// </summary>
        CtrlShiftF10 = Keys.Control + Keys.Shift + Keys.F10,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F11.
        /// </summary>
        CtrlShiftF11 = Keys.Control + Keys.Shift + Keys.F11,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+F12.
        /// </summary>
        CtrlShiftF12 = Keys.Control + Keys.Shift + Keys.F12,

        /// <summary>
        ///  The shortcut key INSERT.
        /// </summary>
        Ins = Keys.Insert,

        /// <summary>
        ///  The shortcut keys CTRL+INSERT.
        /// </summary>
        CtrlIns = Keys.Control + Keys.Insert,

        /// <summary>
        ///  The shortcut keys SHIFT+INSERT.
        /// </summary>
        ShiftIns = Keys.Shift + Keys.Insert,

        /// <summary>
        ///  The shortcut key DELETE.
        /// </summary>
        Del = Keys.Delete,

        /// <summary>
        ///  The shortcut keys CTRL+DELETE.
        /// </summary>
        CtrlDel = Keys.Control + Keys.Delete,

        /// <summary>
        ///  The shortcut keys SHIFT+DELETE.
        /// </summary>
        ShiftDel = Keys.Shift + Keys.Delete,

        /// <summary>
        ///  The shortcut keys Alt + RightArrow.
        /// </summary>
        AltRightArrow = Keys.Alt + Keys.Right,

        /// <summary>
        ///  The shortcut keys ALT+LeftArrow.
        /// </summary>
        AltLeftArrow = Keys.Alt + Keys.Left,

        /// <summary>
        ///  The shortcut keys ALT+UpArrow.
        /// </summary>
        AltUpArrow = Keys.Alt + Keys.Up,

        /// <summary>
        ///  The shortcut keys Alt + DownArrow.
        /// </summary>
        AltDownArrow = Keys.Alt + Keys.Down,

        /// <summary>
        ///  The shortcut keys ALT+BACKSPACE.
        /// </summary>
        AltBksp = Keys.Alt + Keys.Back,

        /// <summary>
        ///  The shortcut keys ALT+F1.
        /// </summary>
        AltF1 = Keys.Alt + Keys.F1,

        /// <summary>
        ///  The shortcut keys ALT+F2.
        /// </summary>
        AltF2 = Keys.Alt + Keys.F2,

        /// <summary>
        ///  The shortcut keys ALT+F3.
        /// </summary>
        AltF3 = Keys.Alt + Keys.F3,

        /// <summary>
        ///  The shortcut keys ALT+F4.
        /// </summary>
        AltF4 = Keys.Alt + Keys.F4,

        /// <summary>
        ///  The shortcut keys ALT+F5.
        /// </summary>
        AltF5 = Keys.Alt + Keys.F5,

        /// <summary>
        ///  The shortcut keys ALT+F6.
        /// </summary>
        AltF6 = Keys.Alt + Keys.F6,

        /// <summary>
        ///  The shortcut keys ALT+F7.
        /// </summary>
        AltF7 = Keys.Alt + Keys.F7,

        /// <summary>
        ///  The shortcut keys ALT+F8.
        /// </summary>
        AltF8 = Keys.Alt + Keys.F8,

        /// <summary>
        ///  The shortcut keys ALT+F9.
        /// </summary>
        AltF9 = Keys.Alt + Keys.F9,

        /// <summary>
        ///  The shortcut keys ALT+F10.
        /// </summary>
        AltF10 = Keys.Alt + Keys.F10,

        /// <summary>
        ///  The shortcut keys ALT+F11.
        /// </summary>
        AltF11 = Keys.Alt + Keys.F11,

        /// <summary>
        ///  The shortcut keys ALT+F12.
        /// </summary>
        AltF12 = Keys.Alt + Keys.F12,

        /// <summary>
        ///  The shortcut keys ALT+0.
        /// </summary>
        Alt0 = Keys.Alt + Keys.D0,

        /// <summary>
        ///  The shortcut keys ALT+1.
        /// </summary>
        Alt1 = Keys.Alt + Keys.D1,

        /// <summary>
        ///  The shortcut keys ALT+2.
        /// </summary>
        Alt2 = Keys.Alt + Keys.D2,

        /// <summary>
        ///  The shortcut keys ALT+3.
        /// </summary>
        Alt3 = Keys.Alt + Keys.D3,

        /// <summary>
        ///  The shortcut keys ALT+4.
        /// </summary>
        Alt4 = Keys.Alt + Keys.D4,

        /// <summary>
        ///  The shortcut keys ALT+5.
        /// </summary>
        Alt5 = Keys.Alt + Keys.D5,

        /// <summary>
        ///  The shortcut keys ALT+6.
        /// </summary>
        Alt6 = Keys.Alt + Keys.D6,

        /// <summary>
        ///  The shortcut keys ALT+7.
        /// </summary>
        Alt7 = Keys.Alt + Keys.D7,

        /// <summary>
        ///  The shortcut keys ALT+8.
        /// </summary>
        Alt8 = Keys.Alt + Keys.D8,

        /// <summary>
        ///  The shortcut keys ALT+9.
        /// </summary>
        Alt9 = Keys.Alt + Keys.D9,

        /// <summary>
        ///  The shortcut keys CTRL+0.
        /// </summary>
        Ctrl0 = Keys.Control + Keys.D0,

        /// <summary>
        ///  The shortcut keys CTRL+1.
        /// </summary>
        Ctrl1 = Keys.Control + Keys.D1,

        /// <summary>
        ///  The shortcut keys CTRL+2.
        /// </summary>
        Ctrl2 = Keys.Control + Keys.D2,

        /// <summary>
        ///  The shortcut keys CTRL+3.
        /// </summary>
        Ctrl3 = Keys.Control + Keys.D3,

        /// <summary>
        ///  The shortcut keys CTRL+4.
        /// </summary>
        Ctrl4 = Keys.Control + Keys.D4,

        /// <summary>
        ///  The shortcut keys CTRL+5.
        /// </summary>
        Ctrl5 = Keys.Control + Keys.D5,

        /// <summary>
        ///  The shortcut keys CTRL+6.
        /// </summary>
        Ctrl6 = Keys.Control + Keys.D6,

        /// <summary>
        ///  The shortcut keys CTRL+7.
        /// </summary>
        Ctrl7 = Keys.Control + Keys.D7,

        /// <summary>
        ///  The shortcut keys CTRL+8.
        /// </summary>
        Ctrl8 = Keys.Control + Keys.D8,

        /// <summary>
        ///  The shortcut keys CTRL+9.
        /// </summary>
        Ctrl9 = Keys.Control + Keys.D9,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+0.
        /// </summary>
        CtrlShift0 = Keys.Control + Keys.Shift + Keys.D0,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+1.
        /// </summary>
        CtrlShift1 = Keys.Control + Keys.Shift + Keys.D1,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+2.
        /// </summary>
        CtrlShift2 = Keys.Control + Keys.Shift + Keys.D2,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+3.
        /// </summary>
        CtrlShift3 = Keys.Control + Keys.Shift + Keys.D3,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+4.
        /// </summary>
        CtrlShift4 = Keys.Control + Keys.Shift + Keys.D4,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+5.
        /// </summary>
        CtrlShift5 = Keys.Control + Keys.Shift + Keys.D5,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+6.
        /// </summary>
        CtrlShift6 = Keys.Control + Keys.Shift + Keys.D6,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+7.
        /// </summary>
        CtrlShift7 = Keys.Control + Keys.Shift + Keys.D7,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+8.
        /// </summary>
        CtrlShift8 = Keys.Control + Keys.Shift + Keys.D8,

        /// <summary>
        ///  The shortcut keys CTRL+SHIFT+9.
        /// </summary>
        CtrlShift9 = Keys.Control + Keys.Shift + Keys.D9,
    }
}
