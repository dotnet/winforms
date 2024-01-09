// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Drawing;

internal static class ModuleInitializer
{
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    [ModuleInitializer]
#pragma warning restore CA2255
    public static void InitializeModule()
    {
        // Ensure GDI+ is initialized with the module.
        bool initialized = SafeNativeMethods.Gdip.Initialized;
        Debug.Assert(initialized);
    }
}
