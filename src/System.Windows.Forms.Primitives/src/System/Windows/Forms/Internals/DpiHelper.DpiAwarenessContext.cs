// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Partial DpiHelper class, with methods specific to thread DpiAwarenessContext
/// </summary>
internal static partial class DpiHelper
{
    /// <summary>
    ///  Enters a scope during which the current thread's DPI awareness context is set to
    /// <paramref name="awareness"/>
    /// </summary>
    /// <param name="awareness">The new DPI awareness for the current thread</param>
    /// <returns>An object that, when disposed, will reset the current thread's
    ///  DPI awareness to the value it had when the object was created.</returns>
    public static IDisposable EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT awareness, DPI_HOSTING_BEHAVIOR dpiHosting = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
    {
        return new DpiAwarenessScope(awareness, dpiHosting);
    }

    /// <summary>
    ///  Create an object in system aware context. This method is mainly designed to create control objects in system aware context.
    ///  class is internal and is mainly used for modal dialogs.
    /// </summary>
    /// <typeparam name="T">return type of the object</typeparam>
    /// <param name="createInstance">lambda expression</param>
    /// <returns> returns object created in system aware mode</returns>
    public static T CreateInstanceInSystemAwareContext<T>(Func<T> createInstance)
    {
        using (EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            return createInstance();
        }
    }
}
