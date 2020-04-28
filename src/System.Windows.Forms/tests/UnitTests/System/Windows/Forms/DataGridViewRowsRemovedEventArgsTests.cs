// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewRowsRemovedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int rowIndex, int rowCount)
        {
            var e = new DataGridViewRowsRemovedEventArgs(rowIndex, rowCount);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(rowCount, e.RowCount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewRowsRemovedEventArgs(rowIndex, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Ctor_NegativeRowCount_ThrowsArgumentOutOfRangeException(int rowCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowCount", () => new DataGridViewRowsRemovedEventArgs(1, rowCount));
        }
    }
}
