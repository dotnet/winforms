// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum DataGridViewDataErrorContexts
    {
        Formatting = 0x0001,
        Display = 0x0002,
        PreferredSize = 0x0004,
        RowDeletion = 0x0008,
        Parsing = 0x0100,
        Commit = 0x0200,
        InitialValueRestoration = 0x0400,
        LeaveControl = 0x0800,
        CurrentCellChange = 0x1000,
        Scroll = 0x2000,
        ClipboardContent = 0x4000
    }
}
