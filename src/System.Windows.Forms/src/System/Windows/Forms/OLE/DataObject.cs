// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms.Ole;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
public unsafe partial class DataObject :
    ITypedDataObject,
    IDataObjectInternal<DataObject, IDataObject>,
    // Built-in COM interop chooses the first interface that implements an IID,
    // we want the CsWin32 to be chosen over System.Runtime.InteropServices.ComTypes
    // so it must come first.
    Com.IDataObject.Interface,
    ComTypes.IDataObject,
    Com.IManagedWrapper<Com.IDataObject>,
    IComVisibleDataObject
{
    private readonly Composition _innerData;

    static DataObject IDataObjectInternal<DataObject, IDataObject>.Create() => new();
    static DataObject IDataObjectInternal<DataObject, IDataObject>.Create(Com.IDataObject* dataObject) => new(dataObject);
    static DataObject IDataObjectInternal<DataObject, IDataObject>.Create(object data) => new(data);

    static IDataObjectInternal IDataObjectInternal<DataObject, IDataObject>.Wrap(IDataObject data) =>
        new DataObjectAdapter(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the raw <see cref="Com.IDataObject"/>
    ///  and the managed data object the raw pointer is associated with.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method will add a reference to the <paramref name="data"/> pointer.
    ///  </para>
    /// </remarks>
    /// <inheritdoc cref="DataObject(object)"/>
    internal DataObject(Com.IDataObject* data) => _innerData = Composition.Create(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    /// <inheritdoc cref="DataObject(object)"/>
    public DataObject() => _innerData = Composition.Create();

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If <paramref name="data"/> implements an <see cref="IDataObject"/> interface,
    ///   we strongly recommend implementing <see cref="ITypedDataObject"/> interface to support the
    ///   `TryGetData{T}` API family that restricts deserialization to the requested and known types.
    ///   <see cref="Clipboard.TryGetData{T}(string, out T)"/> will throw <see cref="NotSupportedException"/>
    ///   if <see cref="ITypedDataObject"/> is not implemented.
    ///  </para>
    /// </remarks>
    public DataObject(object data) => _innerData = Composition.Create<DataObject, IDataObject>(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data and its
    ///  associated format.
    /// </summary>
    public DataObject(string format, object data) : this() => SetData(format, data.OrThrowIfNull());

    internal DataObject(string format, bool autoConvert, object data) : this() =>
        SetData(format, autoConvert, data.OrThrowIfNull());

    bool IDataObjectInternal<DataObject, IDataObject>.TryUnwrapUserDataObject([NotNullWhen(true)] out IDataObject? dataObject) =>
        TryUnwrapUserDataObject(out dataObject);

    internal virtual bool TryUnwrapUserDataObject([NotNullWhen(true)] out IDataObject? dataObject)
    {
        dataObject = _innerData.ManagedDataObject switch
        {
            DataObject data => data,
            DataObjectAdapter adapter => adapter.DataObject,
            DataStore<WinFormsOleServices> => this,
            _ => null
        };

        return dataObject is not null;
    }

    /// <inheritdoc cref="Composition.SetDataAsJson{T, TDataObject}(T, string)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(string format, T data) =>
        _innerData.SetDataAsJson<T, DataObject>(data, format);

    /// <inheritdoc cref="SetDataAsJson{T}(T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(T data) =>
        _innerData.SetDataAsJson<T, DataObject>(data);

    #region IDataObject
    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format, bool autoConvert) => _innerData.GetData(format, autoConvert);

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format) => GetData(format, autoConvert: true);

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(Type format) => format is null ? null : GetData(format.FullName!);

    public virtual bool GetDataPresent(string format, bool autoConvert) => _innerData.GetDataPresent(format, autoConvert);

    public virtual bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public virtual bool GetDataPresent(Type format) => format is not null && GetDataPresent(format.FullName!);

    public virtual string[] GetFormats(bool autoConvert) => _innerData.GetFormats(autoConvert);

    public virtual string[] GetFormats() => GetFormats(autoConvert: true);

    public virtual void SetData(string format, bool autoConvert, object? data) =>
        _innerData.SetData(format, autoConvert, data);

    public virtual void SetData(string format, object? data) => _innerData.SetData(format, data);

    public virtual void SetData(Type format, object? data) => _innerData.SetData(format, data);

    public virtual void SetData(object? data) => _innerData.SetData(data);
    #endregion

    #region ITypedDataObject
    /// <inheritdoc cref="Clipboard.TryGetData{T}(string, Func{TypeName, Type}, out T)"/>
    [CLSCompliant(false)]
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        resolver.OrThrowIfNull();

        return TryGetDataInternal(format, resolver, autoConvert, out data);
    }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(format, resolver: null, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(format, resolver: null, autoConvert: true, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetDataInternal(typeof(T).FullName!, resolver: null, autoConvert: true, out data);
    #endregion

    /// <summary>
    ///  Override this method in the derived class to provide custom data retrieval logic using the typed APIs.
    /// </summary>
    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(string, Func{TypeName, Type}, bool, out T)" />
    [CLSCompliant(false)]
    protected virtual bool TryGetDataCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
        // Invoke the appropriate overload so we don't fail a null check on a nested object if the resolver is null.
        resolver is null
            ? _innerData.TryGetData(format, autoConvert, out data)
            : _innerData.TryGetData(format, resolver, autoConvert, out data);

    public virtual bool ContainsAudio() => GetDataPresent(DataFormatNames.WaveAudio, autoConvert: false);

    public virtual bool ContainsFileDropList() => GetDataPresent(DataFormatNames.FileDrop, autoConvert: true);

    public virtual bool ContainsImage() => GetDataPresent(DataFormatNames.Bitmap, autoConvert: true);

    public virtual bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    public virtual bool ContainsText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetDataPresent(ConvertToDataFormats(format), autoConvert: false);
    }

#pragma warning disable WFDEV005 // Type or member is obsolete
    public virtual Stream? GetAudioStream() => GetData(DataFormats.WaveAudio, autoConvert: false) as Stream;

    public virtual StringCollection GetFileDropList()
    {
        StringCollection dropList = [];
        if (GetData(DataFormatNames.FileDrop, autoConvert: true) is string[] strings)
        {
            dropList.AddRange(strings);
        }

        return dropList;
    }

    public virtual Image? GetImage() => GetData(DataFormats.Bitmap, autoConvert: true) as Image;

    public virtual string GetText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetData(ConvertToDataFormats(format), autoConvert: false) is string text ? text : string.Empty;
    }
#pragma warning restore WFDEV005

    public virtual string GetText() => GetText(TextDataFormat.UnicodeText);

    public virtual void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    public virtual void SetAudio(Stream audioStream) =>
        SetData(DataFormatNames.WaveAudio, autoConvert: false, audioStream.OrThrowIfNull());

    public virtual void SetFileDropList(StringCollection filePaths)
    {
        string[] strings = new string[filePaths.OrThrowIfNull().Count];
        filePaths.CopyTo(strings, 0);
        SetData(DataFormatNames.FileDrop, autoConvert: true, strings);
    }

    public virtual void SetImage(Image image) => SetData(DataFormatNames.Bitmap, true, image.OrThrowIfNull());

    public virtual void SetText(string textData) => SetText(textData, TextDataFormat.UnicodeText);

    public virtual void SetText(string textData, TextDataFormat format)
    {
        textData.ThrowIfNullOrEmpty();

        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        SetData(ConvertToDataFormats(format), false, textData);
    }

    private bool TryGetDataInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;

        if (!ClipboardCore.IsValidTypeForFormat(typeof(T), format))
        {
            // Resolver implementation is specific to the overridden TryGetDataCore method,
            // can't validate if a non-null resolver is required for unbounded types.
            return false;
        }

        return TryGetDataCore(format, resolver, autoConvert, out data);
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.UnicodeText => DataFormatNames.UnicodeText,
        TextDataFormat.Rtf => DataFormatNames.Rtf,
        TextDataFormat.Html => DataFormatNames.Html,
        TextDataFormat.CommaSeparatedValue => DataFormatNames.Csv,
        _ => DataFormatNames.UnicodeText,
    };

    #region ComTypes.IDataObject
    int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) =>
        _innerData.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);

    void ComTypes.IDataObject.DUnadvise(int dwConnection) => _innerData.DUnadvise(dwConnection);

    int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) =>
        _innerData.EnumDAdvise(out enumAdvise);

    IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection) =>
        _innerData.EnumFormatEtc(dwDirection);

    int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut) =>
        _innerData.GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);

    void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium) =>
        _innerData.GetData(ref formatetc, out medium);

    void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium) =>
        _innerData.GetDataHere(ref formatetc, ref medium);

    int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc) =>
        _innerData.QueryGetData(ref formatetc);

    void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease) =>
        _innerData.SetData(ref pFormatetcIn, ref pmedium, fRelease);

    #endregion

    #region Com.IDataObject.Interface

    HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) =>
        _innerData.DAdvise(pformatetc, advf, pAdvSink, pdwConnection);

    HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection) =>
        _innerData.DUnadvise(dwConnection);

    HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) =>
        _innerData.EnumDAdvise(ppenumAdvise);

    HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) =>
        _innerData.EnumFormatEtc(dwDirection, ppenumFormatEtc);

    HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) =>
        _innerData.GetData(pformatetcIn, pmedium);

    HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) =>
        _innerData.GetDataHere(pformatetc, pmedium);

    HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) =>
        _innerData.QueryGetData(pformatetc);

    HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) =>
        _innerData.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);

    HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) =>
        _innerData.SetData(pformatetc, pmedium, fRelease);

    #endregion
}
