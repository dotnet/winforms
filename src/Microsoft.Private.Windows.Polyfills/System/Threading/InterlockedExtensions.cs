// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Threading;

/// <summary>Provides atomic operations for variables that are shared by multiple threads.</summary>
/// <remarks>
/// <para>Contains methods the .NET Framework does not have in <see cref="Interlocked"/>.</para>
/// </remarks>
internal static class InterlockedExtensions
{
    extension(Interlocked)
    {
        /// <summary>Increments a specified variable and stores the result, as an atomic operation.</summary>
        /// <param name="location">The variable whose value is to be incremented.</param>
        /// <returns>The incremented value.</returns>
        /// <exception cref="NullReferenceException">The address of location is a null pointer.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Increment(ref uint location) => Add(ref location, 1);

        /// <summary>Decrements a specified variable and stores the result, as an atomic operation.</summary>
        /// <param name="location">The variable whose value is to be decremented.</param>
        /// <returns>The decremented value.</returns>
        /// <exception cref="NullReferenceException">The address of location is a null pointer.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Decrement(ref uint location) =>
            (uint)Interlocked.Add(ref Unsafe.As<uint, int>(ref location), -1);

        /// <summary>Sets a 32-bit unsigned integer to a specified value and returns the original value, as an atomic operation.</summary>
        /// <param name="location1">The variable to set to the specified value.</param>
        /// <param name="value">The value to which the <paramref name="location1"/> parameter is set.</param>
        /// <returns>The original value of <paramref name="location1"/>.</returns>
        /// <exception cref="NullReferenceException">The address of location1 is a null pointer.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Exchange(ref uint location1, uint value) =>
            (uint)Interlocked.Exchange(ref Unsafe.As<uint, int>(ref location1), (int)value);

        /// <summary>Adds two 32-bit unsigned integers and replaces the first integer with the sum, as an atomic operation.</summary>
        /// <param name="location1">A variable containing the first value to be added. The sum of the two values is stored in <paramref name="location1"/>.</param>
        /// <param name="value">The value to be added to the integer at <paramref name="location1"/>.</param>
        /// <returns>The new value stored at <paramref name="location1"/>.</returns>
        /// <exception cref="NullReferenceException">The address of <paramref name="location1"/> is a null pointer.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Add(ref uint location1, uint value) =>
            (uint)Interlocked.Add(ref Unsafe.As<uint, int>(ref location1), (int)value);
    }
}
