// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms.BinaryFormat;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
public unsafe partial class DataObject :
    IDataObject,
    ComTypes.IDataObject,
    Com.IDataObject.Interface,
    Com.IManagedWrapper<Com.IDataObject>
{
    private const string CF_DEPRECATED_FILENAME = "FileName";
    private const string CF_DEPRECATED_FILENAMEW = "FileNameW";
    private const string BitmapFullName = "System.Drawing.Bitmap";

    private const int DATA_S_SAMEFORMATETC = 0x00040130;

    private const TYMED AllowedTymeds = TYMED.TYMED_HGLOBAL | TYMED.TYMED_ISTREAM | TYMED.TYMED_GDI;

    private readonly IDataObject _innerData;

    // We use this to identify that a stream is actually a serialized object. On read, we don't know if the contents
    // of a stream were saved "raw" or if the stream is really pointing to a serialized object. If we saved an object,
    // we prefix it with this guid.
    private static readonly byte[] s_serializedObjectID = new byte[]
    {
        // FD9EA796-3B13-4370-A679-56106BB288FB
        0x96, 0xa7, 0x9e, 0xfd,
        0x13, 0x3b,
        0x70, 0x43,
        0xa6, 0x79, 0x56, 0x10, 0x6b, 0xb2, 0x88, 0xfb
    };

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the specified <see cref="IDataObject"/>.
    /// </summary>
    internal DataObject(IDataObject data)
    {
        CompModSwitches.DataObject.TraceVerbose("Constructed DataObject based on IDataObject");
        _innerData = data;
    }

    /// <summary>
    ///  Create a <see cref="DataObject"/> from a raw interface pointer.
    /// </summary>
    internal static DataObject FromComPointer(Com.IDataObject* data)
    {
        // Get the RCW for the pointer and continue.
        bool success = ComHelpers.TryGetObjectForIUnknown(
            (Com.IUnknown*)data,
            takeOwnership: true,
            out ComTypes.IDataObject? comTypesData);

        Debug.Assert(success && comTypesData is not null);
        return new(comTypesData!);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the specified <see cref="ComTypes.IDataObject"/>.
    /// </summary>
    internal DataObject(ComTypes.IDataObject data)
    {
        if (data is DataObject dataObject)
        {
            _innerData = dataObject;
        }
        else
        {
            CompModSwitches.DataObject.TraceVerbose("Constructed DataObject based on IComDataObject");
            _innerData = new ComDataObjectAdapter(data);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    public DataObject()
    {
        CompModSwitches.DataObject.TraceVerbose("Constructed DataObject standalone");
        _innerData = new DataStore();
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data.
    /// </summary>
    public DataObject(object data)
    {
        CompModSwitches.DataObject.TraceVerbose($"Constructed DataObject base on Object: {data}");
        if (data is IDataObject dataObject && !Marshal.IsComObject(data))
        {
            _innerData = dataObject;
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            _innerData = new ComDataObjectAdapter(comDataObject);
        }
        else
        {
            _innerData = new DataStore();
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
    ///  Retrieves the data associated with the specified data format, using an automated conversion parameter to
    ///  determine whether to convert the data to the format.
    /// </summary>
    public virtual object? GetData(string format, bool autoConvert)
    {
        CompModSwitches.DataObject.TraceVerbose($"Request data: {format}, {autoConvert}");
        return _innerData.GetData(format, autoConvert);
    }

    /// <summary>
    ///  Retrieves the data associated with the specified data format.
    /// </summary>
    public virtual object? GetData(string format)
    {
        CompModSwitches.DataObject.TraceVerbose($"Request data: {format}");
        return GetData(format, autoConvert: true);
    }

    /// <summary>
    ///  Retrieves the data associated with the specified class type format.
    /// </summary>
    public virtual object? GetData(Type format)
    {
        CompModSwitches.DataObject.TraceVerbose($"Request data: {format?.FullName ?? "(null)"}");
        return format is null ? null : GetData(format.FullName!);
    }

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or can be converted to,
    ///  the specified format.
    /// </summary>
    public virtual bool GetDataPresent(Type format)
    {
        CompModSwitches.DataObject.TraceVerbose($"Check data: {format?.FullName ?? "(null)"}");
        if (format is null)
        {
            return false;
        }

        bool present = GetDataPresent(format.FullName!);
        CompModSwitches.DataObject.TraceVerbose($"  ret: {present}");
        return present;
    }

    /// <summary>
    ///  Determines whether data stored in this instance is associated with the specified format, using an
    ///  automatic conversion parameter to determine whether to convert the data to the format.
    /// </summary>
    public virtual bool GetDataPresent(string format, bool autoConvert)
    {
        CompModSwitches.DataObject.TraceVerbose($"Check data: {format}, {autoConvert}");
        bool present = _innerData.GetDataPresent(format, autoConvert);
        CompModSwitches.DataObject.TraceVerbose($"  ret: {present}");
        return present;
    }

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or can be converted to,
    ///  the specified format.
    /// </summary>
    public virtual bool GetDataPresent(string format)
    {
        CompModSwitches.DataObject.TraceVerbose($"Check data: {format}");
        bool present = GetDataPresent(format, autoConvert: true);
        CompModSwitches.DataObject.TraceVerbose($"  ret: {present}");
        return present;
    }

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is associated with or can be converted to,
    ///  using an automatic conversion parameter <paramref name="autoConvert"/> to determine whether to retrieve
    ///  all formats that the data can be converted to or only native data formats.
    /// </summary>
    public virtual string[] GetFormats(bool autoConvert)
    {
        CompModSwitches.DataObject.TraceVerbose($"Check formats: {autoConvert}");
        return _innerData.GetFormats(autoConvert);
    }

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is associated with or can be converted to.
    /// </summary>
    public virtual string[] GetFormats()
    {
        CompModSwitches.DataObject.TraceVerbose("Check formats:");
        return GetFormats(autoConvert: true);
    }

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
        StringCollection dropList = new();
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

    public virtual void SetAudio(Stream audioStream)
        => SetData(DataFormats.WaveAudioConstant, autoConvert: false, audioStream.OrThrowIfNull());

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
        DataFormats.TextConstant or DataFormats.UnicodeTextConstant or DataFormats.StringConstant => new string[]
            {
                DataFormats.StringConstant,
                DataFormats.UnicodeTextConstant,
                DataFormats.TextConstant
            },
        DataFormats.FileDropConstant or CF_DEPRECATED_FILENAME or CF_DEPRECATED_FILENAMEW => new string[]
            {
                DataFormats.FileDropConstant,
                CF_DEPRECATED_FILENAMEW,
                CF_DEPRECATED_FILENAME
            },
        DataFormats.BitmapConstant or BitmapFullName => new string[]
            {
                BitmapFullName,
                DataFormats.BitmapConstant
            },
        _ => new string[] { format }
    };

    /// <summary>
    ///  Returns true if the tymed is useable.
    /// </summary>
    private static bool GetTymedUseable(TYMED tymed) => (tymed & AllowedTymeds) != 0;

    int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
    {
        CompModSwitches.DataObject.TraceVerbose("DAdvise");
        if (_innerData is ComDataObjectAdapter converter)
        {
            return converter.OleDataObject.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);
        }

        pdwConnection = 0;
        return (int)HRESULT.E_NOTIMPL;
    }

    void ComTypes.IDataObject.DUnadvise(int dwConnection)
    {
        CompModSwitches.DataObject.TraceVerbose("DUnadvise");
        if (_innerData is ComDataObjectAdapter converter)
        {
            converter.OleDataObject.DUnadvise(dwConnection);
            return;
        }

        Marshal.ThrowExceptionForHR((int)HRESULT.E_NOTIMPL);
    }

    int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
    {
        CompModSwitches.DataObject.TraceVerbose("EnumDAdvise");
        if (_innerData is ComDataObjectAdapter converter)
        {
            return converter.OleDataObject.EnumDAdvise(out enumAdvise);
        }

        enumAdvise = null;
        return (int)HRESULT.OLE_E_ADVISENOTSUPPORTED;
    }

    IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection)
    {
        CompModSwitches.DataObject.TraceVerbose($"EnumFormatEtc: {dwDirection}");
        if (_innerData is ComDataObjectAdapter converter)
        {
            return converter.OleDataObject.EnumFormatEtc(dwDirection);
        }

        if (dwDirection == DATADIR.DATADIR_GET)
        {
            return new FormatEnumerator(this);
        }

        throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
    }

    int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut)
    {
        CompModSwitches.DataObject.TraceVerbose("GetCanonicalFormatEtc");
        if (_innerData is ComDataObjectAdapter converter)
        {
            return converter.OleDataObject.GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);
        }

        pformatetcOut = default;
        return DATA_S_SAMEFORMATETC;
    }

    void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium)
    {
        CompModSwitches.DataObject.TraceVerbose("GetData");
        if (_innerData is ComDataObjectAdapter converter)
        {
            converter.OleDataObject.GetData(ref formatetc, out medium);
            return;
        }

        if (DragDropHelper.IsInDragLoop(_innerData))
        {
            string formatName = DataFormats.GetFormat(formatetc.cfFormat).Name;
            if (!_innerData.GetDataPresent(formatName))
            {
                medium = default;
                CompModSwitches.DataObject.TraceVerbose($" drag-and-drop private format requested '{formatName}' not present");
                return;
            }

            if (_innerData.GetData(formatName) is DragDropFormat dragDropFormat)
            {
                medium = dragDropFormat.GetData();
                CompModSwitches.DataObject.TraceVerbose($" drag-and-drop private format retrieved '{formatName}'");
                return;
            }
        }

        medium = default;

        if (!GetTymedUseable(formatetc.tymed))
        {
            Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
        }

        if (!formatetc.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
        {
            medium.tymed = formatetc.tymed;
            ((ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
            return;
        }

        medium.tymed = TYMED.TYMED_HGLOBAL;
        medium.unionmember = PInvoke.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, 1);

        if (medium.unionmember == 0)
        {
            throw new OutOfMemoryException();
        }

        try
        {
            ((ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
        }
        catch
        {
            PInvoke.GlobalFree((HGLOBAL)medium.unionmember);
            medium.unionmember = 0;
            throw;
        }
    }

    void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium)
    {
        CompModSwitches.DataObject.TraceVerbose("GetDataHere");
        if (_innerData is ComDataObjectAdapter converter)
        {
            converter.OleDataObject.GetDataHere(ref formatetc, ref medium);
            return;
        }

        if (!GetTymedUseable(formatetc.tymed) || !GetTymedUseable(medium.tymed))
        {
            Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
        }

        string format = DataFormats.GetFormat(formatetc.cfFormat).Name;

        if (!GetDataPresent(format))
        {
            Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_FORMATETC);
        }

        object? data = GetData(format);

        if (formatetc.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
        {
            try
            {
                SaveDataToHGLOBAL(data!, format, ref medium).ThrowOnFailure();
            }
            catch (NotSupportedException ex)
            {
                // BinaryFormatter is disabled. As all errors get swallowed by Windows, put the exception on the
                // clipboard so consumers can get some indication as to what is wrong. (We handle the binary formatting
                // of this exception, so it will always work.)
                SaveDataToHGLOBAL(ex, format, ref medium).ThrowOnFailure();
            }

            return;
        }

        if (formatetc.tymed.HasFlag(TYMED.TYMED_GDI))
        {
            if (format.Equals(DataFormats.Bitmap) && data is Bitmap bitmap)
            {
                // Save bitmap
                medium.unionmember = (nint)GetCompatibleBitmap(bitmap);
            }

            return;
        }

        Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);

        static HBITMAP GetCompatibleBitmap(Bitmap bitmap)
        {
            using var screenDC = GetDcScope.ScreenDC;

            // GDI+ returns a DIBSECTION based HBITMAP. The clipboard only deals well with bitmaps created using
            // CreateCompatibleBitmap(). So, we convert the DIBSECTION into a compatible bitmap.
            HBITMAP hbitmap = bitmap.GetHBITMAP();

            // Create a compatible DC to render the source bitmap.
            using PInvoke.CreateDcScope sourceDC = new(screenDC);
            using PInvoke.SelectObjectScope sourceBitmapSelection = new(sourceDC, hbitmap);

            // Create a compatible DC and a new compatible bitmap.
            using PInvoke.CreateDcScope destinationDC = new(screenDC);
            HBITMAP compatibleBitmap = PInvoke.CreateCompatibleBitmap(screenDC, bitmap.Size.Width, bitmap.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            using PInvoke.SelectObjectScope destinationBitmapSelection = new(destinationDC, compatibleBitmap);
            PInvoke.BitBlt(
                destinationDC,
                0,
                0,
                bitmap.Size.Width,
                bitmap.Size.Height,
                sourceDC,
                0,
                0,
                ROP_CODE.SRCCOPY);

            return compatibleBitmap;
        }
    }

    int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc)
    {
        CompModSwitches.DataObject.TraceVerbose("QueryGetData");
        if (_innerData is ComDataObjectAdapter converter)
        {
            return converter.OleDataObject.QueryGetData(ref formatetc);
        }

        if (formatetc.dwAspect != DVASPECT.DVASPECT_CONTENT)
        {
            return (int)HRESULT.DV_E_DVASPECT;
        }

        if (!GetTymedUseable(formatetc.tymed))
        {
            return (int)HRESULT.DV_E_TYMED;
        }

        if (formatetc.cfFormat == 0)
        {
            CompModSwitches.DataObject.TraceVerbose("QueryGetData::returning S_FALSE because cfFormat == 0");
            return (int)HRESULT.S_FALSE;
        }

        if (!GetDataPresent(DataFormats.GetFormat(formatetc.cfFormat).Name))
        {
            return (int)HRESULT.DV_E_FORMATETC;
        }

        CompModSwitches.DataObject.TraceVerbose($"QueryGetData::cfFormat {(ushort)formatetc.cfFormat} found");
        return (int)HRESULT.S_OK;
    }

    void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease)
    {
        CompModSwitches.DataObject.TraceVerbose("SetData");
        if (_innerData is ComDataObjectAdapter converter)
        {
            converter.OleDataObject.SetData(ref pFormatetcIn, ref pmedium, fRelease);
            return;
        }

        if (DragDropHelper.IsInDragLoopFormat(pFormatetcIn) || DragDropHelper.IsInDragLoop(_innerData))
        {
            string formatName = DataFormats.GetFormat(pFormatetcIn.cfFormat).Name;
            if (_innerData.GetDataPresent(formatName) && _innerData.GetData(formatName) is DragDropFormat dragDropFormat)
            {
                dragDropFormat.RefreshData(pFormatetcIn.cfFormat, pmedium, !fRelease);
                CompModSwitches.DataObject.TraceVerbose($" drag-and-drop private format refreshed '{formatName}'");
            }
            else
            {
                _innerData.SetData(formatName, new DragDropFormat(pFormatetcIn.cfFormat, pmedium, !fRelease));
                CompModSwitches.DataObject.TraceVerbose($" drag-and-drop private format loaded '{formatName}'");
            }

            return;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    ///  We are restricting serialization of formats that represent strings, bitmaps or OLE types.
    /// </summary>
    /// <param name="format">format name</param>
    /// <returns>true - serialize only safe types, strings or bitmaps.</returns>
    private static bool RestrictDeserializationToSafeTypes(string format)
        => format is DataFormats.StringConstant
            or BitmapFullName
            or DataFormats.CsvConstant
            or DataFormats.DibConstant
            or DataFormats.DifConstant
            or DataFormats.LocaleConstant
            or DataFormats.PenDataConstant
            or DataFormats.RiffConstant
            or DataFormats.SymbolicLinkConstant
            or DataFormats.TiffConstant
            or DataFormats.WaveAudioConstant
            or DataFormats.BitmapConstant
            or DataFormats.EmfConstant
            or DataFormats.PaletteConstant
            or DataFormats.WmfConstant;

    private HRESULT SaveDataToHGLOBAL(object data, string format, ref STGMEDIUM medium) => format switch
    {
        _ when data is Stream dataStream
            => SaveStreamToHGLOBAL(ref Unsafe.As<nint, HGLOBAL>(ref medium.unionmember), dataStream),
        DataFormats.TextConstant or DataFormats.RtfConstant or DataFormats.OemTextConstant
            => SaveStringToHGLOBAL((HGLOBAL)medium.unionmember, data.ToString()!, unicode: false),
        DataFormats.HtmlConstant
            => SaveHtmlToHGLOBAL((HGLOBAL)medium.unionmember, data.ToString()!),
        DataFormats.UnicodeTextConstant
            => SaveStringToHGLOBAL((HGLOBAL)medium.unionmember, data.ToString()!, unicode: true),
        DataFormats.FileDropConstant
            => SaveFileListToHGLOBAL((HGLOBAL)medium.unionmember, (string[])data),
        CF_DEPRECATED_FILENAME
            => SaveStringToHGLOBAL((HGLOBAL)medium.unionmember, ((string[])data)[0], unicode: false),
        CF_DEPRECATED_FILENAMEW
            => SaveStringToHGLOBAL((HGLOBAL)medium.unionmember, ((string[])data)[0], unicode: true),
        DataFormats.DibConstant when data is Image
            // GDI+ does not properly handle saving to DIB images. Since the clipboard will take
            // an HBITMAP and publish a Dib, we don't need to support this.
            => HRESULT.DV_E_TYMED,
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        _ when format == DataFormats.SerializableConstant || data is ISerializable || data.GetType().IsSerializable
#pragma warning restore
            => SaveObjectToHGLOBAL(ref Unsafe.As<nint, HGLOBAL>(ref medium.unionmember), data, RestrictDeserializationToSafeTypes(format)),
        _ => HRESULT.E_FAIL
    };

    private static HRESULT SaveObjectToHGLOBAL(ref HGLOBAL hglobal, object data, bool restrictSerialization)
    {
        using MemoryStream stream = new();
        stream.Write(s_serializedObjectID);
        long position = stream.Position;
        bool success = false;

        try
        {
            success = BinaryFormatWriter.TryWriteFrameworkObject(stream, data);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Being extra cautious here, but the Try method above should never throw in normal circumstances.
            Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
        }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
        if (!success)
        {
            new BinaryFormatter()
            {
                Binder = restrictSerialization ? new BitmapBinder() : null
            }.Serialize(stream, data);
        }
#pragma warning restore SYSLIB0011

        return SaveStreamToHGLOBAL(ref hglobal, stream);
    }

    private static HRESULT SaveStreamToHGLOBAL(ref HGLOBAL hglobal, Stream stream)
    {
        if (hglobal != 0)
        {
            PInvoke.GlobalFree(hglobal);
        }

        int size = checked((int)stream.Length);
        hglobal = PInvoke.GlobalAlloc(GMEM_MOVEABLE, (uint)size);
        if (hglobal == 0)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        void* buffer = PInvoke.GlobalLock(hglobal);
        if (buffer is null)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        try
        {
            Span<byte> span = new(buffer, size);
            stream.Position = 0;
            stream.Read(span);
        }
        finally
        {
            PInvoke.GlobalUnlock(hglobal);
        }

        return HRESULT.S_OK;
    }

    /// <summary>
    ///  Saves a list of files out to the handle in HDROP format.
    /// </summary>
    private HRESULT SaveFileListToHGLOBAL(HGLOBAL hglobal, string[] files)
    {
        if (files is null || files.Length == 0)
        {
            return HRESULT.S_OK;
        }

        if (hglobal == 0)
        {
            return HRESULT.E_INVALIDARG;
        }

        // CF_HDROP consists of a DROPFILES struct followed by an list of strings including the terminating null
        // character. An additional null character is appended to the final string to terminate the array.
        //
        // E.g. if the files c:\temp1.txt and c:\temp2.txt are being transferred, the character array is:
        // "c:\temp1.txt\0c:\temp2.txt\0\0"

        // Determine the size of the data structure.
        uint sizeInBytes = (uint)sizeof(DROPFILES);
        foreach (string file in files)
        {
            sizeInBytes += (uint)(file.Length + 1) * sizeof(char);
        }

        sizeInBytes += sizeof(char);

        // Allocate the Win32 memory
        HGLOBAL newHandle = PInvoke.GlobalReAlloc(hglobal, sizeInBytes, (uint)GMEM_MOVEABLE);
        if (newHandle == 0)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        void* buffer = PInvoke.GlobalLock(newHandle);
        if (buffer is null)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        // Write out the DROPFILES struct.
        DROPFILES* dropFiles = (DROPFILES*)buffer;
        *dropFiles = new DROPFILES()
        {
            pFiles = (uint)sizeof(DROPFILES),
            pt = Point.Empty,
            fNC = false,
            fWide = true
        };

        Span<char> fileBuffer = new(
            (char*)((byte*)buffer + dropFiles->pFiles),
            ((int)sizeInBytes - (int)dropFiles->pFiles) / sizeof(char));

        // Write out the strings.
        foreach (string file in files)
        {
            file.CopyTo(fileBuffer);
            fileBuffer[file.Length] = '\0';
            fileBuffer = fileBuffer[(file.Length + 1)..];
        }

        fileBuffer[0] = '\0';

        PInvoke.GlobalUnlock(newHandle);
        return HRESULT.S_OK;
    }

    /// <summary>
    ///  Save string to handle. If unicode is set to true then the string is saved as Unicode, else it is saves as DBCS.
    /// </summary>
    private HRESULT SaveStringToHGLOBAL(HGLOBAL hglobal, string value, bool unicode)
    {
        if (hglobal == 0)
        {
            return HRESULT.E_INVALIDARG;
        }

        HGLOBAL newHandle = default;
        if (unicode)
        {
            uint byteSize = (uint)value.Length * sizeof(char) + sizeof(char);
            newHandle = PInvoke.GlobalReAlloc(hglobal, byteSize, (uint)(GMEM_MOVEABLE | GMEM_ZEROINIT));
            if (newHandle == 0)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            char* buffer = (char*)PInvoke.GlobalLock(newHandle);
            if (buffer is null)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            Span<char> data = new(buffer, value.Length + 1);
            value.AsSpan().CopyTo(data);

            // Null terminate.
            data[value.Length] = '\0';
        }
        else
        {
            fixed (char* c = value)
            {
                int pinvokeSize = PInvoke.WideCharToMultiByte(PInvoke.CP_ACP, 0, value, value.Length, null, 0, null, null);
                newHandle = PInvoke.GlobalReAlloc(hglobal, (uint)pinvokeSize + 1, (uint)GMEM_MOVEABLE | (uint)GMEM_ZEROINIT);
                if (newHandle == 0)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                byte* buffer = (byte*)PInvoke.GlobalLock(newHandle);
                if (buffer is null)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                PInvoke.WideCharToMultiByte(PInvoke.CP_ACP, 0, value, value.Length, buffer, pinvokeSize, null, null);

                // Null terminate
                buffer[pinvokeSize] = 0;
            }
        }

        PInvoke.GlobalUnlock(newHandle);
        return HRESULT.S_OK;
    }

    private static HRESULT SaveHtmlToHGLOBAL(HGLOBAL hglobal, string value)
    {
        if (hglobal == 0)
        {
            return HRESULT.E_INVALIDARG;
        }

        int byteLength = Encoding.UTF8.GetByteCount(value);
        HGLOBAL newHandle = PInvoke.GlobalReAlloc(hglobal, (uint)byteLength + 1, (uint)(GMEM_MOVEABLE | GMEM_ZEROINIT));
        if (newHandle == 0)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        byte* buffer = (byte*)PInvoke.GlobalLock(newHandle);
        if (buffer is null)
        {
            return HRESULT.E_OUTOFMEMORY;
        }

        try
        {
            Span<byte> span = new(buffer, byteLength + 1);
            Encoding.UTF8.GetBytes(value, span);

            // Null terminate
            span[byteLength] = 0;
        }
        finally
        {
            PInvoke.GlobalUnlock(newHandle);
        }

        return HRESULT.S_OK;
    }

    /// <summary>
    ///  Stores the specified data and its associated format in this instance, using the automatic conversion
    ///  parameter to specify whether the data can be converted to another format.
    /// </summary>
    public virtual void SetData(string format, bool autoConvert, object? data)
    {
        CompModSwitches.DataObject.TraceVerbose($"Set data: {format}, {autoConvert}, {data ?? "(null)"}");
        _innerData.SetData(format, autoConvert, data);
    }

    /// <summary>
    ///  Stores the specified data and its associated format in this instance.
    /// </summary>
    public virtual void SetData(string format, object? data)
    {
        CompModSwitches.DataObject.TraceVerbose($"Set data: {format}, {data ?? "(null)"}");
        _innerData.SetData(format, data);
    }

    /// <summary>
    ///  Stores the specified data and its associated class type in this instance.
    /// </summary>
    public virtual void SetData(Type format, object? data)
    {
        CompModSwitches.DataObject.TraceVerbose($"Set data: {format?.FullName ?? "(null)"}, {data ?? "(null)"}");
        _innerData.SetData(format!, data);
    }

    /// <summary>
    ///  Stores the specified data in this instance, using the class of the data for the format.
    /// </summary>
    public virtual void SetData(object? data)
    {
        CompModSwitches.DataObject.TraceVerbose($"Set data: {data ?? "(null)"}");
        _innerData.SetData(data);
    }

    HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
    {
        if (pmedium is null)
        {
            return HRESULT.E_POINTER;
        }

        ((ComTypes.IDataObject)this).GetData(ref *(FORMATETC*)pformatetcIn, out STGMEDIUM medium);
        *pmedium = (Com.STGMEDIUM)medium;
        return HRESULT.S_OK;
    }

    HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
    {
        if (pmedium is null)
        {
            return HRESULT.E_POINTER;
        }

        STGMEDIUM medium = (STGMEDIUM)(*pmedium);
        ((ComTypes.IDataObject)this).GetDataHere(ref *(FORMATETC*)pformatetc, ref medium);
        *pmedium = (Com.STGMEDIUM)medium;
        return HRESULT.S_OK;
    }

    HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc)
        => (HRESULT)((ComTypes.IDataObject)this).QueryGetData(ref *(FORMATETC*)pformatetc);

    HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut)
        => (HRESULT)((ComTypes.IDataObject)this).GetCanonicalFormatEtc(ref *(FORMATETC*)pformatectIn, out *(FORMATETC*)pformatetcOut);

    HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
    {
        if (pmedium is null)
        {
            return HRESULT.E_POINTER;
        }

        STGMEDIUM medium = (STGMEDIUM)(*pmedium);
        ((ComTypes.IDataObject)this).SetData(ref *(FORMATETC*)pformatetc, ref medium, fRelease);
        return HRESULT.S_OK;
    }

    HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
    {
        if (ppenumFormatEtc is null)
        {
            return HRESULT.E_POINTER;
        }

        var comTypeFormatEtc = ((ComTypes.IDataObject)this).EnumFormatEtc((DATADIR)(int)dwDirection);
        *ppenumFormatEtc = ComHelpers.TryGetComPointer<Com.IEnumFORMATETC>(comTypeFormatEtc, out HRESULT hr);
        return hr.Succeeded ? HRESULT.S_OK : HRESULT.E_NOINTERFACE;
    }

    HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
    {
        var adviseSink = (IAdviseSink)ComHelpers.GetObjectForIUnknown((Com.IUnknown*)pAdvSink);
        return (HRESULT)((ComTypes.IDataObject)this).DAdvise(ref *(FORMATETC*)pformatetc, (ADVF)advf, adviseSink, out *(int*)pdwConnection);
    }

    HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
    {
        ((ComTypes.IDataObject)this).DUnadvise((int)dwConnection);
        return HRESULT.S_OK;
    }

    HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
    {
        if (ppenumAdvise is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppenumAdvise = null;

        HRESULT hr = (HRESULT)((ComTypes.IDataObject)this).EnumDAdvise(out var enumAdvice);
        if (hr.Failed)
        {
            return hr;
        }

        *ppenumAdvise = ComHelpers.TryGetComPointer<Com.IEnumSTATDATA>(enumAdvice, out hr);
        return hr.Succeeded ? hr : HRESULT.E_NOINTERFACE;
    }
}
