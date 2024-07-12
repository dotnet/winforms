// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System;

internal readonly partial struct Value
{
    private readonly struct PackedColor
    {
        private readonly int _argb;
        private readonly KnownColor _knownColor;

        private PackedColor(int argb, KnownColor knownColor)
        {
            _argb = argb;
            _knownColor = knownColor;
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        private readonly struct CastColor
        {
            public readonly string? Name;
            public readonly long Argb;
            public readonly short KnownColor;
            public readonly short State;
        }
#pragma warning restore CS0649

        public static bool TryCreate(in Color color, out PackedColor packedColor)
        {
            CastColor castColor = Unsafe.As<Color, CastColor>(ref Unsafe.AsRef(in color));
            if (castColor.Name is not null)
            {
                // We can't stash the name, so we can't pack it. It might be possible to stash a named color by
                // doing something similar to what we do with ArraySegment<> (store the string, and set the Union
                // to -1 to indicate that it's a named color). That would, however, slow down other logic so it
                // shouldn't be done without strong data to back it up. (It seems highly unlikely that this case
                // is common enough to warrant the extra complexity.)
                packedColor = default;
                return false;
            }

            packedColor = new PackedColor((int)castColor.Argb, (KnownColor)castColor.KnownColor);
            return true;
        }

        public Color Extract() => _knownColor != 0 ? Color.FromKnownColor(_knownColor) : Color.FromArgb(_argb);
    }
}
