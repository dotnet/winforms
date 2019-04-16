﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <summary>
    /// Identifies a cell in the grid.
    /// </summary>    
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct DataGridCell
    {
        /// <summary>
        /// Gets or sets the number of a column in the <see cref='System.Windows.Forms.DataGrid'/> control.
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of a row in the <see cref='System.Windows.Forms.DataGrid'/> control.
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.DataGridCell'/> class.
        /// </summary>
        public DataGridCell(int r, int c)
        {
            RowNumber = r;
            ColumnNumber = c;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref='System.Windows.Forms.DataGridCell'/> is
        /// identical to a second <see cref='System.Windows.Forms.DataGridCell'/>.
        /// </summary>        
        [SuppressMessage("Microsoft.Usage", "CA2231:OverrideOperatorEqualsOnOverridingValueTypeEquals")]
        public override bool Equals(object o)
        {
            if (!(o is DataGridCell rhs))
            {
                return false;
            }

            return rhs.RowNumber == RowNumber && rhs.ColumnNumber == ColumnNumber;
        }

        /// <summary>
        /// Gets a hash value that uniquely identifies the cell.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(RowNumber, ColumnNumber);

        /// <summary>
        /// Gets the row number and column number of the cell.
        /// </summary>
        public override string ToString()
        {
            return "DataGridCell {RowNumber = " + RowNumber + ", ColumnNumber = " + ColumnNumber + "}";
        }
    }
}
