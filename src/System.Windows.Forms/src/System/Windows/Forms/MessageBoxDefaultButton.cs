// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;


namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum MessageBoxDefaultButton {
        /// <devdoc>
        ///    <para>
        ///       Specifies that the first
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button1       = 0x00000000,
        /// <devdoc>
        ///    <para>
        ///       Specifies that the second
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button2       = 0x00000100,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the third
        ///       button on the message box should be the default button. 
        ///    </para>
        /// </devdoc>
        Button3       = 0x00000200,
    }
}

