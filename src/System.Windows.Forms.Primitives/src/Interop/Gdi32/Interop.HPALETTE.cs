// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HPALETTE
        {
            public IntPtr Handle { get; }

            public HPALETTE(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static explicit operator IntPtr(HPALETTE hpalette) => hpalette.Handle;
            public static explicit operator HPALETTE(IntPtr hpalette) => new HPALETTE(hpalette);
            public static implicit operator HGDIOBJ(HPALETTE hpalette) => new HGDIOBJ(hpalette.Handle);
        }
    }
}
