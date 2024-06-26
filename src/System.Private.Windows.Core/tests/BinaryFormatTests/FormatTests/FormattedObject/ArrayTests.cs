// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.BinaryFormat;

namespace FormatTests.FormattedObject;

public class ArrayTests : Common.ArrayTests<FormattedObjectSerializer>
{
    public override void Roundtrip_ArrayContainingArrayAtNonZeroLowerBound()
    {
        Action action = base.Roundtrip_ArrayContainingArrayAtNonZeroLowerBound;
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [MemberData(nameof(ArrayInfo_ParseSuccessData))]
    public void ArrayInfo_Parse_Success(Stream stream, int expectedId, int expectedLength)
    {
        using BinaryReader reader = new(stream);
        Id id = ArrayInfo.Parse(reader, out Count length);
        id.Should().Be((Id)expectedId);
        length.Should().Be(expectedLength);
        length.Should().Be(expectedLength);
    }

    public static TheoryData<Stream, int, int> ArrayInfo_ParseSuccessData => new()
    {
        { new MemoryStream([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]), 0, 0 },
        { new MemoryStream([0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00]), 1, 1 },
        { new MemoryStream([0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00]), 2, 1 },
        { new MemoryStream([0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0x7F]), int.MaxValue, int.MaxValue }
    };

    [Theory]
    [MemberData(nameof(ArrayInfo_ParseNegativeData))]
    public void ArrayInfo_Parse_Negative(Stream stream, Type expectedException)
    {
        using BinaryReader reader = new(stream);
        Assert.Throws(expectedException, () => ArrayInfo.Parse(reader, out Count length));
    }

    public static TheoryData<Stream, Type> ArrayInfo_ParseNegativeData => new()
    {
        // Not enough data
        { new MemoryStream([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]), typeof(EndOfStreamException) },
        { new MemoryStream([0x00, 0x00, 0x00]), typeof(EndOfStreamException) },
        // Negative numbers
        { new MemoryStream([0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF]), typeof(ArgumentOutOfRangeException) }
    };

    [Theory]
    [MemberData(nameof(StringArray_Parse_Data))]
    public void StringArray_Parse(string?[] strings)
    {
        BinaryFormattedObject format = new(Serialize(strings));
        ArraySingleString array = (ArraySingleString)format.RootRecord;
        array.Should().BeEquivalentTo(strings);
    }

    public static TheoryData<string?[]> StringArray_Parse_Data => new()
    {
        new string?[] { "one", "two" },
        new string?[] { "yes", "no", null },
        new string?[] { "same", "same", "same" }
    };

    [Theory]
    [MemberData(nameof(PrimitiveArray_Parse_Data))]
    public void PrimitiveArray_Parse(Array array)
    {
        BinaryFormattedObject format = new(Serialize(array));
        ArrayRecord arrayRecord = (ArrayRecord)format.RootRecord;
        arrayRecord.Should().BeEquivalentTo((IEnumerable)array);
    }

    public static TheoryData<Array> PrimitiveArray_Parse_Data => new()
    {
        new int[] { 1, 2, 3 },
        new int[] { 1, 2, 1 },
        new float[] { 1.0f, float.NaN, float.PositiveInfinity },
        new DateTime[] { DateTime.MaxValue }
    };

    public static IEnumerable<object[]> Array_TestData => StringArray_Parse_Data.Concat(PrimitiveArray_Parse_Data);

    public static TheoryData<Array> Array_UnsupportedTestData => new()
    {
        new Point[] { new() },
        new object[] { new() },
    };

    public override void BinaryArray_InvalidRank_Positive(int rank, byte arrayType)
    {
        // BinaryFormatter doesn't throw on these.
        Action action = () => base.BinaryArray_InvalidRank_Positive(rank, arrayType);
        action.Should().Throw<SerializationException>();
    }
}
