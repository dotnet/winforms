// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

internal unsafe class TestDataObject<TOleServices> :
    IComVisibleDataObject,
    ITestDataObject,
    IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>
    where TOleServices : IOleServices
{
    private readonly Composition<TOleServices, CoreNrbfSerializer, TestFormat> _innerData;

    static TestDataObject<TOleServices> IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>.Create() =>
        new();
    static TestDataObject<TOleServices> IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>.Create(IDataObject* dataObject) =>
        new(dataObject);
    static TestDataObject<TOleServices> IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>.Create(object data) =>
        new(data);
    static IDataObjectInternal IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>.Wrap(ITestDataObject data) =>
        new TestDataObjectAdapter(data);

    bool IDataObjectInternal<TestDataObject<TOleServices>, ITestDataObject>.TryUnwrapUserDataObject([NotNullWhen(true)] out ITestDataObject? dataObject)
        => TryUnwrapUserDataObject(out dataObject);

    internal TestDataObject(IDataObject* data) =>
        _innerData = Composition<TOleServices, CoreNrbfSerializer, TestFormat>.Create(data);

    internal TestDataObject() =>
        _innerData = Composition<TOleServices, CoreNrbfSerializer, TestFormat>.Create();

    internal TestDataObject(object data) =>
        _innerData = Composition<TOleServices, CoreNrbfSerializer, TestFormat>
            .Create<TestDataObject<TOleServices>, ITestDataObject>(data);

    internal TestDataObject(string format, object data) : this() => SetData(format, data);

    internal TestDataObject(string format, bool autoConvert, object data) : this() => SetData(format, autoConvert, data);

    internal virtual bool TryUnwrapUserDataObject([NotNullWhen(true)] out ITestDataObject? dataObject)
    {
        dataObject = _innerData.ManagedDataObject switch
        {
            TestDataObject<TOleServices> data => data,
            TestDataObjectAdapter adapter => adapter.DataObject,
            DataStore<TOleServices> => this,
            _ => null
        };

        return dataObject is not null;
    }

    public virtual object? GetData(string format) => GetData(format, autoConvert: true);
    public virtual object? GetData(Type format) => format is null ? null : GetData(format.FullName!);
    public virtual object? GetData(string format, bool autoConvert) => _innerData.GetData(format, autoConvert);
    public virtual bool GetDataPresent(string format, bool autoConvert) => _innerData.GetDataPresent(format, autoConvert);
    public virtual bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);
    public virtual bool GetDataPresent(Type format) => format is not null && GetDataPresent(format.FullName!);
    public virtual string[] GetFormats(bool autoConvert) => _innerData.GetFormats(autoConvert);
    public virtual string[] GetFormats() => GetFormats(autoConvert: true);
    public virtual void SetData(string format, bool autoConvert, object? data) => _innerData.SetData(format, autoConvert, data);
    public virtual void SetData(string format, object? data) => _innerData.SetData(format, data);
    public virtual void SetData(Type format, object? data) => _innerData.SetData(format, data);
    public virtual void SetData(object? data) => _innerData.SetData(data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        TryGetDataInternal(typeof(T).FullName!, resolver: null, autoConvert: true, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        TryGetDataInternal(format, resolver: null, autoConvert: true, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        TryGetDataInternal(format, resolver: null, autoConvert: autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        TryGetDataInternal(typeof(T).FullName!, resolver: null, autoConvert: autoConvert, out data);

    internal bool TryGetDataInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;

        if (!ClipboardCore<TOleServices>.IsValidTypeForFormat(typeof(T), format))
        {
            // Resolver implementation is specific to the overridden TryGetDataCore method,
            // can't validate if a non-null resolver is required for unbounded types.
            return false;
        }

        // Invoke the appropriate overload so we don't fail a null check on a nested object if the resolver is null.
        return resolver is null
            ? _innerData.TryGetData(format, autoConvert, out data)
            : _innerData.TryGetData(format, resolver, autoConvert, out data);
    }

    // Native callbacks follow

    public unsafe HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium) => _innerData.GetData(pformatetcIn, pmedium);
    public unsafe HRESULT GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium) => _innerData.GetDataHere(pformatetc, pmedium);
    public unsafe HRESULT QueryGetData(FORMATETC* pformatetc) => _innerData.QueryGetData(pformatetc);
    public unsafe HRESULT GetCanonicalFormatEtc(FORMATETC* pformatectIn, FORMATETC* pformatetcOut) => _innerData.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
    public unsafe HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease) => _innerData.SetData(pformatetc, pmedium, fRelease);
    public unsafe HRESULT EnumFormatEtc(uint dwDirection, IEnumFORMATETC** ppenumFormatEtc) => _innerData.EnumFormatEtc(dwDirection, ppenumFormatEtc);
    public unsafe HRESULT DAdvise(FORMATETC* pformatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection) => _innerData.DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
    public HRESULT DUnadvise(uint dwConnection) => _innerData.DUnadvise(dwConnection);
    public unsafe HRESULT EnumDAdvise(IEnumSTATDATA** ppenumAdvise) => _innerData.EnumDAdvise(ppenumAdvise);
}
