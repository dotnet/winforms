// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataStreamFromComStreamTests
{
    [Theory,
        InlineData(0, 0, 1),
        InlineData(1, 1, 1)]
    public unsafe void Write_ThrowsInvalidCount(int bufferSize, int index, int count)
    {
        using MemoryStream memoryStream = new();
        using var stream = memoryStream.ToIStream();
        using DataStreamFromComStream dataStream = new(stream);
        Assert.Throws<IOException>(() => dataStream.Write(new byte[bufferSize], index, count));
    }

    [Theory,
        InlineData(0, 0, 0),
        InlineData(0, 0, -1),
        InlineData(1, 1, 0),
        InlineData(1, 1, -1)]
    public unsafe void Write_DoesNotThrowCountZeroOrLess(int bufferSize, int index, int count)
    {
        using MemoryStream memoryStream = new();
        using var stream = memoryStream.ToIStream();
        using DataStreamFromComStream dataStream = new(stream);
        dataStream.Write(new byte[bufferSize], index, count);
    }
}
