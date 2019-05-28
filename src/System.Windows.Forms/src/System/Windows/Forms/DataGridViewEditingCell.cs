// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IDataGridViewEditingCell
    {
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object EditingCellFormattedValue
        {
            get;
            set;
        }

        bool EditingCellValueChanged
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context);

        void PrepareEditingCellForEdit(bool selectAll);
    }
}
