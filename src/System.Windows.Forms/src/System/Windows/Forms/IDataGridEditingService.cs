// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    
    /// <include file='doc\IDataGridEditingService.uex' path='docs/doc[@for="IDataGridEditingService"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>The DataGrid exposes hooks to request editing commands
    ///       via this interface.</para>
    /// </devdoc>
    public interface IDataGridEditingService {
        /// <include file='doc\IDataGridEditingService.uex' path='docs/doc[@for="IDataGridEditingService.BeginEdit"]/*' />
        bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber);
        
        /// <include file='doc\IDataGridEditingService.uex' path='docs/doc[@for="IDataGridEditingService.EndEdit"]/*' />
        bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort);
    }
}
