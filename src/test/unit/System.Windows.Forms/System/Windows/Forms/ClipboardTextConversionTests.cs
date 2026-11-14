// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// These public-API checks share the desktop clipboard and must not race other clipboard tests.
[Collection("Sequential")]
public class ClipboardTextConversionTests
{
    [WinFormsTheory]
    [InlineData("Text", false)]
    [InlineData("Text", true)]
    [InlineData("UnicodeText", false)]
    [InlineData("UnicodeText", true)]
    [InlineData("OEMText", false)]
    [InlineData("OEMText", true)]
    public void SetDataObject_TextWithoutAutoConvert_PreservesClipboardBehavior(string format, bool copy)
    {
        // Unlike WPF, WinForms already returns the original managed object for nonpersistent data in .NET 9.
        DataObject source = new();
        source.SetData(format, autoConvert: false, "test string");

        try
        {
            Clipboard.SetDataObject(source, copy);
            IDataObject result = Clipboard.GetDataObject()!;

            foreach (string target in new[] { DataFormats.Text, DataFormats.UnicodeText, DataFormats.OemText })
            {
                // Flushing uses the OLE proxy and exposes Windows synthesis; unflushed data retains managed semantics.
                bool present = copy || target == format;
                result.GetDataPresent(target).Should().Be(present);
                result.GetDataPresent(target, autoConvert: false).Should().Be(present);
                result.GetData(target, autoConvert: false).Should().Be(present ? "test string" : null);
                Clipboard.ContainsData(target).Should().Be(present);
            }

            bool unicodePresent = copy || format == DataFormats.UnicodeText;
            Clipboard.ContainsText().Should().Be(unicodePresent);
            Clipboard.ContainsText(TextDataFormat.UnicodeText).Should().Be(unicodePresent);
            Clipboard.GetText().Should().Be(unicodePresent ? "test string" : string.Empty);
            source.GetFormats(autoConvert: false).Should().Equal(format);
        }
        finally
        {
            Clipboard.Clear();
        }
    }
}
