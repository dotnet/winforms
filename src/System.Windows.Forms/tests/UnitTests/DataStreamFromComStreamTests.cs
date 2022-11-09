// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataStreamFromComStreamTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory,
            InlineData(0, 0, 1),
            InlineData(1, 1, 1)]
        public unsafe void Write_ThrowsInvalidCount(int bufferSize, int index, int count)
        {
            using MemoryStream memoryStream = new();
            using var stream = ComHelpers.GetComScope<IStream>(new Interop.Ole32.GPStream(memoryStream), out bool result);
            Assert.True(result);
            using DataStreamFromComStream dataStream = new(stream, ownsHandle: false);
            Assert.Throws<IOException>(() => dataStream.Write(new byte[bufferSize], index, count));
        }

        [Theory,
            InlineData(0, 0, 0),
            InlineData(0, 0, -1),
            InlineData(1, 1, 0),
            InlineData(1, 1, -1)]
        public unsafe void Write_DoesNotThrowCountZeroOrLess(int bufferSize, int index, int count)
        {
            using MemoryStream memoryStream = new();
            using var stream = ComHelpers.GetComScope<IStream>(new Interop.Ole32.GPStream(memoryStream), out bool result);
            Assert.True(result);
            DataStreamFromComStream dataStream = new(stream, ownsHandle: false);
            dataStream.Write(new byte[bufferSize], index, count);
        }
    }
}
