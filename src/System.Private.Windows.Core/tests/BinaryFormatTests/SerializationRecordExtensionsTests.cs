// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Formats.Nrbf;
using System.Runtime.Serialization.Formatters.Binary;
using FormatTests.FormattedObject;

namespace System.Windows.Forms.Nrbf.Tests;

public class SerializationRecordExtensionsTests
{
    public static IEnumerable<object[]> TryGetFrameworkObject_SupportedObjects_TestData =>
        BinaryFormatWriterTests.TryWriteFrameworkObject_SupportedObjects_TestData;

    [Theory]
    [MemberData(nameof(TryGetFrameworkObject_SupportedObjects_TestData))]
    public void TryGetFrameworkObject_SupportedObjects_Read(object value)
    {
        using MemoryStream stream = new();
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        formatter.Serialize(stream, value);
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

    public static IEnumerable<object[]?> TryGetDrawingPrimitives_SupportedObjects_TestData =>
        BinaryFormatWriterTests.DrawingPrimitives_TestData;

    [Theory]
    [MemberData(nameof(TryGetDrawingPrimitives_SupportedObjects_TestData))]
    public void TryGetDrawingPrimitivesObject_SupportedObjects_Read(object value)
    {
        using MemoryStream stream = new();
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        formatter.Serialize(stream, value);
        stream.Position = 0;

        SerializationRecord rootRecord = NrbfDecoder.Decode(stream);
        bool result = rootRecord.TryGetDrawingPrimitivesObject(out object? deserialized);

        result.Should().BeTrue();

        if (value is Color color)
        {
            deserialized.Should().BeOfType<Color>().Which.Should().BeEquivalentTo(color);
        }
        else
        {
            deserialized.Should().Be(value);
        }
    }

    [Fact]
    public void BinaryFormatWriter_TryGetDrawingPrimitivesObject_Fail()
    {
        using MemoryStream stream = new();
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.

        formatter.Serialize(stream, "abc");
        stream.Position = 0;

        SerializationRecord rootRecord = NrbfDecoder.Decode(stream);

        bool result = rootRecord.TryGetDrawingPrimitivesObject(out object? deserialized);
        result.Should().BeFalse();
        deserialized.Should().BeNull();
    }
}
