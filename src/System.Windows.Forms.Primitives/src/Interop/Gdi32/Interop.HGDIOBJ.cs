// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Gdi = Windows.Win32.Graphics.Gdi;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public struct HGDIOBJ : IEquatable<HGDIOBJ>
        {
            public nint Handle { get; }

            public HGDIOBJ(nint handle) => Handle = handle;

            public bool IsNull => Handle == 0;

            public static explicit operator nint(HGDIOBJ hgdiobj) => hgdiobj.Handle;
            public static explicit operator HGDIOBJ(nint hgdiobj) => new(hgdiobj);
            public static implicit operator HGDIOBJ(Gdi.HBITMAP hBITMAP) => new(hBITMAP.Value);
            public static implicit operator HGDIOBJ(Gdi.HPEN hpen) => new(hpen.Value);

            public static bool operator ==(HGDIOBJ value1, HGDIOBJ value2) => value1.Handle == value2.Handle;
            public static bool operator !=(HGDIOBJ value1, HGDIOBJ value2) => value1.Handle != value2.Handle;
            public override bool Equals(object? obj) => obj is HGDIOBJ hgdiobj && hgdiobj.Handle == Handle;
            public bool Equals(HGDIOBJ other) => other.Handle == Handle;
            public override int GetHashCode() => Handle.GetHashCode();

            public OBJ ObjectType => GetObjectType(this);
        }
    }
}
