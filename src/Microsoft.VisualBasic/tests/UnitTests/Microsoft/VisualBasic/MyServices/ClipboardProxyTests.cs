// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using FluentAssertions;
using Microsoft.VisualBasic.Devices;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
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
        //   Public Sub SetAudio(audioBytes As Byte())
        //   Public Sub SetAudio(audioStream As Stream)
    }

    [WinFormsFact]
    public void FileDropList()
    {
        var clipboard = new Computer().Clipboard;
        clipboard.ContainsFileDropList().Should().Be(System.Windows.Forms.Clipboard.ContainsFileDropList());
        // Not tested:
        //   Public Function GetFileDropList() As StringCollection
        //   Public Sub SetFileDropList(filePaths As StringCollection)
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

    [WinFormsFact]
    public void SetDataAsJson()
    {
        var clipboard = new Computer().Clipboard;
        Point point = new(1, 1);
        clipboard.SetDataAsJson("point", point);
        clipboard.ContainsData("point").Should().Be(System.Windows.Forms.Clipboard.ContainsData("point"));
        Point retrieved = clipboard.GetData("point").Should().BeOfType<Point>().Which;
        retrieved.Should().Be(System.Windows.Forms.Clipboard.GetData("point"));
        retrieved.Should().Be(point);
    }

    private static string GetUniqueText() => Guid.NewGuid().ToString("D");
}
