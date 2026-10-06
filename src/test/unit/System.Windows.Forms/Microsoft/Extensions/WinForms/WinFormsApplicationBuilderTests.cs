// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.WinForms;
using Microsoft.Extensions.Hosting;
using System.Runtime.ExceptionServices;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class WinFormsApplicationBuilderTests
{
    [Fact]
    public void CreateBuilder_ReturnsBuilder()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder();

        Assert.NotNull(builder);
    }

    [Fact]
    public void ApplicationCreateBuilder_ReturnsBuilder()
    {
        WinFormsApplicationBuilder builder = WinFormsApplication.CreateBuilder();

        Assert.NotNull(builder);
    }

    [Fact]
    public void Build_CapturesOptionsAndCreatesLifetime()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseStartupForm<TestForm>();

        using WinFormsApplication application = builder.Build();

        Assert.NotNull(application.Lifetime);
        Assert.NotNull(application.Options.StartupFormFactory);
        Assert.Null(application.Options.StartupForm);
        Assert.Null(application.Options.ApplicationContextFactory);
        Assert.Null(application.Options.ApplicationContext);
    }

    [WinFormsFact]
    public void UseStartupForm_Generic_DefersFormCreationUntilFactoryIsInvoked()
    {
        TestForm.s_constructionCount = 0;
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseStartupForm<TestForm>();

        using WinFormsApplication application = builder.Build();

        Assert.Equal(0, TestForm.s_constructionCount);

        using Form form = application.Options.StartupFormFactory!();

        Assert.IsType<TestForm>(form);
        Assert.Equal(1, TestForm.s_constructionCount);
    }

    [WinFormsFact]
    public void UseStartupForm_Instance_StoresSuppliedForm()
    {
        using Form form = new();
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseStartupForm(form);

        using WinFormsApplication application = builder.Build();

        Assert.Same(form, application.Options.StartupForm);
        Assert.Null(application.Options.StartupFormFactory);
    }

    [Fact]
    public void UseStartupForm_Instance_ThrowsOnNull()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.UseStartupForm(null!));
    }

    [Fact]
    public void UseApplicationContext_Default_DefersContextCreationUntilFactoryIsInvoked()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseApplicationContext();

        using WinFormsApplication application = builder.Build();

        Assert.NotNull(application.Options.ApplicationContextFactory);
        Assert.Null(application.Options.ApplicationContext);

        using ApplicationContext context = application.Options.ApplicationContextFactory!();

        Assert.IsType<ApplicationContext>(context);
    }

    [Fact]
    public void UseApplicationContext_Instance_StoresSuppliedContext()
    {
        using ApplicationContext context = new();
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseApplicationContext(context);

        using WinFormsApplication application = builder.Build();

        Assert.Same(context, application.Options.ApplicationContext);
        Assert.Null(application.Options.ApplicationContextFactory);
    }

    [Fact]
    public void UseApplicationContext_Instance_ThrowsOnNull()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.UseApplicationContext(null!));
    }

    [WinFormsFact]
    public void UseStartupForm_OverridesApplicationContextSelection()
    {
        using ApplicationContext context = new();
        using Form form = new();
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseApplicationContext(context)
            .UseStartupForm(form);

        using WinFormsApplication application = builder.Build();

        Assert.Same(form, application.Options.StartupForm);
        Assert.Null(application.Options.ApplicationContext);
        Assert.Null(application.Options.ApplicationContextFactory);
    }

    [WinFormsFact]
    public void UseApplicationContext_OverridesStartupFormSelection()
    {
        using Form form = new();
        using ApplicationContext context = new();
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseStartupForm(form)
            .UseApplicationContext(context);

        using WinFormsApplication application = builder.Build();

        Assert.Null(application.Options.StartupForm);
        Assert.Null(application.Options.StartupFormFactory);
        Assert.Same(context, application.Options.ApplicationContext);
    }

    [Fact]
    public void Build_SnapshotsBuilderOptions()
    {
        WinFormsApplicationBuilder builder = WinFormsApplicationBuilder.CreateBuilder()
            .UseStartupForm<TestForm>();
        using WinFormsApplication firstApplication = builder.Build();

        builder.UseApplicationContext();
        using WinFormsApplication secondApplication = builder.Build();

        Assert.NotNull(firstApplication.Options.StartupFormFactory);
        Assert.Null(firstApplication.Options.ApplicationContextFactory);
        Assert.Null(secondApplication.Options.StartupFormFactory);
        Assert.NotNull(secondApplication.Options.ApplicationContextFactory);
    }

    [Fact]
    public void Dispose_ReleasesApplicationOptions()
    {
        WinFormsApplication application = WinFormsApplication.CreateBuilder()
            .UseStartupForm<TestForm>()
            .Build();

        application.Dispose();
        application.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = application.Options;
        });
    }

    [Fact]
    public void Lifetime_RaisesEachNotificationOnceInOrder()
    {
        using WinFormsApplication application = WinFormsApplication.CreateBuilder()
            .UseApplicationContext()
            .Build();
        List<string> events = [];
        WinFormsApplicationLifetime lifetime = application.Lifetime;
        lifetime.ApplicationStarted += (_, _) => events.Add(nameof(lifetime.ApplicationStarted));
        lifetime.ApplicationStopping += (_, _) => events.Add(nameof(lifetime.ApplicationStopping));
        lifetime.ApplicationStopped += (_, _) => events.Add(nameof(lifetime.ApplicationStopped));

        lifetime.NotifyApplicationStarted();
        lifetime.NotifyApplicationStarted();
        lifetime.NotifyApplicationStopping();
        lifetime.NotifyApplicationStopping();
        lifetime.NotifyApplicationStopped();
        lifetime.NotifyApplicationStopped();

        Assert.Equal(
            [
                nameof(lifetime.ApplicationStarted),
                nameof(lifetime.ApplicationStopping),
                nameof(lifetime.ApplicationStopped)
            ],
            events);
    }

    [Fact]
    public void Lifetime_StoppingBeforeStarted_OmitsStartedAndRaisesStoppedAfterStopping()
    {
        using WinFormsApplication application = WinFormsApplication.CreateBuilder()
            .UseApplicationContext()
            .Build();
        List<string> events = [];
        WinFormsApplicationLifetime lifetime = application.Lifetime;
        lifetime.ApplicationStarted += (_, _) => events.Add(nameof(lifetime.ApplicationStarted));
        lifetime.ApplicationStopping += (_, _) => events.Add(nameof(lifetime.ApplicationStopping));
        lifetime.ApplicationStopped += (_, _) => events.Add(nameof(lifetime.ApplicationStopped));

        lifetime.NotifyApplicationStopping();
        lifetime.NotifyApplicationStarted();
        lifetime.NotifyApplicationStopped();

        Assert.Equal(
            [
                nameof(lifetime.ApplicationStopping),
                nameof(lifetime.ApplicationStopped)
            ],
            events);
    }

    [Fact]
    public void Run_StartupFormClose_RaisesLifetimeInOrder()
    {
        RunOnStaThread(() =>
        {
            List<string> events = [];
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseStartupForm<CloseOnShownForm>()
                .Build();
            application.Lifetime.ApplicationStarted += (_, _) => events.Add("Started");
            application.Lifetime.ApplicationStopping += (_, _) => events.Add("Stopping");
            application.Lifetime.ApplicationStopped += (_, _) => events.Add("Stopped");

            application.Run();

            Assert.Equal(["Started", "Stopping", "Stopped"], events);
        });
    }

    [Fact]
    public void Run_StartsHostBeforeStartedAndStopsHostWhenContextExits()
    {
        RunOnStaThread(() =>
        {
            List<string> events = [];
            TestHost host = new(events);
            using Form form = new();
            form.Shown += (_, _) => form.Close();
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseStartupForm(form)
                .UseHost(host)
                .Build();
            application.Lifetime.ApplicationStarted += (_, _) => events.Add("ApplicationStarted");
            application.Lifetime.ApplicationStopping += (_, _) => events.Add("ApplicationStopping");
            application.Lifetime.ApplicationStopped += (_, _) => events.Add("ApplicationStopped");

            application.Run();

            Assert.Equal(
                ["HostStarted", "ApplicationStarted", "ApplicationStopping", "HostStopping", "ApplicationStopped"],
                events);
            Assert.Equal(1, host.StopCount);
        });
    }

    [Fact]
    public void StopAsync_StopsHostAndExitsTheMessageLoop()
    {
        RunOnStaThread(() =>
        {
            TestHost host = new();
            using Form form = new();
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseStartupForm(form)
                .UseHost(host)
                .Build();
            form.Shown += (_, _) => _ = application.StopAsync();

            application.Run();

            Assert.Equal(1, host.StopCount);
            Assert.True(host.Lifetime.ApplicationStopped.IsCancellationRequested);
        });
    }

    [Fact]
    public void Run_ExternalHostStop_ExitsTheMessageLoop()
    {
        RunOnStaThread(() =>
        {
            TestHost host = new();
            using Form form = new();
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseStartupForm(form)
                .UseHost(host)
                .Build();
            form.Shown += (_, _) => _ = Task.Run(() => host.StopAsync());

            application.Run();

            Assert.Equal(1, host.StopCount);
            Assert.True(host.Lifetime.ApplicationStopped.IsCancellationRequested);
        });
    }

    [Fact]
    public void Run_ApplicationContextExit_IsDeferredUntilHostStops()
    {
        RunOnStaThread(() =>
        {
            TestHost host = new();
            using Form form = new();
            using ApplicationContext context = new(form);
            form.Shown += (_, _) => context.ExitThread();
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseApplicationContext(context)
                .UseHost(host)
                .Build();

            application.Run();

            Assert.Equal(1, host.StopCount);
            Assert.True(host.Lifetime.ApplicationStopped.IsCancellationRequested);
        });
    }

    [Fact]
    public void Run_StartupFormFailureRaisesStoppingAndStoppedWithoutStartingHost()
    {
        RunOnStaThread(() =>
        {
            List<string> events = [];
            TestHost host = new(events);
            using WinFormsApplication application = WinFormsApplication.CreateBuilder()
                .UseStartupForm<ThrowingForm>()
                .UseHost(host)
                .Build();
            application.Lifetime.ApplicationStarted += (_, _) => events.Add("ApplicationStarted");
            application.Lifetime.ApplicationStopping += (_, _) => events.Add("ApplicationStopping");
            application.Lifetime.ApplicationStopped += (_, _) => events.Add("ApplicationStopped");

            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(application.Run);

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(["ApplicationStopping", "ApplicationStopped"], events);
            Assert.Equal(0, host.StartCount);
            Assert.Equal(0, host.StopCount);
        });
    }

    private static void RunOnStaThread(Action action)
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
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    private sealed class TestForm : Form
    {
        internal static int s_constructionCount;

        public TestForm()
        {
            s_constructionCount++;
        }
    }

    private sealed class CloseOnShownForm : Form
    {
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Close();
        }
    }

    private sealed class ThrowingForm : Form
    {
        public ThrowingForm()
        {
            throw new InvalidOperationException("Startup form construction failed.");
        }
    }

    private sealed class TestHost : IHost
    {
        private readonly List<string>? _events;

        internal TestHost(List<string>? events = null)
        {
            _events = events;
            Lifetime = new TestHostApplicationLifetime();
        }

        internal TestHostApplicationLifetime Lifetime { get; }

        internal int StartCount { get; private set; }

        internal int StopCount { get; private set; }

        public IServiceProvider Services => new TestServiceProvider(Lifetime);

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            StartCount++;
            _events?.Add("HostStarted");
            Lifetime.NotifyStarted();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            StopCount++;
            _events?.Add("HostStopping");
            Lifetime.NotifyStopping();
            Lifetime.NotifyStopped();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class TestHostApplicationLifetime : IHostApplicationLifetime
    {
        private readonly CancellationTokenSource _started = new();
        private readonly CancellationTokenSource _stopping = new();
        private readonly CancellationTokenSource _stopped = new();

        public CancellationToken ApplicationStarted => _started.Token;

        public CancellationToken ApplicationStopping => _stopping.Token;

        public CancellationToken ApplicationStopped => _stopped.Token;

        public void StopApplication() => NotifyStopping();

        internal void NotifyStarted() => _started.Cancel();

        internal void NotifyStopping() => _stopping.Cancel();

        internal void NotifyStopped() => _stopped.Cancel();
    }

    private sealed class TestServiceProvider(IHostApplicationLifetime lifetime) : IServiceProvider
    {
        public object? GetService(Type serviceType)
            => serviceType == typeof(IHostApplicationLifetime) ? lifetime : null;
    }
}
