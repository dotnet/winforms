// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;

    // Used with ImageList.
    /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth"]/*' />
    public enum ColorDepth {
        /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth.Depth4Bit"]/*' />
        Depth4Bit         = 4,
        /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth.Depth8Bit"]/*' />
        Depth8Bit         = 8,
        /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth.Depth16Bit"]/*' />
        Depth16Bit        = 16,
        /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth.Depth24Bit"]/*' />
        Depth24Bit        = 24,
        /// <include file='doc\ColorDepth.uex' path='docs/doc[@for="ColorDepth.Depth32Bit"]/*' />
        Depth32Bit        = 32,
    }
}
