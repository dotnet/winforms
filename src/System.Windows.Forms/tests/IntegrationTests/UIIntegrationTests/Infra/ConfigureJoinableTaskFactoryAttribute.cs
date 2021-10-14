﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Microsoft.VisualStudio.Threading;
using Xunit;
using Xunit.Sdk;

namespace System.Windows.Forms.UI.IntegrationTests.Infra
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ConfigureJoinableTaskFactoryAttribute : BeforeAfterTestAttribute
    {
        private DenyExecutionSynchronizationContext? _denyExecutionSynchronizationContext;
        private HangReporter? _hangReporter;
        private ExceptionDispatchInfo? _threadException;

        public override void Before(MethodInfo methodUnderTest)
        {
            Debug.WriteLine(methodUnderTest.Name);

            Application.ThreadException += HandleApplicationThreadException;

            Assert.Null(ThreadHelper.JoinableTaskContext); // "Tests with joinable tasks must not be run in parallel!");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                _denyExecutionSynchronizationContext = new DenyExecutionSynchronizationContext(SynchronizationContext.Current!);
                ThreadHelper.JoinableTaskContext = new JoinableTaskContext(_denyExecutionSynchronizationContext.MainThread, _denyExecutionSynchronizationContext);
                return;
            }

            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());

            // This form is created to obtain a UI synchronization context only.
            using (new Form())
            {
                // Store the shared JoinableTaskContext
                ThreadHelper.JoinableTaskContext = new JoinableTaskContext();
                _hangReporter = new HangReporter(ThreadHelper.JoinableTaskContext);
            }
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Debug.WriteLine(methodUnderTest.Name);

            try
            {
                try
                {
                    // Wait for eventual pending operations triggered by the test.
                    using CancellationTokenSource cts = new(AsyncTestHelper.UnexpectedTimeout);
                    try
                    {
                        // Note that ThreadHelper.JoinableTaskContext.Factory must be used to bypass the default behavior of
                        // ThreadHelper.JoinableTaskFactory since the latter adds new tasks to the collection and would therefore
                        // never complete.
                        ThreadHelper.JoinableTaskContext.Factory.Run(() => ThreadHelper.JoinPendingOperationsAsync(cts.Token));
                    }
                    catch (OperationCanceledException) when (cts.IsCancellationRequested)
                    {
                        if (int.TryParse(Environment.GetEnvironmentVariable("GE_TEST_SLEEP_SECONDS_ON_HANG"), out var sleepSeconds) && sleepSeconds > 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
                        }

                        throw;
                    }
                }
                finally
                {
                    ThreadHelper.JoinableTaskContext = null!;
                    if (_denyExecutionSynchronizationContext is not null)
                    {
                        SynchronizationContext.SetSynchronizationContext(_denyExecutionSynchronizationContext.UnderlyingContext);
                    }
                }

                _denyExecutionSynchronizationContext?.ThrowIfSwitchOccurred();
            }
            catch (Exception ex) when (_threadException is not null)
            {
                StoreThreadException(ex);
            }
            finally
            {
                // Reset _threadException to null, and throw if it was set during the current test.
                Interlocked.Exchange(ref _threadException, null)?.Throw();
            }
        }

        private void HandleApplicationThreadException(object sender, ThreadExceptionEventArgs e)
            => StoreThreadException(e.Exception);

        private void StoreThreadException(Exception ex)
        {
            if (_threadException is not null)
            {
                ex = new AggregateException(new Exception[] { _threadException.SourceException, ex });
            }

            _threadException = ExceptionDispatchInfo.Capture(ex);
        }

        private class DenyExecutionSynchronizationContext : SynchronizationContext
        {
            private readonly SynchronizationContext _underlyingContext;
            private readonly Thread _mainThread;
            private readonly StrongBox<ExceptionDispatchInfo> _failedTransfer;

            public DenyExecutionSynchronizationContext(SynchronizationContext underlyingContext)
                : this(underlyingContext, mainThread: null, failedTransfer: null)
            {
            }

            private DenyExecutionSynchronizationContext(SynchronizationContext underlyingContext, Thread? mainThread, StrongBox<ExceptionDispatchInfo>? failedTransfer)
            {
                _underlyingContext = underlyingContext;
                _mainThread = mainThread ?? new Thread(MainThreadStart);
                _failedTransfer = failedTransfer ?? new StrongBox<ExceptionDispatchInfo>();
            }

            internal SynchronizationContext UnderlyingContext => _underlyingContext;

            internal Thread MainThread => _mainThread;

            private static void MainThreadStart() => throw new InvalidOperationException("This thread should never be started.");

            internal void ThrowIfSwitchOccurred()
            {
                _failedTransfer.Value?.Throw();
            }

            public override void Post(SendOrPostCallback d, object? state)
            {
                try
                {
                    if (_failedTransfer.Value is null)
                    {
                        ThrowFailedTransferExceptionForCapture();
                    }
                }
                catch (InvalidOperationException e)
                {
                    _failedTransfer.Value = ExceptionDispatchInfo.Capture(e);
                }

#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                (_underlyingContext ?? new SynchronizationContext()).Post(d, state);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
            }

            public override void Send(SendOrPostCallback d, object? state)
            {
                try
                {
                    if (_failedTransfer.Value is null)
                    {
                        ThrowFailedTransferExceptionForCapture();
                    }
                }
                catch (InvalidOperationException e)
                {
                    _failedTransfer.Value = ExceptionDispatchInfo.Capture(e);
                }

#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                (_underlyingContext ?? new SynchronizationContext()).Send(d, state);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
            }

            public override SynchronizationContext CreateCopy()
            {
                return new DenyExecutionSynchronizationContext(_underlyingContext.CreateCopy(), _mainThread, _failedTransfer);
            }

            private static void ThrowFailedTransferExceptionForCapture()
            {
                throw new InvalidOperationException("Tests cannot use SwitchToMainThreadAsync unless they are marked with ApartmentState.STA.");
            }
        }

        private sealed class HangReporter : JoinableTaskContextNode
        {
            public HangReporter(JoinableTaskContext context)
                : base(context)
            {
                RegisterOnHangDetected();
            }

            protected override void OnHangDetected(TimeSpan hangDuration, int notificationCount, Guid hangId)
            {
                if (notificationCount > 1)
                {
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}HANG DETECTED: guid {hangId}{Environment.NewLine}");
                Console.ResetColor();

                if (Environment.GetEnvironmentVariable("GE_TEST_LAUNCH_DEBUGGER_ON_HANG") != "1")
                {
                    return;
                }

                Console.WriteLine("launching debugger...");

                Debugger.Launch();
                Debugger.Break();
            }
        }
    }
}
