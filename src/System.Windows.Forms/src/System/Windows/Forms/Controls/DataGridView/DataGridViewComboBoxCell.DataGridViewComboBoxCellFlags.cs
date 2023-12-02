// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxCell
{
    [Flags]
    private enum DataGridViewComboBoxCellFlags : byte
    {
        IgnoreNextMouseClick = 0x01,
        CellSorted = 0x02,
        CellCreateItemsFromDataSource = 0x04,
        CellAutoComplete = 0x08,
        DataSourceInitializedHookedUp = 0x10,
        DropDownHookedUp = 0x20
    }
}
