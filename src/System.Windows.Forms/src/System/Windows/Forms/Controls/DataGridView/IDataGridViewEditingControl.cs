// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public interface IDataGridViewEditingControl
{
    DataGridView? EditingControlDataGridView
    {
        get;
        set;
    }

    [AllowNull]
    object EditingControlFormattedValue
    {
        get;
        set;
    }

    int EditingControlRowIndex
    {
        get;
        set;
    }

    bool EditingControlValueChanged
    {
        get;
        set;
    }

    Cursor EditingPanelCursor
    {
        get;
    }

    bool RepositionEditingControlOnValueChange
    {
        get;
    }

    void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle);

    bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey);

    object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context);

    void PrepareEditingControlForEdit(bool selectAll);
}
