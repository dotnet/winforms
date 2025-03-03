// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Text.Json;
using Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Private.Windows.Ole;

/// <summary>
///  Contains the logic to move between <see cref="IDataObjectInternal"/>, <see cref="IDataObject.Interface"/>,
///  and <see cref="ComTypes.IDataObject"/> calls.
/// </summary>
internal sealed unsafe partial class Composition<TOleServices, TNrbfSerializer, TDataFormat>
    : IDataObjectInternal, IDataObject.Interface, ComTypes.IDataObject
    where TDataFormat : IDataFormat<TDataFormat>
    where TOleServices : IOleServices
    where TNrbfSerializer : INrbfSerializer
{
    private const TYMED AllowedTymeds = TYMED.TYMED_HGLOBAL | TYMED.TYMED_ISTREAM | TYMED.TYMED_GDI;

    // We use this to identify that a stream is actually a serialized object. On read, we don't know if the contents
    // of a stream were saved "raw" or if the stream is really pointing to a serialized object. If we saved an object,
    // we prefix it with this guid.
    private static readonly byte[] s_serializedObjectID =
    [
        // FD9EA796-3B13-4370-A679-56106BB288FB
        0x96, 0xa7, 0x9e, 0xfd,
        0x13, 0x3b,
        0x70, 0x43,
        0xa6, 0x79, 0x56, 0x10, 0x6b, 0xb2, 0x88, 0xfb
    ];

    internal IDataObjectInternal ManagedDataObject { get; }
    private readonly IDataObject.Interface _nativeDataObject;
    private readonly ComTypes.IDataObject _runtimeDataObject;

    private Composition(IDataObjectInternal managedDataObject, IDataObject.Interface nativeDataObject, ComTypes.IDataObject runtimeDataObject)
    {
        ManagedDataObject = managedDataObject;
        _nativeDataObject = nativeDataObject;
        _runtimeDataObject = runtimeDataObject;
    }

    internal static Composition<TOleServices, TNrbfSerializer, TDataFormat> Create() => Create(new DataStore<TOleServices>());

    internal static Composition<TOleServices, TNrbfSerializer, TDataFormat> Create<TDataObject, TIDataObject>(object data)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class
    {
        if (data is IDataObjectInternal internalDataObject)
        {
            return Create(internalDataObject);
        }
        else if (data is TIDataObject iDataObject)
        {
            return Create(TDataObject.Wrap(iDataObject));
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            return Create(comDataObject);
        }

        var composition = Create(new DataStore<TOleServices>());
        composition.SetData(data);
        return composition;
    }

    internal static Composition<TOleServices, TNrbfSerializer, TDataFormat> Create(IDataObjectInternal data)
    {
        ManagedToNativeAdapter winFormsToNative = new(data);

        // The NativeToRuntimeAdapter takes ownership of the native data object.
        NativeToRuntimeAdapter nativeToRuntime = new(ComHelpers.GetComPointer<IDataObject>(winFormsToNative));
        return new(data, winFormsToNative, nativeToRuntime);
    }

    internal static Composition<TOleServices, TNrbfSerializer, TDataFormat> Create(IDataObject* nativeDataObject)
    {
        // Add ref so each adapter can take ownership of the native data object.
        nativeDataObject->AddRef();
        nativeDataObject->AddRef();
        NativeToManagedAdapter nativeToWinForms = new(nativeDataObject);
        NativeToRuntimeAdapter nativeToRuntime = new(nativeDataObject);
        return new(nativeToWinForms, nativeToWinForms, nativeToRuntime);
    }

    internal static Composition<TOleServices, TNrbfSerializer, TDataFormat> Create(ComTypes.IDataObject runtimeDataObject)
    {
        RuntimeToNativeAdapter runtimeToNative = new(runtimeDataObject);

        // The NativeToManagedAdapter takes ownership of the native data object.
        NativeToManagedAdapter nativeToWinForms = new(ComHelpers.GetComPointer<IDataObject>(runtimeToNative));
        return new(nativeToWinForms, runtimeToNative, runtimeDataObject);
    }

    /// <summary>
    ///  Stores the data in the specified format using the <see cref="JsonSerializer"/>.
    /// </summary>
    /// <param name="format">The format associated with the data. See DataFormats for predefined formats.</param>
    /// <param name="data">The data to store.</param>
    /// <remarks>
    ///  <para>
    ///   The default behavior of <see cref="JsonSerializer"/> is used to serialize the data.
    ///  </para>
    ///  <para>
    ///   See
    ///   <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/how-to#serialization-behavior"/>
    ///   and <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/reflection-vs-source-generation#metadata-collection"/>
    ///   for more details on default <see cref="JsonSerializer"/> behavior.
    ///  </para>
    ///  <para>
    ///   If custom JSON serialization behavior is needed, manually JSON serialize the data and then use SetData,
    ///   or create a custom <see cref="Text.Json.Serialization.JsonConverter"/>, attach the
    ///   <see cref="Text.Json.Serialization.JsonConverterAttribute"/>, and then recall this method.
    ///   See <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/converters-how-to"/> for more details
    ///   on custom converters for JSON serialization.
    ///  </para>
    /// </remarks>
    /// <inheritdoc cref="DataObjectCore{TDataObject}.TryJsonSerialize{T}(string, T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T, TDataObject>(T data, string? format = default)
        where TDataObject : IComVisibleDataObject
    {
        format ??= typeof(T).FullName.OrThrowIfNull();
        SetData(
            format,
            autoConvert: false,
            DataObjectCore<TDataObject>.TryJsonSerialize(format, data));
    }

    #region IDataObjectInternal
    public object? GetData(string format, bool autoConvert)
    {
        object? result = ManagedDataObject.GetData(format, autoConvert);

        // Avoid exposing our internal JsonData<T>
        return result is IJsonData json ? json.Deserialize() : result;
    }

    public object? GetData(string format) => ManagedDataObject.GetData(format);
    public object? GetData(Type format) => ManagedDataObject.GetData(format);
    public bool GetDataPresent(string format, bool autoConvert) => ManagedDataObject.GetDataPresent(format, autoConvert);
    public bool GetDataPresent(string format) => ManagedDataObject.GetDataPresent(format);
    public bool GetDataPresent(Type format) => ManagedDataObject.GetDataPresent(format);
    public string[] GetFormats(bool autoConvert) => ManagedDataObject.GetFormats(autoConvert);
    public string[] GetFormats() => ManagedDataObject.GetFormats();
    public void SetData(string format, bool autoConvert, object? data) => ManagedDataObject.SetData(format, autoConvert, data);
    public void SetData(string format, object? data) => ManagedDataObject.SetData(format, data);
    public void SetData(Type format, object? data) => ManagedDataObject.SetData(format, data);
    public void SetData(object? data) => ManagedDataObject.SetData(data);
    #endregion

    #region ITypedDataObject
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            ManagedDataObject.TryGetData(format, resolver, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            ManagedDataObject.TryGetData(format, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            ManagedDataObject.TryGetData(format, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            ManagedDataObject.TryGetData(typeof(T).FullName.OrThrowIfNull(), out data);
    #endregion

    #region Com.IDataObject.Interface
    public HRESULT DAdvise(FORMATETC* pformatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection) => _nativeDataObject.DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
    public HRESULT DUnadvise(uint dwConnection) => _nativeDataObject.DUnadvise(dwConnection);
    public HRESULT EnumDAdvise(IEnumSTATDATA** ppenumAdvise) => _nativeDataObject.EnumDAdvise(ppenumAdvise);
    public HRESULT EnumFormatEtc(uint dwDirection, IEnumFORMATETC** ppenumFormatEtc) => _nativeDataObject.EnumFormatEtc(dwDirection, ppenumFormatEtc);
    public HRESULT GetCanonicalFormatEtc(FORMATETC* pformatectIn, FORMATETC* pformatetcOut) => _nativeDataObject.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
    public HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium) => _nativeDataObject.GetData(pformatetcIn, pmedium);
    public HRESULT GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium) => _nativeDataObject.GetDataHere(pformatetc, pmedium);
    public HRESULT QueryGetData(FORMATETC* pformatetc) => _nativeDataObject.QueryGetData(pformatetc);
    public HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease) => _nativeDataObject.SetData(pformatetc, pmedium, fRelease);
    #endregion

    #region ComTypes.IDataObject.Interface
    public int DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection) => _runtimeDataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
    public void DUnadvise(int connection) => _runtimeDataObject.DUnadvise(connection);
    public int EnumDAdvise(out ComTypes.IEnumSTATDATA? enumAdvise) => _runtimeDataObject.EnumDAdvise(out enumAdvise);
    public ComTypes.IEnumFORMATETC EnumFormatEtc(ComTypes.DATADIR direction) => _runtimeDataObject.EnumFormatEtc(direction);
    public int GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut) => _runtimeDataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
    public void GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => _runtimeDataObject.GetData(ref format, out medium);
    public void GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => _runtimeDataObject.GetDataHere(ref format, ref medium);
    public int QueryGetData(ref ComTypes.FORMATETC format) => _runtimeDataObject.QueryGetData(ref format);
    public void SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => _runtimeDataObject.SetData(ref formatIn, ref medium, release);
    #endregion
}
