// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    /// <include file='doc\ErrorBlinkStyle.uex' path='docs/doc[@for="ErrorBlinkStyle"]/*' />
    /// <devdoc>
    ///     Describes the times that the error icon supplied by an ErrorProvider
    ///     should blink to alert the user that an error has occurred.
    /// </devdoc>
    public enum ErrorBlinkStyle {
        /// <include file='doc\ErrorBlinkStyle.uex' path='docs/doc[@for="ErrorBlinkStyle.BlinkIfDifferentError"]/*' />
        /// <devdoc>
        ///     Blink only if the error icon is already displayed, but a new
        ///     error string is set for the control.  If the icon did not blink 
        ///     in this case, the user might not know that there is a new error.
        /// </devdoc>
        BlinkIfDifferentError,
        
        /// <include file='doc\ErrorBlinkStyle.uex' path='docs/doc[@for="ErrorBlinkStyle.AlwaysBlink"]/*' />
        /// <devdoc>
        ///     Blink the error icon when the error is first displayed, or when 
        ///     a new error description string is set for the control and the
        ///     error icon is already displayed.
        /// </devdoc>
        AlwaysBlink,
        
        /// <include file='doc\ErrorBlinkStyle.uex' path='docs/doc[@for="ErrorBlinkStyle.NeverBlink"]/*' />
        /// <devdoc>
        ///     Never blink the error icon.
        /// </devdoc>
        NeverBlink
    }
}
