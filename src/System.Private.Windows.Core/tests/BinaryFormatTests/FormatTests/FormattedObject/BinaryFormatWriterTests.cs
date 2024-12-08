// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Private.Windows.Core.BinaryFormat;
using System.Formats.Nrbf;
using System.Windows.Forms.Nrbf;
using System.Drawing;
using System.Diagnostics;

namespace FormatTests.FormattedObject;

public class BinaryFormatWriterTests
{
    [Theory]
    [InlineData("Hello World.")]
    [InlineData("")]
    [InlineData("\0")]
    [InlineData("Embedded\0 Null.")]
    public void BinaryFormatWriter_WriteString(string testString)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.WriteString(stream, testString);
        stream.Position = 0;

        BinaryFormatter formatter = new();

        // cs/dangerous-binary-deserialization
        object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
        deserialized.Should().Be(testString);
    }

    [Theory]
    [MemberData(nameof(TryWriteFrameworkObject_SupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteFrameworkObject_SupportedObjects_BinaryFormatterRead(object value)
    {
        Debug.Print(value.ToString());

        using MemoryStream stream = new();
        bool success = BinaryFormatWriter.TryWriteFrameworkObject(stream, value);
        success.Should().BeTrue();
        stream.Position = 0;

        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        // cs/dangerous-binary-deserialization
        object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.

        if (value is Hashtable hashtable)
        {
            Hashtable deserializedHashtable = (Hashtable)deserialized;
            deserializedHashtable.Count.Should().Be(hashtable.Count);
            foreach (object? key in hashtable.Keys)
            {
                deserializedHashtable[key].Should().Be(hashtable[key]);
            }
        }
        else if (value is IEnumerable enumerable)
        {
            ((IEnumerable)deserialized).Should().BeEquivalentTo(enumerable);
        }
        else
        {
            deserialized.Should().Be(value);
        }
    }

    [Theory]
    [MemberData(nameof(DrawingPrimitives_TestData))]
    public void BinaryFormatWriter_TryWriteDrawingPrimitivesObject_SupportedObjects_BinaryFormatterRead(object value)
    {
        using MemoryStream stream = new();
        bool success = BinaryFormatWriter.TryWriteDrawingPrimitivesObject(stream, value);
        success.Should().BeTrue();

        stream.Position = 0;
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        // cs/dangerous-binary-deserialization
        object deserialized = formatter.Deserialize(stream); // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.

        if (value is Color color)
        {
            deserialized.Should().BeOfType<Color>().Which.Should().BeEquivalentTo(color);
        }
        else
        {
            deserialized.Should().Be(value);
        }
    }

    [Theory]
    [MemberData(nameof(TryWriteFrameworkObject_SupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteFrameworkObject_SupportedObjects_RoundTrip(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value).Should().BeTrue();
        stream.Position = 0;

        SerializationRecord rootRecord = NrbfDecoder.Decode(stream);
        bool result = rootRecord.TryGetFrameworkObject(out object? deserialized);

        result.Should().BeTrue();

        if (value is Hashtable hashtable)
        {
            Hashtable deserializedHashtable = (Hashtable)deserialized!;
            deserializedHashtable.Count.Should().Be(hashtable.Count);
            foreach (object? key in hashtable.Keys)
            {
                deserializedHashtable[key].Should().Be(hashtable[key]);
            }
        }
        else if (value is IEnumerable enumerable)
        {
            ((IEnumerable)deserialized!).Should().BeEquivalentTo(enumerable);
        }
        else
        {
            deserialized.Should().Be(value);
        }
    }

    [Theory]
    [MemberData(nameof(DrawingPrimitives_TestData))]
    public void BinaryFormatWriter_TryWriteDrawingPrimitivesObject_SupportedObjects_RoundTrip(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteDrawingPrimitivesObject(stream, value).Should().BeTrue();
        stream.Position = 0;

        SerializationRecord rootRecord = NrbfDecoder.Decode(stream);
        bool result = rootRecord.TryGetDrawingPrimitivesObject(out object? deserialized);

        result.Should().BeTrue();
        deserialized.Should().NotBeNull();

        if (value is Color color)
        {
            deserialized.Should().BeOfType<Color>().Which.Should().BeEquivalentTo(color);
        }
        else
        {
            deserialized.Should().Be(value);
        }
    }

    [Theory]
    [MemberData(nameof(TryWriteObject_UnsupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteFrameworkObject_UnsupportedObjects_RoundTrip(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value).Should().BeFalse();
        stream.Position.Should().Be(0);
    }

    [Fact]
    public void BinaryFormatWriter_TryWriteDrawingPrimitivesObject_UnsupportedObjects_RoundTrip()
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteDrawingPrimitivesObject(stream, Brushes.AliceBlue).Should().BeFalse();
        stream.Position.Should().Be(0);
    }

    public static IEnumerable<object[]> TryWriteFrameworkObject_SupportedObjects_TestData =>
        ((IEnumerable<object[]>)HashtableTests.Hashtables_TestData).Concat(
            ListTests.PrimitiveLists_TestData).Concat(
            ListTests.ArrayLists_TestData).Concat(
            PrimitiveTypeTests.Primitive_Data).Concat(
            SystemDrawing_TestData).Concat(
            Array_TestData).Skip(9);

    public static IEnumerable<object[]?> TryWriteObject_UnsupportedObjects_TestData =>
        ((IEnumerable<object[]?>)HashtableTests.Hashtables_UnsupportedTestData).Concat(
            ListTests.Lists_UnsupportedTestData).Concat(
            ListTests.ArrayLists_UnsupportedTestData).Concat(
            Array_UnsupportedTestData);

    public static TheoryData<object> SystemDrawing_TestData =>
    [
        default(PointF),
        default(RectangleF)
    ];

    public static TheoryData<object> DrawingPrimitives_TestData =>
    [
        new Point(-1, 2),
        new Point(int.MaxValue, int.MinValue),
        Point.Empty,
        Rectangle.Empty,
        new Rectangle(1, 2, 3, 4),
        new Rectangle(int.MinValue, int.MaxValue, 0, 0),
        new Size(6, 7),
        new Size(int.MaxValue, int.MinValue),
        new Size(0, 0),
        new SizeF(7F, 8F),
        new SizeF(float.MaxValue, float.MinValue),
        new SizeF(-float.MaxValue, float.PositiveInfinity),
        new SizeF(0, 0),
        Color.Empty,
        Color.AliceBlue,
        Color.FromKnownColor(KnownColor.ActiveCaption),
        Color.FromArgb(1, 2, 3),
        Color.FromArgb(4, Color.Yellow),
        Color.FromName("Blue"),
        SystemColors.ButtonFace
    ];

    public static TheoryData<Array> StringArray_Parse_Data =>
    [
        new string?[] { "one", "two" },
        new string?[] { "yes", "no", null },
        new string?[] { "same", "same", "same" }
    ];

    public static TheoryData<Array> PrimitiveArray_Parse_Data =>
    [
        new int[] { 1, 2, 3 },
        new int[] { 1, 2, 1 },
        new float[] { 1.0f, float.NaN, float.PositiveInfinity },
        new DateTime[] { DateTime.MaxValue }
    ];

    public static IEnumerable<object[]> Array_TestData =>
        ((IEnumerable<object[]>)StringArray_Parse_Data).Concat(PrimitiveArray_Parse_Data);

    public static TheoryData<Array> Array_UnsupportedTestData =>
    [
        new Point[] { default },
        new object[] { new() },
    ];
}
