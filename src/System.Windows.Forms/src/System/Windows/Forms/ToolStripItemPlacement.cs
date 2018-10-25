// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
	
    /// <include file='doc\ToolStripItemPlacement.uex' path='docs/doc[@for="ToolStripItemPlacement"]/*' />
    /// <devdoc>
    /// ToolStripItemPlacement
    /// This enum represents the current layout of the ToolStripItem. 
    /// </devdoc>
    public enum ToolStripItemPlacement {
        /// <include file='doc\ToolStripItemPlacement.uex' path='docs/doc[@for="ToolStripItemPlacement.Main"]/*' />
        Main,          // in the main winbar itself
        /// <include file='doc\ToolStripItemPlacement.uex' path='docs/doc[@for="ToolStripItemPlacement.Overflow"]/*' />
        Overflow,      // in the overflow window
        /// <include file='doc\ToolStripItemPlacement.uex' path='docs/doc[@for="ToolStripItemPlacement.None"]/*' />
        None           // either offscreen or visible == false so we didn't lay it out
    }
}

