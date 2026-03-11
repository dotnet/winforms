// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class DecimalExtensions
{
    extension(decimal value)
    {
        /// <summary>
        /// Tries to convert the value of a specified instance of <see cref="decimal"/> to its equivalent binary representation.
        /// </summary>
        /// <param name="d">The value to convert.</param>
        /// <param name="destination">The span into which to store the binary representation.</param>
        /// <param name="valuesWritten">The number of integers written to the destination.</param>
        /// <returns>true if the decimal's binary representation was written to the destination; false if the destination wasn't long enough.</returns>
        public static bool TryGetBits(decimal d, Span<int> destination, out int valuesWritten)
        {
            if (destination.Length <= 3)
            {
                valuesWritten = 0;
                return false;
            }

            // Decimal is blittable with DECIMAL in Windows and as such cannot change. Taking advantage of that
            // to cast it to a struct with the same layout to allow getting the data out on .NET Framework.

            DecimalAccessor da = Unsafe.As<decimal, DecimalAccessor>(ref d);
            destination[0] = (int)da.Low;
            destination[1] = (int)da.Mid;
            destination[2] = (int)da.High;
            destination[3] = da._flags;
            valuesWritten = 4;
            return true;
        }
    }

    private readonly struct DecimalAccessor
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value 0
        internal readonly int _flags;
        internal readonly uint _hi32;
        internal readonly ulong _lo64;
#pragma warning restore CS0649

        internal uint High => _hi32;
        internal uint Low => (uint)_lo64;
        internal uint Mid => (uint)(_lo64 >> 32);
    }
}
