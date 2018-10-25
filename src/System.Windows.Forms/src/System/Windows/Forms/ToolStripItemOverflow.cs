// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
	
	
    /// <include file='doc\ToolStripItemOverflow.uex' path='docs/doc[@for="ToolStripItemOverflow"]/*' />
    /// <devdoc>
    /// Summary of ToolStripItemOverflow.
    /// This enum is used to determine placement of the ToolStripItem on the ToolStrip.
    /// </devdoc>
    public enum ToolStripItemOverflow {
        /// <include file='doc\ToolStripItemOverflow.uex' path='docs/doc[@for="ToolStripItemOverflow.Never"]/*' />
        Never,		// on the main winbar itself,
        /// <include file='doc\ToolStripItemOverflow.uex' path='docs/doc[@for="ToolStripItemOverflow.Always"]/*' />
        Always,		// on the overflow window
        /// <include file='doc\ToolStripItemOverflow.uex' path='docs/doc[@for="ToolStripItemOverflow.AsNeeded // DEFAULT try for main"]/*' />
        AsNeeded	// DEFAULT try for main, overflow as necessary
    }
}
