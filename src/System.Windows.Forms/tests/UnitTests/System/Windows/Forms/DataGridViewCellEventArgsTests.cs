// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewCellEventArgsTests
{
    [Theory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void Ctor_Int_Int(int columnIndex, int rowIndex)
    {
        DataGridViewCellEventArgs e = new(columnIndex, rowIndex);
        Assert.Equal(columnIndex, e.ColumnIndex);
        Assert.Equal(rowIndex, e.RowIndex);
    }

    [Fact]
    public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellEventArgs(-2, 0));
    }

    [Fact]
    public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellEventArgs(0, -2));
    }
}
