// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.WinForms;

/// <summary>
///  Represents a configured Windows Forms application.
/// </summary>
/// <remarks>
///  <para>
///   This contract prototype captures application options and exposes the
///   application lifetime. Startup and message-loop execution are implemented
///   by the application runtime.
///  </para>
/// </remarks>
public sealed class WinFormsApplication : IDisposable
{
    private WinFormsApplicationOptions? _options;

    internal WinFormsApplication(WinFormsApplicationOptions options)
    {
        _options = options;
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
    ///  Releases the application's captured options.
    /// </summary>
    public void Dispose()
    {
        Interlocked.Exchange(ref _options, null);
    }

    internal WinFormsApplicationOptions Options
        => _options ?? throw new ObjectDisposedException(nameof(WinFormsApplication));
}
