// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
///  The <see cref="BufferedGraphicsManager"/> is used for accessing a <see cref="BufferedGraphicsContext"/>.
/// </summary>
public static class BufferedGraphicsManager
{
    /// <summary>
    ///  Static constructor. Here, we hook the exit and unload events so we can clean up our context buffer.
    /// </summary>
    static BufferedGraphicsManager()
    {
        AppDomain.CurrentDomain.ProcessExit += OnShutdown;
        AppDomain.CurrentDomain.DomainUnload += OnShutdown;
        Current = new BufferedGraphicsContext();
    }

    /// <summary>
    ///  Retrieves the context associated with the app domain.
    /// </summary>
    public static BufferedGraphicsContext Current { get; }

    /// <summary>
    ///  Called on process exit
    /// </summary>
    private static void OnShutdown(object? sender, EventArgs e) => Current.Invalidate();
}
