// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
public unsafe partial class DataObject :
    IDataObject,
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
    internal DataObject(Com.IDataObject* data) => _innerData = Composition.CreateFromNativeDataObject(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    public DataObject()
    {
        _innerData = Composition.CreateFromWinFormsDataObject(new DataStore());
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data.
    /// </summary>
    public DataObject(object data)
    {
        if (data is DataObject dataObject)
        {
            _innerData = dataObject._innerData;
        }
        else if (data is IDataObject iDataObject)
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
    ///  Flags that the original data was wrapped for clipboard purposes.
    /// </summary>
    internal bool IsWrappedForClipboard { get; init; }

    /// <summary>
    ///  Returns the inner data that the <see cref="DataObject"/> was created with if the original data implemented
    ///  <see cref="IDataObject"/>. Otherwise, returns this.
    ///  This method should only be used if the <see cref="DataObject"/> was created for clipboard purposes.
    /// </summary>
    internal IDataObject TryUnwrapInnerIDataObject()
    {
        Debug.Assert(IsWrappedForClipboard, "This method should only be used for clipboard purposes.");
        return _innerData.OriginalIDataObject is { } original ? original : this;
    }

    /// <inheritdoc cref="Composition.OriginalIDataObject"/>
    internal IDataObject? OriginalIDataObject => _innerData.OriginalIDataObject;

    #region IDataObject
    public virtual object? GetData(string format, bool autoConvert) =>
        ((IDataObject)_innerData).GetData(format, autoConvert);

    public virtual object? GetData(string format) => GetData(format, autoConvert: true);

    public virtual object? GetData(Type format) => format is null ? null : GetData(format.FullName!);

    public virtual bool GetDataPresent(string format, bool autoConvert) =>
        ((IDataObject)_innerData).GetDataPresent(format, autoConvert);

    public virtual bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public virtual bool GetDataPresent(Type format) => format is not null && GetDataPresent(format.FullName!);

    public virtual string[] GetFormats(bool autoConvert) => ((IDataObject)_innerData).GetFormats(autoConvert);

    public virtual string[] GetFormats() => GetFormats(autoConvert: true);

    public virtual void SetData(string format, bool autoConvert, object? data)
        => ((IDataObject)_innerData).SetData(format, autoConvert, data);

    public virtual void SetData(string format, object? data) => ((IDataObject)_innerData).SetData(format, data);

    public virtual void SetData(Type format, object? data) => ((IDataObject)_innerData).SetData(format, data);

    public virtual void SetData(object? data) => ((IDataObject)_innerData).SetData(data);
    #endregion

    public virtual bool ContainsAudio() => GetDataPresent(DataFormats.WaveAudioConstant, autoConvert: false);

    public virtual bool ContainsFileDropList() => GetDataPresent(DataFormats.FileDropConstant, autoConvert: true);

    public virtual bool ContainsImage() => GetDataPresent(DataFormats.BitmapConstant, autoConvert: true);

    public virtual bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    public virtual bool ContainsText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetDataPresent(ConvertToDataFormats(format), autoConvert: false);
    }

    public virtual Stream? GetAudioStream() => GetData(DataFormats.WaveAudio, autoConvert: false) as Stream;

    public virtual StringCollection GetFileDropList()
    {
        StringCollection dropList = [];
        if (GetData(DataFormats.FileDropConstant, autoConvert: true) is string[] strings)
        {
            dropList.AddRange(strings);
        }

        return dropList;
    }

    public virtual Image? GetImage() => GetData(DataFormats.Bitmap, autoConvert: true) as Image;

    public virtual string GetText() => GetText(TextDataFormat.UnicodeText);

    public virtual string GetText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetData(ConvertToDataFormats(format), false) is string text ? text : string.Empty;
    }

    public virtual void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    public virtual void SetAudio(Stream audioStream) =>
        SetData(DataFormats.WaveAudioConstant, autoConvert: false, audioStream.OrThrowIfNull());

    public virtual void SetFileDropList(StringCollection filePaths)
    {
        string[] strings = new string[filePaths.OrThrowIfNull().Count];
        filePaths.CopyTo(strings, 0);
        SetData(DataFormats.FileDropConstant, true, strings);
    }

    public virtual void SetImage(Image image) => SetData(DataFormats.BitmapConstant, true, image.OrThrowIfNull());

    public virtual void SetText(string textData) => SetText(textData, TextDataFormat.UnicodeText);

    public virtual void SetText(string textData, TextDataFormat format)
    {
        textData.ThrowIfNullOrEmpty();

        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        SetData(ConvertToDataFormats(format), false, textData);
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.UnicodeText => DataFormats.UnicodeTextConstant,
        TextDataFormat.Rtf => DataFormats.RtfConstant,
        TextDataFormat.Html => DataFormats.HtmlConstant,
        TextDataFormat.CommaSeparatedValue => DataFormats.CsvConstant,
        _ => DataFormats.UnicodeTextConstant,
    };

    /// <summary>
    ///  Returns all the "synonyms" for the specified format.
    /// </summary>
    private static string[]? GetMappedFormats(string format) => format switch
    {
        null => null,
        DataFormats.TextConstant or DataFormats.UnicodeTextConstant or DataFormats.StringConstant =>
            [
                DataFormats.StringConstant,
                DataFormats.UnicodeTextConstant,
                DataFormats.TextConstant
            ],
        DataFormats.FileDropConstant or CF_DEPRECATED_FILENAME or CF_DEPRECATED_FILENAMEW =>
            [
                DataFormats.FileDropConstant,
                CF_DEPRECATED_FILENAMEW,
                CF_DEPRECATED_FILENAME
            ],
        DataFormats.BitmapConstant or BitmapFullName =>
            [
                BitmapFullName,
                DataFormats.BitmapConstant
            ],
        _ => [format]
    };

    #region ComTypes.IDataObject
    int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) =>
        ((ComTypes.IDataObject)_innerData).DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);

    void ComTypes.IDataObject.DUnadvise(int dwConnection) => ((ComTypes.IDataObject)_innerData).DUnadvise(dwConnection);

    int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) =>
        ((ComTypes.IDataObject)_innerData).EnumDAdvise(out enumAdvise);

    IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection) =>
        ((ComTypes.IDataObject)_innerData).EnumFormatEtc(dwDirection);

    int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut) =>
        ((ComTypes.IDataObject)_innerData).GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);

    void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerData).GetData(ref formatetc, out medium);

    void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerData).GetDataHere(ref formatetc, ref medium);

    int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc) =>
        ((ComTypes.IDataObject)_innerData).QueryGetData(ref formatetc);

    void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease) =>
        ((ComTypes.IDataObject)_innerData).SetData(ref pFormatetcIn, ref pmedium, fRelease);

    #endregion

    #region Com.IDataObject

    HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) =>
        ((Com.IDataObject.Interface)_innerData).DAdvise(pformatetc, advf, pAdvSink, pdwConnection);

    HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection) =>
        ((Com.IDataObject.Interface)_innerData).DUnadvise(dwConnection);

    HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) =>
        ((Com.IDataObject.Interface)_innerData).EnumDAdvise(ppenumAdvise);

    HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) =>
        ((Com.IDataObject.Interface)_innerData).EnumFormatEtc(dwDirection, ppenumFormatEtc);

    HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerData).GetData(pformatetcIn, pmedium);

    HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerData).GetDataHere(pformatetc, pmedium);

    HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) =>
        ((Com.IDataObject.Interface)_innerData).QueryGetData(pformatetc);

    HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) =>
        ((Com.IDataObject.Interface)_innerData).GetCanonicalFormatEtc(pformatectIn, pformatetcOut);

    HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) =>
        ((Com.IDataObject.Interface)_innerData).SetData(pformatetc, pmedium, fRelease);

    #endregion
}
