// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.Ole;
using System.Runtime.Serialization;
using System.Text;
using Windows.Win32.System.Com;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  Maps <see cref="IDataObject"/> to <see cref="Com.IDataObject.Interface"/>.
        /// </summary>
        private unsafe class WinFormsToNativeAdapter : Com.IDataObject.Interface, IManagedWrapper<Com.IDataObject>
        {
            private const int DATA_S_SAMEFORMATETC = 0x00040130;

            private readonly IDataObject _dataObject;

            public WinFormsToNativeAdapter(IDataObject dataObject)
            {
                _dataObject = dataObject;
            }

            /// <summary>
            ///  Returns true if the tymed is usable.
            /// </summary>
            private static bool GetTymedUsable(TYMED tymed) => (tymed & AllowedTymeds) != 0;

            #region  Com.IDataObject.Interface
            public HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
            {
                if (pformatetcIn is null)
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (DragDropHelper.IsInDragLoop(_dataObject))
                {
                    string formatName = DataFormats.GetFormat(pformatetcIn->cfFormat).Name;
                    if (!_dataObject.GetDataPresent(formatName))
                    {
                        *pmedium = default;
                        return HRESULT.S_OK;
                    }

                    if (_dataObject.GetData(formatName) is DragDropFormat dragDropFormat)
                    {
                        *pmedium = dragDropFormat.GetData();
                        return HRESULT.S_OK;
                    }
                }

                *pmedium = default;

                if (!GetTymedUsable((TYMED)pformatetcIn->tymed))
                {
                    return HRESULT.DV_E_TYMED;
                }

                if (!((TYMED)pformatetcIn->tymed).HasFlag(TYMED.TYMED_HGLOBAL))
                {
                    pmedium->tymed = (TYMED)pformatetcIn->tymed;
                    return GetDataHere(pformatetcIn, pmedium);
                }

                pmedium->tymed = TYMED.TYMED_HGLOBAL;
                pmedium->hGlobal = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, 1);

                if (pmedium->hGlobal.IsNull)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                HRESULT result = GetDataHere(pformatetcIn, pmedium);
                if (result.Failed)
                {
                    PInvokeCore.GlobalFree(pmedium->hGlobal);
                    pmedium->hGlobal = HGLOBAL.Null;
                }

                return result;
            }

            public HRESULT GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium)
            {
                if (pformatetc is null)
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (!GetTymedUsable((TYMED)pformatetc->tymed) || !GetTymedUsable(pmedium->tymed))
                {
                    return HRESULT.DV_E_TYMED;
                }

                string format = DataFormats.GetFormat(pformatetc->cfFormat).Name;

                if (!_dataObject.GetDataPresent(format))
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                if (_dataObject.GetData(format) is not object data)
                {
                    return HRESULT.E_UNEXPECTED;
                }

                if (((TYMED)pformatetc->tymed).HasFlag(TYMED.TYMED_HGLOBAL))
                {
                    try
                    {
                        return SaveDataToHGLOBAL(data, format, ref *pmedium);
                    }
                    catch (NotSupportedException ex)
                    {
                        // BinaryFormatter is disabled. As all errors get swallowed by Windows, put the exception on the
                        // clipboard so consumers can get some indication as to what is wrong. (We handle the binary formatting
                        // of this exception, so it will always work.)
                        return SaveDataToHGLOBAL(ex, format, ref *pmedium);
                    }
                    catch (Exception) when (!pmedium->hGlobal.IsNull)
                    {
                        PInvokeCore.GlobalFree(pmedium->hGlobal);
                        pmedium->hGlobal = HGLOBAL.Null;

                        throw;
                    }
                }

                if (((TYMED)pformatetc->tymed).HasFlag(TYMED.TYMED_GDI))
                {
                    if (format.Equals(DataFormats.Bitmap) && data is Bitmap bitmap)
                    {
                        // Save bitmap
                        pmedium->u.hBitmap = GetCompatibleBitmap(bitmap);
                    }

                    return HRESULT.S_OK;
                }

                return HRESULT.DV_E_TYMED;

                static HBITMAP GetCompatibleBitmap(Bitmap bitmap)
                {
                    using var screenDC = GetDcScope.ScreenDC;

                    // GDI+ returns a DIBSECTION based HBITMAP. The clipboard only deals well with bitmaps created using
                    // CreateCompatibleBitmap(). So, we convert the DIBSECTION into a compatible bitmap.
                    HBITMAP hbitmap = bitmap.GetHBITMAP();

                    // Create a compatible DC to render the source bitmap.
                    using CreateDcScope sourceDC = new(screenDC);
                    using SelectObjectScope sourceBitmapSelection = new(sourceDC, hbitmap);

                    // Create a compatible DC and a new compatible bitmap.
                    using CreateDcScope destinationDC = new(screenDC);
                    HBITMAP compatibleBitmap = PInvokeCore.CreateCompatibleBitmap(screenDC, bitmap.Size.Width, bitmap.Size.Height);

                    // Select the new bitmap into a compatible DC and render the blt the original bitmap.
                    using SelectObjectScope destinationBitmapSelection = new(destinationDC, compatibleBitmap);
                    PInvokeCore.BitBlt(
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

            public HRESULT QueryGetData(FORMATETC* pformatetc)
            {
                if (pformatetc is null)
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                if (pformatetc->dwAspect != (uint)DVASPECT.DVASPECT_CONTENT)
                {
                    return HRESULT.DV_E_DVASPECT;
                }

                if (!GetTymedUsable((TYMED)pformatetc->tymed))
                {
                    return HRESULT.DV_E_TYMED;
                }

                if (pformatetc->cfFormat == 0)
                {
                    return HRESULT.S_FALSE;
                }

                if (!_dataObject.GetDataPresent(DataFormats.GetFormat(pformatetc->cfFormat).Name))
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                return HRESULT.S_OK;
            }

            public HRESULT GetCanonicalFormatEtc(FORMATETC* pformatectIn, FORMATETC* pformatetcOut)
            {
                if (pformatetcOut is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pformatetcOut = default;
                return (HRESULT)DATA_S_SAMEFORMATETC;
            }

            public HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease)
            {
                if (pformatetc is null)
                {
                    return HRESULT.DV_E_FORMATETC;
                }

                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (DragDropHelper.IsInDragLoopFormat(*pformatetc) || DragDropHelper.IsInDragLoop(_dataObject))
                {
                    string formatName = DataFormats.GetFormat(pformatetc->cfFormat).Name;
                    if (_dataObject.GetDataPresent(formatName) && _dataObject.GetData(formatName) is DragDropFormat dragDropFormat)
                    {
                        dragDropFormat.RefreshData(pformatetc->cfFormat, *pmedium, !fRelease);
                    }
                    else
                    {
                        _dataObject.SetData(formatName, new DragDropFormat(pformatetc->cfFormat, *pmedium, !fRelease));
                    }

                    return HRESULT.S_OK;
                }

                return HRESULT.E_NOTIMPL;
            }

            public HRESULT EnumFormatEtc(uint dwDirection, IEnumFORMATETC** ppenumFormatEtc)
            {
                if (ppenumFormatEtc is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (dwDirection == (uint)ComTypes.DATADIR.DATADIR_GET)
                {
                    *ppenumFormatEtc = ComHelpers.GetComPointer<IEnumFORMATETC>(new FormatEnumerator(_dataObject));
                    return HRESULT.S_OK;
                }

                return HRESULT.E_NOTIMPL;
            }

            public HRESULT DAdvise(FORMATETC* pformatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection)
            {
                if (pdwConnection is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pdwConnection = 0;
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT DUnadvise(uint dwConnection) => HRESULT.E_NOTIMPL;

            public HRESULT EnumDAdvise(IEnumSTATDATA** ppenumAdvise)
            {
                if (ppenumAdvise is null)
                {
                    return HRESULT.E_POINTER;
                }

                *ppenumAdvise = null;
                return HRESULT.OLE_E_ADVISENOTSUPPORTED;
            }
            #endregion

            private HRESULT SaveDataToHGLOBAL(object data, string format, ref STGMEDIUM medium) => format switch
            {
                _ when data is Stream dataStream
                    => SaveStreamToHGLOBAL(ref medium.hGlobal, dataStream),
                DataFormatNames.Text or DataFormatNames.Rtf or DataFormatNames.OemText
                    => SaveStringToHGLOBAL(medium.hGlobal, data.ToString()!, unicode: false),
                DataFormatNames.Html
                    => SaveHtmlToHGLOBAL(medium.hGlobal, data.ToString()!),
                DataFormatNames.UnicodeText
                    => SaveStringToHGLOBAL(medium.hGlobal, data.ToString()!, unicode: true),
                DataFormatNames.FileDrop
                    => SaveFileListToHGLOBAL(medium.hGlobal, (string[])data),
                CF_DEPRECATED_FILENAME
                    => SaveStringToHGLOBAL(medium.hGlobal, ((string[])data)[0], unicode: false),
                CF_DEPRECATED_FILENAMEW
                    => SaveStringToHGLOBAL(medium.hGlobal, ((string[])data)[0], unicode: true),
                DataFormatNames.Dib when data is Image
                    // GDI+ does not properly handle saving to DIB images. Since the clipboard will take
                    // an HBITMAP and publish a Dib, we don't need to support this.
                    => HRESULT.DV_E_TYMED,
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                _ when format == DataFormatNames.Serializable || data is ISerializable || data.GetType().IsSerializable
#pragma warning restore
                    => SaveObjectToHGLOBAL(ref medium.hGlobal, data, RestrictDeserializationToSafeTypes(format)),
                _ => HRESULT.E_UNEXPECTED
            };

            private static HRESULT SaveObjectToHGLOBAL(ref HGLOBAL hglobal, object data, bool restrictSerialization)
            {
                using MemoryStream stream = new();
                stream.Write(s_serializedObjectID);

                // Throws in case of serialization failure.
                BinaryFormatUtilities.WriteObjectToStream(stream, data, restrictSerialization);

                return SaveStreamToHGLOBAL(ref hglobal, stream);
            }

            private static HRESULT SaveStreamToHGLOBAL(ref HGLOBAL hglobal, Stream stream)
            {
                if (!hglobal.IsNull)
                {
                    PInvokeCore.GlobalFree(hglobal);
                }

                int size = checked((int)stream.Length);
                hglobal = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, (uint)size);
                if (hglobal.IsNull)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                void* buffer = PInvokeCore.GlobalLock(hglobal);
                if (buffer is null)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                try
                {
                    Span<byte> span = new(buffer, size);
                    stream.Position = 0;
                    stream.ReadExactly(span);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
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
                HGLOBAL newHandle = PInvokeCore.GlobalReAlloc(hglobal, sizeInBytes, (uint)GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE);
                if (newHandle == 0)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                void* buffer = PInvokeCore.GlobalLock(newHandle);
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

                PInvokeCore.GlobalUnlock(newHandle);
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
                    newHandle = PInvokeCore.GlobalReAlloc(hglobal, byteSize, (uint)(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT));
                    if (newHandle == 0)
                    {
                        return HRESULT.E_OUTOFMEMORY;
                    }

                    char* buffer = (char*)PInvokeCore.GlobalLock(newHandle);
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
                        newHandle = PInvokeCore.GlobalReAlloc(hglobal, (uint)pinvokeSize + 1, (uint)GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | (uint)GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT);
                        if (newHandle == 0)
                        {
                            return HRESULT.E_OUTOFMEMORY;
                        }

                        byte* buffer = (byte*)PInvokeCore.GlobalLock(newHandle);
                        if (buffer is null)
                        {
                            return HRESULT.E_OUTOFMEMORY;
                        }

                        PInvoke.WideCharToMultiByte(PInvoke.CP_ACP, 0, value, value.Length, buffer, pinvokeSize, null, null);

                        // Null terminate
                        buffer[pinvokeSize] = 0;
                    }
                }

                PInvokeCore.GlobalUnlock(newHandle);
                return HRESULT.S_OK;
            }

            private static HRESULT SaveHtmlToHGLOBAL(HGLOBAL hglobal, string value)
            {
                if (hglobal == 0)
                {
                    return HRESULT.E_INVALIDARG;
                }

                int byteLength = Encoding.UTF8.GetByteCount(value);
                HGLOBAL newHandle = PInvokeCore.GlobalReAlloc(hglobal, (uint)byteLength + 1, (uint)(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT));
                if (newHandle == 0)
                {
                    return HRESULT.E_OUTOFMEMORY;
                }

                byte* buffer = (byte*)PInvokeCore.GlobalLock(newHandle);
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
                    PInvokeCore.GlobalUnlock(newHandle);
                }

                return HRESULT.S_OK;
            }
        }
    }
}
