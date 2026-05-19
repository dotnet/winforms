// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO;

internal static partial class PathExtensions
{
    extension(Path)
    {
        /// <summary>
        ///  Concatenates two path components into a single path.
        /// </summary>
        /// <param name="path1">A character span that contains the first path to join.</param>
        /// <param name="path2">A character span that contains the second path to join.</param>
        /// <returns>The concatenated path.</returns>
        public static string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
        {
            if (path1.Length == 0)
                return path2.ToString();
            if (path2.Length == 0)
                return path1.ToString();

            return JoinInternal(path1, path2);
        }

        /// <summary>
        ///  Concatenates two paths into a single path.
        /// </summary>
        /// <param name="path1">The first path to join.</param>
        /// <param name="path2">The second path to join.</param>
        /// <returns>The concatenated path.</returns>
        public static string Join(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2;
            if (string.IsNullOrEmpty(path2))
                return path1;

            return JoinInternal(path1, path2);
        }

        /// <summary>
        ///  <see langword="true"/> if the given character is a directory separator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDirectorySeparator(char c) => c is DirectorySeparatorChar or AltDirectorySeparatorChar;
    }

#pragma warning disable IDE0051 // Remove unused private members
    private const char DirectorySeparatorChar = '\\';
    private const char AltDirectorySeparatorChar = '/';

    private static string JoinInternal(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        Debug.Assert(first.Length > 0 && second.Length > 0, "should have dealt with empty paths");

        bool hasSeparator = IsDirectorySeparator(first[^1]) || IsDirectorySeparator(second[0]);

        return hasSeparator
            ? string.Concat(first, second)
            : string.Concat(first, @"\", second);
    }
#pragma warning restore IDE0051 // Remove unused private members
}
