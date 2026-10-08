// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Mock implementation of <see cref="IOleServices"/> for testing purposes.
/// </summary>
/// <typeparam name="TTestClass">Used to get an instance for each test class so test classes can run asynchronously.</typeparam>
internal class MockOleServices<TTestClass> : IOleServices
{
    private static DataObjectProxy? s_dataObjectProxy;

    public static Action? AfterOleFlushClipboard { get; set; }
    public static Action<bool>? AfterOleSetClipboard { get; set; }
    public static Action? BeforeOleIsCurrentClipboard { get; set; }
    public static HRESULT? NextOleFlushClipboardResult { get; set; }
    public static HRESULT? NextOleSetClipboardResult { get; set; }
    public static int OleIsCurrentClipboardCallCount { get; private set; }

    public static unsafe void SimulateExternalClipboardChange(IComVisibleDataObject dataObject)
    {
        using ComScope<IDataObject> iDataObject = ComHelpers.GetComScope<IDataObject>(dataObject);
        SetClipboard(iDataObject.Value).Should().Be(HRESULT.S_OK);
    }

    public static void ResetOleIsCurrentClipboardCallCount() => OleIsCurrentClipboardCallCount = 0;

    static bool IOleServices.AllowTypeWithoutResolver<T>() => true;
    static void IOleServices.EnsureThreadState() { }
    static unsafe HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium) => HRESULT.DV_E_TYMED;
    static bool IOleServices.IsValidTypeForFormat(Type type, string format) => true;
    static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) { }

    static unsafe bool IOleServices.TryGetObjectFromDataObject<T>(
        IDataObject* dataObject,
        string requestedFormat,
        [NotNullWhen(true)] out T data)
    {
        data = default!;
        return false;
    }

    static HRESULT IOleServices.OleFlushClipboard()
    {
        HRESULT result = NextOleFlushClipboardResult ?? HRESULT.S_OK;
        NextOleFlushClipboardResult = null;
        AfterOleFlushClipboard?.Invoke();

        return result;
    }

    static unsafe HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject)
    {
        if (dataObject is null)
        {
            return HRESULT.E_POINTER;
        }

        if (s_dataObjectProxy is null)
        {
            *dataObject = null;
            return HRESULT.CLIPBRD_E_BAD_DATA;
        }

        *dataObject = s_dataObjectProxy.Proxy;
        s_dataObjectProxy.Proxy->AddRef();
        return HRESULT.S_OK;
    }

    static unsafe HRESULT IOleServices.OleSetClipboard(IDataObject* dataObject)
    {
        HRESULT result = NextOleSetClipboardResult ?? SetClipboard(dataObject);
        NextOleSetClipboardResult = null;
        AfterOleSetClipboard?.Invoke(dataObject is null);

        return result;
    }

    private static unsafe HRESULT SetClipboard(IDataObject* dataObject)
    {
        if (dataObject is null)
        {
            // Clears the clipboard
            s_dataObjectProxy?.Dispose();
            s_dataObjectProxy = null;
            return HRESULT.S_OK;
        }

        s_dataObjectProxy?.Dispose();

        dataObject->AddRef();

        s_dataObjectProxy = new DataObjectProxy(dataObject);

        return HRESULT.S_OK;
    }

    public static unsafe HRESULT OleIsCurrentClipboard(IDataObject* dataObject)
    {
        OleIsCurrentClipboardCallCount++;
        BeforeOleIsCurrentClipboard?.Invoke();

        return s_dataObjectProxy is not null && s_dataObjectProxy.IsOriginal(dataObject)
            ? HRESULT.S_OK
            : HRESULT.S_FALSE;
    }

    static IComVisibleDataObject IOleServices.CreateDataObject() => new TestDataObject<MockOleServices<TTestClass>>();
}
