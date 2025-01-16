// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Private.Windows;
using System.Private.Windows.Core.OLE;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

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
    internal readonly WinFormsDataObject _innerDataObject;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the raw <see cref="Com.IDataObject"/>
    ///  and the managed data object the raw pointer is associated with.
    /// </summary>
    /// <inheritdoc cref="DataObject(object)"/>
    internal DataObject(Com.IDataObject* data) => _innerDataObject = new(data, this);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    /// <inheritdoc cref="DataObject(object)"/>
    public DataObject() => _innerDataObject = new(this);

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
        if (data is IDataObject iDataObject and not DesktopDataObject)
        {
            _innerDataObject = new(new DataObjectAdapter(iDataObject), this);
        }
        else
        {
            _innerDataObject = new(data, this);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data and its
    ///  associated format.
    /// </summary>
    public DataObject(string format, object data)
    {
        _innerDataObject = new(format, data, this);
    }

    internal DataObject(string format, bool autoConvert, object data)
    {
        _innerDataObject = new(format, autoConvert, data, this);
    }

    /// <inheritdoc cref="SetDataAsJson{T}(string, bool, T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(string format, T data) => _innerDataObject.SetDataAsJson(format, data);

    /// <inheritdoc cref="SetDataAsJson{T}(string, bool, T)"/>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public void SetDataAsJson<T>(T data) => _innerDataObject.SetDataAsJson(data);

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
    public void SetDataAsJson<T>(string format, bool autoConvert, T data) => _innerDataObject.SetDataAsJson(format, autoConvert, data);

    #region IDataObject
    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format, bool autoConvert)
    {
        object? result = _innerDataObject.GetData(format, autoConvert);

        // Avoid exposing our internal JsonData<T>
        return result is IJsonData ? null : result;
    }

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format) => _innerDataObject.GetData(format);

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(Type format) => _innerDataObject.GetData(format);

    public virtual bool GetDataPresent(string format, bool autoConvert) => _innerDataObject.GetDataPresent(format, autoConvert);

    public virtual bool GetDataPresent(string format) => _innerDataObject.GetDataPresent(format);

    public virtual bool GetDataPresent(Type format) => _innerDataObject.GetDataPresent(format);

    public virtual string[] GetFormats(bool autoConvert) => _innerDataObject.GetFormats(autoConvert);

    public virtual string[] GetFormats() => _innerDataObject.GetFormats();

    public virtual void SetData(string format, bool autoConvert, object? data) =>
        _innerDataObject.SetData(format, autoConvert, data);

    public virtual void SetData(string format, object? data) => _innerDataObject.SetData(format, data);

    public virtual void SetData(Type format, object? data) => _innerDataObject.SetData(format, data);

    public virtual void SetData(object? data) => _innerDataObject.SetData(data);
    #endregion

    #region ITypedDataObject
    [CLSCompliant(false)]
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        return _innerDataObject.TryGetData(format, resolver, autoConvert, out data);
    }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            _innerDataObject.TryGetData(format, autoConvert, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            _innerDataObject.TryGetData(format, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            _innerDataObject.TryGetData(out data);
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
            _innerDataObject.TryGetDataCore(format, resolver, autoConvert, out data);

    internal bool TryGetDataCoreInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type>? resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) => TryGetDataCore(format, resolver, autoConvert, out data);

    public virtual bool ContainsAudio() => _innerDataObject.ContainsAudio();

    public virtual bool ContainsFileDropList() => _innerDataObject.ContainsFileDropList();

    public virtual bool ContainsImage() => _innerDataObject.ContainsImage();

    public virtual bool ContainsText() => _innerDataObject.ContainsText();

    public virtual bool ContainsText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return _innerDataObject.ContainsText((DesktopTextDataFormat)format);
    }

#pragma warning disable WFDEV005 // Type or member is obsolete
    public virtual Stream? GetAudioStream() => _innerDataObject.GetAudioStream();

    public virtual StringCollection GetFileDropList()
    {
        return _innerDataObject.GetFileDropList();
    }

    public virtual Image? GetImage() => GetData(DesktopDataFormats.BitmapConstant, autoConvert: true) as Image;

    public virtual string GetText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return _innerDataObject.GetText((DesktopTextDataFormat)format);
    }
#pragma warning restore WFDEV005

    public virtual string GetText() => _innerDataObject.GetText();

    public virtual void SetAudio(byte[] audioBytes) => _innerDataObject.SetAudio(audioBytes);

    public virtual void SetAudio(Stream audioStream) =>
        _innerDataObject.SetAudio(audioStream);

    public virtual void SetFileDropList(StringCollection filePaths)
    {
        _innerDataObject.SetFileDropList(filePaths);
    }

    public virtual void SetImage(Image image) => SetData(DesktopDataFormats.BitmapConstant, true, image.OrThrowIfNull());

    public virtual void SetText(string textData) => _innerDataObject.SetText(textData);

    public virtual void SetText(string textData, TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        _innerDataObject.SetText(textData, (DesktopTextDataFormat)format);
    }

    #region ComTypes.IDataObject
    int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) =>
        ((ComTypes.IDataObject)_innerDataObject).DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);

    void ComTypes.IDataObject.DUnadvise(int dwConnection) => ((ComTypes.IDataObject)_innerDataObject).DUnadvise(dwConnection);

    int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) =>
        ((ComTypes.IDataObject)_innerDataObject).EnumDAdvise(out enumAdvise);

    IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection) =>
        ((ComTypes.IDataObject)_innerDataObject).EnumFormatEtc(dwDirection);

    int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut) =>
        ((ComTypes.IDataObject)_innerDataObject).GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);

    void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerDataObject).GetData(ref formatetc, out medium);

    void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerDataObject).GetDataHere(ref formatetc, ref medium);

    int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc) =>
        ((ComTypes.IDataObject)_innerDataObject).QueryGetData(ref formatetc);

    void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease) =>
        ((ComTypes.IDataObject)_innerDataObject).SetData(ref pFormatetcIn, ref pmedium, fRelease);

    #endregion

    #region Com.IDataObject.Interface

    HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) =>
        ((Com.IDataObject.Interface)_innerDataObject).DAdvise(pformatetc, advf, pAdvSink, pdwConnection);

    HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection) =>
        ((Com.IDataObject.Interface)_innerDataObject).DUnadvise(dwConnection);

    HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) =>
        ((Com.IDataObject.Interface)_innerDataObject).EnumDAdvise(ppenumAdvise);

    HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) =>
        ((Com.IDataObject.Interface)_innerDataObject).EnumFormatEtc(dwDirection, ppenumFormatEtc);

    HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerDataObject).GetData(pformatetcIn, pmedium);

    HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerDataObject).GetDataHere(pformatetc, pmedium);

    HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) =>
        ((Com.IDataObject.Interface)_innerDataObject).QueryGetData(pformatetc);

    HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) =>
        ((Com.IDataObject.Interface)_innerDataObject).GetCanonicalFormatEtc(pformatectIn, pformatetcOut);

    HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) =>
        ((Com.IDataObject.Interface)_innerDataObject).SetData(pformatetc, pmedium, fRelease);

    #endregion
}
