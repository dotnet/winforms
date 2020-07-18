// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ARGB
    {
        [FieldOffset(0)]
        public readonly byte B;
        [FieldOffset(1)]
        public readonly byte G;
        [FieldOffset(2)]
        public readonly byte R;
        [FieldOffset(3)]
        public readonly byte A;

        [FieldOffset(0)]
        public readonly int Value;

        public ARGB(byte a, byte r, byte g, byte b)
        {
            Debug.Assert(BitConverter.IsLittleEndian);
            Unsafe.SkipInit(out this);
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public unsafe ARGB(int value)
        {
            Debug.Assert(BitConverter.IsLittleEndian);
            Unsafe.SkipInit(out this);
            Value = value;
        }

        public static implicit operator ARGB(Color color) => new ARGB(color.ToArgb());
        public static implicit operator Color(ARGB argb) => Color.FromArgb(argb.Value);
    }
}
