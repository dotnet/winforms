// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.Tests;

public class StreamExtensionsTests
{
    [Fact]
    public void Read_Span_ReadsData()
    {
        byte[] data = [1, 2, 3, 4, 5];
        using MemoryStream stream = new(data);

        Span<byte> buffer = stackalloc byte[5];
        int bytesRead = stream.Read(buffer);

        bytesRead.Should().Be(5);
        buffer.SequenceEqual(data).Should().BeTrue();
    }

    [Fact]
    public void Read_Span_PartialRead()
    {
        byte[] data = [10, 20, 30, 40, 50];
        using MemoryStream stream = new(data);

        Span<byte> buffer = stackalloc byte[3];
        int bytesRead = stream.Read(buffer);

        bytesRead.Should().Be(3);
        buffer[0].Should().Be(10);
        buffer[1].Should().Be(20);
        buffer[2].Should().Be(30);
    }

    [Fact]
    public void Read_Span_EmptyBuffer_ReturnsZero()
    {
        byte[] data = [1, 2, 3];
        using MemoryStream stream = new(data);

        Span<byte> buffer = [];
        int bytesRead = stream.Read(buffer);

        bytesRead.Should().Be(0);
    }

    [Fact]
    public void Write_Span_WritesData()
    {
        using MemoryStream stream = new();
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };

        stream.Write(data);

        stream.ToArray().Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void Write_Span_EmptyData_WritesNothing()
    {
        using MemoryStream stream = new();
        ReadOnlySpan<byte> data = [];

        stream.Write(data);

        stream.Length.Should().Be(0);
    }

    [Fact]
    public void Write_Span_MultipleWrites()
    {
        using MemoryStream stream = new();
        ReadOnlySpan<byte> data1 = stackalloc byte[] { 1, 2 };
        ReadOnlySpan<byte> data2 = stackalloc byte[] { 3, 4 };

        stream.Write(data1);
        stream.Write(data2);

        stream.ToArray().Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void ReadExactly_ReadsExactAmount()
    {
        byte[] data = [1, 2, 3, 4, 5];
        using MemoryStream stream = new(data);

        Span<byte> buffer = stackalloc byte[5];
        stream.ReadExactly(buffer);

        buffer.SequenceEqual(data).Should().BeTrue();
    }

    [Fact]
    public void ReadExactly_NotEnoughData_ThrowsEndOfStreamException()
    {
        byte[] data = [1, 2, 3];
        using MemoryStream stream = new(data);

        byte[] buffer = new byte[5];
        Action action = () => stream.ReadExactly(buffer);

        action.Should().Throw<EndOfStreamException>();
    }

    [Fact]
    public void ReadExactly_EmptyBuffer_Succeeds()
    {
        byte[] data = [1, 2, 3];
        using MemoryStream stream = new(data);

        Span<byte> buffer = [];
        stream.ReadExactly(buffer);
    }

    [Fact]
    public void ReadExactly_EmptyStream_ThrowsEndOfStreamException()
    {
        using MemoryStream stream = new([]);

        byte[] buffer = new byte[1];
        Action action = () => stream.ReadExactly(buffer);

        action.Should().Throw<EndOfStreamException>();
    }

    [Fact]
    public void Read_Write_RoundTrip()
    {
        byte[] original = [10, 20, 30, 40, 50, 60, 70, 80];
        using MemoryStream writeStream = new();

        writeStream.Write(original.AsSpan());

        writeStream.Position = 0;
        Span<byte> readBuffer = stackalloc byte[original.Length];
        int bytesRead = writeStream.Read(readBuffer);

        bytesRead.Should().Be(original.Length);
        readBuffer.SequenceEqual(original).Should().BeTrue();
    }
}
