//------------------------------------------------------------------------------
// <copyright file="DisplayMode.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ComponentModel.Design {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\DisplayMode.uex' path='docs/doc[@for="DisplayMode"]/*' />
    /// <devdoc>
    ///    <para>Specifies identifiers to indicate the display modes used 
    ///       by <see cref='System.ComponentModel.Design.ByteViewer'/>.</para>
    /// </devdoc>
    public enum DisplayMode {

        /// <include file='doc\DisplayMode.uex' path='docs/doc[@for="DisplayMode.Hexdump"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates using
        ///       Hexadecimal format.
        ///    </para>
        /// </devdoc>
        Hexdump = 1,
        /// <include file='doc\DisplayMode.uex' path='docs/doc[@for="DisplayMode.Ansi"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates using ANSI format.
        ///    </para>
        /// </devdoc>
        Ansi    = 2,
        /// <include file='doc\DisplayMode.uex' path='docs/doc[@for="DisplayMode.Unicode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates using Unicode format.
        ///    </para>
        /// </devdoc>
        Unicode = 3,
        /// <include file='doc\DisplayMode.uex' path='docs/doc[@for="DisplayMode.Auto"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates using automatic format selection.
        ///    </para>
        /// </devdoc>
        Auto    = 4,

    }
}
