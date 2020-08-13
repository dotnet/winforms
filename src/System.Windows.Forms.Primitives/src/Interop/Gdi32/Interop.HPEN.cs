// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HPEN
        {
            public IntPtr Handle { get; }

            public HPEN(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static explicit operator IntPtr(HPEN hpen) => hpen.Handle;
            public static explicit operator HPEN(IntPtr hpen) => new HPEN(hpen);
            public static implicit operator HGDIOBJ(HPEN hpen) => new HGDIOBJ(hpen.Handle);
        }
    }
}
