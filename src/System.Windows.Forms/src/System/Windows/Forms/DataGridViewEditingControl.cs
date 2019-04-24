// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl"]/*' />
    public interface IDataGridViewEditingControl
    {
        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingControlDataGridView"]/*' />
        DataGridView EditingControlDataGridView
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingControlFormattedValue"]/*' />
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object EditingControlFormattedValue
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingControlRowIndex"]/*' />
        int EditingControlRowIndex
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingControlValueChanged"]/*' />
        bool EditingControlValueChanged
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingPanelCursor"]/*' />
        Cursor EditingPanelCursor
        {
            get;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.RepositionEditingControlOnValueChange"]/*' />
        bool RepositionEditingControlOnValueChange
        {
            get;
        }

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.ApplyCellStyleToEditingControl"]/*' />
        void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle);

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.EditingControlWantsInputKey"]/*' />
        bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey);

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.GetEditingControlFormattedValue"]/*' />
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context);

        /// <include file='doc\DataGridViewEditingControl.uex' path='docs/doc[@for="IDataGridViewEditingControl.PrepareEditingControlForEdit"]/*' />
        void PrepareEditingControlForEdit(bool selectAll);
    }
}
