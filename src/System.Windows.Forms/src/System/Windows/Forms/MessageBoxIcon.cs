// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;



namespace System.Windows.Forms {
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum MessageBoxIcon {
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contain no symbols. 
        ///    </para>
        /// </devdoc>
        None         = 0,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand symbol. 
        ///    </para>
        /// </devdoc>
        Hand         = 0x00000010,

        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       that the message
        ///       box contains a question
        ///       mark symbol. 
        ///    </para>
        /// </devdoc>
        Question     = 0x00000020,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       exclamation symbol. 
        ///    </para>
        /// </devdoc>
        Exclamation  = 0x00000030,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       asterisk symbol. 
        ///    </para>
        /// </devdoc>
        Asterisk     = 0x00000040,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains a hand icon. This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        Stop         = Hand,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand icon. 
        ///    </para>
        /// </devdoc>
        Error        = Hand,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains an exclamation icon. 
        ///    </para>
        /// </devdoc>
        Warning      = Exclamation,

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

