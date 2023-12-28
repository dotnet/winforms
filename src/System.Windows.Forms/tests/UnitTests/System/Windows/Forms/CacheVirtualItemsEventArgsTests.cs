// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class CacheVirtualItemsEventArgsTests
{
    [Theory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void Ctor_Int_Int(int startIndex, int endIndex)
    {
        CacheVirtualItemsEventArgs e = new(startIndex, endIndex);
        Assert.Equal(startIndex, e.StartIndex);
        Assert.Equal(endIndex, e.EndIndex);
    }
}
