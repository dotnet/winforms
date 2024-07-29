// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System;

internal readonly partial struct Value
{
    private sealed class PackedColorFlag : TypeFlag<Color>
    {
        public static PackedColorFlag Instance { get; } = new();

        public override Color To(in Value value)
            => value._union.PackedColor.Extract();
    }
}
