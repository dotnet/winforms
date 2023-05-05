﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResXResourceWriterTests
{
    private readonly byte[] _testBytes = new byte[] { 1, 2, 3 };
    private readonly string _testString1 = "TestString1";
    private readonly string _testString2 = "TestString2";
    private readonly string _ResxFileName = "test.resx";
    private readonly DateTime _testDateTime = new DateTime(2023, 5, 4);
#nullable enable
    private readonly Func<Type?, string> _typeNameConverter = type => type?.AssemblyQualifiedName ?? string.Empty;
#nullable disable

    [Fact]
    public void TestRoundTrip()
    {
        var key = "Some.Key.Name";
        var value = "Some.Key.Value";

        using (var stream = new MemoryStream())
        {
            using (var writer = new ResXResourceWriter(stream))
            {
                writer.AddResource(key, value);
            }

            var buffer = stream.ToArray();
            using (var reader = new ResXResourceReader(new MemoryStream(buffer)))
            {
                var dictionary = new Dictionary<object, object>();
                IDictionaryEnumerator dictionaryEnumerator = reader.GetEnumerator();
                while (dictionaryEnumerator.MoveNext())
                {
                    dictionary.Add(dictionaryEnumerator.Key, dictionaryEnumerator.Value);
                }

                KeyValuePair<object, object> pair = Assert.Single(dictionary);
                Assert.Equal(key, pair.Key);
                Assert.Equal(value, pair.Value);
            }
        }
    }

    [Fact]
    public void ResXResourceWriter_Constructor_FileName_CreatesInstance()
    {
        // Arrange
        string fileName = _ResxFileName;

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(fileName);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_FileNameAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        string fileName = _ResxFileName;

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(fileName, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_Stream_CreatesInstance()
    {
        // Arrange
        using Stream stream = new MemoryStream();

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(stream);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_StreamAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        using Stream stream = new MemoryStream();

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(stream, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriter_CreatesInstance()
    {
        // Arrange
        using TextWriter textWriter = new StringWriter();

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(textWriter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriterAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        using TextWriter textWriter = new StringWriter();

        // Act
        using ResXResourceWriter writer = new ResXResourceWriter(textWriter, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ByteValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddMetadata(_testString1, _testBytes);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testBytes, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_StringValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddMetadata(_testString1, _testString2);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testString2, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ObjectValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddMetadata(_testString1, _testDateTime);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ByteValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddResource(_testString1, _testBytes);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testBytes, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_StringValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddResource(_testString1, _testString2);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testString2, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ObjectValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddResource(_testString1, _testDateTime);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ResxDataNodeValue_AddsResourceToResourceFile()
    {
        // Arrange
        ResXDataNode dataNode = new ResXDataNode(_testString1, _testDateTime);
        using ResXResourceWriter writer = new ResXResourceWriter(_ResxFileName);

        // Act
        writer.AddResource(_testString1, dataNode);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader(_ResxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }
}
