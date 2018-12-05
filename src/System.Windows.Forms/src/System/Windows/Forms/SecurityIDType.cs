// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    
    using System;

    /// <internalonly/>
    /// <devdoc>
    /// </devdoc>
    public enum SecurityIDType {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        User           = 1,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Group          = 2,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Domain         = 3,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Alias          = 4,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        WellKnownGroup = 5,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DeletedAccount = 6,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Invalid        = 7,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unknown        = 8,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Computer       = 9,
    }
}
