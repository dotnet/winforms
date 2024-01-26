// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.Compression;

public class RunLengthEncoderTests
{
    [Fact]
    public void RunLengthEncoder_TryEncode()
    {
        ReadOnlySpan<byte> data = [1, 1, 1, 2, 2, 3, 3, 3, 3];
        Span<byte> encoded = new byte[RunLengthEncoder.GetEncodedLength(data)];
        RunLengthEncoder.TryEncode(data, encoded, out int written).Should().BeTrue();
        written.Should().Be(6);
        encoded.ToArray().Should().BeEquivalentTo([3, 1, 2, 2, 4, 3]);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    [InlineData(255, 2)]
    [InlineData(256, 4)]
    public void RunLengthEncoder_GetEncodedLength(int count, int expectedLength)
    {
        Span<byte> data = new byte[count];
        Span<byte> encoded = new byte[RunLengthEncoder.GetEncodedLength(data)];
        RunLengthEncoder.TryEncode(data, encoded, out int written).Should().BeTrue();
        written.Should().Be(expectedLength);
        encoded.Length.Should().Be(expectedLength);
    }

    [Fact]
    public void RunLengthEncoder_RoundTrip()
    {
        ReadOnlySpan<byte> data = [1, 1, 1, 2, 2, 3, 3, 3, 3];
        Span<byte> encoded = new byte[RunLengthEncoder.GetEncodedLength(data)];
        RunLengthEncoder.TryEncode(data, encoded, out int written).Should().BeTrue();
        written.Should().Be(6);

        Span<byte> decoded = new byte[RunLengthEncoder.GetDecodedLength(encoded)];
        RunLengthEncoder.TryDecode(encoded, decoded, out written).Should().BeTrue();
        written.Should().Be(data.Length);
        decoded.ToArray().Should().BeEquivalentTo(data.ToArray());
    }
}
