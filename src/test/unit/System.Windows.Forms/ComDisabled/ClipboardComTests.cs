// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;
using static System.Windows.Forms.TestUtilities.DataObjectTestHelpers;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Tests;

// Each registered Clipboard format is an OS singleton,
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")]
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
        SimpleTestData testData = new() { X = 1, Y = 1 };

        using BinaryFormatterScope scope = new(enable: false);
        Clipboard.SetDataAsJson("testData", testData);
        ITypedDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<ITypedDataObject>().Subject;
        dataObject.GetDataPresent("testData").Should().BeTrue();
        dataObject.TryGetData("testData", out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsTheory]
    [BoolData]
    public void Clipboard_SetDataObject_WithJson_ReturnsExpected(bool copy)
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };

        using BinaryFormatterScope scope = new(enable: false);
        DataObject dataObject = new();
        dataObject.SetDataAsJson("testData", testData);

        Clipboard.SetDataObject(dataObject, copy);
        ITypedDataObject returnedDataObject = Clipboard.GetDataObject().Should().BeAssignableTo<ITypedDataObject>().Subject;
        returnedDataObject.TryGetData("testData", out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsFact]
    public void Clipboard_GetSet_IDataObject_RoundTrip_ReturnsExpected()
    {
        CustomDataObject realDataObject = new();
        Clipboard.SetDataObject(realDataObject);

        IDataObject clipboardDataObject = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
        clipboardDataObject.Should().BeSameAs(realDataObject);
        clipboardDataObject.GetDataPresent("Foo").Should().BeTrue();
        clipboardDataObject.GetData("Foo").Should().Be("Bar");
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_DerivedDataObject_ReturnsExpected()
    {
        DerivedDataObject derived = new();
        Clipboard.SetDataObject(derived);
        Clipboard.GetDataObject().Should().BeSameAs(derived);
    }

    private class DerivedDataObject : DataObject { }

    private class CustomDataObject : IDataObject, ComTypes.IDataObject
    {
        [DllImport("shell32.dll")]
        public static extern int SHCreateStdEnumFmtEtc(uint cfmt, ComTypes.FORMATETC[] afmt, out ComTypes.IEnumFORMATETC ppenumFormatEtc);

        int ComTypes.IDataObject.DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        void ComTypes.IDataObject.DUnadvise(int connection) => throw new NotImplementedException();
        int ComTypes.IDataObject.EnumDAdvise(out ComTypes.IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
        ComTypes.IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(ComTypes.DATADIR direction)
        {
            if (direction == ComTypes.DATADIR.DATADIR_GET)
            {
                // Create enumerator and return it
                ComTypes.IEnumFORMATETC enumerator;
                if (SHCreateStdEnumFmtEtc(0, [], out enumerator) == 0)
                {
                    return enumerator;
                }
            }

            throw new NotImplementedException();
        }

        int ComTypes.IDataObject.GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut) => throw new NotImplementedException();
        object IDataObject.GetData(string format, bool autoConvert) => format == "Foo" ? "Bar" : null!;
        object IDataObject.GetData(string format) => format == "Foo" ? "Bar" : null!;
        object IDataObject.GetData(Type format) => null!;
        void ComTypes.IDataObject.GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        void ComTypes.IDataObject.GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        bool IDataObject.GetDataPresent(string format, bool autoConvert) => format == "Foo";
        bool IDataObject.GetDataPresent(string format) => format == "Foo";
        bool IDataObject.GetDataPresent(Type format) => false;
        string[] IDataObject.GetFormats(bool autoConvert) => ["Foo"];
        string[] IDataObject.GetFormats() => ["Foo"];
        int ComTypes.IDataObject.QueryGetData(ref ComTypes.FORMATETC format) => throw new NotImplementedException();
        void IDataObject.SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(string format, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(Type format, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(object? data) => throw new NotImplementedException();
        void ComTypes.IDataObject.SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => throw new NotImplementedException();
    }
}
