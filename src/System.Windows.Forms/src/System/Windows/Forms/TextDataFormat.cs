// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat"]/*' />
    /// <devdoc>
    ///    <para>Specifies the formats that can be used with Clipboard.GetText and Clipboard.SetText methods</para>
    /// </devdoc>
    public enum TextDataFormat {
        /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat.Text"]/*' />
        Text,
        /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat.UnicodeText"]/*' />
        UnicodeText,
        /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat.Rtf"]/*' />
        Rtf,
        /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat.Html"]/*' />
        Html,
        /// <include file='doc\TextDataFormat.uex' path='docs/doc[@for="TextDataFormat.CommaSeparatedValue"]/*' />
        CommaSeparatedValue
    }
}

