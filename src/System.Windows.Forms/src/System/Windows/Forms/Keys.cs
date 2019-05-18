// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies key codes and modifiers.
    /// </devdoc>
    [Flags]
    [TypeConverterAttribute(typeof(KeysConverter))]
    [Editor("System.Windows.Forms.Design.ShortcutKeysEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    [ComVisible(true)]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "Certain members of Keys enum are actually meant to be OR'ed.")]
    public enum Keys
    {
        /// <summary>
        /// The bit mask to extract a key code from a key value.
        /// </devdoc>
        KeyCode = 0x0000FFFF,

        /// <summary>
        /// The bit mask to extract modifiers from a key value.
        /// </devdoc>
        Modifiers = unchecked((int)0xFFFF0000),

        /// <summary>
        /// No key pressed.
        /// </devdoc>
        None = 0x00,

        /// <summary>
        /// The left mouse button.
        /// </devdoc>
        LButton = 0x01,

        /// <summary>
        /// The right mouse button.
        /// </devdoc>
        RButton = 0x02,

        /// <summary>
        /// The CANCEL key.
        /// </devdoc>
        Cancel = 0x03,

        /// <summary>
        /// The middle mouse button (three-button mouse).
        /// </devdoc>
        MButton = 0x04,

        /// <summary>
        /// The first x mouse button (five-button mouse).
        /// </devdoc>
        XButton1 = 0x05,

        /// <summary>
        /// The second x mouse button (five-button mouse).
        /// </devdoc>
        XButton2 = 0x06,

        /// <summary>
        ///    <para>
        /// The BACKSPACE key.
        ///    </para>
        /// </devdoc>
        Back = 0x08,

        /// <summary>
        /// The TAB key.
        /// </devdoc>
        Tab = 0x09,

        /// <summary>
        /// The CLEAR key.
        /// </devdoc>
        LineFeed = 0x0A,

        /// The CLEAR key.
        /// </devdoc>
        Clear = 0x0C,

        /// <summary>
        /// The RETURN key.
        /// </devdoc>
        Return = 0x0D,

        /// <summary>
        /// The ENTER key.
        /// </devdoc>
        Enter = Return,

        /// <summary>
        /// The SHIFT key.
        /// </devdoc>
        ShiftKey = 0x10,

        /// <summary>
        /// The CTRL key.
        /// </devdoc>
        ControlKey = 0x11,

        /// <summary>
        /// The ALT key.
        /// </devdoc>
        Menu = 0x12,

        /// <summary>
        /// The PAUSE key.
        /// </devdoc>
        Pause = 0x13,

        /// <summary>
        /// The CAPS LOCK key.
        /// </devdoc>
        Capital = 0x14,

        /// <summary>
        /// The CAPS LOCK key.
        /// </devdoc>
        CapsLock = 0x14,

        /// <summary>
        /// The IME Kana mode key.
        /// </devdoc>
        KanaMode = 0x15,

        /// <summary>
        /// The IME Hanguel mode key.
        /// </devdoc>
        HanguelMode = 0x15,

        /// <summary>
        /// The IME Hangul mode key.
        /// </devdoc>
        HangulMode = 0x15,

        /// <summary>
        /// The IME Junja mode key.
        /// </devdoc>
        JunjaMode = 0x17,

        /// <summary>
        /// The IME Final mode key.
        /// </devdoc>
        FinalMode = 0x18,

        /// <summary>
        /// The IME Hanja mode key.
        /// </devdoc>
        HanjaMode = 0x19,

        /// <summary>
        /// The IME Kanji mode key.
        /// </devdoc>
        KanjiMode = 0x19,

        /// <summary>
        /// The ESC key.
        /// </devdoc>
        Escape = 0x1B,

        /// <summary>
        /// The IME Convert key.
        /// </devdoc>
        IMEConvert = 0x1C,

        /// <summary>
        /// The IME NonConvert key.
        /// </devdoc>
        IMENonconvert = 0x1D,
	   
        /// <summary>
        /// The IME Accept key.
        /// </devdoc>
        IMEAccept = 0x1E,
	    
        /// <summary>
        /// The IME Accept key.
        /// </devdoc>
        IMEAceept = IMEAccept,
        
        /// <summary>
        /// The IME Mode change request.
        /// </devdoc>
        IMEModeChange = 0x1F,
        
        /// <summary>
        /// The SPACEBAR key.
        /// </devdoc>
        Space = 0x20,
        
        /// <summary>
        /// The PAGE UP key.
        /// </devdoc>
        Prior = 0x21,
        
        /// <summary>
        /// The PAGE UP key.
        /// </devdoc>
        PageUp = Prior,
        
        /// <summary>
        /// The PAGE DOWN key.
        /// </devdoc>
        Next = 0x22,
        
        /// <summary>
        /// The PAGE DOWN key.
        /// </devdoc>
        PageDown = Next,
        
        /// <summary>
        /// The END key.
        /// </devdoc>
        End = 0x23,
        
        /// <summary>
        /// The HOME key.
        /// </devdoc>
        Home = 0x24,
        
        /// <summary>
        /// The LEFT ARROW key.
        /// </devdoc>
        Left = 0x25,
        
        /// <summary>
        /// The UP ARROW key.
        /// </devdoc>
        Up = 0x26,
        
        /// <summary>
        /// The RIGHT ARROW key.
        /// </devdoc>
        Right = 0x27,
        
        /// <summary>
        /// The DOWN ARROW key.
        /// </devdoc>
        Down = 0x28,
        
        /// <summary>
        /// The SELECT key.
        /// </devdoc>
        Select = 0x29,
        
        /// <summary>
        /// The PRINT key.
        /// </devdoc>
        Print = 0x2A,
        
        /// <summary>
        /// The EXECUTE key.
        /// </devdoc>
        Execute = 0x2B,
        
        /// <summary>
        /// The PRINT SCREEN key.
        /// </devdoc>
        Snapshot = 0x2C,
        
        /// <summary>
        /// The PRINT SCREEN key.
        /// </devdoc>
        PrintScreen = Snapshot,
        
        /// <summary>
        /// The INS key.
        /// </devdoc>
        Insert = 0x2D,
        
        /// <summary>
        /// The DEL key.
        /// </devdoc>
        Delete = 0x2E,
        
        /// <summary>
        /// The HELP key.
        /// </devdoc>
        Help = 0x2F,
        
        /// <summary>
        /// The 0 key.
        /// </devdoc>
        D0 = 0x30, // 0
        
        /// <summary>
        /// The 1 key.
        /// </devdoc>
        D1 = 0x31, // 1
        
        /// <summary>
        /// The 2 key.
        /// </devdoc>
        D2 = 0x32, // 2
        
        /// <summary>
        /// The 3 key.
        /// </devdoc>
        D3 = 0x33, // 3
        
        /// <summary>
        /// The 4 key.
        /// </devdoc>
        D4 = 0x34, // 4
        
        /// <summary>
        /// The 5 key.
        /// </devdoc>
        D5 = 0x35, // 5
        
        /// <summary>
        /// The 6 key.
        /// </devdoc>
        D6 = 0x36, // 6
        
        /// <summary>
        /// The 7 key.
        /// </devdoc>
        D7 = 0x37, // 7
        
        /// <summary>
        /// The 8 key.
        /// </devdoc>
        D8 = 0x38, // 8
        
        /// <summary>
        /// The 9 key.
        /// </devdoc>
        D9 = 0x39, // 9
        
        /// <summary>
        /// The A key.
        /// </devdoc>
        A = 0x41,
        
        /// <summary>
        /// The B key.
        /// </devdoc>
        B = 0x42,
        
        /// <summary>
        /// The C key.
        /// </devdoc>
        C = 0x43,
        
        /// <summary>
        /// The D key.
        /// </devdoc>
        D = 0x44,
        
        /// <summary>
        /// The E key.
        /// </devdoc>
        E = 0x45,
        
        /// <summary>
        /// The F key.
        /// </devdoc>
        F = 0x46,
        
        /// <summary>
        /// The G key.
        /// </devdoc>
        G = 0x47,
        
        /// <summary>
        /// The H key.
        /// </devdoc>
        H = 0x48,
        
        /// <summary>
        /// The I key.
        /// </devdoc>
        I = 0x49,
        
        /// <summary>
        /// The J key.
        /// </devdoc>
        J = 0x4A,
        
        /// <summary>
        /// The K key.
        /// </devdoc>
        K = 0x4B,
        
        /// <summary>
        /// The L key.
        /// </devdoc>
        L = 0x4C,
        
        /// <summary>
        /// The M key.
        /// </devdoc>
        M = 0x4D,
        
        /// <summary>
        /// The N key.
        /// </devdoc>
        N = 0x4E,
        
        /// <summary>
        /// The O key.
        /// </devdoc>
        O = 0x4F,
        
        /// <summary>
        /// The P key.
        /// </devdoc>
        P = 0x50,
        
        /// <summary>
        /// The Q key.
        /// </devdoc>
        Q = 0x51,
        
        /// <summary>
        /// The R key.
        /// </devdoc>
        R = 0x52,
        
        /// <summary>
        /// The S key.
        /// </devdoc>
        S = 0x53,
        
        /// <summary>
        /// The T key.
        /// </devdoc>
        T = 0x54,
        
        /// <summary>
        /// The U key.
        /// </devdoc>
        U = 0x55,
        
        /// <summary>
        /// The V key.
        /// </devdoc>
        V = 0x56,
        
        /// <summary>
        /// The W key.
        /// </devdoc>
        W = 0x57,
        
        /// <summary>
        /// The X key.
        /// </devdoc>
        X = 0x58,
        
        /// <summary>
        /// The Y key.
        /// </devdoc>
        Y = 0x59,
        
        /// <summary>
        /// The Z key.
        /// </devdoc>
        Z = 0x5A,
        
        /// <summary>
        /// The left Windows logo key (Microsoft Natural Keyboard).
        /// </devdoc>
        LWin = 0x5B,
        
        /// <summary>
        /// The right Windows logo key (Microsoft Natural Keyboard).
        /// </devdoc>
        RWin = 0x5C,
        
        /// <summary>
        /// The Application key (Microsoft Natural Keyboard).
        /// </devdoc>
        
        Apps = 0x5D,
        
        /// <summary>
        /// The Computer Sleep key.
        /// </devdoc>
        Sleep = 0x5F,
        
        /// <summary>
        /// The 0 key on the numeric keypad.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumPad0 = 0x60,
       
        /// <summary>
        /// The 1 key on the numeric keypad.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumPad1 = 0x61,
        
        /// <summary>
        /// The 2 key on the numeric keypad.
        /// </devdoc>
        NumPad2 = 0x62,
        
        /// <summary>
        /// The 3 key on the numeric keypad.
        /// </devdoc>
        NumPad3 = 0x63,
        
        /// <summary>
        /// The 4 key on the numeric keypad.
        /// </devdoc>
        NumPad4 = 0x64,
        
        /// <summary>
        /// The 5 key on the numeric keypad.
        /// </devdoc>
        NumPad5 = 0x65,
        
        /// <summary>
        /// The 6 key on the numeric keypad.
        /// </devdoc>
        NumPad6 = 0x66,
        
        /// <summary>
        /// The 7 key on the numeric keypad.
        /// </devdoc>
        NumPad7 = 0x67,
        
        /// <summary>
        /// The 8 key on the numeric keypad.
        /// </devdoc>
        NumPad8 = 0x68,
        
        /// <summary>
        /// The 9 key on the numeric keypad.
        /// </devdoc>
        NumPad9 = 0x69,
        
        /// <summary>
        /// The Multiply key.
        /// </devdoc>
        Multiply = 0x6A,
        
        /// <summary>
        /// The Add key.
        /// </devdoc>
        Add = 0x6B,
        
        /// <summary>
        /// The Separator key.
        /// </devdoc>
        Separator = 0x6C,
        
        /// <summary>
        /// The Subtract key.
        /// </devdoc>
        Subtract = 0x6D,
        
        /// <summary>
        /// The Decimal key.
        /// </devdoc>
        Decimal = 0x6E,
        
        /// <summary>
        /// The Divide key.
        /// </devdoc>
        Divide = 0x6F,
        
        /// <summary>
        /// The F1 key.
        /// </devdoc>
        F1 = 0x70,
        
        /// <summary>
        /// The F2 key.
        /// </devdoc>
        F2 = 0x71,
        
        /// <summary>
        /// The F3 key.
        /// </devdoc>
        F3 = 0x72,
        
        /// <summary>
        /// The F4 key.
        /// </devdoc>
        F4 = 0x73,
        
        /// <summary>
        /// The F5 key.
        /// </devdoc>
        F5 = 0x74,
        
        /// <summary>
        /// The F6 key.
        /// </devdoc>
        F6 = 0x75,
        
        /// <summary>
        /// The F7 key.
        /// </devdoc>
        F7 = 0x76,
        
        /// <summary>
        /// The F8 key.
        /// </devdoc>
        F8 = 0x77,
        
        /// <summary>
        /// The F9 key.
        /// </devdoc>
        F9 = 0x78,
        
        /// <summary>
        /// The F10 key.
        /// </devdoc>
        F10 = 0x79,
        
        /// <summary>
        /// The F11 key.
        /// </devdoc>
        F11 = 0x7A,
        
        /// <summary>
        /// The F12 key.
        /// </devdoc>
        F12 = 0x7B,
        
        /// <summary>
        /// The F13 key.
        /// </devdoc>
        F13 = 0x7C,
        
        /// <summary>
        /// The F14 key.
        /// </devdoc>
        F14 = 0x7D,
        
        /// <summary>
        /// The F15 key.
        /// </devdoc>
        F15 = 0x7E,
        
        /// <summary>
        /// The F16 key.
        /// </devdoc>
        F16 = 0x7F,
        
        /// <summary>
        /// The F17 key.
        /// </devdoc>
        F17 = 0x80,
        
        /// <summary>
        /// The F18 key.
        /// </devdoc>
        F18 = 0x81,
        
        /// <summary>
        /// The F19 key.
        /// </devdoc>
        F19 = 0x82,
        
        /// <summary>
        /// The F20 key.
        /// </devdoc>
        F20 = 0x83,
        
        /// <summary>
        /// The F21 key.
        /// </devdoc>
        F21 = 0x84,
        
        /// <summary>
        /// The F22 key.
        /// </devdoc>
        F22 = 0x85,
        
        /// <summary>
        /// The F23 key.
        /// </devdoc>
        F23 = 0x86,
        
        /// <summary>
        /// The F24 key.
        /// </devdoc>
        F24 = 0x87,
        
        /// <summary>
        /// The NUM LOCK key.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumLock = 0x90,
        
        /// <summary>
        /// The SCROLL LOCK key.
        /// </devdoc>
        Scroll = 0x91,
        
        /// <summary>
        /// The left SHIFT key.
        /// </devdoc>
        LShiftKey = 0xA0,
        
        /// <summary>
        /// The right SHIFT key.
        /// </devdoc>
        RShiftKey = 0xA1,
        
        /// <summary>
        /// The left CTRL key.
        /// </devdoc>
        LControlKey = 0xA2,
        
        /// <summary>
        /// The right CTRL key.
        /// </devdoc>
        RControlKey = 0xA3,
        
        /// <summary>
        /// The left ALT key.
        /// </devdoc>
        LMenu = 0xA4,
        
        /// <summary>
        /// The right ALT key.
        /// </devdoc>
        RMenu = 0xA5,
        
        /// <summary>
        /// The Browser Back key.
        /// </devdoc>
        BrowserBack = 0xA6,
        
        /// <summary>
        /// The Browser Forward key.
        /// </devdoc>
        BrowserForward= 0xA7,
        
        /// <summary>
        /// The Browser Refresh key.
        /// </devdoc>
        BrowserRefresh= 0xA8,
        
        /// <summary>
        /// The Browser Stop key.
        /// </devdoc>
        BrowserStop = 0xA9,
        
        /// <summary>
        /// The Browser Search key.
        /// </devdoc>
        BrowserSearch = 0xAA,
        
        /// <summary>
        /// The Browser Favorites key.
        /// </devdoc>
        BrowserFavorites = 0xAB,
        
        /// <summary>
        /// The Browser Home key.
        /// </devdoc>
        BrowserHome = 0xAC,
        
        /// <summary>
        /// The Volume Mute key.
        /// </devdoc>
        VolumeMute = 0xAD,
        
        /// <summary>
        /// The Volume Down key.
        /// </devdoc>
        VolumeDown = 0xAE,
        
        /// <summary>
        /// The Volume Up key.
        /// </devdoc>
        VolumeUp = 0xAF,
        
        /// <summary>
        /// The Media Next Track key.
        /// </devdoc>
        MediaNextTrack = 0xB0,
        
        /// <summary>
        /// The Media Previous Track key.
        /// </devdoc>
        MediaPreviousTrack = 0xB1,
        
        /// <summary>
        /// The Media Stop key.
        /// </devdoc>
        MediaStop = 0xB2,
        
        /// <summary>
        /// The Media Play Pause key.
        /// </devdoc>
        MediaPlayPause = 0xB3,
        
        /// <summary>
        /// The Launch Mail key.
        /// </devdoc>
        LaunchMail = 0xB4,
        
        /// <summary>
        /// The Select Media key.
        /// </devdoc>
        SelectMedia = 0xB5,
        
        /// <summary>
        /// The Launch Application1 key.
        /// </devdoc>
        LaunchApplication1 = 0xB6,
        
        /// <summary>
        /// The Launch Application2 key.
        /// </devdoc>
        LaunchApplication2 = 0xB7,
        
        /// <summary>
        /// The Oem Semicolon key.
        /// </devdoc>
        OemSemicolon = 0xBA,
        
        /// <summary>
        /// The Oem 1 key.
        /// </devdoc>
        Oem1 = OemSemicolon,
        
        /// <summary>
        /// The Oem plus key.
        /// </devdoc>
        Oemplus = 0xBB,
        
        /// <summary>
        /// The Oem comma key.
        /// </devdoc>
        Oemcomma = 0xBC,
        
        /// <summary>
        /// The Oem Minus key.
        /// </devdoc>
        OemMinus = 0xBD,
        
        /// <summary>
        /// The Oem Period key.
        /// </devdoc>
        OemPeriod = 0xBE,
        
        /// <summary>
        /// The Oem Question key.
        /// </devdoc>
        OemQuestion = 0xBF,
        
        /// <summary>
        /// The Oem 2 key.
        /// </devdoc>
        Oem2 = OemQuestion,
        
        /// <summary>
        /// The Oem tilde key.
        /// </devdoc>
        Oemtilde = 0xC0,
        
        /// <summary>
        /// The Oem 3 key.
        /// </devdoc>
        Oem3 = Oemtilde,
        
        /// <summary>
        /// The Oem Open Brackets key.
        /// </devdoc>
        OemOpenBrackets = 0xDB,
        
        /// <summary>
        /// The Oem 4 key.
        /// </devdoc>
        Oem4 = OemOpenBrackets,
        
        /// <summary>
        /// The Oem Pipe key.
        /// </devdoc>
        OemPipe = 0xDC,
        
        /// <summary>
        /// The Oem 5 key.
        /// </devdoc>
        Oem5 = OemPipe,
        
        /// <summary>
        /// The Oem Close Brackets key.
        /// </devdoc>
        OemCloseBrackets = 0xDD,
        
        /// <summary>
        /// The Oem 6 key.
        /// </devdoc>
        Oem6 = OemCloseBrackets,
        
        /// <summary>
        /// The Oem Quotes key.
        /// </devdoc>
        OemQuotes = 0xDE,
        
        /// <summary>
        /// The Oem 7 key.
        /// </devdoc>
        Oem7 = OemQuotes,
        
        /// <summary>
        /// The Oem8 key.
        /// </devdoc>
        Oem8 = 0xDF,
        
        /// <summary>
        /// The Oem Backslash key.
        /// </devdoc>
        OemBackslash = 0xE2,
        
        /// <summary>
        /// The Oem 102 key.
        /// </devdoc>
        Oem102 = OemBackslash,
        
        /// <summary>
        /// The PROCESS KEY key.
        /// </devdoc>
        ProcessKey = 0xE5,
        
        /// <summary>
        /// The Packet KEY key.
        /// </devdoc>
        Packet = 0xE7,
        
        /// <summary>
        /// The ATTN key.
        /// </devdoc>
        Attn = 0xF6,
        
        /// <summary>
        /// The CRSEL key.
        /// </devdoc>
        Crsel = 0xF7,
        
        /// <summary>
        /// The EXSEL key.
        /// </devdoc>
        Exsel = 0xF8,
        
        /// <summary>
        /// The ERASE EOF key.
        /// </devdoc>
        EraseEof = 0xF9,
        
        /// <summary>
        /// The PLAY key.
        /// </devdoc>
        Play = 0xFA,
        
        /// <summary>
        /// The ZOOM key.
        /// </devdoc>
        Zoom = 0xFB,
        
        /// <summary>
        /// A constant reserved for future use.
        /// </devdoc>
        NoName = 0xFC,
        
        /// <summary>
        /// The PA1 key.
        /// </devdoc>
        Pa1 = 0xFD,
        
        /// <summary>
        /// The CLEAR key.
        /// </devdoc>
        OemClear = 0xFE,
        
        /// <summary>
        /// The SHIFT modifier key.
        /// </devdoc>
        Shift = 0x00010000,
        
        /// <summary>
        /// The  CTRL modifier key.
        /// </devdoc>
        Control = 0x00020000,
        
        /// <summary>
        /// The ALT modifier key.
        /// </devdoc>
        Alt = 0x00040000,
    }
}
