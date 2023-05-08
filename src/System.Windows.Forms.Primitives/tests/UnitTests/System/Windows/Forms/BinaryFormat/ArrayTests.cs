// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat.Tests;

public class ArrayTests
{
    [Theory]
    [MemberData(nameof(ArrayInfo_ParseSuccessData))]
    public void ArrayInfo_Parse_Success(MemoryStream stream, int expectedId, int expectedLength)
    {
        using BinaryReader reader = new(stream);
        ArrayInfo info = ArrayInfo.Parse(reader, out Count length);
        info.ObjectId.Should().Be(expectedId);
        info.Length.Should().Be(expectedLength);
        length.Should().Be(expectedLength);
    }

    public static TheoryData<Stream, int, int> ArrayInfo_ParseSuccessData => new()
    {
        { new MemoryStream(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }), 0, 0 },
        { new MemoryStream(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }), 1, 1 },
        { new MemoryStream(new byte[] { 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }), 2, 1 },
        { new MemoryStream(new byte[] { 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0x7F }), int.MaxValue, int.MaxValue }
    };

    [Theory]
    [MemberData(nameof(ArrayInfo_ParseNegativeData))]
    public void ArrayInfo_Parse_Negative(MemoryStream stream, Type expectedException)
    {
        using BinaryReader reader = new(stream);
        Assert.Throws(expectedException, () => ArrayInfo.Parse(reader, out Count length));
    }

    public static TheoryData<Stream, Type> ArrayInfo_ParseNegativeData => new()
    {
        // Not enough data
        { new MemoryStream(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }), typeof(EndOfStreamException) },
        { new MemoryStream(new byte[] { 0x00, 0x00, 0x00 }), typeof(EndOfStreamException) },
        // Negative numbers
        { new MemoryStream(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }), typeof(ArgumentOutOfRangeException) },
        { new MemoryStream(new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }), typeof(ArgumentOutOfRangeException) }
    };
}
