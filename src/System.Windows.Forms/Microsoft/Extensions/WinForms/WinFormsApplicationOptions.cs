// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Microsoft.Extensions.WinForms;

/// <summary>
///  Stores the options used to build a Windows Forms application.
/// </summary>
internal sealed class WinFormsApplicationOptions
{
    /// <summary>
    ///  Gets or sets the factory for creating the startup form.
    /// </summary>
    internal Func<Form>? StartupFormFactory { get; set; }

    /// <summary>
    ///  Gets or sets the existing startup form.
    /// </summary>
    internal Form? StartupForm { get; set; }

    /// <summary>
    ///  Gets or sets the factory for creating the application context.
    /// </summary>
    internal Func<ApplicationContext>? ApplicationContextFactory { get; set; }

    /// <summary>
    ///  Gets or sets the existing application context.
    /// </summary>
    internal ApplicationContext? ApplicationContext { get; set; }

    /// <summary>
    ///  Creates a copy of these options.
    /// </summary>
    /// <returns>A new options instance with the same configured startup target.</returns>
    internal WinFormsApplicationOptions Clone()
        => new()
        {
            StartupFormFactory = StartupFormFactory,
            StartupForm = StartupForm,
            ApplicationContextFactory = ApplicationContextFactory,
            ApplicationContext = ApplicationContext
        };
}
