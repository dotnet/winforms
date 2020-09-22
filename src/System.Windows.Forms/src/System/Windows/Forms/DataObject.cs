// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a basic data transfer mechanism.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public partial class DataObject : IDataObject, IComDataObject
    {
        private const string CF_DEPRECATED_FILENAME = "FileName";
        private const string CF_DEPRECATED_FILENAMEW = "FileNameW";

        private const int DATA_S_SAMEFORMATETC = 0x00040130;

        private static readonly TYMED[] ALLOWED_TYMEDS =
        new TYMED[] {
            TYMED.TYMED_HGLOBAL,
            TYMED.TYMED_ISTREAM,
            TYMED.TYMED_GDI};

        private readonly IDataObject innerData;

        // We use this to identify that a stream is actually a serialized object.  On read,
        // we don't know if the contents of a stream were saved "raw" or if the stream is really
        // pointing to a serialized object.  If we saved an object, we prefix it with this
        // guid.
        //
        private static readonly byte[] serializedObjectID = new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray();

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataObject'/> class, with the specified <see cref='IDataObject'/>.
        /// </summary>
        internal DataObject(IDataObject data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Constructed DataObject based on IDataObject");
            innerData = data;
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataObject'/> class, with the specified <see cref='IComDataObject'/>.
        /// </summary>
        internal DataObject(IComDataObject data)
        {
            if (data is DataObject)
            {
                innerData = data as IDataObject;
            }
            else
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Constructed DataObject based on IComDataObject");
                innerData = new OleConverter(data);
            }
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataObject'/>
        ///  class, which can store arbitrary data.
        /// </summary>
        public DataObject()
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Constructed DataObject standalone");
            innerData = new DataStore();
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataObject'/> class, containing the specified data.
        /// </summary>
        public DataObject(object data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Constructed DataObject base on Object: " + data.ToString());
            if (data is IDataObject && !Marshal.IsComObject(data))
            {
                innerData = (IDataObject)data;
            }
            else if (data is IComDataObject)
            {
                innerData = new OleConverter((IComDataObject)data);
            }
            else
            {
                innerData = new DataStore();
                SetData(data);
            }
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataObject'/> class, containing the specified data and its
        ///  associated format.
        /// </summary>
        public DataObject(string format, object data) : this()
        {
            SetData(format, data);
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
        }

        private Gdi32.HBITMAP GetCompatibleBitmap(Bitmap bm)
        {
            using var screenDC = User32.GetDcScope.ScreenDC;

            // GDI+ returns a DIBSECTION based HBITMAP. The clipboard deals well
            // only with bitmaps created using CreateCompatibleBitmap(). So, we
            // convert the DIBSECTION into a compatible bitmap.
            Gdi32.HBITMAP hBitmap = bm.GetHBITMAP();

            // Create a compatible DC to render the source bitmap.
            using var sourceDC = new Gdi32.CreateDcScope(screenDC);
            using var sourceBitmapSelection = new Gdi32.SelectObjectScope(sourceDC, hBitmap);

            // Create a compatible DC and a new compatible bitmap.
            using var destinationDC = new Gdi32.CreateDcScope(screenDC);
            Gdi32.HBITMAP bitmap = Gdi32.CreateCompatibleBitmap(screenDC, bm.Size.Width, bm.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            using var destinationBitmapSelection = new Gdi32.SelectObjectScope(destinationDC, bitmap);
            Gdi32.BitBlt(
                destinationDC,
                0,
                0,
                bm.Size.Width,
                bm.Size.Height,
                sourceDC,
                0,
                0,
                Gdi32.ROP.SRCCOPY);

            return bitmap;
        }

        /// <summary>
        ///  Retrieves the data associated with the specified data
        ///  format, using an automated conversion parameter to determine whether to convert
        ///  the data to the format.
        /// </summary>
        public virtual object GetData(string format, bool autoConvert)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Request data: " + format + ", " + autoConvert.ToString());
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            return innerData.GetData(format, autoConvert);
        }

        /// <summary>
        ///  Retrieves the data associated with the specified data
        ///  format.
        /// </summary>
        public virtual object GetData(string format)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Request data: " + format);
            return GetData(format, true);
        }

        /// <summary>
        ///  Retrieves the data associated with the specified class
        ///  type format.
        /// </summary>
        public virtual object GetData(Type format)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Request data: " + format?.FullName ?? "(null)");
            if (format is null)
            {
                return null;
            }

            return GetData(format.FullName);
        }

        /// <summary>
        ///  Determines whether data stored in this instance is
        ///  associated with, or can be converted to, the specified
        ///  format.
        /// </summary>
        public virtual bool GetDataPresent(Type format)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Check data: " + format?.FullName ?? "(null)");
            if (format is null)
            {
                return false;
            }

            bool b = GetDataPresent(format.FullName);
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "  ret: " + b.ToString());
            return b;
        }

        /// <summary>
        ///  Determines whether data stored in this instance is
        ///  associated with the specified format, using an automatic conversion
        ///  parameter to determine whether to convert the data to the format.
        /// </summary>
        public virtual bool GetDataPresent(string format, bool autoConvert)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Check data: " + format + ", " + autoConvert.ToString());
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            bool b = innerData.GetDataPresent(format, autoConvert);
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "  ret: " + b.ToString());
            return b;
        }

        /// <summary>
        ///  Determines whether data stored in this instance is
        ///  associated with, or can be converted to, the specified
        ///  format.
        /// </summary>
        public virtual bool GetDataPresent(string format)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Check data: " + format);
            bool b = GetDataPresent(format, true);
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "  ret: " + b.ToString());
            return b;
        }

        /// <summary>
        ///  Gets a list of all formats that data stored in this
        ///  instance is associated with or can be converted to, using an automatic
        ///  conversion parameter <paramref name="autoConvert"/> to
        ///  determine whether to retrieve all formats that the data can be converted to or
        ///  only native data formats.
        /// </summary>
        public virtual string[] GetFormats(bool autoConvert)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Check formats: " + autoConvert.ToString());
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            return innerData.GetFormats(autoConvert);
        }

        /// <summary>
        ///  Gets a list of all formats that data stored in this instance is associated
        ///  with or can be converted to.
        /// </summary>
        public virtual string[] GetFormats()
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Check formats:");
            return GetFormats(true);
        }

        // <-- WHIDBEY ADDITIONS

        public virtual bool ContainsAudio()
        {
            return GetDataPresent(DataFormats.WaveAudio, false);
        }

        public virtual bool ContainsFileDropList()
        {
            return GetDataPresent(DataFormats.FileDrop, true);
        }

        public virtual bool ContainsImage()
        {
            return GetDataPresent(DataFormats.Bitmap, true);
        }

        public virtual bool ContainsText()
        {
            return ContainsText(TextDataFormat.UnicodeText);
        }

        public virtual bool ContainsText(TextDataFormat format)
        {
            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            return GetDataPresent(ConvertToDataFormats(format), false);
        }

        public virtual Stream GetAudioStream()
        {
            return GetData(DataFormats.WaveAudio, false) as Stream;
        }

        public virtual StringCollection GetFileDropList()
        {
            StringCollection retVal = new StringCollection();
            if (GetData(DataFormats.FileDrop, true) is string[] strings)
            {
                retVal.AddRange(strings);
            }
            return retVal;
        }

        public virtual Image GetImage()
        {
            return GetData(DataFormats.Bitmap, true) as Image;
        }

        public virtual string GetText()
        {
            return GetText(TextDataFormat.UnicodeText);
        }

        public virtual string GetText(TextDataFormat format)
        {
            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            if (GetData(ConvertToDataFormats(format), false) is string text)
            {
                return text;
            }

            return string.Empty;
        }

        public virtual void SetAudio(byte[] audioBytes)
        {
            if (audioBytes is null)
            {
                throw new ArgumentNullException(nameof(audioBytes));
            }
            SetAudio(new MemoryStream(audioBytes));
        }

        public virtual void SetAudio(Stream audioStream)
        {
            if (audioStream is null)
            {
                throw new ArgumentNullException(nameof(audioStream));
            }
            SetData(DataFormats.WaveAudio, false, audioStream);
        }

        public virtual void SetFileDropList(StringCollection filePaths)
        {
            if (filePaths is null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }
            string[] strings = new string[filePaths.Count];
            filePaths.CopyTo(strings, 0);
            SetData(DataFormats.FileDrop, true, strings);
        }

        public virtual void SetImage(Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            SetData(DataFormats.Bitmap, true, image);
        }

        public virtual void SetText(string textData)
        {
            SetText(textData, TextDataFormat.UnicodeText);
        }

        public virtual void SetText(string textData, TextDataFormat format)
        {
            if (string.IsNullOrEmpty(textData))
            {
                throw new ArgumentNullException(nameof(textData));
            }

            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(format, (int)format, (int)TextDataFormat.Text, (int)TextDataFormat.CommaSeparatedValue))
            {
                throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(TextDataFormat));
            }

            SetData(ConvertToDataFormats(format), false, textData);
        }

        private static string ConvertToDataFormats(TextDataFormat format)
        {
            switch (format)
            {
                case TextDataFormat.UnicodeText:
                    return DataFormats.UnicodeText;

                case TextDataFormat.Rtf:
                    return DataFormats.Rtf;

                case TextDataFormat.Html:
                    return DataFormats.Html;

                case TextDataFormat.CommaSeparatedValue:
                    return DataFormats.CommaSeparatedValue;
            }

            return DataFormats.UnicodeText;
        }

        // END - WHIDBEY ADDITIONS -->

        /// <summary>
        ///  Returns all the "synonyms" for the specified format.
        /// </summary>
        private static string[] GetMappedFormats(string format)
        {
            if (format is null)
            {
                return null;
            }

            if (format.Equals(DataFormats.Text)
                || format.Equals(DataFormats.UnicodeText)
                || format.Equals(DataFormats.StringFormat))
            {
                return new string[] {
                    DataFormats.StringFormat,
                    DataFormats.UnicodeText,
                    DataFormats.Text,
                };
            }

            if (format.Equals(DataFormats.FileDrop)
                || format.Equals(CF_DEPRECATED_FILENAME)
                || format.Equals(CF_DEPRECATED_FILENAMEW))
            {
                return new string[] {
                    DataFormats.FileDrop,
                    CF_DEPRECATED_FILENAMEW,
                    CF_DEPRECATED_FILENAME,
                };
            }

            if (format.Equals(DataFormats.Bitmap)
                || format.Equals((typeof(Bitmap)).FullName))
            {
                return new string[] {
                    (typeof(Bitmap)).FullName,
                    DataFormats.Bitmap,
                };
            }

            return new string[] { format };
        }

        /// <summary>
        ///  Returns true if the tymed is useable.
        /// </summary>
        private bool GetTymedUseable(TYMED tymed)
        {
            for (int i = 0; i < ALLOWED_TYMEDS.Length; i++)
            {
                if ((tymed & ALLOWED_TYMEDS[i]) != 0)
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
        private void GetDataIntoOleStructs(ref FORMATETC formatetc,
                                           ref STGMEDIUM medium)
        {
            if (GetTymedUseable(formatetc.tymed) && GetTymedUseable(medium.tymed))
            {
                string format = DataFormats.GetFormat(formatetc.cfFormat).Name;

                if (GetDataPresent(format))
                {
                    object data = GetData(format);

                    if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != 0)
                    {
                        HRESULT hr = SaveDataToHandle(data, format, ref medium);
                        if (hr.Failed())
                        {
                            Marshal.ThrowExceptionForHR((int)hr);
                        }
                    }
                    else if ((formatetc.tymed & TYMED.TYMED_GDI) != 0)
                    {
                        if (format.Equals(DataFormats.Bitmap) && data is Bitmap bm
                            && bm != null)
                        {
                            // save bitmap
                            medium.unionmember = (IntPtr)GetCompatibleBitmap(bm);
                        }
                    }
                    else
                    {
                        Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
                    }
                }
                else
                {
                    Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_FORMATETC);
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
            }
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        int IComDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DAdvise");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);
            }
            pdwConnection = 0;
            return (int)HRESULT.E_NOTIMPL;
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        void IComDataObject.DUnadvise(int dwConnection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DUnadvise");
            if (innerData is OleConverter)
            {
                ((OleConverter)innerData).OleDataObject.DUnadvise(dwConnection);
                return;
            }
            Marshal.ThrowExceptionForHR((int)HRESULT.E_NOTIMPL);
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        int IComDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "EnumDAdvise");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.EnumDAdvise(out enumAdvise);
            }

            enumAdvise = null;
            return (int)HRESULT.OLE_E_ADVISENOTSUPPORTED;
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        IEnumFORMATETC IComDataObject.EnumFormatEtc(DATADIR dwDirection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "EnumFormatEtc: " + dwDirection.ToString());
            if (innerData is OleConverter innerDataOleConverter)
            {
                return innerDataOleConverter.OleDataObject.EnumFormatEtc(dwDirection);
            }
            if (dwDirection == DATADIR.DATADIR_GET)
            {
                return new FormatEnumerator(this);
            }

            throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        int IComDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "GetCanonicalFormatEtc");
            if (innerData is OleConverter innerDataOleConverter)
            {
                return innerDataOleConverter.OleDataObject.GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);
            }
            pformatetcOut = new FORMATETC();
            return DATA_S_SAMEFORMATETC;
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        void IComDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "GetData");
            if (innerData is OleConverter)
            {
                ((OleConverter)innerData).OleDataObject.GetData(ref formatetc, out medium);
                return;
            }

            medium = new STGMEDIUM();

            if (GetTymedUseable(formatetc.tymed))
            {
                if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != 0)
                {
                    medium.tymed = TYMED.TYMED_HGLOBAL;
                    medium.unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        1);
                    if (medium.unionmember == IntPtr.Zero)
                    {
                        throw new OutOfMemoryException();
                    }

                    try
                    {
                        ((IComDataObject)this).GetDataHere(ref formatetc, ref medium);
                    }
                    catch
                    {
                        Kernel32.GlobalFree(medium.unionmember);
                        medium.unionmember = IntPtr.Zero;
                        throw;
                    }
                }
                else
                {
                    medium.tymed = formatetc.tymed;
                    ((IComDataObject)this).GetDataHere(ref formatetc, ref medium);
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR((int)HRESULT.DV_E_TYMED);
            }
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        void IComDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "GetDataHere");
            if (innerData is OleConverter)
            {
                ((OleConverter)innerData).OleDataObject.GetDataHere(ref formatetc, ref medium);
            }
            else
            {
                GetDataIntoOleStructs(ref formatetc, ref medium);
            }
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        int IComDataObject.QueryGetData(ref FORMATETC formatetc)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "QueryGetData");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.QueryGetData(ref formatetc);
            }
            if (formatetc.dwAspect == DVASPECT.DVASPECT_CONTENT)
            {
                if (GetTymedUseable(formatetc.tymed))
                {
                    if (formatetc.cfFormat == 0)
                    {
                        Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "QueryGetData::returning S_FALSE because cfFormat == 0");
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
#if DEBUG
            int format = unchecked((ushort)formatetc.cfFormat);
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "QueryGetData::cfFormat " + format.ToString(CultureInfo.InvariantCulture) + " found");
#endif
            return (int)HRESULT.S_OK;
        }

        /// <summary>
        ///     Part of IComDataObject, used to interop with OLE.
        /// </summary>
        void IComDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "SetData");
            if (innerData is OleConverter)
            {
                ((OleConverter)innerData).OleDataObject.SetData(ref pFormatetcIn, ref pmedium, fRelease);
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
            return (format.Equals(DataFormats.StringFormat) ||
                    format.Equals(typeof(Bitmap).FullName) ||
                    format.Equals(DataFormats.CommaSeparatedValue) ||
                    format.Equals(DataFormats.Dib) ||
                    format.Equals(DataFormats.Dif) ||
                    format.Equals(DataFormats.Locale) ||
                    format.Equals(DataFormats.PenData) ||
                    format.Equals(DataFormats.Riff) ||
                    format.Equals(DataFormats.SymbolicLink) ||
                    format.Equals(DataFormats.Tiff) ||
                    format.Equals(DataFormats.WaveAudio) ||
                    format.Equals(DataFormats.Bitmap) ||
                    format.Equals(DataFormats.EnhancedMetafile) ||
                    format.Equals(DataFormats.Palette) ||
                    format.Equals(DataFormats.MetafilePict));
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
                hr = SaveStringToHandle(medium.unionmember, data.ToString(), false);
            }
            else if (format.Equals(DataFormats.Html))
            {
                hr = SaveHtmlToHandle(medium.unionmember, data.ToString());
            }
            else if (format.Equals(DataFormats.UnicodeText))
            {
                hr = SaveStringToHandle(medium.unionmember, data.ToString(), true);
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
                     || (data != null && data.GetType().IsSerializable))
            {
                hr = SaveObjectToHandle(ref medium.unionmember, data, DataObject.RestrictDeserializationToSafeTypes(format));
            }
            return hr;
        }

        private HRESULT SaveObjectToHandle(ref IntPtr handle, object data, bool restrictSerialization)
        {
            Stream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(serializedObjectID);
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

#pragma warning disable SYSLIB0011
            formatter.Serialize(stream, data);
#pragma warning restore SYSLIB0011
        }

        /// <summary>
        ///  Saves stream out to handle.
        /// </summary>
        private unsafe HRESULT SaveStreamToHandle(ref IntPtr handle, Stream stream)
        {
            if (handle != IntPtr.Zero)
            {
                Kernel32.GlobalFree(handle);
            }

            int size = (int)stream.Length;
            handle = Kernel32.GlobalAlloc(Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE, (uint)size);
            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            IntPtr ptr = Kernel32.GlobalLock(handle);
            if (ptr == IntPtr.Zero)
            {
                return HRESULT.E_OUTOFMEMORY;
            }
            try
            {
                var span = new Span<byte>(ptr.ToPointer(), size);
                stream.Position = 0;
                stream.Read(span);
            }
            finally
            {
                Kernel32.GlobalUnlock(handle);
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
            uint sizeInBytes = (uint)sizeof(Shell32.DROPFILES);
            for (int i = 0; i < files.Length; i++)
            {
                sizeInBytes += ((uint)files[i].Length + 1) * 2;
            }
            sizeInBytes += 2;

            // Allocate the Win32 memory
            IntPtr newHandle = Kernel32.GlobalReAlloc(
                handle,
                sizeInBytes,
                Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE);
            if (newHandle == IntPtr.Zero)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            IntPtr basePtr = Kernel32.GlobalLock(newHandle);
            if (basePtr == IntPtr.Zero)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            // Write out the DROPFILES struct.
            Shell32.DROPFILES* pDropFiles = (Shell32.DROPFILES*)basePtr;
            pDropFiles->pFiles = (uint)sizeof(Shell32.DROPFILES);
            pDropFiles->pt = Point.Empty;
            pDropFiles->fNC = BOOL.FALSE;
            pDropFiles->fWide = BOOL.TRUE;

            char* dataPtr = (char*)(basePtr + (int)pDropFiles->pFiles);

            // Write out the strings.
            for (int i = 0; i < files.Length; i++)
            {
                int bytesToCopy = files[i].Length * 2;
                fixed (char* pFile = files[i])
                {
                    Buffer.MemoryCopy(pFile, dataPtr, bytesToCopy, bytesToCopy);
                }

                dataPtr = (char*)((IntPtr)dataPtr + bytesToCopy);
                *dataPtr = '\0';
                dataPtr++;
            }

            *dataPtr = '\0';
            dataPtr++;

            Kernel32.GlobalUnlock(newHandle);
            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Save string to handle. If unicode is set to true
        ///  then the string is saved as Unicode, else it is saves as DBCS.
        /// </summary>
        private unsafe HRESULT SaveStringToHandle(IntPtr handle, string str, bool unicode)
        {
            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }

            IntPtr newHandle = IntPtr.Zero;
            if (unicode)
            {
                uint byteSize = (uint)str.Length * 2 + 2;
                newHandle = Kernel32.GlobalReAlloc(
                    handle,
                    byteSize,
                    Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
                if (newHandle == IntPtr.Zero)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                char* ptr = (char*)Kernel32.GlobalLock(newHandle);
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
                    int pinvokeSize = Kernel32.WideCharToMultiByte(Kernel32.CP.ACP, 0, pStr, str.Length, null, 0, IntPtr.Zero, null);
                    newHandle = Kernel32.GlobalReAlloc(
                        handle,
                        (uint)pinvokeSize + 1,
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
                    if (newHandle == IntPtr.Zero)
                    {
                        return HRESULT.E_OUTOFMEMORY;
                    }

                    byte* ptr = (byte*)Kernel32.GlobalLock(newHandle);
                    if (ptr is null)
                    {
                        return HRESULT.E_OUTOFMEMORY;
                    }

                    Kernel32.WideCharToMultiByte(Kernel32.CP.ACP, 0, pStr, str.Length, ptr, pinvokeSize, IntPtr.Zero, null);
                    ptr[pinvokeSize] = 0; // Null terminator
                }
            }

            Kernel32.GlobalUnlock(newHandle);
            return HRESULT.S_OK;
        }

        private unsafe HRESULT SaveHtmlToHandle(IntPtr handle, string str)
        {
            if (handle == IntPtr.Zero)
            {
                return HRESULT.E_INVALIDARG;
            }

            int byteLength = Encoding.UTF8.GetByteCount(str);
            IntPtr newHandle = Kernel32.GlobalReAlloc(
                handle,
                (uint)byteLength + 1,
                Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
            if (newHandle == IntPtr.Zero)
            {
                return HRESULT.E_OUTOFMEMORY;
            }

            byte* ptr = (byte*)Kernel32.GlobalLock(newHandle);
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
                Kernel32.GlobalUnlock(newHandle);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Stores the specified data and its associated format in
        ///  this instance, using the automatic conversion parameter
        ///  to specify whether the
        ///  data can be converted to another format.
        /// </summary>
        public virtual void SetData(string format, bool autoConvert, object data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Set data: " + format + ", " + autoConvert.ToString() + ", " + data?.ToString() ?? "(null)");
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            innerData.SetData(format, autoConvert, data);
        }

        /// <summary>
        ///  Stores the specified data and its associated format in this
        ///  instance.
        /// </summary>
        public virtual void SetData(string format, object data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Set data: " + format + ", " + data?.ToString() ?? "(null)");
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            innerData.SetData(format, data);
        }

        /// <summary>
        ///  Stores the specified data and
        ///  its
        ///  associated class type in this instance.
        /// </summary>
        public virtual void SetData(Type format, object data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Set data: " + format?.FullName ?? "(null)" + ", " + data?.ToString() ?? "(null)");
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            innerData.SetData(format, data);
        }

        /// <summary>
        ///  Stores the specified data in
        ///  this instance, using the class of the data for the format.
        /// </summary>
        public virtual void SetData(object data)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "Set data: " + data?.ToString() ?? "(null)");
            Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
            innerData.SetData(data);
        }
    }
}
