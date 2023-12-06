// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Partial DpiHelper class, with methods specific to thread DpiAwarenessContext
/// </summary>
internal static partial class ScaleHelper
{
    /// <summary>
    ///  Enters a scope during which the current thread's DPI awareness context is set to
    ///  <paramref name="awareness"/>
    /// </summary>
    /// <param name="awareness">The new DPI awareness for the current thread</param>
    /// <returns>
    ///  An object that, when disposed, will reset the current thread's DPI awareness to the value it had when the
    ///  object was created.
    /// </returns>
    public static IDisposable EnterDpiAwarenessScope(
        DPI_AWARENESS_CONTEXT awareness,
        DPI_HOSTING_BEHAVIOR dpiHosting = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
        => new DpiAwarenessScope(awareness, dpiHosting);

    /// <summary>
    ///  Invokes the given action in the System Aware DPI context.
    /// </summary>
    public static T InvokeInSystemAwareContext<T>(Func<T> func)
    {
        using (EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            return func();
        }
    }
}
