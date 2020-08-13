// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HBITMAP
        {
            public IntPtr Handle { get; }

            public HBITMAP(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static implicit operator HGDIOBJ(HBITMAP hbitmap) => new HGDIOBJ(hbitmap.Handle);
            public static explicit operator HBITMAP(HGDIOBJ hbitmap) => new HBITMAP(hbitmap.Handle);
            public static explicit operator HBITMAP(IntPtr hbitmap) => new HBITMAP(hbitmap);
            public static explicit operator IntPtr(HBITMAP hbitmap) => hbitmap.Handle;
        }
    }
}
