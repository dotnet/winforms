// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copied from https://raw.githubusercontent.com/dotnet/runtime/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs

using System.Runtime.CompilerServices;

namespace System;

// Helper method for local caching of compatibility quirks. Keep this lean and simple - this file is included into
// every framework assembly that implements any compatibility quirks.
internal static partial class LocalAppContextSwitches
{
    // Returns value of given switch using provided cache.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSwitchValue(string switchName, ref bool switchValue) =>
        AppContext.TryGetSwitch(switchName, out switchValue);

    // Returns value of given switch using provided cache.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue) => cachedSwitchValue switch
    {
        < 0 => false,
        > 0 => true,
        _ => GetCachedSwitchValueInternal(switchName, ref cachedSwitchValue)
    };

    private static bool GetCachedSwitchValueInternal(string switchName, ref int cachedSwitchValue)
    {
        bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
        if (!hasSwitch)
        {
            isSwitchEnabled = false;
        }

        AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
        if (!disableCaching)
        {
            cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
        }
        else if (!hasSwitch)
        {
            AppContext.SetSwitch(switchName, isSwitchEnabled);
        }

        return isSwitchEnabled;
    }
}
