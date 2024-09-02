// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    internal unsafe partial class Composition : IDataObject, Com.IDataObject.Interface, ComTypes.IDataObject
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
        // This field, using the default BinaryFormatter switch, is used to control trim warnings related to using BinaryFormatter in WinForms trimming.
        // The trimmer will generate a warning when set to true and will not generate a warning when set to false.
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

        /// <summary>
        ///  We are restricting binary serialization and deserialization of formats that represent strings, bitmaps or OLE types.
        /// </summary>
        /// <param name="format">format name</param>
        /// <returns><see langword="true" /> - serialize only safe types, strings or bitmaps.</returns>
        private static bool RestrictDeserializationToSafeTypes(string format) =>
            format is DataFormats.StringConstant
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

        #region IDataObject
        object? IDataObject.GetData(string format, bool autoConvert) => _winFormsDataObject.GetData(format, autoConvert);
        object? IDataObject.GetData(string format) => _winFormsDataObject.GetData(format);
        object? IDataObject.GetData(Type format) => _winFormsDataObject.GetData(format);
        bool IDataObject.GetDataPresent(string format, bool autoConvert) => _winFormsDataObject.GetDataPresent(format, autoConvert);
        bool IDataObject.GetDataPresent(string format) => _winFormsDataObject.GetDataPresent(format);
        bool IDataObject.GetDataPresent(Type format) => _winFormsDataObject.GetDataPresent(format);
        string[] IDataObject.GetFormats(bool autoConvert) => _winFormsDataObject.GetFormats(autoConvert);
        string[] IDataObject.GetFormats() => _winFormsDataObject.GetFormats();
        void IDataObject.SetData(string format, bool autoConvert, object? data) => _winFormsDataObject.SetData(format, autoConvert, data);
        void IDataObject.SetData(string format, object? data) => _winFormsDataObject.SetData(format, data);
        void IDataObject.SetData(Type format, object? data) => _winFormsDataObject.SetData(format, data);
        void IDataObject.SetData(object? data) => _winFormsDataObject.SetData(data);
        #endregion

        #region Com.IDataObject.Interface
        HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) => _nativeDataObject.DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
        HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection) => _nativeDataObject.DUnadvise(dwConnection);
        HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) => _nativeDataObject.EnumDAdvise(ppenumAdvise);
        HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) => _nativeDataObject.EnumFormatEtc(dwDirection, ppenumFormatEtc);
        HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) => _nativeDataObject.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
        HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) => _nativeDataObject.GetData(pformatetcIn, pmedium);
        HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) => _nativeDataObject.GetDataHere(pformatetc, pmedium);
        HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) => _nativeDataObject.QueryGetData(pformatetc);
        HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) => _nativeDataObject.SetData(pformatetc, pmedium, fRelease);
        #endregion

        #region ComTypes.IDataObject
        int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => _runtimeDataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
        void ComTypes.IDataObject.DUnadvise(int connection) => _runtimeDataObject.DUnadvise(connection);
        int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) => _runtimeDataObject.EnumDAdvise(out enumAdvise);
        IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR direction) => _runtimeDataObject.EnumFormatEtc(direction);
        int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => _runtimeDataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
        void ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium) => _runtimeDataObject.GetData(ref format, out medium);
        void ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => _runtimeDataObject.GetDataHere(ref format, ref medium);
        int ComTypes.IDataObject.QueryGetData(ref FORMATETC format) => _runtimeDataObject.QueryGetData(ref format);
        void ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => _runtimeDataObject.SetData(ref formatIn, ref medium, release);
        #endregion
    }
}
