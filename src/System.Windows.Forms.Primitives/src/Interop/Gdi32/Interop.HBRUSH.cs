// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HBRUSH
        {
            public nint Handle { get; }

            public HBRUSH(nint handle) => Handle = handle;

            public bool IsNull => Handle == 0;

            public static implicit operator nint(HBRUSH hbrush) => hbrush.Handle;
            public static explicit operator HBRUSH(nint hbrush) => new(hbrush);
            public static implicit operator HGDIOBJ(HBRUSH hbrush) => new(hbrush.Handle);
            public static explicit operator HBRUSH(HGDIOBJ hbrush) => new(hbrush.Handle);
        }
    }
}
