// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellMouseEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Int_Int_Int_Int_MouseEventArgs_TestData()
        {
            yield return new object[] { -1, -1, -1, -1, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
            yield return new object[] { 0, 0, 0, 0, new MouseEventArgs((MouseButtons)1, 0, 0, 0, 0) };
            yield return new object[] { 1, 2, 3, 4, new MouseEventArgs((MouseButtons)3, -1, -1, -1, -2) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Int_Int_Int_Int_MouseEventArgs_TestData))]
        public void Ctor_Int_Int_Int_Int_MouseEventArgs(int columnIndex, int rowIndex, int localX, int localY, MouseEventArgs mouseE)
        {
            var e = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, localX, localY, mouseE);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(mouseE.Button, e.Button);
            Assert.Equal(mouseE.Clicks, e.Clicks);
            Assert.Equal(localX, e.X);
            Assert.Equal(localY, e.Y);
            Assert.Equal(mouseE.Delta, e.Delta);
        }

        [Fact]
        public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellMouseEventArgs(-2, 0, 1, 2, null));
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellMouseEventArgs(-2, 0, 1, 2, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4)));
        }

        [Fact]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellMouseEventArgs(0, -2, 1, 2, null));
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellMouseEventArgs(0, -2, 1, 2, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4)));
        }

        [Fact]
        public void Ctor_NullE_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("e", () => new DataGridViewCellMouseEventArgs(1, 2, 1, 2, null));
        }
    }
}
