// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using ClipboardCore = System.Private.Windows.Ole.ClipboardCore<System.Private.Windows.Ole.ClipboardTextFallbackTests.TextFallbackOleServices>;
using ClipboardScope = System.Private.Windows.Ole.ClipboardScope<System.Private.Windows.Ole.ClipboardTextFallbackTests.TextFallbackOleServices>;
using Composition = System.Private.Windows.Ole.Composition<System.Private.Windows.Ole.ClipboardTextFallbackTests.TextFallbackOleServices, System.Private.Windows.Nrbf.CoreNrbfSerializer, System.Private.Windows.Ole.TestFormat>;
using DataObject = System.Private.Windows.Ole.TestDataObject<System.Private.Windows.Ole.ClipboardTextFallbackTests.TextFallbackOleServices>;

namespace System.Private.Windows.Ole;

public unsafe class ClipboardTextFallbackTests
{
#if NETFRAMEWORK
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void GetDataObject_FrameworkManagedSource_UsesOptInPolicy(bool enabled, bool legacySource)
    {
        // The Framework mock proxy has its own managed identity. Return an original CCW here to exercise
        // built-in COM recovery and the instance factory, rather than the ordinary native-only reader.
        using ClipboardScope<FrameworkSourceOleServices> scope = new();
        TestDataObject<FrameworkSourceOleServices> source = new(DataFormatNames.Text, autoConvert: true, "test string");
        object payload = new();
        source.SetData("CustomPayload", autoConvert: false, payload);
        ITestDataObject managedSource = legacySource ? new LegacyDataObject(source) : source;
        FrameworkSourceOleServices.Source = legacySource ? new(managedSource) : source;
        FrameworkSourceOleServices.Enabled = enabled;

        try
        {
            ClipboardCore<FrameworkSourceOleServices>.GetDataObject<TestDataObject<FrameworkSourceOleServices>, ITestDataObject>(
                out ITestDataObject? result).Should().Be(HRESULT.S_OK);
            if (enabled)
            {
                result.Should().NotBeSameAs(managedSource);
                result!.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().Be("test string");
                IDataObjectInternal typed = result.Should().BeAssignableTo<IDataObjectInternal>().Subject;
                typed.TryGetData(DataFormatNames.UnicodeText, autoConvert: false, out string? text).Should().BeTrue();
                text.Should().Be("test string");
            }
            else
            {
                result.Should().BeSameAs(managedSource);
                result!.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().BeNull();
            }

            result.GetData("CustomPayload", autoConvert: false).Should().BeSameAs(payload);
            result.GetFormats(autoConvert: false).Should().Equal(managedSource.GetFormats(autoConvert: false));
        }
        finally
        {
            FrameworkSourceOleServices.Source = null;
            FrameworkSourceOleServices.Enabled = false;
        }
    }
#endif

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetDataObject_OptedIn_RetainsCapturedTextAfterClipboardReplacement(bool legacySource)
    {
        // The mock proxy exposes managed conversion through COM, emulating native text synthesis.
        // Explicit managed reads cannot convert, so both typed and legacy sources must use the captured reader.
        using ClipboardScope scope = new();
        DataObject source = new(DataFormatNames.Text, autoConvert: true, "test string");
        DataObject published = legacySource ? new(new LegacyDataObject(source)) : source;

        // The mock disposes its original pointer on replacement; keep it alive for the retained-reader check.
        using ComScope<IDataObject> original = ComHelpers.GetComScope<IDataObject>(published);
        ClipboardCore.SetData(published, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out ITestDataObject? result).Should().Be(HRESULT.S_OK);
        result.Should().NotBeSameAs(published);
        source.GetDataPresent(DataFormatNames.UnicodeText, autoConvert: false).Should().BeFalse();

        IDataObjectInternal typed = result.Should().BeAssignableTo<IDataObjectInternal>().Subject;
        result!.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().Be("test string");
        typed.TryGetData(DataFormatNames.UnicodeText, autoConvert: false, out string? text).Should().BeTrue();
        text.Should().Be("test string");
        typed.TryGetData(DataFormatNames.UnicodeText, static _ => null, autoConvert: false, out text).Should().BeTrue();
        text.Should().Be("test string");

        DataObject replacement = new("Replacement", new object());
        ClipboardCore.SetData(replacement, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out ITestDataObject? current).Should().Be(HRESULT.S_OK);
        current!.GetDataPresent(DataFormatNames.UnicodeText, autoConvert: false).Should().BeFalse();
        result.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().Be("test string");
        typed.TryGetData(DataFormatNames.UnicodeText, autoConvert: false, out text).Should().BeTrue();
        text.Should().Be("test string");
    }

    [Theory]
    [InlineData("Text", true)]
    [InlineData("text", false)]
    [InlineData("TEXT", false)]
    [InlineData("UnicodeText", true)]
    [InlineData("unicodetext", false)]
    [InlineData("UNICODETEXT", false)]
    [InlineData("OEMText", true)]
    [InlineData("oemtext", false)]
    [InlineData("OEMTEXT", false)]
    public void WithManagedDataObject_TextFallback_UsesCaseSensitiveNames(string format, bool useTextFallback)
    {
        // A source with no text forces every overload onto the native reader.
        // Only exact .NET 9 text-format spellings may enable the additional native text fallback.
        Composition view = Compose(new DataObject());
        string? expected = useTextFallback ? "test string" : null;
        view.GetDataPresent(format).Should().Be(useTextFallback);
        view.GetDataPresent(format, autoConvert: false).Should().Be(useTextFallback);
        view.GetData(format).Should().Be(expected);
        view.GetData(format, autoConvert: false).Should().Be(expected);
        view.TryGetData(format, out string? text).Should().Be(useTextFallback);
        text.Should().Be(expected);
        view.TryGetData(format, autoConvert: false, out text).Should().Be(useTextFallback);
        text.Should().Be(expected);
        view.TryGetData(format, static _ => null, autoConvert: false, out text).Should().Be(useTextFallback);
        text.Should().Be(expected);
    }

    [Fact]
    public void WithManagedDataObject_PreservesManagedPayloadEnumerationAndWrites()
    {
        // Text fallback must not round-trip custom objects through COM, expand enumeration, or redirect writes.
        object payload = new();
        DataObject source = new(DataFormatNames.Text, autoConvert: false, "test string");
        source.SetData("CustomPayload", autoConvert: false, payload);
        Composition view = Compose(source);

        view.GetData("CustomPayload").Should().BeSameAs(payload);
        view.GetData("CustomPayload", autoConvert: false).Should().BeSameAs(payload);
        view.TryGetData("CustomPayload", autoConvert: false, out object? typedPayload).Should().BeTrue();
        typedPayload.Should().BeSameAs(payload);
        view.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().Be("test string");
        view.GetFormats().Should().Equal(source.GetFormats());
        view.GetFormats(autoConvert: false).Should().Equal(DataFormatNames.Text, "CustomPayload");

        view.SetData("AddedPayload", autoConvert: false, payload);
        source.GetData("AddedPayload", autoConvert: false).Should().BeSameAs(payload);
        view.SetData("OtherPayload", payload);
        source.GetData("OtherPayload").Should().BeSameAs(payload);
        view.GetFormats(autoConvert: false).Should().Equal(source.GetFormats(autoConvert: false));

        view.GetDataPresent(DataFormatNames.Rtf).Should().BeFalse();
        view.GetData(DataFormatNames.Rtf).Should().BeNull();
    }

    [Fact]
    public void GetDataObject_UsesAutoConvertOverloads()
    {
        // Like .NET 9, clipboard wrappers forward single-argument calls to the autoConvert overloads.
        // A source's independent single-argument overrides must not change that wrapper behavior.
        using ClipboardScope scope = new();
        SingleArgumentDataObject source = new();
        source.GetData("CustomPayload").Should().BeSameAs(source.Payload);
        source.GetDataPresent("CustomPayload").Should().BeTrue();
        Composition view = Compose(source);
        view.GetData("CustomPayload").Should().BeNull();
        view.GetDataPresent("CustomPayload").Should().BeFalse();
        view.GetData("CustomPayload", autoConvert: true).Should().BeNull();
        view.GetDataPresent("CustomPayload", autoConvert: true).Should().BeFalse();

        ClipboardCore.SetData(source, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out ITestDataObject? result).Should().Be(HRESULT.S_OK);
        result.Should().NotBeSameAs(source);
        result!.GetData("CustomPayload").Should().BeNull();
        result.GetDataPresent("CustomPayload").Should().BeFalse();
        result.GetData("CustomPayload", autoConvert: true).Should().BeNull();
        result.GetDataPresent("CustomPayload", autoConvert: true).Should().BeFalse();
    }

    [Fact]
    public void WithManagedDataObject_LegacySource_TypedReadsAreLimitedToPlainText()
    {
        // Legacy sources use typed native text decoding; custom formats must not gain unrestricted typed reads.
        DataObject source = new(DataFormatNames.Text, autoConvert: false, "test string");
        source.SetData("CustomPayload", new object());
        Composition view = Compose(new TestDataObjectAdapter(new LegacyDataObject(source)));
        view.TryGetData(DataFormatNames.Text, autoConvert: false, out string? text).Should().BeTrue();
        text.Should().Be("test string");
        view.TryGetData(DataFormatNames.UnicodeText, out text).Should().BeTrue();
        text.Should().Be("test string");
        view.TryGetData(DataFormatNames.UnicodeText, static _ => null, autoConvert: false, out text).Should().BeTrue();
        text.Should().Be("test string");
        view.TryGetData(DataFormatNames.UnicodeText, autoConvert: false, out int number).Should().BeFalse();
        number.Should().Be(0);
        Action customRead = () => view.TryGetData("CustomPayload", out object? _);
        customRead.Should().Throw<NotSupportedException>();
        Action mixedCaseRead = () => view.TryGetData("unicodetext", out string? _);
        mixedCaseRead.Should().Throw<NotSupportedException>();

        // Without clipboard composition, legacy adapters must keep their existing typed-API exception.
        DataObject ordinary = new(new LegacyDataObject(source));
        Action ordinaryRead = () => ordinary.TryGetData(DataFormatNames.Text, out string? _);
        ordinaryRead.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData(DataFormatNames.Text)]
    [InlineData(DataFormatNames.UnicodeText)]
    [InlineData(DataFormatNames.OemText)]
    public void WithManagedDataObject_LegacySource_TypedReadsDoNotCallLegacyGetData(string format)
    {
        // A legacy getter can deserialize before returning, so typed reads must never call it directly.
        // Keep the native source separate to verify canonical typed decoding without invoking the managed getter.
        Composition view = Compose(new TestDataObjectAdapter(new LegacyDataObject(new DataObject(), throwOnGetData: true)));
        view.TryGetData(format, autoConvert: false, out string? text).Should().BeTrue();
        text.Should().Be("test string");
        view.TryGetData(format, out text).Should().BeTrue();
        text.Should().Be("test string");
        view.TryGetData(format, static _ => null, autoConvert: false, out text).Should().BeTrue();
        text.Should().Be("test string");
    }

    private static Composition Compose(IDataObjectInternal managedData)
    {
        DataObject nativeSource = new(DataFormatNames.Text, autoConvert: true, "test string");
        // Managed text mapping omits OEMText; publish it explicitly to emulate Windows' synthesized formats.
        nativeSource.SetData(DataFormatNames.OemText, autoConvert: false, "test string");
        using ComScope<IDataObject> native = ComHelpers.GetComScope<IDataObject>(nativeSource);
        return Composition.Create(native).WithManagedDataObject(managedData);
    }

    internal class TextFallbackOleServices : MockOleServices<ClipboardTextFallbackTests>, IOleServices
    {
#if NET
        static bool IOleServices.UseNativeClipboardTextFallback => true;
#else
        public override bool UseNativeClipboardTextFallback => true;
#endif
    }

#if NETFRAMEWORK
    internal class FrameworkSourceOleServices : MockOleServices<ClipboardTextFallbackTests>, IOleServices
    {
        internal static TestDataObject<FrameworkSourceOleServices>? Source { get; set; }
        internal static bool Enabled { get; set; }

        public override bool UseNativeClipboardTextFallback => Enabled;

        HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject)
        {
            if (dataObject is null)
            {
                return HRESULT.E_POINTER;
            }

            *dataObject = Source is { } source ? ComHelpers.GetComPointer<IDataObject>(source) : null;
            return *dataObject is null ? HRESULT.CLIPBRD_E_BAD_DATA : HRESULT.S_OK;
        }
    }
#endif

    private class SingleArgumentDataObject : DataObject
    {
        internal object Payload { get; } = new();

        internal SingleArgumentDataObject() : base(DataFormatNames.Text, autoConvert: true, "test string")
        {
        }

        public override object? GetData(string format) => format == "CustomPayload" ? Payload : base.GetData(format);
        public override bool GetDataPresent(string format) => format == "CustomPayload" || base.GetDataPresent(format);
    }

    private class LegacyDataObject(ITestDataObject data, bool throwOnGetData = false) : ITestDataObject
    {
        public object? GetData(string format, bool autoConvert) => throwOnGetData
            ? throw new InvalidOperationException("Typed retrieval must not call legacy GetData.")
            : data.GetData(format, autoConvert);
        public object? GetData(string format) => throwOnGetData
            ? throw new InvalidOperationException("Typed retrieval must not call legacy GetData.")
            : data.GetData(format);
        public object? GetData(Type format) => throwOnGetData
            ? throw new InvalidOperationException("Typed retrieval must not call legacy GetData.")
            : data.GetData(format);
        public bool GetDataPresent(string format, bool autoConvert) => data.GetDataPresent(format, autoConvert);
        public bool GetDataPresent(string format) => data.GetDataPresent(format);
        public bool GetDataPresent(Type format) => data.GetDataPresent(format);
        public string[] GetFormats(bool autoConvert) => data.GetFormats(autoConvert);
        public string[] GetFormats() => data.GetFormats();
        public void SetData(string format, bool autoConvert, object? value) => data.SetData(format, autoConvert, value);
        public void SetData(string format, object? value) => data.SetData(format, value);
        public void SetData(Type format, object? value) => data.SetData(format, value);
        public void SetData(object? value) => data.SetData(value);
    }
}
