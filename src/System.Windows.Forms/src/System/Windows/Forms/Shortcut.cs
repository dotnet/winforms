﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies shortcut keys that can be used by menu items.
    /// </devdoc>
    [ComVisible(true)]
    public enum Shortcut
    {
        /// <devdoc>
        /// No shortcut key is associated with the menu item.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// The shorcut keys CTRL+A.
        /// </devdoc>
        CtrlA = Keys.Control + Keys.A,

        /// <devdoc>
        /// The shorcut keys CTRL+B.
        /// </devdoc>
        CtrlB = Keys.Control + Keys.B,

        /// <devdoc>
        /// The shorcut keys CTRL+C.
        /// </devdoc>
        CtrlC = Keys.Control + Keys.C,

        /// <devdoc>
        /// The shorcut keys CTRL+D.
        /// </devdoc>
        CtrlD = Keys.Control + Keys.D,

        /// <devdoc>
        /// The shorcut keys CTRL+E.
        /// </devdoc>
        CtrlE = Keys.Control + Keys.E,

        /// <devdoc>
        /// The shorcut keys CTRL+F.
        /// </devdoc>
        CtrlF = Keys.Control + Keys.F,

        /// <devdoc>
        /// The shorcut keys CTRL+G.
        /// </devdoc>
        CtrlG = Keys.Control + Keys.G,

        /// <devdoc>
        /// The shorcut keys CTRL+H.
        /// </devdoc>
        CtrlH = Keys.Control + Keys.H,

        /// <devdoc>
        /// The shorcut keys CTRL+I.
        /// </devdoc>
        CtrlI = Keys.Control + Keys.I,

        /// <devdoc>
        /// The shorcut keys CTRL+J.
        /// </devdoc>
        CtrlJ = Keys.Control + Keys.J,

        /// <devdoc>
        /// The shorcut keys CTRL+K.
        /// </devdoc>
        CtrlK = Keys.Control + Keys.K,

        /// <devdoc>
        /// The shorcut keys CTRL+L.
        /// </devdoc>
        CtrlL = Keys.Control + Keys.L,

        /// <devdoc>
        /// The shorcut keys CTRL+M.
        /// </devdoc>
        CtrlM = Keys.Control + Keys.M,

        /// <devdoc>
        /// The shorcut keys CTRL+N.
        /// </devdoc>
        CtrlN = Keys.Control + Keys.N,

        /// <devdoc>
        /// The shorcut keys CTRL+O.
        /// </devdoc>
        CtrlO = Keys.Control + Keys.O,

        /// <devdoc>
        /// The shorcut keys CTRL+P.
        /// </devdoc>
        CtrlP = Keys.Control + Keys.P,

        /// <devdoc>
        /// The shorcut keys CTRL+Q.
        /// </devdoc>
        CtrlQ = Keys.Control + Keys.Q,

        /// <devdoc>
        /// The shorcut keys CTRL+R.
        /// </devdoc>
        CtrlR = Keys.Control + Keys.R,

        /// <devdoc>
        /// The shorcut keys CTRL+S.
        /// </devdoc>
        CtrlS = Keys.Control + Keys.S,

        /// <devdoc>
        /// The shorcut keys CTRL+T.
        /// </devdoc>
        CtrlT = Keys.Control + Keys.T,

        /// <devdoc>
        /// The shorcut keys CTRL+U
        /// </devdoc>
        CtrlU = Keys.Control + Keys.U,

        /// <devdoc>
        /// The shorcut keys CTRL+V.
        /// </devdoc>
        CtrlV = Keys.Control + Keys.V,

        /// <devdoc>
        /// The shorcut keys CTRL+W.
        /// </devdoc>
        CtrlW = Keys.Control + Keys.W,

        /// <devdoc>
        /// The shorcut keys CTRL+X.
        /// </devdoc>
        CtrlX = Keys.Control + Keys.X,

        /// <devdoc>
        /// The shorcut keys CTRL+Y.
        /// </devdoc>
        CtrlY = Keys.Control + Keys.Y,

        /// <devdoc>
        /// The shorcut keys CTRL+Z.
        /// </devdoc>
        CtrlZ = Keys.Control + Keys.Z,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+A.
        /// </devdoc>
        CtrlShiftA = Keys.Control + Keys.Shift + Keys.A,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+B.
        /// </devdoc>
        CtrlShiftB = Keys.Control + Keys.Shift + Keys.B,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+C.
        /// </devdoc>
        CtrlShiftC = Keys.Control + Keys.Shift + Keys.C,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+D.
        /// </devdoc>
        CtrlShiftD = Keys.Control + Keys.Shift + Keys.D,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+E.
        /// </devdoc>
        CtrlShiftE = Keys.Control + Keys.Shift + Keys.E,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F.
        /// </devdoc>
        CtrlShiftF = Keys.Control + Keys.Shift + Keys.F,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+G.
        /// </devdoc>
        CtrlShiftG = Keys.Control + Keys.Shift + Keys.G,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+H.
        /// </devdoc>
        CtrlShiftH = Keys.Control + Keys.Shift + Keys.H,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+I.
        /// </devdoc>
        CtrlShiftI = Keys.Control + Keys.Shift + Keys.I,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+J.
        /// </devdoc>
        CtrlShiftJ = Keys.Control + Keys.Shift + Keys.J,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+K.
        /// </devdoc>
        CtrlShiftK = Keys.Control + Keys.Shift + Keys.K,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+L.
        /// </devdoc>
        CtrlShiftL = Keys.Control + Keys.Shift + Keys.L,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+M.
        /// </devdoc>
        CtrlShiftM = Keys.Control + Keys.Shift + Keys.M,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+N.
        /// </devdoc>
        CtrlShiftN = Keys.Control + Keys.Shift + Keys.N,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+O.
        /// </devdoc>
        CtrlShiftO = Keys.Control + Keys.Shift + Keys.O,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+P.
        /// </devdoc>
        CtrlShiftP = Keys.Control + Keys.Shift + Keys.P,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+Q.
        /// </devdoc>
        CtrlShiftQ = Keys.Control + Keys.Shift + Keys.Q,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+R.
        /// </devdoc>
        CtrlShiftR = Keys.Control + Keys.Shift + Keys.R,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+S.
        /// </devdoc>
        CtrlShiftS = Keys.Control + Keys.Shift + Keys.S,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+T.
        /// </devdoc>
        CtrlShiftT = Keys.Control + Keys.Shift + Keys.T,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+U.
        /// </devdoc>
        CtrlShiftU = Keys.Control + Keys.Shift + Keys.U,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+V.
        /// </devdoc>
        CtrlShiftV = Keys.Control + Keys.Shift + Keys.V,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+W.
        /// </devdoc>
        CtrlShiftW = Keys.Control + Keys.Shift + Keys.W,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+X.
        /// </devdoc>
        CtrlShiftX = Keys.Control + Keys.Shift + Keys.X,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+Y.
        /// </devdoc>
        CtrlShiftY = Keys.Control + Keys.Shift + Keys.Y,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+Z.
        /// </devdoc>
        CtrlShiftZ = Keys.Control + Keys.Shift + Keys.Z,

        /// <devdoc>
        /// The shortcut key F1.
        /// </devdoc>
        F1 = Keys.F1,

        /// <devdoc>
        /// The shortcut key F2.
        /// </devdoc>
        F2 = Keys.F2,

        /// <devdoc>
        /// The shortcut key F3.
        /// </devdoc>
        F3 = Keys.F3,

        /// <devdoc>
        /// The shortcut key F4.
        /// </devdoc>
        F4 = Keys.F4,

        /// <devdoc>
        /// The shortcut key F5.
        /// </devdoc>
        F5 = Keys.F5,

        /// <devdoc>
        /// The shortcut key F6.
        /// </devdoc>
        F6 = Keys.F6,

        /// <devdoc>
        /// The shortcut key F7.
        /// </devdoc>
        F7 = Keys.F7,
        F8 = Keys.F8,

        /// <devdoc>
        /// The shortcut key F9.
        /// </devdoc>
        F9 = Keys.F9,

        /// <devdoc>
        /// The shortcut key F10.
        /// </devdoc>
        F10 = Keys.F10,

        /// <devdoc>
        /// The shortcut key F11.
        /// </devdoc>
        F11 = Keys.F11,

        /// <devdoc>
        /// The shortcut key F12.
        /// </devdoc>
        F12 = Keys.F12,

        /// <devdoc>
        /// The shortcut keys SHIFT+F1.
        /// </devdoc>
        ShiftF1 = Keys.Shift + Keys.F1,

        /// <devdoc>
        /// The shortcut keys SHIFT+F2.
        /// </devdoc>
        ShiftF2 = Keys.Shift + Keys.F2,

        /// <devdoc>
        /// The shortcut keys SHIFT+F3.
        /// </devdoc>
        ShiftF3 = Keys.Shift + Keys.F3,

        /// <devdoc>
        /// The shortcut keys SHIFT+F4.
        /// </devdoc>
        ShiftF4 = Keys.Shift + Keys.F4,

        /// <devdoc>
        /// The shortcut keys SHIFT+F5.
        /// </devdoc>
        ShiftF5 = Keys.Shift + Keys.F5,

        /// <devdoc>
        /// The shortcut keys SHIFT+F6.
        /// </devdoc>
        ShiftF6 = Keys.Shift + Keys.F6,

        /// <devdoc>
        /// The shortcut keys SHIFT+F7.
        /// </devdoc>
        ShiftF7 = Keys.Shift + Keys.F7,

        /// <devdoc>
        /// The shortcut keys SHIFT+F8.
        /// </devdoc>
        ShiftF8 = Keys.Shift + Keys.F8,

        /// <devdoc>
        /// The shortcut keys SHIFT+F9.
        /// </devdoc>
        ShiftF9 = Keys.Shift + Keys.F9,

        /// <devdoc>
        /// The shortcut keys SHIFT+F10.
        /// </devdoc>
        ShiftF10 = Keys.Shift + Keys.F10,

        /// <devdoc>
        /// The shortcut keys SHIFT+F11.
        /// </devdoc>
        ShiftF11 = Keys.Shift + Keys.F11,

        /// <devdoc>
        /// The shortcut keys SHIFT+F12.
        /// </devdoc>
        ShiftF12 = Keys.Shift + Keys.F12,

        /// <devdoc>
        /// The shortcut keys CTRL+F1.
        /// </devdoc>
        CtrlF1 = Keys.Control + Keys.F1,

        /// <devdoc>
        /// The shortcut keys CTRL+F2.
        /// </devdoc>
        CtrlF2 = Keys.Control + Keys.F2,

        /// <devdoc>
        /// The shortcut keys CTRL+F3.
        /// </devdoc>
        CtrlF3 = Keys.Control + Keys.F3,

        /// <devdoc>
        /// The shortcut keys CTRL+F4.
        /// </devdoc>
        CtrlF4 = Keys.Control + Keys.F4,

        /// <devdoc>
        /// The shortcut keys CTRL+F5.
        /// </devdoc>
        CtrlF5 = Keys.Control + Keys.F5,

        /// <devdoc>
        /// The shortcut keys CTRL+F6.
        /// </devdoc>
        CtrlF6 = Keys.Control + Keys.F6,

        /// <devdoc>
        /// The shortcut keys CTRL+F7.
        /// </devdoc>
        CtrlF7 = Keys.Control + Keys.F7,

        /// <devdoc>
        /// The shortcut keys CTRL+F8.
        /// </devdoc>
        CtrlF8 = Keys.Control + Keys.F8,

        /// <devdoc>
        /// The shortcut keys CTRL+F9.
        /// </devdoc>
        CtrlF9 = Keys.Control + Keys.F9,

        /// <devdoc>
        /// The shortcut keys CTRL+F10.
        /// </devdoc>
        CtrlF10 = Keys.Control + Keys.F10,

        /// <devdoc>
        /// The shortcut keys CTRL+F11.
        /// </devdoc>
        CtrlF11 = Keys.Control + Keys.F11,

        /// <devdoc>
        /// The shortcut keys CTRL+F12.
        /// </devdoc>
        CtrlF12 = Keys.Control + Keys.F12,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F1.
        /// </devdoc>
        CtrlShiftF1 = Keys.Control + Keys.Shift + Keys.F1,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F2.
        /// </devdoc>
        CtrlShiftF2 = Keys.Control + Keys.Shift + Keys.F2,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F3.
        /// </devdoc>
        CtrlShiftF3 = Keys.Control + Keys.Shift + Keys.F3,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F4.
        /// </devdoc>
        CtrlShiftF4 = Keys.Control + Keys.Shift + Keys.F4,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F5.
        /// </devdoc>
        CtrlShiftF5 = Keys.Control + Keys.Shift + Keys.F5,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F6.
        /// </devdoc>
        CtrlShiftF6 = Keys.Control + Keys.Shift + Keys.F6,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F7.
        /// </devdoc>
        CtrlShiftF7 = Keys.Control + Keys.Shift + Keys.F7,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F8.
        /// </devdoc>
        CtrlShiftF8 = Keys.Control + Keys.Shift + Keys.F8,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F9.
        /// </devdoc>
        CtrlShiftF9 = Keys.Control + Keys.Shift + Keys.F9,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F10.
        /// </devdoc>
        CtrlShiftF10 = Keys.Control + Keys.Shift + Keys.F10,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F11.
        /// </devdoc>
        CtrlShiftF11 = Keys.Control + Keys.Shift + Keys.F11,

        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+F12.
        /// </devdoc>
        CtrlShiftF12 = Keys.Control + Keys.Shift + Keys.F12,

        /// <devdoc>
        /// The shortcut key INSERT.
        /// </devdoc>
        Ins = Keys.Insert,

        /// <devdoc>
        /// The shortcut keys CTRL+INSERT.
        /// </devdoc>
        CtrlIns = Keys.Control + Keys.Insert,

        /// <devdoc>
        /// The shortcut keys SHIFT+INSERT.
        /// </devdoc>
        ShiftIns = Keys.Shift + Keys.Insert,

        /// <devdoc>
        /// The shortcut key DELETE.
        /// </devdoc>
        Del = Keys.Delete,

        /// <devdoc>
        /// The shortcut keys CTRL+DELETE.
        /// </devdoc>
        CtrlDel = Keys.Control + Keys.Delete,

        /// <devdoc>
        /// The shortcut keys SHIFT+DELETE.
        /// </devdoc>
        ShiftDel = Keys.Shift + Keys.Delete,

        /// <devdoc>
        /// The shortcut keys Alt + RightArrow.
        /// </devdoc>
        AltRightArrow = Keys.Alt + Keys.Right,

        /// <devdoc>
        /// The shortcut keys ALT+LeftArrow.
        /// </devdoc>
        AltLeftArrow = Keys.Alt + Keys.Left,

        /// <devdoc>
        /// The shortcut keys ALT+UpArrow.
        /// </devdoc>
        AltUpArrow = Keys.Alt + Keys.Up,

        /// <devdoc>
        /// The shortcut keys Alt + DownArrow.
        /// </devdoc>
        AltDownArrow = Keys.Alt + Keys.Down,

        /// <devdoc>
        /// The shortcut keys ALT+BACKSPACE.
        /// </devdoc>
        AltBksp = Keys.Alt + Keys.Back,

        /// <devdoc>
        /// The shortcut keys ALT+F1.
        /// </devdoc>
        AltF1 = Keys.Alt + Keys.F1,

        /// <devdoc>
        /// The shortcut keys ALT+F2.
        /// </devdoc>
        AltF2 = Keys.Alt + Keys.F2,

        /// <devdoc>
        /// The shortcut keys ALT+F3.
        /// </devdoc>
        AltF3 = Keys.Alt + Keys.F3,

        /// <devdoc>
        /// The shortcut keys ALT+F4.
        /// </devdoc>
        AltF4 = Keys.Alt + Keys.F4,

        /// <devdoc>
        /// The shortcut keys ALT+F5.
        /// </devdoc>
        AltF5 = Keys.Alt + Keys.F5,

        /// <devdoc>
        /// The shortcut keys ALT+F6.
        /// </devdoc>
        AltF6 = Keys.Alt + Keys.F6,

        /// <devdoc>
        /// The shortcut keys ALT+F7.
        /// </devdoc>
        AltF7 = Keys.Alt + Keys.F7,

        /// <devdoc>
        /// The shortcut keys ALT+F8.
        /// </devdoc>
        AltF8 = Keys.Alt + Keys.F8,

        /// <devdoc>
        /// The shortcut keys ALT+F9.
        /// </devdoc>
        AltF9 = Keys.Alt + Keys.F9,

        /// <devdoc>
        /// The shortcut keys ALT+F10.
        /// </devdoc>
        AltF10 = Keys.Alt + Keys.F10,

        /// <devdoc>
        /// The shortcut keys ALT+F11.
        /// </devdoc>
        AltF11 = Keys.Alt + Keys.F11,

        /// <devdoc>
        /// The shortcut keys ALT+F12.
        /// </devdoc>
        AltF12 = Keys.Alt + Keys.F12,


        /// <devdoc>
        /// The shortcut keys ALT+0.
        /// </devdoc>
        Alt0 = Keys.Alt + Keys.D0,


        /// <devdoc>
        /// The shortcut keys ALT+1.
        /// </devdoc>
        Alt1 = Keys.Alt + Keys.D1,


        /// <devdoc>
        /// The shortcut keys ALT+2.
        /// </devdoc>
        Alt2 = Keys.Alt + Keys.D2,


        /// <devdoc>
        /// The shortcut keys ALT+3.
        /// </devdoc>
        Alt3 = Keys.Alt + Keys.D3,


        /// <devdoc>
        /// The shortcut keys ALT+4.
        /// </devdoc>
        Alt4 = Keys.Alt + Keys.D4,


        /// <devdoc>
        /// The shortcut keys ALT+5.
        /// </devdoc>
        Alt5 = Keys.Alt + Keys.D5,


        /// <devdoc>
        /// The shortcut keys ALT+6.
        /// </devdoc>
        Alt6 = Keys.Alt + Keys.D6,


        /// <devdoc>
        /// The shortcut keys ALT+7.
        /// </devdoc>
        Alt7 = Keys.Alt + Keys.D7,


        /// <devdoc>
        /// The shortcut keys ALT+8.
        /// </devdoc>
        Alt8 = Keys.Alt + Keys.D8,


        /// <devdoc>
        /// The shortcut keys ALT+9.
        /// </devdoc>
        Alt9 = Keys.Alt + Keys.D9,


        /// <devdoc>
        /// The shortcut keys CTRL+0.
        /// </devdoc>
        Ctrl0 = Keys.Control + Keys.D0,


        /// <devdoc>
        /// The shortcut keys CTRL+1.
        /// </devdoc>
        Ctrl1 = Keys.Control + Keys.D1,


        /// <devdoc>
        /// The shortcut keys CTRL+2.
        /// </devdoc>
        Ctrl2 = Keys.Control + Keys.D2,


        /// <devdoc>
        /// The shortcut keys CTRL+3.
        /// </devdoc>
        Ctrl3 = Keys.Control + Keys.D3,


        /// <devdoc>
        /// The shortcut keys CTRL+4.
        /// </devdoc>
        Ctrl4 = Keys.Control + Keys.D4,


        /// <devdoc>
        /// The shortcut keys CTRL+5.
        /// </devdoc>
        Ctrl5 = Keys.Control + Keys.D5,


        /// <devdoc>
        /// The shortcut keys CTRL+6.
        /// </devdoc>
        Ctrl6 = Keys.Control + Keys.D6,


        /// <devdoc>
        /// The shortcut keys CTRL+7.
        /// </devdoc>
        Ctrl7 = Keys.Control + Keys.D7,


        /// <devdoc>
        /// The shortcut keys CTRL+8.
        /// </devdoc>
        Ctrl8 = Keys.Control + Keys.D8,


        /// <devdoc>
        /// The shortcut keys CTRL+9.
        /// </devdoc>
        Ctrl9 = Keys.Control + Keys.D9,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+0.
        /// </devdoc>
        CtrlShift0 = Keys.Control + Keys.Shift + Keys.D0,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+1.
        /// </devdoc>
        CtrlShift1 = Keys.Control + Keys.Shift + Keys.D1,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+2.
        /// </devdoc>
        CtrlShift2 = Keys.Control + Keys.Shift + Keys.D2,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+3.
        /// </devdoc>
        CtrlShift3 = Keys.Control + Keys.Shift + Keys.D3,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+4.
        /// </devdoc>
        CtrlShift4 = Keys.Control + Keys.Shift + Keys.D4,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+5.
        /// </devdoc>
        CtrlShift5 = Keys.Control + Keys.Shift + Keys.D5,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+6.
        /// </devdoc>
        CtrlShift6 = Keys.Control + Keys.Shift + Keys.D6,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+7.
        /// </devdoc>
        CtrlShift7 = Keys.Control + Keys.Shift + Keys.D7,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+8.
        /// </devdoc>
        CtrlShift8 = Keys.Control + Keys.Shift + Keys.D8,


        /// <devdoc>
        /// The shortcut keys CTRL+SHIFT+9.
        /// </devdoc>
        CtrlShift9 = Keys.Control + Keys.Shift + Keys.D9,
    }
}
