// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Private.Windows.Core.BinaryFormat;

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
    [MemberData(nameof(TryWriteObject_SupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteObject_SupportedObjects_BinaryFormatterRead(object value)
    {
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
    [MemberData(nameof(TryWriteObject_SupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteObject_SupportedObjects_RoundTrip(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value).Should().BeTrue();
        stream.Position = 0;

        BinaryFormattedObject format = new(stream);
        format.TryGetFrameworkObject(out object? deserialized).Should().BeTrue();

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
    [MemberData(nameof(TryWriteObject_UnsupportedObjects_TestData))]
    public void BinaryFormatWriter_TryWriteObject_UnsupportedObjects_RoundTrip(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value).Should().BeFalse();
        stream.Position.Should().Be(0);
    }

    public static IEnumerable<object[]?> TryWriteObject_SupportedObjects_TestData =>
        HashtableTests.Hashtables_TestData.Concat(
            ListTests.PrimitiveLists_TestData).Concat(
            ListTests.ArrayLists_TestData).Concat(
            PrimitiveTypeTests.Primitive_Data).Concat(
            SystemDrawingTests.SystemDrawing_TestData).Concat(
            ArrayTests.Array_TestData).Skip(9);

    public static IEnumerable<object[]?> TryWriteObject_UnsupportedObjects_TestData =>
        HashtableTests.Hashtables_UnsupportedTestData.Concat(
            ListTests.Lists_UnsupportedTestData).Concat(
            ListTests.ArrayLists_UnsupportedTestData).Concat(
            ArrayTests.Array_UnsupportedTestData);
}
