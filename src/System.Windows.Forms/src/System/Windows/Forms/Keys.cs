// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies key codes and modifiers.
    /// </devdoc>
    [Flags]
    [TypeConverterAttribute(typeof(KeysConverter))]
    [Editor("System.Windows.Forms.Design.ShortcutKeysEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    [ComVisible(true)]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")] // Certain memberd of Keys enum are actually meant to be OR'ed.
    public enum Keys
    {
        /// <devdoc>
        /// The bit mask to extract a key code from a key value.
        /// </devdoc>
        KeyCode = 0x0000FFFF,

        /// <devdoc>
        /// The bit mask to extract modifiers from a key value.
        /// </devdoc>
        Modifiers = unchecked((int)0xFFFF0000),

        /// <devdoc>
        /// No key pressed.
        /// </devdoc>
        None           = 0x00,

        /// <devdoc>
        /// The left mouse button.
        /// </devdoc>
        LButton        = 0x01,

        /// <devdoc>
        /// The right mouse button.
        /// </devdoc>
        RButton        = 0x02,

        /// <devdoc>
        /// The CANCEL key.
        /// </devdoc>
        Cancel         = 0x03,

        /// <devdoc>
        /// The middle mouse button (three-button mouse).
        /// </devdoc>
        MButton        = 0x04,

        /// <devdoc>
        /// The first x mouse button (five-button mouse).
        /// </devdoc>
        XButton1       = 0x05,

        /// <devdoc>
        /// The second x mouse button (five-button mouse).
        /// </devdoc>
        XButton2       = 0x06,

        /// <devdoc>
        ///    <para>
        /// The BACKSPACE key.
        ///    </para>
        /// </devdoc>
        Back           = 0x08,

        /// <devdoc>
        /// The TAB key.
        /// </devdoc>
        Tab            = 0x09,

        /// <devdoc>
        /// The CLEAR key.
        /// </devdoc>
        LineFeed       = 0x0A,

        /// The CLEAR key.
        /// </devdoc>
        Clear          = 0x0C,

        /// <devdoc>
        /// The RETURN key.
        /// </devdoc>
        Return         = 0x0D,

        /// <devdoc>
        /// The ENTER key.
        /// </devdoc>
        Enter          = Return,

        /// <devdoc>
        /// The SHIFT key.
        /// </devdoc>
        ShiftKey      = 0x10,

        /// <devdoc>
        /// The CTRL key.
        /// </devdoc>
        ControlKey    = 0x11,

        /// <devdoc>
        /// The ALT key.
        /// </devdoc>
        Menu           = 0x12,

        /// <devdoc>
        /// The PAUSE key.
        /// </devdoc>
        Pause          = 0x13,

        /// <devdoc>
        /// The CAPS LOCK key.
        /// </devdoc>
        Capital        = 0x14,

        /// <devdoc>
        /// The CAPS LOCK key.
        /// </devdoc>
        CapsLock       = 0x14,

        /// <devdoc>
        /// The IME Kana mode key.
        /// </devdoc>
        KanaMode      = 0x15,

        /// <devdoc>
        /// The IME Hanguel mode key.
        /// </devdoc>
        HanguelMode   = 0x15,

        /// <devdoc>
        /// The IME Hangul mode key.
        /// </devdoc>
        HangulMode    = 0x15,

        /// <devdoc>
        /// The IME Junja mode key.
        /// </devdoc>
        JunjaMode     = 0x17,

        /// <devdoc>
        /// The IME Final mode key.
        /// </devdoc>
        FinalMode     = 0x18,

        /// <devdoc>
        /// The IME Hanja mode key.
        /// </devdoc>
        HanjaMode     = 0x19,

        /// <devdoc>
        /// The IME Kanji mode key.
        /// </devdoc>
        KanjiMode     = 0x19,

        /// <devdoc>
        /// The ESC key.
        /// </devdoc>
        Escape         = 0x1B,

        /// <devdoc>
        /// The IME Convert key.
        /// </devdoc>
        IMEConvert    = 0x1C,

        /// <devdoc>
        /// The IME NonConvert key.
        /// </devdoc>
        IMENonconvert = 0x1D,
	   
        /// <devdoc>
        /// The IME Accept key.
        /// </devdoc>
        IMEAccept     = 0x1E,
	    
        /// <devdoc>
        /// The IME Accept key.
        /// </devdoc>
        IMEAceept     = IMEAccept,
        
        /// <devdoc>
        /// The IME Mode change request.
        /// </devdoc>
        IMEModeChange = 0x1F,
        
        /// <devdoc>
        /// The SPACEBAR key.
        /// </devdoc>
        Space          = 0x20,
        
        /// <devdoc>
        /// The PAGE UP key.
        /// </devdoc>
        Prior          = 0x21,
        
        /// <devdoc>
        /// The PAGE UP key.
        /// </devdoc>
        PageUp         = Prior,
        
        /// <devdoc>
        /// The PAGE DOWN key.
        /// </devdoc>
        Next           = 0x22,
        
        /// <devdoc>
        /// The PAGE DOWN key.
        /// </devdoc>
        PageDown       = Next,
        
        /// <devdoc>
        /// The END key.
        /// </devdoc>
        End            = 0x23,
        
        /// <devdoc>
        /// The HOME key.
        /// </devdoc>
        Home           = 0x24,
        
        /// <devdoc>
        /// The LEFT ARROW key.
        /// </devdoc>
        Left           = 0x25,
        
        /// <devdoc>
        /// The UP ARROW key.
        /// </devdoc>
        Up             = 0x26,
        
        /// <devdoc>
        /// The RIGHT ARROW key.
        /// </devdoc>
        Right          = 0x27,
        
        /// <devdoc>
        /// The DOWN ARROW key.
        /// </devdoc>
        Down           = 0x28,
        
        /// <devdoc>
        /// The SELECT key.
        /// </devdoc>
        Select         = 0x29,
        
        /// <devdoc>
        /// The PRINT key.
        /// </devdoc>
        Print          = 0x2A,
        
        /// <devdoc>
        /// The EXECUTE key.
        /// </devdoc>
        Execute        = 0x2B,
        
        /// <devdoc>
        /// The PRINT SCREEN key.
        /// </devdoc>
        Snapshot       = 0x2C,
        
        /// <devdoc>
        /// The PRINT SCREEN key.
        /// </devdoc>
        PrintScreen    = Snapshot,
        
        /// <devdoc>
        /// The INS key.
        /// </devdoc>
        Insert         = 0x2D,
        
        /// <devdoc>
        /// The DEL key.
        /// </devdoc>
        Delete         = 0x2E,
        
        /// <devdoc>
        /// The HELP key.
        /// </devdoc>
        Help           = 0x2F,
        
        /// <devdoc>
        /// The 0 key.
        /// </devdoc>
        D0             = 0x30, // 0
        
        /// <devdoc>
        /// The 1 key.
        /// </devdoc>
        D1             = 0x31, // 1
        
        /// <devdoc>
        /// The 2 key.
        /// </devdoc>
        D2             = 0x32, // 2
        
        /// <devdoc>
        /// The 3 key.
        /// </devdoc>
        D3             = 0x33, // 3
        
        /// <devdoc>
        /// The 4 key.
        /// </devdoc>
        D4             = 0x34, // 4
        
        /// <devdoc>
        /// The 5 key.
        /// </devdoc>
        D5             = 0x35, // 5
        
        /// <devdoc>
        /// The 6 key.
        /// </devdoc>
        D6             = 0x36, // 6
        
        /// <devdoc>
        /// The 7 key.
        /// </devdoc>
        D7             = 0x37, // 7
        
        /// <devdoc>
        /// The 8 key.
        /// </devdoc>
        D8             = 0x38, // 8
        
        /// <devdoc>
        /// The 9 key.
        /// </devdoc>
        D9             = 0x39, // 9
        
        /// <devdoc>
        /// The A key.
        /// </devdoc>
        A              = 0x41,
        
        /// <devdoc>
        /// The B key.
        /// </devdoc>
        B              = 0x42,
        
        /// <devdoc>
        /// The C key.
        /// </devdoc>
        C              = 0x43,
        
        /// <devdoc>
        /// The D key.
        /// </devdoc>
        D              = 0x44,
        
        /// <devdoc>
        /// The E key.
        /// </devdoc>
        E              = 0x45,
        
        /// <devdoc>
        /// The F key.
        /// </devdoc>
        F              = 0x46,
        
        /// <devdoc>
        /// The G key.
        /// </devdoc>
        G              = 0x47,
        
        /// <devdoc>
        /// The H key.
        /// </devdoc>
        H              = 0x48,
        
        /// <devdoc>
        /// The I key.
        /// </devdoc>
        I              = 0x49,
        
        /// <devdoc>
        /// The J key.
        /// </devdoc>
        J              = 0x4A,
        
        /// <devdoc>
        /// The K key.
        /// </devdoc>
        K              = 0x4B,
        
        /// <devdoc>
        /// The L key.
        /// </devdoc>
        L              = 0x4C,
        
        /// <devdoc>
        /// The M key.
        /// </devdoc>
        M              = 0x4D,
        
        /// <devdoc>
        /// The N key.
        /// </devdoc>
        N              = 0x4E,
        
        /// <devdoc>
        /// The O key.
        /// </devdoc>
        O              = 0x4F,
        
        /// <devdoc>
        /// The P key.
        /// </devdoc>
        P              = 0x50,
        
        /// <devdoc>
        /// The Q key.
        /// </devdoc>
        Q              = 0x51,
        
        /// <devdoc>
        /// The R key.
        /// </devdoc>
        R              = 0x52,
        
        /// <devdoc>
        /// The S key.
        /// </devdoc>
        S              = 0x53,
        
        /// <devdoc>
        /// The T key.
        /// </devdoc>
        T              = 0x54,
        
        /// <devdoc>
        /// The U key.
        /// </devdoc>
        U              = 0x55,
        
        /// <devdoc>
        /// The V key.
        /// </devdoc>
        V              = 0x56,
        
        /// <devdoc>
        /// The W key.
        /// </devdoc>
        W              = 0x57,
        
        /// <devdoc>
        /// The X key.
        /// </devdoc>
        X              = 0x58,
        
        /// <devdoc>
        /// The Y key.
        /// </devdoc>
        Y              = 0x59,
        
        /// <devdoc>
        /// The Z key.
        /// </devdoc>
        Z              = 0x5A,
        
        /// <devdoc>
        /// The left Windows logo key (Microsoft Natural Keyboard).
        /// </devdoc>
        LWin           = 0x5B,
        
        /// <devdoc>
        /// The right Windows logo key (Microsoft Natural Keyboard).
        /// </devdoc>
        RWin           = 0x5C,
        
        /// <devdoc>
        /// The Application key (Microsoft Natural Keyboard).
        /// </devdoc>
        
        Apps           = 0x5D,
        
        /// <devdoc>
        /// The Computer Sleep key.
        /// </devdoc>
        Sleep          = 0x5F,
        
        /// <devdoc>
        /// The 0 key on the numeric keypad.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumPad0        = 0x60,
       
        /// <devdoc>
        /// The 1 key on the numeric keypad.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumPad1        = 0x61,
        
        /// <devdoc>
        /// The 2 key on the numeric keypad.
        /// </devdoc>
        NumPad2        = 0x62,
        
        /// <devdoc>
        /// The 3 key on the numeric keypad.
        /// </devdoc>
        NumPad3        = 0x63,
        
        /// <devdoc>
        /// The 4 key on the numeric keypad.
        /// </devdoc>
        NumPad4        = 0x64,
        
        /// <devdoc>
        /// The 5 key on the numeric keypad.
        /// </devdoc>
        NumPad5        = 0x65,
        
        /// <devdoc>
        /// The 6 key on the numeric keypad.
        /// </devdoc>
        NumPad6        = 0x66,
        
        /// <devdoc>
        /// The 7 key on the numeric keypad.
        /// </devdoc>
        NumPad7        = 0x67,
        
        /// <devdoc>
        /// The 8 key on the numeric keypad.
        /// </devdoc>
        NumPad8        = 0x68,
        
        /// <devdoc>
        /// The 9 key on the numeric keypad.
        /// </devdoc>
        NumPad9        = 0x69,
        
        /// <devdoc>
        /// The Multiply key.
        /// </devdoc>
        Multiply       = 0x6A,
        
        /// <devdoc>
        /// The Add key.
        /// </devdoc>
        Add            = 0x6B,
        
        /// <devdoc>
        /// The Separator key.
        /// </devdoc>
        Separator      = 0x6C,
        
        /// <devdoc>
        /// The Subtract key.
        /// </devdoc>
        Subtract       = 0x6D,
        
        /// <devdoc>
        /// The Decimal key.
        /// </devdoc>
        Decimal        = 0x6E,
        
        /// <devdoc>
        /// The Divide key.
        /// </devdoc>
        Divide         = 0x6F,
        
        /// <devdoc>
        /// The F1 key.
        /// </devdoc>
        F1             = 0x70,
        
        /// <devdoc>
        /// The F2 key.
        /// </devdoc>
        F2             = 0x71,
        
        /// <devdoc>
        /// The F3 key.
        /// </devdoc>
        F3             = 0x72,
        
        /// <devdoc>
        /// The F4 key.
        /// </devdoc>
        F4             = 0x73,
        
        /// <devdoc>
        /// The F5 key.
        /// </devdoc>
        F5             = 0x74,
        
        /// <devdoc>
        /// The F6 key.
        /// </devdoc>
        F6             = 0x75,
        
        /// <devdoc>
        /// The F7 key.
        /// </devdoc>
        F7             = 0x76,
        
        /// <devdoc>
        /// The F8 key.
        /// </devdoc>
        F8             = 0x77,
        
        /// <devdoc>
        /// The F9 key.
        /// </devdoc>
        F9             = 0x78,
        
        /// <devdoc>
        /// The F10 key.
        /// </devdoc>
        F10            = 0x79,
        
        /// <devdoc>
        /// The F11 key.
        /// </devdoc>
        F11            = 0x7A,
        
        /// <devdoc>
        /// The F12 key.
        /// </devdoc>
        F12            = 0x7B,
        
        /// <devdoc>
        /// The F13 key.
        /// </devdoc>
        F13            = 0x7C,
        
        /// <devdoc>
        /// The F14 key.
        /// </devdoc>
        F14            = 0x7D,
        
        /// <devdoc>
        /// The F15 key.
        /// </devdoc>
        F15            = 0x7E,
        
        /// <devdoc>
        /// The F16 key.
        /// </devdoc>
        F16            = 0x7F,
        
        /// <devdoc>
        /// The F17 key.
        /// </devdoc>
        F17            = 0x80,
        
        /// <devdoc>
        /// The F18 key.
        /// </devdoc>
        F18            = 0x81,
        
        /// <devdoc>
        /// The F19 key.
        /// </devdoc>
        F19            = 0x82,
        
        /// <devdoc>
        /// The F20 key.
        /// </devdoc>
        F20            = 0x83,
        
        /// <devdoc>
        /// The F21 key.
        /// </devdoc>
        F21            = 0x84,
        
        /// <devdoc>
        /// The F22 key.
        /// </devdoc>
        F22            = 0x85,
        
        /// <devdoc>
        /// The F23 key.
        /// </devdoc>
        F23            = 0x86,
        
        /// <devdoc>
        /// The F24 key.
        /// </devdoc>
        F24            = 0x87,
        
        /// <devdoc>
        /// The NUM LOCK key.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
        NumLock        = 0x90,
        
        /// <devdoc>
        /// The SCROLL LOCK key.
        /// </devdoc>
        Scroll         = 0x91,
        
        /// <devdoc>
        /// The left SHIFT key.
        /// </devdoc>
        LShiftKey     = 0xA0,
        
        /// <devdoc>
        /// The right SHIFT key.
        /// </devdoc>
        RShiftKey     = 0xA1,
        
        /// <devdoc>
        /// The left CTRL key.
        /// </devdoc>
        LControlKey   = 0xA2,
        
        /// <devdoc>
        /// The right CTRL key.
        /// </devdoc>
        RControlKey   = 0xA3,
        
        /// <devdoc>
        /// The left ALT key.
        /// </devdoc>
        LMenu          = 0xA4,
        
        /// <devdoc>
        /// The right ALT key.
        /// </devdoc>
        RMenu          = 0xA5,
        
        /// <devdoc>
        /// The Browser Back key.
        /// </devdoc>
        BrowserBack   = 0xA6,
        
        /// <devdoc>
        /// The Browser Forward key.
        /// </devdoc>
        BrowserForward= 0xA7,
        
        /// <devdoc>
        /// The Browser Refresh key.
        /// </devdoc>
        BrowserRefresh= 0xA8,
        
        /// <devdoc>
        /// The Browser Stop key.
        /// </devdoc>
        BrowserStop   = 0xA9,
        
        /// <devdoc>
        /// The Browser Search key.
        /// </devdoc>
        BrowserSearch = 0xAA,
        
        /// <devdoc>
        /// The Browser Favorites key.
        /// </devdoc>
        BrowserFavorites = 0xAB,
        
        /// <devdoc>
        /// The Browser Home key.
        /// </devdoc>
        BrowserHome   = 0xAC,
        
        /// <devdoc>
        /// The Volume Mute key.
        /// </devdoc>
        VolumeMute    = 0xAD,
        
        /// <devdoc>
        /// The Volume Down key.
        /// </devdoc>
        VolumeDown    = 0xAE,
        
        /// <devdoc>
        /// The Volume Up key.
        /// </devdoc>
        VolumeUp      = 0xAF,
        
        /// <devdoc>
        /// The Media Next Track key.
        /// </devdoc>
        MediaNextTrack = 0xB0,
        
        /// <devdoc>
        /// The Media Previous Track key.
        /// </devdoc>
        MediaPreviousTrack = 0xB1,
        
        /// <devdoc>
        /// The Media Stop key.
        /// </devdoc>
        MediaStop     = 0xB2,
        
        /// <devdoc>
        /// The Media Play Pause key.
        /// </devdoc>
        MediaPlayPause = 0xB3,
        
        /// <devdoc>
        /// The Launch Mail key.
        /// </devdoc>
        LaunchMail    = 0xB4,
        
        /// <devdoc>
        /// The Select Media key.
        /// </devdoc>
        SelectMedia   = 0xB5,
        
        /// <devdoc>
        /// The Launch Application1 key.
        /// </devdoc>
        LaunchApplication1 = 0xB6,
        
        /// <devdoc>
        /// The Launch Application2 key.
        /// </devdoc>
        LaunchApplication2 = 0xB7,
        
        /// <devdoc>
        /// The Oem Semicolon key.
        /// </devdoc>
        OemSemicolon  = 0xBA,
        
        /// <devdoc>
        /// The Oem 1 key.
        /// </devdoc>
        Oem1 = OemSemicolon,
        
        /// <devdoc>
        /// The Oem plus key.
        /// </devdoc>
        Oemplus       = 0xBB,
        
        /// <devdoc>
        /// The Oem comma key.
        /// </devdoc>
        Oemcomma      = 0xBC,
        
        /// <devdoc>
        /// The Oem Minus key.
        /// </devdoc>
        OemMinus      = 0xBD,
        
        /// <devdoc>
        /// The Oem Period key.
        /// </devdoc>
        OemPeriod     = 0xBE,
        
        /// <devdoc>
        /// The Oem Question key.
        /// </devdoc>
        OemQuestion   = 0xBF,
        
        /// <devdoc>
        /// The Oem 2 key.
        /// </devdoc>
        Oem2 = OemQuestion,
        
        /// <devdoc>
        /// The Oem tilde key.
        /// </devdoc>
        Oemtilde      = 0xC0,
        
        /// <devdoc>
        /// The Oem 3 key.
        /// </devdoc>
        Oem3 = Oemtilde,
        
        /// <devdoc>
        /// The Oem Open Brackets key.
        /// </devdoc>
        OemOpenBrackets = 0xDB,
        
        /// <devdoc>
        /// The Oem 4 key.
        /// </devdoc>
        Oem4 = OemOpenBrackets,
        
        /// <devdoc>
        /// The Oem Pipe key.
        /// </devdoc>
        OemPipe       = 0xDC,
        
        /// <devdoc>
        /// The Oem 5 key.
        /// </devdoc>
        Oem5 = OemPipe,
        
        /// <devdoc>
        /// The Oem Close Brackets key.
        /// </devdoc>
        OemCloseBrackets = 0xDD,
        
        /// <devdoc>
        /// The Oem 6 key.
        /// </devdoc>
        Oem6 = OemCloseBrackets,
        
        /// <devdoc>
        /// The Oem Quotes key.
        /// </devdoc>
        OemQuotes     = 0xDE,
        
        /// <devdoc>
        /// The Oem 7 key.
        /// </devdoc>
        Oem7 = OemQuotes,
        
        /// <devdoc>
        /// The Oem8 key.
        /// </devdoc>
        Oem8          = 0xDF,
        
        /// <devdoc>
        /// The Oem Backslash key.
        /// </devdoc>
        OemBackslash  = 0xE2,
        
        /// <devdoc>
        /// The Oem 102 key.
        /// </devdoc>
        Oem102 = OemBackslash,
        
        /// <devdoc>
        /// The PROCESS KEY key.
        /// </devdoc>
        ProcessKey     = 0xE5,
        
        /// <devdoc>
        /// The Packet KEY key.
        /// </devdoc>
        Packet     = 0xE7,
        
        /// <devdoc>
        /// The ATTN key.
        /// </devdoc>
        Attn           = 0xF6,
        
        /// <devdoc>
        /// The CRSEL key.
        /// </devdoc>
        Crsel          = 0xF7,
        
        /// <devdoc>
        /// The EXSEL key.
        /// </devdoc>
        Exsel          = 0xF8,
        
        /// <devdoc>
        /// The ERASE EOF key.
        /// </devdoc>
        EraseEof          = 0xF9,
        
        /// <devdoc>
        /// The PLAY key.
        /// </devdoc>
        Play           = 0xFA,
        
        /// <devdoc>
        /// The ZOOM key.
        /// </devdoc>
        Zoom           = 0xFB,
        
        /// <devdoc>
        /// A constant reserved for future use.
        /// </devdoc>
        NoName         = 0xFC,
        
        /// <devdoc>
        /// The PA1 key.
        /// </devdoc>
        Pa1            = 0xFD,
        
        /// <devdoc>
        /// The CLEAR key.
        /// </devdoc>
        OemClear      = 0xFE,
        
        /// <devdoc>
        /// The SHIFT modifier key.
        /// </devdoc>
        Shift   = 0x00010000,
        
        /// <devdoc>
        /// The  CTRL modifier key.
        /// </devdoc>
        Control = 0x00020000,
        
        /// <devdoc>
        /// The ALT modifier key.
        /// </devdoc>
        Alt     = 0x00040000,
    }
}
