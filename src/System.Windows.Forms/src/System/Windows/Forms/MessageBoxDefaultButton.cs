// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;


namespace System.Windows.Forms {

    /// <include file='doc\MessageBoxDefaultButton.uex' path='docs/doc[@for="MessageBoxDefaultButton"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum MessageBoxDefaultButton {
        /// <include file='doc\MessageBoxDefaultButton.uex' path='docs/doc[@for="MessageBoxDefaultButton.Button1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the first
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button1       = 0x00000000,
        /// <include file='doc\MessageBoxDefaultButton.uex' path='docs/doc[@for="MessageBoxDefaultButton.Button2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the second
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button2       = 0x00000100,

        /// <include file='doc\MessageBoxDefaultButton.uex' path='docs/doc[@for="MessageBoxDefaultButton.Button3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the third
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button3       = 0x00000200,
    }
}

