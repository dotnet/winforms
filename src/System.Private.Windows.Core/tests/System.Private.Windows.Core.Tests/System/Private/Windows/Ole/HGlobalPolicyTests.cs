// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Memory;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Private.Windows.Ole;

public unsafe class HGlobalPolicyTests
{
    [Fact]
    public void GetData_SerializationVeto_DoesNotInvokeSerializer()
    {
        RecordingSerializer.WriteCount = 0;
        var dataObject = Composition<HGlobalFirstOleServices, RecordingSerializer, TestFormat>.Create();
        dataObject.SetData("HGlobalPolicy.SerializerProbe", 42);
        FORMATETC formatEtc = CreateFormat("HGlobalPolicy.SerializerProbe", TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;
        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.DV_E_TYMED);
            RecordingSerializer.WriteCount.Should().Be(0);
            medium.hGlobal.IsNull.Should().BeTrue();
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    [Theory]
    [InlineData(DataFormatNames.Emf)]
    [InlineData(DataFormatNames.BinaryFormatMetafile)]
    [InlineData("HGlobalPolicy.Custom")]
    public void GetData_SerializationVeto_RejectsSerializableObjectWithoutErrorPayload(string format)
    {
        TestDataObject<HGlobalFirstOleServices> dataObject = new(format, 42);
        FORMATETC formatEtc = CreateFormat(format, TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;
        try
        {
            dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.S_OK);
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.DV_E_TYMED);
            medium.hGlobal.IsNull.Should().BeTrue();
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    [Fact]
    public void GetDataHere_SerializationVeto_PreservesCallerHGlobal()
    {
        TestDataObject<HGlobalFirstOleServices> dataObject = new("HGlobalPolicy.Custom", 42);
        FORMATETC formatEtc = CreateFormat("HGlobalPolicy.Custom", TYMED.TYMED_HGLOBAL);
        byte[] sentinel = Enumerable.Repeat((byte)0xA5, 256).ToArray();
        HGLOBAL callerStorage = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, (uint)sentinel.Length);
        callerStorage.IsNull.Should().BeFalse();
        STGMEDIUM medium = new() { tymed = TYMED.TYMED_HGLOBAL, hGlobal = callerStorage };
        try
        {
            void* pointer = PInvokeCore.GlobalLock(callerStorage);
            ((nint)pointer).Should().NotBe(0);
            try
            {
                sentinel.CopyTo(new Span<byte>(pointer, sentinel.Length));
            }
            finally
            {
                PInvokeCore.GlobalUnlock(callerStorage);
            }

            dataObject.GetDataHere(&formatEtc, &medium).Should().Be(HRESULT.DV_E_TYMED);
            medium.tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            medium.hGlobal.Should().Be(callerStorage);
            ReadHGlobal(callerStorage, sentinel.Length).Should().Equal(sentinel);
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    [Theory]
    [InlineData(ComTypes.TYMED.TYMED_ENHMF, true)]
    [InlineData(ComTypes.TYMED.TYMED_ENHMF | ComTypes.TYMED.TYMED_ISTREAM, true)]
    [InlineData(ComTypes.TYMED.TYMED_ENHMF | ComTypes.TYMED.TYMED_HGLOBAL, false)]
    [InlineData(ComTypes.TYMED.TYMED_ENHMF | ComTypes.TYMED.TYMED_ISTREAM | ComTypes.TYMED.TYMED_HGLOBAL, false)]
    public void GetData_PreferHGlobalOverEnhMf_NativeRenderingOnlyWhenHGlobalNotOffered(ComTypes.TYMED requestedTymed, bool native)
    {
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<HGlobalFirstOleServices> dataObject = new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateFormat(DataFormatNames.Emf, (TYMED)requestedTymed);
        STGMEDIUM medium = default;
        try
        {
            dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.S_OK);
            dataObject.GetData(&formatEtc, &medium).Should().Be(native ? HRESULT.S_OK : HRESULT.DV_E_TYMED);
            if (native)
            {
                medium.tymed.Should().Be(TYMED.TYMED_ENHMF);
                EmfNativeMethods.IsValid((nint)medium.hGlobal).Should().BeTrue();
            }
            else
            {
                medium.hGlobal.IsNull.Should().BeTrue();
            }
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }

        source.CreatePayload().RecordCount.Should().BeGreaterThan(2);
    }

    [Theory]
    [InlineData(DataFormatNames.Emf, ComTypes.TYMED.TYMED_HGLOBAL)]
    [InlineData(DataFormatNames.Emf, ComTypes.TYMED.TYMED_ENHMF | ComTypes.TYMED.TYMED_HGLOBAL)]
    [InlineData("HGlobalPolicy.Custom", ComTypes.TYMED.TYMED_HGLOBAL)]
    public void GetData_Stream_BypassesSerializationVetoAndCopiesWholeStream(string format, ComTypes.TYMED requestedTymed)
    {
        byte[] bytes = [1, 2, 3, 4, 5, 6, 7];
        using MemoryStream source = new(bytes, writable: false) { Position = 3 };
        TestDataObject<HGlobalFirstOleServices> dataObject = new(format, source);
        FORMATETC formatEtc = CreateFormat(format, (TYMED)requestedTymed);
        STGMEDIUM medium = default;
        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);
            medium.tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            ReadHGlobal(medium.hGlobal, bytes.Length).Should().Equal(bytes);
            source.Position.Should().Be(source.Length);
            source.CanRead.Should().BeTrue();
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    [Theory]
    [InlineData(DataFormatNames.Text, false)]
    [InlineData(DataFormatNames.UnicodeText, true)]
    public void GetData_Text_BypassesSerializationVetoAndConvertsObject(string format, bool unicode)
    {
        TestDataObject<HGlobalFirstOleServices> dataObject = new(format, 42);
        FORMATETC formatEtc = CreateFormat(format, TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;
        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);
            medium.tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            void* pointer = PInvokeCore.GlobalLock(medium.hGlobal);
            ((nint)pointer).Should().NotBe(0);
            try
            {
                string? result = unicode ? Marshal.PtrToStringUni((nint)pointer) : Marshal.PtrToStringAnsi((nint)pointer);
                result.Should().Be("42");
            }
            finally
            {
                PInvokeCore.GlobalUnlock(medium.hGlobal);
            }
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    [Theory]
    [InlineData(DataFormatNames.Emf)]
    [InlineData(DataFormatNames.BinaryFormatMetafile)]
    [InlineData("HGlobalPolicy.Custom")]
    public void GetData_DefaultPolicy_StillSerializesObjects(string format)
    {
        TestDataObject<MockOleServices<HGlobalPolicyTests>> dataObject = new(format, 42);
        FORMATETC formatEtc = CreateFormat(format, TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;
        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);
            medium.tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            ReadHGlobal(medium.hGlobal, 16).Should().Equal(
                new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray());

            // Read through the native adapter, not the original managed data store.
            TestDataObject<MockOleServices<HGlobalPolicyTests>> reader =
                new(ComHelpers.GetComPointer<IDataObject>(dataObject));
            reader.GetData(format, autoConvert: false).Should().Be(42);
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }
    }

    private static FORMATETC CreateFormat(string format, TYMED tymed) => new()
    {
        cfFormat = (ushort)DataFormatsCore<TestFormat>.GetOrAddFormat(format).Id,
        dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
        lindex = -1,
        tymed = (uint)tymed
    };

    private static byte[] ReadHGlobal(HGLOBAL handle, int count)
    {
        void* pointer = PInvokeCore.GlobalLock(handle);
        ((nint)pointer).Should().NotBe(0);
        try
        {
            return new ReadOnlySpan<byte>(pointer, count).ToArray();
        }
        finally
        {
            PInvokeCore.GlobalUnlock(handle);
        }
    }

    private static void ReleaseIfOwned(ref STGMEDIUM medium)
    {
        if (!medium.hGlobal.IsNull)
        {
            PInvokeCore.ReleaseStgMedium(ref medium);
            medium = default;
        }
    }

    // Only the probe test uses this serializer; its counter cannot interfere with other test classes.
    private sealed class RecordingSerializer : INrbfSerializer
    {
        internal static int WriteCount { get; set; }

#if !NET
        public RecordingSerializer() { }
#endif

#if NET
        public static bool TryWriteObject(Stream stream, object value)
#else
        public bool TryWriteObject(Stream stream, object value)
#endif
        {
            WriteCount++;
            return CoreNrbfSerializer.TryWriteObject(stream, value);
        }

#if NET
        public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
            CoreNrbfSerializer.TryGetObject(record, out value);

        public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type) =>
            CoreNrbfSerializer.TryBindToType(typeName, out type);

        public static bool IsFullySupportedType(Type type) => CoreNrbfSerializer.IsFullySupportedType(type);
#else
        public bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
            CoreNrbfSerializer.TryGetObject(record, out value);

        public bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type) =>
            CoreNrbfSerializer.TryBindToType(typeName, out type);

        public bool IsFullySupportedType(Type type) => CoreNrbfSerializer.IsFullySupportedType(type);
#endif
    }

    // No mutable static policy state: existing EmfTestOleServices and MockOleServices keep their defaults.
    private sealed class HGlobalFirstOleServices : IOleServices
    {
#if NET
        public static bool PreferHGlobalOverEnhMf => true;
        // Reject all generic objects to verify that raw-stream and text rendering bypass the serialization veto.
        public static bool AllowHGlobalSerialization(string format, object data) => false;
        public static void EnsureThreadState() { }
        public static bool AllowTypeWithoutResolver<T>() => true;
        public static bool IsValidTypeForFormat(Type type, string format) => true;
        public static void ValidateDataStoreData(ref string format, bool autoConvert, object? data) { }
        public static bool IsNativeTymedSupported(string format, TYMED tymed) =>
            format == DataFormatNames.Emf && tymed.HasFlag(TYMED.TYMED_ENHMF);
#else
        public HGlobalFirstOleServices() { }

        bool IOleServices.PreferHGlobalOverEnhMf => true;
        bool IOleServices.AllowHGlobalSerialization(string format, object data) => false;
        void IOleServices.AddMappedFormats(string format, ICollection<string> formats) { }
        void IOleServices.EnsureThreadState() { }
        bool IOleServices.AllowTypeWithoutResolver<T>() => true;
        bool IOleServices.IsValidTypeForFormat(Type type, string format) => true;
        void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) { }
        bool IOleServices.IsNativeTymedSupported(string format, TYMED tymed) =>
            format == DataFormatNames.Emf && tymed.HasFlag(TYMED.TYMED_ENHMF);
        HRESULT IOleServices.OleIsCurrentClipboard(IDataObject* dataObject) => HRESULT.E_NOTIMPL;
#endif

#if NET
        public static HRESULT GetDataHere(string format, object data, FORMATETC* formatEtc, STGMEDIUM* medium)
#else
        HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* formatEtc, STGMEDIUM* medium)
#endif
        {
            if (format != DataFormatNames.Emf || data is not EmfPayload || medium->tymed != TYMED.TYMED_ENHMF)
            {
                return HRESULT.DV_E_TYMED;
            }

            medium->hGlobal = (HGLOBAL)EmfNativeMethods.CreateDeterministicEnhMetafile();
            return HRESULT.S_OK;
        }

#if NET
        public static bool TryGetObjectFromDataObject<T>(
#else
        bool IOleServices.TryGetObjectFromDataObject<T>(
#endif
            IDataObject* dataObject,
            string format,
            [NotNullWhen(true)] out T data)
        {
            data = default!;
            return false;
        }

#if NET
        public static IComVisibleDataObject CreateDataObject() => new TestDataObject<HGlobalFirstOleServices>();
#else
        IComVisibleDataObject IOleServices.CreateDataObject() => new TestDataObject<HGlobalFirstOleServices>();
#endif

#if NET
        public static HRESULT OleFlushClipboard() => HRESULT.E_NOTIMPL;
#else
        HRESULT IOleServices.OleFlushClipboard() => HRESULT.E_NOTIMPL;
#endif

#if NET
        public static HRESULT OleGetClipboard(IDataObject** dataObject) => HRESULT.E_NOTIMPL;
#else
        HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject) => HRESULT.E_NOTIMPL;
#endif

#if NET
        public static HRESULT OleSetClipboard(IDataObject* dataObject) => HRESULT.E_NOTIMPL;
#else
        HRESULT IOleServices.OleSetClipboard(IDataObject* dataObject) => HRESULT.E_NOTIMPL;
#endif
    }
}
