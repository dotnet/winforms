// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.ComponentModel;
    
    /// <include file='doc\ToolStripManagerRenderMode.uex' path='docs/doc[@for="ToolStripManagerRenderMode"]/*' />
    public enum ToolStripManagerRenderMode {
        /// <include file='doc\ToolStripManagerRenderMode.uex' path='docs/doc[@for="ToolStripManagerRenderMode.Custom"]/*' />
        [Browsable(false)]
        Custom = ToolStripRenderMode.Custom, 
        /// <include file='doc\ToolStripManagerRenderMode.uex' path='docs/doc[@for="ToolStripManagerRenderMode.System"]/*' />
        System = ToolStripRenderMode.System,
        /// <include file='doc\ToolStripManagerRenderMode.uex' path='docs/doc[@for="ToolStripManagerRenderMode.Professional"]/*' />
        Professional = ToolStripRenderMode.Professional
    }
   

}
