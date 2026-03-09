// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Private.Windows.Polyfills.Resources;

namespace System;

internal static class ArgumentOutOfRangeExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value < 0)
            {
                ThrowNegative(value, paramName);
            }
        }

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0)
                ThrowLess(value, other, paramName);
        }
    }

#pragma warning disable IDE0051 // Remove unused private members
    // .NET Framework analyzers don't understand that these methods are only called from the extension methods above,
    // so it thinks it's unused. Suppress that warning since these methods are actually used.

    [DoesNotReturn]
    private static void ThrowNegative<T>(T value, string? paramName) =>
        throw new ArgumentOutOfRangeException(
            paramName,
            value,
            string.Format(SRF.ArgumentOutOfRange_Generic_MustBeNonNegative, paramName, value));

    [DoesNotReturn]
    private static void ThrowLess<T>(T value, T other, string? paramName) =>
    throw new ArgumentOutOfRangeException(
        paramName,
        value,
        string.Format(SRF.ArgumentOutOfRange_Generic_MustBeGreaterOrEqual, paramName, value, other));

#pragma warning restore IDE0051 // Remove unused private members

}
