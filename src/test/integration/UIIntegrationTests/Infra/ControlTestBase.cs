// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.UITests.Input;
using Microsoft.VisualStudio.Threading;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

[UseDefaultXunitCulture]
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public abstract class ControlTestBase : IAsyncLifetime, IDisposable
{
    private const int SPIF_SENDCHANGE = 0x0002;

    private bool _clientAreaAnimation;
    private DenyExecutionSynchronizationContext? _denyExecutionSynchronizationContext;
    private JoinableTaskCollection _joinableTaskCollection = null!;
    private static string s_previousRunTestName = "This is the first test to run.";

    private Point? _mousePosition;

    static ControlTestBase()
    {
        DataCollectionService.InstallFirstChanceExceptionHandler();
    }

    protected ControlTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        DataCollectionService.CurrentTest = GetTest();
        testOutputHelper.WriteLine($" Previous run test: {s_previousRunTestName}");
        s_previousRunTestName = DataCollectionService.CurrentTest.DisplayName;

        Application.EnableVisualStyles();

        // Disable animations for maximum test performance
        bool disabled = false;
        Assert.True(PInvokeCore.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETCLIENTAREAANIMATION, ref _clientAreaAnimation));
        Assert.True(PInvokeCore.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION, ref disabled, SPIF_SENDCHANGE));

        ITest GetTest()
        {
            var type = testOutputHelper.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic)!;
            return (ITest)testMember.GetValue(testOutputHelper)!;
        }
    }

    protected ITestOutputHelper TestOutputHelper { get; }

    protected JoinableTaskContext JoinableTaskContext { get; private set; } = null!;

    protected JoinableTaskFactory JoinableTaskFactory { get; private set; } = null!;

    protected SendInput InputSimulator => new(WaitForIdleAsync);

    public virtual Task InitializeAsync()
    {
        // Verify keyboard and mouse state at the start of the test
        VerifyKeyStates(isStartOfTest: true, TestOutputHelper);

        // Record the mouse position so it can be restored at the end of the test
        _mousePosition = Cursor.Position;

        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            JoinableTaskContext = new JoinableTaskContext();
        }
        else
        {
            _denyExecutionSynchronizationContext = new DenyExecutionSynchronizationContext(SynchronizationContext.Current!);
            JoinableTaskContext = new JoinableTaskContext(_denyExecutionSynchronizationContext.MainThread, _denyExecutionSynchronizationContext);
        }

        _joinableTaskCollection = JoinableTaskContext.CreateCollection();
        JoinableTaskFactory = JoinableTaskContext.CreateFactory(_joinableTaskCollection);
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await _joinableTaskCollection.JoinTillEmptyAsync();

        // Verify keyboard and mouse state at the end of the test
        VerifyKeyStates(isStartOfTest: false, TestOutputHelper);

        // Restore the mouse position
        if (_mousePosition is { } mousePosition)
        {
            Cursor.Position = mousePosition;
        }

        JoinableTaskContext = null!;
        JoinableTaskFactory = null!;
        if (_denyExecutionSynchronizationContext is not null)
        {
            SynchronizationContext.SetSynchronizationContext(_denyExecutionSynchronizationContext.UnderlyingContext);
            _denyExecutionSynchronizationContext.ThrowIfSwitchOccurred();
        }
    }

    public virtual void Dispose()
    {
        Assert.True(PInvokeCore.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION, ref _clientAreaAnimation));
        DataCollectionService.CurrentTest = null;
    }

    private void VerifyKeyStates(bool isStartOfTest, ITestOutputHelper testOutputHelper)
    {
        // Verify that no window has currently captured the cursor
        Assert.Equal(HWND.Null, PInvoke.GetCapture());

        // Verify that no keyboard or mouse keys are in the pressed state at the beginning of the test, since
        // this could interfere with test behavior. This code uses GetAsyncKeyState since GetKeyboardState was
        // not working reliably in local testing.
        foreach (var code in Enum.GetValues<VIRTUAL_KEY>())
        {
            if (code is VIRTUAL_KEY.VK_SCROLL or VIRTUAL_KEY.VK_NUMLOCK)
                continue;

            if (PInvoke.GetAsyncKeyState((int)code) < 0)
            {
                // 😕 VK_LEFT and VK_RIGHT was observed to be pressed at the start of a test even though no test
                // ran before it
                if (isStartOfTest && code is VIRTUAL_KEY.VK_LEFT or VIRTUAL_KEY.VK_RIGHT)
                {
                    testOutputHelper.WriteLine($"Sending WM_KEYUP for '{code}' at the start of the test");
                    new InputSimulator().Keyboard.KeyUp(code);
                }
                else
                {
                    Assert.Fail($"The key with virtual key code '{code}' was unexpectedly pressed at the {(isStartOfTest ? "start" : "end")} of the test.");
                }
            }
        }
    }

    protected async Task WaitForIdleAsync()
    {
        TaskCompletionSource<VoidResult> idleCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Application.Idle += HandleApplicationIdle;
        Application.LeaveThreadModal += HandleApplicationIdle;

        try
        {
            // Queue an event to make sure we don't stall if the application was already idle
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            await Task.Yield();

            if (Application.OpenForms.Count > 0)
            {
                await idleCompletionSource.Task;
            }
        }
        finally
        {
            Application.Idle -= HandleApplicationIdle;
            Application.LeaveThreadModal -= HandleApplicationIdle;
        }

        void HandleApplicationIdle(object? sender, EventArgs e)
        {
            idleCompletionSource.TrySetResult(default);
        }
    }

    protected async Task MoveMouseToControlAsync(Control control)
    {
        var rect = control.DisplayRectangle;
        var centerOfRect = GetCenter(rect);
        var centerOnScreen = control.PointToScreen(centerOfRect);
        await MoveMouseAsync(control.FindForm()!, centerOnScreen);
    }

    protected internal static Point ToVirtualPoint(Point point)
    {
        Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
        return new Point(
            (int)Math.Ceiling((65535.0 / (primaryMonitor.Width - 1)) * point.X),
            (int)Math.Ceiling((65535.0 / (primaryMonitor.Height - 1)) * point.Y));
    }

    protected async Task MoveMouseAsync(Form window, Point point, bool assertCorrectLocation = true)
    {
        TestOutputHelper.WriteLine($"Moving mouse to ({point.X}, {point.Y}).");
        Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
        var virtualPoint = ToVirtualPoint(point);
        TestOutputHelper.WriteLine($"Screen resolution of ({primaryMonitor.Width}, {primaryMonitor.Height}) translates mouse to ({virtualPoint.X}, {virtualPoint.Y}).");

        await InputSimulator.SendAsync(window, inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPoint.X, virtualPoint.Y));

        // ⚠ The call to GetCursorPos is required for correct behavior.
        if (!PInvoke.GetCursorPos(out Point actualPoint))
        {
#pragma warning disable CS8597 // Thrown value may be null.
            throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
#pragma warning restore CS8597
        }

        if (actualPoint.X != point.X || actualPoint.Y != point.Y)
        {
            // Wait and try again
            await Task.Delay(15);
            if (!PInvoke.GetCursorPos(out Point _))
            {
#pragma warning disable CS8597 // Thrown value may be null.
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
#pragma warning restore CS8597
            }
        }

        if (assertCorrectLocation)
        {
            Assert.Equal(point, actualPoint);
        }
    }

    protected async Task RunSingleControlTestAsync<T>(Func<Form, T, Task> testDriverAsync)
        where T : Control, new()
    {
        await RunFormAsync(
            () =>
            {
                Form form = new()
                {
                    TopMost = true
                };

                T control = new();
                form.Controls.Add(control);

                return (form, control);
            },
            testDriverAsync);
    }

    protected async Task RunSingleControlTestAsync<T>(Func<Form, T, Task> testDriverAsync, Func<T> createControl, Func<Form>? createForm = null)
        where T : Control, new()
    {
        await RunFormAsync(
            () =>
            {
                Form form;
                if (createForm is null)
                {
                    form = new();
                }
                else
                {
                    form = createForm();
                }

                form.TopMost = true;

                T control = createControl();
                Assert.NotNull(control);

                form.Controls.Add(control);

                return (form, control);
            },
            testDriverAsync);
    }

    protected async Task RunControlPairTestAsync<T1, T2>(Func<Form, (T1 control1, T2 control2), Task> testDriverAsync)
        where T1 : Control, new()
        where T2 : Control, new()
    {
        await RunFormAsync(
            () =>
            {
                Form form = new()
                {
                    TopMost = true
                };

                var control1 = new T1();
                var control2 = new T2();

                TableLayoutPanel tableLayout = new()
                {
                    ColumnCount = 2,
                    RowCount = 1
                };
                tableLayout.Controls.Add(control1, 0, 0);
                tableLayout.Controls.Add(control2, 1, 0);
                form.Controls.Add(tableLayout);

                return (form, (control1, control2));
            },
            testDriverAsync);
    }

    protected async Task RunFormAsync<T>(Func<(Form dialog, T control)> createDialog, Func<Form, T, Task> testDriverAsync)
    {
        using var screenRecordService = new ScreenRecordService();

        Form? dialog = null;
        T? control = default;

        TaskCompletionSource<VoidResult> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        JoinableTask test = JoinableTaskFactory.RunAsync(async () =>
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await gate.Task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            await WaitForIdleAsync();
            try
            {
                await testDriverAsync(dialog!, control!);
            }
            catch (Exception ex) when (DataCollectionService.LogAndPropagate(ex))
            {
                throw new InvalidOperationException("Not reachable");
            }
            finally
            {
                dialog!.Close();
                dialog.Dispose();
                dialog = null;
            }
        });

        await JoinableTaskFactory.SwitchToMainThreadAsync();
        (dialog, control) = createDialog();
        screenRecordService.RegisterEvents(dialog);

        Assert.NotNull(dialog);
        Assert.NotNull(control);

        dialog.Activated += (sender, e) => gate.TrySetResult(default);
#pragma warning disable VSTHRD103 // Call async methods when in an async method
        dialog.ShowDialog();
#pragma warning restore VSTHRD103

        await test.JoinAsync();
    }

    protected async Task RunFormWithoutControlAsync<TForm>(Func<TForm> createForm, Func<TForm, Task> testDriverAsync)
        where TForm : Form
    {
        using var screenRecordService = new ScreenRecordService();

        TForm? dialog = null;

        TaskCompletionSource<VoidResult> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        JoinableTask test = JoinableTaskFactory.RunAsync(async () =>
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await gate.Task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            await WaitForIdleAsync();
            try
            {
                await testDriverAsync(dialog!);
            }
            catch (Exception ex) when (DataCollectionService.LogAndPropagate(ex))
            {
                throw new InvalidOperationException("Not reachable");
            }
            finally
            {
                dialog!.Close();
                dialog.Dispose();
                dialog = null;
            }
        });

        await JoinableTaskFactory.SwitchToMainThreadAsync();
        dialog = createForm();
        screenRecordService.RegisterEvents(dialog);

        Assert.NotNull(dialog);

        dialog.Activated += (sender, e) => gate.TrySetResult(default);
#pragma warning disable VSTHRD103 // Call async methods when in an async method
        dialog.ShowDialog();
#pragma warning restore VSTHRD103

        await test.JoinAsync();
    }

    internal struct VoidResult
    {
    }

    internal static Point GetCenter(Rectangle cell)
    {
        return new Point(GetMiddle(cell.Right, cell.Left), GetMiddle(cell.Top, cell.Bottom));

        static int GetMiddle(int a, int b) => a + ((b - a) / 2);
    }
}
