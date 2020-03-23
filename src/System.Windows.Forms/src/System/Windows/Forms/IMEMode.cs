// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies a value that determines the IME (Input Method Editor) status
    ///  of the object when that object is selected.
    /// </summary>
    [ComVisible(true)]
    public enum ImeMode
    {
        /// <summary>
        ///  Inherit (Default). This value indicates inherit the ImeMode from the
        ///  parent control. For controls with no parent, the ImeMode will default
        ///  to NoControl.
        /// </summary>
        Inherit = -1,

        /// <summary>
        ///  None. This value indicates "No control to IME". When the IMEMode
        ///  property is set to 0, you can use the IMEStatus function to determine
        ///  the current IME status.
        /// </summary>
        NoControl = 0,

        /// <summary>
        ///  IME on. This value indicates that the IME is on and characters specific
        ///  to Chinese or Japanese can be entered. This setting is valid for
        ///  Japanese, Simplified Chinese, and Traditional Chinese IME only.
        /// </summary>
        On = 1,

        /// <summary>
        ///  IME off. This mode indicates that the IME is off, meaning that the
        ///  object behaves the same as English entry mode. This setting is valid
        ///  for Japanese, Simplified Chinese, and Traditional Chinese IME only.
        /// </summary>
        Off = 2,

        /// <summary>
        ///  IME disabled. This mode is similar to IMEMode = 2, except the value 3
        ///  disables IME. With this setting, the users cannot turn the IME on from
        ///  the keyboard, and the IME floating window is hidden. This setting is
        ///  valid for Japanese IME only.
        /// </summary>
        Disable = 3,

        /// <summary>
        ///  Hiragana double-byte characters (DBC). This setting is valid for
        ///  Japanese IME only.
        /// </summary>
        Hiragana = 4,

        /// <summary>
        ///  Katakana DBC. This setting is valid for Japanese IME only.
        /// </summary>
        Katakana = 5,

        /// <summary>
        ///  Katakana single-byte characters (SBC). This setting is valid for
        ///  Japanese IME only.
        /// </summary>
        KatakanaHalf = 6,

        /// <summary>
        ///  Alphanumeric DBC. This setting is valid for Japanese IME only.
        /// </summary>
        AlphaFull = 7,

        /// <summary>
        ///  Alphanumeric SBC. This setting is valid for Japanese IME only.
        /// </summary>
        Alpha = 8,

        /// <summary>
        ///  Hangeul DBC. This setting is valid for Korean IME only.
        /// </summary>
        HangulFull = 9,

        /// <summary>
        ///  Hangeul SBC. This setting is valid for Korean IME only.
        /// </summary>
        Hangul = 10,

        /// <summary>
        ///  Ime Closed. This setting is valid for Chinese IME only.
        /// </summary>
        Close = 11,

        /// <summary>
        ///  Ime On HalfShape. This setting is valid for Chinese IME only.
        ///  Note: This value is for internal use only - See QFE 4448.
        /// </summary>
        OnHalf = 12,
    }
}
