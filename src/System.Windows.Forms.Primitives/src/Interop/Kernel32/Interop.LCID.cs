// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Kernel32
    {
        public struct LCID
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

            public static implicit operator LCID(uint value) => new LCID(value);

            public static readonly LCID USER_DEFAULT = new LCID(0x0400);
        }
    }
}
