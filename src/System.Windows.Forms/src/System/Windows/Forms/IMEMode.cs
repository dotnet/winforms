// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies a value that determines the IME (Input Method Editor) status of the 
    ///       object when that object is selected.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum ImeMode {
    

        /// <devdoc>
        ///    <para>
        ///       Inherit (Default). This value indicates inherit the ImeMode from the parent control. For controls with no parent,
        ///       the ImeMode will default to NoControl.
        ///    </para>
        /// </devdoc>
        Inherit = -1,


        /// <devdoc>
        ///    <para>
        ///       None. This value indicates "No control to IME". When the IMEMode property is set to 0, you can use the 
        ///       IMEStatus function to determine the current IME status. 
        ///    </para>
        /// </devdoc>
        NoControl = 0,


        /// <devdoc>
        ///    <para>
        ///       IME on. This value indicates that the IME is on and characters specific to Chinese or Japanese can be entered. 
        ///       This setting is valid for Japanese, Simplified Chinese, and Traditional Chinese IME only. 
        ///    </para>
        /// </devdoc>
        On = 1,


        /// <devdoc>
        ///    <para>
        ///       IME off. This mode indicates that the IME is off, meaning that the object behaves the same as English entry mode. 
        ///       This setting is valid for Japanese, Simplified Chinese, and Traditional Chinese IME only. 
        ///    </para>
        /// </devdoc>
        Off = 2,
        

        /// <devdoc>
        ///    <para>
        ///       IME disabled. This mode is similar to IMEMode = 2, except the value 3 disables IME. With this setting, the users 
        ///       cannot turn the IME on from the keyboard, and the IME floating window is hidden. This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        Disable = 3,
        

        /// <devdoc>
        ///    <para>
        ///       Hiragana double-byte characters (DBC). This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        Hiragana = 4,
        

        /// <devdoc>
        ///    <para>
        ///       Katakana DBC. This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        Katakana = 5,
        

        /// <devdoc>
        ///    <para>
        ///       Katakana single-byte characters (SBC). This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        KatakanaHalf = 6,
        

        /// <devdoc>
        ///    <para>
        ///       Alphanumeric DBC. This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        AlphaFull = 7,
        

        /// <devdoc>
        ///    <para>
        ///       Alphanumeric SBC. This setting is valid for Japanese IME only. 
        ///    </para>
        /// </devdoc>
        Alpha = 8,
        

        /// <devdoc>
        ///    <para>
        ///       Hangeul DBC. This setting is valid for Korean IME only. 
        ///    </para>
        /// </devdoc>
        HangulFull = 9,
        

        /// <devdoc>
        ///    <para>
        ///       Hangeul SBC. This setting is valid for Korean IME only.
        ///    </para>
        /// </devdoc>
        Hangul = 10,


        /// <devdoc>
        ///    <para>
        ///       Ime Closed. This setting is valid for Chinese IME only.
        ///    </para>
        /// </devdoc>
        Close = 11,


        /// <devdoc>
        ///    <para>
        ///       Ime On HalfShape. This setting is valid for Chinese IME only.
        ///       Note: This value is for internal use only - See QFE 4448.
        ///    </para>
        /// </devdoc>
        OnHalf = 12,
    }
}
