// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewGroupEventArgsTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Ctor_Int(int groupIndex)
    {
        ListViewGroupEventArgs args = new(groupIndex);
        Assert.Equal(groupIndex, args.GroupIndex);
    }
}
