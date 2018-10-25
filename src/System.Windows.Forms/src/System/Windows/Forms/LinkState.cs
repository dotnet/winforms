// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;

    /// <include file='doc\LinkState.uex' path='docs/doc[@for="LinkState"]/*' />
    /// <internalonly />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum LinkState {
        /// <include file='doc\LinkState.uex' path='docs/doc[@for="LinkState.Normal"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Normal =  0x00,
        /// <include file='doc\LinkState.uex' path='docs/doc[@for="LinkState.Hover"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Hover  =  0x01,
        /// <include file='doc\LinkState.uex' path='docs/doc[@for="LinkState.Active"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Active =  0x02,
        /// <include file='doc\LinkState.uex' path='docs/doc[@for="LinkState.Visited"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Visited = 0x04
    }
}
