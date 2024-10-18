// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // Each registered Clipboard format is an OS singleton,
                           // and we should not run this test at the same time as other tests using the same format.
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public class ClipboardComTests
{
    [WinFormsFact]
    public void Clipboard_SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");

        Clipboard.GetText().Should().Be("text");
        Clipboard.ContainsText().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetDataAsJson_ReturnsExpected()
    {
        Point point = new() { X = 1, Y = 1 };

        Clipboard.SetDataAsJson("point", point);
        IDataObject? dataObject = Clipboard.GetDataObject();
        dataObject.Should().NotBeNull();
        dataObject!.GetDataPresent("point").Should().BeTrue();
        Point deserialized = dataObject.GetData("point").Should().BeOfType<Point>().Which;
        deserialized.Should().BeEquivalentTo(point);
    }

    [WinFormsTheory]
    [BoolData]
    public void Clipboard_SetDataObject_WithJson_ReturnsExpected(bool copy)
    {
        Point point = new() { X = 1, Y = 1 };

        DataObject dataObject = new();
        dataObject.SetDataAsJson("point", point);

        Clipboard.SetDataObject(dataObject, copy);
        IDataObject? returnedDataObject = Clipboard.GetDataObject();
        returnedDataObject.Should().NotBeNull();
        Point deserialized = returnedDataObject!.GetData("point").Should().BeOfType<Point>().Which;
        deserialized.Should().BeEquivalentTo(point);
    }
}
