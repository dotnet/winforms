// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "Intentional use of module initializer to register trace listener.")]
    internal static void Initialize()
    {
        // Ensure the module initializer runs for the main test helper assembly
        RuntimeHelpers.RunModuleConstructor(typeof(BoolDataAttribute).Module.ModuleHandle);
    }
}
