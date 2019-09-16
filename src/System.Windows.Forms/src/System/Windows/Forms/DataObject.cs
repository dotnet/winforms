// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a basic data transfer mechanism.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public class DataObject : IDataObject, IComDataObject
    {
        private static readonly string CF_DEPRECATED_FILENAME = "FileName";
        private static readonly string CF_DEPRECATED_FILENAMEW = "FileNameW";

        private const int DV_E_FORMATETC = unchecked((int)0x80040064);
        private const int DV_E_LINDEX = unchecked((int)0x80040068);
        private const int DV_E_TYMED = unchecked((int)0x80040069);
        private const int DV_E_DVASPECT = unchecked((int)0x8004006B);
        private const int OLE_E_NOTRUNNING = unchecked((int)0x80040005);
        private const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
        private const int DATA_S_SAMEFORMATETC = 0x00040130;

        private static readonly TYMED[] ALLOWED_TYMEDS =
        new TYMED[] {
            TYMED.TYMED_HGLOBAL,
            TYMED.TYMED_ISTREAM,
            TYMED.TYMED_ENHMF,
            TYMED.TYMED_MFPICT,
            TYMED.TYMED_GDI};

        private readonly IDataObject innerData = null;
        internal bool RestrictedFormats { get; set; }

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

        private IntPtr GetCompatibleBitmap(Bitmap bm)
        {
            using ScreenDC hDC = ScreenDC.Create();

            // GDI+ returns a DIBSECTION based HBITMAP. The clipboard deals well
            // only with bitmaps created using CreateCompatibleBitmap(). So, we
            // convert the DIBSECTION into a compatible bitmap.
            IntPtr hBitmap = bm.GetHbitmap();

            // Create a compatible DC to render the source bitmap.
            IntPtr dcSrc = Gdi32.CreateCompatibleDC(hDC);
            IntPtr srcOld = Gdi32.SelectObject(dcSrc, hBitmap);

            // Create a compatible DC and a new compatible bitmap.
            IntPtr dcDest = Gdi32.CreateCompatibleDC(hDC);
            IntPtr hBitmapNew = SafeNativeMethods.CreateCompatibleBitmap(new HandleRef(null, hDC), bm.Size.Width, bm.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            IntPtr destOld = Gdi32.SelectObject(dcDest, hBitmapNew);
            SafeNativeMethods.BitBlt(new HandleRef(null, dcDest), 0, 0, bm.Size.Width, bm.Size.Height, new HandleRef(null, dcSrc), 0, 0, 0x00CC0020);

            // Clear the source and destination compatible DCs.
            Gdi32.SelectObject(dcSrc, srcOld);
            Gdi32.SelectObject(dcDest, destOld);

            Gdi32.DeleteDC(dcSrc);
            Gdi32.DeleteDC(dcDest);

            return hBitmapNew;
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
            if (format == null)
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
            if (format == null)
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
            if (audioBytes == null)
            {
                throw new ArgumentNullException(nameof(audioBytes));
            }
            SetAudio(new MemoryStream(audioBytes));
        }

        public virtual void SetAudio(Stream audioStream)
        {
            if (audioStream == null)
            {
                throw new ArgumentNullException(nameof(audioStream));
            }
            SetData(DataFormats.WaveAudio, false, audioStream);
        }

        public virtual void SetFileDropList(StringCollection filePaths)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }
            string[] strings = new string[filePaths.Count];
            filePaths.CopyTo(strings, 0);
            SetData(DataFormats.FileDrop, true, strings);
        }

        public virtual void SetImage(Image image)
        {
            if (image == null)
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
        ///  Retrieves a list of distinct strings from the array.
        /// </summary>
        private static string[] GetDistinctStrings(string[] formats)
        {
            ArrayList distinct = new ArrayList();
            for (int i = 0; i < formats.Length; i++)
            {
                string s = formats[i];
                if (!distinct.Contains(s))
                {
                    distinct.Add(s);
                }
            }

            string[] temp = new string[distinct.Count];
            distinct.CopyTo(temp, 0);
            return temp;
        }

        /// <summary>
        ///  Returns all the "synonyms" for the specified format.
        /// </summary>
        private static string[] GetMappedFormats(string format)
        {
            if (format == null)
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
                        int hr = SaveDataToHandle(data, format, ref medium);
                        if (NativeMethods.Failed(hr))
                        {
                            Marshal.ThrowExceptionForHR(hr);
                        }
                    }
                    else if ((formatetc.tymed & TYMED.TYMED_GDI) != 0)
                    {
                        if (format.Equals(DataFormats.Bitmap) && data is Bitmap bm
                            && bm != null)
                        {
                            // save bitmap
                            medium.unionmember = GetCompatibleBitmap(bm);
                        }
                    }
                    else
                    {
                        Marshal.ThrowExceptionForHR(DV_E_TYMED);
                    }
                }
                else
                {
                    Marshal.ThrowExceptionForHR(DV_E_FORMATETC);
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR(DV_E_TYMED);
            }
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
        int IComDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DAdvise");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);
            }
            pdwConnection = 0;
            return (NativeMethods.E_NOTIMPL);
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
        void IComDataObject.DUnadvise(int dwConnection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DUnadvise");
            if (innerData is OleConverter)
            {
                ((OleConverter)innerData).OleDataObject.DUnadvise(dwConnection);
                return;
            }
            Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
        int IComDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "EnumDAdvise");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.EnumDAdvise(out enumAdvise);
            }
            enumAdvise = null;
            return (OLE_E_ADVISENOTSUPPORTED);
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
        IEnumFORMATETC IComDataObject.EnumFormatEtc(DATADIR dwDirection)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "EnumFormatEtc: " + dwDirection.ToString());
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.EnumFormatEtc(dwDirection);
            }
            if (dwDirection == DATADIR.DATADIR_GET)
            {
                return new FormatEnumerator(this);
            }
            else
            {
                throw new ExternalException(SR.ExternalException, NativeMethods.E_NOTIMPL);
            }
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
        int IComDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "GetCanonicalFormatEtc");
            if (innerData is OleConverter)
            {
                return ((OleConverter)innerData).OleDataObject.GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);
            }
            pformatetcOut = new FORMATETC();
            return (DATA_S_SAMEFORMATETC);
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
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
                    medium.unionmember = UnsafeNativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE
                                                             | NativeMethods.GMEM_DDESHARE
                                                             | NativeMethods.GMEM_ZEROINIT,
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
                        UnsafeNativeMethods.GlobalFree(new HandleRef(medium, medium.unionmember));
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
                Marshal.ThrowExceptionForHR(DV_E_TYMED);
            }
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
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

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
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
                        return NativeMethods.S_FALSE;
                    }
                    else
                    {
                        if (!GetDataPresent(DataFormats.GetFormat(formatetc.cfFormat).Name))
                        {
                            return (DV_E_FORMATETC);
                        }
                    }
                }
                else
                {
                    return (DV_E_TYMED);
                }
            }
            else
            {
                return (DV_E_DVASPECT);
            }
#if DEBUG
            int format = unchecked((ushort)formatetc.cfFormat);
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "QueryGetData::cfFormat " + format.ToString(CultureInfo.InvariantCulture) + " found");
#endif
            return NativeMethods.S_OK;
        }

        // <summary>
        //     Part of IComDataObject, used to interop with OLE.
        // </summary>
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

        private int SaveDataToHandle(object data, string format, ref STGMEDIUM medium)
        {
            int hr = NativeMethods.E_FAIL;
            if (data is Stream)
            {
                hr = SaveStreamToHandle(ref medium.unionmember, (Stream)data);
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
            else if (format.Equals(DataFormats.Dib)
                     && data is Image)
            {
                // GDI+ does not properly handle saving to DIB images.  Since the
                // clipboard will take an HBITMAP and publish a Dib, we don't need
                // to support this.
                //
                hr = DV_E_TYMED;
            }
            else if (format.Equals(DataFormats.Serializable)
                     || data is ISerializable
                     || (data != null && data.GetType().IsSerializable))
            {
                hr = SaveObjectToHandle(ref medium.unionmember, data, DataObject.RestrictDeserializationToSafeTypes(format));
            }
            return hr;
        }

        private int SaveObjectToHandle(ref IntPtr handle, object data, bool restrictSerialization)
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

            formatter.Serialize(stream, data);
        }

        /// <summary>
        ///  Saves stream out to handle.
        /// </summary>
        private unsafe int SaveStreamToHandle(ref IntPtr handle, Stream stream)
        {
            if (handle != IntPtr.Zero)
            {
                UnsafeNativeMethods.GlobalFree(new HandleRef(null, handle));
            }
            int size = (int)stream.Length;
            handle = UnsafeNativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE | NativeMethods.GMEM_DDESHARE, size);
            if (handle == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, handle));
            if (ptr == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            try
            {
                var span = new Span<byte>(ptr.ToPointer(), size);
                stream.Position = 0;
                stream.Read(span);
            }
            finally
            {
                UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, handle));
            }
            return NativeMethods.S_OK;
        }

        /// <summary>
        ///  Saves a list of files out to the handle in HDROP format.
        /// </summary>
        private int SaveFileListToHandle(IntPtr handle, string[] files)
        {
            if (files == null)
            {
                return NativeMethods.S_OK;
            }
            else if (files.Length < 1)
            {
                return NativeMethods.S_OK;
            }
            if (handle == IntPtr.Zero)
            {
                return (NativeMethods.E_INVALIDARG);
            }

            IntPtr currentPtr = IntPtr.Zero;
            int baseStructSize = 4 + 8 + 4 + 4;
            int sizeInBytes = baseStructSize;

            // First determine the size of the array
            for (int i = 0; i < files.Length; i++)
            {
                sizeInBytes += (files[i].Length + 1) * 2;
            }
            sizeInBytes += 2;

            // Alloc the Win32 memory
            //
            IntPtr newHandle = UnsafeNativeMethods.GlobalReAlloc(new HandleRef(null, handle),
                                                  sizeInBytes,
                                                  NativeMethods.GMEM_MOVEABLE | NativeMethods.GMEM_DDESHARE);
            if (newHandle == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            IntPtr basePtr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, newHandle));
            if (basePtr == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            currentPtr = basePtr;

            // Write out the struct...
            //
            int[] structData = new int[] { baseStructSize, 0, 0, 0, 0 };

            structData[4] = unchecked((int)0xFFFFFFFF);
            Marshal.Copy(structData, 0, currentPtr, structData.Length);
            currentPtr = (IntPtr)((long)currentPtr + baseStructSize);

            // Write out the strings.
            for (int i = 0; i < files.Length; i++)
            {
                UnsafeNativeMethods.CopyMemoryW(currentPtr, files[i], files[i].Length * 2);
                currentPtr = (IntPtr)((long)currentPtr + (files[i].Length * 2));
                Marshal.Copy(new byte[] { 0, 0 }, 0, currentPtr, 2);
                currentPtr = (IntPtr)((long)currentPtr + 2);
            }

            Marshal.Copy(new char[] { '\0' }, 0, currentPtr, 1);
            currentPtr = (IntPtr)((long)currentPtr + 2);

            UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, newHandle));
            return NativeMethods.S_OK;
        }

        /// <summary>
        ///  Save string to handle. If unicode is set to true
        ///  then the string is saved as Unicode, else it is saves as DBCS.
        /// </summary>
        private int SaveStringToHandle(IntPtr handle, string str, bool unicode)
        {
            if (handle == IntPtr.Zero)
            {
                return (NativeMethods.E_INVALIDARG);
            }
            IntPtr newHandle = IntPtr.Zero;
            if (unicode)
            {
                int byteSize = (str.Length * 2 + 2);
                newHandle = UnsafeNativeMethods.GlobalReAlloc(new HandleRef(null, handle),
                                                  byteSize,
                                                  NativeMethods.GMEM_MOVEABLE
                                                  | NativeMethods.GMEM_DDESHARE
                                                  | NativeMethods.GMEM_ZEROINIT);
                if (newHandle == IntPtr.Zero)
                {
                    return (NativeMethods.E_OUTOFMEMORY);
                }
                IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, newHandle));
                if (ptr == IntPtr.Zero)
                {
                    return (NativeMethods.E_OUTOFMEMORY);
                }

                char[] chars = str.ToCharArray(0, str.Length);
                UnsafeNativeMethods.CopyMemoryW(ptr, chars, chars.Length * 2);
            }
            else
            {

                int pinvokeSize = UnsafeNativeMethods.WideCharToMultiByte(0 /*CP_ACP*/, 0, str, str.Length, null, 0, IntPtr.Zero, IntPtr.Zero);

                byte[] strBytes = new byte[pinvokeSize];
                UnsafeNativeMethods.WideCharToMultiByte(0 /*CP_ACP*/, 0, str, str.Length, strBytes, strBytes.Length, IntPtr.Zero, IntPtr.Zero);

                newHandle = UnsafeNativeMethods.GlobalReAlloc(new HandleRef(null, handle),
                                                  pinvokeSize + 1,
                                                  NativeMethods.GMEM_MOVEABLE
                                                  | NativeMethods.GMEM_DDESHARE
                                                  | NativeMethods.GMEM_ZEROINIT);
                if (newHandle == IntPtr.Zero)
                {
                    return (NativeMethods.E_OUTOFMEMORY);
                }
                IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, newHandle));
                if (ptr == IntPtr.Zero)
                {
                    return (NativeMethods.E_OUTOFMEMORY);
                }
                UnsafeNativeMethods.CopyMemory(ptr, strBytes, pinvokeSize);
                Marshal.Copy(new byte[] { 0 }, 0, (IntPtr)((long)ptr + pinvokeSize), 1);
            }

            if (newHandle != IntPtr.Zero)
            {
                UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, newHandle));
            }
            return NativeMethods.S_OK;
        }

        private int SaveHtmlToHandle(IntPtr handle, string str)
        {
            if (handle == IntPtr.Zero)
            {
                return (NativeMethods.E_INVALIDARG);
            }
            IntPtr newHandle = IntPtr.Zero;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(str);

            newHandle = UnsafeNativeMethods.GlobalReAlloc(new HandleRef(null, handle),
                                                    bytes.Length + 1,
                                                    NativeMethods.GMEM_MOVEABLE
                                                    | NativeMethods.GMEM_DDESHARE
                                                    | NativeMethods.GMEM_ZEROINIT);
            if (newHandle == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, newHandle));
            if (ptr == IntPtr.Zero)
            {
                return (NativeMethods.E_OUTOFMEMORY);
            }
            try
            {
                UnsafeNativeMethods.CopyMemory(ptr, bytes, bytes.Length);
                Marshal.Copy(new byte[] { 0 }, 0, (IntPtr)((long)ptr + bytes.Length), 1);
            }
            finally
            {
                UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, newHandle));
            }
            return NativeMethods.S_OK;
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

        /// <summary>
        ///  Part of IComDataObject, used to interop with OLE.
        /// </summary>
        private class FormatEnumerator : IEnumFORMATETC
        {
            internal IDataObject parent = null;
            internal ArrayList formats = new ArrayList();
            internal int current = 0;

            public FormatEnumerator(IDataObject parent) : this(parent, parent.GetFormats())
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString());
            }

            public FormatEnumerator(IDataObject parent, FORMATETC[] formats)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString() + ", FORMATETC[]" + formats.Length.ToString(CultureInfo.InvariantCulture));
                this.formats.Clear();
                this.parent = parent;
                current = 0;
                if (formats != null)
                {
                    if (parent is DataObject dataObject && dataObject.RestrictedFormats)
                    {
                        if (!Clipboard.IsFormatValid(formats))
                        {
                            throw new Security.SecurityException(SR.ClipboardSecurityException);
                        }
                    }

                    for (int i = 0; i < formats.Length; i++)
                    {
                        FORMATETC currentFormat = formats[i];

                        FORMATETC temp = new FORMATETC
                        {
                            cfFormat = currentFormat.cfFormat,
                            dwAspect = currentFormat.dwAspect,
                            ptd = currentFormat.ptd,
                            lindex = currentFormat.lindex,
                            tymed = currentFormat.tymed
                        };
                        this.formats.Add(temp);
                    }
                }
            }

            public FormatEnumerator(IDataObject parent, string[] formats)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Constructed: " + parent.ToString() + ", string[]" + formats.Length.ToString(CultureInfo.InvariantCulture));

                this.parent = parent;
                this.formats.Clear();

                if (formats != null)
                {
                    if (parent is DataObject dataObject && dataObject.RestrictedFormats)
                    {
                        if (!Clipboard.IsFormatValid(formats))
                        {
                            throw new Security.SecurityException(SR.ClipboardSecurityException);
                        }
                    }

                    for (int i = 0; i < formats.Length; i++)
                    {
                        string format = formats[i];
                        FORMATETC temp = new FORMATETC
                        {
                            cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id)),
                            dwAspect = DVASPECT.DVASPECT_CONTENT,
                            ptd = IntPtr.Zero,
                            lindex = -1
                        };

                        if (format.Equals(DataFormats.Bitmap))
                        {
                            temp.tymed = TYMED.TYMED_GDI;
                        }
                        else if (format.Equals(DataFormats.EnhancedMetafile))
                        {
                            temp.tymed = TYMED.TYMED_ENHMF;
                        }
                        else if (format.Equals(DataFormats.Text)
                                 || format.Equals(DataFormats.UnicodeText)
                                 || format.Equals(DataFormats.StringFormat)
                                 || format.Equals(DataFormats.Rtf)
                                 || format.Equals(DataFormats.CommaSeparatedValue)
                                 || format.Equals(DataFormats.FileDrop)
                                 || format.Equals(CF_DEPRECATED_FILENAME)
                                 || format.Equals(CF_DEPRECATED_FILENAMEW))
                        {

                            temp.tymed = TYMED.TYMED_HGLOBAL;
                        }
                        else
                        {
                            temp.tymed = TYMED.TYMED_HGLOBAL;
                        }

                        if (temp.tymed != 0)
                        {
                            this.formats.Add(temp);
                        }
                    }
                }
            }

            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Next");
                if (this.current < formats.Count && celt > 0)
                {

                    FORMATETC current = (FORMATETC)formats[this.current];
                    rgelt[0].cfFormat = current.cfFormat;
                    rgelt[0].tymed = current.tymed;
                    rgelt[0].dwAspect = DVASPECT.DVASPECT_CONTENT;
                    rgelt[0].ptd = IntPtr.Zero;
                    rgelt[0].lindex = -1;

                    if (pceltFetched != null)
                    {
                        pceltFetched[0] = 1;
                    }
                    this.current++;
                }
                else
                {
                    if (pceltFetched != null)
                    {
                        pceltFetched[0] = 0;
                    }
                    return NativeMethods.S_FALSE;
                }
                return NativeMethods.S_OK;
            }

            public int Skip(int celt)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Skip");
                if (current + celt >= formats.Count)
                {
                    return NativeMethods.S_FALSE;
                }
                current += celt;
                return NativeMethods.S_OK;
            }

            public int Reset()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Reset");
                current = 0;
                return NativeMethods.S_OK;
            }

            public void Clone(out IEnumFORMATETC ppenum)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "FormatEnumerator: Clone");
                FORMATETC[] temp = new FORMATETC[formats.Count];
                formats.CopyTo(temp, 0);
                ppenum = new FormatEnumerator(parent, temp);
            }
        }

        /// <summary>
        ///  OLE Converter.  This class embodies the nastiness required to convert from our
        ///  managed types to standard OLE clipboard formats.
        /// </summary>
        private class OleConverter : IDataObject
        {
            internal IComDataObject innerData;

            public OleConverter(IComDataObject data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "OleConverter: Constructed OleConverter");
                innerData = data;
            }

            /// <summary>
            ///  Returns the data Object we are wrapping
            /// </summary>
            public IComDataObject OleDataObject
            {
                get
                {
                    return innerData;
                }
            }

            /// <summary>
            ///  Uses IStream and retrieves the specified format from the bound IComDataObject.
            /// </summary>
            private unsafe object GetDataFromOleIStream(string format)
            {

                FORMATETC formatetc = new FORMATETC();
                STGMEDIUM medium = new STGMEDIUM();

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = TYMED.TYMED_ISTREAM;

                medium.tymed = TYMED.TYMED_ISTREAM;

                // Limit the # of exceptions we may throw below.
                if (NativeMethods.S_OK != QueryGetDataUnsafe(ref formatetc))
                {
                    return null;
                }

                try
                {
                    innerData.GetData(ref formatetc, out medium);
                }
                catch
                {
                    return null;
                }

                if (medium.unionmember != IntPtr.Zero)
                {
                    Ole32.IStream pStream = (Ole32.IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                    Marshal.Release(medium.unionmember);
                    pStream.Stat(out Ole32.STATSTG sstg, Ole32.STATFLAG.STATFLAG_DEFAULT);

                    IntPtr hglobal = UnsafeNativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE
                                                      | NativeMethods.GMEM_DDESHARE
                                                      | NativeMethods.GMEM_ZEROINIT,
                                                      (int)sstg.cbSize);
                    IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(innerData, hglobal));
                    pStream.Read((byte*)ptr, (uint)sstg.cbSize, null);
                    UnsafeNativeMethods.GlobalUnlock(new HandleRef(innerData, hglobal));

                    return GetDataFromHGLOBAL(format, hglobal);
                }

                return null;
            }

            /// <summary>
            ///  Retrieves the specified form from the specified hglobal.
            /// </summary>
            private object GetDataFromHGLOBAL(string format, IntPtr hglobal)
            {
                object data = null;

                if (hglobal != IntPtr.Zero)
                {
                    //=----------------------------------------------------------------=
                    // Convert from OLE to IW objects
                    //=----------------------------------------------------------------=
                    // Add any new formats here...

                    if (format.Equals(DataFormats.Text)
                        || format.Equals(DataFormats.Rtf)
                        || format.Equals(DataFormats.OemText))
                    {
                        data = ReadStringFromHandle(hglobal, false);
                    }
                    else if (format.Equals(DataFormats.Html))
                    {
                        data = ReadHtmlFromHandle(hglobal);
                    }

                    else if (format.Equals(DataFormats.UnicodeText))
                    {
                        data = ReadStringFromHandle(hglobal, true);
                    }
                    else if (format.Equals(DataFormats.FileDrop))
                    {
                        data = ReadFileListFromHandle(hglobal);
                    }
                    else if (format.Equals(CF_DEPRECATED_FILENAME))
                    {
                        data = new string[] { ReadStringFromHandle(hglobal, false) };
                    }
                    else if (format.Equals(CF_DEPRECATED_FILENAMEW))
                    {
                        data = new string[] { ReadStringFromHandle(hglobal, true) };
                    }
                    else
                    {
                        data = ReadObjectFromHandle(hglobal, DataObject.RestrictDeserializationToSafeTypes(format));
                    }

                    UnsafeNativeMethods.GlobalFree(new HandleRef(null, hglobal));
                }

                return data;
            }

            /// <summary>
            ///  Uses HGLOBALs and retrieves the specified format from the bound IComDatabject.
            /// </summary>
            private object GetDataFromOleHGLOBAL(string format, out bool done)
            {
                done = false;
                Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");

                FORMATETC formatetc = new FORMATETC();
                STGMEDIUM medium = new STGMEDIUM();

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = TYMED.TYMED_HGLOBAL;

                medium.tymed = TYMED.TYMED_HGLOBAL;

                object data = null;

                if (NativeMethods.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        innerData.GetData(ref formatetc, out medium);

                        if (medium.unionmember != IntPtr.Zero)
                        {
                            data = GetDataFromHGLOBAL(format, medium.unionmember);
                        }
                    }
                    catch (RestrictedTypeDeserializationException)
                    {
                        done = true;
                    }
                    catch
                    {
                    }
                }
                return data;
            }

            /// <summary>
            ///  Retrieves the specified format data from the bound IComDataObject, from
            ///  other sources that IStream and HGLOBAL... this is really just a place
            ///  to put the "special" formats like BITMAP, ENHMF, etc.
            /// </summary>
            private object GetDataFromOleOther(string format)
            {
                Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");

                FORMATETC formatetc = new FORMATETC();
                STGMEDIUM medium = new STGMEDIUM();

                TYMED tymed = (TYMED)0;

                if (format.Equals(DataFormats.Bitmap))
                {
                    tymed = TYMED.TYMED_GDI;
                }
                else if (format.Equals(DataFormats.EnhancedMetafile))
                {
                    tymed = TYMED.TYMED_ENHMF;
                }

                if (tymed == (TYMED)0)
                {
                    return null;
                }

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = tymed;
                medium.tymed = tymed;

                object data = null;
                if (NativeMethods.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        innerData.GetData(ref formatetc, out medium);
                    }
                    catch
                    {
                    }
                }

                if (medium.unionmember != IntPtr.Zero)
                {
                    if (format.Equals(DataFormats.Bitmap))
                    {
                        // as/urt 140870 -- GDI+ doesn't own this HBITMAP, but we can't
                        // delete it while the object is still around.  So we have to do the really expensive
                        // thing of cloning the image so we can release the HBITMAP.

                        // This bitmap is created by the com object which originally copied the bitmap to tbe
                        // clipboard. We call Add here, since DeleteObject calls Remove.
                        Image clipboardImage = Image.FromHbitmap(medium.unionmember);
                        if (clipboardImage != null)
                        {
                            Image firstImage = clipboardImage;
                            clipboardImage = (Image)clipboardImage.Clone();
                            Gdi32.DeleteObject(medium.unionmember);
                            firstImage.Dispose();
                        }
                        data = clipboardImage;
                    }
                }

                return data;
            }

            /// <summary>
            ///  Extracts a managed Object from the innerData of the specified
            ///  format. This is the base of the OLE to managed conversion.
            /// </summary>
            private object GetDataFromBoundOleDataObject(string format, out bool done)
            {
                object data = null;
                done = false;
                try
                {
                    data = GetDataFromOleOther(format);
                    if (data == null)
                    {
                        data = GetDataFromOleHGLOBAL(format, out done);
                    }
                    if (data == null && !done)
                    {
                        data = GetDataFromOleIStream(format);
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail(e.ToString());
                }
                return data;
            }

            /// <summary>
            ///  Creates an Stream from the data stored in handle.
            /// </summary>
            private Stream ReadByteStreamFromHandle(IntPtr handle, out bool isSerializedObject)
            {
                IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, handle));
                if (ptr == IntPtr.Zero)
                {
                    throw new ExternalException(SR.ExternalException, NativeMethods.E_OUTOFMEMORY);
                }
                try
                {
                    int size = UnsafeNativeMethods.GlobalSize(new HandleRef(null, handle));
                    byte[] bytes = new byte[size];
                    Marshal.Copy(ptr, bytes, 0, size);
                    int index = 0;

                    // The object here can either be a stream or a serialized
                    // object.  We identify a serialized object by writing the
                    // bytes for the guid serializedObjectID at the front
                    // of the stream.  Check for that here.
                    //
                    if (size > serializedObjectID.Length)
                    {
                        isSerializedObject = true;
                        for (int i = 0; i < serializedObjectID.Length; i++)
                        {
                            if (serializedObjectID[i] != bytes[i])
                            {
                                isSerializedObject = false;
                                break;
                            }
                        }

                        // Advance the byte pointer.
                        //
                        if (isSerializedObject)
                        {
                            index = serializedObjectID.Length;
                        }
                    }
                    else
                    {
                        isSerializedObject = false;
                    }

                    return new MemoryStream(bytes, index, bytes.Length - index);
                }
                finally
                {
                    UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, handle));
                }
            }

            /// <summary>
            ///  Creates a new instance of the Object that has been persisted into the
            ///  handle.
            /// </summary>
            private object ReadObjectFromHandle(IntPtr handle, bool restrictDeserialization)
            {
                object value = null;

                Stream stream = ReadByteStreamFromHandle(handle, out bool isSerializedObject);

                if (isSerializedObject)
                {
                    value = ReadObjectFromHandleDeserializer(stream, restrictDeserialization);
                }
                else
                {
                    value = stream;
                }

                return value;
            }

            private static object ReadObjectFromHandleDeserializer(Stream stream, bool restrictDeserialization)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (restrictDeserialization)
                {
                    formatter.Binder = new BitmapBinder();
                }
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                return formatter.Deserialize(stream);
            }

            /// <summary>
            ///  Parses the HDROP format and returns a list of strings using
            ///  the DragQueryFile function.
            /// </summary>
            private string[] ReadFileListFromHandle(IntPtr hdrop)
            {

                string[] files = null;
                StringBuilder sb = new StringBuilder(Kernel32.MAX_PATH);

                int count = UnsafeNativeMethods.DragQueryFile(new HandleRef(null, hdrop), unchecked((int)0xFFFFFFFF), null, 0);
                if (count > 0)
                {
                    files = new string[count];

                    for (int i = 0; i < count; i++)
                    {
                        int charlen = UnsafeNativeMethods.DragQueryFileLongPath(new HandleRef(null, hdrop), i, sb);
                        if (0 == charlen)
                        {
                            continue;
                        }

                        string s = sb.ToString(0, charlen);
                        string fullPath = Path.GetFullPath(s);
                        files[i] = s;
                    }
                }

                return files;
            }

            /// <summary>
            ///  Creates a string from the data stored in handle. If
            ///  unicode is set to true, then the string is assume to be Unicode,
            ///  else DBCS (ASCI) is assumed.
            /// </summary>
            private unsafe string ReadStringFromHandle(IntPtr handle, bool unicode)
            {
                string stringData = null;

                IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, handle));
                try
                {
                    if (unicode)
                    {
                        stringData = new string((char*)ptr);
                    }
                    else
                    {
                        stringData = new string((sbyte*)ptr);
                    }
                }
                finally
                {
                    UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, handle));
                }

                return stringData;
            }

            private unsafe string ReadHtmlFromHandle(IntPtr handle)
            {
                string stringData = null;
                IntPtr ptr = UnsafeNativeMethods.GlobalLock(new HandleRef(null, handle));
                try
                {
                    int size = UnsafeNativeMethods.GlobalSize(new HandleRef(null, handle));
                    stringData = Encoding.UTF8.GetString((byte*)ptr, size);
                }
                finally
                {
                    UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, handle));
                }

                return stringData;
            }

            //=------------------------------------------------------------------------=
            // IDataObject
            //=------------------------------------------------------------------------=
            public virtual object GetData(string format, bool autoConvert)
            {
                object baseVar = GetDataFromBoundOleDataObject(format, out bool done);
                object original = baseVar;

                if (!done && autoConvert && (baseVar == null || baseVar is MemoryStream))
                {
                    string[] mappedFormats = GetMappedFormats(format);
                    if (mappedFormats != null)
                    {
                        for (int i = 0; ((!done) && (i < mappedFormats.Length)); i++)
                        {
                            if (!format.Equals(mappedFormats[i]))
                            {
                                baseVar = GetDataFromBoundOleDataObject(mappedFormats[i], out done);
                                if (!done && baseVar != null && !(baseVar is MemoryStream))
                                {
                                    original = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (original != null)
                {
                    return original;
                }
                else
                {
                    return baseVar;
                }
            }

            public virtual object GetData(string format)
            {
                return GetData(format, true);
            }

            public virtual object GetData(Type format)
            {
                return GetData(format.FullName);
            }

            public virtual void SetData(string format, bool autoConvert, object data)
            {
            }

            public virtual void SetData(string format, object data)
            {
                SetData(format, true, data);
            }

            public virtual void SetData(Type format, object data)
            {
                SetData(format.FullName, data);
            }

            public virtual void SetData(object data)
            {
                if (data is ISerializable)
                {
                    SetData(DataFormats.Serializable, data);
                }
                else
                {
                    SetData(data.GetType(), data);
                }
            }

            private int QueryGetDataUnsafe(ref FORMATETC formatetc)
            {
                return innerData.QueryGetData(ref formatetc);
            }

            private int QueryGetDataInner(ref FORMATETC formatetc)
            {
                return innerData.QueryGetData(ref formatetc);
            }

            public virtual bool GetDataPresent(Type format)
            {
                return GetDataPresent(format.FullName);
            }

            private bool GetDataPresentInner(string format)
            {
                Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");
                FORMATETC formatetc = new FORMATETC
                {
                    cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id)),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1
                };

                for (int i = 0; i < ALLOWED_TYMEDS.Length; i++)
                {
                    formatetc.tymed |= ALLOWED_TYMEDS[i];
                }

                int hr = QueryGetDataUnsafe(ref formatetc);
                return (hr == NativeMethods.S_OK);
            }

            public virtual bool GetDataPresent(string format, bool autoConvert)
            {
                bool baseVar = GetDataPresentInner(format);

                if (!baseVar && autoConvert)
                {
                    string[] mappedFormats = GetMappedFormats(format);
                    if (mappedFormats != null)
                    {
                        for (int i = 0; i < mappedFormats.Length; i++)
                        {
                            if (!format.Equals(mappedFormats[i]))
                            {
                                baseVar = GetDataPresentInner(mappedFormats[i]);
                                if (baseVar)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return baseVar;
            }

            public virtual bool GetDataPresent(string format)
            {
                return GetDataPresent(format, true);
            }

            public virtual string[] GetFormats(bool autoConvert)
            {
                Debug.Assert(innerData != null, "You must have an innerData on all DataObjects");

                IEnumFORMATETC enumFORMATETC = null;
                ArrayList formats = new ArrayList();
                try
                {
                    enumFORMATETC = innerData.EnumFormatEtc(DATADIR.DATADIR_GET);
                }
                catch
                {
                }

                if (enumFORMATETC != null)
                {
                    enumFORMATETC.Reset();

                    FORMATETC[] formatetc = new FORMATETC[] { new FORMATETC() };
                    int[] retrieved = new int[] { 1 };

                    while (retrieved[0] > 0)
                    {
                        retrieved[0] = 0;
                        try
                        {
                            enumFORMATETC.Next(1, formatetc, retrieved);
                        }
                        catch
                        {
                        }

                        if (retrieved[0] > 0)
                        {
                            string name = DataFormats.GetFormat(formatetc[0].cfFormat).Name;
                            if (autoConvert)
                            {
                                string[] mappedFormats = GetMappedFormats(name);
                                for (int i = 0; i < mappedFormats.Length; i++)
                                {
                                    formats.Add(mappedFormats[i]);
                                }
                            }
                            else
                            {
                                formats.Add(name);
                            }
                        }
                    }
                }

                string[] temp = new string[formats.Count];
                formats.CopyTo(temp, 0);
                return GetDistinctStrings(temp);
            }

            public virtual string[] GetFormats()
            {
                return GetFormats(true);
            }

        }

        //--------------------------------------------------------------------------
        // Data Store
        //--------------------------------------------------------------------------

        private class DataStore : IDataObject
        {
            private class DataStoreEntry
            {
                public object data;
                public bool autoConvert;

                public DataStoreEntry(object data, bool autoConvert)
                {
                    this.data = data;
                    this.autoConvert = autoConvert;
                }
            }

            private readonly Hashtable data = new Hashtable(BackCompatibleStringComparer.Default);

            public DataStore()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: Constructed DataStore");
            }

            public virtual object GetData(string format, bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format + ", " + autoConvert.ToString());
                if (string.IsNullOrWhiteSpace(format))
                {
                    return null;
                }

                DataStoreEntry dse = (DataStoreEntry)data[format];
                object baseVar = null;
                if (dse != null)
                {
                    baseVar = dse.data;
                }
                object original = baseVar;

                if (autoConvert
                    && (dse == null || dse.autoConvert)
                    && (baseVar == null || baseVar is MemoryStream))
                {

                    string[] mappedFormats = GetMappedFormats(format);
                    if (mappedFormats != null)
                    {
                        for (int i = 0; i < mappedFormats.Length; i++)
                        {
                            if (!format.Equals(mappedFormats[i]))
                            {
                                DataStoreEntry found = (DataStoreEntry)data[mappedFormats[i]];
                                if (found != null)
                                {
                                    baseVar = found.data;
                                }
                                if (baseVar != null && !(baseVar is MemoryStream))
                                {
                                    original = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (original != null)
                {
                    return original;
                }
                else
                {
                    return baseVar;
                }
            }

            public virtual object GetData(string format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format);
                return GetData(format, true);
            }

            public virtual object GetData(Type format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetData: " + format.FullName);
                return GetData(format.FullName);
            }

            public virtual void SetData(string format, bool autoConvert, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format + ", " + autoConvert.ToString() + ", " + data?.ToString() ?? "(null)");
                if (string.IsNullOrWhiteSpace(format))
                {
                    if (format == null)
                    {
                        throw new ArgumentNullException(nameof(format));
                    }

                    throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
                }

                // We do not have proper support for Dibs, so if the user explicitly asked
                // for Dib and provided a Bitmap object we can't convert.  Instead, publish as an HBITMAP
                // and let the system provide the conversion for us.
                //
                if (data is Bitmap && format.Equals(DataFormats.Dib))
                {
                    if (autoConvert)
                    {
                        format = DataFormats.Bitmap;
                    }
                    else
                    {
                        throw new NotSupportedException(SR.DataObjectDibNotSupported);
                    }
                }

                this.data[format] = new DataStoreEntry(data, autoConvert);
            }
            public virtual void SetData(string format, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format + ", " + data?.ToString() ?? "(null)");
                SetData(format, true, data);
            }

            public virtual void SetData(Type format, object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + format?.FullName ?? "(null)" + ", " + data?.ToString() ?? "(null)");
                if (format == null)
                {
                    throw new ArgumentNullException(nameof(format));
                }

                SetData(format.FullName, data);
            }

            public virtual void SetData(object data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: SetData: " + data?.ToString() ?? "(null)");
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }
    
                if (data is ISerializable
                    && !this.data.ContainsKey(DataFormats.Serializable))
                {

                    SetData(DataFormats.Serializable, data);
                }

                SetData(data.GetType(), data);
            }

            public virtual bool GetDataPresent(Type format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format.FullName);
                return GetDataPresent(format.FullName);
            }

            public virtual bool GetDataPresent(string format, bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format + ", " + autoConvert.ToString());
                if (string.IsNullOrWhiteSpace(format))
                {
                    return false;
                }

                if (!autoConvert)
                {
                    Debug.Assert(data != null, "data must be non-null");
                    return data.ContainsKey(format);
                }
                else
                {
                    string[] formats = GetFormats(autoConvert);
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore:  got " + formats.Length.ToString(CultureInfo.InvariantCulture) + " formats from get formats");
                    Debug.Assert(formats != null, "Null returned from GetFormats");
                    for (int i = 0; i < formats.Length; i++)
                    {
                        Debug.Assert(formats[i] != null, "Null format inside of formats at index " + i.ToString(CultureInfo.InvariantCulture));
                        if (format.Equals(formats[i]))
                        {
                            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: returning true");
                            return true;
                        }
                    }
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: returning false");
                    return false;
                }
            }

            public virtual bool GetDataPresent(string format)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetDataPresent: " + format);
                return GetDataPresent(format, true);
            }

            public virtual string[] GetFormats(bool autoConvert)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetFormats: " + autoConvert.ToString());
                Debug.Assert(data != null, "data collection can't be null");
                Debug.Assert(data.Keys != null, "data Keys collection can't be null");

                string[] baseVar = new string[data.Keys.Count];
                data.Keys.CopyTo(baseVar, 0);
                Debug.Assert(baseVar != null, "Collections should never return NULL arrays!!!");
                if (autoConvert)
                {
                    Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: applying autoConvert");

                    ArrayList formats = new ArrayList();
                    for (int i = 0; i < baseVar.Length; i++)
                    {
                        Debug.Assert(data[baseVar[i]] != null, "Null item in data collection with key '" + baseVar[i] + "'");
                        if (((DataStoreEntry)data[baseVar[i]]).autoConvert)
                        {

                            string[] cur = GetMappedFormats(baseVar[i]);
                            Debug.Assert(cur != null, "GetMappedFormats returned null for '" + baseVar[i] + "'");
                            for (int j = 0; j < cur.Length; j++)
                            {
                                formats.Add(cur[j]);
                            }
                        }
                        else
                        {
                            formats.Add(baseVar[i]);
                        }
                    }

                    string[] temp = new string[formats.Count];
                    formats.CopyTo(temp, 0);
                    baseVar = GetDistinctStrings(temp);
                }
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: returing " + baseVar.Length.ToString(CultureInfo.InvariantCulture) + " formats from GetFormats");
                return baseVar;
            }
            public virtual string[] GetFormats()
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "DataStore: GetFormats");
                return GetFormats(true);
            }
        }

        /// <summary>
        ///  Binder that restricts DataObject content deserialization to Bitmap type and
        ///  serialization to strings and Bitmaps.
        ///  Deserialization of known safe types(strings and arrays of primitives) does not invoke the binder.
        /// </summary>
        private class BitmapBinder : SerializationBinder
        {
            // Bitmap type lives in defferent assemblies in the .NET Framework and in .NET Core.
            // However we allow desktop content to be deserializated in Core and Core content
            // deserialized on desktop. To support this roundtrip,
            // Bitmap type identity is unified to the desktop type during serialization
            // and we use the desktop type name when filtering as well.
            private static readonly string s_allowedTypeName = "System.Drawing.Bitmap";
            private static readonly string s_allowedAssemblyName = "System.Drawing";
            // PublicKeyToken=b03f5f7f11d50a3a
            private static readonly byte[] s_allowedToken = new byte[] { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A };

            /// <summary>
            ///  Only safe to deserialize types are bypassing this callback, Strings
            ///  and arrays of primitive types in particular. We are explicitly allowing
            ///  System.Drawing.Bitmap type to bind using the default binder.
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            /// <returns>null - continue with the default binder.</returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (string.CompareOrdinal(typeName, s_allowedTypeName) == 0)
                {
                    AssemblyName nameToBind = null;
                    try
                    {
                        nameToBind = new AssemblyName(assemblyName);
                    }
                    catch
                    {
                    }
                    if (nameToBind != null)
                    {
                        if (string.CompareOrdinal(nameToBind.Name, s_allowedAssemblyName) == 0)
                        {
                            byte[] tokenToBind = nameToBind.GetPublicKeyToken();
                            if ((tokenToBind != null) &&
                                (s_allowedToken != null) &&
                                (tokenToBind.Length == s_allowedToken.Length))
                            {
                                bool block = false;
                                for (int i = 0; i < s_allowedToken.Length; i++)
                                {
                                    if (s_allowedToken[i] != tokenToBind[i])
                                    {
                                        block = true;
                                        break;
                                    }
                                }
                                if (!block)
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
            }

            /// <summary>
            ///  Bitmap and string types are safe type to serialize/deserialize.
            /// </summary>
            /// <param name="serializedType"></param>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                // null strings will follow the default codepath in BinaryFormatter
                assemblyName = null;
                typeName = null;
                if (serializedType != null && !serializedType.Equals(typeof(string)) && !serializedType.Equals(typeof(Bitmap)))
                {
                    throw new SerializationException(string.Format(SR.UnexpectedTypeForClipboardFormat, serializedType.FullName));
                }
            }
        }

        /// <summary>
        ///  This exception is used to indicate that clipboard contains a serialized
        ///  managed object that contains unexpected types and that we should stop processing this data.
        /// </summary>
        private class RestrictedTypeDeserializationException : Exception
        {
            public RestrictedTypeDeserializationException(string message) : base(message)
            {
            }
        }
    }
}
