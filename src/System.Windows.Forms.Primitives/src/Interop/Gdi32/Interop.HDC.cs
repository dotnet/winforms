// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Gdi = Windows.Win32.Graphics.Gdi;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HDC : IEquatable<HDC>
        {
            public nint Handle { get; }

            public HDC(nint handle) => Handle = handle;

            public bool IsNull => Handle == 0;

            public static implicit operator nint(HDC hdc) => hdc.Handle;
            public static explicit operator HDC(nint hdc) => new(hdc);
            public static implicit operator HGDIOBJ(HDC hdc) => new(hdc.Handle);
            public static implicit operator HDC(Gdi.HDC hdc) => new(hdc.Value);
            public static implicit operator Gdi.HDC(HDC hdc) => new(hdc.Handle);

            public static bool operator ==(HDC value1, HDC value2) => value1.Handle == value2.Handle;
            public static bool operator !=(HDC value1, HDC value2) => value1.Handle != value2.Handle;
            public override bool Equals(object? obj) => obj is HDC hdc && hdc.Handle == Handle;
            public bool Equals(HDC other) => other.Handle == Handle;
            public override int GetHashCode() => Handle.GetHashCode();
        }
    }
}
