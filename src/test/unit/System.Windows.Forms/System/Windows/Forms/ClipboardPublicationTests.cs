// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.DotNet.RemoteExecutor;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Exercises real OLE publication in two processes on a private window station.
/// </summary>
public partial class ClipboardPublicationTests
{
    private const string InitialText = "live \u03A9\r\n\uD83D\uDE80";
    private const string ReplacementText = "replacement \u00E9\r\n\uD83D\uDE80";
    private const string OriginalFormat = "WinForms.ClipboardPublication.Original";
    private const string DesktopName = "Default";
    private const int ProcessTimeoutMilliseconds = 180_000;
    private const int StaTimeoutMilliseconds = 120_000;
    private const int ChildTimeoutMilliseconds = 90_000;
    private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(20);

    /// <summary>
    ///  Named private window stations require an administrator token.
    /// </summary>
    public static bool IsElevated
    {
        get
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    [Theory(Skip = "Private clipboard window stations require elevation.", SkipUnless = nameof(IsElevated))]
    // The live publication remains owned by the replacement publisher until the coordinator clears it.
    [InlineData(false, false)]
    // The live publication is cleared when its publisher exits before coordinator cleanup.
    [InlineData(false, true)]
    // The flushed publication remains available after its publisher exits and is then cleared.
    [InlineData(true, true)]
    public void SetDataObject_CrossProcessReplacement_PreservesPublicationLifetime(bool copy, bool exitBeforeClear)
    {
        // OLE-005: live data crosses process boundaries, replacement transfers ownership,
        // copy=true survives publisher exit, live data is removed on owner exit,
        // and Clear removes the replacement and stale formats.
        using RemoteInvokeHandle process = RemoteExecutor.Invoke(
            RunIsolatedScenario,
            copy.ToString(),
            exitBeforeClear.ToString(),
            new RemoteInvokeOptions { TimeOut = ProcessTimeoutMilliseconds });
    }

    [Fact(Skip = "Private clipboard window stations require elevation.", SkipUnless = nameof(IsElevated))]
    public void SetDataObject_DuringCrossProcessDelayedRead_PreservesQueuedInput()
    {
        // OLE-005: replacement must service a real delayed-rendering request while
        // the reader holds the clipboard, without dispatching or losing queued input.
        using RemoteInvokeHandle process = RemoteExecutor.Invoke(
            RunIsolatedDelayedRead,
            new RemoteInvokeOptions { TimeOut = ProcessTimeoutMilliseconds });
    }

    private static int RunIsolatedScenario(string copyArgument, string exitBeforeClearArgument)
    {
        WindowStation station = new($"WinFormsClipboard_{Guid.NewGuid():N}", create: true);
        RunWithCleanup(
            () => RunSta(station, () => RunScenario(station.Name, bool.Parse(copyArgument), bool.Parse(exitBeforeClearArgument))),
            station.Dispose);
        return RemoteExecutor.SuccessExitCode;
    }

    private static void RunScenario(string stationName, bool copy, bool exitBeforeClear)
    {
        using PublicationWindow window = new();
        DataObject original = new();
        original.SetData(DataFormats.UnicodeText, autoConvert: false, "before mutation");
        original.SetData(OriginalFormat, autoConvert: false, "original only");
        window.QueueMessage();
        Clipboard.SetDataObject(original, copy: false);
        window.Delivered.Should().Be(0, "publication must not dispatch queued input");
        window.VerifyMessagePreserved();
        Clipboard.GetDataObject().Should().BeSameAs(original);
        GetOwnerProcessId().Should().Be((uint)Environment.ProcessId);
        original.SetData(DataFormats.UnicodeText, autoConvert: false, InitialText);

        string pipeName = $"WinFormsClipboard_{Guid.NewGuid():N}";
        using NamedPipeServerStream pipe = new(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        RemoteInvokeHandle publisher = RemoteExecutor.Invoke(
            RunPublisher,
            stationName,
            pipeName,
            copy.ToString(),
            new RemoteInvokeOptions { TimeOut = ChildTimeoutMilliseconds });
        RunWithCleanup(() =>
        {
            WaitWithMessagePump(pipe.WaitForConnectionAsync());
            using StreamReader reader = new(pipe, leaveOpen: true);
            using StreamWriter writer = new(pipe, leaveOpen: true) { AutoFlush = true };
            WaitWithMessagePump(reader.ReadLineAsync()).Should().Be("published");

            if (copy)
            {
                WaitWithMessagePump(publisher.Process.WaitForExitAsync());
                publisher.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
                GetClipboardOwner().Should().Be(nint.Zero, "the flushed publisher has exited");
            }
            else
            {
                publisher.Process.HasExited.Should().BeFalse();
                GetOwnerProcessId().Should().Be((uint)publisher.Process.Id);
            }

            Clipboard.GetDataObject().Should().NotBeSameAs(original);
            Clipboard.ContainsText().Should().BeTrue();
            Clipboard.GetText().Should().Be(ReplacementText);
            Clipboard.ContainsData(OriginalFormat).Should().BeFalse();
            Clipboard.TryGetData(OriginalFormat, out string? stale).Should().BeFalse();
            stale.Should().BeNull();

            if (!copy && exitBeforeClear)
            {
                writer.WriteLine("exit");
                WaitWithMessagePump(publisher.Process.WaitForExitAsync());
                publisher.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
                GetClipboardOwner().Should().Be(nint.Zero);
                VerifyEmptyClipboard();
            }

            Clipboard.Clear();
            VerifyEmptyClipboard();

            if (!copy && !exitBeforeClear)
            {
                writer.WriteLine("exit");
                WaitWithMessagePump(publisher.Process.WaitForExitAsync());
                publisher.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
            }
        }, pipe.Dispose, () => WaitWithMessagePump(publisher.Process.WaitForExitAsync()), publisher.Dispose);
    }

    private static int RunPublisher(string stationName, string pipeName, string copyArgument)
    {
        WindowStation station = new(stationName, create: false);
        RunWithCleanup(() => RunSta(station, () =>
        {
            using NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            pipe.Connect((int)s_timeout.TotalMilliseconds);
            using StreamReader reader = new(pipe, leaveOpen: true);
            using StreamWriter writer = new(pipe, leaveOpen: true) { AutoFlush = true };

            GetOwnerProcessId().Should().NotBe((uint)Environment.ProcessId);
            Clipboard.GetText().Should().Be(InitialText, "the live object was changed after publication");
            Clipboard.TryGetData(OriginalFormat, out string? original).Should().BeTrue();
            original.Should().Be("original only");

            using PublicationWindow window = new();
            DataObject replacement = new();
            replacement.SetData(DataFormats.UnicodeText, autoConvert: false, ReplacementText);
            window.QueueMessage();
            bool copy = bool.Parse(copyArgument);
            Clipboard.SetDataObject(replacement, copy);
            window.Delivered.Should().Be(0, "replacement must not dispatch queued input");
            window.VerifyMessagePreserved();
            writer.WriteLine("published");

            if (!copy)
            {
                string? command = WaitWithMessagePump(reader.ReadLineAsync());
                // EOF also shuts down the worker when a coordinator assertion fails.
                command?.Should().Be("exit");
            }
        }), station.Dispose);
        return RemoteExecutor.SuccessExitCode;
    }

    private static int RunIsolatedDelayedRead()
    {
        WindowStation station = new($"WinFormsClipboard_{Guid.NewGuid():N}", create: true);
        RunWithCleanup(() => RunSta(station, () =>
        {
            DataObject original = new();
            original.SetData(DataFormats.UnicodeText, autoConvert: false, "before mutation");
            original.SetData(OriginalFormat, autoConvert: false, "original only");
            Clipboard.SetDataObject(original, copy: false);
            original.SetData(DataFormats.UnicodeText, autoConvert: false, InitialText);

            string pipeName = $"WinFormsClipboard_{Guid.NewGuid():N}";
            string eventName = $"WinFormsClipboardRead_{Guid.NewGuid():N}";
            using EventWaitHandle beginRead = new(false, EventResetMode.ManualReset, eventName);
            using NamedPipeServerStream pipe = new(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            RemoteInvokeHandle readerProcess = RemoteExecutor.Invoke(
                RunNativeReader,
                station.Name,
                pipeName,
                eventName,
                new RemoteInvokeOptions { TimeOut = ChildTimeoutMilliseconds });
            RunWithCleanup(() =>
            {
                WaitWithMessagePump(pipe.WaitForConnectionAsync());
                using StreamReader reader = new(pipe, leaveOpen: true);
                using StreamWriter writer = new(pipe, leaveOpen: true) { AutoFlush = true };
                WaitWithMessagePump(reader.ReadLineAsync()).Should().Be("locked");

                using PublicationWindow window = new();
                window.QueueMessage();
                DataObject replacement = new();
                replacement.SetData(DataFormats.UnicodeText, autoConvert: false, ReplacementText);
                writer.WriteLine("read");
                // The reader cannot request rendering during the preceding IPC write.
                Check(beginRead.Set());
                Clipboard.SetDataObject(replacement, copy: false, retryTimes: 200, retryDelay: 100);
                window.Delivered.Should().Be(0);
                window.VerifyMessagePreserved();
                WaitWithMessagePump(reader.ReadLineAsync()).Should().Be("read");
                GetOwnerProcessId().Should().Be((uint)Environment.ProcessId);
                Clipboard.GetText().Should().Be(ReplacementText);
                Clipboard.ContainsData(OriginalFormat).Should().BeFalse();
                Clipboard.Clear();
                VerifyEmptyClipboard();
            }, pipe.Dispose, () => WaitWithMessagePump(readerProcess.Process.WaitForExitAsync()), readerProcess.Dispose);
        }), station.Dispose);
        return RemoteExecutor.SuccessExitCode;
    }

    private static int RunNativeReader(string stationName, string pipeName, string eventName)
    {
        WindowStation station = new(stationName, create: false);
        RunWithCleanup(() => RunSta(station, () =>
        {
            using NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            pipe.Connect((int)s_timeout.TotalMilliseconds);
            using StreamReader reader = new(pipe, leaveOpen: true);
            using StreamWriter writer = new(pipe, leaveOpen: true) { AutoFlush = true };
            using EventWaitHandle beginRead = EventWaitHandle.OpenExisting(eventName);
            Check(OpenClipboard(0));
            RunWithCleanup(() =>
            {
                writer.WriteLine("locked");
                string? command = WaitWithMessagePump(reader.ReadLineAsync());
                if (command is null)
                {
                    return;
                }

                command.Should().Be("read");
                beginRead.WaitOne(s_timeout).Should().BeTrue("the coordinator must enter replacement before rendering is requested");
                nint data = GetClipboardData(13); // CF_UNICODETEXT
                Check(data != 0);
                nint buffer = GlobalLock(data);
                Check(buffer != 0);
                RunWithCleanup(
                    () => Marshal.PtrToStringUni(buffer).Should().Be(InitialText),
                    () =>
                    {
                        bool unlocked = GlobalUnlock(data);
                        Check(unlocked || Marshal.GetLastPInvokeError() == 0);
                    });
            }, () => Check(CloseClipboard()));
            writer.WriteLine("read");
        }), station.Dispose);
        return RemoteExecutor.SuccessExitCode;
    }

    private static void VerifyEmptyClipboard()
    {
        Clipboard.ContainsText().Should().BeFalse();
        Clipboard.GetText().Should().BeEmpty();
        Clipboard.ContainsData(OriginalFormat).Should().BeFalse();
        string[]? formats = Clipboard.GetDataObject()?.GetFormats();
        formats.Should().BeNullOrEmpty();
    }

    private static void RunWithCleanup(Action action, params Action[] cleanup)
    {
        List<Exception> failures = [];
        try
        {
            action();
        }
        catch (Exception exception)
        {
            failures.Add(exception);
        }

        foreach (Action release in cleanup)
        {
            try
            {
                release();
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        }

        if (failures.Count == 1)
        {
            ExceptionDispatchInfo.Capture(failures[0]).Throw();
        }
        else if (failures.Count > 1)
        {
            throw new AggregateException(failures);
        }
    }

    private static unsafe void RunSta(WindowStation station, Action action)
    {
        StaInvocation invocation = new(station, action);
        GCHandle context = GCHandle.Alloc(invocation);
        // CLR-created threads initialize COM before their callback. A native thread
        // lets us select the private desktop before explicitly initializing OLE.
        nint thread = CreateThread(0, 0, &RunStaCallback, GCHandle.ToIntPtr(context), 0, out _);
        if (thread == 0)
        {
            int error = Marshal.GetLastPInvokeError();
            context.Free();
            throw new Win32Exception(error);
        }

        station.WorkerRunning = true;
        RunWithCleanup(() =>
        {
            uint wait = WaitForSingleObject(thread, StaTimeoutMilliseconds);
            Check(wait != uint.MaxValue);
            wait.Should().Be(0U, "the isolated STA must finish within its deadline");
            station.WorkerRunning = false;
            invocation.Failure?.Throw();
        }, () => Check(CloseHandle(thread)));
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint RunStaCallback(nint parameter)
    {
        GCHandle context = GCHandle.FromIntPtr(parameter);
        var invocation = (StaInvocation)context.Target!;
        context.Free();
        try
        {
            Check(SetThreadDesktop(invocation.Station.Desktop));
            RunWithCleanup(() =>
            {
                Application.OleRequired().Should().Be(ApartmentState.STA);
                invocation.Action();
            }, Application.ExitThread);
        }
        catch (Exception exception)
        {
            invocation.Failure = ExceptionDispatchInfo.Capture(exception);
        }

        return 0;
    }

    private static void WaitWithMessagePump(Task task)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!task.IsCompleted)
        {
            stopwatch.Elapsed.Should().BeLessThan(s_timeout, "cross-process OLE and IPC must have bounded liveness");
            Application.DoEvents();
            Thread.Sleep(10);
        }

        task.GetAwaiter().GetResult();
    }

    private static T WaitWithMessagePump<T>(Task<T> task)
    {
        WaitWithMessagePump((Task)task);
        return task.GetAwaiter().GetResult();
    }

    private static uint GetOwnerProcessId()
    {
        nint owner = GetClipboardOwner();
        owner.Should().NotBe(nint.Zero);
        GetWindowThreadProcessId(owner, out uint processId).Should().NotBe(0);
        return processId;
    }

    private static void Check(bool result)
    {
        if (!result)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }
    }

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowStationW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    private static partial nint CreateWindowStation(string name, uint flags, uint access, nint security);

    [LibraryImport("user32.dll", EntryPoint = "OpenWindowStationW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    private static partial nint OpenWindowStation(string name, [MarshalAs(UnmanagedType.Bool)] bool inherit, uint access);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetProcessWindowStation(nint station);

    [LibraryImport("user32.dll")]
    private static partial nint GetProcessWindowStation();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseWindowStation(nint station);

    [LibraryImport("user32.dll", EntryPoint = "CreateDesktopW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    private static partial nint CreateDesktop(string name, nint device, nint mode, uint flags, uint access, nint security);

    [LibraryImport("user32.dll", EntryPoint = "OpenDesktopW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    private static partial nint OpenDesktop(string name, uint flags, [MarshalAs(UnmanagedType.Bool)] bool inherit, uint access);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetThreadDesktop(nint desktop);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseDesktop(nint desktop);

    [LibraryImport("user32.dll")]
    private static partial nint GetClipboardOwner();

    [LibraryImport("user32.dll")]
    private static partial uint GetWindowThreadProcessId(nint window, out uint processId);

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PostMessage(nint window, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenClipboard(nint window);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseClipboard();

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial nint GetClipboardData(uint format);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static unsafe partial nint CreateThread(
        nint security,
        nuint stackSize,
        delegate* unmanaged[Stdcall]<nint, uint> callback,
        nint parameter,
        uint flags,
        out uint id);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForSingleObject(nint handle, uint timeout);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint handle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint GlobalLock(nint memory);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalUnlock(nint memory);
}
