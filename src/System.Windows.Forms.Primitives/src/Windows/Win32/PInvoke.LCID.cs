// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public struct LCID : IEquatable<LCID>
    {
        public uint RawValue;

        public LCID(uint id)
        {
            RawValue = id;
        }

        public override bool Equals(object? obj)
        {
            return obj is LCID other
                ? other.RawValue == RawValue
                : false;
        }

        public bool Equals(LCID other) => other.RawValue == RawValue;

        public override int GetHashCode() => RawValue.GetHashCode();

        public static bool operator ==(LCID a, LCID b) => a.RawValue == b.RawValue;

        public static bool operator !=(LCID a, LCID b) => a.RawValue != b.RawValue;

        public static implicit operator LCID(uint value) => new(value);

        public static readonly LCID USER_DEFAULT = new(0x0400);
    }
}
