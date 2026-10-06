// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.WinForms;

/// <summary>
///  Represents a configured Windows Forms application.
/// </summary>
/// <remarks>
///  <para>
///   The thread calling <see cref="Run"/> owns the UI thread and message loop.
///   The application starts and stops an optional Generic Host on that thread's
///   lifecycle, but keeps asynchronous host shutdown from blocking the message
///   pump.
///  </para>
/// </remarks>
public sealed class WinFormsApplication : IDisposable
{
    private readonly Lock _stopLock = new();
    private readonly TaskCompletionSource _hostStoppedSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private WinFormsApplicationOptions? _options;
    private IHost? _host;
    private IHostApplicationLifetime? _hostLifetime;
    private ApplicationContext? _applicationContext;
    private Control? _marshallingControl;
    private Thread? _uiThread;
    private Func<ApplicationContext, bool>? _previousExitThreadHandler;
    private CancellationTokenRegistration _applicationStoppingRegistration;
    private CancellationTokenRegistration _applicationStoppedRegistration;
    private Task? _hostStopTask;
    private Exception? _runtimeException;
    private int _runState;
    private int _messageLoopEntered;
    private int _allowThreadExit;
    private int _hostStopRequestedByApplication;
    private int _hostStartAttempted;
    private int _hostStopped;
    private int _disposeRequested;

    internal WinFormsApplication(WinFormsApplicationOptions options)
    {
        _options = options;
        _host = options.Host;
    }

    /// <summary>
    ///  Creates a builder for a Windows Forms application.
    /// </summary>
    /// <returns>A new application builder.</returns>
    public static WinFormsApplicationBuilder CreateBuilder()
        => WinFormsApplicationBuilder.CreateBuilder();

    /// <summary>
    ///  Gets the lifetime notifications for this application.
    /// </summary>
    public WinFormsApplicationLifetime Lifetime { get; } = new();

    /// <summary>
    ///  Starts the configured host and runs the Windows Forms message loop.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///  No startup form or application context was configured, or this
    ///  application has already been run.
    /// </exception>
    public void Run()
    {
        WinFormsApplicationOptions options = Options;
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeRequested) != 0, this);

        if (Interlocked.CompareExchange(ref _runState, 1, 0) != 0)
        {
            throw new InvalidOperationException("A WinForms application can only be run once.");
        }

        if (_hostStopTask is not null)
        {
            _runState = 2;
            throw new InvalidOperationException("A stopped WinForms application cannot be run.");
        }

        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            _runState = 2;
            throw new InvalidOperationException("The WinForms application must run on an STA thread.");
        }

        if (options.StartupObjectThread is not null
            && !ReferenceEquals(options.StartupObjectThread, Thread.CurrentThread))
        {
            _runState = 2;
            throw new InvalidOperationException(
                "A supplied startup form or application context must be used on its creating thread.");
        }

        bool autoInstall = WindowsFormsSynchronizationContext.AutoInstall;
        SynchronizationContext? originalSynchronizationContext = SynchronizationContext.Current;
        bool installedSynchronizationContext = false;
        Exception? failure = null;
        ApplicationContext? applicationContext = null;

        _uiThread = Thread.CurrentThread;

        try
        {
            WindowsFormsSynchronizationContext.AutoInstall = true;
            WindowsFormsSynchronizationContext.InstallIfNeeded();
            installedSynchronizationContext =
                originalSynchronizationContext is not WindowsFormsSynchronizationContext
                && SynchronizationContext.Current is WindowsFormsSynchronizationContext;
            _marshallingControl = Application.ThreadContext.FromCurrent().MarshallingControl;

            applicationContext = CreateApplicationContext(options);
            _applicationContext = applicationContext;
            _previousExitThreadHandler = applicationContext.ExitThreadHandler;
            applicationContext.ExitThreadHandler = OnExitThreadRequested;

            InitializeHostLifetime();
            StartHost();
            Lifetime.NotifyApplicationStarted();

            bool enterMessageLoop;
            lock (_stopLock)
            {
                enterMessageLoop = Volatile.Read(ref _allowThreadExit) == 0;
                if (enterMessageLoop)
                {
                    Volatile.Write(ref _messageLoopEntered, 1);
                }
            }

            if (enterMessageLoop)
            {
                Application.Run(applicationContext);
            }
        }
        catch (Exception exception)
        {
            failure = exception;
        }
        finally
        {
            if (applicationContext is not null)
            {
                applicationContext.ExitThreadHandler = _previousExitThreadHandler;
                _applicationContext = null;

                try
                {
                    applicationContext.Dispose();
                }
                catch (Exception exception)
                {
                    failure = CombineFailures(failure, exception);
                }
            }

            if (Volatile.Read(ref _hostStartAttempted) != 0
                && Volatile.Read(ref _hostStopped) == 0)
            {
                try
                {
                    StopHostAfterUnexpectedLoopExit();
                }
                catch (Exception exception)
                {
                    failure = CombineFailures(failure, exception);
                }
            }

            try
            {
                Lifetime.NotifyApplicationStopped();
            }
            catch (Exception exception)
            {
                failure = CombineFailures(failure, exception);
            }

            _applicationStoppingRegistration.Dispose();
            _applicationStoppedRegistration.Dispose();

            if (installedSynchronizationContext)
            {
                WindowsFormsSynchronizationContext.Uninstall(turnOffAutoInstall: false);
            }

            if (!ReferenceEquals(SynchronizationContext.Current, originalSynchronizationContext))
            {
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext);
            }

            WindowsFormsSynchronizationContext.AutoInstall = autoInstall;
            Volatile.Write(ref _runState, 2);

            failure = CombineFailures(failure, Interlocked.Exchange(ref _runtimeException, null));

            if (Volatile.Read(ref _disposeRequested) != 0)
            {
                try
                {
                    DisposeHost();
                }
                catch (Exception exception)
                {
                    failure = CombineFailures(failure, exception);
                }
                finally
                {
                    Interlocked.Exchange(ref _options, null);
                }
            }
        }

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    /// <summary>
    ///  Stops the associated host and exits the application's message loop.
    /// </summary>
    /// <param name="cancellationToken">
    ///  A token that can cancel the host's cooperative shutdown operation.
    /// </param>
    /// <returns>A task that completes after the host stop request completes.</returns>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _runState) == 2)
        {
            return Task.CompletedTask;
        }

        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposeRequested) != 0, this);
        NotifyApplicationStopping();

        Task stopTask = EnsureHostStopStarted(cancellationToken);

        return cancellationToken.CanBeCanceled
            ? stopTask.WaitAsync(cancellationToken)
            : stopTask;
    }

    /// <summary>
    ///  Stops the application if it is running and releases the associated host.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposeRequested, 1) != 0)
        {
            return;
        }

        if (Volatile.Read(ref _runState) == 1)
        {
            NotifyApplicationStopping();
            _ = EnsureHostStopStarted(CancellationToken.None);
            return;
        }

        try
        {
            DisposeHost();
        }
        finally
        {
            Interlocked.Exchange(ref _options, null);
        }
    }

    internal WinFormsApplicationOptions Options
        => _options ?? throw new ObjectDisposedException(nameof(WinFormsApplication));

    private static ApplicationContext CreateApplicationContext(WinFormsApplicationOptions options)
    {
        if (options.StartupFormFactory is not null)
        {
            return new ApplicationContext(options.StartupFormFactory());
        }

        if (options.StartupForm is not null)
        {
            return new ApplicationContext(options.StartupForm);
        }

        if (options.ApplicationContextFactory is not null)
        {
            return options.ApplicationContextFactory()
                ?? throw new InvalidOperationException("The application context factory returned null.");
        }

        return options.ApplicationContext
            ?? throw new InvalidOperationException(
                "Configure a startup form or application context before running the application.");
    }

    private void InitializeHostLifetime()
    {
        if (_host is null)
        {
            return;
        }

        _hostLifetime = _host.Services.GetService(typeof(IHostApplicationLifetime))
            as IHostApplicationLifetime;

        if (_hostLifetime is null)
        {
            return;
        }

        _applicationStoppingRegistration = _hostLifetime.ApplicationStopping.Register(
            static state => ((WinFormsApplication)state!).OnHostStopping(),
            this);
        _applicationStoppedRegistration = _hostLifetime.ApplicationStopped.Register(
            static state => ((WinFormsApplication)state!).OnHostStopped(),
            this);

        if (_hostLifetime.ApplicationStopped.IsCancellationRequested)
        {
            Volatile.Write(ref _hostStopped, 1);
            _hostStoppedSource.TrySetResult();
        }
    }

    private void StartHost()
    {
        if (_host is null)
        {
            return;
        }

        if (_hostLifetime?.ApplicationStopped.IsCancellationRequested == true
            || _hostLifetime?.ApplicationStopping.IsCancellationRequested == true)
        {
            throw new InvalidOperationException("The configured Generic Host is already stopping or has stopped.");
        }

        Volatile.Write(ref _hostStartAttempted, 1);

        if (_hostLifetime?.ApplicationStarted.IsCancellationRequested != true)
        {
            Task.Run(() => _host.StartAsync()).GetAwaiter().GetResult();
        }
    }

    private bool OnExitThreadRequested(ApplicationContext context)
    {
        if (_previousExitThreadHandler is not null
            && !_previousExitThreadHandler(context))
        {
            return false;
        }

        if (Volatile.Read(ref _allowThreadExit) != 0)
        {
            return true;
        }

        NotifyApplicationStopping();

        if (_host is null)
        {
            AllowThreadExit();
            return true;
        }

        if (Volatile.Read(ref _hostStopped) != 0
            || _hostLifetime?.ApplicationStopped.IsCancellationRequested == true)
        {
            AllowThreadExit();
            return true;
        }

        if (_hostLifetime?.ApplicationStopping.IsCancellationRequested != true)
        {
            _ = EnsureHostStopStarted(CancellationToken.None);
        }

        return false;
    }

    private void OnHostStopping()
    {
        if (ReferenceEquals(Thread.CurrentThread, _uiThread) || _marshallingControl is null)
        {
            NotifyApplicationStopping();
            return;
        }

        try
        {
            _marshallingControl.BeginInvoke((Action)NotifyApplicationStopping);
        }
        catch (Exception exception)
        {
            RecordRuntimeException(exception);
            NotifyApplicationStopping();
        }
    }

    private void OnHostStopped()
    {
        lock (_stopLock)
        {
            Volatile.Write(ref _hostStopped, 1);
            _hostStoppedSource.TrySetResult();
        }

        if (Volatile.Read(ref _hostStopRequestedByApplication) == 0)
        {
            AllowThreadExit();
            RequestThreadExit();
        }
    }

    private void NotifyApplicationStopping()
    {
        try
        {
            Lifetime.NotifyApplicationStopping();
        }
        catch (Exception exception)
        {
            RecordRuntimeException(exception);
        }
    }

    private Task EnsureHostStopStarted(CancellationToken cancellationToken)
    {
        lock (_stopLock)
        {
            if (_hostStopTask is not null)
            {
                return _hostStopTask;
            }

            if (_host is null)
            {
                Volatile.Write(ref _hostStopped, 1);
                Volatile.Write(ref _allowThreadExit, 1);
                _hostStoppedSource.TrySetResult();
                _hostStopTask = Task.CompletedTask;
                RequestThreadExit();
                return _hostStopTask;
            }

            if (_hostLifetime?.ApplicationStopping.IsCancellationRequested == true)
            {
                _hostStopTask = _hostStoppedSource.Task;
                return _hostStopTask;
            }

            Volatile.Write(ref _hostStopRequestedByApplication, 1);
            _hostStopTask = Task.Run(
                () => StopHostAsync(cancellationToken),
                CancellationToken.None);
            return _hostStopTask;
        }
    }

    private async Task StopHostAsync(CancellationToken cancellationToken)
    {
        Exception? failure = null;

        try
        {
            if (_host is not null)
            {
                await _host.StopAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            failure = exception;
            RecordRuntimeException(exception);
        }
        finally
        {
            lock (_stopLock)
            {
                Volatile.Write(ref _hostStopped, 1);
                _hostStoppedSource.TrySetResult();
            }

            AllowThreadExit();
            RequestThreadExit();
        }

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    private void AllowThreadExit()
    {
        lock (_stopLock)
        {
            Volatile.Write(ref _allowThreadExit, 1);
        }
    }

    private void StopHostAfterUnexpectedLoopExit()
    {
        EnsureHostStopStarted(CancellationToken.None).GetAwaiter().GetResult();
    }

    private void RequestThreadExit()
    {
        ApplicationContext? context = _applicationContext;

        if (Volatile.Read(ref _messageLoopEntered) == 0)
        {
            return;
        }

        if (context is null)
        {
            if (Volatile.Read(ref _runState) != 1)
            {
                try
                {
                    Lifetime.NotifyApplicationStopped();
                }
                catch (Exception exception)
                {
                    RecordRuntimeException(exception);
                }
            }

            return;
        }

        try
        {
            if (ReferenceEquals(Thread.CurrentThread, _uiThread))
            {
                context.ExitThread();
            }
            else
            {
                _marshallingControl!.BeginInvoke(context.ExitThread);
            }
        }
        catch (Exception exception)
        {
            RecordRuntimeException(exception);
        }
    }

    private void RecordRuntimeException(Exception exception)
    {
        lock (_stopLock)
        {
            _runtimeException = CombineFailures(_runtimeException, exception);
        }
    }

    private static Exception? CombineFailures(Exception? first, Exception? second)
    {
        if (first is null)
        {
            return second;
        }

        if (second is null || ReferenceEquals(first, second))
        {
            return first;
        }

        return new AggregateException(first, second);
    }

    private void DisposeHost()
    {
        IHost? host = Interlocked.Exchange(ref _host, null);
        host?.Dispose();
    }
}
