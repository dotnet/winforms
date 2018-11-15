// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Globalization;
    
    /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell"]/*' />
    /// <devdoc>
    ///    <para>Identifies a cell in the grid.</para>
    /// </devdoc>    
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct DataGridCell {
        private int rowNumber;
        private int columnNumber;

        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.ColumnNumber"]/*' />
        /// <devdoc>
        /// <para>Gets or sets the number of a column in the <see cref='System.Windows.Forms.DataGrid'/> control.</para>
        /// </devdoc>
        public int ColumnNumber {
            get {
                return columnNumber;
            }
            set {
                columnNumber = value;
            }
        }
        
        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.RowNumber"]/*' />
        /// <devdoc>
        /// <para>Gets or sets the number of a row in the <see cref='System.Windows.Forms.DataGrid'/> control.</para>
        /// </devdoc>
        public int RowNumber {
            get {
                return rowNumber;
            }
            set {
                rowNumber = value;
            }
        }                
        
        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.DataGridCell"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridCell'/> class.
        ///    </para>
        /// </devdoc>
        public DataGridCell(int r, int c) {
            this.rowNumber = r;
            this.columnNumber = c;
        }
        
        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.Equals"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the <see cref='System.Windows.Forms.DataGridCell'/> is identical to a second
        ///    <see cref='System.Windows.Forms.DataGridCell'/>.
        ///    </para>
        /// </devdoc>        
        [SuppressMessage("Microsoft.Usage", "CA2231:OverrideOperatorEqualsOnOverridingValueTypeEquals")]
        public override bool Equals(object o) {
            if (o is DataGridCell) {
                DataGridCell rhs = (DataGridCell)o;
                return (rhs.RowNumber == RowNumber && rhs.ColumnNumber == ColumnNumber);
            }
            else
                return false;
        }
        
        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.GetHashCode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       a hash value that uniquely identifies the cell.
        ///    </para>
        /// </devdoc>
        public override int GetHashCode() {
            return ((~rowNumber * (columnNumber+1)) & 0x00ffff00) >> 8;
       }

        /// <include file='doc\DataGridCell.uex' path='docs/doc[@for="DataGridCell.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the row number and column number of the cell.
        ///    </para>
        /// </devdoc>
        public override string ToString() {
            return "DataGridCell {RowNumber = " + RowNumber.ToString(CultureInfo.CurrentCulture) + 
                   ", ColumnNumber = " + ColumnNumber.ToString(CultureInfo.CurrentCulture) + "}";
        }
        
    }
                                                                                        
}
