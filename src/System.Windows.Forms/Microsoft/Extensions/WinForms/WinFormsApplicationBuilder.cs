// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.WinForms;

/// <summary>
///  A builder for a Windows Forms application.
/// </summary>
/// <remarks>
///  <para>
///   The builder records the startup UI object without creating it. The
///   application runtime is responsible for activation on the UI thread.
///  </para>
///  <para>
///   Calling a startup selection method replaces any selection previously
///   made on this builder.
///  </para>
/// </remarks>
public sealed class WinFormsApplicationBuilder
{
    private readonly WinFormsApplicationOptions _options = new();

    internal WinFormsApplicationBuilder()
    {
    }

    /// <summary>
    ///  Creates a builder for a Windows Forms application.
    /// </summary>
    /// <returns>A new application builder.</returns>
    public static WinFormsApplicationBuilder CreateBuilder()
        => new();

    /// <summary>
    ///  Selects a startup form to be created by the application runtime.
    /// </summary>
    /// <typeparam name="TForm">The type of the startup form.</typeparam>
    /// <returns>This builder.</returns>
    public WinFormsApplicationBuilder UseStartupForm<TForm>()
        where TForm : Form, new()
    {
        _options.StartupFormFactory = static () => new TForm();
        _options.StartupForm = null;
        _options.ApplicationContextFactory = null;
        _options.ApplicationContext = null;
        _options.StartupObjectThread = null;

        return this;
    }

    /// <summary>
    ///  Selects an existing form as the startup form.
    /// </summary>
    /// <param name="startupForm">The startup form.</param>
    /// <returns>This builder.</returns>
    public WinFormsApplicationBuilder UseStartupForm(Form startupForm)
    {
        ArgumentNullException.ThrowIfNull(startupForm);

        _options.StartupFormFactory = null;
        _options.StartupForm = startupForm;
        _options.ApplicationContextFactory = null;
        _options.ApplicationContext = null;
        _options.StartupObjectThread = Thread.CurrentThread;

        return this;
    }

    /// <summary>
    ///  Selects a default <see cref="ApplicationContext"/> for the application.
    /// </summary>
    /// <returns>This builder.</returns>
    public WinFormsApplicationBuilder UseApplicationContext()
    {
        _options.StartupFormFactory = null;
        _options.StartupForm = null;
        _options.ApplicationContextFactory = static () => new();
        _options.ApplicationContext = null;
        _options.StartupObjectThread = null;

        return this;
    }

    /// <summary>
    ///  Selects an existing application context for the application.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <returns>This builder.</returns>
    public WinFormsApplicationBuilder UseApplicationContext(ApplicationContext applicationContext)
    {
        ArgumentNullException.ThrowIfNull(applicationContext);

        _options.StartupFormFactory = null;
        _options.StartupForm = null;
        _options.ApplicationContextFactory = null;
        _options.ApplicationContext = applicationContext;
        _options.StartupObjectThread = Thread.CurrentThread;

        return this;
    }

    /// <summary>
    ///  Associates a Generic Host with the application.
    /// </summary>
    /// <param name="host">The host to start and stop with the application.</param>
    /// <returns>This builder.</returns>
    /// <remarks>
    ///  <para>
    ///   The application takes ownership of the host and disposes it when the
    ///   application is disposed.
    ///  </para>
    /// </remarks>
    public WinFormsApplicationBuilder UseHost(IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        _options.Host = host;

        return this;
    }

    /// <summary>
    ///  Builds a Windows Forms application from the builder's current options.
    /// </summary>
    /// <returns>The configured application.</returns>
    public WinFormsApplication Build()
        => new(_options.Clone());

    internal WinFormsApplicationOptions Options => _options;
}
