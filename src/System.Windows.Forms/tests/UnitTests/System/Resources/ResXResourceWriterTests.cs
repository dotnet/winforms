// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResXResourceWriterTests
{
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
        string fileName = "test.resx";

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(fileName);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_FileNameAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        string fileName = "test.resx";
        Func<Type?, string> typeNameConverter = type => type?.AssemblyQualifiedName;

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(fileName, typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_Stream_CreatesInstance()
    {
        // Arrange
        Stream stream = new MemoryStream();

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(stream);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_StreamAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        Stream stream = new MemoryStream();
        Func<Type?, string> typeNameConverter = type => type?.AssemblyQualifiedName;

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(stream, typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriter_CreatesInstance()
    {
        // Arrange
        TextWriter textWriter = new StringWriter();

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(textWriter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_Constructor_TextWriterAndTypeNameConverter_CreatesInstance()
    {
        // Arrange
        TextWriter textWriter = new StringWriter();
        Func<Type?, string> typeNameConverter = type => type?.AssemblyQualifiedName;

        // Act
        ResXResourceWriter writer = new ResXResourceWriter(textWriter, typeNameConverter);

        // Assert
        Assert.NotNull(writer);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ByteValue_AddsMetadataToResourceFile()
    {
        // Arrange
        string name = "TestMetaData";
        byte[] value = new byte[] { 1, 2, 3 };
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddMetadata(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_StringValue_AddsMetadataToResourceFile()
    {
        // Arrange
        string name = "TestMetaData";
        string value = "TestString";
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddMetadata(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddMetadata_ObjectValue_AddsMetadataToResourceFile()
    {
        // Arrange
        string name = "TestMetaData";
        object value = new DateTime(2023, 05, 04);
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddMetadata(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var metadataEnumerator = reader.GetMetadataEnumerator();
        Assert.NotNull(metadataEnumerator);
        Assert.True(metadataEnumerator.MoveNext());

        var currentEntry = (DictionaryEntry)metadataEnumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ByteValue_AddsResourceToResourceFile()
    {
        // Arrange
        string name = "TestResource";
        byte[] value = new byte[] { 1, 2, 3 };
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddResource(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_StringValue_AddsResourceToResourceFile()
    {
        // Arrange
        string name = "TestResource";
        string value = "TestString";
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddResource(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ObjectValue_AddsResourceToResourceFile()
    {
        // Arrange
        string name = "TestResource";
        object value = new DateTime(2023, 05, 04);
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddResource(name, value);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }

    [Fact]
    public void ResXResourceWriter_AddResource_ResxDataNodeValue_AddsResourceToResourceFile()
    {
        // Arrange
        string name = "TestResource";
        object value = new DateTime(2023, 05, 04);
        ResXDataNode dataNode = new ResXDataNode(name, value);
        ResXResourceWriter writer = new ResXResourceWriter("test.resx");

        // Act
        writer.AddResource(name, dataNode);
        writer.Generate();
        writer.Close();

        // Assert
        using ResXResourceReader reader = new ResXResourceReader("test.resx");
        var enumerator = reader.GetEnumerator();
        Assert.NotNull(enumerator);
        Assert.True(enumerator.MoveNext());

        var currentEntry = (DictionaryEntry)enumerator.Current;
        Assert.Equal(name, currentEntry.Key);
        Assert.Equal(value, currentEntry.Value);
    }
}
