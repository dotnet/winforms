// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    
    internal interface ISupportToolStripPanel {
        ToolStripPanelRow ToolStripPanelRow {
            get; set;
        }
        
        ToolStripPanelCell ToolStripPanelCell {
            get;
        }
        
        
        bool Stretch {
            get; set;
        }

        bool IsCurrentlyDragging {
            get;
        }

        void BeginDrag();
        
        void EndDrag();
        
    }
}


