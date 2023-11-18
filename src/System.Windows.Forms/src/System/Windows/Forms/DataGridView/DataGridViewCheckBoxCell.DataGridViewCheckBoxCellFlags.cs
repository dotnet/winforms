// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridViewCheckBoxCell
{
    [Flags]
    private enum DataGridViewCheckBoxCellFlags : byte
    {
        ThreeState = 0x01,
        ValueChanged = 0x02,
        Checked = 0x10,
        Indeterminate = 0x20
    }
}
