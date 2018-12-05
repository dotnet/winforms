// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    /// <include file='doc\MessageBoxOptions.uex' path='docs/doc[@for="MessageBoxOptions"]/*' />
    [Flags]
    public enum MessageBoxOptions {
        /// <include file='doc\MessageBoxOptions.uex' path='docs/doc[@for="MessageBoxOptions.ServiceNotification"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box is displayed on the active desktop. 
        ///    </para>
        /// </devdoc>
        ServiceNotification = 0x00200000,

        /// <include file='doc\MessageBoxOptions.uex' path='docs/doc[@for="MessageBoxOptions.DefaultDesktopOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box is displayed on the active desktop. 
        ///    </para>
        /// </devdoc>
        DefaultDesktopOnly = 0x00020000,

        /// <include file='doc\MessageBoxOptions.uex' path='docs/doc[@for="MessageBoxOptions.RightAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box text is right-aligned.
        ///    </para>
        /// </devdoc>
        RightAlign         = 0x00080000,

        /// <include file='doc\MessageBoxOptions.uex' path='docs/doc[@for="MessageBoxOptions.RtlReading"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box text is displayed with Rtl reading order.
        ///    </para>
        /// </devdoc>
        RtlReading         = 0x00100000,
    }
}

