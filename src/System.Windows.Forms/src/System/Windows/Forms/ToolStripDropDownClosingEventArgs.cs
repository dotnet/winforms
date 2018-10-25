// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.ComponentModel;

    // 


        
    public class ToolStripDropDownClosingEventArgs : CancelEventArgs {

        ToolStripDropDownCloseReason closeReason;
    
        public ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason reason) {
            closeReason = reason;
        }
        
 
        public ToolStripDropDownCloseReason CloseReason {
            get { return closeReason; }
        }
        // TBD: CloseReason
     }
}
