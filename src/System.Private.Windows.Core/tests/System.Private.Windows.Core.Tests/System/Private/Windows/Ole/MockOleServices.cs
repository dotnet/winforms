// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Mock implementation of <see cref="IOleServices"/> for testing purposes.
/// </summary>
/// <typeparam name="TTestClass">Used to get an instance for each test class so test classes can run asynchronously.</typeparam>
internal class MockOleServices<TTestClass> : IOleServices
{
    private static AgileComPointer<IDataObject>? s_agileComPointer;

    static bool IOleServices.AllowTypeWithoutResolver<T>() => throw new NotImplementedException();
    static void IOleServices.EnsureThreadState() { }
    static unsafe HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium) => HRESULT.DV_E_TYMED;
    static bool IOleServices.IsValidTypeForFormat(Type type, string format) => true;
    static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) { }

    static unsafe bool IOleServices.TryGetBitmapFromDataObject<T>(IDataObject* dataObject, [NotNullWhen(true)] out T data)
    {
        data = default!;
        return false;
    }

    static HRESULT IOleServices.OleFlushClipboard()
    {
        // Would need to implement copying the raw TYMED data into a new object to mimic the real behavior.
        throw new NotImplementedException();
    }

    static unsafe HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject)
    {
        if (dataObject is null)
        {
            return HRESULT.E_POINTER;
        }

        if (s_agileComPointer is null)
        {
            *dataObject = null;
            return HRESULT.CLIPBRD_E_BAD_DATA;
        }

        *dataObject = s_agileComPointer.GetInterface().Value;
        return HRESULT.S_OK;
    }

    static unsafe HRESULT IOleServices.OleSetClipboard(IDataObject* dataObject)
    {
        if (dataObject is null)
        {
            // Clears the clipboard
            s_agileComPointer?.Dispose();
            s_agileComPointer = null;
            return HRESULT.S_OK;
        }

        s_agileComPointer?.Dispose();

        dataObject->AddRef();

        // Don't track disposal, we depend on finalization for testing.
        s_agileComPointer = new AgileComPointer<IDataObject>(dataObject, takeOwnership: true, trackDisposal: false);
        return HRESULT.S_OK;
    }

    static IComVisibleDataObject IOleServices.CreateDataObject() => new TestDataObject<MockOleServices<TTestClass>>();
}
