// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class ModuleInitializer
{
#if NET
    [Runtime.CompilerServices.ModuleInitializer]
#endif
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "Intentional use of module initializer to register trace listener.")]
    internal static void Initialize()
    {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(ThrowingTraceListener.Instance);
    }

#if NETFRAMEWORK
    private static bool s_initialized;
    private static readonly object s_lock = new object();

    internal static void EnsureInitialized()
    {
        if (!s_initialized)
        {
            lock (s_lock)
            {
                if (s_initialized)
                {
                    return;
                }

                Initialize();
                s_initialized = true;
            }
        }
    }
#endif
}
