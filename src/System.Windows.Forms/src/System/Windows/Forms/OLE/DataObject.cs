// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Reflection.Metadata;
using System.Private.Windows;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Private.Windows.Core.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
public unsafe partial class DataObject :
    ITypedDataObject,
    Com.IDataObject.Interface,
    ComTypes.IDataObject,
    Com.IManagedWrapper<Com.IDataObject>
{
    private const string CF_DEPRECATED_FILENAME = "FileName";
    private const string CF_DEPRECATED_FILENAMEW = "FileNameW";
    private const string BitmapFullName = "System.Drawing.Bitmap";

    private readonly Composition _innerData;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the raw <see cref="Com.IDataObject"/>
    ///  and the managed data object the raw pointer is associated with.
    /// </summary>
    /// <inheritdoc cref="DataObject(object)"/>
    internal DataObject(Com.IDataObject* data) => _innerData = Composition.CreateFromNativeDataObject(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    /// <inheritdoc cref="DataObject(object)"/>
    public DataObject() => _innerData = Composition.CreateFromWinFormsDataObject(new DataStore());

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
    public DataObject(object data)
    {
        if (data is IDataObject iDataObject)
        {
            _innerData = Composition.CreateFromWinFormsDataObject(iDataObject);
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            _innerData = Composition.CreateFromRuntimeDataObject(comDataObject);
        }
        else
        {
            _innerData = Composition.CreateFromWinFormsDataObject(new DataStore());
            SetData(data);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data and its
    ///  associated format.
    /// </summary>
    public DataObject(string format, object data) : this() => SetData(format, data);

    internal DataObject(string format, bool autoConvert, object data) : this() => SetData(format, autoConvert, data);

    /// <summary>
    ///  Flags that the original data was not a user passed <see cref="IDataObject"/>.
    /// </summary>
    internal bool IsOriginalNotIDataObject { get; init; }

    /// <summary>
    ///  Returns the inner data that the <see cref="DataObject"/> was created with if the original data implemented
    ///  <see cref="IDataObject"/>. Otherwise, returns this.
    ///  This method should only be used if the <see cref="DataObject"/> was created for clipboard purposes.
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
    /// <param name="format">The format associated with the data. See <see cref="DataFormats"/> for predefined formats.</param>
    /// <param name="autoConvert"><see langword="true"/> to allow the data to be converted to another format; otherwise, <see langword="false"/>.</param>
    /// <param name="data">The data to store.</param>
    /// <exception cref="InvalidOperationException">
    ///  If <paramref name="data"/> is a non derived <see cref="DataObject"/>. This is for better error reporting as <see cref="DataObject"/> will serialize as empty.
    ///  If <see cref="DataObject"/> needs to be set, JSON serialize the data held in <paramref name="data"/> using this method, then use <see cref="SetData(object?)"/>
    ///  passing in <paramref name="data"/>.
    /// </exception>
    /// <remarks>
    ///  <para>
    ///   If your data is an intrinsically handled type such as primitives, string, or Bitmap
    ///   and you are using a custom format or <see cref="DataFormats.Serializable"/>
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
    private static object TryJsonSerialize<T>(string format, T data)
    {
        if (string.IsNullOrWhiteSpace(format.OrThrowIfNull()))
        {
            throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
        }

        data.OrThrowIfNull(nameof(data));

        if (typeof(T) == typeof(DataObject))
        {
            throw new InvalidOperationException(string.Format(SR.ClipboardOrDragDrop_CannotJsonSerializeDataObject, nameof(SetData)));
        }

        return IsRestrictedFormat(format) || Composition.Binder.IsKnownType<T>()
            ? data
            : new JsonData<T>() { JsonBytes = JsonSerializer.SerializeToUtf8Bytes(data) };
    }

    /// <summary>
    ///  Check if the <paramref name="format"/> is one of the restricted formats, which formats that
    ///  correspond to primitives or are pre-defined in the OS such as strings, bitmaps, and OLE types.
    /// </summary>
    private static bool IsRestrictedFormat(string format) => RestrictDeserializationToSafeTypes(format)
        || format is DataFormatNames.Text
            or DataFormatNames.UnicodeText
            or DataFormatNames.Rtf
            or DataFormatNames.Html
            or DataFormatNames.OemText
            or DataFormatNames.FileDrop
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
        format is DataFormatNames.String
            or BitmapFullName
            or DataFormatNames.Csv
            or DataFormatNames.Dib
            or DataFormatNames.Dif
            or DataFormatNames.Locale
            or DataFormatNames.PenData
            or DataFormatNames.Riff
            or DataFormatNames.SymbolicLink
            or DataFormatNames.Tiff
            or DataFormatNames.WaveAudio
            or DataFormatNames.Bitmap
            or DataFormatNames.Emf
            or DataFormatNames.Palette
            or DataFormatNames.Wmf;

    #region IDataObject
    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format, bool autoConvert)
    {
        object? result = _innerData.GetData(format, autoConvert);
        // Avoid exposing our internal JsonData<T>
        return result is IJsonData ? null : result;
    }

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
    [CLSCompliant(false)]
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
    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(string, Func{TypeName, Type}, bool, out T)" />
    [CLSCompliant(false)]
    protected virtual bool TryGetDataCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type>? resolver,
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
        SetData(DataFormatNames.FileDrop, true, strings);
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

    /// <summary>
    ///  Verify if the specified format is valid and compatible with the specified type <typeparamref name="T"/>.
    /// </summary>
    internal static bool IsValidFormatAndType<T>(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (IsValidPredefinedFormatTypeCombination(format))
        {
            return true;
        }

        throw new NotSupportedException(string.Format(
            SR.ClipboardOrDragDrop_InvalidFormatTypeCombination,
            typeof(T).FullName, format));

        static bool IsValidPredefinedFormatTypeCombination(string format) => format switch
        {
            DataFormatNames.Text
                or DataFormatNames.UnicodeText
                or DataFormatNames.String
                or DataFormatNames.Rtf
                or DataFormatNames.Html
                or DataFormatNames.OemText => typeof(string) == typeof(T),

            DataFormatNames.FileDrop
                or CF_DEPRECATED_FILENAME
                or CF_DEPRECATED_FILENAMEW => typeof(string[]) == typeof(T),

            DataFormatNames.Bitmap or BitmapFullName =>
                typeof(Bitmap) == typeof(T) || typeof(Image) == typeof(T),
            _ => true
        };
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.UnicodeText => DataFormatNames.UnicodeText,
        TextDataFormat.Rtf => DataFormatNames.Rtf,
        TextDataFormat.Html => DataFormatNames.Html,
        TextDataFormat.CommaSeparatedValue => DataFormatNames.Csv,
        _ => DataFormatNames.UnicodeText,
    };

    /// <summary>
    ///  Returns all the "synonyms" for the specified format.
    /// </summary>
    private static string[]? GetMappedFormats(string format) => format switch
    {
        null => null,
        DataFormatNames.Text or DataFormatNames.UnicodeText or DataFormatNames.String =>
            [
                DataFormatNames.String,
                DataFormatNames.UnicodeText,
                DataFormatNames.Text
            ],
        DataFormatNames.FileDrop or CF_DEPRECATED_FILENAME or CF_DEPRECATED_FILENAMEW =>
            [
                DataFormatNames.FileDrop,
                CF_DEPRECATED_FILENAMEW,
                CF_DEPRECATED_FILENAME
            ],
        DataFormatNames.Bitmap or BitmapFullName =>
            [
                BitmapFullName,
                DataFormatNames.Bitmap
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
