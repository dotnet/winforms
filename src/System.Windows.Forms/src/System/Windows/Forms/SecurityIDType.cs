// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    
    using System;

    /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType"]/*' />
    /// <internalonly/>
    /// <devdoc>
    /// </devdoc>
    public enum SecurityIDType {
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.User"]/*' />
        User           = 1,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Group"]/*' />
        Group          = 2,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Domain"]/*' />
        Domain         = 3,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Alias"]/*' />
        Alias          = 4,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.WellKnownGroup"]/*' />
        WellKnownGroup = 5,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.DeletedAccount"]/*' />
        DeletedAccount = 6,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Invalid"]/*' />
        Invalid        = 7,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Unknown"]/*' />
        Unknown        = 8,
        /// <include file='doc\SecurityIDType.uex' path='docs/doc[@for="SecurityIDType.Computer"]/*' />
        Computer       = 9,
    }
}
