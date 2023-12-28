// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowsAddedEventArgsTests
{
    [Theory]
    [InlineData(-2, -2)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void Ctor_Int_Int(int rowIndex, int rowCount)
    {
        DataGridViewRowsAddedEventArgs e = new(rowIndex, rowCount);
        Assert.Equal(rowIndex, e.RowIndex);
        Assert.Equal(rowCount, e.RowCount);
    }
}
