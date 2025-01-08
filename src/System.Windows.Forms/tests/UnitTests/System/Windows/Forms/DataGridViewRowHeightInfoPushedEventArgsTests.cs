// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

public class DataGridViewRowHeightInfoPushedEventArgsTests
{
    [Theory]
    [InlineData(1, 20, 10)]
    [InlineData(0, 20, 10)]
    [InlineData(1, 0, 10)]
    [InlineData(1, 20, 0)]
    [InlineData(-1, 20, 10)]
    [InlineData(1, 20, 5)]
    public void Ctor_ValidArguments_SetsProperties(int rowIndex, int height, int minimumHeight)
    {
        DataGridViewRowHeightInfoPushedEventArgs e = new(rowIndex, height, minimumHeight);

        e.RowIndex.Should().Be(rowIndex);
        e.Height.Should().Be(height);
        e.MinimumHeight.Should().Be(minimumHeight);
    }
}
