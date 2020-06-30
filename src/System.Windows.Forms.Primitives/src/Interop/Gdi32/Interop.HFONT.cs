// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HFONT
        {
            public IntPtr Handle { get; }

            public HFONT(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static implicit operator HGDIOBJ(HFONT hfont) => new HGDIOBJ(hfont.Handle);
            public static explicit operator HFONT(HGDIOBJ hfont) => new HFONT(hfont.Handle);
            public static explicit operator HFONT(IntPtr hfont) => new HFONT(hfont);
            public static explicit operator IntPtr(HFONT hfont) => hfont.Handle;
        }
    }
}
