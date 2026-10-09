// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    public static TheoryData<string, string> ConvertibleTextFormatsTheoryData => new()
    {
        { DataFormats.Text, DataFormats.UnicodeText },
        { DataFormats.Text, DataFormats.StringFormat },
        { DataFormats.UnicodeText, DataFormats.Text },
        { DataFormats.UnicodeText, DataFormats.StringFormat },
        { DataFormats.StringFormat, DataFormats.Text },
        { DataFormats.StringFormat, DataFormats.UnicodeText }
    };

    // Verifies every direction in the mapped text-format group remains invisible to the exact-format APIs.
    [WinFormsTheory]
    [MemberData(nameof(ConvertibleTextFormatsTheoryData))]
    public void Clipboard_ContainsAndGetData_ConvertibleOnlyTextFormat_DoNotAutoConvert(
        string storedFormat,
        string requestedFormat)
    {
        DataObject data = new();
        data.SetData(storedFormat, autoConvert: true, "text");

        Assert.True(data.GetDataPresent(requestedFormat, autoConvert: true));
        Assert.False(data.GetDataPresent(requestedFormat, autoConvert: false));
        Assert.Equal("text", data.GetData(requestedFormat, autoConvert: true));
        Assert.Null(data.GetData(requestedFormat, autoConvert: false));

        Clipboard.SetDataObject(data, copy: false);

        Assert.Same(data, Clipboard.GetDataObject());
        Assert.False(Clipboard.ContainsData(requestedFormat));
        Assert.Null(Clipboard.GetData(requestedFormat));
        Assert.True(Clipboard.ContainsData(storedFormat));
        Assert.Equal("text", Clipboard.GetData(storedFormat));
    }

    public static TheoryData<string, TextDataFormat> ConvertibleTypedTextFormatsTheoryData => new()
    {
        { DataFormats.Text, TextDataFormat.UnicodeText },
        { DataFormats.StringFormat, TextDataFormat.UnicodeText },
        { DataFormats.UnicodeText, TextDataFormat.Text },
        { DataFormats.StringFormat, TextDataFormat.Text }
    };

    // Verifies both mapped typed-text targets use exact-format lookup rather than DataObject conversion.
    [WinFormsTheory]
    [MemberData(nameof(ConvertibleTypedTextFormatsTheoryData))]
    public void Clipboard_ContainsAndGetText_ConvertibleOnlyFormat_DoNotAutoConvert(
        string storedFormat,
        TextDataFormat requestedFormat)
    {
        DataObject data = new();
        data.SetData(storedFormat, autoConvert: true, "text");

        Clipboard.SetDataObject(data, copy: false);

        Assert.False(Clipboard.ContainsText(requestedFormat));
        Assert.Equal(string.Empty, Clipboard.GetText(requestedFormat));
    }

    // Verifies that the format-less SetText overload publishes native Unicode text, including non-BMP characters.
    [WinFormsFact]
    public void Clipboard_SetText_DefaultOverload_StoresUnicodeTextFormat()
    {
        const string text = "Unicode \U0001F642";

        Clipboard.SetText(text);

        Assert.True(Clipboard.ContainsText(TextDataFormat.UnicodeText));
        Assert.Equal(text, Clipboard.GetText(TextDataFormat.UnicodeText));
        Assert.Equal(text, Clipboard.GetData(DataFormats.UnicodeText));

        IDataObject dataObject = Assert.IsAssignableFrom<IDataObject>(Clipboard.GetDataObject());
        Assert.Contains(DataFormats.UnicodeText, dataObject.GetFormats(autoConvert: false));
        Assert.Equal(text, dataObject.GetData(DataFormats.UnicodeText, autoConvert: false));
    }

    // Verifies the neutral results returned by every typed text reader when only non-text data is present.
    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_TextFormats_NoMatchingData_ReturnFalseAndEmpty(TextDataFormat format)
    {
        Clipboard.SetData("WinForms.ClipboardTests.NonText", 42);

        Assert.False(Clipboard.ContainsText(format));
        Assert.Equal(string.Empty, Clipboard.GetText(format));
    }

    // Verifies that formats treated as opaque text preserve their payload exactly rather than normalizing it.
    [WinFormsTheory]
    [InlineData(@"{\rtf1\ansi WinForms clipboard text}", TextDataFormat.Rtf)]
    [InlineData("<html><body>WinForms clipboard text</body></html>", TextDataFormat.Html)]
    [InlineData("alpha,\"beta,gamma\"\r\n", TextDataFormat.CommaSeparatedValue)]
    public void Clipboard_SetText_OpaqueFormatPayload_RoundTrips(string text, TextDataFormat format)
    {
        Clipboard.SetText(text, format);

        Assert.True(Clipboard.ContainsText(format));
        Assert.Equal(text, Clipboard.GetText(format));
    }

    // Verifies that text validation wins when both the text and requested format are invalid.
    [Theory] // x-thread; text validation must short-circuit before enum and clipboard access.
    [InlineData(null)]
    [InlineData("")]
    public void Clipboard_SetText_NullOrEmptyTextWithInvalidFormat_ValidatesTextFirst(string? text)
    {
        Action action = () => Clipboard.SetText(text!, (TextDataFormat)(-1));
        action.Should().Throw<ArgumentNullException>().WithParameterName("text");
    }

    // Verifies that format validation occurs locally and does not get masked by the MTA apartment check.
    [Theory] // x-thread; enum validation must short-circuit before clipboard access.
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_SetText_ValidTextWithInvalidFormat_ValidatesFormatBeforeSta(TextDataFormat format)
    {
        Action action = () => Clipboard.SetText("text", format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }
}
