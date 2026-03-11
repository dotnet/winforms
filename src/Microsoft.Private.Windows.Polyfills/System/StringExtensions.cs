// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Private.Windows.Polyfills.Resources;

namespace System;

internal static partial class StringExtensions
{
    extension(string stringValue)
    {
        /// <summary>
        ///  Copies the contents of this string into the destination span.
        /// </summary>
        /// <param name="destination">The span into which to copy this string's contents.</param>
        /// <exception cref="ArgumentException">The destination span is shorter than the source string.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<char> destination)
        {
            if (destination.Length < stringValue.Length)
            {
                throw new ArgumentException(SRF.String_DestinationSpanTooSmall, nameof(destination));
            }

            stringValue.AsSpan().CopyTo(destination);
        }

        /// <summary>
        ///  Allocates a string of the specified length filled with null characters.
        /// </summary>
        /// <param name="length">The length of the string to allocate.</param>
        /// <returns>A new string of the specified <paramref name="length"/> filled with null characters.</returns>
        internal static string FastAllocateString(int length) =>
            // This calls FastAllocateString in the runtime, with extra checks.
            new string('\0', length);

        /// <inheritdoc cref="Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static unsafe string Concat(ReadOnlySpan<char> str0, ReadOnlySpan<char> str1)
        {
            int length = checked(str0.Length + str1.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            string result = FastAllocateString(length);
            fixed (char* firstChar = result)
            {
                Span<char> resultSpan = new(firstChar, result.Length);
                str0.CopyTo(resultSpan);
                str1.CopyTo(resultSpan[str0.Length..]);
            }

            return result;
        }

        /// <inheritdoc cref="Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>
        public static unsafe string Concat(ReadOnlySpan<char> str0, ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            int length = checked(str0.Length + str1.Length + str2.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            string result = FastAllocateString(length);
            fixed (char* firstChar = result)
            {
                Span<char> resultSpan = new(firstChar, result.Length);
                str0.CopyTo(resultSpan);
                resultSpan = resultSpan[str0.Length..];
                str1.CopyTo(resultSpan);
                resultSpan = resultSpan[str1.Length..];
                str2.CopyTo(resultSpan);
            }

            return result;
        }

        /// <summary>
        ///  Concatenates the string representations of the specified read-only character spans.
        /// </summary>
        /// <param name="str0">The first span to concatenate.</param>
        /// <param name="str1">The second span to concatenate.</param>
        /// <param name="str2">The third span to concatenate.</param>
        /// <param name="str3">The fourth span to concatenate.</param>
        /// <returns>The concatenated string.</returns>
        public static unsafe string Concat(
            ReadOnlySpan<char> str0,
            ReadOnlySpan<char> str1,
            ReadOnlySpan<char> str2,
            ReadOnlySpan<char> str3)
        {
            int length = checked(str0.Length + str1.Length + str2.Length + str3.Length);
            if (length == 0)
            {
                return string.Empty;
            }

            string result = FastAllocateString(length);
            fixed (char* firstChar = result)
            {
                Span<char> resultSpan = new(firstChar, result.Length);
                str0.CopyTo(resultSpan);
                resultSpan = resultSpan[str0.Length..];
                str1.CopyTo(resultSpan);
                resultSpan = resultSpan[str1.Length..];
                str2.CopyTo(resultSpan);
                resultSpan = resultSpan[str2.Length..];
                str3.CopyTo(resultSpan);
            }

            return result;
        }
    }
}
