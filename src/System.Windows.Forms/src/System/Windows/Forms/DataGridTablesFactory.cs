// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Text;
    using System.Runtime.Remoting;

    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <include file='doc\DataGridTablesFactory.uex' path='docs/doc[@for="GridTablesFactory"]/*' />
    public sealed class GridTablesFactory {
        // private static DataTableComparer dtComparer = new DataTableComparer();

        // not creatable...
        //
        private GridTablesFactory() {
        }


        /// <include file='doc\DataGridTablesFactory.uex' path='docs/doc[@for="GridTablesFactory.CreateGridTables"]/*' />
        /// <devdoc>
        ///      Takes a DataView and creates an intelligent mapping of
        ///      DataView storage types into available DataColumn types.
        /// </devdoc>
        public static DataGridTableStyle[]
            CreateGridTables(DataGridTableStyle gridTable, object dataSource, string dataMember, BindingContext bindingManager)
        {
            return new DataGridTableStyle[] {gridTable};
        }
    }
}
