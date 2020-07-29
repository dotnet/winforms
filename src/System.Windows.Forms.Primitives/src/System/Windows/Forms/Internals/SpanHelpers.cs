// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System
{
    internal static class SpanHelpers
    {
        /// <summary>
        ///  Copies the <paramref name="source"/> to the <paramref name="destination"/>,
        ///  terminating with null and truncating <paramref name="source"/> to fit if
        ///  necessary.
        /// </summary>
        public static void CopyAndTerminate(ReadOnlySpan<char> source, Span<char> destination)
        {
            Debug.Assert(destination.Length > 0);

            if (source.Length >= destination.Length)
            {
                source = source.Slice(0, destination.Length - 1);
            }
            source.CopyTo(destination);

            // Null terminate the string
            destination[source.Length] = '\0';
        }

        /// <summary>
        ///  Slices the given <paramref name="span"/> at the first null found (if any).
        /// </summary>
        public static ReadOnlySpan<char> SliceAtFirstNull(this ReadOnlySpan<char> span)
        {
            int index = span.IndexOf('\0');
            return index == -1
                ? span
                : span.Slice(0, index);
        }

        /// <summary>
        ///  Slices the given <paramref name="span"/> at the first null found (if any).
        /// </summary>
        public static Span<char> SliceAtFirstNull(this Span<char> span)
        {
            int index = span.IndexOf('\0');
            return index == -1
                ? span
                : span.Slice(0, index);
        }
    }
}
