// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Private.Windows.Core.Resources;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using IDataObject = System.Private.Windows.Core.OLE.IDataObjectDesktop;

namespace System.Private.Windows.Core.OLE;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
internal abstract unsafe partial class DesktopDataObject :
    ITypedDataObjectDesktop,
    Com.IDataObject.Interface,
    ComTypes.IDataObject,
    Com.IManagedWrapper<Com.IDataObject>
{
    protected const string CF_DEPRECATED_FILENAME = "FileName";
    protected const string CF_DEPRECATED_FILENAMEW = "FileNameW";
    protected const string BitmapFullName = "System.Drawing.Bitmap";

    private readonly Composition _innerData;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DesktopDataObject"/> class, with the raw <see cref="Com.IDataObject"/>
    ///  and the managed data object the raw pointer is associated with.
    /// </summary>
    /// <inheritdoc cref="DesktopDataObject(object, Composition)"/>
    internal DesktopDataObject(Com.IDataObject* data, Composition composition) => _innerData = composition.PopulateFromNativeDataObject(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DesktopDataObject"/> class, which can store arbitrary data.
    /// </summary>
    /// <inheritdoc cref="DesktopDataObject(object, Composition)"/>
    internal DesktopDataObject(Composition composition) => _innerData = composition.PopulateFromDesktopDataObject(CreateIDataObject());

    /// <summary>
    ///  Initializes a new instance of the <see cref="DesktopDataObject"/> class, containing the specified data.
    /// </summary>
    /// <param name="composition">A default <see cref="Composition"/> that will be populated appropriately by this constructor.</param>
    /// <remarks>
    ///  <para>
    ///   If <paramref name="data"/> implements an <see cref="IDataObject"/> interface,
    ///   we strongly recommend implementing <see cref="ITypedDataObjectDesktop"/> interface to support the
    ///   `TryGetData{T}` API family that restricts deserialization to the requested and known types.
    ///   <see cref="DesktopClipboard.TryGetData{T}(string, out T)"/> will throw <see cref="NotSupportedException"/>
    ///   if <see cref="ITypedDataObjectDesktop"/> is not implemented.
    ///  </para>
    /// </remarks>
    internal DesktopDataObject(object data, Composition composition)
    {
        if (data is DesktopDataObject dataObject)
        {
            _innerData = dataObject._innerData;
            IsOriginalNotIDataObject = dataObject.IsOriginalNotIDataObject;
        }
        else if (data is IDataObject iDataObject)
        {
            // This is must be an adapter which knows how to move between internal and public IDataObject.
            _innerData = composition.PopulateFromDesktopDataObject(iDataObject);
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            _innerData = composition.PopulateFromRuntimeDataObject(comDataObject);
        }
        else
        {
            _innerData = composition.PopulateFromDesktopDataObject(CreateIDataObject());
            SetData(data);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DesktopDataObject"/> class, containing the specified data and its
    ///  associated format.
    /// </summary>
    internal DesktopDataObject(string format, object data, Composition composition) : this(composition) => SetData(format, data);

    internal DesktopDataObject(string format, bool autoConvert, object data, Composition composition) : this(composition) => SetData(format, autoConvert, data);

    /// <summary>
    ///  Returns <see langword="true"/> if <typeparamref name="T"/> is a known type to the application.
    ///  Otherwise, <see langword="false"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Be sure to call <see cref="Composition.Binder.IsKnownTypeCore{T}"/> to capture intrinsic types.
    ///  </para>
    /// </remarks>
    public abstract bool IsKnownType<T>();

    /// <summary>
    ///  Ensures <typeparamref name="T"/> is not of type public DataObject for JSON serialization as
    ///  it cannot be serialized meaningfully.
    /// </summary>
    /// <exception cref="NotSupportedException">If <typeparamref name="T"/> is of type public DataObject.</exception>
    public abstract void CheckDataObjectForJsonSet<T>();

    /// <summary>
    ///  Creates the appropriate default <see cref="IDataObject"/> for the application.
    /// </summary>
    public abstract IDataObject CreateIDataObject();

    /// <summary>
    ///  Verify if the specified format is valid and compatible with the specified type <typeparamref name="T"/>.
    /// </summary>
    internal abstract bool IsValidFormatAndType<T>(string format);

    /// <summary>
    ///  Flags that the original data was not a user passed <see cref="IDataObject"/>.
    /// </summary>
    internal bool IsOriginalNotIDataObject { get; init; }

    /// <summary>
    ///  Returns the inner data that the <see cref="DesktopDataObject"/> was created with if the original data implemented
    ///  <see cref="IDataObject"/>. Otherwise, returns this.
    ///  This method should only be used if the <see cref="DesktopDataObject"/> was created for clipboard purposes.
    /// </summary>
    internal IDataObject TryUnwrapInnerIDataObject()
    {
        Debug.Assert(!IsOriginalNotIDataObject, "This method should only be used for clipboard purposes.");
        return _innerData.OriginalIDataObject is { } original ? original : this;
    }

    /// <inheritdoc cref="Composition.OriginalIDataObject"/>
    internal IDataObject? OriginalIDataObject => _innerData.OriginalIDataObject;

    /// <inheritdoc cref="SetDataAsJson{T}(string, bool, T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(string format, T data) => SetData(format, TryJsonSerialize(format, data));

    /// <inheritdoc cref="SetDataAsJson{T}(string, bool, T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(T data) => SetData(typeof(T), TryJsonSerialize(typeof(T).FullName!, data));

    /// <summary>
    ///  Stores the data in the specified format.
    ///  If the data is a managed object and format allows for serialization of managed objects, the object will be serialized as JSON.
    /// </summary>
    /// <param name="format">The format associated with the data. See <see cref="DesktopDataFormats"/> for predefined formats.</param>
    /// <param name="autoConvert"><see langword="true"/> to allow the data to be converted to another format; otherwise, <see langword="false"/>.</param>
    /// <param name="data">The data to store.</param>
    /// <exception cref="InvalidOperationException">
    ///  If <paramref name="data"/> is a non derived <see cref="DesktopDataObject"/>. This is for better error reporting as <see cref="DesktopDataObject"/> will serialize as empty.
    ///  If <see cref="DesktopDataObject"/> needs to be set, JSON serialize the data held in <paramref name="data"/> using this method, then use <see cref="SetData(object?)"/>
    ///  passing in <paramref name="data"/>.
    /// </exception>
    /// <remarks>
    ///  <para>
    ///   If your data is an intrinsically handled type such as primitives, string, or Bitmap
    ///   and you are using a custom format or <see cref="DesktopDataFormats.SerializableConstant"/>
    ///   it is recommended to use the <see cref="SetData(string, object?)"/> APIs to avoid unnecessary overhead.
    ///  </para>
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
    ///   If custom JSON serialization behavior is needed, manually JSON serialize the data and then use <see cref="SetData(object?)"/>,
    ///   or create a custom <see cref="Text.Json.Serialization.JsonConverter"/>, attach the
    ///   <see cref="Text.Json.Serialization.JsonConverterAttribute"/>, and then recall this method.
    ///   See <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/converters-how-to"/> for more details
    ///   on custom converters for JSON serialization.
    ///  </para>
    /// </remarks>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(string format, bool autoConvert, T data) => SetData(format, autoConvert, TryJsonSerialize(format, data));

    /// <summary>
    ///  JSON serialize the data only if the format is not a restricted deserialization format and the data is not an intrinsic type.
    /// </summary>
    /// <returns>
    ///  The passed in <paramref name="data"/> as is if the format is restricted. Otherwise the JSON serialized <paramref name="data"/>.
    /// </returns>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<TValue>(TValue, JsonSerializerOptions)")]
    private object TryJsonSerialize<T>(string format, T data)
    {
        if (string.IsNullOrWhiteSpace(format.OrThrowIfNull()))
        {
            throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
        }

        data.OrThrowIfNull(nameof(data));
        CheckDataObjectForJsonSet<T>();

        return IsRestrictedFormat(format) || IsKnownType<T>()
            ? data
            : new JsonData<T>() { JsonBytes = JsonSerializer.SerializeToUtf8Bytes(data) };
    }

    /// <summary>
    ///  Check if the <paramref name="format"/> is one of the restricted formats, which formats that
    ///  correspond to primitives or are pre-defined in the OS such as strings, bitmaps, and OLE types.
    /// </summary>
    internal static bool IsRestrictedFormat(string format) => RestrictDeserializationToSafeTypes(format)
        || format is DesktopDataFormats.TextConstant
            or DesktopDataFormats.UnicodeTextConstant
            or DesktopDataFormats.RtfConstant
            or DesktopDataFormats.HtmlConstant
            or DesktopDataFormats.OemTextConstant
            or DesktopDataFormats.FileDropConstant
            or CF_DEPRECATED_FILENAME
            or CF_DEPRECATED_FILENAMEW;

    /// <summary>
    ///  We are restricting binary serialization and deserialization of formats that represent strings, bitmaps or OLE types.
    /// </summary>
    /// <param name="format">format name</param>
    /// <returns><see langword="true" /> - serialize only safe types, strings or bitmaps.</returns>
    /// <remarks>
    ///  <para>
    ///   These formats are also restricted in WPF
    ///   https://github.com/dotnet/wpf/blob/db1ae73aae0e043326e2303b0820d361de04e751/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/dataobject.cs#L2801
    ///  </para>
    /// </remarks>
    private static bool RestrictDeserializationToSafeTypes(string format) =>
        format is DesktopDataFormats.StringConstant
            or BitmapFullName
            or DesktopDataFormats.CsvConstant
            or DesktopDataFormats.DibConstant
            or DesktopDataFormats.DifConstant
            or DesktopDataFormats.LocaleConstant
            or DesktopDataFormats.PenDataConstant
            or DesktopDataFormats.RiffConstant
            or DesktopDataFormats.SymbolicLinkConstant
            or DesktopDataFormats.TiffConstant
            or DesktopDataFormats.WaveAudioConstant
            or DesktopDataFormats.BitmapConstant
            or DesktopDataFormats.EmfConstant
            or DesktopDataFormats.PaletteConstant
            or DesktopDataFormats.WmfConstant;

    #region IDataObject
    public object? GetData(string format, bool autoConvert)
    {
        return _innerData.GetData(format, autoConvert);
    }

    public object? GetData(string format) => GetData(format, autoConvert: true);

    public object? GetData(Type format) => format is null ? null : GetData(format.FullName!);

    public bool GetDataPresent(string format, bool autoConvert) => _innerData.GetDataPresent(format, autoConvert);

    public bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public bool GetDataPresent(Type format) => format is not null && GetDataPresent(format.FullName!);

    public string[] GetFormats(bool autoConvert) => _innerData.GetFormats(autoConvert);

    public string[] GetFormats() => GetFormats(autoConvert: true);

    public void SetData(string format, bool autoConvert, object? data) =>
        _innerData.SetData(format, autoConvert, data);

    public void SetData(string format, object? data) => _innerData.SetData(format, data);

    public void SetData(Type format, object? data) => _innerData.SetData(format, data);

    public void SetData(object? data) => _innerData.SetData(data);
    #endregion

    #region ITypedDataObject
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type> resolver,
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
    /// <inheritdoc cref="ITypedDataObjectDesktop.TryGetData{T}(string, Func{TypeName, Type}, bool, out T)" />
    internal virtual bool TryGetDataCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            _innerData.TryGetData(format, resolver!, autoConvert, out data);

    public virtual bool ContainsAudio() => GetDataPresent(DesktopDataFormats.WaveAudioConstant, autoConvert: false);

    public virtual bool ContainsFileDropList() => GetDataPresent(DesktopDataFormats.FileDropConstant, autoConvert: true);

    public virtual bool ContainsImage() => GetDataPresent(DesktopDataFormats.BitmapConstant, autoConvert: true);

    public virtual bool ContainsText() => ContainsText(DesktopTextDataFormat.UnicodeText);

    public virtual bool ContainsText(DesktopTextDataFormat format)
    {
        return GetDataPresent(ConvertToDataFormats(format), autoConvert: false);
    }

#pragma warning disable WFDEV005 // Type or member is obsolete
    public virtual Stream? GetAudioStream() => GetData(DesktopDataFormats.WaveAudioConstant, autoConvert: false) as Stream;

    public virtual StringCollection GetFileDropList()
    {
        StringCollection dropList = [];
        if (GetData(DesktopDataFormats.FileDropConstant, autoConvert: true) is string[] strings)
        {
            dropList.AddRange(strings);
        }

        return dropList;
    }

    public virtual string GetText(DesktopTextDataFormat format)
    {
        return GetData(ConvertToDataFormats(format), autoConvert: false) is string text ? text : string.Empty;
    }
#pragma warning restore WFDEV005

    public virtual string GetText() => GetText(DesktopTextDataFormat.UnicodeText);

    public virtual void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    public virtual void SetAudio(Stream audioStream) =>
        SetData(DesktopDataFormats.WaveAudioConstant, autoConvert: false, audioStream.OrThrowIfNull());

    public virtual void SetFileDropList(StringCollection filePaths)
    {
        string[] strings = new string[filePaths.OrThrowIfNull().Count];
        filePaths.CopyTo(strings, 0);
        SetData(DesktopDataFormats.FileDropConstant, true, strings);
    }

    public virtual void SetText(string textData) => SetText(textData, DesktopTextDataFormat.UnicodeText);

    public virtual void SetText(string textData, DesktopTextDataFormat format)
    {
        textData.ThrowIfNullOrEmpty();
        SetData(ConvertToDataFormats(format), false, textData);
    }

    private bool TryGetDataInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;

        if (!IsValidFormatAndType<T>(format))
        {
            // Resolver implementation is specific to the overridden TryGetDataCore method,
            // can't validate if a non-null resolver is required for unbounded types.
            return false;
        }

        return TryGetDataCore(format, resolver, autoConvert, out data);
    }

    private static string ConvertToDataFormats(DesktopTextDataFormat format) => format switch
    {
        DesktopTextDataFormat.UnicodeText => DesktopDataFormats.UnicodeTextConstant,
        DesktopTextDataFormat.Rtf => DesktopDataFormats.RtfConstant,
        DesktopTextDataFormat.Html => DesktopDataFormats.HtmlConstant,
        DesktopTextDataFormat.CommaSeparatedValue => DesktopDataFormats.CsvConstant,
        _ => DesktopDataFormats.UnicodeTextConstant,
    };

    /// <summary>
    ///  Returns all the "synonyms" for the specified format.
    /// </summary>
    private static string[]? GetMappedFormats(string format) => format switch
    {
        null => null,
        DesktopDataFormats.TextConstant or DesktopDataFormats.UnicodeTextConstant or DesktopDataFormats.StringConstant =>
            [
                DesktopDataFormats.StringConstant,
                DesktopDataFormats.UnicodeTextConstant,
                DesktopDataFormats.TextConstant
            ],
        DesktopDataFormats.FileDropConstant or CF_DEPRECATED_FILENAME or CF_DEPRECATED_FILENAMEW =>
            [
                DesktopDataFormats.FileDropConstant,
                CF_DEPRECATED_FILENAMEW,
                CF_DEPRECATED_FILENAME
            ],
        DesktopDataFormats.BitmapConstant or BitmapFullName =>
            [
                BitmapFullName,
                DesktopDataFormats.BitmapConstant
            ],
        _ => [format]
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
