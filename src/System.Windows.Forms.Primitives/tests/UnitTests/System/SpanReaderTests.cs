// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Tests;

public class SpanReaderTests
{
    [Fact]
    public void SpanReader_TryReadTo_SkipDelimiter()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        SpanReader<byte> reader = new(span);

        reader.TryReadTo(3, out var read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([1, 2]);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([4, 5]);

        reader.TryReadTo(5, out read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([4]);
        reader.UnreadSpan.ToArray().Should().BeEmpty();
    }

    [Fact]
    public void SpanReader_TryReadTo_DontSkipDelimiter()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        SpanReader<byte> reader = new(span);

        reader.TryReadTo(3, advancePastDelimiter: false, out var read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([1, 2]);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([3, 4, 5]);

        reader.TryReadTo(5, advancePastDelimiter: false, out read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([3, 4]);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);

        reader.TryReadTo(5, advancePastDelimiter: false, out read).Should().BeTrue();
        read.ToArray().Should().BeEmpty();
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);
    }

    [Fact]
    public void SpanReader_Advance()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        SpanReader<byte> reader = new(span);

        reader.Advance(2);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([3, 4, 5]);

        reader.Advance(2);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);

        reader.Advance(1);
        reader.UnreadSpan.ToArray().Should().BeEmpty();

        try
        {
            reader.Advance(1);
            Assert.Fail($"Expected {nameof(ArgumentOutOfRangeException)}");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [Fact]
    public void SpanReader_Rewind()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        SpanReader<byte> reader = new(span);

        reader.Advance(2);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([3, 4, 5]);

        reader.Rewind(1);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([2, 3, 4, 5]);

        try
        {
            reader.Rewind(2);
            Assert.Fail($"Expected {nameof(ArgumentOutOfRangeException)}");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }

        reader.Rewind(1);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void SpanReader_TryRead_ReadPoints()
    {
        ReadOnlySpan<uint> span = new uint[] { 1, 2, 3, 4, 5 };
        SpanReader<uint> reader = new(span);

        reader.TryRead(out Point value).Should().BeTrue();
        value.Should().Be(new Point(1, 2));

        reader.TryRead(out value).Should().BeTrue();
        value.Should().Be(new Point(3, 4));

        reader.TryRead(out value).Should().BeFalse();
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);
    }

    [Fact]
    public void SpanReader_TryRead_ReadPointCounts()
    {
        ReadOnlySpan<uint> span = new uint[] { 1, 2, 3, 4, 5 };
        SpanReader<uint> reader = new(span);

        reader.TryRead(2, out ReadOnlySpan<Point> value).Should().BeTrue();
        value.ToArray().Should().BeEquivalentTo([new Point(1, 2), new Point(3, 4)]);

        // This fails to compile as the span is read only, as expected.
        // value[0].X = 0;

        reader.TryRead(2, out value).Should().BeFalse();
        value.ToArray().Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    public void SpanReader_TryRead_ReadPointFCounts_NotEnoughBuffer(int bufferSize, int readCount)
    {
        ReadOnlySpan<float> span = new float[bufferSize];
        SpanReader<float> reader = new(span);

        reader.TryRead<PointF>(readCount, out _).Should().BeFalse();
    }

    [Fact]
    public void SpanReader_TryRead_Count()
    {
        ReadOnlySpan<uint> span = new uint[] { 1, 2, 3, 4, 5 };
        SpanReader<uint> reader = new(span);

        reader.TryRead(2, out var read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([1, 2]);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([3, 4, 5]);

        reader.TryRead(2, out read).Should().BeTrue();
        read.ToArray().Should().BeEquivalentTo([3, 4]);
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);

        reader.TryRead(2, out read).Should().BeFalse();
        reader.UnreadSpan.ToArray().Should().BeEquivalentTo([5]);
    }
}
