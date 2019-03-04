// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowsAddedEventArgsTests
    {
        [Theory]
        [InlineData(-2, -2)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int rowIndex, int rowCount)
        {
            var e = new DataGridViewRowsAddedEventArgs(rowIndex, rowCount);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(rowCount, e.RowCount);
        }
    }
}
