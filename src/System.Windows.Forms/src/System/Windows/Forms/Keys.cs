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
        /// <devdoc>
        ///    <para>
        ///       The bit mask to extract a key code from a key value.
        ///       
        ///    </para>
        /// </devdoc>
        KeyCode = 0x0000FFFF,

        /// <devdoc>
        ///    <para>
        ///       The bit mask to extract modifiers from a key value.
        ///       
        ///    </para>
        /// </devdoc>
        Modifiers = unchecked((int)0xFFFF0000),

        /// <devdoc>
        ///    <para>
        ///       No key pressed.
        ///    </para>
        /// </devdoc>
        None           = 0x00,

        /// <devdoc>
        ///    <para>
        ///       The left mouse button.
        ///       
        ///    </para>
        /// </devdoc>
        LButton        = 0x01,
        /// <devdoc>
        ///    <para>
        ///       The right mouse button.
        ///    </para>
        /// </devdoc>
        RButton        = 0x02,
        /// <devdoc>
        ///    <para>
        ///       The CANCEL key.
        ///    </para>
        /// </devdoc>
        Cancel         = 0x03,
        /// <devdoc>
        ///    <para>
        ///       The middle mouse button (three-button mouse).
        ///    </para>
        /// </devdoc>
        MButton        = 0x04,
        /// <devdoc>
        ///    <para>
        ///       The first x mouse button (five-button mouse).
        ///    </para>
        /// </devdoc>
        XButton1       = 0x05,
        /// <devdoc>
        ///    <para>
        ///       The second x mouse button (five-button mouse).
        ///    </para>
        /// </devdoc>
        XButton2       = 0x06,
        /// <devdoc>
        ///    <para>
        ///       The BACKSPACE key.
        ///    </para>
        /// </devdoc>
        Back           = 0x08,
        /// <devdoc>
        ///    <para>
        ///       The TAB key.
        ///    </para>
        /// </devdoc>
        Tab            = 0x09,
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        LineFeed       = 0x0A,
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        Clear          = 0x0C,
        /// <devdoc>
        ///    <para>
        ///       The RETURN key.
        ///
        ///    </para>
        /// </devdoc>
        Return         = 0x0D,
        /// <devdoc>
        ///    <para>
        ///       The ENTER key.
        ///       
        ///    </para>
        /// </devdoc>
        Enter          = Return,
        /// <devdoc>
        ///    <para>
        ///       The SHIFT key.
        ///    </para>
        /// </devdoc>
        ShiftKey      = 0x10,
        /// <devdoc>
        ///    <para>
        ///       The CTRL key.
        ///    </para>
        /// </devdoc>
        ControlKey    = 0x11,
        /// <devdoc>
        ///    <para>
        ///       The ALT key.
        ///    </para>
        /// </devdoc>
        Menu           = 0x12,
        /// <devdoc>
        ///    <para>
        ///       The PAUSE key.
        ///    </para>
        /// </devdoc>
        Pause          = 0x13,
        /// <devdoc>
        ///    <para>
        ///       The CAPS LOCK key.
        ///
        ///    </para>
        /// </devdoc>
        Capital        = 0x14,
        /// <devdoc>
        ///    <para>
        ///       The CAPS LOCK key.
        ///    </para>
        /// </devdoc>
        CapsLock       = 0x14,
        /// <devdoc>
        ///    <para>
        ///       The IME Kana mode key.
        ///    </para>
        /// </devdoc>
        KanaMode      = 0x15,
        /// <devdoc>
        ///    <para>
        ///       The IME Hanguel mode key.
        ///    </para>
        /// </devdoc>
        HanguelMode   = 0x15,
        /// <devdoc>
        ///    <para>
        ///       The IME Hangul mode key.
        ///    </para>
        /// </devdoc>
        HangulMode    = 0x15,
        /// <devdoc>
        ///    <para>
        ///       The IME Junja mode key.
        ///    </para>
        /// </devdoc>
        JunjaMode     = 0x17,
        /// <devdoc>
        ///    <para>
        ///       The IME Final mode key.
        ///    </para>
        /// </devdoc>
        FinalMode     = 0x18,
        /// <devdoc>
        ///    <para>
        ///       The IME Hanja mode key.
        ///    </para>
        /// </devdoc>
        HanjaMode     = 0x19,
        /// <devdoc>
        ///    <para>
        ///       The IME Kanji mode key.
        ///    </para>
        /// </devdoc>
        KanjiMode     = 0x19,
        /// <devdoc>
        ///    <para>
        ///       The ESC key.
        ///    </para>
        /// </devdoc>
        Escape         = 0x1B,
        /// <devdoc>
        ///    <para>
        ///       The IME Convert key.
        ///    </para>
        /// </devdoc>
        IMEConvert    = 0x1C,
        /// <devdoc>
        ///    <para>
        ///       The IME NonConvert key.
        ///    </para>
        /// </devdoc>
        IMENonconvert = 0x1D,
	/// <devdoc>
        ///    <para>
        ///       The IME Accept key.
        ///    </para>
        /// </devdoc>
        IMEAccept     = 0x1E,
	/// <devdoc>
        ///    <para>
        ///       The IME Accept key.
        ///    </para>
        /// </devdoc>
        IMEAceept     = IMEAccept,
        /// <devdoc>
        ///    <para>
        ///       The IME Mode change request.
        ///    </para>
        /// </devdoc>
        IMEModeChange = 0x1F,
        /// <devdoc>
        ///    <para>
        ///       The SPACEBAR key.
        ///    </para>
        /// </devdoc>
        Space          = 0x20,
        /// <devdoc>
        ///    <para>
        ///       The PAGE UP key.
        ///    </para>
        /// </devdoc>
        Prior          = 0x21,
        /// <devdoc>
        ///    <para>
        ///       The PAGE UP key.
        ///    </para>
        /// </devdoc>
        PageUp         = Prior,
        /// <devdoc>
        ///    <para>
        ///       The PAGE DOWN key.
        ///    </para>
        /// </devdoc>
        Next           = 0x22,
        /// <devdoc>
        ///    <para>
        ///       The PAGE DOWN key.
        ///    </para>
        /// </devdoc>
        PageDown       = Next,
        /// <devdoc>
        ///    <para>
        ///       The END key.
        ///    </para>
        /// </devdoc>
        End            = 0x23,
        /// <devdoc>
        ///    <para>
        ///       The HOME key.
        ///    </para>
        /// </devdoc>
        Home           = 0x24,
        /// <devdoc>
        ///    <para>
        ///       The LEFT ARROW key.
        ///    </para>
        /// </devdoc>
        Left           = 0x25,
        /// <devdoc>
        ///    <para>
        ///       The UP ARROW key.
        ///    </para>
        /// </devdoc>
        Up             = 0x26,
        /// <devdoc>
        ///    <para>
        ///       The RIGHT ARROW key.
        ///    </para>
        /// </devdoc>
        Right          = 0x27,
        /// <devdoc>
        ///    <para>
        ///       The DOWN ARROW key.
        ///    </para>
        /// </devdoc>
        Down           = 0x28,
        /// <devdoc>
        ///    <para>
        ///       The SELECT key.
        ///    </para>
        /// </devdoc>
        Select         = 0x29,
        /// <devdoc>
        ///    <para>
        ///       The PRINT key.
        ///    </para>
        /// </devdoc>
        Print          = 0x2A,
        /// <devdoc>
        ///    <para>
        ///       The EXECUTE key.
        ///    </para>
        /// </devdoc>
        Execute        = 0x2B,
        /// <devdoc>
        ///    <para>
        ///       The PRINT SCREEN key.
        ///
        ///    </para>
        /// </devdoc>
        Snapshot       = 0x2C,
        /// <devdoc>
        ///    <para>
        ///       The PRINT SCREEN key.
        ///    </para>
        /// </devdoc>
        PrintScreen    = Snapshot,
        /// <devdoc>
        ///    <para>
        ///       The INS key.
        ///    </para>
        /// </devdoc>
        Insert         = 0x2D,
        /// <devdoc>
        ///    <para>
        ///       The DEL key.
        ///    </para>
        /// </devdoc>
        Delete         = 0x2E,
        /// <devdoc>
        ///    <para>
        ///       The HELP key.
        ///    </para>
        /// </devdoc>
        Help           = 0x2F,
        /// <devdoc>
        ///    <para>
        ///       The 0 key.
        ///    </para>
        /// </devdoc>
        D0             = 0x30, // 0
        /// <devdoc>
        ///    <para>
        ///       The 1 key.
        ///    </para>
        /// </devdoc>
        D1             = 0x31, // 1
        /// <devdoc>
        ///    <para>
        ///       The 2 key.
        ///    </para>
        /// </devdoc>
        D2             = 0x32, // 2
        /// <devdoc>
        ///    <para>
        ///       The 3 key.
        ///    </para>
        /// </devdoc>
        D3             = 0x33, // 3
        /// <devdoc>
        ///    <para>
        ///       The 4 key.
        ///    </para>
        /// </devdoc>
        D4             = 0x34, // 4
        /// <devdoc>
        ///    <para>
        ///       The 5 key.
        ///    </para>
        /// </devdoc>
        D5             = 0x35, // 5
        /// <devdoc>
        ///    <para>
        ///       The 6 key.
        ///    </para>
        /// </devdoc>
        D6             = 0x36, // 6
        /// <devdoc>
        ///    <para>
        ///       The 7 key.
        ///    </para>
        /// </devdoc>
        D7             = 0x37, // 7
        /// <devdoc>
        ///    <para>
        ///       The 8 key.
        ///    </para>
        /// </devdoc>
        D8             = 0x38, // 8
        /// <devdoc>
        ///    <para>
        ///       The 9 key.
        ///    </para>
        /// </devdoc>
        D9             = 0x39, // 9
        /// <devdoc>
        ///    <para>
        ///       The A key.
        ///    </para>
        /// </devdoc>
        A              = 0x41,
        /// <devdoc>
        ///    <para>
        ///       The B key.
        ///    </para>
        /// </devdoc>
        B              = 0x42,
        /// <devdoc>
        ///    <para>
        ///       The C key.
        ///    </para>
        /// </devdoc>
        C              = 0x43,
        /// <devdoc>
        ///    <para>
        ///       The D key.
        ///    </para>
        /// </devdoc>
        D              = 0x44,
        /// <devdoc>
        ///    <para>
        ///       The E key.
        ///    </para>
        /// </devdoc>
        E              = 0x45,
        /// <devdoc>
        ///    <para>
        ///       The F key.
        ///    </para>
        /// </devdoc>
        F              = 0x46,
        /// <devdoc>
        ///    <para>
        ///       The G key.
        ///    </para>
        /// </devdoc>
        G              = 0x47,
        /// <devdoc>
        ///    <para>
        ///       The H key.
        ///    </para>
        /// </devdoc>
        H              = 0x48,
        /// <devdoc>
        ///    <para>
        ///       The I key.
        ///    </para>
        /// </devdoc>
        I              = 0x49,
        /// <devdoc>
        ///    <para>
        ///       The J key.
        ///    </para>
        /// </devdoc>
        J              = 0x4A,
        /// <devdoc>
        ///    <para>
        ///       The K key.
        ///    </para>
        /// </devdoc>
        K              = 0x4B,
        /// <devdoc>
        ///    <para>
        ///       The L key.
        ///    </para>
        /// </devdoc>
        L              = 0x4C,
        /// <devdoc>
        ///    <para>
        ///       The M key.
        ///    </para>
        /// </devdoc>
        M              = 0x4D,
        /// <devdoc>
        ///    <para>
        ///       The N key.
        ///    </para>
        /// </devdoc>
        N              = 0x4E,
        /// <devdoc>
        ///    <para>
        ///       The O key.
        ///    </para>
        /// </devdoc>
        O              = 0x4F,
        /// <devdoc>
        ///    <para>
        ///       The P key.
        ///    </para>
        /// </devdoc>
        P              = 0x50,
        /// <devdoc>
        ///    <para>
        ///       The Q key.
        ///    </para>
        /// </devdoc>
        Q              = 0x51,
        /// <devdoc>
        ///    <para>
        ///       The R key.
        ///    </para>
        /// </devdoc>
        R              = 0x52,
        /// <devdoc>
        ///    <para>
        ///       The S key.
        ///    </para>
        /// </devdoc>
        S              = 0x53,
        /// <devdoc>
        ///    <para>
        ///       The T key.
        ///    </para>
        /// </devdoc>
        T              = 0x54,
        /// <devdoc>
        ///    <para>
        ///       The U key.
        ///    </para>
        /// </devdoc>
        U              = 0x55,
        /// <devdoc>
        ///    <para>
        ///       The V key.
        ///    </para>
        /// </devdoc>
        V              = 0x56,
        /// <devdoc>
        ///    <para>
        ///       The W key.
        ///    </para>
        /// </devdoc>
        W              = 0x57,
        /// <devdoc>
        ///    <para>
        ///       The X key.
        ///    </para>
        /// </devdoc>
        X              = 0x58,
        /// <devdoc>
        ///    <para>
        ///       The Y key.
        ///    </para>
        /// </devdoc>
        Y              = 0x59,
        /// <devdoc>
        ///    <para>
        ///       The Z key.
        ///    </para>
        /// </devdoc>
        Z              = 0x5A,
        /// <devdoc>
        ///    <para>
        ///       The left Windows logo key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        LWin           = 0x5B,
        /// <devdoc>
        ///    <para>
        ///       The right Windows logo key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        RWin           = 0x5C,
        /// <devdoc>
        ///    <para>
        ///       The Application key (Microsoft Natural Keyboard).
        ///    </para>
        /// </devdoc>
        Apps           = 0x5D,
        /// <devdoc>
        ///    <para>
        ///       The Computer Sleep key.
        ///    </para>
        /// </devdoc>
        Sleep          = 0x5F,
        /// <devdoc>
        ///    <para>
        ///       The 0 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad0        = 0x60,
        /// <devdoc>
        ///    <para>
        ///       The 1 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad1        = 0x61,
        /// <devdoc>
        ///    <para>
        ///       The 2 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad2        = 0x62,
        /// <devdoc>
        ///    <para>
        ///       The 3 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad3        = 0x63,
        /// <devdoc>
        ///    <para>
        ///       The 4 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad4        = 0x64,
        /// <devdoc>
        ///    <para>
        ///       The 5 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad5        = 0x65,
        /// <devdoc>
        ///    <para>
        ///       The 6 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad6        = 0x66,
        /// <devdoc>
        ///    <para>
        ///       The 7 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad7        = 0x67,
        /// <devdoc>
        ///    <para>
        ///       The 8 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad8        = 0x68,
        /// <devdoc>
        ///    <para>
        ///       The 9 key on the numeric keypad.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumPad9        = 0x69,
        /// <devdoc>
        ///    <para>
        ///       The Multiply key.
        ///    </para>
        /// </devdoc>
        Multiply       = 0x6A,
        /// <devdoc>
        ///    <para>
        ///       The Add key.
        ///    </para>
        /// </devdoc>
        Add            = 0x6B,
        /// <devdoc>
        ///    <para>
        ///       The Separator key.
        ///    </para>
        /// </devdoc>
        Separator      = 0x6C,
        /// <devdoc>
        ///    <para>
        ///       The Subtract key.
        ///    </para>
        /// </devdoc>
        Subtract       = 0x6D,
        /// <devdoc>
        ///    <para>
        ///       The Decimal key.
        ///    </para>
        /// </devdoc>
        Decimal        = 0x6E,
        /// <devdoc>
        ///    <para>
        ///       The Divide key.
        ///    </para>
        /// </devdoc>
        Divide         = 0x6F,
        /// <devdoc>
        ///    <para>
        ///       The F1 key.
        ///    </para>
        /// </devdoc>
        F1             = 0x70,
        /// <devdoc>
        ///    <para>
        ///       The F2 key.
        ///    </para>
        /// </devdoc>
        F2             = 0x71,
        /// <devdoc>
        ///    <para>
        ///       The F3 key.
        ///    </para>
        /// </devdoc>
        F3             = 0x72,
        /// <devdoc>
        ///    <para>
        ///       The F4 key.
        ///    </para>
        /// </devdoc>
        F4             = 0x73,
        /// <devdoc>
        ///    <para>
        ///       The F5 key.
        ///    </para>
        /// </devdoc>
        F5             = 0x74,
        /// <devdoc>
        ///    <para>
        ///       The F6 key.
        ///    </para>
        /// </devdoc>
        F6             = 0x75,
        /// <devdoc>
        ///    <para>
        ///       The F7 key.
        ///    </para>
        /// </devdoc>
        F7             = 0x76,
        /// <devdoc>
        ///    <para>
        ///       The F8 key.
        ///    </para>
        /// </devdoc>
        F8             = 0x77,
        /// <devdoc>
        ///    <para>
        ///       The F9 key.
        ///    </para>
        /// </devdoc>
        F9             = 0x78,
        /// <devdoc>
        ///    <para>
        ///       The F10 key.
        ///    </para>
        /// </devdoc>
        F10            = 0x79,
        /// <devdoc>
        ///    <para>
        ///       The F11 key.
        ///    </para>
        /// </devdoc>
        F11            = 0x7A,
        /// <devdoc>
        ///    <para>
        ///       The F12 key.
        ///    </para>
        /// </devdoc>
        F12            = 0x7B,
        /// <devdoc>
        ///    <para>
        ///       The F13 key.
        ///    </para>
        /// </devdoc>
        F13            = 0x7C,
        /// <devdoc>
        ///    <para>
        ///       The F14 key.
        ///    </para>
        /// </devdoc>
        F14            = 0x7D,
        /// <devdoc>
        ///    <para>
        ///       The F15 key.
        ///    </para>
        /// </devdoc>
        F15            = 0x7E,
        /// <devdoc>
        ///    <para>
        ///       The F16 key.
        ///    </para>
        /// </devdoc>
        F16            = 0x7F,
        /// <devdoc>
        ///    <para>
        ///       The F17 key.
        ///    </para>
        /// </devdoc>
        F17            = 0x80,
        /// <devdoc>
        ///    <para>
        ///       The F18 key.
        ///    </para>
        /// </devdoc>
        F18            = 0x81,
        /// <devdoc>
        ///    <para>
        ///       The F19 key.
        ///    </para>
        /// </devdoc>
        F19            = 0x82,
        /// <devdoc>
        ///    <para>
        ///       The F20 key.
        ///    </para>
        /// </devdoc>
        F20            = 0x83,
        /// <devdoc>
        ///    <para>
        ///       The F21 key.
        ///    </para>
        /// </devdoc>
        F21            = 0x84,
        /// <devdoc>
        ///    <para>
        ///       The F22 key.
        ///    </para>
        /// </devdoc>
        F22            = 0x85,
        /// <devdoc>
        ///    <para>
        ///       The F23 key.
        ///    </para>
        /// </devdoc>
        F23            = 0x86,
        /// <devdoc>
        ///    <para>
        ///       The F24 key.
        ///    </para>
        /// </devdoc>
        F24            = 0x87,
        /// <devdoc>
        ///    <para>
        ///       The NUM LOCK key.
        ///    </para>
        /// </devdoc>
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        NumLock        = 0x90,
        /// <devdoc>
        ///    <para>
        ///       The SCROLL LOCK key.
        ///    </para>
        /// </devdoc>
        Scroll         = 0x91,
        /// <devdoc>
        ///    <para>
        ///       The left SHIFT key.
        ///    </para>
        /// </devdoc>
        LShiftKey     = 0xA0,
        /// <devdoc>
        ///    <para>
        ///       The right SHIFT key.
        ///    </para>
        /// </devdoc>
        RShiftKey     = 0xA1,
        /// <devdoc>
        ///    <para>
        ///       The left CTRL key.
        ///    </para>
        /// </devdoc>
        LControlKey   = 0xA2,
        /// <devdoc>
        ///    <para>
        ///       The right CTRL key.
        ///    </para>
        /// </devdoc>
        RControlKey   = 0xA3,
        /// <devdoc>
        ///    <para>
        ///       The left ALT key.
        ///    </para>
        /// </devdoc>
        LMenu          = 0xA4,
        /// <devdoc>
        ///    <para>
        ///       The right ALT key.
        ///    </para>
        /// </devdoc>
        RMenu          = 0xA5,
        /// <devdoc>
        ///    <para>
        ///       The Browser Back key.
        ///    </para>
        /// </devdoc>
        BrowserBack   = 0xA6,
        /// <devdoc>
        ///    <para>
        ///       The Browser Forward key.
        ///    </para>
        /// </devdoc>
        BrowserForward= 0xA7,
        /// <devdoc>
        ///    <para>
        ///       The Browser Refresh key.
        ///    </para>
        /// </devdoc>
        BrowserRefresh= 0xA8,
        /// <devdoc>
        ///    <para>
        ///       The Browser Stop key.
        ///    </para>
        /// </devdoc>
        BrowserStop   = 0xA9,
        /// <devdoc>
        ///    <para>
        ///       The Browser Search key.
        ///    </para>
        /// </devdoc>
        BrowserSearch = 0xAA,
        /// <devdoc>
        ///    <para>
        ///       The Browser Favorites key.
        ///    </para>
        /// </devdoc>
        BrowserFavorites = 0xAB,
        /// <devdoc>
        ///    <para>
        ///       The Browser Home key.
        ///    </para>
        /// </devdoc>
        BrowserHome   = 0xAC,
        /// <devdoc>
        ///    <para>
        ///       The Volume Mute key.
        ///    </para>
        /// </devdoc>
        VolumeMute    = 0xAD,
        /// <devdoc>
        ///    <para>
        ///       The Volume Down key.
        ///    </para>
        /// </devdoc>
        VolumeDown    = 0xAE,
        /// <devdoc>
        ///    <para>
        ///       The Volume Up key.
        ///    </para>
        /// </devdoc>
        VolumeUp      = 0xAF,
        /// <devdoc>
        ///    <para>
        ///       The Media Next Track key.
        ///    </para>
        /// </devdoc>
        MediaNextTrack = 0xB0,
        /// <devdoc>
        ///    <para>
        ///       The Media Previous Track key.
        ///    </para>
        /// </devdoc>
        MediaPreviousTrack = 0xB1,
        /// <devdoc>
        ///    <para>
        ///       The Media Stop key.
        ///    </para>
        /// </devdoc>
        MediaStop     = 0xB2,
        /// <devdoc>
        ///    <para>
        ///       The Media Play Pause key.
        ///    </para>
        /// </devdoc>
        MediaPlayPause = 0xB3,
        /// <devdoc>
        ///    <para>
        ///       The Launch Mail key.
        ///    </para>
        /// </devdoc>
        LaunchMail    = 0xB4,
        /// <devdoc>
        ///    <para>
        ///       The Select Media key.
        ///    </para>
        /// </devdoc>
        SelectMedia   = 0xB5,
        /// <devdoc>
        ///    <para>
        ///       The Launch Application1 key.
        ///    </para>
        /// </devdoc>
        LaunchApplication1 = 0xB6,
        /// <devdoc>
        ///    <para>
        ///       The Launch Application2 key.
        ///    </para>
        /// </devdoc>
        LaunchApplication2 = 0xB7,
        /// <devdoc>
        ///    <para>
        ///       The Oem Semicolon key.
        ///    </para>
        /// </devdoc>
        OemSemicolon  = 0xBA,
        /// <devdoc>
        ///    <para>
        ///       The Oem 1 key.
        ///    </para>
        /// </devdoc>
        Oem1 = OemSemicolon,
        /// <devdoc>
        ///    <para>
        ///       The Oem plus key.
        ///    </para>
        /// </devdoc>
        Oemplus       = 0xBB,
        /// <devdoc>
        ///    <para>
        ///       The Oem comma key.
        ///    </para>
        /// </devdoc>
        Oemcomma      = 0xBC,
        /// <devdoc>
        ///    <para>
        ///       The Oem Minus key.
        ///    </para>
        /// </devdoc>
        OemMinus      = 0xBD,
        /// <devdoc>
        ///    <para>
        ///       The Oem Period key.
        ///    </para>
        /// </devdoc>
        OemPeriod     = 0xBE,
        /// <devdoc>
        ///    <para>
        ///       The Oem Question key.
        ///    </para>
        /// </devdoc>
        OemQuestion   = 0xBF,
        /// <devdoc>
        ///    <para>
        ///       The Oem 2 key.
        ///    </para>
        /// </devdoc>
        Oem2 = OemQuestion,
        /// <devdoc>
        ///    <para>
        ///       The Oem tilde key.
        ///    </para>
        /// </devdoc>
        Oemtilde      = 0xC0,
        /// <devdoc>
        ///    <para>
        ///       The Oem 3 key.
        ///    </para>
        /// </devdoc>
        Oem3 = Oemtilde,
        /// <devdoc>
        ///    <para>
        ///       The Oem Open Brackets key.
        ///    </para>
        /// </devdoc>
        OemOpenBrackets = 0xDB,
        /// <devdoc>
        ///    <para>
        ///       The Oem 4 key.
        ///    </para>
        /// </devdoc>
        Oem4 = OemOpenBrackets,
        /// <devdoc>
        ///    <para>
        ///       The Oem Pipe key.
        ///    </para>
        /// </devdoc>
        OemPipe       = 0xDC,
        /// <devdoc>
        ///    <para>
        ///       The Oem 5 key.
        ///    </para>
        /// </devdoc>
        Oem5 = OemPipe,
        /// <devdoc>
        ///    <para>
        ///       The Oem Close Brackets key.
        ///    </para>
        /// </devdoc>
        OemCloseBrackets = 0xDD,
        /// <devdoc>
        ///    <para>
        ///       The Oem 6 key.
        ///    </para>
        /// </devdoc>
        Oem6 = OemCloseBrackets,
        /// <devdoc>
        ///    <para>
        ///       The Oem Quotes key.
        ///    </para>
        /// </devdoc>
        OemQuotes     = 0xDE,
        /// <devdoc>
        ///    <para>
        ///       The Oem 7 key.
        ///    </para>
        /// </devdoc>
        Oem7 = OemQuotes,
        /// <devdoc>
        ///    <para>
        ///       The Oem8 key.
        ///    </para>
        /// </devdoc>
        Oem8          = 0xDF,
        /// <devdoc>
        ///    <para>
        ///       The Oem Backslash key.
        ///    </para>
        /// </devdoc>
        OemBackslash  = 0xE2,
        /// <devdoc>
        ///    <para>
        ///       The Oem 102 key.
        ///    </para>
        /// </devdoc>
        Oem102 = OemBackslash,
        /// <devdoc>
        ///    <para>
        ///       The PROCESS KEY key.
        ///    </para>
        /// </devdoc>
        ProcessKey     = 0xE5,
        /// <devdoc>
        ///    <para>
        ///       The Packet KEY key.
        ///    </para>
        /// </devdoc>
        Packet     = 0xE7,
        /// <devdoc>
        ///    <para>
        ///       The ATTN key.
        ///    </para>
        /// </devdoc>
        Attn           = 0xF6,
        /// <devdoc>
        ///    <para>
        ///       The CRSEL key.
        ///    </para>
        /// </devdoc>
        Crsel          = 0xF7,
        /// <devdoc>
        ///    <para>
        ///       The EXSEL key.
        ///    </para>
        /// </devdoc>
        Exsel          = 0xF8,
        /// <devdoc>
        ///    <para>
        ///       The ERASE EOF key.
        ///    </para>
        /// </devdoc>
        EraseEof          = 0xF9,
        /// <devdoc>
        ///    <para>
        ///       The PLAY key.
        ///    </para>
        /// </devdoc>
        Play           = 0xFA,
        /// <devdoc>
        ///    <para>
        ///       The ZOOM key.
        ///    </para>
        /// </devdoc>
        Zoom           = 0xFB,
        /// <devdoc>
        ///    <para>
        ///       A constant reserved for future use.
        ///    </para>
        /// </devdoc>
        NoName         = 0xFC,
        /// <devdoc>
        ///    <para>
        ///       The PA1 key.
        ///    </para>
        /// </devdoc>
        Pa1            = 0xFD,
        /// <devdoc>
        ///    <para>
        ///       The CLEAR key.
        ///    </para>
        /// </devdoc>
        OemClear      = 0xFE,
        /// <devdoc>
        ///    <para>
        ///       The SHIFT modifier key.
        ///    </para>
        /// </devdoc>
        Shift   = 0x00010000,
        /// <devdoc>
        ///    <para>
        ///       The
        ///       CTRL modifier key.
        ///
        ///    </para>
        /// </devdoc>
        Control = 0x00020000,
        /// <devdoc>
        ///    <para>
        ///       The ALT modifier key.
        ///
        ///    </para>
        /// </devdoc>
        Alt     = 0x00040000,
    }
}
