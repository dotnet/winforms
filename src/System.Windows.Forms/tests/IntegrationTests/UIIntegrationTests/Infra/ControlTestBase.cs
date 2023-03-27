// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Threading;
using Windows.Win32.UI.WindowsAndMessaging;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    [UseDefaultXunitCulture]
    public abstract class ControlTestBase : IAsyncLifetime, IDisposable
    {
        private const int SPIF_SENDCHANGE = 0x0002;
        private static string? _logPath;
        private string _testName;

        private bool _clientAreaAnimation;
        private DenyExecutionSynchronizationContext? _denyExecutionSynchronizationContext;
        private JoinableTaskCollection _joinableTaskCollection = null!;
        private static string? s_serverManagerPath;

        private static bool s_started;

        protected ControlTestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;

            _testName = GetTestName();
            _logPath ??= Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("XUNIT_LOGS")!, "Screenshots"));

            Application.EnableVisualStyles();

            // Disable animations for maximum test performance
            bool disabled = false;
            Assert.True(PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETCLIENTAREAANIMATION, ref _clientAreaAnimation));
            Assert.True(PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION, ref disabled, SPIF_SENDCHANGE));

            // Test to capture screenshot at the start
            if (!s_started)
            {
                TestOutputHelper.WriteLine("Taking screenshot at the start");
                s_started = true;
                TrySaveScreenshot("InitialScreenShot.png");

                CloseServerManagerWindow();
                TrySaveScreenshot("AfterServerManagerClosedScreenShot.png");
            }

            string GetTestName()
            {
                var type = testOutputHelper.GetType()!;
                var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic)!;
                var test = (ITest)testMember.GetValue(testOutputHelper)!;
                int index = test.DisplayName.IndexOf("("); // Trim arguments from test name.
                return index == -1 ? test.DisplayName : test.DisplayName[..(index - 1)];
            }
        }

        private void CloseServerManagerWindow()
        {
            try
            {
                if (s_serverManagerPath is not null)
                {
                    return;
                }

                foreach (Process process in Process.GetProcesses())
                {
                    if (!process.ProcessName.Equals("ServerManager", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        s_serverManagerPath = process.MainModule!.FileName;
                        TestOutputHelper.WriteLine($"Server Manager path = {s_serverManagerPath}");
                        process.CloseMainWindow();
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                TestOutputHelper.WriteLine($"Server Manager Window issue {ex}");
                return;
            }

            TestOutputHelper.WriteLine($"Server Manager Window not found");
        }

        protected ITestOutputHelper TestOutputHelper { get; }

        protected JoinableTaskContext JoinableTaskContext { get; private set; } = null!;

        protected JoinableTaskFactory JoinableTaskFactory { get; private set; } = null!;

        protected SendInput InputSimulator => new(WaitForIdleAsync);

        public virtual Task InitializeAsync()
        {
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
            Assert.True(PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION, ref _clientAreaAnimation));
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

        protected Point ToVirtualPoint(Point point)
        {
            Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
            return new Point((int)Math.Round((65535.0 / primaryMonitor.Width) * point.X), (int)Math.Round((65535.0 / primaryMonitor.Height) * point.Y));
        }

        protected async Task MoveMouseAsync(Form window, Point point, bool assertCorrectLocation = true)
        {
            TestOutputHelper.WriteLine($"Moving mouse to ({point.X}, {point.Y}).");
            Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
            var virtualPoint = new Point((int)Math.Round((65535.0 / primaryMonitor.Width) * point.X), (int)Math.Round((65535.0 / primaryMonitor.Height) * point.Y));
            TestOutputHelper.WriteLine($"Screen resolution of ({primaryMonitor.Width}, {primaryMonitor.Height}) translates mouse to ({virtualPoint.X}, {virtualPoint.Y}).");

            await InputSimulator.SendAsync(window, inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPoint.X + 1, virtualPoint.Y + 1));

            // ⚠ The call to GetCursorPos is required for correct behavior.
            if (!PInvoke.GetCursorPos(out Point actualPoint))
            {
#pragma warning disable CS8597 // Thrown value may be null.
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
#pragma warning restore CS8597 // Thrown value may be null.
            }

            if (actualPoint.X != point.X || actualPoint.Y != point.Y)
            {
                // Wait and try again
                await Task.Delay(15);
                if (!PInvoke.GetCursorPos(out Point _))
                {
#pragma warning disable CS8597 // Thrown value may be null.
                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
#pragma warning restore CS8597 // Thrown value may be null.
                }
            }

            if (assertCorrectLocation)
            {
                // Allow for rounding errors (observed in certain scenarios)
                Assert.InRange(point.X, actualPoint.X - 1, actualPoint.X + 1);
                Assert.InRange(point.Y, actualPoint.Y - 1, actualPoint.Y + 1);
            }
        }

        protected async Task RunSingleControlTestAsync<T>(Func<Form, T, Task> testDriverAsync)
            where T : Control, new()
        {
            await RunFormAsync(
                () =>
                {
                    var form = new Form();
                    form.TopMost = true;

                    var control = new T();
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
                    var form = new Form();
                    form.TopMost = true;

                    var control1 = new T1();
                    var control2 = new T2();

                    var tableLayout = new TableLayoutPanel();
                    tableLayout.ColumnCount = 2;
                    tableLayout.RowCount = 1;
                    tableLayout.Controls.Add(control1, 0, 0);
                    tableLayout.Controls.Add(control2, 1, 0);
                    form.Controls.Add(tableLayout);

                    return (form, (control1, control2));
                },
                testDriverAsync);
        }

        protected async Task RunFormAsync<T>(Func<(Form dialog, T control)> createDialog, Func<Form, T, Task> testDriverAsync)
        {
            Form? dialog = null;
            T? control = default;

            TaskCompletionSource<VoidResult> gate = new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            JoinableTask test = JoinableTaskFactory.RunAsync(async () =>
            {
                await gate.Task;
                await JoinableTaskFactory.SwitchToMainThreadAsync();
                await WaitForIdleAsync();
                try
                {
                    await testDriverAsync(dialog!, control!);
                }
                catch
                {
                    TrySaveScreenshot();
                    throw;
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

            Assert.NotNull(dialog);
            Assert.NotNull(control);

            dialog.Activated += (sender, e) => gate.TrySetResult(default);
            dialog.ShowDialog();

            await test.JoinAsync();
        }

        protected async Task RunFormWithoutControlAsync<TForm>(Func<TForm> createForm, Func<TForm, Task> testDriverAsync)
            where TForm : Form
        {
            TForm? dialog = null;

            TaskCompletionSource<VoidResult> gate = new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            JoinableTask test = JoinableTaskFactory.RunAsync(async () =>
            {
                await gate.Task;
                await JoinableTaskFactory.SwitchToMainThreadAsync();
                await WaitForIdleAsync();
                try
                {
                    await testDriverAsync(dialog!);
                }
                catch
                {
                    TrySaveScreenshot();
                    throw;
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

            Assert.NotNull(dialog);

            dialog.Activated += (sender, e) => gate.TrySetResult(default);
            dialog.ShowDialog();

            await test.JoinAsync();
        }

        private void TrySaveScreenshot(string? name = null)
        {
            if (_logPath is null)
            {
                return;
            }

            try
            {
                var bounds = Screen.PrimaryScreen!.Bounds;

                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    // Don't try to take a screenshot if there is no screen.
                    // This may not be an interactive session.
                    return;
                }

                using var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(
                        sourceX: bounds.X,
                        sourceY: bounds.Y,
                        destinationX: 0,
                        destinationY: 0,
                        blockRegionSize: bitmap.Size,
                        copyPixelOperation: CopyPixelOperation.SourceCopy);
                }

                int index = _testName.LastIndexOf('.');
                string screenshot = name ?? $@"{_logPath}\{_testName[(index + 1)..]}_{DateTimeOffset.Now:MMddyyyyhhmmsstt}.png";
                bitmap.Save(screenshot);
                TestOutputHelper.WriteLine($"Screenshot saved at {screenshot}");
            }
            catch (Exception ex)
            {
                TestOutputHelper.WriteLine($"Failed to save screenshot: {ex}.");
            }
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
}
