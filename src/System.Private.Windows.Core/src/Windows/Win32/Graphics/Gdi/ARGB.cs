// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32.Graphics.Gdi;

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
    public readonly uint Value;

    public ARGB(byte a, byte r, byte g, byte b)
    {
        Debug.Assert(BitConverter.IsLittleEndian);
        Unsafe.SkipInit(out this);
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public ARGB(uint value)
    {
        Debug.Assert(BitConverter.IsLittleEndian);
        Unsafe.SkipInit(out this);
        Value = value;
    }

    public static implicit operator ARGB(in Color color) => new((uint)color.ToArgb());
    public static implicit operator ARGB(uint color) => new(color);
    public static implicit operator Color(ARGB argb) => Color.FromArgb((int)argb.Value);
    public static implicit operator uint(ARGB argb) => argb.Value;

    public static Color[] ToColorArray(params ReadOnlySpan<ARGB> argbColors)
    {
        Color[] colors = new Color[argbColors.Length];
        for (int i = 0; i < argbColors.Length; i++)
        {
            colors[i] = argbColors[i];
        }

        return colors;
    }

    public static Color[] ToColorArray(params ReadOnlySpan<uint> argbColors) => ToColorArray(
        MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.As<uint, ARGB>(ref MemoryMarshal.GetReference(argbColors)),
            argbColors.Length));
}
