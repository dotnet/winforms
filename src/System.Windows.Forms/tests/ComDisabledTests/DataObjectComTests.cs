// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.TestUtilities.DataObjectTestHelpers;
using Com = Windows.Win32.System.Com;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests;

public unsafe partial class DataObjectTests
{
    private delegate IDataObject CreateWinFormsDataObjectForOutgoingDropData(Com.IDataObject* dataObject);

    [WinFormsFact]
    public void DataObject_WithJson_MockRoundTrip()
    {
        dynamic controlAccessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        SimpleTestData testData = new() { X = 1, Y = 1 };
        DataObject data = new();
        data.SetDataAsJson("testData", testData);

        DataObject inData = controlAccessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().BeSameAs(data);

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        ITypedDataObject typedOutData = outData.Should().BeAssignableTo<ITypedDataObject>().Subject;
        typedOutData.GetDataPresent("testData").Should().BeTrue();
        typedOutData.TryGetData("testData", out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsFact]
    public void DataObject_CustomIDataObject_MockRoundTrip()
    {
        CustomIDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = accessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().NotBeSameAs(data);

        // Simulate COM call. The COM call will eventually hit CreateWinFormsDataObjectForOutgoingDropData.
        // Note that this will be a ComWrappers created object since data has been wrapped in our DataObject.
        var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);

        outData.Should().BeSameAs(data);
    }

    [WinFormsFact]
    public void DataObject_ComTypesIDataObject_MockRoundTrip()
    {
        CustomComTypesDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = accessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().NotBeSameAs(data);

        // Simulate COM call. The COM call will eventually hit CreateWinFormsDataObjectForOutgoingDropData.
        // Note that this will not be a ComWrappers created object since IComDataObject does not get wrapped in our DataObject.
        var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);

        outData.Should().BeSameAs(inData);
    }

    private class CustomIDataObject : IDataObject
    {
        public object GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object GetData(string format) => throw new NotImplementedException();
        public object GetData(Type format) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        public void SetData(string format, object data) => throw new NotImplementedException();
        public void SetData(Type format, object data) => throw new NotImplementedException();
        public void SetData(object data) => throw new NotImplementedException();
    }

    private class CustomComTypesDataObject : IComDataObject
    {
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();
        public void GetData(ref FORMATETC format, out STGMEDIUM medium) => throw new NotImplementedException();
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
        public int QueryGetData(ref FORMATETC format) => throw new NotImplementedException();
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
    }
}
