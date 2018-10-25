// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <include file='doc\DataGridViewEditingCell.uex' path='docs/doc[@for="IDataGridViewEditingCell"]/*' />
    public interface IDataGridViewEditingCell
    {
        /// <include file='doc\DataGridViewEditingCell.uex' path='docs/doc[@for="IDataGridViewEditingCell.EditingCellFormattedValue"]/*' />        
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object EditingCellFormattedValue
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingCell.uex' path='docs/doc[@for="IDataGridViewEditingCell.EditingCellValueChanged"]/*' />
        bool EditingCellValueChanged
        {
            get;
            set;
        }

        /// <include file='doc\DataGridViewEditingCell.uex' path='docs/doc[@for="IDataGridViewEditingCell.GetEditingCellFormattedValue"]/*' />        
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context);

        /// <include file='doc\DataGridViewEditingCell.uex' path='docs/doc[@for="IDataGridViewEditingCell.PrepareEditingCellForEdit"]/*' />
        void PrepareEditingCellForEdit(bool selectAll);
    }
}
