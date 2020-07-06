// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HBRUSH
        {
            public IntPtr Handle { get; }

            public HBRUSH(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static explicit operator IntPtr(HBRUSH hbrush) => hbrush.Handle;
            public static explicit operator HBRUSH(IntPtr hbrush) => new HBRUSH(hbrush);
            public static implicit operator HGDIOBJ(HBRUSH hbrush) => new HGDIOBJ(hbrush.Handle);
            public static explicit operator HBRUSH(HGDIOBJ hbrush) => new HBRUSH(hbrush.Handle);
        }
    }
}
