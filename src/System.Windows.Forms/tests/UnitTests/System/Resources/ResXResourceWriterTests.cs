// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResXResourceWriterTests
{
    private readonly byte[] _testBytes = [1, 2, 3];
    private readonly string _testString1 = "TestString1";
    private readonly string _testString2 = "TestString2";
    private readonly string _resxFileName = "test.resx";
    private readonly DateTime _testDateTime = new(2023, 5, 4);
    private readonly Func<Type, string> _typeNameConverter = type => type?.AssemblyQualifiedName ?? string.Empty;

    [Fact]
    public void TestRoundTrip()
    {
        string key = "Some.Key.Name";
        string value = "Some.Key.Value";

        using MemoryStream stream = new();
        using (ResXResourceWriter writer = new(stream))
        {
            writer.AddResource(key, value);
        }

        byte[] buffer = stream.ToArray();
        using ResXResourceReader reader = new(new MemoryStream(buffer));
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

    [Fact]
    public void ResXResourceWriter_Constructor_FileName_CreatesInstance()
    {
        // Arrange
        string fileName = _resxFileName;

        // Act
        using ResXResourceWriter writer = new(fileName);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_FileNameAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        string fileName = _resxFileName;

        // Act
        using ResXResourceWriter writer = new(fileName, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_Stream_CreatesInstance()
    {
        // Arrange
        using MemoryStream stream = new();

        // Act
        using ResXResourceWriter writer = new(stream);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_StreamAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        using MemoryStream stream = new();

        // Act
        using ResXResourceWriter writer = new(stream, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriter_CreatesInstance()
    {
        // Arrange
        using StringWriter stringWriter = new();

        // Act
        using ResXResourceWriter writer = new(stringWriter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriterAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        using StringWriter stringWriter = new();

        // Act
        using ResXResourceWriter writer = new(stringWriter, _typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ByteValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddMetadata(_testString1, _testBytes);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testBytes, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_StringValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddMetadata(_testString1, _testString2);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testString2, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ObjectValue_AddsMetadataToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddMetadata(_testString1, _testDateTime);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ByteValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddResource(_testString1, _testBytes);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testBytes, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_StringValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddResource(_testString1, _testString2);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testString2, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ObjectValue_AddsResourceToResourceFile()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddResource(_testString1, _testDateTime);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ResxDataNodeValue_AddsResourceToResourceFile()
    {
        // Arrange
        ResXDataNode dataNode = new(_testString1, _testDateTime);
        using ResXResourceWriter writer = new(_resxFileName);

        // Act
        writer.AddResource(_testString1, dataNode);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new(_resxFileName);
        var enumerator = reader.GetEnumerator();
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(_testString1, currentEntry.Key);
        Assert.Equal(_testDateTime, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddAlias_WithNullAssemblyName_ThrowNullException()
    {
        // Arrange
        using ResXResourceWriter writer = new(_resxFileName);

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            writer.AddAlias(aliasName: "MyAlias", assemblyName: null);
        });
    }
}
