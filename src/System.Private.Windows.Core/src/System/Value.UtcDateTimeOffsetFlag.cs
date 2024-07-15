// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal readonly partial struct Value
{
    private sealed class UtcDateTimeOffsetFlag : TypeFlag<DateTimeOffset>
    {
        public static UtcDateTimeOffsetFlag Instance { get; } = new();

        public override DateTimeOffset To(in Value value) => new(new DateTime(value._union.Ticks, DateTimeKind.Utc));
    }
}
