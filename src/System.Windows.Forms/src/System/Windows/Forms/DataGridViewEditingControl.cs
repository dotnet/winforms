// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public interface IDataGridViewEditingControl
    {
        DataGridView EditingControlDataGridView
        {
            get;
            set;
        }

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
}
