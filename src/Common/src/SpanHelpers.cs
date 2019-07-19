// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal static class SpanHelpers
    {
        /// <summary>
        /// Copies the <paramref name="source"/> to the <paramref name="destination"/>,
        /// terminating with null and truncating <paramref name="source"/> to fit if
        /// necessary.
        /// </summary>
        public static void CopyAndTerminate(ReadOnlySpan<char> source, Span<char> destination)
        {
            if (source.Length >= destination.Length)
            {
                source = source.Slice(0, destination.Length - 1);
            }
            source.CopyTo(destination);

            // Null terminate the string
            destination[source.Length] = '\0';
        }
    }
}
