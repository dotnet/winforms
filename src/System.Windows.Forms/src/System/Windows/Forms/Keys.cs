// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Drawing.Design;
    using Microsoft.Win32;

    /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies key codes and modifiers.
    ///    </para>
    /// </devdoc>
    [
    Flags,
    TypeConverterAttribute(typeof(KeysConverter)), 
    Editor("System.Windows.Forms.Design.ShortcutKeysEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
    ]
    [System.Runtime.InteropServices.ComVisible(true)]
    [
        SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")    // Certain memberd of Keys enum are actually meant to be OR'ed.
    ]
    public enum Keys {
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.KeyCode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The bit mask to extract a key code from a key value.
        ///       
        ///    </para>
        /// </devdoc>
        KeyCode = 0x0000FFFF,

        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Modifiers"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The bit mask to extract modifiers from a key value.
        ///       
        ///    </para>
        /// </devdoc>
        Modifiers = unchecked((int)0xFFFF0000),

        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No key pressed.
        ///    </para>
        /// </devdoc>
        None           = 0x00,

        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The left mouse button.
        ///       
        ///    </para>
        /// </devdoc>
        LButton        = 0x01,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The right mouse button.
        ///    </para>
        /// </devdoc>
        RButton        = 0x02,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Cancel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CANCEL key.
        ///    </para>
        /// </devdoc>
        Cancel         = 0x03,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The middle mouse button (three-button mouse).
        ///    </para>
        /// </devdoc>
        MButton        = 0x04,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.XButton1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The first x mouse button (five-button mouse).
        ///    </para>
        /// </devdoc>
        XButton1       = 0x05,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.XButton2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The second x mouse button (five-button mouse).
        ///    </para>
        /// </devdoc>
        XButton2       = 0x06,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Back"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The BACKSPACE key.
        ///    </para>
        /// </devdoc>
        Back           = 0x08,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Tab"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The TAB key.
        ///    </para>
        /// </devdoc>
        Tab            = 0x09,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LineFeed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        LineFeed       = 0x0A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Clear"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        Clear          = 0x0C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Return"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The RETURN key.
        ///
        ///    </para>
        /// </devdoc>
        Return         = 0x0D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Enter"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ENTER key.
        ///       
        ///    </para>
        /// </devdoc>
        Enter          = Return,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.ShiftKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The SHIFT key.
        ///    </para>
        /// </devdoc>
        ShiftKey      = 0x10,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.ControlKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CTRL key.
        ///    </para>
        /// </devdoc>
        ControlKey    = 0x11,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Menu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ALT key.
        ///    </para>
        /// </devdoc>
        Menu           = 0x12,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Pause"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PAUSE key.
        ///    </para>
        /// </devdoc>
        Pause          = 0x13,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Capital"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CAPS LOCK key.
        ///
        ///    </para>
        /// </devdoc>
        Capital        = 0x14,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.CapsLock"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CAPS LOCK key.
        ///    </para>
        /// </devdoc>
        CapsLock       = 0x14,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Kana"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Kana mode key.
        ///    </para>
        /// </devdoc>
        KanaMode      = 0x15,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.HanguelMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Hanguel mode key.
        ///    </para>
        /// </devdoc>
        HanguelMode   = 0x15,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.HangulMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Hangul mode key.
        ///    </para>
        /// </devdoc>
        HangulMode    = 0x15,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.JunjaMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Junja mode key.
        ///    </para>
        /// </devdoc>
        JunjaMode     = 0x17,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.FinalMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Final mode key.
        ///    </para>
        /// </devdoc>
        FinalMode     = 0x18,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.HanjaMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Hanja mode key.
        ///    </para>
        /// </devdoc>
        HanjaMode     = 0x19,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.KanjiMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Kanji mode key.
        ///    </para>
        /// </devdoc>
        KanjiMode     = 0x19,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Escape"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ESC key.
        ///    </para>
        /// </devdoc>
        Escape         = 0x1B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.IMEConvert"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Convert key.
        ///    </para>
        /// </devdoc>
        IMEConvert    = 0x1C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.IMENonconvert"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME NonConvert key.
        ///    </para>
        /// </devdoc>
        IMENonconvert = 0x1D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.IMEAccept"]/*' />
	/// <devdoc>
        ///    <para>
        ///       The IME Accept key.
        ///    </para>
        /// </devdoc>
        IMEAccept     = 0x1E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.IMEAceept"]/*' />
	/// <devdoc>
        ///    <para>
        ///       The IME Accept key.
        ///    </para>
        /// </devdoc>
        IMEAceept     = IMEAccept,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.IMEModeChange"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The IME Mode change request.
        ///    </para>
        /// </devdoc>
        IMEModeChange = 0x1F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Space"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The SPACEBAR key.
        ///    </para>
        /// </devdoc>
        Space          = 0x20,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Prior"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PAGE UP key.
        ///    </para>
        /// </devdoc>
        Prior          = 0x21,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.PageUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PAGE UP key.
        ///    </para>
        /// </devdoc>
        PageUp         = Prior,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Next"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PAGE DOWN key.
        ///    </para>
        /// </devdoc>
        Next           = 0x22,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.PageDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PAGE DOWN key.
        ///    </para>
        /// </devdoc>
        PageDown       = Next,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.End"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The END key.
        ///    </para>
        /// </devdoc>
        End            = 0x23,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Home"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The HOME key.
        ///    </para>
        /// </devdoc>
        Home           = 0x24,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The LEFT ARROW key.
        ///    </para>
        /// </devdoc>
        Left           = 0x25,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Up"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The UP ARROW key.
        ///    </para>
        /// </devdoc>
        Up             = 0x26,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The RIGHT ARROW key.
        ///    </para>
        /// </devdoc>
        Right          = 0x27,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Down"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The DOWN ARROW key.
        ///    </para>
        /// </devdoc>
        Down           = 0x28,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Select"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The SELECT key.
        ///    </para>
        /// </devdoc>
        Select         = 0x29,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Print"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PRINT key.
        ///    </para>
        /// </devdoc>
        Print          = 0x2A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Execute"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The EXECUTE key.
        ///    </para>
        /// </devdoc>
        Execute        = 0x2B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Snapshot"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PRINT SCREEN key.
        ///
        ///    </para>
        /// </devdoc>
        Snapshot       = 0x2C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.PrintScreen"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PRINT SCREEN key.
        ///    </para>
        /// </devdoc>
        PrintScreen    = Snapshot,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Insert"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The INS key.
        ///    </para>
        /// </devdoc>
        Insert         = 0x2D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Delete"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The DEL key.
        ///    </para>
        /// </devdoc>
        Delete         = 0x2E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Help"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The HELP key.
        ///    </para>
        /// </devdoc>
        Help           = 0x2F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 0 key.
        ///    </para>
        /// </devdoc>
        D0             = 0x30, // 0
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 1 key.
        ///    </para>
        /// </devdoc>
        D1             = 0x31, // 1
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 2 key.
        ///    </para>
        /// </devdoc>
        D2             = 0x32, // 2
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 3 key.
        ///    </para>
        /// </devdoc>
        D3             = 0x33, // 3
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 4 key.
        ///    </para>
        /// </devdoc>
        D4             = 0x34, // 4
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 5 key.
        ///    </para>
        /// </devdoc>
        D5             = 0x35, // 5
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 6 key.
        ///    </para>
        /// </devdoc>
        D6             = 0x36, // 6
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 7 key.
        ///    </para>
        /// </devdoc>
        D7             = 0x37, // 7
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 8 key.
        ///    </para>
        /// </devdoc>
        D8             = 0x38, // 8
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 9 key.
        ///    </para>
        /// </devdoc>
        D9             = 0x39, // 9
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.A"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The A key.
        ///    </para>
        /// </devdoc>
        A              = 0x41,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.B"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The B key.
        ///    </para>
        /// </devdoc>
        B              = 0x42,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.C"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The C key.
        ///    </para>
        /// </devdoc>
        C              = 0x43,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The D key.
        ///    </para>
        /// </devdoc>
        D              = 0x44,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.E"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The E key.
        ///    </para>
        /// </devdoc>
        E              = 0x45,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F key.
        ///    </para>
        /// </devdoc>
        F              = 0x46,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.G"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The G key.
        ///    </para>
        /// </devdoc>
        G              = 0x47,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.H"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The H key.
        ///    </para>
        /// </devdoc>
        H              = 0x48,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.I"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The I key.
        ///    </para>
        /// </devdoc>
        I              = 0x49,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.J"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The J key.
        ///    </para>
        /// </devdoc>
        J              = 0x4A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.K"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The K key.
        ///    </para>
        /// </devdoc>
        K              = 0x4B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.L"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The L key.
        ///    </para>
        /// </devdoc>
        L              = 0x4C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.M"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The M key.
        ///    </para>
        /// </devdoc>
        M              = 0x4D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.N"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The N key.
        ///    </para>
        /// </devdoc>
        N              = 0x4E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.O"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The O key.
        ///    </para>
        /// </devdoc>
        O              = 0x4F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.P"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The P key.
        ///    </para>
        /// </devdoc>
        P              = 0x50,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Q"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Q key.
        ///    </para>
        /// </devdoc>
        Q              = 0x51,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.R"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The R key.
        ///    </para>
        /// </devdoc>
        R              = 0x52,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.S"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The S key.
        ///    </para>
        /// </devdoc>
        S              = 0x53,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.T"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The T key.
        ///    </para>
        /// </devdoc>
        T              = 0x54,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.U"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The U key.
        ///    </para>
        /// </devdoc>
        U              = 0x55,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.V"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The V key.
        ///    </para>
        /// </devdoc>
        V              = 0x56,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.W"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The W key.
        ///    </para>
        /// </devdoc>
        W              = 0x57,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The X key.
        ///    </para>
        /// </devdoc>
        X              = 0x58,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Y key.
        ///    </para>
        /// </devdoc>
        Y              = 0x59,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Z"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Z key.
        ///    </para>
        /// </devdoc>
        Z              = 0x5A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LWin"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The left Windows logo key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        LWin           = 0x5B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RWin"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The right Windows logo key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        RWin           = 0x5C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Apps"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Application key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        Apps           = 0x5D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Sleep"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Computer Sleep key.
        ///    </para>
        /// </devdoc>
        Sleep          = 0x5F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 0 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad0        = 0x60,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 1 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad1        = 0x61,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 2 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad2        = 0x62,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 3 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad3        = 0x63,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 4 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad4        = 0x64,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 5 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad5        = 0x65,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 6 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad6        = 0x66,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 7 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad7        = 0x67,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 8 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad8        = 0x68,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The 9 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad9        = 0x69,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Multiply"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Multiply key.
        ///    </para>
        /// </devdoc>
        Multiply       = 0x6A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Add"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Add key.
        ///    </para>
        /// </devdoc>
        Add            = 0x6B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Separator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Separator key.
        ///    </para>
        /// </devdoc>
        Separator      = 0x6C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Subtract"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Subtract key.
        ///    </para>
        /// </devdoc>
        Subtract       = 0x6D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Decimal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Decimal key.
        ///    </para>
        /// </devdoc>
        Decimal        = 0x6E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Divide"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Divide key.
        ///    </para>
        /// </devdoc>
        Divide         = 0x6F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F1 key.
        ///    </para>
        /// </devdoc>
        F1             = 0x70,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F2 key.
        ///    </para>
        /// </devdoc>
        F2             = 0x71,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F3 key.
        ///    </para>
        /// </devdoc>
        F3             = 0x72,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F4 key.
        ///    </para>
        /// </devdoc>
        F4             = 0x73,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F5 key.
        ///    </para>
        /// </devdoc>
        F5             = 0x74,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F6 key.
        ///    </para>
        /// </devdoc>
        F6             = 0x75,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F7 key.
        ///    </para>
        /// </devdoc>
        F7             = 0x76,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F8 key.
        ///    </para>
        /// </devdoc>
        F8             = 0x77,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F9 key.
        ///    </para>
        /// </devdoc>
        F9             = 0x78,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F10 key.
        ///    </para>
        /// </devdoc>
        F10            = 0x79,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F11 key.
        ///    </para>
        /// </devdoc>
        F11            = 0x7A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F12 key.
        ///    </para>
        /// </devdoc>
        F12            = 0x7B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F13"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F13 key.
        ///    </para>
        /// </devdoc>
        F13            = 0x7C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F14"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F14 key.
        ///    </para>
        /// </devdoc>
        F14            = 0x7D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F15"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F15 key.
        ///    </para>
        /// </devdoc>
        F15            = 0x7E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F16"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F16 key.
        ///    </para>
        /// </devdoc>
        F16            = 0x7F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F17"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F17 key.
        ///    </para>
        /// </devdoc>
        F17            = 0x80,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F18"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F18 key.
        ///    </para>
        /// </devdoc>
        F18            = 0x81,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F19"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F19 key.
        ///    </para>
        /// </devdoc>
        F19            = 0x82,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F20"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F20 key.
        ///    </para>
        /// </devdoc>
        F20            = 0x83,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F21"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F21 key.
        ///    </para>
        /// </devdoc>
        F21            = 0x84,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F22"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F22 key.
        ///    </para>
        /// </devdoc>
        F22            = 0x85,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F23"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F23 key.
        ///    </para>
        /// </devdoc>
        F23            = 0x86,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F24"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The F24 key.
        ///    </para>
        /// </devdoc>
        F24            = 0x87,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumLock"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The NUM LOCK key.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumLock        = 0x90,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Scroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The SCROLL LOCK key.
        ///    </para>
        /// </devdoc>
        Scroll         = 0x91,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LShiftKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The left SHIFT key.
        ///    </para>
        /// </devdoc>
        LShiftKey     = 0xA0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RShiftKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The right SHIFT key.
        ///    </para>
        /// </devdoc>
        RShiftKey     = 0xA1,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LControlKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The left CTRL key.
        ///    </para>
        /// </devdoc>
        LControlKey   = 0xA2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RControlKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The right CTRL key.
        ///    </para>
        /// </devdoc>
        RControlKey   = 0xA3,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The left ALT key.
        ///    </para>
        /// </devdoc>
        LMenu          = 0xA4,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The right ALT key.
        ///    </para>
        /// </devdoc>
        RMenu          = 0xA5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserBack"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Back key.
        ///    </para>
        /// </devdoc>
        BrowserBack   = 0xA6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserForward"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Forward key.
        ///    </para>
        /// </devdoc>
        BrowserForward= 0xA7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserRefresh"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Refresh key.
        ///    </para>
        /// </devdoc>
        BrowserRefresh= 0xA8,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserStop"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Stop key.
        ///    </para>
        /// </devdoc>
        BrowserStop   = 0xA9,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserSearch"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Search key.
        ///    </para>
        /// </devdoc>
        BrowserSearch = 0xAA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserFavorites"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Favorites key.
        ///    </para>
        /// </devdoc>
        BrowserFavorites = 0xAB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserHome"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Browser Home key.
        ///    </para>
        /// </devdoc>
        BrowserHome   = 0xAC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeMute"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Volume Mute key.
        ///    </para>
        /// </devdoc>
        VolumeMute    = 0xAD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Volume Down key.
        ///    </para>
        /// </devdoc>
        VolumeDown    = 0xAE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Volume Up key.
        ///    </para>
        /// </devdoc>
        VolumeUp      = 0xAF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaNextTrack"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Media Next Track key.
        ///    </para>
        /// </devdoc>
        MediaNextTrack = 0xB0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaPreviousTrack"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Media Previous Track key.
        ///    </para>
        /// </devdoc>
        MediaPreviousTrack = 0xB1,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaStop"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Media Stop key.
        ///    </para>
        /// </devdoc>
        MediaStop     = 0xB2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaPlayPause"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Media Play Pause key.
        ///    </para>
        /// </devdoc>
        MediaPlayPause = 0xB3,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchMail"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Launch Mail key.
        ///    </para>
        /// </devdoc>
        LaunchMail    = 0xB4,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.SelectMedia"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Select Media key.
        ///    </para>
        /// </devdoc>
        SelectMedia   = 0xB5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchApplication1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Launch Application1 key.
        ///    </para>
        /// </devdoc>
        LaunchApplication1 = 0xB6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchApplication2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Launch Application2 key.
        ///    </para>
        /// </devdoc>
        LaunchApplication2 = 0xB7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemSemicolon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Semicolon key.
        ///    </para>
        /// </devdoc>
        OemSemicolon  = 0xBA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 1 key.
        ///    </para>
        /// </devdoc>
        Oem1 = OemSemicolon,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemplus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem plus key.
        ///    </para>
        /// </devdoc>
        Oemplus       = 0xBB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemcomma"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem comma key.
        ///    </para>
        /// </devdoc>
        Oemcomma      = 0xBC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemMinus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Minus key.
        ///    </para>
        /// </devdoc>
        OemMinus      = 0xBD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemPeriod"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Period key.
        ///    </para>
        /// </devdoc>
        OemPeriod     = 0xBE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemQuestion"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Question key.
        ///    </para>
        /// </devdoc>
        OemQuestion   = 0xBF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 2 key.
        ///    </para>
        /// </devdoc>
        Oem2 = OemQuestion,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemtilde"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem tilde key.
        ///    </para>
        /// </devdoc>
        Oemtilde      = 0xC0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 3 key.
        ///    </para>
        /// </devdoc>
        Oem3 = Oemtilde,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemOpenBrackets"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Open Brackets key.
        ///    </para>
        /// </devdoc>
        OemOpenBrackets = 0xDB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 4 key.
        ///    </para>
        /// </devdoc>
        Oem4 = OemOpenBrackets,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemPipe"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Pipe key.
        ///    </para>
        /// </devdoc>
        OemPipe       = 0xDC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 5 key.
        ///    </para>
        /// </devdoc>
        Oem5 = OemPipe,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemCloseBrackets"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Close Brackets key.
        ///    </para>
        /// </devdoc>
        OemCloseBrackets = 0xDD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 6 key.
        ///    </para>
        /// </devdoc>
        Oem6 = OemCloseBrackets,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemQuotes"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Quotes key.
        ///    </para>
        /// </devdoc>
        OemQuotes     = 0xDE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 7 key.
        ///    </para>
        /// </devdoc>
        Oem7 = OemQuotes,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem8 key.
        ///    </para>
        /// </devdoc>
        Oem8          = 0xDF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemBackslash"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem Backslash key.
        ///    </para>
        /// </devdoc>
        OemBackslash  = 0xE2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem102"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Oem 102 key.
        ///    </para>
        /// </devdoc>
        Oem102 = OemBackslash,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.ProcessKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PROCESS KEY key.
        ///    </para>
        /// </devdoc>
        ProcessKey     = 0xE5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Packet"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Packet KEY key.
        ///    </para>
        /// </devdoc>
        Packet     = 0xE7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Attn"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ATTN key.
        ///    </para>
        /// </devdoc>
        Attn           = 0xF6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Crsel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CRSEL key.
        ///    </para>
        /// </devdoc>
        Crsel          = 0xF7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Exsel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The EXSEL key.
        ///    </para>
        /// </devdoc>
        Exsel          = 0xF8,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.EraseEof"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ERASE EOF key.
        ///    </para>
        /// </devdoc>
        EraseEof          = 0xF9,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Play"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PLAY key.
        ///    </para>
        /// </devdoc>
        Play           = 0xFA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Zoom"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ZOOM key.
        ///    </para>
        /// </devdoc>
        Zoom           = 0xFB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NoName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A constant reserved for future use.
        ///    </para>
        /// </devdoc>
        NoName         = 0xFC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Pa1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The PA1 key.
        ///    </para>
        /// </devdoc>
        Pa1            = 0xFD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemClear"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        OemClear      = 0xFE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Shift"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The SHIFT modifier key.
        ///    </para>
        /// </devdoc>
        Shift   = 0x00010000,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Control"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       CTRL modifier key.
        ///
        ///    </para>
        /// </devdoc>
        Control = 0x00020000,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Alt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The ALT modifier key.
        ///
        ///    </para>
        /// </devdoc>
        Alt     = 0x00040000,
    }
}
