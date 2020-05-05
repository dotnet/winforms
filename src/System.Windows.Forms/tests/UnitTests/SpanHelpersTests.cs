// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class SpanHelpersTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory,
            InlineData("", 1, "\0"),
            InlineData("a", 1, "\0"),
            InlineData("ab", 1, "\0"),
            InlineData("a", 2, "a\0"),
            InlineData("ab", 2, "a\0"),
            InlineData("abc", 2, "a\0"),
            InlineData("a", 3, "a\0@")]
        public unsafe void CopyAndTerminate(string source, int destinationLength, string expected)
        {
            Span<char> destination = stackalloc char[destinationLength];
            destination.Fill('@');
            SpanHelpers.CopyAndTerminate(source.AsSpan(), destination);
            Assert.True(destination.SequenceEqual(expected));
        }

        [Theory,
            InlineData("", ""),
            InlineData("\0", ""),
            InlineData("\0\0", ""),
            InlineData("\0a", ""),
            InlineData("\0a\0", ""),
            InlineData("a", "a"),
            InlineData("a\0", "a"),
            InlineData("a\0a", "a"),
            InlineData("a\0a\0", "a")]
        public unsafe void SliceAtFirstNull(string source, string expected)
        {
            Assert.True(source.AsSpan().SliceAtFirstNull().SequenceEqual(expected));
            Span<char> copy = stackalloc char[source.Length];
            source.AsSpan().CopyTo(copy);
            Assert.True(copy.SliceAtFirstNull().SequenceEqual(expected));
        }
    }
}
