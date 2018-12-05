// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;



namespace System.Windows.Forms {
    /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum MessageBoxIcon {
        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contain no symbols. 
        ///    </para>
        /// </devdoc>
        None         = 0,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Hand"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand symbol. 
        ///    </para>
        /// </devdoc>
        Hand         = 0x00000010,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Question"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       that the message
        ///       box contains a question
        ///       mark symbol. 
        ///    </para>
        /// </devdoc>
        Question     = 0x00000020,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Exclamation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       exclamation symbol. 
        ///    </para>
        /// </devdoc>
        Exclamation  = 0x00000030,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Asterisk"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       asterisk symbol. 
        ///    </para>
        /// </devdoc>
        Asterisk     = 0x00000040,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Stop"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains a hand icon. This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        Stop         = Hand,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Error"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand icon. 
        ///    </para>
        /// </devdoc>
        Error        = Hand,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Warning"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains an exclamation icon. 
        ///    </para>
        /// </devdoc>
        Warning      = Exclamation,

        /// <include file='doc\MessageBoxIcon.uex' path='docs/doc[@for="MessageBoxIcon.Information"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       asterisk icon. 
        ///    </para>
        /// </devdoc>
        Information  = Asterisk,
    }
}

