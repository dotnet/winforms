﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
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

        private bool _clientAreaAnimation;
        private DenyExecutionSynchronizationContext? _denyExecutionSynchronizationContext;
        private JoinableTaskCollection _joinableTaskCollection = null!;

        protected ControlTestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;

            Application.EnableVisualStyles();

            // Disable animations for maximum test performance
            bool disabled = false;
            Assert.True(PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETCLIENTAREAANIMATION, ref _clientAreaAnimation));
            Assert.True(PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION, ref disabled, SPIF_SENDCHANGE));
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
            if (_denyExecutionSynchronizationContext != null)
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
            var centerOfRect = new Point(rect.Left, rect.Top) + new Size(rect.Width / 2, rect.Height / 2);
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
