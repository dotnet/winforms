// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static unsafe bool IsAccelerator<T>(T hAccel, int cAccelEntries, MSG* lpMsg, ushort* lpwCmd)
        where T : IHandle<HACCEL>
    {
        bool result = IsAccelerator(hAccel.Handle, cAccelEntries, lpMsg, lpwCmd);
        GC.KeepAlive(hAccel.Wrapper);
        return result;
    }
}
