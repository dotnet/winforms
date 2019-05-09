// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    public interface IDataGridViewEditingControl
    {
        DataGridView EditingControlDataGridView
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
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

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context);

        void PrepareEditingControlForEdit(bool selectAll);
    }
}
