// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.Tests;

public class BinaryReaderExtensionsTests
{
    [Fact]
    public void Read_Span_ReadsData()
    {
        byte[] data = [1, 2, 3, 4, 5];
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        Span<byte> buffer = stackalloc byte[5];
        int bytesRead = reader.Read(buffer);

        bytesRead.Should().Be(5);
        buffer.SequenceEqual(data).Should().BeTrue();
    }

    [Fact]
    public void Read_Span_PartialRead()
    {
        byte[] data = [10, 20, 30, 40, 50];
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        Span<byte> buffer = stackalloc byte[3];
        int bytesRead = reader.Read(buffer);

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
        using BinaryReader reader = new(stream);

        Span<byte> buffer = [];
        int bytesRead = reader.Read(buffer);

        bytesRead.Should().Be(0);
    }

    [Fact]
    public void Read_Span_PastEndOfStream_ReturnsAvailableBytes()
    {
        byte[] data = [1, 2];
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        Span<byte> buffer = stackalloc byte[5];
        int bytesRead = reader.Read(buffer);

        bytesRead.Should().Be(2);
        buffer[0].Should().Be(1);
        buffer[1].Should().Be(2);
    }

    [Fact]
    public void Read_Span_MultipleReads()
    {
        byte[] data = [1, 2, 3, 4, 5, 6];
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        Span<byte> buffer1 = stackalloc byte[3];
        Span<byte> buffer2 = stackalloc byte[3];

        int read1 = reader.Read(buffer1);
        int read2 = reader.Read(buffer2);

        read1.Should().Be(3);
        read2.Should().Be(3);
        buffer1[0].Should().Be(1);
        buffer1[2].Should().Be(3);
        buffer2[0].Should().Be(4);
        buffer2[2].Should().Be(6);
    }
}
