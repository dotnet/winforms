// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Describes the times that the error icon supplied by an ErrorProvider
    ///  should blink to alert the user that an error has occurred.
    /// </summary>
    public enum ErrorBlinkStyle
    {
        /// <summary>
        ///  Blink only if the error icon is already displayed, but a new
        ///  error string is set for the control.  If the icon did not blink
        ///  in this case, the user might not know that there is a new error.
        /// </summary>
        BlinkIfDifferentError,

        /// <summary>
        ///  Blink the error icon when the error is first displayed, or when
        ///  a new error description string is set for the control and the
        ///  error icon is already displayed.
        /// </summary>
        AlwaysBlink,

        /// <summary>
        ///  Never blink the error icon.
        /// </summary>
        NeverBlink
    }
}
