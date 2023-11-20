// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    [Flags]
    private enum Flags : ushort
    {
        PropertiesChanged          = 0x0001,
        GotDesignerEventService    = 0x0002,
        InternalChange             = 0x0004,
        TabsChanging               = 0x0008,
        BatchMode                  = 0x0010,
        ReInitTab                  = 0x0020,
        SysColorChangeRefresh      = 0x0040,
        FullRefreshAfterBatch      = 0x0080,
        BatchModeChange            = 0x0100,
        RefreshingProperties       = 0x0200
    }
}
