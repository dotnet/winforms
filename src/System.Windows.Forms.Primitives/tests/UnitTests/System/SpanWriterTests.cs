// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System;

public class SpanWriterTests
{
    [Fact]
    public void SpanWriter_TryWrite()
    {
        Span<byte> span = new byte[5];
        SpanWriter<byte> writer = new(span);

        writer.TryWrite(1).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 0, 0, 0, 0]);

        writer.TryWrite(2).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 0, 0, 0]);

        writer.TryWrite(3).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 3, 0, 0]);

        writer.TryWrite(4).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 3, 4, 0]);

        writer.TryWrite(5).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 3, 4, 5]);

        writer.TryWrite(6).Should().BeFalse();
    }

    [Fact]
    public void SpanWriter_TryWrite_Spans()
    {
        Span<byte> span = new byte[5];
        SpanWriter<byte> writer = new(span);

        writer.TryWrite([1, 2]).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 0, 0, 0]);

        writer.TryWrite([3, 4]).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 3, 4, 0]);

        writer.TryWrite([5]).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 2, 3, 4, 5]);

        writer.TryWrite([6]).Should().BeFalse();
    }

    [Fact]
    public void SpanWriter_TryWrite_Count()
    {
        Span<int> span = new int[5];
        SpanWriter<int> writer = new(span);

        writer.TryWriteCount(2, 1).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 1, 0, 0, 0]);

        writer.TryWriteCount(2, 2).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 1, 2, 2, 0]);

        writer.TryWriteCount(1, 3).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([1, 1, 2, 2, 3]);

        writer.TryWriteCount(1, 4).Should().BeFalse();
    }

    [Fact]
    public void SpanWriter_TryWrite_CountPoints()
    {
        Span<Point> span = new Point[5];
        SpanWriter<Point> writer = new(span);

        writer.TryWriteCount(2, new Point(1, 2)).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([new Point(1, 2), new Point(1, 2), default, default, default]);

        writer.TryWriteCount(2, new Point(3, 4)).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([new Point(1, 2), new Point(1, 2), new Point(3, 4), new Point(3, 4), default]);

        writer.TryWriteCount(1, new Point(5, 6)).Should().BeTrue();
        span.ToArray().Should().BeEquivalentTo([new Point(1, 2), new Point(1, 2), new Point(3, 4), new Point(3, 4), new Point(5, 6)]);

        writer.TryWriteCount(1, new Point(7, 8)).Should().BeFalse();
    }
}
