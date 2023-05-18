﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.BinaryFormat.Tests;

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

        using var formatterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore
        object deserialized = formatter.Deserialize(stream);
        deserialized.Should().Be(testString);
    }

    [Theory]
    [MemberData(nameof(TryWriteObject_SupportedObjects_TestData))]
    public void TryWriteObject_SupportedObjects_BinaryFormatterRead(object value)
    {
        using MemoryStream stream = new();
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value).Should().BeTrue();
        stream.Position = 0;

        using BinaryFormatterScope formaterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        object deserialized = formatter.Deserialize(stream);

        if (value is Hashtable hashtable)
        {
            Hashtable deserializedHashtable = (Hashtable)deserialized;
            deserializedHashtable.Count.Should().Be(hashtable.Count);
            foreach (var key in hashtable.Keys)
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
    public void TryWriteObject_SupportedObjects_RoundTrip(object value)
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
            foreach (var key in hashtable.Keys)
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

    public static IEnumerable<object[]?> TryWriteObject_SupportedObjects_TestData =>
        HashTableTests.Hashtables_TestData.Concat(
            ListTests.PrimitiveLists_TestData).Concat(
            ListTests.ArrayLists_TestData).Concat(
            PrimitiveTypeTests.Primitive_Data).Concat(
            SystemDrawingTests.SystemDrawing_TestData);
}
