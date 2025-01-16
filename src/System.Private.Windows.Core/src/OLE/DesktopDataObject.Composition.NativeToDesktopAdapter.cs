// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.Resources;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Windows.Win32;
using Windows.Win32.System.Memory;
using Windows.Win32.UI.Shell;
using Com = Windows.Win32.System.Com;

namespace System.Private.Windows.Core.OLE;

internal abstract unsafe partial class DesktopDataObject
{
    internal abstract unsafe partial class Composition
    {
        /// <summary>
        ///  Maps native pointer <see cref="Com.IDataObject"/> to <see cref="IDataObject"/>.
        /// </summary>
        internal abstract unsafe class NativeToDesktopAdapter : ITypedDataObjectDesktop, Com.IDataObject.Interface
        {
            private readonly AgileComPointer<Com.IDataObject> _nativeDataObject;

            public NativeToDesktopAdapter(Com.IDataObject* dataObject)
            {
#if DEBUG
                _nativeDataObject = new(dataObject, takeOwnership: true, trackDisposal: false);
#else
                _nativeDataObject = new(dataObject, takeOwnership: true);
#endif
            }

            /// <summary>
            ///  Gets the appropriate <see cref="BinaryFormatUtilities"/> for the application.
            /// </summary>
            public abstract BinaryFormatUtilities GetBinaryFormatUtilities();

            /// <summary>
            ///  Tries to extract the bitmap out if it is present in the <paramref name="dataObject"/> under the given format.
            /// </summary>
            /// <returns><see langword="true"/> if successful. Otherwise, <see langword="false"/></returns>
            public abstract bool TryGetBitmap<T>(Com.IDataObject* dataObject, string format, [NotNullWhen(true)] out object? bitmap);

            /// <summary>
            ///  Throws <see cref="NotSupportedException"/> if the format and type combination requires a resolver.
            /// </summary>
            protected abstract void ThrowIfFormatAndTypeRequireResolver<T>(string format);

            #region Com.IDataObject.Interface

            HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
            }

            HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DUnadvise(dwConnection);
            }

            HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumDAdvise(ppenumAdvise);
            }

            HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumFormatEtc(dwDirection, ppenumFormatEtc);
            }

            HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetData(pformatetcIn, pmedium);
            }

            HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetDataHere(pformatetc, pmedium);
            }

            HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->QueryGetData(pformatetc);
            }

            HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
            }

            HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->SetData(pformatetc, pmedium, fRelease);
            }

            #endregion

            /// <summary>
            ///  Retrieves the specified format from the specified <paramref name="hglobal"/>.
            /// </summary>
            private bool TryGetDataFromHGLOBAL<T>(
                HGLOBAL hglobal,
                string format,
                Func<TypeName, Type>? resolver,
                bool legacyMode,
                [NotNullWhen(true)] out T? data)
            {
                data = default;
                if (hglobal == 0)
                {
                    return false;
                }

                object? value = format switch
                {
                    DesktopDataFormats.TextConstant or DesktopDataFormats.RtfConstant or DesktopDataFormats.OemTextConstant =>
                        ReadStringFromHGLOBAL(hglobal, unicode: false),
                    DesktopDataFormats.HtmlConstant => ReadUtf8StringFromHGLOBAL(hglobal),
                    DesktopDataFormats.UnicodeTextConstant => ReadStringFromHGLOBAL(hglobal, unicode: true),
                    DesktopDataFormats.FileDropConstant => ReadFileListFromHDROP((HDROP)(nint)hglobal),
                    CF_DEPRECATED_FILENAME => new string[] { ReadStringFromHGLOBAL(hglobal, unicode: false) },
                    CF_DEPRECATED_FILENAMEW => new string[] { ReadStringFromHGLOBAL(hglobal, unicode: true) },
                    _ => ReadObjectOrStreamFromHGLOBAL(hglobal, RestrictDeserializationToSafeTypes(format), resolver, legacyMode)
                };

                if (value is T t)
                {
                    data = t;
                    return true;
                }

                return false;

                object? ReadObjectOrStreamFromHGLOBAL(
                    HGLOBAL hglobal,
                    bool restrictDeserialization,
                    Func<TypeName, Type>? resolver,
                    bool legacyMode)
                {
                    MemoryStream stream = ReadByteStreamFromHGLOBAL(hglobal, out bool isSerializedObject);
                    return !isSerializedObject
                        ? stream
                        : restrictDeserialization
                            ? GetBinaryFormatUtilities().ReadRestrictedObjectFromStream<T>(stream, resolver, legacyMode)
                            : GetBinaryFormatUtilities().ReadObjectFromStream<T>(stream, resolver, legacyMode);
                }
            }

            private static unsafe MemoryStream ReadByteStreamFromHGLOBAL(HGLOBAL hglobal, out bool isSerializedObject)
            {
                void* buffer = PInvokeCore.GlobalLock(hglobal);
                if (buffer is null)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_OUTOFMEMORY);
                }

                try
                {
                    int size = (int)PInvokeCore.GlobalSize(hglobal);
                    byte[] bytes = new byte[size];
                    Marshal.Copy((nint)buffer, bytes, 0, size);
                    int index = 0;

                    // The object here can either be a stream or a serialized object. We identify a serialized object
                    // by writing the bytes for the GUID serializedObjectID at the front of the stream.

                    if (isSerializedObject = bytes.AsSpan().StartsWith(s_serializedObjectID))
                    {
                        index = s_serializedObjectID.Length;
                    }

                    return new MemoryStream(bytes, index, bytes.Length - index);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }
            }

            private static unsafe string ReadStringFromHGLOBAL(HGLOBAL hglobal, bool unicode)
            {
                string? stringData = null;

                void* buffer = PInvokeCore.GlobalLock(hglobal);
                try
                {
                    stringData = unicode ? new string((char*)buffer) : new string((sbyte*)buffer);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }

                return stringData;
            }

            private static unsafe string ReadUtf8StringFromHGLOBAL(HGLOBAL hglobal)
            {
                void* buffer = PInvokeCore.GlobalLock(hglobal);
                try
                {
                    int size = (int)PInvokeCore.GlobalSize(hglobal);
                    return Encoding.UTF8.GetString((byte*)buffer, size - 1);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }
            }

            private static unsafe string[]? ReadFileListFromHDROP(HDROP hdrop)
            {
                uint count = PInvokeCore.DragQueryFile(hdrop, iFile: 0xFFFFFFFF, lpszFile: null, cch: 0);
                if (count == 0)
                {
                    return null;
                }

                Span<char> fileName = stackalloc char[(int)PInvokeCore.MAX_PATH + 1];
                string[] files = new string[count];

                fixed (char* buffer = fileName)
                {
                    for (uint i = 0; i < count; i++)
                    {
                        uint charactersCopied = PInvokeCore.DragQueryFile(hdrop, i, buffer, (uint)fileName.Length);
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
            ///  Extracts a managed object from <see cref="Com.IDataObject"/> of the specified format.
            /// </summary>
            /// <param name="doNotContinue">
            ///  A restricted type was encountered, do not continue trying to deserialize.
            /// </param>
            /// <returns>
            ///  <para>
            ///   <see langword="true"/> if the managed object of <see cref="Type"/> <typeparamref name="T"/> was successfully
            ///   created, <see langword="false"/> if the payload does not contain the specified format or the specified type.
            ///  </para>
            ///  <para>
            ///   If <paramref name="dataObject"/> contains <see cref="MemoryStream"/> that contains a serialized object,
            ///   we return that object cast to <typeparamref name="T"/> or null. If that <see cref="MemoryStream"/> is
            ///   not a serialized object, and a stream was requested, i.e. can be assigned to <typeparamref name="T"/>
            ///   we return that <see cref="MemoryStream"/>.
            ///  </para>
            /// </returns>
            /// <exception cref="NotSupportedException"> is deserialization failed.</exception>
            private bool TryGetObjectFromDataObject<T>(
                Com.IDataObject* dataObject,
                string format,
                Func<TypeName, Type>? resolver,
                bool legacyMode,
                out bool doNotContinue,
                [NotNullWhen(true)] out T? data)
            {
                data = default;
                doNotContinue = false;
                bool result = false;

                try
                {
                    // Try to get the data as a bitmap first.
                    if (TryGetBitmap<T>(dataObject, format, out object? bitmap))
                    {
                        data = (T)bitmap;
                        return true;
                    }

                    result = TryGetHGLOBALData(dataObject, format, resolver, legacyMode, out doNotContinue, out data);
                    if (!result && !doNotContinue)
                    {
                        // Lastly check to see if the data is an IStream.
                        result = TryGetIStreamData(dataObject, format, resolver, legacyMode, out data);
                    }
                }
                catch (Exception e) when (legacyMode || e is not NotSupportedException)
                {
                    Debug.Fail(e.ToString());
                }

                return result;
            }

            private bool TryGetHGLOBALData<T>(
                Com.IDataObject* dataObject,
                string format,
                Func<TypeName, Type>? resolver,
                bool legacyMode,
                out bool doNotContinue,
                [NotNullWhen(true)] out T? data)
            {
                data = default;
                doNotContinue = false;

                Com.FORMATETC formatetc = new()
                {
                    cfFormat = (ushort)DesktopDataFormats.GetFormat(format).Id,
                    dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)Com.TYMED.TYMED_HGLOBAL
                };

                if (dataObject->QueryGetData(formatetc).Failed)
                {
                    return false;
                }

                HRESULT hr = dataObject->GetData(formatetc, out Com.STGMEDIUM medium);

                // One of the ways this can happen is when we attempt to put binary formatted data onto the
                // clipboard, which will succeed as Windows ignores all errors when putting data on the clipboard.
                // The data state, however, is not good, and this error will be returned by Windows when asking to
                // get the data out.
                Debug.WriteLineIf(hr == HRESULT.CLIPBRD_E_BAD_DATA, "CLIPBRD_E_BAD_DATA returned when trying to get clipboard data.");
                Debug.WriteLineIf(hr == HRESULT.DV_E_TYMED, "DV_E_TYMED returned when trying to get clipboard data.");
                // This happens in copy == false case when the managed type does not have the [Serializable] attribute.
                Debug.WriteLineIf(hr == HRESULT.E_UNEXPECTED, "E_UNEXPECTED returned when trying to get clipboard data.");
                Debug.WriteLineIf(hr == HRESULT.COR_E_SERIALIZATION,
                    "COR_E_SERIALIZATION returned when trying to get clipboard data, for example, BinaryFormatter threw SerializationException.");

                bool result = false;
                try
                {
                    if (medium.tymed == Com.TYMED.TYMED_HGLOBAL && !medium.hGlobal.IsNull && hr != HRESULT.COR_E_SERIALIZATION)
                    {
                        result = TryGetDataFromHGLOBAL(medium.hGlobal, format, resolver, legacyMode, out data);
                    }
                }
                catch (RestrictedTypeDeserializationException)
                {
                    result = false;
                    data = default;
                    doNotContinue = true;
                }
                catch (Exception ex) when (legacyMode || ex is not NotSupportedException)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    PInvokeCore.ReleaseStgMedium(ref medium);
                }

                return result;
            }

            private unsafe bool TryGetIStreamData<T>(
                Com.IDataObject* dataObject,
                string format,
                Func<TypeName, Type>? resolver,
                bool legacyMode,
                [NotNullWhen(true)] out T? data)
            {
                data = default;
                Com.FORMATETC formatEtc = new()
                {
                    cfFormat = (ushort)DesktopDataFormats.GetFormat(format).Id,
                    dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)Com.TYMED.TYMED_ISTREAM
                };

                // Limit the # of exceptions we may throw below.
                if (dataObject->QueryGetData(formatEtc).Failed
                    || dataObject->GetData(formatEtc, out Com.STGMEDIUM medium).Failed)
                {
                    return false;
                }

                HGLOBAL hglobal = default;
                try
                {
                    if (medium.tymed != Com.TYMED.TYMED_ISTREAM || medium.hGlobal.IsNull)
                    {
                        return false;
                    }

                    using ComScope<Com.IStream> pStream = new((Com.IStream*)medium.hGlobal);
                    pStream.Value->Stat(out Com.STATSTG sstg, (uint)Com.STATFLAG.STATFLAG_DEFAULT);

                    hglobal = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, (uint)sstg.cbSize);

                    // Not throwing here because the other out of memory condition on GlobalAlloc
                    // happens inside innerData.GetData and gets turned into a null return value.
                    if (hglobal.IsNull)
                    {
                        return false;
                    }

                    void* ptr = PInvokeCore.GlobalLock(hglobal);
                    pStream.Value->Read((byte*)ptr, (uint)sstg.cbSize, null);
                    PInvokeCore.GlobalUnlock(hglobal);

                    return TryGetDataFromHGLOBAL(hglobal, format, resolver, legacyMode, out data);
                }
                finally
                {
                    if (!hglobal.IsNull)
                    {
                        PInvokeCore.GlobalFree(hglobal);
                    }

                    PInvokeCore.ReleaseStgMedium(ref medium);
                }
            }

            private bool TryGetDataInternal<T>(
                string format,
                Func<TypeName, Type>? resolver,
                bool autoConvert,
                bool legacyMode,
                [NotNullWhen(true)] out T? data)
            {
                data = default;
                if (!legacyMode && resolver is null)
                {
                    // DataObject.GetData methods do not validate format string, but the typed methods do.
                    // This validation is specific to the WinForms DataObject implementation, it's not executed for
                    // overridden methods.
                    ThrowIfFormatAndTypeRequireResolver<T>(format);
                }

                using var nativeDataObject = _nativeDataObject.GetInterface();

                bool result = TryGetObjectFromDataObject(
                    nativeDataObject, format, resolver, legacyMode, out bool doNotContinue, out data);

                if (doNotContinue)
                {
                    // Specified format is a restricted one, as only restricted formats set doNotContinue,
                    // but content required BinaryFormatter deserialization, as doNotContinue is set when
                    // BinaryFormatter fails, legacy methods return null.
                    data = default;
                    return false;
                }

                if (result || !autoConvert || GetMappedFormats(format) is not { } mappedFormats)
                {
                    return result;
                }

                // Try to find a mapped format that works better.
                foreach (string mappedFormat in mappedFormats)
                {
                    if (format.Equals(mappedFormat))
                    {
                        continue;
                    }

                    result = TryGetObjectFromDataObject(
                        nativeDataObject, mappedFormat, resolver, legacyMode, out doNotContinue, out data);
                    if (doNotContinue)
                    {
                        Debug.Fail("All mapped formats must be either restricted or not restricted.");
                        break;
                    }

                    if (result)
                    {
                        return result;
                    }
                }

                return result;
            }

            #region IDataObject
            public object? GetData(string format, bool autoConvert)
            {
                TryGetDataInternal(format, resolver: null, autoConvert, legacyMode: true, out object? data);
                return data;
            }

            public object? GetData(string format) => GetData(format, autoConvert: true);
            public object? GetData(Type format) => GetData(format.FullName!);
            public bool GetDataPresent(Type format) => GetDataPresent(format.FullName!);

            public bool GetDataPresent(string format, bool autoConvert)
            {
                bool dataPresent = GetDataPresentInner(format);

                if (dataPresent || !autoConvert || GetMappedFormats(format) is not { } mappedFormats)
                {
                    return dataPresent;
                }

                foreach (string mappedFormat in mappedFormats)
                {
                    if (!format.Equals(mappedFormat) && (dataPresent = GetDataPresentInner(mappedFormat)))
                    {
                        break;
                    }
                }

                return dataPresent;
            }

            public bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

            public string[] GetFormats(bool autoConvert)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                Debug.Assert(!nativeDataObject.IsNull, "You must have an innerData on all DataObjects");

                using ComScope<Com.IEnumFORMATETC> enumFORMATETC = new(null);
                nativeDataObject.Value->EnumFormatEtc((uint)DATADIR.DATADIR_GET, enumFORMATETC).AssertSuccess();

                if (enumFORMATETC.IsNull)
                {
                    return [];
                }

                // Since we are only adding elements to the HashSet, the order will be preserved.
                HashSet<string> distinctFormats = [];

                enumFORMATETC.Value->Reset();

                Com.FORMATETC formatEtc = default;

                while (enumFORMATETC.Value->Next(1, &formatEtc) == HRESULT.S_OK)
                {
                    string name = DesktopDataFormats.GetFormat(formatEtc.cfFormat).Name;
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

                    formatEtc = default;
                }

                return [.. distinctFormats];
            }

            public string[] GetFormats() => GetFormats(autoConvert: true);

            public void SetData(string format, bool autoConvert, object? data) { }
            public void SetData(string format, object? data) { }
            public void SetData(Type format, object? data) { }
            public void SetData(object? data) { }
            #endregion

            #region ITypedDataObject
            public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
                string format,
                Func<TypeName, Type> resolver,
                bool autoConvert,
                [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                    TryGetDataInternal(format, resolver, autoConvert, legacyMode: false, out data);

            public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
                string format,
                bool autoConvert,
                [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                    TryGetDataInternal(format, resolver: null!, autoConvert, legacyMode: false, out data);

            public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
                string format,
                [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                    TryGetDataInternal(format, resolver: null!, autoConvert: true, legacyMode: false, out data);

            public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
                [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                    TryGetDataInternal(typeof(T).FullName!, resolver: null!, autoConvert: true, legacyMode: false, out data);
            #endregion

            private bool GetDataPresentInner(string format)
            {
                Com.FORMATETC formatEtc = new()
                {
                    cfFormat = (ushort)(DesktopDataFormats.GetFormat(format).Id),
                    dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)AllowedTymeds
                };

                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT hr = nativeDataObject.Value->QueryGetData(formatEtc);

                // APIs will return S_FALSE, which is "success"
                return hr == HRESULT.S_OK;
            }
        }
    }
}
