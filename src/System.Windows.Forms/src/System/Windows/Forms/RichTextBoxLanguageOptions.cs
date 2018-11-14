// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms 
{
    /// <devdoc>
    ///    Rich edit control's option settings for Input Method Editor (IME) and 
    ///    Asian language support
    /// </devdoc>
    [Flags]
    public enum RichTextBoxLanguageOptions
	{
        /// <devdoc>
        ///    <para>
        ///         If this flag is set, the control automatically changes fonts when the user 
        ///         explicitly changes to a different keyboard layout. 
        ///    </para>
        /// </devdoc>
        AutoFont = 0x0002,

        /// <devdoc>
        ///    <para>
        ///         Font-bound font sizes are scaled from insertion point size according to a script. 
        ///         For example, Asian fonts are slightly larger than Western. This is the default. 
        ///    </para>
        /// </devdoc>
        AutoFontSizeAdjust = 0x0010,

        /// <devdoc>
        ///    <para>
        ///         If this flag is set, the control automatically changes the keyboard layout when the 
        ///         user explicitly changes to a different font, or when the user explicitly changes the insertion point to a new location in the text.  
        ///    </para>
        /// </devdoc>
        AutoKeyboard = 0x0001,

        /// <devdoc>
        ///    <para>
        ///         Sets the control to dual-font mode. Used for Asian language text. The control 
        ///         uses an English font for ASCII text and an Asian font for Asian text. 
        ///    </para>
        /// </devdoc>
        DualFont = 0x0080,

        /// <devdoc>
        ///    <para>
        ///         Controls how Rich Edit notifies the client during IME composition:
        ///         0: No EN_CHANGE or EN_SELCHANGE notifications during undetermined state. 
        ///            Send notification when final string comes in. (default)
        ///         1: Send EN_CHANGE and EN_SELCHANGE events during undetermined state.
        ///    </para>
        /// </devdoc>
        ImeAlwaysSendNotify = 0x0008,

        /// <devdoc>
        ///    <para>
        ///         This flag determines how the control uses the composition string of an IME 
        ///         if the user cancels it. If this flag is set, the control discards the composition string. 
        ///         If this flag is not set, the control uses the composition string as the result string. 
        ///    </para>
        /// </devdoc>
        ImeCancelComplete = 0x0004,

        /// <devdoc>
        ///    <para>
        ///         Use UI default fonts. This option is turned off by default. 
        ///    </para>
        /// </devdoc>
        UIFonts = 0x0020
    }
}

