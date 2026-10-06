// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.WinForms;

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

    private sealed class TestForm : Form
    {
        internal static int s_constructionCount;

        public TestForm()
        {
            s_constructionCount++;
        }
    }
}
