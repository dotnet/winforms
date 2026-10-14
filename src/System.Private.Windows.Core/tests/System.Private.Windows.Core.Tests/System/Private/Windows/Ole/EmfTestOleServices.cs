// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

// The generic parameter gives each consuming test class isolated static observations and owned handles.
internal sealed unsafe class EmfTestOleServices<TTestClass> : IOleServices
{
    internal static List<(string Format, TYMED Tymed)> PlatformRequests { get; } = [];
    internal static List<nint> ManagedHandles { get; } = [];
    internal static List<nint> ReleasedHandles { get; } = [];
    internal static string? ManagedFormat { get; private set; }
    internal static object? ManagedData { get; private set; }
    internal static FORMATETC ManagedFormatEtc { get; private set; }
    internal static HRESULT ManagedGetDataResult { get; set; } = HRESULT.S_OK;
    internal static bool AllocateHandleBeforeFailure { get; set; }
    internal static int ConversionCount { get; private set; }
    internal static int ReleaseCount { get; private set; }

    internal static void Reset()
    {
        foreach (nint handle in ManagedHandles)
        {
            if (EmfNativeMethods.IsValid(handle))
            {
                EmfNativeMethods.Delete(handle).Should().BeTrue();
            }
        }

        PlatformRequests.Clear();
        ManagedHandles.Clear();
        ReleasedHandles.Clear();
        ManagedFormat = null;
        ManagedData = null;
        ManagedFormatEtc = default;
        ManagedGetDataResult = HRESULT.S_OK;
        AllocateHandleBeforeFailure = false;
        ConversionCount = 0;
        ReleaseCount = 0;
    }

    static bool IOleServices.AllowTypeWithoutResolver<T>() => true;
    static void IOleServices.EnsureThreadState() { }
    static bool IOleServices.IsNativeTymedSupported(string format, TYMED tymed) =>
        format == DataFormatNames.Emf && tymed.HasFlag(TYMED.TYMED_ENHMF);
    static bool IOleServices.IsValidTypeForFormat(Type type, string format) => true;
    static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) { }

    static HRESULT IOleServices.GetDataHere(
        string format,
        object data,
        FORMATETC* pformatetc,
        STGMEDIUM* pmedium)
    {
        ManagedFormat = format;
        ManagedData = data;
        ManagedFormatEtc = *pformatetc;

        if (ManagedGetDataResult.Failed)
        {
            if (AllocateHandleBeforeFailure)
            {
                nint failedHandle = EmfNativeMethods.CreateDeterministicEnhMetafile();
                ManagedHandles.Add(failedHandle);
                pmedium->tymed = TYMED.TYMED_ENHMF;
                pmedium->hGlobal = (HGLOBAL)failedHandle;
            }

            return ManagedGetDataResult;
        }

        if (format != DataFormatNames.Emf
            || data is not EmfPayload
            || !((TYMED)pformatetc->tymed).HasFlag(TYMED.TYMED_ENHMF)
            || pmedium->tymed != TYMED.TYMED_ENHMF)
        {
            return HRESULT.DV_E_TYMED;
        }

        nint handle = EmfNativeMethods.CreateDeterministicEnhMetafile();
        ManagedHandles.Add(handle);
        pmedium->hGlobal = (HGLOBAL)handle;

        return HRESULT.S_OK;
    }

    static bool IOleServices.TryGetObjectFromDataObject<T>(
        IDataObject* dataObject,
        string format,
        [NotNullWhen(true)] out T data)
    {
        data = default!;
        FORMATETC formatEtc = new()
        {
            cfFormat = (ushort)DataFormatsCore<TestFormat>.GetOrAddFormat(format).Id,
            dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = (uint)TYMED.TYMED_ENHMF
        };

        PlatformRequests.Add((format, TYMED.TYMED_ENHMF));
        HRESULT result = dataObject->QueryGetData(formatEtc);
        if (result.Failed)
        {
            return false;
        }

        result = dataObject->GetData(formatEtc, out STGMEDIUM medium);
        if (result.Failed)
        {
            return false;
        }

        nint handle = (nint)medium.hGlobal;
        try
        {
            medium.tymed.Should().Be(TYMED.TYMED_ENHMF);
            EmfNativeMethods.IsValid(handle).Should().BeTrue();
            ConversionCount++;

            EmfPayload payload = EmfPayload.FromHandle(handle);
            if (payload is T typedPayload)
            {
                data = typedPayload;
                return true;
            }

            return false;
        }
        finally
        {
            // ReleaseStgMedium owns the provider-returned EMF handle after a successful GetData call.
            PInvokeCore.ReleaseStgMedium(ref medium);
            ReleasedHandles.Add(handle);
            ReleaseCount++;
        }
    }

    static IComVisibleDataObject IOleServices.CreateDataObject() => new TestDataObject<EmfTestOleServices<TTestClass>>();
    static HRESULT IOleServices.OleFlushClipboard() => HRESULT.E_NOTIMPL;
    static HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject) => HRESULT.E_NOTIMPL;
    static HRESULT IOleServices.OleSetClipboard(IDataObject* dataObject) => HRESULT.E_NOTIMPL;
}
