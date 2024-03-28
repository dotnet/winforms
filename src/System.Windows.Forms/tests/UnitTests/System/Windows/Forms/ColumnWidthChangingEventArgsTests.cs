// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ColumnWidthChangingEventArgsTests
{
    [Theory]
    [InlineData(-1, -1, true)]
    [InlineData(0, 0, false)]
    [InlineData(1, 2, true)]
    public void Ctor_Int_Int_Bool(int columnIndex, int newWidth, bool cancel)
    {
        ColumnWidthChangingEventArgs e = new(columnIndex, newWidth, cancel);
        Assert.Equal(columnIndex, e.ColumnIndex);
        Assert.Equal(newWidth, e.NewWidth);
        Assert.Equal(cancel, e.Cancel);
    }

    [Theory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void Ctor_Int_Int(int columnIndex, int newWidth)
    {
        ColumnWidthChangingEventArgs e = new(columnIndex, newWidth);
        Assert.Equal(columnIndex, e.ColumnIndex);
        Assert.Equal(newWidth, e.NewWidth);
        Assert.False(e.Cancel);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void NewWidth_Set_GetReturnsExpected(int value)
    {
        ColumnWidthChangingEventArgs e = new(2, 3)
        {
            NewWidth = value
        };
        Assert.Equal(value, e.NewWidth);
    }
}
