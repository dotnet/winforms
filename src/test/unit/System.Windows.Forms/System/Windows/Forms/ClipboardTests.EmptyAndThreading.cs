// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Specialized;
using System.Runtime.ExceptionServices;

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    private const string EmptyClipboardCustomFormat = "WinForms.ClipboardTests.Empty";
    private static readonly TimeSpan s_messageLoopTimeout = TimeSpan.FromSeconds(15);

    private static readonly TextDataFormat[] s_validTextDataFormats =
    [
        TextDataFormat.Text,
        TextDataFormat.UnicodeText,
        TextDataFormat.Rtf,
        TextDataFormat.Html,
        TextDataFormat.CommaSeparatedValue
    ];

    // Verifies that Clear removes every public data view and remains safe when the clipboard is already empty.
    [WinFormsFact]
    public void Clipboard_Clear_PopulatedClipboard_RemovesAllPubliclyReadableData()
    {
        Clipboard.Clear();
        try
        {
            DataObject data = new();
            data.SetData(EmptyClipboardCustomFormat, autoConvert: false, 42);
            data.SetData(DataFormats.UnicodeText, autoConvert: false, "text");
            Clipboard.SetDataObject(data, copy: true);

            Assert.True(Clipboard.ContainsData(EmptyClipboardCustomFormat));
            Assert.True(Clipboard.ContainsText());

            Clipboard.Clear();
            AssertEmptyClipboardViews();

            // Clearing an already empty clipboard is idempotent.
            Clipboard.Clear();
            AssertEmptyClipboardViews();
        }
        finally
        {
            Clipboard.Clear();
        }
    }

    // Verifies that an empty file-drop read returns a new mutable result rather than a shared collection instance.
    [WinFormsFact]
    public void Clipboard_EmptyClipboard_GetFileDropList_ReturnsDistinctEmptyCollections()
    {
        StringCollection first = Clipboard.GetFileDropList();
        StringCollection second = Clipboard.GetFileDropList();

        Assert.Empty(first.Cast<string>());
        Assert.Empty(second.Cast<string>());
        Assert.NotSame(first, second);
    }

    // Verifies that typed readers ignore unrelated clipboard formats and return their documented neutral values.
    [WinFormsFact]
    public void Clipboard_NonMatchingPayload_TypedContainsAndGettersReturnNeutralValues()
    {
        Clipboard.SetData(EmptyClipboardCustomFormat, 42);

        Assert.True(Clipboard.ContainsData(EmptyClipboardCustomFormat));
        Assert.False(Clipboard.ContainsAudio());
        Assert.False(Clipboard.ContainsFileDropList());
        Assert.False(Clipboard.ContainsImage());
        Assert.False(Clipboard.ContainsText());
        Assert.Null(Clipboard.GetAudioStream());
        Assert.Empty(Clipboard.GetFileDropList().Cast<string>());
        Assert.Null(Clipboard.GetImage());
        Assert.Equal(string.Empty, Clipboard.GetText());

        foreach (TextDataFormat format in s_validTextDataFormats)
        {
            Assert.False(Clipboard.ContainsText(format));
            Assert.Equal(string.Empty, Clipboard.GetText(format));
        }
    }

    // Verifies the special MTA behavior where GetDataObject returns null when no WinForms message loop is running.
    [Fact] // x-thread
    public void Clipboard_GetDataObject_MtaWithoutMessageLoop_ReturnsNull()
    {
        Assert.False(Application.MessageLoop);
        Assert.Null(Clipboard.GetDataObject());
    }

    public static TheoryData<Action> MtaReadApisWithoutMessageLoopTheoryData
    {
        get
        {
            TheoryData<Action> data = new()
            {
                () => Assert.False(Clipboard.ContainsAudio()),
                () => Assert.False(Clipboard.ContainsData(EmptyClipboardCustomFormat)),
                () => Assert.False(Clipboard.ContainsFileDropList()),
                () => Assert.False(Clipboard.ContainsImage()),
                () => Assert.False(Clipboard.ContainsText()),
                () => Assert.Null(Clipboard.GetAudioStream()),
                () => Assert.Null(Clipboard.GetData(EmptyClipboardCustomFormat)),
                () => Assert.Empty(Clipboard.GetFileDropList().Cast<string>()),
                () => Assert.Null(Clipboard.GetImage()),
                () => Assert.Equal(string.Empty, Clipboard.GetText())
            };

            foreach (TextDataFormat format in s_validTextDataFormats)
            {
                data.Add(() => Assert.False(Clipboard.ContainsText(format)));
                data.Add(() => Assert.Equal(string.Empty, Clipboard.GetText(format)));
            }

            return data;
        }
    }

    // Verifies that every public read API inherits the neutral-value MTA behavior from GetDataObject.
    [Theory] // x-thread
    [MemberData(nameof(MtaReadApisWithoutMessageLoopTheoryData))]
    public void Clipboard_ReadApis_MtaWithoutMessageLoop_ReturnNeutralValues(Action action)
    {
        Assert.False(Application.MessageLoop);
        action();
    }

    // Verifies that a running WinForms message loop changes unsupported MTA reads from neutral results to exceptions.
    [Fact] // x-thread
    public void Clipboard_ReadApis_MtaWithMessageLoop_ThrowThreadStateException()
    {
        Action[] actions =
        [
            () => Clipboard.GetDataObject(),
            () => Clipboard.ContainsAudio(),
            () => Clipboard.ContainsData(EmptyClipboardCustomFormat),
            () => Clipboard.ContainsFileDropList(),
            () => Clipboard.ContainsImage(),
            () => Clipboard.ContainsText(),
            () => Clipboard.GetAudioStream(),
            () => Clipboard.GetData(EmptyClipboardCustomFormat),
            () => Clipboard.GetFileDropList(),
            () => Clipboard.GetImage(),
            () => Clipboard.GetText()
        ];

        RunMtaMessageLoop(() =>
        {
            foreach (Action action in actions)
            {
                Assert.Throws<ThreadStateException>(action);
            }

            foreach (TextDataFormat format in s_validTextDataFormats)
            {
                Assert.Throws<ThreadStateException>(() => Clipboard.ContainsText(format));
                Assert.Throws<ThreadStateException>(() => Clipboard.GetText(format));
            }
        });
    }

    private static void AssertEmptyClipboardViews()
    {
        Assert.False(Clipboard.ContainsAudio());
        Assert.False(Clipboard.ContainsData(EmptyClipboardCustomFormat));
        Assert.False(Clipboard.ContainsFileDropList());
        Assert.False(Clipboard.ContainsImage());
        Assert.False(Clipboard.ContainsText());

        foreach (TextDataFormat format in s_validTextDataFormats)
        {
            Assert.False(Clipboard.ContainsText(format));
        }

        Assert.Null(Clipboard.GetAudioStream());
        Assert.Null(Clipboard.GetData(EmptyClipboardCustomFormat));
        Assert.Null(Clipboard.GetImage());

        StringCollection fileDropList = Clipboard.GetFileDropList();
        Assert.NotNull(fileDropList);
        Assert.Empty(fileDropList.Cast<string>());

        Assert.Equal(string.Empty, Clipboard.GetText());
        foreach (TextDataFormat format in s_validTextDataFormats)
        {
            Assert.Equal(string.Empty, Clipboard.GetText(format));
        }

        IDataObject? dataObject = Clipboard.GetDataObject();
        Assert.NotNull(dataObject);
        Assert.False(dataObject.GetDataPresent(EmptyClipboardCustomFormat, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false));
    }

    // Exercise a callback only after a real WinForms message loop is active on an MTA thread.
    private static void RunMtaMessageLoop(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        Exception? exception = null;
        ApplicationContext? context = null;
        Control? dispatcher = null;
        ManualResetEventSlim dispatcherReady = new();
        ManualResetEventSlim loopActive = new();

        Thread thread = new(() =>
        {
            try
            {
                using ApplicationContext threadContext = new();
                context = threadContext;
                using Control threadDispatcher = new();
                _ = threadDispatcher.Handle;
                dispatcher = threadDispatcher;
                dispatcherReady.Set();
                using Timer timer = new() { Interval = 1 };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    loopActive.Set();

                    try
                    {
                        Assert.True(Application.MessageLoop);
                        action();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        threadContext.ExitThread();
                    }
                };

                timer.Start();
                Application.Run(threadContext);
            }
            catch (Exception ex)
            {
                exception ??= ex;
                dispatcherReady.Set();
                loopActive.Set();
            }
        })
        {
            IsBackground = true,
            Name = $"{nameof(ClipboardTests)} MTA message loop"
        };
        thread.SetApartmentState(ApartmentState.MTA);
        thread.Start();

        bool started = loopActive.Wait(s_messageLoopTimeout);
        if (!started)
        {
            if (dispatcherReady.Wait(TimeSpan.Zero) && dispatcher is { IsHandleCreated: true })
            {
                dispatcher.BeginInvoke(context!.ExitThread);
            }
        }

        bool stopped = thread.Join(s_messageLoopTimeout);
        if (stopped)
        {
            dispatcherReady.Dispose();
            loopActive.Dispose();
        }

        Assert.True(started, "The MTA WinForms message loop did not become active.");
        Assert.True(stopped, "The MTA WinForms message-loop thread did not terminate.");

        if (exception is not null)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
