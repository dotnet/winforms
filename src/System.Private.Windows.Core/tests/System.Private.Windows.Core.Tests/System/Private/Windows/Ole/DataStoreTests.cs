// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

#if NET
using System.Text.Json.Serialization;
#endif

namespace System.Private.Windows.Ole;

public class DataStoreTests
{
    [Theory]
    [BoolData]
    public void SetData_Serializable_ValidationContenderPreservesFirstEntry(bool concurrent)
    {
        DataStore<CallbackOleServices> store = new();
        SerializableData first = new();
        SerializableData second = new(format =>
        {
            if (format != DataFormatNames.Serializable)
            {
                return;
            }

            if (concurrent)
            {
                RunOnOtherThread(() => store.SetData(first));
            }
            else
            {
                store.SetData(first);
            }
        });

        store.SetData(second);

        store.GetData(DataFormatNames.Serializable).Should().BeSameAs(first);
        store.GetData(typeof(SerializableData)).Should().BeSameAs(second);
    }

    [Fact]
    public void SetData_ValidationCanWaitForAnotherReader()
    {
        DataStore<CallbackOleServices> store = new();
        store.SetData("existing", 42);
        ValidationData data = new(_ => RunOnOtherThread(() =>
        {
            store.GetData("existing").Should().Be(42);
            store.GetDataPresent("existing", autoConvert: false).Should().BeTrue();
            store.GetFormats().Should().Equal("existing");
        }));

        store.SetData("new", data);

        store.GetData("new").Should().BeSameAs(data);
    }

    [Fact]
    public void SetData_Serializable_ExistingEntrySkipsValidation()
    {
        DataStore<CallbackOleServices> store = new();
        store.SetData(DataFormatNames.Serializable, "existing");
        SerializableData data = new(format => format.Should().NotBe(DataFormatNames.Serializable));

        store.SetData(data);

        store.GetData(DataFormatNames.Serializable).Should().Be("existing");
        store.GetData(typeof(SerializableData)).Should().BeSameAs(data);
    }

    [Fact]
    public void SetData_ValidationExceptionDoesNotPublishEntry()
    {
        DataStore<CallbackOleServices> store = new();
        ValidationData data = new(_ => throw new InvalidOperationException("validation"));

        Action action = () => store.SetData("new", data);

        action.Should().Throw<InvalidOperationException>().WithMessage("validation");
        store.GetFormats().Should().BeEmpty();
    }

    [Fact]
    public void GetFormats_PreservesMappingOrderingAndOverwriteBehavior()
    {
        DataStore<MockOleServices<DataStoreTests>> store = new();
        store.SetData(DataFormatNames.Text, autoConvert: true, "first");
        store.SetData("custom", autoConvert: false, null);
        store.SetData(DataFormatNames.UnicodeText, autoConvert: true, "unicode");
        store.SetData(DataFormatNames.Text, autoConvert: true, "last");

        store.GetFormats(autoConvert: false).Should().Equal(
            DataFormatNames.Text, "custom", DataFormatNames.UnicodeText);
        store.GetFormats().Should().Equal(
            DataFormatNames.Text, DataFormatNames.String, DataFormatNames.UnicodeText, "custom");
        store.GetData(DataFormatNames.Text).Should().Be("last");
        store.GetData("custom").Should().BeNull();
        store.GetDataPresent("custom", autoConvert: false).Should().BeTrue();
        store.GetDataPresent("CUSTOM", autoConvert: false).Should().BeFalse();
        store.GetDataPresent("CUSTOM", autoConvert: true).Should().BeTrue();
        store.TryGetData(DataFormatNames.Text, out string? text).Should().BeTrue();
        text.Should().Be("last");
        store.TryGetData(DataFormatNames.Text, out int _).Should().BeFalse();

        store.SetData(DataFormatNames.Text, autoConvert: false, "unmapped");
        store.SetData(DataFormatNames.UnicodeText, autoConvert: false, "unmapped");
        store.GetFormats().Should().Equal(DataFormatNames.Text, "custom", DataFormatNames.UnicodeText);
        store.TryGetData(DataFormatNames.String, autoConvert: false, out text).Should().BeFalse();
        text.Should().BeNull();
        store.TryGetData(DataFormatNames.String, autoConvert: true, out text).Should().BeTrue();
        text.Should().Be("unmapped");
    }

    [Fact]
    public void ConcurrentReadersAndWriters_PreserveAllCollidingEntries()
    {
        const int writerCount = 4;
        const int entriesPerWriter = 128;
        string[][] formats = Enumerable.Range(0, writerCount)
            .Select(writer => Enumerable.Range(0, entriesPerWriter)
                .Select(index => $"format\0{writer}:{index}").ToArray()).ToArray();

        // The compatibility hash stops at the embedded null; equality still compares the complete string.
        formats.SelectMany(f => f).Select(BackCompatibleStringComparer.Default.GetHashCode)
            .Distinct().Should().ContainSingle();

        for (int iteration = 0; iteration < 8; iteration++)
        {
            DataStore<MockOleServices<DataStoreTests>> store = new();
            using ManualResetEventSlim start = new();
            ConcurrentQueue<Exception> failures = new();
            Thread[] threads = Enumerable.Range(0, writerCount + 1).Select(worker => new Thread(() =>
            {
                try
                {
                    start.Wait();
                    for (int index = 0; index < entriesPerWriter; index++)
                    {
                        if (worker < writerCount)
                        {
                            string format = formats[worker][index];
                            store.SetData(format, autoConvert: false, index);
                            store.GetData(format, autoConvert: false).Should().Be(index);
                            store.TryGetData(format, out int value).Should().BeTrue();
                            value.Should().Be(index);
                            store.GetDataPresent(format, autoConvert: false).Should().BeTrue();
                        }
                        else
                        {
                            string[] snapshot = store.GetFormats(autoConvert: false);
                            snapshot.Should().OnlyHaveUniqueItems().And.NotContainNulls();
                            foreach (string format in snapshot)
                            {
                                store.GetDataPresent(format, autoConvert: false).Should().BeTrue();
                            }

                            store.GetFormats().Should().OnlyHaveUniqueItems().And.NotContainNulls();
                            store.GetDataPresent("missing", autoConvert: true).Should().BeFalse();
                        }
                    }
                }
                catch (Exception exception)
                {
                    failures.Enqueue(exception);
                }
            })
            { IsBackground = true }).ToArray();

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            start.Set();
            foreach (Thread thread in threads)
            {
                thread.Join(TimeSpan.FromSeconds(20)).Should().BeTrue("store operations must terminate");
            }

            failures.Should().BeEmpty();
            store.GetFormats(autoConvert: false).Should().BeEquivalentTo(formats.SelectMany(f => f));
            for (int writer = 0; writer < writerCount; writer++)
            {
                for (int index = 0; index < entriesPerWriter; index++)
                {
                    store.GetData(formats[writer][index], autoConvert: false).Should().Be(index);
                }
            }
        }
    }

    [Fact]
    public void ConcurrentOverwritesAndReaders_PreserveCompleteEntries()
    {
        const int writerCount = 2;
        const int readerCount = 2;
        const string directData = "direct";
        const string fallbackData = "fallback";
        object convertedData = 42;
        string[] definedFormats = [DataFormatNames.String, DataFormatNames.Text];
        string[] mappedFormats = [DataFormatNames.String, DataFormatNames.Text, DataFormatNames.UnicodeText];
        DataStore<MockOleServices<DataStoreTests>> store = new();
        store.SetData(DataFormatNames.String, autoConvert: false, fallbackData);
        store.SetData(DataFormatNames.Text, autoConvert: false, directData);

        Barrier phase = new(writerCount + readerCount);
        CancellationTokenSource cancellation = new();
        ConcurrentQueue<Exception> failures = new();
        Thread[] threads = Enumerable.Range(0, writerCount + readerCount).Select(worker => new Thread(() =>
        {
            try
            {
                for (int batch = 0; batch < 128; batch++)
                {
                    phase.SignalAndWait(TimeSpan.FromSeconds(10), cancellation.Token)
                        .Should().BeTrue("readers and writers must participate in every batch");

                    for (int index = 0; index < 64; index++)
                    {
                        cancellation.Token.ThrowIfCancellationRequested();

                        if (worker < writerCount)
                        {
                            if ((worker + index) % 2 == 0)
                            {
                                store.SetData(DataFormatNames.Text, autoConvert: false, directData);
                            }
                            else
                            {
                                store.SetData(DataFormatNames.Text, autoConvert: true, convertedData);
                            }
                        }
                        else
                        {
                            // Either complete entry yields a string; an int paired with AutoConvert=false does not.
                            store.TryGetData(DataFormatNames.Text, autoConvert: false, out string? text)
                                .Should().BeTrue();
                            text.Should().BeOneOf(directData, fallbackData);
                            store.GetData(DataFormatNames.Text, autoConvert: false)
                                .Should().BeOneOf(directData, convertedData);
                            store.GetDataPresent(DataFormatNames.Text, autoConvert: false).Should().BeTrue();
                            store.GetFormats(autoConvert: false).Should().Equal(definedFormats);

                            string[] snapshot = store.GetFormats();
                            snapshot.Should().Equal(snapshot.Length == definedFormats.Length ? definedFormats : mappedFormats);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                failures.Enqueue(exception);
                cancellation.Cancel();
            }
        })
        { IsBackground = true }).ToArray();

        try
        {
            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join(TimeSpan.FromSeconds(30)).Should().BeTrue("store operations must terminate");
            }
        }
        finally
        {
            cancellation.Cancel();
            bool allStopped = true;
            foreach (Thread thread in threads)
            {
                if (thread.IsAlive && !thread.Join(TimeSpan.FromSeconds(10)))
                {
                    allStopped = false;
                }
            }

            // A stuck background worker may still use these after the timeout is reported.
            if (allStopped)
            {
                phase.Dispose();
                cancellation.Dispose();
            }
        }

        failures.Should().BeEmpty();
        store.SetData(DataFormatNames.Text, autoConvert: true, convertedData);
        store.TryGetData(DataFormatNames.Text, autoConvert: false, out string? convertedText).Should().BeTrue();
        convertedText.Should().Be(fallbackData);
        store.GetFormats().Should().Equal(mappedFormats);

        store.SetData(DataFormatNames.Text, autoConvert: false, directData);
        store.TryGetData(DataFormatNames.Text, autoConvert: false, out string? directText).Should().BeTrue();
        directText.Should().Be(directData);
        store.GetFormats().Should().Equal(definedFormats);
    }

    [Theory]
    [BoolData]
    public unsafe void NativeDragFormats_PreserveUpdatesAndMediumOwnership(bool release)
    {
        TestDataObject<MockOleServices<DataStoreTests>> dataObject = new();
        try
        {
            SetNativeData(PInvokeCore.CFSTR_INDRAGLOOP, [1, 0, 0, 0]);
            SetNativeData(PInvokeCore.CFSTR_DROPDESCRIPTION, [42]);
            object? original = dataObject.GetData(PInvokeCore.CFSTR_DROPDESCRIPTION);
            original.Should().BeOfType<DragDropFormat>();
            SetNativeData(PInvokeCore.CFSTR_DROPDESCRIPTION, [43]);
            dataObject.GetData(PInvokeCore.CFSTR_DROPDESCRIPTION).Should().BeSameAs(original);

            FORMATETC format = CreateFormat(PInvokeCore.CFSTR_DROPDESCRIPTION);
            STGMEDIUM result = default;
            dataObject.GetData(&format, &result).Should().Be(HRESULT.S_OK);
            try
            {
                byte* bytes = (byte*)PInvokeCore.GlobalLock(result.hGlobal);
                try
                {
                    ((nint)bytes).Should().NotBe(0);
                    bytes[0].Should().Be(43);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(result.hGlobal);
                }
            }
            finally
            {
                PInvokeCore.ReleaseStgMedium(ref result);
            }
        }
        finally
        {
            DragDropHelper<MockOleServices<DataStoreTests>, TestFormat>.ReleaseDragDropFormats(dataObject);
        }

        void SetNativeData(string name, byte[] bytes)
        {
            FORMATETC format = CreateFormat(name);
            STGMEDIUM medium = new() { tymed = TYMED.TYMED_HGLOBAL };
            using MemoryStream stream = new(bytes);
            stream.SaveStreamToHGLOBAL(ref medium.hGlobal).Should().Be(HRESULT.S_OK);
            bool transferred = false;
            try
            {
                dataObject.SetData(&format, &medium, release).Should().Be(HRESULT.S_OK);
                transferred = release;
                DragDropFormat stored = dataObject.GetData(name).Should().BeOfType<DragDropFormat>().Subject;
                if (release)
                {
                    stored.Medium.hGlobal.Should().Be(medium.hGlobal);
                }
                else
                {
                    stored.Medium.hGlobal.Should().NotBe(medium.hGlobal);
                }
            }
            finally
            {
                if (!transferred)
                {
                    PInvokeCore.ReleaseStgMedium(ref medium);
                }
            }
        }

        static FORMATETC CreateFormat(string name) => new()
        {
            cfFormat = (ushort)DataFormatsCore<TestFormat>.GetOrAddFormat(name).Id,
            dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = (uint)TYMED.TYMED_HGLOBAL
        };
    }

#if NET
    [Fact]
    public void TryGetData_DeserializationCanWaitForAnotherReader()
    {
        DataStore<MockOleServices<DataStoreTests>> store = new();
        store.SetData("json", new JsonData<JsonCallbackData>() { JsonBytes = "{}"u8.ToArray() });
        JsonCallbackData.Deserialized = () => RunOnOtherThread(() =>
        {
            store.GetDataPresent("json", autoConvert: false).Should().BeTrue();
            store.SetData("reentrant", 42);
            store.GetFormats().Should().Equal("json", "reentrant");
        });

        try
        {
            store.TryGetData("json", out JsonCallbackData? data).Should().BeTrue();
            data.Should().NotBeNull();
            store.GetData("reentrant").Should().Be(42);
        }
        finally
        {
            JsonCallbackData.Deserialized = null;
        }
    }

    public sealed class JsonCallbackData : IJsonOnDeserialized
    {
        internal static Action? Deserialized { get; set; }

        void IJsonOnDeserialized.OnDeserialized() => Deserialized?.Invoke();
    }
#endif

    private static void RunOnOtherThread(Action action)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        { IsBackground = true };
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(10)).Should().BeTrue("validation must not hold the store gate");
        failure.Should().BeNull();
    }

    private class ValidationData(Action<string>? validate = null)
    {
        public void Validate(string format) => validate?.Invoke(format);
    }

    private sealed class SerializableData(Action<string>? validate = null) : ValidationData(validate), ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context) => throw new NotSupportedException();
    }

    private sealed class CallbackOleServices : MockOleServices<DataStoreTests>, IOleServices
    {
#if NET
        static
#endif
        void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data)
        {
            if (data is ValidationData validationData)
            {
                validationData.Validate(format);
            }
        }
    }
}
