// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using Utilities = System.Windows.Forms.DataObject.Composition.BinaryFormatUtilities;

namespace System.Windows.Forms.Tests;

public partial class BinaryFormatUtilitiesTests : IDisposable
{
    private readonly MemoryStream _stream;

    public BinaryFormatUtilitiesTests() => _stream = new();

    public void Dispose() => _stream.Dispose();

    private void WriteObjectToStream(object value, bool restrictSerialization = false) =>
        Utilities.WriteObjectToStream(_stream, value, restrictSerialization);

    private object? ReadObjectFromStream(bool restrictDeserialization = false)
    {
        _stream.Position = 0;
        return Utilities.ReadObjectFromStream(_stream, restrictDeserialization);
    }

    private object? RoundTripObject(object value)
    {
        // This is equivalent to SetData/GetData methods with unbounded formats,
        // and works with the BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream();
    }

    private object? RoundTripObject_RestrictedFormat(object value)
    {
        // This is equivalent to SetData/GetData methods using registered OLE formats and thus the BitmapBinder,
        // and works with the BinaryFormat AppCompat switches.
        WriteObjectToStream(value, restrictSerialization: true);
        return ReadObjectFromStream(restrictDeserialization: true);
    }

    // Primitive types as defined by the NRBF spec.
    // https://learn.microsoft.com/dotnet/api/system.formats.nrbf.primitivetyperecord
    public static TheoryData<object> PrimitiveObjects_TheoryData =>
    [
        (byte)1,
        (sbyte)2,
        (short)3,
        (ushort)4,
        5,
        (uint)6,
        (long)7,
        (ulong)8,
        (float)9.0,
        10.0,
        'a',
        true,
        "string",
        DateTime.Now,
        TimeSpan.FromHours(1),
        decimal.MaxValue
    ];

    public static TheoryData<object> KnownObjects_TheoryData =>
    [
        -(nint)11,
        (nuint)12,
        new PointF(1, 2),
        new RectangleF(1, 2, 3, 4),
        new Point(-1, int.MaxValue),
        new Rectangle(-1, int.MinValue, 10, 13),
        new Size(int.MaxValue, int.MinValue),
        new SizeF(float.MaxValue, float.MinValue),
        Color.Red
    ];

    public static TheoryData<IList> PrimitiveListObjects_TheoryData =>
    [
        new List<bool> { false, true },
        new List<char> { char.MinValue, char.MaxValue },
        new List<byte> { byte.MinValue, byte.MaxValue },
        new List<sbyte> { sbyte.MinValue, sbyte.MaxValue },
        new List<short> { short.MinValue, short.MaxValue },
        new List<ushort> { ushort.MinValue, ushort.MaxValue },
        new List<int> { int.MinValue, int.MaxValue },
        new List<uint> { uint.MinValue, uint.MaxValue },
        new List<long> { long.MinValue, long.MaxValue },
        new List<ulong> { ulong.MinValue, ulong.MaxValue },
        new List<float> { float.MinValue, float.MaxValue },
        new List<double> { double.MinValue, double.MaxValue },
        new List<decimal> { decimal.MinValue, decimal.MaxValue },
        new List<DateTime> { DateTime.MinValue, DateTime.MaxValue },
        new List<TimeSpan> { TimeSpan.MinValue, TimeSpan.MaxValue },
        new List<string> { "a", "b", "c" }
    ];

    public static TheoryData<Array> PrimitiveArrayObjects_TheoryData =>
    [
        new bool[] { false, true },
        new char[] { char.MinValue, char.MaxValue },
        new byte[] { byte.MinValue, byte.MaxValue },
        new sbyte[] { sbyte.MinValue, sbyte.MaxValue },
        new short[] { short.MinValue, short.MaxValue },
        new ushort[] { ushort.MinValue, ushort.MaxValue },
        new int[] { int.MinValue, int.MaxValue },
        new uint[] { uint.MinValue, uint.MaxValue },
        new long[] { long.MinValue, long.MaxValue },
        new ulong[] { ulong.MinValue, ulong.MaxValue },
        new float[] { float.MinValue, float.MaxValue },
        new double[] { double.MinValue, double.MaxValue },
        new decimal[] { decimal.MinValue, decimal.MaxValue },
        new DateTime[] { DateTime.MinValue, DateTime.MaxValue },
        new TimeSpan[] { TimeSpan.MinValue, TimeSpan.MaxValue },
        new string[] { "a", "b", "c" }
    ];

    public static TheoryData<ArrayList> PrimitiveArrayListObjects_TheoryData =>
    [
        new ArrayList { null },
        new ArrayList { null, "something" },
        new ArrayList { false, true },
        new ArrayList { char.MinValue, char.MaxValue },
        new ArrayList { byte.MinValue, byte.MaxValue },
        new ArrayList { sbyte.MinValue, sbyte.MaxValue },
        new ArrayList { short.MinValue, short.MaxValue },
        new ArrayList { ushort.MinValue, ushort.MaxValue },
        new ArrayList { int.MinValue, int.MaxValue },
        new ArrayList { uint.MinValue, uint.MaxValue },
        new ArrayList { long.MinValue, long.MaxValue },
        new ArrayList { ulong.MinValue, ulong.MaxValue },
        new ArrayList { float.MinValue, float.MaxValue },
        new ArrayList { double.MinValue, double.MaxValue },
        new ArrayList { decimal.MinValue, decimal.MaxValue },
        new ArrayList { DateTime.MinValue, DateTime.MaxValue },
        new ArrayList { TimeSpan.MinValue, TimeSpan.MaxValue },
        new ArrayList { "a", "b", "c" }
    ];

    public static TheoryData<Hashtable> PrimitiveTypeHashtables_TheoryData =>
    [
        new Hashtable { { "bool", true } },
        new Hashtable { { "char", 'a' } },
        new Hashtable { { "byte", (byte)1 } },
        new Hashtable { { "sbyte", (sbyte)2 } },
        new Hashtable { { "short", (short)3 } },
        new Hashtable { { "ushort", (ushort)4 } },
        new Hashtable { { "int", 5 } },
        new Hashtable { { "uint", (uint)6 } },
        new Hashtable { { "long", (long)7 } },
        new Hashtable { { "ulong", (ulong)8 } },
        new Hashtable { { "float", 9.0f } },
        new Hashtable { { "double", 10.0 } },
        new Hashtable { { "decimal", (decimal)11 } },
        new Hashtable { { "DateTime", DateTime.Now } },
        new Hashtable { { "TimeSpan", TimeSpan.FromHours(1) } },
        new Hashtable { { "string", "test" } }
    ];

    public static TheoryData<NotSupportedException> NotSupportedException_TestData =>
    [
        new NotSupportedException(),
        new NotSupportedException("Error message"),
        new NotSupportedException(null)
    ];

    public static TheoryData<IList> Lists_UnsupportedTestData =>
    [
        new List<object>(),
        new List<nint>(),
        new List<(int, int)>(),
        new List<nint> { nint.MinValue, nint.MaxValue },
        new List<nuint> { nuint.MinValue, nuint.MaxValue }
    ];

    [Theory]
    [MemberData(nameof(PrimitiveObjects_TheoryData))]
    [MemberData(nameof(KnownObjects_TheoryData))]
    public void RoundTrip_Simple(object value) =>
        RoundTripObject(value).Should().Be(value);

    [Theory]
    [MemberData(nameof(PrimitiveObjects_TheoryData))]
    [MemberData(nameof(KnownObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_Simple(object value) =>
        RoundTripObject_RestrictedFormat(value).Should().Be(value);

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_NotSupportedException(NotSupportedException value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_RestrictedFormat_NotSupportedException(NotSupportedException value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Fact]
    public void RoundTrip_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject(value).Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_PrimitiveList(IList value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveList(IList value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayObjects_TheoryData))]
    public void RoundTrip_PrimitiveArray(Array value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_PrimitiveArrayList(ArrayList value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveArrayList(ArrayList value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_PrimitiveHashtable(Hashtable value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveHashtable(Hashtable value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Fact]
    public void RoundTrip_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        var result = RoundTripObject(value).Should().BeOfType<ImageListStreamer>().Which;

        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        var result = RoundTripObject_RestrictedFormat(value).Should().BeOfType<ImageListStreamer>().Which;

        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject(value).Should().BeOfType<Bitmap>().Subject.Size.Should().Be(value.Size);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject_RestrictedFormat(value).Should().BeOfType<Bitmap>().Subject.Size.Should().Be(value.Size);
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value);
        Action reader = () => ReadObjectFromStream();

        writer.Should().Throw<NotSupportedException>();

        using (BinaryFormatterScope scope = new(enable: true))
        {
            WriteObjectToStream(value);
            ReadObjectFromStream().Should().BeEquivalentTo(value);
        }

        reader.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_RestrictedFormat_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value, restrictSerialization: true);
        writer.Should().Throw<SerializationException>();

        using BinaryFormatterScope scope = new(enable: true);
        writer.Should().Throw<SerializationException>();
    }

    [Fact]
    public void RoundTrip_OffsetArray()
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        // Can read offset array with the BinaryFormatter.
        using BinaryFormatterScope scope = new(enable: true);
        var result = RoundTripObject(value).Should().BeOfType<uint[,]>().Subject;

        result.Rank.Should().Be(2);
        result.GetLength(0).Should().Be(2);
        result.GetLength(1).Should().Be(3);
        result.GetLowerBound(0).Should().Be(1);
        result.GetLowerBound(1).Should().Be(2);
        result.GetValue(1, 2).Should().Be(101u);
        result.GetValue(1, 3).Should().Be(102u);
        result.GetValue(1, 4).Should().Be(103u);
        result.GetValue(2, 2).Should().Be(201u);
        result.GetValue(2, 3).Should().Be(202u);
        result.GetValue(2, 4).Should().Be(203u);
    }
}
