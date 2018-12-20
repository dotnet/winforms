// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;    
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies shortcut keys that can be used by menu items.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum Shortcut {

        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No shortcut key is associated with the menu item.
        ///    </para>
        /// </devdoc>
        None = 0,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlA"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+A.
        ///    </para>
        /// </devdoc>
        CtrlA = Keys.Control + Keys.A,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlB"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+B.
        ///    </para>
        /// </devdoc>
        CtrlB = Keys.Control + Keys.B,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlC"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+C.
        ///    </para>
        /// </devdoc>
        CtrlC = Keys.Control + Keys.C,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlD"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+D.
        ///    </para>
        /// </devdoc>
        CtrlD = Keys.Control + Keys.D,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlE"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+E.
        ///    </para>
        /// </devdoc>
        CtrlE = Keys.Control + Keys.E,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+F.
        ///    </para>
        /// </devdoc>
        CtrlF = Keys.Control + Keys.F,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlG"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+G.
        ///    </para>
        /// </devdoc>
        CtrlG = Keys.Control + Keys.G,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlH"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+H.
        ///    </para>
        /// </devdoc>
        CtrlH = Keys.Control + Keys.H,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlI"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+I.
        ///    </para>
        /// </devdoc>
        CtrlI = Keys.Control + Keys.I,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlJ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+J.
        ///    </para>
        /// </devdoc>
        CtrlJ = Keys.Control + Keys.J,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlK"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+K.
        ///    </para>
        /// </devdoc>
        CtrlK = Keys.Control + Keys.K,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlL"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+L.
        ///    </para>
        /// </devdoc>
        CtrlL = Keys.Control + Keys.L,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlM"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+M.
        ///    </para>
        /// </devdoc>
        CtrlM = Keys.Control + Keys.M,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlN"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+N.
        ///    </para>
        /// </devdoc>
        CtrlN = Keys.Control + Keys.N,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlO"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+O.
        ///    </para>
        /// </devdoc>
        CtrlO = Keys.Control + Keys.O,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlP"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+P.
        ///    </para>
        /// </devdoc>
        CtrlP = Keys.Control + Keys.P,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlQ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+Q.
        ///    </para>
        /// </devdoc>
        CtrlQ = Keys.Control + Keys.Q,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlR"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+R.
        ///    </para>
        /// </devdoc>
        CtrlR = Keys.Control + Keys.R,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlS"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+S.
        ///    </para>
        /// </devdoc>
        CtrlS = Keys.Control + Keys.S,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlT"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+T.
        ///    </para>
        /// </devdoc>
        CtrlT = Keys.Control + Keys.T,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlU"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+U
        ///    </para>
        /// </devdoc>
        CtrlU = Keys.Control + Keys.U,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlV"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+V.
        ///    </para>
        /// </devdoc>
        CtrlV = Keys.Control + Keys.V,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlW"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+W.
        ///    </para>
        /// </devdoc>
        CtrlW = Keys.Control + Keys.W,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlX"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+X.                
        ///    </para>
        /// </devdoc>
        CtrlX = Keys.Control + Keys.X,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlY"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+Y.
        ///    </para>
        /// </devdoc>
        CtrlY = Keys.Control + Keys.Y,                     
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlZ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shorcut keys CTRL+Z.
        ///    </para>
        /// </devdoc>
        CtrlZ = Keys.Control + Keys.Z,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftA"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+A.
        ///    </para>
        /// </devdoc>
        CtrlShiftA = Keys.Control + Keys.Shift + Keys.A,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftB"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+B.
        ///    </para>
        /// </devdoc>
        CtrlShiftB = Keys.Control + Keys.Shift + Keys.B,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftC"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+C.
        ///    </para>
        /// </devdoc>
        CtrlShiftC = Keys.Control + Keys.Shift + Keys.C,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftD"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+D.
        ///    </para>
        /// </devdoc>
        CtrlShiftD = Keys.Control + Keys.Shift + Keys.D,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftE"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+E.
        ///    </para>
        /// </devdoc>
        CtrlShiftE = Keys.Control + Keys.Shift + Keys.E,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F.
        ///    </para>
        /// </devdoc>
        CtrlShiftF = Keys.Control + Keys.Shift + Keys.F,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftG"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+G.
        ///    </para>
        /// </devdoc>
        CtrlShiftG = Keys.Control + Keys.Shift + Keys.G,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftH"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+H.
        ///    </para>
        /// </devdoc>
        CtrlShiftH = Keys.Control + Keys.Shift + Keys.H,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftI"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+I.
        ///    </para>
        /// </devdoc>
        CtrlShiftI = Keys.Control + Keys.Shift + Keys.I,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftJ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+J.
        ///    </para>
        /// </devdoc>
        CtrlShiftJ = Keys.Control + Keys.Shift + Keys.J,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftK"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+K.
        ///    </para>
        /// </devdoc>
        CtrlShiftK = Keys.Control + Keys.Shift + Keys.K,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftL"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+L.
        ///    </para>
        /// </devdoc>
        CtrlShiftL = Keys.Control + Keys.Shift + Keys.L,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftM"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+M.
        ///    </para>
        /// </devdoc>
        CtrlShiftM = Keys.Control + Keys.Shift + Keys.M,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftN"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+N.
        ///    </para>
        /// </devdoc>
        CtrlShiftN = Keys.Control + Keys.Shift + Keys.N,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftO"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+O.
        ///    </para>
        /// </devdoc>
        CtrlShiftO = Keys.Control + Keys.Shift + Keys.O,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftP"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+P.
        ///    </para>
        /// </devdoc>
        CtrlShiftP = Keys.Control + Keys.Shift + Keys.P,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftQ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+Q.
        ///    </para>
        /// </devdoc>
        CtrlShiftQ = Keys.Control + Keys.Shift + Keys.Q,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftR"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+R.
        ///    </para>
        /// </devdoc>
        CtrlShiftR = Keys.Control + Keys.Shift + Keys.R,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftS"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+S.
        ///    </para>
        /// </devdoc>
        CtrlShiftS = Keys.Control + Keys.Shift + Keys.S,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftT"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+T.
        ///    </para>
        /// </devdoc>
        CtrlShiftT = Keys.Control + Keys.Shift + Keys.T,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftU"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+U.
        ///    </para>
        /// </devdoc>
        CtrlShiftU = Keys.Control + Keys.Shift + Keys.U,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftV"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+V.
        ///    </para>
        /// </devdoc>
        CtrlShiftV = Keys.Control + Keys.Shift + Keys.V,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftW"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+W.
        ///    </para>
        /// </devdoc>
        CtrlShiftW = Keys.Control + Keys.Shift + Keys.W,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftX"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+X.
        ///    </para>
        /// </devdoc>
        CtrlShiftX = Keys.Control + Keys.Shift + Keys.X,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftY"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+Y.
        ///    </para>
        /// </devdoc>
        CtrlShiftY = Keys.Control + Keys.Shift + Keys.Y,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftZ"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+Z.
        ///    </para>
        /// </devdoc>
        CtrlShiftZ = Keys.Control + Keys.Shift + Keys.Z,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F1.
        ///    </para>
        /// </devdoc>
        F1 = Keys.F1,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F2.
        ///    </para>
        /// </devdoc>
        F2 = Keys.F2,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F3.
        ///    </para>
        /// </devdoc>
        F3 = Keys.F3,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F4.
        ///    </para>
        /// </devdoc>
        F4 = Keys.F4,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F5.
        ///    </para>
        /// </devdoc>
        F5 = Keys.F5,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F6.
        ///    </para>
        /// </devdoc>
        F6 = Keys.F6,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F7.
        ///    </para>
        /// </devdoc>
        F7 = Keys.F7,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F8"]/*' />
        F8 = Keys.F8,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F9.
        ///    </para>
        /// </devdoc>
        F9 = Keys.F9,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F10.
        ///    </para>
        /// </devdoc>
        F10 = Keys.F10,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F11.
        ///    </para>
        /// </devdoc>
        F11 = Keys.F11,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.F12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key F12.
        ///    </para>
        /// </devdoc>
        F12 = Keys.F12,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F1.
        ///    </para>
        /// </devdoc>
        ShiftF1 = Keys.Shift + Keys.F1,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F2.
        ///    </para>
        /// </devdoc>
        ShiftF2 = Keys.Shift + Keys.F2,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F3.
        ///    </para>
        /// </devdoc>
        ShiftF3 = Keys.Shift + Keys.F3,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F4.
        ///    </para>
        /// </devdoc>
        ShiftF4 = Keys.Shift + Keys.F4,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F5.
        ///    </para>
        /// </devdoc>
        ShiftF5 = Keys.Shift + Keys.F5,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F6.
        ///    </para>
        /// </devdoc>
        ShiftF6 = Keys.Shift + Keys.F6,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F7.
        ///    </para>
        /// </devdoc>
        ShiftF7 = Keys.Shift + Keys.F7,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F8.
        ///    </para>
        /// </devdoc>
        ShiftF8 = Keys.Shift + Keys.F8,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F9.
        ///    </para>
        /// </devdoc>
        ShiftF9 = Keys.Shift + Keys.F9,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F10.
        ///    </para>
        /// </devdoc>
        ShiftF10 = Keys.Shift + Keys.F10,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F11.
        ///    </para>
        /// </devdoc>
        ShiftF11 = Keys.Shift + Keys.F11,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftF12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+F12.
        ///    </para>
        /// </devdoc>
        ShiftF12 = Keys.Shift + Keys.F12,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F1.
        ///    </para>
        /// </devdoc>
        CtrlF1 = Keys.Control + Keys.F1,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F2.
        ///    </para>
        /// </devdoc>
        CtrlF2 = Keys.Control + Keys.F2,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F3.
        ///    </para>
        /// </devdoc>
        CtrlF3 = Keys.Control + Keys.F3,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F4.
        ///    </para>
        /// </devdoc>
        CtrlF4 = Keys.Control + Keys.F4,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F5.
        ///    </para>
        /// </devdoc>
        CtrlF5 = Keys.Control + Keys.F5,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F6.
        ///    </para>
        /// </devdoc>
        CtrlF6 = Keys.Control + Keys.F6,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F7.
        ///    </para>
        /// </devdoc>
        CtrlF7 = Keys.Control + Keys.F7,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F8.
        ///    </para>
        /// </devdoc>
        CtrlF8 = Keys.Control + Keys.F8,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F9.
        ///    </para>
        /// </devdoc>
        CtrlF9 = Keys.Control + Keys.F9,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F10.
        ///    </para>
        /// </devdoc>
        CtrlF10 = Keys.Control + Keys.F10,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F11.
        ///    </para>
        /// </devdoc>
        CtrlF11 = Keys.Control + Keys.F11,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlF12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+F12.
        ///    </para>
        /// </devdoc>
        CtrlF12 = Keys.Control + Keys.F12,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F1.
        ///    </para>
        /// </devdoc>
        CtrlShiftF1 = Keys.Control + Keys.Shift + Keys.F1,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F2.
        ///    </para>
        /// </devdoc>
        CtrlShiftF2 = Keys.Control + Keys.Shift + Keys.F2,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F3.
        ///    </para>
        /// </devdoc>
        CtrlShiftF3 = Keys.Control + Keys.Shift + Keys.F3,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F4.
        ///    </para>
        /// </devdoc>
        CtrlShiftF4 = Keys.Control + Keys.Shift + Keys.F4,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F5.
        ///    </para>
        /// </devdoc>
        CtrlShiftF5 = Keys.Control + Keys.Shift + Keys.F5,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F6.
        ///    </para>
        /// </devdoc>
        CtrlShiftF6 = Keys.Control + Keys.Shift + Keys.F6,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F7.
        ///    </para>
        /// </devdoc>
        CtrlShiftF7 = Keys.Control + Keys.Shift + Keys.F7,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F8.
        ///    </para>
        /// </devdoc>
        CtrlShiftF8 = Keys.Control + Keys.Shift + Keys.F8,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F9.
        ///    </para>
        /// </devdoc>
        CtrlShiftF9 = Keys.Control + Keys.Shift + Keys.F9,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F10.
        ///    </para>
        /// </devdoc>
        CtrlShiftF10 = Keys.Control + Keys.Shift + Keys.F10,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F11.
        ///    </para>
        /// </devdoc>
        CtrlShiftF11 = Keys.Control + Keys.Shift + Keys.F11,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShiftF12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+F12.
        ///    </para>
        /// </devdoc>
        CtrlShiftF12 = Keys.Control + Keys.Shift + Keys.F12,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ins"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key INSERT.
        ///    </para>
        /// </devdoc>
        Ins = Keys.Insert,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlIns"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+INSERT.
        ///    </para>
        /// </devdoc>
        CtrlIns = Keys.Control + Keys.Insert,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftIns"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+INSERT.
        ///    </para>
        /// </devdoc>
        ShiftIns = Keys.Shift + Keys.Insert,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Del"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut key DELETE.
        ///    </para>
        /// </devdoc>
        Del = Keys.Delete,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlDel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+DELETE.
        ///    </para>
        /// </devdoc>
        CtrlDel = Keys.Control + Keys.Delete,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.ShiftDel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys SHIFT+DELETE.
        ///    </para>
        /// </devdoc>
        ShiftDel = Keys.Shift + Keys.Delete,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltRightArrow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys Alt + RightArrow.
        ///    </para>
        /// </devdoc>
        AltRightArrow = Keys.Alt + Keys.Right,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltLeftArrow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+LeftArrow.
        ///    </para>
        /// </devdoc>
        AltLeftArrow = Keys.Alt + Keys.Left,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltUpArrow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+UpArrow.
        ///    </para>
        /// </devdoc>
        AltUpArrow = Keys.Alt + Keys.Up,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltDownArrow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys Alt + DownArrow.
        ///    </para>
        /// </devdoc>
        AltDownArrow = Keys.Alt + Keys.Down,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltBksp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+BACKSPACE.
        ///    </para>
        /// </devdoc>
        AltBksp = Keys.Alt + Keys.Back,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F1.
        ///    </para>
        /// </devdoc>
        AltF1 = Keys.Alt + Keys.F1,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F2.
        ///    </para>
        /// </devdoc>
        AltF2 = Keys.Alt + Keys.F2,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F3.
        ///    </para>
        /// </devdoc>
        AltF3 = Keys.Alt + Keys.F3,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F4.
        ///    </para>
        /// </devdoc>
        AltF4 = Keys.Alt + Keys.F4,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F5.
        ///    </para>
        /// </devdoc>
        AltF5 = Keys.Alt + Keys.F5,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F6.
        ///    </para>
        /// </devdoc>
        AltF6 = Keys.Alt + Keys.F6,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F7.
        ///    </para>
        /// </devdoc>
        AltF7 = Keys.Alt + Keys.F7,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F8.
        ///    </para>
        /// </devdoc>
        AltF8 = Keys.Alt + Keys.F8,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F9.
        ///    </para>
        /// </devdoc>
        AltF9 = Keys.Alt + Keys.F9,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F10.
        ///    </para>
        /// </devdoc>
        AltF10 = Keys.Alt + Keys.F10,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F11.
        ///    </para>
        /// </devdoc>
        AltF11 = Keys.Alt + Keys.F11,
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.AltF12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+F12.
        ///    </para>
        /// </devdoc>
        AltF12 = Keys.Alt + Keys.F12,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+0.
        ///    </para>
        /// </devdoc>
        Alt0 = Keys.Alt + Keys.D0,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+1.
        ///    </para>
        /// </devdoc>
        Alt1 = Keys.Alt + Keys.D1,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+2.
        ///    </para>
        /// </devdoc>
        Alt2 = Keys.Alt + Keys.D2,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+3.
        ///    </para>
        /// </devdoc>
        Alt3 = Keys.Alt + Keys.D3,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+4.
        ///    </para>
        /// </devdoc>
        Alt4 = Keys.Alt + Keys.D4,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+5.
        ///    </para>
        /// </devdoc>
        Alt5 = Keys.Alt + Keys.D5,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+6.
        ///    </para>
        /// </devdoc>
        Alt6 = Keys.Alt + Keys.D6,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+7.
        ///    </para>
        /// </devdoc>
        Alt7 = Keys.Alt + Keys.D7,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+8.
        ///    </para>
        /// </devdoc>
        Alt8 = Keys.Alt + Keys.D8,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Alt9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys ALT+9.
        ///    </para>
        /// </devdoc>
        Alt9 = Keys.Alt + Keys.D9,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+0.
        ///    </para>
        /// </devdoc>
        Ctrl0 = Keys.Control + Keys.D0,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+1.
        ///    </para>
        /// </devdoc>
        Ctrl1 = Keys.Control + Keys.D1,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+2.
        ///    </para>
        /// </devdoc>
        Ctrl2 = Keys.Control + Keys.D2,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+3.
        ///    </para>
        /// </devdoc>
        Ctrl3 = Keys.Control + Keys.D3,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+4.
        ///    </para>
        /// </devdoc>
        Ctrl4 = Keys.Control + Keys.D4,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+5.
        ///    </para>
        /// </devdoc>
        Ctrl5 = Keys.Control + Keys.D5,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+6.
        ///    </para>
        /// </devdoc>
        Ctrl6 = Keys.Control + Keys.D6,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+7.
        ///    </para>
        /// </devdoc>
        Ctrl7 = Keys.Control + Keys.D7,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+8.
        ///    </para>
        /// </devdoc>
        Ctrl8 = Keys.Control + Keys.D8,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.Ctrl9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+9.
        ///    </para>
        /// </devdoc>
        Ctrl9 = Keys.Control + Keys.D9,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+0.
        ///    </para>
        /// </devdoc>
        CtrlShift0 = Keys.Control + Keys.Shift + Keys.D0,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+1.
        ///    </para>
        /// </devdoc>
        CtrlShift1 = Keys.Control + Keys.Shift + Keys.D1,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+2.
        ///    </para>
        /// </devdoc>
        CtrlShift2 = Keys.Control + Keys.Shift + Keys.D2,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+3.
        ///    </para>
        /// </devdoc>
        CtrlShift3 = Keys.Control + Keys.Shift + Keys.D3,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+4.
        ///    </para>
        /// </devdoc>
        CtrlShift4 = Keys.Control + Keys.Shift + Keys.D4,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+5.
        ///    </para>
        /// </devdoc>
        CtrlShift5 = Keys.Control + Keys.Shift + Keys.D5,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+6.
        ///    </para>
        /// </devdoc>
        CtrlShift6 = Keys.Control + Keys.Shift + Keys.D6,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+7.
        ///    </para>
        /// </devdoc>
        CtrlShift7 = Keys.Control + Keys.Shift + Keys.D7,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+8.
        ///    </para>
        /// </devdoc>
        CtrlShift8 = Keys.Control + Keys.Shift + Keys.D8,
        
        /// <include file='doc\Shortcut.uex' path='docs/doc[@for="Shortcut.CtrlShift9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The shortcut keys CTRL+SHIFT+9.
        ///    </para>
        /// </devdoc>
        CtrlShift9 = Keys.Control + Keys.Shift + Keys.D9,
        
    }
}
