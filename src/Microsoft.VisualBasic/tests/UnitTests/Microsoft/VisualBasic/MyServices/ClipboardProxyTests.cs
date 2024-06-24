// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection.Metadata;
using FluentAssertions;
using Microsoft.VisualBasic.Devices;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests;

[Collection("Sequential")]
public class ClipboardProxyTests
{
    [WinFormsFact]
    public void Clear()
    {
        var clipboard = new Computer().Clipboard;
        string text = GetUniqueText();
        clipboard.SetText(text);
        System.Windows.Forms.Clipboard.ContainsText().Should().BeTrue();
        clipboard.Clear();
        System.Windows.Forms.Clipboard.ContainsText().Should().BeFalse();
    }

    [WinFormsFact]
    public void Text()
    {
        var clipboard = new Computer().Clipboard;
        string text = GetUniqueText();
        clipboard.SetText(text, TextDataFormat.UnicodeText);
        clipboard.ContainsText().Should().Be(System.Windows.Forms.Clipboard.ContainsText());
        clipboard.GetText().Should().Be(System.Windows.Forms.Clipboard.GetText());
        clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(System.Windows.Forms.Clipboard.GetText(TextDataFormat.UnicodeText));
        clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(text);
    }

    [WinFormsFact]
    public void Image()
    {
        var clipboard = new Computer().Clipboard;
        Bitmap image = new(2, 2);
        clipboard.SetImage(image);
        clipboard.ContainsImage().Should().Be(System.Windows.Forms.Clipboard.ContainsImage());
        clipboard.GetImage().Should().BeEquivalentTo(System.Windows.Forms.Clipboard.GetImage());
    }

    [WinFormsFact]
    public void Audio()
    {
        var clipboard = new Computer().Clipboard;
        clipboard.ContainsAudio().Should().Be(System.Windows.Forms.Clipboard.ContainsAudio());
        // Not tested:
        //   Public Function GetAudioStream() As Stream
        //   Public Sub SetAudio(ByVal audioBytes As Byte())
        //   Public Sub SetAudio(ByVal audioStream As Stream)
    }

    [WinFormsFact]
    public void FileDropList()
    {
        var clipboard = new Computer().Clipboard;
        clipboard.ContainsFileDropList().Should().Be(System.Windows.Forms.Clipboard.ContainsFileDropList());
        // Not tested:
        //   Public Function GetFileDropList() As StringCollection
        //   Public Sub SetFileDropList(ByVal filePaths As StringCollection)
    }

    [WinFormsFact]
    public void Data()
    {
        var clipboard = new Computer().Clipboard;
        object data = GetUniqueText();
        clipboard.SetData(DataFormats.UnicodeText, data);
        clipboard.ContainsData(DataFormats.UnicodeText).Should().Be(System.Windows.Forms.Clipboard.ContainsData(DataFormats.UnicodeText));
        clipboard.GetData(DataFormats.UnicodeText).Should().Be(System.Windows.Forms.Clipboard.GetData(DataFormats.UnicodeText));
    }

    [WinFormsFact]
    public void DataObject()
    {
        var clipboard = new Computer().Clipboard;
        object data = GetUniqueText();
        clipboard.SetDataObject(new System.Windows.Forms.DataObject(data));
        clipboard.GetDataObject().GetData(DataFormats.UnicodeText).Should().Be(System.Windows.Forms.Clipboard.GetDataObject().GetData(DataFormats.UnicodeText));
    }

#nullable enable
    [WinFormsFact]
    public void DataOfT_Bitmap()
    {
        var clipboard = new Computer().Clipboard;
        Bitmap data = new(16, 16);
        clipboard.SetDataAsJson(data);
        clipboard.TryGetData(out Bitmap? actual).Should().Be(System.Windows.Forms.Clipboard.TryGetData(out Bitmap? expected));
        actual.Should().BeEquivalentTo(expected);
    }

    [WinFormsFact]
    public void DataOfT_Text()
    {
        var clipboard = new Computer().Clipboard;
        string data = GetUniqueText();
        clipboard.SetData(DataFormats.Text, data);
        clipboard.TryGetData(DataFormats.Text, out string? actual).Should().Be(System.Windows.Forms.Clipboard.TryGetData(DataFormats.Text, out string? expected));
        actual.Should().Be(expected);
    }

    [WinFormsFact]
    public void DataOfT_CustomType()
    {
        var clipboard = new Computer().Clipboard;
        DataWithObjectField data = new("thing1", "thing2");
        clipboard.SetDataAsJson(data);
        clipboard.TryGetData(typeof(DataWithObjectField).FullName!, DataResolver, out DataWithObjectField? actual).Should()
            .Be(System.Windows.Forms.Clipboard.TryGetData(typeof(DataWithObjectField).FullName!, DataResolver, out DataWithObjectField? expected));
        actual.Should().BeEquivalentTo(expected);
    }

    [WinFormsFact]
    public void DataOfT_CustomType_BinaryFormatterRequired()
    {
        var clipboard = new Computer().Clipboard;
        DataWithObjectField data = new("thing1", "thing2");
        using BinaryFormatterScope scope = new(enable: true);
        clipboard.SetData(typeof(DataWithObjectField).FullName!, data);
        clipboard.TryGetData(typeof(DataWithObjectField).FullName!, DataResolver, out DataWithObjectField? actual).Should()
            .Be(System.Windows.Forms.Clipboard.TryGetData(typeof(DataWithObjectField).FullName!, DataResolver, out DataWithObjectField? expected));
        actual.Should().BeEquivalentTo(expected);
    }

#nullable disable

    private static string GetUniqueText() => Guid.NewGuid().ToString("D");

    [Serializable]
    private class DataWithObjectField
    {
        public DataWithObjectField(string text1, object object2)
        {
            _text1 = text1;
            _object2 = object2;
        }

        public string _text1;
        public object _object2;
    }

    private static Type DataResolver(TypeName typeName)
    {
        Type type = typeof(DataWithObjectField);
        TypeName parsed = TypeName.Parse($"{type.FullName}, {type.Assembly.FullName}");

        // Namespace-qualified type name.
        if (typeName.FullName == parsed.FullName
            // Ignore version, culture, and public key token in the assembly name.
            && typeName.AssemblyName?.Name == parsed.AssemblyName?.Name)
        {
            return type;
        }

        throw new NotSupportedException($"Unexpected type {typeName.AssemblyQualifiedName}.");
    }
}
