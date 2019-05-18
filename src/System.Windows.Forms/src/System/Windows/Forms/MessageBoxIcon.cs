// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum MessageBoxIcon
    {
        /// <summary>
        /// Specifies that the message box contain no symbols.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// Specifies that the message box contains a hand symbol.
        /// </devdoc>
        Hand = 0x00000010,

        /// <summary>
        /// Specifies that the message box contains a question mark symbol.
        /// </devdoc>
        Question = 0x00000020,

        /// <summary>
        /// Specifies that the message box contains an exclamation symbol.
        /// </devdoc>
        Exclamation  = 0x00000030,

        /// <summary>
        /// Specifies that the message box contains an asterisk symbol.
        /// </devdoc>
        Asterisk = 0x00000040,

        /// <summary>
        /// Specifies that the message box contains a hand icon. This field is
        /// constant.
        /// </devdoc>
        Stop = Hand,

        /// <summary>
        /// Specifies that the message box contains a hand icon.
        /// </devdoc>
        Error = Hand,

        /// <summary>
        /// Specifies that the message box contains an exclamation icon.
        /// </devdoc>
        Warning = Exclamation,

        /// <summary>
        /// Specifies that the message box contains an asterisk icon.
        /// </devdoc>
        Information  = Asterisk,
    }
}
