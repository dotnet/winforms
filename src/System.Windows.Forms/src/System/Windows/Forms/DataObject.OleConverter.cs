// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
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

                // Limit the # of exceptions we may throw below.
                if ((int)HRESULT.S_OK != QueryGetDataUnsafe(ref formatetc))
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

                Ole32.IStream pStream = null;
                IntPtr hglobal = IntPtr.Zero;
                try
                {
                    if (medium.tymed == TYMED.TYMED_ISTREAM && medium.unionmember != IntPtr.Zero)
                    {
                        pStream = (Ole32.IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                        pStream.Stat(out Ole32.STATSTG sstg, Ole32.STATFLAG.DEFAULT);

                        hglobal = Kernel32.GlobalAlloc(
                            Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                            (uint)sstg.cbSize);
                        // not throwing here because the other out of memory condition on GlobalAlloc
                        // happens inside innerData.GetData and gets turned into a null return value
                        if (hglobal == IntPtr.Zero)
                            return null;
                        IntPtr ptr = Kernel32.GlobalLock(hglobal);
                        pStream.Read((byte*)ptr, (uint)sstg.cbSize, null);
                        Kernel32.GlobalUnlock(hglobal);

                        return GetDataFromHGLOBAL(format, hglobal);
                    }

                    return null;
                }
                finally
                {
                    if (hglobal != IntPtr.Zero)
                        Kernel32.GlobalFree(hglobal);

                    if (pStream != null)
                        Marshal.ReleaseComObject(pStream);

                    Ole32.ReleaseStgMedium(ref medium);
                }
            }

            /// <summary>
            ///  Retrieves the specified form from the specified hglobal.
            /// </summary>
            private object GetDataFromHGLOBAL(string format, IntPtr hglobal)
            {
                object data = null;

                if (hglobal != IntPtr.Zero)
                {
                    // Convert from OLE to IW objects
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

                object data = null;

                if ((int)HRESULT.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        innerData.GetData(ref formatetc, out medium);

                        if (medium.tymed == TYMED.TYMED_HGLOBAL && medium.unionmember != IntPtr.Zero)
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
                    finally
                    {
                        Ole32.ReleaseStgMedium(ref medium);
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

                if (tymed == (TYMED)0)
                {
                    return null;
                }

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = tymed;

                object data = null;
                if ((int)HRESULT.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        innerData.GetData(ref formatetc, out medium);
                    }
                    catch
                    {
                    }
                }

                try
                {
                    if (medium.tymed == TYMED.TYMED_GDI && medium.unionmember != IntPtr.Zero)
                    {
                        if (format.Equals(DataFormats.Bitmap))
                        {
                            // as/urt 140870 -- GDI+ doesn't own this HBITMAP, but we can't
                            // delete it while the object is still around.  So we have to do the really expensive
                            // thing of cloning the image so we can release the HBITMAP.

                            // This bitmap is created by the com object which originally copied the bitmap to the
                            // clipboard. We call Add here, since DeleteObject calls Remove.
                            Image clipboardImage = Image.FromHbitmap(medium.unionmember);
                            if (clipboardImage != null)
                            {
                                Image firstImage = clipboardImage;
                                clipboardImage = (Image)clipboardImage.Clone();
                                firstImage.Dispose();
                            }
                            data = clipboardImage;
                        }
                    }
                }
                finally
                {
                    Ole32.ReleaseStgMedium(ref medium);
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
                    if (data is null)
                    {
                        data = GetDataFromOleHGLOBAL(format, out done);
                    }
                    if (data is null && !done)
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
                IntPtr ptr = Kernel32.GlobalLock(handle);
                if (ptr == IntPtr.Zero)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_OUTOFMEMORY);
                }
                try
                {
                    int size = Kernel32.GlobalSize(handle);
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
                    Kernel32.GlobalUnlock(handle);
                }
            }

            /// <summary>
            ///  Creates a new instance of the Object that has been persisted into the
            ///  handle.
            /// </summary>
            private object ReadObjectFromHandle(IntPtr handle, bool restrictDeserialization)
            {
                Stream stream = ReadByteStreamFromHandle(handle, out bool isSerializedObject);
                if (isSerializedObject)
                {
                    return ReadObjectFromHandleDeserializer(stream, restrictDeserialization);
                }

                return stream;
            }

            private static object ReadObjectFromHandleDeserializer(Stream stream, bool restrictDeserialization)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (restrictDeserialization)
                {
                    formatter.Binder = new BitmapBinder();
                }
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
#pragma warning disable SYSLIB0011
                return formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }

            /// <summary>
            ///  Parses the HDROP format and returns a list of strings using
            ///  the DragQueryFile function.
            /// </summary>
            private string[] ReadFileListFromHandle(IntPtr hdrop)
            {
                uint count = Shell32.DragQueryFileW(hdrop, 0xFFFFFFFF, null);
                if (count == 0)
                {
                    return null;
                }

                var sb = new StringBuilder(Kernel32.MAX_PATH);
                var files = new string[count];
                for (uint i = 0; i < count; i++)
                {
                    uint charlen = Shell32.DragQueryFileW(hdrop, i, sb);
                    if (charlen == 0)
                    {
                        continue;
                    }

                    string s = sb.ToString(0, (int)charlen);
                    string fullPath = Path.GetFullPath(s);
                    files[i] = s;
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

                IntPtr ptr = Kernel32.GlobalLock(handle);
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
                    Kernel32.GlobalUnlock(handle);
                }

                return stringData;
            }

            private unsafe string ReadHtmlFromHandle(IntPtr handle)
            {
                IntPtr ptr = Kernel32.GlobalLock(handle);
                try
                {
                    int size = Kernel32.GlobalSize(handle);
                    return Encoding.UTF8.GetString((byte*)ptr, size - 1);
                }
                finally
                {
                    Kernel32.GlobalUnlock(handle);
                }
            }

            //=------------------------------------------------------------------------=
            // IDataObject
            //=------------------------------------------------------------------------=
            public virtual object GetData(string format, bool autoConvert)
            {
                object baseVar = GetDataFromBoundOleDataObject(format, out bool done);
                object original = baseVar;

                if (!done && autoConvert && (baseVar is null || baseVar is MemoryStream))
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
                return hr == (int)HRESULT.S_OK;
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

                // Since we are only adding elements to the HashSet, the order will be preserved.
                HashSet<string> distinctFormats = new HashSet<string>();
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
                                    distinctFormats.Add(mappedFormats[i]);
                                }
                            }
                            else
                            {
                                distinctFormats.Add(name);
                            }
                        }
                    }
                }

                return distinctFormats.ToArray();
            }

            public virtual string[] GetFormats()
            {
                return GetFormats(true);
            }
        }
    }
}
