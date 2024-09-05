// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Microsoft.VisualBasic.Devices;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests;

// Each registered Clipboard format is an OS singleton,
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")] 
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class ClipboardProxyTests
{
    private static string GetUniqueText() => Guid.NewGuid().ToString("D");

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
    public void FileDropList()
    {
        var clipboard = new Computer().Clipboard;
        System.Windows.Forms.Clipboard.ContainsFileDropList().Should().Be(clipboard.ContainsFileDropList());
        // Not tested:
        //   Public Function GetFileDropList() As StringCollection
        //   Public Sub SetFileDropList(filePaths As StringCollection)
    }

    [WinFormsFact]
    public void Image()
    {
        var clipboard = new Computer().Clipboard;
        using Bitmap image = new(2, 2);
        System.Windows.Forms.Clipboard.ContainsImage().Should().Be(clipboard.ContainsImage());
        System.Windows.Forms.Clipboard.GetImage().Should().Be(clipboard.GetImage());
        clipboard.SetImage(image);
    }

    [WinFormsFact]
    public void Text()
    {
        var clipboard = new Computer().Clipboard;
        string text = GetUniqueText();
        clipboard.SetText(text, TextDataFormat.UnicodeText);
        System.Windows.Forms.Clipboard.ContainsText().Should().Be(clipboard.ContainsText());
        System.Windows.Forms.Clipboard.GetText().Should().Be(clipboard.GetText());
        System.Windows.Forms.Clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(clipboard.GetText(TextDataFormat.UnicodeText));
        clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(text);
    }
}
