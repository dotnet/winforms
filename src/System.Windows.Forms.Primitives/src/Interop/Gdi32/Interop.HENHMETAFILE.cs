// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HENHMETAFILE
        {
            public IntPtr Handle { get; }

            public HENHMETAFILE(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static explicit operator IntPtr(HENHMETAFILE hmetafile) => hmetafile.Handle;
        }
    }
}
