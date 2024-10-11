﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Reflection.Metadata;
using FluentAssertions;
using Microsoft.VisualBasic.Devices;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests;

// Each registered Clipboard format is an OS singleton,
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")]
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public class ClipboardProxyTests
{
#pragma warning disable WFDEV005 // Type or member is obsolete
    private static string GetUniqueText() => Guid.NewGuid().ToString("D");

    [WinFormsFact]
    public void Audio()
    {
        var clipboard = new Computer().Clipboard;
        clipboard.ContainsAudio().Should().Be(System.Windows.Forms.Clipboard.ContainsAudio());
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
        clipboard.GetDataObject().GetData(DataFormats.UnicodeText).Should().Be(System.Windows.Forms.Clipboard.GetDataObject()?.GetData(DataFormats.UnicodeText));
    }

    [WinFormsFact]
    public void FileDropList()
    {
        var clipboard = new Computer().Clipboard;
        System.Windows.Forms.Clipboard.ContainsFileDropList().Should().Be(clipboard.ContainsFileDropList());
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

    [WinFormsFact]
    public void SetDataAsJson()
    {
        var clipboard = new Computer().Clipboard;
        Point point = new(1, 1);
        clipboard.SetDataAsJson("point", point);
        clipboard.ContainsData("point").Should().Be(System.Windows.Forms.Clipboard.ContainsData("point"));
        Point retrieved = clipboard.GetData("point").Should().BeOfType<Point>().Which;
        retrieved.Should().BeEquivalentTo(System.Windows.Forms.Clipboard.GetData("point"));
        retrieved.Should().BeEquivalentTo(point);
    }

    [WinFormsFact]
    public void DataOfT_StringArray()
        string format = nameof(DataOfT_StringArray);
        // Array of primitive types does not require the OOB assembly.
        string[] data = ["thing1", "thing2"];
        clipboard.SetData(format, data);
        // Both methods return true.
        clipboard.TryGetData(format, out string[]? actual).Should()
            .Be(System.Windows.Forms.Clipboard.TryGetData(format, out string[]? expected));
        actual.Should().BeEquivalentTo(expected);
    }

    [WinFormsFact]
    public void DataOfT_BinaryFormatterRequired()
    {
        var clipboard = new Computer().Clipboard;
        string format = nameof(DataOfT_BinaryFormatterRequired);
        DataWithObjectField data = new("thing1", "thing2");
        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        // This test assembly does not reference the OOB package, we will write the NotSupportedException to the clipboard.
        clipboard.SetData(format, data);
        // Both methods return false.
        Action tryGetData = () => clipboard.TryGetData(format, DataWithObjectField.Resolver, out DataWithObjectField? actual);
        string actual = tryGetData.Should().Throw<NotSupportedException>().Which.Message;
        Action tryGetData1 = () => System.Windows.Forms.Clipboard.TryGetData(format, DataWithObjectField.Resolver, out DataWithObjectField? expected);
        string expected = tryGetData1.Should().Throw<NotSupportedException>().Which.Message;
        actual.Should().BeEquivalentTo(expected);
    }

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

        public static Type Resolver(TypeName typeName) =>
            typeof(DataWithObjectField).FullName == typeName.FullName
                ? typeof(DataWithObjectField)
                : throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
        Point point = new(1, 1);
        clipboard.SetDataAsJson("point", point);
        clipboard.ContainsData("point").Should().Be(System.Windows.Forms.Clipboard.ContainsData("point"));
        Point retrieved = clipboard.GetData("point").Should().BeOfType<Point>().Which;
        retrieved.Should().BeEquivalentTo(System.Windows.Forms.Clipboard.GetData("point"));
        retrieved.Should().BeEquivalentTo(point);
    }
}
