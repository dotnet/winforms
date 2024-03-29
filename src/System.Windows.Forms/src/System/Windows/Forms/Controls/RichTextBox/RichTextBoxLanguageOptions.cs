﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Rich edit control's option settings for Input Method Editor (IME) and
///  Asian language support
/// </summary>
[Flags]
public enum RichTextBoxLanguageOptions
{
    /// <summary>
    ///  If this flag is set, the control automatically changes fonts when the
    ///  user explicitly changes to a different keyboard layout.
    /// </summary>
    AutoFont = 0x0002,

    /// <summary>
    ///  Font-bound font sizes are scaled from insertion point size according to
    ///  a script. For example, Asian fonts are slightly larger than Western.
    ///  This is the default.
    /// </summary>
    AutoFontSizeAdjust = 0x0010,

    /// <summary>
    ///  If this flag is set, the control automatically changes the keyboard
    ///  layout when the  user explicitly changes to a different font, or when
    ///  the user explicitly changes the insertion point to a new location in
    ///  the text.
    /// </summary>
    AutoKeyboard = 0x0001,

    /// <summary>
    ///  Sets the control to dual-font mode. Used for Asian language text.
    ///  The control uses an English font for ASCII text and an Asian font for
    ///  Asian text.
    /// </summary>
    DualFont = 0x0080,

    /// <summary>
    ///  Controls how Rich Edit notifies the client during IME composition:
    ///  0: No EN_CHANGE or EN_SELCHANGE notifications during undetermined state.
    ///  Send notification when final string comes in. (default)
    ///  1: Send EN_CHANGE and EN_SELCHANGE events during undetermined state.
    /// </summary>
    ImeAlwaysSendNotify = 0x0008,

    /// <summary>
    ///  This flag determines how the control uses the composition string of an
    ///  IME if the user cancels it. If this flag is set, the control discards
    ///  the composition string. If this flag is not set, the control uses the
    ///  composition string as the result string.
    /// </summary>
    ImeCancelComplete = 0x0004,

    /// <summary>
    ///  Use UI default fonts. This option is turned off by default.
    /// </summary>
    UIFonts = 0x0020
}
