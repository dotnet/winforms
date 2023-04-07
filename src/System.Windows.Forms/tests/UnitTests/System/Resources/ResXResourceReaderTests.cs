// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Text;
using System.Windows.Forms;
using Xunit;

namespace System.Resources.Tests;

public class ResXResourceReaderTests
{
    [Fact]
    public void ResXResourceReader_Deserialize_AxHost_FormatterEnabled_Throws()
    {
        using var formatterScope = new BinaryFormatterScope(enable: true);

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
        using var formatterScope = new BinaryFormatterScope(enable: false);

        string resxPath = Path.GetFullPath(@".\Resources\AxHosts.resx");
        using MemoryStream resourceStream = new();

        using ResXResourceReader resxReader = new(resxPath);
        Assert.Throws<ArgumentException>(() => resxReader.GetEnumerator());
    }

    private static string CreateResx(string data = null) =>
    $"""
        <root>
            <resheader name="resmimetype">
                <value>text/microsoft-resx</value>
            </resheader>
            <resheader name="version">
                <value>2.0</value>
            </resheader>
            <resheader name="reader">
                <value>System.Resources.ResXResourceReader</value>
            </resheader>
            <resheader name="writer">
                <value>System.Resources.ResXResourceWriter</value>
            </resheader>
            {data ?? string.Empty}
        </root>
        """;

    [Fact]
    public void ResXResourceReader_Constructor_FileName()
    {
        // Create a temp file and write the resx to it.
        string tempFileName = Path.GetTempFileName();
        File.WriteAllText(tempFileName, CreateResx());

        try
        {
            using ResXResourceReader resXReader = new(tempFileName);
            IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
            Assert.NotNull(enumerator);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public void ResXResourceReader_Constructor_TextReader()
    {
        using TextReader textReader = new StringReader(CreateResx());
        using ResXResourceReader resXReader = new(textReader);
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_Constructor_Stream()
    {
        byte[] resxBytes = Encoding.UTF8.GetBytes(CreateResx());
        using Stream resxStream = new MemoryStream(resxBytes);
        using ResXResourceReader resXReader = new ResXResourceReader(resxStream);
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_FromFileContents()
    {
        using var resXReader = ResXResourceReader.FromFileContents(CreateResx());
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();
        Assert.NotNull(enumerator);
    }

    [Fact]
    public void ResXResourceReader_GetEnumerator()
    {
        const string data = """
            <data name="TestName1" xml:space="preserve">
              <value>TestValue1</value>
            </data>
            <data name="TestName2" xml:space="preserve">
              <value>TestValue2</value>
            </data>
            """;

        using var resXReader = ResXResourceReader.FromFileContents(CreateResx(data));
        IDictionaryEnumerator enumerator = resXReader.GetEnumerator();

        int itemCount = 0;

        while (enumerator.MoveNext())
        {
            itemCount++;
            string key = (string)enumerator.Key;
            string value = (string)enumerator.Value;
            Assert.True(!string.IsNullOrEmpty(key));
            Assert.True(!string.IsNullOrEmpty(value));
        }

        Assert.Equal(2, itemCount);
    }
}
