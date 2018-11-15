// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
	
    /// <include file='doc\ToolStripItemDisplayStyle.uex' path='docs/doc[@for="ToolStripItemDisplayStyle"]/*' />
    /// <devdoc>
    /// Specifies what to render for the ToolStripItem
    /// </devdoc>
    public enum ToolStripItemDisplayStyle {
       /// <include file='doc\ToolStripItemDisplayStyle.uex' path='docs/doc[@for="ToolStripItemDisplayStyle.None"]/*' />
       None                     = 0x0000,       
       /// <include file='doc\ToolStripItemDisplayStyle.uex' path='docs/doc[@for="ToolStripItemDisplayStyle.Text"]/*' />
       Text                     = 0x0001, // 0001
       /// <include file='doc\ToolStripItemDisplayStyle.uex' path='docs/doc[@for="ToolStripItemDisplayStyle.Image"]/*' />
       Image                    = 0x0002, // 0010
       /// <include file='doc\ToolStripItemDisplayStyle.uex' path='docs/doc[@for="ToolStripItemDisplayStyle.ImageAndText"]/*' />
       ImageAndText             = 0x0003, // 0011
    }
}
