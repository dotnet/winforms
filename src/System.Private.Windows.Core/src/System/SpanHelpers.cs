// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class SpanHelpers
{
    /// <summary>
    ///  Copies the <paramref name="source"/> to the <paramref name="destination"/>,
    ///  terminating with null and truncating <paramref name="source"/> to fit if
    ///  necessary.
    /// </summary>
    public static void CopyAndTerminate(this ReadOnlySpan<char> source, Span<char> destination)
    {
        Debug.Assert(!destination.IsEmpty);

        if (source.Length >= destination.Length)
        {
            source = source[..(destination.Length - 1)];
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
        return index == -1 ? span : span[..index];
    }

    /// <summary>
    ///  Slices the given <paramref name="span"/> at the first null found (if any).
    /// </summary>
    public static Span<char> SliceAtFirstNull(this Span<char> span)
    {
        int index = span.IndexOf('\0');
        return index == -1 ? span : span[..index];
    }
}
