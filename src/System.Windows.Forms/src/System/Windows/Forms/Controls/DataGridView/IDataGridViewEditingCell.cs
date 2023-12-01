// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public interface IDataGridViewEditingCell
{
    object? EditingCellFormattedValue
    {
        get;
        set;
    }

    bool EditingCellValueChanged
    {
        get;
        set;
    }

    object? GetEditingCellFormattedValue(DataGridViewDataErrorContexts context);

    void PrepareEditingCellForEdit(bool selectAll);
}
