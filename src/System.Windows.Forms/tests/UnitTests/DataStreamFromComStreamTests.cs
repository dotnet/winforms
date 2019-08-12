// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System.IO;
using Moq;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataStreamFromComStreamTests
    {
        [Theory,
            InlineData(0, 0, 1),
            InlineData(1, 1, 1)]
        public void Write_ThrowsInvalidCount(int bufferSize, int index, int count)
        {
            // The mock should never be called in outlier cases
            var comStreamMock = new Mock<Ole32.IStream>(MockBehavior.Strict);
            var dataStream = new DataStreamFromComStream(comStreamMock.Object);
            Assert.Throws<IOException>(() => dataStream.Write(new byte[bufferSize], index, count));
        }

        [Theory,
            InlineData(0, 0, 0),
            InlineData(0, 0, -1),
            InlineData(1, 1, 0),
            InlineData(1, 1, -1)]
        public void Write_DoesNotThrowCountZeroOrLess(int bufferSize, int index, int count)
        {
            // The mock should never be called in outlier cases
            var comStreamMock = new Mock<Ole32.IStream>(MockBehavior.Strict);
            var dataStream = new DataStreamFromComStream(comStreamMock.Object);
            dataStream.Write(new byte[bufferSize], index, count);
        }
    }
}
