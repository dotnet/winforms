// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    /// <summary>
    ///  Contains the logic to move between <see cref="IDataObject"/>, <see cref="Com.IDataObject.Interface"/>,
    ///  and <see cref="ComTypes.IDataObject"/> calls.
    /// </summary>
    internal unsafe partial class Composition : ITypedDataObject, Com.IDataObject.Interface, ComTypes.IDataObject
    {
        private const Com.TYMED AllowedTymeds = Com.TYMED.TYMED_HGLOBAL | Com.TYMED.TYMED_ISTREAM | Com.TYMED.TYMED_GDI;

        // We use this to identify that a stream is actually a serialized object. On read, we don't know if the contents
        // of a stream were saved "raw" or if the stream is really pointing to a serialized object. If we saved an object,
        // we prefix it with this guid.
        private static readonly byte[] s_serializedObjectID =
        [
            // FD9EA796-3B13-4370-A679-56106BB288FB
            0x96, 0xa7, 0x9e, 0xfd,
            0x13, 0x3b,
            0x70, 0x43,
            0xa6, 0x79, 0x56, 0x10, 0x6b, 0xb2, 0x88, 0xfb
        ];

        private readonly IDataObject _winFormsDataObject;
        private readonly Com.IDataObject.Interface _nativeDataObject;
        private readonly ComTypes.IDataObject _runtimeDataObject;

        // Feature switch, when set to false, BinaryFormatter is not supported in trimmed applications.
        // This field, using the default BinaryFormatter switch, is used to control trim warnings related
        // to using BinaryFormatter in WinForms trimming. The trimmer will generate a warning when set
        // to true and will not generate a warning when set to false.
        [FeatureSwitchDefinition("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization")]
        internal static bool EnableUnsafeBinaryFormatterInNativeObjectSerialization =>
            !AppContext.TryGetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", out bool isEnabled) || isEnabled;

        private Composition(IDataObject winFormsDataObject, Com.IDataObject.Interface nativeDataObject, ComTypes.IDataObject runtimeDataObject)
        {
            _winFormsDataObject = winFormsDataObject;
            _nativeDataObject = nativeDataObject;
            _runtimeDataObject = runtimeDataObject;
            if (winFormsDataObject is not NativeToWinFormsAdapter and not DataStore)
            {
                OriginalIDataObject = winFormsDataObject;
            }
        }

        public static Composition CreateFromWinFormsDataObject(IDataObject winFormsDataObject)
        {
            WinFormsToNativeAdapter winFormsToNative = new(winFormsDataObject);
            NativeToRuntimeAdapter nativeToRuntime = new(ComHelpers.GetComPointer<Com.IDataObject>(winFormsToNative));
            return new(winFormsDataObject, winFormsToNative, nativeToRuntime);
        }

        public static Composition CreateFromNativeDataObject(Com.IDataObject* nativeDataObject)
        {
            // Add ref so each adapter can take ownership of the native data object.
            nativeDataObject->AddRef();
            nativeDataObject->AddRef();
            NativeToWinFormsAdapter nativeToWinForms = new(nativeDataObject);
            NativeToRuntimeAdapter nativeToRuntime = new(nativeDataObject);
            return new(nativeToWinForms, nativeToWinForms, nativeToRuntime);
        }

        public static Composition CreateFromRuntimeDataObject(ComTypes.IDataObject runtimeDataObject)
        {
            RuntimeToNativeAdapter runtimeToNative = new(runtimeDataObject);
            NativeToWinFormsAdapter nativeToWinForms = new(ComHelpers.GetComPointer<Com.IDataObject>(runtimeToNative));
            return new(nativeToWinForms, runtimeToNative, runtimeDataObject);
        }

        /// <summary>
        ///  The <see cref="IDataObject"/> the user passed to us, so that it can be passed back out.
        /// </summary>
        public IDataObject? OriginalIDataObject { get; private set; }

        #region IDataObject
        public object? GetData(string format, bool autoConvert) => _winFormsDataObject.GetData(format, autoConvert);
        public object? GetData(string format) => _winFormsDataObject.GetData(format);
        public object? GetData(Type format) => _winFormsDataObject.GetData(format);
        public bool GetDataPresent(string format, bool autoConvert) => _winFormsDataObject.GetDataPresent(format, autoConvert);
        public bool GetDataPresent(string format) => _winFormsDataObject.GetDataPresent(format);
        public bool GetDataPresent(Type format) => _winFormsDataObject.GetDataPresent(format);
        public string[] GetFormats(bool autoConvert) => _winFormsDataObject.GetFormats(autoConvert);
        public string[] GetFormats() => _winFormsDataObject.GetFormats();
        public void SetData(string format, bool autoConvert, object? data) => _winFormsDataObject.SetData(format, autoConvert, data);
        public void SetData(string format, object? data) => _winFormsDataObject.SetData(format, data);
        public void SetData(Type format, object? data) => _winFormsDataObject.SetData(format, data);
        public void SetData(object? data) => _winFormsDataObject.SetData(data);
        #endregion

        #region ITypedDataObject
        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
            string format,
            Func<TypeName, Type> resolver,
            bool autoConvert,
            [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                _winFormsDataObject.TryGetData(format, resolver, autoConvert, out data);

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
            string format,
            bool autoConvert,
            [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                _winFormsDataObject.TryGetData(format, autoConvert, out data);

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
            string format,
            [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                _winFormsDataObject.TryGetData(format, out data);

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
            [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
                _winFormsDataObject.TryGetData(typeof(T).FullName!, out data);
        #endregion

        #region Com.IDataObject.Interface
        public HRESULT DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) => _nativeDataObject.DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
        public HRESULT DUnadvise(uint dwConnection) => _nativeDataObject.DUnadvise(dwConnection);
        public HRESULT EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) => _nativeDataObject.EnumDAdvise(ppenumAdvise);
        public HRESULT EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) => _nativeDataObject.EnumFormatEtc(dwDirection, ppenumFormatEtc);
        public HRESULT GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) => _nativeDataObject.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
        public HRESULT GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) => _nativeDataObject.GetData(pformatetcIn, pmedium);
        public HRESULT GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) => _nativeDataObject.GetDataHere(pformatetc, pmedium);
        public HRESULT QueryGetData(Com.FORMATETC* pformatetc) => _nativeDataObject.QueryGetData(pformatetc);
        public HRESULT SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) => _nativeDataObject.SetData(pformatetc, pmedium, fRelease);
        #endregion

        #region ComTypes.IDataObject.Interface
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => _runtimeDataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
        public void DUnadvise(int connection) => _runtimeDataObject.DUnadvise(connection);
        public int EnumDAdvise(out IEnumSTATDATA? enumAdvise) => _runtimeDataObject.EnumDAdvise(out enumAdvise);
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => _runtimeDataObject.EnumFormatEtc(direction);
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => _runtimeDataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
        public void GetData(ref FORMATETC format, out STGMEDIUM medium) => _runtimeDataObject.GetData(ref format, out medium);
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => _runtimeDataObject.GetDataHere(ref format, ref medium);
        public int QueryGetData(ref FORMATETC format) => _runtimeDataObject.QueryGetData(ref format);
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => _runtimeDataObject.SetData(ref formatIn, ref medium, release);
        #endregion
    }
}
