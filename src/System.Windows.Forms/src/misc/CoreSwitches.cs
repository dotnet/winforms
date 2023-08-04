// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel;

// Shared between dlls
internal static class CoreSwitches
{
    private static BooleanSwitch? s_perfTrack;
    public static BooleanSwitch PerfTrack => s_perfTrack ??= new BooleanSwitch("PERFTRACK", "Debug performance critical sections.");
}
