// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Threading;
using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests
{
    public abstract class ControlTestBase : IAsyncLifetime, IDisposable
    {
        private const int SPIF_SENDCHANGE = 0x0002;

        private bool clientAreaAnimation;
        private DenyExecutionSynchronizationContext? _denyExecutionSynchronizationContext;
        private JoinableTaskCollection _joinableTaskCollection = null!;

        protected ControlTestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;

            Application.EnableVisualStyles();

            // Disable animations for maximum test performance
            bool disabled = false;
            Assert.True(User32.SystemParametersInfoW(User32.SPI.GETCLIENTAREAANIMATION, ref clientAreaAnimation));
            Assert.True(User32.SystemParametersInfoW(User32.SPI.SETCLIENTAREAANIMATION, ref disabled, SPIF_SENDCHANGE));
        }

        protected ITestOutputHelper TestOutputHelper { get; }

        protected JoinableTaskContext JoinableTaskContext { get; private set; } = null!;

        protected JoinableTaskFactory JoinableTaskFactory { get; private set; } = null!;

        protected SendInput InputSimulator => new SendInput(WaitForIdleAsync);

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
            if (_denyExecutionSynchronizationContext != null)
            {
                SynchronizationContext.SetSynchronizationContext(_denyExecutionSynchronizationContext.UnderlyingContext);
                _denyExecutionSynchronizationContext.ThrowIfSwitchOccurred();
            }
        }

        public virtual void Dispose()
        {
            Assert.True(User32.SystemParametersInfoW(User32.SPI.SETCLIENTAREAANIMATION, ref clientAreaAnimation));
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
            var centerOfRect = new Point(rect.Left, rect.Top) + new Size(rect.Width / 2, rect.Height / 2);
            var centerOnScreen = control.PointToScreen(centerOfRect);
            await MoveMouseAsync(control.FindForm(), centerOnScreen);
        }

        protected async Task MoveMouseAsync(Form window, Point point, bool assertCorrectLocation = true)
        {
            TestOutputHelper.WriteLine($"Moving mouse to ({point.X}, {point.Y}).");
            int horizontalResolution = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
            int verticalResolution = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);
            var virtualPoint = new Point((int)Math.Round((65535.0 / horizontalResolution) * point.X), (int)Math.Round((65535.0 / verticalResolution) * point.Y));
            TestOutputHelper.WriteLine($"Screen resolution of ({horizontalResolution}, {verticalResolution}) translates mouse to ({virtualPoint.X}, {virtualPoint.Y}).");

            await InputSimulator.SendAsync(window, inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPoint.X + 1, virtualPoint.Y + 1));

            // ⚠ The call to GetCursorPos is required for correct behavior.
            if (User32.GetCursorPos(out Point actualPoint).IsFalse())
            {
#pragma warning disable CS8597 // Thrown value may be null.
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
#pragma warning restore CS8597 // Thrown value may be null.
            }

            if (actualPoint.X != point.X || actualPoint.Y != point.Y)
            {
                // Wait and try again
                await Task.Delay(15);
                if (User32.GetCursorPos(out Point _).IsFalse())
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

        protected async Task RunControlPairTestAsync<T>(Func<Form, (T control1, T control2), Task> testDriverAsync)
            where T : Control, new()
        {
            await RunFormAsync(
                () =>
                {
                    var form = new Form();
                    form.TopMost = true;

                    var control1 = new T();
                    var control2 = new T();

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

        internal struct VoidResult
        {
        }
    }
}
