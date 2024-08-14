// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal readonly partial struct Value
{
    private sealed class PackedDateTimeOffsetFlag : TypeFlag<DateTimeOffset>
    {
        public static PackedDateTimeOffsetFlag Instance { get; } = new();

        public override DateTimeOffset To(in Value value) => value._union.PackedDateTimeOffset.Extract();
    }
}
