// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Interop;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a basic data transfer mechanism.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public partial class DataObject : IDataObject, ComTypes.IDataObject, Com.IDataObject.Interface, Com.IManagedWrapper<Com.IDataObject>
    {
        private const string CF_DEPRECATED_FILENAME = "FileName";
        private const string CF_DEPRECATED_FILENAMEW = "FileNameW";

        private const int DATA_S_SAMEFORMATETC = 0x00040130;

        private static readonly TYMED[] s_allowedTymeds =
            new TYMED[]
            {
                TYMED.TYMED_HGLOBAL,
                TYMED.TYMED_ISTREAM,
                TYMED.TYMED_GDI
            };

        private readonly IDataObject _innerData;

        // We use this to identify that a stream is actually a serialized object.  On read,
        // we don't know if the contents of a stream were saved "raw" or if the stream is really
        // pointing to a serialized object.  If we saved an object, we prefix it with this
        // guid.

        private static readonly byte[] s_serializedObjectID = new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray();

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataObject"/> class, with the specified <see cref="IDataObject"/>.
        /// </summary>
        internal DataObject(IDataObject data)
        {
            CompModSwitches.DataObject.TraceVerbose("Constructed DataObject based on IDataObject");
            _innerData = data;
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
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
                _innerData = new OleConverter(data);
            }

            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
        /// </summary>
        public DataObject()
        {
            CompModSwitches.DataObject.TraceVerbose("Constructed DataObject standalone");
            _innerData = new DataStore();
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
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
                _innerData = new OleConverter(comDataObject);
            }
            else
            {
                _innerData = new DataStore();
                SetData(data);
            }

            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data and its
        ///  associated format.
        /// </summary>
        public DataObject(string format, object data) : this()
        {
            SetData(format, data);
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
        }

        private static HBITMAP GetCompatibleBitmap(Bitmap bm)
        {
            using var screenDC = User32.GetDcScope.ScreenDC;

            // GDI+ returns a DIBSECTION based HBITMAP. The clipboard deals well
            // only with bitmaps created using CreateCompatibleBitmap(). So, we
            // convert the DIBSECTION into a compatible bitmap.
            HBITMAP hBitmap = bm.GetHBITMAP();

            // Create a compatible DC to render the source bitmap.
            using PInvoke.CreateDcScope sourceDC = new(screenDC);
            using PInvoke.SelectObjectScope sourceBitmapSelection = new(sourceDC, hBitmap);

            // Create a compatible DC and a new compatible bitmap.
            using PInvoke.CreateDcScope destinationDC = new(screenDC);
            HBITMAP bitmap = PInvoke.CreateCompatibleBitmap(screenDC, bm.Size.Width, bm.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            using PInvoke.SelectObjectScope destinationBitmapSelection = new(destinationDC, bitmap);
            PInvoke.BitBlt(
                destinationDC,
                0,
                0,
                bm.Size.Width,
                bm.Size.Height,
                sourceDC,
                0,
                0,
                ROP_CODE.SRCCOPY);

            return bitmap;
        }

        /// <summary>
        ///  Retrieves the data associated with the specified data format, using an automated conversion parameter to
        ///  determine whether to convert the data to the format.
        /// </summary>
        public virtual object? GetData(string format, bool autoConvert)
        {
            CompModSwitches.DataObject.TraceVerbose($"Request data: {format}, {autoConvert}");
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            return _innerData.GetData(format, autoConvert);
        }

        /// <summary>
        ///  Retrieves the data associated with the specified data format.
        /// </summary>
        public virtual object? GetData(string format)
        {
            CompModSwitches.DataObject.TraceVerbose($"Request data: {format}");
            return GetData(format, true);
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
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
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
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            return _innerData.GetFormats(autoConvert);
        }

        /// <summary>
        ///  Gets a list of all formats that data stored in this instance is associated
        ///  with or can be converted to.
        /// </summary>
        public virtual string[] GetFormats()
        {
            CompModSwitches.DataObject.TraceVerbose("Check formats:");
            return GetFormats(autoConvert: true);
        }

        // <-- WHIDBEY ADDITIONS

        public virtual bool ContainsAudio()
        {
            return GetDataPresent(DataFormats.WaveAudio, autoConvert: false);
        }

        public virtual bool ContainsFileDropList()
        {
            return GetDataPresent(DataFormats.FileDrop, autoConvert: true);
        }

        public virtual bool ContainsImage()
        {
            return GetDataPresent(DataFormats.Bitmap, autoConvert: true);
        }

        public virtual bool ContainsText()
        {
            return ContainsText(TextDataFormat.UnicodeText);
        }

        public virtual bool ContainsText(TextDataFormat format)
        {
            //valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            return GetDataPresent(ConvertToDataFormats(format), autoConvert: false);
        }

        public virtual Stream? GetAudioStream()
        {
            return GetData(DataFormats.WaveAudio, false) as Stream;
        }

        public virtual StringCollection GetFileDropList()
        {
            StringCollection dropList = new StringCollection();
            if (GetData(DataFormats.FileDrop, true) is string[] strings)
            {
                dropList.AddRange(strings);
            }

            return dropList;
        }

        public virtual Image? GetImage()
        {
            return GetData(DataFormats.Bitmap, true) as Image;
        }

        public virtual string GetText()
        {
            return GetText(TextDataFormat.UnicodeText);
        }

        public virtual string GetText(TextDataFormat format)
        {
            // Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            return GetData(ConvertToDataFormats(format), false) is string text ? text : string.Empty;
        }

        public virtual void SetAudio(byte[] audioBytes)
        {
            ArgumentNullException.ThrowIfNull(audioBytes);

            SetAudio(new MemoryStream(audioBytes));
        }

        public virtual void SetAudio(Stream audioStream)
        {
            ArgumentNullException.ThrowIfNull(audioStream);

            SetData(DataFormats.WaveAudio, false, audioStream);
        }

        public virtual void SetFileDropList(StringCollection filePaths)
        {
            ArgumentNullException.ThrowIfNull(filePaths);

            string[] strings = new string[filePaths.Count];
            filePaths.CopyTo(strings, 0);
            SetData(DataFormats.FileDrop, true, strings);
        }

        public virtual void SetImage(Image image)
        {
            ArgumentNullException.ThrowIfNull(image);

            SetData(DataFormats.Bitmap, true, image);
        }

        public virtual void SetText(string textData)
        {
            SetText(textData, TextDataFormat.UnicodeText);
        }

        public virtual void SetText(string textData, TextDataFormat format)
        {
            textData.ThrowIfNullOrEmpty();

            // Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(format, nameof(format));

            SetData(ConvertToDataFormats(format), false, textData);
        }

        private static string ConvertToDataFormats(TextDataFormat format) => format switch
        {
            TextDataFormat.UnicodeText => DataFormats.UnicodeText,
            TextDataFormat.Rtf => DataFormats.Rtf,
            TextDataFormat.Html => DataFormats.Html,
            TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
            _ => DataFormats.UnicodeText,
        };

        // END - WHIDBEY ADDITIONS -->

        /// <summary>
        ///  Returns all the "synonyms" for the specified format.
        /// </summary>
        private static string[]? GetMappedFormats(string format)
        {
            if (format is null)
            {
                return null;
            }

            if (format.Equals(DataFormats.Text)
                || format.Equals(DataFormats.UnicodeText)
                || format.Equals(DataFormats.StringFormat))
            {
                return new string[]
                {
                    DataFormats.StringFormat,
                    DataFormats.UnicodeText,
                    DataFormats.Text,
                };
            }

            if (format.Equals(DataFormats.FileDrop)
                || format.Equals(CF_DEPRECATED_FILENAME)
                || format.Equals(CF_DEPRECATED_FILENAMEW))
            {
                return new string[]
                {
                    DataFormats.FileDrop,
                    CF_DEPRECATED_FILENAMEW,
                    CF_DEPRECATED_FILENAME,
                };
            }

            if (format.Equals(DataFormats.Bitmap)
                || format.Equals((typeof(Bitmap)).FullName))
            {
                return new string[]
                {
                    (typeof(Bitmap)).FullName!,
                    DataFormats.Bitmap,
                };
            }

            return new string[] { format };
        }

        /// <summary>
        ///  Returns true if the tymed is useable.
        /// </summary>
        private static bool GetTymedUseable(TYMED tymed)
        {
            for (int i = 0; i < s_allowedTymeds.Length; i++)
            {
                if ((tymed & s_allowedTymeds[i]) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Populates Ole datastructes from a WinForms dataObject. This is the core
        ///  of WinForms to OLE conversion.
        /// </summary>
        private void GetDataIntoOleStructs(ref FORMATETC formatetc, ref STGMEDIUM medium)
        {
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

            if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != 0)
            {
                HRESULT hr = SaveDataToHandle(data!, format, ref medium);
                hr.ThrowOnFailure();
            }
            else if ((formatetc.tymed & TYMED.TYMED_GDI) != 0)
            {
                if (format.Equals(DataFormats.Bitmap) && data is Bitmap bm
                    && bm is not null)
                {
                    // Save bitmap
                    medium.unionmember = (IntPtr)GetCompatibleBitmap(bm);
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
            }
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
        {
            CompModSwitches.DataObject.TraceVerbose("DAdvise");
            if (_innerData is OleConverter converter)
            {
                return converter.OleDataObject.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);
            }

            pdwConnection = 0;
            return (int)HRESULT.E_NOTIMPL;
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        void ComTypes.IDataObject.DUnadvise(int dwConnection)
        {
            CompModSwitches.DataObject.TraceVerbose("DUnadvise");
            if (_innerData is OleConverter converter)
            {
                converter.OleDataObject.DUnadvise(dwConnection);
                return;
            }

            Marshal.ThrowExceptionForHR((int)HRESULT.E_NOTIMPL);
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
        {
            CompModSwitches.DataObject.TraceVerbose("EnumDAdvise");
            if (_innerData is OleConverter converter)
            {
                return converter.OleDataObject.EnumDAdvise(out enumAdvise);
            }

            enumAdvise = null;
            return (int)HRESULT.OLE_E_ADVISENOTSUPPORTED;
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection)
        {
            CompModSwitches.DataObject.TraceVerbose($"EnumFormatEtc: {dwDirection}");
            if (_innerData is OleConverter converter)
            {
                return converter.OleDataObject.EnumFormatEtc(dwDirection);
            }

            if (dwDirection == DATADIR.DATADIR_GET)
            {
                return new FormatEnumerator(this);
            }

            throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
        }

        /// <summary>
        /// Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut)
        {
            CompModSwitches.DataObject.TraceVerbose("GetCanonicalFormatEtc");
            if (_innerData is OleConverter converter)
            {
                return converter.OleDataObject.GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);
            }

            pformatetcOut = default;
            return DATA_S_SAMEFORMATETC;
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium)
        {
            CompModSwitches.DataObject.TraceVerbose("GetData");
            if (_innerData is OleConverter converter)
            {
                converter.OleDataObject.GetData(ref formatetc, out medium);
                return;
            }
            else if (DragDropHelper.IsInDragLoop(_innerData))
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

            if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != 0)
            {
                medium.tymed = TYMED.TYMED_HGLOBAL;
                medium.unionmember = PInvoke.GlobalAlloc(
                    GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
                    1);
                if (medium.unionmember == IntPtr.Zero)
                {
                    throw new OutOfMemoryException();
                }

                try
                {
                    ((ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
                }
                catch
                {
                    PInvoke.GlobalFree(medium.unionmember);
                    medium.unionmember = IntPtr.Zero;
                    throw;
                }
            }
            else
            {
                medium.tymed = formatetc.tymed;
                ((ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
            }
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium)
        {
            CompModSwitches.DataObject.TraceVerbose("GetDataHere");
            if (_innerData is OleConverter converter)
            {
                converter.OleDataObject.GetDataHere(ref formatetc, ref medium);
            }
            else
            {
                GetDataIntoOleStructs(ref formatetc, ref medium);
            }
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc)
        {
            CompModSwitches.DataObject.TraceVerbose("QueryGetData");
            if (_innerData is OleConverter converter)
            {
                return converter.OleDataObject.QueryGetData(ref formatetc);
            }

            if (formatetc.dwAspect == DVASPECT.DVASPECT_CONTENT)
            {
                if (GetTymedUseable(formatetc.tymed))
                {
                    if (formatetc.cfFormat == 0)
                    {
                        CompModSwitches.DataObject.TraceVerbose(
                            "QueryGetData::returning S_FALSE because cfFormat == 0");
                        return (int)HRESULT.S_FALSE;
                    }
                    else if (!GetDataPresent(DataFormats.GetFormat(formatetc.cfFormat).Name))
                    {
                        return (int)HRESULT.DV_E_FORMATETC;
                    }
                }
                else
                {
                    return (int)HRESULT.DV_E_TYMED;
                }
            }
            else
            {
                return (int)HRESULT.DV_E_DVASPECT;
            }

            CompModSwitches.DataObject.TraceVerbose(
                $"QueryGetData::cfFormat {(ushort)formatetc.cfFormat} found");

            return (int)HRESULT.S_OK;
        }

        /// <summary>
        ///  Part of <see cref="ComTypes.IDataObject"/>, used to interop with OLE.
        /// </summary>
        void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease)
        {
            CompModSwitches.DataObject.TraceVerbose("SetData");
            if (_innerData is OleConverter converter)
            {
                converter.OleDataObject.SetData(ref pFormatetcIn, ref pmedium, fRelease);
                return;
            }
            else if (DragDropHelper.IsInDragLoopFormat(pFormatetcIn) || DragDropHelper.IsInDragLoop(_innerData))
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
        {
            return format.Equals(DataFormats.StringFormat)
                || format.Equals(typeof(Bitmap).FullName)
                || format.Equals(DataFormats.CommaSeparatedValue)
                || format.Equals(DataFormats.Dib)
                || format.Equals(DataFormats.Dif)
                || format.Equals(DataFormats.Locale)
                || format.Equals(DataFormats.PenData)
                || format.Equals(DataFormats.Riff)
                || format.Equals(DataFormats.SymbolicLink)
                || format.Equals(DataFormats.Tiff)
                || format.Equals(DataFormats.WaveAudio)
                || format.Equals(DataFormats.Bitmap)
                || format.Equals(DataFormats.EnhancedMetafile)
                || format.Equals(DataFormats.Palette)
                || format.Equals(DataFormats.MetafilePict);
        }

        private HRESULT SaveDataToHandle(object data, string format, ref STGMEDIUM medium)
        {
            HRESULT hr = HRESULT.E_FAIL;
            if (data is Stream dataStream)
            {
                hr = SaveStreamToHandle(ref medium.unionmember, dataStream);
            }
            else if (format.Equals(DataFormats.Text)
                || format.Equals(DataFormats.Rtf)
                || format.Equals(DataFormats.OemText))
            {
                hr = SaveStringToHandle(medium.unionmember, data.ToString()!, false);
            }
            else if (format.Equals(DataFormats.Html))
            {
                hr = SaveHtmlToHandle(medium.unionmember, data.ToString()!);
            }
            else if (format.Equals(DataFormats.UnicodeText))
            {
                hr = SaveStringToHandle(medium.unionmember, data.ToString()!, true);
            }
            else if (format.Equals(DataFormats.FileDrop))
            {
                hr = SaveFileListToHandle(medium.unionmember, (string[])data);
            }
            else if (format.Equals(CF_DEPRECATED_FILENAME))
            {
                string[] filelist = (string[])data;
                hr = SaveStringToHandle(medium.unionmember, filelist[0], false);
            }
            else if (format.Equals(CF_DEPRECATED_FILENAMEW))
            {
                string[] filelist = (string[])data;
                hr = SaveStringToHandle(medium.unionmember, filelist[0], true);
            }
            else if (format.Equals(DataFormats.Dib) && data is Image)
            {
                // GDI+ does not properly handle saving to DIB images. Since the clipboard will take
                // an HBITMAP and publish a Dib, we don't need to support this.
                hr = HRESULT.DV_E_TYMED;
            }
            else if (format.Equals(DataFormats.Serializable)
                || data is ISerializable
                || (data is not null && data.GetType().IsSerializable))
            {
                hr = SaveObjectToHandle(ref medium.unionmember, data, RestrictDeserializationToSafeTypes(format));
            }

            return hr;
        }

        private static HRESULT SaveObjectToHandle(ref IntPtr handle, object data, bool restrictSerialization)
        {
            Stream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(s_serializedObjectID);
            SaveObjectToHandleSerializer(stream, data, restrictSerialization);
            return SaveStreamToHandle(ref handle, stream);
        }

        private static void SaveObjectToHandleSerializer(Stream stream, object data, bool restrictSerialization)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            if (restrictSerialization)
            {
                formatter.Binder = new BitmapBinder();
            }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
            formatter.Serialize(stream, data);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }

        /// <summary>
        ///  Saves stream out to handle.
        /// </summary>
        private static unsafe HRESULT SaveStreamToHandle(ref nint handle, Stream stream)
        {
            if (handle != 0)
            {
                PInvoke.GlobalFree(handle);
            }

            int size = (int)stream.Length;
            handle = PInvoke.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, (uint)size);
            if (handle == 0)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            void* ptr = PInvoke.GlobalLock(handle);
            if (ptr is null)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            try
            {
                var span = new Span<byte>(ptr, size);
                stream.Position = 0;
                stream.Read(span);
            }
            finally
            {
                PInvoke.GlobalUnlock(handle);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Saves a list of files out to the handle in HDROP format.
        /// </summary>
        private unsafe HRESULT SaveFileListToHandle(IntPtr handle, string[] files)
        {
            if (files is null || files.Length == 0)
            {
                return HRESULT.S_OK;
            }

            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }

            // CF_HDROP consists of a DROPFILES struct followed by an list of strings
            // including the terminating null character. An additional null character
            // is appended to the final string to terminate the array.
            // E.g. if the files c:\temp1.txt and c:\temp2.txt are being transferred,
            // the character array is: "c:\temp1.txt\0c:\temp2.txt\0\0"

            // Determine the size of the data structure.
            uint sizeInBytes = (uint)sizeof(DROPFILES);
            for (int i = 0; i < files.Length; i++)
            {
                sizeInBytes += ((uint)files[i].Length + 1) * 2;
            }

            sizeInBytes += 2;

            // Allocate the Win32 memory
            nint newHandle = PInvoke.GlobalReAlloc(
                handle,
                sizeInBytes,
                (uint)GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE);
            if (newHandle == 0)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            void* basePtr = PInvoke.GlobalLock(newHandle);
            if (basePtr is null)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            // Write out the DROPFILES struct.
            DROPFILES* pDropFiles = (DROPFILES*)basePtr;
            pDropFiles->pFiles = (uint)sizeof(DROPFILES);
            pDropFiles->pt = Point.Empty;
            pDropFiles->fNC = false;
            pDropFiles->fWide = true;

            char* dataPtr = (char*)((byte*)basePtr + pDropFiles->pFiles);

            // Write out the strings.
            for (int i = 0; i < files.Length; i++)
            {
                int bytesToCopy = files[i].Length * 2;
                fixed (char* pFile = files[i])
                {
                    Buffer.MemoryCopy(pFile, dataPtr, bytesToCopy, bytesToCopy);
                }

                dataPtr = (char*)((byte*)dataPtr + bytesToCopy);
                *dataPtr = '\0';
                dataPtr++;
            }

            *dataPtr = '\0';
            dataPtr++;

            PInvoke.GlobalUnlock(newHandle);
            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Save string to handle. If unicode is set to true then the string is saved as Unicode,
        ///  else it is saves as DBCS.
        /// </summary>
        private unsafe HRESULT SaveStringToHandle(IntPtr handle, string str, bool unicode)
        {
            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }

            nint newHandle = 0;
            if (unicode)
            {
                uint byteSize = (uint)str.Length * 2 + 2;
                newHandle = PInvoke.GlobalReAlloc(
                    handle,
                    byteSize,
                    (uint)(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT));
                if (newHandle == 0)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                char* ptr = (char*)PInvoke.GlobalLock(newHandle);
                if (ptr is null)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                var data = new Span<char>(ptr, str.Length + 1);
                str.AsSpan().CopyTo(data);
                data[str.Length] = '\0'; // Null terminator.
            }
            else
            {
                fixed (char* pStr = str)
                {
                    int pinvokeSize = PInvoke.WideCharToMultiByte(PInvoke.CP_ACP, 0, str, str.Length, null, 0, null, null);
                    newHandle = PInvoke.GlobalReAlloc(
                        handle,
                        (uint)pinvokeSize + 1,
                        (uint)GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | (uint)GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT);
                    if (newHandle == 0)
                    {
                        return HRESULT.E_OUTOFMEMORY;
                    }

                    byte* ptr = (byte*)PInvoke.GlobalLock(newHandle);
                    if (ptr is null)
                    {
                        return HRESULT.E_OUTOFMEMORY;
                    }

                    PInvoke.WideCharToMultiByte(PInvoke.CP_ACP, 0, str, str.Length, ptr, pinvokeSize, null, null);
                    ptr[pinvokeSize] = 0; // Null terminator
                }
            }

            PInvoke.GlobalUnlock(newHandle);
            return HRESULT.S_OK;
        }

        private static unsafe HRESULT SaveHtmlToHandle(IntPtr handle, string str)
        {
            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }

            int byteLength = Encoding.UTF8.GetByteCount(str);
            nint newHandle = PInvoke.GlobalReAlloc(
                handle,
                (uint)byteLength + 1,
                (uint)(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT));
            if (newHandle == 0)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            byte* ptr = (byte*)PInvoke.GlobalLock(newHandle);
            if (ptr is null)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            try
            {
                var span = new Span<byte>(ptr, byteLength + 1);
                Encoding.UTF8.GetBytes(str, span);
                span[byteLength] = 0; // Null terminator
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
            CompModSwitches.DataObject.TraceVerbose(
                $"Set data: {format}, {autoConvert}, {data ?? "(null)"}");
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            _innerData.SetData(format, autoConvert, data);
        }

        /// <summary>
        ///  Stores the specified data and its associated format in this instance.
        /// </summary>
        public virtual void SetData(string format, object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"Set data: {format}, {data ?? "(null)"}");
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            _innerData.SetData(format, data);
        }

        /// <summary>
        ///  Stores the specified data and its associated class type in this instance.
        /// </summary>
        public virtual void SetData(Type format, object? data)
        {
            CompModSwitches.DataObject.TraceVerbose(
                $"Set data: {format?.FullName ?? "(null)"}, {data ?? "(null)"}");
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            _innerData.SetData(format!, data);
        }

        /// <summary>
        ///  Stores the specified data in this instance, using the class of the data for the format.
        /// </summary>
        public virtual void SetData(object? data)
        {
            CompModSwitches.DataObject.TraceVerbose($"Set data: {data ?? "(null)"}");
            Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
            _innerData.SetData(data);
        }

        unsafe HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
        {
            if (pmedium is null)
            {
                return HRESULT.E_POINTER;
            }

            try
            {
                ((ComTypes.IDataObject)this).GetData(ref *(FORMATETC*)pformatetcIn, out STGMEDIUM medium);
                *pmedium = (Com.STGMEDIUM)medium;
                return HRESULT.S_OK;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (HRESULT)ex.HResult;
            }
        }

        unsafe HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
        {
            if (pmedium is null)
            {
                return HRESULT.E_POINTER;
            }

            try
            {
                STGMEDIUM medium = (STGMEDIUM)(*pmedium);
                ((ComTypes.IDataObject)this).GetDataHere(ref *(FORMATETC*)pformatetc, ref medium);
                *pmedium = (Com.STGMEDIUM)medium;
                return HRESULT.S_OK;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (HRESULT)ex.HResult;
            }
        }

        unsafe HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc)
            => (HRESULT)((ComTypes.IDataObject)this).QueryGetData(ref *(FORMATETC*)pformatetc);

        unsafe HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut)
            => (HRESULT)((ComTypes.IDataObject)this).GetCanonicalFormatEtc(ref *(FORMATETC*)pformatectIn, out *(FORMATETC*)pformatetcOut);

        unsafe HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
        {
            if (pmedium is null)
            {
                return HRESULT.E_POINTER;
            }

            try
            {
                STGMEDIUM medium = (STGMEDIUM)(*pmedium);
                ((ComTypes.IDataObject)this).SetData(ref *(FORMATETC*)pformatetc, ref medium, fRelease);
                return HRESULT.S_OK;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (HRESULT)ex.HResult;
            }
        }

        unsafe HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
        {
            if (ppenumFormatEtc is null)
            {
                return HRESULT.E_POINTER;
            }

            try
            {
                var comTypeFormatEtc = ((ComTypes.IDataObject)this).EnumFormatEtc((DATADIR)(int)dwDirection);
                if (!ComHelpers.TryGetComPointer(comTypeFormatEtc, out Com.IEnumFORMATETC* formatEtcPtr))
                {
                    return HRESULT.E_NOINTERFACE;
                }

                *ppenumFormatEtc = formatEtcPtr;
                return HRESULT.S_OK;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (HRESULT)ex.HResult;
            }
        }

        unsafe HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
        {
            var adviseSink = (IAdviseSink)Marshal.GetObjectForIUnknown((nint)(void*)pAdvSink);
            return (HRESULT)((ComTypes.IDataObject)this).DAdvise(ref *(FORMATETC*)pformatetc, (ADVF)advf, adviseSink, out *(int*)pdwConnection);
        }

        HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
        {
            try
            {
                ((ComTypes.IDataObject)this).DUnadvise((int)dwConnection);
                return HRESULT.S_OK;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (HRESULT)ex.HResult;
            }
        }

        unsafe HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
        {
            if (ppenumAdvise is null)
            {
                return HRESULT.E_POINTER;
            }

            *ppenumAdvise = null;

            var result = (HRESULT)((ComTypes.IDataObject)this).EnumDAdvise(out var enumAdvice);
            if (result.Failed)
            {
                return result;
            }

            if (!ComHelpers.TryGetComPointer(enumAdvice, out Com.IEnumSTATDATA* enumAdvicePtr))
            {
                return HRESULT.E_NOINTERFACE;
            }

            *ppenumAdvise = enumAdvicePtr;
            return result;
        }
    }
}
