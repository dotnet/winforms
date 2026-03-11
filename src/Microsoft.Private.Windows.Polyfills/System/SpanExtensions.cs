// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static partial class SpanExtensions
{
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The target span.</param>
    extension<T>(ReadOnlySpan<T> span) where T : IEquatable<T>
    {
        /// <summary>
        ///  Searches for the first index of any value other than the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A value to avoid.</param>
        /// <remarks>
        ///  <para>
        ///   .NET Framework extension to match .NET functionality.
        ///  </para>
        /// </remarks>
        /// <returns>
        ///  The index in the span of the first occurrence of any value other than <paramref name="value"/>.
        ///  If all of the values are <paramref name="value"/>, returns -1.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfAnyExcept(T value)
        {
            for (int i = 0; i < span.Length; i++)
            {
                if (!span[i].Equals(value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
