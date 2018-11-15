// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
	
  
    internal enum ToolStripItemStates {
        None     = 0x00000000,
        Selected = 0x00000001, 
        Focused  = 0x00000002,
        Hot      = 0x00000004,
        Pressed  = 0x00000008,
        Disabled = 0x00000010
    }
}
