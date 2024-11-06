// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Text;
using System.Windows.Forms.TestUtilities;

namespace System.Resources.Tests;

public class ResXResourceReaderTests
{
    [Fact]
    public void ResXResourceReader_Deserialize_AxHost_FormatterEnabled_Throws()
    {
        using BinaryFormatterScope formatterScope = new(enable: true);

        string resxPath = Path.GetFullPath(@".\Resources\AxHosts.resx");
        using MemoryStream resourceStream = new();

        // ResourceWriter Dispose calls Generate method which will throw.
        Assert.Throws<PlatformNotSupportedException>(() =>
        {
            using ResourceWriter resourceWriter = new(resourceStream);
            using ResXResourceReader resxReader = new(resxPath);
            var enumerator = resxReader.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            string key = enumerator.Key.ToString();
            object value = enumerator.Value;
            Assert.Equal("axWindowsMediaPlayer1.OcxState", key);
            Assert.Equal(typeof(AxHost.State), value.GetType());
            Assert.False(enumerator.MoveNext());

            resourceWriter.AddResource(key, value);

            // ResourceWriter no longer supports BinaryFormatter in core.
            Assert.Throws<PlatformNotSupportedException>(resourceWriter.Generate);
        });
    }

    [Fact]
    public void ResXResourceReader_Deserialize_AxHost_FormatterDisabled_Throws()
    {
        using BinaryFormatterScope formatterScope = new(enable: false);

        string resxPath = Path.GetFullPath(@".\Resources\AxHosts.resx");
        using MemoryStream resourceStream = new();

        using ResXResourceReader resxReader = new(resxPath);
        Assert.Throws<ArgumentException>(resxReader.GetEnumerator);
    }

    [Fact]
    public void ResXResourceReader_Constructor_FileName()
    {
        // Create a temp file and write the resx to it.
        using TempFile tempFile = TempFile.Create(ResxHelper.CreateResx());
        using ResXResourceReader resXReader = new(tempFile.Path);
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_Constructor_TextReader()
    {
        using TextReader textReader = new StringReader(ResxHelper.CreateResx());
        using ResXResourceReader resXReader = new(textReader);
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_Constructor_Stream()
    {
        byte[] resxBytes = Encoding.UTF8.GetBytes(ResxHelper.CreateResx());
        using Stream resxStream = new MemoryStream(resxBytes);
        using ResXResourceReader resXReader = new(resxStream);
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_FromFileContents()
    {
        using var resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx());
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_GetEnumerator_String()
    {
        // Arrange
        string keyName1 = "TestString";
        string keyName2 = "TestString2";
        string testValue1 = "TestValue1";
        string testValue2 = "TestValue2";

        IDictionary expectedData = new Hashtable { { keyName1, testValue1 }, { keyName2, testValue2 } };
        string data = $"""
            <data name="{keyName1}" xml:space="preserve">
              <value>{testValue1}</value>
            </data>
            <data name="{keyName2}" xml:space="preserve">
              <value>{testValue2}</value>
            </data>
            """;

        // Act
        using var resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx(data));
        IDictionary actualData = new Hashtable();
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();

        while (enumerator.MoveNext())
        {
            actualData.Add(enumerator.Key, enumerator.Value);
        }

        // Assert
        Assert.Equal(expectedData, actualData);
    }

    [Fact]
    public void ResXResourceReader_GetEnumerator_Int()
    {
        // Arrange
        string keyName = "TestNumber";
        int testValue = 42;

        IDictionary expectedData = new Hashtable { { keyName, testValue } };
        string data = $"""
            <data name="{keyName}" type="System.Int32">
              <value>{testValue}</value>
            </data>
            """;

        // Act
        using var resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx(data));
        IDictionary actualData = new Hashtable();
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        while (enumerator.MoveNext())
        {
            actualData.Add(enumerator.Key, enumerator.Value);
        }

        // Assert
        Assert.Equal(expectedData, actualData);
    }

    [Fact]
    public void GetMetadataEnumerator_ReturnsMetadataNodes()
    {
        // Arrange
        string metadataName = "TestMetadata";
        string sampleMetadataValue = "Sample metadata";

        IDictionary expectedMetadata = new Hashtable { { metadataName, sampleMetadataValue } };
        string metadata = $"""
            <metadata name="{metadataName}" type="System.String">
              <value>{sampleMetadataValue}</value>
            </metadata>
            """;

        // Act
        using ResXResourceReader resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx(metadata));
        IDictionary actualMetadata = new Hashtable();
        IDictionaryEnumerator metadataEnumerator = resXReader.GetMetadataEnumerator();
        while (metadataEnumerator.MoveNext())
        {
            actualMetadata.Add(metadataEnumerator.Key, metadataEnumerator.Value);
        }

        // Assert
        Assert.Equal(expectedMetadata, actualMetadata);
    }

    [Fact]
    public void ResXResourceReader_GetEnumerator_UnknownType_ThowsException()
    {
        // Arrange
        string unknownType = "System.XYZ";
        string data = $"""
            <data name="UnknownType" type="{unknownType}">
              <value>UnknownValueType</value>
            </data>
            """;

        // Act
        using var resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx(data));

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => { IDictionaryEnumerator enumerator = resXReader.GetEnumerator(); });
        Assert.Contains($"ResX file Type {unknownType} in the data", exception.Message);
    }

    [Fact]
    public void ResXResourceReader_GetEnumerator_InvalidValue_ThowsException()
    {
        // Arrange
        string keyName = "TestKey";
        string testValue = "FortyTwo";

        IDictionary expectedData = new Hashtable { { keyName, testValue } };
        string data = $"""
            <data name="{keyName}" type="System.Int32">
              <value>{testValue}</value>
            </data>
            """;

        // Act
        using var resXReader = ResXResourceReader.FromFileContents(ResxHelper.CreateResx(data));

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        });
        Assert.Contains($"ResX file {testValue} is not a valid value for Int32", exception.Message);
    }
}
