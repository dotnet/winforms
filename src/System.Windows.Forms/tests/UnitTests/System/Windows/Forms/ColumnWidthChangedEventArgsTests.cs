﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ColumnWidthChangedEventArgsTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Ctor_Int(int columnIndex)
    {
        var e = new ColumnWidthChangedEventArgs(columnIndex);
        Assert.Equal(columnIndex, e.ColumnIndex);
    }
}
