// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

internal partial struct CY : IEquatable<CY>
{
    public readonly bool Equals(CY other) => int64 == other.int64;
    public override readonly bool Equals(object? obj) => obj is CY cy && Equals(cy);
    public override readonly int GetHashCode() => int64.GetHashCode();

    public static bool operator ==(CY left, CY right) => left.Equals(right);
    public static bool operator !=(CY left, CY right) => !left.Equals(right);

    // https://learn.microsoft.com/openspecs/windows_protocols/ms-oaut/5a2b34c4-d109-438e-9ec8-84816d8de40d

    public static explicit operator decimal(CY value) => decimal.FromOACurrency(value.int64);
    public static explicit operator CY(decimal value) => new() { int64 = decimal.ToOACurrency(value) };

    public static explicit operator float(CY value) => (float)(value.int64 / 10000f);
    public static explicit operator CY(float value) => new() { int64 = (long)(value * 10000) };
}
