// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [Flags]
    public enum DataGridViewElementStates
    {
        None = 0x0000,
        Displayed = 0x0001,
        Frozen = 0x0002,
        ReadOnly = 0x0004,
        Resizable = 0x0008,
        ResizableSet = 0x0010,
        Selected = 0x0020,
        Visible = 0x0040
    }
}
