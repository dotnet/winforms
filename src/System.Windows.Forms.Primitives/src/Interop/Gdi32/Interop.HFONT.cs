// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HFONT
        {
            public nint Handle { get; }

            public HFONT(nint handle) => Handle = handle;

            public bool IsNull => Handle == 0;

            public static implicit operator HGDIOBJ(HFONT hfont) => new HGDIOBJ(hfont.Handle);
            public static explicit operator HFONT(HGDIOBJ hfont) => new HFONT(hfont.Handle);
            public static explicit operator HFONT(nint hfont) => new HFONT(hfont);
            public static implicit operator IntPtr(HFONT hfont) => hfont.Handle;
        }
    }
}
