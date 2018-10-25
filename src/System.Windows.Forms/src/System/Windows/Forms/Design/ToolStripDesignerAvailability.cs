// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design {
using System.Diagnostics.CodeAnalysis;
    [Flags()]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]     // PM reviewed the enum name
    public enum ToolStripItemDesignerAvailability {
        None = 0x00000000, 
        ToolStrip = 0x00000001, 
        MenuStrip = 0x00000002, 
        ContextMenuStrip = 0x00000004, 
        StatusStrip = 0x0000008,
        All = ToolStrip | MenuStrip | ContextMenuStrip | StatusStrip
    }
}
 

