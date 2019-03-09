// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Defines the possible kinds of input/output streams used by RichTextBox
    /// control's load/save mechansim. These stream options are also used the
    /// control's text modification methods.
    ///
    /// </devdoc>
    public enum RichTextBoxStreamType
    {
        /// <devdoc>
        /// Rich Text Format (RTF).
        /// </devdoc>
        RichText = 0,

        /// <devdoc>
        /// Text with spaces in place of OLE objects.
        /// </devdoc>
        PlainText = 1,

        /// <devdoc>
        /// RTF with spaces in place of OLE object (valid only for saveFile).
        /// </devdoc>
        RichNoOleObjs = 2,

        /// <devdoc>
        /// Text with a text representation of OLE objects (valid only for saveFile).
        /// </devdoc>
        TextTextOleObjs = 3,

        /// <devdoc>
        /// Text with spaces in place of OLE objects, encoded in Unicode.
        /// </devdoc>
        UnicodePlainText = 4,
    }
}
