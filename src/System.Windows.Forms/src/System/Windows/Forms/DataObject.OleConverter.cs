// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Interop;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;
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
            internal IComDataObject _innerData;

            public OleConverter(IComDataObject data)
            {
                Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, "OleConverter: Constructed OleConverter");
                _innerData = data;
            }

            /// <summary>
            ///  Returns the data Object we are wrapping
            /// </summary>
            public IComDataObject OleDataObject
            {
                get
                {
                    return _innerData;
                }
            }

            /// <summary>
            ///  Uses IStream and retrieves the specified format from the bound IComDataObject.
            /// </summary>
            private unsafe object? GetDataFromOleIStream(string format)
            {
                FORMATETC formatetc = new FORMATETC();
                STGMEDIUM medium = new STGMEDIUM();

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = TYMED.TYMED_ISTREAM;

                // Limit the # of exceptions we may throw below.
                if ((int)HRESULT.Values.S_OK != QueryGetDataUnsafe(ref formatetc))
                {
                    return null;
                }

                try
                {
                    _innerData.GetData(ref formatetc, out medium);
                }
                catch
                {
                    return null;
                }

                Ole32.IStream? pStream = null;
                nint hglobal = 0;
                try
                {
                    if (medium.tymed == TYMED.TYMED_ISTREAM && medium.unionmember != IntPtr.Zero)
                    {
                        pStream = (Ole32.IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                        pStream.Stat(out Ole32.STATSTG sstg, Ole32.STATFLAG.DEFAULT);

                        hglobal = PInvoke.GlobalAlloc(
                            GMEM_MOVEABLE | GMEM_ZEROINIT,
                            (uint)sstg.cbSize);
                        // not throwing here because the other out of memory condition on GlobalAlloc
                        // happens inside innerData.GetData and gets turned into a null return value
                        if (hglobal == 0)
                            return null;
                        void* ptr = PInvoke.GlobalLock(hglobal);
                        pStream.Read((byte*)ptr, (uint)sstg.cbSize, null);
                        PInvoke.GlobalUnlock(hglobal);

                        return GetDataFromHGLOBAL(format, hglobal);
                    }

                    return null;
                }
                finally
                {
                    if (hglobal != 0)
                        PInvoke.GlobalFree(hglobal);

                    if (pStream is not null)
                        Marshal.ReleaseComObject(pStream);

                    Ole32.ReleaseStgMedium(ref medium);
                }
            }

            /// <summary>
            ///  Retrieves the specified form from the specified hglobal.
            /// </summary>
            private static object? GetDataFromHGLOBAL(string format, IntPtr hglobal)
            {
                object? data = null;

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
                        data = ReadFileListFromHandle((HDROP)hglobal);
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
            ///  Uses HGLOBALs and retrieves the specified format from the bound IComDataObject.
            /// </summary>
            private object? GetDataFromOleHGLOBAL(string format, out bool done)
            {
                done = false;
                Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");

                FORMATETC formatetc = new FORMATETC();
                STGMEDIUM medium = new STGMEDIUM();

                formatetc.cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id));
                formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
                formatetc.lindex = -1;
                formatetc.tymed = TYMED.TYMED_HGLOBAL;

                object? data = null;

                if ((int)HRESULT.Values.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        _innerData.GetData(ref formatetc, out medium);

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
            private object? GetDataFromOleOther(string format)
            {
                Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");

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

                object? data = null;
                if ((int)HRESULT.Values.S_OK == QueryGetDataUnsafe(ref formatetc))
                {
                    try
                    {
                        _innerData.GetData(ref formatetc, out medium);
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
                            // ASURT 140870 -- GDI+ doesn't own this HBITMAP, but we can't
                            // delete it while the object is still around.  So we have to do the really expensive
                            // thing of cloning the image so we can release the HBITMAP.

                            // This bitmap is created by the com object which originally copied the bitmap to the
                            // clipboard. We call Add here, since DeleteObject calls Remove.
                            Image clipboardImage = Image.FromHbitmap(medium.unionmember);
                            if (clipboardImage is not null)
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
            private object? GetDataFromBoundOleDataObject(string format, out bool done)
            {
                object? data = null;
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
            private unsafe static Stream ReadByteStreamFromHandle(nint handle, out bool isSerializedObject)
            {
                void* ptr = PInvoke.GlobalLock(handle);
                if (ptr is null)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.Values.E_OUTOFMEMORY);
                }

                try
                {
                    int size = (int)PInvoke.GlobalSize(handle);
                    byte[] bytes = new byte[size];
                    Marshal.Copy((nint)ptr, bytes, 0, size);
                    int index = 0;

                    // The object here can either be a stream or a serialized
                    // object.  We identify a serialized object by writing the
                    // bytes for the guid serializedObjectID at the front
                    // of the stream.  Check for that here.
                    //
                    if (size > s_serializedObjectID.Length)
                    {
                        isSerializedObject = true;
                        for (int i = 0; i < s_serializedObjectID.Length; i++)
                        {
                            if (s_serializedObjectID[i] != bytes[i])
                            {
                                isSerializedObject = false;
                                break;
                            }
                        }

                        // Advance the byte pointer.
                        //
                        if (isSerializedObject)
                        {
                            index = s_serializedObjectID.Length;
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
                    PInvoke.GlobalUnlock(handle);
                }
            }

            /// <summary>
            ///  Creates a new instance of the Object that has been persisted into the
            ///  handle.
            /// </summary>
            private static object ReadObjectFromHandle(IntPtr handle, bool restrictDeserialization)
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
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                return formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }

            /// <summary>
            ///  Parses the HDROP format and returns a list of strings using
            ///  the DragQueryFile function.
            /// </summary>
            private unsafe static string[]? ReadFileListFromHandle(HDROP hdrop)
            {
                uint count = PInvoke.DragQueryFile(hdrop, iFile: 0xFFFFFFFF, lpszFile: null, cch: 0);
                if (count == 0)
                {
                    return null;
                }

                Span<char> fileName = stackalloc char[PInvoke.MAX_PATH + 1];
                var files = new string[count];

                fixed (char* buffer = fileName)
                {
                    for (uint i = 0; i < count; i++)
                    {
                        uint charactersCopied = PInvoke.DragQueryFile(hdrop, i, buffer, (uint)fileName.Length);
                        if (charactersCopied == 0)
                        {
                            continue;
                        }

                        string s = fileName[..(int)charactersCopied].ToString();
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
            private static unsafe string ReadStringFromHandle(nint handle, bool unicode)
            {
                string? stringData = null;

                void* ptr = PInvoke.GlobalLock(handle);
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
                    PInvoke.GlobalUnlock(handle);
                }

                return stringData;
            }

            private static unsafe string ReadHtmlFromHandle(nint handle)
            {
                void* ptr = PInvoke.GlobalLock(handle);
                try
                {
                    int size = (int)PInvoke.GlobalSize(handle);
                    return Encoding.UTF8.GetString((byte*)ptr, size - 1);
                }
                finally
                {
                    PInvoke.GlobalUnlock(handle);
                }
            }

            //=------------------------------------------------------------------------=
            // IDataObject
            //=------------------------------------------------------------------------=
            public virtual object? GetData(string format, bool autoConvert)
            {
                object? baseVar = GetDataFromBoundOleDataObject(format, out bool done);
                object? original = baseVar;

                if (!done && autoConvert && (baseVar is null || baseVar is MemoryStream))
                {
                    string[]? mappedFormats = GetMappedFormats(format);
                    if (mappedFormats is not null)
                    {
                        for (int i = 0; ((!done) && (i < mappedFormats.Length)); i++)
                        {
                            if (!format.Equals(mappedFormats[i]))
                            {
                                baseVar = GetDataFromBoundOleDataObject(mappedFormats[i], out done);
                                if (!done && baseVar is not null && !(baseVar is MemoryStream))
                                {
                                    original = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (original is not null)
                {
                    return original;
                }
                else
                {
                    return baseVar;
                }
            }

            public virtual object? GetData(string format)
            {
                return GetData(format, true);
            }

            public virtual object? GetData(Type format)
            {
                return GetData(format.FullName!);
            }

            public virtual void SetData(string format, bool autoConvert, object? data)
            {
            }

            public virtual void SetData(string format, object? data)
            {
                SetData(format, true, data);
            }

            public virtual void SetData(Type format, object? data)
            {
                SetData(format.FullName!, data);
            }

            public virtual void SetData(object? data)
            {
                if (data is ISerializable)
                {
                    SetData(DataFormats.Serializable, data);
                }
                else
                {
                    SetData(data!.GetType(), data);
                }
            }

            private int QueryGetDataUnsafe(ref FORMATETC formatetc)
            {
                return _innerData.QueryGetData(ref formatetc);
            }

            public virtual bool GetDataPresent(Type format)
            {
                return GetDataPresent(format.FullName!);
            }

            private bool GetDataPresentInner(string format)
            {
                Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");
                FORMATETC formatetc = new FORMATETC
                {
                    cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id)),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1
                };

                for (int i = 0; i < s_allowedTymeds.Length; i++)
                {
                    formatetc.tymed |= s_allowedTymeds[i];
                }

                int hr = QueryGetDataUnsafe(ref formatetc);
                return hr == (int)HRESULT.Values.S_OK;
            }

            public virtual bool GetDataPresent(string format, bool autoConvert)
            {
                bool baseVar = GetDataPresentInner(format);

                if (!baseVar && autoConvert)
                {
                    string[]? mappedFormats = GetMappedFormats(format);
                    if (mappedFormats is not null)
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
                Debug.Assert(_innerData is not null, "You must have an innerData on all DataObjects");

                IEnumFORMATETC? enumFORMATETC = null;

                // Since we are only adding elements to the HashSet, the order will be preserved.
                HashSet<string> distinctFormats = new HashSet<string>();
                try
                {
                    enumFORMATETC = _innerData.EnumFormatEtc(DATADIR.DATADIR_GET);
                }
                catch
                {
                }

                if (enumFORMATETC is not null)
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
                                string[] mappedFormats = GetMappedFormats(name)!;
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
