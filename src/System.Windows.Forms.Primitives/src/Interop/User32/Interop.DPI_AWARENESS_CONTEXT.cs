// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public static IntPtr UNSPECIFIED_DPI_AWARENESS_CONTEXT = IntPtr.Zero;

        public static class DPI_AWARENESS_CONTEXT
        {
            public static readonly IntPtr UNAWARE = (IntPtr)(-1);
            public static readonly IntPtr SYSTEM_AWARE = (IntPtr)(-2);
            public static readonly IntPtr PER_MONITOR_AWARE = (IntPtr)(-3);
            public static readonly IntPtr PER_MONITOR_AWARE_V2 = (IntPtr)(-4);
            public static readonly IntPtr UNAWARE_GDISCALED = (IntPtr)(-5);
        }
    }
}
