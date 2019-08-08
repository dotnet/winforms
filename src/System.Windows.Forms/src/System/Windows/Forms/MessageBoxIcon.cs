// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum MessageBoxIcon
    {
        /// <summary>
        ///  Specifies that the message box contain no symbols.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Specifies that the message box contains a hand symbol.
        /// </summary>
        Hand = 0x00000010,

        /// <summary>
        ///  Specifies that the message box contains a question mark symbol.
        /// </summary>
        Question = 0x00000020,

        /// <summary>
        ///  Specifies that the message box contains an exclamation symbol.
        /// </summary>
        Exclamation = 0x00000030,

        /// <summary>
        ///  Specifies that the message box contains an asterisk symbol.
        /// </summary>
        Asterisk = 0x00000040,

        /// <summary>
        ///  Specifies that the message box contains a hand icon. This field is
        ///  constant.
        /// </summary>
        Stop = Hand,

        /// <summary>
        ///  Specifies that the message box contains a hand icon.
        /// </summary>
        Error = Hand,

        /// <summary>
        ///  Specifies that the message box contains an exclamation icon.
        /// </summary>
        Warning = Exclamation,

        /// <summary>
        ///  Specifies that the message box contains an asterisk icon.
        /// </summary>
        Information = Asterisk,
    }
}
