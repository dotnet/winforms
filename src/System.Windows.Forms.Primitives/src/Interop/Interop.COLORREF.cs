// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct COLORREF
    {
        [FieldOffset(0)]
        public byte R;
        [FieldOffset(1)]
        public byte G;
        [FieldOffset(2)]
        public byte B;

        [FieldOffset(0)]
        public uint Value;

        public COLORREF(byte red, byte green, byte blue)
        {
            Value = 0;
            R = red;
            G = green;
            B = blue;
        }

        public COLORREF(uint value)
        {
            Unsafe.SkipInit(out this);
            Value = value & 0x00FFFFFF;
        }

        public bool IsInvalid => Value == 0xFFFFFFFF;

        public override bool Equals(object? obj) => obj is COLORREF other && Equals(other);
        public bool Equals(COLORREF other) => other.Value == Value;
        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => $"[R={R}, G={G}, B={B}]";

        public static bool operator ==(COLORREF a, COLORREF b) => a.Value == b.Value;
        public static bool operator !=(COLORREF a, COLORREF b) => a.Value != b.Value;
        public static implicit operator COLORREF(uint value) => new COLORREF(value);
        public static implicit operator COLORREF(Color color) => new COLORREF(color.R, color.G, color.B);
        public static implicit operator Color(COLORREF color) => Color.FromArgb(color.R, color.G, color.B);
    }
}
