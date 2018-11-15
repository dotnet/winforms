// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms;
    using System.ComponentModel;

    /// <include file='doc\GetChildAtPointSkip.uex' path='docs/doc[@for="GetChildAtPointSkip"]/*' />
    [
    Flags,
    ]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]     // PM reviewed the enum name
    public enum GetChildAtPointSkip {
        /// <include file='doc\GetChildAtPointSkip.uex' path='docs/doc[@for="GetChildAtPointSkip.None"]/*' />
        None = 0x0000,
        /// <include file='doc\GetChildAtPointSkip.uex' path='docs/doc[@for="GetChildAtPointSkip.Invisible"]/*' />
        Invisible = 0x0001,         
        /// <include file='doc\GetChildAtPointSkip.uex' path='docs/doc[@for="GetChildAtPointSkip.Disabled"]/*' />
        Disabled = 0x0002,   
        /// <include file='doc\GetChildAtPointSkip.uex' path='docs/doc[@for="GetChildAtPointSkip.Transparent"]/*' />
        Transparent = 0x0004
    }
}

